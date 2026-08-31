using System;
using System.Runtime.CompilerServices;
using System.Threading;
using _0003;
using _000F;
using F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using Z;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Base class that provides data for rendering SunBurn's terrain.
/// </summary>
[EditorObject(true)]
public abstract class BaseTerrainEffect : BaseRenderableEffect, _0003.B, ITerrainEffect, Z.w, IEditorObject, INamedObject, ICollisionMaterial
{
	internal new delegate void _0001CB();

	private _0001CB HCB;

	private string HC_0002 = "";

	private int HC_0012;

	private int HCH;

	private float HC7 = 0.75f;

	private float HC_0001 = 0.25f;

	private int HCw;

	private int HCZ;

	private int HC_000F;

	private float HCy;

	private float HC6;

	private float HCD;

	private float HC_0011;

	private float HCK;

	private float HC_0003;

	private Vector3 HCk;

	private Texture2D HCs;

	private Texture2D HC_0013;

	private Texture2D HCX;

	private Texture2D HCz;

	private Texture2D HCA;

	private Texture2D HCc;

	private Texture2D HCY;

	private new Texture2D HCV;

	private Texture2D HCu;

	private Texture2D HCq;

	private Texture2D HCR;

	private Texture2D HCN;

	private Texture2D HCF;

	private Texture2D HCf;

	private EffectParameter HCG;

	private EffectParameter HC_0010;

	private EffectParameter HC_0014;

	private EffectParameter HCL;

	private EffectParameter HCh;

	private EffectParameter HCT;

	private EffectParameter HCa;

	private EffectParameter HCS;

	private EffectParameter HCr;

	private EffectParameter HCJ;

	private EffectParameter HC_0006;

	private EffectParameter HCo;

	private EffectParameter HCe;

	private EffectParameter HC_0015;

	private EffectParameter HCU;

	private EffectParameter HC8;

	private EffectParameter HCj;

	private EffectParameter HCi;

	private EffectParameter HCO;

	private EffectParameter HC_0017;

	private EffectParameter HC_0019;

	private EffectParameter HCp;

	private static byte[] HC1;

	private static HalfSingle[] HCb;

	[CompilerGenerated]
	private bool HCl;

	[CompilerGenerated]
	private string HCg;

	[CompilerGenerated]
	private string HCW;

	[CompilerGenerated]
	private string HC_0018;

	[CompilerGenerated]
	private string HC_000E;

	[CompilerGenerated]
	private string HCd;

	[CompilerGenerated]
	private string HCn;

	[CompilerGenerated]
	private string HC5;

	[CompilerGenerated]
	private string HC3;

	[CompilerGenerated]
	private string HCE;

	[CompilerGenerated]
	private string HC9;

	[CompilerGenerated]
	private string HC_0004;

	[CompilerGenerated]
	private string HCM;

	[CompilerGenerated]
	private bool HCP;

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
			return HCl;
		}
		[CompilerGenerated]
		set
		{
			HCl = value;
		}
	}

	internal string MaterialFile
	{
		get
		{
			return HC_0002;
		}
		set
		{
			HC_0002 = text;
		}
	}

	string _0003.B.MaterialFile => HC_0002;

	internal string MaterialName
	{
		[CompilerGenerated]
		get
		{
			return HCg;
		}
		[CompilerGenerated]
		set
		{
			HCg = hCg;
		}
	}

	internal string ProjectFile
	{
		[CompilerGenerated]
		get
		{
			return HCW;
		}
		[CompilerGenerated]
		set
		{
			HCW = hCW;
		}
	}

	string Z.w.ProjectFile => ProjectFile;

	internal string DiffuseMapLayer1File
	{
		[CompilerGenerated]
		get
		{
			return HC_0018;
		}
		[CompilerGenerated]
		set
		{
			HC_0018 = text;
		}
	}

	internal string DiffuseMapLayer2File
	{
		[CompilerGenerated]
		get
		{
			return HC_000E;
		}
		[CompilerGenerated]
		set
		{
			HC_000E = text;
		}
	}

	internal string DiffuseMapLayer3File
	{
		[CompilerGenerated]
		get
		{
			return HCd;
		}
		[CompilerGenerated]
		set
		{
			HCd = hCd;
		}
	}

	internal string DiffuseMapLayer4File
	{
		[CompilerGenerated]
		get
		{
			return HCn;
		}
		[CompilerGenerated]
		set
		{
			HCn = hCn;
		}
	}

	internal string NormalMapLayer1File
	{
		[CompilerGenerated]
		get
		{
			return HC5;
		}
		[CompilerGenerated]
		set
		{
			HC5 = hC;
		}
	}

	internal string NormalMapLayer2File
	{
		[CompilerGenerated]
		get
		{
			return HC3;
		}
		[CompilerGenerated]
		set
		{
			HC3 = hC;
		}
	}

	internal string NormalMapLayer3File
	{
		[CompilerGenerated]
		get
		{
			return HCE;
		}
		[CompilerGenerated]
		set
		{
			HCE = hCE;
		}
	}

	internal string NormalMapLayer4File
	{
		[CompilerGenerated]
		get
		{
			return HC9;
		}
		[CompilerGenerated]
		set
		{
			HC9 = hC;
		}
	}

	internal string HeightMapFile
	{
		[CompilerGenerated]
		get
		{
			return HC_0004;
		}
		[CompilerGenerated]
		set
		{
			HC_0004 = text;
		}
	}

	internal string BlendMapFile
	{
		[CompilerGenerated]
		get
		{
			return HCM;
		}
		[CompilerGenerated]
		set
		{
			HCM = hCM;
		}
	}

	/// <summary>
	/// Diffuse texture used in blend mapping (associated with the Red
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[EditorProperty(true, Description = "Diffuse 1", MajorGrouping = 2, MinorGrouping = 1, ToolTipText = "")]
	[_000F.B("DiffuseMapLayer1File", false)]
	public Texture2D DiffuseMapLayer1Texture
	{
		get
		{
			return HCz;
		}
		set
		{
			EffectHelper._00120(value, HC_0013, ref HCz, ref HC_0010);
		}
	}

	/// <summary>
	/// Diffuse texture used in blend mapping (associated with the Green
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[EditorProperty(true, Description = "Diffuse 2", MajorGrouping = 3, MinorGrouping = 1, ToolTipText = "")]
	[_000F.B("DiffuseMapLayer2File", false)]
	public Texture2D DiffuseMapLayer2Texture
	{
		get
		{
			return HCA;
		}
		set
		{
			EffectHelper._00120(value, HC_0013, ref HCA, ref HC_0014);
			_0012t();
		}
	}

	/// <summary>
	/// Diffuse texture used in blend mapping (associated with the Blue
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[_000F.B("DiffuseMapLayer3File", false)]
	[EditorProperty(true, Description = "Diffuse 3", MajorGrouping = 4, MinorGrouping = 1, ToolTipText = "")]
	public Texture2D DiffuseMapLayer3Texture
	{
		get
		{
			return HCc;
		}
		set
		{
			EffectHelper._00120(value, HC_0013, ref HCc, ref HCL);
			_0012t();
		}
	}

	/// <summary>
	/// Diffuse texture used in blend mapping (associated with the Alpha
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[_000F.B("DiffuseMapLayer4File", false)]
	[EditorProperty(true, Description = "Diffuse 4", MajorGrouping = 5, MinorGrouping = 1, ToolTipText = "")]
	public Texture2D DiffuseMapLayer4Texture
	{
		get
		{
			return HCY;
		}
		set
		{
			EffectHelper._00120(value, HC_0013, ref HCY, ref HCh);
			_0012t();
		}
	}

	/// <summary>
	/// Normal map texture used in blend mapping (associated with the Red
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[_000F.B("NormalMapLayer1File", false)]
	[EditorProperty(true, Description = "Normal 1", MajorGrouping = 2, MinorGrouping = 2, ToolTipText = "")]
	public Texture2D NormalMapLayer1Texture
	{
		get
		{
			return HCV;
		}
		set
		{
			EffectHelper._00120(value, HCX, ref HCV, ref HCT);
		}
	}

	/// <summary>
	/// Normal map texture used in blend mapping (associated with the Green
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[EditorProperty(true, Description = "Normal 2", MajorGrouping = 3, MinorGrouping = 2, ToolTipText = "")]
	[_000F.B("NormalMapLayer2File", false)]
	public Texture2D NormalMapLayer2Texture
	{
		get
		{
			return HCu;
		}
		set
		{
			EffectHelper._00120(value, HCX, ref HCu, ref HCa);
			_0012t();
		}
	}

	/// <summary>
	/// Normal map texture used in blend mapping (associated with the Blue
	/// blend map texture channel).
	///
	/// For optimal performance always use the lowest layers first (for instance:
	/// if using two layers use layer 1 and layer 2).
	/// </summary>
	[_000F.B("NormalMapLayer3File", false)]
	[EditorProperty(true, Description = "Normal 3", MajorGrouping = 4, MinorGrouping = 2, ToolTipText = "")]
	public Texture2D NormalMapLayer3Texture
	{
		get
		{
			return HCq;
		}
		set
		{
			EffectHelper._00120(value, HCX, ref HCq, ref HCS);
			_0012t();
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
	[_000F.B("NormalMapLayer4File", false)]
	public Texture2D NormalMapLayer4Texture
	{
		get
		{
			return HCR;
		}
		set
		{
			EffectHelper._00120(value, HCX, ref HCR, ref HCr);
			_0012t();
		}
	}

	/// <summary>
	/// Texture containing height values used to displace a terrain mesh. Also used
	/// for low frequency lighting.
	///
	/// Requires a HalfSingle format texture.
	/// </summary>
	[_000F.B("HeightMapFile", true)]
	[EditorProperty(true, Description = "Height Map", MajorGrouping = 1, MinorGrouping = 1, ToolTipText = "")]
	public Texture2D HeightMapTexture
	{
		get
		{
			return HCN;
		}
		set
		{
			if (value == HCN)
			{
				return;
			}
			EffectHelper._00120(value, HCs, ref HCN, ref HCJ);
			_0012M();
			if (HCN == null)
			{
				HC_0006.SetValue(HCN);
				return;
			}
			if (HCN.Format != SurfaceFormat.HalfSingle)
			{
				throw new Exception("Terrain height map requires a HalfSingle format texture.");
			}
			int num = HCN.Width;
			int num2 = HCN.Height;
			int levelCount = HCN.LevelCount;
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
			if (HCF == null || num != HCF.Width || num2 != HCF.Height)
			{
				F.B._7_0004(ref HCF);
				HCF = new Texture2D(graphicsDevice, num, num2, levelCount > 1, SurfaceFormat.Alpha8);
			}
			if (HC1 == null || num3 > HC1.Length)
			{
				HC1 = new byte[num3];
				HCb = new HalfSingle[num3];
			}
			for (int k = 0; k < levelCount; k++)
			{
				int num4 = num * num2;
				HCN.GetData(k, null, HCb, 0, num4);
				for (int l = 0; l < num4; l++)
				{
					float num5 = HCb[l].ToSingle();
					HC1[l] = (byte)(num5 * 255f);
				}
				HCF.SetData(k, null, HC1, 0, num4);
				num = Math.Max(1, num >> 1);
				num2 = Math.Max(1, num2 >> 1);
			}
			HC_0006.SetValue(HCF);
			int num6 = HCN.Width / 3;
			EffectHelper._0012_0005(num6, ref HC_000F, ref HCU);
		}
	}

	/// <summary>
	/// Texture containing intensity values used to blend diffuse and normal map textures
	/// into the final material. Each texture channel (Red, Green, Blue, Alpha) controls
	/// a terrain texture layer (layer 1, 2, 3, 4).
	/// </summary>
	[_000F.B("BlendMapFile", false)]
	[EditorProperty(true, Description = "Blend Map", MajorGrouping = 1, MinorGrouping = 2, ToolTipText = "")]
	public Texture2D BlendMapTexture
	{
		get
		{
			return HCf;
		}
		set
		{
			EffectHelper._00120(value, HC_0013, ref HCf, ref HCo);
		}
	}

	/// <summary>
	/// Controls the depth or detail level of low frequency lighting on a terrain.
	/// </summary>
	[EditorNumberPadOptions(2, 0.0, 32.0, 0.1)]
	[EditorProperty(true, Description = "Normal Strength", MajorGrouping = 6, MinorGrouping = 4, ToolTipText = "")]
	public float NormalMapStrength
	{
		get
		{
			return HCy;
		}
		set
		{
			EffectHelper._00124(value, ref HCy, ref HC8);
		}
	}

	/// <summary>
	/// Adjusts the number of times the blend mapped materials tile across a terrain's
	/// mesh. Similar to uv scale when texture mapping.
	/// </summary>
	[EditorProperty(true, Description = "Material Scale", MajorGrouping = 6, MinorGrouping = 3, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 512.0, 0.2)]
	public float DiffuseScale
	{
		get
		{
			return HC6;
		}
		set
		{
			EffectHelper._00124(value, ref HC6, ref HCj);
		}
	}

	/// <summary>
	/// Adjusts the terrain displacement magnitude.
	/// </summary>
	[EditorNumberPadOptions(3, 0.0, 100.0, 0.01)]
	[EditorProperty(true, Description = "Height Scale", MajorGrouping = 6, MinorGrouping = 1, ToolTipText = "")]
	public float HeightScale
	{
		get
		{
			return HCD;
		}
		set
		{
			float num = Math.Max(1E-06f, value);
			EffectHelper._00124(num, ref HCD, ref HCi);
			_0012M();
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
			return HC_0011;
		}
		set
		{
			float num = Math.Max(1E-06f, value);
			EffectHelper._00124(num, ref HC_0011, ref HCO);
			_0012M();
		}
	}

	/// <summary>
	/// Power applied to material specular reflections. Affects how shiny a material appears.
	/// </summary>
	[EditorProperty(true, Description = "Specular Power", MajorGrouping = 7, MinorGrouping = 1, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 256.0, 0.5)]
	public float SpecularPower
	{
		get
		{
			return HCK;
		}
		set
		{
			EffectHelper._00124(value, ref HCK, ref HC_0017);
		}
	}

	/// <summary>
	/// Intensity applied to material specular reflections. Affects how intense the specular appears.
	/// </summary>
	[EditorProperty(true, Description = "Specular Amount", MajorGrouping = 7, MinorGrouping = 2, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 32.0, 0.5)]
	public float SpecularAmount
	{
		get
		{
			return HC_0003;
		}
		set
		{
			EffectHelper._00124(value, ref HC_0003, ref HC_0019);
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
			return HCk;
		}
		set
		{
			EffectHelper._00122(value, ref HCk, ref HCp);
		}
	}

	/// <summary>
	/// Determines the number of times the height map tiles before the terrain ends.
	/// </summary>
	[EditorProperty(true, Description = "Repeat Count", MajorGrouping = 6, MinorGrouping = 3, ToolTipText = "")]
	[EditorNumberPadOptions(3, 1.0, 100.0, 1.0)]
	public int TileRepeatCount
	{
		get
		{
			return HCZ;
		}
		set
		{
			int num = Math.Max(value, 1);
			EffectHelper._0012_0005(num, ref HCZ, ref HC_0015);
			_0012M();
		}
	}

	/// <summary>
	/// Density or tessellation of the terrain mesh.
	/// </summary>
	public int MeshSegments
	{
		get
		{
			return HCw;
		}
		set
		{
			EffectHelper._0012_0005(value, ref HCw, ref HCe);
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
			return HCP;
		}
		[CompilerGenerated]
		set
		{
			HCP = value;
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
			return HC7;
		}
		set
		{
			HC7 = MathHelper.Clamp(value, 0f, 1f);
			HCH++;
		}
	}

	/// <summary>
	/// Amount material resists objects moving across its surface.
	/// </summary>
	[EditorProperty(true, Description = "Friction", HorizontalAlignment = true, MajorGrouping = 8, MinorGrouping = 2, ToolTipText = "")]
	[EditorNumberPadOptions(3, 0.0, 1.0, 0.05)]
	public float Friction
	{
		get
		{
			return HC_0001;
		}
		set
		{
			HC_0001 = MathHelper.Clamp(value, 0f, 1f);
			HCH++;
		}
	}

	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	public int CollisionId => HCH;

	[SpecialName]
	internal void _00129(_0001CB P_0)
	{
		_0001CB obj = HCB;
		_0001CB obj2;
		do
		{
			obj2 = obj;
			_0001CB value = (_0001CB)Delegate.Combine(obj2, P_0);
			obj = Interlocked.CompareExchange(ref HCB, value, obj2);
		}
		while ((object)obj != obj2);
	}

	[SpecialName]
	internal void _0012_0004(_0001CB P_0)
	{
		_0001CB obj = HCB;
		_0001CB obj2;
		do
		{
			obj2 = obj;
			_0001CB value = (_0001CB)Delegate.Remove(obj2, P_0);
			obj = Interlocked.CompareExchange(ref HCB, value, obj2);
		}
		while ((object)obj != obj2);
	}

	internal void _0012M()
	{
		HCH++;
		if (HCB != null)
		{
			HCB();
		}
	}

	/// <summary>
	/// Returns the width of the 
	/// </summary>
	/// <returns></returns>
	public float GetTileWidth()
	{
		return 1f / HC_0011;
	}

	private bool _0012P(Texture2D P_0, Texture2D P_1)
	{
		if (P_0 != P_1)
		{
			return P_0 == null;
		}
		return true;
	}

	private void _0012t()
	{
		if (HCG != null)
		{
			int num = 1;
			if (!_0012P(HCY, HC_0013) || !_0012P(HCR, HCX))
			{
				num = 4;
			}
			else if (!_0012P(HCc, HC_0013) || !_0012P(HCq, HCX))
			{
				num = 3;
			}
			else if (!_0012P(HCA, HC_0013) || !_0012P(HCu, HCX))
			{
				num = 2;
			}
			EffectHelper._0012_0005(num, ref HC_0012, ref HCG);
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
		B(graphicsdevice, true);
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
		B(P_0, P_2);
	}

	private void B(GraphicsDevice P_0, bool P_1)
	{
		HCG = base.Parameters["LayerCount"];
		HC_0010 = base.Parameters["DiffuseLayer1Texture"];
		HC_0014 = base.Parameters["DiffuseLayer2Texture"];
		HCL = base.Parameters["DiffuseLayer3Texture"];
		HCh = base.Parameters["DiffuseLayer4Texture"];
		HCT = base.Parameters["NormalLayer1Texture"];
		HCa = base.Parameters["NormalLayer2Texture"];
		HCS = base.Parameters["NormalLayer3Texture"];
		HCr = base.Parameters["NormalLayer4Texture"];
		HCJ = base.Parameters["HeightMapTexture"];
		HC_0006 = base.Parameters["HeightMapPSTexture"];
		HCo = base.Parameters["BlendMapTexture"];
		HCe = base.Parameters["MeshSegments"];
		HC_0015 = base.Parameters["MeshRepeatCount"];
		HCU = base.Parameters["NormalMapSize"];
		HC8 = base.Parameters["NormalMapStrength"];
		HCj = base.Parameters["DiffuseScale"];
		HCi = base.Parameters["HeightScale"];
		HCO = base.Parameters["Tiling"];
		HC_0017 = base.Parameters["SpecularPower"];
		HC_0019 = base.Parameters["SpecularAmount"];
		HCp = base.Parameters["SpecularColor"];
		HC_0013 = SunBurnCoreSystem.Instance._0002l("White");
		HCs = SunBurnCoreSystem.Instance._0002_000E();
		HCX = SunBurnCoreSystem.Instance._0002l("Normal");
		DiffuseMapLayer1Texture = HC_0013;
		DiffuseMapLayer2Texture = HC_0013;
		DiffuseMapLayer3Texture = HC_0013;
		DiffuseMapLayer4Texture = HC_0013;
		NormalMapLayer1Texture = HCX;
		NormalMapLayer2Texture = HCX;
		NormalMapLayer3Texture = HCX;
		NormalMapLayer4Texture = HCX;
		BlendMapTexture = HC_0013;
		HeightMapTexture = HCs;
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
		_0012t();
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
		F.B._7_0004(ref HCF);
		SunBurnEditor.OnDisposeResource(this);
	}
}
