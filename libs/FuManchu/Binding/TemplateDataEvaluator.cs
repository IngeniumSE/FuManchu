// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Binding;

using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// Provides evaluation of dynamic expressions against template data.
/// </summary>
public static class TemplateDataEvaluator
{
	public static TemplateDataInfo? Eval(TemplateData templateData, string expression)
	{
		return EvalComplexExpression(templateData, expression);
	}

	public static TemplateDataInfo? Eval(object indexableObject, string expression)
	{
		return (indexableObject == null) ? null : EvalComplexExpression(indexableObject, expression);
	}

	private static TemplateDataInfo? EvalComplexExpression(object indexableObject, string expression)
	{
		foreach (var pair in GetRightToLeftExpressions(expression))
		{
			var subExpression = pair.Left;
			var postExpression = pair.Right;

			var subTargetInfo = GetPropertyValue(indexableObject, subExpression);
			if (subTargetInfo != null)
			{
				if (string.IsNullOrWhiteSpace(postExpression))
				{
					return subTargetInfo;
				}
				if (subTargetInfo.Value != null)
				{
					var potential = EvalComplexExpression(subTargetInfo.Value, postExpression);
					if (potential != null)
					{
						return potential;
					}
				}
			}
		}

		return null;
	}

	private static IEnumerable<ExpressionPair> GetRightToLeftExpressions(string expression)
	{
		yield return new ExpressionPair(expression, string.Empty);

		var lastDot = expression.LastIndexOf('.');

		var subExpression = expression;
		var postExpression = string.Empty;

		while (lastDot > -1)
		{
			subExpression = expression.Substring(0, lastDot);
			postExpression = expression.Substring(lastDot + 1);
			yield return new ExpressionPair(subExpression, postExpression);

			lastDot = subExpression.LastIndexOf('.');
		}
	}

	private static TemplateDataInfo? GetIndexPropertyValue(object? indexableObject, string key)
	{
		var dict = indexableObject as Map;
		object? value = null;
		bool success = false;

		if (dict != null)
		{
			string lookupKey = key;
			var periodIndex = key.IndexOf('.');
			if (periodIndex > -1)
			{
				lookupKey = key.Substring(0, periodIndex);
				key = key.Substring(periodIndex + 1);
			}

			success = dict.TryGetValue(lookupKey, out value);
			if (success && value != null && key.Length > 0 && key.Length != lookupKey.Length)
			{
				return EvalComplexExpression(value!, key);
			}
		}
		else if (indexableObject != null)
		{
			var tryDelegate = TryGetValueProvider.CreateInstance(indexableObject.GetType());
			if (tryDelegate != null)
			{
				success = tryDelegate(indexableObject, key, out value);
			}
		}

		if (success)
		{
			return new TemplateDataInfo(indexableObject, value);
		}

		return null;
	}

	private static TemplateDataInfo? GetPropertyValue(object? container, string propertyName)
	{

		var templateData = container as TemplateData;
		if (templateData != null)
		{
			container = templateData.Model;
		}

		var value = GetIndexPropertyValue(container, propertyName);
		if (value != null)
		{
			return value;
		}

		if (container == null)
		{
			return null;
		}

		var propertyInfo = container.GetType().GetRuntimeProperty(propertyName);
		if (propertyInfo == null)
		{
			return null;
		}

		return new TemplateDataInfo(container, propertyInfo, () => propertyInfo.GetValue(container));
	}

	private struct ExpressionPair
	{
		public readonly string Left;
		public readonly string Right;

		public ExpressionPair(string left, string right)
		{
			Left = left;
			Right = right;
		}
	}
}
