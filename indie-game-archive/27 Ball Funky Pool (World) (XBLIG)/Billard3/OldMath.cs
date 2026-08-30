using System;
using Maximinus;
using Microsoft.Xna.Framework;

namespace Billard3;

public class OldMath
{
	public class lineEquationSimple
	{
		public float A;

		public float B;

		public bool colinearAxisY;

		public float colinearAxisYValue;

		public bool colinearAxisX()
		{
			if (!colinearAxisY)
			{
				return A == 0f;
			}
			return false;
		}

		public float getYfromX(float X)
		{
			Utils.assertStatic(!colinearAxisY, "lineEquation getYfromX but Colinear Axis Y");
			return A * X + B;
		}

		public float getXfromY(float Y)
		{
			Utils.assertStatic(!colinearAxisX(), "lineEquation getXfromY but Colinear Axis X");
			if (colinearAxisY)
			{
				return colinearAxisYValue;
			}
			return (Y - B) / A;
		}
	}

	private const float test_Ball_PROCHE_Line_THRESHOLD = 0.27777767f;

	private const float testPointDansLineTHRESHOLD = 0.01f;

	public static float convert_angle_0_360(float a)
	{
		float num = a % 360f;
		if (num < 0f)
		{
			num += 360f;
		}
		return num;
	}

	public static lineEquationSimple lineSimpleEquationFrom2Points(Vector2 p1, Vector2 p2)
	{
		lineEquationSimple lineEquationSimple2 = new lineEquationSimple();
		Vector2 vector = p2 - p1;
		if (vector.X != 0f)
		{
			lineEquationSimple2.A = vector.Y / vector.X;
			lineEquationSimple2.B = p1.Y - lineEquationSimple2.A * p1.X;
		}
		else
		{
			lineEquationSimple2.colinearAxisY = true;
			lineEquationSimple2.colinearAxisYValue = p1.X;
		}
		return lineEquationSimple2;
	}

	public static float velocityGameUnitsPerFrame_To_MPH(float veloGameUnitsPerFrame)
	{
		return veloGameUnitsPerFrame * 9.204545f;
	}

	public static bool intersectionPointTwoLines(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 outIP)
	{
		Vector2 zero = Vector2.Zero;
		lineEquationSimple lineEquationSimple2 = lineSimpleEquationFrom2Points(p1, p2);
		lineEquationSimple lineEquationSimple3 = lineSimpleEquationFrom2Points(p3, p4);
		if (lineEquationSimple2.colinearAxisX())
		{
			zero.Y = lineEquationSimple2.B;
			if (lineEquationSimple3.colinearAxisX())
			{
				outIP = Vector2.Zero;
				return false;
			}
			zero.X = lineEquationSimple3.getXfromY(zero.Y);
			outIP = zero;
			return true;
		}
		if (lineEquationSimple2.colinearAxisY)
		{
			zero.X = lineEquationSimple2.colinearAxisYValue;
			if (lineEquationSimple3.colinearAxisY)
			{
				outIP = Vector2.Zero;
				return false;
			}
			zero.Y = lineEquationSimple3.getYfromX(zero.X);
			outIP = zero;
			return true;
		}
		if (lineEquationSimple3.colinearAxisX())
		{
			zero.Y = lineEquationSimple3.B;
			zero.X = lineEquationSimple2.getXfromY(zero.Y);
			outIP = zero;
			return true;
		}
		if (lineEquationSimple3.colinearAxisY)
		{
			zero.X = lineEquationSimple3.colinearAxisYValue;
			zero.Y = lineEquationSimple2.getYfromX(zero.X);
			outIP = zero;
			return true;
		}
		if (lineEquationSimple2.A == lineEquationSimple3.A)
		{
			outIP = Vector2.Zero;
			return false;
		}
		zero.X = (lineEquationSimple3.B - lineEquationSimple2.B) / (lineEquationSimple2.A - lineEquationSimple3.A);
		if (Math.Abs(lineEquationSimple2.A) < Math.Abs(lineEquationSimple3.A))
		{
			zero.Y = lineEquationSimple2.getYfromX(zero.X);
		}
		else
		{
			zero.Y = lineEquationSimple3.getYfromX(zero.X);
		}
		outIP = zero;
		return true;
	}

	public static bool testIntersectionEntreDemiDroiteEtSegment(Vector2 demiDroitePos, Vector2 demiDroiteVel, Vector2 segP1, Vector2 segP2, out Vector2 intersectionPoint)
	{
		Vector2 outIP = default(Vector2);
		if (!intersectionPointTwoLines(demiDroitePos, demiDroitePos + demiDroiteVel, segP1, segP2, out outIP))
		{
			intersectionPoint = Vector2.Zero;
			return false;
		}
		float num = Vector2.Distance(outIP, demiDroitePos);
		if (num < 1E-05f)
		{
			outIP = demiDroitePos;
		}
		if (testPointDeDroiteAppartientASegment(segP1, segP2, outIP))
		{
			if (testPointDeDroiteAppartientDemiDroite(demiDroitePos, demiDroiteVel, outIP))
			{
				intersectionPoint = outIP;
				return true;
			}
			intersectionPoint = Vector2.Zero;
			return false;
		}
		intersectionPoint = Vector2.Zero;
		return false;
	}

	public static bool testPointDeDroiteAppartientASegment(Vector2 segP1, Vector2 segP2, Vector2 p)
	{
		if (p.X >= Math.Min(segP1.X, segP2.X) && p.X <= Math.Max(segP1.X, segP2.X) && p.Y >= Math.Min(segP1.Y, segP2.Y))
		{
			return p.Y <= Math.Max(segP1.Y, segP2.Y);
		}
		return false;
	}

	public static bool testPointDeDroiteAppartientDemiDroite(Vector2 pointDemiDroite, Vector2 vecteurDemiDroite, Vector2 p)
	{
		if (vecteurDemiDroite.X >= 0f)
		{
			if (vecteurDemiDroite.Y >= 0f)
			{
				return (p.X >= pointDemiDroite.X) & (p.Y >= pointDemiDroite.Y);
			}
			return (p.X >= pointDemiDroite.X) & (p.Y <= pointDemiDroite.Y);
		}
		if (vecteurDemiDroite.Y > 0f)
		{
			return (p.X <= pointDemiDroite.X) & (p.Y >= pointDemiDroite.Y);
		}
		return (p.X <= pointDemiDroite.X) & (p.Y <= pointDemiDroite.Y);
	}

	public static bool testBallProcheSegment(Vector2 segP1, Vector2 segP2, Vector2 p)
	{
		if (testPointQuelconqueAppartientADroite(segP1, segP2, p, 0.27777767f))
		{
			return testPointQuelconqueAppartientASegment(segP1, segP2, p);
		}
		return false;
	}

	public static bool testPointQuelconqueAppartientASegment(Vector2 segP1, Vector2 segP2, Vector2 p)
	{
		if (testPointQuelconqueAppartientADroite(segP1, segP2, p, 0.01f))
		{
			return testPointDeDroiteAppartientASegment(segP1, segP2, p);
		}
		return false;
	}

	public static bool testPointQuelconqueAppartientADroite(Vector2 segP1, Vector2 segP2, Vector2 p, float approximationThreshold)
	{
		lineEquationSimple lineEquationSimple2 = lineSimpleEquationFrom2Points(segP1, segP2);
		if (lineEquationSimple2.colinearAxisX())
		{
			return p.Y == lineEquationSimple2.B;
		}
		if (lineEquationSimple2.colinearAxisY)
		{
			return p.X == lineEquationSimple2.colinearAxisYValue;
		}
		return Math.Abs(p.Y - (p.X * lineEquationSimple2.A + lineEquationSimple2.B)) <= approximationThreshold;
	}

	public static Vector2 vector2Normal(Vector2 v)
	{
		Vector2 zero = Vector2.Zero;
		zero.X = 0f - v.Y;
		zero.Y = v.X;
		return zero;
	}
}
