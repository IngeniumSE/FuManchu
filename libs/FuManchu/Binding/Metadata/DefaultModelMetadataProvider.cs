// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Binding;

using System;

/// <summary>
/// Provides default services for managing model metadata.
/// </summary>
public class DefaultModelMetadataProvider : AssociatedMetadataProvider<ModelMetadata>
{
	/// <inheritdoc />
	protected override ModelMetadata CreateMetadataPrototype(Type? containerType, Type modelType, string? propertyName)
	{
		return new ModelMetadata(this, containerType, null, modelType, propertyName)
		{
			Valid = false
		};
	}

	/// <inheritdoc />
	protected override ModelMetadata CreateMetadataFromPrototype(ModelMetadata prototype, Func<object?>? modelAccessor)
	{
		return new ModelMetadata(this, prototype.ContainerType, modelAccessor, prototype.ModelType, prototype.PropertyName)
		{
			Valid = true
		};
	}
}
