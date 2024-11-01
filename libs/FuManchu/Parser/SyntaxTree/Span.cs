// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser.SyntaxTree;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FuManchu.Text;
using FuManchu.Tokenizer;

/// <summary>
/// Represents a span.
/// </summary>
public class Span : SyntaxTreeNode
{
	private SourceLocation _start;

	/// <summary>
	/// Initialises a new instance of <see cref="Span"/>
	/// </summary>
	/// <param name="builder">The span builder.</param>
	public Span(SpanBuilder builder)
	{
		ReplaceWith(builder);
	}

	/// <summary>
	/// Gets whether the span is collapsed.
	/// </summary>
	public bool Collapsed { get; set; }

	/// <summary>
	/// Gets the content of the span.
	/// </summary>
	public string? Content { get; private set; }

	/// <inheritdoc />
	public override bool IsBlock
	{
		get { return false; }
	}

	/// <inheritdoc />
	public override int Length => Content is { Length: > 0 } ? Content.Length : 0;

	/// <summary>
	/// Gets the kind.
	/// </summary>
	public SpanKind Kind { get; private set; }

	/// <summary>
	/// Gets the next span.
	/// </summary>
	public Span? Next { get; set; }

	/// <summary>
	/// Gets the previous span.
	/// </summary>
	public Span? Previous { get; set; }

	/// <inheritdoc />
	public override SourceLocation Start
	{
		get { return _start; }
	}

	/// <summary>
	/// Gets the set of symbols.
	/// </summary>
	public IEnumerable<ISymbol>? Symbols { get; private set; }

	/// <inheritdoc />
	public override void Accept(IParserVisitor visitor)
	{
		visitor.VisitSpan(this);
	}

	/// <summary>
	/// Changes the start position of the current span.
	/// </summary>
	/// <param name="newStart">The new start location.</param>
	public void ChangeStart(SourceLocation newStart)
	{
		_start = newStart;
		var current = this;
		var tracker = new SourceLocationTracker(newStart);
		tracker.UpdateLocation(Content);
		while ((current = current.Next) != null)
		{
			current._start = tracker.CurrentLocation;
			tracker.UpdateLocation(current.Content);
		}
	}

	/// <inheritdoc />
	public override bool EquivalentTo(SyntaxTreeNode node)
	{
		var other = node as Span;
		return other != null &&
				 Kind.Equals(other.Kind) &&
					 Start.Equals(other.Start) &&
				 Collapsed.Equals(other.Collapsed) &&
					 string.Equals(Content, other.Content, StringComparison.Ordinal);
	}

	/// <inheritdoc />
	public override bool EquivalentTo(SyntaxTreeNode node, StringBuilder builder, int level)
	{
		var result = EquivalentTo(node);
		builder.Append(string.Join("", Enumerable.Repeat("\t", level)));
		builder.AppendFormat("{0}: Expected: {1}, Actual: {2}\n", result ? "P" : "F", this, node);
		return result;
	}

	/// <inheritdoc />
	public override bool Equals(object obj) => obj is Span other && Equals(other);

	public bool Equals(Span other)
	{
		return Kind == other.Kind &&
			Start.Equals(other.Start) &&
			string.Equals(Content, other.Content, StringComparison.Ordinal) &&
			Collapsed == other.Collapsed;
	}	

	/// <inheritdoc />
	public override int GetHashCode() => HashCode.Combine(Kind, Start, Content, Collapsed);

	/// <summary>
	/// Replaces the content of the span with that built form the given builder.
	/// </summary>
	/// <param name="builder">The span builder.</param>
	public void ReplaceWith(SpanBuilder builder)
	{
		Collapsed = builder.Collapsed;
		Kind = builder.Kind;
		Symbols = builder.Symbols;
		_start = builder.Start;

		builder.Reset();

		Content = Symbols.Aggregate(
			new StringBuilder(),
			(sb, sym) => sb.Append(sym.Content),
			sb => sb.ToString());
	}

	/// <summary>
	/// Sets the start of the span.
	/// </summary>
	/// <param name="newStart"></param>
	internal void SetStart(SourceLocation newStart)
	{
		_start = newStart;
	}

	/// <inheritdoc />
	public override string ToString() => $"SPAN: {Kind}, {Content} @ {Start}{(Collapsed ? " [Collapsed]" : "")}";

#if DEBUG
	public override string DebugToString()
	{
		return Content ?? "";
	}
#endif
}
