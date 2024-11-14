// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tokenizer;

using FuManchu.Text;

/// <summary>
/// Defines the required contract for implementing a symbol.
/// </summary>
public interface ISymbol
{
	/// <summary>
	/// Gets the content of the symbol.
	/// </summary>
	string Content { get; }

	/// <summary>
	/// Gets whether the symbol has value.
	/// </summary>
	bool HasValue { get; }

	/// <summary>
	/// Gets the start of the symbol.
	/// </summary>
	SourceLocation Start { get; }

	/// <summary>
	/// Gets the errors for the symbol.
	/// </summary>
	IEnumerable<Error> Errors { get; }

	/// <summary>
	/// Changes the start of the symbol.
	/// </summary>
	/// <param name="newStart">The new start.</param>
	void ChangeStart(SourceLocation newStart);

	/// <summary>
	/// Offsets the start of the symbol based on the document start.
	/// </summary>
	/// <param name="documentStart">The document start.</param>
	void OffsetStart(SourceLocation documentStart);
}

public interface ISymbol<T> : ISymbol where T : struct
{
	/// <summary>
	/// Gets the symbol type.
	/// </summary>
	T Type { get; }
}
