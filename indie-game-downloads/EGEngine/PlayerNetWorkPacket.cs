using System.IO;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class PlayerNetWorkPacket
{
	public const float NetworkUpdateTimeStep = 1f;

	private static HalfVector4 readPak0;

	private static HalfVector4 readPak1;

	private static NormalizedByte4 readPak2;

	private static NormalizedByte4 readPak3;

	private static Vector4 readUnpacker0;

	private static Vector4 readUnpacker1;

	private static Vector3 Angles;

	private static Vector3 position;

	private static Vector3 direction;

	private static Vector3 movedirection;

	public static void WriteLocalGamer(PacketWriter pWriter, LocalNetworkGamer gamer)
	{
		PlayerBase playerBase = gamer.Tag as PlayerBase;
		playerBase.NetworkUpdateTimer++;
		if (playerBase.NetworkUpdateTimer > 2f)
		{
			playerBase.NetworkUpdateTimer = 0f;
			((BinaryWriter)pWriter).Write((byte)101);
			pWriter.Write(playerBase.vecPosition);
		}
		((BinaryWriter)pWriter).Write((byte)100);
		((BinaryWriter)pWriter).Write((byte)playerBase.fpsWeapon.CurrentWeapon.WepType);
		((BinaryWriter)pWriter).Write((byte)playerBase.PlayerFlags);
		((BinaryWriter)pWriter).Write((sbyte)playerBase.tmpMergeAnim);
		((BinaryWriter)pWriter).Write((sbyte)playerBase.cPlayer.CurrentAnimation);
		((BinaryWriter)pWriter).Write((byte)(playerBase.IsAttached0 ? 1u : 0u));
		int num = (int)(playerBase.AnimBlend * 255f);
		num = ((num < 255) ? num : 255);
		((BinaryWriter)pWriter).Write((byte)num);
		((BinaryWriter)pWriter).Write((sbyte)playerBase.Speed);
		((BinaryWriter)pWriter).Write((sbyte)playerBase.SideStep);
		((BinaryWriter)pWriter).Write((byte)(playerBase.Angles.X * 0.7083f));
		((BinaryWriter)pWriter).Write((sbyte)playerBase.Angles.Y);
		int num2 = (int)playerBase.BloodLevel;
		((BinaryWriter)pWriter).Write((byte)num2);
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
				((BinaryWriter)pWriter).Write((byte)101);
				((BinaryWriter)pWriter).Write(gamer.Id);
				pWriter.Write(playerBase.vecPosition);
				localNetworkGamer.SendData(pWriter, SendDataOptions.InOrder);
			}
			((BinaryWriter)pWriter).Write((byte)100);
			((BinaryWriter)pWriter).Write(gamer.Id);
			((BinaryWriter)pWriter).Write((byte)playerBase.fpsWeapon.CurrentWeapon.WepType);
			((BinaryWriter)pWriter).Write((byte)playerBase.PlayerFlags);
			((BinaryWriter)pWriter).Write((sbyte)playerBase.tmpMergeAnim);
			((BinaryWriter)pWriter).Write((sbyte)playerBase.cPlayer.CurrentAnimation);
			((BinaryWriter)pWriter).Write((byte)(playerBase.IsAttached0 ? 1u : 0u));
			int num = (int)(playerBase.AnimBlend * 255f);
			num = ((num < 255) ? num : 255);
			((BinaryWriter)pWriter).Write((byte)num);
			((BinaryWriter)pWriter).Write((sbyte)playerBase.Speed);
			((BinaryWriter)pWriter).Write((sbyte)playerBase.SideStep);
			((BinaryWriter)pWriter).Write((byte)(playerBase.Angles.X * 0.7083f));
			((BinaryWriter)pWriter).Write((sbyte)playerBase.Angles.Y);
			playerBase.PlayerFlags &= (FPS_NET_FLAGS)(-5);
			playerBase.PlayerFlags &= (FPS_NET_FLAGS)(-9);
			localNetworkGamer.SendData(pWriter, SendDataOptions.InOrder);
		}
		else
		{
			((BinaryWriter)pWriter).Write((byte)100);
			((BinaryWriter)pWriter).Write(gamer.Id);
			((BinaryWriter)pWriter).Write((byte)playerBase.fpsWeapon.CurrentWeapon.WepType);
			((BinaryWriter)pWriter).Write((byte)playerBase.PlayerFlags);
			((BinaryWriter)pWriter).Write((sbyte)playerBase.tmpMergeAnim);
			((BinaryWriter)pWriter).Write((sbyte)playerBase.cPlayer.CurrentAnimation);
			((BinaryWriter)pWriter).Write((byte)(playerBase.IsAttached0 ? 1u : 0u));
			int num2 = (int)(playerBase.AnimBlend * 255f);
			num2 = ((num2 < 255) ? num2 : 255);
			((BinaryWriter)pWriter).Write((byte)num2);
			((BinaryWriter)pWriter).Write((sbyte)playerBase.Speed);
			((BinaryWriter)pWriter).Write((sbyte)playerBase.SideStep);
			((BinaryWriter)pWriter).Write((byte)(playerBase.Angles.X * 0.7083f));
			((BinaryWriter)pWriter).Write((sbyte)playerBase.Angles.Y);
			localNetworkGamer.SendData(pWriter, SendDataOptions.InOrder);
		}
		playerBase.tmpMergeAnim = WeaponAnim.Invalid;
	}

	public static void ServerReadClientGamer(PacketReader pReader, NetworkGamer sender)
	{
		PlayerBase playerBase = sender.Tag as PlayerBase;
		playerBase.currentWeaponType = (WeaponType)((BinaryReader)pReader).ReadByte();
		playerBase.PlayerFlags = (FPS_NET_FLAGS)((BinaryReader)pReader).ReadByte();
		playerBase.ServerFlags = playerBase.PlayerFlags;
		playerBase.Spawned = (playerBase.PlayerFlags & FPS_NET_FLAGS.Spawned) > FPS_NET_FLAGS.Clear;
		WeaponAnim weaponAnim = (WeaponAnim)((BinaryReader)pReader).ReadSByte();
		WeaponAnim weaponAnim2 = (WeaponAnim)((BinaryReader)pReader).ReadSByte();
		playerBase.IsAttached0 = ((BinaryReader)pReader).ReadByte() > 0;
		playerBase.AnimBlend = (float)(int)((BinaryReader)pReader).ReadByte() * 0.003921569f;
		playerBase.Speed = ((BinaryReader)pReader).ReadSByte();
		playerBase.SideStep = ((BinaryReader)pReader).ReadSByte();
		playerBase.vecTargetAngles.X = (float)(int)((BinaryReader)pReader).ReadByte() * 1.4117f;
		playerBase.vecTargetAngles.Y = ((BinaryReader)pReader).ReadSByte();
		playerBase.vecTargetAngles.Z = 0f;
		playerBase.BloodLevel = (int)((BinaryReader)pReader).ReadByte();
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
		byte gameId = ((BinaryReader)pReader).ReadByte();
		WeaponType currentWeaponType = (WeaponType)((BinaryReader)pReader).ReadByte();
		FPS_NET_FLAGS playerFlags = (FPS_NET_FLAGS)((BinaryReader)pReader).ReadByte();
		WeaponAnim weaponAnim = (WeaponAnim)((BinaryReader)pReader).ReadSByte();
		WeaponAnim weaponAnim2 = (WeaponAnim)((BinaryReader)pReader).ReadSByte();
		bool isAttached = ((BinaryReader)pReader).ReadByte() > 0;
		float animBlend = (float)(int)((BinaryReader)pReader).ReadByte() * 0.003921569f;
		float speed = ((BinaryReader)pReader).ReadSByte();
		float sideStep = ((BinaryReader)pReader).ReadSByte();
		Angles.X = (float)(int)((BinaryReader)pReader).ReadByte() * 1.4117f;
		Angles.Y = ((BinaryReader)pReader).ReadSByte();
		Angles.Z = 0f;
		NetworkGamer networkGamer = EGENetWorkNext.networkSession.FindGamerById(gameId);
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

	static PlayerNetWorkPacket()
	{
		readPak0 = default(HalfVector4);
		readPak1 = default(HalfVector4);
		readPak2 = default(NormalizedByte4);
		readPak3 = default(NormalizedByte4);
		readUnpacker0 = Vector4.Zero;
		readUnpacker1 = Vector4.Zero;
		Angles = Vector3.Zero;
		position = Vector3.Zero;
		direction = Vector3.Zero;
		movedirection = Vector3.Zero;
	}
}
