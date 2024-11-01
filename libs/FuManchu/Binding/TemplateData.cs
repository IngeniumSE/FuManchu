// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Binding;

/// <summary>
/// Represents template data.
/// </summary>
public class TemplateData
{
	/// <summary>
	/// Gets or sets the model metadata.
	/// </summary>
	public ModelMetadata? ModelMetadata { get; set; }

	/// <summary>
	/// Gets or sets the model.
	/// </summary>
	public object? Model { get; set; }
}
