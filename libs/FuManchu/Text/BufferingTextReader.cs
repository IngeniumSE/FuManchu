// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// Provides a buffered text reader with lookahead operations.
/// </summary>
public class BufferingTextReader : LookaheadTextReader
{
	private readonly Stack<BacktrackContext> _backtrackStack = new Stack<BacktrackContext>();
	private int _currentBufferPosition;
	private int _currentCharacter;
	private readonly SourceLocationTracker _tracker;

	/// <summary>
	/// Initializes a new instance of the <see cref="BufferingTextReader"/> class.
	/// </summary>
	/// <param name="source">The source.</param>
	public BufferingTextReader(TextReader source)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}

		InnerReader = source;
		_tracker = new SourceLocationTracker();

		UpdateCurrentCharacter();
	}

	/// <summary>
	/// Gets or sets the buffer.
	/// </summary>
	internal StringBuilder? Buffer { get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="BufferingTextReader"/> is buffering.
	/// </summary>
	internal bool Buffering { get; set; }

	/// <summary>
	/// Gets the current character.
	/// </summary>
	public virtual int CurrentCharacter
	{
		get { return _currentCharacter; }
	}

	/// <inheritdoc />
	public override SourceLocation CurrentLocation
	{
		get { return _tracker.CurrentLocation; }
	}

	/// <summary>
	/// Gets or sets the inner reader.
	/// </summary>
	internal TextReader InnerReader { get; set; }

	/// <inheritdoc />
	public override IDisposable BeginLookahead()
	{
		if (Buffer == null)
		{
			Buffer = new StringBuilder();
		}

		if (!Buffering)
		{
			ExpandBuffer();
			Buffering = true;
		}

		var context = new BacktrackContext
		{
			BufferIndex = _currentBufferPosition,
			Location = CurrentLocation
		};
		_backtrackStack.Push(context);

		return new DisposableAction(() => EndLookahead(context));
	}

	/// <inheritdoc />
	public override void CancelBacktrack()
	{
		_backtrackStack.Pop();
	}

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			InnerReader.Dispose();
		}
		base.Dispose(disposing);
	}

	/// <inheritdoc />
	public override int Peek()
	{
		return CurrentCharacter;
	}

	/// <inheritdoc />
	public override int Read()
	{
		int @char = CurrentCharacter;
		NextCharacter();
		return @char;
	}

	/// <summary>
	/// Ends the lookahead.
	/// </summary>
	/// <param name="context">The context.</param>
	private void EndLookahead(BacktrackContext context)
	{
		if (_backtrackStack.Count > 0 && ReferenceEquals(_backtrackStack.Peek(), context))
		{
			_backtrackStack.Pop();
			_currentBufferPosition = context.BufferIndex;
			_tracker.CurrentLocation = context.Location;

			UpdateCurrentCharacter();
		}
	}

	/// <summary>
	/// Attempts to expand the buffer using the next character from the stream.
	/// </summary>
	/// <returns>True if the buffer was expanded, otherwise false (and we're at the end of the stream).</returns>
	protected bool ExpandBuffer()
	{
		int @char = InnerReader.Read();

		if (@char != -1)
		{
			if (Buffer is null)
			{
				Buffer = new();
			}

			Buffer.Append((char)@char);
			_currentBufferPosition = Buffer.Length - 1;
			return true;
		}

		return false;
	}

	/// <summary>
	/// Moves to the next character in the stream.
	/// </summary>
	protected virtual void NextCharacter()
	{
		int prev = CurrentCharacter;
		if (prev == -1)
		{
			// At the end of the source.
			return;
		}

		if (Buffering && Buffer is not null)
		{
			if (_currentBufferPosition >= Buffer.Length - 1)
			{
				if (_backtrackStack.Count == 0)
				{
					Buffer.Length = 0;
					_currentBufferPosition = 0;
					Buffering = false;
				}
				else if (!ExpandBuffer())
				{
					_currentBufferPosition = Buffer.Length;
				}
			}
			else
			{
				_currentBufferPosition++;
			}
		}
		else
		{
			InnerReader.Read();
		}

		UpdateCurrentCharacter();
		_tracker.UpdateLocation((char)prev, (char)CurrentCharacter);
	}

	/// <summary>
	/// Updates the current character.
	/// </summary>
	private void UpdateCurrentCharacter()
	{
		if (Buffer is null)
		{
			return;
		}

		if (Buffering && _currentBufferPosition < Buffer.Length)
		{
			_currentCharacter = Buffer[_currentBufferPosition];
		}
		else
		{
			_currentCharacter = InnerReader.Read();
		}
	}
}
