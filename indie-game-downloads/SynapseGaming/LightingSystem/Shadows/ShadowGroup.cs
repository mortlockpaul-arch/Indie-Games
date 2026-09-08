using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;

namespace SynapseGaming.LightingSystem.Shadows;

/// <summary>
/// Defines a group of lights that share the same shadow source.
/// </summary>
public class ShadowGroup
{
	private ShadowSourceTypeCaster _3A_0018;

	private IShadowSource _3AL;

	private IShadowMap _3A_0019;

	private BoundingSphere _3A3;

	private BoundingBox _3A6;

	private List<BaseLight> _3AD = new List<BaseLight>(128);

	/// <summary>
	/// Shared shadow source used to determine shadow casting information.
	/// </summary>
	public IShadowSource ShadowSource => _3AL;

	/// <summary>
	/// Shadow object used to store and render shadows.
	/// </summary>
	public IShadowMap Shadow
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
		}
	}

	/// <summary>
	/// Shadow bounding sphere originating at the shadow source center.
	/// </summary>
	public BoundingSphere BoundingSphereCentered
	{
		get
		{
			return _3A3;
		}
		internal set
		{
			_3A3 = boundingSphere;
		}
	}

	/// <summary>
	/// Shadow bounding box fitted to the shadow region. For some light types like
	/// spotlights this is not necessarily centered around the shadow source.  For
	/// others like directional lights this is only the shadow bounding area and does
	/// not relate to the illuminated area.
	/// </summary>
	public BoundingBox BoundingBox
	{
		get
		{
			return _3A6;
		}
		internal set
		{
			_3A6 = boundingBox;
		}
	}

	/// <summary>
	/// List of lights that share the shadow source.
	/// </summary>
	public List<BaseLight> Lights => _3AD;

	internal ShadowSourceTypeCaster ShadowSourceTypes => _3A_0018;

	/// <summary>
	/// Builds the shadow group information based on the shadow source.
	/// </summary>
	/// <param name="shadowsource"></param>
	/// <param name="scenestate">Scene state used to render the current view.</param>
	public void Build(IShadowSource shadowsource, ISceneState scenestate)
	{
		if (_3AD.Count < 1)
		{
			throw new Exception("Cannot build an empty shadow group.");
		}
		_3A_0018 = OptimizationSystem.ShadowSourceTypeCasters.Get(shadowsource);
		_3AL = shadowsource;
		if (_3A_0018.PointSource != null)
		{
			bool flag = true;
			foreach (BaseLight item in _3AD)
			{
				if (!flag)
				{
					_3A6 = BoundingBox.CreateMerged(_3A6, item.WorldBoundingBox);
					continue;
				}
				_3A6 = item.WorldBoundingBox;
				flag = false;
			}
			BoundingSphere boundingSphere = BoundingSphere.CreateFromBoundingBox(_3A6);
			float radius = Vector3.Distance(boundingSphere.Center, shadowsource.ShadowPosition) + boundingSphere.Radius;
			_3A3 = new BoundingSphere(shadowsource.ShadowPosition, radius);
		}
		else
		{
			if (_3A_0018.DirectionalSource == null)
			{
				throw new Exception("Unknown light type - only point, spot, and directional lights are supported at this time.");
			}
			float shadowCasterDistance = scenestate.Environment.ShadowCasterDistance;
			Vector3 translation = scenestate.ViewToWorld.Translation;
			Vector3 vector = new Vector3(shadowCasterDistance);
			_3A6 = new BoundingBox(translation - vector, translation + vector);
			_3A3 = new BoundingSphere(translation - _3A_0018.DirectionalSource.Direction * scenestate.Environment.ShadowCasterDistance, shadowCasterDistance * 2f);
		}
	}
}
