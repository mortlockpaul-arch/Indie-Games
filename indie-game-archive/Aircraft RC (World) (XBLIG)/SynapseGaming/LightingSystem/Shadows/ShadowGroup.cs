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
	private ShadowSourceTypeCaster HCB;

	private IShadowSource HC_0002;

	private IShadowMap HC_0012;

	private BoundingSphere HCH;

	private BoundingBox HC7;

	private List<BaseLight> HC_0001 = new List<BaseLight>(128);

	/// <summary>
	/// Shared shadow source used to determine shadow casting information.
	/// </summary>
	public IShadowSource ShadowSource => HC_0002;

	/// <summary>
	/// Shadow object used to store and render shadows.
	/// </summary>
	public IShadowMap Shadow
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
		}
	}

	/// <summary>
	/// Shadow bounding sphere originating at the shadow source center.
	/// </summary>
	public BoundingSphere BoundingSphereCentered
	{
		get
		{
			return HCH;
		}
		internal set
		{
			HCH = hCH;
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
			return HC7;
		}
		internal set
		{
			HC7 = hC;
		}
	}

	/// <summary>
	/// List of lights that share the shadow source.
	/// </summary>
	public List<BaseLight> Lights => HC_0001;

	internal ShadowSourceTypeCaster ShadowSourceTypes => HCB;

	/// <summary>
	/// Builds the shadow group information based on the shadow source.
	/// </summary>
	/// <param name="shadowsource"></param>
	/// <param name="scenestate">Scene state used to render the current view.</param>
	public void Build(IShadowSource shadowsource, ISceneState scenestate)
	{
		if (HC_0001.Count < 1)
		{
			throw new Exception("Cannot build an empty shadow group.");
		}
		HCB = OptimizationSystem.ShadowSourceTypeCasters.Get(shadowsource);
		HC_0002 = shadowsource;
		if (HCB.PointSource != null)
		{
			bool flag = true;
			foreach (BaseLight item in HC_0001)
			{
				if (!flag)
				{
					HC7 = BoundingBox.CreateMerged(HC7, item.WorldBoundingBox);
					continue;
				}
				HC7 = item.WorldBoundingBox;
				flag = false;
			}
			BoundingSphere boundingSphere = BoundingSphere.CreateFromBoundingBox(HC7);
			float radius = Vector3.Distance(boundingSphere.Center, shadowsource.ShadowPosition) + boundingSphere.Radius;
			HCH = new BoundingSphere(shadowsource.ShadowPosition, radius);
		}
		else
		{
			if (HCB.DirectionalSource == null)
			{
				throw new Exception("Unknown light type - only point, spot, and directional lights are supported at this time.");
			}
			float shadowCasterDistance = scenestate.Environment.ShadowCasterDistance;
			Vector3 translation = scenestate.ViewToWorld.Translation;
			Vector3 vector = new Vector3(shadowCasterDistance);
			HC7 = new BoundingBox(translation - vector, translation + vector);
			HCH = new BoundingSphere(translation - HCB.DirectionalSource.Direction * scenestate.Environment.ShadowCasterDistance, shadowCasterDistance * 2f);
		}
	}
}
