// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

/// <summary>
/// Performs an equality operation.
/// </summary>
public class EqualityOperator : OperatorBase
{
	private readonly bool _strict;

	/// <summary>
	/// Initialises a new instance of <see cref="EqualityOperator"/>
	/// </summary>
	/// <param name="strict">True if strict equality checking should be used, otherwise false.</param>
	public EqualityOperator(bool strict = false)
		: base(strict ? "===" : "==")
	{
		_strict = strict;
	}

	/// <inheritdoc />
	public override bool Result(object? x, object? y)
	{
		return _strict
			? TestStrictEquality(x, y)
			: TestEquality(x, y);
	}

	/// <summary>
	/// Tests equality of the given values, converting types where required.
	/// </summary>
	/// <param name="x">The left operand.</param>
	/// <param name="y">The right operand.</param>
	/// <returns>True if the values are equal, otherwise false.</returns>
	public bool TestEquality(object? x, object? y)
	{
		if (x == null && y == null)
		{
			return true;
		}

		if (x == null || y == null)
		{
			return false;
		}

		var xType = x.GetType();
		var yType = y.GetType();

		try
		{
			var converted = Convert.ChangeType(y, xType);

			return Equals(x, converted);
		}
		catch
		{
			return false;
		}
	}

	/// <summary>
	/// Tests equality of the given values, using strict type.
	/// </summary>
	/// <param name="x">The left operand.</param>
	/// <param name="y">The right operand.</param>
	/// <returns>True if the values are equal, otherwise false.</returns>
	public bool TestStrictEquality(object? x, object? y)
	{
		if (x == null && y == null)
		{
			return true;
		}

		if (x == null || y == null)
		{
			return false;
		}

		var xType = x.GetType();
		var yType = y.GetType();

		if (xType != yType)
		{
			return false;
		}

		return Equals(x, y);
	}
}
