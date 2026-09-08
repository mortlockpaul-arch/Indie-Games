using System;
using System.Collections.Generic;
using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.Xna.Framework.GamerServices.Internal;

internal class VertexSkinning
{
	public Skeleton Skeleton;

	public Matrix[] LocalSpace;

	public Matrix[] InverseBindPoses;

	public Matrix[] VertexMatrices;

	public Matrix[] NormalMatrices;

	public VertexSkinning(Skeleton skeleton)
	{
		Skeleton = skeleton;
		int num = skeleton.Joints.Length;
		LocalSpace = new Matrix[num];
		InverseBindPoses = new Matrix[num];
		VertexMatrices = new Matrix[num];
		NormalMatrices = new Matrix[num];
		for (int i = 0; i < num; i++)
		{
			Matrix matrix = Matrix.CreateFromQuaternion(skeleton.Joints[i].Local.rotation.ToXnaQuaternion());
			matrix = Matrix.Multiply(matrix, Matrix.CreateScale(skeleton.Joints[i].Local.scale.ToXnaVector3()));
			matrix.M41 = skeleton.Joints[i].Local.position.X;
			matrix.M42 = skeleton.Joints[i].Local.position.Y;
			matrix.M43 = skeleton.Joints[i].Local.position.Z;
			LocalSpace[i] = matrix;
			Matrix matrix2 = Matrix.CreateFromQuaternion(skeleton.Joints[i].BindRotation.ToXnaQuaternion());
			matrix2.M41 = skeleton.Joints[i].BindPosition.X;
			matrix2.M42 = skeleton.Joints[i].BindPosition.Y;
			matrix2.M43 = skeleton.Joints[i].BindPosition.Z;
			matrix2 = Matrix.Invert(matrix2);
			InverseBindPoses[i] = matrix2;
			VertexMatrices[i] = matrix;
		}
	}

	public bool Update(IList<Matrix> bones, Matrix model)
	{
		if (bones == null)
		{
			throw new ArgumentNullException("bones", "bone matrices are not available");
		}
		bool result = false;
		int count = bones.Count;
		if (count < LocalSpace.Length || count > LocalSpace.Length)
		{
			throw new ArgumentException("incorrect number of bone matrices");
		}
		for (int i = 0; i < count; i++)
		{
			Matrix matrix = bones[i];
			matrix = Matrix.Multiply(matrix, LocalSpace[i]);
			Matrix matrix2 = ((i == 0) ? model : VertexMatrices[Skeleton.Joints[i].Parent]);
			ref Matrix reference = ref VertexMatrices[i];
			reference = Matrix.Multiply(matrix, matrix2);
		}
		for (int j = 0; j < count; j++)
		{
			Matrix matrix3 = Matrix.Multiply(InverseBindPoses[j], VertexMatrices[j]);
			VertexMatrices[j] = matrix3;
			Matrix matrix4 = new Matrix
			{
				M11 = matrix3.M22 * matrix3.M33 - matrix3.M23 * matrix3.M32,
				M12 = matrix3.M23 * matrix3.M31 - matrix3.M21 * matrix3.M33,
				M13 = matrix3.M21 * matrix3.M32 - matrix3.M22 * matrix3.M31,
				M21 = matrix3.M13 * matrix3.M32 - matrix3.M12 * matrix3.M33,
				M22 = matrix3.M11 * matrix3.M33 - matrix3.M13 * matrix3.M31,
				M23 = matrix3.M12 * matrix3.M31 - matrix3.M11 * matrix3.M32,
				M31 = matrix3.M12 * matrix3.M23 - matrix3.M13 * matrix3.M22,
				M32 = matrix3.M13 * matrix3.M21 - matrix3.M11 * matrix3.M23,
				M33 = matrix3.M11 * matrix3.M22 - matrix3.M12 * matrix3.M21
			};
			float num = (float)Math.Sqrt(matrix4.M11 * matrix4.M11 + matrix4.M21 * matrix4.M21 + matrix4.M31 * matrix4.M31);
			if (num > 1E-08f)
			{
				num = 1f / num;
				matrix4.M11 *= num;
				matrix4.M21 *= num;
				matrix4.M31 *= num;
				matrix4.M12 *= num;
				matrix4.M22 *= num;
				matrix4.M32 *= num;
				matrix4.M13 *= num;
				matrix4.M23 *= num;
				matrix4.M33 *= num;
				result = true;
			}
			NormalMatrices[j] = matrix4;
		}
		return result;
	}

	internal void ApplySkinning(SkinnedVertex[] inputVertices, PositionColorVertex[] outputVertices)
	{
		Vector3 vector = default(Vector3);
		Vector3 vector2 = default(Vector3);
		Vector3 vector3 = default(Vector3);
		Vector3 vector4 = default(Vector3);
		Vector3 vector5 = default(Vector3);
		Vector3 vector6 = default(Vector3);
		Vector3 vector7 = default(Vector3);
		Vector3 vector8 = default(Vector3);
		Vector3 position = default(Vector3);
		Vector3 skinnedNormal = default(Vector3);
		Matrix[] vertexMatrices = VertexMatrices;
		Matrix[] normalMatrices = NormalMatrices;
		for (int i = 0; i < inputVertices.Length; i++)
		{
			SkinnedVertex skinnedVertex = inputVertices[i];
			Vector3 position2 = skinnedVertex.position;
			Vector3 normal = skinnedVertex.normal;
			if (skinnedVertex.w0 > 0f)
			{
				Matrix matrix = vertexMatrices[skinnedVertex.i0];
				vector.X = matrix.M11 * position2.X + matrix.M21 * position2.Y + matrix.M31 * position2.Z + matrix.M41;
				vector.Y = matrix.M12 * position2.X + matrix.M22 * position2.Y + matrix.M32 * position2.Z + matrix.M42;
				vector.Z = matrix.M13 * position2.X + matrix.M23 * position2.Y + matrix.M33 * position2.Z + matrix.M43;
				matrix = normalMatrices[skinnedVertex.i0];
				vector5.X = matrix.M11 * normal.X + matrix.M21 * normal.Y + matrix.M31 * normal.Z;
				vector5.Y = matrix.M12 * normal.X + matrix.M22 * normal.Y + matrix.M32 * normal.Z;
				vector5.Z = matrix.M13 * normal.X + matrix.M23 * normal.Y + matrix.M33 * normal.Z;
			}
			if (skinnedVertex.w1 > 0f)
			{
				Matrix matrix2 = vertexMatrices[skinnedVertex.i1];
				vector2.X = matrix2.M11 * position2.X + matrix2.M21 * position2.Y + matrix2.M31 * position2.Z + matrix2.M41;
				vector2.Y = matrix2.M12 * position2.X + matrix2.M22 * position2.Y + matrix2.M32 * position2.Z + matrix2.M42;
				vector2.Z = matrix2.M13 * position2.X + matrix2.M23 * position2.Y + matrix2.M33 * position2.Z + matrix2.M43;
				matrix2 = normalMatrices[skinnedVertex.i1];
				vector6.X = matrix2.M11 * normal.X + matrix2.M21 * normal.Y + matrix2.M31 * normal.Z;
				vector6.Y = matrix2.M12 * normal.X + matrix2.M22 * normal.Y + matrix2.M32 * normal.Z;
				vector6.Z = matrix2.M13 * normal.X + matrix2.M23 * normal.Y + matrix2.M33 * normal.Z;
			}
			if (skinnedVertex.w2 > 0f)
			{
				Matrix matrix3 = vertexMatrices[skinnedVertex.i2];
				vector3.X = matrix3.M11 * position2.X + matrix3.M21 * position2.Y + matrix3.M31 * position2.Z + matrix3.M41;
				vector3.Y = matrix3.M12 * position2.X + matrix3.M22 * position2.Y + matrix3.M32 * position2.Z + matrix3.M42;
				vector3.Z = matrix3.M13 * position2.X + matrix3.M23 * position2.Y + matrix3.M33 * position2.Z + matrix3.M43;
				matrix3 = normalMatrices[skinnedVertex.i2];
				vector7.X = matrix3.M11 * normal.X + matrix3.M21 * normal.Y + matrix3.M31 * normal.Z;
				vector7.Y = matrix3.M12 * normal.X + matrix3.M22 * normal.Y + matrix3.M32 * normal.Z;
				vector7.Z = matrix3.M13 * normal.X + matrix3.M23 * normal.Y + matrix3.M33 * normal.Z;
			}
			if (skinnedVertex.w3 > 0f)
			{
				Matrix matrix4 = vertexMatrices[skinnedVertex.i3];
				vector4.X = matrix4.M11 * position2.X + matrix4.M21 * position2.Y + matrix4.M31 * position2.Z + matrix4.M41;
				vector4.Y = matrix4.M12 * position2.X + matrix4.M22 * position2.Y + matrix4.M32 * position2.Z + matrix4.M42;
				vector4.Z = matrix4.M13 * position2.X + matrix4.M23 * position2.Y + matrix4.M33 * position2.Z + matrix4.M43;
				matrix4 = normalMatrices[skinnedVertex.i3];
				vector8.X = matrix4.M11 * normal.X + matrix4.M21 * normal.Y + matrix4.M31 * normal.Z;
				vector8.Y = matrix4.M12 * normal.X + matrix4.M22 * normal.Y + matrix4.M32 * normal.Z;
				vector8.Z = matrix4.M13 * normal.X + matrix4.M23 * normal.Y + matrix4.M33 * normal.Z;
			}
			position.X = skinnedVertex.w0 * vector.X + skinnedVertex.w1 * vector2.X + skinnedVertex.w2 * vector3.X + skinnedVertex.w3 * vector4.X;
			position.Y = skinnedVertex.w0 * vector.Y + skinnedVertex.w1 * vector2.Y + skinnedVertex.w2 * vector3.Y + skinnedVertex.w3 * vector4.Y;
			position.Z = skinnedVertex.w0 * vector.Z + skinnedVertex.w1 * vector2.Z + skinnedVertex.w2 * vector3.Z + skinnedVertex.w3 * vector4.Z;
			outputVertices[i].position = position;
			skinnedNormal.X = skinnedVertex.w0 * vector5.X + skinnedVertex.w1 * vector6.X + skinnedVertex.w2 * vector7.X + skinnedVertex.w3 * vector8.X;
			skinnedNormal.Y = skinnedVertex.w0 * vector5.Y + skinnedVertex.w1 * vector6.Y + skinnedVertex.w2 * vector7.Y + skinnedVertex.w3 * vector8.Y;
			skinnedNormal.Z = skinnedVertex.w0 * vector5.Z + skinnedVertex.w1 * vector6.Z + skinnedVertex.w2 * vector7.Z + skinnedVertex.w3 * vector8.Z;
			inputVertices[i].skinnedNormal = skinnedNormal;
		}
	}
}
