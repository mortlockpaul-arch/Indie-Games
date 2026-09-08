using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

namespace Microsoft.Xna.Framework.GamerServices;

public sealed class SignedInGamer : Gamer
{
	private GamerAction statStoreAction;

	private GamerAction statReceiveAction;

	public GameDefaults GameDefaults { get; private set; }

	public bool IsGuest { get; private set; }

	public bool IsSignedInToLive { get; private set; }

	public int PartySize { get; set; }

	public PlayerIndex PlayerIndex { get; private set; }

	public GamerPresence Presence { get; private set; }

	public GamerPrivileges Privileges { get; private set; }

	public static event EventHandler<SignedInEventArgs> SignedIn;

	public static event EventHandler<SignedOutEventArgs> SignedOut;

	internal SignedInGamer(string gamertag, bool isSignedInToLive = false, bool isGuest = false, PlayerIndex playerIndex = PlayerIndex.One)
		: base(gamertag, gamertag)
	{
		IsGuest = isGuest;
		IsSignedInToLive = isSignedInToLive;
		PlayerIndex = playerIndex;
		GameDefaults = new GameDefaults();
		Presence = new GamerPresence();
		Privileges = new GamerPrivileges();
		PartySize = 1;
	}

	public bool IsFriend(Gamer gamer)
	{
		return false;
	}

	public bool IsHeadset(Microphone microphone)
	{
		return microphone.IsHeadset;
	}

	public FriendCollection GetFriends()
	{
		List<FriendGamer> friends = new List<FriendGamer>();
		return new FriendCollection(friends);
	}

	public void AwardAchievement(string achievementKey)
	{
	}

	public IAsyncResult BeginAwardAchievement(string achievementKey, AsyncCallback callback, object state)
	{
		if (statStoreAction != null)
		{
		}
		statStoreAction = new GamerAction(state, callback);
		statStoreAction.IsCompleted = true;
		return statStoreAction;
	}

	public void EndAwardAchievement(IAsyncResult result)
	{
		statStoreAction = null;
	}

	public AchievementCollection GetAchievements()
	{
		IAsyncResult asyncResult = BeginGetAchievements(null, null);
		while (!asyncResult.IsCompleted)
		{
			if (!GamerServicesDispatcher.UpdateAsync())
			{
				statReceiveAction.IsCompleted = true;
			}
		}
		return EndGetAchievements(asyncResult);
	}

	public IAsyncResult BeginGetAchievements(AsyncCallback callback, object asyncState)
	{
		if (statReceiveAction != null)
		{
			throw new InvalidOperationException();
		}
		statReceiveAction = new GamerAction(asyncState, callback);
		return statReceiveAction;
	}

	public AchievementCollection EndGetAchievements(IAsyncResult result)
	{
		List<Achievement> collection = new List<Achievement>();
		statReceiveAction = null;
		return new AchievementCollection(collection);
	}

	internal static void OnSignIn(SignedInGamer gamer)
	{
		if (SignedIn != null)
		{
			SignedIn(null, new SignedInEventArgs(gamer));
		}
	}

	internal static void OnSignOut(SignedInGamer gamer)
	{
		if (SignedOut != null)
		{
			SignedOut(null, new SignedOutEventArgs(gamer));
		}
	}
}
