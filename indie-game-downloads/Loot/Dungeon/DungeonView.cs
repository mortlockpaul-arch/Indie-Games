using System;
using Eyehook.Framework;
using Loot.NPCs;
using Loot.TileSets;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Loot.Dungeon;

public class DungeonView : Screen
{
	public const int cellSize = 64;

	public static Camera Camera = new Camera();

	public static Vector2 Center = new Vector2(MC.Game.GraphicsDevice.Viewport.Width / 2, MC.Game.GraphicsDevice.Viewport.Height / 2);

	private static Controller Controller = new Controller();

	private Screen hudScreen;

	private static Texture2D fogTile;

	private static Texture2D lightMask;

	private static Texture2D lightMaskOverlay;

	private static readonly Vector2 lightMaskOrigin = new Vector2(256f, 256f);

	private static Texture2D dangerTexture;

	private static Sprite goldDoor;

	private static Sprite secretDoorTip;

	private static AnimatedSprite secretDoorPerception;

	private static Color[] fogColorTable;

	private float perRot;

	private Color shadowColor = new Color(192, 192, 192);

	private Color lightMaskOverlayColor = Color.White * 0.25f;

	private float fogRotation;

	private bool wasCrit;

	private Color critColor;

	private TimeSpan critTimer;

	private readonly TimeSpan critHeartbeatInterval = TimeSpan.FromMilliseconds(1000.0);

	private int minRow;

	private int maxRow;

	private int minCol;

	private int maxCol;

	private int minLitRow;

	private int maxLitRow;

	private int minLitCol;

	private int maxLitCol;

	private Color fogTileColor = new Color(64, 64, 64);

	private Vector3 SquareCenter;

	private VertexPositionColor[] Square = new VertexPositionColor[4]
	{
		new VertexPositionColor(Vector3.Zero, Color.Black),
		new VertexPositionColor(Vector3.Zero, Color.Black),
		new VertexPositionColor(Vector3.Zero, Color.Black),
		new VertexPositionColor(Vector3.Zero, Color.Black)
	};

	private static VertexPositionColor[] SquareTemplate = new VertexPositionColor[4]
	{
		new VertexPositionColor(new Vector3(-32.5f, 32.5f, 0f), Color.Black),
		new VertexPositionColor(new Vector3(-32.5f, -32.5f, 0f), Color.Black),
		new VertexPositionColor(new Vector3(32.5f, 32.5f, 0f), Color.Black),
		new VertexPositionColor(new Vector3(32.5f, -32.5f, 0f), Color.Black)
	};

	public DungeonView()
		: base(modal: true)
	{
		updateLighting();
	}

	public static void Reset()
	{
		Camera = new Camera();
		Center = new Vector2(MC.Game.GraphicsDevice.Viewport.Width / 2, MC.Game.GraphicsDevice.Viewport.Height / 2);
	}

	public override void transitionOn()
	{
		hudScreen = new HUDScreen();
		MC.ScreenManager.addScreen(hudScreen);
	}

	public override void transitionOff()
	{
		MC.ScreenManager.removeScreen(hudScreen);
	}

	public static void Load(ContentManager content)
	{
		fogTile = content.Load<Texture2D>("Sprites\\Dungeon\\FogTile");
		lightMask = content.Load<Texture2D>("Sprites\\Dungeon\\LightMask");
		lightMaskOverlay = content.Load<Texture2D>("Sprites\\Dungeon\\LightMaskOverlay");
		dangerTexture = content.Load<Texture2D>("Sprites\\HUD\\Danger");
		goldDoor = new StillSprite(content.Load<Texture2D>("Sprites\\Dungeon\\GoldDoor"));
		secretDoorTip = new StillSprite(content.Load<Texture2D>("Sprites\\Dungeon\\SecretDoorTip"));
		secretDoorPerception = new AnimatedSprite(content.Load<Texture2D>("Sprites\\Dungeon\\secretDoorPerception"), new Rectangle(0, 0, 64, 64), new Vector2(32f, 32f), 8, TimeSpan.FromMilliseconds(100.0));
		fogColorTable = new Color[10];
		for (int i = 0; i < 10; i++)
		{
			byte b = (byte)(64 + DM.Random.Next(32));
			fogColorTable[i] = new Color(b, b, b);
		}
	}

	public override void update(GameTime gameTime)
	{
		UpdateSprites(gameTime);
		updateLighting();
		updateFog(gameTime);
		updateCritical(gameTime);
		DM.KillCount = 0;
		Controller.Update(gameTime);
		DM.Update(gameTime);
	}

	private void UpdateSprites(GameTime gameTime)
	{
		perRot += (float)(3.1415927410125732 * gameTime.ElapsedGameTime.TotalSeconds);
		if ((double)perRot > 6.2831854820251465)
		{
			perRot -= (float)Math.PI * 2f;
		}
		secretDoorPerception.Update(gameTime);
		secretDoorPerception.Rotation = perRot;
	}

	private void updateLighting()
	{
		float num = DM.Player.Lantern.Fuel * 2f;
		float amount = (((double)num < 1.0) ? ((float)(0.3499999940395355 + 0.6499999761581421 * (double)num)) : 1f);
		shadowColor = Color.Lerp(Color.Black, new Color(192, 192, 192), amount);
		lightMaskOverlayColor = Color.Lerp(Color.Black, Color.Black * 0.25f, amount);
	}

	protected void updateFog(GameTime gameTime)
	{
		if (Profile.Preferences.Wobble)
		{
			fogRotation += (float)(gameTime.ElapsedGameTime.TotalSeconds * 3.1415927410125732);
			if (!((double)fogRotation <= 6.2831854820251465))
			{
				fogRotation -= (float)Math.PI * 2f;
			}
		}
	}

	private void updateCritical(GameTime gameTime)
	{
		if (!DM.Player.IsCritical)
		{
			wasCrit = false;
			return;
		}
		if (!wasCrit)
		{
			critTimer = critHeartbeatInterval;
			wasCrit = true;
		}
		critTimer += gameTime.ElapsedGameTime;
		if (critTimer >= critHeartbeatInterval)
		{
			PlaySound.Heartbeat();
			if (Profile.Preferences.Vibrate)
			{
				MC.GamePadManager.vibrate(TimeSpan.FromMilliseconds(250.0), 0.5f, 0.5f);
			}
			critTimer = TimeSpan.Zero;
		}
		critColor = Color.White * (float)Math.Abs(Math.Sin(3.1415927410125732 * (critTimer.TotalSeconds / critHeartbeatInterval.TotalSeconds)));
	}

	public override void draw(GameTime gameTime)
	{
		Camera.Update(gameTime);
		SetBounds();
		drawFogTile();
		drawLevel(gameTime);
		base.spriteBatch.Begin(SpriteSortMode.Immediate, null, GraphicUtil.PointFilter, null, null);
		drawNPCs();
		drawFog();
		if (DM.Player.Thought != null)
		{
			DM.Player.Thought.Draw(base.spriteBatch, Center, Camera.Zoom);
		}
		for (int i = 0; i < DM.EffectList.Count; i++)
		{
			DM.EffectList[i].Draw(base.spriteBatch);
		}
		drawCritical();
		base.spriteBatch.End();
	}

	private void SetBounds()
	{
		minRow = DM.Player.Location.Row - (int)Math.Round((double)Center.Y / (64.0 * (double)Camera.Zoom)) - 1;
		maxRow = DM.Player.Location.Row + (int)Math.Round(((double)base.viewportRect.Height - (double)Center.Y) / (64.0 * (double)Camera.Zoom)) + 2;
		minCol = DM.Player.Location.Col - (int)Math.Round((double)Center.X / (64.0 * (double)Camera.Zoom)) - 1;
		maxCol = DM.Player.Location.Col + (int)Math.Round(((double)base.viewportRect.Width - (double)Center.X) / (64.0 * (double)Camera.Zoom)) + 2;
		if (minRow < 0)
		{
			minRow = 0;
		}
		if (maxRow > DM.Map.Rows)
		{
			maxRow = DM.Map.Rows;
		}
		if (minCol < 0)
		{
			minCol = 0;
		}
		if (maxCol > DM.Map.Cols)
		{
			maxCol = DM.Map.Cols;
		}
		minLitRow = DM.Player.Location.Row - 6;
		maxLitRow = DM.Player.Location.Row + 6;
		minLitCol = DM.Player.Location.Col - 6;
		maxLitCol = DM.Player.Location.Col + 6;
		if (minLitRow < 0)
		{
			minLitRow = 0;
		}
		if (maxLitRow > DM.Map.Rows)
		{
			maxLitRow = DM.Map.Rows;
		}
		if (minLitCol < 0)
		{
			minLitCol = 0;
		}
		if (maxLitCol > DM.Map.Cols)
		{
			maxLitCol = DM.Map.Cols;
		}
	}

	private void drawFogTile()
	{
		base.spriteBatch.Begin(SpriteSortMode.Immediate, null, GraphicUtil.Wrap, null, null);
		Rectangle value = new Rectangle(0, 0, (int)((double)(base.viewportRect.Width + 192) / (double)Camera.Zoom), (int)((double)(base.viewportRect.Height + 192) / (double)Camera.Zoom));
		Vector2 vector = DM.Player.Offset * Camera.Zoom + new Vector2(704f, 424f) * Camera.Zoom - new Vector2(704f, 424f);
		vector.X %= 64f * Camera.Zoom;
		vector.Y %= 64f * Camera.Zoom;
		base.spriteBatch.Draw(fogTile, new Vector2(-64f, -64f) - vector, value, fogTileColor, 0f, Vector2.Zero, Camera.Zoom, SpriteEffects.None, 0f);
		base.spriteBatch.End();
	}

	private void setSquare(Vector2 pos, float scale)
	{
		SquareCenter = Vector3.Zero;
		for (int i = 0; i < 4; i++)
		{
			Square[i].Position = SquareTemplate[i].Position * scale;
			Square[i].Position.X += pos.X;
			Square[i].Position.Y += pos.Y;
			SquareCenter += Square[i].Position;
		}
		SquareCenter /= 4f;
	}

	private void drawLevel(GameTime gameTime)
	{
		base.spriteBatch.GraphicsDevice.Clear(ClearOptions.Stencil, Color.Black, 0f, 0);
		base.spriteBatch.Begin(SpriteSortMode.Immediate, null, GraphicUtil.PointFilter, GraphicUtil.Stencil1Always, null, GraphicUtil.AlphaGreaterThan0);
		drawLevelPass(isLit: false);
		base.spriteBatch.End();
		base.spriteBatch.Begin(SpriteSortMode.Immediate, GraphicUtil.NoColorWrite, null, GraphicUtil.Stencil1Increment, null, GraphicUtil.AlphaGreaterThan0);
		base.spriteBatch.Draw(lightMask, Center, null, Color.Black, 0f, lightMaskOrigin, DM.Player.Lantern.Radius * Camera.Zoom, SpriteEffects.None, 0f);
		base.spriteBatch.End();
		base.spriteBatch.Begin(SpriteSortMode.Immediate, GraphicUtil.NoColorWrite, null, GraphicUtil.Stencil0Always, null, GraphicUtil.AlphaGreaterThan0);
		int num = DM.Player.Location.Row - 6;
		int num2 = DM.Player.Location.Row + 6;
		int num3 = DM.Player.Location.Col - 6;
		int num4 = DM.Player.Location.Col + 6;
		Vector3 lightPos = new Vector3(Center.X + DM.Player.LightOffset.X * Camera.Zoom, Center.Y + DM.Player.LightOffset.Y * Camera.Zoom, 0f);
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				Location loc = new Location(i, j);
				if (DM.Map.IsDiscovered(loc) && DM.Map.IsWall(loc))
				{
					setSquare(Camera.Offset + new Vector2((float)(j * 64) * Camera.Zoom, (float)(i * 64) * Camera.Zoom), Camera.Zoom);
					Shadow.DrawShadow(lightPos, Square, SquareCenter);
				}
			}
		}
		base.spriteBatch.End();
		base.spriteBatch.Begin(SpriteSortMode.Immediate, null, GraphicUtil.PointFilter, GraphicUtil.Stencil2Match, null);
		drawLevelPass(isLit: true);
		base.spriteBatch.Draw(lightMaskOverlay, Center, null, lightMaskOverlayColor, 0f, lightMaskOrigin, DM.Player.Lantern.Radius * Camera.Zoom, SpriteEffects.None, 1f);
		base.spriteBatch.End();
	}

	private void drawLevelPass(bool isLit)
	{
		Color color = (isLit ? Color.White : shadowColor);
		int num;
		int num2;
		int num3;
		int num4;
		if (isLit)
		{
			num = minLitRow;
			num2 = maxLitRow;
			num3 = minLitCol;
			num4 = maxLitCol;
		}
		else
		{
			num = minRow;
			num2 = maxRow;
			num3 = minCol;
			num4 = maxCol;
		}
		bool flag = Profile.Preferences.Tips && DM.Player.Depth <= 6;
		for (int i = num; i < num2; i++)
		{
			for (int j = num3; j < num4; j++)
			{
				Location loc = new Location(i, j);
				if (!DM.Map.IsDiscovered(loc))
				{
					continue;
				}
				Vector2 vector = Camera.Offset + new Vector2((float)(j * 64) * Camera.Zoom, (float)(i * 64) * Camera.Zoom);
				DM.Map.DrawCell(base.spriteBatch, loc, vector, color, Camera.Zoom);
				Tile tile = DM.Map.GetTile(loc);
				if (tile.IsSecretDoor())
				{
					if (flag)
					{
						secretDoorTip.Draw(base.spriteBatch, vector, color, Camera.Zoom);
					}
					else if (DM.Player.SkillSet.Perception.Level > 0)
					{
						float num5 = (float)DM.Player.SkillSet.Perception.Level / 10f;
						secretDoorPerception.Draw(base.spriteBatch, vector, Color.White * num5, Camera.Zoom);
					}
				}
				else if ((double)Camera.Zoom == 0.5 && tile.IsDoor() && !tile.IsSecretDoor())
				{
					goldDoor.Draw(base.spriteBatch, vector, color, Camera.Zoom);
				}
			}
		}
	}

	private void drawNPCs()
	{
		for (int i = minRow; i < maxRow; i++)
		{
			for (int j = minCol; j < maxCol; j++)
			{
				Location location = new Location(i, j);
				NPC nPC = DM.Map.GetNPC(location);
				if (nPC != null && (DM.Map.IsDiscovered(nPC.Location) || DM.Map.IsDiscovered(nPC.LastLocation)))
				{
					nPC.Draw(base.spriteBatch, getNpcColor(nPC));
				}
				if (location == DM.Player.Location)
				{
					DM.Player.Draw(base.spriteBatch);
				}
			}
		}
	}

	private Color getNpcColor(NPC npc)
	{
		float num = 64f * Camera.Zoom;
		float num2 = Vector2.Distance(Center, Camera.Position(npc)) - num;
		float num3 = DM.Player.Lantern.Radius * 3f * num;
		return ((double)num2 > (double)num3) ? shadowColor : Color.Lerp(Color.White, shadowColor, num2 / num3);
	}

	private void drawFog()
	{
		int num = minRow;
		if (minRow == 0)
		{
			num = -1;
		}
		for (int i = num; i <= maxRow; i++)
		{
			for (int j = minCol; j <= maxCol; j++)
			{
				Location loc = new Location(i, j);
				if ((!DM.Map.IsValid(loc) || !DM.Map.IsDiscovered(loc)) && (DM.Map.IsDiscovered(loc.N) || DM.Map.IsDiscovered(loc.NE) || DM.Map.IsDiscovered(loc.NW) || DM.Map.IsDiscovered(loc.S) || DM.Map.IsDiscovered(loc.SE) || DM.Map.IsDiscovered(loc.SW) || DM.Map.IsDiscovered(loc.E) || DM.Map.IsDiscovered(loc.W)))
				{
					Vector2 position = new Vector2(Camera.Offset.X + (float)(j * 64) * Camera.Zoom, Camera.Offset.Y + (float)(i * 64) * Camera.Zoom);
					float num2 = fogRotation + (float)((double)((i + j) % 7) * 6.2831854820251465 / 6.0);
					position += new Vector2((float)Math.Cos(num2), (float)Math.Sin(num2)) * 4f * Camera.Zoom;
					int num3 = (i + j) % 10;
					if (num3 < 0)
					{
						num3 = 0;
					}
					Color color = fogColorTable[num3];
					Fog.Draw(base.spriteBatch, position, color);
				}
			}
		}
		for (int k = 0; k < DM.PoofList.Count; k++)
		{
			DM.PoofList[k].Draw(base.spriteBatch);
		}
	}

	private void drawCritical()
	{
		if (DM.Player.IsCritical)
		{
			base.spriteBatch.Draw(dangerTexture, base.viewportRect, critColor);
		}
	}
}
