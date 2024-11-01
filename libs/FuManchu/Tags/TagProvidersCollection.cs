// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Tags;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using FuManchu.Renderer;

/// <summary>
/// Represents the collection of available tag providers.
/// </summary>
public class TagProvidersCollection : ICollection<ITagProvider>
{
	private readonly List<ITagProvider> _providers = new List<ITagProvider>();
	public static readonly TagProvidersCollection Default = new TagProvidersCollection();

	/// <summary>
	/// Initialises the <see cref="TagProvidersCollection"/> type.
	/// </summary>
	static TagProvidersCollection()
	{
		// Add the default tags.
		Default.Add<StandardTagProvider>();
	}

	/// <summary>
	/// Initialises a new instance of <see cref="TagProvidersCollection"/>
	/// </summary>
	public TagProvidersCollection() : this(Default) { }

	/// <summary>
	/// Initialises a new instance of <see cref="TagProvidersCollection"/>
	/// </summary>
	/// <param name="providers">The set of providers.</param>
	public TagProvidersCollection(IEnumerable<ITagProvider> providers)
	{
		if (providers != null)
		{
			_providers.AddRange(providers);
		}
	}

	/// <inheritdoc />
	public int Count { get { return _providers.Count; } }

	/// <inheritdoc />
	public bool IsReadOnly { get { return false; } }

	/// <inheritdoc />
	public void Add(ITagProvider item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}

		if (!Contains(item))
		{
			_providers.Add(item);
		}
	}

	public void Add<T>() where T : ITagProvider, new()
	{
		Add(new T());
	}

	/// <inheritdoc />
	public void Clear()
	{
		_providers.Clear();
	}

	/// <inheritdoc />
	public bool Contains(ITagProvider item)
	{
		if (item == null)
		{
			throw new ArgumentNullException("item");
		}

		var type = item.GetType();
		return _providers.FirstOrDefault(p => p.GetType() == type) != null;
	}

	public bool Contains<T>() where T : ITagProvider
	{
		return _providers.FirstOrDefault(p => p is T) != null;
	}

	/// <inheritdoc />
	public void CopyTo(ITagProvider[] array, int arrayIndex)
	{
		_providers.CopyTo(array, arrayIndex);
	}

	public TagDescriptor GetDescriptor(string tagName, bool isNegatedSection)
	{
		if (isNegatedSection)
		{
			// Negated sections (blocks) cannot be used with built-in tags.
			return CreateImplictTagDescriptor(tagName);
		}

		return _providers.SelectMany(p => p.GetTags()).FirstOrDefault(p => string.Equals(p.Name, tagName, StringComparison.OrdinalIgnoreCase))
					 ?? CreateImplictTagDescriptor(tagName);
	}

	public static TagDescriptor CreateImplictTagDescriptor(string tagName)
	{
		return new TagDescriptor(tagName, new ImplicitBlockRenderer(), requiredArguments: 0, maxArguments: 0, allowMappedParamters: false, hasChildContent: true)
		{
			IsImplicit = true
		};
	}

	/// <inheritdoc />
	public IEnumerator<ITagProvider> GetEnumerator()
	{
		return _providers.GetEnumerator();
	}

	/// <inheritdoc />
	public bool Remove(ITagProvider item)
	{
		return _providers.Remove(item);
	}

	public bool Remove<T>() where T : ITagProvider
	{
		var provider = _providers.FirstOrDefault(p => p is T);
		if (provider == null)
		{
			return false;
		}

		return Remove(provider);
	}

	/// <inheritdoc />
	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
