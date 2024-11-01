// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Operators;

using Xunit;

/// <summary>
/// Provides a base implementation of a test harness for operators.
/// </summary>
public abstract class OperatorFactsBase
{
	private readonly IOperator _operator;

	protected OperatorFactsBase(IOperator @operator)
	{
		_operator = @operator;
	}

	public IOperator Operator { get { return _operator; } }

	protected void RunTest(object? x, object? y, bool expected)
	{
		bool result = Operator.Result(x, y);

		Assert.Equal(expected, result);
	}
}
