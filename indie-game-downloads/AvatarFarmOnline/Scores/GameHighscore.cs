using System;
using System.IO;
using System.Text;
using Quasar.GameUtils.Scores;
using Quasar.GameUtils.XBLIG.Scores;
using Quasar.Global;

namespace AvatarFarmOnline.Scores;

internal class GameHighscore : XBLIGHighscore, IComparable<AvatarFarmOnline.Scores.GameHighscore>, IEquatable<AvatarFarmOnline.Scores.GameHighscore>
{
	private uint level;

	private int cash;

	private int coins;

	private int ticks;

	public uint Level => level;

	public int Cash => cash;

	public int Coins => coins;

	public int Ticks => ticks;

	public override long CheckSum => (coins + cash) * level / 7 + When.Ticks / 13687124537L + Gamer.GetHashCode() / 4;

	public GameHighscore()
	{
	}

	public GameHighscore(string gamertag, DateTime when, uint level, int coins, int cash, int ticks, bool IsLocal)
		: base(gamertag, when, IsLocal)
	{
		this.level = level;
		this.cash = cash;
		this.coins = coins;
		this.ticks = ticks;
	}

	protected override void DoEncode(StringBuilder sb)
	{
		base.DoEncode(sb);
		sb.Append(";");
		sb.AppendNumber(level, AppendNumberOptions.None);
		sb.Append(";");
		sb.AppendNumber(cash, AppendNumberOptions.None);
		sb.Append(";");
		sb.AppendNumber(coins, AppendNumberOptions.None);
		sb.Append(";");
		sb.AppendNumber(ticks, AppendNumberOptions.None);
	}

	protected override bool DoDecode(string[] data)
	{
		if (!base.DoDecode(data))
		{
			return false;
		}
		if (data.Length < 6)
		{
			return false;
		}
		level = GameMath.ParseUInt(data[2]);
		cash = GameMath.ParseInt(data[3]);
		coins = GameMath.ParseInt(data[4]);
		ticks = GameMath.ParseInt(data[5]);
		return true;
	}

	public override void Write(BinaryWriter wr)
	{
		base.Write(wr);
		wr.Write(level);
		wr.Write(cash);
		wr.Write(coins);
		wr.Write(ticks);
	}

	public override bool Read(BinaryReader rd)
	{
		if (!base.Read(rd))
		{
			return false;
		}
		level = rd.ReadUInt32();
		cash = rd.ReadInt32();
		coins = rd.ReadInt32();
		ticks = rd.ReadInt32();
		return true;
	}

	public int CompareTo(AvatarFarmOnline.Scores.GameHighscore ghs)
	{
		if (ghs == null)
		{
			return -1;
		}
		if (ghs.Level > Level)
		{
			if (!Highscore.ReverseOrder)
			{
				return 1;
			}
			return -1;
		}
		if (ghs.Level < Level)
		{
			if (!Highscore.ReverseOrder)
			{
				return -1;
			}
			return 1;
		}
		if (ghs.Coins > Coins)
		{
			if (!Highscore.ReverseOrder)
			{
				return 1;
			}
			return -1;
		}
		if (ghs.Coins < Coins)
		{
			if (!Highscore.ReverseOrder)
			{
				return -1;
			}
			return 1;
		}
		if (ghs.Cash > Cash)
		{
			if (!Highscore.ReverseOrder)
			{
				return 1;
			}
			return -1;
		}
		if (ghs.Cash < Cash)
		{
			if (!Highscore.ReverseOrder)
			{
				return -1;
			}
			return 1;
		}
		if (ghs.Ticks > Ticks)
		{
			if (!Highscore.ReverseOrder)
			{
				return 1;
			}
			return -1;
		}
		if (ghs.Ticks < Ticks)
		{
			if (!Highscore.ReverseOrder)
			{
				return -1;
			}
			return 1;
		}
		return base.CompareTo(ghs);
	}

	public bool Equals(AvatarFarmOnline.Scores.GameHighscore whs)
	{
		if (whs == null)
		{
			return false;
		}
		if (whs.level == level && whs.ticks == ticks && whs.coins == coins && whs.cash == cash)
		{
			return base.Equals(whs);
		}
		return false;
	}
}
