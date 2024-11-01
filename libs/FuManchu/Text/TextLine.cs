// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

using System.Text;

/// <summary>
/// Represents a line of text.
/// </summary>
public class TextLine
{
	private readonly StringBuilder _builder = new StringBuilder();

	/// <summary>
	/// Initializes a new instance of the <see cref="TextLine"/> class.
	/// </summary>
	/// <param name="start">The start.</param>
	/// <param name="index">The index.</param>
	public TextLine(int start, int index)
	{
		Start = start;
		Index = index;
	}

	/// <summary>
	/// Gets the string builder.
	/// </summary>
	public StringBuilder Content { get { return _builder; } }

	/// <summary>
	/// Gets the end.
	/// </summary>
	public int End
	{
		get { return Start + Length; }
	}

	/// <summary>
	/// Gets or sets the length.
	/// </summary>
	public int Index { get; set; }

	/// <summary>
	/// Gets the length.
	/// </summary>
	public int Length { get { return Content.Length; } }

	/// <summary>
	/// Gets or sets the start.
	/// </summary>
	public int Start { get; set; }

	/// <summary>
	/// Determines whether the text line contains the given index.
	/// </summary>
	/// <param name="index">The index.</param>
	/// <returns>True if the text line contains the index, otherwise false.</returns>
	public bool Contains(int index)
	{
		return index < End && index >= Start;
	}
}
