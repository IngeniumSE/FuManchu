// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Renderer;

using Xunit;

public class ScopeBlockRendererFacts : ParserVisitorFactsBase
{
	[Fact]
	public void CanCreateScopeBlock()
	{
		string template = "{{#with person}}Name: {{forename}} {{surname}}{{/with}}";
		string expected = "Name: Matthew Abbott";
		var model = new { person = new { forename = "Matthew", surname = "Abbott" } };

		RenderTest(template, expected, model);
	}

	[Fact]
	public void CanCreateNestedScopeBlocks()
	{
		string template = "{{#with person}}Name: {{forename}} {{surname}}, {{#with job}}({{title}}){{/with}}{{/with}}";
		string expected = "Name: Matthew Abbott, (Developer)";
		var model = new { person = new { forename = "Matthew", surname = "Abbott", job = new { title = "Developer" } } };

		RenderTest(template, expected, model);
	}

	[Fact]
	public void CanCreateAlternativeContentUsingElseTag()
	{
		string template = "{{#with person}}Name: {{forename}} {{surname}}, {{#with job}}({{title}}){{else}}Unemployed :-({{/with}}{{/with}}";
		string expected = "Name: Matthew Abbott, Unemployed :-(";
		var model = new { person = new { forename = "Matthew", surname = "Abbott" } };

		RenderTest(template, expected, model);
	}

	[Fact]
	public void CanCreateAlternativeContentUsingElseTagUsingNegation()
	{
		string template = "{{#with person}}Name: {{forename}} {{surname}}, {{#with job}}({{title}}){{^}}Unemployed :-({{/with}}{{/with}}";
		string expected = "Name: Matthew Abbott, Unemployed :-(";
		var model = new { person = new { forename = "Matthew", surname = "Abbott" } };

		RenderTest(template, expected, model);
	}
}
