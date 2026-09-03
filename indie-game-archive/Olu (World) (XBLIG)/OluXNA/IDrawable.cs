using Microsoft.Xna.Framework;

namespace OluXNA;

internal interface IDrawable
{
	void Update(GameTime gametime);

	void Draw(GameTime gametime);
}
