using System.IO;

namespace ZP2K9.store;

public class Settings
{
	public const int VERSION = 123;

	public bool rumble = true;

	public int curTeam;

	public bool showNames = true;

	public bool vibration = true;

	public bool autoSwitch = true;

	public bool allowHandicapping = true;

	public bool upToJetpack = true;

	public bool twinStickShooter = true;

	public int sfx = 10;

	public int bgm = 10;

	public void Write(BinaryWriter writer)
	{
		writer.Write(rumble);
		writer.Write(showNames);
		writer.Write(vibration);
		writer.Write(autoSwitch);
		writer.Write(allowHandicapping);
		writer.Write(upToJetpack);
		writer.Write(twinStickShooter);
		writer.Write(sfx);
		writer.Write(bgm);
		Game1.zProfile.Write(writer);
	}

	public void Read(BinaryReader reader)
	{
		rumble = reader.ReadBoolean();
		showNames = reader.ReadBoolean();
		vibration = reader.ReadBoolean();
		autoSwitch = reader.ReadBoolean();
		allowHandicapping = reader.ReadBoolean();
		upToJetpack = reader.ReadBoolean();
		twinStickShooter = reader.ReadBoolean();
		sfx = reader.ReadInt32();
		bgm = reader.ReadInt32();
		Game1.zProfile.Read(reader);
	}
}
