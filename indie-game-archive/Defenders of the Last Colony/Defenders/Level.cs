using System;
using System.Collections.Generic;
using Hammer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Defenders;

internal class Level
{
	public Vector2 position = Vector2.Zero;

	public float cameraZoom = 1f;

	public bool active;

	public float angle = 0f;

	public float size;

	public Texture2D texture;

	private float brfSize = 0f;

	public bool selected;

	public bool briefing;

	public bool locked;

	public bool played;

	public string name;

	public Texture2D textureBrf;

	public Texture2D textureI;

	public Texture2D textureBar;

	private float barV = 0f;

	public string music = "DarkMatter";

	public int asteroids = -1;

	public Color color;

	public List<EnemyLevel> enemyList;

	private Primitive2D p2d;

	public int Width => texture.Width;

	public int Height => texture.Height;

	public Level(Texture2D texture, Texture2D textureBrf, string name, Texture2D textureI, Texture2D textureBar, GraphicsDevice GraphicsDevice)
	{
		Initialize(texture, textureBrf, name, textureI, textureBar, GraphicsDevice);
	}

	public void Initialize(Texture2D texture, Texture2D textureBrf, string name, Texture2D textureI, Texture2D textureBar, GraphicsDevice GraphicsDevice)
	{
		this.texture = texture;
		this.textureBrf = textureBrf;
		this.textureI = textureI;
		this.textureBar = textureBar;
		this.name = name;
		briefing = false;
		locked = true;
		if (Program.arguments.Length > 0 && Program.arguments[0] == "EDITOR")
		{
			locked = false;
		}
		played = false;
		active = true;
		enemyList = new List<EnemyLevel>();
		color = Color.Black;
		p2d = new Primitive2D(GraphicsDevice);
		createLevels();
	}

	public void createLevels()
	{
		Random random = new Random();
		switch (name)
		{
		case "Hines":
			position = new Vector2(-650f, -73f);
			asteroids = -1;
			cameraZoom = 1.3f;
			locked = false;
			music = "DarkMatter";
			color = new Color(0.001f, 0.09f, 0.1f);
			break;
		case "Nymeriah":
			position = new Vector2(-540f, 70f);
			asteroids = -1;
			cameraZoom = 1.2f;
			locked = false;
			music = "musicLevel01";
			color = new Color(0f, 0.09f, 0.007f);
			break;
		case "Herschel":
			position = new Vector2(-363f, 139f);
			asteroids = -1;
			cameraZoom = 1f;
			music = "TimeToRun";
			color = new Color(0.1f, 0.11f, 0.15f);
			break;
		case "Danae":
			position = new Vector2(-227f, 323f);
			asteroids = 0;
			cameraZoom = 1.4f;
			music = "HeIsAlive";
			color = new Color(0.06f, 0.05f, 0f);
			break;
		case "Clarke":
			position = new Vector2(-7f, 149f);
			asteroids = 0;
			cameraZoom = 1f;
			music = "HeartAndDanger";
			color = new Color(0.2f, 0.0075f, 0.22f);
			break;
		case "Gea Moon":
			position = new Vector2(225f, 211f);
			asteroids = -1;
			cameraZoom = 1.4f;
			music = "MoonStrings";
			color = new Color(0f, 0f, 0.02f);
			break;
		case "Calypso":
			position = new Vector2(178f, 46f);
			asteroids = -1;
			cameraZoom = 1.4f;
			music = "In_a_heart_beat";
			color = new Color(0.04f, 0.08f, 0.1f);
			break;
		case "Bradbury":
			position = new Vector2(64f, -83f);
			asteroids = -1;
			cameraZoom = 1.4f;
			music = "Pit";
			color = new Color(0.3f, 0.01f, 0.002f);
			break;
		case "Eos rests":
			position = new Vector2(222f, -166f);
			asteroids = 0;
			cameraZoom = 1.4f;
			music = "WeirdDimensions";
			color = new Color(0.1f, 0.08f, 0.05f);
			break;
		case "Olbers 4":
			position = new Vector2(455f, -85f);
			asteroids = -1;
			cameraZoom = 1.4f;
			music = "UnknownBellow";
			color = new Color(0.1f, 0.05f, 0.2f);
			break;
		case "Eneas":
			position = new Vector2(573f, 68f);
			asteroids = -1;
			cameraZoom = 1.4f;
			music = "In_a_heart_beat";
			color = new Color(0.35f, 0f, 0f);
			break;
		case "Prometheus":
			position = new Vector2(793f, 189f);
			asteroids = -1;
			cameraZoom = 1.4f;
			music = "LongIsTheWay";
			color = new Color(0.08f, 0.1f, 0.12f);
			break;
		case "Tutorial Level":
			position = new Vector2(793f, 189f);
			asteroids = -1;
			cameraZoom = 1.4f;
			music = "FaceYourFears";
			color = new Color(0.04f, 0.08f, 0.16f);
			break;
		default:
			position = new Vector2(833f, 209f);
			asteroids = -1;
			cameraZoom = 1.2f;
			music = "DarkMatter";
			color = Color.Black;
			break;
		}
	}

	public void ResetLevel()
	{
		for (int i = 0; i < enemyList.Count; i++)
		{
			enemyList[i].Reset();
		}
		briefing = false;
		brfSize = 0f;
		active = true;
	}

	public void Update(bool selected)
	{
		this.selected = selected;
		if (selected)
		{
			size = MathHelper.Lerp(size, 1f + brfSize, 0.25f);
		}
		else
		{
			size = MathHelper.Lerp(size, 0.75f, 0.1f);
		}
		if (briefing)
		{
			brfSize = MathHelper.Lerp(brfSize, 1f, 0.25f);
		}
		else
		{
			brfSize = MathHelper.Lerp(brfSize, 0f, 0.15f);
		}
		barV += 1.5f;
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(texture, position, null, new Color(1f, 1f, 1f, size / 2f + 0.5f), angle, new Vector2(Width / 2, Height / 2), size, SpriteEffects.None, 0f);
	}

	public void DrawBrf(SpriteBatch spriteBatch, Vector2 res)
	{
		if (!locked)
		{
			spriteBatch.Draw(textureBrf, new Vector2(res.X / 2f, res.Y / 2f), null, new Color(brfSize, brfSize, brfSize, brfSize), 0f, new Vector2(textureBrf.Width / 2, textureBrf.Height / 2), new Vector2(3f - brfSize * 2f, brfSize), SpriteEffects.None, 1f);
		}
		for (int i = 0; (float)i < res.Y / 4f; i++)
		{
			p2d.DrawPixel(spriteBatch, new Rectangle(0, i * 4, (int)res.X, 2), Color.Black * brfSize);
		}
		if (barV > res.Y + (float)textureBar.Height)
		{
			barV = -textureBar.Height;
		}
		spriteBatch.Draw(textureBar, new Vector2(res.X / 2f, barV), null, new Color(brfSize / 4f, brfSize / 4f, brfSize / 3f, brfSize / 4f), 0f, new Vector2(textureBar.Width / 2, textureBar.Height / 2), new Vector2(res.X / (float)textureBar.Width, 1f), SpriteEffects.None, 0f);
	}
}
