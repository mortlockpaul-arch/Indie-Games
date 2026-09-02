using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RacingGame.Landscapes;

namespace RacingGame.Tracks;

public class TrackLine
{
	public class RoadHelperPosition
	{
		public TrackData.RoadHelper.HelperType type;

		public int startNum;

		public int endNum;

		public RoadHelperPosition(TrackData.RoadHelper.HelperType setType, int setStartNum, int setEndNum)
		{
			type = setType;
			startNum = setStartNum;
			endNum = setEndNum;
		}
	}

	protected const int NumberOfIterationsPer100Meters = 40;

	private const float CurveFactor = 0.25f;

	private const float UpFactorCorrector = 0.6f;

	private const float RoadTextureStrechFactor = 0.125f;

	private const int NumberOfUpSmoothValues = 10;

	private const float MinimumLandscapeDistance = 2f;

	private static readonly Vector3[] LoopingPoints;

	protected List<TrackVertex> points = new List<TrackVertex>();

	protected List<RoadHelperPosition> helperPositions = new List<RoadHelperPosition>();

	public TrackLine(Vector3[] inputPoints, List<TrackData.WidthHelper> widthHelpers, List<TrackData.RoadHelper> roadHelpers, List<TrackData.NeutralObject> neutralObjects, Landscape landscape)
	{
		Load(inputPoints, widthHelpers, roadHelpers, neutralObjects, landscape);
	}

	public TrackLine(TrackData inputPointsFromTrack, Landscape landscape)
		: this(inputPointsFromTrack.TrackPoints.ToArray(), inputPointsFromTrack.WidthHelpers, inputPointsFromTrack.RoadHelpers, inputPointsFromTrack.NeutralsObjects, landscape)
	{
	}

	protected void Load(Vector3[] inputPoints, List<TrackData.WidthHelper> widthHelpers, List<TrackData.RoadHelper> roadHelpers, List<TrackData.NeutralObject> neutralObjects, Landscape landscape)
	{
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_034a: Unknown result type (might be due to invalid IL or missing references)
		//IL_034f: Unknown result type (might be due to invalid IL or missing references)
		//IL_035b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0360: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_052d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0541: Unknown result type (might be due to invalid IL or missing references)
		//IL_0551: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0566: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_056d: Unknown result type (might be due to invalid IL or missing references)
		//IL_056f: Unknown result type (might be due to invalid IL or missing references)
		//IL_061e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0731: Unknown result type (might be due to invalid IL or missing references)
		//IL_0739: Unknown result type (might be due to invalid IL or missing references)
		//IL_073e: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0750: Unknown result type (might be due to invalid IL or missing references)
		//IL_0759: Unknown result type (might be due to invalid IL or missing references)
		//IL_075e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_076d: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0771: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0401: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_042f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_045f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_0788: Unknown result type (might be due to invalid IL or missing references)
		//IL_078d: Unknown result type (might be due to invalid IL or missing references)
		//IL_078f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0794: Unknown result type (might be due to invalid IL or missing references)
		//IL_096f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0483: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_048c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_0596: Unknown result type (might be due to invalid IL or missing references)
		//IL_0598: Unknown result type (might be due to invalid IL or missing references)
		//IL_059a: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_098c: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0707: Unknown result type (might be due to invalid IL or missing references)
		//IL_0709: Unknown result type (might be due to invalid IL or missing references)
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_081c: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0833: Unknown result type (might be due to invalid IL or missing references)
		//IL_0835: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_083b: Unknown result type (might be due to invalid IL or missing references)
		//IL_083d: Unknown result type (might be due to invalid IL or missing references)
		//IL_083f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_084e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0856: Unknown result type (might be due to invalid IL or missing references)
		//IL_0858: Unknown result type (might be due to invalid IL or missing references)
		//IL_085f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0864: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a0f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a21: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a26: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a39: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0883: Unknown result type (might be due to invalid IL or missing references)
		//IL_0885: Unknown result type (might be due to invalid IL or missing references)
		//IL_088c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0891: Unknown result type (might be due to invalid IL or missing references)
		//IL_086c: Unknown result type (might be due to invalid IL or missing references)
		//IL_086e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0870: Unknown result type (might be due to invalid IL or missing references)
		//IL_087a: Unknown result type (might be due to invalid IL or missing references)
		//IL_087f: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0901: Unknown result type (might be due to invalid IL or missing references)
		//IL_0903: Unknown result type (might be due to invalid IL or missing references)
		//IL_0908: Unknown result type (might be due to invalid IL or missing references)
		//IL_091e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0920: Unknown result type (might be due to invalid IL or missing references)
		//IL_0925: Unknown result type (might be due to invalid IL or missing references)
		//IL_0927: Unknown result type (might be due to invalid IL or missing references)
		//IL_0929: Unknown result type (might be due to invalid IL or missing references)
		//IL_092e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0944: Unknown result type (might be due to invalid IL or missing references)
		//IL_0946: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fd: Unknown result type (might be due to invalid IL or missing references)
		points.Clear();
		helperPositions.Clear();
		landscape?.KillAllLoadedObjects();
		if (inputPoints == null || inputPoints.Length < 3)
		{
			throw new ArgumentException("inputPoints is invalid, we need at least 3 valid input points to generate a TrackLine.");
		}
		if (landscape != null)
		{
			for (int i = 0; i < inputPoints.Length; i++)
			{
				float num = landscape.GetMapHeight(inputPoints[i].X, inputPoints[i].Y) + 4.5f;
				if (inputPoints[i].Z < num)
				{
					inputPoints[i].Z = num;
				}
			}
			for (int j = 0; j < inputPoints.Length; j++)
			{
				for (int k = 1; k < 25; k++)
				{
					float num2 = (float)k / 25f;
					float num3 = inputPoints[j].Z * (1f - num2) + inputPoints[(j + 1) % inputPoints.Length].Z * num2;
					for (int l = 0; l < 2; l++)
					{
						for (int m = 0; m < 2; m++)
						{
							float num4 = landscape.GetMapHeight(-5f + 10f * (float)l + inputPoints[j].X * (1f - num2) + inputPoints[(j + 1) % inputPoints.Length].X * num2, -5f + 10f * (float)m + inputPoints[j].Y * (1f - num2) + inputPoints[(j + 1) % inputPoints.Length].Y * num2) + 3.2f;
							if (num3 < num4)
							{
								float num5 = num4 - num3;
								ref Vector3 reference = ref inputPoints[j];
								reference.Z += num5;
								ref Vector3 reference2 = ref inputPoints[(j + 1) % inputPoints.Length];
								reference2.Z += num5;
							}
						}
					}
				}
			}
		}
		Vector3 val4 = default(Vector3);
		Matrix val6 = default(Matrix);
		for (int n = 1; n < inputPoints.Length - 3; n++)
		{
			Vector3 val = inputPoints[n + 1] - inputPoints[n];
			float num6 = (float)Math.Sqrt(val.X * val.X + val.Y * val.Y);
			float num7 = Math.Abs(val.Z);
			Vector3 val2 = inputPoints[n + 2] - inputPoints[n + 1];
			if (!(num7 / 2f > num6) || !(Math.Abs(val.Z + val2.Z) < num7 / 2f))
			{
				continue;
			}
			Vector3 val3 = inputPoints[n] - inputPoints[n - 1];
			((Vector3)(ref val3)).Normalize();
			((Vector3)(ref val4))._002Ector(0f, 0f, 1f);
			Vector3 val5 = Vector3.Cross(val3, val4);
			((Matrix)(ref val6))._002Ector(val5.X, val5.Y, val5.Z, 0f, val3.X, val3.Y, val3.Z, 0f, val4.X, val4.Y, val4.Z, 0f, 0f, 0f, 0f, 1f);
			Vector3 val7 = inputPoints[n];
			Vector3 val8 = inputPoints[n + 2];
			Vector3[] array = (Vector3[])inputPoints.Clone();
			inputPoints = (Vector3[])(object)new Vector3[inputPoints.Length + 7];
			for (int num8 = 0; num8 < array.Length; num8++)
			{
				if (num8 < n)
				{
					ref Vector3 reference3 = ref inputPoints[num8];
					reference3 = array[num8];
				}
				else
				{
					ref Vector3 reference4 = ref inputPoints[num8 + 7];
					reference4 = array[num8];
				}
			}
			for (int num9 = 0; num9 < LoopingPoints.Length; num9++)
			{
				float num10 = (float)num9 / (float)(LoopingPoints.Length - 1);
				ref Vector3 reference5 = ref inputPoints[n + num9];
				reference5 = val7 * (1f - num10) + val8 * num10 + num7 * Vector3.Transform(LoopingPoints[num9], val6);
			}
			Vector3 val9 = inputPoints[n + 10] - inputPoints[n + 8];
			if (((Vector3)(ref val9)).Length() > num7 * 2f)
			{
				((Vector3)(ref val9)).Normalize();
				val9 *= num7;
				ref Vector3 reference6 = ref inputPoints[n + 9];
				reference6 = inputPoints[n + 8] + val9;
			}
			else
			{
				ref Vector3 reference7 = ref inputPoints[n + 9];
				reference7 = (inputPoints[n + 8] + inputPoints[n + 10]) / 2f;
			}
			n += 10;
		}
		for (int num11 = 0; num11 < inputPoints.Length; num11++)
		{
			Vector3 val10 = inputPoints[(num11 - 1 < 0) ? (inputPoints.Length - 1) : (num11 - 1)];
			Vector3 val11 = inputPoints[num11];
			Vector3 val12 = inputPoints[(num11 + 1) % inputPoints.Length];
			Vector3 val13 = inputPoints[(num11 + 2) % inputPoints.Length];
			float num12 = Vector3.Distance(val11, val12);
			int num13 = (int)(40f * (num12 / 100f));
			if (num13 <= 0)
			{
				num13 = 1;
			}
			for (int num14 = 0; num14 < num13; num14++)
			{
				TrackVertex item = new TrackVertex(Vector3.CatmullRom(val10, val11, val12, val13, (float)num14 / (float)num13));
				points.Add(item);
			}
		}
		List<Vector3> list = new List<Vector3>();
		Vector3 val14 = default(Vector3);
		((Vector3)(ref val14))._002Ector(0f, 0f, 1f);
		Vector3 val15 = val14;
		for (int num15 = 0; num15 < points.Count; num15++)
		{
			Vector3 dir = points[(num15 + 1) % points.Count].pos - points[(num15 - 1 < 0) ? (points.Count - 1) : (num15 - 1)].pos;
			((Vector3)(ref dir)).Normalize();
			Vector3 val16 = (points[(num15 + 1) % points.Count].pos + points[(num15 - 1 < 0) ? (points.Count - 1) : (num15 - 1)].pos) / 2f;
			Vector3 val17 = val16 - points[num15].pos;
			if (((Vector3)(ref val17)).Length() < 0.0001f)
			{
				val17 = val15;
			}
			((Vector3)(ref val17)).Normalize();
			list.Add(val17);
			points[num15].dir = dir;
			val15 = val17;
		}
		list[0] = list[list.Count - 1] + list[1];
		Vector3 val18 = list[0];
		((Vector3)(ref val18)).Normalize();
		val15 = Vector3.Lerp(val14, list[0], 0.22500001f);
		Vector3 val19 = val15;
		for (int num16 = 0; num16 < points.Count; num16++)
		{
			Vector3 dir2 = points[num16].dir;
			Vector3 val20 = Vector3.Zero;
			for (int num17 = -5; num17 <= 5; num17++)
			{
				val20 += list[(num16 + points.Count + num17) % points.Count];
			}
			((Vector3)(ref val20)).Normalize();
			bool flag = val20.Z < -0.25f && val19.Z < -0.05f;
			bool flag2 = dir2.Z > 0.75f;
			bool flag3 = dir2.Z < -0.75f;
			val20 = Vector3.Lerp(val15, val20, 0.25f);
			((Vector3)(ref val20)).Normalize();
			val19 = val20;
			val15 = (flag2 ? Vector3.Lerp(val20, -val14, 0.6f) : (flag3 ? Vector3.Lerp(val20, val14, 0.6f) : ((!flag) ? Vector3.Lerp(val20, val14, 0.6f) : Vector3.Lerp(val20, -val14, 0.6f))));
			if (landscape != null)
			{
				float mapHeight = landscape.GetMapHeight(points[num16].pos.X, points[num16].pos.Y);
				if (points[num16].pos.Z - mapHeight < 8f)
				{
					val15 = Vector3.Lerp(val20, val14, 1.0500001f);
				}
			}
			Vector3 val21 = Vector3.Cross(dir2, val20);
			((Vector3)(ref val21)).Normalize();
			points[num16].right = val21;
			val20 = Vector3.Cross(val21, dir2);
			((Vector3)(ref val20)).Normalize();
			points[num16].up = val20;
		}
		val15 = points[0].up;
		for (int num18 = 0; num18 < points.Count; num18++)
		{
			list[num18] = points[num18].up;
		}
		for (int num19 = 0; num19 < points.Count; num19++)
		{
			Vector3 val22 = Vector3.Zero;
			for (int num20 = -10; num20 <= 10; num20++)
			{
				val22 += list[(num19 + points.Count + num20) % points.Count];
			}
			((Vector3)(ref val22)).Normalize();
			points[num19].up = val22;
			Vector3 dir3 = points[num19].dir;
			points[num19].right = Vector3.Cross(dir3, val22);
		}
		AdjustRoadWidths(widthHelpers);
		GenerateUTextureCoordinates();
		GenerateTunnelsAndLandscapeObjects(roadHelpers, neutralObjects, landscape);
	}

	protected void Load(TrackData trackData, Landscape landscape)
	{
		Load(trackData.TrackPoints.ToArray(), trackData.WidthHelpers, trackData.RoadHelpers, trackData.NeutralsObjects, landscape);
	}

	private void AdjustRoadWidths(List<TrackData.WidthHelper> widthHelpers)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		float num = 1f;
		float num2 = num;
		for (int i = 0; i < points.Count; i++)
		{
			Vector3 pos = points[i].pos;
			foreach (TrackData.WidthHelper widthHelper in widthHelpers)
			{
				float num3 = Vector3.Distance(widthHelper.pos, pos);
				if (num3 < 25f)
				{
					float num4 = 1f - num3 / 25f;
					num2 = (1f - num4) * num2 + num4 * widthHelper.scale;
				}
			}
			num = num * 0.9f + num2 * 0.1f;
			if (i > points.Count - 7)
			{
				float num5 = ((i == points.Count - 1) ? 0.75f : ((i == points.Count - 2) ? 0.5f : ((i == points.Count - 2) ? 0.25f : 0.175f)));
				num = num5 * points[0].roadWidth + (1f - num5) * num;
			}
			if (num < 0.25f)
			{
				num = 0.25f;
			}
			if (num > 2f)
			{
				num = 2f;
			}
			points[i].roadWidth = num;
		}
	}

	private void GenerateUTextureCoordinates()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		for (int i = 0; i < points.Count; i++)
		{
			points[i].uv.X = num;
			float num2 = num;
			Vector3 val = points[(i + 1) % points.Count].pos - points[i % points.Count].pos;
			num = num2 + 0.125f * ((Vector3)(ref val)).Length();
		}
		points.Add(new TrackVertex(points[0].pos, points[0].right, points[0].up, points[0].dir, new Vector2(num, 0f), points[0].roadWidth));
	}

	private void GenerateTunnelsAndLandscapeObjects(List<TrackData.RoadHelper> roadHelpers, List<TrackData.NeutralObject> neutralObjects, Landscape landscape)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		int num = -1;
		TrackData.RoadHelper.HelperType setType = TrackData.RoadHelper.HelperType.Reset;
		for (int i = 0; i < points.Count; i++)
		{
			Vector3 pos = points[i].pos;
			foreach (TrackData.RoadHelper roadHelper in roadHelpers)
			{
				float num2 = Vector3.Distance(roadHelper.pos, pos);
				if (!(num2 < 25f))
				{
					continue;
				}
				if (num >= 0)
				{
					helperPositions.Add(new RoadHelperPosition(setType, num, i));
					if (roadHelper.type == TrackData.RoadHelper.HelperType.Reset)
					{
						num = -1;
					}
					else
					{
						num = i;
						setType = roadHelper.type;
					}
				}
				else
				{
					num = i;
					setType = roadHelper.type;
				}
				roadHelpers.Remove(roadHelper);
				break;
			}
		}
		if (num > 0)
		{
			helperPositions.Add(new RoadHelperPosition(setType, num, points.Count - 3));
		}
		if (landscape != null)
		{
			for (int j = 0; j < neutralObjects.Count; j++)
			{
				TrackData.NeutralObject neutralObject = neutralObjects[j];
				landscape.AddObjectToRender(neutralObject.modelName, neutralObject.matrix, isNearTrackForShadowGeneration: false);
			}
		}
	}

	static TrackLine()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		LoopingPoints = (Vector3[])(object)new Vector3[9]
		{
			new Vector3(0f, 0f, 0f),
			new Vector3(0f, 0.353553f, 0.146447f),
			new Vector3(0f, 0.5f, 0.5f),
			new Vector3(0f, 0.353553f, 0.853553f),
			new Vector3(0f, 0f, 1f),
			new Vector3(0f, -0.353553f, 0.853553f),
			new Vector3(0f, -0.5f, 0.5f),
			new Vector3(0f, -0.353553f, 0.146447f),
			new Vector3(0f, 0f, 0f)
		};
	}
}
