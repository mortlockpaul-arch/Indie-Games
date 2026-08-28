using Zilog;

namespace ZXBox.Snapshot;

public class MemoryHandler
{
	public static void LoadBytesintoMemory(byte[] bytes, int MemoryStartIndex, Z80 cpu)
	{
		LoadBytesintoMemory(bytes, 0, MemoryStartIndex, cpu);
	}

	public static void LoadBytesintoMemory(byte[] bytes, int ByteArrayStartIndex, int MemoryStartIndex, Z80 cpu)
	{
		for (int i = ByteArrayStartIndex; i < bytes.Length; i++)
		{
			cpu.Memory[MemoryStartIndex++] = bytes[i];
		}
	}
}
