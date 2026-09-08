namespace Quasar.GameUtils.Network;

public interface IAvailableSession
{
	string HostGamertag { get; }

	int CurrentGamerCount { get; }

	int OpenPrivateGamerSlots { get; }

	int OpenPublicGamerSlots { get; }

	int AverageRoundtripTime { get; }

	bool IsAverageRoundtripTimeAvailable { get; }

	ISessionProperties SessionProperties { get; }
}
