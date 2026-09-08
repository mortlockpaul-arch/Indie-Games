using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Quasar.GameUtils.Game;

namespace AvatarFarmOnline.Logic.Stage;

internal abstract class PlayerSelection
{
	public enum PlayerType
	{
		Dummy,
		Local,
		Network
	}

	private PlayerType type;

	private short id;

	public PlayerType Type => type;

	public abstract string PlayerName { get; }

	public abstract AvatarDescription Avatar { get; }

	public abstract IGamer Gamer { get; }

	public abstract Texture2D Texture { get; }

	public short Id => id;

	protected PlayerSelection(PlayerType type, short id)
	{
		this.type = type;
		this.id = id;
	}
}
