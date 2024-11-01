// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Renderer;

using System;

/// <summary>
/// Represents a disposable scope around a context.
/// </summary>
public class RenderContextScope : DisposableAction
{
	/// <summary>
	/// Initialises a new instance of <see cref="RenderContextScope"/>
	/// </summary>
	/// <param name="scopeContext">The scoped context.</param>
	/// <param name="action">The dispose action.</param>
	public RenderContextScope(RenderContext scopeContext, Action action) : base(action)
	{
		ScopeContext = scopeContext;
	}

	/// <summary>
	/// Gets or sets the scoped context.
	/// </summary>
	public RenderContext ScopeContext { get; private set; }
}
