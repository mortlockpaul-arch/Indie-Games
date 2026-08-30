#define DEBUG
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury.Emitters;

namespace ProjectMercury.Renderers;

public sealed class SpriteBatchRenderer : Renderer
{
	private SpriteBatch Batch;

	private BlendState NonPremultipliedAdditive { get; set; }

	protected override void Dispose(bool disposing)
	{
		if (disposing && Batch != null)
		{
			Batch.Dispose();
		}
		base.Dispose(disposing);
	}

	public override void LoadContent(ContentManager content)
	{
		Guard.IsTrue(GraphicsDeviceService == null, "GraphicsDeviceService property has not been initialised with a valid value.");
		if (Batch == null)
		{
			Batch = new SpriteBatch(GraphicsDeviceService.GraphicsDevice);
		}
		if (NonPremultipliedAdditive == null)
		{
			NonPremultipliedAdditive = new BlendState
			{
				AlphaBlendFunction = BlendFunction.Add,
				AlphaDestinationBlend = Blend.One,
				AlphaSourceBlend = Blend.SourceAlpha,
				ColorBlendFunction = BlendFunction.Add,
				ColorDestinationBlend = Blend.One,
				ColorSourceBlend = Blend.SourceAlpha
			};
		}
	}

	public override void RenderEmitter(Emitter emitter, ref Matrix transform)
	{
		Guard.ArgumentNull("emitter", emitter);
		Guard.IsTrue(Batch == null, "SpriteBatchRenderer is not ready! Did you forget to LoadContent?");
		if (emitter.ParticleTexture != null && emitter.ActiveParticlesCount > 0 && emitter.BlendMode != EmitterBlendMode.None)
		{
			Rectangle value = new Rectangle(0, 0, emitter.ParticleTexture.Width, emitter.ParticleTexture.Height);
			Vector2 origin = new Vector2((float)value.Width / 2f, (float)value.Height / 2f);
			BlendState blendState = GetBlendState(emitter.BlendMode);
			Batch.Begin(SpriteSortMode.Deferred, blendState);
			for (int i = 0; i < emitter.ActiveParticlesCount; i++)
			{
				Particle particle = emitter.Particles[i];
				float scale = particle.Scale / (float)emitter.ParticleTexture.Width;
				Batch.Draw(emitter.ParticleTexture, particle.Position, value, new Color(particle.Colour), particle.Rotation, origin, scale, SpriteEffects.None, 0f);
			}
			Batch.End();
		}
	}

	public void RenderEffect(ParticleEffect effect, SpriteBatch spriteBatch)
	{
		Guard.ArgumentNull("effect", effect);
		Guard.ArgumentNull("spriteBatch", spriteBatch);
		for (int i = 0; i < effect.Count; i++)
		{
			RenderEmitter(effect[i], spriteBatch);
		}
	}

	public unsafe void RenderEmitter(Emitter emitter, SpriteBatch spriteBatch)
	{
		Guard.ArgumentNull("emitter", emitter);
		Guard.ArgumentNull("spriteBatch", spriteBatch);
		if (emitter.ParticleTexture == null || emitter.ActiveParticlesCount <= 0)
		{
			return;
		}
		Rectangle value = new Rectangle(0, 0, emitter.ParticleTexture.Width, emitter.ParticleTexture.Height);
		Vector2 origin = new Vector2((float)value.Width / 2f, (float)value.Height / 2f);
		fixed (Particle* particles = emitter.Particles)
		{
			for (int i = 0; i < emitter.ActiveParticlesCount; i++)
			{
				Particle* ptr = particles + i;
				float scale = ptr->Scale / (float)emitter.ParticleTexture.Width;
				spriteBatch.Draw(emitter.ParticleTexture, ptr->Position, value, new Color(ptr->Colour), ptr->Rotation, origin, scale, SpriteEffects.None, 0f);
			}
		}
	}

	private BlendState GetBlendState(EmitterBlendMode emitterBlendMode)
	{
		return emitterBlendMode switch
		{
			EmitterBlendMode.Alpha => BlendState.NonPremultiplied, 
			EmitterBlendMode.Additive => NonPremultipliedAdditive, 
			_ => throw new InvalidEnumArgumentException("emitterBlendMode", (int)emitterBlendMode, typeof(EmitterBlendMode)), 
		};
	}
}
