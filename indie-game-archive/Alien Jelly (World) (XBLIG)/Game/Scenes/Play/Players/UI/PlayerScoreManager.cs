using GKEngine;
using GKEngine.Entities;
using GKEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Scenes.Play.Players.UI;

public class PlayerScoreManager
{
	private const float TIME = 150f;

	private const int COUNT = 8;

	private const float LERP = 0.15f;

	private const float THRESHOLD = 15f;

	private static Range ALPHA = new Range(0f, 150f);

	public PlayerUI ui;

	public SpriteFont fontKA_40;

	public bool active;

	public Vector2 positionTo;

	public SpriteString[] items;

	public int[] values;

	public PlayerScoreManager(PlayerUI pUI)
	{
		ui = pUI;
		Init();
	}

	private void Init()
	{
		items = new SpriteString[8];
		values = new int[8];
		positionTo = new Vector2(ui.stringScore.X + 60f + 107f, ui.stringScore.Y + 101f);
		Load();
	}

	private void Load()
	{
		fontKA_40 = GameEngine.SceneContent.Load<SpriteFont>("Content/Fonts/KA_40");
		for (int i = 0; i < 8; i++)
		{
			values[i] = 0;
			SpriteString spriteString = new SpriteString(ui.spriteManager, fontKA_40, "", 0f);
			spriteString.color = new Color(255, 255, 255, 255);
			spriteString.visible = false;
			items[i] = spriteString;
		}
	}

	public void Update(GameTime elapsed)
	{
		int num = 0;
		if (!active)
		{
			return;
		}
		for (int i = 0; i < 8; i++)
		{
			if (values[i] > 0)
			{
				items[i].position = Vector2.Lerp(items[i].position, positionTo, 0.15f);
				ref Color color = ref items[i].color;
				ref Color color2 = ref items[i].color;
				ref Color color3 = ref items[i].color;
				byte b = (items[i].color.B = (byte)(ALPHA.Ratio(Vector2.Distance(items[i].position, positionTo)) * 255f));
				byte b3 = (color3.G = b);
				byte a = (color2.R = b3);
				color.A = a;
				if (Vector2.Distance(items[i].position, positionTo) < 15f)
				{
					ui.score += values[i];
					items[i].visible = false;
					values[i] = 0;
				}
				num++;
			}
		}
		if (num == 0)
		{
			active = false;
		}
	}

	public void Dispose()
	{
		Resolve();
		fontKA_40 = null;
		active = false;
		for (int i = 0; i < 8; i++)
		{
			if (items[i] != null)
			{
				items[i].Dispose();
				items[i] = null;
			}
		}
		values = null;
		items = null;
		ui = null;
	}

	public void Resolve()
	{
		for (int i = 0; i < 8; i++)
		{
			if (values[i] > 0)
			{
				ui.score += values[i];
				items[i].visible = false;
				values[i] = 0;
			}
		}
	}

	public void Add(int pValue, Vector3 pPos)
	{
		int num = Stack_GetFree();
		values[num] = pValue;
		items[num].SetText(MathUtils.Commas(pValue, 3u));
		ref Color color = ref items[num].color;
		ref Color color2 = ref items[num].color;
		ref Color color3 = ref items[num].color;
		byte b = (items[num].color.B = byte.MaxValue);
		byte b3 = (color3.G = b);
		byte a = (color2.R = b3);
		color.A = a;
		items[num].visible = true;
		items[num].position = MathUtils.Vect3DTo2D(pPos, ui.manager.camera.camera.view, ui.manager.camera.camera.projection, GameEngine.Graphics.GraphicsDevice.Viewport.Width, GameEngine.Graphics.GraphicsDevice.Viewport.Height);
		active = true;
	}

	private int Stack_GetFree()
	{
		int num = -1;
		float num2 = float.MaxValue;
		float num3 = float.MaxValue;
		for (int i = 0; i < 8; i++)
		{
			if (values[i] <= 0)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			for (int i = 0; i < 8; i++)
			{
				num3 = Vector2.Distance(items[i].position, positionTo);
				if (num3 < num2)
				{
					num2 = num3;
					num = i;
				}
			}
		}
		return num;
	}
}
