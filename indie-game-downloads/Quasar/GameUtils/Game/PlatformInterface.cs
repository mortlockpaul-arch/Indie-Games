using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Quasar.GUI;
using Quasar.GameUtils.Awards;
using Quasar.GameUtils.Network;

namespace Quasar.GameUtils.Game;

public abstract class PlatformInterface
{
	public delegate void TextInputResult(bool isValid, PlayerIndex playerIndex, string text);

	protected static PlatformInterface instance;

	public Action<PlayerIndex, ISignedInGamer> SignedIn;

	public Action<PlayerIndex, ISignedInGamer> SignedOut;

	protected List<ISignedInGamer> gamers = new List<ISignedInGamer>(4);

	protected List<ISignedInGamer> signedGamers = new List<ISignedInGamer>(4);

	private ReadOnlyCollection<ISignedInGamer> signedGamersReadonly;

	public static PlatformInterface Instance => instance;

	public ReadOnlyCollection<ISignedInGamer> Gamers => signedGamersReadonly;

	public abstract bool IsTrial { get; }

	public abstract bool IsGuideVisible { get; }

	public abstract bool HasMessaging { get; }

	public abstract bool SupportsFriends { get; }

	public abstract bool SupportsInGameUnlock { get; }

	public abstract bool SupportsUnlock { get; }

	public virtual void WriteGamer(IGamer gamer, IPacketWriter writer)
	{
	}

	public virtual IGamer ReadGamer(IPacketReader reader)
	{
		return null;
	}

	protected PlatformInterface()
	{
		signedGamersReadonly = signedGamers.AsReadOnly();
		for (int i = 0; i < 4; i++)
		{
			gamers.Add(null);
		}
	}

	public virtual void InitAwardManager()
	{
		AwardManager.Init();
	}

	public ISignedInGamer GetGamer(PlayerIndex playerIndex)
	{
		return gamers[(int)playerIndex];
	}

	public abstract void Update();

	public abstract void ShowSignIn(int PaneCount, bool onlineOnly);

	public abstract void ShowGamerCard(PlayerIndex who, IGamer gamer);

	public abstract void TryBuy(PlayerIndex who, Layout layout);

	public abstract void ShowTextInput(PlayerIndex who, string title, string message, string defaultText, TextInputResult handler, Layout layout);

	public abstract bool TryBuy(PlayerIndex who);

	public abstract bool CanSendMessages(PlayerIndex who);

	public abstract bool SendToFriends(PlayerIndex who, string message);
}
