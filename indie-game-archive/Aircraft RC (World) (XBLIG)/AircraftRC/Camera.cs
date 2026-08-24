using Microsoft.Xna.Framework;

namespace AircraftRC;

public class Camera : GameComponent
{
	public Vector3 target;

	public Vector3 viewVector;

	private float aspectRatio;

	private float nearPlane;

	public float farPlane;

	public Matrix view { get; set; }

	public Matrix projection { get; set; }

	public Vector3 position { get; set; }

	public float fov { get; set; }

	public Vector3 Target
	{
		set
		{
			target = value;
		}
	}

	public float Fov
	{
		set
		{
			fov = value;
		}
	}

	public Matrix View
	{
		get
		{
			view = Matrix.CreateLookAt(position, target, new Vector3(0f, 1f, 0f));
			return view;
		}
	}

	public Matrix Projection
	{
		get
		{
			projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fov), aspectRatio, nearPlane, farPlane);
			return projection;
		}
	}

	public Camera(Game game)
		: base(game)
	{
		aspectRatio = (float)game.Window.ClientBounds.Width / (float)game.Window.ClientBounds.Height;
		fov = 47f;
		nearPlane = 0.5f;
		farPlane = 5000000f;
	}

	public override void Initialize()
	{
		position = new Vector3(0f, 0f, 0f);
		target = new Vector3(0f, 2f, 0f);
		base.Initialize();
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}
}
