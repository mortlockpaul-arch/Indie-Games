using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OluXNA;

public class PauseComponent : DrawableGameComponent
{
	private Texture2D grayFilter;

	private StretchTex pauseWindow;

	private int tempZone;

	private Menu pauseMenu;

	private Menu exitMenu;

	private bool confirmExit;

	private ParticleSystem ps;

	private GamePadState enterState;

	private bool enterLeft;

	private bool enterRight;

	private bool tentativeExit;

	private bool tentativeResume;

	private Vector3 particleSource;

	private float partDelay;

	private float curPart;

	private int numPart;

	private MenuItem currentMenuItem
	{
		get
		{
			if (!confirmExit)
			{
				return pauseMenu.ActiveItem;
			}
			return exitMenu.ActiveItem;
		}
	}

	private float genTime => partDelay / (float)numPart;

	public PauseComponent(Game game, GamePadState curState, bool l, bool r)
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		partDelay = 0.01f;
		curPart = 0.01f;
		numPart = 5;
		((DrawableGameComponent)this)._002Ector(game);
		tempZone = BaseGame.Get().level.activeZone;
		confirmExit = false;
		enterState = curState;
		enterLeft = l;
		enterRight = r;
		tentativeExit = false;
		tentativeResume = false;
	}

	public override void Initialize()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		grayFilter = new Texture2D(BaseGame.Get().graphics.GraphicsDevice, 1, 1, 1, (TextureUsage)0, (SurfaceFormat)1);
		grayFilter.SetData<Color>((Color[])(object)new Color[1]
		{
			new Color(new Vector4(0.2f, 0.2f, 0.2f, 0.75f))
		});
		pauseWindow = new StretchTex();
		pauseWindow.Initialize(9, 12, 9, 12, "Content\\WindowTex");
		((DrawableGameComponent)this).Initialize();
	}

	protected override void LoadContent()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		Color green = Color.Green;
		Color white = Color.White;
		Color val = default(Color);
		((Color)(ref val))._002Ector((byte)100, (byte)100, (byte)100);
		ps = new ParticleSystem();
		ps.LoadGraphics();
		pauseMenu = new Menu();
		pauseMenu.Add("[- Resume -]", new Vector2((float)(BaseGame.WIDTH / 8), (float)(3 * BaseGame.HEIGHT / 8)), green, white, "resume");
		pauseMenu.AddDisabled("NETWORK ANALYSIS PAUSED", new Vector2((float)(BaseGame.WIDTH / 8), (float)(2 * BaseGame.HEIGHT / 8)), val, val, "");
		pauseMenu.Add("[- Invert Y Axis -]", new Vector2((float)(BaseGame.WIDTH / 8), (float)(4 * BaseGame.HEIGHT / 8)), green, white, "invert", BaseGame.OnOptionSwitchedHandler, "On", "Off");
		((MenuItemOption)pauseMenu.items[2]).SetChoice(BaseGame.Get().invert ? "On" : "Off");
		pauseMenu.Add("[- Rumble -]", new Vector2((float)(BaseGame.WIDTH / 8), (float)(5 * BaseGame.HEIGHT / 8)), green, white, "rumble", BaseGame.OnOptionSwitchedHandler, "On", "Off");
		((MenuItemOption)pauseMenu.items[3]).SetChoice(BaseGame.Get().rumble ? "On" : "Off");
		pauseMenu.Add("[- Exit to Menu -]", new Vector2((float)(BaseGame.WIDTH / 8), (float)(6 * BaseGame.HEIGHT / 8)), green, white, "exit");
		exitMenu = new Menu();
		exitMenu.Add("[- Exit -]", new Vector2((float)(BaseGame.WIDTH / 4), (float)(4 * BaseGame.HEIGHT / 8)), green, white, "exit");
		exitMenu.Add("[- Cancel -]", new Vector2((float)(BaseGame.WIDTH / 4), (float)(5 * BaseGame.HEIGHT / 8)), green, white, "cancel");
		((DrawableGameComponent)this).LoadContent();
	}

	public override void Update(GameTime gameTime)
	{
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		if (BaseGame.Get().debug)
		{
			if (BaseGame.Get().input.KeyPressed((Keys)37) || BaseGame.Get().input.PadPressed((Buttons)256))
			{
				tempZone = (tempZone - 1 + BaseGame.Get().level.zones.Count) % BaseGame.Get().level.zones.Count;
			}
			if (BaseGame.Get().input.KeyPressed((Keys)39) || BaseGame.Get().input.PadPressed((Buttons)512))
			{
				tempZone = (tempZone + 1) % BaseGame.Get().level.zones.Count;
			}
		}
		particleSource = Vector3.Lerp(particleSource, new Vector3(currentMenuItem.position + new Vector2(-60f, 16f), 0f), 4f * (float)gameTime.ElapsedGameTime.TotalSeconds);
		curPart -= (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (curPart < 0f)
		{
			curPart += partDelay;
			BaseGame.Get().ps.AddParticlesFlat(particleSource, new Vector3(50f, 0f, 0f), 0.2f, 180f, Vector3.Zero, 0f, 1.2f, 0.2f, 0f, new Vector4(1f, 1f, 1f, 1f), numPart, genTime, 1f);
		}
		if (!confirmExit)
		{
			if (BaseGame.Get().input.DirectionUp())
			{
				pauseMenu.MoveUp();
			}
			if (BaseGame.Get().input.DirectionDown())
			{
				pauseMenu.MoveDown();
			}
			if (BaseGame.Get().input.Select())
			{
				if (pauseMenu.ActiveItem.command == "resume")
				{
					Resume();
				}
				else if (pauseMenu.ActiveItem.command == "exit")
				{
					confirmExit = true;
				}
			}
			if (BaseGame.Get().input.PadPressed((Buttons)8192) || BaseGame.Get().input.KeyPressed((Keys)80) || BaseGame.Get().input.PadPressed((Buttons)16))
			{
				Resume();
			}
		}
		else
		{
			if (BaseGame.Get().input.DirectionUp())
			{
				exitMenu.MoveUp();
			}
			if (BaseGame.Get().input.DirectionDown())
			{
				exitMenu.MoveDown();
			}
			if (BaseGame.Get().input.Select())
			{
				if (exitMenu.ActiveItem.command == "cancel")
				{
					confirmExit = false;
				}
				else if (exitMenu.ActiveItem.command == "exit")
				{
					tentativeExit = true;
					BaseGame.Get().BeginSavePlayer();
					BaseGame.Get().StopAndClearAllCues();
				}
			}
			if (BaseGame.Get().input.PadPressed((Buttons)8192))
			{
				confirmExit = false;
			}
		}
		if (tentativeExit && BaseGame.Get().PlayerSaved)
		{
			for (int num = ((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Count - 1; num >= 0; num--)
			{
				if (((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components)[num] is BaseComponent)
				{
					((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).RemoveAt(num);
				}
			}
			((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Remove((IGameComponent)(object)this);
			((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Add((IGameComponent)(object)new MainMenuComponent(((GameComponent)this).Game, 6));
		}
		if (tentativeResume && BaseGame.Get().PlayerSaved)
		{
			if (tempZone != BaseGame.Get().level.activeZone)
			{
				BaseGame.Get().zoneToLoad = tempZone;
			}
			BaseGame.Get().paused = false;
			BaseGame.Get().PlayMusic();
			BaseGame.Get().input.SetState(enterState);
			BaseGame.Get().input.leftHeld = enterLeft;
			BaseGame.Get().input.rightHeld = enterRight;
			((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Remove((IGameComponent)(object)this);
			for (int num2 = ((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).Count - 1; num2 >= 0; num2--)
			{
				if (((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components)[num2] is VerifySaveLocationComponent)
				{
					((Collection<IGameComponent>)(object)((GameComponent)this).Game.Components).RemoveAt(num2);
				}
			}
		}
		pauseMenu.Update(gameTime);
		((GameComponent)this).Update(gameTime);
	}

	public void Resume()
	{
		tentativeResume = true;
		BaseGame.Get().BeginSavePlayer();
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		((Effect)BaseGame.Get().flatEffect).Begin();
		((Effect)BaseGame.Get().flatEffect).CurrentTechnique.Passes[0].Begin();
		BaseGame.Get().spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
		BaseGame.Get().spriteBatch.Draw(grayFilter, new Rectangle(0, 0, BaseGame.WIDTH, BaseGame.HEIGHT), Color.White);
		pauseWindow.Draw(new Vector2(100f, 100f), new Vector2((float)BaseGame.WIDTH - 100f, (float)BaseGame.HEIGHT - 100f), Color.Orange, 0f);
		if (BaseGame.Get().debug)
		{
			BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, tempZone.ToString(), new Vector2((float)(BaseGame.WIDTH / 4), (float)(BaseGame.HEIGHT / 4)), Color.White, 0f, new Vector2(0f, 0f), HUD.textScale, (SpriteEffects)0, 0f);
		}
		pauseMenu.Draw(gameTime);
		if (confirmExit)
		{
			pauseWindow.Draw(new Vector2(200f, 200f), new Vector2((float)BaseGame.WIDTH - 200f, (float)BaseGame.HEIGHT - 200f), Color.Red);
			BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.HUDfont, "Exit level and lose progress?", new Vector2((float)BaseGame.WIDTH / 5f), Color.White, 0f, new Vector2(0f, 0f), HUD.textScale, (SpriteEffects)0, 0f);
			exitMenu.Draw(gameTime);
		}
		BaseGame.Get().spriteBatch.End();
		((Effect)BaseGame.Get().flatEffect).CurrentTechnique.Passes[0].End();
		((Effect)BaseGame.Get().flatEffect).End();
		ps.Draw(gameTime);
		((DrawableGameComponent)this).Draw(gameTime);
	}
}
