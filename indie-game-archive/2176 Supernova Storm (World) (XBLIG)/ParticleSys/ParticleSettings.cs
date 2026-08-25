using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleSys;

public class ParticleSettings
{
	public string TextureName;

	public int MaxParticles = 120;

	public TimeSpan Duration = TimeSpan.FromSeconds(1.0);

	public float DurationRandomness;

	public float EmitterVelocitySensitivity = 1f;

	public float MinHorizontalVelocity;

	public float MaxHorizontalVelocity;

	public float MinVerticalVelocity;

	public float MaxVerticalVelocity;

	public Vector3 Gravity = Vector3.Zero;

	public float EndVelocity = 1f;

	public Color MinColor = Color.White;

	public Color MaxColor = Color.White;

	public float MinRotateSpeed;

	public float MaxRotateSpeed;

	public float MinStartSize = 100f;

	public float MaxStartSize = 100f;

	public float MinEndSize = 100f;

	public float MaxEndSize = 100f;

	public Blend SourceBlend = Blend.SourceAlpha;

	public Blend DestinationBlend = Blend.InverseSourceAlpha;
}
