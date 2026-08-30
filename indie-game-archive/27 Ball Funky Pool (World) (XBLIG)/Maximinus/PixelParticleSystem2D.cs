using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class PixelParticleSystem2D : ParticleSystem2D
{
	private Texture2D blankTex;

	private Color color;

	private float scale;

	public override void DrawManual(GameTime gameTime)
	{
		if (UseBaseDraw)
		{
			SB.Begin(SpriteSortMode.Deferred, blendState);
		}
		Particle2D[] getParticles = base.GetParticles;
		foreach (Particle2D particle2D in getParticles)
		{
			if (particle2D.Active)
			{
				float num = particle2D.TimeSinceStart / particle2D.Lifetime;
				float num2 = 4f * num * (1f - num);
				Color color = this.color * num2;
				Vector2 vector = ParticleSystem2D.PositionWithOffset(particle2D.Position);
				if (Utils.IsVisible(vector, scale, num2, 1, 1, canRotate: false, new Point(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height)))
				{
					SB.Draw(blankTex, vector, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
				}
			}
		}
		if (UseBaseDraw)
		{
			SB.End();
		}
	}

	public PixelParticleSystem2D(Game game, Drawing2D draw2D, Color color, float scale, int howManyEffects)
		: base(game, draw2D.SpriteBatch, null, howManyEffects)
	{
		blankTex = draw2D.BlankTex;
		UseBaseDraw = false;
		this.color = color;
		this.scale = scale;
	}

	protected override void InitializeConstants()
	{
		PixelMode = true;
		minInitialSpeed = 2f;
		maxInitialSpeed = 10f;
		minAcceleration = 0f;
		maxAcceleration = 0f;
		minLifetime = 2f;
		maxLifetime = 3f;
		minScale = 1f;
		maxScale = 1f;
		minNumParticles = 7;
		maxNumParticles = 15;
		minRotationSpeed = -(float)Math.PI / 8f;
		maxRotationSpeed = (float)Math.PI / 8f;
		blendState = BlendState.AlphaBlend;
		base.DrawOrder = 100;
	}

	protected override Vector2 PickRandomDirection()
	{
		float num = Utils.RandomBetween(MathHelper.ToRadians(80f), MathHelper.ToRadians(100f));
		Vector2 zero = Vector2.Zero;
		zero.X = (float)Math.Cos(num);
		zero.Y = 0f - (float)Math.Sin(num);
		return zero;
	}
}
