// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Binding;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

/// <summary>
/// Provides base services for reading associated metadata for types and properties.
/// </summary>
public abstract class AssociatedMetadataProvider<TModelMetadata> : IModelMetadataProvider where TModelMetadata : ModelMetadata
{
	private readonly ConcurrentDictionary<Type, TypeInformation> _typeInfoCache = new ConcurrentDictionary<Type, TypeInformation>();

	/// <inheritdoc />
	public IEnumerable<ModelMetadata> GetMetadataForProperties(object? container, Type? containerType)
	{
		return GetMetadataForPropertiesCore(container, containerType);
	}

	/// <inheritdoc />
	public ModelMetadata GetMetadataForProperty(ModelAccessor? modelAccessor, Type? containerType, string? propertyName)
	{
		if (string.IsNullOrWhiteSpace(propertyName))
		{
			throw new ArgumentException("The property name is required.");
		}

		var typeInfo = GetTypeInformation(containerType!);
		PropertyInformation propertyInfo;
		if (!typeInfo.Properties.TryGetValue(propertyName, out propertyInfo))
		{
			throw new ArgumentException("The property '" + propertyName + "' was not found on type '" + containerType!.FullName + "'");
		}

		return CreatePropertyMetadata(modelAccessor, propertyInfo);
	}

	/// <inheritdoc />
	public ModelMetadata GetMetadataForType(ModelAccessor? modelAccessor, Type modelType)
	{
		var prototype = GetTypeInformation(modelType).Prototype;
		return CreateMetadataFromPrototype(prototype, modelAccessor);
	}

	/// <summary>
	/// Creates a metadata prototype for the given model type.
	/// </summary>
	/// <param name="containerType">The container type.</param>
	/// <param name="modelType">The model type.</param>
	/// <param name="propertyName">The property name.</param>
	/// <returns>The metadata prototype.</returns>
	protected abstract TModelMetadata CreateMetadataPrototype(Type? containerType, Type modelType, string? propertyName);

	/// <summary>
	/// Creates metadata from a prototype used to generate the final metadata.
	/// </summary>
	/// <param name="prototype">The metadata prototype.</param>
	/// <param name="modelAccessor">The model accessor.</param>
	/// <returns>The metadata instance.</returns>
	protected abstract TModelMetadata CreateMetadataFromPrototype(TModelMetadata prototype, Func<object?>? modelAccessor);

	/// <summary>
	/// Creates property information for the given helper.
	/// </summary>
	/// <param name="containerType">The container type.</param>
	/// <param name="helper">The property helper.</param>
	/// <returns>The property information.</returns>
	private PropertyInformation CreatePropertyInformation(Type containerType, PropertyHelper helper)
	{
		var property = helper.Property;

		return new PropertyInformation
		{
			PropertyHelper = helper,
			Prototype = CreateMetadataPrototype(containerType, property.PropertyType, property.Name)
		};
	}

	/// <summary>
	/// Creates finalised metadata for the given model/property.
	/// </summary>
	/// <param name="modelAccessor">The model accessor.</param>
	/// <param name="propertyInfo">The property information.</param>
	/// <returns>The property model metadata.</returns>
	private TModelMetadata CreatePropertyMetadata(Func<object?>? modelAccessor, PropertyInformation propertyInfo)
	{
		var metadata = CreateMetadataFromPrototype(propertyInfo.Prototype, modelAccessor);
		return metadata;
	}

	/// <summary>
	/// Creates type information for the given type.
	/// </summary>
	/// <param name="type">The type instance.</param>
	/// <returns>The type information.</returns>
	private TypeInformation CreateTypeInformation(Type type)
	{
		var props = new Dictionary<string, PropertyInformation>(StringComparer.Ordinal);
		foreach (var helper in PropertyHelper.GetProperties(type))
		{
			if (!props.ContainsKey(helper.Name))
			{
				props.Add(helper.Name, CreatePropertyInformation(type, helper));
			}
		}

		var info = new TypeInformation
		{
			Prototype = CreateMetadataPrototype(null, type, null),
			Properties = props
		};

		return info;
	}

	/// <summary>
	/// Gets the metadata for all properties of the given container type.
	/// </summary>
	/// <param name="container">The container instance.</param>
	/// <param name="containerType">The container type.</param>
	/// <returns>The property metadata instances.</returns>
	private IEnumerable<ModelMetadata> GetMetadataForPropertiesCore(object? container, Type? containerType)
	{
		var typeInfo = GetTypeInformation(containerType!);
		foreach (var pair in typeInfo.Properties)
		{
			var propertyInfo = pair.Value;
			Func<object>? modelAccessor = null;

			if (container != null)
			{
				modelAccessor = () => propertyInfo.PropertyHelper.GetValue(container);
			}
			yield return CreatePropertyMetadata(modelAccessor, propertyInfo);
		}
	}

	/// <summary>
	/// Gets the type information for the given type.
	/// </summary>
	/// <param name="type">The type instance.</param>
	/// <returns>The type information.</returns>
	private TypeInformation GetTypeInformation(Type type)
	{
		TypeInformation typeInfo;
		if (!_typeInfoCache.TryGetValue(type, out typeInfo))
		{
			typeInfo = CreateTypeInformation(type);
			_typeInfoCache.TryAdd(type, typeInfo);
		}
		return typeInfo;
	}

	/// <summary>
	/// Represents basic type information.
	/// </summary>
	private sealed class TypeInformation
	{
		/// <summary>
		/// Gets or sets the metadata prototype.
		/// </summary>
		public required TModelMetadata Prototype { get; set; }

		/// <summary>
		/// Gets or sets the set of properties.
		/// </summary>
		public required Dictionary<string, PropertyInformation> Properties { get; set; }
	}

	/// <summary>
	/// Represents basic property information.
	/// </summary>
	private sealed class PropertyInformation
	{
		/// <summary>
		/// Gets or sets the property helper.
		/// </summary>
		public required PropertyHelper PropertyHelper { get; set; }

		/// <summary>
		/// Gets or sets the metadata prototype.
		/// </summary>
		public required TModelMetadata Prototype { get; set; }
	}
}
