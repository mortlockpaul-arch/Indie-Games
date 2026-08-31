using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;
using Z;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Provides basic rendering support.
/// </summary>
public abstract class BaseRenderableEffect : Effect, IRenderableEffect
{
	internal class _0001CB
	{
		internal SystemStatistic HCB = SystemConsole.GetStatistic("Effect_TechniqueChanges", SystemStatisticCategory.Rendering);

		internal SystemStatistic HC_0002 = SystemConsole.GetStatistic("Effect_MatrixParameterChanges", SystemStatisticCategory.Rendering);

		internal SystemStatistic HC_0012 = SystemConsole.GetStatistic("Effect_LightSourceChanges", SystemStatisticCategory.Rendering);
	}

	private bool HCB;

	private DetailPreference HC_0002;

	private Matrix HC_0012;

	private Matrix HCH;

	private Matrix HC7;

	private Matrix HC_0001;

	private Matrix HCw;

	private Matrix HCZ;

	private Matrix HC_000F;

	private Matrix HCy;

	private Matrix HC6;

	private float HCD;

	private EffectParameter HC_0011;

	private EffectParameter HCK;

	private EffectParameter HC_0003;

	private EffectParameter HCk;

	private EffectParameter HCs;

	private EffectParameter HC_0013;

	private EffectParameter HCX;

	private EffectParameter HCz;

	private EffectParameter HCA;

	private EffectParameter HCc;

	private EffectParameter HCY;

	internal _0001CB HCV = new _0001CB();

	/// <summary>
	/// Set value to true when changes to a property cause calls to EffectParameter.SetValue.
	/// This tells the renderer to commit changes made during Effect Begin/End.
	/// </summary>
	protected bool _UpdatedByBatch;

	/// <summary>
	/// World matrix applied to geometry using this effect.
	/// </summary>
	public Matrix World
	{
		get
		{
			return HC_0012;
		}
		set
		{
			if (!_0012A(ref value))
			{
				EffectHelper._0012_0016(value, ref HC_0012, ref HCH, ref HC_0011, ref HCK);
				SetWorldViewProjection(viewprojectionchanged: false, setslowwindingdirection: true);
			}
		}
	}

	/// <summary>
	/// Inverse world matrix applied to geometry using this effect.
	/// </summary>
	public Matrix WorldToObject => HCH;

	/// <summary>
	/// View matrix applied to geometry using this effect.
	/// </summary>
	public Matrix View
	{
		get
		{
			return HC7;
		}
		set
		{
			EffectHelper._0012_0016(value, ref HC7, ref HC_0001, ref HC_0003, ref HCk);
			SetWorldViewProjection(viewprojectionchanged: true, setslowwindingdirection: true);
		}
	}

	/// <summary>
	/// Inverse view matrix applied to geometry using this effect.
	/// </summary>
	public Matrix ViewToWorld => HC_0001;

	/// <summary>
	/// Projection matrix applied to geometry using this effect.
	/// </summary>
	public Matrix Projection
	{
		get
		{
			return HCw;
		}
		set
		{
			if (value != HCw)
			{
				HCw = value;
				if (HCs != null)
				{
					HCs.SetValue(HCw);
				}
				if (HC_0013 != null || HCY != null)
				{
					HCZ = Matrix.Invert(HCw);
					if (HC_0013 != null)
					{
						HC_0013.SetValue(HCZ);
					}
				}
			}
			SetWorldViewProjection(viewprojectionchanged: true, setslowwindingdirection: true);
		}
	}

	/// <summary>
	/// Inverse projection matrix applied to geometry using this effect.
	/// </summary>
	public Matrix ProjectionToView => HCZ;

	/// <summary>
	/// Surfaces rendered with the effect should be visible from both sides.
	/// </summary>
	public abstract bool DoubleSided { get; set; }

	/// <summary>
	/// Applies the user's effect preference. This generally trades detail
	/// for performance based on the user's selection.
	/// </summary>
	public DetailPreference EffectDetail
	{
		get
		{
			return HC_0002;
		}
		set
		{
			if (value != HC_0002)
			{
				HC_0002 = value;
				SetTechnique();
			}
		}
	}

	/// <summary>
	/// Determines if the renderer should call Apply within an effect Begin/End due
	/// to internal calls to EffectParameter.SetValue. The renderer should set this value
	/// to false after calling Apply.
	/// </summary>
	public bool UpdatedByBatch
	{
		get
		{
			return _UpdatedByBatch;
		}
		set
		{
			_UpdatedByBatch = false;
		}
	}

	private float _0012z()
	{
		if (HC6.Determinant() < 0f)
		{
			return 1f;
		}
		return -1f;
	}

	/// <summary>
	/// Sets the effect technique based on its current property values.
	/// </summary>
	protected abstract void SetTechnique();

	/// <summary>
	/// Recalculates the combination view-projection and world-view-projection matrix
	/// based on the individual world, view, and projection.
	/// </summary>
	protected virtual void SetWorldViewProjection(bool viewprojectionchanged, bool setslowwindingdirection)
	{
		if (HCA == null && HCz == null && HCX == null && HCc == null && HCY == null)
		{
			return;
		}
		if (HCz != null)
		{
			Matrix.Multiply(ref HC_0012, ref HC7, out var result);
			if (!result.Equals(HCy))
			{
				HCy = result;
				HCz.SetValue(HCy);
				_UpdatedByBatch = true;
				HCV.HC_0002.AccumulationValue++;
			}
		}
		if (HCX != null || HCA != null || HCc != null)
		{
			if (viewprojectionchanged)
			{
				Matrix.Multiply(ref HC7, ref HCw, out HC_000F);
				if (HCX != null)
				{
					HCX.SetValue(HC_000F);
					_UpdatedByBatch = true;
					HCV.HC_0002.AccumulationValue++;
				}
			}
			if (HCA != null || HCc != null)
			{
				Matrix.Multiply(ref HC_0012, ref HC_000F, out var result2);
				if (!result2.Equals(HC6))
				{
					HC6 = result2;
					if (HCA != null)
					{
						HCA.SetValue(HC6);
						_UpdatedByBatch = true;
						HCV.HC_0002.AccumulationValue++;
					}
					if (setslowwindingdirection && HCc != null)
					{
						HCc.SetValue(_0012z());
						_UpdatedByBatch = true;
						HCV.HC_0002.AccumulationValue++;
					}
				}
			}
		}
		if (viewprojectionchanged && HCY != null)
		{
			Vector4 vector = Vector4.Transform(new Vector4(0f, 0f, 1f, 1f), ProjectionToView);
			float num = 0f;
			if (vector.W != 0f)
			{
				num = Math.Abs(vector.Z / vector.W);
			}
			if (HCD != num)
			{
				HCD = num;
				HCY.SetValue(num);
				_UpdatedByBatch = true;
			}
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
		if (view != HC7)
		{
			HC7 = view;
			if (HC_0003 != null)
			{
				HC_0003.SetValue(HC7);
				HCV.HC_0002.AccumulationValue++;
			}
			if (HCk != null)
			{
				HC_0001 = viewtoworld;
				HCk.SetValue(HC_0001);
				HCV.HC_0002.AccumulationValue++;
			}
			flag = true;
		}
		if (projection != HCw)
		{
			HCw = projection;
			if (HCs != null)
			{
				HCs.SetValue(HCw);
				HCV.HC_0002.AccumulationValue++;
			}
			if (HC_0013 != null || HCY != null)
			{
				HCZ = projectiontoview;
				if (HC_0013 != null)
				{
					HC_0013.SetValue(HCZ);
					HCV.HC_0002.AccumulationValue++;
				}
			}
			flag = true;
		}
		if (flag)
		{
			SetWorldViewProjection(viewprojectionchanged: true, setslowwindingdirection: true);
		}
	}

	private bool _0012A(ref Matrix P_0)
	{
		if (HCB)
		{
			HC_0012 = Matrix.Identity;
			HCB = false;
			return false;
		}
		return P_0.Equals(HC_0012);
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
		if (!_0012A(ref world))
		{
			_UpdatedByBatch = true;
			HC_0012 = world;
			if (HC_0011 != null)
			{
				HC_0011.SetValue(HC_0012);
				HCV.HC_0002.AccumulationValue++;
			}
			if (HCK != null)
			{
				HCH = worldtoobj;
				HCK.SetValue(HCH);
				HCV.HC_0002.AccumulationValue++;
			}
			SetWorldViewProjection(viewprojectionchanged: false, setslowwindingdirection: true);
		}
	}

	internal BaseRenderableEffect(GraphicsDevice P_0, string P_1)
		: base(P_0, SunBurnCoreSystem.Instance._00021(P_1).ByteCode)
	{
		HC_0011 = base.Parameters["_World"];
		HCK = base.Parameters["_WorldToObject"];
		HC_0003 = base.Parameters["_View"];
		HCk = base.Parameters["_ViewToWorld"];
		HCs = base.Parameters["_Projection"];
		HC_0013 = base.Parameters["_ProjectionToView"];
		HCX = base.Parameters["_ViewProjection"];
		HCz = base.Parameters["_WorldView"];
		HCA = base.Parameters["_WorldViewProjection"];
		HCc = base.Parameters["_WindingDirection"];
		HCY = base.Parameters["_FarClippingDistance"];
	}

	/// <summary>
	/// Creates a new effect of the same class type, with the same property values, and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	public override Effect Clone()
	{
		Effect effect = Create();
		Z._7._0002w(this, effect);
		return effect;
	}

	/// <summary>
	/// Creates a new empty effect of the same class type and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	protected abstract Effect Create();
}
