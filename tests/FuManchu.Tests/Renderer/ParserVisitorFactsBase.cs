// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Renderer;

using System;

using FuManchu.Tags;

using Xunit;

public abstract class ParserVisitorFactsBase
{
	private readonly Lazy<IHandlebarsService> _handlebarsService;

	protected ParserVisitorFactsBase()
	{
		_handlebarsService = new Lazy<IHandlebarsService>(CreateHandlebarsService);
	}

	protected IHandlebarsService HandlebarsService { get { return _handlebarsService.Value; } }

	protected virtual IHandlebarsService CreateHandlebarsService()
	{
		return new HandlebarsService();
	}

	protected void RenderTest(string content, string expected, object? model = null, TagProvidersCollection? providers = null)
	{
		var func = HandlebarsService.Compile(Guid.NewGuid().ToString(), content);
		string result = func(model);

		Assert.Equal(expected, result);
	}
}
