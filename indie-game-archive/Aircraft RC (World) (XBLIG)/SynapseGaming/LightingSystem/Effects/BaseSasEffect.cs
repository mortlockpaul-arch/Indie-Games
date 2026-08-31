using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using _0003;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Editor;
using SynapseGaming.LightingSystem.Lights;
using Z;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Effect class with full support for, and binding of, FX Standard Annotations and Semantics (SAS).
/// </summary>
public abstract class BaseSasEffect : BaseSasBindEffect, IRenderableEffect, ISkinnedEffect, _0003.B, Z.w, IEditorObject, INamedObject, IStaticLightingEffect
{
	/// <summary />
	protected byte[] EffectByteCode;

	private Matrix HCB;

	private Matrix HC_0002;

	private Matrix HC_0012;

	private Matrix HCH;

	private Matrix HC7;

	private Matrix HC_0001;

	private string HCw = "";

	private string HCZ = "";

	private string HC_000F = "";

	private new string HCy = "";

	private bool HC6;

	private Matrix[] HCD = new Matrix[1];

	private CompositeLighting HC_0011;

	private Texture2D HCK;

	private Texture2D HC_0003;

	private LightMap HCk;

	private EffectParameter HCs;

	private EffectParameter HC_0013;

	private EffectParameter HCX;

	private EffectParameter HCz;

	private EffectParameter HCA;

	private EffectParameter HCc;

	private Matrix[] HCY = new Matrix[1];

	private Matrix[] HCV = new Matrix[1];

	[CompilerGenerated]
	private bool HCu;

	/// <summary>
	/// World matrix applied to geometry using this effect.
	/// </summary>
	public Matrix World
	{
		get
		{
			return HCB;
		}
		set
		{
			if (!(HCB == value))
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
			return HC_0012;
		}
		set
		{
			if (!(HC_0012 == value))
			{
				HC_0012 = value;
				HCH = Matrix.Invert(value);
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
			return HC7;
		}
		set
		{
			if (!(HC7 == value))
			{
				HC7 = value;
				HC_0001 = Matrix.Invert(value);
				SyncTransformEffectData();
			}
		}
	}

	/// <summary>
	/// Inverse projection matrix applied to geometry using this effect.
	/// </summary>
	protected Matrix ProjectionToView => HC_0001;

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
			return HCD;
		}
		set
		{
			if (!HC6 || HCs == null)
			{
				return;
			}
			if (value != null)
			{
				HCD = value;
				SyncSkinBoneEffectData();
				return;
			}
			if (HCY.Length < HCs.Elements.Count)
			{
				HCY = new Matrix[HCs.Elements.Count];
				for (int i = 0; i < HCY.Length; i++)
				{
					ref Matrix reference = ref HCY[i];
					reference = Matrix.Identity;
				}
			}
			if (HCD != HCY)
			{
				HCD = HCY;
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
			return HC6;
		}
		set
		{
			HC6 = value;
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
			return HCu;
		}
		[CompilerGenerated]
		set
		{
			HCu = value;
		}
	}

	internal string MaterialName
	{
		get
		{
			return HCw;
		}
		set
		{
			HCw = hCw;
		}
	}

	string _0003.B.MaterialFile => HCZ;

	internal string MaterialFile
	{
		get
		{
			return HCZ;
		}
		set
		{
			HCZ = hCZ;
		}
	}

	internal string ProjectFile
	{
		get
		{
			return HC_000F;
		}
		set
		{
			HC_000F = text;
		}
	}

	string Z.w.ProjectFile => HC_000F;

	internal string EffectFile
	{
		get
		{
			return HCy;
		}
		set
		{
			HCy = hCy;
		}
	}

	/// <summary>
	/// Effect parameter used to set the bone transform array.
	/// </summary>
	protected EffectParameter SkinBonesEffectParameter
	{
		get
		{
			return HCs;
		}
		set
		{
			HCs = value;
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
			lightmap = HCk;
		}
		if (lightingmode == StaticLightingEffectMode.Ambient || lightingmode == StaticLightingEffectMode.BakedDown)
		{
			compositelighting = default(CompositeLighting);
		}
		EffectHelper._00120(lightmap.LightMapColorTexture, ref HCK, ref HC_0013);
		EffectHelper._00120(lightmap.LightMapDirectionalTexture, ref HC_0003, ref HCX);
		if (HCz != null && HCA != null && HCc != null && !HC_0011.Equals(compositelighting))
		{
			HCz.SetValue(compositelighting.AmbientColor);
			HCA.SetValue(compositelighting.DiffuseColor);
			HCc.SetValue(compositelighting.Direction);
			HC_0011 = compositelighting;
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
		SetStaticLighting(lightingmode, lightmap, ref HC_0011);
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
		if (!(HCB == world))
		{
			HCB = world;
			HC_0002 = worldtoobj;
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
		if (view != HC_0012 || viewtoworld != HCH)
		{
			HC_0012 = view;
			HCH = viewtoworld;
			flag = true;
		}
		if (projection != HC7)
		{
			HC7 = projection;
			HC_0001 = projectiontoview;
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
		EffectHelper._0012v(base.SasAutoBindTable.Find("Sas.Camera.Position"), new Vector4(HCH.Translation, 1f));
		EffectHelper._0012I(base.SasAutoBindTable.Find("Sas.Camera.World"), HCB);
		EffectHelper._0012I(base.SasAutoBindTable.Find("Sas.Camera.WorldInverse"), HC_0002);
		EffectHelper._0012I(base.SasAutoBindTable.Find("Sas.Camera.WorldToView"), HC_0012);
		EffectHelper._0012I(base.SasAutoBindTable.Find("Sas.Camera.WorldToViewInverse"), HCH);
		EffectHelper._0012I(base.SasAutoBindTable.Find("Sas.Camera.Projection"), HC7);
		EffectHelper._0012I(base.SasAutoBindTable.Find("Sas.Camera.ProjectionInverse"), HC_0001);
		EffectHelper._0012m(base.SasAutoBindTable.Find("Sas.Camera.WorldTranspose"), HCB);
		EffectHelper._0012m(base.SasAutoBindTable.Find("Sas.Camera.WorldInverseTranspose"), HC_0002);
		EffectHelper._0012m(base.SasAutoBindTable.Find("Sas.Camera.WorldToViewTranspose"), HC_0012);
		EffectHelper._0012m(base.SasAutoBindTable.Find("Sas.Camera.WorldToViewInverseTranspose"), HCH);
		EffectHelper._0012m(base.SasAutoBindTable.Find("Sas.Camera.ProjectionTranspose"), HC7);
		EffectHelper._0012m(base.SasAutoBindTable.Find("Sas.Camera.ProjectionInverseTranspose"), HC_0001);
		List<EffectParameter> list = base.SasAutoBindTable.Find("Sas.Camera.ObjectToView");
		List<EffectParameter> list2 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToViewTranspose");
		List<EffectParameter> list3 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToProjection");
		List<EffectParameter> list4 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToProjectionTranspose");
		if (list != null || list2 != null || list3 != null || list4 != null)
		{
			Matrix matrix = HCB * HC_0012;
			Matrix matrix2 = matrix * HC7;
			EffectHelper._0012I(list, matrix);
			EffectHelper._0012m(list2, matrix);
			EffectHelper._0012I(list3, matrix2);
			EffectHelper._0012m(list4, matrix2);
		}
		List<EffectParameter> list5 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToViewInverse");
		List<EffectParameter> list6 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToViewInverseTranspose");
		List<EffectParameter> list7 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToProjectionInverse");
		List<EffectParameter> list8 = base.SasAutoBindTable.Find("Sas.Camera.ObjectToProjectionInverseTranspose");
		if (list5 != null || list6 != null || list7 != null || list8 != null)
		{
			Matrix matrix3 = HCH * HC_0002;
			Matrix matrix4 = HC_0001 * matrix3;
			EffectHelper._0012I(list5, matrix3);
			EffectHelper._0012m(list6, matrix3);
			EffectHelper._0012I(list7, matrix4);
			EffectHelper._0012m(list8, matrix4);
		}
	}

	/// <summary>
	/// Applies the current bone transform information to the bound effect parameters.
	/// </summary>
	protected virtual void SyncSkinBoneEffectData()
	{
		if (HC6 && HCs != null)
		{
			if (HCV.Length < HCD.Length)
			{
				HCV = new Matrix[HCD.Length];
			}
			for (int i = 0; i < HCD.Length; i++)
			{
				ref Matrix reference = ref HCV[i];
				reference = HCD[i] * HCB;
			}
			Math.Min(HCV.Length, HCs.Elements.Count);
			HCs.SetValue(HCV);
		}
	}

	internal BaseSasEffect(GraphicsDevice P_0, byte[] P_1, bool P_2)
		: base(P_0, P_1)
	{
		HCs = FindBySasAddress("Sas.Skeleton.MeshToJointToWorld[*]");
		B(P_2);
	}

	private void B(bool P_0)
	{
		HC_0013 = FindBySemantic("LIGHTMAPCOLORTEXTURE");
		HCX = FindBySemantic("LIGHTMAPDIRECTIONALTEXTURE");
		HCz = FindBySemantic("COMPOSITELIGHTINGAMBIENT");
		HCA = FindBySemantic("COMPOSITELIGHTINGDIFFUSE");
		HCc = FindBySemantic("COMPOSITELIGHTINGDIRECTION");
		Texture2D texture2D = SunBurnCoreSystem.Instance._0002l("Black");
		HCk = new LightMap(texture2D, texture2D);
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
