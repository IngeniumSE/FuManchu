// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using System;
using System.Collections.Generic;
using System.Linq;

using FuManchu.Parser.SyntaxTree;
using FuManchu.Text;
using FuManchu.Tokenizer;

/// <summary>
/// Represents a parser backed by a tokenizer.
/// </summary>
public abstract class TokenizerBackedParser : ParserBase
{
	private TokenizerView? _tokenizer;

	/// <summary>
	/// Initializes a new instance of the <see cref="TokenizerBackedParser"/> class.
	/// </summary>
	protected TokenizerBackedParser()
	{
		Span = new SpanBuilder();
	}

	/// <summary>
	/// Gets the current location.
	/// </summary>
	protected SourceLocation CurrentLocation
	{
		get { return (EndOfFile || !CurrentSymbol.HasValue) ? Context!.Source.Location : CurrentSymbol.Value.Start; }
	}

	/// <summary>
	/// Gets the current symbol.
	/// </summary>
	protected HandlebarsSymbol? CurrentSymbol
	{
		get { return Tokenizer?.Current; }
	}

	/// <summary>
	/// Gets a value indicating whether we are at the end of the input stream.
	/// </summary>
	protected bool EndOfFile { get { return Tokenizer.EndOfFile; } }

	/// <summary>
	/// Gets the language characteristics.
	/// </summary>
	protected abstract LanguageCharacteristics<HandlebarsTokenizer, HandlebarsSymbol, HandlebarsSymbolType> Language { get; }

	/// <summary>
	/// Gets the previous symbol.
	/// </summary>
	protected HandlebarsSymbol? PreviousSymbol { get; private set; }

	/// <summary>
	/// Gets or sets the span (builder).
	/// </summary>
	protected SpanBuilder Span { get; set; }

	/// <summary>
	/// Gets or sets the span configuration.
	/// </summary>
	protected Action<SpanBuilder>? SpanConfig { get; set; }

	/// <summary>
	/// Gets the tokenizer.
	/// </summary>
	protected TokenizerView Tokenizer
	{
		get { return _tokenizer ?? InitTokenizer(); }
	}

	/// <inheritdoc />
	public override void BuildSpan(SpanBuilder span, SourceLocation start, string content)
	{
		foreach (var sym in Language.TokenizeString(start, content))
		{
			span.Accept(sym);
		}
	}

	/// <summary>
	/// Initializes the specified span builder.
	/// </summary>
	/// <param name="span">The span builder.</param>
	protected void Initialize(SpanBuilder span)
	{
		if (SpanConfig != null)
		{
			SpanConfig(span);
		}
	}

	/// <summary>
	/// Initializes the tokenizer.
	/// </summary>
	/// <returns>The tokenizer instance.</returns>
	private TokenizerView InitTokenizer()
	{
		return _tokenizer = new TokenizerView(
			Language.CreateTokenizer(Context!.Source));
	}

	/// <summary>
	/// Accepts the specified symbol.
	/// </summary>
	/// <param name="symbol">The symbol.</param>
	protected internal void Accept(HandlebarsSymbol symbol)
	{
		if (symbol.HasValue)
		{
			foreach (var error in symbol.Errors)
			{
				Context?.OnError(error);
			}
			Span.Accept(symbol);
		}
	}

	/// <summary>
	/// Accepts the specified symbols.
	/// </summary>
	/// <param name="symbols">The symbols.</param>
	protected internal void Accept(IEnumerable<HandlebarsSymbol> symbols)
	{
		foreach (var sym in symbols)
		{
			Accept(sym);
		}
	}

	/// <summary>
	/// Accepts all symbols of the given types (in order).
	/// </summary>
	/// <param name="types">The types.</param>
	/// <returns>True if all symbol types were accepted, otherwise false.</returns>
	protected internal bool AcceptAll(params HandlebarsSymbolType[] types)
	{
		foreach (var type in types)
		{
			if (CurrentSymbol == null || !Equals(type, CurrentSymbol.Value.Type))
			{
				return false;
			}
			AcceptAndMoveNext();
		}
		return true;
	}

	/// <summary>
	/// Accepts the current token and moves to the next token.
	/// </summary>
	/// <returns>True if we could move to the next token.</returns>
	protected internal bool AcceptAndMoveNext()
	{
		Accept(CurrentSymbol!.Value);
		return NextToken();
	}


	/// <summary>
	/// Accepts all tokens until they match the given type.
	/// </summary>
	/// <param name="type">The first type.</param>
	protected internal void AcceptUntil(HandlebarsSymbolType type)
	{
		AcceptWhile(sym => !Equals(type, sym.Type));
	}

	/// <summary>
	/// Accepts all tokens until they match any of the given types.
	/// </summary>
	/// <param name="type1">The first type.</param>
	/// <param name="type2">The second type.</param>
	protected internal void AcceptUntil(HandlebarsSymbolType type1, HandlebarsSymbolType type2)
	{
		AcceptWhile(sym => !Equals(type1, sym.Type) && !Equals(type2, sym.Type));
	}

	/// <summary>
	/// Accepts all tokens until they match any of the given types.
	/// </summary>
	/// <param name="type1">The first type.</param>
	/// <param name="type2">The second type.</param>
	/// <param name="type3">The third type.</param>
	protected internal void AcceptUntil(HandlebarsSymbolType type1, HandlebarsSymbolType type2, HandlebarsSymbolType type3)
	{
		AcceptWhile(sym => !Equals(type1, sym.Type) && !Equals(type2, sym.Type) && !Equals(type3, sym.Type));
	}

	/// <summary>
	/// Accepts all tokens until they match any of the given types.
	/// </summary>
	/// <param name="types">The types.</param>
	protected internal void AcceptUntil(params HandlebarsSymbolType[] types)
	{
		AcceptWhile(sym => types.All(t => !Equals(t, sym.Type)));
	}

	/// <summary>
	/// Accepts all tokens while they match the given type.
	/// </summary>
	/// <param name="type">The type.</param>
	protected internal void AcceptWhile(HandlebarsSymbolType type)
	{
		AcceptWhile(sym => Equals(type, sym.Type));
	}

	/// <summary>
	/// Accepts all tokens while they match any of the given types.
	/// </summary>
	/// <param name="type1">The first type.</param>
	/// <param name="type2">The second type.</param>
	protected internal void AcceptWhile(HandlebarsSymbolType type1, HandlebarsSymbolType type2)
	{
		AcceptWhile(sym => Equals(type1, sym.Type) || Equals(type2, sym.Type));
	}

	/// <summary>
	/// Accepts all tokens while they match any of the given types.
	/// </summary>
	/// <param name="type1">The first type.</param>
	/// <param name="type2">The second type.</param>
	/// <param name="type3">The third type.</param>
	protected internal void AcceptWhile(HandlebarsSymbolType type1, HandlebarsSymbolType type2, HandlebarsSymbolType type3)
	{
		AcceptWhile(sym => Equals(type1, sym.Type) || Equals(type2, sym.Type) || Equals(type3, sym.Type));
	}

	/// <summary>
	/// Accepts all tokens while they match any of the given types.
	/// </summary>
	/// <param name="types">The types.</param>
	protected internal void AcceptWhile(params HandlebarsSymbolType[] types)
	{
		AcceptWhile(sym => types.Any(t => Equals(t, sym.Type)));
	}

	/// <summary>
	/// Accepts all tokens while the given condition is met.
	/// </summary>
	/// <param name="condition">The condition.</param>
	protected internal void AcceptWhile(Func<HandlebarsSymbol, bool> condition)
	{
		Accept(ReadWhileLazy(condition));
	}

	/// <summary>
	/// Determines if the parser is currently at a symbol of the specified type.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>True if we are currently at a symbol of the specified type, otherwise false.</returns>
	protected internal bool At(HandlebarsSymbolType type)
	{
		return !EndOfFile && CurrentSymbol != null && Equals(type, CurrentSymbol.Value.Type);
	}

	/// <summary>
	/// Configures Span to be of the given kind.
	/// </summary>
	/// <param name="kind">The kind of span..</param>
	/// <param name="collapsed">Whether the span is collapsed.</param>
	private void Configure(SpanKind kind, bool collapsed)
	{
		Span.Kind = kind;
		Span.Collapsed = collapsed;
	}

	/// <summary>
	/// Configures the span builder using the given configuration delegate.
	/// </summary>
	/// <param name="config">The configuration.</param>
	protected void ConfigureSpan(Action<SpanBuilder> config)
	{
		SpanConfig = config;
		Initialize(Span);
	}

	/// <summary>
	/// Configures the span builder using the given configuration delegate.
	/// </summary>
	/// <param name="config">The configuration.</param>
	protected void ConfigureSpan(Action<SpanBuilder, Action<SpanBuilder>> config)
	{
		Action<SpanBuilder>? prev = SpanConfig;
		if (config == null)
		{
			SpanConfig = null;
		}
		else
		{
			SpanConfig = span => config(span, prev!);
		}
		Initialize(Span);
	}

	/// <summary>
	/// Ensures the current symbol is read.
	/// </summary>
	/// <returns>True if the current symbol was read, otherwise false.</returns>
	protected bool EnsureCurrent()
	{
		if (CurrentSymbol == null)
		{
			return NextToken();
		}
		return true;
	}

	/// <summary>
	/// Accepts all tokens of the given type (in order).
	/// </summary>
	/// <param name="types">The types.</param>
	protected internal void Expected(params HandlebarsSymbolType[] types)
	{
		AcceptAndMoveNext();
	}

	/// <summary>
	/// Determines if the next symbol matches the given type.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>True if the next symbol matches the type, otherwise false.</returns>
	protected internal bool NextIs(HandlebarsSymbolType type)
	{
		return NextIs(sym => sym.HasValue && Equals(type, sym.Type));
	}

	/// <summary>
	/// Determines if the next symbol matches any of the given types.
	/// </summary>
	/// <param name="types">The typse.</param>
	/// <returns>True if the next symbol matches any of the given types, otherwise false.</returns>
	protected internal bool NextIs(params HandlebarsSymbolType[] types)
	{
		return NextIs(sym => sym.HasValue && types.Any(t => Equals(t, sym.Type)));
	}

	/// <summary>
	/// Determines if the next symbol matches the given condition.
	/// </summary>
	/// <param name="condition">The condition.</param>
	/// <returns>True if the next symbol matches the condition, otherwise false.</returns>
	protected internal bool NextIs(Func<HandlebarsSymbol, bool> condition)
	{
		var current = CurrentSymbol;
		if (NextToken())
		{
			var result = condition(CurrentSymbol!.Value);
			PutCurrentBack();
			PutBack(current!.Value);
			EnsureCurrent();
			return result;
		}

		return false;
	}

	/// <summary>
	/// Moves to the next token.
	/// </summary>
	/// <returns>True if we advanced to the next token, otherwise false.</returns>
	protected internal bool NextToken()
	{
		PreviousSymbol = CurrentSymbol;
		return Tokenizer.Next();
	}

	/// <summary>
	/// Outputs the current set of matched symbols as a span.
	/// </summary>
	protected internal void Output()
	{
		if (Span.Symbols.Count > 0)
		{
			Context?.AddSpan(Span.Build());
			Initialize(Span);
		}
	}

	/// <summary>
	/// Outputs the current set of symbols as the given span kind.
	/// </summary>
	/// <param name="kind">The kind.</param>
	/// <param name="collapsed">Whether the span is collapsed.</param>
	protected internal void Output(SpanKind kind, bool collapsed = false)
	{
		Configure(kind, collapsed);
		Output();
	}

	/// <summary>
	/// Accepts an option symbol type.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>True if the optional type was found, otherwise false.</returns>
	protected internal bool Optional(HandlebarsSymbolType type)
	{
		if (At(type))
		{
			AcceptAndMoveNext();
			return true;
		}
		return false;
	}

	/// <summary>
	/// Pushes the span configuration.
	/// </summary>
	/// <returns>The disposable used to restore the previous span configuration.</returns>
	protected IDisposable PushSpanConfig()
	{
		return PushSpanConfig(newConfig: (Action<SpanBuilder, Action<SpanBuilder>>?)null);
	}

	/// <summary>
	/// Pushes the span configuration.
	/// </summary>
	/// <param name="newConfig">The new configuration.</param>
	/// <returns>The disposable used to restore the previous span configuration.</returns>
	protected IDisposable PushSpanConfig(Action<SpanBuilder> newConfig)
	{
		return PushSpanConfig(newConfig == null ? (Action<SpanBuilder, Action<SpanBuilder>>?)null : (span, _) => newConfig(span));
	}

	/// <summary>
	/// Pushes the span configuration.
	/// </summary>
	/// <param name="newConfig">The new configuration.</param>
	/// <returns>The disposable used to restore the previous span configuration.</returns>
	protected IDisposable PushSpanConfig(Action<SpanBuilder, Action<SpanBuilder>>? newConfig)
	{
		Action<SpanBuilder>? old = SpanConfig;
		if (newConfig is not null)
		{
			ConfigureSpan(newConfig);
		}
		return new DisposableAction(() => SpanConfig = old);
	}

	/// <summary>
	/// Resets the source back to the beginning of the symbol.
	/// </summary>
	/// <param name="symbol">The symbol.</param>
	protected internal void PutBack(HandlebarsSymbol symbol)
	{
		if (symbol.HasValue)
		{
			Tokenizer.PutBack(symbol);
		}
	}

	/// <summary>
	/// Puts the set of symbols back (in reverse order).
	/// </summary>
	/// <param name="symbols">The symbols.</param>
	protected internal void PutBack(IEnumerable<HandlebarsSymbol> symbols)
	{
		foreach (var symbol in symbols.Reverse())
		{
			PutBack(symbol);
		}
	}

	/// <summary>
	/// Puts the current back in the input stream.
	/// </summary>
	protected internal void PutCurrentBack()
	{
		if (!EndOfFile && CurrentSymbol != null)
		{
			PutBack(CurrentSymbol!.Value);
		}
	}

	/// <summary>
	/// Determines if the current token is of the given required type.
	/// </summary>
	/// <param name="expected">The expected.</param>
	/// <param name="errorIfNotFound">if set to <c>true</c> [error if not found].</param>
	/// <returns>True if the token was found, otherwise false.</returns>
	protected internal bool Required(HandlebarsSymbolType expected, bool errorIfNotFound)
	{
		bool found = At(expected);
		if (!found && errorIfNotFound)
		{
			string error = "Expected: " + expected.ToString();
			Context?.OnError(CurrentLocation, error);
		}
		return found;
	}

	/// <summary>
	/// Reads all tokens while the condition is met.
	/// </summary>
	/// <param name="condition">The condition.</param>
	/// <returns>The set of read tokens.</returns>
	protected internal IEnumerable<HandlebarsSymbol> ReadWhile(Func<HandlebarsSymbol, bool> condition)
	{
		return ReadWhileLazy(condition).ToList();
	}

	/// <summary>
	/// Lazily reads the tokens while the condition is met.
	/// </summary>
	/// <param name="condition">The condition.</param>
	/// <returns>The set of read tokens.</returns>
	internal IEnumerable<HandlebarsSymbol> ReadWhileLazy(Func<HandlebarsSymbol, bool> condition)
	{
		while (EnsureCurrent() && condition(CurrentSymbol!.Value))
		{
			yield return CurrentSymbol!.Value;
			NextToken();
		}
	}

	/// <summary>
	/// Determines if the previous symbol matches the given type.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>True if the previous symbol matches the given type, otherwise false.</returns>
	protected internal bool Was(HandlebarsSymbolType type)
	{
		return PreviousSymbol != null && Equals(type, PreviousSymbol.Value.Type);
	}
}
