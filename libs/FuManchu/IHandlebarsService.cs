// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using FuManchu.Renderer;

/// <summary>
/// Defines the required contract for implementing a Handlebars service.
/// </summary>
public interface IHandlebarsService
{
	/// <summary>
	/// Gets the operators collection.
	/// </summary>
	OperatorCollection Operators { get; }

	/// <summary>
	/// Compiles a Handlebars template
	/// </summary>
	/// <param name="name">The name of the template.</param>
	/// <param name="template">The handlebars template.</param>
	/// <returns>The compiled template as an executable delegate.</returns>
	HandlebarTemplate Compile(string name, string template);

	/// <summary>
	/// Compiles and runs the given Handlebars template.
	/// </summary>
	/// <param name="name">The name of the template.</param>
	/// <param name="template">The handlebars template.</param>
	/// <param name="model">The model for the template.</param>
	/// <param name="unknownValueResolver">The resolver used to handle unknown values.</param>
	/// <returns>The template result.</returns>
	string CompileAndRun(string name, string template, object? model = null, UnknownValueResolver? unknownValueResolver = null);

	/// <summary>
	/// Determines if the service has a registered helper.
	/// </summary>
	/// <param name="name">The name of the helper.</param>
	/// <returns>True if the helper is registere, otherwise false.</returns>
	bool HasRegisteredHelper(string name);

	/// <summary>
	/// Registers a helper with the given name.
	/// </summary>
	/// <param name="name">The name of the helper.</param>
	/// <param name="helper">The helper delegate.</param>
	void RegisterHelper(string name, HandlebarHelper helper);

	/// <summary>
	/// Registers a partial template with the given name.
	/// </summary>
	/// <param name="name">The name of the partial template.</param>
	/// <param name="template">The partial template content.</param>
	void RegisterPartial(string name, string template);

	/// <summary>
	/// Runs a pre-compiled template.
	/// </summary>
	/// <param name="name">The name of the template.</param>
	/// <param name="model">The model for the template.</param>
	/// <param name="unknownValueResolver">The resolver used to handle unknown values.</param>
	/// <returns>The template result.</returns>
	string Run(string name, object? model = null, UnknownValueResolver? unknownValueResolver = null);

	/// <summary>
	/// Runs a pre-compiled partial template.
	/// </summary>
	/// <param name="name">The name of the partial template.</param>
	/// <param name="context">The render context.</param>
	/// <returns>The template result.</returns>
	string RunPartial(string name, RenderContext context);

	/// <summary>
	/// Runs a registered helper.
	/// </summary>
	/// <param name="name">The name of the helper.</param>
	/// <param name="options">The options.</param>
	string RunHelper(string name, HelperOptions options);
}
