// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Operators;

using Xunit;

/// <summary>
/// Provides tests for the <see cref="NegatedEqualityOperator"/>, in strict mode.
/// </summary>
public class StrictNegatedEqualityOperatorFacts : OperatorFactsBase
{
	public StrictNegatedEqualityOperatorFacts() : base(new NegatedEqualityOperator(true)) { }

	[Fact]
	public void PassingTwoNullValues_ShouldBeFalse()
	{
		RunTest(null, null, false);
	}

	[Fact]
	public void PassingOneNullValue_ShouldBeTrue()
	{
		RunTest(true, null, true);
	}

	[Fact]
	public void PassingSameValues_ShouldBeFalse()
	{
		RunTest(true, true, false);
	}

	[Fact]
	public void PassingDifferentValues_ShouldBeTrue()
	{
		RunTest(true, false, true);
	}

	[Fact]
	public void PassingDifferentTypes_WillNotUseTypeConversion()
	{
		RunTest(true, "true", true);
	}
}
