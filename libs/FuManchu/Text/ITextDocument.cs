// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

/// <summary>
/// Represents a text document.
/// </summary>
public interface ITextDocument : ITextBuffer
{
	/// <summary>
	/// Gets the current location in the document.
	/// </summary>
	SourceLocation Location { get; }
}
