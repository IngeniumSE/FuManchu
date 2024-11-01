// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

using System.IO;

/// <summary>
/// Provides text reader services with seek actions.
/// </summary>
public class SeekableTextReader : TextReader, ITextDocument
{
	private int _position = 0;
	private readonly LineTrackingStringBuffer _buffer = new LineTrackingStringBuffer();
	private SourceLocation _location = SourceLocation.Zero;
	private char? _current;

	/// <summary>
	/// Initializes a new instance of the <see cref="SeekableTextReader"/> class.
	/// </summary>
	/// <param name="content">The content.</param>
	public SeekableTextReader(string content)
	{
		_buffer.Append(content);
		UpdateState();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SeekableTextReader"/> class.
	/// </summary>
	/// <param name="source">The source.</param>
	public SeekableTextReader(TextReader source)
		: this(source.ReadToEnd()) { }

	/// <summary>
	/// Initializes a new instance of the <see cref="SeekableTextReader"/> class.
	/// </summary>
	/// <param name="buffer">The buffer.</param>
	public SeekableTextReader(ITextBuffer buffer)
		: this(buffer.ReadToEnd()) { }

	/// <summary>
	/// Gets the buffer.
	/// </summary>
	internal LineTrackingStringBuffer Buffer { get { return _buffer; } }

	/// <inheritdoc />
	public int Length { get { return _buffer.Length; } }

	/// <inheritdoc />
	public SourceLocation Location { get { return _location; } }

	/// <inheritdoc />
	public int Position
	{
		get { return _position; }
		set
		{
			if (_position != value)
			{
				_position = value;
				UpdateState();
			}
		}
	}

	/// <inheritdoc />
	public override int Peek()
	{
		if (_current == null)
		{
			return -1;
		}

		return _current.Value;
	}

	/// <inheritdoc />
	public override int Read()
	{
		if (_current == null)
		{
			return -1;
		}

		var chr = _current.Value;
		_position++;
		UpdateState();
		return chr;
	}

	/// <summary>
	/// Updates the state of the reader.
	/// </summary>
	private void UpdateState()
	{
		if (_position < _buffer.Length)
		{
			var reference = _buffer.CharAt(_position);
			_current = reference.Character;
			_location = reference.Location;
		}
		else if (_buffer.Length == 0)
		{
			_current = null;
			_location = SourceLocation.Zero;
		}
		else
		{
			_current = null;
			_location = _buffer.EndLocation;
		}
	}
}
