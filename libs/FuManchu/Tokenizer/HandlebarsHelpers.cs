// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System;
using System.Globalization;

/// <summary>
/// Provides helper methods for Handlebars syntax.
/// </summary>
public static class HandlebarsHelpers
{
	/// <summary>
	/// Determines whether the given character is the start of an identifier.
	/// </summary>
	/// <param name="character">The character.</param>
	/// <returns>True if the character is the start of an identifier, otherwise false.</returns>
	public static bool IsIdentifierStart(char character)
	{
		return Char.IsLetter(character) || character == '_' || character == '$' || character == '@' || CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.LetterNumber;
	}

	/// <summary>
	/// Determines whether the given character is part of an identifier.
	/// </summary>
	/// <param name="character">The character.</param>
	/// <returns>True if the character can be considered part of an identifier, otherwise false.</returns>
	public static bool IsIdentifierPart(char character)
	{
		return Char.IsDigit(character) || IsIdentifierStart(character) || IsIdentifierPartByUnicodeCategory(character);
	}

	/// <summary>
	/// Determines whether the given character can be considered part of an identifier by the unicode category of the character.
	/// </summary>
	/// <param name="character">The character.</param>
	/// <returns>True if the character can be considered part of an identifier, otherwise false.</returns>
	private static bool IsIdentifierPartByUnicodeCategory(char character)
	{
		var category = CharUnicodeInfo.GetUnicodeCategory(character);

		return category == UnicodeCategory.NonSpacingMark || category == UnicodeCategory.SpacingCombiningMark || category == UnicodeCategory.ConnectorPunctuation || category == UnicodeCategory.Format;
	}
}
