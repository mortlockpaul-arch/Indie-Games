using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace _8;

[DebuggerNonUserCode]
[CompilerGenerated]
[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
internal class _6
{
	private static ResourceManager _3A_0018;

	private static CultureInfo _3AL;

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static ResourceManager ResourceManager
	{
		get
		{
			if (object.ReferenceEquals(_3A_0018, null))
			{
				ResourceManager resourceManager = new ResourceManager("SynapseGaming.LightingSystem.Effects.Resources-Xbox360", typeof(_6).Assembly);
				_3A_0018 = resourceManager;
			}
			return _3A_0018;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Advanced)]
	internal static CultureInfo Culture
	{
		get
		{
			return _3AL;
		}
		set
		{
			_3AL = cultureInfo;
		}
	}

	internal static byte[] BillboardEffect
	{
		get
		{
			object obj = ResourceManager.GetObject("BillboardEffect", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] Black
	{
		get
		{
			object obj = ResourceManager.GetObject("Black", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] ConsoleFont
	{
		get
		{
			object obj = ResourceManager.GetObject("ConsoleFont", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] DefaultEffect
	{
		get
		{
			object obj = ResourceManager.GetObject("DefaultEffect", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] DeferredDepthEffect
	{
		get
		{
			object obj = ResourceManager.GetObject("DeferredDepthEffect", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] DeferredLightingEffect
	{
		get
		{
			object obj = ResourceManager.GetObject("DeferredLightingEffect", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] DeferredObjectEffect
	{
		get
		{
			object obj = ResourceManager.GetObject("DeferredObjectEffect", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] DeferredTerrainEffect
	{
		get
		{
			object obj = ResourceManager.GetObject("DeferredTerrainEffect", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] FogEffect
	{
		get
		{
			object obj = ResourceManager.GetObject("FogEffect", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] FullSphere
	{
		get
		{
			object obj = ResourceManager.GetObject("FullSphere", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] HighDynamicRange
	{
		get
		{
			object obj = ResourceManager.GetObject("HighDynamicRange", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] LightingEffect
	{
		get
		{
			object obj = ResourceManager.GetObject("LightingEffect", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] Normal
	{
		get
		{
			object obj = ResourceManager.GetObject("Normal", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] ShadowEffect
	{
		get
		{
			object obj = ResourceManager.GetObject("ShadowEffect", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] SplashScreen
	{
		get
		{
			object obj = ResourceManager.GetObject("SplashScreen", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] TerrainEffect
	{
		get
		{
			object obj = ResourceManager.GetObject("TerrainEffect", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] VolumeLightBeam
	{
		get
		{
			object obj = ResourceManager.GetObject("VolumeLightBeam", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] VolumeLightEffect
	{
		get
		{
			object obj = ResourceManager.GetObject("VolumeLightEffect", _3AL);
			return (byte[])obj;
		}
	}

	internal static byte[] White
	{
		get
		{
			object obj = ResourceManager.GetObject("White", _3AL);
			return (byte[])obj;
		}
	}

	internal _6()
	{
	}
}
