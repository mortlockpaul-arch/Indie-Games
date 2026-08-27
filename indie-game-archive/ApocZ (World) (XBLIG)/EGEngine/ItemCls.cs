using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class ItemCls
{
	public const ushort Invalid = 0;

	public const ushort Consumable = 256;

	public const ushort Weapon = 512;

	public const ushort Equipment = 1024;

	public const ushort Vehicle = 2048;

	public const ushort Reserve00 = 4096;

	public const ushort Reserve01 = 8192;

	public const ushort MultiSlotItem = 16384;

	public const ushort InTent = 32768;

	public const ushort NotConsumable = 65279;

	public const ushort NotWeapon = 65023;

	public const ushort NotEquipment = 64511;

	public const ushort NotVehicle = 63487;

	public const ushort NotReserve00 = 61439;

	public const ushort NotReserve01 = 57343;

	public const ushort NotMultiSlotItem = 49151;

	public const ushort NotInTent = 32767;

	public const ushort Mask = 255;

	public ushort uid;

	public ushort desc;

	public Vector3 pos;

	public byte ownerNetId;

	public byte reserved0;

	public ushort ItemType
	{
		get
		{
			return (ushort)(desc & 0xFF);
		}
		set
		{
		}
	}

	public bool IsConsumable => (desc & 0x100) > 0;

	public bool IsEquipment => (desc & 0x400) > 0;

	public bool IsWeapon => (desc & 0x200) > 0;

	public bool IsVehicle => (desc & 0x800) > 0;

	public bool IsInTent => (desc & 0x8000) > 0;

	public bool IsValid => desc != 0;

	public ItemCls()
	{
	}

	public ItemCls(ItemCls e)
	{
		uid = e.uid;
		desc = e.desc;
		pos = e.pos;
		ownerNetId = e.ownerNetId;
		reserved0 = e.reserved0;
	}

	public ItemCls(byte[] buff, ref int idx)
	{
		BufferRead(buff, ref idx);
	}

	public void NetworkWrite(PacketWriter pw)
	{
		pw.Write(uid);
		pw.Write(desc);
		pw.Write(pos);
		pw.Write(ownerNetId);
		pw.Write(reserved0);
	}

	public void NetworkRead(PacketReader pr)
	{
		uid = pr.ReadUInt16();
		desc = pr.ReadUInt16();
		pos = pr.ReadVector3();
		ownerNetId = pr.ReadByte();
		reserved0 = pr.ReadByte();
	}

	public void StreamWrite(Stream writer)
	{
		int num = (int)pos.X;
		int num2 = (int)pos.Y;
		int num3 = (int)pos.Z;
		writer.WriteByte((byte)(desc & 0xFF));
		writer.WriteByte((byte)(desc >> 8));
		writer.WriteByte((byte)(uid & 0xFF));
		writer.WriteByte((byte)(uid >> 8));
		writer.WriteByte(reserved0);
		writer.WriteByte((byte)(num & 0xFF));
		writer.WriteByte((byte)((num >> 8) & 0xFF));
		writer.WriteByte((byte)((num >> 16) & 0xFF));
		writer.WriteByte((byte)((num >> 24) & 0xFF));
		writer.WriteByte((byte)(num2 & 0xFF));
		writer.WriteByte((byte)((num2 >> 8) & 0xFF));
		writer.WriteByte((byte)((num2 >> 16) & 0xFF));
		writer.WriteByte((byte)((num2 >> 24) & 0xFF));
		writer.WriteByte((byte)(num3 & 0xFF));
		writer.WriteByte((byte)((num3 >> 8) & 0xFF));
		writer.WriteByte((byte)((num3 >> 16) & 0xFF));
		writer.WriteByte((byte)((num3 >> 24) & 0xFF));
	}

	public void StreamRead(Stream reader)
	{
		desc = (ushort)reader.ReadByte();
		desc |= (ushort)((ushort)reader.ReadByte() << 8);
		uid = (ushort)reader.ReadByte();
		uid |= (ushort)((ushort)reader.ReadByte() << 8);
		reserved0 = (byte)reader.ReadByte();
		int num = reader.ReadByte();
		num |= reader.ReadByte() << 8;
		num |= reader.ReadByte() << 16;
		num |= reader.ReadByte() << 24;
		int num2 = reader.ReadByte();
		num2 |= reader.ReadByte() << 8;
		num2 |= reader.ReadByte() << 16;
		num2 |= reader.ReadByte() << 24;
		int num3 = reader.ReadByte();
		num3 |= reader.ReadByte() << 8;
		num3 |= reader.ReadByte() << 16;
		num3 |= reader.ReadByte() << 24;
		pos.X = num;
		pos.Y = num2;
		pos.Z = num3;
	}

	public void BufferWrite(byte[] buff, ref int idx)
	{
		int num = (int)pos.X;
		int num2 = (int)pos.Y;
		int num3 = (int)pos.Z;
		buff[idx++] = (byte)(desc & 0xFF);
		buff[idx++] = (byte)(desc >> 8);
		buff[idx++] = (byte)(uid & 0xFF);
		buff[idx++] = (byte)(uid >> 8);
		buff[idx++] = reserved0;
		buff[idx++] = (byte)(num & 0xFF);
		buff[idx++] = (byte)((num >> 8) & 0xFF);
		buff[idx++] = (byte)((num >> 16) & 0xFF);
		buff[idx++] = (byte)((num >> 24) & 0xFF);
		buff[idx++] = (byte)(num2 & 0xFF);
		buff[idx++] = (byte)((num2 >> 8) & 0xFF);
		buff[idx++] = (byte)((num2 >> 16) & 0xFF);
		buff[idx++] = (byte)((num2 >> 24) & 0xFF);
		buff[idx++] = (byte)(num3 & 0xFF);
		buff[idx++] = (byte)((num3 >> 8) & 0xFF);
		buff[idx++] = (byte)((num3 >> 16) & 0xFF);
		buff[idx++] = (byte)((num3 >> 24) & 0xFF);
	}

	public void BufferRead(byte[] buff, ref int idx)
	{
		desc = buff[idx++];
		desc |= (ushort)(buff[idx++] << 8);
		uid = buff[idx++];
		uid |= (ushort)(buff[idx++] << 8);
		reserved0 = buff[idx++];
		int num = buff[idx++];
		num |= buff[idx++] << 8;
		num |= buff[idx++] << 16;
		num |= buff[idx++] << 24;
		int num2 = buff[idx++];
		num2 |= buff[idx++] << 8;
		num2 |= buff[idx++] << 16;
		num2 |= buff[idx++] << 24;
		int num3 = buff[idx++];
		num3 |= buff[idx++] << 8;
		num3 |= buff[idx++] << 16;
		num3 |= buff[idx++] << 24;
		pos.X = num;
		pos.Y = num2;
		pos.Z = num3;
	}
}
