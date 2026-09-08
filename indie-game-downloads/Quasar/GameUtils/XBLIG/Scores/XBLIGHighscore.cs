using System;
using System.IO;
using Quasar.GameUtils.Scores;

namespace Quasar.GameUtils.XBLIG.Scores;

public class XBLIGHighscore : Highscore
{
	public XBLIGHighscore()
	{
	}

	public XBLIGHighscore(string gamertag, DateTime when, bool isLocal)
		: base(gamertag, when, isLocal)
	{
	}

	public virtual void Write(BinaryWriter writer)
	{
		writer.Write(Gamer ?? string.Empty);
		writer.Write(When.ToBinary());
		writer.Write(IsLocal);
		writer.Write(IsFake);
	}

	public virtual bool Read(BinaryReader reader)
	{
		Gamer = reader.ReadString();
		When = DateTime.FromBinary(reader.ReadInt64());
		IsLocal = reader.ReadBoolean();
		IsFake = reader.ReadBoolean();
		return true;
	}
}
