// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using System;
using System.Linq;

using FuManchu.Parser.SyntaxTree;
using FuManchu.Tags;
using FuManchu.Tokenizer;

/// <summary>
/// Provides parsing services for the Handlebars language.
/// </summary>
public class HandlebarsParser : TokenizerBackedParser
{
	/// <inheritdoc />
	protected override LanguageCharacteristics<HandlebarsTokenizer, HandlebarsSymbol, HandlebarsSymbolType> Language
	{
		get { return HandlebarsLanguageCharacteristics.Instance; }
	}

	/// <summary>
	/// Parses a block tag.
	/// </summary>
	public void AtBlockTag(HandlebarsSymbolType expectedPrefixSymbol = HandlebarsSymbolType.Hash)
	{
		var parent = Context!.CurrentBlock;

		string? tagName = null;
		TagDescriptor? descriptor = null;
		// Start a new block.

		Context.StartBlock(BlockType.Tag);

		using (Context.StartBlock(BlockType.TagElement))
		{
			// Accept the open tag.
			AcceptAndMoveNext();
			// Output that tag as metacode.
			Output(SpanKind.MetaCode);

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			if (Required(expectedPrefixSymbol, true))
			{
				// Accept the prefix symbol type. tag.
				AcceptAndMoveNext();
				// Output that tag as metacode.
				Output(SpanKind.MetaCode);
			}

			if (Optional(HandlebarsSymbolType.RightArrow))
			{
				Context!.CurrentBlock.IsPartialBlock = true;

				// Output that tag as metacode.
				Output(SpanKind.MetaCode);
			}

			// Accept everything until either the close of the tag, or the first element of whitespace.
			AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag);
			// Output the first part as an expression.
			Output(SpanKind.Expression);

			// Get the tag name and set it for the block.
			tagName = LastSpanContent();
			descriptor = Context.TagProviders.GetDescriptor(tagName!, expectedPrefixSymbol == HandlebarsSymbolType.Negate);

			Context.CurrentBlock.Name = tagName;
			Context.CurrentBlock.Descriptor = descriptor;

			while (CurrentSymbol!.Value.Type != HandlebarsSymbolType.CloseTag && CurrentSymbol.Value.Type != HandlebarsSymbolType.RawCloseTag && CurrentSymbol.Value.Type != HandlebarsSymbolType.Tilde)
			{
				if (CurrentSymbol.Value.Type == HandlebarsSymbolType.WhiteSpace)
				{
					// Accept all the whitespace.
					AcceptAll(HandlebarsSymbolType.WhiteSpace);
					// Take all the whitespace, and output that.
					Output(SpanKind.WhiteSpace);
				}

				if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Assign)
				{
					// We're in a parameterised argument (e.g. one=two
					AcceptAndMoveNext();
					// Accept everything until the next whitespace or closing tag.
					AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
					// Output this as a map.
					Output(SpanKind.Map);
				}
				else
				{
					// Accept everything until the next whitespace or closing tag.
					AcceptUntil(HandlebarsSymbolType.Assign, HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
					if (CurrentSymbol.Value.Type != HandlebarsSymbolType.Assign)
					{
						// Output this as a parameter.
						Output(SpanKind.Parameter);
					}
				}
			}

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			// Accept the closing tag.
			AcceptAndMoveNext();
			// Output this as metacode.
			Output(SpanKind.MetaCode);
		}

		// Special case, as we need to handle branching, so let's merge the last block into the parent block.
		if (tagName == "elseif" && parent != null && parent.Name == "if")
		{
			// Let's merge the current block with the parent and re-instate it.
			Context.MergeCurrentWithParent();
		}
		else
		{
			Context.CurrentBlock.Name = tagName;
			Context.CurrentBlock.Descriptor = descriptor;
		}

		// Switch back to parsing the content of the block.
		ParseBlock();
	}

	/// <summary>
	/// Parses the end of a tag block.
	/// </summary>
	public void AtBlockEndTag()
	{
		string? tagName = Context!.CurrentBlock.Name;

		var blockType = Context.CurrentBlock switch
		{
			{ IsPartialBlock: true, IsPartialBlockContent: true } => BlockType.PartialBlockContentElement,
			{ IsPartialBlock: true } => BlockType.PartialBlockElement,
			_ => BlockType.TagElement
		};

		using (Context.StartBlock(blockType))
		{
			// Accept the open tag.
			AcceptAndMoveNext();
			// Output that tag as metacode.
			Output(SpanKind.MetaCode);

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			// Accept the slash tag.
			AcceptAndMoveNext();
			// Output that tag as metacode.
			Output(SpanKind.MetaCode);

			// Accept everything until either the close of the tag.
			AcceptUntil(HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
			// Output the first part as an expression.
			Output(SpanKind.Expression);

			// Get the name of the tag.
			string? name = LastSpanContent();
			if (!string.Equals(name, tagName))
			{
				Context.OnError(CurrentLocation, "Unbalanced tags - expected a closing tag for '" + Context.CurrentBlock.Name + "' but instead found '" + name + "'");
			}

			Context.CurrentBlock.Name = name;

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			// Accept the closing tag.
			AcceptAndMoveNext();
			// Output this as metacode.
			Output(SpanKind.MetaCode);
		}

		// End the current block;
		Context.EndBlock();
	}

	/// <summary>
	/// Parses a comment tag.
	/// </summary>
	public void AtCommentTag()
	{
		using (var block = Context!.StartBlock(BlockType.Comment))
		{
			// Accept the open tag.
			AcceptAndMoveNext();
			// Output the tag as metacode.
			Output(SpanKind.MetaCode);

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			// Accept the ! symbol.
			AcceptAndMoveNext();
			// Output the symbol as metacode.
			Output(SpanKind.MetaCode);

			// Accept the comment.
			AcceptAndMoveNext();
			Output(SpanKind.Comment, collapsed: true);

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			// Accept the closing tag.
			AcceptAndMoveNext();
			Output(SpanKind.MetaCode);
		}
	}

	/// <summary>
	/// Parses an expression.
	/// </summary>
	public void AtExpressionTag(HandlebarsSymbolType? expectedPrefixSymbol = null, SpanKind expectedPrefixSpanKind = SpanKind.MetaCode)
	{
		string? tagName = Context!.CurrentBlock.Name;

		using (var block = Context.StartBlock(BlockType.Expression))
		{
			// Accept the open tag.
			AcceptAndMoveNext();
			// Output that tag as metacode.
			Output(SpanKind.MetaCode);

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			if (expectedPrefixSymbol != null && Required(expectedPrefixSymbol.Value, true))
			{
				//Accept the prefix symbol and move next.
				AcceptAndMoveNext();
				// Output the prefix symbol.
				Output(expectedPrefixSpanKind);
			}

			// Accept everything until either the close of the tag, or the first element of whitespace.
			AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
			// Output the first part as an expression.
			Output(SpanKind.Expression);

			string? name = LastSpanContent();
			Context.CurrentBlock.Name = name;

			// Special case - else expressions become tag elements themselves.
			if (name == "else" || name == "^")
			{
				// Change the tag type to ensure this is matched as a tag element.
				Context.CurrentBlock.Type = BlockType.TagElement;
			}

			while (CurrentSymbol!.Value.Type != HandlebarsSymbolType.CloseTag && CurrentSymbol.Value.Type != HandlebarsSymbolType.RawCloseTag && CurrentSymbol.Value.Type != HandlebarsSymbolType.Tilde)
			{
				if (CurrentSymbol.Value.Type == HandlebarsSymbolType.WhiteSpace)
				{
					// Accept all the whitespace.
					AcceptAll(HandlebarsSymbolType.WhiteSpace);
					// Take all the whitespace, and output that.
					Output(SpanKind.WhiteSpace);
				}

				if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Assign)
				{
					// We're in a parameterised argument (e.g. one=two
					AcceptAndMoveNext();
					// Accept everything until the next whitespace or closing tag.
					AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
					// Output this as a map.
					Output(SpanKind.Map);
				}
				else
				{
					// Accept everything until the next whitespace or closing tag.
					AcceptUntil(HandlebarsSymbolType.Assign, HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
					if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Assign)
					{
						continue;
					}
					// Output this as a map.
					Output(SpanKind.Parameter);
				}
			}

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			// Accept the closing tag.
			AcceptAndMoveNext();
			// Output this as metacode.
			Output(SpanKind.MetaCode);
		}
	}

	/// <summary>
	/// Parses an expression.
	/// </summary>
	public void AtPartialTag()
	{
		string? tagName = Context!.CurrentBlock.Name;

		using (var block = Context.StartBlock(BlockType.Partial))
		{
			// Accept the open tag.
			AcceptAndMoveNext();
			// Output that tag as metacode.
			Output(SpanKind.MetaCode);

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			// Accept the right arrow >.
			AcceptAndMoveNext();
			// Output that tag as metacode.
			Output(SpanKind.MetaCode);

			// Accept everything until either the close of the tag, or the first element of whitespace.
			AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
			// Output the first part as an expression.
			Output(SpanKind.Expression);

			tagName = LastSpanContent();
			Context.CurrentBlock.Name = tagName;

			while (CurrentSymbol!.Value.Type != HandlebarsSymbolType.CloseTag && CurrentSymbol.Value.Type != HandlebarsSymbolType.RawCloseTag && CurrentSymbol.Value.Type != HandlebarsSymbolType.Tilde)
			{
				if (CurrentSymbol.Value.Type == HandlebarsSymbolType.WhiteSpace)
				{
					// Accept all the whitespace.
					AcceptAll(HandlebarsSymbolType.WhiteSpace);
					// Take all the whitespace, and output that.
					Output(SpanKind.WhiteSpace);
				}

				if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Assign)
				{
					// We're in a parameterised argument (e.g. one=two
					AcceptAndMoveNext();
					// Accept everything until the next whitespace or closing tag.
					AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
					// Output this as a map.
					Output(SpanKind.Map);
				}
				else
				{
					// Accept everything until the next whitespace or closing tag.
					AcceptUntil(HandlebarsSymbolType.Assign, HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
					if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Assign)
					{
						continue;
					}
					// Output this as a map.
					Output(SpanKind.Parameter);
				}
			}

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			// Accept the closing tag.
			AcceptAndMoveNext();
			// Output this as metacode.
			Output(SpanKind.MetaCode);
		}
	}

	/// <summary>
	/// Parses a partial block tag.
	/// </summary>
	public void AtPartialBlockTag()
	{
		var parent = Context!.CurrentBlock;

		string? tagName = null;
		TagDescriptor? descriptor = null;
		// Start a new block.

		Context.StartBlock(BlockType.PartialBlock);
		Context.CurrentBlock.IsPartialBlock = true;

		using (Context.StartBlock(BlockType.PartialBlockElement))
		{
			Context.CurrentBlock.IsPartialBlock = true;

			// Accept the open tag.
			AcceptAndMoveNext();
			// Output that tag as metacode.
			Output(SpanKind.MetaCode);

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			if (Required(HandlebarsSymbolType.Hash, true))
			{
				// Accept the prefix symbol type. tag.
				AcceptAndMoveNext();
				// Output that tag as metacode.
				Output(SpanKind.MetaCode);
			}

			if (Required(HandlebarsSymbolType.RightArrow, true))
			{
				// Accept the prefix symbol type. tag.
				AcceptAndMoveNext();
				// Output that tag as metacode.
				Output(SpanKind.MetaCode);
			}

			// Accept everything until either the close of the tag, or the first element of whitespace.
			AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag);
			// Output the first part as an expression.
			Output(SpanKind.Expression);

			// Get the tag name and set it for the block.
			tagName = LastSpanContent();
			descriptor = null;

			Context.CurrentBlock.Name = tagName;
			Context.CurrentBlock.Descriptor = descriptor;

			while (CurrentSymbol!.Value.Type != HandlebarsSymbolType.CloseTag && CurrentSymbol.Value.Type != HandlebarsSymbolType.RawCloseTag && CurrentSymbol.Value.Type != HandlebarsSymbolType.Tilde)
			{
				if (CurrentSymbol.Value.Type == HandlebarsSymbolType.WhiteSpace)
				{
					// Accept all the whitespace.
					AcceptAll(HandlebarsSymbolType.WhiteSpace);
					// Take all the whitespace, and output that.
					Output(SpanKind.WhiteSpace);
				}

				if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Assign)
				{
					// We're in a parameterised argument (e.g. one=two
					AcceptAndMoveNext();
					// Accept everything until the next whitespace or closing tag.
					AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
					// Output this as a map.
					Output(SpanKind.Map);
				}
				else
				{
					// Accept everything until the next whitespace or closing tag.
					AcceptUntil(HandlebarsSymbolType.Assign, HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
					if (CurrentSymbol.Value.Type != HandlebarsSymbolType.Assign)
					{
						// Output this as a parameter.
						Output(SpanKind.Parameter);
					}
				}
			}

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			// Accept the closing tag.
			AcceptAndMoveNext();
			// Output this as metacode.
			Output(SpanKind.MetaCode);
		}

		Context.CurrentBlock.Name = tagName;
		Context.CurrentBlock.Descriptor = descriptor;

		// Switch back to parsing the content of the block.
		ParseBlock();
	}

	/// <summary>
	/// Parses a partial block tag.
	/// </summary>
	public void AtPartialBlockContentTag()
	{
		var parent = Context!.CurrentBlock;

		string? tagName = null;
		TagDescriptor? descriptor = null;
		// Start a new block.

		Context.StartBlock(BlockType.PartialBlockContent);
		Context.CurrentBlock.IsPartialBlockContent = Context.CurrentBlock.IsPartialBlock = true;

		using (Context.StartBlock(BlockType.PartialBlockContentElement))
		{
			Context.CurrentBlock.IsPartialBlockContent = Context.CurrentBlock.IsPartialBlock = true;

			// Accept the open tag.
			AcceptAndMoveNext();
			// Output that tag as metacode.
			Output(SpanKind.MetaCode);

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			if (Required(HandlebarsSymbolType.RightArrow, true))
			{
				// Accept the prefix symbol type. tag.
				AcceptAndMoveNext();
				// Output that tag as metacode.
				Output(SpanKind.MetaCode);
			}

			// Accept everything until either the close of the tag, or the first element of whitespace.
			AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag);
			// Output the first part as an expression.
			Output(SpanKind.Expression);

			// Get the tag name and set it for the block.
			tagName = LastSpanContent();
			descriptor = null;

			Context.CurrentBlock.Name = tagName;
			Context.CurrentBlock.Descriptor = descriptor;

			while (CurrentSymbol!.Value.Type != HandlebarsSymbolType.CloseTag && CurrentSymbol.Value.Type != HandlebarsSymbolType.RawCloseTag && CurrentSymbol.Value.Type != HandlebarsSymbolType.Tilde)
			{
				if (CurrentSymbol.Value.Type == HandlebarsSymbolType.WhiteSpace)
				{
					// Accept all the whitespace.
					AcceptAll(HandlebarsSymbolType.WhiteSpace);
					// Take all the whitespace, and output that.
					Output(SpanKind.WhiteSpace);
				}

				if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Assign)
				{
					// We're in a parameterised argument (e.g. one=two
					AcceptAndMoveNext();
					// Accept everything until the next whitespace or closing tag.
					AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
					// Output this as a map.
					Output(SpanKind.Map);
				}
				else
				{
					// Accept everything until the next whitespace or closing tag.
					AcceptUntil(HandlebarsSymbolType.Assign, HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
					if (CurrentSymbol.Value.Type != HandlebarsSymbolType.Assign)
					{
						// Output this as a parameter.
						Output(SpanKind.Parameter);
					}
				}
			}

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			// Accept the closing tag.
			AcceptAndMoveNext();
			// Output this as metacode.
			Output(SpanKind.MetaCode);
		}

		Context.CurrentBlock.Name = tagName;
		Context.CurrentBlock.Descriptor = descriptor;

		// Switch back to parsing the content of the block.
		ParseBlock();
	}

	/// <summary>
	/// Parses an expression.
	/// </summary>
	public void AtZoneTag()
	{
		string? tagName = null;

		using (var block = Context!.StartBlock(BlockType.Zone))
		{
			// Accept the open tag.
			AcceptAndMoveNext();
			// Output that tag as metacode.
			Output(SpanKind.MetaCode);

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			// Accept the left arrow <.
			AcceptAndMoveNext();
			// Output that tag as metacode.
			Output(SpanKind.MetaCode);

			// Accept everything until either the close of the tag, or the first element of whitespace.
			AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
			// Output the first part as an expression.
			Output(SpanKind.Expression);

			// Get the tag name and set it for the block.
			tagName = LastSpanContent();
			Context.CurrentBlock.Name = tagName;

			while (CurrentSymbol!.Value.Type != HandlebarsSymbolType.CloseTag && CurrentSymbol.Value.Type != HandlebarsSymbolType.RawCloseTag && CurrentSymbol.Value.Type != HandlebarsSymbolType.Tilde)
			{
				if (CurrentSymbol.Value.Type == HandlebarsSymbolType.WhiteSpace)
				{
					// Accept all the whitespace.
					AcceptAll(HandlebarsSymbolType.WhiteSpace);
					// Take all the whitespace, and output that.
					Output(SpanKind.WhiteSpace);
				}

				if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Assign)
				{
					// We're in a parameterised argument (e.g. one=two
					AcceptAndMoveNext();
					// Accept everything until the next whitespace or closing tag.
					AcceptUntil(HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
					// Output this as a map.
					Output(SpanKind.Map);
				}
				else
				{
					// Accept everything until the next whitespace or closing tag.
					AcceptUntil(HandlebarsSymbolType.Assign, HandlebarsSymbolType.WhiteSpace, HandlebarsSymbolType.CloseTag, HandlebarsSymbolType.RawCloseTag, HandlebarsSymbolType.Tilde);
					if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Assign)
					{
						continue;
					}
					// Output this as a map.
					Output(SpanKind.Parameter);
				}
			}

			if (Optional(HandlebarsSymbolType.Tilde))
			{
				// Output the tilde.
				Output(SpanKind.MetaCode);
			}

			// Accept the closing tag.
			AcceptAndMoveNext();
			// Output this as metacode.
			Output(SpanKind.MetaCode);
		}
	}

	/// <summary>
	/// Parses a tag.
	/// </summary>
	public void AtTag()
	{
		var current = CurrentSymbol;
		NextToken();
		HandlebarsSymbol? tilde = null;

		if (CurrentSymbol!.Value.Type == HandlebarsSymbolType.Tilde)
		{
			tilde = CurrentSymbol;
			NextToken();
		}

		if (CurrentSymbol!.Value.Type == HandlebarsSymbolType.Hash)
		{
			var hash = CurrentSymbol;
			bool isPartialBlock = false;

			NextToken();
			if (CurrentSymbol.Value.Type == HandlebarsSymbolType.RightArrow)
			{
				isPartialBlock = true;
			}
			PutBack(CurrentSymbol.Value); // Put back the > character

			// Put the opening tag back.
			PutBack(hash!.Value); // Put back the # character
			if (tilde != null)
			{
				PutBack(tilde!.Value);
			}

			PutBack(current!.Value); // Put back the opening tag symbol
			NextToken();

			if (isPartialBlock)
			{
				// We're at a partial block tag {{#>hello}} etc.
				AtPartialBlockTag();
			}
			else
			{
				// We're at a block tag {{#hello}} etc.
				AtBlockTag();
			}
		}
		else if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Bang)
		{
			// Put the opening tag back.
			PutBack(CurrentSymbol!.Value);
			if (tilde != null)
			{
				PutBack(tilde!.Value);
			}
			PutBack(current!.Value);
			NextToken();
			// We're at a comment {{!....}}
			AtCommentTag();
		}
		else if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Slash)
		{
			// Put the opening tag back.
			PutBack(CurrentSymbol!.Value);
			if (tilde != null)
			{
				PutBack(tilde!.Value);
			}
			PutBack(current!.Value);
			NextToken();
			// We're at a closing block tag {{/each}}
			AtBlockEndTag();
		}
		else if (CurrentSymbol.Value.Type == HandlebarsSymbolType.RightArrow)
		{
			// Put the opening tag back.
			PutBack(CurrentSymbol!.Value);
			if (tilde != null)
			{
				PutBack(tilde!.Value);
			}
			PutBack(current!.Value);
			NextToken();
			if (Context!.CurrentBlock.IsPartialBlock && !Context.CurrentBlock.IsPartialBlockContent)
			{
				// This is a zone content element, which is actually a block, not a span
				AtPartialBlockContentTag();
			}
			else
			{
				// We're at a partial include tag {{>body}}
				AtPartialTag();
			}
		}
		else if (CurrentSymbol.Value.Type == HandlebarsSymbolType.LeftArrow)
		{
			// Put the opening tag back.
			PutBack(CurrentSymbol!.Value);
			if (tilde != null)
			{
				PutBack(tilde!.Value);
			}
			PutBack(current!.Value);
			NextToken();
			// We're at a zone include tag {{<body}}
			AtZoneTag();
		}
		else if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Negate)
		{
			var current2 = CurrentSymbol;
			// Step foward and see if this is a block or expression.
			NextToken();
			if (CurrentSymbol.Value.Type == HandlebarsSymbolType.CloseTag)
			{
				// This is an expression.	
				PutBack(CurrentSymbol!.Value);

				// Put the opening tag back.
				PutBack(current2!.Value);
				if (tilde != null)
				{
					PutBack(tilde!.Value);
				}
				PutBack(current!.Value);
				NextToken();
				// We're at a negated block tag {{^hello}} etc.
				AtExpressionTag(HandlebarsSymbolType.Negate, SpanKind.Expression);
			}
			else
			{
				PutBack(CurrentSymbol!.Value);

				// Put the opening tag back.
				PutBack(current2!.Value);
				if (tilde != null)
				{
					PutBack(tilde!.Value);
				}
				PutBack(current!.Value);
				NextToken();
				// We're at a negated block tag {{^hello}} etc.
				AtBlockTag(HandlebarsSymbolType.Negate);
			}
		}
		else if (CurrentSymbol.Value.Type == HandlebarsSymbolType.Ampersand)
		{
			// Put the opening tag back.
			PutBack(CurrentSymbol!.Value);
			if (tilde != null)
			{
				PutBack(tilde!.Value);
			}
			PutBack(current!.Value);
			NextToken();
			// Handle an expression tag.
			AtExpressionTag(HandlebarsSymbolType.Ampersand);
		}
		else
		{
			// Put the opening tag back.
			PutBack(CurrentSymbol!.Value);
			if (tilde != null)
			{
				PutBack(tilde!.Value);
			}
			PutBack(current!.Value);
			NextToken();
			// Handle an expression tag.
			AtExpressionTag();
		}
	}

	/// <summary>
	/// Gets the content of the last span.
	/// </summary>
	/// <returns>The span content.</returns>
	private string? LastSpanContent()
	{
		var span = Context!.CurrentBlock.Children.LastOrDefault() as Span;
		if (span != null)
		{
			return span.Content;
		}
		return null;
	}

	/// <inheritdoc />
	public override void ParseBlock()
	{
		// Accept any leading whitespace.
		AcceptWhile(HandlebarsSymbolType.WhiteSpace);
		// Output the whitespace.
		Output(SpanKind.WhiteSpace);

		// Accept everything until we meet a tag (either {{ or {{{).
		AcceptUntil(HandlebarsSymbolType.OpenTag, HandlebarsSymbolType.RawOpenTag, HandlebarsSymbolType.WhiteSpace);

		// Output everything we have so far as text.
		Output(SpanKind.Text);

		if (EndOfFile || CurrentSymbol == null)
		{
			return;
		}

		if (CurrentSymbol.Value.Type == HandlebarsSymbolType.OpenTag || CurrentSymbol.Value.Type == HandlebarsSymbolType.RawOpenTag)
		{
			// Now we're at a tag.
			AtTag();
		}
	}

	/// <inheritdoc />
	public override void ParseDocument()
	{
		using (PushSpanConfig())
		{
			if (Context == null)
			{
				throw new InvalidOperationException("Context has not been set.");
			}

			using (Context.StartBlock(BlockType.Document))
			{
				if (!NextToken())
				{
					return;
				}

				while (!EndOfFile)
				{
					ParseBlock();
				}
			}
		}
	}
}
