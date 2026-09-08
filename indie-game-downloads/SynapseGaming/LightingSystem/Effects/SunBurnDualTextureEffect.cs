using System.Runtime.CompilerServices;
using _0003;
using Microsoft.Xna.Framework.Graphics;
using SynapseGaming.LightingSystem.Collision;
using SynapseGaming.LightingSystem.Core;

namespace SynapseGaming.LightingSystem.Effects;

/// <summary>
/// Used to fix issues with XNA 4.0 effects.
/// </summary>
public class SunBurnDualTextureEffect : DualTextureEffect, IExtendedXNAEffect, ICollisionMaterial
{
	private int _3A_0018;

	private float _3AL;

	private float _3A_0019;

	[CompilerGenerated]
	private bool _3A3;

	[CompilerGenerated]
	private TransparencyMode _3A6;

	/// <summary>
	/// Surfaces rendered with the effect should be visible from both sides.
	/// </summary>
	public bool DoubleSided
	{
		[CompilerGenerated]
		get
		{
			return _3A3;
		}
		[CompilerGenerated]
		set
		{
			_3A3 = value;
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
			return _3A6;
		}
		[CompilerGenerated]
		set
		{
			_3A6 = value;
		}
	}

	/// <summary>
	/// Amount material absorbs impact force.
	/// </summary>
	public float Elasticity
	{
		get
		{
			return _3AL;
		}
		set
		{
			_3AL = value;
			_3A_0018++;
		}
	}

	/// <summary>
	/// Amount material resists objects moving across its surface.
	/// </summary>
	public float Friction
	{
		get
		{
			return _3A_0019;
		}
		set
		{
			_3A_0019 = value;
			_3A_0018++;
		}
	}

	/// <summary>
	/// Indicates if collision related properties changed. This value increments each time the object
	/// collision properties change.
	/// </summary>
	public int CollisionId => _3A_0018;

	/// <summary>
	/// Creates a SunBurnDualTextureEffect instance.
	/// </summary>
	public SunBurnDualTextureEffect(GraphicsDevice device)
		: base(device)
	{
	}

	/// <summary>
	/// Creates a clone of the current effect instance.
	/// </summary>
	public override Effect Clone()
	{
		Effect effect = new SunBurnDualTextureEffect(base.GraphicsDevice);
		_0003._6.L_0003(this, effect);
		return effect;
	}
}
