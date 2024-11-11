// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using FuManchu.Parser.SyntaxTree;
using FuManchu.Text;
using FuManchu.Tokenizer;

/// <summary>
/// Provides a base implementation of a syntax tree renderer.
/// </summary>
/// <typeparam name="T">The node type.</typeparam>
public abstract class SyntaxTreeNodeRenderer<T> : ISyntaxTreeNodeRenderer<T> where T : SyntaxTreeNode
{
	/// <summary>
	/// Determines if the given value is truthy.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <returns>True if the value is truthy, otherwise false.</returns>
	public bool IsTruthy(object? value)
	{
		if (value == null)
		{
			return false;
		}

		if (value is string)
		{
			if (string.IsNullOrEmpty((string)value))
			{
				return false;
			}
		}

		if (value is Array)
		{
			if (((Array)value).Length == 0)
			{
				return false;
			}
		}

		if (value is ICollection)
		{
			if (((ICollection)value).Count == 0)
			{
				return false;
			}
		}

#if DOTNET5_4
					// MA - This needs to be improved once GetTypeCode is supported via CoreFX
					if (value.GetType().GetTypeInfo().IsValueType)
					{
			if ((int)(Convert.ChangeType(value, typeof(int))) == 0)
			{
				return false;
			}
					}
#else
		switch (Type.GetTypeCode(value.GetType()))
		{
			case TypeCode.Boolean:
				{
					return (bool)value;
				}
			case TypeCode.Byte:
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
				{
					return ((byte)value == 0);
				}
			case TypeCode.Decimal:
				{
					return ((Decimal)value == 0.0m);
				}
			case TypeCode.Single:
			case TypeCode.Double:
				{
					return ((float)value == 0.0F);
				}
		}
#endif

		return true;
	}

	/// <inheritdoc />
	public abstract void Render(T target, RenderContext context, TextWriter writer);

	/// <summary>
	/// Gets the arguments and mapped parameters from the given node.
	/// </summary>
	/// <param name="block">The target block.</param>
	/// <param name="context">The render context.</param>
	/// <returns>The set of arguments and mapped parameters.</returns>
	protected virtual Tuple<object?[], Map> GetArgumentsAndMappedParameters(Block block, RenderContext context)
	{
		var arguments = new List<object?>();
		var maps = new Map();

		var items = block.Children.OfType<Span>().Where(c => c.Kind == SpanKind.Parameter || c.Kind == SpanKind.Map);

		foreach (var span in items)
		{
			if (span.Kind == SpanKind.Parameter)
			{
				arguments.Add(context.ResolveValue(span));
			}
			else
			{
				var symbols = span.Symbols.Cast<HandlebarsSymbol>().ToList();
				string key = symbols[0].Content;

				object? value = context.ResolveValue(span);

				if (maps.ContainsKey(key))
				{
					maps[key] = value;
				}
				else
				{
					maps.Add(key, value);
				}
			}
		}

		return Tuple.Create(arguments.ToArray(), maps);
	}

	/// <summary>
	/// Writes the given value to the text writer.
	/// </summary>
	/// <param name="context">The render context.</param>
	/// <param name="writer">The text writer.</param>
	/// <param name="value">The value to write.</param>
	protected virtual void Write(RenderContext context, TextWriter writer, object? value)
	{
		if (context == null)
		{
			throw new ArgumentNullException("context");
		}

		if (writer == null)
		{
			throw new ArgumentNullException("writer");
		}

		if (value == null)
		{
			return;
		}

		string output = "";

		var encoded = value as IEncodedString;
		if (encoded == null)
		{
			output = context.EscapeEncoding ? value.ToString() : WebUtility.HtmlEncode(value.ToString());
		}
		else
		{
			output = encoded.ToEncodedString();
		}

		writer.Write(output);
	}
}
