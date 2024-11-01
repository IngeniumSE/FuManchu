// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Operators;

using Xunit;

/// <summary>
/// Provides tests for the <see cref="NumericOperator"/>
/// </summary>
public class NumericOperatorFacts
{
	[Fact]
	public void GreaterThanOperation_ShouldBeTrue_ForTwoValueTypes()
	{
		var op = new NumericOperator(NumericOperation.GreaterThan);

		RunTest(op, 2, 1, true);
		RunTest(op, 2.0m, 1, true);
		RunTest(op, 2.0f, 1, true);
		RunTest(op, 2L, 1, true);
		RunTest(op, 2ul, 1, true);
		RunTest(op, 2u, 1, true);
	}

	[Fact]
	public void GreaterThanOperation_ShouldBeFalse_ForTwoValueTypes_WithGreaterOrEqualSecondOperand()
	{
		var op = new NumericOperator(NumericOperation.GreaterThan);

		RunTest(op, 2, 2, false);
		RunTest(op, 2.0m, 2, false);
		RunTest(op, 2.0f, 2, false);
		RunTest(op, 2L, 2, false);
		RunTest(op, 2ul, 2, false);
		RunTest(op, 2u, 2, false);
	}

	[Fact]
	public void GreaterThanOperation_ShouldBeTrue_ForFirstValueType_AndTypeConversion()
	{
		var op = new NumericOperator(NumericOperation.GreaterThan);

		RunTest(op, 2, "1", true);
		RunTest(op, 2.0m, "1", true);
		RunTest(op, 2.0f, "1", true);
		RunTest(op, 2L, "1", true);
		RunTest(op, 2ul, "1", true);
		RunTest(op, 2u, "1", true);
	}

	[Fact]
	public void GreaterThanOperation_ShouldBeFalse_ForFirstValueType_AndInvalidTypeConversion()
	{
		var op = new NumericOperator(NumericOperation.GreaterThan);

		RunTest(op, 2, "hello", false);
		RunTest(op, 2.0m, "hello", false);
		RunTest(op, 2.0f, "hello", false);
		RunTest(op, 2L, "hello", false);
		RunTest(op, 2ul, "hello", false);
		RunTest(op, 2u, "hello", false);
	}

	[Fact]
	public void GreaterThanOperation_ShouldBeFalse_ForNulls()
	{
		var op = new NumericOperator(NumericOperation.GreaterThan);

		RunTest(op, null, null, false);
		RunTest(op, 1, null, false);
		RunTest(op, null, 1, false);
	}

	[Fact]
	public void GreaterThanOperation_ShouldBeTrue_ForComparableTypes_WithGreaterFirstOperand()
	{
		var op = new NumericOperator(NumericOperation.GreaterThan);

		RunTest(op, "b", "a", true);
	}

	[Fact]
	public void GreaterThanOperation_ShouldBeFalse_ForNonComparableTypes()
	{
		var op = new NumericOperator(NumericOperation.GreaterThan);

		RunTest(op, "hello", new { one = 1 }, false);
	}

	[Fact]
	public void GreaterThanEqualToOperation_ShouldBeTrue_ForTwoValueTypes()
	{
		var op = new NumericOperator(NumericOperation.GreaterThanEqualTo);

		RunTest(op, 2, 2, true);
		RunTest(op, 2.0m, 2, true);
		RunTest(op, 2.0f, 2, true);
		RunTest(op, 2L, 2, true);
		RunTest(op, 2ul, 2, true);
		RunTest(op, 2u, 2, true);
	}

	[Fact]
	public void GreaterThanEqualToOperation_ShouldBeFalse_ForTwoValueTypes_WithGreaterSecondOperand()
	{
		var op = new NumericOperator(NumericOperation.GreaterThanEqualTo);

		RunTest(op, 2, 3, false);
		RunTest(op, 2.0m, 3, false);
		RunTest(op, 2.0f, 3, false);
		RunTest(op, 2L, 3, false);
		RunTest(op, 2ul, 3, false);
		RunTest(op, 2u, 3, false);
	}

	[Fact]
	public void GreaterThanEqualToOperation_ShouldBeTrue_ForFirstValueType_AndTypeConversion()
	{
		var op = new NumericOperator(NumericOperation.GreaterThanEqualTo);

		RunTest(op, 2, "2", true);
		RunTest(op, 2.0m, "2", true);
		RunTest(op, 2.0f, "2", true);
		RunTest(op, 2L, "2", true);
		RunTest(op, 2ul, "2", true);
		RunTest(op, 2u, "2", true);
	}

	[Fact]
	public void GreaterThanEqualToOperation_ShouldBeFalse_ForFirstValueType_AndInvalidTypeConversion()
	{
		var op = new NumericOperator(NumericOperation.GreaterThanEqualTo);

		RunTest(op, 2, "hello", false);
		RunTest(op, 2.0m, "hello", false);
		RunTest(op, 2.0f, "hello", false);
		RunTest(op, 2L, "hello", false);
		RunTest(op, 2ul, "hello", false);
		RunTest(op, 2u, "hello", false);
	}

	[Fact]
	public void GreaterThanEqualToOperation_ShouldBeFalse_ForNulls()
	{
		var op = new NumericOperator(NumericOperation.GreaterThanEqualTo);

		RunTest(op, null, null, false);
		RunTest(op, 1, null, false);
		RunTest(op, null, 1, false);
	}

	[Fact]
	public void GreaterThanEqualToOperation_ShouldBeTrue_ForComparableTypes_WithGreaterFirstOperand()
	{
		var op = new NumericOperator(NumericOperation.GreaterThanEqualTo);

		RunTest(op, "b", "a", true);
	}

	[Fact]
	public void GreaterThanEqualToOperation_ShouldBeFalse_ForNonComparableTypes()
	{
		var op = new NumericOperator(NumericOperation.GreaterThanEqualTo);

		RunTest(op, "hello", new { one = 1 }, false);
	}

	[Fact]
	public void LessThanOperation_ShouldBeTrue_ForTwoValueTypes()
	{
		var op = new NumericOperator(NumericOperation.LessThan);

		RunTest(op, 1, 2, true);
		RunTest(op, 1.0m, 2, true);
		RunTest(op, 1.0f, 2, true);
		RunTest(op, 1L, 2, true);
		RunTest(op, 1ul, 2, true);
		RunTest(op, 1u, 2, true);
	}

	[Fact]
	public void LessThanOperation_ShouldBeFalse_ForTwoValueTypes_WithGreaterOrEqualSecondOperand()
	{
		var op = new NumericOperator(NumericOperation.LessThan);

		RunTest(op, 2, 2, false);
		RunTest(op, 2.0m, 2, false);
		RunTest(op, 2.0f, 2, false);
		RunTest(op, 2L, 2, false);
		RunTest(op, 2ul, 2, false);
		RunTest(op, 2u, 2, false);
	}

	[Fact]
	public void LessThanOperation_ShouldBeTrue_ForFirstValueType_AndTypeConversion()
	{
		var op = new NumericOperator(NumericOperation.LessThan);

		RunTest(op, 1, "2", true);
		RunTest(op, 1.0m, "2", true);
		RunTest(op, 1.0f, "2", true);
		RunTest(op, 1L, "2", true);
		RunTest(op, 1ul, "2", true);
		RunTest(op, 1u, "2", true);
	}

	[Fact]
	public void LessThanOperation_ShouldBeFalse_ForFirstValueType_AndInvalidTypeConversion()
	{
		var op = new NumericOperator(NumericOperation.LessThan);

		RunTest(op, 2, "hello", false);
		RunTest(op, 2.0m, "hello", false);
		RunTest(op, 2.0f, "hello", false);
		RunTest(op, 2L, "hello", false);
		RunTest(op, 2ul, "hello", false);
		RunTest(op, 2u, "hello", false);
	}

	[Fact]
	public void LessThanOperation_ShouldBeFalse_ForNulls()
	{
		var op = new NumericOperator(NumericOperation.LessThan);

		RunTest(op, null, null, false);
		RunTest(op, 1, null, false);
		RunTest(op, null, 1, false);
	}

	[Fact]
	public void LessThanOperation_ShouldBeTrue_ForComparableTypes_WithGreaterSecondOperand()
	{
		var op = new NumericOperator(NumericOperation.LessThan);

		RunTest(op, "a", "b", true);
	}

	[Fact]
	public void LessThanOperation_ShouldBeFalse_ForNonComparableTypes()
	{
		var op = new NumericOperator(NumericOperation.LessThan);

		RunTest(op, "hello", new { one = 1 }, false);
	}

	[Fact]
	public void LessThanEqualToOperation_ShouldBeTrue_ForTwoValueTypes()
	{
		var op = new NumericOperator(NumericOperation.LessThanEqualTo);

		RunTest(op, 2, 2, true);
		RunTest(op, 2.0m, 2, true);
		RunTest(op, 2.0f, 2, true);
		RunTest(op, 2L, 2, true);
		RunTest(op, 2ul, 2, true);
		RunTest(op, 2u, 2, true);
	}

	[Fact]
	public void LessThanEqualToOperation_ShouldBeFalse_ForTwoValueTypes_WithGreaterFirstOperand()
	{
		var op = new NumericOperator(NumericOperation.LessThanEqualTo);

		RunTest(op, 3, 2, false);
		RunTest(op, 3.0m, 2, false);
		RunTest(op, 3.0f, 2, false);
		RunTest(op, 3L, 2, false);
		RunTest(op, 3ul, 2, false);
		RunTest(op, 3u, 2, false);
	}

	[Fact]
	public void LessThanEqualToOperation_ShouldBeTrue_ForFirstValueType_AndTypeConversion()
	{
		var op = new NumericOperator(NumericOperation.LessThanEqualTo);

		RunTest(op, 2, "2", true);
		RunTest(op, 2.0m, "2", true);
		RunTest(op, 2.0f, "2", true);
		RunTest(op, 2L, "2", true);
		RunTest(op, 2ul, "2", true);
		RunTest(op, 2u, "2", true);
	}

	[Fact]
	public void LessThanEqualToOperation_ShouldBeFalse_ForFirstValueType_AndInvalidTypeConversion()
	{
		var op = new NumericOperator(NumericOperation.LessThanEqualTo);

		RunTest(op, 2, "hello", false);
		RunTest(op, 2.0m, "hello", false);
		RunTest(op, 2.0f, "hello", false);
		RunTest(op, 2L, "hello", false);
		RunTest(op, 2ul, "hello", false);
		RunTest(op, 2u, "hello", false);
	}

	[Fact]
	public void LessThanEqualToOperation_ShouldBeFalse_ForNulls()
	{
		var op = new NumericOperator(NumericOperation.LessThanEqualTo);

		RunTest(op, null, null, false);
		RunTest(op, 1, null, false);
		RunTest(op, null, 1, false);
	}

	[Fact]
	public void LessThanEqualToOperation_ShouldBeTrue_ForComparableTypes_WithGreaterFirstOperand()
	{
		var op = new NumericOperator(NumericOperation.LessThanEqualTo);

		RunTest(op, "a", "b", true);
	}

	[Fact]
	public void LessThanEqualToOperation_ShouldBeFalse_ForNonComparableTypes()
	{
		var op = new NumericOperator(NumericOperation.LessThanEqualTo);

		RunTest(op, "hello", new { one = 1 }, false);
	}

	protected void RunTest(IOperator @operator, object? x, object? y, bool expected)
	{
		bool result = @operator.Result(x, y);

		Assert.Equal(expected, result);
	}
}
