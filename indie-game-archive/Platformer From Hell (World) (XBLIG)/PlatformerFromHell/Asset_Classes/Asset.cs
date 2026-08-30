using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PlatformerFromHell.Asset_Classes;

internal abstract class Asset
{
	public enum Dir
	{
		UpLeft,
		UpRight,
		DownLeft,
		DownRight
	}

	private struct ImageID(Texture2D newText, Dir newDir)
	{
		private Texture2D texture = newText;

		private Dir dir = newDir;
	}

	private float elapsed;

	private float previousElapsed = 42f;

	public short gravCode = 16;

	private Color takenMoneyColor = Color.BurlyWood;

	public int frameWidth;

	public int frameHeight;

	private static Dictionary<ImageID, Texture2D> flippedTextures = new Dictionary<ImageID, Texture2D>();

	public static bool showHitmaps = false;

	private Texture2D internalCurrImage;

	private Texture2D internalCurrHitmap;

	private Texture2D fullHitmap;

	public Texture2D fullTexture;

	private Rectangle internalCurrRect;

	private Rectangle internalCurrHitmapRect;

	public Color[] textureData;

	public Color[] hitmapData;

	public string texturename;

	public float canBeTouched = 101f;

	protected Dir flip;

	public Dir originalFlip;

	public AssetTextureMaster textureMaster;

	private int frameCount;

	public bool disabled = false;

	public bool gotMoney = false;

	protected Vector2 position;

	private Level level;

	public Level Level => level;

	public Vector2 Position => position;

	public Texture2D currHitmap
	{
		get
		{
			if (flip != Dir.UpLeft)
			{
				ImageID key = new ImageID(internalCurrHitmap, flip);
				if (!flippedTextures.ContainsKey(key))
				{
					flippedTextures.Add(key, FlipTexture(internalCurrHitmap, flip == Dir.DownLeft || flip == Dir.DownRight, flip == Dir.UpRight || flip == Dir.DownRight));
				}
				return flippedTextures[key];
			}
			return internalCurrHitmap;
		}
	}

	public Rectangle currRect => new Rectangle((int)Position.X, (int)Position.Y, frameWidth, frameHeight);

	public Rectangle currHitmapRect => internalCurrHitmapRect;

	public Asset(Level level, Vector2 position, string texturename, int frameCount, Dir newFlip)
	{
		this.level = level;
		this.position = position;
		this.texturename = texturename;
		this.frameCount = frameCount;
		flip = newFlip;
		originalFlip = newFlip;
		LoadContent();
	}

	public virtual void LoadContent()
	{
		if (texturename.Contains("switch_"))
		{
			frameCount = 8;
		}
		if (texturename.Contains("_sign"))
		{
			texturename += "PC";
		}
		string text = "Sprites/Assets/world" + level.worldNumber + "/";
		if (texturename.Contains("gravity") && gravCode != 16)
		{
			frameCount = gravCode;
			text = "Sprites/Assets/generic/";
			texturename = gravCode + "frame_gravity";
		}
		fullTexture = Level.Content.Load<Texture2D>(text + texturename);
		textureData = new Color[fullTexture.Width * fullTexture.Height];
		fullTexture.GetData(textureData);
		frameWidth = fullTexture.Width / frameCount;
		frameHeight = fullTexture.Height;
		internalCurrImage = fullTexture;
		if (texturename.Contains("top_ground"))
		{
			fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/generic_ground_hitmap_top");
		}
		else if (texturename.Contains("edge_ground"))
		{
			fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/generic_ground_hitmap_edge");
		}
		else if (texturename.Contains("corner_ground"))
		{
			fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/generic_ground_hitmap_corner");
		}
		else if (texturename.Contains("joint_ground"))
		{
			fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/generic_ground_hitmap_joint");
		}
		else if (texturename.Contains("ground"))
		{
			fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/generic_ground_hitmap");
		}
		else if (texturename.Contains("gravity"))
		{
			fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/generic_gravity_hitmap");
		}
		else if (texturename.Contains("edge_lava"))
		{
			fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/generic_edge_lava_hitmap");
		}
		else if (texturename.ToLower().Contains("deep_lava"))
		{
			fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/generic_deep_lava_hitmap");
		}
		else if (texturename.ToLower().Contains("top_lava"))
		{
			fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/generic_top_lava_hitmap");
		}
		else if (texturename.Contains("_blades"))
		{
			fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/generic_blades_hitmap");
		}
		else if (texturename.Contains("switch_"))
		{
			if (frameCount == 8)
			{
				fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/generic_switch_hitmap");
			}
			else
			{
				fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/14frame_switch_hitmap");
			}
		}
		else if (texturename.Contains("_sign"))
		{
			fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/generic_sign_hitmap");
		}
		else
		{
			fullHitmap = Level.Content.Load<Texture2D>("Sprites/Assets/hitmaps/" + texturename + "_hitmap");
		}
		hitmapData = new Color[fullHitmap.Width * fullHitmap.Height];
		fullHitmap.GetData(hitmapData);
		internalCurrHitmap = fullHitmap;
		textureMaster = new AssetTextureMaster(fullTexture, fullHitmap, frameCount);
		if (frameCount > 1)
		{
			fullTexture.Name = texturename;
			textureMaster.PlayAnimation(new Animation(fullTexture, fullHitmap, 0.1f, frameCount, isLooping: true));
		}
	}

	public virtual void Flip(Dir newFlip)
	{
		flip = newFlip;
	}

	public virtual void ChangeFlip(int change)
	{
		flip += change;
	}

	public virtual Dir GetFlip()
	{
		return flip;
	}

	public void Update(GameTime gameTime)
	{
		if (canBeTouched <= 100f)
		{
			canBeTouched += (float)gameTime.ElapsedGameTime.TotalSeconds * 60f;
		}
		elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
		if (elapsed >= previousElapsed * 1.5f)
		{
			elapsed /= 1.5f;
		}
		previousElapsed = elapsed;
		internalCurrHitmap = textureMaster.getHitmapFrame(gameTime);
		internalCurrRect = textureMaster.getRect(gameTime);
		internalCurrHitmapRect = textureMaster.getRect(gameTime);
	}

	public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
	{
		if (disabled)
		{
			return;
		}
		Texture2D texture = fullTexture;
		if (showHitmaps)
		{
			texture = fullHitmap;
		}
		Rectangle destinationRectangle = new Rectangle((int)position.X, (int)position.Y, internalCurrRect.Width, internalCurrRect.Height);
		Rectangle value = internalCurrRect;
		switch (flip)
		{
		case Dir.UpLeft:
			if (texturename.Contains("money"))
			{
				Color white = Color.White;
				if (level.platformerGame.worldNumber == 1)
				{
					if (level.platformerGame.world1Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 2)
				{
					if (level.platformerGame.world2Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 3)
				{
					if (level.platformerGame.world3Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 4)
				{
					if (level.platformerGame.world4Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 5 && level.platformerGame.world5Moneys[level.levelNumber] == 1)
				{
					white = takenMoneyColor;
				}
				spriteBatch.Draw(texture, destinationRectangle, value, white);
			}
			else
			{
				spriteBatch.Draw(texture, destinationRectangle, value, Color.White);
			}
			break;
		case Dir.UpRight:
			if (texturename.Contains("money"))
			{
				Color white = Color.White;
				if (level.platformerGame.worldNumber == 1)
				{
					if (level.platformerGame.world1Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 2)
				{
					if (level.platformerGame.world2Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 3)
				{
					if (level.platformerGame.world3Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 4)
				{
					if (level.platformerGame.world4Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 5 && level.platformerGame.world5Moneys[level.levelNumber] == 1)
				{
					white = takenMoneyColor;
				}
				spriteBatch.Draw(texture, destinationRectangle, value, white);
			}
			else
			{
				value = new Rectangle(value.X + value.Width, value.Y, -value.Width, value.Height);
				spriteBatch.Draw(texture, destinationRectangle, value, Color.White);
			}
			break;
		case Dir.DownLeft:
			if (texturename.Contains("money"))
			{
				Color white = Color.White;
				if (level.platformerGame.worldNumber == 1)
				{
					if (level.platformerGame.world1Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 2)
				{
					if (level.platformerGame.world2Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 3)
				{
					if (level.platformerGame.world3Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 4)
				{
					if (level.platformerGame.world4Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 5 && level.platformerGame.world5Moneys[level.levelNumber] == 1)
				{
					white = takenMoneyColor;
				}
				spriteBatch.Draw(texture, destinationRectangle, value, white);
			}
			else
			{
				value = new Rectangle(value.X, value.Y + value.Height, value.Width, -value.Height);
				spriteBatch.Draw(texture, destinationRectangle, value, Color.White);
			}
			break;
		case Dir.DownRight:
			if (texturename.Contains("money"))
			{
				Color white = Color.White;
				if (level.platformerGame.worldNumber == 1)
				{
					if (level.platformerGame.world1Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 2)
				{
					if (level.platformerGame.world2Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 3)
				{
					if (level.platformerGame.world3Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 4)
				{
					if (level.platformerGame.world4Moneys[level.levelNumber] == 1)
					{
						white = takenMoneyColor;
					}
				}
				else if (level.platformerGame.worldNumber == 5 && level.platformerGame.world5Moneys[level.levelNumber] == 1)
				{
					white = takenMoneyColor;
				}
				spriteBatch.Draw(texture, destinationRectangle, value, white);
			}
			else
			{
				value = new Rectangle(value.X + value.Width, value.Y + value.Height, -value.Width, -value.Height);
				spriteBatch.Draw(texture, destinationRectangle, value, Color.White);
			}
			break;
		}
	}

	public static Texture2D FlipTexture(Texture2D source, bool vertical, bool horizontal)
	{
		Texture2D texture2D = new Texture2D(source.GraphicsDevice, source.Width, source.Height);
		Color[] array = new Color[source.Width * source.Height];
		Color[] array2 = new Color[array.Length];
		source.GetData(array);
		for (int i = 0; i < source.Width; i++)
		{
			for (int j = 0; j < source.Height; j++)
			{
				int num = (horizontal ? (source.Width - 1 - i) : i) + (vertical ? (source.Height - 1 - j) : j) * source.Width;
				ref Color reference = ref array2[i + j * source.Width];
				reference = array[num];
			}
		}
		texture2D.SetData(array2);
		return texture2D;
	}

	public Color[] getHitmapData()
	{
		Color[] array = new Color[currHitmap.Width * currHitmap.Height];
		currHitmap.GetData(array);
		return array;
	}

	public void ChangeTexture(string newPath)
	{
		fullTexture = Level.Content.Load<Texture2D>(newPath);
	}

	public static void StaticDispose()
	{
		foreach (Texture2D value in flippedTextures.Values)
		{
			value.Dispose();
		}
		flippedTextures.Clear();
		AssetTextureMaster.StaticDispose();
	}
}
