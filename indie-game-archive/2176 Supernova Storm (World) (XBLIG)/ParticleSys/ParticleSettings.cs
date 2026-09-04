using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ParticleSys;

public class ParticleSettings
{
	public string TextureName;

	public int MaxParticles;

	public TimeSpan Duration;

	public float DurationRandomness;

	public float EmitterVelocitySensitivity;

	public float MinHorizontalVelocity;

	public float MaxHorizontalVelocity;

	public float MinVerticalVelocity;

	public float MaxVerticalVelocity;

	public Vector3 Gravity;

	public float EndVelocity;

	public Color MinColor;

	public Color MaxColor;

	public float MinRotateSpeed;

	public float MaxRotateSpeed;

	public float MinStartSize;

	public float MaxStartSize;

	public float MinEndSize;

	public float MaxEndSize;

	public Blend SourceBlend;

	public Blend DestinationBlend;

	public ParticleSettings()
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		MaxParticles = 120;
		Duration = TimeSpan.FromSeconds(1.0);
		EmitterVelocitySensitivity = 1f;
		Gravity = Vector3.Zero;
		EndVelocity = 1f;
		MinColor = Color.White;
		MaxColor = Color.White;
		MinStartSize = 100f;
		MaxStartSize = 100f;
		MinEndSize = 100f;
		MaxEndSize = 100f;
		SourceBlend = (Blend)5;
		DestinationBlend = (Blend)6;
		base._002Ector();
	}
}
