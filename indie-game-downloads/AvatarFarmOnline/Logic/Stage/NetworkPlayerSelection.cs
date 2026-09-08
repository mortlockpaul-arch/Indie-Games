using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Quasar.GameUtils.Game;
using Quasar.GameUtils.Network;
using Quasar.GameUtils.Player;
using Quasar.GameUtils.Tasks;
using Quasar.GameUtils.XBLIG.Avatar;
using Quasar.GameUtils.XBLIG.Network;
using Quasar.Textures;

namespace AvatarFarmOnline.Logic.Stage;

internal class NetworkPlayerSelection : AvatarFarmOnline.Logic.Stage.PlayerSelection
{
	private Texture2D texture;

	private INetworkGamer gamer;

	public override AvatarDescription Avatar
	{
		get
		{
			bool isRandom;
			return AvatarUtils.GetAvatarDescription((XBLIGNetworkGamer)gamer, out isRandom);
		}
	}

	public override Texture2D Texture => texture;

	public INetworkGamer NetworkGamer => gamer;

	public override IGamer Gamer => gamer.Gamer;

	public override string PlayerName => gamer.Gamer.Gamertag;

	public NetworkPlayerSelection(INetworkGamer gamer)
		: base(PlayerType.Network, gamer.Id)
	{
		this.gamer = gamer;
		texture = TextureManager.Textures.DefaultItem;
		TaskManager.Post(LoadGamerTexture, null);
	}

	private void LoadGamerTexture(object parameters)
	{
		Texture2D playerPicture = Player.GetPlayerPicture(gamer.Gamer);
		texture = playerPicture;
	}
}
