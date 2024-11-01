// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace FuManchu.Binding;

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

/// <summary>
/// Providers helepr methods for working with properties.
/// </summary>
internal class PropertyHelper
{
	private delegate TValue ByRefFunc<TDeclaringType, out TValue>(ref TDeclaringType arg);

	private static readonly MethodInfo CallPropertyGetterOpenGenericMethod = typeof(PropertyHelper).GetTypeInfo().GetDeclaredMethod("CallPropertyGetter");
	private static readonly MethodInfo CallPropertyGetterByReferenceOpenGenericMethod = typeof(PropertyHelper).GetTypeInfo().GetDeclaredMethod("CallPropertyGetterByReference");
	private static readonly ConcurrentDictionary<Type, PropertyHelper[]> ReflectionCache = new ConcurrentDictionary<Type, PropertyHelper[]>();
	private readonly Func<object, object> _valueGetter;

	/// <summary>
	/// Initialises a new instance of <see cref="PropertyHelper"/>
	/// </summary>
	/// <param name="property">The property.</param>
	public PropertyHelper(PropertyInfo property)
	{
		Property = property;
		Name = property.Name;
		_valueGetter = MakeFastPropertyGetter(property);
	}

	/// <summary>
	/// Gets the property name.
	/// </summary>
	public virtual string Name { get; protected set; }

	/// <summary>
	/// Gets the property.
	/// </summary>
	public PropertyInfo Property { get; private set; }

	/// <summary>
	/// Gets the set of property helpers for the given container type.
	/// </summary>
	/// <param name="type">The container type.</param>
	/// <returns>The set of property helpers.</returns>
	public static PropertyHelper[] GetProperties(Type type)
	{
		return GetProperties(type, CreateInstance, ReflectionCache);
	}

	/// <summary>
	/// Gets the set of property helpers for the given container instance.
	/// </summary>
	/// <param name="container">The container instance.</param>
	/// <returns>The set of property helpers.</returns>
	public static PropertyHelper[] GetProperties(object container)
	{
		return GetProperties(container.GetType());
	}

	/// <summary>
	/// Gets the value of the property from the given instance.
	/// </summary>
	/// <param name="instance">the container instance.</param>
	/// <returns>The property value.</returns>
	public object GetValue(object instance)
	{
		return _valueGetter(instance);
	}

	/// <summary>
	/// Creates a single fast property getter.
	/// </summary>
	/// <param name="propertyInfo">The property.</param>
	/// <returns>The property getter.</returns>
	public static Func<object, object> MakeFastPropertyGetter(PropertyInfo propertyInfo)
	{
		var getMethod = propertyInfo.GetMethod;

		var typeInput = getMethod.DeclaringType;
		var typeInputInfo = typeInput.GetTypeInfo();
		var typeOutput = getMethod.ReturnType;

		Delegate callPropertyGetterDelegate;
		if (typeInputInfo.IsValueType)
		{
			var delegateType = typeof(ByRefFunc<,>).MakeGenericType(typeInput, typeOutput);
			var propertyGetterAsFunc = getMethod.CreateDelegate(delegateType);
			var callPropertyGetterClosedGenericMethod = CallPropertyGetterByReferenceOpenGenericMethod.MakeGenericMethod(typeInput, typeOutput);
			callPropertyGetterDelegate = callPropertyGetterClosedGenericMethod.CreateDelegate(typeof(Func<object, object>), propertyGetterAsFunc);
		}
		else
		{
			var propertyGetterAsFunc = getMethod.CreateDelegate(typeof(Func<,>).MakeGenericType(typeInput, typeOutput));
			var callPropertyGetterClosedGenericMethod = CallPropertyGetterOpenGenericMethod.MakeGenericMethod(typeInput, typeOutput);
			callPropertyGetterDelegate = callPropertyGetterClosedGenericMethod.CreateDelegate(typeof(Func<object, object>), propertyGetterAsFunc);
		}

		return (Func<object, object>)callPropertyGetterDelegate;
	}

	/// <summary>
	/// Creates an instance of <see cref="PropertyHelper"/> for the given property information.
	/// </summary>
	/// <param name="propertyInfo">The property.</param>
	/// <returns>The property helper.</returns>
	private static PropertyHelper CreateInstance(PropertyInfo propertyInfo)
	{
		return new PropertyHelper(propertyInfo);
	}

	/// <summary>
	/// Calls the property getter for the given reference type.
	/// </summary>
	/// <typeparam name="TDeclaringType">The reference type.</typeparam>
	/// <typeparam name="TValue">The value type.</typeparam>
	/// <param name="getter">The getter delegate</param>
	/// <param name="target">The target container.</param>
	/// <returns>The property value.</returns>
	private static object? CallPropertyGetter<TDeclaringType, TValue>(Func<TDeclaringType, TValue> getter, object target)
	{
		return getter((TDeclaringType)target);
	}

	/// <summary>
	/// Calls the property getter for the given value type, called by reference.
	/// </summary>
	/// <typeparam name="TDeclaringType">The reference type.</typeparam>
	/// <typeparam name="TValue">The value type.</typeparam>
	/// <param name="getter">The getter delegate</param>
	/// <param name="target">The target container.</param>
	/// <returns>The property value.</returns>
	private static object? CallPropertyGetterByReference<TDeclaringType, TValue>(ByRefFunc<TDeclaringType, TValue> getter, object target)
	{
		var unboxed = (TDeclaringType)target;
		return getter(ref unboxed);
	}

	/// <summary>
	/// Gets the properties for the given type.
	/// </summary>
	/// <param name="type">The container type.</param>
	/// <param name="createPropertyHelper">The delegate used to create a property helper instance.</param>
	/// <param name="cache">The cache of available lookups.</param>
	/// <returns>The property helpers.</returns>
	protected static PropertyHelper[] GetProperties(Type type, Func<PropertyInfo, PropertyHelper> createPropertyHelper, ConcurrentDictionary<Type, PropertyHelper[]> cache)
	{
		type = Nullable.GetUnderlyingType(type) ?? type;

		PropertyHelper[] helpers;

		if (!cache.TryGetValue(type, out helpers))
		{
			var properties = type.GetRuntimeProperties().Where(
				p => p.GetIndexParameters().Length == 0 && p.GetMethod != null && p.GetMethod.IsPublic && !p.GetMethod.IsStatic);

			helpers = properties.Select(createPropertyHelper).ToArray();
			cache.TryAdd(type, helpers);
		}

		return helpers;
	}
}
