using Microsoft.Xna.Framework;
using Quasar.GameUtils.Logic.Mode;

namespace AvatarFarmOnline.Logic.Mode.Farm;

internal class FarmGameSetup : SinglePlayerSetup<AvatarFarmOnline.Logic.Mode.GameMode>
{
	public FarmGameSetup(PlayerIndex index)
		: base(AvatarFarmOnline.Logic.Mode.GameMode.Farm, index)
	{
	}

	public override PersistentGameData SetupPersistentData()
	{
		return new AvatarFarmOnline.Logic.Mode.Farm.FarmPersistentGameData(this);
	}
}
