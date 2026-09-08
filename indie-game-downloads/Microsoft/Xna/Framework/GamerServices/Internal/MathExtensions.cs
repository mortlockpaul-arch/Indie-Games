using Microsoft.XboxLive.MathUtilities;

namespace Microsoft.Xna.Framework.GamerServices.Internal;

internal static class MathExtensions
{
	internal static Vector2 ToXnaVector2(this Microsoft.XboxLive.MathUtilities.Vector2 v)
	{
		return new Vector2(v.X, v.Y);
	}

	internal static Vector3 ToXnaVector3(this Microsoft.XboxLive.MathUtilities.Vector3 v)
	{
		return new Vector3(v.X, v.Y, v.Z);
	}

	internal static Quaternion ToXnaQuaternion(this Microsoft.XboxLive.MathUtilities.Quaternion q)
	{
		return new Quaternion(q.X, q.Y, q.Z, q.W);
	}

	internal static Matrix ToXnaMatrix(this Microsoft.XboxLive.MathUtilities.Matrix m)
	{
		return new Matrix(m.M11, m.M12, m.M13, m.M14, m.M21, m.M22, m.M23, m.M24, m.M31, m.M32, m.M33, m.M34, m.M41, m.M42, m.M43, m.M44);
	}
}
