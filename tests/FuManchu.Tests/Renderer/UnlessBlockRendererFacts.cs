// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Renderer
{
	using Xunit;

	public class UnlessBlockRendererFacts : ParserVisitorFactsBase
	{
		[Fact]
		public void CanRenderUnlessTagContent()
		{
			string template = "{{#unless license}}<h3 class=\"warning\">WARNING: This entry does not have a license!</h3>{{/unless}}";
			string expected = "<h3 class=\"warning\">WARNING: This entry does not have a license!</h3>";

			var model = new { license = false };
			RenderTest(template, expected, model);
		}

		[Fact]
		public void CanSkipRenderingUnlessTagContent()
		{
			string template = "{{#unless license}}<h3 class=\"warning\">WARNING: This entry does not have a license!</h3>{{/unless}}";
			string expected = "";

			var model = new { license = true };
			RenderTest(template, expected, model);
		}

		[Fact]
		public void CanRenderElseTagContent()
		{
			string template = "{{#unless license}}<h3 class=\"warning\">WARNING: This entry does not have a license!</h3>{{else}}<h3>You have a license</h3>{{/unless}}";
			string expected = "<h3>You have a license</h3>";

			var model = new { license = true };
			RenderTest(template, expected, model);
		}

		[Fact]
		public void CanRenderElseTagContentUsingNegation()
		{
			string template = "{{#unless license}}<h3 class=\"warning\">WARNING: This entry does not have a license!</h3>{{^}}<h3>You have a license</h3>{{/unless}}";
			string expected = "<h3>You have a license</h3>";

			var model = new { license = true };
			RenderTest(template, expected, model);
		}
	}
}
