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
}
