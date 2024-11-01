// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System.Collections.Generic;

/// <summary>
/// Represents the standard set of operators.
/// </summary>
public class StandardOperatorProvider : IOperatorProvider
{
	/// <inheritdoc />
	public IEnumerable<IOperator> GetOperators()
	{
		/* == */
		yield return new EqualityOperator();
		/* === */
		yield return new EqualityOperator(true);
		/* != */
		yield return new NegatedEqualityOperator();
		/* not */
		yield return new NegatedEqualityOperator("not");
		/* !== */
		yield return new NegatedEqualityOperator(true);
		/* >= */
		yield return new NumericOperator(NumericOperation.GreaterThanEqualTo);
		/* > */
		yield return new NumericOperator(NumericOperation.GreaterThan);
		/* <= */
		yield return new NumericOperator(NumericOperation.LessThanEqualTo);
		/* < */
		yield return new NumericOperator(NumericOperation.LessThan);
		/* in */
		yield return new InOperator();
	}
}
