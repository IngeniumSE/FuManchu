// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using System;
using System.Collections.Generic;
using System.Linq;

using FuManchu.Parser.SyntaxTree;
using FuManchu.Tags;
using FuManchu.Text;

/// <summary>
/// Represents a context for parsing.
/// </summary>
public partial class ParserContext
{
	private bool _terminated = false;
	private readonly Stack<BlockBuilder> _blockStack = new Stack<BlockBuilder>();
	private readonly ParserErrorSink _errorSink;

	/// <summary>
	/// Initializes a new instance of the <see cref="ParserContext"/> class.
	/// </summary>
	/// <param name="source">The source.</param>
	/// <param name="parser">The parser.</param>
	/// <param name="errorSink">The error sink.</param>
	/// <param name="providers">The set of tag providers.</param>
	public ParserContext(ITextDocument source, ParserBase parser, ParserErrorSink errorSink, TagProvidersCollection providers)
	{
		Source = new TextDocumentReader(source);
		Parser = parser;
		_errorSink = errorSink;
		TagProviders = providers;
	}

	/// <summary>
	/// Gets the block stack.
	/// </summary>
	internal Stack<BlockBuilder> BlockStack { get { return _blockStack; } }

	/// <summary>
	/// Gets the current block (builder).
	/// </summary>
	public BlockBuilder CurrentBlock
	{
		get { return _blockStack.Peek(); }
	}

	/// <summary>
	/// Gets the current character.
	/// </summary>
	public char CurrentCharacter
	{
		get
		{
			if (_terminated)
			{
				return '\0';
			}

			var ch = Source.Peek();
			if (ch == -1)
			{
				return '\0';
			}
			return (char)ch;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether we at the end of the input source.
	/// </summary>
	public bool EndOfFile { get { return _terminated || Source.Peek() == -1; } }

	/// <summary>
	/// Gets the errors generated through parsing.
	/// </summary>
	public IEnumerable<Error> Errors
	{
		get { return _errorSink.Errors; }
	}

	/// <summary>
	/// Gets the last span.
	/// </summary>
	public Span? LastSpan { get; private set; }

	/// <summary>
	/// Gets the parser.
	/// </summary>
	public ParserBase Parser { get; private set; }

	/// <summary>
	/// Gets or sets the source.
	/// </summary>
	public TextDocumentReader Source { get; set; }

	/// <summary>
	/// Gets or sets the collection of tag providers.
	/// </summary>
	public TagProvidersCollection TagProviders { get; set; }

	/// <summary>
	/// Adds the span.
	/// </summary>
	/// <param name="span">The span.</param>
	public void AddSpan(Span span)
	{
		EnsureNotTerminated();

		if (_blockStack.Count == 0)
		{
			throw new InvalidOperationException("No current block");
		}

		span.Previous = LastSpan;
		if (LastSpan != null)
		{
			LastSpan.Next = span;
		}

		_blockStack.Peek().Children?.Add(span);
		LastSpan = span;
	}

	/// <summary>
	/// Completes the parse operation.
	/// </summary>
	/// <returns>The parser results.</returns>
	public ParserResults CompleteParse()
	{
		if (_blockStack.Count == 0)
		{
			throw new InvalidOperationException("No root block.");
		}
		if (_blockStack.Count != 1)
		{
			throw new InvalidOperationException("Outstanding blocks.");
		}

		return new ParserResults(_blockStack.Pop().Build(), _errorSink.Errors.ToList());
	}

	/// <summary>
	/// Ends the block.
	/// </summary>
	public void EndBlock()
	{
		EnsureNotTerminated();

		if (_blockStack.Count == 0)
		{
			throw new InvalidOperationException("EndBlock called without matching StartBlock call.");
		}

		if (_blockStack.Count > 1)
		{
			var block = _blockStack.Pop();
			_blockStack.Peek().Children?.Add(block.Build());
		}
		else
		{
			_terminated = true;
		}
	}

	/// <summary>
	/// Ensures the not parse is not terminated.
	/// </summary>
	private void EnsureNotTerminated()
	{
		if (_terminated)
		{
			throw new InvalidOperationException("Parsing has been completed.");
		}
	}

	/// <summary>
	/// Merges the current block with it's parent.
	/// </summary>
	public void MergeCurrentWithParent()
	{
		if (_blockStack.Count > 2)
		{
			var current = _blockStack.Pop();
			var parent = _blockStack.Peek();

			if (current.Children is { Count: > 0 })
			{
				foreach (var node in current.Children)
				{
					parent.Children.Add(node);
				}
			}
		}
	}

	/// <summary>
	/// Adds an error to the sink.
	/// </summary>
	/// <param name="error">The error.</param>
	public void OnError(Error error)
	{
		EnsureNotTerminated();

		_errorSink.OnError(error);
	}

	/// <summary>
	/// Adds an error to the sink.
	/// </summary>
	/// <param name="location">The location.</param>
	/// <param name="message">The message.</param>
	public void OnError(SourceLocation location, string message)
	{
		OnError(new Error(message, location));
	}

	/// <summary>
	/// Adds an error to the sink.
	/// </summary>
	/// <param name="location">The location.</param>
	/// <param name="message">The message.</param>
	/// <param name="length">The length.</param>
	public void OnError(SourceLocation location, string message, int length)
	{
		OnError(new Error(message, location, length));
	}

	/// <summary>
	/// Starts a block.
	/// </summary>
	/// <param name="blockType">Type of the block.</param>
	/// <param name="name">[Optional] The block name.</param>
	/// <returns>The disposable used to end the block.</returns>
	public IDisposable StartBlock(BlockType? blockType = null, string? name = null)
	{
		EnsureNotTerminated();

		_blockStack.Push(new BlockBuilder() { Type = blockType.GetValueOrDefault(BlockType.Text), Name = name });

		return new DisposableAction(EndBlock);
	}
}
