// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser.SyntaxTree;

/// <summary>
/// The possible kinds of span.
/// </summary>
public enum SpanKind
{
	MetaCode,
	Comment,
	Expression,
	Text,
	WhiteSpace,
	Map,
	Parameter
}
