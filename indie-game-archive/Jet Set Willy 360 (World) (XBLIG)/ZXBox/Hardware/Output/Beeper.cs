using System;
using System.Collections.Generic;
using System.Linq;
using ZXBox.Hardware.Interfaces;

namespace ZXBox.Hardware.Output;

public class Beeper : IOutput
{
	public ushort[] buffer;

	private int BufferCount;

	private int lastTState;

	private ushort lastByte;

	private int Samplesperframe;

	private ushort[] LargerThanZeroBuffer;

	private ushort[] EqualToZeroBuffer;

	private ushort upper = 65280;

	private ushort lower = 32512;

	private Queue<ushort[]> BufferQueue = new Queue<ushort[]>();

	public Beeper(int samplesPerFrame, int bufferCount)
	{
		BufferCount = bufferCount;
		Samplesperframe = samplesPerFrame;
		buffer = new ushort[BufferCount];
		LargerThanZeroBuffer = Enumerable.Repeat(upper, BufferCount).ToArray();
		EqualToZeroBuffer = Enumerable.Repeat(lower, BufferCount).ToArray();
	}

	public ushort[] GetSoundBuffer()
	{
		if (BufferQueue.Count > 0)
		{
			return BufferQueue.Dequeue();
		}
		return EqualToZeroBuffer;
	}

	public void AddSoundBuffer(int maxTstates)
	{
		if (lastTState < maxTstates)
		{
			int num = lastTState / Samplesperframe;
			if (num <= BufferCount)
			{
				if (lastByte > lower)
				{
					Array.Copy(LargerThanZeroBuffer, 0, buffer, num, BufferCount - num);
				}
				else
				{
					Array.Copy(EqualToZeroBuffer, 0, buffer, num, BufferCount - num);
				}
			}
		}
		ushort[] array = new ushort[BufferCount];
		Buffer.BlockCopy(buffer, 0, array, 0, BufferCount * 2);
		BufferQueue.Enqueue(array);
		lastTState = 0;
	}

	public void Output(int Port, int ByteValue, int tState)
	{
		if ((Port & 0xFF) != 254)
		{
			return;
		}
		if (lastTState > tState)
		{
			lastTState = 0;
		}
		int num = (tState - lastTState) / Samplesperframe;
		int num2 = lastTState / Samplesperframe;
		if (num2 + num > BufferCount)
		{
			num = BufferCount - num2;
		}
		if (num2 < BufferCount)
		{
			if (lastByte > lower)
			{
				Array.Copy(LargerThanZeroBuffer, 0, buffer, num2, num);
			}
			else
			{
				Array.Copy(EqualToZeroBuffer, 0, buffer, num2, num);
			}
		}
		lastByte = (((ByteValue & 0x10) == 16) ? upper : lower);
		if (tState / Samplesperframe < BufferCount)
		{
			buffer[tState / Samplesperframe] = lastByte;
		}
		lastTState = tState;
	}

	public void ClearBuffer()
	{
		buffer = new ushort[80000];
	}
}
