// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

using System;

/// <summary>
/// Represents a location in template source.
/// </summary>
public struct SourceLocation : IEquatable<SourceLocation>, IComparable<SourceLocation>
{
	public static readonly SourceLocation Undefined = new SourceLocation(-1, -1, -1);
	public static readonly SourceLocation Zero = new SourceLocation(0, 0, 0);

	private readonly int _absolute;
	private readonly int _line;
	private readonly int _character;

	/// <summary>
	/// Initializes a new instance of the <see cref="SourceLocation"/> struct.
	/// </summary>
	/// <param name="absolute">The absolute.</param>
	/// <param name="line">The line.</param>
	/// <param name="character">The character.</param>
	public SourceLocation(int absolute, int line, int character)
	{
		_absolute = absolute;
		_line = line;
		_character = character;
	}

	/// <summary>
	/// Gets the absolute index.
	/// </summary>
	public int Absolute { get { return _absolute; } }

	/// <summary>
	/// Gets the character index.
	/// </summary>
	public int Character { get { return _character; } }

	/// <summary>
	/// Gets the line index.
	/// </summary>
	public int Line { get { return _line; } }

	/// <summary>
	/// Adds the two source locations together.
	/// </summary>
	/// <param name="left">The left source location.</param>
	/// <param name="right">The right source location.</param>
	/// <returns>The result source location.</returns>
	public static SourceLocation Add(SourceLocation left, SourceLocation right)
	{
		if (right.Line > 0)
		{
			return new SourceLocation(
				left.Absolute + right.Absolute,
				left.Line + right.Line,
				right.Character);
		}

		return new SourceLocation(
			left.Absolute + right.Absolute,
			left.Line + right.Line,
			left.Character + right.Character);
	}

	/// <summary>
	/// Subtracts the right source location from the left.
	/// </summary>
	/// <param name="left">The left source location.</param>
	/// <param name="right">The right source location.</param>
	/// <returns>The result source location.</returns>
	public static SourceLocation Subtract(SourceLocation left, SourceLocation right)
	{
		return new SourceLocation(
			left.Absolute - right.Absolute,
			left.Line - right.Line,
			left.Line != right.Line ? left.Character : left.Character - right.Character);
	}

	/// <inheritdoc />
	public int CompareTo(SourceLocation other)
	{
		return Absolute.CompareTo(other.Absolute);
	}

	/// <inheritdoc />
	public bool Equals(SourceLocation other)
	{
		return (Absolute == other.Absolute && Line == other.Line && Character == other.Character);
	}

	/// <inheritdoc />
	public override bool Equals(object obj)
	{
		return (obj is SourceLocation) && Equals((SourceLocation)obj);
	}

	/// <inheritdoc />
	public override int GetHashCode()
	{
		return Absolute;
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return string.Format("({0}:{1},{2})", Absolute, Line, Character);
	}

	/// <summary>
	/// Implements the operator &lt;.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	/// The result of the operator.
	/// </returns>
	public static bool operator <(SourceLocation left, SourceLocation right)
	{
		return left.CompareTo(right) < 0;
	}

	/// <summary>
	/// Implements the operator &gt;.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	/// The result of the operator.
	/// </returns>
	public static bool operator >(SourceLocation left, SourceLocation right)
	{
		return left.CompareTo(right) > 0;
	}

	/// <summary>
	/// Implements the operator ==.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	/// The result of the operator.
	/// </returns>
	public static bool operator ==(SourceLocation left, SourceLocation right)
	{
		return left.Equals(right);
	}

	/// <summary>
	/// Implements the operator !=.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	/// The result of the operator.
	/// </returns>
	public static bool operator !=(SourceLocation left, SourceLocation right)
	{
		return !left.Equals(right);
	}

	/// <summary>
	/// Implements the operator +.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	/// The result of the operator.
	/// </returns>
	public static SourceLocation operator +(SourceLocation left, SourceLocation right)
	{
		return Add(left, right);
	}

	/// <summary>
	/// Implements the operator -.
	/// </summary>
	/// <param name="left">The left.</param>
	/// <param name="right">The right.</param>
	/// <returns>
	/// The result of the operator.
	/// </returns>
	public static SourceLocation operator -(SourceLocation left, SourceLocation right)
	{
		return Subtract(left, right);
	}
}
