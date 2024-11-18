// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System.IO;
using System.Linq;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Renders a zone include.
/// </summary>
public class ZoneBlockRenderer : BlockRenderer
{
	protected override void Render(Block block, Arguments? arguments, Map? maps, RenderContext context, TextWriter writer)
	{
		if (context.Service == null || string.IsNullOrEmpty(block.Name))
		{
			// No service, or no zone name, can't do anything.
			return;
		}

		string name = block.Name;
		var (zone, targetContext) = TryGetZone(name, context);
		if (zone is null)
		{
			return;
		}

		var children = zone.IsImplicitPartialBlockContent
			? zone.Children
			: zone.Children.Skip(1).Take(zone.Children.Count - 2);

		RenderChildren(children, targetContext!);
	}

	(Block? zone, RenderContext? targetContext) TryGetZone(string name, RenderContext context)
	{
		if (context.PartialBlockContent is { Count: > 0 } && context.PartialBlockContent.TryGetValue(name, out var zone))
		{
			return (zone, context);
		}

		if (context.ParentRenderContext is not null)
		{
			return TryGetZone(name, context.ParentRenderContext);
		}

		return (null, null);
	}
}
