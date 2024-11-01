// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

/// <summary>
/// Represents a state machine.
/// </summary>
/// <typeparam name="T">The state machine.</typeparam>
public abstract partial class StateMachine<T>
{
	public delegate StateResult State();

	/// <summary>
	/// Gets the start state.
	/// </summary>
	protected abstract State StartState { get; }

	/// <summary>
	/// Gets or sets the current state.
	/// </summary>
	public State? CurrentState { get; set; }

	/// <summary>
	/// Stays this at the current state.
	/// </summary>
	/// <returns>The state result.</returns>
	protected StateResult Stay() => new StateResult(CurrentState);

	/// <summary>
	/// Stays this at the current state.
	/// </summary>
	/// <param name="output">The output.</param>
	/// <returns>The state result.</returns>
	protected StateResult Stay(T? output) => new StateResult(output, CurrentState);

	/// <summary>
	/// Stops this state machine.
	/// </summary>
	/// <returns>The state result.</returns>
	protected StateResult Stop() => new(null);

	/// <summary>
	/// Transitions the specified new state.
	/// </summary>
	/// <param name="newState">The new state.</param>
	/// <returns>The state result.</returns>
	protected StateResult Transition(State? newState) => new(newState);

	/// <summary>
	/// Transitions the specified new state.
	/// </summary>
	/// <param name="output">The output.</param>
	/// <param name="newState">The new state.</param>
	/// <returns>The state result.</returns>
	protected StateResult Transition(T? output, State newState) => new(output, newState);

	/// <summary>
	/// Advances to the next state.
	/// </summary>
	/// <returns>The output.</returns>
	protected virtual T? Turn()
	{
		if (CurrentState is not null)
		{
			StateResult? result = null;
			do
			{
				if (CurrentState is null)
				{
					break;
				}

				result = CurrentState();
				if (result is null || result.Next is null)
				{
					break;
				}
				CurrentState = result.Next;
			}
			while (!result.HasOutput);

			if (result is null)
			{
				return default;
			}

			return result.Output;
		}

		return default;
	}
}
