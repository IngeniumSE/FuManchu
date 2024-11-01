// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tags;

using System.Collections.Generic;

using FuManchu.Renderer;

/// <summary>
/// Provides the standard set of Handlebars tags.
/// </summary>
public class StandardTagProvider : ITagProvider
{
	internal static readonly TagDescriptor If = new TagDescriptor("if", new ConditionalBlockRenderer(), 1, maxArguments: 1, allowMappedParamters: false, hasChildContent: true);
	internal static readonly TagDescriptor ElseIf = new TagDescriptor("elseif", new ConditionalBlockRenderer(), 1, maxArguments: 1, allowMappedParamters: false, hasChildContent: true);
	internal static readonly TagDescriptor Else = new TagDescriptor("else", new ConditionalBlockRenderer(), 0, maxArguments: 0, allowMappedParamters: false, hasChildContent: true);
	internal static readonly TagDescriptor Unless = new TagDescriptor("unless", new UnlessBlockRenderer(), 1, maxArguments: 1, allowMappedParamters: false, hasChildContent: true);
	internal static readonly TagDescriptor Each = new TagDescriptor("each", new EnumerableBlockRenderer(), 1, maxArguments: 1, allowMappedParamters: false, hasChildContent: true);
	internal static readonly TagDescriptor With = new TagDescriptor("with", new ScopeBlockRenderer(), 1, maxArguments: 1, allowMappedParamters: false, hasChildContent: true);
	internal static readonly TagDescriptor Is = new TagDescriptor("is", new IsBlockRenderer(), 1, 3, false, hasChildContent: true);

	/// <inheritdoc />
	public IEnumerable<TagDescriptor> GetTags()
	{
		// Logic tags.
		yield return If;
		yield return ElseIf;
		yield return Else;
		yield return Unless;

		// Enumerator tags.
		yield return Each;

		// Scope tags.
		yield return With;

		// Is tag.
		yield return Is;
	}
}
