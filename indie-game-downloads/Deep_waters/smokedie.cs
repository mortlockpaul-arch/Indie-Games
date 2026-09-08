using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Deep_waters;

public class smokedie : ParticleSystem
{
	public smokedie(GraphicsDevice gdev, Texture2D tex, Effect eff, Vector3 position)
		: base(gdev, tex, eff, position)
	{
	}

	protected override void InitializeSettings(ParticleSettings settings)
	{
		settings.TextureName = "smoke2";
		settings.MaxParticles = 20;
		settings.totalparticles = 20;
		settings.Duration = TimeSpan.FromSeconds(0.30000001192092896);
		settings.MinHorizontalVelocity = -2f;
		settings.MaxHorizontalVelocity = 2f;
		settings.MinVerticalVelocity = 2f;
		settings.MaxVerticalVelocity = 3f;
		settings.Gravity = new Vector3(0f, 0f, 0f);
		settings.EndVelocity = 0f;
		settings.MinColor = Color.DarkGray;
		settings.MaxColor = Color.LightGray;
		settings.MinRotateSpeed = -1f;
		settings.MaxRotateSpeed = 1f;
		settings.MinStartSize = 0.5f;
		settings.MaxStartSize = 5f;
		settings.MinEndSize = 6f;
		settings.MaxEndSize = 9f;
	}
}
