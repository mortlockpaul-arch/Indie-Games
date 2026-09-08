using AvatarFarmOnline.Logic.Mode.Farm;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.XBLIG.Avatar;

namespace AvatarFarmOnline.Logic.Stage;

internal class LocalPlayerSelection : AvatarFarmOnline.Logic.Stage.PlayerSelection
{
	private PlayerIndex playerIndex;

	private Texture2D texture;

	private AvatarFarmOnline.Logic.Mode.Farm.PlayerExperience playerExperience;

	public PlayerIndex PlayerIndex => playerIndex;

	public override AvatarDescription Avatar => AvatarUtils.GetAvatarDescription(playerIndex);

	public override IGamer Gamer => PlatformInterface.Instance.GetGamer(playerIndex);

	public override Texture2D Texture => texture;

	public AvatarFarmOnline.Logic.Mode.Farm.PlayerExperience PlayerExperience => playerExperience;

	public override string PlayerName => Player.GetPlayerName(playerIndex);

	public LocalPlayerSelection(PlayerIndex pi, AvatarFarmOnline.Logic.Mode.Farm.PlayerExperience playerExperience, short id)
		: base(PlayerType.Local, id)
	{
		this.playerExperience = playerExperience;
		playerIndex = pi;
		texture = Player.GetPlayerPicture(pi);
	}
}
