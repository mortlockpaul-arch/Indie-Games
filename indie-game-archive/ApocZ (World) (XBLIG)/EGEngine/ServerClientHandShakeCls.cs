using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class ServerClientHandShakeCls
{
	public static void LocalClientSendRequestToServerr(LocalNetworkGamer gamer, ePacketTypes e)
	{
		_ = EGENetWorkNext.packetWriter;
		if (e != ePacketTypes.DamageData)
		{
			_ = 139;
		}
	}

	public static void ServerReadClientRquest(LocalNetworkGamer server, NetworkGamer client)
	{
		_ = EGENetWorkNext.packetReader;
	}

	public static void ServerRespondToClientRequest(LocalNetworkGamer server, NetworkGamer client)
	{
		_ = EGENetWorkNext.packetWriter;
	}

	public static void ClientsRecieveServerRespond(LocalNetworkGamer gamer, NetworkGamer server)
	{
		_ = EGENetWorkNext.packetReader;
	}
}
