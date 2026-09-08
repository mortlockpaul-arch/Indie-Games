using System.Runtime.CompilerServices;
using _0003;
using _8;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using l;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Base class that provides data for SunBurn materials (bump, specular, parallax, ...).  Used by the
/// forward rendering LightingEffect and deferred rendering DeferredObjectEffect classes.
/// </summary>
[EditorObject(true)]
public abstract class BaseMaterialEffect : BaseSkinnedEffect, IAddressableEffect, IStaticLightingEffect, ITransparentEffect, _8._0018, _0003._0017, IEditorObject, INamedObject, ICollisionMaterial
{
	private bool _3A_0018;

	/// <summary />
	protected BaseLight _CurrentLight;

	/// <summary />
	protected StaticLightingEffectMode _CurrentStaticLightingEffectMode;

	private TransparencyMode _3AL;

	private float _3A_0019 = 0.5f;

	private float _3A3;

	private EffectParameter _3A6;

	private int _3AD;

	private float _3A_0017 = 0.75f;

	private float _3A_0003 = 0.25f;

	/// <summary />
	protected Texture2D _NormalMapTexture;

	/// <summary />
	protected Texture2D _DiffuseMapTexture;

	private int _3Al;

	private CompositeLighting _3At = default(CompositeLighting);

	private Texture2D _3AF;

	private Texture2D _3Ac;

	/// <summary />
	protected Texture2D _DefaultDiffuseMapTexture;

	/// <summary />
	protected Texture2D _DefaultNormalMapTexture;

	private Texture2D _3Ag;

	/// <summary />
	protected Texture2D _DefaultEmissiveMapTexture;

	private LightMap _3AI;

	private string _3A8 = "";

	/// <summary />
	protected Vector3 _AlphaPreblend_NoPreBlend_Additive;

	/// <summary />
	protected EffectParameter _AlphaPreblend_NoPreBlend_AdditiveParam;

	private EffectParameter _3AZ;

	private EffectParameter _3Ax;

	private EffectParameter _3Aq;

	private EffectParameter _3Ab;

	private EffectParameter _3AT;

	private EffectParameter _3Ay;

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

	private EffectParameter _3A_0015;

	private float _3A_0001;

	private new float _3A7;

	private EffectParameter _3AX;

	[CompilerGenerated]
	private bool _3A_0010;

	[CompilerGenerated]
	private string _3A_0016;

	[CompilerGenerated]
	private string _3Ak;

	[CompilerGenerated]
	private string _3Ap;

	[CompilerGenerated]
	private string _3Au;

	[CompilerGenerated]
	private TextureAddressMode _3AU;

	[CompilerGenerated]
	private TextureAddressMode _3Az;

	[CompilerGenerated]
	private TextureAddressMode _3A_0014;

	[CompilerGenerated]
	private bool _3Av;

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
			return _3A_0010;
		}
		[CompilerGenerated]
		set
		{
			_3A_0010 = value;
		}
	}

	internal string MaterialFile
	{
		get
		{
			return _3A8;
		}
		set
		{
			_3A8 = text;
		}
	}

	string _8._0018.MaterialFile => _3A8;

	string _0003._0017.ProjectFile => ProjectFile;

	internal string MaterialName
	{
		[CompilerGenerated]
		get
		{
			return _3A_0016;
		}
		[CompilerGenerated]
		set
		{
			_3A_0016 = text;
		}
	}

	internal string ProjectFile
	{
		[CompilerGenerated]
		get
		{
			return _3Ak;
		}
		[CompilerGenerated]
		set
		{
			_3Ak = text;
		}
	}

	internal string NormalMapFile
	{
		[CompilerGenerated]
		get
		{
			return _3Ap;
		}
		[CompilerGenerated]
		set
		{
			_3Ap = text;
		}
	}

	internal string DiffuseMapFile
	{
		[CompilerGenerated]
		get
		{
			return _3Au;
		}
		[CompilerGenerated]
		set
		{
			_3Au = text;
		}
	}

	/// <summary>
	/// Texture normal-map used to apply bump mapping to materials. Setting the
	/// texture to null disables this feature.
	/// </summary>
	[l._0018("NormalMapFile", false)]
	[EditorProperty(true, Description = "Normal Map", HorizontalAlignment = false, MajorGrouping = 1, MinorGrouping = 3, ToolTipText = "")]
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
	[l._0018("DiffuseMapFile", false)]
	[EditorProperty(true, Description = "Diffuse Map", HorizontalAlignment = false, MajorGrouping = 1, MinorGrouping = 1, ToolTipText = "")]
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
	[EditorNumberPadOptions(2, 0.0, 1.0, 0.02)]
	[EditorProperty(true, Description = "Amount", HorizontalAlignment = true, MajorGrouping = 7, MinorGrouping = 13, ToolTipText = "")]
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
			EffectHelper._0019_0004(new Vector4(value.X, value.Y, value.Z, 1f), ref _EmissiveColor, ref _3A_0015);
		}
	}

	/// <summary>
	/// Power applied to material specular reflections. Affects how shiny a material appears.
	/// </summary>
	[EditorNumberPadOptions(2, 0.0, 256.0, 0.5)]
	[EditorProperty(true, Description = "Specular Power", HorizontalAlignment = false, MajorGrouping = 3, MinorGrouping = 2, ToolTipText = "")]
	public float SpecularPower
	{
		get
		{
			return _3A_0001;
		}
		set
		{
			_0019p(value, _3A7);
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
			return _3A7;
		}
		set
		{
			_0019p(_3A_0001, value);
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
			return _3AU;
		}
		[CompilerGenerated]
		set
		{
			_3AU = value;
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
			return _3Az;
		}
		[CompilerGenerated]
		set
		{
			_3Az = value;
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
			return _3A_0014;
		}
		[CompilerGenerated]
		set
		{
			_3A_0014 = value;
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
			return _3AL;
		}
		set
		{
			if (_3AL != value)
			{
				_3AL = value;
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
	[EditorNumberPadOptions(3, 0.0, 1.0, 0.005)]
	[EditorProperty(true, Description = "Threshold", HorizontalAlignment = true, MajorGrouping = 7, MinorGrouping = 12, ToolTipText = "")]
	public virtual float TransparencyThreshold
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
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
			return _3Av;
		}
		[CompilerGenerated]
		set
		{
			_3Av = value;
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
			return _3A_0017;
		}
		set
		{
			_3A_0017 = MathHelper.Clamp(value, 0f, 1f);
			_3AD++;
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
			return _3A_0003;
		}
		set
		{
			_3A_0003 = MathHelper.Clamp(value, 0f, 1f);
			_3AD++;
		}
	}

	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	public int CollisionId => _3AD;

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
			lightmap = _3AI;
		}
		int num = (flag ? 1 : 0);
		Texture2D lightMapColorTexture = lightmap.LightMapColorTexture;
		if (num != _3Al || (flag && lightMapColorTexture != _3AF))
		{
			if (flag)
			{
				EffectHelper._0019h(lightMapColorTexture, ref _3AF, ref _3Ax);
				EffectHelper._0019h(lightmap.LightMapDirectionalTexture, ref _3Ac, ref _3Aq);
			}
			EffectHelper._0019_0013(num, ref _3Al, ref _3AZ);
			_UpdatedByBatch = true;
		}
		if (flag2 && _3Ab != null && _3AT != null && _3Ay != null && !_3At.Equals(compositelighting))
		{
			_3Ab.SetValue(compositelighting.AmbientColor);
			_3AT.SetValue(compositelighting.DiffuseColor);
			_3Ay.SetValue(compositelighting.Direction);
			_3At = compositelighting;
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
		SetStaticLighting(lightingmode, lightmap, ref _3At);
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
		bool changedmode = _3AL != mode;
		_3AL = mode;
		_3A_0019 = threshold;
		DiffuseMapTexture = map as Texture2D;
		SyncTransparency(changedmode);
	}

	/// <summary>
	/// Applies the object's transparency information to its effect parameters.
	/// </summary>
	protected virtual void SyncTransparency(bool changedmode)
	{
		float num = _3A3;
		Vector3 vector = Vector3.UnitY;
		if (_3AL == TransparencyMode.Clip)
		{
			num = TransparencyThreshold;
		}
		else if (_3AL == TransparencyMode.None)
		{
			num = 0f;
		}
		else
		{
			vector = ((_3AL != TransparencyMode.Additive) ? Vector3.UnitX : new Vector3(1f, 0f, 1f));
			num = 0.04f;
		}
		EffectHelper._00190(num, ref _3A3, ref _3A6);
		EffectHelper._0019J(vector, ref _AlphaPreblend_NoPreBlend_Additive, ref _AlphaPreblend_NoPreBlend_AdditiveParam);
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
			EffectHelper._0019h(_DefaultDiffuseMapTexture, ref _DiffuseMapTexture, ref _DiffuseMapTextureIndirectParam);
			EffectHelper._0019_0004(diffusecolor, ref _DiffuseColorCached, ref _DiffuseColorIndirectParam);
		}
		else
		{
			EffectHelper._0019h(diffusemap, ref _DiffuseMapTexture, ref _DiffuseMapTextureIndirectParam);
			EffectHelper._0019_0004(new Vector4(Vector3.One, diffusecolor.W), ref _DiffuseColorCached, ref _DiffuseColorIndirectParam);
		}
		if (normalmap == null || normalmap == _DefaultNormalMapTexture)
		{
			if (_3A7 > 0f && _3A_0001 > 0f)
			{
				EffectHelper._0019h(_DefaultNormalMapTexture, ref _NormalMapTexture, ref _NormalMapTextureIndirectParam);
			}
			else
			{
				EffectHelper._0019h(null, ref _NormalMapTexture, ref _NormalMapTextureIndirectParam);
			}
		}
		else
		{
			EffectHelper._0019h(normalmap, ref _NormalMapTexture, ref _NormalMapTextureIndirectParam);
		}
	}

	private void _0019p(float P_0, float P_1)
	{
		if ((_3A_0001 != P_0 || _3A7 != P_1) && _3AX != null)
		{
			_3A_0001 = P_0;
			_3A7 = P_1;
			if (_3A_0001 <= 0f || _3A7 <= 0f)
			{
				_3AX.SetValue(new Vector4(10000f, 0f, 0f, 0f));
			}
			else
			{
				_3AX.SetValue(new Vector4(_3A_0001, _3A7, 0f, 0f));
			}
		}
	}

	/// <summary>
	/// Sets the effect technique based on its current property values.
	/// </summary>
	protected override void SetTechnique()
	{
		base._3A7._3A_0018.AccumulationValue++;
		bool flag = DoubleSided && _3A_0018;
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
					base.CurrentTechnique = base.Techniques[_8.L._3L(_8.L.DA_0018.Ambient, _8.L.DAL.Tangent, 1, false, false, base.Skinned, false)];
				}
				else
				{
					base.CurrentTechnique = base.Techniques[_8.L._3L(_8.L.DA_0018.Ambient, _8.L.DAL.None, 1, false, false, base.Skinned, false)];
				}
			}
			else
			{
				_8.L.DAL dAL = _8.L.DAL.Diffuse;
				if (_NormalMapTexture != null)
				{
					dAL = ((!flag3 || !(_3A_0001 > 0f) || !(_3A7 > 0f)) ? _8.L.DAL.DiffuseBump : _8.L.DAL.DiffuseBumpSpecular);
				}
				base.CurrentTechnique = base.Techniques[_8.L._3L(_8.L.DA_0018.Lighting, dAL, 1, flag, false, base.Skinned, false)];
			}
		}
		else
		{
			base.CurrentTechnique = base.Techniques[_8.L._3L(_8.L.DA_0018.Ambient, _8.L.DAL.None, 1, false, false, base.Skinned, false)];
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
		_0018(graphicsdevice, true);
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
		_0018(P_0, P_2);
	}

	private void _0018(GraphicsDevice P_0, bool P_1)
	{
		_3A_0018 = base.GraphicsDevice.GraphicsProfile == GraphicsProfile.HiDef;
		_3AX = base.Parameters["_SpecularPower_And_Amount"];
		_3A6 = base.Parameters["_TransClipRef"];
		_DiffuseColorIndirectParam = base.Parameters["_DiffuseColor"];
		_DiffuseMapTextureIndirectParam = base.Parameters["_DiffuseMapTexture"];
		_NormalMapTextureIndirectParam = base.Parameters["_NormalMapTexture"];
		_3A_0015 = base.Parameters["_EmissiveColor"];
		_AlphaPreblend_NoPreBlend_AdditiveParam = base.Parameters["_AlphaPreblend_NoPreBlend_Additive"];
		_3AZ = base.Parameters["_LightMapped"];
		_3Ax = base.Parameters["_LightMapColorTexture"];
		_3Aq = base.Parameters["_LightMapDirectionalTexture"];
		_3Ab = base.Parameters["_CompositeLightingAmbient"];
		_3AT = base.Parameters["_CompositeLightingDiffuse"];
		_3Ay = base.Parameters["_CompositeLightingDirection"];
		Texture2D texture2D = SunBurnCoreSystem.Instance.Lw("White");
		Texture2D texture2D2 = SunBurnCoreSystem.Instance.Lw("Black");
		_DefaultDiffuseMapTexture = texture2D;
		_DefaultNormalMapTexture = SunBurnCoreSystem.Instance.Lw("Normal");
		_3Ag = texture2D;
		_DefaultEmissiveMapTexture = texture2D2;
		_3AI = new LightMap(texture2D2, texture2D2);
		DiffuseColor = Vector3.One;
		TransparencyAmount = 1f;
		SpecularPower = 4f;
		SpecularAmount = 0.25f;
		SetStaticLighting(StaticLightingEffectMode.Ambient, _3AI);
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
