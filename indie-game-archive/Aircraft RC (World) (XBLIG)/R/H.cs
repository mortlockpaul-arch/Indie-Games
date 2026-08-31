using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;

namespace R;

internal class H
{
	private Dictionary<Type, MemberInfo[]> HCB = new Dictionary<Type, MemberInfo[]>(16);

	private Dictionary<string, Type> HC_0002 = new Dictionary<string, Type>(16);

	internal void _7h()
	{
		_7S(typeof(ContentRepository));
		_7S(typeof(Vector2));
		_7S(typeof(Vector3));
		_7S(typeof(Vector4));
		_7S(typeof(Matrix));
		_7S(typeof(Scene));
		_7S(typeof(SceneEnvironment));
	}

	internal void G()
	{
		HCB.Clear();
		HC_0002.Clear();
	}

	internal Type _7T(string P_0, string P_1, string P_2)
	{
		if (HC_0002.TryGetValue(P_2, out var value))
		{
			return value;
		}
		value = _7r(P_0, P_1, P_2);
		if ((object)value == null)
		{
			throw new Exception($"Type '{P_0}' not registered with the serialization type dictionary.");
		}
		_7S(P_2, value);
		return value;
	}

	internal void _5(string P_0, Type P_1)
	{
		if (!string.IsNullOrEmpty(P_0) && !HC_0002.ContainsKey(P_0))
		{
			_7S(P_0, P_1);
		}
	}

	internal void _7a(string P_0, out Type P_1, out MemberInfo[] P_2)
	{
		if (string.IsNullOrEmpty(P_0) || !HC_0002.ContainsKey(P_0))
		{
			throw new Exception("Type '" + P_0 + "' not registered with the serialization type dictionary.");
		}
		P_1 = HC_0002[P_0];
		P_2 = HCB[P_1];
	}

	private void _7S(Type P_0)
	{
		string name = P_0.Name;
		if (!string.IsNullOrEmpty(name) && !HC_0002.ContainsKey(name))
		{
			_7S(name, P_0);
		}
	}

	private void _7S(string P_0, Type P_1)
	{
		HC_0002.Add(P_0, P_1);
		MemberInfo[] value = ((!P_1.IsGenericTypeDefinition) ? _0002._7G(P_1) : new MemberInfo[0]);
		if (!HCB.ContainsKey(P_1))
		{
			HCB.Add(P_1, value);
		}
	}

	internal static Type _7r(string P_0, string P_1, string P_2)
	{
		Type type = Type.GetType($"{P_0}, {P_1}");
		if ((object)type != null)
		{
			return type;
		}
		type = Type.GetType(P_0);
		if ((object)type != null)
		{
			return type;
		}
		P_0 = _7J(P_0);
		type = Type.GetType(P_0);
		if ((object)type != null)
		{
			return type;
		}
		return null;
	}

	private static string _7J(string P_0)
	{
		if (P_0.StartsWith("SynapseGaming.LightingSystem.Core.IComponent"))
		{
			return P_0.Replace("SynapseGaming.LightingSystem.Core.IComponent", "SynapseGaming.LightingSystem.Components.IComponent");
		}
		return P_0;
	}
}
