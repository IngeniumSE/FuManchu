// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using System.Globalization;

/// <summary>
/// Provides helper methods for parsing.
/// </summary>
public static class ParserHelpers
{
	/// <summary>
	/// Determines whether the specified value is a hex digit.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>True if the value is a hex digit, otherwise false.</returns>
	public static bool IsHexDigit(char value)
	{
		return (value >= '0' && value <= '9') || (value >= 'A' && value <= 'F') || (value >= 'a' && value <= 'f');
	}

	/// <summary>
	/// Determines whether the given character represents a new line.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>True if the character is a new line, otherwise false.</returns>
	public static bool IsNewLine(char value)
	{
		return value == '\r' || value == '\n' || value == '\u0085' || value == '\u2028' || value == '\u2029';
	}

	/// <summary>
	/// Determines whether the given character represents whitespace.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>True if the character is whitespace, otherwise false.</returns>
	public static bool IsWhiteSpace(char value)
	{
		return value == ' ' || value == '\f' || value == '\t' || value == '\u000B' || CharUnicodeInfo.GetUnicodeCategory(value) == UnicodeCategory.SpaceSeparator;
	}
}
