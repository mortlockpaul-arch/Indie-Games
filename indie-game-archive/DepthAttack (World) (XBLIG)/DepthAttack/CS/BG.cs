using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class BG : DrawableGameComponent
{
	public enum enuBGScene
	{
		Main00,
		Main01,
		Main02,
		Main03,
		Main04,
		Pause
	}

	private const string cstrMainBG01 = "PNG\\BackGround\\BG10";

	private SpriteBatch spritesBatch;

	private Texture2D imgMainBG01;

	private int intTimeCount = 0;

	public enuBGScene penuBGSelect;

	public BG(Game game)
		: base(game)
	{
		penuBGSelect = enuBGScene.Main00;
	}

	public override void Initialize()
	{
		penuBGSelect = enuBGScene.Main00;
		intTimeCount = 0;
		base.Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		switch (penuBGSelect)
		{
		case enuBGScene.Main02:
			intTimeCount = 0;
			penuBGSelect = enuBGScene.Main03;
			break;
		case enuBGScene.Main03:
			intTimeCount++;
			if (intTimeCount > 180)
			{
				penuBGSelect = enuBGScene.Main04;
			}
			break;
		}
		base.Update(gameTime);
	}

	protected override void LoadContent()
	{
		spritesBatch = new SpriteBatch(base.GraphicsDevice);
		imgMainBG01 = base.Game.Content.Load<Texture2D>("PNG\\BackGround\\BG10");
		base.LoadContent();
	}

	public override void Draw(GameTime gameTime)
	{
		spritesBatch.Begin();
		switch (penuBGSelect)
		{
		case enuBGScene.Main03:
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					spritesBatch.Draw(imgMainBG01, new Vector2(i * imgMainBG01.Width, j * -1 * imgMainBG01.Height - intTimeCount * -20 - 3000), null, Color.White, 0f, new Vector2(0f, 0f), new Vector2(1f, 1f), SpriteEffects.None, 1f);
				}
			}
			break;
		}
		case enuBGScene.Main04:
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					spritesBatch.Draw(imgMainBG01, new Vector2(i * imgMainBG01.Width, j * imgMainBG01.Height), null, Color.White, 0f, new Vector2(0f, 0f), new Vector2(1f, 1f), SpriteEffects.None, 1f);
				}
			}
			break;
		}
		}
		spritesBatch.End();
		base.Draw(gameTime);
	}
}
