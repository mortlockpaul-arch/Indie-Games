using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OluXNA;

internal class LoopEnemy : Enemy
{
	public List<XmlNode> nodeList;

	public ConditionSet csFail;

	public ConditionSet csSucceed;

	public string mode;

	public TextDisplay hintText;

	public string messageText;

	public Vector2 buttonStart;

	public Vector2 buttonEnd;

	public string button;

	public Buttons buttonChoice;

	public LoopEnemy()
	{
		nodeList = new List<XmlNode>();
		csFail = new ConditionSet();
		csSucceed = new ConditionSet();
		messageText = "";
		button = "";
	}

	public LoopEnemy(Dictionary<string, string> attributes, XmlNode node)
		: this()
	{
		foreach (XmlNode childNode in node.SelectSingleNode("enems").ChildNodes)
		{
			nodeList.Add(childNode);
		}
		mode = attributes["mode"];
	}

	private void SetupRightTrigger()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		buttonChoice = (Buttons)4194304;
		SetupButton(0.8f * (float)BaseGame.WIDTH);
	}

	private void SetupButton(float xPos)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		button = BaseGame.Get().hud.KeyMap[buttonChoice];
		buttonStart = (buttonEnd = new Vector2(xPos, 0.5f * (float)BaseGame.HEIGHT));
		buttonStart -= BaseGame.Get().hud.BigHUDfont.MeasureString(button) / 2f;
		buttonEnd += buttonEnd - buttonStart;
	}

	private void SetupLeftTrigger()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		buttonChoice = (Buttons)8388608;
		SetupButton(0.2f * (float)BaseGame.WIDTH);
	}

	public void SetupHit8()
	{
		csSucceed.set.Add(new Hit8Condition());
		csFail.set.Add(new OneEnemCondition());
		messageText = "Target all 8 enemies, then release the trigger";
	}

	public void SetupHitOpposite()
	{
		csSucceed.set.Add(new HitCorrectCondition());
		csFail.set.Add(new OneEnemCondition());
		messageText = "Target enemies with the opposite weapon";
	}

	public void SetupMega()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		csSucceed.set.Add(new UseMegaCondition());
		csFail.set.Add(new OneEnemCondition());
		messageText = "Fire the MEGA drive with the left bumper";
		buttonChoice = (Buttons)256;
		SetupButton(0.3f * (float)BaseGame.WIDTH);
	}

	public void SetupFreeze()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		csSucceed.set.Add(new UseFreezeCondition());
		csFail.set.Add(new OneEnemCondition());
		messageText = "Fire the FREEZE drive with the right bumper";
		buttonChoice = (Buttons)512;
		SetupButton(0.7f * (float)BaseGame.WIDTH);
	}

	public override void draw(GameTime gametime)
	{
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		if (button != "")
		{
			BaseGame.Get().spriteBatch.Begin((SpriteBlendMode)1, (SpriteSortMode)0, (SaveStateMode)0);
			BaseGame.Get().spriteBatch.DrawString(BaseGame.Get().hud.ControllerFont, button, buttonStart, BaseGame.Get().input.PadDown(buttonChoice) ? Color.LightGreen : Color.White, 0f, Vector2.Zero, HUD.textScale, (SpriteEffects)0, 0f);
			BaseGame.Get().spriteBatch.End();
			BaseGame.Get().GraphicsSettings();
		}
	}

	public override void act(GameTime gametime)
	{
		csSucceed.Update();
		csFail.Update();
		if (csFail.ConditionsMet())
		{
			if (csSucceed.ConditionsMet())
			{
				BaseGame.Get().PlayCue("Success", 0f);
				leave();
			}
			else
			{
				BaseGame.Get().PlayCue("Fail", 0f);
				SpawnEnems();
			}
		}
	}

	public override void start()
	{
		base.start();
		BaseGame.Get().actualEnem--;
		switch (mode)
		{
		case "hit8":
			SetupHit8();
			break;
		case "hitopp":
			SetupHitOpposite();
			break;
		case "hitoppw":
			SetupHitOpposite();
			SetupRightTrigger();
			break;
		case "hitopps":
			SetupHitOpposite();
			SetupLeftTrigger();
			break;
		case "mega":
			SetupMega();
			break;
		case "freeze":
			SetupFreeze();
			break;
		}
		if (messageText != "")
		{
			hintText = new TextDisplay(messageText, 8f, _requireButton: false, -0.05f);
			hintText.start();
		}
		SpawnEnems();
	}

	private void SpawnEnems()
	{
		for (int num = nodeList.Count - 1; num >= 0; num--)
		{
			EnemyQueuePart enemyQueuePart = LevelLoader.MakeEnemy(nodeList[num]);
			enemyQueuePart.enem.start();
			BaseGame.Get().enems.Add(enemyQueuePart.enem);
		}
		csFail.Start();
		csSucceed.Start();
	}

	public override Vector3 getPos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Zero;
	}

	public override string name()
	{
		return "[]";
	}

	public override bool Check(int numEnem)
	{
		return false;
	}

	public override void HitSound(int lockNum, float volume)
	{
	}

	public override void die()
	{
		BaseGame.Get().actualEnem++;
		hintText.die();
		base.die();
	}

	public override void leave()
	{
		BaseGame.Get().actualEnem++;
		hintText.leave();
		base.leave();
	}
}
