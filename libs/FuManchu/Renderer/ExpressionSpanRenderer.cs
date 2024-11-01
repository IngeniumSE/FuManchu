// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System.IO;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Provides rendering of expressions.
/// </summary>
public class ExpressionSpanRenderer : SpanRenderer
{
	/// <inheritdoc />
	public override void Render(Span target, RenderContext context, TextWriter writer)
	{
		object? value = context.ResolveValue(target);

		Write(context, writer, value);
	}
}
