using System;
using System.Runtime.CompilerServices;
using System.Threading;
using _0003;
using _8;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using l;
using p;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Base class that provides data for rendering SunBurn's terrain.
/// </summary>
[EditorObject(true)]
public abstract class BaseTerrainEffect : BaseRenderableEffect, _8._0018, ITerrainEffect, _0003._0017, IEditorObject, INamedObject, ICollisionMaterial
{
	internal new delegate void DA_0018();

	private DA_0018 _3A_0018;

	private string _3AL = "";

	private int _3A_0019;

	private int _3A3;

	private float _3A6 = 0.75f;

	private float _3AD = 0.25f;

	private int _3A_0017;

	private int _3A_0003;

	private int _3Al;

	private float _3At;

	private float _3AF;

	private float _3Ac;

	private float _3Ag;

	private float _3AI;

	private float _3A8;

	private Vector3 _3AZ;

	private Texture2D _3Ax;

	private Texture2D _3Aq;

	private Texture2D _3Ab;

	private Texture2D _3AT;

	private Texture2D _3Ay;

	private Texture2D _3A_0015;

	private Texture2D _3A_0001;

	private new Texture2D _3A7;

	private Texture2D _3AX;

	private Texture2D _3A_0010;

	private Texture2D _3A_0016;

	private Texture2D _3Ak;

	private Texture2D _3Ap;

	private Texture2D _3Au;

	private EffectParameter _3AU;

	private EffectParameter _3Az;

	private EffectParameter _3A_0014;

	private EffectParameter _3Av;

	private EffectParameter _3AC;

	private EffectParameter _3Aa;

	private EffectParameter _3A5;

	private EffectParameter _3Ar;

	private EffectParameter _3A_0012;

	private EffectParameter _3Ao;

	private EffectParameter _3Af;

	private EffectParameter _3A1;

	private EffectParameter _3AG;

	private EffectParameter _3An;

	private EffectParameter _3AR;

	private EffectParameter _3A2;

	private EffectParameter _3A_0002;

	private EffectParameter _3A4;

	private EffectParameter _3A_0005;

	private EffectParameter _3A_000F;

	private EffectParameter _3AH;

	private EffectParameter _3AM;

	private static byte[] _3AP;

	private static HalfSingle[] _3Aw;

	[CompilerGenerated]
	private bool _3Ae;

	[CompilerGenerated]
	private string _3AO;

	[CompilerGenerated]
	private string _3AN;

	[CompilerGenerated]
	private string _3Am;

	[CompilerGenerated]
	private string _3AE;

	[CompilerGenerated]
	private string _3AS;

	[CompilerGenerated]
	private string _3AQ;

	[CompilerGenerated]
	private string _3AB;

	[CompilerGenerated]
	private string _3AV;

	[CompilerGenerated]
	private string _3Ad;

	[CompilerGenerated]
	private string _3A_0006;

	[CompilerGenerated]
	private string _3Ai;

	[CompilerGenerated]
	private string _3AK;

	[CompilerGenerated]
	private bool _3AY;

	/// <summary>
	/// Notifies the editor that this object is partially controlled via code. The editor
	/// will display information to the user indicating some property values are
	/// overridden in code and changes may not take effect.
	/// </summary>
	public bool AffectedInCode
	{
		[CompilerGenerated]
		get
		{
			return _3Ae;
		}
		[CompilerGenerated]
		set
		{
			_3Ae = value;
		}
	}

	internal string MaterialFile
	{
		get
		{
			return _3AL;
		}
		set
		{
			_3AL = text;
		}
	}

	string _8._0018.MaterialFile => _3AL;

	internal string MaterialName
	{
		[CompilerGenerated]
		get
		{
			return _3AO;
		}
		[CompilerGenerated]
		set
		{
			_3AO = text;
		}
	}

	internal string ProjectFile
	{
		[CompilerGenerated]
		get
		{
			return _3AN;
		}
		[CompilerGenerated]
		set
		{
			_3AN = text;
		}
	}

	string _0003._0017.ProjectFile => ProjectFile;

	internal string DiffuseMapLayer1File
	{
		[CompilerGenerated]
		get
		{
			return _3Am;
		}
		[CompilerGenerated]
		set
		{
			_3Am = text;
		}
	}

	internal string DiffuseMapLayer2File
	{
		[CompilerGenerated]
		get
		{
			return _3AE;
		}
		[CompilerGenerated]
		set
		{
			_3AE = text;
		}
	}

	internal string DiffuseMapLayer3File
	{
		[CompilerGenerated]
		get
		{
			return _3AS;
		}
		[CompilerGenerated]
		set
		{
			_3AS = text;
		}
	}

	internal string DiffuseMapLayer4File
	{
		[CompilerGenerated]
		get
		{
			return _3AQ;
		}
		[CompilerGenerated]
		set
		{
			_3AQ = text;
		}
	}

	internal string NormalMapLayer1File
	{
		[CompilerGenerated]
		get
		{
			return _3AB;
		}
		[CompilerGenerated]
		set
		{
			_3AB = text;
		}
	}

	internal string NormalMapLayer2File
	{
		[CompilerGenerated]
		get
		{
			return _3AV;
		}
		[CompilerGenerated]
		set
		{
			_3AV = text;
		}
	}

	internal string NormalMapLayer3File
	{
		[CompilerGenerated]
		get
		{
			return _3Ad;
		}
		[CompilerGenerated]
		set
		{
			_3Ad = text;
		}
	}

	internal string NormalMapLayer4File
	{
		[CompilerGenerated]
		get
		{
			return _3A_0006;
		}
		[CompilerGenerated]
		set
		{
			_3A_0006 = text;
		}
	}

	internal string HeightMapFile
	{
		[CompilerGenerated]
		get
		{
			return _3Ai;
		}
		[CompilerGenerated]
		set
		{
			_3Ai = text;
		}
	}

	internal string BlendMapFile
	{
		[CompilerGenerated]
		get
		{
			return _3AK;
		}
		[CompilerGenerated]
		set
		{
			_3AK = text;
		}
	}

	/// <summary>
	/// Diffuse texture used in blend mapping (associated with the Red
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[l._0018("DiffuseMapLayer1File", false)]
	[EditorProperty(true, Description = "Diffuse 1", MajorGrouping = 2, MinorGrouping = 1, ToolTipText = "")]
	public Texture2D DiffuseMapLayer1Texture
	{
		get
		{
			return _3AT;
		}
		set
		{
			EffectHelper._0019h(value, _3Aq, ref _3AT, ref _3Az);
		}
	}

	/// <summary>
	/// Diffuse texture used in blend mapping (associated with the Green
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[l._0018("DiffuseMapLayer2File", false)]
	[EditorProperty(true, Description = "Diffuse 2", MajorGrouping = 3, MinorGrouping = 1, ToolTipText = "")]
	public Texture2D DiffuseMapLayer2Texture
	{
		get
		{
			return _3Ay;
		}
		set
		{
			EffectHelper._0019h(value, _3Aq, ref _3Ay, ref _3A_0014);
			_0019Y();
		}
	}

	/// <summary>
	/// Diffuse texture used in blend mapping (associated with the Blue
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[l._0018("DiffuseMapLayer3File", false)]
	[EditorProperty(true, Description = "Diffuse 3", MajorGrouping = 4, MinorGrouping = 1, ToolTipText = "")]
	public Texture2D DiffuseMapLayer3Texture
	{
		get
		{
			return _3A_0015;
		}
		set
		{
			EffectHelper._0019h(value, _3Aq, ref _3A_0015, ref _3Av);
			_0019Y();
		}
	}

	/// <summary>
	/// Diffuse texture used in blend mapping (associated with the Alpha
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[l._0018("DiffuseMapLayer4File", false)]
	[EditorProperty(true, Description = "Diffuse 4", MajorGrouping = 5, MinorGrouping = 1, ToolTipText = "")]
	public Texture2D DiffuseMapLayer4Texture
	{
		get
		{
			return _3A_0001;
		}
		set
		{
			EffectHelper._0019h(value, _3Aq, ref _3A_0001, ref _3AC);
			_0019Y();
		}
	}

	/// <summary>
	/// Normal map texture used in blend mapping (associated with the Red
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[l._0018("NormalMapLayer1File", false)]
	[EditorProperty(true, Description = "Normal 1", MajorGrouping = 2, MinorGrouping = 2, ToolTipText = "")]
	public Texture2D NormalMapLayer1Texture
	{
		get
		{
			return _3A7;
		}
		set
		{
			EffectHelper._0019h(value, _3Ab, ref _3A7, ref _3Aa);
		}
	}

	/// <summary>
	/// Normal map texture used in blend mapping (associated with the Green
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[l._0018("NormalMapLayer2File", false)]
	[EditorProperty(true, Description = "Normal 2", MajorGrouping = 3, MinorGrouping = 2, ToolTipText = "")]
	public Texture2D NormalMapLayer2Texture
	{
		get
		{
			return _3AX;
		}
		set
		{
			EffectHelper._0019h(value, _3Ab, ref _3AX, ref _3A5);
			_0019Y();
		}
	}

	/// <summary>
	/// Normal map texture used in blend mapping (associated with the Blue
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[l._0018("NormalMapLayer3File", false)]
	[EditorProperty(true, Description = "Normal 3", MajorGrouping = 4, MinorGrouping = 2, ToolTipText = "")]
	public Texture2D NormalMapLayer3Texture
	{
		get
		{
			return _3A_0010;
		}
		set
		{
			EffectHelper._0019h(value, _3Ab, ref _3A_0010, ref _3Ar);
			_0019Y();
		}
	}

	/// <summary>
	/// Normal map texture used in blend mapping (associated with the Alpha
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[EditorProperty(true, Description = "Normal 4", MajorGrouping = 5, MinorGrouping = 2, ToolTipText = "")]
	[l._0018("NormalMapLayer4File", false)]
	public Texture2D NormalMapLayer4Texture
	{
		get
		{
			return _3A_0016;
		}
		set
		{
			EffectHelper._0019h(value, _3Ab, ref _3A_0016, ref _3A_0012);
			_0019Y();
		}
	}

	/// <summary>
	/// Texture containing height values used to displace a terrain mesh. Also used
	/// for low frequency lighting.
	///
	/// Requires a HalfSingle format texture.
	/// </summary>
	[EditorProperty(true, Description = "Height Map", MajorGrouping = 1, MinorGrouping = 1, ToolTipText = "")]
	[l._0018("HeightMapFile", true)]
	public Texture2D HeightMapTexture
	{
		get
		{
			return _3Ak;
		}
		set
		{
			if (value == _3Ak)
			{
				return;
			}
			EffectHelper._0019h(value, _3Ax, ref _3Ak, ref _3Ao);
			_0019i();
			if (_3Ak == null)
			{
				_3Af.SetValue(_3Ak);
				return;
			}
			if (_3Ak.Format != SurfaceFormat.HalfSingle)
			{
				throw new Exception("Terrain height map requires a HalfSingle format texture.");
			}
			int num = _3Ak.Width;
			int num2 = _3Ak.Height;
			int levelCount = _3Ak.LevelCount;
			int num3 = num * num2;
			GraphicsDevice graphicsDevice = base.GraphicsDevice;
			for (int i = 0; i < 16; i++)
			{
				graphicsDevice.Textures[i] = null;
			}
			for (int j = 0; j < 4; j++)
			{
				graphicsDevice.VertexTextures[j] = null;
			}
			if (_3Ap == null || num != _3Ap.Width || num2 != _3Ap.Height)
			{
				p._0018._6_0006(ref _3Ap);
				_3Ap = new Texture2D(graphicsDevice, num, num2, levelCount > 1, SurfaceFormat.Alpha8);
			}
			if (_3AP == null || num3 > _3AP.Length)
			{
				_3AP = new byte[num3];
				_3Aw = new HalfSingle[num3];
			}
			for (int k = 0; k < levelCount; k++)
			{
				int num4 = num * num2;
				_3Ak.GetData(k, null, _3Aw, 0, num4);
				for (int m = 0; m < num4; m++)
				{
					float num5 = _3Aw[m].ToSingle();
					_3AP[m] = (byte)(num5 * 255f);
				}
				_3Ap.SetData(k, null, _3AP, 0, num4);
				num = Math.Max(1, num >> 1);
				num2 = Math.Max(1, num2 >> 1);
			}
			_3Af.SetValue(_3Ap);
			int num6 = _3Ak.Width / 3;
			EffectHelper._0019_0013(num6, ref _3Al, ref _3AR);
		}
	}

	/// <summary>
	/// Texture containing intensity values used to blend diffuse and normal map textures
	/// into the final material. Each texture channel (Red, Green, Blue, Alpha) controls
	/// a terrain texture layer (layer 1, 2, 3, 4).
	/// </summary>
	[EditorProperty(true, Description = "Blend Map", MajorGrouping = 1, MinorGrouping = 2, ToolTipText = "")]
	[l._0018("BlendMapFile", false)]
	public Texture2D BlendMapTexture
	{
		get
		{
			return _3Au;
		}
		set
		{
			EffectHelper._0019h(value, _3Aq, ref _3Au, ref _3A1);
		}
	}

	/// <summary>
	/// Controls the depth or detail level of low frequency lighting on a terrain.
	/// </summary>
	[EditorProperty(true, Description = "Normal Strength", MajorGrouping = 6, MinorGrouping = 4, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 32.0, 0.1)]
	public float NormalMapStrength
	{
		get
		{
			return _3At;
		}
		set
		{
			EffectHelper._00190(value, ref _3At, ref _3A2);
		}
	}

	/// <summary>
	/// Adjusts the number of times the blend mapped materials tile across a terrain's
	/// mesh. Similar to uv scale when texture mapping.
	/// </summary>
	[EditorNumberPadOptions(2, 0.0, 512.0, 0.2)]
	[EditorProperty(true, Description = "Material Scale", MajorGrouping = 6, MinorGrouping = 3, ToolTipText = "")]
	public float DiffuseScale
	{
		get
		{
			return _3AF;
		}
		set
		{
			EffectHelper._00190(value, ref _3AF, ref _3A_0002);
		}
	}

	/// <summary>
	/// Adjusts the terrain displacement magnitude.
	/// </summary>
	[EditorProperty(true, Description = "Height Scale", MajorGrouping = 6, MinorGrouping = 1, ToolTipText = "")]
	[EditorNumberPadOptions(3, 0.0, 100.0, 0.01)]
	public float HeightScale
	{
		get
		{
			return _3Ac;
		}
		set
		{
			float num = Math.Max(1E-06f, value);
			EffectHelper._00190(num, ref _3Ac, ref _3A4);
			_0019i();
		}
	}

	/// <summary>
	/// Adjusts the number of times the height map tiles across a terrain's
	/// mesh. Similar to uv scale when texture mapping.
	/// </summary>
	[EditorNumberPadOptions(3, 0.0, 100.0, 0.01)]
	[EditorProperty(true, Description = "Tiling Amount", MajorGrouping = 6, MinorGrouping = 2, ToolTipText = "")]
	public float Tiling
	{
		get
		{
			return _3Ag;
		}
		set
		{
			float num = Math.Max(1E-06f, value);
			EffectHelper._00190(num, ref _3Ag, ref _3A_0005);
			_0019i();
		}
	}

	/// <summary>
	/// Power applied to material specular reflections. Affects how shiny a material appears.
	/// </summary>
	[EditorNumberPadOptions(2, 0.0, 256.0, 0.5)]
	[EditorProperty(true, Description = "Specular Power", MajorGrouping = 7, MinorGrouping = 1, ToolTipText = "")]
	public float SpecularPower
	{
		get
		{
			return _3AI;
		}
		set
		{
			EffectHelper._00190(value, ref _3AI, ref _3A_000F);
		}
	}

	/// <summary>
	/// Intensity applied to material specular reflections. Affects how intense the specular appears.
	/// </summary>
	[EditorNumberPadOptions(2, 0.0, 32.0, 0.5)]
	[EditorProperty(true, Description = "Specular Amount", MajorGrouping = 7, MinorGrouping = 2, ToolTipText = "")]
	public float SpecularAmount
	{
		get
		{
			return _3A8;
		}
		set
		{
			EffectHelper._00190(value, ref _3A8, ref _3AH);
		}
	}

	/// <summary>
	/// Color applied to material specular reflections.
	/// </summary>
	[EditorProperty(true, Description = "Specular Color", MajorGrouping = 7, MinorGrouping = 11, ControlType = ControlType.ColorSelection, ToolTipText = "")]
	public Vector3 SpecularColor
	{
		get
		{
			return _3AZ;
		}
		set
		{
			EffectHelper._0019J(value, ref _3AZ, ref _3AM);
		}
	}

	/// <summary>
	/// Determines the number of times the height map tiles before the terrain ends.
	/// </summary>
	[EditorNumberPadOptions(3, 1.0, 100.0, 1.0)]
	[EditorProperty(true, Description = "Repeat Count", MajorGrouping = 6, MinorGrouping = 3, ToolTipText = "")]
	public int TileRepeatCount
	{
		get
		{
			return _3A_0003;
		}
		set
		{
			int num = Math.Max(value, 1);
			EffectHelper._0019_0013(num, ref _3A_0003, ref _3An);
			_0019i();
		}
	}

	/// <summary>
	/// Density or tessellation of the terrain mesh.
	/// </summary>
	public int MeshSegments
	{
		get
		{
			return _3A_0017;
		}
		set
		{
			EffectHelper._0019_0013(value, ref _3A_0017, ref _3AG);
		}
	}

	/// <summary>
	/// Surfaces rendered with the effect should be visible from both sides.
	/// </summary>
	[EditorProperty(true, Description = "Double Sided", HorizontalAlignment = true, MajorGrouping = 7, MinorGrouping = 1, ToolTipText = "")]
	public override bool DoubleSided
	{
		[CompilerGenerated]
		get
		{
			return _3AY;
		}
		[CompilerGenerated]
		set
		{
			_3AY = value;
		}
	}

	/// <summary>
	/// Amount material absorbs impact force.
	/// </summary>
	[EditorNumberPadOptions(3, 0.0, 1.0, 0.05)]
	[EditorProperty(true, Description = "Elasticity", HorizontalAlignment = true, MajorGrouping = 8, MinorGrouping = 1, ToolTipText = "")]
	public float Elasticity
	{
		get
		{
			return _3A6;
		}
		set
		{
			_3A6 = MathHelper.Clamp(value, 0f, 1f);
			_3A3++;
		}
	}

	/// <summary>
	/// Amount material resists objects moving across its surface.
	/// </summary>
	[EditorNumberPadOptions(3, 0.0, 1.0, 0.05)]
	[EditorProperty(true, Description = "Friction", HorizontalAlignment = true, MajorGrouping = 8, MinorGrouping = 2, ToolTipText = "")]
	public float Friction
	{
		get
		{
			return _3AD;
		}
		set
		{
			_3AD = MathHelper.Clamp(value, 0f, 1f);
			_3A3++;
		}
	}

	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	public int CollisionId => _3A3;

	[SpecialName]
	internal void _0019d(DA_0018 P_0)
	{
		DA_0018 obj = _3A_0018;
		DA_0018 obj2;
		do
		{
			obj2 = obj;
			DA_0018 value = (DA_0018)Delegate.Combine(obj2, P_0);
			obj = Interlocked.CompareExchange(ref _3A_0018, value, obj2);
		}
		while ((object)obj != obj2);
	}

	[SpecialName]
	internal void _0019_0006(DA_0018 P_0)
	{
		DA_0018 obj = _3A_0018;
		DA_0018 obj2;
		do
		{
			obj2 = obj;
			DA_0018 value = (DA_0018)Delegate.Remove(obj2, P_0);
			obj = Interlocked.CompareExchange(ref _3A_0018, value, obj2);
		}
		while ((object)obj != obj2);
	}

	internal void _0019i()
	{
		_3A3++;
		if (_3A_0018 != null)
		{
			_3A_0018();
		}
	}

	/// <summary>
	/// Returns the width of the 
	/// </summary>
	/// <returns></returns>
	public float GetTileWidth()
	{
		return 1f / _3Ag;
	}

	private bool _0019K(Texture2D P_0, Texture2D P_1)
	{
		if (P_0 != P_1)
		{
			return P_0 == null;
		}
		return true;
	}

	private void _0019Y()
	{
		if (_3AU != null)
		{
			int num = 1;
			if (!_0019K(_3A_0001, _3Aq) || !_0019K(_3A_0016, _3Ab))
			{
				num = 4;
			}
			else if (!_0019K(_3A_0015, _3Aq) || !_0019K(_3A_0010, _3Ab))
			{
				num = 3;
			}
			else if (!_0019K(_3Ay, _3Aq) || !_0019K(_3AX, _3Ab))
			{
				num = 2;
			}
			EffectHelper._0019_0013(num, ref _3A_0019, ref _3AU);
		}
	}

	/// <summary>
	/// Creates a new BaseTerrainEffect instance.
	/// </summary>
	/// <param name="graphicsdevice"></param>
	/// <param name="effectname"></param>
	public BaseTerrainEffect(GraphicsDevice graphicsdevice, string effectname)
		: base(graphicsdevice, effectname)
	{
		_0018(graphicsdevice, true);
	}

	/// <summary>
	/// Creates a new BaseTerrainEffect instance.
	/// </summary>
	/// <param name="graphicsdevice"></param>
	/// <param name="effectname"></param>
	/// <param name="trackeffect"></param>
	internal BaseTerrainEffect(GraphicsDevice P_0, string P_1, bool P_2)
		: base(P_0, P_1)
	{
		_0018(P_0, P_2);
	}

	private void _0018(GraphicsDevice P_0, bool P_1)
	{
		_3AU = base.Parameters["LayerCount"];
		_3Az = base.Parameters["DiffuseLayer1Texture"];
		_3A_0014 = base.Parameters["DiffuseLayer2Texture"];
		_3Av = base.Parameters["DiffuseLayer3Texture"];
		_3AC = base.Parameters["DiffuseLayer4Texture"];
		_3Aa = base.Parameters["NormalLayer1Texture"];
		_3A5 = base.Parameters["NormalLayer2Texture"];
		_3Ar = base.Parameters["NormalLayer3Texture"];
		_3A_0012 = base.Parameters["NormalLayer4Texture"];
		_3Ao = base.Parameters["HeightMapTexture"];
		_3Af = base.Parameters["HeightMapPSTexture"];
		_3A1 = base.Parameters["BlendMapTexture"];
		_3AG = base.Parameters["MeshSegments"];
		_3An = base.Parameters["MeshRepeatCount"];
		_3AR = base.Parameters["NormalMapSize"];
		_3A2 = base.Parameters["NormalMapStrength"];
		_3A_0002 = base.Parameters["DiffuseScale"];
		_3A4 = base.Parameters["HeightScale"];
		_3A_0005 = base.Parameters["Tiling"];
		_3A_000F = base.Parameters["SpecularPower"];
		_3AH = base.Parameters["SpecularAmount"];
		_3AM = base.Parameters["SpecularColor"];
		_3Aq = SunBurnCoreSystem.Instance.Lw("White");
		_3Ax = SunBurnCoreSystem.Instance.Lm();
		_3Ab = SunBurnCoreSystem.Instance.Lw("Normal");
		DiffuseMapLayer1Texture = _3Aq;
		DiffuseMapLayer2Texture = _3Aq;
		DiffuseMapLayer3Texture = _3Aq;
		DiffuseMapLayer4Texture = _3Aq;
		NormalMapLayer1Texture = _3Ab;
		NormalMapLayer2Texture = _3Ab;
		NormalMapLayer3Texture = _3Ab;
		NormalMapLayer4Texture = _3Ab;
		BlendMapTexture = _3Aq;
		HeightMapTexture = _3Ax;
		HeightScale = 1f;
		Tiling = 1f;
		MeshSegments = 128;
		TileRepeatCount = 1;
		DiffuseScale = 16f;
		NormalMapStrength = 1f;
		SpecularAmount = 1f;
		SpecularColor = Vector3.One;
		SpecularPower = 0f;
		SetTechnique();
		_0019Y();
		if (P_1)
		{
			SunBurnEditor.OnCreateResource(this);
		}
	}

	/// <summary>
	/// Releases the unmanaged resources used by the Effect and optionally releases the managed resources.
	/// </summary>
	/// <param name="releasemanaged"></param>
	protected override void Dispose(bool releasemanaged)
	{
		base.Dispose(releasemanaged);
		p._0018._6_0006(ref _3Ap);
		SunBurnEditor.OnDisposeResource(this);
	}
}
