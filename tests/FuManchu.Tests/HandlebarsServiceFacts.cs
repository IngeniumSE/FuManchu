// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests;

using System.Collections;
using System.Linq;

using Xunit;

public class HandlebarsServiceFacts
{
	[Fact]
	public void CanCreateReusableTemplateDelegate()
	{
		var service = new HandlebarsService();
		var func = service.Compile("Hello {{name}}");

		Assert.Equal("Hello Matt", func(new { name = "Matt" }));
		Assert.Equal("Hello Stuart", func(new { name = "Stuart" }));
	}

	[Fact]
	public void CanCompileAndRunInOneCall()
	{
		var service = new HandlebarsService();

		Assert.Equal("Hello Matt", service.CompileAndRun("my-template", "Hello {{name}}", new { name = "Matt" }));
	}

	[Fact]
	public void CanCompileAndRunLater()
	{
		var service = new HandlebarsService();
		service.Compile("template-name", "Hello {{name}}");

		Assert.Equal("Hello Matt", service.Run("template-name", new { name = "Matt" }));
	}

	[Fact]
	public void CanRunBlockHelper()
	{
		var service = new HandlebarsService();
		service.RegisterHelper("list", options =>
		{
			var enumerable = options.Data as IEnumerable ?? new[] { options.Data };
			return "<ul>" + string.Join("", enumerable.OfType<object>().Select(options.Fn!)) + "</ul>";
		});

		string template = "{{#list this}}<li>{{name}}</li>{{/list}}";
		var model = new[]
								{
									new { name = "Matt" },
						new { name = "Stuart" }
								};

		string result = service.CompileAndRun("test", template, model);

		Assert.Equal("<ul><li>Matt</li><li>Stuart</li></ul>", result);
	}

	[Fact]
	public void CanAccessHashParametersInHelper()
	{
		var service = new HandlebarsService();
		service.RegisterHelper("formatted_name", options =>
																						 {
																							 return "<h1 class=\"" + options.Hash!["class"] + "\">" + options.Fn(options.Data) + "</h1>";
																						 });

		string template = "{{#formatted_name class=\"test\"}}<strong>{{name}}</strong>{{/formatted_name}}";
		var model = new { name = "Matt" };

		string result = service.CompileAndRun("test", template, model);

		Assert.Equal("<h1 class=\"test\"><strong>Matt</strong></h1>", result);
	}

	[Fact]
	public void CanRunExpressionHelper()
	{
		var service = new HandlebarsService();
		service.RegisterHelper("fullname", options => string.Format("{0} {1}", options.Data!.forename, options.Data.surname));

		string template = "Your full name is: {{fullname person}}";
		string expected = "Your full name is: Matthew Abbott";

		var model = new
		{
			person = new { forename = "Matthew", surname = "Abbott" }
		};

		Assert.Equal(expected, service.CompileAndRun("test", template, model));
	}

	[Fact]
	public void CanRunUnknownValueResolver()
	{
		var service = new HandlebarsService();

		UnknownValueResolver resolver = (e) => (e == "name" ? "Matthew Abbott" : null);

		string template = "Your full name is: {{name}}";
		string expected = "Your full name is: Matthew Abbott";

		var model = new
		{

		};

		Assert.Equal(expected, service.CompileAndRun("test", template, model, resolver));
	}
}
