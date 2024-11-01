// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests;

using System;
using System.Linq;

using FuManchu.Parser.SyntaxTree;
using FuManchu.Text;
using FuManchu.Tokenizer;

public class Factory
{
	private readonly SourceLocationTracker _tracker = new SourceLocationTracker();
	private Span? _last;

	public Block Block(BlockType type, string? name = null, params SyntaxTreeNode[] children)
	{
		_last = null;

		var builder = new BlockBuilder();
		builder.Type = type;
		builder.Name = name;

		foreach (var child in children)
		{
			builder.Children.Add(child);
		}

		return builder.Build();
	}

	public Block Document(params SyntaxTreeNode[] children)
	{
		return Block(BlockType.Document, null, children);
	}

	public Block Tag(string name, params SyntaxTreeNode[] children)
	{
		return Block(BlockType.Tag, name, children);
	}

	public Block TagElement(string name, params SyntaxTreeNode[] children)
	{
		return Block(BlockType.TagElement, name, children);
	}

	public Block Expression(params SyntaxTreeNode[] children)
	{
		string? name = null;
		var span = children.FirstOrDefault(c => !c.IsBlock && ((Span)c).Kind == SpanKind.Expression) as Span;
		if (span != null)
		{
			name = span.Content;
		}

		return Block(BlockType.Expression, name, children);
	}

	public Block Partial(params SyntaxTreeNode[] children)
	{
		return Block(BlockType.Partial, null, children);
	}

	public Span Span(SpanKind kind, params ISymbol[] symbols)
	{
		var builder = new SpanBuilder();
		builder.Kind = kind;

		foreach (var symbol in symbols)
		{
			builder.Accept(symbol);
		}

		var span = builder.Build();
		if (_last != null)
		{
			span.Previous = _last;
			_last.Next = span;
		}
		_last = span;

		return span;
	}

	public Span Text(string content)
	{
		return Span(SpanKind.Text, Symbol(content, HandlebarsSymbolType.Text));
	}

	public Span WhiteSpace(int length, bool collapsed = false)
	{
		var span = Span(SpanKind.WhiteSpace, Symbol(string.Join("", Enumerable.Repeat(" ", length)), HandlebarsSymbolType.WhiteSpace));

		span.Collapsed = collapsed;

		return span;
	}

	public Span Parameter(string content, HandlebarsSymbolType type = HandlebarsSymbolType.Identifier)
	{
		return Span(SpanKind.Parameter, Symbol(content, HandlebarsSymbolType.Identifier));
	}

	public Span Parameter(params ISymbol[] symbols)
	{
		return Span(SpanKind.Parameter, symbols);
	}

	public Span Map(string identifier, string value, HandlebarsSymbolType valueType)
	{
		return Span(SpanKind.Map,
			Symbol(identifier, HandlebarsSymbolType.Identifier),
			Symbol("=", HandlebarsSymbolType.Assign),
			Symbol(value, valueType));
	}

	public Span Map(string identifier, Func<ISymbol[]> valueSymbols)
	{
		return Span(SpanKind.Map,
			new ISymbol[]
			{
				Symbol(identifier, HandlebarsSymbolType.Identifier),
				Symbol("=", HandlebarsSymbolType.Assign)
			}.Concat(valueSymbols()).ToArray());
	}

	public Span MetaCode(string content, HandlebarsSymbolType type)
	{
		return Span(SpanKind.MetaCode, Symbol(content, type));
	}

	public Span Expression(params ISymbol[] symbols)
	{
		return Span(SpanKind.Expression, symbols);
	}

	public ISymbol Symbol(string content, HandlebarsSymbolType type)
	{
		var location = _tracker.CurrentLocation;
		_tracker.UpdateLocation(content);

		return new HandlebarsSymbol(location, content, type);
	}
}
