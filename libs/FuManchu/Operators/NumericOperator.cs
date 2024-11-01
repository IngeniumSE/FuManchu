// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System;
using System.Reflection;

/// <summary>
/// Provides a of a numeric operator.
/// </summary>
public class NumericOperator : OperatorBase
{
	private readonly NumericOperation _operation;
	private readonly static Type _generatorType = typeof(ValueTypeExpressionGenerator);
	private readonly static MethodInfo _greaterThanMethod = _generatorType.GetTypeInfo().GetMethod("GreaterThan");
	private readonly static MethodInfo _greaterThanEqualToMethod = _generatorType.GetTypeInfo().GetMethod("GreaterThanEqualTo");
	private readonly static MethodInfo _lessThanMethod = _generatorType.GetTypeInfo().GetMethod("LessThan");
	private readonly static MethodInfo _lessThanEqualToMethod = _generatorType.GetTypeInfo().GetMethod("LessThanEqualTo");

	/// <summary>
	/// Initialises a new instance of <see cref="NumericOperator"/>
	/// </summary>
	/// <param name="operation">The name of the numeric operator.</param>
	public NumericOperator(NumericOperation operation)
: base(GetName(operation))
	{
		_operation = operation;
	}

	/// <inheritdoc />
	public override bool Result(object? x, object? y)
	{
		if (x == null || y == null)
		{
			return false;
		}

		var xType = x.GetType();
		var yType = y.GetType();

		if (xType.GetTypeInfo().IsValueType)
		{
			object? yConverted = null;
			try
			{
				yConverted = Convert.ChangeType(y, xType);
			}
			catch { return false; }

			var func = GetDelegate(xType);

			return (bool)func.DynamicInvoke(x, yConverted);
		}

		if (xType == yType || xType.GetTypeInfo().IsAssignableFrom(yType))
		{
			var xComparable = x as IComparable;
			if (xComparable != null)
			{
				return CompareTo(xComparable, y);
			}
		}

		return false;
	}

	/// <summary>
	/// Compares the given values using the <see cref="IComparable"/> interface.
	/// </summary>
	/// <param name="comparable">The comparable instance.</param>
	/// <param name="y">The second operand.</param>
	/// <returns>True if the value</returns>
	private bool CompareTo(IComparable comparable, object y)
	{
		switch (_operation)
		{
			case NumericOperation.GreaterThan:
				return comparable.CompareTo(y) > 0;
			case NumericOperation.GreaterThanEqualTo:
				return comparable.CompareTo(y) >= 0;
			case NumericOperation.LessThan:
				return comparable.CompareTo(y) < 0;
			case NumericOperation.LessThanEqualTo:
				return comparable.CompareTo(y) <= 0;
			default:
				throw new ArgumentException("The operation '" + _operation.ToString() + "' is not supported.");
		}
	}

	/// <summary>
	/// Gets the delegate used to perform the operation.
	/// </summary>
	/// <param name="type">The value type.</param>
	/// <returns>The expression delegate.</returns>
	private Delegate GetDelegate(Type type)
	{
		MethodInfo? method = null;

		switch (_operation)
		{
			case NumericOperation.GreaterThan:
				method = _greaterThanMethod.MakeGenericMethod(type);
				break;
			case NumericOperation.GreaterThanEqualTo:
				method = _greaterThanEqualToMethod.MakeGenericMethod(type);
				break;
			case NumericOperation.LessThan:
				method = _lessThanMethod.MakeGenericMethod(type);
				break;
			case NumericOperation.LessThanEqualTo:
				method = _lessThanEqualToMethod.MakeGenericMethod(type);
				break;
		}

		return (Delegate)method!.Invoke(null, null);
	}

	/// <summary>
	/// Gets the name of the operator.
	/// </summary>
	/// <param name="operation">The numeric operation.</param>
	/// <returns>The operator name.</returns>
	private static string GetName(NumericOperation operation)
	{
		switch (operation)
		{
			case NumericOperation.GreaterThan:
				return ">";
			case NumericOperation.GreaterThanEqualTo:
				return ">=";
			case NumericOperation.LessThan:
				return "<";
			case NumericOperation.LessThanEqualTo:
				return "<=";
			default:
				throw new ArgumentException("The operation '" + operation.ToString() + "' is not supported.");
		}
	}
}
