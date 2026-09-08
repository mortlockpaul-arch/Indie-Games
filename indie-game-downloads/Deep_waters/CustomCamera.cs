using Microsoft.Xna.Framework;

namespace Deep_waters;

public class CustomCamera : Camera
{
	public Vector3 endposition = new Vector3(10f, 5f, 0f);

	public Vector3 endlookat = new Vector3(0f, 0f, 0f);

	public float Velocity = 1f;

	public CustomCamera(float visibledistance, float AspectRatio)
		: base(visibledistance, AspectRatio)
	{
		endposition = position;
		endlookat = lookat;
	}

	private void gotonextposition(GameTime gameTime)
	{
		if (position.X < endposition.X)
		{
			position.X += (float)gameTime.ElapsedGameTime.Milliseconds * Velocity;
			if (position.X >= endposition.X)
			{
				position.X = endposition.X;
			}
		}
		if (position.X > endposition.X)
		{
			position.X -= (float)gameTime.ElapsedGameTime.Milliseconds * Velocity;
			if (position.X <= endposition.X)
			{
				position.X = endposition.X;
			}
		}
		if (position.Y < endposition.Y)
		{
			position.Y += (float)gameTime.ElapsedGameTime.Milliseconds * Velocity;
			if (position.Y >= endposition.Y)
			{
				position.Y = endposition.Y;
			}
		}
		if (position.Y > endposition.Y)
		{
			position.Y -= (float)gameTime.ElapsedGameTime.Milliseconds * Velocity;
			if (position.Y <= endposition.Y)
			{
				position.Y = endposition.Y;
			}
		}
		if (position.Z < endposition.Z)
		{
			position.Z += (float)gameTime.ElapsedGameTime.Milliseconds * Velocity;
			if (position.Z >= endposition.Z)
			{
				position.Z = endposition.Z;
			}
		}
		if (position.Z > endposition.Z)
		{
			position.Z -= (float)gameTime.ElapsedGameTime.Milliseconds * Velocity;
			if (position.Z <= endposition.Z)
			{
				position.Z = endposition.Z;
			}
		}
	}

	private void gotonextlookat(GameTime gameTime)
	{
		if (lookat.X < endlookat.X)
		{
			lookat.X += (float)gameTime.ElapsedGameTime.Milliseconds * Velocity;
			if (lookat.X >= endlookat.X)
			{
				lookat.X = endlookat.X;
			}
		}
		if (lookat.X > endlookat.X)
		{
			lookat.X -= (float)gameTime.ElapsedGameTime.Milliseconds * Velocity;
			if (lookat.X <= endlookat.X)
			{
				lookat.X = endlookat.X;
			}
		}
		if (lookat.Y < endlookat.Y)
		{
			lookat.Y += (float)gameTime.ElapsedGameTime.Milliseconds * Velocity;
			if (lookat.Y >= endlookat.Y)
			{
				lookat.Y = endlookat.Y;
			}
		}
		if (lookat.Y > endlookat.Y)
		{
			lookat.Y -= (float)gameTime.ElapsedGameTime.Milliseconds * Velocity;
			if (lookat.Y <= endlookat.Y)
			{
				lookat.Y = endlookat.Y;
			}
		}
		if (lookat.Z < endlookat.Z)
		{
			lookat.Z += (float)gameTime.ElapsedGameTime.Milliseconds * Velocity;
			if (lookat.Z >= endlookat.Z)
			{
				lookat.Z = endlookat.Z;
			}
		}
		if (lookat.Z > endlookat.Z)
		{
			lookat.Z -= (float)gameTime.ElapsedGameTime.Milliseconds * Velocity;
			if (lookat.Z <= endlookat.Z)
			{
				lookat.Z = endlookat.Z;
			}
		}
	}

	public override void update(GameTime gameTime)
	{
		if (endposition != position)
		{
			gotonextposition(gameTime);
		}
		if (endlookat != lookat)
		{
			gotonextlookat(gameTime);
		}
		base.update(gameTime);
	}
}
