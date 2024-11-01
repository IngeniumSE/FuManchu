// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Represents a visitor for walking a parsed syntax tree.
/// </summary>
public abstract class ParserVisitor : IParserVisitor
{
	/// <inheritdoc />
	public virtual void OnComplete() { }

	/// <inheritdoc />
	public virtual void VisitBlock(Block block)
	{
		VisitStartBlock(block);
		foreach (var node in block.Children)
		{
			node.Accept(this);
		}
		VisitEndBlock(block);
	}

	/// <summary>
	/// Processed after a all child nodes of a block are visited.
	/// </summary>
	/// <param name="block">The block.</param>
	public virtual void VisitEndBlock(Block block) { }

	/// <inheritdoc />
	public virtual void VisitError(Error error) { }

	/// <inheritdoc />
	public virtual void VisitSpan(Span span)
	{

	}

	/// <summary>
	/// Processed before any child nodes of a block are visited.
	/// </summary>
	/// <param name="block">The block.</param>
	public virtual void VisitStartBlock(Block block) { }
}
