// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser.SyntaxTree;

using System.Diagnostics;
using System.Text;

using FuManchu.Text;

/// <summary>
/// Represents a node in a syntax tree.
/// </summary>
#if DEBUG
[DebuggerDisplay("{DebugToString()}")]
#endif
public abstract class SyntaxTreeNode
{
	/// <summary>
	/// Gets whether this node represents a block.
	/// </summary>
	public abstract bool IsBlock { get; }

	/// <summary>
	/// Gets the length of the node.
	/// </summary>
	public abstract int Length { get; }

	/// <summary>
	/// Gets the parent block.
	/// </summary>
	public Block? Parent { get; internal set; }

	/// <summary>
	/// Gets the stat location of the node.
	/// </summary>
	public abstract SourceLocation Start { get; }

	/// <summary>
	/// Accepts a parser visitor to walk the syntax tree.
	/// </summary>
	/// <param name="visitor">The parser visitor</param>
	public abstract void Accept(IParserVisitor visitor);

	/// <summary>
	/// Determines if the given node is equivalent to the current node.
	/// </summary>
	/// <param name="node">The other node.</param>
	/// <returns>True if the nodes are equivalent otherwise false.</returns>
	public abstract bool EquivalentTo(SyntaxTreeNode node);

	/// <summary>
	/// Determines if the given node is equivalent to the current node.
	/// </summary>
	/// <param name="node">The other node.</param>
	/// <param name="builder">The string builder.</param>
	/// <param name="level">The current node level.</param>
	/// <returns>True if the nodes are equivalent otherwise false.</returns>
	public abstract bool EquivalentTo(SyntaxTreeNode node, StringBuilder builder, int level);

#if DEBUG
	public virtual string DebugToString()
	{
		return "";
	}
#endif
}
