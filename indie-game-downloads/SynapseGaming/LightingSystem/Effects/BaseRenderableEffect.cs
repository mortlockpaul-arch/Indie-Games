using System;
using _0003;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Provides basic rendering support.
/// </summary>
public abstract class BaseRenderableEffect : Effect, IRenderableEffect
{
	internal class DA_0018
	{
		internal SystemStatistic _3A_0018 = SystemConsole.GetStatistic("Effect_TechniqueChanges", SystemStatisticCategory.Rendering);

		internal SystemStatistic _3AL = SystemConsole.GetStatistic("Effect_MatrixParameterChanges", SystemStatisticCategory.Rendering);

		internal SystemStatistic _3A_0019 = SystemConsole.GetStatistic("Effect_LightSourceChanges", SystemStatisticCategory.Rendering);
	}

	private bool _3A_0018;

	private DetailPreference _3AL;

	private Matrix _3A_0019;

	private Matrix _3A3;

	private Matrix _3A6;

	private Matrix _3AD;

	private Matrix _3A_0017;

	private Matrix _3A_0003;

	private Matrix _3Al;

	private Matrix _3At;

	private Matrix _3AF;

	private float _3Ac;

	private EffectParameter _3Ag;

	private EffectParameter _3AI;

	private EffectParameter _3A8;

	private EffectParameter _3AZ;

	private EffectParameter _3Ax;

	private EffectParameter _3Aq;

	private EffectParameter _3Ab;

	private EffectParameter _3AT;

	private EffectParameter _3Ay;

	private EffectParameter _3A_0015;

	private EffectParameter _3A_0001;

	internal DA_0018 _3A7 = new DA_0018();

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
			return _3A_0019;
		}
		set
		{
			if (!_0019T(ref value))
			{
				EffectHelper._00199(value, ref _3A_0019, ref _3A3, ref _3Ag, ref _3AI);
				SetWorldViewProjection(viewprojectionchanged: false, setslowwindingdirection: true);
			}
		}
	}

	/// <summary>
	/// Inverse world matrix applied to geometry using this effect.
	/// </summary>
	public Matrix WorldToObject => _3A3;

	/// <summary>
	/// View matrix applied to geometry using this effect.
	/// </summary>
	public Matrix View
	{
		get
		{
			return _3A6;
		}
		set
		{
			EffectHelper._00199(value, ref _3A6, ref _3AD, ref _3A8, ref _3AZ);
			SetWorldViewProjection(viewprojectionchanged: true, setslowwindingdirection: true);
		}
	}

	/// <summary>
	/// Inverse view matrix applied to geometry using this effect.
	/// </summary>
	public Matrix ViewToWorld => _3AD;

	/// <summary>
	/// Projection matrix applied to geometry using this effect.
	/// </summary>
	public Matrix Projection
	{
		get
		{
			return _3A_0017;
		}
		set
		{
			if (value != _3A_0017)
			{
				_3A_0017 = value;
				if (_3Ax != null)
				{
					_3Ax.SetValue(_3A_0017);
				}
				if (_3Aq != null || _3A_0001 != null)
				{
					_3A_0003 = Matrix.Invert(_3A_0017);
					if (_3Aq != null)
					{
						_3Aq.SetValue(_3A_0003);
					}
				}
			}
			SetWorldViewProjection(viewprojectionchanged: true, setslowwindingdirection: true);
		}
	}

	/// <summary>
	/// Inverse projection matrix applied to geometry using this effect.
	/// </summary>
	public Matrix ProjectionToView => _3A_0003;

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
			return _3AL;
		}
		set
		{
			if (value != _3AL)
			{
				_3AL = value;
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

	private float _0019b()
	{
		if (_3AF.Determinant() < 0f)
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
		if (_3Ay == null && _3AT == null && _3Ab == null && _3A_0015 == null && _3A_0001 == null)
		{
			return;
		}
		if (_3AT != null)
		{
			Matrix.Multiply(ref _3A_0019, ref _3A6, out var result);
			if (!result.Equals(_3At))
			{
				_3At = result;
				_3AT.SetValue(_3At);
				_UpdatedByBatch = true;
				_3A7._3AL.AccumulationValue++;
			}
		}
		if (_3Ab != null || _3Ay != null || _3A_0015 != null)
		{
			if (viewprojectionchanged)
			{
				Matrix.Multiply(ref _3A6, ref _3A_0017, out _3Al);
				if (_3Ab != null)
				{
					_3Ab.SetValue(_3Al);
					_UpdatedByBatch = true;
					_3A7._3AL.AccumulationValue++;
				}
			}
			if (_3Ay != null || _3A_0015 != null)
			{
				Matrix.Multiply(ref _3A_0019, ref _3Al, out var result2);
				if (!result2.Equals(_3AF))
				{
					_3AF = result2;
					if (_3Ay != null)
					{
						_3Ay.SetValue(_3AF);
						_UpdatedByBatch = true;
						_3A7._3AL.AccumulationValue++;
					}
					if (setslowwindingdirection && _3A_0015 != null)
					{
						_3A_0015.SetValue(_0019b());
						_UpdatedByBatch = true;
						_3A7._3AL.AccumulationValue++;
					}
				}
			}
		}
		if (viewprojectionchanged && _3A_0001 != null)
		{
			Vector4 vector = Vector4.Transform(new Vector4(0f, 0f, 1f, 1f), ProjectionToView);
			float num = 0f;
			if (vector.W != 0f)
			{
				num = Math.Abs(vector.Z / vector.W);
			}
			if (_3Ac != num)
			{
				_3Ac = num;
				_3A_0001.SetValue(num);
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
		if (view != _3A6)
		{
			_3A6 = view;
			if (_3A8 != null)
			{
				_3A8.SetValue(_3A6);
				_3A7._3AL.AccumulationValue++;
			}
			if (_3AZ != null)
			{
				_3AD = viewtoworld;
				_3AZ.SetValue(_3AD);
				_3A7._3AL.AccumulationValue++;
			}
			flag = true;
		}
		if (projection != _3A_0017)
		{
			_3A_0017 = projection;
			if (_3Ax != null)
			{
				_3Ax.SetValue(_3A_0017);
				_3A7._3AL.AccumulationValue++;
			}
			if (_3Aq != null || _3A_0001 != null)
			{
				_3A_0003 = projectiontoview;
				if (_3Aq != null)
				{
					_3Aq.SetValue(_3A_0003);
					_3A7._3AL.AccumulationValue++;
				}
			}
			flag = true;
		}
		if (flag)
		{
			SetWorldViewProjection(viewprojectionchanged: true, setslowwindingdirection: true);
		}
	}

	private bool _0019T(ref Matrix P_0)
	{
		if (_3A_0018)
		{
			_3A_0019 = Matrix.Identity;
			_3A_0018 = false;
			return false;
		}
		return P_0.Equals(_3A_0019);
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
		if (!_0019T(ref world))
		{
			_UpdatedByBatch = true;
			_3A_0019 = world;
			if (_3Ag != null)
			{
				_3Ag.SetValue(_3A_0019);
				_3A7._3AL.AccumulationValue++;
			}
			if (_3AI != null)
			{
				_3A3 = worldtoobj;
				_3AI.SetValue(_3A3);
				_3A7._3AL.AccumulationValue++;
			}
			SetWorldViewProjection(viewprojectionchanged: false, setslowwindingdirection: true);
		}
	}

	internal BaseRenderableEffect(GraphicsDevice P_0, string P_1)
		: base(P_0, SunBurnCoreSystem.Instance.LM(P_1).ByteCode)
	{
		_3Ag = base.Parameters["_World"];
		_3AI = base.Parameters["_WorldToObject"];
		_3A8 = base.Parameters["_View"];
		_3AZ = base.Parameters["_ViewToWorld"];
		_3Ax = base.Parameters["_Projection"];
		_3Aq = base.Parameters["_ProjectionToView"];
		_3Ab = base.Parameters["_ViewProjection"];
		_3AT = base.Parameters["_WorldView"];
		_3Ay = base.Parameters["_WorldViewProjection"];
		_3A_0015 = base.Parameters["_WindingDirection"];
		_3A_0001 = base.Parameters["_FarClippingDistance"];
	}

	/// <summary>
	/// Creates a new effect of the same class type, with the same property values, and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	public override Effect Clone()
	{
		Effect effect = Create();
		_0003._6.L_0003(this, effect);
		return effect;
	}

	/// <summary>
	/// Creates a new empty effect of the same class type and using the same effect file as this object.
	/// </summary>
	/// <returns></returns>
	protected abstract Effect Create();
}
