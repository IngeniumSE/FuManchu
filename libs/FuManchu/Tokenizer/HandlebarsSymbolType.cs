// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tokenizer;

/// <summary>
/// Represents the possible symbol types for Handlerbars documents.
/// </summary>
public enum HandlebarsSymbolType
{
	Unknown,        // Unknown symbol
	Identifier,     // An identifier, e.g. 'person'
	Keyword,        // A keyword, e.g. 'if'
	IntegerLiteral, // An integer literal, e.g. 1
	NewLine,        // A new line, e.g. \r, \n, \r\n
	WhiteSpace,     // Whitespace
	RealLiteral,    // A real number literal e.g. 1.5e-10
	StringLiteral,  // A string literal, e.g. 'hello'
	Comment,        // A comment

	Assign,         // =
	Bang,           // !
	Dot,            // .
	Slash,          // /
	Escape,         // \
	RightBracket,   // [
	LeftBracket,    // ]
	RightBrace,     // {
	LeftBrace,      // }
	Tilde,          // ~
	Hash,           // #,
	RightArrow,     // >
	LeftArrow,      // <
	Negate,         // ^
	Ampersand,      // &
	Asterisk,       // *
	Dash,           // -
	At,             // @

	OpenTag,        // {{
	RawOpenTag,     // {{{
	CloseTag,       // }}
	RawCloseTag,    // }}}

	CurrentContext, // ./
	ParentContext,  // ../

	Text            // All other text.
}
