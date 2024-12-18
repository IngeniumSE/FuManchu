﻿// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser.SyntaxTree;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using FuManchu.Text;
using FuManchu.Tokenizer;

/// <summary>
/// Builds span instances.
/// </summary>
public class SpanBuilder
{
	private IList<HandlebarsSymbol> _symbols = new List<HandlebarsSymbol>();
	private SourceLocationTracker _tracker = new SourceLocationTracker();

	/// <summary>
	/// Initializes a new instance of the <see cref="SpanBuilder"/> class.
	/// </summary>
	public SpanBuilder()
	{
		Reset();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SpanBuilder"/> class.
	/// </summary>
	/// <param name="original">The original span.</param>
	public SpanBuilder(Span original)
	{
		Collapsed = original.Collapsed;
		Kind = original.Kind;
		_symbols = new List<HandlebarsSymbol>(original.Symbols);
		Start = original.Start;
	}

	/// <summary>
	/// Gets or sets whether the span is collapsed.
	/// </summary>
	public bool Collapsed { get; set; }

	/// <summary>
	/// Gets or sets the kind.
	/// </summary>
	public SpanKind Kind { get; set; }

	/// <summary>
	/// Gets or sets the start.
	/// </summary>
	public SourceLocation Start { get; set; }

	/// <summary>
	/// Gets the symbols.
	/// </summary>
	public ReadOnlyCollection<HandlebarsSymbol> Symbols { get { return new ReadOnlyCollection<HandlebarsSymbol>(_symbols); } }

	/// <summary>
	/// Accepts the specified symbol.
	/// </summary>
	/// <param name="symbol">The symbol.</param>
	public void Accept(HandlebarsSymbol? symbol)
	{
		if (symbol == null)
		{
			return;
		}

		if (_symbols.Count == 0)
		{
			Start = symbol.Value.Start;
			symbol.Value.ChangeStart(SourceLocation.Zero);
			_tracker.CurrentLocation = SourceLocation.Zero;
		}
		else
		{
			symbol.Value.ChangeStart(_tracker.CurrentLocation);
		}

		_symbols.Add(symbol.Value);
		_tracker.UpdateLocation(symbol.Value.Content);
	}

	/// <summary>
	/// Builds a new Span
	/// </summary>
	/// <returns>The Span instance.</returns>
	public Span Build()
	{
		return new Span(this);
	}

	/// <summary>
	/// Clears the symbols.
	/// </summary>
	public void ClearSymbols()
	{
		_symbols.Clear();
	}

	/// <summary>
	/// Resets this instance.
	/// </summary>
	public void Reset()
	{
		_symbols = new List<HandlebarsSymbol>();
		Start = SourceLocation.Zero;
	}
}
