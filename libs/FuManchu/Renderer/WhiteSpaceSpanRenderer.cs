// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System.IO;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Renders whitespace to the output writer.
/// </summary>
public class WhiteSpaceSpanRenderer : SpanRenderer
{
	/// <inheritdoc />
	public override void Render(Span target, RenderContext context, TextWriter writer)
	{
		if (target.Collapsed)
		{
			// Span is collapsed, so do not render.
			return;
		}

		string content = target == null || target.Content == null ? string.Empty : target.Content;

		Write(context, writer, new SafeString(content));
	}
}
