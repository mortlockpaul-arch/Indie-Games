using System.Collections.Generic;
using ZXBox.Hardware.Interfaces;
using Zilog;

namespace ZXBox;

public class ZxSpectrum : Z80
{
	public List<IInput> InputHardware = new List<IInput>();

	public List<IOutput> OutputHardware = new List<IOutput>();

	public int bordercolor = 1;

	public override int In(int port)
	{
		int num = 255;
		for (int i = 0; i < InputHardware.Count; i++)
		{
			num &= InputHardware[i].Input(port, NumberOfTstates);
		}
		return num;
	}

	public override void Out(int Port, int ByteValue, int tStates)
	{
		for (int i = 0; i < OutputHardware.Count; i++)
		{
			OutputHardware[i].Output(Port, ByteValue, tStates);
		}
	}
}
