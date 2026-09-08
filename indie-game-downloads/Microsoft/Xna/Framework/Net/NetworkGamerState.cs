using System;

namespace Microsoft.Xna.Framework.Net;

[Flags]
internal enum NetworkGamerState
{
	IsHost = 1,
	IsLocal = 2,
	IsPrivateSlot = 4,
	IsReady = 8,
	HasVoice = 0x10,
	IsTalking = 0x20,
	IsMutedByLocalUser = 0x40,
	IsGuest = 0x80,
	HasLeftSession = 0x100
}
