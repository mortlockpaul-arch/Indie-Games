using System;
using System.Runtime.CompilerServices;
using _0003;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Effects.Forward;

/// <summary>
/// Effect provides per-pixel fog.
/// </summary>
public class FogEffect : BaseSkinnedEffect, ITerrainEffect
{
	private float HCB;

	private float HC_0002;

	private Vector3 HC_0012;

	private int HCH;

	private int HC7;

	private float HC_0001;

	private float HCw;

	private Texture2D HCZ;

	private EffectParameter HC_000F;

	private EffectParameter HCy;

	private EffectParameter HC6;

	private EffectParameter HCD;

	private EffectParameter HC_0011;

	private EffectParameter HCK;

	private EffectParameter HC_0003;

	[CompilerGenerated]
	private bool HCk;

	/// <summary>
	/// Distance from the camera in world space that fog begins.
	/// </summary>
	public float StartDistance
	{
		get
		{
			return HCB;
		}
		set
		{
			H7(value, HC_0002);
		}
	}

	/// <summary>
	/// Distance from the camera in world space that fog ends.
	/// </summary>
	public float EndDistance
	{
		get
		{
			return HC_0002;
		}
		set
		{
			H7(HCB, value);
		}
	}

	/// <summary>
	/// Color of the applied fog.
	/// </summary>
	public Vector3 Color
	{
		get
		{
			return HC_0012;
		}
		set
		{
			if (!(HC_0012 == value) && HCy != null)
			{
				HC_0012 = value;
				HCy.SetValue(new Vector4(HC_0012.X, HC_0012.Y, HC_0012.Z, 0f));
			}
		}
	}

	/// <summary>
	/// Texture containing height values used to displace a terrain mesh. Also used
	/// for low frequency lighting.
	/// </summary>
	public Texture2D HeightMapTexture
	{
		get
		{
			return HCZ;
		}
		set
		{
			if (value != HCZ)
			{
				EffectHelper._00120(value, ref HCZ, ref HC_0003);
				SetTechnique();
			}
		}
	}

	/// <summary>
	/// Adjusts the terrain displacement magnitude.
	/// </summary>
	public float HeightScale
	{
		get
		{
			return HC_0001;
		}
		set
		{
			EffectHelper._00124(value, ref HC_0001, ref HC_0011);
		}
	}

	/// <summary>
	/// Adjusts the number of times the height map tiles across a terrain's
	/// mesh. Similar to uv scale when texture mapping.
	/// </summary>
	public float Tiling
	{
		get
		{
			return HCw;
		}
		set
		{
			EffectHelper._00124(value, ref HCw, ref HCK);
		}
	}

	/// <summary>
	/// Determines the number of times the height map tiles before the terrain ends.
	/// </summary>
	public int TileRepeatCount
	{
		get
		{
			return HC7;
		}
		set
		{
			EffectHelper._0012_0005(value, ref HC7, ref HCD);
		}
	}

	/// <summary>
	/// Density or tessellation of the terrain mesh.
	/// </summary>
	public int MeshSegments
	{
		get
		{
			return HCH;
		}
		set
		{
			EffectHelper._0012_0005(value, ref HCH, ref HC6);
		}
	}

	/// <summary>
	/// Surfaces rendered with the effect should be visible from both sides.
	/// </summary>
	public override bool DoubleSided
	{
		[CompilerGenerated]
		get
		{
			return HCk;
		}
		[CompilerGenerated]
		set
		{
			HCk = value;
		}
	}

	private void H7(float P_0, float P_1)
	{
		if (HC_000F != null && (HCB != P_0 || HC_0002 != P_1))
		{
			HCB = Math.Max(P_0, 0f);
			HC_0002 = Math.Max(HCB * 1.01f, P_1);
			float num = HC_0002 - HCB;
			if (num != 0f)
			{
				num = 1f / num;
			}
			HC_000F.SetValue(new Vector4(HCB, num, 0f, 0f));
		}
	}

	/// <summary>
	/// Sets the effect technique based on its current property values.
	/// </summary>
	protected override void SetTechnique()
	{
		HCV.HCB.AccumulationValue++;
		if (HCZ != null)
		{
			base.CurrentTechnique = base.Techniques["Fog_Terrain_Technique"];
		}
		else
		{
			base.CurrentTechnique = base.Techniques[_0003._0002.H_0012(_0003._0002._0001CB.Fog, _0003._0002._0001C_0002.None, 0, false, false, base.Skinned, false)];
		}
	}

	/// <summary>
	/// Creates a new FogEffect instance.
	/// </summary>
	/// <param name="graphicsdevice"></param>
	public FogEffect(GraphicsDevice graphicsdevice)
		: base(graphicsdevice, "FogEffect")
	{
		HC_000F = base.Parameters["_FogStartDist_And_EndDistInv"];
		HCy = base.Parameters["_FogColor"];
		HC_0003 = base.Parameters["HeightMapTexture"];
		HC6 = base.Parameters["MeshSegments"];
		HCD = base.Parameters["MeshRepeatCount"];
		HC_0011 = base.Parameters["HeightScale"];
		HCK = base.Parameters["Tiling"];
		StartDistance = 1000f;
		EndDistance = 100000f;
		Color = new Vector3(0.5f, 0.5f, 0.5f);
		TileRepeatCount = 1;
		SetTechnique();
	}

	/// <summary>
	/// Creates a new empty effect of the same class type and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	protected override Effect Create()
	{
		return new FogEffect(base.GraphicsDevice);
	}
}
