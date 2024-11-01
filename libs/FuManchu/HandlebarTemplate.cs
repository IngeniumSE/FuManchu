// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

/// <summary>
/// Represents a compiled Handlebars template.
/// </summary>
/// <param name="model">The model instance.</param>
/// <param name="resolver">The resolver used to handle unknown values.</param>
/// <returns>The template result.</returns>
public delegate string HandlebarTemplate(
	object? model = null,
	UnknownValueResolver? resolver = null);
