// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

/// <summary>
/// Provides a context for a backtrack operation.
/// </summary>
internal class BacktrackContext
{
	/// <summary>
	/// Gets or sets the index of the buffer.
	/// </summary>
	/// <value>
	/// The index of the buffer.
	/// </value>
	public int BufferIndex { get; set; }

	/// <summary>
	/// Gets or sets the location.
	/// </summary>
	public SourceLocation Location { get; set; }
}
