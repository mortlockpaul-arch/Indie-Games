using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace SpaceBlast;

internal abstract class LocalPlayer : Player
{
	protected float m_LastFrontShotFired;

	protected float m_LastRearShotFired;

	public LocalPlayer(byte playerid, Vector3 pos, Gamer gamer, ShipColor colour, ETeam team)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(playerid, pos, gamer, colour, team);
	}

	protected override void Reset(bool newGame)
	{
		m_LastFrontShotFired = 0f;
		m_LastRearShotFired = 0f;
		base.Reset(newGame);
	}

	public override void Update()
	{
		if (!IsActive && TimeManager.TotalSeconds > RespawnTime)
		{
			Respawn();
		}
		base.Update();
	}

	public override void Die(Player killedBy)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		MainGame.AudioMan.Play(Sound.Explosion, TheShip.Position);
		if (MainGame.NetMan.IsNetworkGame)
		{
			sbyte b = -1;
			if (killedBy != null)
			{
				b = (sbyte)killedBy.PlayerID;
			}
			MainGame.NetMan.SendShipDestroyedPacket(base.PlayerID, (byte)b);
		}
		base.Die(killedBy);
	}

	public void Respawn()
	{
		RespawnLocation playerRespawnPosition = MainGame.LevelData.GetPlayerRespawnPosition(base.PlayerID);
		base.Respawn(playerRespawnPosition);
		if (MainGame.NetMan.IsNetworkGame)
		{
			MainGame.NetMan.SendPlayerRespawnedPacket(base.PlayerID, playerRespawnPosition);
		}
	}
}
