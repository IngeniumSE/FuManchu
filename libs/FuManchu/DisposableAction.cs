// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System;

/// <summary>
/// Represents a disposable action.
/// </summary>
public class DisposableAction : IDisposable
{
	private readonly Action _action;
	private bool _disposed;

	/// <summary>
	/// Initializes a new instance of the <see cref="DisposableAction"/> class.
	/// </summary>
	/// <param name="action">The action.</param>
	/// <exception cref="System.ArgumentNullException">action</exception>
	public DisposableAction(Action action)
	{
		if (action == null)
		{
			throw new ArgumentNullException("action");
		}

		_action = action;
	}

	/// <inheritdoc />
	public void Dispose()
	{
		if (!_disposed)
		{
			_action();
			_disposed = true;
		}
	}
}
