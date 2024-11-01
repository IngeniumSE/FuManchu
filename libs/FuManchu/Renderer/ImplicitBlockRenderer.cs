// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Provides rendering of implicit block tags, e.g. {{#people}}{{/people}}.
/// </summary>
public class ImplicitBlockRenderer : EnumerableBlockRenderer
{
	/// <inheritdoc />
	protected override void Render(Block block, Arguments? arguments, Map? maps, RenderContext context, TextWriter writer)
	{
		string? name = block.Name;

		var children = block.Children.ToList();

		// Get the TagElement block.
		var tagElement = (Block)children[0];
		// Determine if the block prefix symbol (either # or ^) is a caret.
		bool isNegatedSection = tagElement.Children.Cast<Span>().Where(s => s.Kind == SpanKind.MetaCode).ToArray()[1].Content == "^";

		children.RemoveAt(0);
		children.RemoveAt(children.Count - 1);

		if (string.IsNullOrEmpty(name))
		{
			// Nothing we can do.
			return;
		}

		object? value = context.ResolveValue(name, false);
		if (value == null && !isNegatedSection)
		{
			// No value, nothing we can do :-(
			return;
		}

		if ((value is IEnumerable) && !(value is string) && !isNegatedSection)
		{
			RenderEnumerable((IEnumerable)value, context, children, null);
		}
		else
		{
			bool isTruthy = IsTruthy(value);

			// Treat this as a conditional block.
			if (isTruthy != isNegatedSection)
			{
				if (isTruthy)
				{
					// Create a scope around the value.
					using (var scope = context.BeginScope(value))
					{
						RenderChildren(children, context);
					}
				}
				else
				{
					RenderChildren(children, context);
				}
			}
		}
	}
}
