using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OluXNA;

internal class CreditComponent : DrawableGameComponent
{
	public EnemyQueue eq;

	public bool firstTimePlaying;

	public float scale = HUD.textScale;

	public SpriteFont spFont;

	public int value;

	public int scoreSlot;

	public CreditComponent(Game game, int _value, int _scoreSlot)
		: base(game)
	{
		eq = new EnemyQueue(0f);
		firstTimePlaying = BaseGame.Get().curUserData.levelsCleared < 4;
		spFont = BaseGame.Get().hud.BigHUDfont;
		value = _value;
		scoreSlot = _scoreSlot;
	}

	public override void Initialize()
	{
		FormatString("Developed and Produced\nby\n\n\nDaniel Frandsen\n\n\n\n", 0.8f * (float)BaseGame.WIDTH, 0f, 12f);
		FormatString("Level Designer\nAudio Director\nArt Director\n\n\nDaniel Frandsen\n\n\n", 0.8f * (float)BaseGame.WIDTH, 12f, 12f);
		FormatString("CG Artists\n\n\nDaniel Frandsen\nJesse Thompson\n\n\n\n", 0.8f * (float)BaseGame.WIDTH, 12f, 12f);
		FormatString("Audio Composers\n\n\nDaniel Frandsen\nGordon van Gent\n\n\nAudio Engineers\n\n\nDaniel Frandsen\nRobert Hutchison", 0.8f * (float)BaseGame.WIDTH, 12f, 12f);
		FormatString("\"Addicted to Toads\"\nMusic by\nDaniel Frandsen\n\n\"Breathless\"\nMusic by\nDaniel Frandsen", 0.8f * (float)BaseGame.WIDTH, 12f, 12f);
		FormatString("\"Hall of Giants\"\nMusic by\nDaniel Frandsen\n\n\"Voodoo MK2\"\nMusic by\nGordon van Gent", 0.8f * (float)BaseGame.WIDTH, 12f, 12f);
		FormatString("Advisors\n\n\nKarlis Kaugars\nKevin Abbott\nRobert Trenary\n\n", 0.8f * (float)BaseGame.WIDTH, 12f, 12f);
		FormatString("Testers\n\nKeith Furry\nBrenna Halpin\nJustin Hostetler\nSasson Jamshidi\nAlex Shafer\nSimon Tower\nXNA Creators Club Members", 0.8f * (float)BaseGame.WIDTH, 12f, 12f);
		FormatString("Thanks for playing!", 0.8f * (float)BaseGame.WIDTH, 12f, 18f);
		eq.Start();
		BaseGame.Get().PlayCue("voodoo");
		((DrawableGameComponent)this).Initialize();
	}

	public void FormatString(string strText, float width)
	{
		FormatString(strText, width, 8f);
	}

	public void FormatString(string strText, float width, float baseTime)
	{
		FormatString(strText, width, baseTime, 6f);
	}

	public void FormatString(string strText, float width, float baseTime, float lengthTime)
	{
		bool flag = true;
		strText = BaseGame.WrapString(strText, width, scale, spFont);
		string[] array = strText.Split('\n');
		int num = 0;
		int num2 = array.Length - 1;
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text != null && text != "")
			{
				string text2 = "";
				for (int j = 0; j < num; j++)
				{
					text2 += "\n";
				}
				text2 += text;
				for (int k = 0; k < num2; k++)
				{
					text2 += "\n";
				}
				if (flag)
				{
					eq.Push(new EnemyQueuePart(new TextDisplay(text2, lengthTime, _requireButton: false, 0.15f, center: true), baseTime));
				}
				else
				{
					eq.Push(new EnemyQueuePart(new TextDisplay(text2, lengthTime, _requireButton: false, 0.15f, center: true), 0f));
				}
				flag = false;
			}
			num++;
			num2--;
		}
	}

	public override void Update(GameTime gameTime)
	{
		BaseGame.Get().UpdateTotalTime(gameTime);
		eq.Update(gameTime);
		BaseGame.Get().input.Update();
		BaseGame.Get().CheckAndResetRumble();
		for (int num = BaseGame.Get().tdColl.tDisplay.Count - 1; num >= 0; num--)
		{
			BaseGame.Get().tdColl.tDisplay[num].act(gameTime);
		}
		while (eq.enemyReady())
		{
			eq.Peek().start();
			eq.Popoff();
		}
		if ((BaseGame.Get().tdColl.tDisplay.Count == 0 && eq.EnemCount() == 0) || (!firstTimePlaying && (BaseGame.Get().input.PadPressed((Buttons)4096) || BaseGame.Get().input.PadPressed((Buttons)16))))
		{
			BaseGame.Get().StopAndClearAllCues();
			if (BaseGame.Get().EasyMode || value == 0 || BaseGame.Get().continueWithoutSaving)
			{
				((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components).Add((IGameComponent)(object)new MainMenuComponent(BaseGame.Get().CoreGame));
			}
			else
			{
				((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components).Add((IGameComponent)(object)new EndLevelComponent(BaseGame.Get().CoreGame, value, scoreSlot));
			}
			((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components).Remove((IGameComponent)(object)this);
		}
		((GameComponent)this).Update(gameTime);
	}

	public override void Draw(GameTime gameTime)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Get().graphics.GraphicsDevice.Clear(Color.Black);
		BaseGame.Get().GraphicsSettings();
		BaseGame.Get().tdColl.Draw(gameTime);
		((DrawableGameComponent)this).Draw(gameTime);
	}
}
