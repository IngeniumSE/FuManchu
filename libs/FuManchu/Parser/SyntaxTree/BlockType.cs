// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser.SyntaxTree;

/// <summary>
/// Represents the possible block types.
/// </summary>
public enum BlockType
{
	Document,
	Text,
	Comment,
	Tag,
	TagElement,
	Expression,
	Partial,
	PartialBlock,
	PartialBlockElement,
	PartialBlockContent,
	PartialBlockContentElement,
	Zone
}
