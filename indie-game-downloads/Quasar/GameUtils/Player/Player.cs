using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.GameUtils.Game;
using Quasar.Global;
using Quasar.Input;
using Quasar.Language;
using Quasar.Textures;

namespace Quasar.GameUtils.Player;

public static class Player
{
	public const string DEFAULT_NAME = "PLAYER_GUEST";

	public const string DEFAULT_PLAYER_TEXTURE = "HUD/Guest_Avatar";

	public const string DEFAULT_PLAYER_TEXTURE_0 = "HUD/Guest_Avatar0";

	public static string GuestName => LanguageManager.Texts["PLAYER_GUEST"];

	public static Texture2D GetPlayerPicture(PlayerIndex player)
	{
		ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(player);
		if (gamer == null)
		{
			TextureManager textures = TextureManager.Textures;
			int num = (int)player;
			return textures["HUD/Guest_Avatar" + num];
		}
		string name = "Profiles/" + gamer.Gamertag;
		if (!TextureManager.Textures.TryGetValue(name, out var item))
		{
			try
			{
				item = gamer.GetTexture();
				TextureManager.Textures.Add(name, item);
			}
			catch (Exception)
			{
				TextureManager textures2 = TextureManager.Textures;
				int num2 = (int)player;
				item = textures2["HUD/Guest_Avatar" + num2];
				TextureManager.Textures.Add(name, item);
			}
		}
		return item;
	}

	public static Texture2D GetPlayerPicture(IGamer gamer)
	{
		if (gamer == null)
		{
			return TextureManager.Textures["HUD/Guest_Avatar0"];
		}
		string name = "Profiles/" + gamer.Gamertag;
		if (!TextureManager.Textures.TryGetValue(name, out var item))
		{
			try
			{
				item = gamer.GetTexture();
				TextureManager.Textures.Add(name, item);
			}
			catch (Exception)
			{
				item = TextureManager.Textures["HUD/Guest_Avatar0"];
				TextureManager.Textures.Add(name, item);
			}
		}
		return item;
	}

	public static bool IsSignedIn(string gamertag)
	{
		PlayerIndex playerIndex = PlayerIndex.One;
		return IsSignedIn(gamertag, out playerIndex);
	}

	public static string GetRandomGamertag()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int num = GameMath.Random.Next(6, 12);
		for (int i = 0; i < num; i++)
		{
			stringBuilder.Append((char)GameMath.Random.Next(97, 122));
		}
		return stringBuilder.ToString();
	}

	public static void SetPresence(GamerPresenceMode presenceMode)
	{
		for (int i = 0; i <= 3; i++)
		{
			SetPresence((PlayerIndex)i, presenceMode);
		}
	}

	public static void SetPresence(PlayerIndex pi, GamerPresenceMode presenceMode)
	{
		PlatformInterface.Instance.GetGamer(pi)?.SetPresence(presenceMode);
	}

	public static void SetPresence(PlayerIndex pi, GamerPresenceMode presenceMode, int presenceValue)
	{
		PlatformInterface.Instance.GetGamer(pi)?.SetPresence(presenceMode, presenceValue);
	}

	public static void SetPresence(GamerPresenceMode presenceMode, int presenceValue)
	{
		for (int i = 0; i <= 3; i++)
		{
			SetPresence((PlayerIndex)i, presenceMode, presenceValue);
		}
	}

	public static bool IsSignedIn(string gamertag, out PlayerIndex playerIndex)
	{
		bool flag = gamertag == GuestName;
		for (int i = 0; i < 4; i++)
		{
			if (GetPlayerName((PlayerIndex)i) == gamertag && (!flag || Gamepad.Instance((PlayerIndex)i).Connected))
			{
				playerIndex = (PlayerIndex)i;
				return true;
			}
		}
		playerIndex = PlayerIndex.One;
		return false;
	}

	public static bool IsSignedIn(PlayerIndex i)
	{
		return PlatformInterface.Instance.GetGamer(i) != null;
	}

	public static string GetPlayerName(PlayerIndex player)
	{
		ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(player);
		if (gamer != null)
		{
			return gamer.Gamertag;
		}
		return LanguageManager.Texts["PLAYER_GUEST"];
	}

	public static Vector3 GetPlayerColor(PlayerIndex player)
	{
		return player switch
		{
			PlayerIndex.One => Vector3.UnitX, 
			PlayerIndex.Two => new Vector3(1f, 1f, 0f), 
			PlayerIndex.Three => Vector3.UnitY, 
			PlayerIndex.Four => new Vector3(0f, 0.375f, 1f), 
			_ => Vector3.One, 
		};
	}
}
