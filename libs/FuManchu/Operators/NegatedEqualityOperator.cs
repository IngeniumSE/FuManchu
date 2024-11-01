// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

/// <summary>
/// Performs a negated equality operation.
/// </summary>
public class NegatedEqualityOperator : OperatorBase
{
	private readonly EqualityOperator _equalityOperator;

	/// <summary>
	/// Initialises a new instance of <see cref="NegatedEqualityOperator"/>
	/// </summary>
	/// <param name="strict">True if strict equality checking should be used, otherwise false.</param>
	public NegatedEqualityOperator(bool strict = false)
		: base(strict ? "!==" : "!=")
	{
		_equalityOperator = new EqualityOperator(strict);
	}

	/// <summary>
	/// Initialises a new instance of <see cref="NegatedEqualityOperator"/>
	/// </summary>
	/// <param name="name">The name of the operator.</param>
	internal NegatedEqualityOperator(string name)
		: base(name)
	{
		_equalityOperator = new EqualityOperator(false);
	}

	/// <inheritdoc />
	public override bool Result(object? x, object? y)
	{
		return !_equalityOperator.Result(x, y);
	}
}
