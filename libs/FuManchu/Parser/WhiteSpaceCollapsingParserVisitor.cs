// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using System.Collections.Generic;
using System.Linq;

using FuManchu.Parser.SyntaxTree;
using FuManchu.Tokenizer;

/// <summary>
/// Collapses whitespace in a syntax tree.
/// </summary>
public class WhiteSpaceCollapsingParserVisitor : ParserVisitor
{
	/// <summary>
	/// Collapses the next whitespace instance.
	/// </summary>
	/// <param name="span">The tilde span.</param>
	private void CollapseNextWhiteSpace(Span span)
	{
		// Traverse to parent block. (TagElement | Expression)
		var element = span.Parent;
		Block? scope = null;
		List<SyntaxTreeNode> children;

		if (element is not null)
		{
			if (element.Type == BlockType.TagElement || element.Type == BlockType.Expression || element.Type == BlockType.Comment || element.Type == BlockType.Partial)
			{
				scope = element.Parent; // Up to parent block containing the Expression tag.
				if (scope is not null)
				{
					children = scope.Children.ToList();

					if (children[children.Count - 1].Equals(element))
					{
						CollapseNextWhiteSpace(scope.Parent, scope);
					}
					else
					{
						CollapseNextWhiteSpace(scope, element);
					}
				}
			}
		}
	}

	/// <summary>
	/// Collapses the next whitespace element from the given block, offset by the child element.
	/// </summary>
	/// <param name="block">The block.</param>
	/// <param name="element">The child element.</param>
	private void CollapseNextWhiteSpace(Block? block, SyntaxTreeNode element)
	{
		if (block == null)
		{
			return;
		}

		var children = block.Children.ToList();
		int index = children.IndexOf(element);
		if (index < (children.Count - 1))
		{
			var potential = children[index + 1] as Span;
			if (potential != null && potential.Kind == SpanKind.WhiteSpace)
			{
				potential.Collapsed = true;
			}
		}
	}

	/// <summary>
	/// Collapses the previous whitespace instance.
	/// </summary>
	/// <param name="span">The tilde span.</param>
	private void CollapsePreviousWhiteSpace(Span span)
	{
		// Traverse to parent block. (TagElement | Expression)
		var element = span.Parent;
		Block? scope = null;
		List<SyntaxTreeNode> children;

		if (element is not null)
		{
			if (element.Type == BlockType.TagElement || element.Type == BlockType.Expression || element.Type == BlockType.Comment || element.Type == BlockType.Partial)
			{
				scope = element.Parent; // Up to parent block containing the Expression tag.

				if (scope is not null)
				{
					children = scope.Children.ToList();

					if (children[0].Equals(element))
					{
						CollapsePreviousWhiteSpace(scope.Parent, scope);
					}
					else
					{
						CollapsePreviousWhiteSpace(scope, element);
					}
				}
			}
		}
	}

	/// <summary>
	/// Collapses the previous whitespace element from the given block, offset by the child element.
	/// </summary>
	/// <param name="block">The block.</param>
	/// <param name="element">The child element.</param>
	private void CollapsePreviousWhiteSpace(Block? block, SyntaxTreeNode element)
	{
		if (block == null)
		{
			return;
		}

		var children = block.Children.ToList();
		int index = children.IndexOf(element);
		if (index > 0)
		{
			var potential = children[index - 1] as Span;
			if (potential != null && potential.Kind == SpanKind.WhiteSpace)
			{
				potential.Collapsed = true;
			}
		}
	}

	/// <inheritdoc />
	public override void VisitSpan(Span span)
	{
		if (span.Kind == SpanKind.MetaCode)
		{
			var symbol = span.Symbols.FirstOrDefault() as HandlebarsSymbol;
			if (symbol != null && symbol.Type == HandlebarsSymbolType.Tilde)
			{
				VisitTilde(span);
			}
		}

		base.VisitSpan(span);
	}

	/// <summary>
	/// Visits a ~ character which allows collapsing of whitespace.
	/// </summary>
	/// <param name="span">The tilde span.</param>
	public void VisitTilde(Span span)
	{
		if (span.Previous is { Kind: SpanKind.MetaCode, Content: { Length: > 0 } } && span.Previous.Content.EndsWith("{{"))
		{
			CollapsePreviousWhiteSpace(span);
		}
		if (span.Next is { Kind: SpanKind.MetaCode, Content: { Length: > 0 } } && span.Next.Content.StartsWith("}}"))
		{
			CollapseNextWhiteSpace(span);
		}
	}
}
