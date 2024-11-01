// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Defines the required contract for implementing a parser visitor.
/// </summary>
public interface IParserVisitor
{
	/// <summary>
	/// Called when the visitor has finished walking the syntax tree.
	/// </summary>
	void OnComplete();

	/// <summary>
	/// Visits the block.
	/// </summary>
	/// <param name="block">The block.</param>
	void VisitBlock(Block block);

	/// <summary>
	/// Processed after an error has been encountered.
	/// </summary>
	/// <param name="error">The error.</param>
	void VisitError(Error error);

	/// <summary>
	/// Visits the span.
	/// </summary>
	/// <param name="span">The span.</param>
	void VisitSpan(Span span);
}
