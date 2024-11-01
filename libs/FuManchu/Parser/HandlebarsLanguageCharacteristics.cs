// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using System.Collections.Generic;

using FuManchu.Text;
using FuManchu.Tokenizer;

/// <summary>
/// Represents characteristics for the Handlebars language.
/// </summary>
public class HandlebarsLanguageCharacteristics : LanguageCharacteristics<HandlebarsTokenizer, HandlebarsSymbol, HandlebarsSymbolType>
{
	public static readonly HandlebarsLanguageCharacteristics Instance = new HandlebarsLanguageCharacteristics();

	/// <inheritdoc />
	protected override HandlebarsSymbol CreateSymbol(SourceLocation location, string content, HandlebarsSymbolType type, IEnumerable<Error> errors)
	{
		return new HandlebarsSymbol(location, content, type, errors);
	}

	/// <inheritdoc />
	public override HandlebarsTokenizer CreateTokenizer(ITextDocument source)
	{
		return new HandlebarsTokenizer(source);
	}
}
