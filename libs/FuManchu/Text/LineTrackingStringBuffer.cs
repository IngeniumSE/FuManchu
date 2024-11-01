// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

using System;
using System.Collections.Generic;

using FuManchu.Parser;

/// <summary>
/// Represents a string buffer with line tracking.
/// </summary>
public class LineTrackingStringBuffer
{
	private TextLine? _currentLine;
	private TextLine _endLine;
	private readonly IList<TextLine> _lines;

	/// <summary>
	/// Initializes a new instance of the <see cref="LineTrackingStringBuffer"/> class.
	/// </summary>
	public LineTrackingStringBuffer()
	{
		_endLine = new TextLine(0, 0);
		_lines = new List<TextLine> { _endLine };
	}

	/// <summary>
	/// Gets the end location.
	/// </summary>
	public SourceLocation EndLocation
	{
		get { return new SourceLocation(Length, _lines.Count - 1, _lines[_lines.Count - 1].Length); }
	}

	/// <summary>
	/// Gets the length of the buffer.
	/// </summary>
	public int Length
	{
		get { return _endLine.End; }
	}

	/// <summary>
	/// Appends the specified content to the buffer.
	/// </summary>
	/// <param name="content">The content.</param>
	public void Append(string content)
	{
		for (int i = 0; i < content.Length; i++)
		{
			AppendCore(content[i]);

			if ((content[i] == '\r' && (i + 1 == content.Length || content[i + 1] != '\n'))
				|| (content[i] != '\r' && ParserHelpers.IsNewLine(content[i])))
			{
				PushNewLine();
			}
		}
	}

	/// <summary>
	/// Appends the specified character to the current line.
	/// </summary>
	/// <param name="character">The character.</param>
	private void AppendCore(char character)
	{
		_lines[_lines.Count - 1].Content.Append(character);
	}

	/// <summary>
	/// Gets the character at the given absolute index.
	/// </summary>
	/// <param name="absolute">The absolute.</param>
	/// <returns>The character reference.</returns>
	/// <exception cref="System.ArgumentOutOfRangeException">absolute</exception>
	public CharacterReference CharAt(int absolute)
	{
		var line = FindLine(absolute);
		if (line is null)
		{
			throw new ArgumentOutOfRangeException("absolute");
		}
		var idx = absolute - line.Start;
		return new CharacterReference(line.Content[idx], new SourceLocation(absolute, line.Index, idx));
	}

	/// <summary>
	/// Finds the line that contains the given index.
	/// </summary>
	/// <param name="absolute">The absolute.</param>
	/// <returns>The line that contains the given index.</returns>
	private TextLine? FindLine(int absolute)
	{
		TextLine? selected = null;

		if (_currentLine is not null)
		{
			if (_currentLine.Contains(absolute))
			{
				selected = _currentLine;
			}
			else if (absolute > _currentLine.Index && _currentLine.Index + 1 < _lines.Count)
			{
				selected = ScanLines(absolute, _currentLine.Index);
			}
		}

		if (selected is null)
		{
			selected = ScanLines(absolute, 0);
		}

		_currentLine = selected;
		return selected;
	}

	/// <summary>
	/// Pushes a new line.
	/// </summary>
	private void PushNewLine()
	{
		_endLine = new TextLine(_endLine.End, _endLine.Index + 1);
		_lines.Add(_endLine);
	}

	/// <summary>
	/// Scans the lines to try and find the line that contains the given absolute index.
	/// </summary>
	/// <param name="absolute">The absolute.</param>
	/// <param name="start">The start.</param>
	/// <returns>The text line.</returns>
	private TextLine? ScanLines(int absolute, int start)
	{
		for (int i = 0; i < _lines.Count; i++)
		{
			var idx = (i + start) % _lines.Count;
			if (_lines[idx].Contains(absolute))
			{
				return _lines[idx];
			}
		}

		return null;
	}

	/// <summary>
	/// Represents a reference to a character.
	/// </summary>
	public record struct CharacterReference(char Character, SourceLocation Location);
}
