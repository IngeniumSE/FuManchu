// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Renders a partial include.
/// </summary>
public class PartialBlockRenderer : BlockRenderer
{
	/// <inheritdoc />
	protected override void Render(Block block, Arguments? arguments, Map? maps, RenderContext context, TextWriter writer)
	{
		if (context.Service == null)
		{
			// No service, can't do anything.
			return;
		}

		var span = block.Children.FirstOrDefault(c => !c.IsBlock && ((Span)c).Kind == SpanKind.Expression) as Span;
		if (span == null)
		{
			// Malformed tag?
			return;
		}

		string name = span.Content!;
		object? model = arguments!.FirstOrDefault();

		if (model != null)
		{
			using (var scope = context.BeginScope(model))
			{
				context.Service.RunPartial(name, scope.ScopeContext, writer);
			}
		}
		else if (maps is { Count: > 0 })
		{
			using (var scope = context.BeginScope(maps))
			{
				context.Service.RunPartial(name, scope.ScopeContext, writer);
			}
		}
		else
		{
			context.Service.RunPartial(name, context, writer);
		}
	}
}
