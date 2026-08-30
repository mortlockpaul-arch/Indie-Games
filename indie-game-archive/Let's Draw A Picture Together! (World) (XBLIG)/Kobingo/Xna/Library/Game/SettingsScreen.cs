using Kobingo.Xna.Library.Data;
using Microsoft.Xna.Framework.Storage;

namespace Kobingo.Xna.Library.Game;

public class SettingsScreen : MenuScreen
{
	public SettingsScreen(ScreenManager screenManager)
		: base(screenManager, "Settings")
	{
	}

	public override void Show()
	{
		base.Entries.Clear();
		foreach (string name in GameManager.Settings.Names)
		{
			MenuEntry menuEntry = new MenuEntry(name, string.Empty, GameManager.Settings[name].Options);
			for (int i = 0; i < menuEntry.Values.Length; i++)
			{
				if (GameManager.Settings[name].Value == menuEntry.Values[i])
				{
					menuEntry.SelectedIndex = i;
					break;
				}
			}
			base.Entries.Add(menuEntry);
		}
		base.Show();
	}

	public override void Close()
	{
		foreach (MenuEntry entry in base.Entries)
		{
			GameManager.Settings[entry.Text].Value = entry.SelectedValue;
		}
		StorageManager.PerformOperation(delegate(StorageContainer container)
		{
			if (container != null)
			{
				GameManager.Settings.Save(container);
			}
		});
		base.Close();
	}
}
