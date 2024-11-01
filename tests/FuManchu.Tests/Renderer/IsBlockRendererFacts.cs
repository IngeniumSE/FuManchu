// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Renderer;

using Xunit;

public class IsBlockRendererFacts : ParserVisitorFactsBase
{
	[Fact]
	public void CanParseIsTag_WithSingleArgument()
	{
		var model = new
		{
			x = true
		};

		RenderTest("{{#is x}}true{{/is}}", "true", model);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments()
	{
		var model = new
		{
			x = true,
			y = true
		};

		RenderTest("{{#is x y}}true{{/is}}", "true", model);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments_WithEqualityOperator()
	{
		var model = new
		{
			x = true,
			y = "true"
		};

		RenderTest("{{#is x \"==\" y}}true{{/is}}", "true", model);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments_WithNegatedEqualityOperator()
	{
		var model = new
		{
			x = true,
			y = "false"
		};

		RenderTest("{{#is x \"!=\" y}}true{{/is}}", "true", model);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments_WithNotOperator()
	{
		var model = new
		{
			x = true,
			y = "false"
		};

		RenderTest("{{#is x \"not\" y}}true{{/is}}", "true", model);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments_WithStrictEqualityOperator()
	{
		var model = new
		{
			x = true,
			y = "true"
		};

		RenderTest("{{#is x \"===\" y}}true{{else}}false{{/is}}", "false", model);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments_WithStrictNegatedEqualityOperator()
	{
		var model = new
		{
			x = true,
			y = "true"
		};

		RenderTest("{{#is x \"!==\" y}}true{{else}}false{{/is}}", "true", model);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments_WithGreaterThanOperator()
	{
		var model = new
		{
			x = 1,
			y = 0
		};

		RenderTest("{{#is x \">\" y}}true{{/is}}", "true", model);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments_WithGreaterThanEqualToOperator()
	{
		var model = new
		{
			x = 1,
			y = 1
		};

		RenderTest("{{#is x \">=\" y}}true{{/is}}", "true", model);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments_WithLessThanOperator()
	{
		var model = new
		{
			x = 0,
			y = 1
		};

		RenderTest("{{#is x \"<\" y}}true{{/is}}", "true", model);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments_WithLessThanEqualToOperator()
	{
		var model = new
		{
			x = 0,
			y = 1
		};

		RenderTest("{{#is x \"<=\" y}}true{{/is}}", "true", model);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments_WithInOperator()
	{
		var model = new
		{
			x = "one"
		};

		RenderTest("{{#is x \"in\" \"one,two,three\"}}true{{/is}}", "true", model);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments_WithInOperator_UsingEnumerableParameter()
	{
		var model = new
		{
			x = "one",
			y = new[] { "one", "two", "three" }
		};

		RenderTest("{{#is x \"in\" y}}true{{/is}}", "true", model);
	}
}
