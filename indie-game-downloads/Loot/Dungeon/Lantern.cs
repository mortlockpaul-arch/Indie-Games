using System.IO;
using Eyehook.Framework;
using Microsoft.Xna.Framework;

namespace Loot.Dungeon;

public class Lantern : BinaryRW
{
	private const double maxFuel = 480.0;

	private const double fade = 240.0;

	private const float minRadius = 0.25f;

	private double fuel;

	public float Fuel => (float)(fuel / 480.0);

	public float Radius => (fuel > 240.0) ? 1f : MathHelper.Clamp((float)DM.Player.SkillSet.Perception.Level / 10f + (float)(fuel / 240.0), 0.25f, 1f);

	public Lantern()
	{
		fuel = 480.0;
	}

	public void Update(GameTime gameTime)
	{
		fuel -= gameTime.ElapsedGameTime.TotalSeconds;
		if (!(fuel >= 0.0))
		{
			fuel = 0.0;
		}
	}

	public void Refill()
	{
		fuel = 480.0;
		DM.Discover(DM.Player.Location);
	}

	public void Read(BinaryReader reader)
	{
		fuel = reader.ReadDouble();
	}

	public void Write(BinaryWriter writer)
	{
		writer.Write(fuel);
	}
}
