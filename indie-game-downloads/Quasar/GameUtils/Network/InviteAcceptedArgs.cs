using Quasar.GameUtils.Game;

namespace Quasar.GameUtils.Network;

public class InviteAcceptedArgs
{
	public ISignedInGamer Gamer { get; private set; }

	public InviteAcceptedArgs(ISignedInGamer gamer)
	{
		Gamer = gamer;
	}
}
