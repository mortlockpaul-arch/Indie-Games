using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Quasar.GameUtils.Game;

public interface ISignedInGamer : IGamer
{
	PlayerIndex PlayerIndex { get; }

	bool AllowOnlineSessions { get; }

	bool IsSignedInToService { get; }

	bool IsGuest { get; }

	void SetPresence(GamerPresenceMode presenceMode);

	void SetPresence(GamerPresenceMode presenceMode, int presenceValue);

	IEnumerable<IFriendGamer> GetFriends();

	bool IsFriend(string gamertag);
}
