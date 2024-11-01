// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Binding;

using System;

/// <summary>
/// Represents a cache key with pregenerated hashcode.
/// </summary>
/// <typeparam name="T1">The first key type.</typeparam>
/// <typeparam name="T2">The second key type.</typeparam>
internal class EfficientTypePropertyKey<T1, T2> : Tuple<T1, T2>
{
	private readonly int _hashcode;

	/// <summary>
	/// Intialises a new instance of <see cref="EfficientTypePropertyKey{T1,T2}"/>
	/// </summary>
	/// <param name="item1">The first item.</param>
	/// <param name="item2">The second item.</param>
	public EfficientTypePropertyKey(T1 item1, T2 item2) : base(item1, item2)
	{
		_hashcode = base.GetHashCode();
	}

	/// <inheritdoc />
	public override int GetHashCode()
	{
		return _hashcode;
	}
}
