// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

using System.Text;

/// <summary>
/// Provides extensions for text services.
/// </summary>
internal static class TextExtensions
{
	/// <summary>
	/// Begins the lookahead in the given buffer.
	/// </summary>
	/// <param name="self">The text buffer.</param>
	/// <returns>The lookahead token.</returns>
	public static LookaheadToken BeginLookahead(this ITextBuffer self)
	{
		var start = self.Position;
		return new LookaheadToken(() => self.Position = start);
	}

	/// <summary>
	/// Reads the remaining content in the buffer until the end.
	/// </summary>
	/// <param name="self">The text buffer.</param>
	/// <returns>The read string.</returns>
	public static string ReadToEnd(this ITextBuffer self)
	{
		var builder = new StringBuilder();
		int read;
		while ((read = self.Read()) != -1)
		{
			builder.Append((char)read);
		}
		return builder.ToString();
	}

	/// <summary>
	/// Seeks the number of characters in the buffer.
	/// </summary>
	/// <param name="self">The text buffer.</param>
	/// <param name="characters">The characters.</param>
	public static void Seek(this ITextBuffer self, int characters)
	{
		self.Position += characters;
	}
}
