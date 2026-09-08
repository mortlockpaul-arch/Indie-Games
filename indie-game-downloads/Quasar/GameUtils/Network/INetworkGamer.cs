using System;
using Quasar.GameUtils.Game;

namespace Quasar.GameUtils.Network;

public interface INetworkGamer
{
	Session Session { get; }

	IGamer Gamer { get; }

	bool HasVoice { get; }

	bool IsHost { get; }

	bool IsTalking { get; }

	bool IsLocal { get; }

	byte Id { get; }

	TimeSpan RoundtripTime { get; }

	bool IsDisposed { get; }

	object Tag { get; set; }

	bool IsReady { get; }

	void RemoveFromSession();
}
