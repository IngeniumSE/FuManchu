// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Issues;

using Xunit;

public class Issue20
{
	/// <summary>
	/// NRF executing basic IS template
	/// https://github.com/Antaris/FuManchu/issues/15
	/// </summary>
	[Fact]
	public void BlockNodeContent_InPartial_ShouldNot_BeRenderedOutOfOrder_InParentTemplate()
	{
		// Arrange
		string partialTemplate = @"<outer>
	{{#if title}}{{title}}{{/if}}
</outer>";

		string template = @"{{>partial title=""Hello World""}}";
		Handlebars.RegisterPartial("partial", partialTemplate);

		// Act
		string content = Handlebars.CompileAndRun("test", template, new { });

		// Assert
		Assert.NotNull(content);
		Assert.Equal("<outer>\r\n\tHello World\r\n</outer>", content);
	}
}
