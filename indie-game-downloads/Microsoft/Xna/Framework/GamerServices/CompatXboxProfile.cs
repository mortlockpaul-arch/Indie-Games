using System;
using System.IO;
using System.Threading;

namespace Microsoft.Xna.Framework.GamerServices;

public static class CompatXboxProfile
{
	private static readonly string RuntimeDataDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserData");

	private static readonly string GamertagPath = Path.Combine(RuntimeDataDirectory, "xbox-gamertag.txt");

	private static readonly string AvatarInfoPath = Path.Combine(RuntimeDataDirectory, "xbox-avatar-info.bin");

	private static readonly string BundledAvatarInfoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AvatarAssets", "avatar-description.bin");

	private static readonly string LegacyBundledAvatarInfoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "avatar-description.bin");

	private static string gamertag;

	private static byte[] avatarInfo;

	private static int loaded;

	public static string Gamertag
	{
		get
		{
			EnsureLoaded();
			return string.IsNullOrEmpty(gamertag) ? "Player1" : gamertag;
		}
	}

	private static void EnsureLoaded()
	{
		if (Interlocked.Exchange(ref loaded, 1) != 0)
		{
			return;
		}
		try
		{
			if (File.Exists(GamertagPath))
			{
				gamertag = File.ReadAllText(GamertagPath).Trim();
			}
		}
		catch
		{
		}
		try
		{
			if (File.Exists(AvatarInfoPath))
			{
				avatarInfo = File.ReadAllBytes(AvatarInfoPath);
			}
			else if (File.Exists(BundledAvatarInfoPath))
			{
				avatarInfo = File.ReadAllBytes(BundledAvatarInfoPath);
			}
			else if (File.Exists(LegacyBundledAvatarInfoPath))
			{
				avatarInfo = File.ReadAllBytes(LegacyBundledAvatarInfoPath);
			}
		}
		catch
		{
		}
	}

	public static AvatarDescription CreateAvatarDescription()
	{
		EnsureLoaded();
		AvatarDescription avatarDescription = new AvatarDescription();
		if (avatarInfo != null && avatarInfo.Length != 0)
		{
			avatarDescription.RawData = avatarInfo;
		}
		return avatarDescription;
	}
}
