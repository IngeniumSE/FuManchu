// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tokenizer;

using System;
using System.Collections.Generic;
using System.Linq;


using System.Text;

using FuManchu.Parser;
using FuManchu.Text;

/// <summary>
/// Provides a base implementation of a tokenizer.
/// </summary>
/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
/// <typeparam name="TSymbolType">The type of the symbol type.</typeparam>
public abstract class Tokenizer<TSymbol, TSymbolType> : StateMachine<TSymbol>, ITokenizer
	where TSymbol : SymbolBase<TSymbolType>
	where TSymbolType : struct
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Tokenizer{TSymbol, TSymbolType}"/> class.
	/// </summary>
	/// <param name="source">The source.</param>
	protected Tokenizer(ITextDocument source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}

		Source = new TextDocumentReader(source);
		Buffer = new StringBuilder();
		CurrentErrors = new List<Error>();

		StartSymbol();
	}

	/// <summary>
	/// Gets the buffer.
	/// </summary>
	protected StringBuilder Buffer { get; private set; }

	/// <summary>
	/// Gets the current character.
	/// </summary>
	protected char CurrentCharacter
	{
		get
		{
			int peek = Source.Peek();

			return peek == -1 ? '\0' : (char)peek;
		}
	}

	/// <summary>
	/// Gets the current errors.
	/// </summary>
	protected IList<Error> CurrentErrors { get; private set; }

	/// <summary>
	/// Gets the current location.
	/// </summary>
	protected SourceLocation CurrentLocation
	{
		get { return Source.Location; }
	}

	/// <summary>
	/// Gets the current start location.
	/// </summary>
	protected SourceLocation CurrentStart { get; private set; }

	/// <summary>
	/// Gets a value indicating whether we are at the end of the source stream.
	/// </summary>
	protected bool EndOfFile
	{
		get { return Source.Peek() == -1; }
	}

	/// <summary>
	/// Gets a value indicating whether we have content.
	/// </summary>
	protected bool HaveContent
	{
		get { return Buffer.Length > 0; }
	}

	/// <summary>
	/// Gets the source.
	/// </summary>
	public TextDocumentReader Source { get; private set; }

	/// <summary>
	/// Returns a predicate that determines if the given character is a character or whitespace (including new lines).
	/// </summary>
	/// <param name="character">The character.</param>
	/// <returns></returns>
	protected Func<char, bool> CharOrWhiteSpace(char character)
	{
		return c => c == character || ParserHelpers.IsWhiteSpace(character) || ParserHelpers.IsNewLine(character);
	}

	/// <summary>
	/// Creates the symbol.
	/// </summary>
	/// <param name="start">The start.</param>
	/// <param name="content">The content.</param>
	/// <param name="type">The type.</param>
	/// <param name="errors">The errors.</param>
	/// <returns>The symbol.</returns>
	public abstract TSymbol CreateSymbol(SourceLocation start, string content, TSymbolType type, IEnumerable<Error> errors);

	/// <summary>
	/// Ends the symbol.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>The symbol instance.</returns>
	protected TSymbol? EndSymbol(TSymbolType type)
	{
		return EndSymbol(CurrentStart, type);
	}

	/// <summary>
	/// Ends the symbol.
	/// </summary>
	/// <param name="start">The start.</param>
	/// <param name="type">The type.</param>
	/// <returns>The symbol instance.</returns>
	protected TSymbol? EndSymbol(SourceLocation start, TSymbolType type)
	{
		TSymbol? sym = null;
		if (HaveContent)
		{
			sym = CreateSymbol(start, Buffer.ToString(), type, CurrentErrors.ToArray());
		}
		StartSymbol();
		return sym;
	}

	/// <summary>
	/// Looks ahead in the source to determine if the given string is found.
	/// </summary>
	/// <param name="expected">The expected string.</param>
	/// <param name="takeIfMatch">if set to <c>true</c> we should read the matched string into the buffer.</param>
	/// <param name="caseSensitive">if set to <c>true</c> we should use a case-sensistive match.</param>
	/// <returns>If the expected string was matched, otherwise false.</returns>
	private bool Lookahead(string expected, bool takeIfMatch, bool caseSensitive)
	{
		Func<char, char> charFilter = c => c;
		if (!caseSensitive)
		{
			charFilter = Char.ToLowerInvariant;
		}

		if (expected.Length == 0 || charFilter(CurrentCharacter) != charFilter(expected[0]))
		{
			return false;
		}

		string? oldBuffer = null;
		if (takeIfMatch)
		{
			oldBuffer = Buffer.ToString();
		}

		using (var lookahead = Source.BeginLookahead())
		{
			for (int i = 0; i < expected.Length; i++)
			{
				if (charFilter(CurrentCharacter) != charFilter(expected[i]))
				{
					if (takeIfMatch)
					{
						Buffer.Clear();
						Buffer.Append(oldBuffer);
					}

					return false;
				}
				if (takeIfMatch)
				{
					TakeCurrent();
				}
				else
				{
					MoveNext();
				}
			}
			if (takeIfMatch)
			{
				lookahead.Accept();
			}
		}

		return true;
	}

	/// <summary>
	/// Moves the next character in the source.
	/// </summary>
	protected void MoveNext()
	{
		Source.Read();
	}

	/// <summary>
	/// Reads the next symbol
	/// </summary>
	/// <returns>The next symbol.</returns>
	public virtual TSymbol? NextSymbol()
	{
		StartSymbol();

		if (EndOfFile)
		{
			return null;
		}

		var sym = Turn();

		return sym;
	}

	/// <inheritdoc />
	ISymbol? ITokenizer.NextSymbol()
	{
		return (ISymbol?)NextSymbol();
	}

	/// <summary>
	/// Peeks the next character in the source.
	/// </summary>
	/// <returns></returns>
	protected char Peek()
	{
		using (var token = Source.BeginLookahead())
		{
			MoveNext();
			return CurrentCharacter;
		}
	}

	/// <summary>
	/// Resets this tokenizer.
	/// </summary>
	public void Reset()
	{
		CurrentState = StartState;
	}

	/// <summary>
	/// Resumes the symbol.
	/// </summary>
	/// <param name="previous">The previous.</param>
	/// <exception cref="System.InvalidOperationException">Cannot resume symbol unless it was the previous symbol.</exception>
	protected void ResumeSymbol(TSymbol previous)
	{
		if (previous.Start.Absolute + previous.Content.Length != CurrentStart.Absolute)
		{
			throw new InvalidOperationException("Cannot resume symbol unless it was the previous symbol.");
		}

		CurrentStart = previous.Start;
		string newContent = Buffer.ToString();

		Buffer.Clear();
		Buffer.Append(previous.Content);
		Buffer.Append(newContent);
	}

	/// <summary>
	/// Takes a single instance of the given symbol type.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>The symbol</returns>
	protected TSymbol? Single(TSymbolType type)
	{
		TakeCurrent();
		return EndSymbol(type);
	}

	/// <summary>
	/// Starts a symbol.
	/// </summary>
	protected void StartSymbol()
	{
		Buffer.Clear();
		CurrentStart = CurrentLocation;
		CurrentErrors.Clear();
	}

	/// <summary>
	/// Takes the given string if it is matched in the source stream.
	/// </summary>
	/// <param name="expected">The expected.</param>
	/// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
	/// <returns>True if the expected string was matched, otherwise false.</returns>
	protected bool TakeAll(string expected, bool caseSensitive)
	{
		return Lookahead(expected, takeIfMatch: true, caseSensitive: caseSensitive);
	}

	/// <summary>
	/// Takes the current character and appends it to the buffer.
	/// </summary>
	protected void TakeCurrent()
	{
		if (EndOfFile)
		{
			return;
		}

		Buffer.Append(CurrentCharacter);
		MoveNext();
	}

	/// <summary>
	/// Reads the given string from the source stream.
	/// </summary>
	/// <param name="input">The input.</param>
	/// <param name="caseSensitive">if set to <c>true</c> [case sensitive].</param>
	/// <returns>True if the entire string was read, otherwise false.</returns>
	protected bool TakeString(string input, bool caseSensitive)
	{
		int position = 0;
		Func<char, char> charFilter = c => c;
		if (caseSensitive)
		{
			charFilter = Char.ToLower;
		}
		while (!EndOfFile && position < input.Length && charFilter(CurrentCharacter) == charFilter(input[position++]))
		{
			TakeCurrent();
		}
		return position == input.Length;
	}

	/// <summary>
	/// Takes the input from the source stream until the predicate is matched.
	/// </summary>
	/// <param name="predicate">The predicate.</param>
	/// <returns>True if we are not at the end of the file, otherwise false.</returns>
	protected bool TakeUntil(Func<char, bool> predicate)
	{
		while (!EndOfFile && !predicate(CurrentCharacter))
		{
			TakeCurrent();
		}

		return !EndOfFile;
	}
}
