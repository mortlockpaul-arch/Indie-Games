using Microsoft.Xna.Framework;
using SpaceBlast.AI;
using SpaceBlast.Weapons;

namespace SpaceBlast;

internal class AIPlayer : LocalPlayer
{
	private AIBrain m_Brain;

	public AIPlayer(byte playerid, AISkill skill, Vector3 pos, ShipColor colour, ETeam team)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector(playerid, pos, null, colour, team);
		m_Brain = new AIBrain(this, skill);
	}

	public override void Terminate()
	{
		m_Brain.Terminate();
	}

	public override void Update()
	{
		m_Brain.Think();
		TheShip.UpdateAIShip();
		base.Update();
	}

	public override void Respawn(RespawnLocation pos)
	{
		base.Respawn(pos);
		m_Brain.Reset();
	}

	public void AttemptFireFrontWeapons()
	{
		Weapon activeFrontWeapon = TheShip.Weapons.ActiveFrontWeapon;
		if (activeFrontWeapon.Ammo > 0 && TimeManager.TotalSeconds >= activeFrontWeapon.EarliestNextShot && !m_PowerCut)
		{
			TheShip.Weapons.FireFrontWeapon();
		}
	}

	public void AttemptFireRearWeapon()
	{
		Weapon activeFrontWeapon = TheShip.Weapons.ActiveFrontWeapon;
		if (activeFrontWeapon.IsRearWeaponFitted && activeFrontWeapon.Ammo > 0 && TimeManager.TotalSeconds >= activeFrontWeapon.EarliestNextShot && !m_PowerCut)
		{
			TheShip.Weapons.FireRearWeapon();
		}
	}
}
