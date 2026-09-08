using System.Text;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Game;

namespace AvatarFarmImport;

public static class AvatarFarmImport
{
	public static string GenerateCode(PlayerIndex who, uint xp, uint coins, uint cash)
	{
		ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(who);
		if (gamer == null)
		{
			return "";
		}
		string gamertag = gamer.Gamertag;
		uint hashCode = (uint)gamertag.GetHashCode();
		StringBuilder stringBuilder = new StringBuilder(100);
		GetChars(xp + (hashCode & 0xFF0000), stringBuilder);
		GetChars(coins + (hashCode & 0xFF00), stringBuilder);
		GetChars(cash + (hashCode & 0xFF), stringBuilder);
		hashCode = (hashCode << 2) + (xp >> 2) * 67 + (coins << 2) * 24 + (cash << 3) * 12;
		GetChars(hashCode, stringBuilder);
		int length = stringBuilder.Length;
		if (length % 6 > 0)
		{
			int num = 6 - length % 6;
			for (int i = 0; i < num; i++)
			{
				stringBuilder.Append((char)(67 + i * 3));
			}
		}
		int num2 = stringBuilder.Length / 6 - 1;
		for (int j = 0; j < num2; j++)
		{
			stringBuilder.Insert(6 * (j + 1) + j, " ");
		}
		return stringBuilder.ToString();
	}

	public static bool CheckCode(PlayerIndex who, string text, out uint xp, out uint coins, out uint cash)
	{
		text = text.Replace(" ", "");
		text = text.Replace("-", "");
		ISignedInGamer gamer = PlatformInterface.Instance.GetGamer(who);
		if (gamer == null)
		{
			xp = (coins = (cash = 0u));
			return false;
		}
		string gamertag = gamer.Gamertag;
		uint hashCode = (uint)gamertag.GetHashCode();
		int index = 0;
		xp = ReadChars(text, ref index) - (hashCode & 0xFF0000);
		coins = ReadChars(text, ref index) - (hashCode & 0xFF00);
		cash = ReadChars(text, ref index) - (hashCode & 0xFF);
		uint num = ReadChars(text, ref index);
		hashCode = (hashCode << 2) + (xp >> 2) * 67 + (coins << 2) * 24 + (cash << 3) * 12;
		return num == hashCode;
	}

	private static void GetChars(uint value, StringBuilder sb)
	{
		uint num = 0u;
		for (uint num2 = value; num2 != 0; num2 /= 36)
		{
			num++;
		}
		sb.Append(GetChar(num));
		for (uint num2 = value; num2 != 0; num2 /= 36)
		{
			uint value2 = num2 % 36;
			sb.Append(GetChar(value2));
		}
	}

	private static uint ReadChars(string text, ref int index)
	{
		if (text.Length <= index)
		{
			return 0u;
		}
		char c = text[index];
		uint charVal = GetCharVal(c);
		index++;
		uint num = 0u;
		for (int i = 0; i < charVal; i++)
		{
			if (text.Length <= index)
			{
				return 0u;
			}
			uint num2 = GetCharVal(text[index++]);
			for (int j = 0; j < i; j++)
			{
				num2 *= 36;
			}
			num += num2;
		}
		return num;
	}

	private static uint GetCharVal(char c)
	{
		c = char.ToUpperInvariant(c);
		if (c >= 'A' && c <= 'Z')
		{
			return (uint)(c - 65);
		}
		if (c >= '0' && c <= '9')
		{
			return (uint)(c - 48 + 26);
		}
		return 0u;
	}

	private static char GetChar(uint value)
	{
		switch (value)
		{
		case 0u:
		case 1u:
		case 2u:
		case 3u:
		case 4u:
		case 5u:
		case 6u:
		case 7u:
		case 8u:
		case 9u:
		case 10u:
		case 11u:
		case 12u:
		case 13u:
		case 14u:
		case 15u:
		case 16u:
		case 17u:
		case 18u:
		case 19u:
		case 20u:
		case 21u:
		case 22u:
		case 23u:
		case 24u:
		case 25u:
			return (char)(65 + (ushort)value);
		case 26u:
		case 27u:
		case 28u:
		case 29u:
		case 30u:
		case 31u:
		case 32u:
		case 33u:
		case 34u:
		case 35u:
			return (char)(48 + (ushort)(value - 26));
		default:
			return ' ';
		}
	}
}
