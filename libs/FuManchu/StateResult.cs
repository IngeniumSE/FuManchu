// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using FuManchu.Tokenizer;

namespace FuManchu;

partial class StateMachine
{
	/// <summary>
	/// Represents a state result.
	/// </summary>
	public readonly struct StateResult(int? state = null, HandlebarsSymbol? result = default)
	{
		/// <summary>
		/// Represents the next state to transition to.
		/// </summary>
		public int? State { get; } = state;

		/// <summary>
		/// Represents the output of the state.
		/// </summary>
		public HandlebarsSymbol? Result { get; } = result;
	}
}
