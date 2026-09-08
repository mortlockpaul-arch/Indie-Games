using Microsoft.Xna.Framework;

namespace AvatarFarmOnline.Logic.Stage;

internal class PlayerSpawnData
{
	private AvatarFarmOnline.Logic.Stage.PlayerSelection selection;

	private Vector2 position;

	private int level;

	public AvatarFarmOnline.Logic.Stage.PlayerSelection Selection => selection;

	public Vector2 Position => position;

	public int Level => level;

	public PlayerSpawnData(AvatarFarmOnline.Logic.Stage.PlayerSelection selection, Vector2 position, int level)
	{
		this.selection = selection;
		this.position = position;
		this.level = level;
	}
}
