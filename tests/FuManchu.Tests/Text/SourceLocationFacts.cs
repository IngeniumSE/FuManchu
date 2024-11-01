// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Text;

using FuManchu.Text;

using Xunit;

/// <summary>
/// Provides tests for the <see cref="SourceLocation"/> type.
/// </summary>
public class SourceLocationFacts
{
	[Fact]
	public void ConstructorSetsAssociatedProperties()
	{
		var location = new SourceLocation(0, 42, 24);

		Assert.Equal(0, location.Absolute);
		Assert.Equal(42, location.Line);
		Assert.Equal(24, location.Character);
	}

	[Fact]
	public void CanAddTwoLocations()
	{
		var first = new SourceLocation(1, 1, 1);
		var second = new SourceLocation(4, 1, 1);

		var expected = new SourceLocation(5, 2, 1);
		var result = SourceLocation.Add(first, second);

		Assert.Equal(expected.Absolute, result.Absolute);
		Assert.Equal(expected.Line, result.Line);
		Assert.Equal(expected.Character, result.Character);
	}

	[Fact]
	public void CanSubtractTwoLocations()
	{
		var first = new SourceLocation(5, 2, 1);
		var second = new SourceLocation(4, 1, 1);

		var expected = new SourceLocation(1, 1, 1);
		var result = SourceLocation.Subtract(first, second);

		Assert.Equal(expected.Absolute, result.Absolute);
		Assert.Equal(expected.Line, result.Line);
		Assert.Equal(expected.Character, result.Character);
	}
}
