// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Binding;

using System;
using System.Linq;

/// <summary>
/// Provides metadata from expressions.
/// </summary>
public static class ExpressionMetadataProvider
{
	public static ModelMetadata? FromStringExpression(string expression, TemplateData templateData, IModelMetadataProvider provider)
	{
		if (string.IsNullOrWhiteSpace(expression))
		{
			return templateData.ModelMetadata;
		}

		var templateDataInfo = TemplateDataEvaluator.Eval(templateData, expression);
		Type? containerType = null;
		Type? modelType = null;
		Func<object?>? modelAccessor = null;
		string? propertyName = null;
		bool valid = (templateDataInfo != null);

		if (templateDataInfo != null)
		{
			if (templateDataInfo.Container != null)
			{
				containerType = templateDataInfo.Container.GetType();
			}

			modelAccessor = () => templateDataInfo.Value;

			if (templateDataInfo.PropertyInfo != null)
			{
				propertyName = templateDataInfo.PropertyInfo.Name;
				modelType = templateDataInfo.PropertyInfo.PropertyType;
			}
			else if (templateDataInfo.Value != null)
			{
				modelType = templateDataInfo.Value.GetType();
			}
		}
		else if (templateData.ModelMetadata != null)
		{
			var propertyMetadata = templateData.ModelMetadata.Properties.FirstOrDefault(p => p.PropertyName == expression);
			if (propertyMetadata != null)
			{
				return propertyMetadata;
			}
		}

		var metadata = GetMetadataFromProvider(modelAccessor, modelType ?? typeof(string), propertyName, containerType, provider);
		if (!valid)
		{
			metadata.Valid = false;
		}
		return metadata;
	}

	private static ModelMetadata GetMetadataFromProvider(Func<object?>? modelAccessor, Type modelType, string? propertyName, Type? containerType, IModelMetadataProvider provider)
	{
		if (containerType != null && !string.IsNullOrWhiteSpace(propertyName))
		{
			return provider.GetMetadataForProperty(modelAccessor, containerType, propertyName);
		}

		return provider.GetMetadataForType(modelAccessor, modelType);
	}
}
