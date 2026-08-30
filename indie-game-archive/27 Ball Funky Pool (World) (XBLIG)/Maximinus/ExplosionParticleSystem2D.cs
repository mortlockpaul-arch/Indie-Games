using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class ExplosionParticleSystem2D : ParticleSystem2D
{
	public ExplosionParticleSystem2D(Game game, SpriteBatch sb, string texFileName, int howManyEffects, bool dontBeAComponent)
		: base(game, sb, texFileName, howManyEffects, dontUseGameTime: false, dontBeAComponent)
	{
	}

	protected override void InitializeConstants()
	{
		minInitialSpeed = 40f;
		maxInitialSpeed = 500f;
		minAcceleration = 0f;
		maxAcceleration = 0f;
		minLifetime = 0.5f;
		maxLifetime = 1f;
		minScale = 0.3f;
		maxScale = 1f;
		minNumParticles = 20;
		maxNumParticles = 25;
		minRotationSpeed = -(float)Math.PI / 4f;
		maxRotationSpeed = (float)Math.PI / 4f;
		blendState = BlendState.Additive;
		base.DrawOrder = 200;
	}

	protected override void InitializeParticle(Particle2D p, Vector2 where)
	{
		base.InitializeParticle(p, where);
		p.Acceleration = -p.Velocity / p.Lifetime;
	}
}
