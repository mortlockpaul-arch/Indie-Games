using RuntimeXNA.Banks;

namespace RuntimeXNA.Sprites;

public interface IDrawing
{
	void drawableDraw(SpriteBatchEffect batch, CSprite sprite, CImageBank bank, int x, int y);

	void drawableKill();

	CMask drawableGetMask(int flags);
}
