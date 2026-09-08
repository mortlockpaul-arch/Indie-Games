using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace T;

internal static class a
{
	public static _0006 BouncinessBlendMethod;

	public static v BouncinessBlender;

	public static _0006 FrictionBlendMethod;

	public static v FrictionBlender;

	public static float DefaultKineticFriction;

	public static float DefaultStaticFriction;

	public static float DefaultBounciness;

	[CompilerGenerated]
	private static Dictionary<_7, T.b> a5h;

	public static Dictionary<_7, T.b> MaterialInteractions
	{
		[CompilerGenerated]
		get
		{
			return a5h;
		}
		[CompilerGenerated]
		set
		{
			a5h = value;
		}
	}

	static a()
	{
		BouncinessBlendMethod = _0006.Max;
		FrictionBlendMethod = _0006.Average;
		DefaultKineticFriction = 0.6f;
		DefaultStaticFriction = 0.8f;
		FrictionBlender = DefaultFrictionBlender;
		BouncinessBlender = DefaultBouncinessBlender;
		MaterialInteractions = new Dictionary<_7, T.b>();
	}

	public static void GetInteractionProperties(T._6 materialA, T._6 materialB, out T.b properties)
	{
		if (!MaterialInteractions.TryGetValue(new _7(materialA, materialB), out properties))
		{
			properties = default(T.b);
			properties.StaticFriction = FrictionBlender(materialA.a5b, materialB.a5b, null);
			properties.KineticFriction = FrictionBlender(materialA.a5h, materialB.a5h, null);
			properties.Bounciness = BouncinessBlender(materialA.a56, materialB.a56, null);
		}
	}

	public static void GetStaticFriction(T._6 materialA, T._6 materialB, out float blendedCoefficient)
	{
		if (materialA != null && materialB != null && MaterialInteractions.TryGetValue(new _7(materialA, materialB), out var value))
		{
			blendedCoefficient = value.StaticFriction;
		}
		else
		{
			blendedCoefficient = FrictionBlender(materialA.a5b, materialB.a5b, null);
		}
	}

	public static void GetKineticFriction(T._6 materialA, T._6 materialB, out float blendedCoefficient)
	{
		if (materialA != null && materialB != null && MaterialInteractions.TryGetValue(new _7(materialA, materialB), out var value))
		{
			blendedCoefficient = value.KineticFriction;
		}
		else
		{
			blendedCoefficient = FrictionBlender(materialA.KineticFriction, materialB.KineticFriction, null);
		}
	}

	public static void GetBounciness(T._6 materialA, T._6 materialB, out float blendedCoefficient)
	{
		if (materialA != null && materialB != null && MaterialInteractions.TryGetValue(new _7(materialA, materialB), out var value))
		{
			blendedCoefficient = value.Bounciness;
		}
		else
		{
			blendedCoefficient = BouncinessBlender(materialA.Bounciness, materialB.Bounciness, null);
		}
	}

	public static void GetStaticFriction(T._6 materialA, T._6 materialB, v blender, out float blendedCoefficient)
	{
		if (materialA != null && materialB != null && MaterialInteractions.TryGetValue(new _7(materialA, materialB), out var value))
		{
			blendedCoefficient = value.StaticFriction;
		}
		else
		{
			blendedCoefficient = blender(materialA.a5b, materialB.a5b, null);
		}
	}

	public static void GetKineticFriction(T._6 materialA, T._6 materialB, v blender, out float blendedCoefficient)
	{
		if (materialA != null && materialB != null && MaterialInteractions.TryGetValue(new _7(materialA, materialB), out var value))
		{
			blendedCoefficient = value.KineticFriction;
		}
		else
		{
			blendedCoefficient = blender(materialA.KineticFriction, materialB.KineticFriction, null);
		}
	}

	public static void GetBounciness(T._6 materialA, T._6 materialB, v blender, out float blendedCoefficient)
	{
		if (materialA != null && materialB != null && MaterialInteractions.TryGetValue(new _7(materialA, materialB), out var value))
		{
			blendedCoefficient = value.Bounciness;
		}
		else
		{
			blendedCoefficient = blender(materialA.Bounciness, materialB.Bounciness, null);
		}
	}

	public static float DefaultBouncinessBlender(float aValue, float bValue, object extraInfo)
	{
		return BouncinessBlendMethod switch
		{
			_0006.Average => (aValue + bValue) / 2f, 
			_0006.Max => Math.Max(aValue, bValue), 
			_0006.Min => Math.Min(aValue, bValue), 
			_0006.BiasHigh => Math.Max(aValue, bValue) * 0.75f + Math.Min(aValue, bValue) * 0.25f, 
			_0006.BiasLow => Math.Max(aValue, bValue) * 0.25f + Math.Min(aValue, bValue) * 0.75f, 
			_ => (aValue + bValue) / 2f, 
		};
	}

	public static float DefaultFrictionBlender(float aValue, float bValue, object extraInfo)
	{
		return FrictionBlendMethod switch
		{
			_0006.Average => (aValue + bValue) / 2f, 
			_0006.Max => Math.Max(aValue, bValue), 
			_0006.Min => Math.Min(aValue, bValue), 
			_0006.BiasHigh => Math.Max(aValue, bValue) * 0.75f + Math.Min(aValue, bValue) * 0.25f, 
			_0006.BiasLow => Math.Max(aValue, bValue) * 0.25f + Math.Min(aValue, bValue) * 0.75f, 
			_ => (aValue + bValue) / 2f, 
		};
	}
}
