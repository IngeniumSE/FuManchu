// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using FuManchu.Parser.SyntaxTree;
using FuManchu.Tags;

/// <summary>
/// Provides a base implementation of a block renderer.
/// </summary>
public abstract class BlockRenderer : SyntaxTreeNodeRenderer<Block>, IBlockRenderer
{
	/// <inheritdoc />
	public override void Render(Block target, RenderContext context, TextWriter writer)
	{
		Block parametersBlock = target;

		if (target.Type == BlockType.Tag || target.Type == BlockType.PartialBlock || target.Type == BlockType.PartialBlockContent)
		{
			// The arguments will be provided by the TagElement instance.
			parametersBlock = (Block)target.Children.First();
		}

		var parameters = GetArgumentsAndMappedParameters(parametersBlock, context);

		// Validate the tag.
		ValidateTag(target.Name, target.Descriptor, parameters.Item1, parameters.Item2);

		Render(target, parameters.Item1, parameters.Item2, context, writer);
	}

	/// <summary>
	/// Renders the specified block.
	/// </summary>
	/// <param name="block">The block.</param>
	/// <param name="arguments">The arguments.</param>
	/// <param name="maps">The maps.</param>
	/// <param name="context">The context.</param>
	/// <param name="writer">The writer.</param>
	protected abstract void Render(Block block, Arguments? arguments, Map? maps, RenderContext context, TextWriter writer);

	/// <inheritdoc />
	public void RenderChild(SyntaxTreeNode node, RenderContext context)
	{
		if (node == null)
		{
			throw new ArgumentNullException("node");
		}

		if (context == null)
		{
			throw new ArgumentNullException("context");
		}

		node.Accept(context.Visitor);
	}

	/// <summary>
	/// Renders the set of child nodes.
	/// </summary>
	/// <param name="children">The set of children.</param>
	/// <param name="context">Te render context.</param>
	public void RenderChildren(IEnumerable<SyntaxTreeNode> children, RenderContext context)
	{
		foreach (var child in children)
		{
			RenderChild(child, context);
		}
	}

	/// <summary>
	/// Validates a tag based on the descriptor.
	/// </summary>
	/// <param name="name">The tag name.</param>
	/// <param name="descriptor">The tag descriptor.</param>
	/// <param name="arguments">The set of arguments.</param>
	/// <param name="parameters">The set of parameters.</param>
	public void ValidateTag(string? name, TagDescriptor? descriptor, Arguments? arguments, Map? parameters)
	{
		if (name is null || descriptor is null || descriptor.IsImplicit)
		{
			// Can't validate the tag.
			return;
		}

		if (descriptor.RequiredArguments > 0 && (arguments is not null && arguments.Length < descriptor.RequiredArguments))
		{
			throw new InvalidOperationException(string.Format("The tag {0} requires at least {1} argument(s)", name, descriptor.RequiredArguments));
		}

		if (arguments is not null && arguments.Length > descriptor.MaxArguments)
		{
			throw new InvalidOperationException(string.Format("The tag {0} requires at most {1} argument(s)", name, descriptor.MaxArguments));
		}

		bool hasParameters = parameters is { Count: > 0 };
		if (descriptor.AllowMappedParameters != hasParameters)
		{
			throw new InvalidOperationException(string.Format("{0} parameters to tag {1}", hasParameters ? "Unexpected" : "Expected", name));
		}
	}
}
