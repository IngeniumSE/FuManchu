// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using System.Collections.Generic;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Represents the result of parsing.
/// </summary>
public class ParserResults
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ParserResults"/> class.
	/// </summary>
	/// <param name="document">The document.</param>
	/// <param name="errors">The errors.</param>
	public ParserResults(Block document, IList<Error>? errors)
		: this(errors == null || errors.Count == 0, document, errors)
	{

	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ParserResults"/> class.
	/// </summary>
	/// <param name="succes">We parsing succesful?</param>
	/// <param name="document">The document.</param>
	/// <param name="errors">The errors.</param>
	public ParserResults(bool succes, Block document, IList<Error>? errors)
	{
		Success = succes;
		Document = document;
		Errors = errors;
	}

	/// <summary>
	/// Gets the root block of the syntax tree.
	/// </summary>
	public Block Document { get; private set; }

	/// <summary>
	/// Gets the errors.
	/// </summary>
	public IList<Error>? Errors { get; private set; }

	/// <summary>
	/// Gets a whether the parse was succesful.
	/// </summary>
	public bool Success { get; private set; }
}
