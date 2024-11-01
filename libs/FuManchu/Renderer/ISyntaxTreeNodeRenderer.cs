// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System.IO;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Defines the required contract for implementing a syntax tree renderer.
/// </summary>
public interface ISyntaxTreeNodeRenderer<in T> where T : SyntaxTreeNode
{
	/// <summary>
	/// Renders the specified syntax tree node.
	/// </summary>
	/// <param name="target">The target node.</param>
	/// <param name="context">The context.</param>
	/// <param name="writer">The writer.</param>
	void Render(T target, RenderContext context, TextWriter writer);
}
