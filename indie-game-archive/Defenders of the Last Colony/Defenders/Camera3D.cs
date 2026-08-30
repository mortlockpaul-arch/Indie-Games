using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Defenders;

public class Camera3D : GameComponent
{
	private Vector3 cameraDirection;

	private Vector3 cameraUp;

	private Vector3 cameraUpVector;

	private float speed = 0.1f;

	private MouseState prevMouseState;

	public Matrix view { get; protected set; }

	public Matrix projection { get; protected set; }

	public Vector3 cameraPosition { get; protected set; }

	public Camera3D(Game game, Vector3 pos, Vector3 target, Vector3 up)
		: base(game)
	{
		cameraPosition = pos;
		cameraDirection = target - pos;
		cameraDirection.Normalize();
		cameraUpVector = up;
		cameraUp = up;
		CreateLookAt();
		projection = Matrix.CreatePerspectiveFieldOfView((float)Math.PI / 4f, (float)base.Game.Window.ClientBounds.Width / (float)base.Game.Window.ClientBounds.Height, 1f, 3000f);
	}

	private void CreateLookAt()
	{
		view = Matrix.CreateLookAt(cameraPosition, cameraPosition + cameraDirection, cameraUp);
	}

	public override void Initialize()
	{
		Mouse.SetPosition(base.Game.Window.ClientBounds.Width / 2, base.Game.Window.ClientBounds.Height / 2);
		prevMouseState = Mouse.GetState();
		base.Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		if (Keyboard.GetState().IsKeyDown(Keys.W))
		{
			cameraPosition += cameraDirection * speed;
		}
		if (Keyboard.GetState().IsKeyDown(Keys.S))
		{
			cameraPosition -= cameraDirection * speed;
		}
		if (Keyboard.GetState().IsKeyDown(Keys.A))
		{
			cameraPosition += Vector3.Cross(cameraUp, cameraDirection) * speed;
		}
		if (Keyboard.GetState().IsKeyDown(Keys.D))
		{
			cameraPosition -= Vector3.Cross(cameraUp, cameraDirection) * speed;
		}
		cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection), 0.007853982f * (float)(Mouse.GetState().Y - prevMouseState.Y)));
		cameraUp = Vector3.Transform(cameraUp, Matrix.CreateFromAxisAngle(Vector3.Cross(cameraUp, cameraDirection), 0.007853982f * (float)(Mouse.GetState().Y - prevMouseState.Y)));
		if (Mouse.GetState().LeftButton == ButtonState.Pressed)
		{
			cameraUpVector = Vector3.Transform(cameraUpVector, Matrix.CreateFromAxisAngle(cameraDirection, (float)Math.PI / 180f));
		}
		if (Mouse.GetState().RightButton == ButtonState.Pressed)
		{
			cameraUpVector = Vector3.Transform(cameraUpVector, Matrix.CreateFromAxisAngle(cameraDirection, -(float)Math.PI / 180f));
		}
		cameraDirection = Vector3.Transform(cameraDirection, Matrix.CreateFromAxisAngle(cameraUp, -0.0029088822f * (float)(Mouse.GetState().X - prevMouseState.X)));
		prevMouseState = Mouse.GetState();
		cameraUp = cameraUpVector;
		CreateLookAt();
		base.Update(gameTime);
	}
}
