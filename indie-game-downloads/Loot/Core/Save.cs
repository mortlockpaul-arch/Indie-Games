using System;
using Eyehook.Framework;
using Loot.Dungeon;
using Loot.Maps;
using Microsoft.Xna.Framework.Storage;

namespace Loot.Core;

public static class Save
{
	private const int mapDeleteDepth = 24;

	public static void DeleteGame()
	{
		try
		{
			using StorageContainer container = MC.StorageManager.OpenContainer(Profile.Container);
			DM.DeleteGame(container);
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}

	public static void NewGame()
	{
		try
		{
			using (StorageContainer container = MC.StorageManager.OpenContainer(Profile.Container))
			{
				DM.DeleteGame(container);
				DM.SaveGame(container);
				DM.SaveMap(container);
			}
			if (!Graveyard.Updated)
			{
				return;
			}
			using StorageContainer container2 = MC.StorageManager.OpenContainer(Profile.GlobalContainer);
			Graveyard.Save(container2);
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}

	public static void ExitLevel()
	{
		try
		{
			using StorageContainer container = MC.StorageManager.OpenContainer(Profile.Container);
			Profile.Awardments.Save(container);
			DM.SaveMap(container);
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}

	public static void EnterLevel(bool generated)
	{
		try
		{
			using (StorageContainer container = MC.StorageManager.OpenContainer(Profile.Container))
			{
				DM.SaveGame(container);
				if (generated)
				{
					DM.SaveMap(container);
				}
				if (DM.Player.Depth > 24)
				{
					Map.Delete(container, DM.Player.Depth - 24);
				}
			}
			if (!Graveyard.Updated)
			{
				return;
			}
			using StorageContainer container2 = MC.StorageManager.OpenContainer(Profile.GlobalContainer);
			Graveyard.Save(container2);
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}

	public static void QuitGame()
	{
		try
		{
			using StorageContainer container = MC.StorageManager.OpenContainer(Profile.Container);
			Profile.Awardments.Save(container);
			DM.SaveGame(container);
			DM.SaveMap(container);
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}

	public static void GameOver()
	{
		try
		{
			using (StorageContainer container = MC.StorageManager.OpenContainer(Profile.GlobalContainer))
			{
				HighScores.Save(container);
				if (Graveyard.Updated)
				{
					Graveyard.Save(container);
				}
			}
			using StorageContainer container2 = MC.StorageManager.OpenContainer(Profile.Container);
			Profile.Awardments.Save(container2);
			DM.DeleteGame(container2);
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}

	public static void Preferences()
	{
		try
		{
			using StorageContainer container = MC.StorageManager.OpenContainer(Profile.Container);
			Profile.Preferences.Save(container);
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}
}
