// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tags;

using System.Collections.Generic;

/// <summary>
/// Defines the required contract for implementing a tag provider.
/// </summary>
public interface ITagProvider
{
	/// <summary>
	/// Gets the set of available tags.
	/// </summary>
	/// <returns>The set of tag descriptors.</returns>
	IEnumerable<TagDescriptor> GetTags();
}
