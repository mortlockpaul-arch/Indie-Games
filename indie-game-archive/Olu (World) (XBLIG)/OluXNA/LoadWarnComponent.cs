using System;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class LoadWarnComponent : DrawableGameComponent
{
	public IAsyncResult toCheck;

	private StretchTex messageWindow;

	public LoadWarnComponent(Game _game, IAsyncResult _toCheck)
		: base(_game)
	{
		toCheck = _toCheck;
	}

	protected override void LoadContent()
	{
		messageWindow = new StretchTex();
		messageWindow.Initialize(9, 12, 9, 12, "Content\\WindowTex");
		((DrawableGameComponent)this).LoadContent();
	}

	public override void Update(GameTime gameTime)
	{
		if (toCheck.IsCompleted)
		{
			((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Remove((IGameComponent)(object)this);
		}
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		((Effect)BaseGame.Get().flatEffect).Begin();
		((Effect)BaseGame.Get().flatEffect).CurrentTechnique.Passes[0].Begin();
		BaseGame.Get().spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		messageWindow.Draw(new Vector2((float)(BaseGame.WIDTH / 4), (float)(3 * BaseGame.HEIGHT / 8)), new Vector2((float)(3 * BaseGame.WIDTH / 4), (float)(5 * BaseGame.HEIGHT / 8)), Color.LightGreen);
		BaseGame.Get().spriteBatch.End();
		((Effect)BaseGame.Get().flatEffect).CurrentTechnique.Passes[0].End();
		((Effect)BaseGame.Get().flatEffect).End();
		((DrawableGameComponent)this).Draw(gameTime);
	}
}
