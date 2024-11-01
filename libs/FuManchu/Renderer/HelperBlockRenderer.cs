// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using FuManchu.Parser.SyntaxTree;

/// <summary>
/// Renders custom helper blocks.
/// </summary>
public class HelperBlockRenderer : BlockRenderer
{
	/// <inheritdoc />
	protected override void Render(
		Block block,
		Arguments? arguments,
		Map? maps,
		RenderContext context,
		TextWriter writer)
	{
		if (context.Service == null)
		{
			// There is nothing we can do, no service has been assigned.
			return;
		}

		var options = new HelperOptions(
			context,
			arguments,
			maps);

		if (block.Type == BlockType.Tag)
		{
			var children = block.Children.ToList();
			children.RemoveAt(0);
			children.RemoveAt(children.Count - 1);

			options.Render = (data) => RenderHelperChildren(children, context, data);
		}

		string result = context.Service.RunHelper(block.Name!, options);

		Write(context, writer, new SafeString(result));
	}

	/// <summary>
	/// Renders the content of a block helper.
	/// </summary>
	/// <param name="children">The children of the helper block.</param>
	/// <param name="context">The render context.</param>
	/// <param name="data">The new data model.</param>
	/// <returns>The string content of the result.</returns>
	private string RenderHelperChildren(
		IEnumerable<SyntaxTreeNode> children,
		RenderContext context,
		object data)
	{
		RenderContext targetContext = context;
		RenderContextScope? scope = null;
		if (data is not null)
		{
			scope = context.BeginScope(data);
			targetContext = scope.ScopeContext;
		}

		using (var writer = new StringWriter())
		{
			var renderer = new RenderingParserVisitor(writer, targetContext, context.ModelMetadataProvider);

			foreach (var node in children)
			{
				node.Accept(renderer);
			}

			if (scope is not null)
			{
				scope.Dispose();
			}

			return writer.GetStringBuilder().ToString();
		}
	}
}
