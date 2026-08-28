using System.Linq;
using Zilog;

namespace ZXBox.Snapshot;

public class SNAFileFormat : ISnapshot
{
	public void LoadSnapshot(byte[] snapshotbytes, Z80 cpu)
	{
		cpu.Reset();
		cpu.I = snapshotbytes[0];
		cpu.HLPrim = snapshotbytes[1] | (snapshotbytes[2] << 8);
		cpu.DEPrim = snapshotbytes[3] | (snapshotbytes[4] << 8);
		cpu.BCPrim = snapshotbytes[5] | (snapshotbytes[6] << 8);
		cpu.AFPrim = snapshotbytes[7] | (snapshotbytes[8] << 8);
		cpu.HL = snapshotbytes[9] | (snapshotbytes[10] << 8);
		cpu.DE = snapshotbytes[11] | (snapshotbytes[12] << 8);
		cpu.BC = snapshotbytes[13] | (snapshotbytes[14] << 8);
		cpu.IY = snapshotbytes[15] | (snapshotbytes[16] << 8);
		cpu.IX = snapshotbytes[17] | (snapshotbytes[18] << 8);
		cpu.IFF = (cpu.IFF2 = (snapshotbytes[19] & 4) == 4);
		cpu.R = snapshotbytes[20];
		cpu.AF = snapshotbytes[21] | (snapshotbytes[22] << 8);
		cpu.SP = snapshotbytes[23] | (snapshotbytes[24] << 8);
		cpu.IM = (byte)(snapshotbytes[25] & 3);
		if (cpu.IM > 2)
		{
			cpu.IM = 2;
		}
		cpu.Out(254, snapshotbytes[26], 0);
		MemoryHandler.LoadBytesintoMemory(snapshotbytes, 27, 16384, cpu);
		int pC = cpu.ReadWordFromMemory(cpu.SP);
		cpu.PC = pC;
		cpu.RET(condition: true, 0, 0);
	}

	public byte[] SaveSnapshot(Z80 cpu)
	{
		byte[] array = new byte[49179];
		ushort num = (ushort)(cpu.SP - 2);
		array[0] = (byte)cpu.I;
		array[1] = (byte)(cpu.HLPrim & 0xFF);
		array[2] = (byte)(cpu.HLPrim >> 8);
		array[3] = (byte)(cpu.DEPrim & 0xFF);
		array[4] = (byte)(cpu.DEPrim >> 8);
		array[5] = (byte)(cpu.BCPrim & 0xFF);
		array[6] = (byte)(cpu.BCPrim >> 8);
		array[7] = (byte)(cpu.AFPrim & 0xFF);
		array[8] = (byte)(cpu.AFPrim >> 8);
		array[9] = (byte)(cpu.HL & 0xFF);
		array[10] = (byte)(cpu.HL >> 8);
		array[11] = (byte)(cpu.DE & 0xFF);
		array[12] = (byte)(cpu.DE >> 8);
		array[13] = (byte)(cpu.BC & 0xFF);
		array[14] = (byte)(cpu.BC >> 8);
		array[15] = (byte)(cpu.IY & 0xFF);
		array[16] = (byte)(cpu.IY >> 8);
		array[17] = (byte)(cpu.IX & 0xFF);
		array[18] = (byte)(cpu.IX >> 8);
		array[20] = (byte)cpu.R;
		array[21] = (byte)(cpu.AF & 0xFF);
		array[22] = (byte)(cpu.AF >> 8);
		array[23] = (byte)(num & 0xFF);
		array[24] = (byte)(num >> 8);
		array[25] = (byte)(cpu.IM & 3);
		array[19] = (byte)(cpu.IFF2 ? 4u : 0u);
		array[26] = (byte)cpu.In(254);
		cpu.ReadByteFromMemory(num);
		cpu.WriteByteToMemory(num++, (byte)(cpu.PC & 0xFF));
		cpu.ReadByteFromMemory(num);
		cpu.WriteByteToMemory(num++, (byte)(cpu.PC >> 8));
		num -= 2;
		int num2 = 27;
		foreach (int item in cpu.Memory.Skip(16384))
		{
			byte b = (byte)item;
			array[num2++] = b;
		}
		cpu.ReadWordFromMemory(cpu.SP);
		return array;
	}
}
