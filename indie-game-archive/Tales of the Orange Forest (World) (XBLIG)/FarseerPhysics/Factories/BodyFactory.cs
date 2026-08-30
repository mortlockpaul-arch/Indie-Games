using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Factories;

public static class BodyFactory
{
	public static Body CreateBody(World world)
	{
		return world.CreateBody();
	}

	public static Body CreateBody(World world, Vector2 position)
	{
		Body body = world.CreateBody();
		body.Position = position;
		return body;
	}
}
