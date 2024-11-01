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
/// <typeparam name="TTokenizer">The type of the tokenizer.</typeparam>
/// <typeparam name="TSymbol">The type of the symbol.</typeparam>
/// <typeparam name="TSymbolType">The type of the symbol type.</typeparam>
public abstract class TokenizerBackedParser<TTokenizer, TSymbol, TSymbolType> : ParserBase
	where TTokenizer : Tokenizer<TSymbol, TSymbolType>
	where TSymbol : SymbolBase<TSymbolType>
	where TSymbolType : struct
{
	private TokenizerView<TTokenizer, TSymbol, TSymbolType>? _tokenizer;

	/// <summary>
	/// Initializes a new instance of the <see cref="TokenizerBackedParser{TTokenizer, TSymbol, TSymbolType}"/> class.
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
		get { return (EndOfFile || CurrentSymbol == null) ? Context!.Source.Location : CurrentSymbol.Start; }
	}

	/// <summary>
	/// Gets the current symbol.
	/// </summary>
	protected TSymbol? CurrentSymbol
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
	protected abstract LanguageCharacteristics<TTokenizer, TSymbol, TSymbolType> Language { get; }

	/// <summary>
	/// Gets the previous symbol.
	/// </summary>
	protected TSymbol? PreviousSymbol { get; private set; }

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
	protected TokenizerView<TTokenizer, TSymbol, TSymbolType> Tokenizer
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
	private TokenizerView<TTokenizer, TSymbol, TSymbolType> InitTokenizer()
	{
		return _tokenizer = new TokenizerView<TTokenizer, TSymbol, TSymbolType>(
			Language.CreateTokenizer(Context!.Source));
	}

	/// <summary>
	/// Accepts the specified symbol.
	/// </summary>
	/// <param name="symbol">The symbol.</param>
	protected internal void Accept(TSymbol symbol)
	{
		if (symbol != null)
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
	protected internal void Accept(IEnumerable<TSymbol> symbols)
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
	protected internal bool AcceptAll(params TSymbolType[] types)
	{
		foreach (var type in types)
		{
			if (CurrentSymbol == null || !Equals(type, CurrentSymbol.Type))
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
		Accept(CurrentSymbol!);
		return NextToken();
	}


	/// <summary>
	/// Accepts all tokens until they match the given type.
	/// </summary>
	/// <param name="type">The first type.</param>
	protected internal void AcceptUntil(TSymbolType type)
	{
		AcceptWhile(sym => !Equals(type, sym.Type));
	}

	/// <summary>
	/// Accepts all tokens until they match any of the given types.
	/// </summary>
	/// <param name="type1">The first type.</param>
	/// <param name="type2">The second type.</param>
	protected internal void AcceptUntil(TSymbolType type1, TSymbolType type2)
	{
		AcceptWhile(sym => !Equals(type1, sym.Type) && !Equals(type2, sym.Type));
	}

	/// <summary>
	/// Accepts all tokens until they match any of the given types.
	/// </summary>
	/// <param name="type1">The first type.</param>
	/// <param name="type2">The second type.</param>
	/// <param name="type3">The third type.</param>
	protected internal void AcceptUntil(TSymbolType type1, TSymbolType type2, TSymbolType type3)
	{
		AcceptWhile(sym => !Equals(type1, sym.Type) && !Equals(type2, sym.Type) && !Equals(type3, sym.Type));
	}

	/// <summary>
	/// Accepts all tokens until they match any of the given types.
	/// </summary>
	/// <param name="types">The types.</param>
	protected internal void AcceptUntil(params TSymbolType[] types)
	{
		AcceptWhile(sym => types.All(t => !Equals(t, sym.Type)));
	}

	/// <summary>
	/// Accepts all tokens while they match the given type.
	/// </summary>
	/// <param name="type">The type.</param>
	protected internal void AcceptWhile(TSymbolType type)
	{
		AcceptWhile(sym => Equals(type, sym.Type));
	}

	/// <summary>
	/// Accepts all tokens while they match any of the given types.
	/// </summary>
	/// <param name="type1">The first type.</param>
	/// <param name="type2">The second type.</param>
	protected internal void AcceptWhile(TSymbolType type1, TSymbolType type2)
	{
		AcceptWhile(sym => Equals(type1, sym.Type) || Equals(type2, sym.Type));
	}

	/// <summary>
	/// Accepts all tokens while they match any of the given types.
	/// </summary>
	/// <param name="type1">The first type.</param>
	/// <param name="type2">The second type.</param>
	/// <param name="type3">The third type.</param>
	protected internal void AcceptWhile(TSymbolType type1, TSymbolType type2, TSymbolType type3)
	{
		AcceptWhile(sym => Equals(type1, sym.Type) || Equals(type2, sym.Type) || Equals(type3, sym.Type));
	}

	/// <summary>
	/// Accepts all tokens while they match any of the given types.
	/// </summary>
	/// <param name="types">The types.</param>
	protected internal void AcceptWhile(params TSymbolType[] types)
	{
		AcceptWhile(sym => types.Any(t => Equals(t, sym.Type)));
	}

	/// <summary>
	/// Accepts all tokens while the given condition is met.
	/// </summary>
	/// <param name="condition">The condition.</param>
	protected internal void AcceptWhile(Func<TSymbol, bool> condition)
	{
		Accept(ReadWhileLazy(condition));
	}

	/// <summary>
	/// Determines if the parser is currently at a symbol of the specified type.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>True if we are currently at a symbol of the specified type, otherwise false.</returns>
	protected internal bool At(TSymbolType type)
	{
		return !EndOfFile && CurrentSymbol != null && Equals(type, CurrentSymbol.Type);
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
	protected internal void Expected(params TSymbolType[] types)
	{
		AcceptAndMoveNext();
	}

	/// <summary>
	/// Determines if the next symbol matches the given type.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>True if the next symbol matches the type, otherwise false.</returns>
	protected internal bool NextIs(TSymbolType type)
	{
		return NextIs(sym => sym != null && Equals(type, sym.Type));
	}

	/// <summary>
	/// Determines if the next symbol matches any of the given types.
	/// </summary>
	/// <param name="types">The typse.</param>
	/// <returns>True if the next symbol matches any of the given types, otherwise false.</returns>
	protected internal bool NextIs(params TSymbolType[] types)
	{
		return NextIs(sym => sym != null && types.Any(t => Equals(t, sym.Type)));
	}

	/// <summary>
	/// Determines if the next symbol matches the given condition.
	/// </summary>
	/// <param name="condition">The condition.</param>
	/// <returns>True if the next symbol matches the condition, otherwise false.</returns>
	protected internal bool NextIs(Func<TSymbol, bool> condition)
	{
		var current = CurrentSymbol;
		NextToken();
		var result = condition(CurrentSymbol!);
		PutCurrentBack();
		PutBack(current!);
		EnsureCurrent();
		return result;
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
	protected internal bool Optional(TSymbolType type)
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
	protected internal void PutBack(TSymbol symbol)
	{
		if (symbol != null)
		{
			Tokenizer.PutBack(symbol);
		}
	}

	/// <summary>
	/// Puts the set of symbols back (in reverse order).
	/// </summary>
	/// <param name="symbols">The symbols.</param>
	protected internal void PutBack(IEnumerable<TSymbol> symbols)
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
			PutBack(CurrentSymbol);
		}
	}

	/// <summary>
	/// Determines if the current token is of the given required type.
	/// </summary>
	/// <param name="expected">The expected.</param>
	/// <param name="errorIfNotFound">if set to <c>true</c> [error if not found].</param>
	/// <returns>True if the token was found, otherwise false.</returns>
	protected internal bool Required(TSymbolType expected, bool errorIfNotFound)
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
	protected internal IEnumerable<TSymbol> ReadWhile(Func<TSymbol, bool> condition)
	{
		return ReadWhileLazy(condition).ToList();
	}

	/// <summary>
	/// Lazily reads the tokens while the condition is met.
	/// </summary>
	/// <param name="condition">The condition.</param>
	/// <returns>The set of read tokens.</returns>
	internal IEnumerable<TSymbol> ReadWhileLazy(Func<TSymbol, bool> condition)
	{
		while (EnsureCurrent() && condition(CurrentSymbol!))
		{
			yield return CurrentSymbol!;
			NextToken();
		}
	}

	/// <summary>
	/// Determines if the previous symbol matches the given type.
	/// </summary>
	/// <param name="type">The type.</param>
	/// <returns>True if the previous symbol matches the given type, otherwise false.</returns>
	protected internal bool Was(TSymbolType type)
	{
		return PreviousSymbol != null && Equals(type, PreviousSymbol.Type);
	}
}
