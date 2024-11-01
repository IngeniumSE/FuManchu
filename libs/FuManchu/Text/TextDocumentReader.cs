// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

using System.IO;

/// <summary>
/// Provides read operations on a text document.
/// </summary>
public class TextDocumentReader : TextReader, ITextDocument
{
	/// <summary>
	/// Initializes a new instance of the <see cref="TextDocumentReader"/> class.
	/// </summary>
	/// <param name="source">The source.</param>
	public TextDocumentReader(ITextDocument source)
	{
		Document = source;
	}

	/// <summary>
	/// Gets the document.
	/// </summary>
	internal ITextDocument Document { get; private set; }

	/// <inheritdoc />
	public int Length { get { return Document.Length; } }

	/// <inheritdoc />
	public SourceLocation Location { get { return Document.Location; } }

	/// <inheritdoc />
	public int Position
	{
		get { return Document.Position; }
		set { Document.Position = value; }
	}

	/// <inheritdoc />
	public override int Peek()
	{
		return Document.Peek();
	}

	/// <inheritdoc />
	public override int Read()
	{
		return Document.Read();
	}
}
