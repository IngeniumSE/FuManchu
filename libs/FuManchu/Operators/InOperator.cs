// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System;
using System.Collections;
using System.Linq;

/// <summary>
/// Provides operations for matching values within sets.
/// </summary>
public class InOperator : OperatorBase
{
	/// <summary>
	/// Intialises a new instance of <see cref="InOperator"/>
	/// </summary>
	public InOperator() : base("in") { }

	/// <inheritdoc />
	public override bool Result(object? x, object? y)
	{
		if (x == null || y == null)
		{
			return false;
		}

		string? yString = y as string;
		if (yString != null)
		{
			string[] values = yString.Trim().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
			return values.Any(v => string.Equals(x.ToString(), v, StringComparison.Ordinal));
		}

		var enumerable = y as IEnumerable;
		if (enumerable != null)
		{
			return enumerable.Cast<object>().Any(v => object.Equals(x, v));
		}

		return false;
	}
}
