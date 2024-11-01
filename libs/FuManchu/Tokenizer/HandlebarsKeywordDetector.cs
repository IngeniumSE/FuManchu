// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tokenizer;

using System.Collections.Generic;

/// <summary>
/// Provides mappings of strings to <see cref="HandlebarsKeyword"/> instances.
/// </summary>
internal static class HandlebarsKeywordDetector
{
	private static readonly Dictionary<string, HandlebarsKeyword> _keywords = new Dictionary<string, HandlebarsKeyword>
	{
		{ "each", HandlebarsKeyword.Each },
		{ "if", HandlebarsKeyword.If },
		{ "unless", HandlebarsKeyword.Unless },
		{ "else", HandlebarsKeyword.Else },
		{ "with", HandlebarsKeyword.With },
		{ "this", HandlebarsKeyword.This },
		{ "is", HandlebarsKeyword.Is }
	};

	/// <summary>
	/// Gets the keyword for the given symbol identifier.
	/// </summary>
	/// <param name="id">The identifier.</param>
	/// <returns>The Handlebars keyword.</returns>
	public static HandlebarsKeyword? SymbolTypeForIdentifier(string id)
	{
		HandlebarsKeyword type;
		if (_keywords.TryGetValue(id, out type))
		{
			return type;
		}

		return null;
	}
}
