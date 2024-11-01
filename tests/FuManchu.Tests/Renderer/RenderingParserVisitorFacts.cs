// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Renderer;

using Xunit;

public class RenderingParserVisitorFacts : ParserVisitorFactsBase
{
	[Fact]
	public void CanRenderSimpleExpression()
	{
		RenderTest("Hello {{world}}", "Hello World!", new { world = "World!" });
	}


	[Fact]
	public void CanRenderComplexExpression()
	{
		RenderTest("Hello {{person.forename}} {{person.surname}}!", "Hello Matthew Abbott!", new { person = new { forename = "Matthew", surname = "Abbott" } });
		RenderTest("Hello {{person/forename}} {{person/surname}}!", "Hello Matthew Abbott!", new { person = new { forename = "Matthew", surname = "Abbott" } });
	}

	[Fact]
	public void CanRenderExpressionFromCurrentContext()
	{
		RenderTest("Hello {{./world}}", "Hello World!", new { world = "World!" });
	}

	[Fact]
	public void CanRenderExpressionFromCurrentContextWithThis()
	{
		RenderTest("Hello {{this/world}}", "Hello World!", new { world = "World!" });
		RenderTest("Hello {{this.world}}", "Hello World!", new { world = "World!" });
	}

	[Fact]
	public void CanRenderExpressionFromParentContext()
	{
		string template = "Hello {{#with type}}{{../forename}} {{../surname}} is a {{description}}!{{/with}}";
		string expected = "Hello Matthew Abbott is a Hero!";
		var model = new { forename = "Matthew", surname = "Abbott", type = new { description = "Hero" } };

		RenderTest(template, expected, model);
	}

	[Fact]
	public void IgnoresTemplateComments()
	{
		RenderTest("Hello {{!-- Here is a comment --}}", "Hello ", null);
		RenderTest("Hello {{! Here is a comment }}", "Hello ", null);
		RenderTest("Hello {{~! Here is a comment }}", "Hello", null);
	}

	[Fact]
	public void CanCollapseWhiteSpace()
	{
		RenderTest("Hello {{~world~}} !", "HelloWorld!", new { world = "World" });
	}
}
