// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using FuManchu.Renderer;

/// <summary>
/// Represents a compiled Handlebars partial template.
/// </summary>
/// <param name="context">The parent render context.</param>
/// <param name="writer">The output text writer.</param>
/// <returns>The template result.</returns>
public delegate void HandlebarPartialTemplate(RenderContext context, TextWriter writer);
