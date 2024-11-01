// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

/// <summary>
/// Represents a compiled Handlebars helper.
/// </summary>
/// <param name="options">The helper options.</param>
/// <returns>The helper result.</returns>
public delegate string HandlebarHelper(HelperOptions options);
