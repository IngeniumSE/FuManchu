// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tokenizer;

using System.Collections.Generic;
using System.Linq;

using FuManchu.Text;

/// <summary>
/// Represents a Handlerbars symbol.
/// </summary>
public class HandlebarsSymbol : SymbolBase<HandlebarsSymbolType>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="HandlebarsSymbol"/> class.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <param name="line">The line.</param>
	/// <param name="column">The column.</param>
	/// <param name="content">The content.</param>
	/// <param name="type">The type.</param>
	public HandlebarsSymbol(int offset, int line, int column, string content, HandlebarsSymbolType type)
		: this(new SourceLocation(offset, line, column), content, type, Enumerable.Empty<Error>())
	{

	}

	/// <summary>
	/// Initializes a new instance of the <see cref="HandlebarsSymbol"/> class.
	/// </summary>
	/// <param name="start">The start.</param>
	/// <param name="content">The content.</param>
	/// <param name="type">The type.</param>
	public HandlebarsSymbol(SourceLocation start, string content, HandlebarsSymbolType type)
		: base(start, content, type, Enumerable.Empty<Error>())
	{

	}

	/// <summary>
	/// Initializes a new instance of the <see cref="HandlebarsSymbol"/> class.
	/// </summary>
	/// <param name="offset">The offset.</param>
	/// <param name="line">The line.</param>
	/// <param name="column">The column.</param>
	/// <param name="content">The content.</param>
	/// <param name="type">The type.</param>
	/// <param name="errors">The errors.</param>
	public HandlebarsSymbol(int offset, int line, int column, string content, HandlebarsSymbolType type, IEnumerable<Error> errors)
		: base(new SourceLocation(offset, line, column), content, type, errors ?? Enumerable.Empty<Error>())
	{

	}

	/// <summary>
	/// Initializes a new instance of the <see cref="HandlebarsSymbol"/> class.
	/// </summary>
	/// <param name="start">The start.</param>
	/// <param name="content">The content.</param>
	/// <param name="type">The type.</param>
	/// <param name="errors">The errors.</param>
	public HandlebarsSymbol(SourceLocation start, string content, HandlebarsSymbolType type, IEnumerable<Error> errors)
		: base(start, content, type, errors ?? Enumerable.Empty<Error>())
	{

	}

	/// <summary>
	/// Gets or sets the keyword.
	/// </summary>
	public HandlebarsKeyword? Keyword { get; set; }
}
