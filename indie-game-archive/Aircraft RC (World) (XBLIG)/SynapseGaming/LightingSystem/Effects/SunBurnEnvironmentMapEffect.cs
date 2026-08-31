using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;
using Z;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Used to fix issues with XNA 4.0 effects.
/// </summary>
public class SunBurnEnvironmentMapEffect : EnvironmentMapEffect, IExtendedXNAEffect, ICollisionMaterial
{
	private int HCB;

	private float HC_0002;

	private float HC_0012;

	[CompilerGenerated]
	private bool HCH;

	[CompilerGenerated]
	private TransparencyMode HC7;

	/// <summary>
	/// Surfaces rendered with the effect should be visible from both sides.
	/// </summary>
	public bool DoubleSided
	{
		[CompilerGenerated]
		get
		{
			return HCH;
		}
		[CompilerGenerated]
		set
		{
			HCH = value;
		}
	}

	/// <summary>
	/// The transparency style used when rendering the effect.
	/// </summary>
	public TransparencyMode TransparencyMode
	{
		[CompilerGenerated]
		get
		{
			return HC7;
		}
		[CompilerGenerated]
		set
		{
			HC7 = value;
		}
	}

	/// <summary>
	/// Amount material absorbs impact force.
	/// </summary>
	public float Elasticity
	{
		get
		{
			return HC_0002;
		}
		set
		{
			HC_0002 = value;
			HCB++;
		}
	}

	/// <summary>
	/// Amount material resists objects moving across its surface.
	/// </summary>
	public float Friction
	{
		get
		{
			return HC_0012;
		}
		set
		{
			HC_0012 = value;
			HCB++;
		}
	}

	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	public int CollisionId => HCB;

	/// <summary>
	/// Creates a SunBurnEnvironmentMapEffect instance.
	/// </summary>
	public SunBurnEnvironmentMapEffect(GraphicsDevice device)
		: base(device)
	{
	}

	/// <summary>
	/// Creates a clone of the current effect instance.
	/// </summary>
	public override Effect Clone()
	{
		Effect effect = new SunBurnEnvironmentMapEffect(base.GraphicsDevice);
		Z._7._0002w(this, effect);
		return effect;
	}
}
