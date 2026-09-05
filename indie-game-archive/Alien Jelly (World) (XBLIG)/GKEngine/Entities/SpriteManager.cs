using System.Collections.Generic;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Entities;

public class SpriteManager : Entity2D
{
	protected List<ISprite> sprites = new List<ISprite>();

	protected int spritesCount;

	public EntityStack renderStack;

	public SpriteBatch batch;

	public BlendState blendState = BlendState.AlphaBlend;

	public SpriteSortMode sortMode = SpriteSortMode.Immediate;

	public Effect effect;

	public bool useEffect;

	public EffectTechnique technique;

	public SpriteManager(Scene oScene, EntityStack oRenderStack)
	{
		scene = oScene;
		renderStack = oRenderStack;
		Load();
	}

	public SpriteManager(Scene oScene, EntityStack oRenderStack, int xDepth)
	{
		scene = oScene;
		renderStack = oRenderStack;
		depth = xDepth;
		Load();
	}

	public void Set(Sprite oSprite)
	{
		oSprite.manager = this;
		oSprite.scene = scene;
		Add(oSprite);
	}

	public void Add(ISprite oSprite)
	{
		if (!sprites.Contains(oSprite))
		{
			sprites.Add(oSprite);
		}
		spritesCount = sprites.Count;
	}

	public void Remove(ISprite oSprite)
	{
		sprites.Remove(oSprite);
		spritesCount = sprites.Count;
	}

	public bool Contains(ISprite oSprite)
	{
		return sprites.Contains(oSprite);
	}

	public override void Load()
	{
		batch = GameEngine.instance.renderer.spriteBatch;
		renderStack.Add(guid.value, this);
	}

	public override void Dispose()
	{
		base.Dispose();
		renderStack.Remove(guid.value);
		for (int i = 0; i < sprites.Count; i++)
		{
			sprites[i].Dispose();
		}
		sprites.Clear();
		spritesCount = 0;
		if (effect != null)
		{
			effect.Dispose();
		}
	}

	public void SetEffect(Effect oEffect, string xDefaultPass)
	{
		effect = oEffect;
		technique = oEffect.CurrentTechnique;
		useEffect = true;
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible)
		{
			RenderSprites(oGameTime);
		}
	}

	protected void RenderSprites(GameTime oGameTime)
	{
		batch.Begin(sortMode, blendState, null, null, null, effect);
		for (int i = 0; i < spritesCount; i++)
		{
			sprites[i].Render(oGameTime, ref batch, ref tint);
		}
		batch.End();
	}
}
