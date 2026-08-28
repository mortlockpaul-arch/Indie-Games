using System;
using System.Collections.Generic;
using Zilog;

namespace ZXBox.Snapshot;

public class Z80FileFormat : ISnapshot
{
	public void LoadSnapshot(byte[] snapshotbytes, Z80 cpu)
	{
		List<MemoryBlock> list = new List<MemoryBlock>();
		int num = 0;
		cpu.A = snapshotbytes[0];
		cpu.F = snapshotbytes[1];
		cpu.C = snapshotbytes[2];
		cpu.B = snapshotbytes[3];
		cpu.L = snapshotbytes[4];
		cpu.H = snapshotbytes[5];
		cpu.PC = (snapshotbytes[7] << 8) | snapshotbytes[6];
		cpu.SP = (snapshotbytes[9] << 8) | snapshotbytes[8];
		cpu.I = snapshotbytes[10];
		cpu.R = snapshotbytes[11];
		int num2 = snapshotbytes[12];
		if (num2 == 255)
		{
			num2 = 1;
		}
		cpu.Out(254, (num2 >> 1) & 7, 0);
		if ((num2 & 1) != 0)
		{
			cpu.R7 = 128;
		}
		bool compressed = (num2 & 0x20) != 0;
		cpu.E = snapshotbytes[13];
		cpu.D = snapshotbytes[14];
		cpu.CPrim = snapshotbytes[15];
		cpu.BPrim = snapshotbytes[16];
		cpu.EPrim = snapshotbytes[17];
		cpu.DPrim = snapshotbytes[18];
		cpu.LPrim = snapshotbytes[19];
		cpu.HPrim = snapshotbytes[20];
		cpu.APrim = snapshotbytes[21];
		cpu.FPrim = snapshotbytes[22];
		cpu.IY = snapshotbytes[23] | (snapshotbytes[24] << 8);
		cpu.IX = snapshotbytes[25] | (snapshotbytes[26] << 8);
		cpu.IFF = snapshotbytes[27] != 0;
		cpu.IFF2 = snapshotbytes[28] != 0;
		switch (snapshotbytes[29] & 3)
		{
		case 0:
			cpu.IM = 0;
			break;
		case 1:
			cpu.IM = 1;
			break;
		default:
			cpu.IM = 2;
			break;
		}
		num = 30;
		if (cpu.PC == 0)
		{
			int num3 = snapshotbytes[num++] | (snapshotbytes[num++] << 8);
			cpu.PC = snapshotbytes[32] | (snapshotbytes[33] << 8);
			num += num3;
			while (num < snapshotbytes.Length)
			{
				MemoryBlock memoryBlock = new MemoryBlock();
				int num4 = snapshotbytes[num++] | (snapshotbytes[num++] << 8);
				int memoryBlockNumber = snapshotbytes[num++];
				if (num4 == 65535)
				{
					num4 = 16384;
					compressed = false;
				}
				else
				{
					compressed = true;
				}
				memoryBlock = GetMemoryBlock(snapshotbytes, num, num4, compressed, memoryBlockNumber);
				num += num4;
				list.Add(memoryBlock);
			}
		}
		else
		{
			MemoryBlock memoryBlock2 = GetMemoryBlock(snapshotbytes, 30, snapshotbytes.Length - 30, compressed, -1);
			list.Add(memoryBlock2);
		}
		foreach (MemoryBlock item in list)
		{
			switch (item.MemoryBlockNumber)
			{
			case -1:
				MemoryHandler.LoadBytesintoMemory(item.MemoryData.ToArray(), 16384, cpu);
				break;
			case 0:
				MemoryHandler.LoadBytesintoMemory(item.MemoryData.ToArray(), 0, cpu);
				break;
			case 4:
				MemoryHandler.LoadBytesintoMemory(item.MemoryData.ToArray(), 32768, cpu);
				break;
			case 5:
				MemoryHandler.LoadBytesintoMemory(item.MemoryData.ToArray(), 49152, cpu);
				break;
			case 8:
				MemoryHandler.LoadBytesintoMemory(item.MemoryData.ToArray(), 16384, cpu);
				break;
			}
		}
	}

	public static MemoryBlock GetMemoryBlock(byte[] SnapshotBytes, int StartPosition, int Length, bool Compressed, int MemoryBlockNumber)
	{
		List<byte> list = new List<byte>();
		if (Compressed)
		{
			byte b = 0;
			int num = StartPosition;
			while (num < StartPosition + Length && num < SnapshotBytes.Length)
			{
				b = SnapshotBytes[num++];
				if (b != 237)
				{
					list.Add(b);
					continue;
				}
				b = SnapshotBytes[num++];
				if (b != 237)
				{
					list.Add(237);
					num--;
					continue;
				}
				int num2 = SnapshotBytes[num++];
				b = SnapshotBytes[num++];
				while (num2-- != 0)
				{
					list.Add(b);
				}
			}
		}
		else
		{
			for (int i = StartPosition; i < Length; i++)
			{
				list.Add(SnapshotBytes[i]);
			}
		}
		MemoryBlock memoryBlock = new MemoryBlock();
		memoryBlock.MemoryBlockNumber = MemoryBlockNumber;
		memoryBlock.MemoryData = list;
		if (MemoryBlockNumber != -1 && list.Count > 16384)
		{
			memoryBlock.MemoryData.RemoveRange(16384, 16384 - list.Count);
		}
		return memoryBlock;
	}

	public byte[] SaveSnapshot(Z80 cpu)
	{
		throw new NotImplementedException();
	}
}
