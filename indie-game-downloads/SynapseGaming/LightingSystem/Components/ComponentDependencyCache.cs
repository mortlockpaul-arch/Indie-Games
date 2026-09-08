using System;
using System.Collections.Generic;

namespace SynapseGaming.LightingSystem.Components;

/// <summary>
/// Used to find and cache a component's dependencies.
/// </summary>
public class ComponentDependencyCache
{
	private static Dictionary<Type, List<Type>> _3A_0018 = new Dictionary<Type, List<Type>>(16);

	/// <summary>
	/// Find and cache the component's dependencies.
	/// </summary>
	/// <param name="componenttype"></param>
	/// <returns>List of types the component is dependent on.
	///
	/// WARNING: the list is a shared reference. Do not alter the list or it will
	/// affect internal component dependencies.</returns>
	public static List<Type> GetDependencies(Type componenttype)
	{
		if (_3A_0018.TryGetValue(componenttype, out var value))
		{
			return value;
		}
		value = new List<Type>(4);
		value.Add(componenttype);
		o(componenttype, value);
		_3A_0018.Add(componenttype, value);
		return value;
	}

	private static void o(Type P_0, List<Type> P_1)
	{
		object[] customAttributes = P_0.GetCustomAttributes(typeof(ComponentDependencyAttribute), inherit: true);
		object[] array = customAttributes;
		foreach (object obj in array)
		{
			if (obj is ComponentDependencyAttribute componentDependencyAttribute && !P_1.Contains(componentDependencyAttribute.Dependency))
			{
				P_1.Add(componentDependencyAttribute.Dependency);
				o(componentDependencyAttribute.Dependency, P_1);
			}
		}
	}
}
