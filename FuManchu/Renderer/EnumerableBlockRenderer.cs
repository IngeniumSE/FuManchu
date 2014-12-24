﻿namespace FuManchu.Renderer
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using FuManchu.Parser.SyntaxTree;

	/// <summary>
	/// Provides rendering of enumerations.
	/// </summary>
	public class EnumerableBlockRenderer : BlockRenderer
	{
		/// <inheritdoc />
		protected override void Render(Block block, object[] arguments, Dictionary<string, object> maps, RenderContext context, TextWriter writer)
		{
			var enumerable = arguments[0];
			if (!(enumerable is IEnumerable))
			{
				enumerable = new object[] { enumerable };
			}

			var children = block.Children.ToList();
			children.RemoveAt(0);
			children.RemoveAt(children.Count - 1);

			var elseChildren = new List<SyntaxTreeNode>();

			// Determine if there is an alternate {{else}} block which denotes content to display when there are no items.
			var elseNode = children.Find(n => n.IsBlock && ((Block)n).Name == "else");
			if (elseNode != null)
			{
				int elseIndex = children.IndexOf(elseNode);
				elseChildren = children.Skip(elseIndex + 1).ToList();
				children = children.Take(elseIndex).ToList();
			}

			int index = 0;
			bool hasItems = false;

			var dict = enumerable as IDictionary;
			if (dict != null)
			{
				int maxIndex = dict.Count - 1;

				foreach (var key in dict.Keys)
				{
					hasItems = true;
					var item = dict[key];

					using (var scope = context.BeginScope(item))
					{
						scope.ScopeContext.SetVariable("first", (index == 0));
						scope.ScopeContext.SetVariable("last", (index == maxIndex));
						scope.ScopeContext.SetVariable("index", index);
						scope.ScopeContext.SetVariable("key", key);

						foreach (var child in children)
						{
							RenderChild(child, scope.ScopeContext);
						}
					}
					index++;
				}
			}
			else
			{
				var array = (enumerable is Array) ? (object[])enumerable : (((IEnumerable)enumerable).Cast<object>().ToArray());
				int maxIndex = array.Length - 1;

				for (index = 0; index <= maxIndex; index++)
				{
					hasItems = true;
					var item = array[index];

					using (var scope = context.BeginScope(item))
					{
						scope.ScopeContext.SetVariable("first", (index == 0));
						scope.ScopeContext.SetVariable("last", (index == maxIndex));
						scope.ScopeContext.SetVariable("index", index);

						foreach (var child in children)
						{
							RenderChild(child, scope.ScopeContext);
						}
					}
				}
			}

			if (!hasItems && elseChildren.Count > 0)
			{
				RenderChildren(elseChildren, context);
			}
		}
	}
}