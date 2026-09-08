using System;
using Eyehook.Framework;
using Loot.Awardments;
using Loot.Core;
using Loot.Dungeon;
using Loot.Effects;
using Loot.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Loot.Platforms;

public class PlatformerScreen : Screen
{
	private const int PlayerCol = 6;

	private const int Gravity = 1400;

	private Vector2 Jump = new Vector2(0f, -500f);

	private Vector2 JumpBoost = new Vector2(0f, -500f);

	private bool IntroMode;

	private Particle Player;

	private TimeSpan RunTime;

	private int GPCollected;

	private Vector2[] PopPos = new Vector2[lavaPop.Length];

	private bool HasLeapOfFaith;

	private TimeSpan LeapOfFaithTime = TimeSpan.FromSeconds(59.0);

	private float ColumnOffset;

	private ColumnWindow cols = new ColumnWindow(24);

	private bool FirstCol;

	private TimeSpan TipTimer;

	private int lastHeight;

	private int curHeight;

	private int remainingWidth;

	private TimeSpan s5 = TimeSpan.FromSeconds(5.0);

	private TimeSpan s10 = TimeSpan.FromSeconds(10.0);

	private TimeSpan s30 = TimeSpan.FromSeconds(30.0);

	private TimeSpan TotalDuration = TimeSpan.FromSeconds(57.0);

	private static Sprite gpIcon;

	private static Sprite dust;

	private static RunningSprite playerSprite;

	private static ParallaxLayer[] paraLayer = new ParallaxLayer[3];

	private static AnimatedSprite[] lavaPop = new AnimatedSprite[16];

	private bool Paused;

	private bool IsJumping;

	private bool CanBoost;

	private bool animateDust;

	private TimeSpan tsDust;

	private TimeSpan dustDuration = TimeSpan.FromSeconds(1.0);

	private Particle[] pDust = new Particle[7];

	private string tip = "Press \u0080\u0081 to Jump!";

	private FormattedText pauseTip = new FormattedText("Tap \u0080\u0081 for short jumps.\nHold \u0080\u0081 for long jumps.", 552);

	private static Color goldTextColor = new Color(255, 255, 51);

	private static Color goldTextShadowColor = new Color(204, 153, 0);

	private Vector2 playerOffset = new Vector2(0f, -16f);

	public PlatformerScreen()
		: base(modal: true)
	{
		HasLeapOfFaith = Profile.Awardments.IsUnlocked(Awardment.LeapOfFaith);
		reset();
	}

	public override void transitionOn()
	{
		BgMusic.PlatformerOpen();
	}

	private void reset()
	{
		GPCollected = 0;
		RunTime = TimeSpan.Zero;
		ColumnOffset = 0f;
		TipTimer = TimeSpan.FromSeconds(4.0);
		Paused = false;
		for (int i = 0; i < paraLayer.Length; i++)
		{
			paraLayer[i].Reset();
		}
		for (int j = 0; j < PopPos.Length; j++)
		{
			PopPos[j] = new Vector2(DM.Random.Next(1280), 350 + DM.Random.Next(150));
		}
		InitIntro();
		FirstCol = true;
		curHeight = 5;
		remainingWidth = 13;
		cols.SetMark(0);
		for (int k = 0; k < cols.Length - 1; k++)
		{
			CreateColumn(k);
		}
		Player.pos = FloorPos();
		Player.pos.Y = -64f;
		Player.vel = new Vector2(0f, 0f);
	}

	private void CreateColumn(int i)
	{
		if (remainingWidth == 0)
		{
			if (curHeight > 0)
			{
				FirstCol = false;
				lastHeight = curHeight;
				curHeight = 0;
				remainingWidth = ((RunTime < s5) ? 1 : ((RunTime < s10) ? 2 : ((!(RunTime < s30)) ? (DM.Random.Next(2) + 3) : (DM.Random.Next(2) + 2))));
			}
			else if (RunTime > TotalDuration)
			{
				curHeight = 0;
				remainingWidth = 100;
			}
			else
			{
				curHeight = ((!(RunTime < s30)) ? (lastHeight + DM.Random.Next(5) - 2) : (lastHeight + DM.Random.Next(3) - 1));
				if (curHeight > 7)
				{
					curHeight = 6;
				}
				if (curHeight < 3)
				{
					curHeight = 4;
				}
				remainingWidth = DM.Random.Next(5) + 3;
			}
		}
		Column col = new Column(curHeight);
		col.collected = false;
		if (FirstCol)
		{
			col.gold = 0;
		}
		else if (curHeight > 0 && (cols.Mark + i) % 2 == 0)
		{
			int num = DM.Player.Depth / 2 + 1;
			col.gold = (int)MathHelper.Clamp((float)((double)num * RunTime.TotalSeconds / 60.0), 1f, num);
		}
		else
		{
			col.gold = 0;
		}
		cols.Set(i, col);
		remainingWidth--;
	}

	private Vector2 FloorPos()
	{
		return FloorPos(6);
	}

	private Vector2 FloorPos(int column)
	{
		return new Vector2(384f, 720 - cols.Get(column).height * 64);
	}

	private Vector2 GoldPos()
	{
		return new Vector2(416f + ColumnOffset, 720 - cols.Get(6).height * 64);
	}

	public static void Load(ContentManager content)
	{
		gpIcon = new StillSprite(content.Load<Texture2D>("Sprites\\UI\\GP"));
		paraLayer[0] = new ParallaxLayer(content.Load<Texture2D>("Sprites\\Platformer\\Lava"), 128f);
		paraLayer[1] = new ParallaxLayer(content.Load<Texture2D>("Sprites\\Platformer\\StalagtiteBack"), 160f);
		paraLayer[2] = new ParallaxLayer(content.Load<Texture2D>("Sprites\\Platformer\\StalagtiteFront"), 192f);
		GPAnim.LoadContent(content);
		Column.LoadContent(content);
		Texture2D texture = content.Load<Texture2D>("Sprites\\Platformer\\LavaPop");
		for (int i = 0; i < lavaPop.Length; i++)
		{
			lavaPop[i] = new AnimatedSprite(texture, new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 16, TimeSpan.FromMilliseconds(80.0));
			lavaPop[i].Frame = DM.Random.Next(16);
		}
		dust = new StillSprite(content.Load<Texture2D>("Sprites\\Platformer\\Dust"));
		playerSprite = new RunningSprite(content.Load<Texture2D>("Sprites\\Player\\Body"));
	}

	public override void update(GameTime gameTime)
	{
		if (Paused)
		{
			if (MC.GamePadManager.isNewButtonDown(Buttons.Start) || MC.GamePadManager.isNewButtonDown(Buttons.A) || MC.GamePadManager.isNewButtonDown(Buttons.B))
			{
				PlaySound.MenuClick();
				MC.AudioManager.ResumeMusic();
				Paused = false;
				return;
			}
		}
		else if (MC.GamePadManager.isNewButtonDown(Buttons.Start))
		{
			MC.AudioManager.PauseMusic();
			PlaySound.MenuClick();
			Paused = true;
		}
		if (Paused)
		{
			return;
		}
		GPAnim.Update(gameTime);
		for (int i = 0; i < lavaPop.Length; i++)
		{
			lavaPop[i].Update(gameTime);
		}
		cols.Update(gameTime);
		if (TipTimer > TimeSpan.Zero)
		{
			TipTimer -= gameTime.ElapsedGameTime;
		}
		if (IntroMode)
		{
			UpdateIntro(gameTime);
			return;
		}
		for (int j = 0; j < lavaPop.Length; j++)
		{
			PopPos[j].X -= (float)(128.0 * gameTime.ElapsedGameTime.TotalSeconds);
			if ((double)PopPos[j].X < -64.0)
			{
				PopPos[j] = new Vector2(1344f, 350 + DM.Random.Next(150));
			}
		}
		RunTime += gameTime.ElapsedGameTime;
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
		float num2 = 5f + (float)(RunTime.TotalSeconds / 15.0);
		playerSprite.Update(gameTime, num2);
		if (CanBoost)
		{
			if (MC.GamePadManager.isButtonDown(Buttons.A))
			{
				Player.vel += JumpBoost * num;
			}
			else
			{
				CanBoost = false;
			}
		}
		Vector2 pos = Player.pos;
		Vector2 vector = FloorPos();
		Player.vel.Y += 1400f * num;
		Player.pos += Player.vel * num;
		if ((double)vector.Y < 720.0 && (double)pos.Y <= (double)vector.Y && (double)Player.pos.Y > (double)vector.Y)
		{
			Player.pos.Y = vector.Y;
			Player.vel = Vector2.Zero;
			IsJumping = false;
			CanBoost = false;
		}
		if ((double)Player.pos.Y > 784.0)
		{
			gameOver();
			return;
		}
		Column column = cols.Get(6);
		if (!column.collected && column.gold > 0 && (double)(Player.pos - GoldPos()).Length() < 12.0)
		{
			GPCollected += cols.Collect(6);
			PlaySound.Coin();
		}
		if ((Player.vel == Vector2.Zero || ((double)vector.Y < 720.0 && (double)new Vector2(0f, Player.pos.Y - vector.Y).Length() < 16.0)) && MC.GamePadManager.isNewButtonDown(Buttons.A))
		{
			PlaySound.Jump();
			Player.vel = Jump;
			IsJumping = true;
			CanBoost = true;
		}
		ColumnOffset -= (float)(gameTime.ElapsedGameTime.TotalSeconds * 64.0) * num2;
		if ((double)ColumnOffset <= -64.0)
		{
			ColumnOffset += 64f;
			CreateColumn(-1);
			cols.IncrementMark();
		}
		for (int k = 0; k < paraLayer.Length; k++)
		{
			paraLayer[k].Update(gameTime);
		}
		if (!HasLeapOfFaith && RunTime >= LeapOfFaithTime)
		{
			Profile.Awardments.Unlock(Awardment.LeapOfFaith);
			HasLeapOfFaith = true;
		}
	}

	private void gameOver()
	{
		DM.Player.Gold += GPCollected;
		BgMusic.PlatformerClose();
		SaveScreen.GoToDepth(DM.Player.Depth + 1, () => DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor), delegate
		{
			DM.Player.Facing = Direction.South;
			DM.AddEffect(new FXDustCloud(DM.Player.Location));
		});
	}

	private void InitIntro()
	{
		IntroMode = true;
		IsJumping = true;
		animateDust = false;
		tsDust = dustDuration;
	}

	private void UpdateIntro(GameTime gameTime)
	{
		if (animateDust)
		{
			tsDust -= gameTime.ElapsedGameTime;
			if (tsDust <= TimeSpan.Zero)
			{
				IntroMode = false;
			}
			for (int i = 0; i < pDust.Length; i++)
			{
				pDust[i].pos += pDust[i].vel * (float)gameTime.ElapsedGameTime.TotalSeconds;
			}
			return;
		}
		float num = (float)gameTime.ElapsedGameTime.TotalSeconds;
		Player.vel.Y += 1400f * num;
		Player.pos += Player.vel * num;
		Vector2 pos = FloorPos();
		if (!((double)Player.pos.Y <= (double)pos.Y))
		{
			PlaySound.DustCloud();
			Player.vel = Vector2.Zero;
			Player.pos.Y = pos.Y;
			animateDust = true;
			float num2 = -(float)Math.PI;
			float num3 = 92f;
			for (int j = 0; j < pDust.Length; j++)
			{
				pDust[j].pos = pos;
				pDust[j].vel = new Vector2((float)Math.Cos(num2) * num3, (float)Math.Sin(num2) * num3);
				num2 += (float)Math.PI / 6f;
			}
		}
	}

	public override void draw(GameTime gameTime)
	{
		paraLayer[0].Draw(base.spriteBatch);
		base.spriteBatch.Begin(SpriteSortMode.Immediate, null, GraphicUtil.PointFilter, null, null);
		for (int i = 0; i < lavaPop.Length; i++)
		{
			lavaPop[i].Draw(base.spriteBatch, PopPos[i], Color.White, 1f);
		}
		base.spriteBatch.End();
		paraLayer[1].Draw(base.spriteBatch);
		paraLayer[2].Draw(base.spriteBatch);
		base.spriteBatch.Begin(SpriteSortMode.Immediate, null, GraphicUtil.PointFilter, null, null);
		for (int j = 0; j <= 20; j++)
		{
			Column column = cols.Get(j);
			if (column.height != 0)
			{
				Column prev = cols.Get(j - 1);
				Column next = cols.Get(j + 1);
				column.Draw(base.spriteBatch, (float)(32 + 64 * j) + ColumnOffset, prev, next);
			}
		}
		drawDust(base.spriteBatch);
		drawPlayer(base.spriteBatch);
		drawTip(base.spriteBatch);
		drawHud(base.spriteBatch);
		drawPause(base.spriteBatch);
		base.spriteBatch.End();
	}

	private void drawTip(SpriteBatch spriteBatch)
	{
		if (Profile.Preferences.Tips && TipTimer > TimeSpan.Zero)
		{
			float num = 1f;
			if (TipTimer < TimeSpan.FromSeconds(1.0))
			{
				num = (float)TipTimer.TotalSeconds;
			}
			Text.DrawCentered(spriteBatch, new Vector2(640f, 240f), tip, Color.White * num);
		}
	}

	private void drawPause(SpriteBatch spriteBatch)
	{
		if (Paused)
		{
			Pixel.Draw(spriteBatch, base.viewportRect, Color.Black * 0.25f);
			MenuBox.Draw(spriteBatch, new Rectangle(340, 294, 600, 172), "Game Paused", pauseTip);
		}
	}

	private void drawDust(SpriteBatch spriteBatch)
	{
		if (animateDust)
		{
			float num = (float)(tsDust.TotalSeconds / dustDuration.TotalSeconds);
			dust.Rotation = (float)Math.PI * 2f * num;
			for (int i = 0; i < pDust.Length; i++)
			{
				dust.Draw(spriteBatch, pDust[i].pos, Color.White * num, 1f);
			}
		}
	}

	private void drawHud(SpriteBatch spriteBatch)
	{
		gpIcon.Draw(spriteBatch, new Vector2(1120f, 104f));
		Vector2 v = new Vector2(1088 - Text.Width(GPCollected), 104f);
		Text.DrawShadowString(spriteBatch, ref v, GPCollected, goldTextColor, goldTextShadowColor, 1f);
	}

	private void drawPlayer(SpriteBatch spriteBatch)
	{
		if (IsJumping)
		{
			playerSprite.DrawJump(spriteBatch, Player.pos + playerOffset);
		}
		else
		{
			playerSprite.Draw(spriteBatch, Player.pos + playerOffset);
		}
		if (DM.Player.Armor != null)
		{
			DM.Player.Armor.DrawOnPlayer(spriteBatch, Direction.East, Player.pos + playerOffset, 1f);
		}
	}
}
