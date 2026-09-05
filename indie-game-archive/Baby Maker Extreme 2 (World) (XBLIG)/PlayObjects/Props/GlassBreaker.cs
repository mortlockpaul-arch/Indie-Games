using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Renderer;

namespace PlayObjects.Props;

public class GlassBreaker : PropEffector
{
	private PhysicsOutfit m_outfit;

	private List<SpriteImage> m_images;

	public GlassBreaker(PhysicsOutfit outfit)
	{
		m_outfit = outfit;
		m_images = new List<SpriteImage>();
		m_images.Add(TextureContainer.GetImage("images/spriteSheets/glassShards", new Rectangle(62, 13, 10, 78)));
		m_images.Add(TextureContainer.GetImage("images/spriteSheets/glassShards", new Rectangle(109, 19, 10, 131)));
		m_images.Add(TextureContainer.GetImage("images/spriteSheets/glassShards", new Rectangle(62, 101, 10, 91)));
	}

	public override void CollisionResponse(Player p, Vector2 pos)
	{
		List<SpriteInstance> sprites = m_outfit.GetSprites();
		ParticleManager.GetParticle().Initialize(m_images[0], sprites[0].Position - new Vector2(0f, sprites[0].SurfaceScale.Y / 2f - 40f), sprites[0].Depth, 4000, new Vector2(300f, -100f), fadesOut: true, new Color(56, 108, 255, 1), new Color(56, 108, 255, 1), 10f, 10f, additive: false, new Vector2(0f, 240f), SceneRenderer.GetRand(0f, (float)Math.PI), 0.00184f, default(Vector2), isFlat: false);
		for (int i = 0; i < 7; i++)
		{
			ParticleManager.GetParticle().Initialize(m_images[1], sprites[0].Position - new Vector2(0f, sprites[0].SurfaceScale.Y / 2f - 40f - (float)(100 * (i + 1))), sprites[0].Depth, 4000, new Vector2(300f - SceneRenderer.GetRand(-30f, 50f), -100f - SceneRenderer.GetRand(0f, 140f)), fadesOut: true, new Color(56, 108, 255, 1), new Color(56, 108, 255, 1), 10f, 10f, additive: false, new Vector2(0f, 240f), SceneRenderer.GetRand(0f, (float)Math.PI), 0.001f + SceneRenderer.GetRand(0f, 0.006f), default(Vector2), isFlat: false);
		}
		ParticleManager.GetParticle().Initialize(m_images[2], sprites[0].Position - new Vector2(0f, (0f - sprites[0].SurfaceScale.Y) / 2f + 45f), sprites[0].Depth, 4000, new Vector2(300f, -100f), fadesOut: true, new Color(56, 108, 255, 1), new Color(56, 108, 255, 1), 10f, 10f, additive: false, new Vector2(0f, 240f), SceneRenderer.GetRand(0f, (float)Math.PI), 0.001f, default(Vector2), isFlat: false);
		for (int j = 0; j < sprites.Count; j++)
		{
			sprites[j].Alpha = 0f;
		}
	}

	public override void Reset()
	{
		List<SpriteInstance> sprites = m_outfit.GetSprites();
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].Alpha = 1f;
		}
	}
}
