using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Entities;

public interface ISprite
{
	void Render(GameTime oGameTime, ref SpriteBatch batch, ref Color globalTint);

	void Dispose();
}
