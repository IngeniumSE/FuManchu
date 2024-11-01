// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Tokenizer
{
	using FuManchu.Tokenizer;

	using Xunit;

	using T = FuManchu.Tokenizer.HandlebarsSymbolType;

	/// <summary>
	/// Provides tag tests for the <see cref="HandlebarsTokenizer"/>
	/// </summary>
	public class HandlebarsTokenizerTagFacts : HandlebarsTokenizerFactBase
	{
		[Fact]
		public void RecognisesTagSymbols()
		{
			TestTokenizerSymbols("{{", T.OpenTag);
			TestTokenizerSymbols("{{{", T.RawOpenTag);
			TestTokenizerSymbols("{{name}}", T.OpenTag, T.Identifier, T.CloseTag);
			TestTokenizerSymbols("{{{name}}}", T.RawOpenTag, T.Identifier, T.RawCloseTag);
		}

		[Fact]
		public void RecognisesTagSwitchCharacters()
		{
			TestTokenizerSymbols("{{#", T.OpenTag, T.Hash);
			TestTokenizerSymbols("{{/", T.OpenTag, T.Slash);
			TestTokenizerSymbols("{{!", T.OpenTag, T.Bang);
			TestTokenizerSymbols("{{>", T.OpenTag, T.RightArrow);
			TestTokenizerSymbols("{{^", T.OpenTag, T.Negate);
			TestTokenizerSymbols("{{&", T.OpenTag, T.Ampersand);
		}

		[Fact]
		public void RecognisesBlockTag()
		{
			TestTokenizerSymbols("{{#if condition}}", T.OpenTag, T.Hash, T.Keyword, T.WhiteSpace, T.Identifier, T.CloseTag);
		}

		[Fact]
		public void RecognisesExpressionTag()
		{
			TestTokenizerSymbols("{{person.name}}", T.OpenTag, T.Identifier, T.Dot, T.Identifier, T.CloseTag);
			TestTokenizerSymbols("{{./name}}", T.OpenTag, T.CurrentContext, T.Identifier, T.CloseTag);
			TestTokenizerSymbols("{{this/name}}", T.OpenTag, T.Keyword, T.Slash, T.Identifier, T.CloseTag);
			TestTokenizerSymbols("{{../name}}", T.OpenTag, T.ParentContext, T.Identifier, T.CloseTag);
		}

		[Fact]
		public void RecognisesArgumentsInTag()
		{
			TestTokenizerSymbols("{{#helper model person}}", T.OpenTag, T.Hash, T.Identifier, T.WhiteSpace, T.Identifier, T.WhiteSpace, T.Identifier, T.CloseTag);
		}

		[Fact]
		public void RecognisesMapsInTag()
		{
			TestTokenizerSymbols("{{#helper one=two three=four}}", T.OpenTag, T.Hash, T.Identifier, T.WhiteSpace, T.Identifier, T.Assign, T.Identifier, T.WhiteSpace, T.Identifier, T.Assign, T.Identifier, T.CloseTag);
		}

		[Fact]
		public void RecognizesInvertedSection()
		{
			TestTokenizerSymbols("{{^people}}", T.OpenTag, T.Negate, T.Identifier, T.CloseTag);
		}

		[Fact]
		public void RecognizesInversionAsElse()
		{
			TestTokenizerSymbols("{{^}}", T.OpenTag, T.Negate, T.CloseTag);
		}

		[Fact]
		public void RecognisesStringsInTag()
		{
			TestTokenizerSymbols(
				"{{#helper class=\"btn btn-primary\"}}",
				T.OpenTag, T.Hash, T.Identifier, T.WhiteSpace,
				T.Identifier, T.Assign, T.StringLiteral,
				T.CloseTag
			);
		}

		[Fact]
		public void RecognisesComplexTag()
		{
			TestTokenizerSymbols(
				"{{#list people class=\"person\"}}",
				T.OpenTag, T.Hash, T.Identifier, T.WhiteSpace,
				T.Identifier, T.WhiteSpace,
				T.Identifier, T.Assign, T.StringLiteral,
				T.CloseTag);
		}

		[Fact]
		public void RecognisesCommentTag()
		{
			TestTokenizer("{{!comment}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "!", T.Bang),
				S(3, 0, 3, "comment", T.Comment),
				S(10, 0, 10, "}}", T.CloseTag)
			);
			TestTokenizer("{{!--comment--}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "!", T.Bang),
				S(3, 0, 3, "--comment--", T.Comment),
				S(14, 0, 14, "}}", T.CloseTag)
			);
		}

		[Fact]
		public void RecognisesMultilineCommentTag()
		{
			TestTokenizer("{{!\n\ncomment text here\n\n}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "!", T.Bang),
				S(3, 0, 3, "\n\ncomment text here\n\n", T.Comment),
				S(24, 4, 0, "}}", T.CloseTag)
			);
		}

		[Fact]
		public void RecognisesEscapedExpressionTagUsingAmpersand()
		{
			TestTokenizer("{{&name}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "&", T.Ampersand),
				S(3, 0, 3, "name", T.Identifier),
				S(7, 0, 7, "}}", T.CloseTag));
		}

		[Fact]
		public void RecognisesIsTag()
		{
			TestTokenizer("{{#is x}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "#", T.Hash),
				S(3, 0, 3, "is", T.Keyword),
				S(5, 0, 5, " ", T.WhiteSpace),
				S(6, 0, 6, "x", T.Identifier),
				S(7, 0, 7, "}}", T.CloseTag));
		}

		[Fact]
		public void RecognisesIsTagWithTwoArguments()
		{
			TestTokenizer("{{#is x y}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "#", T.Hash),
				S(3, 0, 3, "is", T.Keyword),
				S(5, 0, 5, " ", T.WhiteSpace),
				S(6, 0, 6, "x", T.Identifier),
				S(7, 0, 7, " ", T.WhiteSpace),
				S(8, 0, 8, "y", T.Identifier),
				S(9, 0, 9, "}}", T.CloseTag));
		}

		[Fact]
		public void RecognisesIsTagWithTwoArgumentsAndOperator()
		{
			TestTokenizer("{{#is x \"==\" y}}",
				S(0, 0, 0, "{{", T.OpenTag),
				S(2, 0, 2, "#", T.Hash),
				S(3, 0, 3, "is", T.Keyword),
				S(5, 0, 5, " ", T.WhiteSpace),
				S(6, 0, 6, "x", T.Identifier),
				S(7, 0, 7, " ", T.WhiteSpace),
				S(8, 0, 8, "\"==\"", T.StringLiteral),
				S(12, 0, 12, " ", T.WhiteSpace),
				S(13, 0, 13, "y", T.Identifier),
				S(14, 0, 14, "}}", T.CloseTag));
		}
	}
}
