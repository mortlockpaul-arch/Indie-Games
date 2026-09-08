using System;
using Microsoft.Xna.Framework;

namespace Deep_waters;

public class ParticleEmitter
{
	private ParticleSystem particleSystem;

	private float timeBetweenParticles;

	private Vector3 previousPosition;

	private float timeLeftOver;

	public ParticleEmitter(ParticleSystem particleSystem, float particlesPerSecond, Vector3 initialPosition)
	{
		this.particleSystem = particleSystem;
		timeBetweenParticles = 1f / particlesPerSecond;
		previousPosition = initialPosition;
	}

	public void Update(GameTime gameTime, Vector3 newPosition)
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
				Vector3.Lerp(previousPosition, newPosition, amount);
				particleSystem.AddParticle();
			}
			timeLeftOver = num2;
		}
		previousPosition = newPosition;
	}
}
