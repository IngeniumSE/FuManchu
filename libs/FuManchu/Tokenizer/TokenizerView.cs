// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tokenizer;

using System;

using FuManchu.Text;

/// <summary>
/// Provides a simplified abstraction of a tokenizer.
/// </summary>
public class TokenizerView
{
	/// <summary>
	/// Initialises a new instance of <see cref="TokenizerView"/>
	/// </summary>
	/// <param name="tokenizer">The tokenizer.</param>
	public TokenizerView(HandlebarsTokenizer tokenizer) => Tokenizer = tokenizer;

	/// <summary>
	/// Gets the current symbol.
	/// </summary>
	public HandlebarsSymbol? Current { get; private set; }

	/// <summary>
	/// Gets whether we are at the end of the source.
	/// </summary>
	public bool EndOfFile { get; private set; }

	/// <summary>
	/// Gets the source document.
	/// </summary>
	public ITextDocument Source { get { return Tokenizer.Source; } }

	/// <summary>
	/// Gets the tokenizer.
	/// </summary>
	public HandlebarsTokenizer Tokenizer { get; private set; }

	/// <summary>
	/// Reads the next symbol from the document
	/// </summary>
	/// <returns>True if the symbol was read, otherwise false (end of file).</returns>
	public bool Next()
	{
		Current = Tokenizer.NextSymbol();
		EndOfFile = (Current is null);
		return !EndOfFile;
	}

	/// <summary>
	/// Resets the source back to the beginning of the symbol.
	/// </summary>
	/// <param name="symbol">The symbol.</param>
	public void PutBack(HandlebarsSymbol symbol)
	{
		if (Source.Position != symbol.Start.Absolute + symbol.Content.Length)
		{
			throw new InvalidOperationException("Cannot put symbol back.");
		}

		Source.Position -= symbol.Content.Length;
		Current = null;
		EndOfFile = Source.Position >= Source.Length;
		Tokenizer.Reset();
	}
}
