// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Operators;

using System.Collections.Generic;

using Xunit;

/// <summary>
/// Provides tests for the <see cref="InOperator"/>
/// </summary>
public class InOperatorFacts : OperatorFactsBase
{
	public InOperatorFacts() : base(new InOperator()) { }

	[Fact]
	public void ForNulls_ShouldBeFalse()
	{
		RunTest(null, null, false);
		RunTest("hello", null, false);
		RunTest(null, "hello", false);
	}

	[Fact]
	public void ForSimpleString_ShouldBeTrue()
	{
		RunTest("hello", "hello", true);
	}

	[Fact]
	public void ForCommaDelimitedString_ShouldBeTrue()
	{
		RunTest("hello", "hello,world", true);
	}

	[Fact]
	public void ForCommaDelimietedString_WithSpacing_ShouldBeTrue()
	{
		RunTest("hello", " hello, world", true);
	}

	[Fact]
	public void ForListOfStrings_ShouldBeTrue()
	{
		RunTest("hello", new List<string> { "hello", "world" }, true);
	}

	[Fact]
	public void ForMixedSetOfObjects_ShouldBeTrue()
	{
		RunTest("hello", new object[] { 1, "two", 3.0f, "hello" }, true);
	}

	[Fact]
	public void ForNonEnumerables_ShouldBeFalse()
	{
		RunTest("hello", new object(), false);
	}
}
