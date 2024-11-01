// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Parser;

using System.IO;
using System.Text;

using FuManchu.Parser;
using FuManchu.Parser.SyntaxTree;
using FuManchu.Tags;
using FuManchu.Text;
using FuManchu.Tokenizer;

using Xunit;

using T = FuManchu.Tokenizer.HandlebarsSymbolType;

public class HandlebarsParserFacts
{
	[Fact]
	public void CanParseTextDocument()
	{
		var factory = new Factory();

		ParserTest("<h1>Hello World</h1>", factory.Document(
			factory.Text("<h1>Hello"),
			factory.WhiteSpace(1),
			factory.Text("World</h1>")));
	}

	[Fact]
	public void CanParseExpressionTag()
	{
		var factory = new Factory();

		ParserTest("{{hello}}", factory.Document(
			factory.Expression(
				factory.MetaCode("{{", T.OpenTag),
				factory.Expression(
					factory.Symbol("hello", T.Identifier)
				),
				factory.MetaCode("}}", T.CloseTag)
			)
		));
	}

	[Fact]
	public void CanParseBlockTag()
	{
		var factory = new Factory();

		ParserTest("{{#if condition}}True!{{/if}}", factory.Document(
			factory.Tag("if",
				factory.TagElement("if",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Symbol("if", T.Keyword)),
					factory.WhiteSpace(1),
					factory.Parameter("condition"),
					factory.MetaCode("}}", T.CloseTag)
				),
				factory.Text("True!"),
				factory.TagElement("if",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("/", T.Hash),
					factory.Expression(factory.Symbol("if", T.Keyword)),
					factory.MetaCode("}}", T.CloseTag)
				)
			)
		));
	}

	[Fact]
	public void CanParseIfElseTag()
	{
		var factory = new Factory();

		ParserTest("{{#if condition}}True!{{else}}False!{{/if}}", factory.Document(
			factory.Tag("if",
				factory.TagElement("if",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Symbol("if", T.Keyword)),
					factory.WhiteSpace(1),
					factory.Parameter("condition"),
					factory.MetaCode("}}", T.CloseTag)
				),
				factory.Text("True!"),
				factory.TagElement("else",
					factory.MetaCode("{{", T.OpenTag),
					factory.Expression(factory.Symbol("else", T.Keyword)),
					factory.MetaCode("}}", T.CloseTag)
				),
				factory.Text("False!"),
				factory.TagElement("if",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("/", T.Hash),
					factory.Expression(factory.Symbol("if", T.Keyword)),
					factory.MetaCode("}}", T.CloseTag)
				)
			)
		));
	}

	[Fact]
	public void CanParseIfElseTagUsingNegation()
	{
		var factory = new Factory();

		ParserTest("{{#if condition}}True!{{^}}False!{{/if}}", factory.Document(
			factory.Tag("if",
				factory.TagElement("if",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Symbol("if", T.Keyword)),
					factory.WhiteSpace(1),
					factory.Parameter("condition"),
					factory.MetaCode("}}", T.CloseTag)
				),
				factory.Text("True!"),
				factory.TagElement("^",
					factory.MetaCode("{{", T.OpenTag),
					factory.Expression(factory.Symbol("^", T.Negate)),
					factory.MetaCode("}}", T.CloseTag)
				),
				factory.Text("False!"),
				factory.TagElement("if",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("/", T.Hash),
					factory.Expression(factory.Symbol("if", T.Keyword)),
					factory.MetaCode("}}", T.CloseTag)
				)
			)
		));
	}

	[Fact]
	public void CanParseIfElseIfTag()
	{
		var factory = new Factory();

		ParserTest("{{#if condition}}True!{{#elseif what}}Maybe?{{else}}False!{{/if}}", factory.Document(
			factory.Tag("if",
				factory.TagElement("if",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Symbol("if", T.Keyword)),
					factory.WhiteSpace(1),
					factory.Parameter("condition"),
					factory.MetaCode("}}", T.CloseTag)
				),
				factory.Text("True!"),
				factory.TagElement("elseif",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Symbol("elseif", T.Keyword)),
					factory.WhiteSpace(1),
					factory.Parameter("what"),
					factory.MetaCode("}}", T.CloseTag)
				),
				factory.Text("Maybe?"),
				factory.TagElement("else",
					factory.MetaCode("{{", T.OpenTag),
					factory.Expression(factory.Symbol("else", T.Keyword)),
					factory.MetaCode("}}", T.CloseTag)
				),
				factory.Text("False!"),
				factory.TagElement("if",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("/", T.Hash),
					factory.Expression(factory.Symbol("if", T.Keyword)),
					factory.MetaCode("}}", T.CloseTag)
				)
			)
		));
	}

	[Fact]
	public void CanParseMappedParameters()
	{
		var factory = new Factory();

		ParserTest("{{#list people class=\"value\"}}", factory.Document(
			factory.Tag("list",
				factory.TagElement("list",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Symbol("list", T.Identifier)),
					factory.WhiteSpace(1),
					factory.Parameter("people"),
					factory.WhiteSpace(1),
					factory.Map("class", "\"value\"", T.StringLiteral),
					factory.MetaCode("}}", T.CloseTag)
				)
			)
		));
	}

	[Fact]
	public void CanParseMultipleMappedParameters()
	{
		var factory = new Factory();

		ParserTest("{{#list people class=\"value\" age=10.5}}", factory.Document(
			factory.Tag("list",
				factory.TagElement("list",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Symbol("list", T.Identifier)),
					factory.WhiteSpace(1),
					factory.Parameter("people"),
					factory.WhiteSpace(1),
					factory.Map("class", "\"value\"", T.StringLiteral),
					factory.WhiteSpace(1),
					factory.Map("age", "10.5", T.RealLiteral),
					factory.MetaCode("}}", T.CloseTag)
				)
			)
		));
	}

	[Fact]
	public void CanParseMultipleMappedParametersWithExpressions()
	{
		var factory = new Factory();

		var expected = factory.Document(
			factory.Tag("list",
				factory.TagElement("list",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Symbol("list", T.Identifier)),
					factory.WhiteSpace(1),
					factory.Parameter(
						factory.Symbol("model", T.Identifier),
						factory.Symbol(".", T.Dot),
						factory.Symbol("people", T.Identifier)
						),
					factory.WhiteSpace(1),
					factory.Map("class",
						() => new ISymbol[]
									{
										factory.Symbol("model", T.Identifier),
										factory.Symbol(".", T.Dot),
										factory.Symbol("cssClass", T.Identifier)
									}),
					factory.WhiteSpace(1),
					factory.Map("age",
						() => new ISymbol[]
									{
										factory.Symbol(".", T.Dot),
										factory.Symbol("/", T.Slash),
										factory.Symbol("model", T.Identifier),
										factory.Symbol(".", T.Dot),
										factory.Symbol("ages", T.Identifier)
									}
						),
					factory.MetaCode("}}", T.CloseTag)
					)
				)
			);

		ParserTest("{{#list model.people class=model.cssClass age=./model.ages}}", expected);
	}

	[Fact]
	public void CanParsePartialTag()
	{
		var factory = new Factory();

		ParserTest("{{>body}}", factory.Document(
			factory.Partial(
				factory.MetaCode("{{", T.OpenTag),
				factory.MetaCode(">", T.RightArrow),
				factory.Span(SpanKind.Expression, factory.Symbol("body", T.Identifier)),
				factory.MetaCode("}}", T.CloseTag))
			));
	}

	[Fact]
	public void CanParseRootVariable()
	{
		var factory = new Factory();

		ParserTest("{{@root.name}}", factory.Document(
			factory.Expression(
				factory.MetaCode("{{", T.OpenTag),
				factory.Span(SpanKind.Expression,
					factory.Symbol("@", T.At),
					factory.Symbol("root", T.Identifier),
					factory.Symbol(".", T.Dot),
					factory.Symbol("name", T.Identifier)
				),
				factory.MetaCode("}}", T.CloseTag))
			));
	}

	[Fact]
	public void CanParseEscapedExpressionUsingAmpersand()
	{
		var factory = new Factory();

		ParserTest("{{&person.name}}", factory.Document(
			factory.Expression(
				factory.MetaCode("{{", T.OpenTag),
				factory.MetaCode("&", T.Ampersand),
				factory.Span(SpanKind.Expression,
					factory.Symbol("person", T.Identifier),
					factory.Symbol(".", T.Dot),
					factory.Symbol("name", T.Identifier)
				),
				factory.MetaCode("}}", T.CloseTag))
			));
	}

	[Fact]
	public void CanParseNegatedSection()
	{
		var factory = new Factory();

		var document = factory.Document(
		factory.Tag("person",
			factory.TagElement("person",
				factory.MetaCode("{{", T.OpenTag),
				factory.MetaCode("^", T.Negate),
				factory.Expression(factory.Symbol("person", T.Identifier)),
				factory.MetaCode("}}", T.CloseTag)),
			factory.Text("Text"),
			factory.TagElement("person",
				factory.MetaCode("{{", T.OpenTag),
				factory.MetaCode("/", T.Slash),
				factory.Expression(factory.Symbol("person", T.Identifier)),
				factory.MetaCode("}}", T.CloseTag))));

		ParserTest("{{^person}}Text{{/person}}", document);
	}

	[Fact]
	public void CanParseIsTag_WithSingleArgument()
	{
		var factory = new Factory();

		var document = factory.Document(
			factory.Tag("is",
				factory.TagElement("is",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Symbol("is", T.Keyword)),
					factory.WhiteSpace(1),
					factory.Parameter(factory.Symbol("x", T.Identifier)),
					factory.MetaCode("}}", T.CloseTag)),
				factory.Text("True"),
				factory.TagElement("is",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("/", T.Slash),
					factory.Expression(factory.Symbol("is", T.Keyword)),
					factory.MetaCode("}}", T.CloseTag))));

		ParserTest("{{#is x}}True{{/is}}", document);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArguments()
	{
		var factory = new Factory();

		var document = factory.Document(
			factory.Tag("is",
				factory.TagElement("is",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Symbol("is", T.Keyword)),
					factory.WhiteSpace(1),
					factory.Parameter(factory.Symbol("x", T.Identifier)),
					factory.WhiteSpace(1),
					factory.Parameter(factory.Symbol("y", T.Identifier)),
					factory.MetaCode("}}", T.CloseTag)),
				factory.Text("True"),
				factory.TagElement("is",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("/", T.Slash),
					factory.Expression(factory.Symbol("is", T.Keyword)),
					factory.MetaCode("}}", T.CloseTag))));

		ParserTest("{{#is x y}}True{{/is}}", document);
	}

	[Fact]
	public void CanParseIsTag_WithTwoArgumentsAndOperator()
	{
		var factory = new Factory();

		var document = factory.Document(
			factory.Tag("is",
				factory.TagElement("is",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Symbol("is", T.Keyword)),
					factory.WhiteSpace(1),
					factory.Parameter(factory.Symbol("x", T.Identifier)),
					factory.WhiteSpace(1),
					factory.Parameter(factory.Symbol("\"==\"", T.StringLiteral)),
					factory.WhiteSpace(1),
					factory.Parameter(factory.Symbol("y", T.Identifier)),
					factory.MetaCode("}}", T.CloseTag)),
				factory.Text("True"),
				factory.TagElement("is",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("/", T.Slash),
					factory.Expression(factory.Symbol("is", T.Keyword)),
					factory.MetaCode("}}", T.CloseTag))));

		ParserTest("{{#is x \"==\" y}}True{{/is}}", document);
	}

	private void ParserTest(string content, Block document, TagProvidersCollection? providers = null)
	{
		providers = providers ?? TagProvidersCollection.Default;

		var output = new StringBuilder();
		using (var reader = new StringReader(content))
		{
			using (var source = new SeekableTextReader(reader))
			{
				var errors = new ParserErrorSink();
				var parser = new HandlebarsParser();

				var context = new ParserContext(source, parser, errors, providers);
				parser.Context = context;

				parser.ParseDocument();

				var results = context.CompleteParse();

				var comparer = new EquivalanceComparer(output, 0);

				Assert.True(comparer.Equals(document, results.Document), output.ToString());
			}
		}
	}
}
