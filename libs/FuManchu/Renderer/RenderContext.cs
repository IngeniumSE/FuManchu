// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System.Collections.Generic;
using System.Linq;

using FuManchu.Binding;
using FuManchu.Parser;
using FuManchu.Parser.SyntaxTree;
using FuManchu.Tokenizer;

/// <summary>
/// Represents a context for rendering a syntax tree node.
/// </summary>
public class RenderContext
{
	private readonly Map _variables = new Map();

	/// <summary>
	/// Initializes a new instance of the <see cref="RenderContext"/> class.
	/// </summary>
	/// <param name="visitor">The visitor.</param>
	/// <param name="parentRenderContext">The parent render context.</param>
	public RenderContext(ParserVisitor<RenderContext> visitor, RenderContext? parentRenderContext = null)
	{
		ParentRenderContext = parentRenderContext;
		Visitor = visitor;
	}

	/// <summary>
	/// Gets or sets whether we are escaping text encoding.
	/// </summary>
	public bool EscapeEncoding { get; internal set; }

	/// <summary>
	/// Gets or sets the model metadata provider.
	/// </summary>
	public required IModelMetadataProvider ModelMetadataProvider { get; set; }

	/// <summary>
	/// Gets or sets the Handlebars service.
	/// </summary>
	public required IHandlebarsService? Service { get; set; }

	/// <summary>
	/// Gets or sets the template data.
	/// </summary>
	public required TemplateData TemplateData { get; set; }

	/// <summary>
	/// Gets the parent render context.
	/// </summary>
	public RenderContext? ParentRenderContext { get; private set; }

	/// <summary>
	/// Gets the root render context.
	/// </summary>
	public RenderContext? RootRenderContext { get; internal set; }

	/// <summary>
	/// Gets the unknown value resolver.
	/// </summary>
	public UnknownValueResolver? UnknownValueResolver { get; set; }

	/// <summary>
	/// Gets the parser visitor.
	/// </summary>
	public ParserVisitor<RenderContext> Visitor { get; private set; }

	/// <summary>
	/// Gets the partial block content for this context.
	/// </summary>
	public IDictionary<string, Block>? PartialBlockContent { get; set; }

	/// <summary>
	/// Begins a child scope given the specified model.
	/// </summary>
	/// <returns>The disposable used to revert the scope.</returns>
	public RenderContextScope BeginScope(object? model)
	{
		var context = RenderContextFactory.CreateRenderContext(this, model);
		Visitor.SetScope(context);

		return new RenderContextScope(context, () => Visitor.RevertScope());
	}

	/// <summary>
	/// Gets the variable with the given name.
	/// </summary>
	/// <param name="name">The variable name.</param>
	/// <param name="default">[Optional] The default value for the variable.</param>
	/// <returns>The variable value.</returns>
	public object? GetVariable(string name, object? @default = null)
	{
		object? value;
		if (_variables.TryGetValue(name, out value))
		{
			return value;
		}

		return @default;
	}

	/// <summary>
	/// Resolves the value represented by the given span.
	/// </summary>
	/// <param name="span">The span representing the expression.</param>
	/// <returns>The resolved value.</returns>
	public object? ResolveValue(Span span)
	{
		IEnumerable<HandlebarsSymbol>? symbols;

		switch (span.Kind)
		{
			case SpanKind.Expression:
			case SpanKind.Parameter:
				{
					symbols = span.Symbols;
					break;
				}
			case SpanKind.Map:
				{
					symbols = span.Symbols.Skip(2);
					break;
				}
			default:
				{
					return TemplateData.Model;
				}
		}

		return ResolveValueFromSymbols(symbols.ToArray());
	}

	/// <summary>
	/// Resolves the value represented by the given set of symbols.
	/// </summary>
	/// <param name="symbols">The set of symbols.</param>
	/// <returns>The resolved value.</returns>
	private object? ResolveValueFromSymbols(HandlebarsSymbol[] symbols)
	{
		if (symbols.Length == 0)
		{
			return TemplateData.Model;
		}

		var context = this;
		var templateData = TemplateData;

		string expression = string.Empty;
		bool isVariableLookup = false;

		foreach (var symbol in symbols)
		{
			switch (symbol.Type)
			{
				case HandlebarsSymbolType.At:
					{
						isVariableLookup = true;

						break;
					}

				case HandlebarsSymbolType.CurrentContext:
					{
						// Ensure the model property is set.
						templateData = TemplateData;

						if (templateData == null)
						{
							return null;
						}

						break;
					}

				case HandlebarsSymbolType.ParentContext:
					{
						context = context.ParentRenderContext;
						if (context == null)
						{
							// We couldn't reach the parent context.
							return null;
						}

						// Set the model to be that of the parent context.
						templateData = context.TemplateData;

						break;
					}

				case HandlebarsSymbolType.IntegerLiteral:
				case HandlebarsSymbolType.RealLiteral:
					{
						return symbol.Content;
					}

				case HandlebarsSymbolType.StringLiteral:
					{
						return symbol.Content.Substring(1, symbol.Content.Length - 2);
					}

				case HandlebarsSymbolType.Identifier:
				case HandlebarsSymbolType.Keyword:
				case HandlebarsSymbolType.Dot:
				case HandlebarsSymbolType.LeftBracket:
				case HandlebarsSymbolType.RightBracket:
					{
						if (symbol.Content == "this" && expression.Length == 0)
						{
							// Ensure the model property is set.
							templateData = TemplateData;

							if (templateData == null)
							{
								return null;
							}

							break;
						}

						expression += symbol.Content;
						break;
					}
				case HandlebarsSymbolType.Slash:
					{
						expression += ".";
						break;
					}
			}
		}

		if (expression.StartsWith("."))
		{
			// Skip the leading '.'
			expression = expression.Substring(1);
		}

		return ResolveValue(context, templateData, expression, isVariableLookup);
	}

	/// <summary>
	/// Resolves the value for the given expression.
	/// </summary>
	/// <param name="expression">The expression.</param>
	/// <param name="isVariableLookup">True if this is a variable lookup, otherwise false.</param>
	/// <returns>The resolved value.</returns>
	public object? ResolveValue(string expression, bool isVariableLookup)
	{
		return ResolveValue(this, TemplateData, expression, isVariableLookup);
	}

	/// <summary>
	/// Resolves the value for the given expression.
	/// </summary>
	/// <param name="context">The render context.</param>
	/// <param name="templateData">The template data.</param>
	/// <param name="expression">The expression.</param>
	/// <param name="isVariableLookup">True if this is a variable lookup, otherwise false.</param>
	/// <returns>The resolved value.</returns>
	public object? ResolveValue(RenderContext context, TemplateData templateData, string expression, bool isVariableLookup)
	{
		if (isVariableLookup && !string.IsNullOrEmpty(expression) && !expression.StartsWith("root."))
		{
			return context.GetVariable(expression);
		}

		if (isVariableLookup && !string.IsNullOrEmpty(expression) && expression.StartsWith("root."))
		{
			context = context.RootRenderContext ?? context;
			templateData = context.TemplateData;
			expression = expression.Substring(5);
		}

		var modelMetadata = ExpressionMetadataProvider.FromStringExpression(expression, templateData, context.ModelMetadataProvider);
		if (modelMetadata == null || !modelMetadata.Valid)
		{
			if (context.UnknownValueResolver != null)
			{
				return context.UnknownValueResolver(expression);
			}
			return null;
		}
		return modelMetadata.Model;
	}

	/// <summary>
	/// Sets the variable with the given name.
	/// </summary>
	/// <param name="name">The name of the variable.</param>
	/// <param name="value">The variable value.</param>
	public void SetVariable(string name, object value)
	{
		_variables[name] = value;
	}
}
