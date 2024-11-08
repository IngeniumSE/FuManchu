// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;

using FuManchu.Binding;
using FuManchu.Parser;
using FuManchu.Parser.SyntaxTree;
using FuManchu.Renderer;
using FuManchu.Tags;
using FuManchu.Text;

/// <summary>
/// Provides services for running Handlebars templates.
/// </summary>
public class HandlebarsService : IHandlebarsService
{
	private readonly ConcurrentDictionary<string, HandlebarHelper> _helpers = new ConcurrentDictionary<string, HandlebarHelper>();
	private readonly ConcurrentDictionary<string, HandlebarPartialTemplate> _partials = new ConcurrentDictionary<string, HandlebarPartialTemplate>();
	private readonly ConcurrentDictionary<string, HandlebarTemplate> _templates = new ConcurrentDictionary<string, HandlebarTemplate>();

	/// <summary>
	/// Initialises a new instance of <see cref="HandlebarsService"/>
	/// </summary>
	public HandlebarsService()
	{
		Operators = new OperatorCollection(OperatorCollection.Default);
		TagProviders = new TagProvidersCollection(TagProvidersCollection.Default);
		ModelMetadataProvider = new DefaultModelMetadataProvider();
	}

	/// <summary>
	/// The collection of operators.
	/// </summary>
	public OperatorCollection Operators { get; private set; }

	/// <summary>
	/// The collection of tag providers.
	/// </summary>
	public TagProvidersCollection TagProviders { get; private set; }

	/// <summary>
	/// The model metadata provider.
	/// </summary>
	public IModelMetadataProvider ModelMetadataProvider { get; set; }

	/// <inheritdoc />
	public HandlebarTemplate Compile(string template)
	{
		var document = CreateDocument(template);

		// Collapse any whitespace.
		var whitespace = new WhiteSpaceCollapsingParserVisitor();
		document.Accept(whitespace);

		return (model, resolver) =>
		{
			using (var writer = new StringWriter())
			{
				var render = new RenderingParserVisitor(writer, model, ModelMetadataProvider ?? new DefaultModelMetadataProvider(), resolver)
				{
					Service = this
				};

				// Render the document.
				document.Accept(render);

				return writer.GetStringBuilder().ToString();
			}
		};
	}

	/// <inheritdoc />
	public HandlebarTemplate Compile(string name, string template)
	{
		HandlebarTemplate func;
		if (_templates.TryGetValue(name, out func))
		{
			return func;
		}

		func = Compile(template);
		_templates.TryAdd(name, func);

		return func;
	}

	/// <inheritdoc />
	public string CompileAndRun(
		string name,
		string template,
		object? model = null,
		UnknownValueResolver? unknownValueResolver = null)
	{
		HandlebarTemplate func = (string.IsNullOrEmpty(name)) ? Compile(template) : Compile(name, template);

		return func(model, unknownValueResolver);
	}

	/// <inheritdoc />
	public HandlebarPartialTemplate CompilePartial(string template)
	{
		var document = CreateDocument(template);

		// Collapse any whitespace.
		var whitespace = new WhiteSpaceCollapsingParserVisitor();
		document.Accept(whitespace);

		return (context, writer) =>
		{
			var render = new RenderingParserVisitor(writer, context, ModelMetadataProvider ?? new DefaultModelMetadataProvider())
			{
				Service = this
			};

			// Render the document.
			document.Accept(render);
		};
	}

	/// <inheritdoc />
	public bool HasRegisteredHelper(string name)
	{
		return _helpers.ContainsKey(name);
	}

	/// <inheritdoc />
	public void RegisterHelper(string name, HandlebarHelper helper)
	{
		HandlebarHelper temp;
		if (!_helpers.TryGetValue(name, out temp))
		{
			_helpers.TryAdd(name, helper);
		}
	}

	/// <summary>
	/// Registers a partial template with the given name.
	/// </summary>
	/// <param name="name">The name of the partial template.</param>
	/// <param name="partial">The partial delegate.</param>
	public void RegisterPartial(string name, HandlebarPartialTemplate partial)
	{
		HandlebarPartialTemplate temp;
		if (!_partials.TryGetValue(name, out temp))
		{
			_partials.TryAdd(name, partial);
		}
	}

	/// <inheritdoc />
	public void RegisterPartial(string name, string template)
	{
		HandlebarPartialTemplate func;
		if (!_partials.TryGetValue(name, out func))
		{
			func = CompilePartial(template);
			_partials.TryAdd(name, func);
		}
	}

	/// <summary>
	/// Removes a compiled template.
	/// </summary>
	/// <param name="name">The name of the compiled template.</param>
	public void RemoveCompiledTemplate(string name)
	{
		HandlebarTemplate func;
		_templates.TryRemove(name, out func);
	}

	/// <summary>
	/// Removes a compiled partial template.
	/// </summary>
	/// <param name="name">The name of the compiled partial template.</param>
	public void RemoveCompiledPartial(string name)
	{
		HandlebarPartialTemplate func;
		_partials.TryRemove(name, out func);
	}

	/// <inheritdoc />
	public string Run(
		string name,
		object? model = null,
		UnknownValueResolver? unknownValueResolver = null)
	{
		HandlebarTemplate func;
		if (_templates.TryGetValue(name, out func))
		{
			return func(model, unknownValueResolver);
		}

		throw new ArgumentException("No template called '" + name + "' has been compiled.");
	}

	/// <inheritdoc />
	public string RunPartial(string name, RenderContext context)
	{
		using (var writer = new StringWriter())
		{
			RunPartial(name, context, writer);

			return writer.GetStringBuilder().ToString();
		}
	}

	/// <inheritdoc />
	public void RunPartial(string name, RenderContext context, TextWriter writer)
	{
		HandlebarPartialTemplate func;
		if (_partials.TryGetValue(name, out func))
		{
			func(context, writer);

			return;
		}

		throw new ArgumentException("No partial template called '" + name + "' has been compiled.");
	}

	/// <inheritdoc />
	public string RunHelper(string name, HelperOptions options)
	{
		HandlebarHelper func;
		if (_helpers.TryGetValue(name, out func))
		{
			return func(options);
		}

		throw new ArgumentException("No helper called '" + name + "' has been registered.");
	}

	/// <summary>
	/// Creates a document <see cref="Block"/> from the given template.
	/// </summary>
	/// <param name="template">The template.</param>
	/// <returns>The document <see cref="Block"/></returns>
	private Block CreateDocument(string template)
	{
		using (var reader = new StringReader(template))
		{
			using (var source = new SeekableTextReader(reader))
			{
				var errors = new ParserErrorSink();
				var parser = new HandlebarsParser();

				var context = new ParserContext(source, parser, errors, TagProviders);
				parser.Context = context;

				parser.ParseDocument();
				var results = context.CompleteParse();

				if (results.Success)
				{
					return results.Document;
				}

				throw new InvalidOperationException(
					string.Join("\n", results.Errors.Select(e => string.Format("{0}: {1}", e.Location, e.Message))));
			}
		}
	}
}
