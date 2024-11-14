// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Text;

using FuManchu.Text;

using Xunit;

/// <summary>
/// Provides tests for the <see cref="LineTrackingStringBuffer"/> type.
/// </summary>
public class LineTrackingStringBufferFacts
{
	[Fact]
	public void ConstructorInitialisesProperties()
	{
		using var buffer = new LineTrackingStringBuffer();

		Assert.Equal(0, buffer.Length);
	}

	[Fact]
	public void CharAtCorrectlyReturnsLocation()
	{
		using var buffer = new LineTrackingStringBuffer();
		buffer.Append("foo\rbar\nbaz\r\nbiz");

		var chr = buffer.CharAt(14);

		Assert.Equal('i', chr.Character);
		Assert.Equal(new SourceLocation(14, 3, 1), chr.Location);
	}
}
