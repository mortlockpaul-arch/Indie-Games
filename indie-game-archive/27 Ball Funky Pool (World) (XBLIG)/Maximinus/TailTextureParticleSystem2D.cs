using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class TailTextureParticleSystem2D : ParticleSystem2D
{
	public TailTextureParticleSystem2D(Game game, SpriteBatch sb, string texFileName, int howManyEffects, bool dontUseGameTime)
		: base(game, sb, texFileName, howManyEffects, dontUseGameTime, dontBeAComponent: true)
	{
		UseBaseDraw = false;
	}

	protected override void InitializeConstants()
	{
		minInitialSpeed = 20f;
		maxInitialSpeed = 100f;
		minAcceleration = 0f;
		maxAcceleration = 0f;
		minLifetime = 5f;
		maxLifetime = 7f;
		minScale = 0.5f;
		maxScale = 1f;
		minNumParticles = 7;
		maxNumParticles = 15;
		minRotationSpeed = -(float)Math.PI / 8f;
		maxRotationSpeed = (float)Math.PI / 8f;
		blendState = BlendState.AlphaBlend;
		base.DrawOrder = 100;
	}

	public void AddParticles_WithVelocity(float thrustRatio, Vector3[] positions, Vector2 velo)
	{
		int num = (int)((float)Utils.Random.Next(minNumParticles, maxNumParticles) * thrustRatio);
		_ = Vector2.Zero;
		for (int i = 0; i < num; i++)
		{
			if (!base.AnyFreeParticle)
			{
				break;
			}
			Particle2D particle2D = DequeueFreePart();
			float amount = (float)i / (float)num;
			Vector3 vector = Utils.LerpVector3(positions[2], positions[3], amount);
			InitializeParticle(particle2D, new Vector2(vector.X, vector.Y));
			particle2D.Velocity += velo;
			_ = particle2D.Position;
		}
	}
}
