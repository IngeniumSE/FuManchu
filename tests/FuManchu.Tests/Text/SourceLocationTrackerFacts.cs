// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Text;

using FuManchu.Text;

using Xunit;

/// <summary>
/// Provides tests for the <see cref="SourceLocationTracker"/> type.
/// </summary>
public class SourceLocationTrackerFacts
{
	private static readonly SourceLocation TestStartLocation = new SourceLocation(10, 42, 45);

	[Fact]
	public void ConstructorSetsCurrentLocationToZero()
	{
		var tracker = new SourceLocationTracker();

		Assert.Equal(SourceLocation.Zero, tracker.CurrentLocation);
	}

	[Fact]
	public void ConstructorWithSourceLocationSetsCurrentLocationToSpecifiedValue()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);

		Assert.Equal(TestStartLocation, tracker.CurrentLocation);
	}

	[Fact]
	public void UpdateLocationAdvancesCorrectlyForMultilineString()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);
		const string text = "foo\nbar\rbaz\r\nbox";

		tracker.UpdateLocation(text);

		Assert.Equal(26, tracker.CurrentLocation.Absolute);
		Assert.Equal(45, tracker.CurrentLocation.Line);
		Assert.Equal(3, tracker.CurrentLocation.Character);
	}

	[Fact]
	public void UpdateLocationAdvancesAbsoluteIndexOnNonNewlineCharacter()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);

		tracker.UpdateLocation('f', 'o');

		Assert.Equal(11, tracker.CurrentLocation.Absolute);
	}

	[Fact]
	public void UpdateLocationAdvancesCharacterIndexOnNonNewlineCharacter()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);

		tracker.UpdateLocation('f', 'o');

		Assert.Equal(46, tracker.CurrentLocation.Character);
	}

	[Fact]
	public void UpdateLocationDoesNotAdvancedLineIndexOnNonNewlineCharacter()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);

		tracker.UpdateLocation('f', 'o');

		Assert.Equal(42, tracker.CurrentLocation.Line);
	}

	[Fact]
	public void UpdateLocationAdvancesLineIndexOnSlashN()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);

		tracker.UpdateLocation('\n', 'o');

		Assert.Equal(43, tracker.CurrentLocation.Line);
	}

	[Fact]
	public void UpdateLocationRestsCharacterIndexOnSlashN()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);

		tracker.UpdateLocation('\n', 'o');

		Assert.Equal(0, tracker.CurrentLocation.Character);
	}

	[Fact]
	public void UpdateLocationAdvancesLineIndexOnSlashRFollowedByNonNewlineCharacter()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);

		tracker.UpdateLocation('\r', 'o');

		Assert.Equal(43, tracker.CurrentLocation.Line);
	}

	[Fact]
	public void UpdateLocationAdvancesAbsoluteIndexOnSlashRFollowedByNonNewlineCharacter()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);

		tracker.UpdateLocation('\r', 'o');

		Assert.Equal(11, tracker.CurrentLocation.Absolute);
	}

	[Fact]
	public void UpdateLocationResetsCharacterIndexOnSlashRFollowedByNonNewlineCharacter()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);

		tracker.UpdateLocation('\r', 'o');

		Assert.Equal(0, tracker.CurrentLocation.Character);
	}

	[Fact]
	public void UpdateLocationDoesNotAdvancedLineIndexOnSlashRFollowedBySlashN()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);

		tracker.UpdateLocation('\r', '\n');

		Assert.Equal(42, tracker.CurrentLocation.Line);
	}

	[Fact]
	public void UpdateLocationAdvancesAbsoluteIndexOnSlashRFollowedBySlashN()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);

		tracker.UpdateLocation('\r', '\n');

		Assert.Equal(11, tracker.CurrentLocation.Absolute);
	}

	[Fact]
	public void UpdateLocationAdvancesCharacterIndexOnSlashRFollowedBySlashN()
	{
		var tracker = new SourceLocationTracker(TestStartLocation);

		tracker.UpdateLocation('\r', '\n');

		Assert.Equal(46, tracker.CurrentLocation.Character);
	}
}
