// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Text;

using System;

/// <summary>
/// Provides a token for managing a lookahead action.
/// </summary>
public class LookaheadToken : IDisposable
{
	private readonly Action _action;
	private bool _accepted;

	/// <summary>
	/// Initializes a new instance of the <see cref="LookaheadToken"/> class.
	/// </summary>
	/// <param name="action">The action.</param>
	public LookaheadToken(Action action)
	{
		_action = action;
	}

	/// <summary>
	/// Accepts this lookahead to prevent the rollback.
	/// </summary>
	public void Accept()
	{
		_accepted = true;
	}

	/// <inheritdoc />>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Releases unmanaged and - optionally - managed resources.
	/// </summary>
	/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
	protected virtual void Dispose(bool disposing)
	{
		if (!_accepted)
		{
			_action();
		}
	}
}
