// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using FuManchu.Text;

/// <summary>
/// Represents a safe string that doesn't support encoding, it is already considered to be encoded.
/// </summary>
public class SafeString(object? value) : IEncodedString
{
	private readonly object? _value = value;

	/// <inheritdoc />
	public string ToEncodedString()
		=> _value is null ? string.Empty : _value.ToString();
}
