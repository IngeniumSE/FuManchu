// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tags;

using System;

using FuManchu.Parser.SyntaxTree;
using FuManchu.Renderer;

/// <summary>
/// Represents a tag descriptor providing metadata about known tags.
/// </summary>
public class TagDescriptor
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TagDescriptor"/> class.
	/// </summary>
	/// <param name="name">The name.</param>
	/// <param name="renderer">The renderer.</param>
	/// <param name="requiredArguments">The required number of arguments.</param>
	/// <param name="maxArguments">The maximum number of arguments.</param>
	/// <param name="allowMappedParamters">if set to <c>true</c> [allow mapped paramters].</param>
	/// <param name="hasChildContent">if set to <c>true</c> [has child content].</param>
	public TagDescriptor(
		string name,
		ISyntaxTreeNodeRenderer<Block> renderer,
		int requiredArguments,
		int? maxArguments = null,
		bool allowMappedParamters = false,
		bool hasChildContent = true)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("The tag name is expected");
		}

		if (renderer == null)
		{
			throw new ArgumentNullException("renderer");
		}

		if (requiredArguments < 0)
		{
			throw new ArgumentOutOfRangeException("requiredArguments");
		}

		if (maxArguments.HasValue && maxArguments.Value < 0)
		{
			throw new ArgumentOutOfRangeException("maxArguments");
		}

		Name = name;
		Renderer = renderer;
		RequiredArguments = requiredArguments;
		MaxArguments = maxArguments.GetValueOrDefault(int.MaxValue);
		AllowMappedParameters = allowMappedParamters;
		HasChildContent = hasChildContent;
	}

	/// <summary>
	/// Gets a value indicating whether to allow mapped parameters.
	/// </summary>
	public bool AllowMappedParameters { get; private set; }

	/// <summary>
	/// Gets a value indicating whether this instance has child content.
	/// </summary>
	public bool HasChildContent { get; private set; }

	/// <summary>
	/// Gets whether the tag is an implicit tag.
	/// </summary>
	public bool IsImplicit { get; internal set; }

	/// <summary>
	/// Gets the count of maximum arguments.
	/// </summary>
	public int MaxArguments { get; private set; }

	/// <summary>
	/// Gets the name of the tag.
	/// </summary>
	public string Name { get; private set; }

	/// <summary>
	/// Gets syntax tree renderer.
	/// </summary>
	public ISyntaxTreeNodeRenderer<Block> Renderer { get; private set; }

	/// <summary>
	/// Gets the count of required arguments.
	/// </summary>
	public int RequiredArguments { get; private set; }
}
