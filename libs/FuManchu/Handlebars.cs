// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System;

using FuManchu.Renderer;

/// <summary>
/// Provides rendering of Handlebars templates.
/// </summary>
public static class Handlebars
{
	private static Lazy<IHandlebarsService> _handlebarsService;

	/// <summary>
	/// Initialises the <see cref="Handlebars"/> type.
	/// </summary>
	static Handlebars()
	{
		_handlebarsService = new Lazy<IHandlebarsService>(() => new HandlebarsService());
	}

	/// <summary>
	/// Gets or sets the global handlebars service.
	/// </summary>
	public static IHandlebarsService Service
	{
		get { return _handlebarsService.Value; }
		set { _handlebarsService = new Lazy<IHandlebarsService>(() => value); }
	}

	/// <summary>
	/// Compiles a Handlebars template
	/// </summary>
	/// <param name="name">The name of the template.</param>
	/// <param name="template">The handlebars template.</param>
	/// <returns>The compiled template as an executable delegate.</returns>
	public static HandlebarTemplate Compile(string name, string template)
	{
		return _handlebarsService.Value.Compile(name, template);
	}

	/// <summary>
	/// Compiles and runs the given Handlebars template.
	/// </summary>
	/// <param name="name">The name of the template.</param>
	/// <param name="template">The handlebars template.</param>
	/// <param name="model">The model for the template.</param>
	/// <param name="unknownValueResolver">The resolver used to handle unknown keys.</param>
	/// <returns>The template result.</returns>
	public static string CompileAndRun(
		string name,
		string template,
		object? model = null,
		UnknownValueResolver? unknownValueResolver = null)
	{
		return _handlebarsService.Value.CompileAndRun(name, template, model, unknownValueResolver);
	}

	/// <summary>
	/// Registers a helper function.
	/// </summary>
	/// <param name="name">The name of the helper.</param>
	/// <param name="helper">The helper delegate.</param>
	public static void RegisterHelper(string name, HandlebarHelper helper)
	{
		_handlebarsService.Value.RegisterHelper(name, helper);
	}

	/// <summary>
	/// Registers a partial template with the given name.
	/// </summary>
	/// <param name="name">The name of the partial template.</param>
	/// <param name="template">The partial template content.</param>
	public static void RegisterPartial(string name, string template)
	{
		_handlebarsService.Value.RegisterPartial(name, template);
	}

	/// <summary>
	/// Runs a pre-compiled template.
	/// </summary>
	/// <param name="name">The name of the template.</param>
	/// <param name="model">The model for the template.</param>
	/// <param name="unknownValueResolver">The resolver to use to handle unknown keys.</param>
	/// <returns>The template result.</returns>
	public static string Run(
		string name,
		object? model = null,
		UnknownValueResolver? unknownValueResolver = null)
	{
		return _handlebarsService.Value.Run(name, model, unknownValueResolver);
	}

	/// <summary>
	/// Runs a pre-compiled partial template.
	/// </summary>
	/// <param name="name">The name of the partial template.</param>
	/// <param name="context">The render context.</param>
	/// <returns>The template result.</returns>
	public static string RunPartial(string name, RenderContext context)
	{
		return _handlebarsService.Value.RunPartial(name, context);
	}
}
