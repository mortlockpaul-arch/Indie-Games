using System;

namespace Loot.PC;

public static class PlayerFactory
{
	private static int classType;

	public static DemoPlayer CreateDemoPlayer()
	{
		classType = (classType + 1) % 4;
		return new DemoPlayer(CreatePlayer((PlayerClassType)classType));
	}

	public static Player CreatePlayer(PlayerClassType pc)
	{
		return pc switch
		{
			PlayerClassType.Berserker => new Berserker(), 
			PlayerClassType.Shaman => new Shaman(), 
			PlayerClassType.Tinkerer => new Tinkerer(), 
			PlayerClassType.Gambler => new Gambler(), 
			PlayerClassType.Goblin => new Goblin(), 
			PlayerClassType.Peasant => new Peasant(), 
			_ => throw new Exception("Unknown class: " + pc), 
		};
	}
}
