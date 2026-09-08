using System;
using System.Collections.Generic;

namespace Microsoft.Xna.Framework.GamerServices;

public static class GamerServicesDispatcher
{
	public static bool IsInitialized { get; private set; }

	public static nint WindowHandle { get; set; }

	public static event EventHandler<EventArgs> InstallingTitleUpdate;

	public static void Initialize(IServiceProvider serviceProvider)
	{
		IsInitialized = true;
		AppDomain.CurrentDomain.ProcessExit += delegate
		{
			IsInitialized = false;
		};
		List<SignedInGamer> list = new List<SignedInGamer>(1);
		list.Add(new SignedInGamer("Stub Gamer", IsInitialized));
		list.Add(new SignedInGamer("Stub Gamer (1)", IsInitialized, isGuest: true, PlayerIndex.Two));
		list.Add(new SignedInGamer("Stub Gamer (2)", IsInitialized, isGuest: true, PlayerIndex.Three));
		list.Add(new SignedInGamer("Stub Gamer (3)", IsInitialized, isGuest: true, PlayerIndex.Four));
		Gamer.SignedInGamers = new SignedInGamerCollection(list);
		foreach (SignedInGamer signedInGamer in Gamer.SignedInGamers)
		{
			SignedInGamer.OnSignIn(signedInGamer);
		}
	}

	public static void Update()
	{
	}

	internal static bool UpdateAsync()
	{
		if (IsInitialized)
		{
			Update();
		}
		return IsInitialized;
	}
}
