// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Renderer;

using Xunit;

public class ImplictBlockRendererFacts : ParserVisitorFactsBase
{
	[Fact]
	public void RendersImplicitBlockExpression()
	{
		string template = "{{#person}}{{forename}} {{surname}}{{/person}}";
		string expected = "Matthew Abbott";
		var model = new { person = new { forename = "Matthew", surname = "Abbott" } };

		RenderTest(template, expected, model);
	}

	[Fact]
	public void RendersImplicitEnumerableBlockExpression()
	{
		string template = "<ul>{{#people}}<li>{{forename}} {{surname}}</li>{{/people}}</ul>";
		string expected = "<ul><li>Matthew Abbott</li><li>Stuart Stubbs</li></ul>";
		var model = new
		{
			people = new[]
			{
				new {forename = "Matthew", surname = "Abbott"},
				new {forename = "Stuart", surname = "Stubbs"}
			}
		};

		RenderTest(template, expected, model);
	}

	[Fact]
	public void RendersInvertedImplicitBlockExpression()
	{
		string template = "{{^person}}You have no power here!{{/person}}";
		string expected = "You have no power here!";
		var model = new { person = (object?)null };

		RenderTest(template, expected, model);
	}
}
