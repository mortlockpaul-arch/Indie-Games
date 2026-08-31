using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace _0001
{
	internal class _7 : B
	{
	}
}
namespace _0003
{
	[CompilerGenerated]
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
	[DebuggerNonUserCode]
	internal class _7
	{
		private static ResourceManager HCB;

		private static CultureInfo HC_0002;

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (object.ReferenceEquals(HCB, null))
				{
					ResourceManager hCB = new ResourceManager("SynapseGaming.LightingSystem.Effects.Resources-Xbox360", typeof(_7).Assembly);
					HCB = hCB;
				}
				return HCB;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return HC_0002;
			}
			set
			{
				HC_0002 = cultureInfo;
			}
		}

		internal static byte[] BillboardEffect
		{
			get
			{
				object obj = ResourceManager.GetObject("BillboardEffect", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] Black
		{
			get
			{
				object obj = ResourceManager.GetObject("Black", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] ConsoleFont
		{
			get
			{
				object obj = ResourceManager.GetObject("ConsoleFont", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] DefaultEffect
		{
			get
			{
				object obj = ResourceManager.GetObject("DefaultEffect", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] DeferredDepthEffect
		{
			get
			{
				object obj = ResourceManager.GetObject("DeferredDepthEffect", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] DeferredLightingEffect
		{
			get
			{
				object obj = ResourceManager.GetObject("DeferredLightingEffect", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] DeferredObjectEffect
		{
			get
			{
				object obj = ResourceManager.GetObject("DeferredObjectEffect", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] DeferredTerrainEffect
		{
			get
			{
				object obj = ResourceManager.GetObject("DeferredTerrainEffect", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] FogEffect
		{
			get
			{
				object obj = ResourceManager.GetObject("FogEffect", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] FullSphere
		{
			get
			{
				object obj = ResourceManager.GetObject("FullSphere", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] HighDynamicRange
		{
			get
			{
				object obj = ResourceManager.GetObject("HighDynamicRange", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] LightingEffect
		{
			get
			{
				object obj = ResourceManager.GetObject("LightingEffect", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] Normal
		{
			get
			{
				object obj = ResourceManager.GetObject("Normal", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] ShadowEffect
		{
			get
			{
				object obj = ResourceManager.GetObject("ShadowEffect", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] SplashScreen
		{
			get
			{
				object obj = ResourceManager.GetObject("SplashScreen", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] TerrainEffect
		{
			get
			{
				object obj = ResourceManager.GetObject("TerrainEffect", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] VolumeLightBeam
		{
			get
			{
				object obj = ResourceManager.GetObject("VolumeLightBeam", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] VolumeLightEffect
		{
			get
			{
				object obj = ResourceManager.GetObject("VolumeLightEffect", HC_0002);
				return (byte[])obj;
			}
		}

		internal static byte[] White
		{
			get
			{
				object obj = ResourceManager.GetObject("White", HC_0002);
				return (byte[])obj;
			}
		}

		internal _7()
		{
		}
	}
}
