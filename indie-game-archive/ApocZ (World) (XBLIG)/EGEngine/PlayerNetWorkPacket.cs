using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class PlayerNetWorkPacket
{
	public const float NetworkUpdateTimeStep = 1f;

	private static HalfVector4 readPak0 = default(HalfVector4);

	private static HalfVector4 readPak1 = default(HalfVector4);

	private static NormalizedByte4 readPak2 = default(NormalizedByte4);

	private static NormalizedByte4 readPak3 = default(NormalizedByte4);

	private static Vector4 readUnpacker0 = Vector4.Zero;

	private static Vector4 readUnpacker1 = Vector4.Zero;

	private static Vector3 Angles = Vector3.Zero;

	private static Vector3 position = Vector3.Zero;

	private static Vector3 direction = Vector3.Zero;

	private static Vector3 movedirection = Vector3.Zero;

	public static void WriteLocalGamer(PacketWriter pWriter, LocalNetworkGamer gamer)
	{
		PlayerBase playerBase = gamer.Tag as PlayerBase;
		playerBase.NetworkUpdateTimer++;
		if (playerBase.NetworkUpdateTimer > 2f)
		{
			playerBase.NetworkUpdateTimer = 0f;
			pWriter.Write((byte)101);
			pWriter.Write(playerBase.vecPosition);
		}
		pWriter.Write((byte)100);
		pWriter.Write((byte)playerBase.fpsWeapon.CurrentWeapon.WepType);
		pWriter.Write((byte)playerBase.PlayerFlags);
		pWriter.Write((sbyte)playerBase.tmpMergeAnim);
		pWriter.Write((sbyte)playerBase.cPlayer.CurrentAnimation);
		pWriter.Write((byte)(playerBase.IsAttached0 ? 1u : 0u));
		int num = (int)(playerBase.AnimBlend * 255f);
		num = ((num < 255) ? num : 255);
		pWriter.Write((byte)num);
		pWriter.Write((sbyte)playerBase.Speed);
		pWriter.Write((sbyte)playerBase.SideStep);
		pWriter.Write((byte)(playerBase.Angles.X * 0.7083f));
		pWriter.Write((sbyte)playerBase.Angles.Y);
		int num2 = (int)playerBase.BloodLevel;
		pWriter.Write((byte)num2);
		playerBase.tmpMergeAnim = WeaponAnim.Invalid;
		playerBase.numFramesSinceLastUpdate = 0;
		playerBase.PlayerFlags &= (FPS_NET_FLAGS)(-5);
		playerBase.PlayerFlags &= (FPS_NET_FLAGS)(-9);
	}

	public static void WriteNetworkGamer(PacketWriter pWriter, NetworkGamer gamer)
	{
		PlayerBase playerBase = gamer.Tag as PlayerBase;
		LocalNetworkGamer localNetworkGamer = (LocalNetworkGamer)gamer.Session.Host;
		if (gamer.IsHost)
		{
			playerBase.NetworkUpdateTimer++;
			if (playerBase.NetworkUpdateTimer > 2f)
			{
				playerBase.NetworkUpdateTimer = 0f;
				pWriter.Write((byte)101);
				pWriter.Write(gamer.Id);
				pWriter.Write(playerBase.vecPosition);
				localNetworkGamer.SendData(pWriter, SendDataOptions.InOrder);
			}
			pWriter.Write((byte)100);
			pWriter.Write(gamer.Id);
			pWriter.Write((byte)playerBase.fpsWeapon.CurrentWeapon.WepType);
			pWriter.Write((byte)playerBase.PlayerFlags);
			pWriter.Write((sbyte)playerBase.tmpMergeAnim);
			pWriter.Write((sbyte)playerBase.cPlayer.CurrentAnimation);
			pWriter.Write((byte)(playerBase.IsAttached0 ? 1u : 0u));
			int num = (int)(playerBase.AnimBlend * 255f);
			num = ((num < 255) ? num : 255);
			pWriter.Write((byte)num);
			pWriter.Write((sbyte)playerBase.Speed);
			pWriter.Write((sbyte)playerBase.SideStep);
			pWriter.Write((byte)(playerBase.Angles.X * 0.7083f));
			pWriter.Write((sbyte)playerBase.Angles.Y);
			playerBase.PlayerFlags &= (FPS_NET_FLAGS)(-5);
			playerBase.PlayerFlags &= (FPS_NET_FLAGS)(-9);
			localNetworkGamer.SendData(pWriter, SendDataOptions.InOrder);
		}
		else
		{
			pWriter.Write((byte)100);
			pWriter.Write(gamer.Id);
			pWriter.Write((byte)playerBase.fpsWeapon.CurrentWeapon.WepType);
			pWriter.Write((byte)playerBase.PlayerFlags);
			pWriter.Write((sbyte)playerBase.tmpMergeAnim);
			pWriter.Write((sbyte)playerBase.cPlayer.CurrentAnimation);
			pWriter.Write((byte)(playerBase.IsAttached0 ? 1u : 0u));
			int num2 = (int)(playerBase.AnimBlend * 255f);
			num2 = ((num2 < 255) ? num2 : 255);
			pWriter.Write((byte)num2);
			pWriter.Write((sbyte)playerBase.Speed);
			pWriter.Write((sbyte)playerBase.SideStep);
			pWriter.Write((byte)(playerBase.Angles.X * 0.7083f));
			pWriter.Write((sbyte)playerBase.Angles.Y);
			localNetworkGamer.SendData(pWriter, SendDataOptions.InOrder);
		}
		playerBase.tmpMergeAnim = WeaponAnim.Invalid;
	}

	public static void ServerReadClientGamer(PacketReader pReader, NetworkGamer sender)
	{
		PlayerBase playerBase = sender.Tag as PlayerBase;
		playerBase.currentWeaponType = (WeaponType)pReader.ReadByte();
		playerBase.PlayerFlags = (FPS_NET_FLAGS)pReader.ReadByte();
		playerBase.ServerFlags = playerBase.PlayerFlags;
		playerBase.Spawned = (playerBase.PlayerFlags & FPS_NET_FLAGS.Spawned) > FPS_NET_FLAGS.Clear;
		WeaponAnim weaponAnim = (WeaponAnim)pReader.ReadSByte();
		WeaponAnim weaponAnim2 = (WeaponAnim)pReader.ReadSByte();
		playerBase.IsAttached0 = pReader.ReadByte() > 0;
		playerBase.AnimBlend = (float)(int)pReader.ReadByte() * 0.003921569f;
		playerBase.Speed = pReader.ReadSByte();
		playerBase.SideStep = pReader.ReadSByte();
		playerBase.vecTargetAngles.X = (float)(int)pReader.ReadByte() * 1.4117f;
		playerBase.vecTargetAngles.Y = pReader.ReadSByte();
		playerBase.vecTargetAngles.Z = 0f;
		playerBase.BloodLevel = (int)pReader.ReadByte();
		playerBase.fpsWeapon.SetWeapon(playerBase.currentWeaponType);
		if (weaponAnim2 != WeaponAnim.Invalid)
		{
			playerBase.cPlayer.PlayAnimation(weaponAnim2, force: true);
		}
		if (weaponAnim != WeaponAnim.Invalid)
		{
			playerBase.cPlayer.PlayMergedAnimation(weaponAnim, EndGameEngine.FIXED_TIME_STEP + (int)(0.5f * (float)EndGameEngine.FIXED_TIME_STEP));
		}
		playerBase.tmpMergeAnim = WeaponAnim.Invalid;
		playerBase.TargetFrameCounter = 0;
	}

	public static void ClientReadFromServer(PacketReader pReader, NetworkGamer sender)
	{
		byte gamerId = pReader.ReadByte();
		WeaponType currentWeaponType = (WeaponType)pReader.ReadByte();
		FPS_NET_FLAGS playerFlags = (FPS_NET_FLAGS)pReader.ReadByte();
		WeaponAnim weaponAnim = (WeaponAnim)pReader.ReadSByte();
		WeaponAnim weaponAnim2 = (WeaponAnim)pReader.ReadSByte();
		bool isAttached = pReader.ReadByte() > 0;
		float animBlend = (float)(int)pReader.ReadByte() * 0.003921569f;
		float speed = pReader.ReadSByte();
		float sideStep = pReader.ReadSByte();
		Angles.X = (float)(int)pReader.ReadByte() * 1.4117f;
		Angles.Y = pReader.ReadSByte();
		Angles.Z = 0f;
		NetworkGamer networkGamer = EGENetWorkNext.networkSession.FindGamerById(gamerId);
		if (networkGamer != null && !networkGamer.IsLocal)
		{
			PlayerBase playerBase = networkGamer.Tag as PlayerBase;
			playerBase.currentWeaponType = currentWeaponType;
			playerBase.PlayerFlags = playerFlags;
			playerBase.Spawned = (playerBase.PlayerFlags & FPS_NET_FLAGS.Spawned) > FPS_NET_FLAGS.Clear;
			playerBase.IsAttached0 = isAttached;
			playerBase.AnimBlend = animBlend;
			playerBase.Speed = speed;
			playerBase.SideStep = sideStep;
			playerBase.vecTargetAngles = Angles;
			playerBase.fpsWeapon.SetWeapon(playerBase.currentWeaponType);
			if (weaponAnim2 != WeaponAnim.Invalid)
			{
				playerBase.cPlayer.PlayAnimation(weaponAnim2, force: false);
			}
			if (weaponAnim != WeaponAnim.Invalid)
			{
				playerBase.cPlayer.PlayMergedAnimation(weaponAnim, EndGameEngine.FIXED_TIME_STEP + (int)(0.5f * (float)EndGameEngine.FIXED_TIME_STEP));
			}
			playerBase.tmpMergeAnim = WeaponAnim.Invalid;
			playerBase.TargetFrameCounter = 0;
		}
	}
}
