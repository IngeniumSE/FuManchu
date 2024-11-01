// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Tokenizer
{
	using System;

	using FuManchu.Tokenizer;

	using Xunit;

	/// <summary>
	/// Provides tests for the <see cref="HandlebarsTokenizer"/> type.
	/// </summary>
	public class HandlebarsTokenizerFacts : HandlebarsTokenizerFactBase
	{
		[Fact]
		public void ConstructorThrowsArgumentNullExceptionWhenNoSourceProvided()
		{
			Assert.Throws<ArgumentNullException>(() => new HandlebarsTokenizer(null!));
		}

		[Fact]
		public void NextReturnsNullWhenEOFReached()
		{
			TestTokenizer(string.Empty);
		}

		[Fact]
		public void NextReturnsTokenForWhitespaceCharacters()
		{
			TestTokenizer(" \f\t\u000B \n ", new HandlebarsSymbol(0, 0, 0, " \f\t\u000B \n ", HandlebarsSymbolType.WhiteSpace));
		}
	}
}
