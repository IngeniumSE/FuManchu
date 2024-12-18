﻿// This work is licensed under the terms of the MIT license.
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

	public Block Block(BlockType type, string? name = null, bool isZonedPartial = false, params SyntaxTreeNode[] children)
	{
		_last = null;

		var builder = new BlockBuilder();
		builder.Type = type;
		builder.Name = name;
		builder.IsPartialBlock = isZonedPartial;

		foreach (var child in children)
		{
			builder.Children.Add(child);
		}

		return builder.Build();
	}

	public Block Document(params SyntaxTreeNode[] children)
	{
		return Block(BlockType.Document, null, false, children);
	}

	public Block Tag(string name, params SyntaxTreeNode[] children)
	{
		return Block(BlockType.Tag, name, false, children);
	}

	public Block TagElement(string name, params SyntaxTreeNode[] children)
	{
		return Block(BlockType.TagElement, name, false, children);
	}

	public Block Expression(params SyntaxTreeNode[] children)
	{
		string? name = null;
		var span = children.FirstOrDefault(c => !c.IsBlock && ((Span)c).Kind == SpanKind.Expression) as Span;
		if (span != null)
		{
			name = span.Content;
		}

		return Block(BlockType.Expression, name, false, children);
	}

	public Block Partial(string name, params SyntaxTreeNode[] children)
	{
		return Block(BlockType.Partial, name, false, children);
	}

	public Block Zone(string name, params SyntaxTreeNode[] children)
	{
		return Block(BlockType.Zone, name, false, children);
	}

	public Block PartialBlock(string name, params SyntaxTreeNode[] children)
	{
		return Block(BlockType.PartialBlock, name, true, children);
	}

	public Block PartialBlockElement(string name, params SyntaxTreeNode[] children)
	{
		return Block(BlockType.PartialBlockElement, name, true, children);
	}

	public Block PartialBlockContent(string name, params SyntaxTreeNode[] children)
	{
		return Block(BlockType.PartialBlockContent, name, true, children);
	}

	public Block PartialBlockContentElement(string name, params SyntaxTreeNode[] children)
	{
		return Block(BlockType.PartialBlockContentElement, name, true, children);
	}

	public Span Span(SpanKind kind, params HandlebarsSymbol[] symbols)
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

	public Span Parameter(params HandlebarsSymbol[] symbols)
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

	public Span Map(string identifier, Func<HandlebarsSymbol[]> valueSymbols)
	{
		return Span(SpanKind.Map,
			new HandlebarsSymbol[]
			{
				Symbol(identifier, HandlebarsSymbolType.Identifier),
				Symbol("=", HandlebarsSymbolType.Assign)
			}.Concat(valueSymbols()).ToArray());
	}

	public Span MetaCode(string content, HandlebarsSymbolType type)
	{
		return Span(SpanKind.MetaCode, Symbol(content, type));
	}

	public Span Expression(params HandlebarsSymbol[] symbols)
	{
		return Span(SpanKind.Expression, symbols);
	}

	public HandlebarsSymbol Symbol(string content, HandlebarsSymbolType type)
	{
		var location = _tracker.CurrentLocation;
		_tracker.UpdateLocation(content);

		return new HandlebarsSymbol(location, content, type);
	}
}
