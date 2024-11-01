// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Provides a base implementation of a span renderer.
/// </summary>
public abstract class SpanRenderer : SyntaxTreeNodeRenderer<Span>, ISpanRenderer { }
