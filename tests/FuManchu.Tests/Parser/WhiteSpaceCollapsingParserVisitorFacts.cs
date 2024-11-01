// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tests.Parser;

using System.Text;

using FuManchu.Parser;
using FuManchu.Parser.SyntaxTree;

using Xunit;

using T = FuManchu.Tokenizer.HandlebarsSymbolType;

/// <summary>
/// Provides tests for the <see cref="WhiteSpaceCollapsingParserVisitor"/>
/// </summary>
public class WhiteSpaceCollapsingParserVisitorFacts
{
	[Fact]
	public void CollapsesPreviousWhiteSpace()
	{
		var factory = new Factory();

		// "Hello {{~test}}"
		var document = factory.Document(
			factory.Text("Hello"),
			factory.WhiteSpace(1),
			factory.Expression(
				factory.MetaCode("{{", T.OpenTag),
				factory.MetaCode("~", T.Tilde),
				factory.Span(SpanKind.Expression, factory.Symbol("this", T.Identifier)),
				factory.MetaCode("}}", T.CloseTag)));

		factory = new Factory();

		// "Hello{{~test}}"
		var expected = factory.Document(
			factory.Text("Hello"),
			factory.WhiteSpace(1, collapsed: true),
			factory.Expression(
				factory.MetaCode("{{", T.OpenTag),
				factory.MetaCode("~", T.Tilde),
				factory.Span(SpanKind.Expression, factory.Symbol("this", T.Identifier)),
				factory.MetaCode("}}", T.CloseTag)));

		var visitor = new WhiteSpaceCollapsingParserVisitor();
		visitor.VisitBlock(document);

		var builder = new StringBuilder();
		var comparer = new EquivalanceComparer(builder, 0);

		Assert.True(comparer.Equals(expected, document), builder.ToString());
	}

	[Fact]
	public void CollapsesNextWhiteSpace()
	{
		var factory = new Factory();

		// "Hello {{test~}} World"
		var document = factory.Document(
			factory.Text("Hello"),
			factory.WhiteSpace(1),
			factory.Expression(
				factory.MetaCode("{{", T.OpenTag),
				factory.Span(SpanKind.Expression, factory.Symbol("this", T.Identifier)),
				factory.MetaCode("~", T.Tilde),
				factory.MetaCode("}}", T.CloseTag)),
			factory.WhiteSpace(1),
			factory.Text("World"));

		factory = new Factory();

		// "Hello {{test~}}World"
		var expected = factory.Document(
			factory.Text("Hello"),
			factory.WhiteSpace(1),
			factory.Expression(
				factory.MetaCode("{{", T.OpenTag),
				factory.Span(SpanKind.Expression, factory.Symbol("this", T.Identifier)),
				factory.MetaCode("~", T.Tilde),
				factory.MetaCode("}}", T.CloseTag)),
			factory.WhiteSpace(1, collapsed: true),
			factory.Text("World"));

		var visitor = new WhiteSpaceCollapsingParserVisitor();
		visitor.VisitBlock(document);

		var builder = new StringBuilder();
		var comparer = new EquivalanceComparer(builder, 0);

		Assert.True(comparer.Equals(expected, document), builder.ToString());
	}

	[Fact]
	public void CollapsesWhiteSpaceInParentBlocks()
	{
		var factory = new Factory();

		// "Message:{{#this~}} Hello {{~/this~}} There"
		var document = factory.Document(
			factory.Text("Message:"),
			factory.Tag("this",
				factory.TagElement("this",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Parameter("this")),
					factory.MetaCode("~", T.Tilde),
					factory.MetaCode("}}", T.CloseTag)),
				factory.WhiteSpace(1),
				factory.Text("Hello"),
				factory.WhiteSpace(1),
					factory.TagElement("this",
						factory.MetaCode("{{", T.OpenTag),
						factory.MetaCode("~", T.Tilde),
						factory.MetaCode("/", T.Slash),
						factory.Expression(factory.Parameter("this")),
						factory.MetaCode("~", T.Tilde),
						factory.MetaCode("}}", T.CloseTag))),
			factory.WhiteSpace(1),
			factory.Text("There"));

		factory = new Factory();

		// "Message:{{#this~}}Hello{{~/this~}}There"
		var expected = factory.Document(
			factory.Text("Message:"),
			factory.Tag("this",
				factory.TagElement("this",
					factory.MetaCode("{{", T.OpenTag),
					factory.MetaCode("#", T.Hash),
					factory.Expression(factory.Parameter("this")),
					factory.MetaCode("~", T.Tilde),
					factory.MetaCode("}}", T.CloseTag)),
				factory.WhiteSpace(1, collapsed: true),
				factory.Text("Hello"),
				factory.WhiteSpace(1, collapsed: true),
					factory.TagElement("this",
						factory.MetaCode("{{", T.OpenTag),
						factory.MetaCode("~", T.Tilde),
						factory.MetaCode("/", T.Slash),
						factory.Expression(factory.Parameter("this")),
						factory.MetaCode("~", T.Tilde),
						factory.MetaCode("}}", T.CloseTag))),
			factory.WhiteSpace(1, collapsed: true),
			factory.Text("There"));

		var visitor = new WhiteSpaceCollapsingParserVisitor();
		visitor.VisitBlock(document);

		var builder = new StringBuilder();
		var comparer = new EquivalanceComparer(builder, 0);

		Assert.True(comparer.Equals(expected, document), builder.ToString());
	}
}
