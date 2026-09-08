using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Quasar.GUI;
using Quasar.GameUtils.Game;
using Quasar.Global;

namespace Quasar.GameUtils.XBLIG.Game;

public class XBLIGPlatformInterface : PlatformInterface
{
	private static XBLIGPlatformInterface stubInstance;

	public new static XBLIGPlatformInterface Instance => stubInstance;

	public override bool IsTrial => false;

	public override bool IsGuideVisible => Guide.IsVisible;

	public override bool HasMessaging => false;

	public override bool SupportsFriends => false;

	public override bool SupportsInGameUnlock => false;

	public override bool SupportsUnlock => false;

	public static void Init(XNAGame game)
	{
		if (stubInstance == null)
		{
			stubInstance = new XBLIGPlatformInterface();
			PlatformInterface.instance = stubInstance;
			SignedInGamer signedInGamer = new SignedInGamer(PlayerIndex.One, CompatXboxProfile.Gamertag);
			signedInGamer.IsGuest = false;
			signedInGamer.IsSignedInToLive = true;
			Gamer.SignedInGamers.SetPlayer(signedInGamer);
			XBLIGSignedInGamer xBLIGSignedInGamer = new XBLIGSignedInGamer(signedInGamer, allowOnlineSessions: false, isSignedInToService: true);
			stubInstance.gamers[0] = xBLIGSignedInGamer;
			stubInstance.signedGamers.Add(xBLIGSignedInGamer);
		}
	}

	public override void Update()
	{
	}

	public override void ShowSignIn(int paneCount, bool onlineOnly)
	{
	}

	public override void ShowGamerCard(PlayerIndex who, IGamer gamer)
	{
	}

	public override void TryBuy(PlayerIndex who, Layout layout)
	{
	}

	public override bool TryBuy(PlayerIndex who)
	{
		return false;
	}

	public override void ShowTextInput(PlayerIndex who, string title, string message, string defaultText, TextInputResult handler, Layout layout)
	{
		handler?.Invoke(isValid: true, who, defaultText ?? string.Empty);
	}

	public override bool CanSendMessages(PlayerIndex who)
	{
		return false;
	}

	public override bool SendToFriends(PlayerIndex who, string message)
	{
		return false;
	}
}
