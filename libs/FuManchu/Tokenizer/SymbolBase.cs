// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tokenizer;

using System;
using System.Collections.Generic;

using FuManchu.Text;

/// <summary>
/// Represents a base implementation of a symbol.
/// </summary>
/// <typeparam name="T">The symbol type.</typeparam>
public class SymbolBase<T> : ISymbol where T : struct
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SymbolBase{T}"/> class.
	/// </summary>
	/// <param name="start">The start.</param>
	/// <param name="content">The content.</param>
	/// <param name="type">The type.</param>
	/// <param name="errors">The errors.</param>
	protected SymbolBase(SourceLocation start, string content, T type, IEnumerable<Error> errors)
	{
		if (content == null)
		{
			throw new ArgumentNullException("content");
		}

		Start = start;
		Content = content;
		Type = type;
		Errors = errors;
	}

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
	public T Type { get; private set; }

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
		var other = obj as SymbolBase<T>;
		return other != null && Start.Equals(other.Start) && string.Equals(Content, other.Content, StringComparison.Ordinal) && Type.Equals(other.Type);
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
