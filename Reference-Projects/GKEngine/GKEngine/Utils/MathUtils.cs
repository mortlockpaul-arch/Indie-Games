using System;
using GKEngine.Entities;
using Microsoft.Xna.Framework;

namespace GKEngine.Utils;

public class MathUtils
{
	public const float Rad_90 = (float)Math.PI / 2f;

	public const float Rad_180 = (float)Math.PI;

	public const float Rad_270 = 4.712389f;

	public const float Rad_360 = (float)Math.PI * 2f;

	public static double Tables_Resolution;

	public static double[] Tables_Sin = new double[0];

	public static double[] Tables_Cos = new double[0];

	public static void Init()
	{
		Tables_Resolution = 100.0;
		int num = (int)Math.Floor(6.2831854820251465 * Tables_Resolution);
		Tables_Sin = new double[num];
		for (int i = 0; i < Tables_Sin.Length; i++)
		{
			double num2 = (double)i / (double)num;
			Tables_Sin[i] = Math.Sin(num2 * 6.2831854820251465);
		}
	}

	public static float AbsNorm(float xNumber, float xLimit)
	{
		xNumber %= xLimit;
		if (xNumber < 0f)
		{
			xNumber += xLimit;
		}
		return xNumber;
	}

	public static bool VectInRectangle(Vector2 oVect, Rectangle oRect)
	{
		bool result = false;
		if (oVect.X >= (float)oRect.X && oVect.X < (float)(oRect.X + oRect.Width) && oVect.Y >= (float)oRect.Y && oVect.Y < (float)(oRect.Y + oRect.Height))
		{
			result = true;
		}
		return result;
	}

	public static void VectSnap(ref Vector3 oVect)
	{
		oVect.X = (float)Math.Round(Math.Abs(oVect.X)) * (float)Math.Sign(oVect.X);
		oVect.Y = (float)Math.Round(Math.Abs(oVect.Y)) * (float)Math.Sign(oVect.Y);
		oVect.Z = (float)Math.Round(Math.Abs(oVect.Z)) * (float)Math.Sign(oVect.Z);
	}

	public static Vector3 VectSnap(Vector3 oVect)
	{
		VectSnap(ref oVect);
		return oVect;
	}

	public static bool VectInTriangle(Vector2 Point0, Vector2 Point1, Vector2 Point2, Vector2 ThePoint)
	{
		bool flag = false;
		if (Point0 == ThePoint || Point1 == ThePoint || Point2 == ThePoint)
		{
			return true;
		}
		Vector2 vector = Point2 - Point0;
		Vector2 vector2 = Point1 - Point0;
		Vector2 value = ThePoint - Point0;
		float num = Vector2.Dot(vector, vector);
		float num2 = Vector2.Dot(vector, vector2);
		float num3 = Vector2.Dot(vector, value);
		float num4 = Vector2.Dot(vector2, vector2);
		float num5 = Vector2.Dot(vector2, value);
		float num6 = 1f / (num * num4 - num2 * num2);
		float num7 = (num4 * num3 - num2 * num5) * num6;
		float num8 = (num * num5 - num2 * num3) * num6;
		return num7 > 0f && num8 > 0f && num7 + num8 < 1f;
	}

	public static bool VectInTriangle(float Point0X, float Point0Y, float Point1X, float Point1Y, float Point2X, float Point2Y, float ThePointX, float ThePointY)
	{
		bool flag = false;
		if ((Point0X == ThePointX && Point0Y == ThePointY) || (Point1X == ThePointX && Point1Y == ThePointY) || (Point2X == ThePointX && Point2Y == ThePointY))
		{
			return true;
		}
		Vector2 value = default(Vector2);
		Vector2 value2 = default(Vector2);
		Vector2 value3 = default(Vector2);
		value.X = Point2X - Point0X;
		value.Y = Point2Y - Point0Y;
		value2.X = Point1X - Point0X;
		value2.Y = Point1Y - Point0Y;
		value3.X = ThePointX - Point0X;
		value3.Y = ThePointY - Point0Y;
		Vector2.Dot(ref value, ref value, out var result);
		Vector2.Dot(ref value, ref value2, out var result2);
		Vector2.Dot(ref value, ref value3, out var result3);
		Vector2.Dot(ref value2, ref value2, out var result4);
		Vector2.Dot(ref value2, ref value3, out var result5);
		float num = 1f / (result * result4 - result2 * result2);
		float num2 = (result4 * result3 - result2 * result5) * num;
		float num3 = (result * result5 - result2 * result3) * num;
		return num2 > 0f && num3 > 0f && num2 + num3 < 1f;
	}

	public static float FloatSafeParse(string pValue)
	{
		float num = 0f;
		string[] array = new string[0];
		try
		{
			if (pValue.Contains("."))
			{
				array = pValue.Split('.');
			}
			else if (pValue.Contains(","))
			{
				array = pValue.Split(',');
			}
			if (array.Length == 2)
			{
				float num2 = int.Parse(array[0]);
				float num3 = (float)int.Parse(array[1]) / (float)Math.Pow(10.0, array[1].Length);
				return num2 + num3;
			}
			return float.Parse(pValue);
		}
		catch
		{
			return 0f;
		}
	}

	public static Vector2 Vect3DTo2D(Vector3 vec, Matrix viewMatrix, Matrix projMatrix, int Width, int Height)
	{
		Matrix matrix = Matrix.Identity * viewMatrix * projMatrix;
		Vector4 vector = Vector4.Transform(vec, matrix);
		return new Vector2((vector.X / vector.W + 1f) * (float)(Width / 2), (1f - vector.Y / vector.W) * (float)(Height / 2));
	}

	public static Matrix MatrixFromRadians(Vector3 oRadians)
	{
		return Matrix.CreateRotationX(oRadians.X) * Matrix.CreateRotationY(oRadians.Y) * Matrix.CreateRotationZ(oRadians.Z);
	}

	public static Quaternion QuatFromMatrixNorm(Matrix oMatrix)
	{
		return Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(oMatrix));
	}

	public static Quaternion GetLookAt(Base3D oObserver, Vector3 oFocusPoint)
	{
		Quaternion identity = Quaternion.Identity;
		return Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(Matrix.CreateBillboard(oFocusPoint, oObserver.position, oObserver.matrix.Up, oObserver.matrix.Forward)));
	}

	public static int Fold(int xIndex, int xDir, int xCount)
	{
		int num = xIndex + xDir;
		if (num < 0)
		{
			return num + xCount;
		}
		return num % xCount;
	}

	public static Quaternion UnitLookAtUp(Vector3 oUnit, Vector3 oUp)
	{
		if ((double)oUnit.Y > 0.999 || (double)oUnit.Y < -0.999)
		{
			oUnit.X = 1E-05f;
		}
		return Quaternion.Multiply(Quaternion.CreateFromRotationMatrix(Matrix.CreateBillboard(Vector3.Zero, oUnit, oUp, null)), Quaternion.CreateFromAxisAngle(Vector3.Left, (float)Math.PI / 2f));
	}

	public static Quaternion UnitLookAtLeft(Vector3 oUnit)
	{
		if (oUnit == Vector3.Left || oUnit == Vector3.Right)
		{
			oUnit.Z = 1E-10f;
		}
		return QuatFromMatrixNorm(Matrix.CreateBillboard(Vector3.Zero, oUnit, Vector3.Up, null));
	}

	public static Quaternion UnitLookAtXZ(Vector3 oUnit)
	{
		return Quaternion.Normalize(Quaternion.CreateFromRotationMatrix(Matrix.CreateConstrainedBillboard(Vector3.Zero, oUnit, Matrix.Identity.Up, Matrix.Identity.Forward, null)));
	}

	public static Matrix UnitToMatrix(Vector3 oForward, Vector3 oUp)
	{
		return Matrix.CreateBillboard(Vector3.Zero, oForward, oUp, null);
	}

	public static Quaternion UnitToRotation(Vector3 oForward, Vector3 oUp)
	{
		return Quaternion.CreateFromRotationMatrix(Matrix.CreateBillboard(Vector3.Zero, oForward, oUp, null));
	}

	public static void UnitSnapXZ(ref Vector3 vUnit)
	{
		vUnit.Normalize();
		if (Math.Abs(vUnit.X) >= 0.5f)
		{
			vUnit.X = Math.Sign(vUnit.X);
			vUnit.Y = 0f;
			vUnit.Z = 0f;
		}
		else if (Math.Abs(vUnit.Z) >= 0.5f)
		{
			vUnit.X = 0f;
			vUnit.Y = 0f;
			vUnit.Z = Math.Sign(vUnit.Z);
		}
		else
		{
			vUnit.X = 0f;
			vUnit.Y = 0f;
			vUnit.Z = 0f;
		}
	}

	public static float CosLow(float xAngle)
	{
		if (xAngle < -(float)Math.PI)
		{
			xAngle += (float)Math.PI * 2f;
		}
		else if (xAngle > (float)Math.PI)
		{
			xAngle -= (float)Math.PI * 2f;
		}
		xAngle += (float)Math.PI / 2f;
		if (xAngle > (float)Math.PI)
		{
			xAngle -= (float)Math.PI * 2f;
		}
		if (xAngle < 0f)
		{
			return 4f / (float)Math.PI * xAngle + 0.40528473f * xAngle * xAngle;
		}
		return 4f / (float)Math.PI * xAngle - 0.40528473f * xAngle * xAngle;
	}

	public static float SinLow(float xAngle)
	{
		if (xAngle < -(float)Math.PI)
		{
			xAngle += (float)Math.PI * 2f;
		}
		else if (xAngle > (float)Math.PI)
		{
			xAngle -= (float)Math.PI * 2f;
		}
		if (xAngle < 0f)
		{
			return 4f / (float)Math.PI * xAngle + 0.40528473f * xAngle * xAngle;
		}
		return 4f / (float)Math.PI * xAngle - 0.40528473f * xAngle * xAngle;
	}

	public static float CosHigh(float xAngle)
	{
		if (xAngle < -(float)Math.PI)
		{
			xAngle += (float)Math.PI * 2f;
		}
		else if (xAngle > (float)Math.PI)
		{
			xAngle -= (float)Math.PI * 2f;
		}
		xAngle += (float)Math.PI / 2f;
		if (xAngle > (float)Math.PI)
		{
			xAngle -= (float)Math.PI * 2f;
		}
		float num;
		if (xAngle < 0f)
		{
			num = 4f / (float)Math.PI * xAngle + 0.40528473f * xAngle * xAngle;
			if (num < 0f)
			{
				return 0.225f * (num * (0f - num) - num) + num;
			}
			return 0.225f * (num * num - num) + num;
		}
		num = 4f / (float)Math.PI * xAngle - 0.40528473f * xAngle * xAngle;
		if (num < 0f)
		{
			return 0.225f * (num * (0f - num) - num) + num;
		}
		return 0.225f * (num * num - num) + num;
	}

	public static float SinHigh(float xAngle)
	{
		if (xAngle < -(float)Math.PI)
		{
			xAngle += (float)Math.PI * 2f;
		}
		else if (xAngle > (float)Math.PI)
		{
			xAngle -= (float)Math.PI * 2f;
		}
		float num;
		if (xAngle < 0f)
		{
			num = 4f / (float)Math.PI * xAngle + 0.40528473f * xAngle * xAngle;
			if (num < 0f)
			{
				return 0.225f * (num * (0f - num) - num) + num;
			}
			return 0.225f * (num * num - num) + num;
		}
		num = 4f / (float)Math.PI * xAngle - 0.40528473f * xAngle * xAngle;
		if (num < 0f)
		{
			return 0.225f * (num * (0f - num) - num) + num;
		}
		return 0.225f * (num * num - num) + num;
	}

	public static string Commas(float pValue, uint pPlaces)
	{
		string text = pValue.ToString();
		string[] array = text.Split('.');
		string text2 = array[0];
		if (array.Length > 1)
		{
			_ = array[1];
		}
		text = "";
		int num = -1;
		for (int num2 = text2.Length - 1; num2 >= 0; num2--)
		{
			num = (int)((num + 1) % pPlaces);
			text = text2.Substring(num2, 1) + text;
			if (num2 > 0 && num >= pPlaces - 1)
			{
				text = "," + text;
			}
		}
		return text;
	}

	public static string FormatTimeHHMMSS(float pMS)
	{
		int num = (int)Math.Floor(pMS / 3600000f);
		pMS -= (float)(num * 3600000);
		int num2 = (int)Math.Floor(pMS / 60000f);
		pMS -= (float)(num2 * 60000);
		int num3 = (int)Math.Floor(pMS / 1000f);
		return num.ToString().PadLeft(2, '0') + ":" + num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0');
	}
}
