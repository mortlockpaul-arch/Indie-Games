using System;
using Microsoft.Xna.Framework;

namespace SpaceBlast.Weapons;

internal class StarBurstSpecialWeapon : SpecialWeapon
{
	private const float constBulletSpeed = 800f;

	private const float constBulletLifetime = 4f;

	private const float constFinalRadius = 3200f;

	public StarBurstSpecialWeapon(WeaponSystem system, Player player)
		: base(system, player)
	{
		StarburstRound.SetupStatics();
	}

	public override void FireWeapon()
	{
		double num = TimeManager.TotalSeconds + 4.0;
		m_Ship.GetWeaponBayPositions(out var frontBay, out var _, out var _, out var _);
		Vector2 vec = new Vector2
		{
			X = 800f
		};
		for (int i = 0; i < 50; i++)
		{
			Utils.AdjustVector(ref vec, (float)Math.PI / 25f);
			Vector3 velocity = new Vector3(vec, 0f);
			m_WeaponSystem.ActiveRounds.Add(new StarburstRound(frontBay, velocity, num));
		}
		MainGame.AudioMan.Play(Sound.StarBurst, m_Ship.Position);
		if (MainGame.NetMan.IsNetworkGame)
		{
			MainGame.NetMan.SendSpecialWeaponFiredPacket(m_Player.PlayerID, num, SpecialWeaponType.Starburst, frontBay);
		}
	}

	public override void FireRemoteWeapon(ref Vector3 startPos, double expires)
	{
		double num = expires - TimeManager.TotalSeconds;
		float x = (float)(3200.0 / num);
		m_Ship.GetWeaponBayPositions(out var frontBay, out var _, out var _, out var _);
		Vector2 vec = new Vector2
		{
			X = x
		};
		for (int i = 0; i < 50; i++)
		{
			Utils.AdjustVector(ref vec, (float)Math.PI / 25f);
			Vector3 velocity = new Vector3(vec, 0f);
			m_WeaponSystem.ActiveRounds.Add(new StarburstRound(frontBay, velocity, expires));
		}
		MainGame.AudioMan.Play(Sound.StarBurst, m_Ship.Position);
	}
}
