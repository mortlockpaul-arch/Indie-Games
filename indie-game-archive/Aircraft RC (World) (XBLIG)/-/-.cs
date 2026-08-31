using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace _0002
{
	internal class _0002
	{
		private static byte[] HCB = new byte[8];

		internal static int z(byte[] P_0, int P_1)
		{
			HCB[3] = P_0[P_1++];
			HCB[2] = P_0[P_1++];
			HCB[1] = P_0[P_1++];
			HCB[0] = P_0[P_1++];
			return BitConverter.ToInt32(HCB, 0);
		}

		internal static uint A(byte[] P_0, int P_1)
		{
			HCB[3] = P_0[P_1++];
			HCB[2] = P_0[P_1++];
			HCB[1] = P_0[P_1++];
			HCB[0] = P_0[P_1++];
			return BitConverter.ToUInt32(HCB, 0);
		}

		internal static char c(byte[] P_0, int P_1)
		{
			HCB[1] = P_0[P_1++];
			HCB[0] = P_0[P_1++];
			return BitConverter.ToChar(HCB, 0);
		}
	}
}
namespace _0001
{
	internal class _0002 : List<B>
	{
	}
	internal class _0012 : B
	{
	}
	internal class _0001
	{
		public _7 DocumentElement;

		public void Load(Stream stream)
		{
			XDocument xDocument = XDocument.Load(stream);
			DocumentElement = new _7();
			DocumentElement.Load(xDocument.Root);
		}
	}
}
namespace _000F
{
	internal delegate void _0002(float percent, string description);
	internal enum _0012
	{
		Error,
		InUse
	}
}
namespace _0003
{
	internal class _0002
	{
		internal enum _0001CB
		{
			DeferredDepth,
			DeferredGBuffer,
			DeferredFinal,
			DeferredFinalFog,
			Lighting,
			Ambient,
			Shadow,
			ShadowGen,
			Fog,
			Billboard
		}

		internal enum _0001C_0002
		{
			None,
			Diffuse,
			DiffuseBump,
			DiffuseBumpSpecular,
			DiffuseBumpSpecularColor,
			DiffuseBumpFresnel,
			DiffuseBumpFresnelColor,
			DiffuseParallax,
			DiffuseParallaxSpecular,
			DiffuseParallaxSpecularColor,
			DiffuseParallaxFresnel,
			DiffuseParallaxFresnelColor,
			DiffuseAmbient,
			DiffuseBumpAmbient,
			DiffuseParallaxAmbient,
			DiffuseAmbientEmissive,
			DiffuseBumpAmbientEmissive,
			DiffuseParallaxAmbientEmissive,
			DiffuseParallaxSpecularColorEmissive,
			DiffuseParallaxEmissive,
			DiffuseBumpSpecularColorEmissive,
			DiffuseBumpEmissive,
			Tangent,
			Linear,
			Point,
			Directional,
			Point3,
			Directional3,
			Point4,
			Directional4,
			Count
		}

		private static Dictionary<int, string> HCB = new Dictionary<int, string>(32);

		private static int HB(_0001CB P_0, _0001C_0002 P_1, int P_2, bool P_3, bool P_4, bool P_5, bool P_6)
		{
			int num = (int)(P_0 + ((int)P_1 << 8));
			num += P_2 << 16;
			if (P_5)
			{
				num += 16777216;
			}
			if (P_3)
			{
				num += 33554432;
			}
			if (P_4)
			{
				num += 67108864;
			}
			if (P_6)
			{
				num += 134217728;
			}
			return num;
		}

		internal static void H_0002()
		{
			Dictionary<int, char> dictionary = new Dictionary<int, char>(16);
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 30; j++)
				{
					for (int k = 0; k < 3; k++)
					{
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, false, false, false, false), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, true, false, false, false), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, false, true, false, false), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, true, true, false, false), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, false, false, true, false), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, true, false, true, false), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, false, true, true, false), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, true, true, true, false), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, false, false, false, true), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, true, false, false, true), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, false, true, false, true), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, true, true, false, true), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, false, false, true, true), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, true, false, true, true), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, false, true, true, true), '0');
						dictionary.Add(HB((_0001CB)i, (_0001C_0002)j, k, true, true, true, true), '0');
					}
				}
			}
		}

		internal static string H_0012(_0001CB P_0, _0001C_0002 P_1, int P_2, bool P_3, bool P_4, bool P_5, bool P_6)
		{
			int key = HB(P_0, P_1, P_2, P_3, P_4, P_5, P_6);
			if (HCB.ContainsKey(key))
			{
				return HCB[key];
			}
			string text = "";
			switch (P_0)
			{
			case _0001CB.DeferredDepth:
				text = "DeferredDepth_";
				break;
			case _0001CB.DeferredGBuffer:
				text = "DeferredGBuffer_";
				break;
			case _0001CB.DeferredFinal:
				text = "DeferredFinal_";
				break;
			case _0001CB.DeferredFinalFog:
				text = "DeferredFinalFog_";
				break;
			case _0001CB.Lighting:
				text = "Lighting_";
				break;
			case _0001CB.Ambient:
				text = "Ambient_";
				break;
			case _0001CB.Shadow:
				text = "Shadow_";
				break;
			case _0001CB.ShadowGen:
				text = "ShadowGen_";
				break;
			case _0001CB.Fog:
				text = "Fog_";
				break;
			case _0001CB.Billboard:
				text = "Billboard_";
				break;
			}
			switch (P_1)
			{
			case _0001C_0002.Diffuse:
				text += "D_";
				break;
			case _0001C_0002.DiffuseBump:
				text += "DB_";
				break;
			case _0001C_0002.DiffuseBumpSpecular:
				text += "DBS_";
				break;
			case _0001C_0002.DiffuseBumpSpecularColor:
				text += "DBSC_";
				break;
			case _0001C_0002.DiffuseBumpFresnel:
				text += "DBF_";
				break;
			case _0001C_0002.DiffuseBumpFresnelColor:
				text += "DBFC_";
				break;
			case _0001C_0002.DiffuseParallax:
				text += "DP_";
				break;
			case _0001C_0002.DiffuseParallaxSpecular:
				text += "DPS_";
				break;
			case _0001C_0002.DiffuseParallaxSpecularColor:
				text += "DPSC_";
				break;
			case _0001C_0002.DiffuseParallaxFresnel:
				text += "DPF_";
				break;
			case _0001C_0002.DiffuseParallaxFresnelColor:
				text += "DPFC_";
				break;
			case _0001C_0002.DiffuseAmbient:
				text += "DA_";
				break;
			case _0001C_0002.DiffuseBumpAmbient:
				text += "DBA_";
				break;
			case _0001C_0002.DiffuseParallaxAmbient:
				text += "DPA_";
				break;
			case _0001C_0002.DiffuseAmbientEmissive:
				text += "DAG_";
				break;
			case _0001C_0002.DiffuseBumpAmbientEmissive:
				text += "DBAG_";
				break;
			case _0001C_0002.DiffuseParallaxAmbientEmissive:
				text += "DPAG_";
				break;
			case _0001C_0002.DiffuseParallaxSpecularColorEmissive:
				text += "DPSCE_";
				break;
			case _0001C_0002.DiffuseParallaxEmissive:
				text += "DPE_";
				break;
			case _0001C_0002.DiffuseBumpSpecularColorEmissive:
				text += "DBSCE_";
				break;
			case _0001C_0002.DiffuseBumpEmissive:
				text += "DBE_";
				break;
			case _0001C_0002.Tangent:
				text += "Tangent_";
				break;
			case _0001C_0002.Linear:
				text += "Linear_";
				break;
			case _0001C_0002.Point:
				text += "Point_";
				break;
			case _0001C_0002.Point3:
				text += "Point3_";
				break;
			case _0001C_0002.Point4:
				text += "Point4_";
				break;
			case _0001C_0002.Directional:
				text += "Directional_";
				break;
			case _0001C_0002.Directional3:
				text += "Directional3_";
				break;
			case _0001C_0002.Directional4:
				text += "Directional4_";
				break;
			}
			if (P_0 == _0001CB.Lighting)
			{
				text = text + "L" + P_2 + "_";
			}
			if (P_3)
			{
				text += "Double_";
			}
			if (P_4)
			{
				text += "Transparent_";
			}
			if (P_5)
			{
				text += "Skinned_";
			}
			if (P_6)
			{
				text += "Terrain_";
			}
			text += "Technique";
			HCB.Add(key, text);
			return text;
		}
	}
	internal interface _0012
	{
		TextureCube ShadowFaceMap { get; set; }

		TextureCube ShadowCoordMap { get; set; }

		Texture2D ShadowMap { get; }

		BoundingSphere ShadowArea { set; }

		Vector4 ShadowViewDistance { get; set; }

		Vector4[] ShadowMapLocationAndSpan { set; }

		Matrix[] ShadowViewProjection { set; }

		DetailPreference EffectDetail { get; set; }

		void SetShadowMapAndType(Texture2D shadowmap, H type);
	}
}
