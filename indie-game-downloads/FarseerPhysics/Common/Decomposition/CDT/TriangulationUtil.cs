using FarseerPhysics.Common.Decomposition.CDT.Polygon;

namespace FarseerPhysics.Common.Decomposition.CDT;

public static class TriangulationUtil
{
	public const double Epsilon = 1E-12;

	public static bool SmartIncircle(PolygonPoint pa, PolygonPoint pb, PolygonPoint pc, PolygonPoint pd)
	{
		double x = pd.X;
		double y = pd.Y;
		double num = pa.X - x;
		double num2 = pa.Y - y;
		double num3 = pb.X - x;
		double num4 = pb.Y - y;
		double num5 = num * num4;
		double num6 = num3 * num2;
		double num7 = num5 - num6;
		if (num7 <= 0.0)
		{
			return false;
		}
		double num8 = pc.X - x;
		double num9 = pc.Y - y;
		double num10 = num8 * num2;
		double num11 = num * num9;
		double num12 = num10 - num11;
		if (num12 <= 0.0)
		{
			return false;
		}
		double num13 = num3 * num9;
		double num14 = num8 * num4;
		double num15 = num * num + num2 * num2;
		double num16 = num3 * num3 + num4 * num4;
		double num17 = num8 * num8 + num9 * num9;
		double num18 = num15 * (num13 - num14) + num16 * num12 + num17 * num7;
		return num18 > 0.0;
	}

	public static bool InScanArea(PolygonPoint pa, PolygonPoint pb, PolygonPoint pc, PolygonPoint pd)
	{
		double x = pd.X;
		double y = pd.Y;
		double num = pa.X - x;
		double num2 = pa.Y - y;
		double num3 = pb.X - x;
		double num4 = pb.Y - y;
		double num5 = num * num4;
		double num6 = num3 * num2;
		double num7 = num5 - num6;
		if (num7 <= 0.0)
		{
			return false;
		}
		double num8 = pc.X - x;
		double num9 = pc.Y - y;
		double num10 = num8 * num2;
		double num11 = num * num9;
		double num12 = num10 - num11;
		if (num12 <= 0.0)
		{
			return false;
		}
		return true;
	}

	public static Orientation Orient2D(PolygonPoint pa, PolygonPoint pb, PolygonPoint pc)
	{
		double num = (pa.X - pc.X) * (pb.Y - pc.Y);
		double num2 = (pa.Y - pc.Y) * (pb.X - pc.X);
		double num3 = num - num2;
		if (num3 > -1E-12 && num3 < 1E-12)
		{
			return Orientation.Collinear;
		}
		if (num3 > 0.0)
		{
			return Orientation.CCW;
		}
		return Orientation.CW;
	}
}
