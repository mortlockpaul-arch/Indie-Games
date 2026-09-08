using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.Factories;

public static class BodyFactory
{
	public static Body CreateBody(World world)
	{
		return CreateBody(world, null);
	}

	public static Body CreateBody(World world, object userData)
	{
		return new Body(world, userData);
	}

	public static Body CreateBody(World world, Vector2 position)
	{
		return CreateBody(world, position, null);
	}

	public static Body CreateBody(World world, Vector2 position, object userData)
	{
		Body body = CreateBody(world, userData);
		body.Position = position;
		return body;
	}
}
