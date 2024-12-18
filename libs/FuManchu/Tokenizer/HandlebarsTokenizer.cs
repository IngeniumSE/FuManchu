﻿// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tokenizer;

using System;
using System.Collections.Generic;
using System.Diagnostics;

using FuManchu.Parser;
using FuManchu.Text;

/// <summary>
/// Provides tokenizer services for Handlebars syntax.
/// </summary>
public class HandlebarsTokenizer : Tokenizer<HandlebarsSymbol, HandlebarsSymbolType>
{
	const int StopState = -1;

	public enum HandlebarsTokenizerStates
	{
		Data = 0,
		WhiteSpace,
		BeginTag,
		BeginTagContent,
		BeginTagContentRaw,
		ContinueTagContent,
		ContinueTagContentRaw,
		EndTag,
		EndTagRaw,
		BeginComment,
		ContinueComment,
		ContinueCommentExplicitTerminal
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="HandlebarsTokenizer"/> class.
	/// </summary>
	/// <param name="source">The source.</param>
	public HandlebarsTokenizer(ITextDocument source)
		: base(source)
	{
		base.CurrentState = StartState;
	}

	/// <inheritdoc />
	protected override int StartState { get; } = (int)HandlebarsTokenizerStates.Data;

	new HandlebarsTokenizerStates? CurrentState => (HandlebarsTokenizerStates?)base.CurrentState;

	protected override StateResult Dispatch()
	{
		if (base.CurrentState == StopState)
		{
			return Stop();
		}

		switch (CurrentState)
		{
			case HandlebarsTokenizerStates.Data: return Data();
			case HandlebarsTokenizerStates.WhiteSpace: return WhiteSpace();
			case HandlebarsTokenizerStates.BeginTag: return BeginTag();
			case HandlebarsTokenizerStates.BeginTagContent: return BeginTagContent(false);
			case HandlebarsTokenizerStates.BeginTagContentRaw: return BeginTagContent(true);
			case HandlebarsTokenizerStates.ContinueTagContent: return ContinueTagContent(false);
			case HandlebarsTokenizerStates.ContinueTagContentRaw: return ContinueTagContent(true);
			case HandlebarsTokenizerStates.EndTag: return EndTag(false);
			case HandlebarsTokenizerStates.EndTagRaw: return EndTag(true);
			case HandlebarsTokenizerStates.BeginComment: return BeginComment();
			case HandlebarsTokenizerStates.ContinueComment: return ContinueComment(false);
			case HandlebarsTokenizerStates.ContinueCommentExplicitTerminal: return ContinueComment(true);
			default:
				Debug.Fail("Invalid tokenizer state.");
				return default;
		}
	}

	#region Tag Structure
	/// <summary>
	/// Attempts to tokenize a comment.
	/// </summary>
	/// <returns>The state result.</returns>
	private StateResult BeginComment()
	{
		if (CurrentCharacter == '-' && Peek() == '-')
		{
			return Transition((int)HandlebarsTokenizerStates.ContinueCommentExplicitTerminal);
		}

		return Transition((int)HandlebarsTokenizerStates.ContinueComment);
	}

	/// <summary>
	/// Attempts to begin a tag by matching the opening braces.
	/// </summary>
	/// <returns>The state result.</returns>
	private StateResult BeginTag()
	{
		var type = HandlebarsSymbolType.OpenTag;

		if (CurrentCharacter != '{')
		{
			CurrentErrors.Add(new Error("Expected '{'", CurrentLocation));

			// We can't process this any more, so stop.
			return Transition(StopState);
		}

		TakeCurrent();

		if (CurrentCharacter != '{')
		{
			CurrentErrors.Add(new Error("Expected '{'", CurrentLocation));

			// We can't process this any more, so stop.
			return Transition(StopState);
		}

		TakeCurrent();

		if (CurrentCharacter == '{')
		{
			// We're at a raw tag '{{{'
			TakeCurrent();
			// Change the symbol type.
			type = HandlebarsSymbolType.RawOpenTag;
		}
		else if (CurrentCharacter == '#')
		{
			// We're at the start of a block tag.
			return Transition(EndSymbol(type), (int)HandlebarsTokenizerStates.BeginTagContent);
		}

		// Transition to the start of tag content.
		return Transition(EndSymbol(type), type == HandlebarsSymbolType.RawOpenTag
			? (int)HandlebarsTokenizerStates.BeginTagContentRaw
			: (int)HandlebarsTokenizerStates.BeginTagContent);
	}

	/// <summary>
	/// Attempts to begin matching the content of a tag.
	/// </summary>
	/// <param name="raw">True if we are expected a raw tag.</param>
	/// <returns>The state result.</returns>
	private StateResult BeginTagContent(bool raw)
	{
		switch (CurrentCharacter)
		{
			case '~':
				{
					TakeCurrent();
					// We've matched a ~ character - this is for ensuring the tag braces are expanded as whitespace instead of being collapsed.
					return Stay(EndSymbol(HandlebarsSymbolType.Tilde));
				}
			case '!':
				{
					if (raw)
					{
						// This is an invalid tag, so set and error and exit.
						CurrentErrors.Add(new Error("Unexpected '!' in raw tag.", CurrentLocation));
						return Transition(StopState);
					}
					TakeCurrent();
					// We've matched a ! character - this is the start of a comment.
					return Transition(EndSymbol(HandlebarsSymbolType.Bang), (int)HandlebarsTokenizerStates.BeginComment);
				}
			case '>':
				{
					if (raw)
					{
						// This is an invalid tag, so set and error and exit.
						CurrentErrors.Add(new Error("Unexpected '>' in raw tag.", CurrentLocation));
						return Transition(StopState);
					}
					TakeCurrent();
					// We've matched a > character - this is the start of a reference to a partial template.
					return Transition(EndSymbol(HandlebarsSymbolType.RightArrow), (int)HandlebarsTokenizerStates.ContinueTagContent);
				}
			case '<':
				{
					if (raw)
					{
						// This is an invalid tag, so set and error and exit.
						CurrentErrors.Add(new Error("Unexpected '<' in raw tag.", CurrentLocation));
						return Transition(StopState);
					}
					TakeCurrent();
					// We've matched a > character - this is the start of a reference to a partial template.
					return Transition(EndSymbol(HandlebarsSymbolType.LeftArrow), (int)HandlebarsTokenizerStates.ContinueTagContent);
				}
			case '^':
				{
					if (raw)
					{
						// This is an invalid tag, so set and error and exit.
						CurrentErrors.Add(new Error("Unexpected '^' in raw tag.", CurrentLocation));
						return Transition(StopState);
					}
					TakeCurrent();
					// We've matched a ^ character - this is the start of a negation.
					return Transition(EndSymbol(HandlebarsSymbolType.Negate), (int)HandlebarsTokenizerStates.ContinueTagContent);
				}
			case '#':
				{
					if (raw)
					{
						// This is an invalid tag, so set and error and exit.
						CurrentErrors.Add(new Error("Unexpected '#' in raw tag.", CurrentLocation));
						return Transition(StopState);
					}
					TakeCurrent();
					if (CurrentCharacter == '>')
					{
						// We've matched a > character - this is the start of a partial block tag
						return Transition(EndSymbol(HandlebarsSymbolType.Hash), (int)HandlebarsTokenizerStates.BeginTagContent);
					}
					// We've matched a # character - this is the start of a block tag
					return Transition(EndSymbol(HandlebarsSymbolType.Hash), (int)HandlebarsTokenizerStates.ContinueTagContent);
				}
			case '&':
				{
					TakeCurrent();
					// We've matched a & character
					return Transition(EndSymbol(HandlebarsSymbolType.Ampersand), (int)HandlebarsTokenizerStates.ContinueTagContent);
				}
			case '@':
				{
					TakeCurrent();
					// We've matched a variable reference character.
					return Transition(EndSymbol(HandlebarsSymbolType.At), (int)HandlebarsTokenizerStates.ContinueTagContent);
				}
			default:
				{
					// Transition to the tag content.
					return Transition(raw ? (int)HandlebarsTokenizerStates.ContinueTagContentRaw : (int)HandlebarsTokenizerStates.ContinueTagContent);
				}
		}
	}

	/// <summary>
	/// Tokenizes a comment.
	/// </summary>
	/// <param name="explicitTerminal">True if we should be expecting a terminal '--' squence to end the comment.</param>
	/// <returns>The state result.</returns>
	private StateResult ContinueComment(bool explicitTerminal)
	{
		if (explicitTerminal)
		{
			TakeUntil(c => c == '-' && Peek() == '-');
			TakeCurrent();
			TakeCurrent();
			if (CurrentCharacter == '}' && Peek() == '}')
			{
				// We've finished at an explicit -- sequence.
				return Transition(EndSymbol(HandlebarsSymbolType.Comment), (int)HandlebarsTokenizerStates.EndTag);
			}

			// Stay at the current state.
			return Stay();
		}

		TakeUntil(c => c == '}' && Peek() == '}');
		return Transition(EndSymbol(HandlebarsSymbolType.Comment), (int)HandlebarsTokenizerStates.EndTag);
	}

	/// <summary>
	/// Continues the content of the tag.
	/// </summary>
	/// <param name="raw">True if we are expected a raw tag.</param>
	/// <returns>The state result.</returns>
	private StateResult ContinueTagContent(bool raw)
	{
		if (CurrentCharacter == '@')
		{
			TakeCurrent();

			return Stay(EndSymbol(HandlebarsSymbolType.At));
		}

		if (HandlebarsHelpers.IsIdentifierStart(CurrentCharacter))
		{
			return Identifier();
		}

		if (Char.IsDigit(CurrentCharacter))
		{
			return NumericLiteral();
		}

		switch (CurrentCharacter)
		{
			case '.':
				{
					TakeCurrent();
					if (CurrentCharacter == '/')
					{
						// We've matched a link to the current context.
						TakeCurrent();
						return Stay(EndSymbol(HandlebarsSymbolType.CurrentContext));
					}

					if (CurrentCharacter == '.' && Peek() == '/')
					{
						// We've matched a link to the parent context.
						TakeCurrent();
						TakeCurrent();
						return Stay(EndSymbol(HandlebarsSymbolType.ParentContext));
					}

					// We've matched a dot, which could be part of an expression.
					return Stay(EndSymbol(HandlebarsSymbolType.Dot));
				}
			case '/':
				{
					TakeCurrent();
					// We've matched a forward-slash, which could be part of an expression.
					return Stay(EndSymbol(HandlebarsSymbolType.Slash));
				}
			case ' ':
				{
					// Take all the available whitespace.
					TakeUntil(c => !ParserHelpers.IsWhiteSpace(c));
					return Stay(EndSymbol(HandlebarsSymbolType.WhiteSpace));
				}
			case '~':
				{
					TakeCurrent();
					// We've reached a '~' character, so jump to the end of the tag.
					return Transition(EndSymbol(HandlebarsSymbolType.Tilde), raw ? (int)HandlebarsTokenizerStates.EndTagRaw : (int)HandlebarsTokenizerStates.EndTag);
				}
			case '"':
			case '\'':
				{
					var quote = CurrentCharacter;
					TakeCurrent();
					// We've reached a quoted literal.
					return QuotedLiteral(quote);
				}
			case '=':
				{
					// We're reached a map assignment.
					TakeCurrent();
					return Stay(EndSymbol(HandlebarsSymbolType.Assign));
				}
			case '}':
				{
					// We've reached a closing tag, so transition away.
					return Transition(raw ? (int)HandlebarsTokenizerStates.EndTagRaw : (int)HandlebarsTokenizerStates.EndTag);
				}
			case '-':
				{
					// This could be a special case, like 'partial-block'.
					TakeCurrent();

					return Stay(EndSymbol(HandlebarsSymbolType.Dash));
				}
			default:
				{
					CurrentErrors.Add(new Error("Unexpected character: " + CurrentCharacter, CurrentLocation));
					return Transition(StopState);
				}
		}
	}

	/// <summary>
	/// Attempts to end a tag by matching the closing braces.
	/// </summary>
	/// <param name="raw">True if we are expected to end a raw tag '}}}'</param>
	/// <returns>The state result.</returns>
	private StateResult EndTag(bool raw)
	{
		if (CurrentCharacter != '}')
		{
			CurrentErrors.Add(new Error("Expected '}'", CurrentLocation));

			// We can't process this any more, so stop.
			return Transition(StopState);
		}

		TakeCurrent();

		if (CurrentCharacter != '}')
		{
			CurrentErrors.Add(new Error("Expected '}'", CurrentLocation));

			// We can't process this any more, so stop.
			return Transition(StopState);
		}

		TakeCurrent();

		if (CurrentCharacter != '}' && raw)
		{
			CurrentErrors.Add(new Error("Expected '}'", CurrentLocation));

			// We can't process this any more, so stop.
			return Transition(StopState);
		}

		if (CurrentCharacter == '}' && raw)
		{
			TakeCurrent();
			// We're done processing this '}}}' sequence, so let's finish here and return to the Data state.
			return Transition(EndSymbol(HandlebarsSymbolType.RawCloseTag), (int)HandlebarsTokenizerStates.Data);
		}

		// We're done processing this '}}' sequence, so let's finish here and return to the Data state.
		return Transition(EndSymbol(HandlebarsSymbolType.CloseTag), (int)HandlebarsTokenizerStates.Data);
	}
	#endregion

	/// <inheritdoc />
	public override HandlebarsSymbol CreateSymbol(SourceLocation start, string content, HandlebarsSymbolType type, IEnumerable<Error> errors)
	{
		return new HandlebarsSymbol(start, content, type, errors);
	}

	/// <summary>
	/// Represents the default state of the tokenizer.
	/// </summary>
	/// <returns>The state result.</returns>
	private StateResult Data()
	{
		if (EndOfFile)
		{
			if (HaveContent)
			{
				return Transition(EndSymbol(HandlebarsSymbolType.Text), StopState);
			}

			return Stop();
		}

		if (ParserHelpers.IsWhiteSpace(CurrentCharacter) || ParserHelpers.IsNewLine(CurrentCharacter))
		{
			if (HaveContent)
			{
				return Transition(EndSymbol(HandlebarsSymbolType.Text), (int)HandlebarsTokenizerStates.WhiteSpace);
			}

			return Transition((int)HandlebarsTokenizerStates.WhiteSpace);
		}

		TakeUntil(c => c == '{' || ParserHelpers.IsWhiteSpace(c));

		if (ParserHelpers.IsWhiteSpace(CurrentCharacter))
		{
			return Stay();
		}

		if (HaveContent && CurrentCharacter == '{')
		{
			if (Buffer[Buffer.Length - 1] == '\\')
			{
				// The { character is being escaped, so move on.
				TakeCurrent();
				return Stay();
			}

			if (Peek() == '{')
			{
				// We're at the start of a tag.
				return Transition(EndSymbol(HandlebarsSymbolType.Text), (int)HandlebarsTokenizerStates.BeginTag);
			}
		}
		if (Peek() == '{')
		{
			// We're at the start of a tag.
			return Transition((int)HandlebarsTokenizerStates.BeginTag);
		}

		TakeCurrent();

		return Stay();
	}

	/// <summary>
	/// Tokenizes a decimal literal.
	/// </summary>
	/// <returns>The state result</returns>
	private StateResult DecimalLiteral()
	{
		TakeUntil(c => !Char.IsDigit(c));

		if (CurrentCharacter == '.' && Char.IsDigit(Peek()))
		{
			return RealLiteral();
		}

		if (CurrentCharacter == 'E' || CurrentCharacter == 'e')
		{
			return RealLiteralExponantPart();
		}

		return Stay(EndSymbol(HandlebarsSymbolType.IntegerLiteral));
	}

	/// <summary>
	/// Tokenizes a hex literal.
	/// </summary>
	/// <returns>The state result.</returns>
	private StateResult HexLiteral()
	{
		TakeUntil(c => !ParserHelpers.IsHexDigit(c));

		return Stay(EndSymbol(HandlebarsSymbolType.IntegerLiteral));
	}

	/// <summary>
	/// Tokenizes an identifier.
	/// </summary>
	/// <returns>The state result</returns>
	private StateResult Identifier()
	{
		// We assume the first character has been considered.
		TakeCurrent();
		// Take all characters that are considered identifiers.
		TakeUntil(c => !HandlebarsHelpers.IsIdentifierPart(c));

		HandlebarsSymbol? symbol = null;
		if (HaveContent)
		{
			var keyword = HandlebarsKeywordDetector.SymbolTypeForIdentifier(Buffer.ToString());
			var type = HandlebarsSymbolType.Identifier;
			if (keyword != null)
			{
				type = HandlebarsSymbolType.Keyword;
			}
			symbol = new HandlebarsSymbol(CurrentStart, Buffer.ToString(), type) { Keyword = keyword };
		}
		// Start a new symbol.
		StartSymbol();

		return Stay(symbol);
	}

	/// <summary>
	/// Tokenizes a numeric litera.
	/// </summary>
	/// <returns>The state result</returns>
	private StateResult NumericLiteral()
	{
		if (TakeAll("0x", true))
		{
			return HexLiteral();
		}

		return DecimalLiteral();
	}

	/// <summary>
	/// Tokenizes a quoted literal.
	/// </summary>
	/// <param name="quote">The quote character.</param>
	/// <returns>The state result.</returns>
	private StateResult QuotedLiteral(char quote)
	{
		TakeUntil(c => c == '\\' || c == quote || ParserHelpers.IsNewLine(c));
		if (CurrentCharacter == '\\')
		{
			TakeCurrent();

			if (CurrentCharacter == quote || CurrentCharacter == '\\')
			{
				TakeCurrent();
			}

			return Stay();
		}

		if (EndOfFile || ParserHelpers.IsNewLine(CurrentCharacter))
		{
			CurrentErrors.Add(new Error("Untermined string literal", CurrentStart));
		}
		else
		{
			TakeCurrent();
		}

		return Stay(EndSymbol(HandlebarsSymbolType.StringLiteral));
	}

	/// <summary>
	/// Tokenizes a real literal.
	/// </summary>
	/// <returns>The state result</returns>
	private StateResult RealLiteral()
	{
		TakeCurrent();
		TakeUntil(c => !Char.IsDigit(c));

		return RealLiteralExponantPart();
	}

	/// <summary>
	/// Tokenizes the exponent part of a real literal.
	/// </summary>
	/// <returns>The state result.</returns>
	private StateResult RealLiteralExponantPart()
	{
		if (CurrentCharacter == 'e' || CurrentCharacter == 'E')
		{
			TakeCurrent();
			if (CurrentCharacter == '+' || CurrentCharacter == '-')
			{
				TakeCurrent();
			}
			TakeUntil(c => !Char.IsDigit(c));
		}

		return Stay(EndSymbol(HandlebarsSymbolType.RealLiteral));
	}

	/// <summary>
	/// Tokenizes a block of whitespace.
	/// </summary>
	/// <returns>The state result.</returns>
	private StateResult WhiteSpace()
	{
		TakeUntil(c => !ParserHelpers.IsWhiteSpace(c) && !ParserHelpers.IsNewLine(c));
		if (HaveContent)
		{
			return Transition(EndSymbol(HandlebarsSymbolType.WhiteSpace), (int)HandlebarsTokenizerStates.Data);
		}

		return Transition((int)HandlebarsTokenizerStates.Data);
	}
}
