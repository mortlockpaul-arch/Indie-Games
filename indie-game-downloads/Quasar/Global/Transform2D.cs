using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Quasar.Global;

public class Transform2D
{
	private Matrix3 parent = Matrix3.Identity;

	private bool updated;

	private bool worldUpdated;

	private Matrix3 worldMatrix = Matrix3.Identity;

	private bool ignoreParent;

	private Matrix3 matrix = Matrix3.Identity;

	private Vector2 scale = new Vector2(1f, 1f);

	private bool scaleUpdated = true;

	private Matrix3 scaleMatrix = Matrix3.Identity;

	private Vector2 translation = default(Vector2);

	private bool translationUpdated = true;

	private Matrix3 translationMatrix = Matrix3.Identity;

	private float rotation;

	private bool rotationUpdated = true;

	private Matrix3 rotationMatrix = Matrix3.Identity;

	public Matrix3 Parent
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

	public Matrix3 WorldMatrix
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

	public Vector2 WorldTranslation => WorldMatrix.Translation;

	public Matrix3 Matrix
	{
		get
		{
			if (!updated)
			{
				updateMatrix();
			}
			return matrix;
		}
	}

	public Vector2 Scale
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

	public Vector2 WorldScale
	{
		get
		{
			if (!worldUpdated)
			{
				updateWorld();
			}
			return new Vector2(new Vector2(worldMatrix.M11, worldMatrix.M21).Length(), new Vector2(worldMatrix.M12, worldMatrix.M22).Length());
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
			return (float)Math.Sqrt(Math.Max(new Vector2(worldMatrix.M11, worldMatrix.M21).LengthSquared(), new Vector2(worldMatrix.M12, worldMatrix.M22).LengthSquared()));
		}
	}

	public Vector2 Translation
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

	public float Rotation
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

	public Vector2 XVector => GameMath.RotateVector(Vector2.UnitX, rotation);

	public Vector2 YVector => GameMath.RotateVector(Vector2.UnitY, rotation);

	public void Assign(Transform2D motion)
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
				Matrix3.CreateScale(ref scale, out scaleMatrix);
				scaleUpdated = true;
			}
			if (!rotationUpdated)
			{
				Matrix3.CreateRotation(rotation, out rotationMatrix);
				rotationUpdated = true;
			}
			if (!translationUpdated)
			{
				Matrix3.CreateTranslation(ref translation, out translationMatrix);
				translationUpdated = true;
			}
			Matrix3.Multiply(ref rotationMatrix, ref scaleMatrix, out matrix);
			Matrix3.Multiply(ref matrix, ref translationMatrix, out matrix);
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
			Matrix3.Multiply(ref matrix, ref parent, out worldMatrix);
		}
		worldUpdated = true;
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
		Parent = Matrix3.Identity;
		Translation = Vector2.Zero;
		Rotation = 0f;
		Scale = Vector2.One;
	}

	public void TransformPoint(ref Vector2 point, out Vector2 result)
	{
		if (!worldUpdated)
		{
			updateWorld();
		}
		Matrix3.Transform(ref point, ref worldMatrix, out result);
	}

	public void RotateVector(ref Vector2 direction, out Vector2 result)
	{
		TransformPoint(ref direction, out result);
		result -= WorldTranslation;
	}

	public static void Multiply(Transform2D motion, Transform2D motion2, out Matrix3 result)
	{
		if (!motion.worldUpdated)
		{
			motion.updateWorld();
		}
		if (!motion2.worldUpdated)
		{
			motion2.updateWorld();
		}
		Matrix3.Multiply(ref motion.worldMatrix, ref motion2.worldMatrix, out result);
	}

	public static void Multiply(Transform2D motion, ref Matrix3 motion2, out Matrix3 result)
	{
		if (!motion.worldUpdated)
		{
			motion.updateWorld();
		}
		Matrix3.Multiply(ref motion.worldMatrix, ref motion2, out result);
	}

	public static implicit operator Matrix3(Transform2D motion)
	{
		return motion.WorldMatrix;
	}

	public void FromXml(XElement xe)
	{
		Translation = XDocHelper.ParseVector2Attribute(xe, "translation", Vector2.Zero);
		Rotation = XDocHelper.ParseFloatAttribute(xe, "rotation", 0f) * ((float)Math.PI / 180f);
		Scale = XDocHelper.ParseVector2Attribute(xe, "scale", Vector2.One);
	}
}
