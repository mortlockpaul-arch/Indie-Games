using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Effects;

public abstract class FX
{
	protected Vector2 Position;

	public static void LoadContent(ContentManager content)
	{
		FXFogPoof.Load(content);
		FXFreeze.Load(content);
		FXHit.Load(content);
		FXIce.Load(content);
		FXKill.Load(content);
		FXOrbGore.Load(content);
		FXOrbNova.Load(content);
		FXPoison.Load(content);
		FXPoof.Load(content);
		FXResurrect.Load(content);
		FXDustCloud.Load(content);
		FXText.Load(content);
		FXThorns.Load(content);
	}

	public abstract void Update(GameTime gameTime);

	public abstract void Draw(SpriteBatch spriteBatch);
}
