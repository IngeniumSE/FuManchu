// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

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

		string? name = GetPartialName(block);
		if (name == null)
		{
			// Malformed tag.
			return;
		}
		object? model = arguments!.FirstOrDefault();

		if (block.IsPartialBlock)
		{
			// We need to extact the PartialBlockContent tags from this block and store them in them against the current context.
			context.PartialBlockContent = block.Children
				.Where(c => c is Block b && b.Type == BlockType.PartialBlockContent)
				.Cast<Block>()
				.ToDictionary(c => c.Name!, c => c, StringComparer.InvariantCultureIgnoreCase);

			if (context.PartialBlockContent is not { Count: >0 })
			{
				// This tag has implicit content, so capture all children as the implicit zone 'content'.
				var zone = new BlockBuilder();
				zone.Name = "content";
				zone.IsPartialBlock = true;
				zone.IsPartialBlockContent = true;
				zone.IsImplicitPartialBlockContent = true;
				zone.Children.AddRange(
					block.Children.Skip(1).Take(block.Children.Count - 2));

				context.PartialBlockContent.Add(
					"content",
					zone.Build());
			}
		}

		if (string.Equals("@partial-block", name, StringComparison.InvariantCulture))
		{
			// This is actually the compatability tag added to enable the JS syntax.
			var zone = new BlockBuilder();
			zone.Name = "content";

			new ZoneBlockRenderer().Render(zone.Build(), context, writer);

			return;
		}

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

	string? GetPartialName(Block block)
	{
		if (block.IsPartialBlock)
		{
			var bl = block.Children.FirstOrDefault() as Block;
			return bl?.Name;
		}
		else
		{
			var span = block.Children.FirstOrDefault(c => !c.IsBlock && ((Span)c).Kind == SpanKind.Expression) as Span;
			if (span == null)
			{
				// Malformed tag?
				return null;
			}

			return span.Content!;
		}
	}
}
