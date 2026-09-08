using AvatarFarmOnline.Logic.Stage;

namespace AvatarFarmOnline.Logic.Mode.Farm;

internal abstract class FarmHeader
{
	protected int level;

	protected int cash;

	protected int coins;

	protected PlayMode playMode;

	protected AvatarFarmOnline.Logic.PlayerPermissions friendPermissions;

	protected AvatarFarmOnline.Logic.PlayerPermissions publicPermissions;

	public int Level => level;

	public int Cash => cash;

	public int Coins => coins;

	public PlayMode PlayMode => playMode;

	public bool IsOnline
	{
		get
		{
			if (playMode != PlayMode.Private)
			{
				return playMode == PlayMode.Public;
			}
			return true;
		}
	}

	public AvatarFarmOnline.Logic.PlayerPermissions FriendPermissions => friendPermissions;

	public AvatarFarmOnline.Logic.PlayerPermissions PublicPermissions => publicPermissions;

	public void SetPermissions(PlayMode playMode, AvatarFarmOnline.Logic.PlayerPermissions friendPermissions, AvatarFarmOnline.Logic.PlayerPermissions publicPermissions)
	{
		this.playMode = playMode;
		this.friendPermissions = friendPermissions;
		this.publicPermissions = publicPermissions;
	}

	public abstract AvatarFarmOnline.Logic.Stage.FarmData LoadFarm();
}
