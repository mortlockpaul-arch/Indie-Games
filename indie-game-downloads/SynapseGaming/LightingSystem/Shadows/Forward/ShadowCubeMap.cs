using System;
using Microsoft.Xna.Framework.Graphics;
using Z;

namespace SynapseGaming.LightingSystem.Shadows.Forward;

/// <summary>
/// Shadow map class that implements cube-mapped shadows with
/// per surface level-of-detail. Used for point based lights.
/// </summary>
public class ShadowCubeMap : BaseShadowCubeMap
{
	/// <summary>
	/// Gets the effect type that performs rendering specific to the shadow
	/// mapping implementation used by this object.
	/// </summary>
	/// <returns></returns>
	protected override Type GetEffectType()
	{
		return typeof(Z._0018);
	}

	/// <summary>
	/// Creates a new effect that performs rendering specific to the shadow
	/// mapping implementation used by this object.
	/// </summary>
	/// <returns></returns>
	protected override Effect CreateEffect()
	{
		return new Z._0018(base.Device);
	}
}
