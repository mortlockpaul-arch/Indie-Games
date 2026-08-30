using System;
using Microsoft.Xna.Framework;

namespace Maximinus;

public class ParticleEmitter2D
{
	private TailTextureParticleSystem2D particleSystem;

	private float timeBetweenParticles;

	private Vector2 previousPosition;

	private float timeLeftOver;

	public ParticleEmitter2D(TailTextureParticleSystem2D particleSystem, float particlesPerSecond, Vector2 initialPosition)
	{
		this.particleSystem = particleSystem;
		timeBetweenParticles = 1f / particlesPerSecond;
		previousPosition = initialPosition;
	}

	public void AddParticles(GameTime gameTime, Vector2 newPosition, Vector2 initialVelo)
	{
		if (gameTime == null)
		{
			throw new ArgumentNullException("gameTime");
		}
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (num > 0f)
		{
			_ = (newPosition - previousPosition) / num;
			float num2 = timeLeftOver + num;
			float num3 = 0f - timeLeftOver;
			while (num2 > timeBetweenParticles)
			{
				num3 += timeBetweenParticles;
				num2 -= timeBetweenParticles;
				float amount = num3 / num;
				Vector2.Lerp(previousPosition, newPosition, amount);
			}
			timeLeftOver = num2;
		}
		previousPosition = newPosition;
	}
}
