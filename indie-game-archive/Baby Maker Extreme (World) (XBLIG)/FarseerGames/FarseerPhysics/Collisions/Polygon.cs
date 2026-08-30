using System;
using FarseerGames.FarseerPhysics.Mathematics;
using Microsoft.Xna.Framework;

namespace FarseerGames.FarseerPhysics.Collisions;

internal class Polygon
{
	private const int maxVerticesPerPolygon = 32;

	private const float angularSlop = (float)Math.PI / 180f;

	private float[] x;

	private float[] y;

	private int nVertices;

	private float area;

	private Polygon(float[] _x, float[] _y, int nVert)
	{
		nVertices = nVert;
		x = new float[nVertices];
		y = new float[nVertices];
		for (int i = 0; i < nVertices; i++)
		{
			x[i] = _x[i];
			y[i] = _y[i];
		}
	}

	private Polygon(Vector2[] v, int nVert)
	{
		nVertices = nVert;
		x = new float[nVertices];
		y = new float[nVertices];
		for (int i = 0; i < nVertices; i++)
		{
			x[i] = v[i].X;
			y[i] = v[i].Y;
		}
	}

	private Polygon()
	{
		x = null;
		y = null;
		nVertices = 0;
	}

	private float GetArea()
	{
		area = 0f;
		area += x[nVertices - 1] * y[0] - x[0] * y[nVertices - 1];
		for (int i = 0; i < nVertices - 1; i++)
		{
			area += x[i] * y[i + 1] - x[i + 1] * y[i];
		}
		area *= 0.5f;
		return area;
	}

	private bool IsCCW()
	{
		return GetArea() > 0f;
	}

	private void MergeParallelEdges(float tolerance)
	{
		if (nVertices <= 3)
		{
			return;
		}
		bool[] array = new bool[nVertices];
		int num = nVertices;
		for (int i = 0; i < nVertices; i++)
		{
			int num2 = ((i == 0) ? (nVertices - 1) : (i - 1));
			int num3 = i;
			int num4 = ((i != nVertices - 1) ? (i + 1) : 0);
			float num5 = x[num3] - x[num2];
			float num6 = y[num3] - y[num2];
			float num7 = x[num4] - x[num3];
			float num8 = y[num4] - y[num3];
			float num9 = (float)Math.Sqrt(num5 * num5 + num6 * num6);
			float num10 = (float)Math.Sqrt(num7 * num7 + num8 * num8);
			if ((!(num9 > 0f) || !(num10 > 0f)) && num > 3)
			{
				array[i] = true;
				num--;
			}
			num5 /= num9;
			num6 /= num9;
			num7 /= num10;
			num8 /= num10;
			float value = num5 * num8 - num7 * num6;
			float num11 = num5 * num7 + num6 * num8;
			if (Math.Abs(value) < tolerance && num11 > 0f && num > 3)
			{
				array[i] = true;
				num--;
			}
			else
			{
				array[i] = false;
			}
		}
		if (num == nVertices || num == 0)
		{
			return;
		}
		float[] array2 = new float[num];
		float[] array3 = new float[num];
		int num12 = 0;
		for (int j = 0; j < nVertices; j++)
		{
			if (!array[j] && num != 0 && num12 != num)
			{
				array2[num12] = x[j];
				array3[num12] = y[j];
				num12++;
			}
		}
		x = array2;
		y = array3;
		nVertices = num;
	}

	private Polygon(Triangle t)
	{
		nVertices = 3;
		x = new float[nVertices];
		y = new float[nVertices];
		for (int i = 0; i < nVertices; i++)
		{
			x[i] = t.x[i];
			y[i] = t.y[i];
		}
	}

	private Polygon(Polygon p)
	{
		nVertices = p.nVertices;
		x = new float[nVertices];
		y = new float[nVertices];
		for (int i = 0; i < nVertices; i++)
		{
			x[i] = p.x[i];
			y[i] = p.y[i];
		}
	}

	private void Set(Polygon p)
	{
		if (nVertices != p.nVertices)
		{
			nVertices = p.nVertices;
			x = new float[nVertices];
			y = new float[nVertices];
		}
		for (int i = 0; i < nVertices; i++)
		{
			x[i] = p.x[i];
			y[i] = p.y[i];
		}
	}

	private bool IsConvex()
	{
		bool flag = false;
		for (int i = 0; i < nVertices; i++)
		{
			int num = ((i == 0) ? (nVertices - 1) : (i - 1));
			int num2 = i;
			int num3 = ((i != nVertices - 1) ? (i + 1) : 0);
			float num4 = x[num2] - x[num];
			float num5 = y[num2] - y[num];
			float num6 = x[num3] - x[num2];
			float num7 = y[num3] - y[num2];
			float num8 = num4 * num7 - num6 * num5;
			bool flag2 = num8 >= 0f;
			if (i == 0)
			{
				flag = flag2;
			}
			else if (flag != flag2)
			{
				return false;
			}
		}
		return true;
	}

	private Polygon Add(Triangle t)
	{
		int num = -1;
		int num2 = -1;
		int num3 = -1;
		int num4 = -1;
		for (int i = 0; i < nVertices; i++)
		{
			if (t.x[0] == x[i] && t.y[0] == y[i])
			{
				if (num == -1)
				{
					num = i;
					num2 = 0;
				}
				else
				{
					num3 = i;
					num4 = 0;
				}
			}
			else if (t.x[1] == x[i] && t.y[1] == y[i])
			{
				if (num == -1)
				{
					num = i;
					num2 = 1;
				}
				else
				{
					num3 = i;
					num4 = 1;
				}
			}
			else if (t.x[2] == x[i] && t.y[2] == y[i])
			{
				if (num == -1)
				{
					num = i;
					num2 = 2;
				}
				else
				{
					num3 = i;
					num4 = 2;
				}
			}
		}
		if (num == 0 && num3 == nVertices - 1)
		{
			num = nVertices - 1;
			num3 = 0;
		}
		if (num3 == -1)
		{
			return null;
		}
		int num5 = 0;
		if (num5 == num2 || num5 == num4)
		{
			num5 = 1;
		}
		if (num5 == num2 || num5 == num4)
		{
			num5 = 2;
		}
		float[] array = new float[nVertices + 1];
		float[] array2 = new float[nVertices + 1];
		int num6 = 0;
		for (int j = 0; j < nVertices; j++)
		{
			array[num6] = x[j];
			array2[num6] = y[j];
			if (j == num)
			{
				num6++;
				array[num6] = t.x[num5];
				array2[num6] = t.y[num5];
			}
			num6++;
		}
		return new Polygon(array, array2, nVertices + 1);
	}

	private static bool ResolvePinchPoint(Polygon pin, out Polygon poutA, out Polygon poutB)
	{
		poutA = new Polygon();
		poutB = new Polygon();
		if (pin.nVertices < 3)
		{
			return false;
		}
		bool flag = false;
		int num = -1;
		int num2 = -1;
		for (int i = 0; i < pin.nVertices; i++)
		{
			for (int j = i + 1; j < pin.nVertices; j++)
			{
				if (Math.Abs(pin.x[i] - pin.x[j]) < 0.001f && Math.Abs(pin.y[i] - pin.y[j]) < 0.001f && j != i + 1)
				{
					num = i;
					num2 = j;
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		if (flag)
		{
			int num3 = num2 - num;
			if (num3 == pin.nVertices)
			{
				return false;
			}
			float[] array = new float[num3];
			float[] array2 = new float[num3];
			for (int k = 0; k < num3; k++)
			{
				int num4 = Remainder(num + k, pin.nVertices);
				array[k] = pin.x[num4];
				array2[k] = pin.y[num4];
			}
			Polygon p = new Polygon(array, array2, num3);
			poutA.Set(p);
			int num5 = pin.nVertices - num3;
			float[] array3 = new float[num5];
			float[] array4 = new float[num5];
			for (int l = 0; l < num5; l++)
			{
				int num6 = Remainder(num2 + l, pin.nVertices);
				array3[l] = pin.x[num6];
				array4[l] = pin.y[num6];
			}
			Polygon p2 = new Polygon(array3, array4, num5);
			poutB.Set(p2);
		}
		return flag;
	}

	private static int TriangulatePolygon(float[] xv, float[] yv, int vNum, out Triangle[] results)
	{
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		results = new Triangle[175];
		if (vNum < 3)
		{
			return 0;
		}
		Polygon pin = new Polygon(xv, yv, vNum);
		if (ResolvePinchPoint(pin, out var poutA, out var poutB))
		{
			Triangle[] results2 = new Triangle[poutA.nVertices];
			Triangle[] results3 = new Triangle[poutB.nVertices];
			int num = TriangulatePolygon(poutA.x, poutA.y, poutA.nVertices, out results2);
			int num2 = TriangulatePolygon(poutB.x, poutB.y, poutB.nVertices, out results3);
			if (num == -1 || num2 == -1)
			{
				return -1;
			}
			for (int i = 0; i < num; i++)
			{
				results[i] = new Triangle(results2[i]);
			}
			for (int j = 0; j < num2; j++)
			{
				results[num + j] = new Triangle(results3[j]);
			}
			return num + num2;
		}
		Triangle[] array = new Triangle[vNum - 2];
		int num3 = 0;
		float[] array2 = new float[vNum];
		float[] array3 = new float[vNum];
		for (int k = 0; k < vNum; k++)
		{
			array2[k] = xv[k];
			array3[k] = yv[k];
		}
		while (vNum > 3)
		{
			int num4 = -1;
			float num5 = -10f;
			for (int l = 0; l < vNum; l++)
			{
				if (IsEar(l, array2, array3, vNum))
				{
					int num6 = Remainder(l - 1, vNum);
					int num7 = Remainder(l + 1, vNum);
					Vector2 value = new Vector2(array2[num7] - array2[l], array3[num7] - array3[l]);
					Vector2 value2 = new Vector2(array2[l] - array2[num6], array3[l] - array3[num6]);
					Vector2 value3 = new Vector2(array2[num6] - array2[num7], array3[num6] - array3[num7]);
					((Vector2)(ref value)).Normalize();
					((Vector2)(ref value2)).Normalize();
					((Vector2)(ref value3)).Normalize();
					float val = Math.Abs(Calculator.Cross(ref value, ref value2));
					float val2 = Math.Abs(Calculator.Cross(ref value2, ref value3));
					float val3 = Math.Abs(Calculator.Cross(ref value3, ref value));
					float num8 = Math.Min(val, Math.Min(val2, val3));
					if (num8 > num5)
					{
						num4 = l;
						num5 = num8;
					}
				}
			}
			if (num4 == -1)
			{
				for (int m = 0; m < num3; m++)
				{
					results[m] = new Triangle(array[m]);
				}
				if (num3 > 0)
				{
					return num3;
				}
				return -1;
			}
			vNum--;
			float[] array4 = new float[vNum];
			float[] array5 = new float[vNum];
			int num9 = 0;
			for (int n = 0; n < vNum; n++)
			{
				if (num9 == num4)
				{
					num9++;
				}
				array4[n] = array2[num9];
				array5[n] = array3[num9];
				num9++;
			}
			int num10 = ((num4 == 0) ? vNum : (num4 - 1));
			int num11 = ((num4 != vNum) ? (num4 + 1) : 0);
			Triangle t = new Triangle(array2[num4], array3[num4], array2[num11], array3[num11], array2[num10], array3[num10]);
			array[num3] = new Triangle(t);
			num3++;
			array2 = array4;
			array3 = array5;
		}
		Triangle t2 = new Triangle(array2[1], array3[1], array2[2], array3[2], array2[0], array3[0]);
		array[num3] = new Triangle(t2);
		num3++;
		for (int num12 = 0; num12 < num3; num12++)
		{
			results[num12] = new Triangle(array[num12]);
		}
		return num3;
	}

	private static int Remainder(int x, int modulus)
	{
		int i;
		for (i = x % modulus; i < 0; i += modulus)
		{
		}
		return i;
	}

	private static int PolygonizeTriangles(Triangle[] triangulated, int triangulatedLength, out Polygon[] polys, int polysLength)
	{
		int num = 0;
		polys = new Polygon[50];
		if (triangulatedLength <= 0)
		{
			return 0;
		}
		bool[] array = new bool[triangulatedLength];
		for (int i = 0; i < triangulatedLength; i++)
		{
			array[i] = false;
			if ((triangulated[i].x[0] == triangulated[i].x[1] && triangulated[i].y[0] == triangulated[i].y[1]) || (triangulated[i].x[1] == triangulated[i].x[2] && triangulated[i].y[1] == triangulated[i].y[2]) || (triangulated[i].x[0] == triangulated[i].x[2] && triangulated[i].y[0] == triangulated[i].y[2]))
			{
				array[i] = true;
			}
		}
		bool flag = true;
		while (flag)
		{
			int num2 = -1;
			for (int j = 0; j < triangulatedLength; j++)
			{
				if (!array[j])
				{
					num2 = j;
					break;
				}
			}
			if (num2 == -1)
			{
				flag = false;
				continue;
			}
			Polygon polygon = new Polygon(triangulated[num2]);
			array[num2] = true;
			int num3 = 0;
			int num4 = 0;
			while (num4 < 2 * triangulatedLength)
			{
				while (num3 >= triangulatedLength)
				{
					num3 -= triangulatedLength;
				}
				if (!array[num3])
				{
					Polygon polygon2 = polygon.Add(triangulated[num3]);
					if (polygon2 != null)
					{
						if (polygon2.nVertices > 32)
						{
							polygon2 = null;
						}
						else if (polygon2.IsConvex())
						{
							polygon = new Polygon(polygon2);
							polygon2 = null;
							array[num3] = true;
						}
						else
						{
							polygon2 = null;
						}
					}
				}
				num4++;
				num3++;
			}
			if (num < polysLength)
			{
				polygon.MergeParallelEdges((float)Math.PI / 180f);
				if (polygon.nVertices >= 3)
				{
					polys[num] = new Polygon(polygon);
				}
			}
			if (polygon.nVertices >= 3)
			{
				num++;
			}
		}
		return num;
	}

	private static bool IsEar(int i, float[] xv, float[] yv, int xvLength)
	{
		if (i >= xvLength || i < 0 || xvLength < 3)
		{
			return false;
		}
		int num = i + 1;
		int num2 = i - 1;
		float num3;
		float num4;
		float num5;
		float num6;
		if (i == 0)
		{
			num3 = xv[0] - xv[xvLength - 1];
			num4 = yv[0] - yv[xvLength - 1];
			num5 = xv[1] - xv[0];
			num6 = yv[1] - yv[0];
			num2 = xvLength - 1;
		}
		else if (i == xvLength - 1)
		{
			num3 = xv[i] - xv[i - 1];
			num4 = yv[i] - yv[i - 1];
			num5 = xv[0] - xv[i];
			num6 = yv[0] - yv[i];
			num = 0;
		}
		else
		{
			num3 = xv[i] - xv[i - 1];
			num4 = yv[i] - yv[i - 1];
			num5 = xv[i + 1] - xv[i];
			num6 = yv[i + 1] - yv[i];
		}
		float num7 = num3 * num6 - num5 * num4;
		if (num7 > 0f)
		{
			return false;
		}
		Triangle triangle = new Triangle(xv[i], yv[i], xv[num], yv[num], xv[num2], yv[num2]);
		for (int j = 0; j < xvLength; j++)
		{
			if (j != i && j != num2 && j != num && triangle.IsInside(xv[j], yv[j]))
			{
				return false;
			}
		}
		return true;
	}

	private static void ReversePolygon(float[] x, float[] y, int n)
	{
		if (n != 1)
		{
			int num = 0;
			int num2 = n - 1;
			while (num < num2)
			{
				float num3 = x[num];
				x[num] = x[num2];
				x[num2] = num3;
				num3 = y[num];
				y[num] = y[num2];
				y[num2] = num3;
				num++;
				num2--;
			}
		}
	}

	private static int DecomposeConvex(Polygon p, out Polygon[] results, int maxPolys)
	{
		results = new Polygon[1];
		if (p.nVertices < 3)
		{
			return 0;
		}
		Triangle[] results2 = new Triangle[p.nVertices - 2];
		int num;
		if (p.IsCCW())
		{
			Polygon polygon = new Polygon(p);
			ReversePolygon(polygon.x, polygon.y, polygon.nVertices);
			num = TriangulatePolygon(polygon.x, polygon.y, polygon.nVertices, out results2);
		}
		else
		{
			num = TriangulatePolygon(p.x, p.y, p.nVertices, out results2);
		}
		if (num < 1)
		{
			return -1;
		}
		return PolygonizeTriangles(results2, num, out results, maxPolys);
	}

	public static Vertices[] DecomposeVertices(Vertices v, int max)
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		Polygon p = new Polygon(v.ToArray(), v.Count);
		DecomposeConvex(p, out var results, max);
		Vertices[] array = new Vertices[results.Length];
		int i;
		for (i = 0; i < results.Length && results[i] != null; i++)
		{
			array[i] = new Vertices();
			for (int j = 0; j < results[i].nVertices; j++)
			{
				array[i].Add(new Vector2(results[i].x[j], results[i].y[j]));
			}
		}
		Vertices[] array2 = new Vertices[i];
		for (int k = 0; k < i; k++)
		{
			array2[k] = new Vertices(array[k]);
		}
		return array2;
	}
}
