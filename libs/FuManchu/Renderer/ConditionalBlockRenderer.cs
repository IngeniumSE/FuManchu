// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Provides rendering of {{#if ...}} {{/if}} blocks, with support for {{#elseif}} and {{else}}
/// </summary>
public class ConditionalBlockRenderer : BlockRenderer
{
	/// <inheritdoc />
	public override void Render(Block target, RenderContext context, TextWriter writer)
	{
		// 1. Figure out branching blocks.
		// 2. For each branching block (first to last), resolve conditional argument.
		var blocks = ParseConditionalBlocks(target);

		foreach (var block in blocks)
		{
			var arguments = GetArgumentsAndMappedParameters(block.Item1, context);
			if (block.Item1.Name == "if" || block.Item1.Name == "elseif")
			{
				if (IsTruthy(arguments.Item1[0]))
				{
					RenderChildren(block.Item2, context);
					break;
				}
			}
			else
			{
				RenderChildren(block.Item2, context);
				break;
			}
		}
	}

	/// <inheritdoc />
	protected override void Render(Block block, Arguments? arguments, Map? maps, RenderContext context, TextWriter writer) { }

	/// <summary>
	/// Parses the conditional blocks.
	/// </summary>
	/// <param name="target">The target.</param>
	/// <returns>The set of parsed conditional blocks.</returns>
	private IEnumerable<Tuple<Block, List<SyntaxTreeNode>>> ParseConditionalBlocks(Block target)
	{
		var result = new List<Tuple<Block, List<SyntaxTreeNode>>>();

		Block? current = null;
		var nodes = new List<SyntaxTreeNode>();

		var children = target.Children.ToList();
		children.RemoveAt(children.Count - 1);

		foreach (var node in children)
		{
			bool isNewElement = false;
			if (node.IsBlock)
			{
				var block = (Block)node;
				if (block.Type == BlockType.TagElement && (block.Name == "if" || block.Name == "elseif" || block.Name == "else" || block.Name == "^"))
				{
					if (current != null)
					{
						result.Add(Tuple.Create(current, nodes));
						nodes = new List<SyntaxTreeNode>();
					}
					current = block;
					isNewElement = true;
				}
			}

			if (!isNewElement)
			{
				nodes.Add(node);
			}
		}

		if (current != null)
		{
			result.Add(Tuple.Create(current, nodes));
		}

		return result;
	}
}
