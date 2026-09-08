using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;

namespace Quasar;

public class Transform
{
	private Matrix parent = Matrix.Identity;

	private bool updated;

	private bool worldUpdated;

	private Matrix worldMatrix = Matrix.Identity;

	private bool ignoreParent;

	private Matrix matrix = Matrix.Identity;

	private Vector3 scale = new Vector3(1f, 1f, 1f);

	private bool scaleUpdated = true;

	private Matrix scaleMatrix = Matrix.Identity;

	private bool scaleBoundsUpdated;

	private float scaleBounds = 1f;

	private Vector3 translation = default(Vector3);

	private bool translationUpdated = true;

	private Matrix translationMatrix = Matrix.Identity;

	private Quaternion rotation = Quaternion.Identity;

	private bool rotationUpdated = true;

	private Matrix rotationMatrix = Matrix.Identity;

	public Matrix Parent
	{
		get
		{
			return parent;
		}
		set
		{
			parent = value;
			if (!ignoreParent)
			{
				worldUpdated = false;
			}
		}
	}

	public bool IgnoreParent
	{
		get
		{
			return ignoreParent;
		}
		set
		{
			if (ignoreParent != value)
			{
				ignoreParent = value;
				worldUpdated = false;
			}
		}
	}

	public Matrix WorldMatrix
	{
		get
		{
			if (!worldUpdated)
			{
				updateWorld();
			}
			return worldMatrix;
		}
	}

	public Vector3 WorldX
	{
		get
		{
			if (!worldUpdated)
			{
				updateWorld();
			}
			return worldMatrix.Right;
		}
	}

	public Vector3 WorldY
	{
		get
		{
			if (!worldUpdated)
			{
				updateWorld();
			}
			return worldMatrix.Up;
		}
	}

	public Vector3 WorldZ
	{
		get
		{
			if (!worldUpdated)
			{
				updateWorld();
			}
			return worldMatrix.Backward;
		}
	}

	public Vector3 WorldTranslation
	{
		get
		{
			if (!worldUpdated)
			{
				updateWorld();
			}
			return worldMatrix.Translation;
		}
	}

	public Quaternion WorldRotation => Quaternion.CreateFromRotationMatrix(WorldMatrix);

	public Matrix Matrix
	{
		get
		{
			if (!updated)
			{
				updateMatrix();
			}
			return matrix;
		}
		set
		{
			matrix = value;
			updated = true;
			DecomposeMatrix();
			worldUpdated = false;
		}
	}

	public Vector3 Scale
	{
		get
		{
			return scale;
		}
		set
		{
			scale = value;
			scaleUpdated = false;
			updated = false;
			worldUpdated = false;
		}
	}

	public Vector3 WorldScale
	{
		get
		{
			if (!worldUpdated)
			{
				updateWorld();
			}
			return new Vector3(new Vector3(worldMatrix.M11, worldMatrix.M21, worldMatrix.M31).Length(), new Vector3(worldMatrix.M12, worldMatrix.M22, worldMatrix.M32).Length(), new Vector3(worldMatrix.M13, worldMatrix.M23, worldMatrix.M33).Length());
		}
	}

	public float ScaleBounds
	{
		get
		{
			if (!worldUpdated)
			{
				updateWorld();
			}
			if (!scaleBoundsUpdated)
			{
				scaleBounds = (float)Math.Sqrt(Math.Max(Math.Max(worldMatrix.M11 * worldMatrix.M11 + worldMatrix.M21 * worldMatrix.M21 + worldMatrix.M31 * worldMatrix.M31, worldMatrix.M12 * worldMatrix.M12 + worldMatrix.M22 * worldMatrix.M22 + worldMatrix.M32 * worldMatrix.M32), worldMatrix.M13 * worldMatrix.M13 + worldMatrix.M23 * worldMatrix.M23 + worldMatrix.M33 * worldMatrix.M33));
				scaleBoundsUpdated = true;
			}
			return scaleBounds;
		}
	}

	public Vector3 Translation
	{
		get
		{
			return translation;
		}
		set
		{
			translation = value;
			translationUpdated = false;
			updated = false;
			worldUpdated = false;
		}
	}

	public Quaternion Rotation
	{
		get
		{
			return rotation;
		}
		set
		{
			rotation = value;
			rotationUpdated = false;
			updated = false;
			worldUpdated = false;
		}
	}

	public Vector3 XVector => Vector3.Transform(Vector3.UnitX, rotation);

	public Vector3 YVector => Vector3.Transform(Vector3.UnitY, rotation);

	public Vector3 ZVector => Vector3.Transform(Vector3.UnitZ, rotation);

	public void SetParent(Transform transform)
	{
		if (!transform.worldUpdated)
		{
			transform.updateWorld();
		}
		if (!GameMath.MatrixEquals(ref transform.worldMatrix, ref parent))
		{
			parent = transform.worldMatrix;
			worldUpdated = false;
		}
	}

	public void LookAt(Vector3 position, Vector3 target, Vector3 upVector)
	{
		Matrix = Matrix.Invert(Matrix.CreateLookAt(position, target, upVector));
	}

	private void DecomposeMatrix()
	{
		if (matrix.Decompose(out var vector, out var quaternion, out var vector2))
		{
			if (scale != vector)
			{
				scale = vector;
				scaleUpdated = false;
			}
			if (rotation != quaternion)
			{
				rotation = quaternion;
				rotationUpdated = false;
			}
			if (translation != vector2)
			{
				translation = vector2;
				translationUpdated = false;
			}
		}
	}

	public void Assign(Transform motion)
	{
		parent = motion.parent;
		matrix = motion.matrix;
		rotation = motion.rotation;
		scale = motion.scale;
		translation = motion.translation;
		updated = motion.updated;
		worldMatrix = motion.worldMatrix;
		worldUpdated = motion.worldUpdated;
		ignoreParent = motion.ignoreParent;
		rotationMatrix = motion.rotationMatrix;
		rotationUpdated = motion.rotationUpdated;
		scaleMatrix = motion.scaleMatrix;
		scaleUpdated = motion.scaleUpdated;
		translationMatrix = motion.translationMatrix;
		translationUpdated = motion.translationUpdated;
	}

	protected void updateMatrix()
	{
		if (!scaleUpdated || !rotationUpdated || !translationUpdated)
		{
			if (!scaleUpdated)
			{
				Matrix.CreateScale(ref scale, out scaleMatrix);
				scaleUpdated = true;
			}
			if (!rotationUpdated)
			{
				Matrix.CreateFromQuaternion(ref rotation, out rotationMatrix);
				rotationUpdated = true;
			}
			if (!translationUpdated)
			{
				Matrix.CreateTranslation(ref translation, out translationMatrix);
				translationUpdated = true;
			}
			Matrix.Multiply(ref scaleMatrix, ref rotationMatrix, out matrix);
			Matrix.Multiply(ref matrix, ref translationMatrix, out matrix);
			worldUpdated = false;
		}
		updated = true;
	}

	protected void updateWorld()
	{
		if (!updated)
		{
			updateMatrix();
		}
		if (ignoreParent)
		{
			worldMatrix = matrix;
		}
		else
		{
			Matrix.Multiply(ref matrix, ref parent, out worldMatrix);
		}
		worldUpdated = true;
		scaleBoundsUpdated = false;
	}

	public void Update()
	{
		if (!updated)
		{
			updateMatrix();
		}
	}

	public void Reset()
	{
		Parent = Matrix.Identity;
		Translation = Vector3.Zero;
		Rotation = Quaternion.Identity;
		Scale = Vector3.One;
	}

	public void TransformPoint(ref Vector3 point, out Vector3 result)
	{
		if (!worldUpdated)
		{
			updateWorld();
		}
		Vector3.Transform(ref point, ref worldMatrix, out result);
	}

	public Vector3 TransformPoint(Vector3 point)
	{
		if (!worldUpdated)
		{
			updateWorld();
		}
		Vector3 position = point;
		Vector3.Transform(ref position, ref worldMatrix, out var result);
		return result;
	}

	public void RotateVector(ref Vector3 direction, out Vector3 result)
	{
		TransformPoint(ref direction, out result);
		result -= WorldTranslation;
	}

	public void RotateVectorLocal(ref Vector3 direction, out Vector3 result)
	{
		if (rotation != Quaternion.Identity)
		{
			Vector3.Transform(ref direction, ref rotation, out result);
		}
		else
		{
			result = direction;
		}
	}

	public static void Multiply(Transform motion, Transform motion2, out Matrix result)
	{
		if (!motion.worldUpdated)
		{
			motion.updateWorld();
		}
		if (!motion2.worldUpdated)
		{
			motion2.updateWorld();
		}
		Matrix.Multiply(ref motion.worldMatrix, ref motion2.worldMatrix, out result);
	}

	public static void Multiply(Transform motion, ref Matrix motion2, out Matrix result)
	{
		if (!motion.worldUpdated)
		{
			motion.updateWorld();
		}
		Matrix.Multiply(ref motion.worldMatrix, ref motion2, out result);
	}

	public static implicit operator Matrix(Transform motion)
	{
		return motion.WorldMatrix;
	}

	public void FromXml(XElement xe)
	{
		Translation = XDocHelper.ParseVector3Attribute(xe, "translation", Vector3.Zero);
		Vector3 vector = XDocHelper.ParseVector3Attribute(xe, "rotation", Vector3.Zero) * ((float)Math.PI / 180f);
		Rotation = Quaternion.CreateFromYawPitchRoll(vector.Y, vector.X, vector.Z);
		Scale = XDocHelper.ParseVector3Attribute(xe, "scale", Vector3.One);
	}
}
