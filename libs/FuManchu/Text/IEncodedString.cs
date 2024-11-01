// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

/// <summary>
/// Defines the required contract for implementing an encoded string.
/// </summary>
public interface IEncodedString
{
	/// <summary>
	/// Returns the encoded string.
	/// </summary>
	/// <returns>The encoded string.</returns>
	string ToEncodedString();
}
