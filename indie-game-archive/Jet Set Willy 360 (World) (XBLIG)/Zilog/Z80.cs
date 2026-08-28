using System;

namespace Zilog;

public class Z80
{
	public enum IndexRegistryEnum
	{
		IX,
		IY
	}

	private bool fS;

	private bool fZ;

	private bool f5;

	private bool fH;

	private bool f3;

	private bool fPV;

	private bool fN;

	private bool fC;

	private static int F_C = 1;

	private static int F_N = 2;

	private static int F_PV = 4;

	private static int F_3 = 8;

	private static int F_H = 16;

	private static int F_5 = 32;

	private static int F_Z = 64;

	private static int F_S = 128;

	private static int R_A = 7;

	private static int R_B = 0;

	private static int R_C = 1;

	private static int R_D = 2;

	private static int R_E = 3;

	private static int R_H = 4;

	private static int R_L = 5;

	public int[] IndexRegistry = new int[2];

	public int I;

	public int[] Registers = new int[10];

	public int[] RegitersPrim = new int[10];

	public int Fprim;

	public int PC;

	public int SP = 65536;

	private int opcode;

	public int NumberOfTStatesLeft;

	public bool[] Parity = new bool[256];

	private int _EndTstates2;

	public bool IFF;

	public bool IFF2;

	public int IM = 2;

	public int _R7;

	public int _R;

	public int[] Memory = new int[65536];

	public int NumberOfTstates;

	private int[] bitArray = new int[8] { 1, 2, 4, 8, 16, 32, 64, 128 };

	public int F
	{
		get
		{
			return (fS ? F_S : 0) | (fZ ? F_Z : 0) | (f5 ? F_5 : 0) | (fH ? F_H : 0) | (f3 ? F_3 : 0) | (fPV ? F_PV : 0) | (fN ? F_N : 0) | (fC ? F_C : 0);
		}
		set
		{
			fS = (value & F_S) != 0;
			fZ = (value & F_Z) != 0;
			f5 = (value & F_5) != 0;
			fH = (value & F_H) != 0;
			f3 = (value & F_3) != 0;
			fPV = (value & F_PV) != 0;
			fN = (value & F_N) != 0;
			fC = (value & F_C) != 0;
		}
	}

	public int IX
	{
		get
		{
			return IndexRegistry[0];
		}
		set
		{
			IndexRegistry[0] = value;
		}
	}

	public int IY
	{
		get
		{
			return IndexRegistry[1];
		}
		set
		{
			IndexRegistry[1] = value;
		}
	}

	public int A
	{
		get
		{
			return Registers[R_A];
		}
		set
		{
			Registers[R_A] = value;
		}
	}

	public int B
	{
		get
		{
			return Registers[R_B];
		}
		set
		{
			Registers[R_B] = value;
		}
	}

	public int C
	{
		get
		{
			return Registers[R_C];
		}
		set
		{
			Registers[R_C] = value;
		}
	}

	public int D
	{
		get
		{
			return Registers[R_D];
		}
		set
		{
			Registers[R_D] = value;
		}
	}

	public int E
	{
		get
		{
			return Registers[R_E];
		}
		set
		{
			Registers[R_E] = value;
		}
	}

	public int H
	{
		get
		{
			return Registers[R_H];
		}
		set
		{
			Registers[R_H] = value;
		}
	}

	public int L
	{
		get
		{
			return Registers[R_L];
		}
		set
		{
			Registers[R_L] = value;
		}
	}

	public int APrim
	{
		get
		{
			return RegitersPrim[R_A];
		}
		set
		{
			RegitersPrim[R_A] = value;
		}
	}

	public int BPrim
	{
		get
		{
			return RegitersPrim[R_B];
		}
		set
		{
			RegitersPrim[R_B] = value;
		}
	}

	public int CPrim
	{
		get
		{
			return RegitersPrim[R_C];
		}
		set
		{
			RegitersPrim[R_C] = value;
		}
	}

	public int DPrim
	{
		get
		{
			return RegitersPrim[R_D];
		}
		set
		{
			RegitersPrim[R_D] = value;
		}
	}

	public int EPrim
	{
		get
		{
			return RegitersPrim[R_E];
		}
		set
		{
			RegitersPrim[R_E] = value;
		}
	}

	public int HPrim
	{
		get
		{
			return RegitersPrim[R_H];
		}
		set
		{
			RegitersPrim[R_H] = value;
		}
	}

	public int LPrim
	{
		get
		{
			return RegitersPrim[R_L];
		}
		set
		{
			RegitersPrim[R_L] = value;
		}
	}

	public int FPrim
	{
		get
		{
			return Fprim;
		}
		set
		{
			Fprim = value;
		}
	}

	public int HL
	{
		get
		{
			return (Registers[R_H] << 8) | Registers[R_L];
		}
		set
		{
			Registers[R_H] = value >> 8;
			Registers[R_L] = value & 0xFF;
		}
	}

	public int HLPrim
	{
		get
		{
			return (RegitersPrim[R_H] << 8) | RegitersPrim[R_L];
		}
		set
		{
			RegitersPrim[R_H] = value >> 8;
			RegitersPrim[R_L] = value & 0xFF;
		}
	}

	public int DE
	{
		get
		{
			return (Registers[R_D] << 8) | Registers[R_E];
		}
		set
		{
			Registers[R_D] = value >> 8;
			Registers[R_E] = value & 0xFF;
		}
	}

	public int DEPrim
	{
		get
		{
			return (RegitersPrim[R_D] << 8) | RegitersPrim[R_E];
		}
		set
		{
			RegitersPrim[R_D] = value >> 8;
			RegitersPrim[R_E] = value & 0xFF;
		}
	}

	public int BC
	{
		get
		{
			return (Registers[R_B] << 8) | Registers[R_C];
		}
		set
		{
			Registers[R_B] = value >> 8;
			Registers[R_C] = value & 0xFF;
		}
	}

	public int BCPrim
	{
		get
		{
			return (RegitersPrim[R_B] << 8) | RegitersPrim[R_C];
		}
		set
		{
			RegitersPrim[R_B] = value >> 8;
			RegitersPrim[R_C] = value & 0xFF;
		}
	}

	public int AF
	{
		get
		{
			return (Registers[R_A] << 8) | F;
		}
		set
		{
			Registers[R_A] = value >> 8;
			F = value & 0xFF;
		}
	}

	public int AFPrim
	{
		get
		{
			return (RegitersPrim[R_A] << 8) | FPrim;
		}
		set
		{
			RegitersPrim[R_A] = value >> 8;
			Fprim = value & 0xFF;
		}
	}

	public int IXL
	{
		get
		{
			return IX & 0xFF;
		}
		set
		{
			IX = (IX & 0xFF00) | value;
		}
	}

	public int IXH
	{
		get
		{
			return (IX >> 8) & 0xFF;
		}
		set
		{
			IX = (IX & 0xFF) | (value << 8);
		}
	}

	public int IYL
	{
		get
		{
			return IY & 0xFF;
		}
		set
		{
			IY = (IY & 0xFF00) | value;
		}
	}

	public int IYH
	{
		get
		{
			return (IY >> 8) & 0xFF;
		}
		set
		{
			IY = (IY & 0xFF) | (value << 8);
		}
	}

	public int BitValueFromOP => (opcode >> 3) & 7;

	private int d => Sign(ReadByteFromMemory(PC++));

	public int EndTstates2 => _EndTstates2 + NumberOfTStatesLeft * -1;

	public int R7
	{
		get
		{
			return _R7;
		}
		set
		{
			_R7 = value;
		}
	}

	public int R
	{
		get
		{
			return (_R & 0x7F) | _R7;
		}
		set
		{
			_R = value;
			_R7 = value & 0x80;
		}
	}

	public void DoDDorFDPrefixInstruction(IndexRegistryEnum IRindex)
	{
		int num = 0;
		Refresh(1);
		switch (opcode)
		{
		case 132:
			if (IRindex == IndexRegistryEnum.IX)
			{
				A = ADDADC8(A, IXH, Carry: false, 4);
			}
			else
			{
				A = ADDADC8(A, IYH, Carry: false, 4);
			}
			break;
		case 140:
			if (IRindex == IndexRegistryEnum.IX)
			{
				A = ADDADC8(A, IXH, Carry: true, 4);
			}
			else
			{
				A = ADDADC8(A, IYH, Carry: true, 4);
			}
			break;
		case 141:
			if (IRindex == IndexRegistryEnum.IX)
			{
				A = ADDADC8(A, IXL, Carry: true, 4);
			}
			else
			{
				A = ADDADC8(A, IYL, Carry: true, 4);
			}
			break;
		case 133:
			if (IRindex == IndexRegistryEnum.IX)
			{
				A = ADDADC8(A, IXL, Carry: false, 4);
			}
			else
			{
				A = ADDADC8(A, IYL, Carry: false, 4);
			}
			break;
		case 164:
			if (IRindex == IndexRegistryEnum.IX)
			{
				A = AND8(A, IXH, 4);
			}
			else
			{
				A = AND8(A, IYH, 4);
			}
			break;
		case 165:
			if (IRindex == IndexRegistryEnum.IX)
			{
				A = AND8(A, IXL, 4);
			}
			else
			{
				A = AND8(A, IYL, 4);
			}
			break;
		case 42:
			IndexRegistry[(int)IRindex] = ReadWordFromMemory(GetNextPCWord());
			NumberOfTStatesLeft -= 20;
			break;
		case 34:
			WriteWordToMemory(GetNextPCWord(), IndexRegistry[(int)IRindex]);
			NumberOfTStatesLeft -= 14;
			break;
		case 33:
			IndexRegistry[(int)IRindex] = GetNextPCWord();
			NumberOfTStatesLeft -= 14;
			break;
		case 54:
			WriteByteToMemory(IndexRegistry[(int)IRindex] + d, GetNextPCByte());
			NumberOfTStatesLeft -= 19;
			break;
		case 142:
			A = ADDADC8(A, ReadByteFromMemory(IndexRegistry[(int)IRindex] + d), Carry: true, 19);
			break;
		case 134:
			A = ADDADC8(A, ReadByteFromMemory(IndexRegistry[(int)IRindex] + d), Carry: false, 19);
			break;
		case 9:
			IndexRegistry[(int)IRindex] = ADDADC16(IndexRegistry[(int)IRindex], BC, Carry: false, 15);
			break;
		case 25:
			IndexRegistry[(int)IRindex] = ADDADC16(IndexRegistry[(int)IRindex], DE, Carry: false, 15);
			break;
		case 41:
			IndexRegistry[(int)IRindex] = ADDADC16(IndexRegistry[(int)IRindex], IndexRegistry[(int)IRindex], Carry: false, 15);
			break;
		case 57:
			IndexRegistry[(int)IRindex] = ADDADC16(IndexRegistry[(int)IRindex], SP, Carry: false, 15);
			break;
		case 166:
			A = AND8(A, ReadByteFromMemory(IndexRegistry[(int)IRindex] + d), 19);
			break;
		case 190:
			CP(ReadByteFromMemory(IndexRegistry[(int)IRindex] + d), 19);
			break;
		case 188:
			if (IRindex == IndexRegistryEnum.IX)
			{
				CP(IXH, 7);
			}
			else
			{
				CP(IYH, 7);
			}
			break;
		case 189:
			if (IRindex == IndexRegistryEnum.IX)
			{
				CP(IXL, 7);
			}
			else
			{
				CP(IYL, 7);
			}
			break;
		case 53:
			num = IndexRegistry[(int)IRindex] + d;
			WriteByteToMemory(num, DEC8(ReadByteFromMemory(num), 23));
			break;
		case 37:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXH = DEC8(IXH, 4);
			}
			else
			{
				IYH = DEC8(IYH, 4);
			}
			break;
		case 43:
			IndexRegistry[(int)IRindex] = DEC16(IndexRegistry[(int)IRindex], 10);
			break;
		case 45:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXL = DEC8(IXL, 6);
			}
			else
			{
				IYL = DEC8(IYL, 6);
			}
			break;
		case 227:
		{
			int num4 = ReadWordFromMemory(SP);
			WriteWordToMemory(SP, IndexRegistry[(int)IRindex]);
			IndexRegistry[(int)IRindex] = num4;
			NumberOfTStatesLeft -= 23;
			break;
		}
		case 52:
		{
			int num2 = d;
			WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, INC8(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 0));
			NumberOfTStatesLeft -= 23;
			break;
		}
		case 36:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXH = INC8(IXH, 4);
			}
			else
			{
				IYH = INC8(IYH, 4);
			}
			break;
		case 35:
			IndexRegistry[(int)IRindex] = INC16(IndexRegistry[(int)IRindex], 10);
			break;
		case 44:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXL = INC8(IXL, 4);
			}
			else
			{
				IYL = INC8(IYL, 4);
			}
			break;
		case 233:
			JP(argument: true, IndexRegistry[(int)IRindex], 8);
			break;
		case 112:
		case 113:
		case 114:
		case 115:
		case 116:
		case 117:
		case 119:
			WriteByteToMemory(IndexRegistry[(int)IRindex] + d, RegisterValueFromOP(0));
			NumberOfTStatesLeft -= 19;
			break;
		case 126:
			A = ReadByteFromMemory(IndexRegistry[(int)IRindex] + d);
			NumberOfTStatesLeft -= 19;
			break;
		case 124:
			if (IRindex == IndexRegistryEnum.IX)
			{
				A = IXH;
			}
			else
			{
				A = IYH;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 125:
			if (IRindex == IndexRegistryEnum.IX)
			{
				A = IXL;
			}
			else
			{
				A = IYL;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 70:
			B = ReadByteFromMemory(IndexRegistry[(int)IRindex] + d);
			NumberOfTStatesLeft -= 19;
			break;
		case 68:
			if (IRindex == IndexRegistryEnum.IX)
			{
				B = IXH;
			}
			else
			{
				B = IYH;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 69:
			if (IRindex == IndexRegistryEnum.IX)
			{
				B = IXL;
			}
			else
			{
				B = IYL;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 78:
			C = ReadByteFromMemory(IndexRegistry[(int)IRindex] + d);
			NumberOfTStatesLeft -= 19;
			break;
		case 76:
			if (IRindex == IndexRegistryEnum.IX)
			{
				C = IXH;
			}
			else
			{
				C = IYH;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 77:
			if (IRindex == IndexRegistryEnum.IX)
			{
				C = IXL;
			}
			else
			{
				C = IYL;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 86:
			D = ReadByteFromMemory(IndexRegistry[(int)IRindex] + d);
			NumberOfTStatesLeft -= 19;
			break;
		case 84:
			if (IRindex == IndexRegistryEnum.IX)
			{
				D = IXH;
			}
			else
			{
				D = IYH;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 85:
			if (IRindex == IndexRegistryEnum.IX)
			{
				D = IXL;
			}
			else
			{
				D = IYL;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 94:
			E = ReadByteFromMemory(IndexRegistry[(int)IRindex] + d);
			NumberOfTStatesLeft -= 19;
			break;
		case 92:
			if (IRindex == IndexRegistryEnum.IX)
			{
				E = IXH;
			}
			else
			{
				E = IYH;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 93:
			if (IRindex == IndexRegistryEnum.IX)
			{
				E = IXL;
			}
			else
			{
				E = IYL;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 102:
			H = ReadByteFromMemory(IndexRegistry[(int)IRindex] + d);
			NumberOfTStatesLeft -= 19;
			break;
		case 103:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXH = A;
			}
			else
			{
				IYH = A;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 96:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXH = B;
			}
			else
			{
				IYH = B;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 97:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXH = C;
			}
			else
			{
				IYH = C;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 98:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXH = D;
			}
			else
			{
				IYH = D;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 99:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXH = E;
			}
			else
			{
				IYH = E;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 100:
			NumberOfTStatesLeft -= 4;
			break;
		case 101:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXH = IXL;
			}
			else
			{
				IYH = IYL;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 38:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXH = GetNextPCByte();
			}
			else
			{
				IYH = GetNextPCByte();
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 110:
			L = ReadByteFromMemory(IndexRegistry[(int)IRindex] + d);
			NumberOfTStatesLeft -= 19;
			break;
		case 111:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXL = A;
			}
			else
			{
				IYL = A;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 104:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXL = B;
			}
			else
			{
				IYL = B;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 105:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXL = C;
			}
			else
			{
				IYL = C;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 106:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXL = D;
			}
			else
			{
				IYL = D;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 107:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXL = E;
			}
			else
			{
				IYL = E;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 108:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXL = IXH;
			}
			else
			{
				IYL = IYH;
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 109:
			NumberOfTStatesLeft -= 4;
			break;
		case 46:
			if (IRindex == IndexRegistryEnum.IX)
			{
				IXL = GetNextPCByte();
			}
			else
			{
				IYL = GetNextPCByte();
			}
			NumberOfTStatesLeft -= 4;
			break;
		case 249:
			SP = IndexRegistry[(int)IRindex];
			NumberOfTStatesLeft -= 10;
			break;
		case 182:
			OR(ReadByteFromMemory(IndexRegistry[(int)IRindex] + d), 7);
			break;
		case 180:
			if (IRindex == IndexRegistryEnum.IX)
			{
				OR(IXH, 7);
			}
			else
			{
				OR(IYH, 7);
			}
			break;
		case 181:
			if (IRindex == IndexRegistryEnum.IX)
			{
				OR(IXL, 7);
			}
			else
			{
				OR(IYL, 7);
			}
			break;
		case 203:
		{
			int num2 = d;
			int num3 = 0;
			NextOpcode();
			switch (opcode)
			{
			case 0:
				num3 = RLC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				B = num3;
				break;
			case 1:
				num3 = RLC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				C = num3;
				break;
			case 2:
				num3 = RLC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				D = num3;
				break;
			case 3:
				num3 = RLC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				E = num3;
				break;
			case 4:
				num3 = RLC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				H = num3;
				break;
			case 5:
				num3 = RLC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				L = num3;
				break;
			case 6:
				num3 = RLC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				break;
			case 34:
				num3 = SLA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				D = num3;
				break;
			case 7:
				num3 = RLC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				A = num3;
				break;
			case 8:
				num3 = RRC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				B = num3;
				break;
			case 9:
				num3 = RRC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				C = num3;
				break;
			case 10:
				num3 = RRC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				D = num3;
				break;
			case 11:
				num3 = RRC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				E = num3;
				break;
			case 12:
				num3 = RRC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				H = num3;
				break;
			case 13:
				num3 = RRC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				L = num3;
				break;
			case 14:
				num3 = RRC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				break;
			case 15:
				num3 = RRC(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				A = num3;
				break;
			case 16:
				num3 = RL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				B = num3;
				break;
			case 17:
				num3 = RL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				C = num3;
				break;
			case 18:
				num3 = RL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				D = num3;
				break;
			case 19:
				num3 = RL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				E = num3;
				break;
			case 20:
				num3 = RL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				H = num3;
				break;
			case 21:
				num3 = RL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				L = num3;
				break;
			case 22:
				num3 = RL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				break;
			case 23:
				num3 = RL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				A = num3;
				break;
			case 24:
				num3 = RR(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				B = num3;
				break;
			case 25:
				num3 = RR(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				C = num3;
				break;
			case 26:
				num3 = RR(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				D = num3;
				break;
			case 27:
				num3 = RR(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				E = num3;
				break;
			case 28:
				num3 = RR(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				H = num3;
				break;
			case 29:
				num3 = RR(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				L = num3;
				break;
			case 30:
				num3 = RR(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				break;
			case 31:
				num3 = RR(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				A = num3;
				break;
			case 32:
				num3 = SLA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				B = num3;
				break;
			case 33:
				num3 = SLA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				C = num3;
				break;
			case 35:
				num3 = SLA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				E = num3;
				break;
			case 36:
				num3 = SLA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				H = num3;
				break;
			case 37:
				num3 = SLA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				L = num3;
				break;
			case 38:
				num3 = SLA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				break;
			case 39:
				num3 = SLA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				A = num3;
				break;
			case 40:
				num3 = SRA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				B = num3;
				break;
			case 41:
				num3 = SRA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				C = num3;
				break;
			case 42:
				num3 = SRA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				D = num3;
				break;
			case 43:
				num3 = SRA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				E = num3;
				break;
			case 44:
				num3 = SRA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				H = num3;
				break;
			case 45:
				num3 = SRA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				L = num3;
				break;
			case 46:
				num3 = SRA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				break;
			case 47:
				num3 = SRA(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				A = num3;
				break;
			case 48:
				num3 = SLL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				B = num3;
				break;
			case 49:
				num3 = SLL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				C = num3;
				break;
			case 50:
				num3 = SLL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				D = num3;
				break;
			case 51:
				num3 = SLL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				E = num3;
				break;
			case 52:
				num3 = SLL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				H = num3;
				break;
			case 53:
				num3 = SLL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				L = num3;
				break;
			case 54:
				num3 = SLL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				break;
			case 55:
				num3 = SLL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				A = num3;
				break;
			case 56:
				num3 = SRL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				B = num3;
				break;
			case 57:
				num3 = SRL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				C = num3;
				break;
			case 58:
				num3 = SRL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				D = num3;
				break;
			case 59:
				num3 = SRL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				E = num3;
				break;
			case 60:
				num3 = SRL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				H = num3;
				break;
			case 61:
				num3 = SRL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				L = num3;
				break;
			case 62:
				num3 = SRL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				break;
			case 63:
				num3 = SRL(ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				A = num3;
				break;
			case 64:
			case 65:
			case 66:
			case 67:
			case 68:
			case 69:
			case 70:
			case 71:
				BITixyd(0, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), IndexRegistry[(int)IRindex] + num2, 20);
				break;
			case 72:
			case 73:
			case 74:
			case 75:
			case 76:
			case 77:
			case 78:
			case 79:
				BITixyd(1, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), IndexRegistry[(int)IRindex] + num2, 20);
				break;
			case 80:
			case 81:
			case 82:
			case 83:
			case 84:
			case 85:
			case 86:
			case 87:
				BITixyd(2, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), IndexRegistry[(int)IRindex] + num2, 20);
				break;
			case 88:
			case 89:
			case 90:
			case 91:
			case 92:
			case 93:
			case 94:
				BITixyd(3, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), IndexRegistry[(int)IRindex] + num2, 20);
				break;
			case 95:
			case 96:
			case 97:
			case 98:
			case 99:
			case 100:
			case 101:
			case 102:
			case 103:
				BITixyd(4, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), IndexRegistry[(int)IRindex] + num2, 20);
				break;
			case 104:
			case 105:
			case 106:
			case 107:
			case 108:
			case 109:
			case 110:
			case 111:
				BITixyd(5, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), IndexRegistry[(int)IRindex] + num2, 20);
				break;
			case 112:
			case 113:
			case 114:
			case 115:
			case 116:
			case 117:
			case 118:
			case 119:
				BITixyd(6, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), IndexRegistry[(int)IRindex] + num2, 20);
				break;
			case 120:
			case 121:
			case 122:
			case 123:
			case 124:
			case 125:
			case 126:
			case 127:
				BITixyd(7, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), IndexRegistry[(int)IRindex] + num2, 20);
				break;
			case 135:
			case 143:
			case 151:
			case 159:
			case 167:
			case 175:
			case 183:
			case 191:
				num3 = RES(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				A = num3;
				break;
			case 199:
			case 207:
			case 215:
			case 223:
			case 231:
			case 239:
			case 247:
			case 255:
				num3 = SET(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				A = num3;
				break;
			case 128:
			case 136:
			case 144:
			case 152:
			case 160:
			case 168:
			case 176:
			case 184:
				num3 = RES(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				B = num3;
				break;
			case 192:
			case 200:
			case 208:
			case 216:
			case 224:
			case 232:
			case 240:
			case 248:
				num3 = SET(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				B = num3;
				break;
			case 129:
			case 137:
			case 145:
			case 153:
			case 161:
			case 169:
			case 177:
			case 185:
				num3 = RES(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				C = num3;
				break;
			case 193:
			case 201:
			case 209:
			case 217:
			case 225:
			case 233:
			case 241:
			case 249:
				num3 = SET(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				C = num3;
				break;
			case 130:
			case 138:
			case 146:
			case 154:
			case 162:
			case 170:
			case 178:
			case 186:
				num3 = RES(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				D = num3;
				break;
			case 194:
			case 202:
			case 210:
			case 218:
			case 226:
			case 234:
			case 242:
			case 250:
				num3 = SET(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				D = num3;
				break;
			case 131:
			case 139:
			case 147:
			case 155:
			case 163:
			case 171:
			case 179:
			case 187:
				num3 = RES(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				E = num3;
				break;
			case 195:
			case 203:
			case 211:
			case 219:
			case 227:
			case 235:
			case 243:
			case 251:
				num3 = SET(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				E = num3;
				break;
			case 132:
			case 140:
			case 148:
			case 156:
			case 164:
			case 172:
			case 180:
			case 188:
				num3 = RES(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				H = num3;
				break;
			case 196:
			case 204:
			case 212:
			case 220:
			case 228:
			case 236:
			case 244:
			case 252:
				num3 = SET(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				H = num3;
				break;
			case 133:
			case 141:
			case 149:
			case 157:
			case 165:
			case 173:
			case 181:
			case 189:
				num3 = RES(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				L = num3;
				break;
			case 197:
			case 205:
			case 213:
			case 221:
			case 229:
			case 237:
			case 245:
			case 253:
				num3 = SET(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				L = num3;
				break;
			case 134:
			case 142:
			case 150:
			case 158:
			case 166:
			case 174:
			case 182:
			case 190:
				num3 = RES(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				break;
			case 198:
			case 206:
			case 214:
			case 222:
			case 230:
			case 238:
			case 246:
			case 254:
				num3 = SET(BitValueFromOP, ReadByteFromMemory(IndexRegistry[(int)IRindex] + num2), 23);
				WriteByteToMemory(IndexRegistry[(int)IRindex] + num2, num3);
				break;
			}
			break;
		}
		case 225:
			IndexRegistry[(int)IRindex] = POP();
			NumberOfTStatesLeft -= 14;
			break;
		case 229:
			PUSH(IndexRegistry[(int)IRindex]);
			NumberOfTStatesLeft -= 15;
			break;
		case 158:
			A = SBC8(ReadByteFromMemory(IndexRegistry[(int)IRindex] + d), 19);
			break;
		case 156:
			if (IRindex == IndexRegistryEnum.IX)
			{
				A = SBC8(IXH, 4);
			}
			else
			{
				A = SBC8(IYH, 4);
			}
			break;
		case 157:
			if (IRindex == IndexRegistryEnum.IX)
			{
				A = SBC8(IXL, 4);
			}
			else
			{
				A = SBC8(IYL, 4);
			}
			break;
		case 150:
			SUB(ReadByteFromMemory(IndexRegistry[(int)IRindex] + d), 7);
			break;
		case 148:
			if (IRindex == IndexRegistryEnum.IX)
			{
				SUB(IXH, 4);
			}
			else
			{
				SUB(IYH, 4);
			}
			break;
		case 149:
			if (IRindex == IndexRegistryEnum.IX)
			{
				SUB(IXL, 4);
			}
			else
			{
				SUB(IYL, 4);
			}
			break;
		case 174:
			XOR(ReadByteFromMemory(IndexRegistry[(int)IRindex] + d), 19);
			break;
		case 172:
			if (IRindex == IndexRegistryEnum.IX)
			{
				XOR(IXH, 4);
			}
			else
			{
				XOR(IYH, 4);
			}
			break;
		case 173:
			if (IRindex == IndexRegistryEnum.IX)
			{
				XOR(IXL, 19);
			}
			else
			{
				XOR(IYL, 19);
			}
			break;
		}
	}

	public void DoCBPrefixInstruction()
	{
		Refresh(1);
		switch (opcode)
		{
		case 54:
			WriteByteToMemory(HL, SLL(ReadByteFromMemory(HL), 12));
			break;
		case 55:
			A = SLL(A, 8);
			break;
		case 48:
			B = SLL(B, 8);
			break;
		case 49:
			C = SLL(C, 8);
			break;
		case 50:
			D = SLL(D, 8);
			break;
		case 51:
			E = SLL(E, 8);
			break;
		case 52:
			H = SLL(H, 8);
			break;
		case 53:
			L = SLL(L, 8);
			break;
		case 70:
		case 78:
		case 86:
		case 94:
		case 102:
		case 110:
		case 118:
		case 126:
			BIT(BitValueFromOP, ReadByteFromMemory(HL), 12);
			break;
		case 64:
		case 65:
		case 66:
		case 67:
		case 68:
		case 69:
		case 71:
		case 72:
		case 73:
		case 74:
		case 75:
		case 76:
		case 77:
		case 79:
		case 80:
		case 81:
		case 82:
		case 83:
		case 84:
		case 85:
		case 87:
		case 88:
		case 89:
		case 90:
		case 91:
		case 92:
		case 93:
		case 95:
		case 96:
		case 97:
		case 98:
		case 99:
		case 100:
		case 101:
		case 103:
		case 104:
		case 105:
		case 106:
		case 107:
		case 108:
		case 109:
		case 111:
		case 112:
		case 113:
		case 114:
		case 115:
		case 116:
		case 117:
		case 119:
		case 120:
		case 121:
		case 122:
		case 123:
		case 124:
		case 125:
		case 127:
			BIT(BitValueFromOP, RegisterValueFromOP(0), 8);
			break;
		case 134:
		case 142:
		case 150:
		case 158:
		case 166:
		case 174:
		case 182:
		case 190:
			WriteByteToMemory(HL, RES(BitValueFromOP, ReadByteFromMemory(HL), 15));
			break;
		case 135:
		case 143:
		case 151:
		case 159:
		case 167:
		case 175:
		case 183:
		case 191:
			A = RES(BitValueFromOP, A, 8);
			break;
		case 128:
		case 136:
		case 144:
		case 152:
		case 160:
		case 168:
		case 176:
		case 184:
			B = RES(BitValueFromOP, B, 8);
			break;
		case 129:
		case 137:
		case 145:
		case 153:
		case 161:
		case 169:
		case 177:
		case 185:
			C = RES(BitValueFromOP, C, 8);
			break;
		case 130:
		case 138:
		case 146:
		case 154:
		case 162:
		case 170:
		case 178:
		case 186:
			D = RES(BitValueFromOP, D, 8);
			break;
		case 131:
		case 139:
		case 147:
		case 155:
		case 163:
		case 171:
		case 179:
		case 187:
			E = RES(BitValueFromOP, E, 8);
			break;
		case 132:
		case 140:
		case 148:
		case 156:
		case 164:
		case 172:
		case 180:
		case 188:
			H = RES(BitValueFromOP, H, 8);
			break;
		case 133:
		case 141:
		case 149:
		case 157:
		case 165:
		case 173:
		case 181:
		case 189:
			L = RES(BitValueFromOP, L, 8);
			break;
		case 22:
			WriteByteToMemory(HL, RL(ReadByteFromMemory(HL), 15));
			break;
		case 23:
			A = RL(A, 8);
			break;
		case 16:
			B = RL(B, 8);
			break;
		case 17:
			C = RL(C, 8);
			break;
		case 18:
			D = RL(D, 8);
			break;
		case 19:
			E = RL(E, 8);
			break;
		case 20:
			H = RL(H, 8);
			break;
		case 21:
			L = RL(L, 8);
			break;
		case 6:
			WriteByteToMemory(HL, RLC(ReadByteFromMemory(HL), 15));
			break;
		case 7:
			A = RLC(A, 8);
			break;
		case 0:
			B = RLC(B, 8);
			break;
		case 1:
			C = RLC(C, 8);
			break;
		case 2:
			D = RLC(D, 8);
			break;
		case 3:
			E = RLC(E, 8);
			break;
		case 4:
			H = RLC(H, 8);
			break;
		case 5:
			L = RLC(L, 8);
			break;
		case 30:
			WriteByteToMemory(HL, RR(ReadByteFromMemory(HL), 15));
			break;
		case 31:
			A = RR(A, 8);
			break;
		case 24:
			B = RR(B, 8);
			break;
		case 25:
			C = RR(C, 8);
			break;
		case 26:
			D = RR(D, 8);
			break;
		case 27:
			E = RR(E, 8);
			break;
		case 28:
			H = RR(H, 8);
			break;
		case 29:
			L = RR(L, 8);
			break;
		case 14:
			WriteByteToMemory(HL, RRC(ReadByteFromMemory(HL), 15));
			break;
		case 15:
			A = RRC(A, 8);
			break;
		case 8:
			B = RRC(B, 8);
			break;
		case 9:
			C = RRC(C, 8);
			break;
		case 10:
			D = RRC(D, 8);
			break;
		case 11:
			E = RRC(E, 8);
			break;
		case 12:
			H = RRC(H, 8);
			break;
		case 13:
			L = RRC(L, 8);
			break;
		case 198:
		case 206:
		case 214:
		case 222:
		case 230:
		case 238:
		case 246:
		case 254:
			WriteByteToMemory(HL, SET(BitValueFromOP, ReadByteFromMemory(HL), 15));
			break;
		case 199:
		case 207:
		case 215:
		case 223:
		case 231:
		case 239:
		case 247:
		case 255:
			A = SET(BitValueFromOP, A, 8);
			break;
		case 192:
		case 200:
		case 208:
		case 216:
		case 224:
		case 232:
		case 240:
		case 248:
			B = SET(BitValueFromOP, B, 8);
			break;
		case 193:
		case 201:
		case 209:
		case 217:
		case 225:
		case 233:
		case 241:
		case 249:
			C = SET(BitValueFromOP, C, 8);
			break;
		case 194:
		case 202:
		case 210:
		case 218:
		case 226:
		case 234:
		case 242:
		case 250:
			D = SET(BitValueFromOP, D, 8);
			break;
		case 195:
		case 203:
		case 211:
		case 219:
		case 227:
		case 235:
		case 243:
		case 251:
			E = SET(BitValueFromOP, E, 8);
			break;
		case 196:
		case 204:
		case 212:
		case 220:
		case 228:
		case 236:
		case 244:
		case 252:
			H = SET(BitValueFromOP, H, 8);
			break;
		case 197:
		case 205:
		case 213:
		case 221:
		case 229:
		case 237:
		case 245:
		case 253:
			L = SET(BitValueFromOP, L, 8);
			break;
		case 38:
			WriteByteToMemory(HL, SLA(ReadByteFromMemory(HL), 15));
			break;
		case 39:
			A = SLA(A, 8);
			break;
		case 32:
			B = SLA(B, 8);
			break;
		case 33:
			C = SLA(C, 8);
			break;
		case 34:
			D = SLA(D, 8);
			break;
		case 35:
			E = SLA(E, 8);
			break;
		case 36:
			H = SLA(H, 8);
			break;
		case 37:
			L = SLA(L, 8);
			break;
		case 46:
			WriteByteToMemory(HL, SRA(ReadByteFromMemory(HL), 15));
			break;
		case 47:
			A = SRA(A, 8);
			break;
		case 40:
			B = SRA(B, 8);
			break;
		case 41:
			C = SRA(C, 8);
			break;
		case 42:
			D = SRA(D, 8);
			break;
		case 43:
			E = SRA(E, 8);
			break;
		case 44:
			H = SRA(H, 8);
			break;
		case 45:
			L = SRA(L, 8);
			break;
		case 62:
			WriteByteToMemory(HL, SRL(ReadByteFromMemory(HL), 15));
			break;
		case 63:
			A = SRL(A, 8);
			break;
		case 56:
			B = SRL(B, 8);
			break;
		case 57:
			C = SRL(C, 8);
			break;
		case 58:
			D = SRL(D, 8);
			break;
		case 59:
			E = SRL(E, 8);
			break;
		case 60:
			H = SRL(H, 8);
			break;
		case 61:
			L = SRL(L, 8);
			break;
		}
	}

	public Z80()
	{
		for (int i = 0; i < 256; i++)
		{
			bool flag = true;
			for (int j = 0; j < 8; j++)
			{
				if ((i & (1 << j)) != 0)
				{
					flag = !flag;
				}
			}
			Parity[i] = flag;
		}
	}

	public virtual int In(int port)
	{
		return 255;
	}

	public virtual void Out(int Port, int ByteValue, int tStates)
	{
	}

	public int RegisterValueFromOP(int rpos)
	{
		return Registers[(opcode >> rpos) & 7];
	}

	public void PCToStack()
	{
		StackpushWord(PC);
	}

	public void PCFromStack()
	{
		PC = StackpopWord();
	}

	private int GetNextPCByte()
	{
		int result = ReadByteFromMemory(PC);
		PC = (PC + 1) & 0xFFFF;
		return result;
	}

	private int GetNextPCWord()
	{
		int num = ReadByteFromMemory(PC++);
		return num | (ReadByteFromMemory(PC++ & 0xFFFF) << 8);
	}

	public void StackpushWord(int word)
	{
		WriteWordToMemory(SP = (SP - 2) & 0xFFFF, word);
	}

	public int StackpopWord()
	{
		int num = ReadByteFromMemory(SP);
		SP++;
		num |= ReadByteFromMemory(SP & 0xFFFF) << 8;
		SP = (SP + 1) & 0xFFFF;
		return num;
	}

	public void WriteWordToMemory(int address, int word)
	{
		if (address >= 16384)
		{
			Memory[address] = word & 0xFF;
			address++;
			Memory[address & 0xFFFF] = word >> 8;
		}
	}

	public void WriteByteToMemory(int address, int bytetowrite)
	{
		if (address >= 16384)
		{
			Memory[address & 0xFFFF] = bytetowrite & 0xFF;
		}
	}

	public int ReadByteFromMemory(int address)
	{
		return Memory[address & 0xFFFF] & 0xFF;
	}

	public int ReadWordFromMemory(int address)
	{
		return ((Memory[(address + 1) & 0xFFFF] << 8) | Memory[address & 0xFFFF]) & 0xFFFF;
	}

	public void Refresh(int t)
	{
		_R = (byte)(((_R + 1) & 0x7F) | (_R & 0x80));
	}

	public void Reset()
	{
		PC = 0;
		SP = 0;
		A = 0;
		F = 0;
		BC = 0;
		DE = 0;
		HL = 0;
		EXX();
		AFPrim = 0;
		A = 0;
		F = 0;
		BC = 0;
		DE = 0;
		HL = 0;
		IX = 0;
		IY = 0;
		R = 0;
		I = 0;
		IFF = false;
		IFF2 = false;
		IM = 0;
		NumberOfTStatesLeft = 0;
		Out(254, 5, 0);
	}

	public void NextOpcode()
	{
		opcode = Memory[PC] & 0xFF;
		PC = (PC + 1) & 0xFFFF;
	}

	public int interrupt()
	{
		if (!IFF)
		{
			return 0;
		}
		switch (IM)
		{
		case 0:
		case 1:
			PCToStack();
			IFF = false;
			IFF2 = false;
			PC = 56;
			return 13;
		case 2:
		{
			PCToStack();
			IFF = false;
			IFF2 = false;
			int address = (I << 8) | 0xFF;
			PC = ReadWordFromMemory(address);
			return 19;
		}
		default:
			return 0;
		}
	}

	public void DoIntructions(int numberOfTStates)
	{
		NumberOfTstates = numberOfTStates;
		NumberOfTStatesLeft += numberOfTStates;
		_EndTstates2 = numberOfTStates;
		while (!interruptTriggered(NumberOfTStatesLeft))
		{
			NextOpcode();
			switch (opcode)
			{
			case 203:
				NextOpcode();
				DoCBPrefixInstruction();
				break;
			case 221:
				Refresh(1);
				NextOpcode();
				DoDDorFDPrefixInstruction(IndexRegistryEnum.IX);
				break;
			case 237:
				Refresh(1);
				NextOpcode();
				DoEDPrefixInstruction();
				break;
			case 253:
				Refresh(1);
				NextOpcode();
				DoDDorFDPrefixInstruction(IndexRegistryEnum.IY);
				break;
			default:
				Refresh(1);
				DoNoPrefixInstruction();
				break;
			}
		}
		NumberOfTStatesLeft -= interrupt();
	}

	public void DoEDPrefixInstruction()
	{
		Refresh(1);
		switch (opcode)
		{
		case 74:
			HL = ADDADC16(HL, BC, Carry: true, 15);
			break;
		case 90:
			HL = ADDADC16(HL, DE, Carry: true, 15);
			break;
		case 106:
			HL = ADDADC16(HL, HL, Carry: true, 15);
			break;
		case 122:
			HL = ADDADC16(HL, SP, Carry: true, 15);
			break;
		case 169:
			CPD();
			break;
		case 185:
			CPDR();
			break;
		case 161:
			CPI();
			break;
		case 177:
			CPIR();
			break;
		case 70:
		case 78:
		case 102:
		case 110:
			IM = 0;
			NumberOfTStatesLeft -= 8;
			break;
		case 86:
		case 118:
			IM = 1;
			NumberOfTStatesLeft -= 8;
			break;
		case 94:
		case 126:
			IM = 2;
			NumberOfTStatesLeft -= 8;
			break;
		case 99:
			WriteWordToMemory(GetNextPCWord(), HL);
			NumberOfTStatesLeft -= 16;
			break;
		case 107:
			HL = ReadWordFromMemory(GetNextPCWord());
			NumberOfTStatesLeft -= 20;
			break;
		case 115:
			WriteWordToMemory(GetNextPCWord(), SP);
			NumberOfTStatesLeft -= 16;
			break;
		case 123:
			SP = ReadWordFromMemory(GetNextPCWord());
			NumberOfTStatesLeft -= 20;
			break;
		case 162:
			INI(16);
			break;
		case 120:
			A = INBC(12);
			break;
		case 64:
			B = INBC(12);
			break;
		case 72:
			C = INBC(12);
			break;
		case 80:
			D = INBC(12);
			break;
		case 88:
			E = INBC(12);
			break;
		case 112:
			INBC(12);
			break;
		case 96:
			H = INBC(12);
			break;
		case 104:
			L = INBC(12);
			break;
		case 170:
			IND(16);
			break;
		case 186:
			INDR();
			break;
		case 87:
			LDAI();
			break;
		case 95:
			LDAR();
			break;
		case 71:
			I = A;
			NumberOfTStatesLeft -= 9;
			break;
		case 79:
			R = A;
			NumberOfTStatesLeft -= 9;
			break;
		case 168:
			LDD();
			break;
		case 184:
			LDDR();
			break;
		case 160:
			LDI();
			break;
		case 176:
			LDIR();
			break;
		case 68:
		case 76:
		case 84:
		case 92:
		case 100:
		case 108:
		case 116:
		case 124:
			NEG();
			break;
		case 187:
			OTDR();
			break;
		case 179:
			OTIR();
			break;
		case 113:
			NumberOfTStatesLeft -= 8;
			Out(BC, 0, NumberOfTstates - Math.Abs(NumberOfTStatesLeft));
			NumberOfTStatesLeft -= 4;
			break;
		case 65:
		case 73:
		case 81:
		case 89:
		case 97:
		case 105:
		case 121:
			NumberOfTStatesLeft -= 8;
			Out(BC, RegisterValueFromOP(3), NumberOfTstates - Math.Abs(NumberOfTStatesLeft));
			NumberOfTStatesLeft -= 4;
			break;
		case 171:
			OUTD();
			break;
		case 163:
			OUTI();
			break;
		case 77:
			RET(condition: true, 14, 0);
			break;
		case 69:
		case 85:
		case 93:
		case 101:
		case 109:
		case 117:
		case 125:
			RET(condition: true, 14, 0);
			IFF = IFF2;
			break;
		case 111:
			RLD();
			break;
		case 103:
			RRD();
			break;
		case 66:
			HL = SBC16(HL, BC, 15);
			break;
		case 82:
			HL = SBC16(HL, DE, 15);
			break;
		case 98:
			HL = SBC16(HL, HL, 15);
			break;
		case 114:
			HL = SBC16(HL, SP, 15);
			break;
		case 83:
			WriteWordToMemory(GetNextPCWord(), DE);
			NumberOfTStatesLeft -= 20;
			break;
		case 67:
			WriteWordToMemory(GetNextPCWord(), BC);
			NumberOfTStatesLeft -= 20;
			break;
		case 75:
			BC = ReadWordFromMemory(GetNextPCWord());
			NumberOfTStatesLeft -= 10;
			break;
		case 91:
			DE = ReadWordFromMemory(GetNextPCWord());
			NumberOfTStatesLeft -= 10;
			break;
		case 178:
			INIR();
			break;
		case 119:
		case 127:
		case 128:
		case 129:
		case 130:
		case 131:
		case 132:
		case 133:
		case 134:
		case 135:
		case 136:
		case 137:
		case 138:
		case 139:
		case 140:
		case 141:
		case 142:
		case 143:
		case 144:
		case 145:
		case 146:
		case 147:
		case 148:
		case 149:
		case 150:
		case 151:
		case 152:
		case 153:
		case 154:
		case 155:
		case 156:
		case 157:
		case 158:
		case 159:
		case 164:
		case 165:
		case 166:
		case 167:
		case 172:
		case 173:
		case 174:
		case 175:
		case 180:
		case 181:
		case 182:
		case 183:
			break;
		}
	}

	public void DoNoPrefixInstruction()
	{
		switch (opcode)
		{
		case 42:
			HL = ReadWordFromMemory(GetNextPCWord());
			NumberOfTStatesLeft -= 20;
			break;
		case 33:
			HL = GetNextPCWord();
			NumberOfTStatesLeft -= 10;
			break;
		case 136:
		case 137:
		case 138:
		case 139:
		case 140:
		case 141:
		case 143:
			A = ADDADC8(A, RegisterValueFromOP(0), Carry: true, 4);
			break;
		case 142:
			A = ADDADC8(A, ReadByteFromMemory(HL), Carry: true, 7);
			break;
		case 206:
			A = ADDADC8(A, GetNextPCByte(), Carry: true, 7);
			break;
		case 134:
			A = ADDADC8(A, ReadByteFromMemory(HL), Carry: false, 7);
			break;
		case 128:
		case 129:
		case 130:
		case 131:
		case 132:
		case 133:
		case 135:
			A = ADDADC8(A, RegisterValueFromOP(0), Carry: false, 4);
			break;
		case 198:
			A = ADDADC8(A, GetNextPCByte(), Carry: false, 7);
			break;
		case 9:
			HL = ADDADC16(HL, BC, Carry: false, 11);
			break;
		case 25:
			HL = ADDADC16(HL, DE, Carry: false, 11);
			break;
		case 41:
			HL = ADDADC16(HL, HL, Carry: false, 11);
			break;
		case 57:
			HL = ADDADC16(HL, SP, Carry: false, 11);
			break;
		case 166:
			A = AND8(A, ReadByteFromMemory(HL), 7);
			break;
		case 160:
		case 161:
		case 162:
		case 163:
		case 164:
		case 165:
		case 167:
			A = AND8(A, RegisterValueFromOP(0), 4);
			break;
		case 230:
			A = AND8(A, GetNextPCByte(), 12);
			break;
		case 205:
			CALLnn();
			break;
		case 220:
			CALL(fC);
			break;
		case 252:
			CALL(fS);
			break;
		case 212:
			CALL(!fC);
			break;
		case 196:
			CALL(!fZ);
			break;
		case 244:
			CALL(!fS);
			break;
		case 236:
			CALL(fPV);
			break;
		case 228:
			CALL(!fPV);
			break;
		case 204:
			CALL(fZ);
			break;
		case 63:
			CCF();
			break;
		case 190:
			CP(ReadByteFromMemory(HL), 7);
			break;
		case 184:
		case 185:
		case 186:
		case 187:
		case 191:
			CP(RegisterValueFromOP(0), 4);
			break;
		case 188:
			CP(H, 4);
			break;
		case 189:
			CP(L, 4);
			break;
		case 254:
			CP(GetNextPCByte(), 7);
			break;
		case 47:
			CPL();
			break;
		case 39:
			DAA();
			break;
		case 53:
			WriteByteToMemory(HL, DEC8(ReadByteFromMemory(HL), 11));
			break;
		case 61:
			A = DEC8(A, 4);
			break;
		case 5:
			B = DEC8(B, 4);
			break;
		case 11:
			BC = DEC16(BC, 6);
			break;
		case 13:
			C = DEC8(C, 4);
			break;
		case 21:
			D = DEC8(D, 4);
			break;
		case 27:
			DE = DEC16(DE, 6);
			break;
		case 29:
			E = DEC8(E, 6);
			break;
		case 37:
			H = DEC8(H, 6);
			break;
		case 43:
			HL = DEC16(HL, 6);
			break;
		case 45:
			L = DEC8(L, 6);
			break;
		case 59:
			SP = DEC16(SP, 6);
			break;
		case 243:
			IFF = (IFF2 = false);
			NumberOfTStatesLeft -= 4;
			break;
		case 16:
			DNJZ();
			break;
		case 251:
			IFF = (IFF2 = true);
			NumberOfTStatesLeft -= 4;
			break;
		case 227:
		{
			int hL = ReadWordFromMemory(SP);
			WriteWordToMemory(SP, HL);
			HL = hL;
			NumberOfTStatesLeft -= 19;
			break;
		}
		case 8:
		{
			int aF = AF;
			AF = AFPrim;
			AFPrim = aF;
			NumberOfTStatesLeft -= 4;
			break;
		}
		case 235:
		{
			int dE = DE;
			DE = HL;
			HL = dE;
			NumberOfTStatesLeft -= 4;
			break;
		}
		case 217:
			EXX();
			break;
		case 118:
		{
			int num = (NumberOfTStatesLeft - 1) / 4 + 1;
			NumberOfTStatesLeft -= num * 4;
			Refresh(num - 1);
			break;
		}
		case 219:
			A = In((A << 8) | GetNextPCByte());
			NumberOfTStatesLeft -= 11;
			break;
		case 52:
			WriteByteToMemory(HL, INC8(ReadByteFromMemory(HL), 0));
			NumberOfTStatesLeft -= 11;
			break;
		case 60:
			A = INC8(A, 4);
			break;
		case 4:
			B = INC8(B, 4);
			break;
		case 3:
			BC = INC16(BC, 6);
			break;
		case 12:
			C = INC8(C, 4);
			break;
		case 20:
			D = INC8(D, 4);
			break;
		case 19:
			DE = INC16(DE, 6);
			break;
		case 28:
			E = INC8(E, 4);
			break;
		case 36:
			H = INC8(H, 4);
			break;
		case 35:
			HL = INC16(HL, 6);
			break;
		case 44:
			L = INC8(L, 4);
			break;
		case 51:
			SP = INC16(SP, 6);
			break;
		case 233:
			JP(argument: true, HL, 4);
			break;
		case 195:
			JP(argument: true, GetNextPCWord(), 10);
			break;
		case 218:
			JP(fC, GetNextPCWord(), 10);
			break;
		case 250:
			JP(fS, GetNextPCWord(), 10);
			break;
		case 210:
			JP(!fC, GetNextPCWord(), 10);
			break;
		case 194:
			JP(!fZ, GetNextPCWord(), 10);
			break;
		case 242:
			JP(!fS, GetNextPCWord(), 10);
			break;
		case 234:
			JP(fPV, GetNextPCWord(), 10);
			break;
		case 226:
			JP(!fPV, GetNextPCWord(), 10);
			break;
		case 202:
			JP(fZ, GetNextPCWord(), 10);
			break;
		case 24:
			JR(argument: true, GetNextPCByte(), 12);
			break;
		case 56:
			JR(fC, GetNextPCByte(), fC ? 12 : 7);
			break;
		case 48:
			JR(!fC, GetNextPCByte(), (!fC) ? 12 : 7);
			break;
		case 32:
			JR(!fZ, GetNextPCByte(), (!fZ) ? 12 : 7);
			break;
		case 40:
			JR(fZ, GetNextPCByte(), fZ ? 12 : 7);
			break;
		case 2:
			WriteByteToMemory(BC, A);
			NumberOfTStatesLeft -= 7;
			break;
		case 18:
			WriteByteToMemory(DE, A);
			NumberOfTStatesLeft -= 7;
			break;
		case 112:
		case 113:
		case 114:
		case 115:
		case 116:
		case 117:
		case 119:
			WriteByteToMemory(HL, RegisterValueFromOP(0));
			NumberOfTStatesLeft -= 7;
			break;
		case 54:
			WriteByteToMemory(HL, GetNextPCByte());
			NumberOfTStatesLeft -= 10;
			break;
		case 50:
			WriteByteToMemory(GetNextPCWord(), A);
			NumberOfTStatesLeft -= 13;
			break;
		case 34:
		{
			int nextPCWord = GetNextPCWord();
			WriteWordToMemory(nextPCWord, HL);
			NumberOfTStatesLeft -= 20;
			break;
		}
		case 10:
			A = ReadByteFromMemory(BC);
			NumberOfTStatesLeft -= 7;
			break;
		case 26:
			A = ReadByteFromMemory(DE);
			NumberOfTStatesLeft -= 7;
			break;
		case 126:
			A = ReadByteFromMemory(HL);
			NumberOfTStatesLeft -= 7;
			break;
		case 58:
			A = ReadByteFromMemory(GetNextPCWord());
			NumberOfTStatesLeft -= 13;
			break;
		case 120:
		case 121:
		case 122:
		case 123:
		case 127:
			A = RegisterValueFromOP(0);
			NumberOfTStatesLeft -= 4;
			break;
		case 124:
			A = H;
			NumberOfTStatesLeft -= 4;
			break;
		case 125:
			A = L;
			NumberOfTStatesLeft -= 4;
			break;
		case 62:
			A = GetNextPCByte();
			NumberOfTStatesLeft -= 7;
			break;
		case 70:
			B = ReadByteFromMemory(HL);
			NumberOfTStatesLeft -= 7;
			break;
		case 64:
		case 65:
		case 66:
		case 67:
		case 71:
			B = RegisterValueFromOP(0);
			NumberOfTStatesLeft -= 4;
			break;
		case 68:
			B = H;
			NumberOfTStatesLeft -= 4;
			break;
		case 69:
			B = L;
			NumberOfTStatesLeft -= 4;
			break;
		case 6:
			B = GetNextPCByte();
			NumberOfTStatesLeft -= 7;
			break;
		case 1:
			BC = GetNextPCWord();
			NumberOfTStatesLeft -= 10;
			break;
		case 78:
			C = ReadByteFromMemory(HL);
			NumberOfTStatesLeft -= 7;
			break;
		case 72:
		case 73:
		case 74:
		case 75:
		case 79:
			C = RegisterValueFromOP(0);
			NumberOfTStatesLeft -= 4;
			break;
		case 76:
			C = H;
			NumberOfTStatesLeft -= 4;
			break;
		case 77:
			C = L;
			NumberOfTStatesLeft -= 4;
			break;
		case 14:
			C = GetNextPCByte();
			NumberOfTStatesLeft -= 7;
			break;
		case 86:
			D = ReadByteFromMemory(HL);
			NumberOfTStatesLeft -= 7;
			break;
		case 80:
		case 81:
		case 82:
		case 83:
		case 87:
			D = RegisterValueFromOP(0);
			NumberOfTStatesLeft -= 4;
			break;
		case 84:
			D = H;
			NumberOfTStatesLeft -= 4;
			break;
		case 85:
			D = L;
			NumberOfTStatesLeft -= 4;
			break;
		case 22:
			D = GetNextPCByte();
			NumberOfTStatesLeft -= 7;
			break;
		case 17:
			DE = GetNextPCWord();
			NumberOfTStatesLeft -= 7;
			break;
		case 94:
			E = ReadByteFromMemory(HL);
			NumberOfTStatesLeft -= 7;
			break;
		case 88:
		case 89:
		case 90:
		case 91:
		case 95:
			E = RegisterValueFromOP(0);
			NumberOfTStatesLeft -= 4;
			break;
		case 92:
			E = H;
			NumberOfTStatesLeft -= 4;
			break;
		case 93:
			E = L;
			NumberOfTStatesLeft -= 4;
			break;
		case 30:
			E = GetNextPCByte();
			NumberOfTStatesLeft -= 7;
			break;
		case 102:
			H = ReadByteFromMemory(HL);
			NumberOfTStatesLeft -= 7;
			break;
		case 103:
			H = A;
			NumberOfTStatesLeft -= 4;
			break;
		case 96:
			H = B;
			NumberOfTStatesLeft -= 4;
			break;
		case 97:
			H = C;
			NumberOfTStatesLeft -= 4;
			break;
		case 98:
			H = D;
			NumberOfTStatesLeft -= 4;
			break;
		case 99:
			H = E;
			NumberOfTStatesLeft -= 4;
			break;
		case 100:
			H = H;
			NumberOfTStatesLeft -= 4;
			break;
		case 101:
			H = L;
			NumberOfTStatesLeft -= 4;
			break;
		case 38:
			H = GetNextPCByte();
			NumberOfTStatesLeft -= 7;
			break;
		case 110:
			L = ReadByteFromMemory(HL);
			NumberOfTStatesLeft -= 7;
			break;
		case 111:
			L = A;
			NumberOfTStatesLeft -= 4;
			break;
		case 104:
			L = B;
			NumberOfTStatesLeft -= 4;
			break;
		case 105:
			L = C;
			NumberOfTStatesLeft -= 4;
			break;
		case 106:
			L = D;
			NumberOfTStatesLeft -= 4;
			break;
		case 107:
			L = E;
			NumberOfTStatesLeft -= 4;
			break;
		case 108:
			L = H;
			NumberOfTStatesLeft -= 4;
			break;
		case 109:
			L = L;
			NumberOfTStatesLeft -= 4;
			break;
		case 46:
			L = GetNextPCByte();
			NumberOfTStatesLeft -= 7;
			break;
		case 249:
			SP = HL;
			NumberOfTStatesLeft -= 6;
			break;
		case 49:
			SP = GetNextPCWord();
			NumberOfTStatesLeft -= 10;
			break;
		case 0:
			NOP();
			break;
		case 182:
			OR(ReadByteFromMemory(HL), 7);
			break;
		case 176:
		case 177:
		case 178:
		case 179:
		case 183:
			OR(RegisterValueFromOP(0), 4);
			break;
		case 180:
			OR(H, 4);
			break;
		case 181:
			OR(L, 4);
			break;
		case 246:
			OR(GetNextPCByte(), 7);
			break;
		case 211:
			NumberOfTStatesLeft -= 7;
			Out(GetNextPCByte(), A, NumberOfTstates - Math.Abs(NumberOfTStatesLeft));
			NumberOfTStatesLeft -= 4;
			break;
		case 241:
			AF = POP();
			NumberOfTStatesLeft -= 10;
			break;
		case 193:
			BC = POP();
			NumberOfTStatesLeft -= 10;
			break;
		case 209:
			DE = POP();
			NumberOfTStatesLeft -= 10;
			break;
		case 225:
			HL = POP();
			NumberOfTStatesLeft -= 10;
			break;
		case 245:
			PUSH(AF);
			NumberOfTStatesLeft -= 11;
			break;
		case 197:
			PUSH(BC);
			NumberOfTStatesLeft -= 11;
			break;
		case 213:
			PUSH(DE);
			NumberOfTStatesLeft -= 11;
			break;
		case 229:
			PUSH(HL);
			NumberOfTStatesLeft -= 11;
			break;
		case 201:
			RET(condition: true, 10, 0);
			break;
		case 216:
			RET(fC, 10, 5);
			break;
		case 248:
			RET(fS, 11, 5);
			break;
		case 208:
			RET(!fC, 11, 5);
			break;
		case 192:
			RET(!fZ, 11, 5);
			break;
		case 240:
			RET(!fS, 11, 5);
			break;
		case 232:
			RET(fPV, 11, 5);
			break;
		case 224:
			RET(!fPV, 11, 5);
			break;
		case 200:
			RET(fZ, 11, 5);
			break;
		case 23:
			RLA();
			break;
		case 7:
			RLCA();
			break;
		case 31:
			RRA();
			break;
		case 15:
			RRCA();
			break;
		case 199:
			RST(0);
			break;
		case 207:
			RST(8);
			break;
		case 215:
			RST(16);
			break;
		case 223:
			RST(24);
			break;
		case 231:
			RST(32);
			break;
		case 239:
			RST(40);
			break;
		case 247:
			RST(48);
			break;
		case 255:
			RST(56);
			break;
		case 158:
			A = SBC8(ReadByteFromMemory(HL), 7);
			break;
		case 152:
		case 153:
		case 154:
		case 155:
		case 156:
		case 157:
		case 159:
			A = SBC8(RegisterValueFromOP(0), 4);
			break;
		case 222:
			A = SBC8(GetNextPCByte(), 7);
			break;
		case 55:
			fC = true;
			fN = false;
			fH = false;
			f3 = (A & F_3) != 0;
			f5 = (A & F_5) != 0;
			NumberOfTStatesLeft -= 4;
			break;
		case 150:
			SUB(ReadByteFromMemory(HL), 7);
			break;
		case 144:
		case 145:
		case 146:
		case 147:
		case 148:
		case 149:
		case 151:
			SUB(RegisterValueFromOP(0), 4);
			break;
		case 214:
			SUB(GetNextPCByte(), 7);
			break;
		case 174:
			XOR(ReadByteFromMemory(HL), 7);
			break;
		case 168:
		case 169:
		case 170:
		case 171:
		case 172:
		case 173:
		case 175:
			XOR(RegisterValueFromOP(0), 4);
			break;
		case 238:
			XOR(GetNextPCByte(), 7);
			break;
		case 203:
		case 221:
		case 237:
		case 253:
			break;
		}
	}

	public void Refresh()
	{
	}

	public bool interruptTriggered(int tstates)
	{
		return tstates <= 0;
	}

	public void PUSH(int word)
	{
		WriteWordToMemory(SP = (SP - 2) & 0xFFFF, word);
	}

	private int RES(int bit, int value, int tstates)
	{
		NumberOfTStatesLeft -= tstates;
		return value & ~bitArray[bit];
	}

	public int RL(int value, int tstates)
	{
		bool flag = (value & 0x80) != 0;
		value = ((!fC) ? (value << 1) : ((value << 1) | 1));
		value &= 0xFF;
		fS = (value & F_S) != 0;
		f3 = (value & F_3) != 0;
		f5 = (value & F_5) != 0;
		fZ = value == 0;
		fPV = Parity[value];
		fH = false;
		fN = false;
		fC = flag;
		NumberOfTStatesLeft -= tstates;
		return value;
	}

	public void RLCA()
	{
		int a = A;
		bool flag = (a & 0x80) != 0;
		a = ((!flag) ? (a << 1) : ((a << 1) | 1));
		a &= 0xFF;
		f3 = (a & F_3) != 0;
		f5 = (a & F_5) != 0;
		fN = false;
		fH = false;
		fC = flag;
		NumberOfTStatesLeft -= 4;
		A = a;
	}

	private int SBC8(int b, int tstates)
	{
		int a = A;
		int num = (fC ? 1 : 0);
		int num2 = a - b - num;
		int num3 = num2 & 0xFF;
		fS = (num3 & F_S) != 0;
		f3 = (num3 & F_3) != 0;
		f5 = (num3 & F_5) != 0;
		fZ = num3 == 0;
		fC = (num2 & 0x100) != 0;
		fPV = ((a ^ b) & (a ^ num3) & 0x80) != 0;
		fH = (((a & 0xF) - (b & 0xF) - num) & F_H) != 0;
		fN = true;
		NumberOfTStatesLeft -= tstates;
		return num3;
	}

	private int SBC16(int a, int b, int tstates)
	{
		int num = (fC ? 1 : 0);
		int num2 = a - b - num;
		int num3 = num2 & 0xFFFF;
		fS = (num3 & (F_S << 8)) != 0;
		f3 = (num3 & (F_3 << 8)) != 0;
		f5 = (num3 & (F_5 << 8)) != 0;
		fZ = num3 == 0;
		fC = (num2 & 0x10000) != 0;
		fPV = ((a ^ b) & (a ^ num3) & 0x8000) != 0;
		fH = (((a & 0xFFF) - (b & 0xFFF) - num) & 0x1000) != 0;
		fN = true;
		NumberOfTStatesLeft -= tstates;
		return num3;
	}

	public int RLC(int value, int tstates)
	{
		bool flag = (value & 0x80) != 0;
		value = ((!flag) ? (value << 1) : ((value << 1) | 1));
		value &= 0xFF;
		fS = (value & F_S) != 0;
		f3 = (value & F_3) != 0;
		f5 = (value & F_5) != 0;
		fZ = value == 0;
		fPV = Parity[value];
		fH = false;
		fN = false;
		fC = flag;
		NumberOfTStatesLeft -= tstates;
		return value;
	}

	public void RLA()
	{
		int a = A;
		bool flag = (a & 0x80) != 0;
		a = ((!fC) ? (a << 1) : ((a << 1) | 1));
		a &= 0xFF;
		f3 = (a & F_3) != 0;
		f5 = (a & F_5) != 0;
		fN = false;
		fH = false;
		fC = flag;
		NumberOfTStatesLeft -= 4;
		A = a;
	}

	public int RR(int value, int tstates)
	{
		bool flag = (value & 1) != 0;
		value = ((!fC) ? (value >> 1) : ((value >> 1) | 0x80));
		fS = (value & F_S) != 0;
		f3 = (value & F_3) != 0;
		f5 = (value & F_5) != 0;
		fZ = value == 0;
		fPV = Parity[value];
		fH = false;
		fN = false;
		fC = flag;
		NumberOfTStatesLeft -= tstates;
		return value;
	}

	public void RST(int position)
	{
		PUSH(PC);
		PC = position;
		NumberOfTStatesLeft -= 11;
	}

	public void RRD()
	{
		int a = A;
		int num = ReadByteFromMemory(HL);
		int num2 = num;
		num = (num >> 4) | (a << 4);
		a = (a & 0xF0) | (num2 & 0xF);
		WriteByteToMemory(HL, num);
		fS = (a & F_S) != 0;
		f3 = (a & F_3) != 0;
		f5 = (a & F_5) != 0;
		fZ = a == 0;
		fPV = Parity[a];
		fH = false;
		fN = false;
		NumberOfTStatesLeft -= 18;
		A = a;
	}

	public void RRCA()
	{
		int a = A;
		bool flag = (a & 1) != 0;
		a = ((!flag) ? (a >> 1) : ((a >> 1) | 0x80));
		f3 = (a & F_3) != 0;
		f5 = (a & F_5) != 0;
		fN = false;
		fH = false;
		fC = flag;
		NumberOfTStatesLeft -= 4;
		A = a;
	}

	public int RRC(int value, int tstates)
	{
		bool flag = (value & 1) != 0;
		value = ((!flag) ? (value >> 1) : ((value >> 1) | 0x80));
		fS = (value & F_S) != 0;
		f3 = (value & F_3) != 0;
		f5 = (value & F_5) != 0;
		fZ = value == 0;
		fPV = Parity[value];
		fH = false;
		fN = false;
		fC = flag;
		NumberOfTStatesLeft -= tstates;
		return value;
	}

	public void RRA()
	{
		int a = A;
		bool flag = (a & 1) != 0;
		a = ((!fC) ? (a >> 1) : ((a >> 1) | 0x80));
		f3 = (a & F_3) != 0;
		f5 = (a & F_5) != 0;
		fN = false;
		fH = false;
		fC = flag;
		NumberOfTStatesLeft -= 4;
		A = a;
	}

	public void RLD()
	{
		int a = A;
		int num = ReadByteFromMemory(HL);
		int num2 = num;
		num = (num << 4) | (a & 0xF);
		a = (a & 0xF0) | (num2 >> 4);
		WriteByteToMemory(HL, num & 0xFF);
		fS = (a & F_S) != 0;
		f3 = (a & F_3) != 0;
		f5 = (a & F_5) != 0;
		fZ = a == 0;
		fPV = Parity[a];
		fH = false;
		fN = false;
		NumberOfTStatesLeft -= 18;
		A = a;
	}

	public int SRA(int value, int tstates)
	{
		bool flag = (value & 1) != 0;
		value = (value >> 1) | (value & 0x80);
		fS = (value & F_S) != 0;
		f3 = (value & F_3) != 0;
		f5 = (value & F_5) != 0;
		fZ = value == 0;
		fPV = Parity[value];
		fH = false;
		fN = false;
		fC = flag;
		NumberOfTStatesLeft -= tstates;
		return value;
	}

	public void XOR(int value, int tstates)
	{
		int num = (A ^ value) & 0xFF;
		fS = (num & F_S) != 0;
		f3 = (num & F_3) != 0;
		f5 = (num & F_5) != 0;
		fH = false;
		fPV = Parity[num];
		fZ = num == 0;
		fN = false;
		fC = false;
		NumberOfTStatesLeft -= tstates;
		A = num;
	}

	private int SRL(int value, int tstates)
	{
		bool flag = (value & 1) != 0;
		value >>= 1;
		fS = (value & F_S) != 0;
		f3 = (value & F_3) != 0;
		f5 = (value & F_5) != 0;
		fZ = value == 0;
		fPV = Parity[value];
		fH = false;
		fN = false;
		fC = flag;
		NumberOfTStatesLeft -= tstates;
		return value;
	}

	public int SLA(int value, int tstates)
	{
		bool flag = (value & 0x80) != 0;
		value = (value << 1) & 0xFF;
		fS = (value & F_S) != 0;
		f3 = (value & F_3) != 0;
		f5 = (value & F_5) != 0;
		fZ = value == 0;
		fPV = Parity[value];
		fH = false;
		fN = false;
		fC = flag;
		NumberOfTStatesLeft -= tstates;
		return value;
	}

	private int SET(int bit, int value, int tstates)
	{
		NumberOfTStatesLeft -= tstates;
		return value | bitArray[bit];
	}

	public void RET(bool condition, int tstates, int notmettstates)
	{
		if (condition)
		{
			PC = POP();
			NumberOfTStatesLeft -= tstates;
		}
		else
		{
			NumberOfTStatesLeft -= notmettstates;
		}
	}

	public int POP()
	{
		int sP = SP;
		int num = ReadByteFromMemory(sP);
		sP++;
		num |= ReadByteFromMemory(sP & 0xFFFF) << 8;
		SP = ++sP & 0xFFFF;
		return num;
	}

	public void OUTI()
	{
		B = DEC8(B, 0);
		NumberOfTStatesLeft -= 9;
		Out(BC, ReadByteFromMemory(HL), NumberOfTstates - Math.Abs(NumberOfTStatesLeft));
		HL = INC16(HL, 0);
		fZ = B == 0;
		fN = true;
		if (ReadByteFromMemory(HL) + L > 255)
		{
			fH = true;
			fC = true;
		}
		else
		{
			fH = false;
			fC = false;
		}
		fPV = Parity[((ReadByteFromMemory(HL) + L) & 7) ^ B];
		NumberOfTStatesLeft -= 7;
	}

	public void OUTD()
	{
		B = DEC8(B, 0);
		int num = ReadByteFromMemory(HL);
		NumberOfTStatesLeft -= 9;
		Out(BC, num, NumberOfTstates - Math.Abs(NumberOfTStatesLeft));
		HL = DEC16(HL, 0);
		fZ = B == 0;
		fN = ((num >> 7) & 1) != 1;
		if (num + L > 255)
		{
			fH = true;
			fC = true;
		}
		else
		{
			fH = false;
			fC = false;
		}
		fPV = Parity[((num + L) & 7) ^ B];
		NumberOfTStatesLeft -= 7;
	}

	public void OTIR()
	{
		int num = ReadByteFromMemory(HL);
		NumberOfTStatesLeft -= 9;
		B = DEC8(B, 0);
		Out(BC, num, NumberOfTstates - Math.Abs(NumberOfTStatesLeft));
		HL = INC16(HL, 0);
		fN = ((num >> 7) & 1) == 1;
		if (num + L > 255)
		{
			fH = true;
			fC = true;
		}
		else
		{
			fH = true;
			fC = true;
		}
		fPV = Parity[((num + L) & 7) ^ B];
		if (B != 0)
		{
			PC = (PC - 2) & 0xFFFF;
			NumberOfTStatesLeft -= 12;
		}
		else
		{
			NumberOfTStatesLeft -= 7;
		}
	}

	public void OTDR()
	{
		B = DEC8(B, 0);
		NumberOfTStatesLeft -= 9;
		Out(BC, ReadByteFromMemory(HL), NumberOfTstates - Math.Abs(NumberOfTStatesLeft));
		HL = DEC16(HL, 0);
		fZ = true;
		fZ = true;
		if (B != 0)
		{
			PC = (PC - 2) & 0xFFFF;
			NumberOfTStatesLeft -= 12;
		}
		else
		{
			NumberOfTStatesLeft -= 7;
		}
	}

	public void OR(int b, int tstates)
	{
		int num = A | b;
		fS = (num & F_S) != 0;
		f3 = (num & F_3) != 0;
		f5 = (num & F_5) != 0;
		fH = false;
		fPV = Parity[num];
		fZ = num == 0;
		fN = false;
		fC = false;
		NumberOfTStatesLeft -= tstates;
		A = num;
	}

	public void NOP()
	{
		NumberOfTStatesLeft -= 4;
	}

	public void NEG()
	{
		int a = A;
		A = 0;
		SUB(a, 0);
		NumberOfTStatesLeft -= 8;
	}

	public void LDD()
	{
		int num = ReadByteFromMemory(HL);
		WriteByteToMemory(DE, num);
		DE = DEC16(DE, 0);
		HL = DEC16(HL, 0);
		BC = DEC16(BC, 0);
		fPV = BC != 0;
		fH = false;
		fN = false;
		int num2 = num + A;
		f5 = (num2 & 1) == 1;
		f3 = ((num2 >> 3) & 1) == 1;
		NumberOfTStatesLeft -= 16;
	}

	public void LDDR()
	{
		int num = 0;
		int num2 = BC;
		int num3 = DE;
		int num4 = HL;
		Refresh(-2);
		do
		{
			WriteByteToMemory(num3, ReadByteFromMemory(num4));
			num4 = DEC16(num4, 0);
			num3 = DEC16(num3, 0);
			num2 = DEC16(num2, 0);
			num += 21;
			Refresh(2);
		}
		while (!interruptTriggered(num) && num2 != 0);
		if (num2 != 0)
		{
			PC = (PC - 2) & 0xFFFF;
			fH = false;
			fN = false;
			fPV = true;
		}
		else
		{
			num += -5;
			fH = false;
			fN = false;
			fPV = false;
		}
		DE = num3;
		HL = num4;
		BC = num2;
		NumberOfTStatesLeft -= num;
	}

	public void LDI()
	{
		int num = ReadByteFromMemory(HL);
		WriteByteToMemory(DE, num);
		DE = INC16(DE, 0);
		HL = INC16(HL, 0);
		BC = DEC16(BC, 0);
		int num2 = num + A;
		fPV = BC != 0;
		fH = false;
		fN = false;
		f5 = (num2 & 1) == 1;
		f3 = ((num2 >> 3) & 1) == 1;
		NumberOfTStatesLeft -= 16;
	}

	public void LDIR()
	{
		int num = 0;
		int num2 = BC;
		int num3 = DE;
		int num4 = HL;
		Refresh(-2);
		do
		{
			Memory[num3] = Memory[num4];
			num4 = INC16(num4, 0);
			num3 = INC16(num3, 0);
			num2 = DEC16(num2, 0);
			num += 21;
			Refresh(2);
		}
		while (!interruptTriggered(NumberOfTStatesLeft - num) && num2 != 0);
		if (num2 != 0)
		{
			PC = (PC - 2) & 0xFFFF;
			fH = false;
			fN = false;
			fPV = true;
		}
		else
		{
			num += -5;
			fH = false;
			fN = false;
			fPV = false;
		}
		DE = num3;
		HL = num4;
		BC = num2;
		NumberOfTStatesLeft -= num;
	}

	private void LDAR()
	{
		int r = R;
		fS = (r & F_S) != 0;
		f3 = (r & F_3) != 0;
		f5 = (r & F_5) != 0;
		fZ = r == 0;
		fPV = IFF2;
		fH = false;
		fN = false;
		NumberOfTStatesLeft -= 9;
		A = r;
	}

	public void LDAI()
	{
		int i = I;
		fS = (i & F_S) != 0;
		f3 = (i & F_3) != 0;
		f5 = (i & F_5) != 0;
		fZ = i == 0;
		fPV = IFF2;
		fH = false;
		fN = false;
		NumberOfTStatesLeft -= 9;
		A = i;
	}

	public void JP(bool argument, int position, int tstates)
	{
		if (argument)
		{
			PC = position;
		}
		NumberOfTStatesLeft -= tstates;
	}

	private int Sign(int nn)
	{
		return nn - ((nn & 0x80) << 1);
	}

	public void JR(bool argument, int position, int tstates)
	{
		if (argument)
		{
			PC = (PC + Sign(position)) & 0xFFFF;
		}
		NumberOfTStatesLeft -= tstates;
	}

	public int INBC(int tstates)
	{
		int num = In(BC);
		NumberOfTStatesLeft -= tstates;
		fZ = num == 0;
		fS = (num & F_S) != 0;
		f3 = (num & F_3) != 0;
		f5 = (num & F_5) != 0;
		fPV = Parity[num];
		fN = false;
		fH = false;
		return num;
	}

	public void INDR()
	{
		IND(0);
		if (B != 0)
		{
			PC -= 2;
			NumberOfTStatesLeft -= 21;
		}
		else
		{
			NumberOfTStatesLeft -= 16;
		}
	}

	public void IND(int tstates)
	{
		int num = DEC8(B, 0);
		WriteByteToMemory(HL, In(BC));
		B = num;
		HL = DEC16(HL, 0);
		fZ = num == 0;
		fN = true;
		if (ReadByteFromMemory(HL) + ((C - 1) & 0xFF) > 255)
		{
			fC = true;
			fH = true;
		}
		else
		{
			fC = false;
			fH = false;
		}
		fPV = Parity[((ReadByteFromMemory(HL) + ((C - 1) & 0xFF)) & 7) ^ B];
		NumberOfTStatesLeft -= tstates;
	}

	public void INI(int tstates)
	{
		int num = DEC8(B, 0);
		int num2 = In(BC);
		WriteByteToMemory(HL, num2);
		B = num;
		HL = INC16(HL, 0);
		fZ = num == 0;
		fN = true;
		if (num2 + ((C + 1) & 0xFF) > 255)
		{
			fC = true;
			fH = true;
		}
		else
		{
			fC = false;
			fH = false;
		}
		fPV = Parity[((num2 + ((C + 1) & 0xFF)) & 7) ^ B];
		NumberOfTStatesLeft -= tstates;
	}

	public void INIR()
	{
		INI(0);
		if (B != 0)
		{
			NumberOfTStatesLeft -= 21;
			PC -= 2;
		}
		else
		{
			NumberOfTStatesLeft -= 16;
		}
	}

	public int ADDADC8(int a, int b, bool Carry, int tStates)
	{
		int num = 0;
		if (Carry)
		{
			num = (fC ? 1 : 0);
		}
		int num2 = a + b + num;
		int num3 = num2 & 0xFF;
		fS = (num3 & F_S) != 0;
		f3 = (num3 & F_3) != 0;
		f5 = (num3 & F_5) != 0;
		fZ = num3 == 0;
		fC = (num2 & 0x100) != 0;
		fPV = ((a ^ ~b) & (a ^ num3) & 0x80) != 0;
		fH = (((a & 0xF) + (b & 0xF) + num) & F_H) != 0;
		fN = false;
		NumberOfTStatesLeft -= tStates;
		return num3;
	}

	private int ADDADC16(int a, int b, bool Carry, int tStates)
	{
		int num = ((fC && Carry) ? 1 : 0);
		int num2 = a + b + num;
		int num3 = num2 & 0xFFFF;
		f3 = (num3 & (F_3 << 8)) != 0;
		f5 = (num3 & (F_5 << 8)) != 0;
		fC = (num2 & 0x10000) != 0;
		fH = (((a & 0xFFF) + (b & 0xFFF) + num) & 0x1000) != 0;
		fN = false;
		if (Carry)
		{
			fS = (num3 & (F_S << 8)) != 0;
			fPV = ((a ^ ~b) & (a ^ num3) & 0x8000) != 0;
			fZ = num3 == 0;
		}
		NumberOfTStatesLeft -= tStates;
		return num3;
	}

	public int AND8(int a, int b, int tStates)
	{
		int num = a & b;
		fS = (num & F_S) != 0;
		f3 = (num & F_3) != 0;
		f5 = (num & F_5) != 0;
		fH = true;
		fPV = Parity[num];
		fZ = num == 0;
		fN = false;
		fC = false;
		NumberOfTStatesLeft -= tStates;
		return num;
	}

	public void BIT(int bit, int regvalue, int tStates)
	{
		_ = bitArray[bit];
		NumberOfTStatesLeft -= tStates;
		F = (byte)((F & F_C) | F_H | (regvalue & (F_3 | F_5)) | (((regvalue & (1 << bit)) == 0) ? (F_PV | F_Z) : 0));
	}

	public void BITixyd(int bit, int regvalue, int ixyd, int tStates)
	{
		bool flag = (regvalue & bitArray[bit]) != 0;
		fN = false;
		fH = true;
		f3 = ((ixyd >> 11) & 1) == 1;
		f5 = ((ixyd >> 13) & 1) == 1;
		fS = bit == 7 && flag;
		fZ = !flag;
		fPV = !flag;
		NumberOfTStatesLeft -= tStates;
	}

	public void CALLnn()
	{
		int nextPCWord = GetNextPCWord();
		PCToStack();
		PC = nextPCWord;
		NumberOfTStatesLeft -= 17;
	}

	private int SLL(int value, int tstates)
	{
		int num = (value & 0x80) >> 7;
		value = ((value << 1) | 1) & 0xFF;
		fS = (value & F_S) != 0;
		f3 = (value & F_3) != 0;
		f5 = (value & F_5) != 0;
		fZ = value == 0;
		fPV = Parity[value];
		fH = false;
		fN = false;
		fC = num == 1;
		NumberOfTStatesLeft -= tstates;
		return value;
	}

	private void CP(int value, int tstates)
	{
		int a = A;
		int num = a - value;
		int num2 = num & 0xFF;
		fS = (num2 & F_S) != 0;
		f3 = (value & F_3) != 0;
		f5 = (value & F_5) != 0;
		fN = true;
		fZ = num2 == 0;
		fC = (num & 0x100) != 0;
		fH = (((a & 0xF) - (value & 0xF)) & F_H) != 0;
		fPV = ((a ^ value) & (a ^ num2) & 0x80) != 0;
		NumberOfTStatesLeft -= tstates;
	}

	public void CP2(int s, int tStates)
	{
		NumberOfTStatesLeft -= tStates;
		int num = A - s;
		int num2 = num & 0xFF;
		fS = (num2 & F_S) != 0;
		f3 = (s & F_3) != 0;
		f5 = (s & F_5) != 0;
		fN = true;
		fZ = num2 == 0;
		fC = (num & 0x100) != 0;
		fH = (((A & 0xF) - (s & 0xF)) & F_H) != 0;
		fPV = ((A ^ s) & (A ^ num2) & 0x80) != 0;
	}

	public void CALL(bool argument)
	{
		if (argument)
		{
			int nextPCWord = GetNextPCWord();
			PCToStack();
			PC = nextPCWord;
			NumberOfTStatesLeft -= 17;
		}
		else
		{
			PC = (PC + 2) & 0xFFFF;
			NumberOfTStatesLeft -= 10;
		}
	}

	public void CCF()
	{
		f3 = (A & F_3) != 0;
		f5 = (A & F_5) != 0;
		fN = false;
		fC = !fC;
		NumberOfTStatesLeft -= 4;
	}

	public void CPD()
	{
		bool flag = fC;
		CP(ReadByteFromMemory(HL), 0);
		HL = DEC16(HL, 0);
		BC = DEC16(BC, 0);
		fPV = BC != 0;
		fC = flag;
		int num = A - ReadByteFromMemory(HL) - (fH ? 1 : 0);
		_ = BC;
		fN = true;
		fC = flag;
		f5 = (num & 1) == 1;
		f3 = ((num >> 3) & 1) == 1;
		NumberOfTStatesLeft -= 16;
	}

	public void CPI()
	{
		bool flag = fC;
		int num = ReadByteFromMemory(HL);
		CP(num, 0);
		HL = INC16(HL, 0);
		BC = DEC16(BC, 0);
		int num2 = A - num - (fH ? 1 : 0);
		f5 = (num2 & 1) == 1;
		f3 = ((num2 >> 3) & 1) == 1;
		fPV = BC != 0;
		fC = flag;
		NumberOfTStatesLeft -= 16;
	}

	public void CPIR()
	{
		bool flag = fC;
		int num = ReadByteFromMemory(HL);
		CP(num, 0);
		HL = INC16(HL, 0);
		BC = DEC16(BC, 0);
		int num2 = A - num - (fH ? 1 : 0);
		bool flag2 = BC != 0;
		fN = true;
		fPV = flag2;
		fC = flag;
		f5 = (num2 & 1) == 1;
		f3 = ((num2 >> 3) & 1) == 1;
		if (BC != 0 && A != num)
		{
			PC = (PC - 2) & 0xFFFF;
			NumberOfTStatesLeft -= 21;
		}
		else
		{
			NumberOfTStatesLeft -= 16;
		}
	}

	private int INC16(int value, int tStates)
	{
		NumberOfTStatesLeft -= tStates;
		return (value + 1) & 0xFFFF;
	}

	private int INC8(int value, int tStates)
	{
		bool flag = value == 127;
		bool flag2 = (((value & 0xF) + 1) & F_H) != 0;
		value = (value + 1) & 0xFF;
		fS = (value & F_S) != 0;
		f3 = (value & F_3) != 0;
		f5 = (value & F_5) != 0;
		fZ = value == 0;
		fPV = flag;
		fH = flag2;
		fN = false;
		NumberOfTStatesLeft -= tStates;
		return value;
	}

	private int DEC16(int value, int tStates)
	{
		NumberOfTStatesLeft -= tStates;
		return (value - 1) & 0xFFFF;
	}

	private int DEC8(int value, int tStates)
	{
		NumberOfTStatesLeft -= tStates;
		bool flag = value == 128;
		bool flag2 = (((value & 0xF) - 1) & F_H) != 0;
		value = (value - 1) & 0xFF;
		fS = (value & F_S) != 0;
		f3 = (value & F_3) != 0;
		f5 = (value & F_5) != 0;
		fZ = value == 0;
		fPV = flag;
		fH = flag2;
		fN = true;
		return value;
	}

	private int INC8NoFlags(int a)
	{
		return (a + 1) & 0xFF;
	}

	private int DEC8NoFlags(int a)
	{
		return (a - 1) & 0xFF;
	}

	public void CPDR()
	{
		bool flag = fC;
		CP(ReadByteFromMemory(HL), 0);
		HL = DEC16(HL, 0);
		BC = DEC16(BC, 0);
		bool flag2 = (fPV = BC != 0);
		fC = flag;
		if (flag2 && !fZ)
		{
			PC = (PC - 2) & 0xFFFF;
			NumberOfTStatesLeft -= 21;
		}
		else
		{
			NumberOfTStatesLeft -= 16;
		}
	}

	public void CPL()
	{
		NumberOfTStatesLeft -= 4;
		int num = A ^ 0xFF;
		f3 = (num & F_3) != 0;
		f5 = (num & F_5) != 0;
		fH = true;
		fN = true;
		A = num;
	}

	public void DAA()
	{
		int a = A;
		int num = 0;
		bool flag = fC;
		if (fH || (a & 0xF) > 9)
		{
			num |= 6;
		}
		if (flag || a > 159 || (a > 143 && (a & 0xF) > 9))
		{
			num |= 0x60;
		}
		if (a > 153)
		{
			flag = true;
		}
		if (fN)
		{
			SUB(num, 0);
		}
		else
		{
			A = ADDADC8(A, num, Carry: false, 0);
		}
		fC = flag;
		fPV = Parity[A];
		NumberOfTStatesLeft -= 4;
	}

	public void SUB(int b, int tStates)
	{
		int a = A;
		int num = a - b;
		int num2 = num & 0xFF;
		fS = (num2 & F_S) != 0;
		f3 = (num2 & F_3) != 0;
		f5 = (num2 & F_5) != 0;
		fZ = num2 == 0;
		fC = (num & 0x100) != 0;
		fPV = ((a ^ b) & (a ^ num2) & 0x80) != 0;
		fH = (((a & 0xF) - (b & 0xF)) & F_H) != 0;
		fN = true;
		A = num2;
		NumberOfTStatesLeft -= tStates;
	}

	public void DNJZ()
	{
		B = (B - 1) & 0xFF;
		if (B != 0)
		{
			NumberOfTStatesLeft -= 13;
			PC += Sign(GetNextPCByte());
			PC++;
		}
		else
		{
			NumberOfTStatesLeft -= 8;
			PC++;
		}
	}

	public void EXX()
	{
		int bC = BC;
		BC = BCPrim;
		BCPrim = bC;
		bC = DE;
		DE = DEPrim;
		DEPrim = bC;
		bC = HL;
		HL = HLPrim;
		HLPrim = bC;
		NumberOfTStatesLeft -= 4;
	}
}
