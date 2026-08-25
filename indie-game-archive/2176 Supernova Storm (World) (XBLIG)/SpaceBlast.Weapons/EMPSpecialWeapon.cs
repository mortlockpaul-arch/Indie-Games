using Microsoft.Xna.Framework;

namespace SpaceBlast.Weapons;

internal class EMPSpecialWeapon : SpecialWeapon
{
	public EMPSpecialWeapon(WeaponSystem system, Player player)
		: base(system, player)
	{
		EMPRound.SetupStatics();
	}

	public override void FireWeapon()
	{
		m_Ship.GetWeaponBayPositions(out var frontBay, out var _, out var _, out var _);
		m_WeaponSystem.ActiveRounds.Add(new EMPRound(frontBay));
		MainGame.AudioMan.Play(Sound.EMP, m_Ship.Position);
		if (MainGame.NetMan.IsNetworkGame)
		{
			MainGame.NetMan.SendSpecialWeaponFiredPacket(m_Player.PlayerID, 0.0, SpecialWeaponType.EMP, frontBay);
		}
	}

	public override void FireRemoteWeapon(ref Vector3 startPos, double expires)
	{
		m_WeaponSystem.ActiveRounds.Add(new EMPRound(startPos));
		MainGame.AudioMan.Play(Sound.EMP, startPos);
	}
}
