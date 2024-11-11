// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Renderer;

using Xunit;

public class PartialBlockRendererFacts : ParserVisitorFactsBase
{
	[Fact]
	public void CanRenderPartial()
	{
		HandlebarsService.RegisterPartial("body", "Hello World");

		string template = "{{>body}}";
		string expected = "Hello World";

		RenderTest(template, expected);
	}

	[Fact]
	public void CanRenderPartialWithCurrentModel()
	{
		HandlebarsService.RegisterPartial("body", "{{forename}} {{surname}}");

		string template = "{{>body}}";
		string expected = "Matthew Abbott";

		RenderTest(template, expected, new { forename = "Matthew", surname = "Abbott" });
	}

	[Fact]
	public void CanRenderPartialWithChildModel()
	{
		HandlebarsService.RegisterPartial("body", "{{forename}} {{surname}}");

		string template = "{{>body person}}";
		string expected = "Matthew Abbott";

		RenderTest(template, expected, new { person = new { forename = "Matthew", surname = "Abbott" } });
	}

	[Fact]
	public void CanRenderPartialWithArguments()
	{
		HandlebarsService.RegisterPartial("body", "{{firstName}} {{lastName}}");

		string template = "{{>body firstName=person.forename lastName=person.surname}}";
		string expected = "Matthew Abbott";

		RenderTest(template, expected, new { person = new { forename = "Matthew", surname = "Abbott" } });
	}

	[Fact]
	public void CanRenderPartialWithArguments_UsingNestedValue()
	{
		HandlebarsService.RegisterPartial("body", "{{person.forename}} {{person.surname}}");

		string template = "{{>body person=person}}";
		string expected = "Matthew Abbott";

		RenderTest(template, expected, new { person = new { forename = "Matthew", surname = "Abbott" } });
	}

	[Fact]
	public void CanRenderPartialBlock()
	{
		HandlebarsService.RegisterPartial("layout", "<h1>{{<content}}</h1>");

		string template = "{{#>layout}}{{>content}}Hello World{{/content}}{{/layout}}";
		string expected = "<h1>Hello World</h1>";

		RenderTest(template, expected);
	}

	[Fact]
	public void CanRenderPartialBlock_UsingCompatExpressionTag()
	{
		HandlebarsService.RegisterPartial("layout", "<h1>{{>@partial-block}}</h1>");

		string template = "{{#>layout}}{{>content}}Hello World{{/content}}{{/layout}}";
		string expected = "<h1>Hello World</h1>";

		RenderTest(template, expected);
	}

	[Fact]
	public void CanRenderPartialBlock_WithImplicitZone()
	{
		HandlebarsService.RegisterPartial("layout", "<h1>{{<content}}</h1>");

		string template = "{{#>layout}}Hello World{{/layout}}";
		string expected = "<h1>Hello World</h1>";

		RenderTest(template, expected);
	}

	[Fact]
	public void CanRenderPartialBlock_UsingContext_FromOuterTemplate()
	{
		HandlebarsService.RegisterPartial("layout", "<h1>{{<content}}</h1>");

		string template = "{{#>layout}}Hello {{name}}{{/layout}}";
		string expected = "<h1>Hello Matt</h1>";

		RenderTest(template, expected, model: new { name = "Matt" });
	}

	[Fact]
	public void CanRenderPartialBlock_WithManyZones()
	{
		HandlebarsService.RegisterPartial("layout", "<h1>{{<content}}</h1>\n<p>{{<lead}}</p>");

		string template = "{{#>layout}}{{>content}}Hello World{{/content}}{{>lead}}Welcome to the show{{/lead}}{{/layout}}";
		string expected = "<h1>Hello World</h1>\n<p>Welcome to the show</p>";

		RenderTest(template, expected);
	}

	[Fact]
	public void CanRenderPartialBlock_WithNestedPartial()
	{
		HandlebarsService.RegisterPartial("paragraph", "<p>{{text}}</p>");
		HandlebarsService.RegisterPartial("layout", "{{<content}}");

		string template = "{{#>layout}}{{>content}}{{>paragraph text=\"Hello World\"}}{{/content}}{{/layout}}";
		string expected = "<p>Hello World</p>";

		RenderTest(template, expected);
	}
}
