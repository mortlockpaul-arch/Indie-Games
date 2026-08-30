using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TabeBallMk4;

public class Game1 : Game
{
	private GraphicsDeviceManager graphics;

	private SpriteBatch spriteBatch;

	private Texture2D[] idleHan;

	private bool loadScrean;

	private bool calledLoad;

	private bool isGameLoaded;

	private int loadFrame;

	private int totalLoadFrames;

	private SoundEffect idleHanSig;

	private Menus gameMenus;

	public Game1()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		idleHan = (Texture2D[])(object)new Texture2D[26];
		loadScrean = true;
		calledLoad = false;
		isGameLoaded = false;
		loadFrame = 0;
		totalLoadFrames = 480;
		((Game)this)._002Ector();
		graphics = new GraphicsDeviceManager((Game)(object)this);
		((Game)this).Content.RootDirectory = "Content";
	}

	protected override void Initialize()
	{
		gameMenus = new Menus((Game)(object)this);
		((Game)this).Initialize();
	}

	protected override void LoadContent()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Expected O, but got Unknown
		spriteBatch = new SpriteBatch(((Game)this).GraphicsDevice);
		idleHanSig = ((Game)this).Content.Load<SoundEffect>("sounds\\idleHanSig");
		for (int i = 1; i < 27; i++)
		{
			if (i < 10)
			{
				idleHan[i - 1] = ((Game)this).Content.Load<Texture2D>("idleHan\\000" + i);
			}
			else
			{
				idleHan[i - 1] = ((Game)this).Content.Load<Texture2D>("idleHan\\00" + i);
			}
		}
	}

	protected override void UnloadContent()
	{
	}

	protected override void Update(GameTime gameTime)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Invalid comparison between Unknown and I4
		GamePadState state = GamePad.GetState((PlayerIndex)0);
		GamePadButtons buttons = ((GamePadState)(ref state)).Buttons;
		if ((int)((GamePadButtons)(ref buttons)).Back == 1)
		{
			((Game)this).Exit();
		}
		if (!calledLoad)
		{
			calledLoad = true;
			isGameLoaded = gameMenus.LoadGameContent();
		}
		if (isGameLoaded && !loadScrean)
		{
			((GameComponent)gameMenus).Update(gameTime);
		}
		((Game)this).Update(gameTime);
	}

	protected override void Draw(GameTime gameTime)
	{
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		if (loadScrean)
		{
			((Game)this).GraphicsDevice.Clear(Color.Black);
			loadFrame++;
			if (loadFrame > totalLoadFrames)
			{
				loadScrean = false;
			}
			Viewport viewport = ((Game)this).GraphicsDevice.Viewport;
			int num = ((Viewport)(ref viewport)).Height / 2;
			viewport = ((Game)this).GraphicsDevice.Viewport;
			int num2 = (((Viewport)(ref viewport)).Width - num) / 2;
			viewport = ((Game)this).GraphicsDevice.Viewport;
			Rectangle val = default(Rectangle);
			((Rectangle)(ref val))._002Ector(num2, (((Viewport)(ref viewport)).Height - (int)((double)num * 1.2)) / 2, num, num);
			if (loadFrame > 30)
			{
				spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)1);
				if (loadFrame < 100)
				{
					spriteBatch.Draw(idleHan[0], val, Color.White);
				}
				else if (loadFrame < 150)
				{
					spriteBatch.Draw(idleHan[loadFrame / 2 - 50], val, Color.White);
				}
				else if (loadFrame < 190)
				{
					spriteBatch.Draw(idleHan[24], val, Color.White);
				}
				else
				{
					spriteBatch.Draw(idleHan[25], val, Color.White);
				}
				spriteBatch.End();
			}
			if (loadFrame == 10)
			{
				idleHanSig.Play();
			}
		}
		else
		{
			((Game)this).GraphicsDevice.Clear(Color.Pink);
			gameMenus.Draw();
		}
		((Game)this).Draw(gameTime);
	}
}
