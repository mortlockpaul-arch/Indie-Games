using RuntimeXNA.Sprites;

namespace RuntimeXNA.RunLoop;

public interface IControl
{
	void drawControl(SpriteBatchEffect batch);

	int getX();

	int getY();

	void setFocus(bool bFlag);

	void click(int nClicks);

	void setMouseControlled(bool bFlag);
}
