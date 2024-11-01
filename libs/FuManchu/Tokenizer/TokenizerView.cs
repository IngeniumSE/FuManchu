// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tokenizer;

using System;

using FuManchu.Text;

/// <summary>
/// Provides a simplified abstraction of a tokenizer.
/// </summary>
public class TokenizerView<TTokenizer, TSymbol, TSymbolType>
	where TTokenizer : Tokenizer<TSymbol, TSymbolType>
	where TSymbol : SymbolBase<TSymbolType>
	where TSymbolType : struct
{
	/// <summary>
	/// Initialises a new instance of <see cref="TokenizerView{TTokenizer,TSymbol,TSymbolType}"/>
	/// </summary>
	/// <param name="tokenizer">The tokenizer.</param>
	public TokenizerView(TTokenizer tokenizer) => Tokenizer = tokenizer;

	/// <summary>
	/// Gets the current symbol.
	/// </summary>
	public TSymbol? Current { get; private set; }

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
	public TTokenizer Tokenizer { get; private set; }

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
	public void PutBack(TSymbol symbol)
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
