// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

/// <summary>
/// Defines the required contract for implementing a text buffer.
/// </summary>
public interface ITextBuffer
{
	/// <summary>
	/// Gets the length of the text buffer.
	/// </summary>
	int Length { get; }

	/// <summary>
	/// Gets or sets the read position of the text buffer.
	/// </summary>
	int Position { get; set; }

	/// <summary>
	/// Peeks at the next character in the text buffer.
	/// </summary>
	/// <returns>The read character.</returns>
	int Peek();

	/// <summary>
	/// Reads the next character and advances the read position.
	/// </summary>
	/// <returns>The read character.</returns>
	int Read();
}
