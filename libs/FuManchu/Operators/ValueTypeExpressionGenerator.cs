// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

/// <summary>
/// Generates expressions for working with value types.
/// </summary>
internal static class ValueTypeExpressionGenerator
{
	private static readonly ConcurrentDictionary<string, Delegate> _delegates = new ConcurrentDictionary<string, Delegate>();

	/// <summary>
	/// Generates a greater-than expression.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	/// <returns>The expression delegate.</returns>
	public static Delegate GreaterThan<T>() where T : struct
	{
		return _delegates.GetOrAdd(Key<T>(">"), k => CreateExpression<T>((x, y) => Expression.GreaterThan(x, y)));
	}

	/// <summary>
	/// Generates a greater-than-equal-to expression.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	/// <returns>The expression delegate.</returns>
	public static Delegate GreaterThanEqualTo<T>() where T : struct
	{
		return _delegates.GetOrAdd(Key<T>(">="), k => CreateExpression<T>((x, y) => Expression.GreaterThanOrEqual(x, y)));
	}

	/// <summary>
	/// Generates a less-than-equal-to expression.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	/// <returns>The expression delegate.</returns>
	public static Delegate LessThan<T>() where T : struct
	{
		return _delegates.GetOrAdd(Key<T>("<"), k => CreateExpression<T>((x, y) => Expression.LessThan(x, y)));
	}

	/// <summary>
	/// Generates a less-than-equal-to expression.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	/// <returns>The expression delegate.</returns>
	public static Delegate LessThanEqualTo<T>() where T : struct
	{
		return _delegates.GetOrAdd(Key<T>("<="), k => CreateExpression<T>((x, y) => Expression.LessThanOrEqual(x, y)));
	}

	/// <summary>
	/// Generates a expression delegate.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	/// <param name="factory">The factory used to create the binary expression.</param>
	/// <returns>The expression.delegate.</returns>
	private static Delegate CreateExpression<T>(Func<ParameterExpression, ParameterExpression, BinaryExpression> factory) where T : struct
	{
		var type = typeof(T);
		var operandX = Expression.Parameter(type);
		var operandY = Expression.Parameter(type);
		var binary = factory(operandX, operandY);
		var lambda = Expression.Lambda(binary, operandX, operandY);

		return lambda.Compile();
	}

	/// <summary>
	/// Generates a cache key for the given type.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	/// <param name="prefix">The key prefix.</param>
	/// <returns>The cache key.</returns>
	private static string Key<T>(string prefix)
	{
		return string.Format("{0}-{1}", prefix, typeof(T).Name);
	}
}
