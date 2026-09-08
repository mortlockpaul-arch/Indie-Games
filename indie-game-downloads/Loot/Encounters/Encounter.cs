using System.IO;
using Eyehook.Framework;
using Loot.Awardments;
using Loot.Dungeon;
using Loot.Effects;
using Loot.NPCs;
using Microsoft.Xna.Framework;

namespace Loot.Encounters;

public abstract class Encounter : BinaryRW
{
	public Location Location;

	protected int depth => DM.Player.Depth;

	protected bool goodOutcome => DM.Player.IsLucky();

	public Encounter(Location loc)
	{
		Location = loc;
	}

	public Encounter(BinaryReader reader)
	{
		Read(reader);
	}

	public virtual void Update(GameTime gameTime)
	{
	}

	public virtual void OnClick()
	{
		Profile.Awardments.Unlock(Awardment.Encounter);
		Remove();
		DM.ClearEffects();
	}

	public virtual void Remove()
	{
		DM.Map.ClearWidget(Location);
	}

	protected Location NearestOpenFloor()
	{
		return DM.Map.FindNearest(DM.Player.Location, DM.Map.IsOpenFloor);
	}

	protected void PoofNPC(NPC npc)
	{
		DM.AddNPC(npc);
		DM.AddEffect(new FXPoof(npc));
	}

	public virtual void Read(BinaryReader reader)
	{
		Location.Read(reader);
	}

	public virtual void Write(BinaryWriter writer)
	{
		Location.Write(writer);
	}
}
