using System;
using BEPUphysics.MathExtensions;
using Microsoft.Xna.Framework;

namespace BEPUphysics.Constraints;

/// <summary>
/// Defines a three dimensional orthonormal basis used by a constraint.
/// </summary>
public class JointBasis3D
{
	internal Vector3 localPrimaryAxis = Vector3.Backward;

	internal Vector3 localXAxis = Vector3.Right;

	internal Vector3 localYAxis = Vector3.Up;

	internal Vector3 primaryAxis = Vector3.Backward;

	internal Matrix3X3 rotationMatrix = Matrix3X3.Identity;

	internal Vector3 xAxis = Vector3.Right;

	internal Vector3 yAxis = Vector3.Up;

	/// <summary>
	/// Gets the primary axis of the transform in local space.
	/// </summary>
	public Vector3 LocalPrimaryAxis => localPrimaryAxis;

	/// <summary>
	/// Gets or sets the local transform of the basis.
	/// </summary>
	public Matrix3X3 LocalTransform
	{
		get
		{
			return new Matrix3X3
			{
				Right = localXAxis,
				Up = localYAxis,
				Backward = localPrimaryAxis
			};
		}
		set
		{
			SetLocalAxes(value);
		}
	}

	/// <summary>
	/// Gets the X axis of the transform in local space.
	/// </summary>
	public Vector3 LocalXAxis => localXAxis;

	/// <summary>
	/// Gets the Y axis of the transform in local space.
	/// </summary>
	public Vector3 LocalYAxis => localYAxis;

	/// <summary>
	/// Gets the primary axis of the transform.
	/// </summary>
	public Vector3 PrimaryAxis => primaryAxis;

	/// <summary>
	/// Gets or sets the rotation matrix used by the joint transform to convert local space axes to world space.
	/// </summary>
	public Matrix3X3 RotationMatrix
	{
		get
		{
			return rotationMatrix;
		}
		set
		{
			rotationMatrix = value;
			ComputeWorldSpaceAxes();
		}
	}

	/// <summary>
	/// Gets or sets the world transform of the basis.
	/// </summary>
	public Matrix3X3 WorldTransform
	{
		get
		{
			return new Matrix3X3
			{
				Right = xAxis,
				Up = yAxis,
				Backward = primaryAxis
			};
		}
		set
		{
			SetWorldAxes(value);
		}
	}

	/// <summary>
	/// Gets the X axis of the transform.
	/// </summary>
	public Vector3 XAxis => xAxis;

	/// <summary>
	/// Gets the Y axis of the transform.
	/// </summary>
	public Vector3 YAxis => yAxis;

	/// <summary>
	/// Sets up the axes of the transform and ensures that it is an orthonormal basis.
	/// </summary>
	/// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
	/// <param name="xAxis">Second axis in the transform.</param>
	/// <param name="yAxis">Third axis in the transform.</param>
	/// <param name="rotationMatrix">Matrix to use to transform the local axes into world space.</param>
	public void SetLocalAxes(Vector3 primaryAxis, Vector3 xAxis, Vector3 yAxis, Matrix3X3 rotationMatrix)
	{
		this.rotationMatrix = rotationMatrix;
		SetLocalAxes(primaryAxis, xAxis, yAxis);
	}

	/// <summary>
	/// Sets up the axes of the transform and ensures that it is an orthonormal basis.
	/// </summary>
	/// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
	/// <param name="xAxis">Second axis in the transform.</param>
	/// <param name="yAxis">Third axis in the transform.</param>
	public void SetLocalAxes(Vector3 primaryAxis, Vector3 xAxis, Vector3 yAxis)
	{
		if (Math.Abs(Vector3.Dot(primaryAxis, xAxis)) > 1E-05f || Math.Abs(Vector3.Dot(primaryAxis, yAxis)) > 1E-05f || Math.Abs(Vector3.Dot(xAxis, yAxis)) > 1E-05f)
		{
			throw new ArgumentException("The axes provided to the joint transform do not form an orthonormal basis.  Ensure that each axis is perpendicular to the other two.");
		}
		localPrimaryAxis = Vector3.Normalize(primaryAxis);
		localXAxis = Vector3.Normalize(xAxis);
		localYAxis = Vector3.Normalize(yAxis);
		ComputeWorldSpaceAxes();
	}

	/// <summary>
	/// Sets up the axes of the transform and ensures that it is an orthonormal basis.
	/// </summary>
	/// <param name="matrix">Rotation matrix representing the three axes.
	/// The matrix's backward vector is used as the primary axis.  
	/// The matrix's right vector is used as the x axis.
	/// The matrix's up vector is used as the y axis.</param>
	public void SetLocalAxes(Matrix3X3 matrix)
	{
		if (Math.Abs(Vector3.Dot(matrix.Backward, matrix.Right)) > 1E-05f || Math.Abs(Vector3.Dot(matrix.Backward, matrix.Up)) > 1E-05f || Math.Abs(Vector3.Dot(matrix.Right, matrix.Up)) > 1E-05f)
		{
			throw new ArgumentException("The axes provided to the joint transform do not form an orthonormal basis.  Ensure that each axis is perpendicular to the other two.");
		}
		localPrimaryAxis = Vector3.Normalize(matrix.Backward);
		localXAxis = Vector3.Normalize(matrix.Right);
		localYAxis = Vector3.Normalize(matrix.Up);
		ComputeWorldSpaceAxes();
	}

	/// <summary>
	/// Sets up the axes of the transform and ensures that it is an orthonormal basis.
	/// </summary>
	/// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
	/// <param name="xAxis">Second axis in the transform.</param>
	/// <param name="yAxis">Third axis in the transform.</param>
	/// <param name="rotationMatrix">Matrix to use to transform the local axes into world space.</param>
	public void SetWorldAxes(Vector3 primaryAxis, Vector3 xAxis, Vector3 yAxis, Matrix3X3 rotationMatrix)
	{
		this.rotationMatrix = rotationMatrix;
		SetWorldAxes(primaryAxis, xAxis, yAxis);
	}

	/// <summary>
	/// Sets up the axes of the transform and ensures that it is an orthonormal basis.
	/// </summary>
	/// <param name="primaryAxis">First axis in the transform.  Usually aligned along the main axis of a joint, like the twist axis of a TwistLimit.</param>
	/// <param name="xAxis">Second axis in the transform.</param>
	/// <param name="yAxis">Third axis in the transform.</param>
	public void SetWorldAxes(Vector3 primaryAxis, Vector3 xAxis, Vector3 yAxis)
	{
		if (Math.Abs(Vector3.Dot(primaryAxis, xAxis)) > 1E-05f || Math.Abs(Vector3.Dot(primaryAxis, yAxis)) > 1E-05f || Math.Abs(Vector3.Dot(xAxis, yAxis)) > 1E-05f)
		{
			throw new ArgumentException("The axes provided to the joint transform do not form an orthonormal basis.  Ensure that each axis is perpendicular to the other two.");
		}
		this.primaryAxis = Vector3.Normalize(primaryAxis);
		this.xAxis = Vector3.Normalize(xAxis);
		this.yAxis = Vector3.Normalize(yAxis);
		Matrix3X3.TransformTranspose(ref this.primaryAxis, ref rotationMatrix, out localPrimaryAxis);
		Matrix3X3.TransformTranspose(ref this.xAxis, ref rotationMatrix, out localXAxis);
		Matrix3X3.TransformTranspose(ref this.yAxis, ref rotationMatrix, out localYAxis);
	}

	/// <summary>
	/// Sets up the axes of the transform and ensures that it is an orthonormal basis.
	/// </summary>
	/// <param name="matrix">Rotation matrix representing the three axes.
	/// The matrix's backward vector is used as the primary axis.  
	/// The matrix's right vector is used as the x axis.
	/// The matrix's up vector is used as the y axis.</param>
	public void SetWorldAxes(Matrix3X3 matrix)
	{
		if (Math.Abs(Vector3.Dot(matrix.Backward, matrix.Right)) > 1E-05f || Math.Abs(Vector3.Dot(matrix.Backward, matrix.Up)) > 1E-05f || Math.Abs(Vector3.Dot(matrix.Right, matrix.Up)) > 1E-05f)
		{
			throw new ArgumentException("The axes provided to the joint transform do not form an orthonormal basis.  Ensure that each axis is perpendicular to the other two.");
		}
		primaryAxis = Vector3.Normalize(matrix.Backward);
		xAxis = Vector3.Normalize(matrix.Right);
		yAxis = Vector3.Normalize(matrix.Up);
		Matrix3X3.TransformTranspose(ref primaryAxis, ref rotationMatrix, out localPrimaryAxis);
		Matrix3X3.TransformTranspose(ref xAxis, ref rotationMatrix, out localXAxis);
		Matrix3X3.TransformTranspose(ref yAxis, ref rotationMatrix, out localYAxis);
	}

	internal void ComputeWorldSpaceAxes()
	{
		Matrix3X3.Transform(ref localPrimaryAxis, ref rotationMatrix, out primaryAxis);
		Matrix3X3.Transform(ref localXAxis, ref rotationMatrix, out xAxis);
		Matrix3X3.Transform(ref localYAxis, ref rotationMatrix, out yAxis);
	}
}
