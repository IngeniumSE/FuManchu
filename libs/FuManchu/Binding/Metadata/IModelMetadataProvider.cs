// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Binding;

using System;
using System.Collections.Generic;

/// <summary>
/// Defines the required contract for implementing a model metadata provider.
/// </summary>
public interface IModelMetadataProvider
{
	/// <summary>
	/// Gets the metadata for all available properties of the given container.
	/// </summary>
	/// <param name="container">The container instance.</param>
	/// <param name="containerType">The container type.</param>
	/// <returns>The set of metadata instances.</returns>
	IEnumerable<ModelMetadata> GetMetadataForProperties(object? container, Type? containerType);

	/// <summary>
	/// Gets the metadata for the given named property.
	/// </summary>
	/// <param name="modelAccessor">The model accessor.</param>
	/// <param name="containerType">The container type.</param>
	/// <param name="propertyName">The property name.</param>
	/// <returns>The metadata instance.</returns>
	ModelMetadata GetMetadataForProperty(Func<object?>? modelAccessor, Type? containerType, string? propertyName);

	/// <summary>
	/// Gets the metadata for the given model type.
	/// </summary>
	/// <param name="modelAccessor">The model accessor.</param>
	/// <param name="modelType">The model type.</param>
	/// <returns>The metadata instance.</returns>
	ModelMetadata GetMetadataForType(Func<object?>? modelAccessor, Type modelType);
}
