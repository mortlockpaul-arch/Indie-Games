using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using _0003;
using _8;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Effect class with full support for, and binding of, FX Standard Annotations and Semantics (SAS).
/// </summary>
public abstract class BaseSasEffect : BaseSasBindEffect, IRenderableEffect, ISkinnedEffect, _8._0018, _0003._0017, IEditorObject, INamedObject, IStaticLightingEffect
{
	/// <summary />
	protected byte[] EffectByteCode;

	private Matrix _3A_0018;

	private Matrix _3AL;

	private Matrix _3A_0019;

	private Matrix _3A3;

	private Matrix _3A6;

	private Matrix _3AD;

	private string _3A_0017 = "";

	private string _3A_0003 = "";

	private string _3Al = "";

	private new string _3At = "";

	private bool _3AF;

	private Matrix[] _3Ac = new Matrix[1];

	private CompositeLighting _3Ag;

	private Texture2D _3AI;

	private Texture2D _3A8;

	private LightMap _3AZ;

	private EffectParameter _3Ax;

	private EffectParameter _3Aq;

	private EffectParameter _3Ab;

	private EffectParameter _3AT;

	private EffectParameter _3Ay;

	private EffectParameter _3A_0015;

	private Matrix[] _3A_0001 = new Matrix[1];

	private Matrix[] _3A7 = new Matrix[1];

	[CompilerGenerated]
	private bool _3AX;

	/// <summary>
	/// World matrix applied to geometry using this effect.
	/// </summary>
	public Matrix World
	{
		get
		{
			return _3A_0018;
		}
		set
		{
			if (!(_3A_0018 == value))
			{
				Matrix.Invert(ref value, out var result);
				SetWorldAndWorldToObject(ref value, ref result);
			}
		}
	}

	/// <summary>
	/// View matrix applied to geometry using this effect.
	/// </summary>
	public Matrix View
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			if (!(_3A_0019 == value))
			{
				_3A_0019 = value;
				_3A3 = Matrix.Invert(value);
				SyncTransformEffectData();
			}
		}
	}

	/// <summary>
	/// Projection matrix applied to geometry using this effect.
	/// </summary>
	public Matrix Projection
	{
		get
		{
			return _3A6;
		}
		set
		{
			if (!(_3A6 == value))
			{
				_3A6 = value;
				_3AD = Matrix.Invert(value);
				SyncTransformEffectData();
			}
		}
	}

	/// <summary>
	/// Inverse projection matrix applied to geometry using this effect.
	/// </summary>
	protected Matrix ProjectionToView => _3AD;

	/// <summary>
	/// Applies the user's effect preference. This generally trades detail
	/// for performance based on the user's selection.
	/// </summary>
	public DetailPreference EffectDetail
	{
		get
		{
			return DetailPreference.High;
		}
		set
		{
		}
	}

	/// <summary>
	/// Array of bone transforms for the skeleton's current pose. The matrix index is the
	/// same as the bone order used in the model or vertex buffer.
	/// </summary>
	public Matrix[] SkinBones
	{
		get
		{
			return _3Ac;
		}
		set
		{
			if (!_3AF || _3Ax == null)
			{
				return;
			}
			if (value != null)
			{
				_3Ac = value;
				SyncSkinBoneEffectData();
				return;
			}
			if (_3A_0001.Length < _3Ax.Elements.Count)
			{
				_3A_0001 = new Matrix[_3Ax.Elements.Count];
				for (int i = 0; i < _3A_0001.Length; i++)
				{
					ref Matrix reference = ref _3A_0001[i];
					reference = Matrix.Identity;
				}
			}
			if (_3Ac != _3A_0001)
			{
				_3Ac = _3A_0001;
				SyncSkinBoneEffectData();
			}
		}
	}

	/// <summary>
	/// Determines if the effect is currently rendering skinned objects.
	/// </summary>
	public bool Skinned
	{
		get
		{
			return _3AF;
		}
		set
		{
			_3AF = value;
			SetTechnique();
		}
	}

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
			return _3AX;
		}
		[CompilerGenerated]
		set
		{
			_3AX = value;
		}
	}

	internal string MaterialName
	{
		get
		{
			return _3A_0017;
		}
		set
		{
			_3A_0017 = text;
		}
	}

	string _8._0018.MaterialFile => _3A_0003;

	internal string MaterialFile
	{
		get
		{
			return _3A_0003;
		}
		set
		{
			_3A_0003 = text;
		}
	}

	internal string ProjectFile
	{
		get
		{
			return _3Al;
		}
		set
		{
			_3Al = text;
		}
	}

	string _0003._0017.ProjectFile => _3Al;

	internal string EffectFile
	{
		get
		{
			return _3At;
		}
		set
		{
			_3At = text;
		}
	}

	/// <summary>
	/// Effect parameter used to set the bone transform array.
	/// </summary>
	protected EffectParameter SkinBonesEffectParameter
	{
		get
		{
			return _3Ax;
		}
		set
		{
			_3Ax = value;
		}
	}

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
		if (lightmap == null || lightingmode == StaticLightingEffectMode.Composite || lightingmode == StaticLightingEffectMode.Ambient)
		{
			lightmap = _3AZ;
		}
		if (lightingmode == StaticLightingEffectMode.Ambient || lightingmode == StaticLightingEffectMode.BakedDown)
		{
			compositelighting = default(CompositeLighting);
		}
		EffectHelper._0019h(lightmap.LightMapColorTexture, ref _3AI, ref _3Aq);
		EffectHelper._0019h(lightmap.LightMapDirectionalTexture, ref _3A8, ref _3Ab);
		if (_3AT != null && _3Ay != null && _3A_0015 != null && !_3Ag.Equals(compositelighting))
		{
			_3AT.SetValue(compositelighting.AmbientColor);
			_3Ay.SetValue(compositelighting.DiffuseColor);
			_3A_0015.SetValue(compositelighting.Direction);
			_3Ag = compositelighting;
		}
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
		SetStaticLighting(lightingmode, lightmap, ref _3Ag);
	}

	/// <summary>
	/// Sets both the world and inverse world matrices.  Used to improve
	/// performance in effects that automatically generate an inverse
	/// world matrix when the world matrix is set, by providing a cached
	/// or precalculated inverse matrix with the world matrix.
	/// </summary>
	/// <param name="world">World matrix applied to geometry using this effect.</param>
	/// <param name="worldtoobj">Inverse world matrix applied to geometry using this effect.</param>
	public void SetWorldAndWorldToObject(ref Matrix world, ref Matrix worldtoobj)
	{
		if (!(_3A_0018 == world))
		{
			_3A_0018 = world;
			_3AL = worldtoobj;
			SyncTransformEffectData();
			SyncSkinBoneEffectData();
		}
	}

	/// <summary>
	/// Sets both the view, projection, and their inverse matrices.  Used to improve
	/// performance in effects that automatically generate an inverse
	/// matrix when the view and project are set, by providing a cached
	/// or precalculated inverse matrix with the view and project matrices.
	/// </summary>
	/// <param name="view">View matrix applied to geometry using this effect.</param>
	/// <param name="viewtoworld">Inverse view matrix applied to geometry using this effect.</param>
	/// <param name="projection">Projection matrix applied to geometry using this effect.</param>
	/// <param name="projectiontoview">Inverse projection matrix applied to geometry using this effect.</param>
	public void SetViewAndProjection(Matrix view, Matrix viewtoworld, Matrix projection, Matrix projectiontoview)
	{
		bool flag = false;
		if (view != _3A_0019 || viewtoworld != _3A3)
		{
			_3A_0019 = view;
			_3A3 = viewtoworld;
			flag = true;
		}
		if (projection != _3A6)
		{
			_3A6 = projection;
			_3AD = projectiontoview;
			flag = true;
		}
		if (flag)
		{
			SyncTransformEffectData();
		}
	}

	/// <summary>
	/// Sets the effect technique based on its current property values.
	/// </summary>
	protected virtual void SetTechnique()
	{
	}

	/// <summary>
	/// Applies the current transform information to the bound effect parameters.
	/// </summary>
	protected virtual void SyncTransformEffectData()
	{
		EffectHelper._0019_0004(base.SasAutoBindTable.Find("Sas.Camera.Position"), new Vector4(_3A3.Translation, 1f));
		EffectHelper._0019s(base.SasAutoBindTable.Find("Sas.Camera.World"), _3A_0018);
		EffectHelper._0019s(base.SasAutoBindTable.Find("Sas.Camera.WorldInverse"), _3AL);
		EffectHelper._0019s(base.SasAutoBindTable.Find("Sas.Camera.WorldToView"), _3A_0019);
		EffectHelper._0019s(base.SasAutoBindTable.Find("Sas.Camera.WorldToViewInverse"), _3A3);
		EffectHelper._0019s(base.SasAutoBindTable.Find("Sas.Camera.Projection"), _3A6);
		EffectHelper._0019s(base.SasAutoBindTable.Find("Sas.Camera.ProjectionInverse"), _3AD);
		EffectHelper._0019_0011(base.SasAutoBindTable.Find("Sas.Camera.WorldTranspose"), _3A_0018);
		EffectHelper._0019_0011(base.SasAutoBindTable.Find("Sas.Camera.WorldInverseTranspose"), _3AL);
		EffectHelper._0019_0011(base.SasAutoBindTable.Find("Sas.Camera.WorldToViewTranspose"), _3A_0019);
		EffectHelper._0019_0011(base.SasAutoBindTable.Find("Sas.Camera.WorldToViewInverseTranspose"), _3A3);
		EffectHelper._0019_0011(base.SasAutoBindTable.Find("Sas.Camera.ProjectionTranspose"), _3A6);
		EffectHelper._0019_0011(base.SasAutoBindTable.Find("Sas.Camera.ProjectionInverseTranspose"), _3AD);
		List<EffectParameter> list = base.SasAutoBindTable.Find("Sas.Camera.ObjectToView");
		List<EffectParameter> list2 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToViewTranspose");
		List<EffectParameter> list3 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToProjection");
		List<EffectParameter> list4 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToProjectionTranspose");
		if (list != null || list2 != null || list3 != null || list4 != null)
		{
			Matrix matrix = _3A_0018 * _3A_0019;
			Matrix matrix2 = matrix * _3A6;
			EffectHelper._0019s(list, matrix);
			EffectHelper._0019_0011(list2, matrix);
			EffectHelper._0019s(list3, matrix2);
			EffectHelper._0019_0011(list4, matrix2);
		}
		List<EffectParameter> list5 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToViewInverse");
		List<EffectParameter> list6 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToViewInverseTranspose");
		List<EffectParameter> list7 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToProjectionInverse");
		List<EffectParameter> list8 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToProjectionInverseTranspose");
		if (list5 != null || list6 != null || list7 != null || list8 != null)
		{
			Matrix matrix3 = _3A3 * _3AL;
			Matrix matrix4 = _3AD * matrix3;
			EffectHelper._0019s(list5, matrix3);
			EffectHelper._0019_0011(list6, matrix3);
			EffectHelper._0019s(list7, matrix4);
			EffectHelper._0019_0011(list8, matrix4);
		}
	}

	/// <summary>
	/// Applies the current bone transform information to the bound effect parameters.
	/// </summary>
	protected virtual void SyncSkinBoneEffectData()
	{
		if (_3AF && _3Ax != null)
		{
			if (_3A7.Length < _3Ac.Length)
			{
				_3A7 = new Matrix[_3Ac.Length];
			}
			for (int i = 0; i < _3Ac.Length; i++)
			{
				ref Matrix reference = ref _3A7[i];
				reference = _3Ac[i] * _3A_0018;
			}
			Math.Min(_3A7.Length, _3Ax.Elements.Count);
			_3Ax.SetValue(_3A7);
		}
	}

	internal BaseSasEffect(GraphicsDevice P_0, byte[] P_1, bool P_2)
		: base(P_0, P_1)
	{
		_3Ax = FindBySasAddress("Sas.Skeleton.MeshToJointToWorld[*]");
		_0018(P_2);
	}

	private void _0018(bool P_0)
	{
		_3Aq = FindBySemantic("LIGHTMAPCOLORTEXTURE");
		_3Ab = FindBySemantic("LIGHTMAPDIRECTIONALTEXTURE");
		_3AT = FindBySemantic("COMPOSITELIGHTINGAMBIENT");
		_3Ay = FindBySemantic("COMPOSITELIGHTINGDIFFUSE");
		_3A_0015 = FindBySemantic("COMPOSITELIGHTINGDIRECTION");
		Texture2D texture2D = SunBurnCoreSystem.Instance.Lw("Black");
		_3AZ = new LightMap(texture2D, texture2D);
		if (P_0)
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
