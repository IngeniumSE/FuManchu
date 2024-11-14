// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Tokenizer;

using System.IO;
using System.Text;

using FuManchu.Text;
using FuManchu.Tokenizer;

using Xunit;

/// <summary>
/// Provides a base implementation of a Fact set for testing tokenizers.
/// </summary>
public abstract class TokenizerFactBase
{
	/// <summary>
	/// Gets the symbold that triggers the test to finish testing symbol types.
	/// </summary>
	protected abstract HandlebarsSymbol IgnoreRemaining { get; }

	/// <summary>
	/// Creates the tokenizer.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <returns>The tokenizer instance.</returns>
	protected abstract Tokenizer<HandlebarsSymbol, HandlebarsSymbolType> CreateTokenizer(ITextDocument source);

	/// <summary>
	/// Tests the tokenizer.
	/// </summary>
	/// <param name="input">The input.</param>
	/// <param name="expected">The expected symbols.</param>
	protected void TestTokenizer(string input, params HandlebarsSymbol[] expected)
	{
		bool success = true;
		var output = new StringBuilder();

		using (var reader = new StringReader(input))
		{
			using (var source = new SeekableTextReader(reader))
			{
				var tokenizer = CreateTokenizer(source);
				int counter = 0;
				HandlebarsSymbol? current = null;

				while ((current = tokenizer.NextSymbol()) != null)
				{
					if (counter >= expected.Length)
					{
						output.AppendFormat("F: Expected: << Nothing >>; Actual: {0}\n", current);
						success = false;
					}

					else if (Equals(expected[counter], IgnoreRemaining))
					{
						output.AppendFormat("P: Ignored {0}\n", current);
					}

					else
					{
						if (!Equals(expected[counter], current))
						{
							output.AppendFormat("F: Expected: {0}; Actual: {1}\n", expected[counter], current);
							success = false;
						}

						else
						{
							output.AppendFormat("P: Expected: {0}\n", current);
						}
						counter++;
					}
				}

				if (counter < expected.Length && !Equals(expected[counter], IgnoreRemaining))
				{
					success = false;
					for (; counter < expected.Length; counter++)
					{
						output.AppendFormat("F: Expected: {0}; Actual: << None >>\n", expected[counter]);
					}
				}
			}
		}

		Assert.True(success, "\r\n" + output.ToString());
	}

	/// <summary>
	/// Tests the tokenizer.
	/// </summary>
	/// <param name="input">The input.</param>
	/// <param name="expected">The expected symbols.</param>
	protected void TestTokenizerSymbols(string input, params HandlebarsSymbolType[] expected)
	{
		bool success = true;
		var output = new StringBuilder();

		using (var reader = new StringReader(input))
		{
			using (var source = new SeekableTextReader(reader))
			{
				var tokenizer = CreateTokenizer(source);
				int counter = 0;
				HandlebarsSymbol? current = null;

				while ((current = tokenizer.NextSymbol()) != null)
				{
					if (counter > expected.Length)
					{
						output.AppendFormat("F: Expected: << Nothing >>; Actual: {0}\n", current.Value.Type);
						success = false;
					}

					else if (Equals(current.Value.Type, IgnoreRemaining.Type))
					{
						output.AppendFormat("P: Ignored {0}\n", current);
					}

					else
					{
						if (!Equals(expected[counter], current.Value.Type))
						{
							output.AppendFormat("F: Expected: {0}; Actual: {1}\n", expected[counter], current.Value.Type);
							success = false;
						}

						else
						{
							output.AppendFormat("P: Expected: {0}\n", current.Value.Type);
						}
						counter++;
					}
				}

				if (counter < expected.Length && !Equals(expected[counter], IgnoreRemaining.Type))
				{
					success = false;
					for (; counter < expected.Length; counter++)
					{
						output.AppendFormat("F: Expected: {0}; Actual: << None >>\n", expected[counter]);
					}
				}
			}
		}

		Assert.True(success, "\r\n" + output.ToString());
	}
}
