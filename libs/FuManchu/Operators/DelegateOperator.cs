// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System;

/// <summary>
/// Represents a delegated operator.
/// </summary>
public class DelegateOperator : IOperator
{
	private readonly string _name;
	private readonly Func<object?, object?, bool> _operator;

	/// <summary>
	/// Initialises a new instance of <see cref="DelegateOperator"/>
	/// </summary>
	/// <param name="name">The operator name.</param>
	/// <param name="operator">The operator delegate.</param>
	public DelegateOperator(string name, Func<object?, object?, bool> @operator)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentException("The operator name must be provided.", "name");
		}

		if (@operator == null)
		{
			throw new ArgumentNullException("operator");
		}

		_name = name;
		_operator = @operator;
	}

	/// <inheritdoc />
	public string Name { get { return _name; } }

	/// <inheritdoc />
	public bool Result(object? x, object? y)
	{
		return _operator(x, y);
	}
}
