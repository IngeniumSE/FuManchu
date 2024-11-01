// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Issues;

using Xunit;

public class Issue15
{
	/// <summary>
	/// NRF executing basic IS template
	/// https://github.com/Antaris/FuManchu/issues/15
	/// </summary>
	[Fact]
	public void NRF_executing_basic_IS_template()
	{
		// Arrange
		string template = "{{#is Value \"!=\" 1}}True!{{/is}}";

		// Act
		string content = Handlebars.CompileAndRun("test", template, new { Value = 2 });

		// Assert
		Assert.NotNull(content);
		Assert.Equal("True!", content);
	}
}
