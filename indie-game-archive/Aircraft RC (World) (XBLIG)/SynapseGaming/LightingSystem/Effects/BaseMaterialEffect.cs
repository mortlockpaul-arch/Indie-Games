using System.Runtime.CompilerServices;
using _0003;
using _000F;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using Z;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Base class that provides data for SunBurn materials (bump, specular, parallax, ...).  Used by the
/// forward rendering LightingEffect and deferred rendering DeferredObjectEffect classes.
/// </summary>
[EditorObject(true)]
public abstract class BaseMaterialEffect : BaseSkinnedEffect, IAddressableEffect, IStaticLightingEffect, ITransparentEffect, _0003.B, Z.w, IEditorObject, INamedObject, ICollisionMaterial
{
	private bool HCB;

	/// <summary />
	protected BaseLight _CurrentLight;

	/// <summary />
	protected StaticLightingEffectMode _CurrentStaticLightingEffectMode;

	private TransparencyMode HC_0002;

	private float HC_0012 = 0.5f;

	private float HCH;

	private EffectParameter HC7;

	private int HC_0001;

	private float HCw = 0.75f;

	private float HCZ = 0.25f;

	/// <summary />
	protected Texture2D _NormalMapTexture;

	/// <summary />
	protected Texture2D _DiffuseMapTexture;

	private int HC_000F;

	private CompositeLighting HCy = default(CompositeLighting);

	private Texture2D HC6;

	private Texture2D HCD;

	/// <summary />
	protected Texture2D _DefaultDiffuseMapTexture;

	/// <summary />
	protected Texture2D _DefaultNormalMapTexture;

	private Texture2D HC_0011;

	/// <summary />
	protected Texture2D _DefaultEmissiveMapTexture;

	private LightMap HCK;

	private string HC_0003 = "";

	/// <summary />
	protected Vector3 _AlphaPreblend_NoPreBlend_Additive;

	/// <summary />
	protected EffectParameter _AlphaPreblend_NoPreBlend_AdditiveParam;

	private EffectParameter HCk;

	private EffectParameter HCs;

	private EffectParameter HC_0013;

	private EffectParameter HCX;

	private EffectParameter HCz;

	private EffectParameter HCA;

	/// <summary />
	protected EffectParameter _DiffuseColorIndirectParam;

	/// <summary />
	protected EffectParameter _DiffuseMapTextureIndirectParam;

	/// <summary />
	protected EffectParameter _NormalMapTextureIndirectParam;

	/// <summary />
	protected Vector4 _DiffuseColorOriginal;

	/// <summary />
	protected Vector4 _DiffuseColorCached;

	/// <summary />
	protected Vector4 _EmissiveColor;

	private EffectParameter HCc;

	private float HCY;

	private new float HCV;

	private EffectParameter HCu;

	[CompilerGenerated]
	private bool HCq;

	[CompilerGenerated]
	private string HCR;

	[CompilerGenerated]
	private string HCN;

	[CompilerGenerated]
	private string HCF;

	[CompilerGenerated]
	private string HCf;

	[CompilerGenerated]
	private TextureAddressMode HCG;

	[CompilerGenerated]
	private TextureAddressMode HC_0010;

	[CompilerGenerated]
	private TextureAddressMode HC_0014;

	[CompilerGenerated]
	private bool HCL;

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
			return HCq;
		}
		[CompilerGenerated]
		set
		{
			HCq = value;
		}
	}

	internal string MaterialFile
	{
		get
		{
			return HC_0003;
		}
		set
		{
			HC_0003 = text;
		}
	}

	string _0003.B.MaterialFile => HC_0003;

	string Z.w.ProjectFile => ProjectFile;

	internal string MaterialName
	{
		[CompilerGenerated]
		get
		{
			return HCR;
		}
		[CompilerGenerated]
		set
		{
			HCR = hCR;
		}
	}

	internal string ProjectFile
	{
		[CompilerGenerated]
		get
		{
			return HCN;
		}
		[CompilerGenerated]
		set
		{
			HCN = hCN;
		}
	}

	internal string NormalMapFile
	{
		[CompilerGenerated]
		get
		{
			return HCF;
		}
		[CompilerGenerated]
		set
		{
			HCF = hCF;
		}
	}

	internal string DiffuseMapFile
	{
		[CompilerGenerated]
		get
		{
			return HCf;
		}
		[CompilerGenerated]
		set
		{
			HCf = hCf;
		}
	}

	/// <summary>
	/// Texture normal-map used to apply bump mapping to materials. Setting the
	/// texture to null disables this feature.
	/// </summary>
	[EditorProperty(true, Description = "Normal Map", HorizontalAlignment = false, MajorGrouping = 1, MinorGrouping = 3, ToolTipText = "")]
	[_000F.B("NormalMapFile", false)]
	public Texture2D NormalMapTexture
	{
		get
		{
			return _NormalMapTexture;
		}
		set
		{
			SyncDiffuseAndNormalData(_DiffuseColorOriginal, _DiffuseMapTexture, value);
			SetTechnique();
		}
	}

	/// <summary>
	/// Texture used as the primary color map for materials. Generally this texture
	/// includes shading and lighting information when bump mapping is not used. Setting
	/// the texture to null disables this feature.
	/// </summary>
	[EditorProperty(true, Description = "Diffuse Map", HorizontalAlignment = false, MajorGrouping = 1, MinorGrouping = 1, ToolTipText = "")]
	[_000F.B("DiffuseMapFile", false)]
	public Texture2D DiffuseMapTexture
	{
		get
		{
			return _DiffuseMapTexture;
		}
		set
		{
			SyncDiffuseAndNormalData(_DiffuseColorOriginal, value, _NormalMapTexture);
		}
	}

	/// <summary>
	/// Base color applied to materials when no DiffuseMapTexture is specified.
	/// </summary>
	[EditorProperty(true, Description = "Diffuse Color", HorizontalAlignment = true, MajorGrouping = 2, MinorGrouping = 2, ControlType = ControlType.ColorSelection, ToolTipText = "")]
	public Vector3 DiffuseColor
	{
		get
		{
			return new Vector3(_DiffuseColorOriginal.X, _DiffuseColorOriginal.Y, _DiffuseColorOriginal.Z);
		}
		set
		{
			SyncDiffuseAndNormalData(new Vector4(value, _DiffuseColorOriginal.W), _DiffuseMapTexture, _NormalMapTexture);
		}
	}

	/// <summary>
	/// Adjusts material transparency when TransparencyMode is Blend or Additive.
	/// </summary>
	[EditorProperty(true, Description = "Amount", HorizontalAlignment = true, MajorGrouping = 7, MinorGrouping = 13, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 1.0, 0.02)]
	public float TransparencyAmount
	{
		get
		{
			return _DiffuseColorOriginal.W;
		}
		set
		{
			SyncDiffuseAndNormalData(new Vector4(_DiffuseColorOriginal.X, _DiffuseColorOriginal.Y, _DiffuseColorOriginal.Z, value), _DiffuseMapTexture, _NormalMapTexture);
		}
	}

	/// <summary>
	/// Color used to apply emissive lighting and self-illumination to materials.
	/// </summary>
	[EditorProperty(true, Description = "Emissive Color", HorizontalAlignment = true, MajorGrouping = 2, MinorGrouping = 3, ControlType = ControlType.ColorSelection, ToolTipText = "")]
	public Vector3 EmissiveColor
	{
		get
		{
			return new Vector3(_EmissiveColor.X, _EmissiveColor.Y, _EmissiveColor.Z);
		}
		set
		{
			EffectHelper._0012v(new Vector4(value.X, value.Y, value.Z, 1f), ref _EmissiveColor, ref HCc);
		}
	}

	/// <summary>
	/// Power applied to material specular reflections. Affects how shiny a material appears.
	/// </summary>
	[EditorProperty(true, Description = "Specular Power", HorizontalAlignment = false, MajorGrouping = 3, MinorGrouping = 2, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 256.0, 0.5)]
	public float SpecularPower
	{
		get
		{
			return HCY;
		}
		set
		{
			_0012f(value, HCV);
			SyncDiffuseAndNormalData(_DiffuseColorOriginal, _DiffuseMapTexture, _NormalMapTexture);
			SetTechnique();
		}
	}

	/// <summary>
	/// Intensity applied to material specular reflections. Affects how intense the specular appears.
	/// </summary>
	[EditorProperty(true, Description = "Specular Amount", HorizontalAlignment = false, MajorGrouping = 3, MinorGrouping = 3, ToolTipText = "")]
	[EditorNumberPadOptions(2, 0.0, 32.0, 0.05)]
	public float SpecularAmount
	{
		get
		{
			return HCV;
		}
		set
		{
			_0012f(HCY, value);
			SyncDiffuseAndNormalData(_DiffuseColorOriginal, _DiffuseMapTexture, _NormalMapTexture);
			SetTechnique();
		}
	}

	/// <summary>
	/// Determines the effect's texture address mode in the U texture-space direction.
	/// </summary>
	[EditorProperty(true, Description = "Addressing U", HorizontalAlignment = true, MajorGrouping = 6, MinorGrouping = 1, ToolTipText = "")]
	public TextureAddressMode AddressModeU
	{
		[CompilerGenerated]
		get
		{
			return HCG;
		}
		[CompilerGenerated]
		set
		{
			HCG = value;
		}
	}

	/// <summary>
	/// Determines the effect's texture address mode in the V texture-space direction.
	/// </summary>
	[EditorProperty(true, Description = "Addressing V", HorizontalAlignment = true, MajorGrouping = 6, MinorGrouping = 2, ToolTipText = "")]
	public TextureAddressMode AddressModeV
	{
		[CompilerGenerated]
		get
		{
			return HC_0010;
		}
		[CompilerGenerated]
		set
		{
			HC_0010 = value;
		}
	}

	/// <summary>
	/// Determines the effect's texture address mode in the W texture-space direction.
	/// </summary>
	[EditorProperty(true, Description = "Addressing W", HorizontalAlignment = true, MajorGrouping = 6, MinorGrouping = 3, ToolTipText = "")]
	public TextureAddressMode AddressModeW
	{
		[CompilerGenerated]
		get
		{
			return HC_0014;
		}
		[CompilerGenerated]
		set
		{
			HC_0014 = value;
		}
	}

	/// <summary>
	/// The transparency style used when rendering the effect.
	/// </summary>
	[EditorCheckboxOptions(true)]
	[EditorProperty(true, Description = "Transparent", HorizontalAlignment = true, MajorGrouping = 7, MinorGrouping = 11, ControlType = ControlType.DropDown, ToolTipText = "")]
	public virtual TransparencyMode TransparencyMode
	{
		get
		{
			return HC_0002;
		}
		set
		{
			if (HC_0002 != value)
			{
				HC_0002 = value;
				SyncTransparency(changedmode: true);
			}
		}
	}

	/// <summary>
	/// Used with TransparencyMode to determine the effect clipped transparency.
	///   -For Clip mode this value is a comparison value, where all TransparencyMap
	///    alpha values below the value are *not* rendered.
	///   -For Blend and Additive mode this value is a comparison value for the *shadow*
	///    transparency, where all TransparencyMap alpha values below the value are
	///    *not* rendered.
	/// </summary>
	[EditorProperty(true, Description = "Threshold", HorizontalAlignment = true, MajorGrouping = 7, MinorGrouping = 12, ToolTipText = "")]
	[EditorNumberPadOptions(3, 0.0, 1.0, 0.005)]
	public virtual float TransparencyThreshold
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
			SyncTransparency(changedmode: false);
		}
	}

	/// <summary>
	/// The texture map used for transparency (values are pulled from the alpha channel).
	/// </summary>
	public Texture TransparencyMap
	{
		get
		{
			return _DiffuseMapTexture;
		}
		set
		{
			DiffuseMapTexture = (Texture2D)value;
		}
	}

	/// <summary>
	/// Surfaces rendered with the effect should be visible from both sides.
	/// </summary>
	[EditorProperty(true, Description = "Double Sided", HorizontalAlignment = true, MajorGrouping = 2, MinorGrouping = 1, ToolTipText = "")]
	public override bool DoubleSided
	{
		[CompilerGenerated]
		get
		{
			return HCL;
		}
		[CompilerGenerated]
		set
		{
			HCL = value;
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
			return HCw;
		}
		set
		{
			HCw = MathHelper.Clamp(value, 0f, 1f);
			HC_0001++;
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
			return HCZ;
		}
		set
		{
			HCZ = MathHelper.Clamp(value, 0f, 1f);
			HC_0001++;
		}
	}

	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	public int CollisionId => HC_0001;

	/// <summary>
	/// Sets the static lighting applied to the effect during rendering.
	/// </summary>
	/// <param name="lightingmode">Determines the static lighting
	/// mode used by the effect.</param>
	/// <param name="lightmap">Light map texture containing static
	/// lighting. If the static lighting mode does not use light
	/// mapping this value can be null.</param>
	/// <param name="compositelighting">Composite lighting containing
	/// static lighting. Only used if the static lighting mode specifies
	/// composite lighting.</param>
	public void SetStaticLighting(StaticLightingEffectMode lightingmode, LightMap lightmap, ref CompositeLighting compositelighting)
	{
		bool flag = lightingmode == StaticLightingEffectMode.BakedDown || lightingmode == StaticLightingEffectMode.BakedDownAndComposite;
		bool flag2 = lightingmode == StaticLightingEffectMode.Composite || lightingmode == StaticLightingEffectMode.BakedDownAndComposite;
		if (lightmap == null)
		{
			lightmap = HCK;
		}
		int num = (flag ? 1 : 0);
		Texture2D lightMapColorTexture = lightmap.LightMapColorTexture;
		if (num != HC_000F || (flag && lightMapColorTexture != HC6))
		{
			if (flag)
			{
				EffectHelper._00120(lightMapColorTexture, ref HC6, ref HCs);
				EffectHelper._00120(lightmap.LightMapDirectionalTexture, ref HCD, ref HC_0013);
			}
			EffectHelper._0012_0005(num, ref HC_000F, ref HCk);
			_UpdatedByBatch = true;
		}
		if (flag2 && HCX != null && HCz != null && HCA != null && !HCy.Equals(compositelighting))
		{
			HCX.SetValue(compositelighting.AmbientColor);
			HCz.SetValue(compositelighting.DiffuseColor);
			HCA.SetValue(compositelighting.Direction);
			HCy = compositelighting;
			_UpdatedByBatch = true;
		}
		_CurrentStaticLightingEffectMode = lightingmode;
		SetTechniqueShaderArrayIndices();
	}

	/// <summary>
	/// Sets the static lighting applied to the effect during rendering.
	/// </summary>
	/// <param name="lightingmode">Determines the static lighting
	/// mode used by the effect.</param>
	/// <param name="lightmap">Light map texture containing static
	/// lighting. If the static lighting mode does not use light
	/// mapping this value can be null.</param>
	public void SetStaticLighting(StaticLightingEffectMode lightingmode, LightMap lightmap)
	{
		SetStaticLighting(lightingmode, lightmap, ref HCy);
	}

	/// <summary>
	/// Sets all transparency information at once.  Used to improve performance
	/// by avoiding multiple effect technique changes.
	/// </summary>
	/// <param name="mode">The transparency style used when rendering the effect.</param>
	/// <param name="threshold">Used with TransparencyMode to determine the effect transparency.
	///   -For Clip mode this value is a comparison value, where all TransparencyMap
	///    alpha values below the value are *not* rendered.
	///   -For Blend and Additive mode this value is a comparison value for the shadow
	///    transparency, where all TransparencyMap alpha values below the value are
	///    *not* rendered.</param>
	/// <param name="map">The texture map used for transparency (values are pulled from the alpha channel).</param>
	public void SetTransparencyModeAndMap(TransparencyMode mode, float threshold, Texture map)
	{
		bool changedmode = HC_0002 != mode;
		HC_0002 = mode;
		HC_0012 = threshold;
		DiffuseMapTexture = map as Texture2D;
		SyncTransparency(changedmode);
	}

	/// <summary>
	/// Applies the object's transparency information to its effect parameters.
	/// </summary>
	protected virtual void SyncTransparency(bool changedmode)
	{
		float hCH = HCH;
		Vector3 vector = Vector3.UnitY;
		if (HC_0002 == TransparencyMode.Clip)
		{
			hCH = TransparencyThreshold;
		}
		else if (HC_0002 == TransparencyMode.None)
		{
			hCH = 0f;
		}
		else
		{
			vector = ((HC_0002 != TransparencyMode.Additive) ? Vector3.UnitX : new Vector3(1f, 0f, 1f));
			hCH = 0.04f;
		}
		EffectHelper._00124(hCH, ref HCH, ref HC7);
		EffectHelper._00122(vector, ref _AlphaPreblend_NoPreBlend_Additive, ref _AlphaPreblend_NoPreBlend_AdditiveParam);
	}

	/// <summary>
	/// Applies the provided diffuse information to the object and its effect parameters.
	/// </summary>
	/// <param name="diffusecolor"></param>
	/// <param name="diffusemap"></param>
	/// <param name="normalmap"></param>
	protected virtual void SyncDiffuseAndNormalData(Vector4 diffusecolor, Texture2D diffusemap, Texture2D normalmap)
	{
		_DiffuseColorOriginal = diffusecolor;
		if (diffusemap == null || diffusemap == _DefaultDiffuseMapTexture)
		{
			EffectHelper._00120(_DefaultDiffuseMapTexture, ref _DiffuseMapTexture, ref _DiffuseMapTextureIndirectParam);
			EffectHelper._0012v(diffusecolor, ref _DiffuseColorCached, ref _DiffuseColorIndirectParam);
		}
		else
		{
			EffectHelper._00120(diffusemap, ref _DiffuseMapTexture, ref _DiffuseMapTextureIndirectParam);
			EffectHelper._0012v(new Vector4(Vector3.One, diffusecolor.W), ref _DiffuseColorCached, ref _DiffuseColorIndirectParam);
		}
		if (normalmap == null || normalmap == _DefaultNormalMapTexture)
		{
			if (HCV > 0f && HCY > 0f)
			{
				EffectHelper._00120(_DefaultNormalMapTexture, ref _NormalMapTexture, ref _NormalMapTextureIndirectParam);
			}
			else
			{
				EffectHelper._00120(null, ref _NormalMapTexture, ref _NormalMapTextureIndirectParam);
			}
		}
		else
		{
			EffectHelper._00120(normalmap, ref _NormalMapTexture, ref _NormalMapTextureIndirectParam);
		}
	}

	private void _0012f(float P_0, float P_1)
	{
		if ((HCY != P_0 || HCV != P_1) && HCu != null)
		{
			HCY = P_0;
			HCV = P_1;
			if (HCY <= 0f || HCV <= 0f)
			{
				HCu.SetValue(new Vector4(10000f, 0f, 0f, 0f));
			}
			else
			{
				HCu.SetValue(new Vector4(HCY, HCV, 0f, 0f));
			}
		}
	}

	/// <summary>
	/// Sets the effect technique based on its current property values.
	/// </summary>
	protected override void SetTechnique()
	{
		base.HCV.HCB.AccumulationValue++;
		bool flag = DoubleSided && HCB;
		if (_CurrentLight != null)
		{
			bool flag2 = _CurrentLight is AmbientLight;
			bool fillLight = _CurrentLight.FillLight;
			_ = base.EffectDetail;
			_ = base.EffectDetail;
			bool flag3 = base.EffectDetail <= DetailPreference.Low && !fillLight;
			if (flag2)
			{
				if (_NormalMapTexture != null)
				{
					base.CurrentTechnique = base.Techniques[_0003._0002.H_0012(_0003._0002._0001CB.Ambient, _0003._0002._0001C_0002.Tangent, 1, false, false, base.Skinned, false)];
				}
				else
				{
					base.CurrentTechnique = base.Techniques[_0003._0002.H_0012(_0003._0002._0001CB.Ambient, _0003._0002._0001C_0002.None, 1, false, false, base.Skinned, false)];
				}
			}
			else
			{
				_0003._0002._0001C_0002 obj = _0003._0002._0001C_0002.Diffuse;
				if (_NormalMapTexture != null)
				{
					obj = ((!flag3 || !(HCY > 0f) || !(HCV > 0f)) ? _0003._0002._0001C_0002.DiffuseBump : _0003._0002._0001C_0002.DiffuseBumpSpecular);
				}
				base.CurrentTechnique = base.Techniques[_0003._0002.H_0012(_0003._0002._0001CB.Lighting, obj, 1, flag, false, base.Skinned, false)];
			}
		}
		else
		{
			base.CurrentTechnique = base.Techniques[_0003._0002.H_0012(_0003._0002._0001CB.Ambient, _0003._0002._0001C_0002.None, 1, false, false, base.Skinned, false)];
		}
		SetTechniqueShaderArrayIndices();
	}

	/// <summary>
	/// Sets the EffectParameter(s) associated with the index into the current technique's
	/// shader array. This method cannot change the current technique, instead use SetTechnique().
	/// </summary>
	protected abstract void SetTechniqueShaderArrayIndices();

	/// <summary>
	/// Creates a new BaseMaterialEffect instance.
	/// </summary>
	/// <param name="graphicsdevice"></param>
	/// <param name="effectname"></param>
	public BaseMaterialEffect(GraphicsDevice graphicsdevice, string effectname)
		: base(graphicsdevice, effectname)
	{
		B(graphicsdevice, true);
	}

	/// <summary>
	/// Creates a new BaseMaterialEffect instance.
	/// </summary>
	/// <param name="graphicsdevice"></param>
	/// <param name="effectname"></param>
	/// <param name="trackeffect"></param>
	internal BaseMaterialEffect(GraphicsDevice P_0, string P_1, bool P_2)
		: base(P_0, P_1)
	{
		B(P_0, P_2);
	}

	private void B(GraphicsDevice P_0, bool P_1)
	{
		HCB = base.GraphicsDevice.GraphicsProfile == GraphicsProfile.HiDef;
		HCu = base.Parameters["_SpecularPower_And_Amount"];
		HC7 = base.Parameters["_TransClipRef"];
		_DiffuseColorIndirectParam = base.Parameters["_DiffuseColor"];
		_DiffuseMapTextureIndirectParam = base.Parameters["_DiffuseMapTexture"];
		_NormalMapTextureIndirectParam = base.Parameters["_NormalMapTexture"];
		HCc = base.Parameters["_EmissiveColor"];
		_AlphaPreblend_NoPreBlend_AdditiveParam = base.Parameters["_AlphaPreblend_NoPreBlend_Additive"];
		HCk = base.Parameters["_LightMapped"];
		HCs = base.Parameters["_LightMapColorTexture"];
		HC_0013 = base.Parameters["_LightMapDirectionalTexture"];
		HCX = base.Parameters["_CompositeLightingAmbient"];
		HCz = base.Parameters["_CompositeLightingDiffuse"];
		HCA = base.Parameters["_CompositeLightingDirection"];
		Texture2D texture2D = SunBurnCoreSystem.Instance._0002l("White");
		Texture2D texture2D2 = SunBurnCoreSystem.Instance._0002l("Black");
		_DefaultDiffuseMapTexture = texture2D;
		_DefaultNormalMapTexture = SunBurnCoreSystem.Instance._0002l("Normal");
		HC_0011 = texture2D;
		_DefaultEmissiveMapTexture = texture2D2;
		HCK = new LightMap(texture2D2, texture2D2);
		DiffuseColor = Vector3.One;
		TransparencyAmount = 1f;
		SpecularPower = 4f;
		SpecularAmount = 0.25f;
		SetStaticLighting(StaticLightingEffectMode.Ambient, HCK);
		SetTechnique();
		MaterialName = string.Empty;
		ProjectFile = string.Empty;
		NormalMapFile = string.Empty;
		DiffuseMapFile = string.Empty;
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
		SunBurnEditor.OnDisposeResource(this);
	}
}
