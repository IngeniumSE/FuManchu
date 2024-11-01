// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Operators;

using Xunit;

/// <summary>
/// Provides tests for the <see cref="EqualityOperator"/> in strict mode.
/// </summary>
public class StrictEqualityOperatorFacts : OperatorFactsBase
{
	public StrictEqualityOperatorFacts() : base(new EqualityOperator(true)) { }

	[Fact]
	public void PassingTwoNullValues_ShouldBeTrue()
	{
		RunTest(null, null, true);
	}

	[Fact]
	public void PassingOneNullValue_ShouldBeFalse()
	{
		RunTest(true, null, false);
	}

	[Fact]
	public void PassingSameValues_ShouldBeTrue()
	{
		RunTest(true, true, true);
	}

	[Fact]
	public void PassingDifferentValues_ShouldBeFalse()
	{
		RunTest(true, false, false);
	}

	[Fact]
	public void PassingDifferentTypes_WillNotUseTypeConversion()
	{
		RunTest(true, "true", false);
	}
}
