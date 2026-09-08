using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Quasar.GameUtils.Game;

namespace Quasar.GameUtils.XBLIG.Game;

public class XBLIGSignedInGamer : XBLIGGamer, ISignedInGamer, IGamer
{
	private readonly SignedInGamer signedInGamer;

	private readonly bool allowOnlineSessions;

	private readonly bool isSignedInToService;

	public PlayerIndex PlayerIndex => signedInGamer.PlayerIndex;

	public bool AllowOnlineSessions => allowOnlineSessions;

	public bool IsSignedInToService => isSignedInToService;

	public bool IsGuest => signedInGamer.IsGuest;

	public XBLIGSignedInGamer(SignedInGamer gamer, bool allowOnlineSessions, bool isSignedInToService)
		: base(gamer)
	{
		signedInGamer = gamer;
		this.allowOnlineSessions = allowOnlineSessions;
		this.isSignedInToService = isSignedInToService;
	}

	public IEnumerable<IFriendGamer> GetFriends()
	{
		return new IFriendGamer[0];
	}

	public bool IsFriend(string gamertag)
	{
		return false;
	}

	public void SetPresence(GamerPresenceMode presenceMode)
	{
	}

	public void SetPresence(GamerPresenceMode presenceMode, int presenceValue)
	{
	}
}
