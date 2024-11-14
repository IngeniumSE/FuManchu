// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

using FuManchu.Tokenizer;

namespace FuManchu;

/// <summary>
/// Represents a state machine.
/// </summary>
public abstract partial class StateMachine
{
	/// <summary>
	/// Gets the start state.
	/// </summary>
	protected abstract int StartState { get; }

	/// <summary>
	/// Gets or sets the current state.
	/// </summary>
	public int? CurrentState { get; set; }

	/// <summary>
	/// Gets or sets the current symbol.
	/// </summary>
	public HandlebarsSymbol? CurrentSymbol { get; set; }

	/// <summary>
	/// Stays this at the current state.
	/// </summary>
	/// <returns>The state result.</returns>
	protected StateResult Stay() => new(CurrentState);

	/// <summary>
	/// Stays this at the current state.
	/// </summary>
	/// <param name="result">The state output.</param>
	/// <returns>The state result.</returns>
	protected StateResult Stay(HandlebarsSymbol? result) => new(CurrentState, result);

	/// <summary>
	/// Stops this state machine.
	/// </summary>
	/// <returns>The state result.</returns>
	protected StateResult Stop() => default;

	/// <summary>
	/// Transitions the specified new state.
	/// </summary>
	/// <param name="state">The new state.</param>
	/// <returns>The state result.</returns>
	protected StateResult Transition(int state) => new(state);

	/// <summary>
	/// Transitions the specified new state.
	/// </summary>
	/// <param name="result">The last state output.</param>
	/// <param name="state">The new state.</param>
	/// <returns>The state result.</returns>
	protected StateResult Transition(HandlebarsSymbol? result, int state) => new(state, result);

	/// <summary>
	/// Advances to the next state.
	/// </summary>
	/// <returns>The output.</returns>
	protected virtual HandlebarsSymbol? Turn()
	{
		if (CurrentState != null)
		{
			do
			{
				var next = Dispatch();

				CurrentState = next.State;
				CurrentSymbol = next.Result;
			}
			while (CurrentState != null && CurrentSymbol == null);

			if (CurrentState == null)
			{
				return default; // Terminated;
			}

			return CurrentSymbol;
		}

		return default;
	}

	protected abstract StateResult Dispatch();
}
