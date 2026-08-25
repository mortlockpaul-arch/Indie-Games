using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Xna.Framework.GamerServices;
using ZP2K9.hud;

namespace ZP2K9.menu.levels;

public class Main : MenuLevel
{
	private const int ITEM_XBOXLIVE = 0;

	private const int ITEM_PLAYER_SETUP = 1;

	private const int ITEM_PRACTICE = 2;

	private const int ITEM_SETTINGS = 3;

	private const int ITEM_CONTROLS = 4;

	private const int ITEM_QUIT = 5;

	public Main()
	{
		item = new MenuItem[6]
		{
			new MenuItem("Play on Xbox Live", 0),
			new MenuItem("Character Roster", 1),
			new MenuItem("Practice", 2),
			new MenuItem("Settings", 3),
			new MenuItem("Controls", 4),
			new MenuItem("Quit", 5)
		};
		name = new StringBuilder("Main Menu");
		item[1].newBump = 10f;
		width = 200;
		height = 300;
	}

	public override void CheckNewUnlocks()
	{
		item[1].newunlock = false;
		Game1.menu.menuLevel[19].CheckNewUnlocks();
		for (int i = 0; i < 9; i++)
		{
			if (Game1.menu.menuLevel[19].item[i].newunlock)
			{
				item[1].newunlock = true;
			}
		}
		base.CheckNewUnlocks();
	}

	public override void Update(InterfaceKeys iKeys, Menu menu)
	{
		if (active)
		{
			bool flag = false;
			bool flag2 = false;
			if (!menu.infoBox.active)
			{
				menu.InitInfoBox();
			}
			for (int i = 0; i < ((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers).Count; i++)
			{
				flag2 = true;
				if (((ReadOnlyCollection<SignedInGamer>)(object)Gamer.SignedInGamers)[i].IsSignedInToLive)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				item[0].disabled = true;
				item[0].locked = true;
			}
			else
			{
				item[0].disabled = false;
				item[0].locked = false;
			}
			if (!flag2)
			{
				item[2].disabled = true;
				item[1].disabled = true;
				item[3].disabled = true;
				item[2].locked = true;
				item[1].locked = true;
				item[3].locked = true;
			}
			else
			{
				item[2].disabled = false;
				item[1].disabled = false;
				item[3].disabled = false;
				item[2].locked = false;
				item[1].locked = false;
				item[3].locked = false;
			}
			if (Guide.IsTrialMode)
			{
				item[0].disabled = true;
				item[0].locked = true;
			}
			CheckNewUnlocks();
		}
		base.Update(iKeys, menu);
	}

	public override void SelectItem(Menu menu)
	{
		switch (selected)
		{
		case 2:
			Game1.netSession.netType = 2;
			active = false;
			menu.menuLevel[11].active = true;
			break;
		case 0:
			active = false;
			menu.menuLevel[6].active = true;
			break;
		case 1:
		{
			active = false;
			menu.menuLevel[19].active = true;
			for (int i = 0; i < 8; i++)
			{
				menu.menuLevel[19].item[i].text = new StringBuilder(Game1.zProfile.ClassSet(i).name);
			}
			if (Game1.zProfile.clanTag != null)
			{
				menu.menuLevel[19].item[8].text = new StringBuilder("Clan Tag: [" + Game1.zProfile.clanTag.ToString() + "]");
			}
			else
			{
				menu.menuLevel[19].item[8].text = new StringBuilder("Clan Tag");
			}
			break;
		}
		case 3:
			active = false;
			menu.menuLevel[16].active = true;
			menu.menuLevel[16].item[1].selX = (Game1.settings.vibration ? 1 : 0);
			menu.menuLevel[16].item[0].selX = (Game1.settings.showNames ? 1 : 0);
			menu.menuLevel[16].item[2].selX = (Game1.settings.autoSwitch ? 1 : 0);
			menu.menuLevel[16].item[3].selX = (Game1.settings.upToJetpack ? 1 : 0);
			menu.menuLevel[16].item[4].selX = (Game1.settings.twinStickShooter ? 1 : 0);
			menu.menuLevel[16].item[5].selX = Game1.settings.sfx;
			menu.menuLevel[16].item[6].selX = Game1.settings.bgm;
			break;
		case 4:
			active = false;
			menu.menuLevel[14].active = true;
			break;
		case 5:
			active = false;
			menu.menuLevel[1].active = true;
			break;
		}
	}

	public override void Cancel(Menu menu)
	{
		active = false;
		menu.menuLevel[13].active = true;
		Game1.mainPlayerIndex = -1;
	}
}
