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
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		double num = TimeManager.TotalSeconds + 4.0;
		m_Ship.GetWeaponBayPositions(out var frontBay, out var _, out var _, out var _);
		Vector2 vec = new Vector2
		{
			X = 800f
		};
		Vector3 velocity = default(Vector3);
		for (int i = 0; i < 50; i++)
		{
			Utils.AdjustVector(ref vec, (float)Math.PI / 25f);
			((Vector3)(ref velocity))._002Ector(vec, 0f);
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
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		double num = expires - TimeManager.TotalSeconds;
		float x = (float)(3200.0 / num);
		m_Ship.GetWeaponBayPositions(out var frontBay, out var _, out var _, out var _);
		Vector2 vec = new Vector2
		{
			X = x
		};
		Vector3 velocity = default(Vector3);
		for (int i = 0; i < 50; i++)
		{
			Utils.AdjustVector(ref vec, (float)Math.PI / 25f);
			((Vector3)(ref velocity))._002Ector(vec, 0f);
			m_WeaponSystem.ActiveRounds.Add(new StarburstRound(frontBay, velocity, expires));
		}
		MainGame.AudioMan.Play(Sound.StarBurst, m_Ship.Position);
	}
}
