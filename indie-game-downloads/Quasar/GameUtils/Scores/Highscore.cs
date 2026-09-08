using System;
using System.Text;
using Quasar.Global;

namespace Quasar.GameUtils.Scores;

public abstract class Highscore : IComparable<Highscore>, IEquatable<Highscore>
{
	protected const int ComponentNumber = 2;

	public static bool ReverseOrder;

	public DateTime When;

	public string Gamer = new string(' ', 21);

	protected char[] gamerChars = new char[21];

	protected int gamerLength;

	public bool IsLocal;

	public bool IsFake;

	public virtual long CheckSum => When.Ticks / 13687124537L + Gamer.GetHashCode() / 4;

	protected Highscore(string gamertag, DateTime when, bool isLocal)
	{
		When = when.AddMilliseconds(-when.Millisecond);
		ReadGamer(gamertag);
		IsLocal = isLocal;
	}

	public bool CompareGamer(string gamertag)
	{
		if (gamertag.Length == gamerLength && string.Compare(gamertag, 0, Gamer, 0, gamerLength) == 0)
		{
			return true;
		}
		return false;
	}

	protected void UpdateGamerFromChars()
	{
		GameMath.CopyIntoString(ref Gamer, gamerChars, 21);
		gamerLength = Gamer.IndexOf('\0');
	}

	protected void ReadGamer(string gamertag)
	{
		int i = 0;
		gamerLength = gamertag.IndexOf('\0');
		if (gamerLength == -1)
		{
			gamerLength = Math.Min(gamertag.Length, 20);
		}
		for (; i < gamerLength; i++)
		{
			gamerChars[i] = gamertag[i];
		}
		for (; i < 21; i++)
		{
			gamerChars[i] = '\0';
		}
		GameMath.CopyIntoString(ref Gamer, gamerChars, 21);
	}

	protected Highscore()
	{
		ReadGamer("");
	}

	public void Encode(StringBuilder sb)
	{
		DoEncode(sb);
		sb.Append(";");
		sb.AppendNumber(CheckSum, AppendNumberOptions.None);
	}

	protected virtual void DoEncode(StringBuilder sb)
	{
		sb.AppendNumber(When.Ticks, AppendNumberOptions.None);
		sb.Append(";");
		sb.Append(Gamer);
	}

	protected virtual bool DoDecode(string[] data)
	{
		if (data.Length < 2)
		{
			return false;
		}
		When = new DateTime(GameMath.ParseLong(data[0]));
		ReadGamer(data[1]);
		return true;
	}

	public bool Decode(string str)
	{
		IsFake = false;
		IsLocal = false;
		if (str == null)
		{
			return false;
		}
		string[] array = str.Split(';');
		if (!DoDecode(array))
		{
			return false;
		}
		long num = GameMath.ParseLong(array[array.Length - 1]);
		if (num != CheckSum)
		{
			return false;
		}
		return true;
	}

	public virtual int CompareTo(Highscore other)
	{
		if (other == null)
		{
			return -1;
		}
		if (other.When > When)
		{
			return 1;
		}
		if (other.When < When)
		{
			return -1;
		}
		return other.Gamer.CompareTo(Gamer);
	}

	public virtual bool Equals(Highscore other)
	{
		if (Gamer == other.Gamer)
		{
			return When == other.When;
		}
		return false;
	}
}
