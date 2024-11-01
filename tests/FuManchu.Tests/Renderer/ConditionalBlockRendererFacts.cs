// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Renderer;

using Xunit;

public class ConditionalBlockRendererFacts : ParserVisitorFactsBase
{
	[Fact]
	public void CanRenderIfBlock()
	{
		string template = "{{#if this}}True!{{/if}}";
		string expected = "True!";

		RenderTest(template, expected, true);
	}

	[Fact]
	public void CanRenderElseBlock()
	{
		string template = "{{#if this}}True!{{else}}False!{{/if}}";
		string expected = "False!";

		RenderTest(template, expected, false);
	}

	[Fact]
	public void CanRenderElseBlockUsingNegation()
	{
		string template = "{{#if this}}True!{{^}}False!{{/if}}";
		string expected = "False!";

		RenderTest(template, expected, false);
	}

	[Fact]
	public void CanRenderElseIfBlock()
	{
		string template = "{{#if value1}}Value 1{{#elseif value2}}Value 2{{/if}}";
		string expected = "Value 2";

		RenderTest(template, expected, new { value1 = false, value2 = true });
	}

	[Fact]
	public void CanRenderComplexNestedIfBlock()
	{
		string template = "<h1>{{#if page.title}}{{page.title}}{{else}}{{#if page.url}}{{page.url}}{{/if}}{{/if}}</h1>";
		string expected = "<h1>http://localhost</h1>";

		RenderTest(template, expected, new { page = new { title = "", url = "http://localhost" } });
	}
}
