// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Renderer;

using System;
using System.Collections.Generic;

using Xunit;

public class EnumerableBlockRendererFacts : ParserVisitorFactsBase
{
	[Fact]
	public void CanRenderEnumerableItems()
	{
		var model = new
		{
			items = new[]
							{
								new {name = "Matt"},
								new {name = "Stuart"}
							}
		};

		RenderTest("<ul>{{#each items}}<li>{{name}}</li>{{/each}}</ul>", "<ul><li>Matt</li><li>Stuart</li></ul>", model);
	}

	[Fact]
	public void CanRenderEnumerableIndexes()
	{
		var model = new
		{
			items = new[]
							{
								new {name = "Matt"},
								new {name = "Stuart"}
							}
		};

		RenderTest("<ul>{{#each items}}<li>{{@index}}: {{name}}</li>{{/each}}</ul>", "<ul><li>0: Matt</li><li>1: Stuart</li></ul>", model);
	}

	[Fact]
	public void CanRenderDictionaryItems()
	{
		var model = new Dictionary<string, string>
								{
									{ "first", "Matt" },
						{ "second", "Stuart" }
								};

		RenderTest("<ul>{{#each this}}<li>{{this}}</li>{{/each}}</ul>", "<ul><li>Matt</li><li>Stuart</li></ul>", model);
	}

	[Fact]
	public void CanRenderDictionaryKeys()
	{
		var model = new Dictionary<string, string>
								{
									{ "first", "Matt" },
						{ "second", "Stuart" }
								};

		RenderTest("<ul>{{#each this}}<li>{{@key}}: {{this}}</li>{{/each}}</ul>", "<ul><li>first: Matt</li><li>second: Stuart</li></ul>", model);
	}

	[Fact]
	public void CanRenderAlternativeWhenNoItems()
	{
		var model = new
		{
			items = new Object[0]
		};

		RenderTest("<ul>{{#each items}}<li>{{name}}</li>{{else}}<li>No items</li>{{/each}}</ul>", "<ul><li>No items</li></ul>", model);
	}

	[Fact]
	public void CanRenderAlternativeWhenNoItemsUsingNegation()
	{
		var model = new
		{
			items = new Object[0]
		};

		RenderTest("<ul>{{#each items}}<li>{{name}}</li>{{^}}<li>No items</li>{{/each}}</ul>", "<ul><li>No items</li></ul>", model);
	}
}
