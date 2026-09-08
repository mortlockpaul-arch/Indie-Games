using System;
using System.Runtime.CompilerServices;
using _8;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SynapseGaming.LightingSystem.Effects.Forward;

/// <summary>
/// Effect provides per-pixel fog.
/// </summary>
public class FogEffect : BaseSkinnedEffect, ITerrainEffect
{
	private float _3A_0018;

	private float _3AL;

	private Vector3 _3A_0019;

	private int _3A3;

	private int _3A6;

	private float _3AD;

	private float _3A_0017;

	private Texture2D _3A_0003;

	private EffectParameter _3Al;

	private EffectParameter _3At;

	private EffectParameter _3AF;

	private EffectParameter _3Ac;

	private EffectParameter _3Ag;

	private EffectParameter _3AI;

	private EffectParameter _3A8;

	[CompilerGenerated]
	private bool _3AZ;

	/// <summary>
	/// Distance from the camera in world space that fog begins.
	/// </summary>
	public float StartDistance
	{
		get
		{
			return _3A_0018;
		}
		set
		{
			_33(value, _3AL);
		}
	}

	/// <summary>
	/// Distance from the camera in world space that fog ends.
	/// </summary>
	public float EndDistance
	{
		get
		{
			return _3AL;
		}
		set
		{
			_33(_3A_0018, value);
		}
	}

	/// <summary>
	/// Color of the applied fog.
	/// </summary>
	public Vector3 Color
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			if (!(_3A_0019 == value) && _3At != null)
			{
				_3A_0019 = value;
				_3At.SetValue(new Vector4(_3A_0019.X, _3A_0019.Y, _3A_0019.Z, 0f));
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
			return _3A_0003;
		}
		set
		{
			if (value != _3A_0003)
			{
				EffectHelper._0019h(value, ref _3A_0003, ref _3A8);
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
			return _3AD;
		}
		set
		{
			EffectHelper._00190(value, ref _3AD, ref _3Ag);
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
			return _3A_0017;
		}
		set
		{
			EffectHelper._00190(value, ref _3A_0017, ref _3AI);
		}
	}

	/// <summary>
	/// Determines the number of times the height map tiles before the terrain ends.
	/// </summary>
	public int TileRepeatCount
	{
		get
		{
			return _3A6;
		}
		set
		{
			EffectHelper._0019_0013(value, ref _3A6, ref _3Ac);
		}
	}

	/// <summary>
	/// Density or tessellation of the terrain mesh.
	/// </summary>
	public int MeshSegments
	{
		get
		{
			return _3A3;
		}
		set
		{
			EffectHelper._0019_0013(value, ref _3A3, ref _3AF);
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
			return _3AZ;
		}
		[CompilerGenerated]
		set
		{
			_3AZ = value;
		}
	}

	private void _33(float P_0, float P_1)
	{
		if (_3Al != null && (_3A_0018 != P_0 || _3AL != P_1))
		{
			_3A_0018 = Math.Max(P_0, 0f);
			_3AL = Math.Max(_3A_0018 * 1.01f, P_1);
			float num = _3AL - _3A_0018;
			if (num != 0f)
			{
				num = 1f / num;
			}
			_3Al.SetValue(new Vector4(_3A_0018, num, 0f, 0f));
		}
	}

	/// <summary>
	/// Sets the effect technique based on its current property values.
	/// </summary>
	protected override void SetTechnique()
	{
		_3A7._3A_0018.AccumulationValue++;
		if (_3A_0003 != null)
		{
			base.CurrentTechnique = base.Techniques["Fog_Terrain_Technique"];
		}
		else
		{
			base.CurrentTechnique = base.Techniques[_8.L._3L(_8.L.DA_0018.Fog, _8.L.DAL.None, 0, false, false, base.Skinned, false)];
		}
	}

	/// <summary>
	/// Creates a new FogEffect instance.
	/// </summary>
	/// <param name="graphicsdevice"></param>
	public FogEffect(GraphicsDevice graphicsdevice)
		: base(graphicsdevice, "FogEffect")
	{
		_3Al = base.Parameters["_FogStartDist_And_EndDistInv"];
		_3At = base.Parameters["_FogColor"];
		_3A8 = base.Parameters["HeightMapTexture"];
		_3AF = base.Parameters["MeshSegments"];
		_3Ac = base.Parameters["MeshRepeatCount"];
		_3Ag = base.Parameters["HeightScale"];
		_3AI = base.Parameters["Tiling"];
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
