// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tokenizer;

using System.Collections.Generic;
using System.Linq;

using FuManchu.Text;

/// <summary>
/// Represents a Handlerbars symbol.
/// </summary>
public struct HandlebarsSymbol : ISymbol<HandlebarsSymbolType>
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
		: this(start, content, type, Enumerable.Empty<Error>())
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
		: this(new SourceLocation(offset, line, column), content, type, errors ?? Enumerable.Empty<Error>())
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
	{
		if (content == null)
		{
			throw new ArgumentNullException("content");
		}

		Start = start;
		Content = content;
		Type = type;
		Errors = errors;
		HasValue = true;
	}

	/// <inheritdoc />
	public bool HasValue { get; }

	/// <summary>
	/// Gets or sets the keyword.
	/// </summary>
	public HandlebarsKeyword? Keyword { get; set; }

	/// <inheritdoc />
	public SourceLocation Start { get; private set; }

	/// <inheritdoc />
	public string Content { get; private set; }

	/// <summary>
	/// Gets the errors generated because of this symbol.
	/// </summary>
	public IEnumerable<Error> Errors { get; private set; }

	/// <summary>
	/// Gets the symbol type.
	/// </summary>
	public HandlebarsSymbolType Type { get; private set; }

	/// <summary>
	/// Changes the start of the symbol.
	/// </summary>
	/// <param name="newStart">The new start.</param>
	public void ChangeStart(SourceLocation newStart)
	{
		Start = newStart;
	}

	/// <inheritdoc />
	public override bool Equals(object obj)
	{
		if (obj is ISymbol<HandlebarsSymbolType> value)
		{
			return Start.Equals(value.Start) && string.Equals(Content, value.Content, StringComparison.Ordinal) && Type.Equals(value.Type);
		}

		return false;
	}

	/// <inheritdoc />
	public override int GetHashCode() => HashCode.Combine(Start, Content, Type);

	/// <summary>
	/// Offsets the start of the symbol based on the document start.
	/// </summary>
	/// <param name="documentStart">The document start.</param>
	public void OffsetStart(SourceLocation documentStart)
	{
		Start = documentStart + Start;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return string.Format("{0} {1} - {2}", Start, Type, Content);
	}
}
