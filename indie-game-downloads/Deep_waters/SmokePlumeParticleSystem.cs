using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class SmokePlumeParticleSystem : ParticleSystem
{
	public SmokePlumeParticleSystem(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 position)
		: base(gdev, tex, eff, position)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "smoke";
		settings.MaxParticles = 100;
		settings.totalparticles = -1;
		settings.Duration = TimeSpan.FromSeconds(1.0);
		settings.BlendState = BlendState.AlphaBlend;
		settings.MinHorizontalVelocity = 1f;
		settings.MaxHorizontalVelocity = 1f;
		settings.MinVerticalVelocity = 0f;
		settings.MaxVerticalVelocity = 0f;
		settings.Gravity = new Vector3(0f, 0f, 0f);
		settings.EndVelocity = 1f;
		settings.MinRotateSpeed = 0f;
		settings.MaxRotateSpeed = 0f;
		settings.MinStartSize = 10f;
		settings.MaxStartSize = 10f;
		settings.MinColor = Color.DarkGray;
		settings.MaxColor = Color.LightGray;
		settings.MinEndSize = 20f;
		settings.MaxEndSize = 20f;
	}
}
