// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Operators;

using Xunit;

/// <summary>
/// Provides tests for the <see cref="EqualityOperator"/>
/// </summary>
public class EqualityOperatorFacts : OperatorFactsBase
{
	public EqualityOperatorFacts() : base(new EqualityOperator()) { }

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
	public void PassingDifferentTypes_WillUseTypeConversion()
	{
		RunTest(true, "true", true);
	}

	[Fact]
	public void PassingDifferentTypes_WithNonConvertableValues_ShouldBeFalse()
	{
		RunTest(true, "hello", false);
	}
}
