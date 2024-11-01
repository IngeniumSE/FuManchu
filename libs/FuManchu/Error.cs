// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using FuManchu.Text;

/// <summary>
/// Represents an error in a Handlerbars template.
/// </summary>
public class Error
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Error"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="location">The location.</param>
	/// <param name="length">The length of the error.</param>
	public Error(string message, SourceLocation location, int length = 0)
	{
		Message = message;
		Location = location;
		Length = length;
	}

	/// <summary>
	/// Gets the length.
	/// </summary>
	public int Length { get; private set; }

	/// <summary>
	/// Gets the location of the error.
	/// </summary>
	public SourceLocation Location { get; private set; }

	/// <summary>
	/// Gets the message.
	/// </summary>
	public string Message { get; private set; }
}
