using System.IO;
using Eyehook.Framework;
using Loot.Dungeon;
using Microsoft.Xna.Framework;

namespace Loot.Statuses;

public abstract class StatusEffect : BinaryRW
{
	public StatusEffect()
	{
	}

	public StatusEffect(BinaryReader reader)
	{
		Read(reader);
	}

	public abstract void Update(GameTime gameTime, Character character);

	public abstract void Read(BinaryReader reader);

	public abstract void Write(BinaryWriter writer);
}
