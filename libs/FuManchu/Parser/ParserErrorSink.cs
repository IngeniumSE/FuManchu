// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using System.Collections.Generic;

using FuManchu.Text;

/// <summary>
/// Manages <see cref="Error"/>'s that are encountered during parsing.
/// </summary>
public class ParserErrorSink
{
	private readonly List<Error> _errors = new List<Error>();

	/// <summary>
	/// Gets the errors.
	/// </summary>
	public IEnumerable<Error> Errors { get { return _errors; } }

	/// <summary>
	/// Adds an error to the sink.
	/// </summary>
	/// <param name="error">The error.</param>
	public void OnError(Error error)
	{
		_errors.Add(error);
	}

	/// <summary>
	/// Adds an error to the sink.
	/// </summary>
	/// <param name="location">The location.</param>
	/// <param name="message">The message.</param>
	public void OnError(SourceLocation location, string message)
	{
		OnError(new Error(message, location));
	}

	/// <summary>
	/// Adds an error to the sink.
	/// </summary>
	/// <param name="location">The location.</param>
	/// <param name="message">The message.</param>
	/// <param name="length">The length.</param>
	public void OnError(SourceLocation location, string message, int length)
	{
		OnError(new Error(message, location, length));
	}
}
