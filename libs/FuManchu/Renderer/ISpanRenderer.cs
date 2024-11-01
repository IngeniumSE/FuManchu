// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Defines the required contract for implementing a span renderer.
/// </summary>
public interface ISpanRenderer : ISyntaxTreeNodeRenderer<Span>
{
}
