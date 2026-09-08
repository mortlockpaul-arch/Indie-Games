using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace StarbeamDefenderGame;

public class GamerServicesManager
{
	private GamerServicesComponent gamer;

	private string[] playernames;

	public bool IsTrialMode => Guide.IsTrialMode;

	public string[] PlayerNames => playernames;

	public bool EnterTrialMode
	{
		set
		{
			if (value)
			{
				Guide.SimulateTrialMode = true;
			}
			else
			{
				Guide.SimulateTrialMode = false;
			}
		}
	}

	public bool IsGuideVisible => Guide.IsVisible;

	public GamerServicesManager(StarbeamDefender game)
	{
		playernames = new string[4];
		for (int i = 0; i < playernames.Length; i++)
		{
			playernames[i] = "Player " + (i + 1);
		}
		gamer = new GamerServicesComponent(game);
		gamer.Initialize();
		game.Components.Add(gamer);
		SignedInGamer.SignedIn += NewGamer;
		SignedInGamer.SignedOut += DropGamer;
	}

	public void ShowPurchase(PlayerIndex player)
	{
		string[] buttons = new string[1] { "Ok" };
		if (Guide.IsVisible)
		{
			return;
		}
		for (int i = 0; i < Gamer.SignedInGamers.Count; i++)
		{
			if (Gamer.SignedInGamers[i].PlayerIndex == player)
			{
				if (Gamer.SignedInGamers[i].IsSignedInToLive)
				{
					if (Gamer.SignedInGamers[i].Privileges.AllowPurchaseContent)
					{
						try
						{
							Guide.ShowMarketplace(player);
							break;
						}
						catch
						{
							break;
						}
					}
					try
					{
						Guide.BeginShowMessageBox("Account Problem", "This user is not permitted to purchase online content", buttons, 0, MessageBoxIcon.Error, MessageClosed, null);
						break;
					}
					catch
					{
						break;
					}
				}
				try
				{
					Guide.BeginShowMessageBox("Account Problem", "The user must be signed into Xbox Live to purchase this game", buttons, 0, MessageBoxIcon.Error, MessageClosed, null);
					break;
				}
				catch
				{
					break;
				}
			}
			Guide.BeginShowMessageBox("Account Problem", "The user must be signed into Xbox Live to purchase this game", buttons, 0, MessageBoxIcon.Error, MessageClosed, null);
		}
	}

	private void MessageClosed(IAsyncResult result)
	{
		Guide.EndShowMessageBox(result);
	}

	private void NewGamer(object Sender, SignedInEventArgs NewPlayer)
	{
		playernames[(int)NewPlayer.Gamer.PlayerIndex] = NewPlayer.Gamer.Gamertag;
	}

	private void DropGamer(object Sender, SignedOutEventArgs OldPlayer)
	{
		playernames[(int)OldPlayer.Gamer.PlayerIndex] = "Player " + (int)(OldPlayer.Gamer.PlayerIndex + 1);
	}
}
