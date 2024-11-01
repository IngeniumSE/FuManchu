// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Handles rendering {{#is s...}} blocks.
/// </summary>
public class IsBlockRenderer : BlockRenderer
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
			if (block.Item1.Name == "is" || block.Item1.Name == "elseis")
			{
				if (IsTrue(arguments.Item1, context))
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
				if (block.Type == BlockType.TagElement && (block.Name == "is" || block.Name == "elseis" || block.Name == "else" || block.Name == "^"))
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

	/// <summary>
	/// Determines if the expression represented by the given arguments is true.
	/// </summary>
	/// <param name="parameters">The set of parameters.</param>
	/// <param name="context">The render context.</param>
	/// <returns>True if the expression is true, otherwise false.</returns>
	private bool IsTrue(Arguments? parameters, RenderContext context)
	{
		if (parameters is not { Length: > 0 })
		{
			throw new ArgumentException("The 'is' block requires at least one argument.");
		}

		if (parameters is { Length: 1 })
		{
			// Act like an {{#if ...}} block, and just determine truthyness of the given value.
			return IsTruthy(parameters[0]);
		}

		string op = "";
		object? x = parameters[0];
		object? y = null;

		if (parameters.Length == 2)
		{
			y = parameters[1];
			op = "==";
		}
		else
		{
			y = parameters[2];
			op = parameters[1]!.ToString();
		}

		// Get the operator.
		var @operator = context.Service!.Operators.GetOperator(op);
		if (@operator == null)
		{
			throw new ArgumentException("The operator '" + op + "' is not supported.");
		}

		return @operator.Result(x, y);
	}
}
