// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using System;

using FuManchu.Parser.SyntaxTree;
using FuManchu.Text;

/// <summary>
/// Provides a base implementation of a parser.
/// </summary>
public abstract class ParserBase
{
	/// <summary>
	/// Gets or sets the context.
	/// </summary>
	public virtual ParserContext? Context { get; set; }

	/// <summary>
	/// Builds the span.
	/// </summary>
	/// <param name="span">The span.</param>
	/// <param name="start">The start.</param>
	/// <param name="content">The content.</param>
	public abstract void BuildSpan(SpanBuilder span, SourceLocation start, string content);

	/// <summary>
	/// Parses the block.
	/// </summary>
	public abstract void ParseBlock();

	/// <summary>
	/// Parses the document.
	/// </summary>
	public virtual void ParseDocument()
	{
		throw new NotSupportedException("This operation is not supported.");
	}
}
