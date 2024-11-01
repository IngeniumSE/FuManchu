// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Parser;

using System.Collections.Generic;

/// <summary>
/// Represents a parser visitor that supports scoping.
/// </summary>
/// <typeparam name="TScope">The scope type.</typeparam>
public abstract class ParserVisitor<TScope> : ParserVisitor
{
	private readonly Stack<TScope> _scopes = new Stack<TScope>();

	/// <summary>
	/// Gets the scope.
	/// </summary>
	public TScope Scope
	{
		get { return _scopes.Peek(); }
	}

	/// <summary>
	/// Reverts the current scope.
	/// </summary>
	public virtual void RevertScope()
	{
		_scopes.Pop();
	}

	/// <summary>
	/// Sets the current scope.
	/// </summary>
	/// <param name="scope">The scope instance.</param>
	public virtual void SetScope(TScope scope)
	{
		_scopes.Push(scope);
	}
}
