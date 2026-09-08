using System.Collections.Generic;

namespace _8;

internal class L
{
	internal enum DA_0018
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

	internal enum DAL
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

	private static Dictionary<int, string> _3A_0018 = new Dictionary<int, string>(32);

	private static int _0019A(DA_0018 P_0, DAL P_1, int P_2, bool P_3, bool P_4, bool P_5, bool P_6)
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

	internal static void _3_0018()
	{
		Dictionary<int, char> dictionary = new Dictionary<int, char>(16);
		for (int i = 0; i < 9; i++)
		{
			for (int j = 0; j < 30; j++)
			{
				for (int k = 0; k < 3; k++)
				{
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, false, false, false, false), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, true, false, false, false), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, false, true, false, false), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, true, true, false, false), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, false, false, true, false), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, true, false, true, false), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, false, true, true, false), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, true, true, true, false), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, false, false, false, true), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, true, false, false, true), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, false, true, false, true), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, true, true, false, true), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, false, false, true, true), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, true, false, true, true), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, false, true, true, true), '0');
					dictionary.Add(_0019A((DA_0018)i, (DAL)j, k, true, true, true, true), '0');
				}
			}
		}
	}

	internal static string _3L(DA_0018 P_0, DAL P_1, int P_2, bool P_3, bool P_4, bool P_5, bool P_6)
	{
		int key = _0019A(P_0, P_1, P_2, P_3, P_4, P_5, P_6);
		if (_3A_0018.ContainsKey(key))
		{
			return _3A_0018[key];
		}
		string text = "";
		switch (P_0)
		{
		case DA_0018.DeferredDepth:
			text = "DeferredDepth_";
			break;
		case DA_0018.DeferredGBuffer:
			text = "DeferredGBuffer_";
			break;
		case DA_0018.DeferredFinal:
			text = "DeferredFinal_";
			break;
		case DA_0018.DeferredFinalFog:
			text = "DeferredFinalFog_";
			break;
		case DA_0018.Lighting:
			text = "Lighting_";
			break;
		case DA_0018.Ambient:
			text = "Ambient_";
			break;
		case DA_0018.Shadow:
			text = "Shadow_";
			break;
		case DA_0018.ShadowGen:
			text = "ShadowGen_";
			break;
		case DA_0018.Fog:
			text = "Fog_";
			break;
		case DA_0018.Billboard:
			text = "Billboard_";
			break;
		}
		switch (P_1)
		{
		case DAL.Diffuse:
			text += "D_";
			break;
		case DAL.DiffuseBump:
			text += "DB_";
			break;
		case DAL.DiffuseBumpSpecular:
			text += "DBS_";
			break;
		case DAL.DiffuseBumpSpecularColor:
			text += "DBSC_";
			break;
		case DAL.DiffuseBumpFresnel:
			text += "DBF_";
			break;
		case DAL.DiffuseBumpFresnelColor:
			text += "DBFC_";
			break;
		case DAL.DiffuseParallax:
			text += "DP_";
			break;
		case DAL.DiffuseParallaxSpecular:
			text += "DPS_";
			break;
		case DAL.DiffuseParallaxSpecularColor:
			text += "DPSC_";
			break;
		case DAL.DiffuseParallaxFresnel:
			text += "DPF_";
			break;
		case DAL.DiffuseParallaxFresnelColor:
			text += "DPFC_";
			break;
		case DAL.DiffuseAmbient:
			text += "DA_";
			break;
		case DAL.DiffuseBumpAmbient:
			text += "DBA_";
			break;
		case DAL.DiffuseParallaxAmbient:
			text += "DPA_";
			break;
		case DAL.DiffuseAmbientEmissive:
			text += "DAG_";
			break;
		case DAL.DiffuseBumpAmbientEmissive:
			text += "DBAG_";
			break;
		case DAL.DiffuseParallaxAmbientEmissive:
			text += "DPAG_";
			break;
		case DAL.DiffuseParallaxSpecularColorEmissive:
			text += "DPSCE_";
			break;
		case DAL.DiffuseParallaxEmissive:
			text += "DPE_";
			break;
		case DAL.DiffuseBumpSpecularColorEmissive:
			text += "DBSCE_";
			break;
		case DAL.DiffuseBumpEmissive:
			text += "DBE_";
			break;
		case DAL.Tangent:
			text += "Tangent_";
			break;
		case DAL.Linear:
			text += "Linear_";
			break;
		case DAL.Point:
			text += "Point_";
			break;
		case DAL.Point3:
			text += "Point3_";
			break;
		case DAL.Point4:
			text += "Point4_";
			break;
		case DAL.Directional:
			text += "Directional_";
			break;
		case DAL.Directional3:
			text += "Directional3_";
			break;
		case DAL.Directional4:
			text += "Directional4_";
			break;
		}
		if (P_0 == DA_0018.Lighting)
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
		_3A_0018.Add(key, text);
		return text;
	}
}
