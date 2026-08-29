using GKEngine.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Cameras;

public class Camera : Base3D
{
	public enum ProjectionMode
	{
		Perspective,
		Orthagonal
	}

	public static float DEFAULT_FOV = 45f;

	public static float DEFAULT_FOV_MIN = 1f;

	public static float DEFAULT_FOV_MAX = 179f;

	public static float DEFAULT_NEAR = 1f;

	public static float DEFAULT_FAR = 100000f;

	protected Viewport _viewport;

	protected float _fov;

	protected ProjectionMode _projectionMode;

	public CameraManager manager;

	public string name;

	public Matrix view;

	public Matrix projection;

	public float orthoZoom = 1f;

	public float focalLength = 1f;

	public ProjectionMode projectionMode
	{
		get
		{
			return _projectionMode;
		}
		set
		{
			_projectionMode = value;
			Update_Projection();
		}
	}

	public float fov
	{
		get
		{
			return _fov;
		}
		set
		{
			_fov = MathHelper.Clamp(value, DEFAULT_FOV_MIN, DEFAULT_FOV_MAX);
			Update_Projection();
		}
	}

	public Viewport viewport
	{
		get
		{
			return _viewport;
		}
		set
		{
			_viewport = value;
			_viewport.MinDepth = DEFAULT_NEAR;
			_viewport.MaxDepth = DEFAULT_FAR;
			Update_Projection();
		}
	}

	public override Vector3 position
	{
		get
		{
			return _position;
		}
		set
		{
			_position = value;
			Update_View();
		}
	}

	public override Quaternion rotation
	{
		get
		{
			return _rotation;
		}
		set
		{
			_rotation = value;
			Update_View();
		}
	}

	public override Vector3 scale
	{
		get
		{
			return _scale;
		}
		set
		{
			_scale = value;
			Update_View();
		}
	}

	public override Matrix matrix
	{
		get
		{
			return Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
		}
		set
		{
			value.Decompose(out var vector, out var quaternion, out var translation);
			_position = translation;
			_rotation = Quaternion.Normalize(quaternion);
			_scale = vector;
			Update_View();
		}
	}

	public override float X
	{
		get
		{
			return _position.X;
		}
		set
		{
			_position.X = value;
			Update_View();
		}
	}

	public override float Y
	{
		get
		{
			return _position.Y;
		}
		set
		{
			_position.Y = value;
			Update_View();
		}
	}

	public override float Z
	{
		get
		{
			return _position.Z;
		}
		set
		{
			_position.Z = value;
			Update_View();
		}
	}

	public Camera(string xName, Viewport oViewport, CameraManager oManager)
		: base(Matrix.Identity)
	{
		name = xName;
		manager = oManager;
		_fov = DEFAULT_FOV;
		_projectionMode = ProjectionMode.Perspective;
		viewport = oViewport;
		Update_View();
	}

	public void Update_Projection()
	{
		Update_Projection((float)_viewport.Width / (float)_viewport.Height);
	}

	public void Update_Projection(float xAspectRatio)
	{
		if (projectionMode == ProjectionMode.Perspective)
		{
			projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(_fov), xAspectRatio, _viewport.MinDepth, _viewport.MaxDepth);
		}
		else if (projectionMode == ProjectionMode.Orthagonal)
		{
			projection = Matrix.CreateOrthographic((float)_viewport.Width * orthoZoom, (float)_viewport.Height * orthoZoom, _viewport.MinDepth, _viewport.MaxDepth * orthoZoom);
		}
	}

	public void Update_View()
	{
		view = Matrix.Invert(Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position));
	}

	public string GetAttributes()
	{
		string text = "Camera Data\n";
		text = "Pos: " + position.ToString() + "\n";
		return text + "Rot: " + rotation.ToString() + "\n";
	}

	public Ray ScreenRay(Vector2 vPoint)
	{
		Vector3 screenSpace = new Vector3(vPoint, viewport.MinDepth);
		Vector3 screenSpace2 = new Vector3(vPoint, viewport.MaxDepth);
		screenSpace = Unproject(screenSpace);
		screenSpace2 = Unproject(screenSpace2);
		Vector3 direction = screenSpace2 - screenSpace;
		direction.Normalize();
		return new Ray(screenSpace, direction);
	}

	public Vector3 Unproject(Vector3 screenSpace)
	{
		Vector4 vector = new Vector4(new Vector3
		{
			X = (screenSpace.X - (float)viewport.X) / (float)viewport.Width * 2f - 1f,
			Y = 0f - ((screenSpace.Y - (float)viewport.Y) / (float)viewport.Height * 2f - 1f),
			Z = (screenSpace.Z - viewport.MinDepth) / (viewport.MaxDepth - viewport.MinDepth)
		}, 1f);
		Matrix matrix = Matrix.Invert(projection);
		Matrix matrix2 = Matrix.Invert(view);
		Vector4 vector2 = Vector4.Transform(vector, matrix);
		Vector4 vector3 = Vector4.Transform(vector2, matrix2);
		Vector3 vector4 = new Vector3(vector3.X, vector3.Y, vector3.Z);
		return vector4 / vector3.W;
	}

	public override string ToString()
	{
		return "Camera: Pos:" + position.ToString();
	}
}
