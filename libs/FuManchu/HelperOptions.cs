// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System;
using System.Collections.Generic;
using System.Linq;

using FuManchu.Renderer;

/// <summary>
/// Represents options passed to Handlebars helpers.
/// </summary>
public class HelperOptions(
	RenderContext context,
	Arguments? arguments = null,
	Map? parameters = null)
{
	readonly RenderContext _context = context;
	readonly Arguments? _arguments = arguments;
	readonly Map? _parameters = parameters;

	/// <summary>
	/// Gets the set of input arguments.
	/// </summary>
	public object?[]? Arguments => _arguments;

	/// <summary>
	/// Gets the input argument. This property exists for API compatability with HandlebarsJS
	/// </summary>
	public dynamic? Data => Arguments?.FirstOrDefault();

	/// <summary>
	/// Gets the parameters collection. This property exists for API compatability with HandlebarsJS
	/// </summary>
	public Map? Hash => Parameters;

	/// <summary>
	/// Gets the render function. This property exists for API compatability with HandlebarsJS
	/// </summary>
	public Func<object, string>? Fn => Render;

	/// <summary>
	/// Gets the parameters collection.
	/// </summary>
	public Map? Parameters => _parameters;

	/// <summary>
	/// Gets or sets the render function.
	/// </summary>
	public Func<object, string>? Render { get; set; }

	/// <summary>
	/// Gets or sets the render context.
	/// </summary>
	public RenderContext RenderContext => _context;
}
