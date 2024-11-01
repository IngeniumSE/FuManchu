// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Represents the collection of available operators.
/// </summary>
public class OperatorCollection : ICollection<IOperator>
{
	private readonly Dictionary<string, IOperator> _operators = new Dictionary<string, IOperator>();
	public static readonly OperatorCollection Default = new OperatorCollection();

	/// <summary>
	/// Initialises the <see cref="OperatorCollection"/> type.
	/// </summary>
	static OperatorCollection()
	{
		// Add the default operators.
		Default.Add<StandardOperatorProvider>();
	}

	/// <summary>
	/// Initialises a new instance of <see cref="OperatorCollection"/>
	/// </summary>
	public OperatorCollection() : this(Default) { }

	/// <summary>
	/// Initialises a new instance of <see cref="OperatorCollection"/>
	/// </summary>
	/// <param name="operators">The set of operators.</param>
	public OperatorCollection(IEnumerable<IOperator> operators)
	{
		if (operators != null)
		{
			Add(operators.ToArray());
		}
	}

	/// <summary>
	/// Initialises a new instance of <see cref="OperatorCollection"/>
	/// </summary>
	/// <param name="providers">The set of operator providers.</param>
	public OperatorCollection(IEnumerable<IOperatorProvider> providers)
	{
		if (providers != null)
		{
			Add(providers.ToArray());
		}
	}

	/// <inheritdoc />
	public int Count { get { return _operators.Count; } }

	/// <inheritdoc />
	public bool IsReadOnly { get { return false; } }

	/// <inheritdoc />
	public void Add(IOperator @operator)
	{
		if (@operator == null)
		{
			throw new ArgumentNullException("operator");
		}

		if (!Contains(@operator))
		{
			_operators.Add(@operator.Name, @operator);
		}
	}

	/// <summary>
	/// Adds the set of operators provided.
	/// </summary>
	/// <param name="operators">The set of operators.</param>
	public void Add(params IOperator[] operators)
	{
		if (operators == null)
		{
			throw new ArgumentNullException("operators");
		}

		if (operators.Length == 0)
		{
			throw new ArgumentException("At least 1 operator must be provided.");
		}

		foreach (var @operator in operators)
		{
			Add(@operator);
		}
	}

	/// <summary>
	/// Adds the set of operators given by the specified providers.
	/// </summary>
	/// <param name="providers">The set of providers.</param>
	public void Add(params IOperatorProvider[] providers)
	{
		if (providers == null)
		{
			throw new ArgumentNullException("providers");
		}

		if (providers.Length == 0)
		{
			throw new ArgumentException("At least 1 provided must be provided.");
		}

		foreach (var provider in providers)
		{
			var @operators = provider.GetOperators().ToArray();
			Add(@operators);
		}
	}

	/// <summary>
	/// Adds the operators given by the specified provider.
	/// </summary>
	/// <typeparam name="T">The operator provider type.</typeparam>
	public void Add<T>() where T : IOperatorProvider, new()
	{
		Add(new T());
	}

	/// <inheritdoc />
	public void Clear()
	{
		_operators.Clear();
	}

	/// <inheritdoc />
	public bool Contains(IOperator @operator)
	{
		if (@operator == null)
		{
			throw new ArgumentNullException("operator");
		}

		return Contains(@operator.Name);
	}

	/// <summary>
	/// Determines if the operator collection contains the given operator.
	/// </summary>
	/// <param name="name">The operator name.</param>
	/// <returns>True if the operator exists, otherwise false.</returns>
	public bool Contains(string name)
	{
		return _operators.ContainsKey(name);
	}

	/// <inheritdoc />
	public void CopyTo(IOperator[] array, int arrayIndex)
	{
		_operators.Values.CopyTo(array, arrayIndex);
	}

	/// <inheritdoc />
	public IEnumerator<IOperator> GetEnumerator()
	{
		return _operators.Values.GetEnumerator();
	}

	/// <summary>
	/// Gets the operator with the given name.
	/// </summary>
	/// <param name="name">The operator name.</param>
	/// <returns>The operator, or null.</returns>
	public IOperator? GetOperator(string name)
	{
		if (Contains(name))
		{
			return _operators[name];
		}

		return null;
	}

	/// <inheritdoc />
	public bool Remove(IOperator @operator)
	{
		if (@operator == null)
		{
			throw new ArgumentNullException("operator");
		}

		return Remove(@operator.Name);
	}

	/// <summary>
	/// Removes the operator with the given name.
	/// </summary>
	/// <param name="name">The name of the operator.</param>
	/// <returns>True if the item was removed, otherwise false.</returns>
	public bool Remove(string name)
	{
		if (Contains(name))
		{
			return _operators.Remove(name);
		}

		return false;
	}

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
