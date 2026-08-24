using System;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Yuki_Win;
using ZP2K9.characters;
using ZP2K9.characters.weapons;
using ZP2K9.debug;
using ZP2K9.hud.messageHud;
using ZP2K9.store;

namespace ZP2K9.hud;

public class HUD
{
	private const float POPUP_SCORE_TIME = 1f;

	private MessageMgr messageMgr;

	public Scoreboard scoreBoard;

	private StringBuilder nilStr = new StringBuilder("-");

	private StringBuilder serverChangingSettingsStr = new StringBuilder("* Head's up! Host is changing settings *");

	private Popup popup;

	private Pickup pickup;

	private float ammoA;

	public float suitDescFrame;

	public int suitDescIdx = -1;

	private int pSuit;

	public int popScoreAdd;

	public float popScoreFrame;

	private StringBuilder popupScoreAddStr;

	public float red;

	private int pickupShowType;

	private int pickupShowCue;

	public StringBuilder deadString;

	private float frame;

	public float serverChangingSettingsFrame;

	private float nullCharCrashFrame;

	public void SetServerChangingSettings()
	{
		serverChangingSettingsFrame = 1f;
	}

	public HUD()
	{
		messageMgr = new MessageMgr();
		scoreBoard = new Scoreboard();
		popup = new Popup();
		pickup = new Pickup();
	}

	public bool IsPopupActive()
	{
		return popup.IsActive();
	}

	public void AddMessage(StringBuilder txt1, StringBuilder txt2, int team1, int team2, int kill)
	{
		messageMgr.AddMessage(txt1, txt2, team1, team2, kill);
	}

	public void AddPopup(string msg, int points, float duration)
	{
		popup.Add(msg, points, this, duration);
	}

	public void AddPopup(string msg, int unlockType, int unlockIdx, int level, float duration)
	{
		popup.Add(msg, unlockType, unlockIdx, level, this, duration);
	}

	public void Update(InterfaceKeys ikeys, Character c)
	{
		if (serverChangingSettingsFrame > 0f)
		{
			serverChangingSettingsFrame -= Game1.frameTime;
		}
		messageMgr.Update();
		scoreBoard.Update(ikeys);
		popup.Update(this);
		pickup.Update();
		if (popScoreFrame > 0f)
		{
			popScoreFrame -= Game1.frameTime * 0.9f;
		}
		pickupShowType = -1;
		frame += Game1.frameTime;
		if (frame > 1f)
		{
			frame--;
		}
		if (c != null)
		{
			if (c.weapon[c.curWeap] > -1)
			{
				try
				{
					int num = WeaponCatalog.weapons[c.weapon[c.curWeap]].maxClip;
					if (num > 1 && c.perk[2] == 7)
					{
						num *= 3;
					}
					float num2 = (float)c.magazine[c.curWeap] / (float)num;
					ammoA += (num2 - ammoA) * Game1.frameTime * 3f;
					if (c.suit != pSuit && c.suit > 0)
					{
						suitDescFrame = 5f;
						suitDescIdx = c.suit;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.StackTrace);
				}
			}
			pSuit = c.suit;
			nullCharCrashFrame = 0f;
		}
		else
		{
			nullCharCrashFrame += Game1.frameTime;
			if (nullCharCrashFrame > 1f)
			{
				nullCharCrashFrame = 0f;
				Game1.netSession.NullCrash();
			}
		}
		if (suitDescFrame > 0f)
		{
			suitDescFrame -= Game1.frameTime;
		}
	}

	public void Draw(Character c, SpriteBatch sprite)
	{
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0458: Unknown result type (might be due to invalid IL or missing references)
		//IL_045d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0462: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_0601: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04be: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_051a: Unknown result type (might be due to invalid IL or missing references)
		//IL_051c: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0548: Unknown result type (might be due to invalid IL or missing references)
		//IL_054a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0558: Unknown result type (might be due to invalid IL or missing references)
		//IL_055d: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0863: Unknown result type (might be due to invalid IL or missing references)
		//IL_0865: Unknown result type (might be due to invalid IL or missing references)
		//IL_0887: Unknown result type (might be due to invalid IL or missing references)
		//IL_089d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_090b: Unknown result type (might be due to invalid IL or missing references)
		//IL_091a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0930: Unknown result type (might be due to invalid IL or missing references)
		//IL_0936: Unknown result type (might be due to invalid IL or missing references)
		//IL_0954: Unknown result type (might be due to invalid IL or missing references)
		//IL_0968: Unknown result type (might be due to invalid IL or missing references)
		//IL_0977: Unknown result type (might be due to invalid IL or missing references)
		//IL_1af1: Unknown result type (might be due to invalid IL or missing references)
		//IL_1afb: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b0a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b14: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b2b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b49: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b5d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b6c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b95: Unknown result type (might be due to invalid IL or missing references)
		//IL_1b9a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1bb3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_071e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0732: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0756: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_077e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0792: Unknown result type (might be due to invalid IL or missing references)
		//IL_07fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1756: Unknown result type (might be due to invalid IL or missing references)
		//IL_1049: Unknown result type (might be due to invalid IL or missing references)
		//IL_104b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0807: Unknown result type (might be due to invalid IL or missing references)
		//IL_081b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1772: Unknown result type (might be due to invalid IL or missing references)
		//IL_1777: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ce1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cfc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d05: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d18: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d27: Unknown result type (might be due to invalid IL or missing references)
		//IL_0d84: Unknown result type (might be due to invalid IL or missing references)
		//IL_0da9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0db3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dbc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0dc2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_184e: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_17c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_179d: Unknown result type (might be due to invalid IL or missing references)
		//IL_17ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_17b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ad9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1870: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_17e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e00: Unknown result type (might be due to invalid IL or missing references)
		//IL_1892: Unknown result type (might be due to invalid IL or missing references)
		//IL_1828: Unknown result type (might be due to invalid IL or missing references)
		//IL_180c: Unknown result type (might be due to invalid IL or missing references)
		//IL_181b: Unknown result type (might be due to invalid IL or missing references)
		//IL_1820: Unknown result type (might be due to invalid IL or missing references)
		//IL_1825: Unknown result type (might be due to invalid IL or missing references)
		//IL_10fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_1110: Unknown result type (might be due to invalid IL or missing references)
		//IL_112e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1142: Unknown result type (might be due to invalid IL or missing references)
		//IL_115d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1179: Unknown result type (might be due to invalid IL or missing references)
		//IL_1197: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_12c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_12cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_12d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e3f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e75: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e9f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ea9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eba: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ed5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0eeb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ef5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f06: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e1e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1910: Unknown result type (might be due to invalid IL or missing references)
		//IL_1922: Unknown result type (might be due to invalid IL or missing references)
		//IL_193d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1942: Unknown result type (might be due to invalid IL or missing references)
		//IL_1956: Unknown result type (might be due to invalid IL or missing references)
		//IL_1965: Unknown result type (might be due to invalid IL or missing references)
		//IL_198f: Unknown result type (might be due to invalid IL or missing references)
		//IL_19a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_19ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_19b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_19cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_19d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_11ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_11e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_1204: Unknown result type (might be due to invalid IL or missing references)
		//IL_1218: Unknown result type (might be due to invalid IL or missing references)
		//IL_1352: Unknown result type (might be due to invalid IL or missing references)
		//IL_1379: Unknown result type (might be due to invalid IL or missing references)
		//IL_137e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a75: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a7f: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a84: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_1aa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_19fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a09: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a13: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a18: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a32: Unknown result type (might be due to invalid IL or missing references)
		//IL_1a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_16cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_16da: Unknown result type (might be due to invalid IL or missing references)
		//IL_16df: Unknown result type (might be due to invalid IL or missing references)
		//IL_16e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_1702: Unknown result type (might be due to invalid IL or missing references)
		//IL_170c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1720: Unknown result type (might be due to invalid IL or missing references)
		//IL_14cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_14dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_14e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_1513: Unknown result type (might be due to invalid IL or missing references)
		//IL_151d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1531: Unknown result type (might be due to invalid IL or missing references)
		//IL_155a: Unknown result type (might be due to invalid IL or missing references)
		//IL_1570: Unknown result type (might be due to invalid IL or missing references)
		//IL_1575: Unknown result type (might be due to invalid IL or missing references)
		//IL_157a: Unknown result type (might be due to invalid IL or missing references)
		//IL_13bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_13cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_13d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_13ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_13f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_140d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_142c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1438: Unknown result type (might be due to invalid IL or missing references)
		//IL_143d: Unknown result type (might be due to invalid IL or missing references)
		//IL_145c: Unknown result type (might be due to invalid IL or missing references)
		//IL_1466: Unknown result type (might be due to invalid IL or missing references)
		//IL_147a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b43: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b45: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b49: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b53: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b58: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b61: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b7c: Unknown result type (might be due to invalid IL or missing references)
		//IL_15bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_15d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_1609: Unknown result type (might be due to invalid IL or missing references)
		//IL_1613: Unknown result type (might be due to invalid IL or missing references)
		//IL_1627: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f4a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f78: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0f93: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fbe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fc3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fdc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0fe6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ff7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		if (c == null || DebugManager.hideHud)
		{
			return;
		}
		bool flag = false;
		if (GameState.gameType == 4 && c.team == 1)
		{
			flag = true;
		}
		if (!Game1.netSession.postLobby && !Game1.menu.IsActive())
		{
			try
			{
				if (Game1.settings.showNames)
				{
					Color color = default(Color);
					for (int i = 0; i < Game1.character.Length; i++)
					{
						if (Game1.character[i] == null || i == c.ID || !(Game1.character[i].nameAlpha > 0f) || i >= scoreBoard.charName.Length || scoreBoard.charName[i] == null)
						{
							continue;
						}
						Vector2 loc = Scroll.GetLoc(Game1.character[i].drawVec);
						if (loc.X > 0f && loc.Y > 0f && loc.X < 1280f && loc.Y < 720f && Game1.character[i].hp >= 0)
						{
							((Color)(ref color))._002Ector(new Vector4(1f, 1f, 1f, Game1.character[i].nameAlpha));
							switch (Game1.character[i].GetTeam())
							{
							case 1:
								((Color)(ref color))._002Ector(new Vector4(0.5f, 0.5f, 1f, Game1.character[i].nameAlpha));
								break;
							case 2:
								((Color)(ref color))._002Ector(new Vector4(1f, 0.5f, 0.5f, Game1.character[i].nameAlpha));
								break;
							}
							if (GameState.gameType == 4 && c.team == 0 && Game1.character[i].team == 1 && c.dyingFrame <= 0f)
							{
								((Color)(ref color))._002Ector(0f, 0f, 0f, 0f);
							}
							Game1.text.color = color;
							Game1.text.size = 1f;
							Game1.text.DrawString(loc, scoreBoard.charName[i], 1, -1f, Game1.impact, sprite);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.StackTrace);
			}
			try
			{
				int num = 0;
				Color val = default(Color);
				for (int j = 0; j < ((ReadOnlyCollection<NetworkGamer>)(object)Game1.netSession.netSession.AllGamers).Count; j++)
				{
					if (!((ReadOnlyCollection<NetworkGamer>)(object)Game1.netSession.netSession.AllGamers)[j].IsTalking)
					{
						continue;
					}
					byte id = ((ReadOnlyCollection<NetworkGamer>)(object)Game1.netSession.netSession.AllGamers)[j].Id;
					if (!Game1.netSession.playerList.ContainsKey(id))
					{
						continue;
					}
					int num2 = Game1.netSession.playerList[id];
					if (scoreBoard.charName[num2] != null && Game1.character[num2] != null)
					{
						((Color)(ref val))._002Ector(new Vector4(1f, 1f, 1f, 1f));
						switch (Game1.character[num2].GetTeam())
						{
						case 1:
							((Color)(ref val))._002Ector(new Vector4(0.5f, 0.5f, 1f, 1f));
							break;
						case 2:
							((Color)(ref val))._002Ector(new Vector4(1f, 0.5f, 0.5f, 1f));
							break;
						}
						float num3 = 800f;
						Game1.text.color = val;
						Game1.text.size = 1f;
						Game1.text.DrawString(new Vector2(num3, 610f - (float)num * 27f), scoreBoard.charName[num2], 0, -1f, Game1.impact, sprite);
						sprite.Draw(Game1.spritesTex, new Vector2(num3 - 30f, 604f - (float)num * 27f), (Rectangle?)new Rectangle(864, 96, 32, 32), val);
						num++;
					}
				}
			}
			catch (Exception ex2)
			{
				Console.WriteLine(ex2.StackTrace);
			}
			Color white = Color.White;
			Vector2 loc2 = Scroll.GetLoc(c.loc - new Vector2(0f, 40f));
			float num4 = ((Vector2)(ref c.charKeys.shootVec)).Length();
			if (num4 > 0.1f)
			{
				Vector2 shootVec = c.charKeys.shootVec;
				if (num4 > 1f)
				{
					num4 = 1f;
				}
				num4 = num4 / 2f + 0.5f;
				((Vector2)(ref shootVec)).Normalize();
				sprite.Draw(Game1.spritesTex, loc2 + shootVec * 220f * num4, (Rectangle?)new Rectangle(224, 32, 32, 32), new Color(new Vector4(0f, 0f, 0f, (num4 - 0.1f) / 2f)), Trig.GetAngle(default(Vector2), shootVec), new Vector2(16f, 16f), 0.3f, (SpriteEffects)1, 1f);
				sprite.Draw(Game1.spritesTex, loc2 + shootVec * 250f * num4, (Rectangle?)new Rectangle(224, 32, 32, 32), new Color(new Vector4(1f, 1f, 1f, (num4 - 0.1f) / 2f)), Trig.GetAngle(default(Vector2), shootVec), new Vector2(16f, 16f), 0.5f, (SpriteEffects)1, 1f);
			}
			switch (c.GetTeam())
			{
			case 1:
				((Color)(ref white))._002Ector(new Vector4(0.5f, 0.5f, 1f, 1f));
				break;
			case 2:
				((Color)(ref white))._002Ector(new Vector4(1f, 0.5f, 0.5f, 1f));
				break;
			}
			DrawPopScore(sprite);
			popup.Draw(sprite);
			pickup.Draw(sprite, white);
			if (c.hp < 0)
			{
				DrawYouDied(sprite);
			}
			if (pickupShowType > -1)
			{
				int num5 = pickupShowType - 1;
				Vector2 val2 = default(Vector2);
				((Vector2)(ref val2))._002Ector(640f, 440f);
				float num6 = ((pickupShowCue == 0) ? 32f : 0f);
				sprite.Draw(Game1.spritesTex, val2 + new Vector2(0f - num6, 0f), (Rectangle?)new Rectangle(672, 192, 32, 32), Color.White, 0f, new Vector2(32f, 16f), 1f, (SpriteEffects)0, 1f);
				if (pickupShowCue == 0)
				{
					sprite.Draw(Game1.spritesTex, val2, (Rectangle?)new Rectangle(672, 224, 96, 64), Color.White, 0f, new Vector2(48f, 32f), 0.6f, (SpriteEffects)0, 1f);
				}
				if (pickupShowCue == 4)
				{
					sprite.Draw(Game1.spritesTex, val2 + new Vector2(num6, 0f), (Rectangle?)new Rectangle(768, 32, 64, 64), Color.White, 0f, new Vector2(0f, 32f), 1f, (SpriteEffects)0, 1f);
				}
				else
				{
					sprite.Draw(Game1.spritesTex, val2 + new Vector2(num6, 0f), (Rectangle?)((pickupShowCue == 1) ? new Rectangle(768, 225, 64, 64) : new Rectangle(num5 % 16 * 64, 320 + num5 / 16 * 64, 64, 64)), Color.White, 0f, new Vector2(0f, 32f), 1f, (SpriteEffects)0, 1f);
				}
			}
			Vector2 val3 = default(Vector2);
			((Vector2)(ref val3))._002Ector(160f, 570f);
			Vector2 val4 = default(Vector2);
			((Vector2)(ref val4))._002Ector(450f, 610f);
			Vector2 val5 = default(Vector2);
			((Vector2)(ref val5))._002Ector(1100f, 610f);
			Color val6 = white;
			((Color)(ref val6)).A = 80;
			Vector2 val7 = default(Vector2);
			((Vector2)(ref val7))._002Ector(1120f, 140f);
			sprite.Draw(Game1.spritesTex, val7, (Rectangle?)new Rectangle(256, 672, 128, 128), val6, 0f, new Vector2(64f, 64f), 1f, (SpriteEffects)0, 1f);
			sprite.Draw(Game1.nullTex, val7, (Rectangle?)new Rectangle(0, 0, 1, 1), new Color(1f, 1f, 1f, 0.2f), 0f, new Vector2(0.5f, 0.5f), new Vector2(113f, 1f), (SpriteEffects)0, 1f);
			sprite.Draw(Game1.nullTex, val7, (Rectangle?)new Rectangle(0, 0, 1, 1), new Color(1f, 1f, 1f, 0.2f), 0f, new Vector2(0.5f, 0.5f), new Vector2(1f, 113f), (SpriteEffects)0, 1f);
			Color val8 = default(Color);
			for (int k = 0; k < Game1.character.Length; k++)
			{
				if (Game1.character[k] == null || Game1.character[k].hp < 0)
				{
					continue;
				}
				float num7 = ((Vector2)(ref Game1.character[k].radarTraj)).LengthSquared() / 80000f;
				if (Game1.character[k].perk[2] == 6)
				{
					num7 = 0f;
				}
				if (GameState.gameType == 4 && c.team == 0 && Game1.character[k].team == 1)
				{
					num7 = 0f;
				}
				if (Game1.character[Game1.netSession.GetPlayerOne()].perk[1] == 2 && num7 < 0.5f)
				{
					num7 = 0.5f;
				}
				if (!(num7 > 0.1f))
				{
					continue;
				}
				num7 -= 0.1f;
				((Color)(ref val8))._002Ector(1f, 1f, 1f, num7);
				if (k != Game1.netSession.GetPlayerOne())
				{
					if (GameState.gameType == 0)
					{
						((Color)(ref val8))._002Ector(1f, 0f, 0f, num7);
					}
					else if (Game1.character[k].team == 0)
					{
						((Color)(ref val8))._002Ector(0f, 0.2f, 1f, num7);
					}
					else
					{
						((Color)(ref val8))._002Ector(1f, 0f, 0f, num7);
					}
				}
				Vector2 val9 = Game1.character[k].loc - Game1.character[Game1.netSession.GetPlayerOne()].loc;
				float num8 = 2000f;
				if (val9.X > 0f - num8 && val9.X < num8 && val9.Y > 0f - num8 && val9.Y < num8 && ((Vector2)(ref val9)).Length() < num8)
				{
					sprite.Draw(Game1.nullTex, val7 + val9 / num8 * 57f, (Rectangle?)new Rectangle(0, 0, 1, 1), val8, 0f, new Vector2(0.5f, 0.5f), 4f, (SpriteEffects)0, 1f);
				}
			}
			if (!flag)
			{
				float num9 = 0.25f;
				int num10 = (int)(c.jetGas / num9);
				Color val10 = default(Color);
				for (int l = 0; l < num10 + 1; l++)
				{
					float num11 = 25f;
					if (l == num10)
					{
						num11 *= c.jetGas / num9 - (float)l;
					}
					float num12 = 0f;
					((Color)(ref val10))._002Ector(1f, 1f, 1f, 0.35f + num12);
					if (l == num10)
					{
						switch (l)
						{
						case 0:
							((Color)(ref val10))._002Ector(1f, 0f, 0f, 0.5f);
							if ((int)(frame * 60f) % 4 == 0)
							{
								((Color)(ref val10))._002Ector(1f, 1f, 1f, 1f);
							}
							break;
						case 1:
							((Color)(ref val10))._002Ector(1f, 1f, 0f, 0.5f);
							if ((int)(frame * 30f) % 4 == 0)
							{
								((Color)(ref val10))._002Ector(1f, 1f, 0.5f, 1f);
							}
							break;
						default:
							((Color)(ref val10))._002Ector(1f, 1f, 1f, 0.9f);
							break;
						}
					}
					sprite.Draw(Game1.nullTex, val4 + new Vector2(-75f + (float)l * 28f, 32f), (Rectangle?)new Rectangle(0, 0, 1, 1), val10, 0f, default(Vector2), new Vector2(num11, 14f), (SpriteEffects)0, 1f);
				}
				for (int m = 0; m < 3; m++)
				{
					int num13 = Game1.character[Game1.netSession.GetPlayerOne()].perk[m];
					sprite.Draw(Game1.perksTex, new Vector2(600f + (float)m * 60f, 585f), (Rectangle?)new Rectangle(768 + m * 128, num13 * 128, 128, 128), val6, 0f, default(Vector2), 0.4f, (SpriteEffects)0, 1f);
				}
				Color val11 = default(Color);
				for (int n = 0; n < 2; n++)
				{
					((Color)(ref val11))._002Ector(new Vector4(0f, 0f, 0f, 0.5f));
					if (n == 1)
					{
						sprite.End();
						sprite.Begin((SpriteBlendMode)2);
						val11 = white;
					}
					sprite.Draw(Game1.spritesTex, val3, (Rectangle?)new Rectangle(128 * n, 576, 128, 128), val11, 0f, new Vector2(64f, 64f), 1.5f, (SpriteEffects)0, 1f);
					sprite.Draw(Game1.spritesTex, val4 + new Vector2(96f, -3f), (Rectangle?)new Rectangle(128 * n, 702, 128, 64), val11, 0f, new Vector2(64f, 32f), 1.5f, (SpriteEffects)0, 1f);
					sprite.Draw(Game1.spritesTex, val4, (Rectangle?)new Rectangle(128 * n, 768, 128, 64), val11, 0f, new Vector2(64f, 32f), 1.5f, (SpriteEffects)0, 1f);
					int num14 = 2;
					if (!Game1.settings.twinStickShooter)
					{
						num14 = 1;
					}
					for (int num15 = 0; num15 < num14; num15++)
					{
						if (c.grenAmmo[num15] > 0)
						{
							sprite.Draw(Game1.spritesTex, val5 + new Vector2(48f, -64f * (float)num15), (Rectangle?)new Rectangle(32 + 128 * n, 702, 64, 64), val11, 0f, new Vector2(32f, 32f), 1.5f, (SpriteEffects)0, 1f);
							sprite.Draw(Game1.spritesTex, val5 + new Vector2(0f, -64f * (float)num15), (Rectangle?)new Rectangle(32 + 128 * n, 702, 64, 64), val11, 0f, new Vector2(32f, 32f), 1.5f, (SpriteEffects)0, 1f);
						}
					}
					if (n == 1)
					{
						sprite.End();
						sprite.Begin((SpriteBlendMode)1);
					}
				}
			}
			if (!flag)
			{
				for (int num16 = 0; num16 < c.weapon.Length; num16++)
				{
					Vector2 val12 = val3;
					switch (num16)
					{
					case 0:
						val12.X -= 48f;
						break;
					case 1:
						val12.X += 48f;
						break;
					case 2:
						val12.Y -= 48f;
						break;
					case 3:
						val12.Y += 48f;
						break;
					}
					if (c.weapon[num16] > -1)
					{
						int num17 = WeaponCatalog.weapons[c.weapon[num16]].imgIdx;
						bool flag2 = false;
						if (num17 >= 64)
						{
							num17 -= 64;
							flag2 = true;
						}
						sprite.Draw(Game1.spritesTex, val12, (Rectangle?)new Rectangle(0, 832, 192, 192), new Color(1f, 1f, 1f, 0.15f), 0f, new Vector2(96f, 96f), 0.4f, (SpriteEffects)0, 1f);
						sprite.Draw(Game1.spritesTex, val12, (Rectangle?)new Rectangle(num17 % 16 * 64, 384 + num17 / 16 * 64, 64, 64), new Color(1f, 1f, 1f, 1f), 0f, new Vector2(32f, 32f), 1f, (SpriteEffects)0, 1f);
						if (flag2)
						{
							sprite.Draw(Game1.spritesTex, val12, (Rectangle?)new Rectangle(num17 % 16 * 64, 384 + num17 / 16 * 64, 64, 64), new Color(1f, 1f, 1f, 1f), 0f, new Vector2(32f, 32f), 1f, (SpriteEffects)1, 1f);
						}
					}
				}
				if (c.weapon[c.curWeap] > -1)
				{
					int num18 = c.ammo[WeaponCatalog.weapons[c.weapon[c.curWeap]].ammoType] + c.magazine[c.curWeap];
					if (WeaponCatalog.weapons[c.weapon[c.curWeap]].ammoType == 0)
					{
						num18 = -1;
					}
					sprite.DrawString(Game1.impact, (num18 == -1) ? nilStr : Numbers.GetNumber(num18), val4 + new Vector2(80f, -10f), white);
					int num19 = WeaponCatalog.weapons[c.weapon[c.curWeap]].maxClip;
					if (num19 > 1 && c.perk[2] == 7)
					{
						num19 *= 3;
					}
					_ = (float)c.magazine[c.curWeap] / (float)num19;
					sprite.Draw(Game1.nullTex, new Rectangle((int)val4.X - 69, (int)val4.Y - 19, (int)(137f * ammoA), 41), new Color(new Vector4(0.85f, 0.85f * ammoA, 1f * ammoA, 0.5f)));
					int num20 = WeaponCatalog.weapons[c.weapon[c.curWeap]].imgIdx;
					bool flag3 = false;
					if (num20 >= 64)
					{
						num20 -= 64;
						flag3 = true;
					}
					sprite.Draw(Game1.spritesTex, val4 - new Vector2(80f, 0f), (Rectangle?)new Rectangle(num20 % 16 * 64, 384 + num20 / 16 * 64, 64, 64), Color.White, 0f, new Vector2(32f, 32f), 1.5f, (SpriteEffects)0, 1f);
					if (flag3)
					{
						sprite.Draw(Game1.spritesTex, val4 - new Vector2(80f, 0f), (Rectangle?)new Rectangle(num20 % 16 * 64, 384 + num20 / 16 * 64, 64, 64), Color.White, 0f, new Vector2(32f, 32f), 1.5f, (SpriteEffects)1, 1f);
					}
				}
				int num21 = 2;
				if (!Game1.settings.twinStickShooter)
				{
					num21 = 1;
				}
				for (int num22 = 0; num22 < num21; num22++)
				{
					if (c.grenType[num22] > -1 && c.grenAmmo[num22] > 0)
					{
						sprite.Draw(Game1.spritesTex, val5 + new Vector2(0f, -64f * (float)num22), (Rectangle?)new Rectangle((c.grenType[num22] - 1) % 16 * 64, 320 + (c.grenType[num22] - 1) / 16 * 64, 64, 64), Color.White, 0f, new Vector2(32f, 32f), 1f, (SpriteEffects)1, 1f);
						sprite.DrawString(Game1.impact, Numbers.GetNumber(c.grenAmmo[num22]), val5 + new Vector2(42f, -64f * (float)num22 - 8f), white);
					}
				}
				for (int num23 = num21; num23 < c.grenType.Length; num23++)
				{
					if (c.grenType[num23] > -1 && c.grenAmmo[num23] > 0)
					{
						sprite.Draw(Game1.spritesTex, val5 + new Vector2(64f, -36f * (float)num23 - 60f), (Rectangle?)new Rectangle((c.grenType[num23] - 1) % 16 * 64, 320 + (c.grenType[num23] - 1) / 16 * 64, 64, 64), Color.White, 0f, new Vector2(32f, 32f), 1f, (SpriteEffects)1, 1f);
					}
				}
			}
			if (c.GetTeam() != 0)
			{
				for (int num24 = 0; num24 < Game1.character.Length; num24++)
				{
					if (num24 != c.ID && Game1.character[num24] != null && Game1.character[num24].GetTeam() == c.GetTeam() && Game1.character[num24].hp >= 0 && Game1.character[num24].spawnFrame <= 0f)
					{
						sprite.Draw(Game1.spritesTex, Scroll.GetLoc(Game1.character[num24].loc - new Vector2(0f, 70f)), (Rectangle?)new Rectangle(672, 32 * Game1.character[num24].GetTeam(), 64, 32), Color.White, 0f, new Vector2(32f, 32f), 0.7f, (SpriteEffects)0, 1f);
					}
				}
			}
			if (GameState.gameType == 2)
			{
				Vector2 gVec = default(Vector2);
				if (Game1.netSession.redFlagState == 200)
				{
					gVec = Game1.gameMap.redFlagHome;
				}
				else if (Game1.character[Game1.netSession.redFlagState] != null)
				{
					gVec = Game1.character[Game1.netSession.redFlagState].loc - new Vector2(0f, 48f);
				}
				DrawPointer(gVec, c, 1, sprite);
				gVec = default(Vector2);
				if (Game1.netSession.blueFlagState == 200)
				{
					gVec = Game1.gameMap.blueFlagHome;
				}
				else if (Game1.character[Game1.netSession.blueFlagState] != null)
				{
					gVec = Game1.character[Game1.netSession.blueFlagState].loc - new Vector2(0f, 48f);
				}
				DrawPointer(gVec, c, 2, sprite);
			}
			else if (GameState.gameType == 3)
			{
				if (Game1.netSession.hillState == 0)
				{
					DrawPointer(Game1.gameMap.hill, c, 0, sprite);
				}
				else if (Game1.netSession.hillState == 1)
				{
					DrawPointer(Game1.gameMap.hill, c, 2, sprite);
				}
				else if (Game1.netSession.hillState == 2)
				{
					DrawPointer(Game1.gameMap.hill, c, 1, sprite);
				}
			}
			if (Game1.netSession.netPlay != null)
			{
				Game1.netSession.netPlay.DrawHud(sprite);
			}
			if (suitDescFrame > 0f)
			{
				int num25 = suitDescIdx - 1;
				if (suitDescIdx == 100)
				{
					num25 = 6;
				}
				float num26 = suitDescFrame / 2f;
				if (num26 > 0.5f)
				{
					num26 = 0.5f;
				}
				sprite.Draw(Game1.spritesTex, new Vector2(640f, 510f), (Rectangle?)new Rectangle(0, 768, 128, 64), new Color(new Vector4(0f, 0f, 0f, num26)), 0f, new Vector2(64f, 32f), new Vector2(5f, 2.2f), (SpriteEffects)0, 1f);
				sprite.DrawString(Game1.impact, SuitManager.suitText[num25 * 2], new Vector2(640f, 500f) - Game1.impact.MeasureString(SuitManager.suitText[num25 * 2]) / 2f, new Color(new Vector4(1f, 1f, 1f, suitDescFrame)));
				if (suitDescIdx == 100)
				{
					sprite.DrawString(Game1.impact, SuitManager.phoenixFix, new Vector2(640f, 520f) - Game1.impact.MeasureString(SuitManager.phoenixFix) / 2f, new Color(new Vector4(1f, 1f, 0f, suitDescFrame)));
				}
				else
				{
					sprite.DrawString(Game1.impact, SuitManager.suitText[num25 * 2 + 1], new Vector2(640f, 520f) - Game1.impact.MeasureString(SuitManager.suitText[num25 * 2 + 1]) / 2f, new Color(new Vector4(1f, 1f, 0f, suitDescFrame)));
				}
			}
		}
		if (scoreBoard.alpha <= 0f)
		{
			messageMgr.Draw(sprite);
		}
		scoreBoard.Draw(sprite);
		if (serverChangingSettingsFrame > 0f)
		{
			Vector2 val13 = new Vector2(1280f, 720f) / 2f + new Vector2(0f, -250f);
			sprite.Draw(Game1.spritesTex, val13, (Rectangle?)new Rectangle(0, 768, 128, 64), new Color(0f, 0f, 0f, 0.75f), 0f, new Vector2(64f, 24f), new Vector2(4.25f, 2f), (SpriteEffects)0, 1f);
			Game1.text.color = new Color(1f, 1f, 1f, 1f);
			Game1.text.size = 1f;
			Game1.text.DrawString(val13, serverChangingSettingsStr, 1, -1f, Game1.impact, sprite);
		}
	}

	private void DrawPopScore(SpriteBatch sprite)
	{
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		if (!(popScoreFrame <= 0f))
		{
			float num = 1f;
			Game1.text.size = 2f;
			if (popScoreFrame < 0.25f)
			{
				Game1.text.size -= (0.25f - popScoreFrame) * 2f;
				num = popScoreFrame * 4f;
			}
			if (popScoreFrame > 0.75f)
			{
				Game1.text.size += (popScoreFrame - 0.75f) * 4f;
				num = (1f - popScoreFrame) * 4f;
			}
			Game1.text.size *= 0.9f;
			Game1.text.color = new Color(1f, 1f, 1f, num);
			Game1.text.DrawString(new Vector2(640f, 150f), popupScoreAddStr, 1, -1f, Game1.impact, sprite);
		}
	}

	private void DrawYouDied(SpriteBatch sprite)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		Game1.text.size = 2f;
		Game1.text.size *= 0.9f;
		Game1.text.color = new Color(1f, 1f, 1f, 1f);
		if (deadString == null)
		{
			deadString = new StringBuilder("You died");
		}
		Game1.text.DrawString(new Vector2(640f, 250f), deadString, 1, -1f, Game1.impact, sprite);
	}

	private void DrawPointer(Vector2 gVec, Character c, int idx, SpriteBatch sprite)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		gVec -= c.loc;
		if (((Vector2)(ref gVec)).Length() < 300f)
		{
			gVec /= 300f;
		}
		else
		{
			((Vector2)(ref gVec)).Normalize();
		}
		sprite.Draw(Game1.spritesTex, Scroll.GetLoc(c.loc - new Vector2(0f, 48f)) + gVec * 300f, (Rectangle?)new Rectangle(672 + idx * 64, 96, 64, 64), new Color(new Vector4(1f, 1f, 1f, 0.8f)), Trig.GetAngle(default(Vector2), gVec), new Vector2(32f, 32f), 0.75f, (SpriteEffects)1, 1f);
	}

	public void AddPopScore(int p)
	{
		if (p % 10 != 0)
		{
			p = p / 10 * 10;
			if (p < 10)
			{
				p = 10;
			}
			Console.WriteLine("Score not a multiple of 10.");
		}
		if (p > 0)
		{
			if (popScoreFrame <= 0f)
			{
				popScoreAdd = p;
			}
			else
			{
				popScoreAdd += p;
			}
			popScoreFrame = 1f;
			int num = popScoreAdd;
			if (Leveling.IsHappyHour(DateTime.Now.TimeOfDay.Hours))
			{
				num *= 2;
			}
			popupScoreAddStr = new StringBuilder("+" + num);
			Sound.PlayCue("chime");
		}
	}

	internal void AddPickup(byte type, int cue)
	{
		pickupShowType = type;
		pickupShowCue = cue;
	}

	internal void SetDead(string p)
	{
		deadString = new StringBuilder(p);
	}

	internal void DoPickup(int weapImgIdx)
	{
		pickup.DoPickup(weapImgIdx);
	}

	internal void DoName(int iidx)
	{
		pickup.DoName(iidx);
	}
}
