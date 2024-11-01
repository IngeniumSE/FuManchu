// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using System;
using System.Collections.Generic;
using System.Linq;

using FuManchu.Text;
using FuManchu.Tokenizer;

/// <summary>
/// Provides services for a language.
/// </summary>
public abstract class LanguageCharacteristics<TTokenizer, TSymbol, TSymbolType>
	where TTokenizer : Tokenizer<TSymbol, TSymbolType>
	where TSymbol : SymbolBase<TSymbolType>
	where TSymbolType : struct
{
	/// <summary>
	/// Creates the symbol.
	/// </summary>
	/// <param name="location">The location.</param>
	/// <param name="content">The content.</param>
	/// <param name="type">The type.</param>
	/// <param name="errors">The errors.</param>
	/// <returns>The symbol instance.</returns>
	protected abstract TSymbol CreateSymbol(SourceLocation location, string content, TSymbolType type, IEnumerable<Error> errors);

	/// <summary>
	/// Creates the tokenizer.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <returns>The tokenizer instance.</returns>
	public abstract TTokenizer CreateTokenizer(ITextDocument source);

	/// <summary>
	/// Splits the symbol.
	/// </summary>
	/// <param name="symbol">The symbol.</param>
	/// <param name="splitAt">The index at which to split the symbol.</param>
	/// <param name="leftType">The left symbol type.</param>
	/// <returns>The split symbol as two new symbols.</returns>
	public virtual Tuple<TSymbol, TSymbol?> SplitSymbol(TSymbol symbol, int splitAt, TSymbolType leftType)
	{
		var left = CreateSymbol(symbol.Start, symbol.Content.Substring(0, splitAt), leftType, Enumerable.Empty<Error>());
		TSymbol? right = null;
		if (splitAt < symbol.Content.Length)
		{
			right = CreateSymbol(SourceLocationTracker.CalculateNewLocation(symbol.Start, left.Content), symbol.Content.Substring(splitAt), symbol.Type, symbol.Errors);
		}

		return Tuple.Create(left, right);
	}

	/// <summary>
	/// Tokenizes the string.
	/// </summary>
	/// <param name="content">The content.</param>
	/// <returns>The set of symbols from the source string.</returns>
	public virtual IEnumerable<TSymbol> TokenizeString(string content)
	{
		return TokenizeString(SourceLocation.Zero, content);
	}

	/// <summary>
	/// Tokenizes the string.
	/// </summary>
	/// <param name="start">The start.</param>
	/// <param name="content">The content.</param>
	/// <returns>The set of symbols from the source string.</returns>
	public virtual IEnumerable<TSymbol> TokenizeString(SourceLocation start, string content)
	{
		using (var reader = new SeekableTextReader(content))
		{
			var tok = CreateTokenizer(reader);
			TSymbol? sym;
			while ((sym = tok.NextSymbol()) != null)
			{
				sym.OffsetStart(start);
				yield return sym;
			}
		}
	}
}
