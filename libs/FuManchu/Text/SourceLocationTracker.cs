// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

using FuManchu.Parser;

/// <summary>
/// Provides tracking of source location changes.
/// </summary>
public class SourceLocationTracker
{
	private int _absolute = 0;
	private int _line = 0;
	private int _character = 0;
	private SourceLocation _currentLocation;

	/// <summary>
	/// Initializes a new instance of the <see cref="SourceLocationTracker"/> class.
	/// </summary>
	public SourceLocationTracker() : this(SourceLocation.Zero) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="SourceLocationTracker"/> class.
	/// </summary>
	/// <param name="currentLocation">The current location.</param>
	public SourceLocationTracker(SourceLocation currentLocation)
	{
		_currentLocation = currentLocation;

		UpdateInternalState();
	}

	/// <summary>
	/// Gets or sets the current location.
	/// </summary>
	public SourceLocation CurrentLocation
	{
		get { return _currentLocation; }
		set
		{
			if (_currentLocation != value)
			{
				_currentLocation = value;
				UpdateInternalState();
			}
		}
	}

	/// <summary>
	/// Calculates the new location.
	/// </summary>
	/// <param name="lastPosition">The last position.</param>
	/// <param name="content">The content.</param>
	/// <returns>The new source location.</returns>
	public static SourceLocation CalculateNewLocation(SourceLocation lastPosition, string content)
	{
		return new SourceLocationTracker(lastPosition).UpdateLocation(content).CurrentLocation;
	}

	/// <summary>
	/// Updates the location based on the character read.
	/// </summary>
	/// <param name="read">The read character.</param>
	/// <param name="next">The next character.</param>
	public void UpdateLocation(char read, char next)
	{
		UpdateCharacterCore(read, next);
		RecalculateSourceLocation();
	}

	/// <summary>
	/// Updates the location using the content read.
	/// </summary>
	/// <param name="content">The content.</param>
	/// <returns>This tracker.</returns>
	public SourceLocationTracker UpdateLocation(string? content)
	{
		if (content is { Length: > 0 })
		{
			for (int i = 0; i < content.Length; i++)
			{
				var next = '\0';
				if (i < content.Length - 1)
				{
					next = content[i + 1];
				}
				UpdateCharacterCore(content[i], next);
			}
		}
		RecalculateSourceLocation();
		return this;
	}

	/// <summary>
	/// Updates the character and line indexes based on the changes.
	/// </summary>
	/// <param name="read">The read.</param>
	/// <param name="next">The next.</param>
	private void UpdateCharacterCore(char read, char next)
	{
		_absolute++;

		if (ParserHelpers.IsNewLine(read) && (read != '\r' || next != '\n'))
		{
			_line++;
			_character = 0;
		}
		else
		{
			_character++;
		}
	}

	/// <summary>
	/// Updates the internal state of the tracker.
	/// </summary>
	private void UpdateInternalState()
	{
		_absolute = _currentLocation.Absolute;
		_line = _currentLocation.Line;
		_character = _currentLocation.Character;
	}

	/// <summary>
	/// Recalculates the source location.
	/// </summary>
	private void RecalculateSourceLocation()
	{
		_currentLocation = new SourceLocation(_absolute, _line, _character);
	}
}
