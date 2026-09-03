using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class GameplayChange : Enemy
{
	public string command;

	private float value;

	private int scoreSlot;

	private float fadeVal;

	private bool playerSaveRequested;

	public GameplayChange()
	{
	}

	public GameplayChange(string _command, float _value)
		: this()
	{
		command = _command;
		value = _value;
		playerSaveRequested = false;
	}

	public GameplayChange(Dictionary<string, string> attributes, XmlNode node)
		: this(attributes.ContainsKey("command") ? attributes["command"] : "", LevelLoader.GetFloatFromAtt(attributes, "value", -1f))
	{
	}

	public override void draw(GameTime gametime)
	{
	}

	public override void act(GameTime gametime)
	{
		if (!exists)
		{
			return;
		}
		if (command == "fade")
		{
			fadeVal -= (float)gametime.ElapsedGameTime.TotalSeconds;
			BaseGame.Get().channels[8] = 1f - fadeVal / value;
			if (fadeVal <= 0f)
			{
				BaseGame.Get().channels[8] = 1f;
				leave();
			}
		}
		else if (command == "unfade")
		{
			fadeVal -= (float)gametime.ElapsedGameTime.TotalSeconds;
			BaseGame.Get().channels[8] = fadeVal / value;
			if (fadeVal <= 0f)
			{
				BaseGame.Get().channels[8] = 0f;
				leave();
			}
		}
		else if (command == "quit")
		{
			if (BaseGame.Get().HSSaved && !BaseGame.Get().PlayerSaved && !playerSaveRequested)
			{
				playerSaveRequested = true;
				BaseGame.Get().PlayerSaved = true;
				BaseGame.Get().EndStageSavePartThree();
			}
			if (!BaseGame.Get().PlayerSaved)
			{
				return;
			}
			for (int num = ((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components).Count - 1; num >= 0; num--)
			{
				if (((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components)[num] is BaseComponent || ((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components)[num] is PauseComponent)
				{
					((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components).RemoveAt(num);
				}
			}
			if (BaseGame.Get().EasyMode || (int)value == 0 || BaseGame.Get().continueWithoutSaving)
			{
				((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components).Add((IGameComponent)(object)new MainMenuComponent(BaseGame.Get().CoreGame, 6));
			}
			else
			{
				((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components).Add((IGameComponent)(object)new EndLevelComponent(BaseGame.Get().CoreGame, (int)value, scoreSlot));
			}
			leave();
		}
		else
		{
			if (!(command == "credits"))
			{
				return;
			}
			if (BaseGame.Get().HSSaved && !BaseGame.Get().PlayerSaved && !playerSaveRequested)
			{
				playerSaveRequested = true;
				BaseGame.Get().PlayerSaved = true;
				BaseGame.Get().EndStageSavePartThree();
			}
			if (!BaseGame.Get().PlayerSaved)
			{
				return;
			}
			for (int num2 = ((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components).Count - 1; num2 >= 0; num2--)
			{
				if (((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components)[num2] is BaseComponent || ((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components)[num2] is PauseComponent)
				{
					((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components).RemoveAt(num2);
				}
			}
			((Collection<IGameComponent>)(object)BaseGame.Get().CoreGame.Components).Add((IGameComponent)(object)new CreditComponent(BaseGame.Get().CoreGame, (int)value, scoreSlot));
			leave();
		}
	}

	public override void start()
	{
		base.start();
		switch (command)
		{
		case "fade":
			BaseGame.Get().channels[8] = 0f;
			fadeVal = value;
			break;
		case "unfade":
			BaseGame.Get().channels[8] = 1f;
			fadeVal = value;
			break;
		case "clearscore":
			BaseGame.Get().score = 0;
			BaseGame.Get().powerAmounts[0] = 0;
			BaseGame.Get().powerAmounts[1] = 0;
			BaseGame.Get().powerScore[0] = 0;
			BaseGame.Get().powerScore[1] = 0;
			leave();
			break;
		case "credits":
			BaseGame.Get().StopAndClearAllCues();
			BaseGame.Get().HSSaved = true;
			BaseGame.Get().PlayerSaved = false;
			scoreSlot = BaseGame.Get().EndStageSavePartOne((int)value);
			BaseGame.Get().EndStageSavePartTwo();
			break;
		case "quit":
			BaseGame.Get().StopAndClearAllCues();
			BaseGame.Get().HSSaved = true;
			BaseGame.Get().PlayerSaved = false;
			scoreSlot = BaseGame.Get().EndStageSavePartOne((int)value);
			BaseGame.Get().EndStageSavePartTwo();
			break;
		case "fillWirePower":
			BaseGame.Get().powerAmounts[0] += 3;
			leave();
			break;
		case "fillSolidPower":
			BaseGame.Get().powerAmounts[1] += 3;
			leave();
			break;
		case "score":
			BaseGame.Get().score += (int)value;
			leave();
			break;
		}
	}
}
