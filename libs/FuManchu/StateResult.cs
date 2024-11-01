// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

partial class StateMachine<T>
{
	/// <summary>
	/// Represents a state result.
	/// </summary>
	public class StateResult
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StateResult"/> class.
		/// </summary>
		/// <param name="next">The next state.</param>
		public StateResult(State? next)
		{
			HasOutput = false;
			Next = next;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StateResult"/> class.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="next">The next.</param>
		public StateResult(T? output, State? next)
		{
			HasOutput = true;
			Output = output;
			Next = next;
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance has output.
		/// </summary>
		public bool HasOutput { get; set; }

		/// <summary>
		/// Gets or sets the output.
		/// </summary>
		public T? Output { get; set; }

		/// <summary>
		/// Gets or sets the next state.
		/// </summary>
		public State? Next { get; set; }
	}
}
