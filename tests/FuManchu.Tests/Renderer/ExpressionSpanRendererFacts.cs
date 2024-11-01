// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Renderer;

using Xunit;

public class ExpressionSpanRendererFacts : ParserVisitorFactsBase
{
	[Fact]
	public void EncodesHtmlContent()
	{
		string template = "{{this}}";
		string expected = "&lt;h1&gt;Hello World&lt;/h1&gt;";

		RenderTest(template, expected, "<h1>Hello World</h1>");
	}

	[Fact]
	public void RendersHtmlContentUsingTripleBraces()
	{
		string template = "{{{this}}}";
		string expected = "<h1>Hello World</h1>";

		RenderTest(template, expected, "<h1>Hello World</h1>");
	}

	[Fact]
	public void RendersHtmlContentUsingAmpersand()
	{
		string template = "{{&this}}";
		string expected = "<h1>Hello World</h1>";

		RenderTest(template, expected, "<h1>Hello World</h1>");
	}
}
