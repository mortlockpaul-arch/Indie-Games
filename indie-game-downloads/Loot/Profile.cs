using System;
using Eyehook.Framework;
using Loot.Awardments;
using Microsoft.Xna.Framework.Storage;

namespace Loot;

public static class Profile
{
	public static Preferences Preferences;

	public static AwardmentProfile Awardments;

	public static string GlobalContainer => "Cursed Loot Shared Data";

	public static string Container => "Cursed Loot - " + Gamertag;

	private static string Gamertag => "Default";

	public static void SetPreferences(Preferences p)
	{
		Preferences = p;
	}

	public static void Load()
	{
		try
		{
			using StorageContainer container = MC.StorageManager.OpenContainer(Container);
			Preferences = new Preferences(container);
			Awardments = new AwardmentProfile(container);
		}
		catch (Exception)
		{
			throw new ResetIOException();
		}
	}
}
