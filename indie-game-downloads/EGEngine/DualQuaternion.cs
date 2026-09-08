using Microsoft.Xna.Framework;

namespace EGEngine;

internal class DualQuaternion
{
	public Quaternion Ordinary;

	public Quaternion Dual;

	public static DualQuaternion QuatTrans2UDQ(Quaternion q0, Vector3 t)
	{
		DualQuaternion dualQuaternion = new DualQuaternion();
		dualQuaternion.Ordinary = q0;
		dualQuaternion.Dual.W = -0.5f * (t.X * q0.X + t.Y * q0.Y + t.Z * q0.Z);
		dualQuaternion.Dual.X = 0.5f * (t.X * q0.W + t.Y * q0.Z - t.Z * q0.Y);
		dualQuaternion.Dual.Y = 0.5f * ((0f - t.X) * q0.Z + t.Y * q0.W + t.Z * q0.X);
		dualQuaternion.Dual.Z = 0.5f * (t.X * q0.Y - t.Y * q0.X + t.Z * q0.W);
		return dualQuaternion;
	}

	public static Matrix UDQToMatrix(DualQuaternion dq)
	{
		Matrix identity = Matrix.Identity;
		float num = Quaternion.Dot(dq.Ordinary, dq.Ordinary);
		float w = dq.Ordinary.W;
		float x = dq.Ordinary.X;
		float y = dq.Ordinary.Y;
		float z = dq.Ordinary.Z;
		float w2 = dq.Dual.W;
		float x2 = dq.Dual.X;
		float y2 = dq.Dual.Y;
		float z2 = dq.Dual.Z;
		identity.M11 = w * w + x * x - y * y - z * z;
		identity.M21 = 2f * x * y - 2f * w * z;
		identity.M31 = 2f * x * z + 2f * w * y;
		identity.M12 = 2f * x * y + 2f * w * z;
		identity.M22 = w * w + y * y - x * x - z * z;
		identity.M32 = 2f * y * z - 2f * w * x;
		identity.M13 = 2f * x * z - 2f * w * y;
		identity.M23 = 2f * y * z + 2f * w * x;
		identity.M33 = w * w + z * z - x * x - y * y;
		identity.M41 = -2f * w2 * x + 2f * w * x2 - 2f * y2 * z + 2f * y * z2;
		identity.M42 = -2f * w2 * y + 2f * x2 * z - 2f * x * z2 + 2f * w * y2;
		identity.M43 = -2f * w2 * z + 2f * x * y2 + 2f * w * z2 - 2f * x2 * y;
		identity.M14 = 0f;
		identity.M24 = 0f;
		identity.M34 = 0f;
		identity.M44 = num;
		return identity / num;
	}
}
