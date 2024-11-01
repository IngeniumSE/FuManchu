// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Tokenizer;

using FuManchu.Text;
using FuManchu.Tokenizer;

/// <summary>
/// Provides a base implementation of a Fact set for testing the <see cref="HandlebarsTokenizer"/>
/// </summary>
public abstract class HandlebarsTokenizerFactBase : TokenizerFactBase<HandlebarsTokenizer, HandlebarsSymbol, HandlebarsSymbolType>
{
	private static readonly HandlebarsSymbol _ignoreRemaining = new HandlebarsSymbol(0, 0, 0, string.Empty, HandlebarsSymbolType.Unknown);

	/// <inheritdoc />
	protected override HandlebarsSymbol IgnoreRemaining
	{
		get { return _ignoreRemaining; }
	}

	/// <inheritdoc />
	protected override Tokenizer<HandlebarsSymbol, HandlebarsSymbolType> CreateTokenizer(ITextDocument source)
	{
		return new HandlebarsTokenizer(source);
	}

	protected virtual HandlebarsSymbol S(int absolute, int line, int character, string content, HandlebarsSymbolType type)
	{
		return new HandlebarsSymbol(absolute, line, character, content, type);
	}
}
