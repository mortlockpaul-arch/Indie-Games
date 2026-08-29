using System;
using GKEngine.Cameras;
using GKEngine.Entities;
using Microsoft.Xna.Framework;

namespace GKEngine.Edit.Gysmos;

public class Gysmo : Entity3D
{
	public enum Mode
	{
		Position,
		Rotation,
		Scale
	}

	public enum Axis
	{
		None = -1,
		X,
		Y,
		Z
	}

	public enum Translation
	{
		World,
		Orbit
	}

	public const string PATH_MODEL = "./Content/_protected/Edit/Gysmos/Position";

	public GysmoModel model;

	public GysmoModelPart modelX;

	public GysmoModelPart modelY;

	public GysmoModelPart modelZ;

	public Editor editor;

	public Axis axis = Axis.None;

	public Mode mode;

	public Translation translation;

	public bool transforming;

	public Editable item;

	public Vector3 axisUnit;

	private Poly selectedPoly;

	private Plane worldPlane;

	private Vector3 worldUnit;

	private Vector3 offset;

	private Base3D startBase;

	private Vector2 startPoint;

	public Gysmo(Editor oEditor)
	{
		editor = oEditor;
		scene = oEditor.scene;
		Load();
	}

	public override void Render(GameTime oGameTime)
	{
		if (visible && model != null)
		{
			Camera camera = scene.cameras.camera;
			_ = GameEngine.instance.GraphicsDevice;
			scale = new Vector3(Vector3.Distance(camera.position, position) * 0.01f);
			model.Render(matrix, camera.view, camera.projection);
		}
	}

	public void Set(Editable oItem, Mode oMode)
	{
		item = oItem;
		mode = oMode;
		if (oItem.parent is Base3D base3D)
		{
			position = base3D.position;
			rotation = base3D.rotation;
		}
	}

	public void Transform_Start(Axis xAxis, Vector2 vPoint)
	{
		if (!(item.parent is Base3D base3D))
		{
			return;
		}
		Camera camera = scene.cameras.camera;
		axis = xAxis;
		startPoint = vPoint;
		startBase = new Base3D(base3D.matrix);
		worldUnit = Axis_Unit(axis);
		if (mode == Mode.Position)
		{
			Ray ray = scene.cameras.camera.ScreenRay(vPoint);
			worldPlane = new Plane(startBase.position, startBase.position + worldUnit, startBase.position + camera.matrix.Up);
			if (float.IsNaN(worldPlane.D) || (double)Math.Abs(Vector3.Dot(worldPlane.Normal, camera.matrix.Forward)) < 0.5)
			{
				worldPlane = new Plane(startBase.position, startBase.position + worldUnit, startBase.position + camera.matrix.Left);
			}
			float? num = ray.Intersects(worldPlane);
			if (num.HasValue)
			{
				Vector3 vFreePos = ray.Position + ray.Direction * num.Value;
				offset = Axis_LockRay(axis, vFreePos);
			}
		}
		transforming = true;
	}

	public void Transform_Update(Vector2 vPoint)
	{
		if (item.parent is Base3D base3D && transforming)
		{
			Camera camera = scene.cameras.camera;
			Ray screenRay = camera.ScreenRay(vPoint);
			switch (mode)
			{
			case Mode.Position:
				Transform_Position(base3D, screenRay, vPoint);
				break;
			case Mode.Rotation:
				Transform_Rotation(base3D, vPoint);
				break;
			case Mode.Scale:
				base3D.scale = startBase.scale + new Vector3(Vector2.Distance(startPoint, vPoint) * 0.002f * (float)Math.Sign(vPoint.X - startPoint.X));
				break;
			}
		}
	}

	public void Transform_Position(Base3D oBase, Ray screenRay, Vector2 vPoint)
	{
		float? num = screenRay.Intersects(worldPlane);
		if (!num.HasValue)
		{
			return;
		}
		if (item.translation == Translation.Orbit && (axis == Axis.X || axis == Axis.Z))
		{
			Vector3 vector = ((axis != Axis.X) ? startBase.matrix.Left : startBase.matrix.Forward);
			Base3D base3D = new Base3D();
			if (item.translationOrbitParent != null)
			{
				base3D.matrix = item.translationOrbitParent.matrix;
			}
			Base3D base3D2 = new Base3D(Matrix.Multiply(startBase.matrix, Matrix.Invert(base3D.matrix)));
			base3D.rotation *= Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(Matrix.CreateFromAxisAngle(vector, (vPoint.X - startPoint.X) * 0.001f)));
			oBase.matrix = Matrix.Multiply(base3D2.matrix, base3D.matrix);
			position = oBase.position;
			rotation = oBase.rotation;
		}
		else
		{
			Vector3 vFreePos = screenRay.Position + screenRay.Direction * num.Value;
			position = startBase.position + (Axis_LockRay(axis, vFreePos) - offset);
			oBase.position = position;
		}
	}

	public void Transform_Rotation(Base3D oBase, Vector2 vPoint)
	{
		if ((item.translation == Translation.Orbit && axis == Axis.Y) || item.translation == Translation.World)
		{
			float num = Vector2.Distance(startPoint, vPoint);
			int num2 = Math.Sign(vPoint.X - startPoint.X);
			rotation = startBase.rotation * Quaternion.CreateFromAxisAngle(Axis_IdentityUnit(axis), num * 0.01f * (float)num2);
			oBase.rotation = rotation;
		}
	}

	public override void Load()
	{
		model = GameEngine.Content.Load<GysmoModel>("./Content/_protected/Edit/Gysmos/Position");
		model.parent = this;
		for (int i = 0; i < model.modelParts.Count; i++)
		{
			switch (model.modelParts[i].name)
			{
			case "X":
				modelX = model.modelParts[i];
				break;
			case "Y":
				modelY = model.modelParts[i];
				break;
			case "Z":
				modelZ = model.modelParts[i];
				break;
			}
		}
		scene.renderStackLast.Add(guid.value, this);
	}

	public override void Dispose()
	{
		model = null;
	}

	public Axis MouseCollide(Vector2 vMouse)
	{
		Axis axis = Axis.None;
		selectedPoly = null;
		if (model != null)
		{
			for (int i = 0; i < model.modelParts.Count; i++)
			{
				if (axis != Axis.None)
				{
					break;
				}
				GysmoModelPart gysmoModelPart = model.modelParts[i];
				Poly poly = gysmoModelPart.collision.Collide_ScreenXY(vMouse, matrix, scene.cameras.camera);
				if (poly != null)
				{
					selectedPoly = poly;
					axis = Axis_FromString(gysmoModelPart.name);
				}
			}
		}
		return axis;
	}

	private Vector3 Axis_Unit(Axis xAxis)
	{
		return xAxis switch
		{
			Axis.X => matrix.Left, 
			Axis.Y => matrix.Up, 
			Axis.Z => matrix.Forward, 
			_ => matrix.Forward, 
		};
	}

	private Vector3 Axis_IdentityUnit(Axis xAxis)
	{
		return xAxis switch
		{
			Axis.X => Matrix.Identity.Left, 
			Axis.Y => Matrix.Identity.Up, 
			Axis.Z => Matrix.Identity.Forward, 
			_ => Matrix.Identity.Forward, 
		};
	}

	private Axis Axis_FromString(string xAxis)
	{
		return xAxis switch
		{
			"X" => Axis.X, 
			"Y" => Axis.Y, 
			"Z" => Axis.Z, 
			_ => Axis.None, 
		};
	}

	private Vector3 Axis_LockRay(Axis xAxis, Vector3 vFreePos)
	{
		Vector3 vector = Vector3.Transform(vFreePos, Matrix.Invert(matrix));
		switch (xAxis)
		{
		case Axis.X:
			vector = new Vector3(vector.X, 0f, 0f);
			break;
		case Axis.Y:
			vector = new Vector3(0f, vector.Y, 0f);
			break;
		case Axis.Z:
			vector = new Vector3(0f, 0f, vector.Z);
			break;
		}
		return Vector3.Transform(vector, matrix);
	}
}
