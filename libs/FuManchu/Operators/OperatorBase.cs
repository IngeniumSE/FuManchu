// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System;

/// <summary>
/// Provides a base implementation of an operator.
/// </summary>
public abstract class OperatorBase : IOperator
{
	private readonly string _name;

	/// <summary>
	/// Initialises a new instance of <see cref="OperatorBase"/>
	/// </summary>
	/// <param name="name">The operator name.</param>
	protected OperatorBase(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("An operator name must be provided.");
		}

		_name = name;
	}

	/// <inheritdoc />
	public virtual string Name
	{
		get { return _name; }
	}

	/// <inheritdoc />
	public abstract bool Result(object? x, object? y);
}
