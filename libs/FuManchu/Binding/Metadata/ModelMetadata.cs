// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Binding;

using System;
using System.Collections.Generic;

/// <summary>
/// Represents metadata for a model.
/// </summary>
public class ModelMetadata
{
	private readonly IModelMetadataProvider _provider;
	private readonly Type? _containerType;
	private readonly Type _modelType;
	private readonly string? _propertyName;

	private object? _model;
	private Func<object?>? _modelAccessor;
	private IEnumerable<ModelMetadata>? _properties;
	private Type? _realModelType;
	private EfficientTypePropertyKey<Type, string?>? _cacheKey;

	/// <summary>
	/// Initialises a new instance of <see cref="ModelMetadata"/>.
	/// </summary>
	/// <param name="provider">The metadata provider.</param>
	/// <param name="containerType">The container type.</param>
	/// <param name="modelAccessor">The model accessor.</param>
	/// <param name="modelType">The model type.</param>
	/// <param name="propertyName">The property name.</param>
	public ModelMetadata(IModelMetadataProvider provider, Type? containerType, Func<object?>? modelAccessor, Type modelType, string? propertyName)
	{
		_provider = provider;
		_containerType = containerType;
		_modelAccessor = modelAccessor;
		_modelType = modelType;
		_propertyName = propertyName;
	}

	/// <summary>
	/// Gets the cache key for the current instance.
	/// </summary>
	internal EfficientTypePropertyKey<Type, string?> CacheKey
	{
		get
		{
			if (_cacheKey == null)
			{
				_cacheKey = CreateCacheKey(ContainerType, ModelType, PropertyName);
			}

			return _cacheKey;
		}
		set
		{
			_cacheKey = value;
		}
	}

	/// <summary>
	/// Gets the container type.
	/// </summary>
	public Type? ContainerType { get { return _containerType; } }

	/// <summary>
	/// Gets whether the model type is a nullable value type.
	/// </summary>
	public bool IsNullableValueType
	{
		get { return ModelType.IsNullableValueType(); }
	}

	/// <summary>
	/// Gets the model.
	/// </summary>
	public object? Model
	{
		get
		{
			if (_modelAccessor != null)
			{
				_model = _modelAccessor();
				_modelAccessor = null;
			}
			return _model;
		}
		set
		{
			_model = value;
			_modelAccessor = null;
			_properties = null;
			_realModelType = null;
		}
	}

	/// <summary>
	/// Gets the model type.
	/// </summary>
	public Type ModelType { get { return _modelType; } }

	/// <summary>
	/// Gets the properties of the given model.
	/// </summary>
	public IEnumerable<ModelMetadata> Properties
	{
		get
		{
			if (_properties == null)
			{
				_properties = Provider.GetMetadataForProperties(Model, RealModelType);
			}
			return _properties;
		}
	}

	/// <summary>
	/// Gets the property name.
	/// </summary>
	public string? PropertyName { get { return _propertyName; } }

	/// <summary>
	/// Gets the metadata provider.
	/// </summary>
	public IModelMetadataProvider Provider { get { return _provider; } }

	/// <summary>
	/// Gets or sets whether the metadata is valid.
	/// </summary>
	public bool Valid { get; set; }

	/// <summary>
	/// Gets the runtime model type.
	/// </summary>
	public Type RealModelType
	{
		get
		{
			if (_realModelType == null)
			{
				_realModelType = ModelType;

				if (Model != null && !ModelType.IsNullableValueType())
				{
					_realModelType = Model.GetType();
				}
			}

			return _realModelType;
		}
	}

	/// <summary>
	/// Creates a cache key for the given container/model type and property name.
	/// </summary>
	/// <param name="containerType">The container type.</param>
	/// <param name="modelType">The model type.</param>
	/// <param name="propertyName">The property name.</param>
	/// <returns>The cache key.</returns>
	private static EfficientTypePropertyKey<Type, string?> CreateCacheKey(Type? containerType, Type modelType, string? propertyName)
	{
		return new EfficientTypePropertyKey<Type, string?>(containerType ?? modelType, propertyName);
	}
}
