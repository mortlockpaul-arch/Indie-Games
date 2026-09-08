using System.IO;

namespace Loot.Items;

public abstract class Junk : Item
{
	public Junk()
	{
		Identify();
	}

	public Junk(BinaryReader reader)
		: base(reader)
	{
	}

	public override void onActivate(int id)
	{
	}
}
