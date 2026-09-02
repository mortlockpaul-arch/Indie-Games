using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Graphics;
using RacingGame.Helpers;
using RacingGame.Landscapes;
using RacingGame.Shaders;

namespace RacingGame.Tracks;

public class Track : TrackLine, IDisposable
{
	private const float RoadBackHullTextureWidthFactor = 1f;

	private const float RoadTunnelTextureWidthFactor = 0.25f;

	private const float RoadBackSideTextureHeight = 0.135f;

	private const float RoadTunnelSideTextureHeight = 0.235f;

	private const float PalmAndLaternGap = 20f;

	private const float CheckpointGap = 500f;

	private const float SignGap = 24f;

	private Material roadMaterial;

	private Material roadBackMaterial;

	private Material roadTunnelMaterial;

	private Material roadCementMaterial;

	private Material guardRailMaterial;

	private TangentVertex[] roadVertices;

	private VertexBuffer roadVb;

	private IndexBuffer roadIb;

	private TangentVertex[] roadBackVertices;

	private VertexBuffer roadBackVb;

	private IndexBuffer roadBackIb;

	private TangentVertex[] roadTunnelVertices;

	private int[] roadTunnelIndices;

	private VertexBuffer roadTunnelVb;

	private IndexBuffer roadTunnelIb;

	private GuardRail leftRail;

	private GuardRail rightRail;

	private TrackColumns columns;

	private List<int> checkpointSegmentPositions;

	internal static bool disableLensFlareInTunnel;

	public Vector3 StartPosition
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return points[0].pos;
		}
	}

	public Vector3 StartDirection
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return points[0].dir;
		}
	}

	public Vector3 StartUpVector
	{
		get
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			return points[0].up;
		}
	}

	public float Length => (float)points.Count * 100f / 40f;

	public int NumberOfSegments => points.Count;

	public List<int> CheckpointSegmentPositions => checkpointSegmentPositions;

	public Track(string setTrackName, Landscape landscape)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		roadMaterial = new Material("Road", "RoadNormal");
		roadBackMaterial = new Material("Road", "RoadBackNormal");
		roadTunnelMaterial = new Material(new Color((byte)182, (byte)182, (byte)182), new Color((byte)80, (byte)80, (byte)80), new Color((byte)64, (byte)64, (byte)64), "RoadTunnel", "RoadTunnelNormal", "", "");
		roadCementMaterial = new Material("RoadCement", "RoadCementNormal");
		guardRailMaterial = new Material(new Color((byte)72, (byte)72, (byte)72), new Color((byte)182, (byte)182, (byte)182), new Color((byte)225, (byte)225, (byte)225), "banner", "LeitplankeNormal", "", "");
		checkpointSegmentPositions = new List<int>();
		base._002Ector(TrackData.Load(setTrackName), landscape);
		GenerateVerticesAndObjects(landscape);
	}

	public void Reload(string setTrackName, Landscape landscape)
	{
		Load(TrackData.Load(setTrackName), landscape);
		GenerateVerticesAndObjects(landscape);
	}

	private void GenerateVerticesAndObjects(Landscape landscape)
	{
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Expected O, but got Unknown
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fd: Expected O, but got Unknown
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02df: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Expected O, but got Unknown
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e5: Expected O, but got Unknown
		//IL_07d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07de: Expected O, but got Unknown
		//IL_05d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0636: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0691: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0708: Unknown result type (might be due to invalid IL or missing references)
		//IL_070d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0721: Unknown result type (might be due to invalid IL or missing references)
		//IL_072b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0730: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_074c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0751: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		//IL_0952: Unknown result type (might be due to invalid IL or missing references)
		//IL_095c: Expected O, but got Unknown
		roadVertices = new TangentVertex[points.Count * 5];
		for (int i = 0; i < points.Count; i++)
		{
			ref TangentVertex reference = ref roadVertices[i * 5];
			reference = points[i].RightTangentVertex;
			ref TangentVertex reference2 = ref roadVertices[i * 5 + 1];
			reference2 = points[i].MiddleRightTangentVertex;
			ref TangentVertex reference3 = ref roadVertices[i * 5 + 2];
			reference3 = points[i].MiddleTangentVertex;
			ref TangentVertex reference4 = ref roadVertices[i * 5 + 3];
			reference4 = points[i].MiddleLeftTangentVertex;
			ref TangentVertex reference5 = ref roadVertices[i * 5 + 4];
			reference5 = points[i].LeftTangentVertex;
		}
		roadVb = new VertexBuffer(BaseGame.Device, typeof(TangentVertex), roadVertices.Length, (BufferUsage)8);
		roadVb.SetData<TangentVertex>(roadVertices);
		int[] array = new int[(points.Count - 1) * 8 * 3];
		int num = 0;
		for (int j = 0; j < points.Count - 1; j++)
		{
			for (int k = 0; k < 4; k++)
			{
				array[j * 24 + 6 * k] = num + k;
				array[j * 24 + 6 * k + 1] = num + 5 + 1 + k;
				array[j * 24 + 6 * k + 2] = num + 5 + k;
				array[j * 24 + 6 * k + 3] = num + 5 + 1 + k;
				array[j * 24 + 6 * k + 4] = num + k;
				array[j * 24 + 6 * k + 5] = num + 1 + k;
			}
			num += 5;
		}
		roadIb = new IndexBuffer(BaseGame.Device, typeof(int), array.Length, (BufferUsage)8);
		roadIb.SetData<int>(array);
		roadBackVertices = new TangentVertex[points.Count * 4];
		for (int l = 0; l < points.Count; l++)
		{
			ref TangentVertex reference6 = ref roadBackVertices[l * 4];
			reference6 = points[l].LeftTangentVertex;
			roadBackVertices[l * 4].uv = new Vector2(roadBackVertices[l * 4].U * 1f, 0f);
			ref TangentVertex reference7 = ref roadBackVertices[l * 4 + 1];
			reference7 = points[l].BottomLeftSideTangentVertex;
			roadBackVertices[l * 4 + 1].uv = new Vector2(roadBackVertices[l * 4].U * 1f, 0.135f);
			ref TangentVertex reference8 = ref roadBackVertices[l * 4 + 2];
			reference8 = points[l].BottomRightSideTangentVertex;
			roadBackVertices[l * 4 + 2].uv = new Vector2(roadBackVertices[l * 4].U * 1f, 0.865f);
			ref TangentVertex reference9 = ref roadBackVertices[l * 4 + 3];
			reference9 = points[l].RightTangentVertex;
			roadBackVertices[l * 4 + 3].uv = new Vector2(roadBackVertices[l * 4 + 3].U * 1f, 1f);
		}
		roadBackVb = new VertexBuffer(BaseGame.Device, typeof(TangentVertex), roadBackVertices.Length, (BufferUsage)8);
		roadBackVb.SetData<TangentVertex>(roadBackVertices);
		int[] array2 = new int[(points.Count - 1) * 6 * 3];
		num = 0;
		for (int m = 0; m < points.Count - 1; m++)
		{
			for (int n = 0; n < 3; n++)
			{
				array2[m * 18 + 6 * n] = num + n;
				array2[m * 18 + 6 * n + 1] = num + 5 + n;
				array2[m * 18 + 6 * n + 2] = num + 4 + n;
				array2[m * 18 + 6 * n + 3] = num + 5 + n;
				array2[m * 18 + 6 * n + 4] = num + n;
				array2[m * 18 + 6 * n + 5] = num + 1 + n;
			}
			num += 4;
		}
		roadBackIb = new IndexBuffer(BaseGame.Device, typeof(int), array2.Length, (BufferUsage)8);
		roadBackIb.SetData<int>(array2);
		int num2 = 0;
		foreach (RoadHelperPosition helperPosition in helperPositions)
		{
			if (helperPosition.type == TrackData.RoadHelper.HelperType.Tunnel)
			{
				num2 += 1 + (helperPosition.endNum - helperPosition.startNum);
			}
		}
		roadTunnelVertices = new TangentVertex[num2 * 4];
		num = 0;
		foreach (RoadHelperPosition helperPosition2 in helperPositions)
		{
			if (helperPosition2.type == TrackData.RoadHelper.HelperType.Tunnel)
			{
				for (int num3 = helperPosition2.startNum; num3 <= helperPosition2.endNum; num3++)
				{
					ref TangentVertex reference10 = ref roadTunnelVertices[num];
					reference10 = points[num3].LeftTangentVertex;
					roadTunnelVertices[num].uv = new Vector2(roadTunnelVertices[num].U * 0.25f, 0f);
					ref TangentVertex reference11 = ref roadTunnelVertices[num + 1];
					reference11 = points[num3].TunnelTopLeftSideTangentVertex;
					roadTunnelVertices[num + 1].uv = new Vector2(roadTunnelVertices[num + 1].U * 0.25f, 0.235f);
					ref TangentVertex reference12 = ref roadTunnelVertices[num + 2];
					reference12 = points[num3].TunnelTopRightSideTangentVertex;
					roadTunnelVertices[num + 2].uv = new Vector2(roadTunnelVertices[num + 2].U * 0.25f, 0.765f);
					ref TangentVertex reference13 = ref roadTunnelVertices[num + 3];
					reference13 = points[num3].RightTangentVertex;
					roadTunnelVertices[num + 3].uv = new Vector2(roadTunnelVertices[num + 3].U * 0.25f, 1f);
					ref TangentVertex reference14 = ref roadTunnelVertices[num];
					reference14.normal *= -1f;
					ref TangentVertex reference15 = ref roadTunnelVertices[num + 3];
					reference15.normal *= -1f;
					ref TangentVertex reference16 = ref roadTunnelVertices[num];
					reference16.tangent *= -1f;
					ref TangentVertex reference17 = ref roadTunnelVertices[num + 3];
					reference17.tangent *= -1f;
					num += 4;
				}
			}
		}
		if (roadTunnelVertices.Length > 0)
		{
			roadTunnelVb = new VertexBuffer(BaseGame.Device, typeof(TangentVertex), roadTunnelVertices.Length, (BufferUsage)8);
			roadTunnelVb.SetData<TangentVertex>(roadTunnelVertices);
			int num4 = 0;
			foreach (RoadHelperPosition helperPosition3 in helperPositions)
			{
				if (helperPosition3.type == TrackData.RoadHelper.HelperType.Tunnel)
				{
					num4 += helperPosition3.endNum - helperPosition3.startNum;
				}
			}
			roadTunnelIndices = new int[num4 * 6 * 3];
			num = 0;
			int num5 = 0;
			foreach (RoadHelperPosition helperPosition4 in helperPositions)
			{
				if (helperPosition4.type != TrackData.RoadHelper.HelperType.Tunnel)
				{
					continue;
				}
				for (int num6 = helperPosition4.startNum; num6 < helperPosition4.endNum; num6++)
				{
					for (int num7 = 0; num7 < 3; num7++)
					{
						roadTunnelIndices[num5] = num + num7;
						roadTunnelIndices[num5 + 2] = num + 4 + num7;
						roadTunnelIndices[num5 + 1] = num + 5 + num7;
						roadTunnelIndices[num5 + 3] = num + 5 + num7;
						roadTunnelIndices[num5 + 5] = num + 1 + num7;
						roadTunnelIndices[num5 + 4] = num + num7;
						num5 += 6;
					}
					num += 4;
				}
				num += 4;
			}
			roadTunnelIb = new IndexBuffer(BaseGame.Device, typeof(int), roadTunnelIndices.Length, (BufferUsage)8);
			roadTunnelIb.SetData<int>(roadTunnelIndices);
		}
		leftRail = new GuardRail(points, GuardRail.Modes.Left, landscape);
		rightRail = new GuardRail(points, GuardRail.Modes.Right, landscape);
		columns = new TrackColumns(points, landscape);
		GenerateObjectsForTrack(landscape);
	}

	private void GenerateObjectsForTrack(Landscape landscape)
	{
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Unknown result type (might be due to invalid IL or missing references)
		//IL_0421: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0444: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_044e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0453: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0486: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04da: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_052a: Unknown result type (might be due to invalid IL or missing references)
		//IL_053c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0555: Unknown result type (might be due to invalid IL or missing references)
		//IL_055a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_056e: Unknown result type (might be due to invalid IL or missing references)
		//IL_057d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05db: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b5f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b72: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b77: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b86: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		//IL_061a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Unknown result type (might be due to invalid IL or missing references)
		//IL_062e: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_066d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0672: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_068a: Unknown result type (might be due to invalid IL or missing references)
		//IL_068c: Unknown result type (might be due to invalid IL or missing references)
		//IL_068e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0694: Unknown result type (might be due to invalid IL or missing references)
		//IL_0699: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_076c: Unknown result type (might be due to invalid IL or missing references)
		//IL_076e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0770: Unknown result type (might be due to invalid IL or missing references)
		//IL_0777: Unknown result type (might be due to invalid IL or missing references)
		//IL_077c: Unknown result type (might be due to invalid IL or missing references)
		//IL_079a: Unknown result type (might be due to invalid IL or missing references)
		//IL_079f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07df: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_07e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0804: Unknown result type (might be due to invalid IL or missing references)
		//IL_0809: Unknown result type (might be due to invalid IL or missing references)
		//IL_0822: Unknown result type (might be due to invalid IL or missing references)
		//IL_0827: Unknown result type (might be due to invalid IL or missing references)
		//IL_083b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0840: Unknown result type (might be due to invalid IL or missing references)
		//IL_0845: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_070a: Unknown result type (might be due to invalid IL or missing references)
		//IL_070f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0714: Unknown result type (might be due to invalid IL or missing references)
		//IL_0719: Unknown result type (might be due to invalid IL or missing references)
		//IL_071b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0720: Unknown result type (might be due to invalid IL or missing references)
		//IL_0722: Unknown result type (might be due to invalid IL or missing references)
		//IL_0727: Unknown result type (might be due to invalid IL or missing references)
		//IL_088b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_0897: Unknown result type (might be due to invalid IL or missing references)
		//IL_089c: Unknown result type (might be due to invalid IL or missing references)
		//IL_089e: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0900: Unknown result type (might be due to invalid IL or missing references)
		//IL_0905: Unknown result type (might be due to invalid IL or missing references)
		//IL_090a: Unknown result type (might be due to invalid IL or missing references)
		//IL_090c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0911: Unknown result type (might be due to invalid IL or missing references)
		//IL_0913: Unknown result type (might be due to invalid IL or missing references)
		//IL_0918: Unknown result type (might be due to invalid IL or missing references)
		//IL_093c: Unknown result type (might be due to invalid IL or missing references)
		//IL_096f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0974: Unknown result type (might be due to invalid IL or missing references)
		//IL_0979: Unknown result type (might be due to invalid IL or missing references)
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0980: Unknown result type (might be due to invalid IL or missing references)
		//IL_0985: Unknown result type (might be due to invalid IL or missing references)
		//IL_0987: Unknown result type (might be due to invalid IL or missing references)
		//IL_098c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c9b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cad: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_035f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_036b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0372: Unknown result type (might be due to invalid IL or missing references)
		//IL_0377: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0287: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a47: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a94: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a99: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a9e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aa5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aaa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aac: Unknown result type (might be due to invalid IL or missing references)
		//IL_0ab1: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		int num2 = 0;
		for (int i = 0; i < points.Count; i++)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (RoadHelperPosition helperPosition in helperPositions)
			{
				if (i >= helperPosition.startNum && i <= helperPosition.endNum)
				{
					if (helperPosition.type == TrackData.RoadHelper.HelperType.Palms)
					{
						flag = true;
					}
					else if (helperPosition.type == TrackData.RoadHelper.HelperType.Laterns)
					{
						flag2 = true;
					}
				}
			}
			if (!flag && !flag2)
			{
				continue;
			}
			float num3 = Vector3.Distance(points[(i + 1) % points.Count].pos, points[i].pos);
			if (num - num3 <= 0f)
			{
				Vector3 right = points[i].right;
				Vector3 dir = points[i].dir;
				Vector3 up = points[i].up;
				bool flag3 = up.Z < 0.05f;
				bool flag4 = dir.Z > 0.65f;
				bool flag5 = dir.Z < -0.65f;
				if (flag3 || flag4 || flag5)
				{
					continue;
				}
				Matrix identity = Matrix.Identity;
				((Matrix)(ref identity)).Right = right;
				((Matrix)(ref identity)).Up = dir;
				((Matrix)(ref identity)).Forward = -up;
				Vector3 pos = points[(i - 1 < 0) ? (points.Count - 1) : (i - 1)].pos;
				Vector3 pos2 = points[i].pos;
				Vector3 pos3 = points[(i + 1) % points.Count].pos;
				Vector3 pos4 = points[(i + 2) % points.Count].pos;
				Vector3 val = Vector3.CatmullRom(pos, pos2, pos3, pos4, num / num3);
				num2++;
				if (landscape != null)
				{
					if (flag)
					{
						if (val.Z - landscape.GetMapHeight(val.X, val.Y) < 11f)
						{
							int randomInt = RandomHelper.GetRandomInt(4);
							if (randomInt == 3)
							{
								randomInt = RandomHelper.GetRandomInt(4);
							}
							landscape.AddObjectToRender(randomInt switch
							{
								2 => "AlphaPalm3", 
								1 => "AlphaPalm2", 
								0 => "AlphaPalm", 
								_ => "AlphaPalmSmall", 
							}, Matrix.CreateScale(1.25f) * Matrix.CreateRotationZ(RandomHelper.GetRandomFloat(0f, (float)Math.PI * 2f)) * Matrix.CreateTranslation(right * ((num2 % 2 == 0) ? 0.6f : (-0.6f)) * points[i].roadWidth * 13.25f) * Matrix.CreateTranslation(new Vector3(0f, 0f, -50f)) * Matrix.CreateTranslation(val), isNearTrackForShadowGeneration: true);
						}
					}
					else
					{
						landscape.AddObjectToRender("Laterne", Matrix.CreateRotationZ((num2 % 2 == 0) ? ((float)Math.PI) : 0f) * Matrix.CreateTranslation(new Vector3(((num2 % 2 == 0) ? 0.5f : (-0.5f)) * points[i].roadWidth * 13.25f - 0.35f, 0f, -0.2f)) * identity * Matrix.CreateTranslation(val), isNearTrackForShadowGeneration: true);
					}
				}
				num += 20f;
			}
			num -= num3;
		}
		if (landscape != null)
		{
			Vector3 right2 = points[0].right;
			Vector3 dir2 = points[0].dir;
			Vector3 up2 = points[0].up;
			Matrix identity2 = Matrix.Identity;
			((Matrix)(ref identity2)).Right = right2;
			((Matrix)(ref identity2)).Up = dir2;
			((Matrix)(ref identity2)).Forward = -up2;
			landscape.AddObjectToRender("Banner6", Matrix.CreateScale(points[0].roadWidth) * Matrix.CreateScale(1.051f) * Matrix.CreateTranslation(new Vector3(0f, -5.1f, 0f)) * identity2 * Matrix.CreateTranslation(points[0].pos), isNearTrackForShadowGeneration: true);
			landscape.AddObjectToRender("StartLight3", Matrix.CreateScale(1.1f) * Matrix.CreateTranslation(new Vector3(points[0].roadWidth * 13.25f * 0.5f - 0.3f, 6f, -0.2f)) * identity2 * Matrix.CreateTranslation(points[0].pos), isNearTrackForShadowGeneration: true);
		}
		checkpointSegmentPositions.Clear();
		num = 500f;
		float num4 = 24f;
		for (int j = 0; j < points.Count - 24; j++)
		{
			float num5 = Vector3.Distance(points[(j + 1) % points.Count].pos, points[j].pos);
			Vector3 right3 = points[j].right;
			Vector3 dir3 = points[j].dir;
			Vector3 up3 = points[j].up;
			bool flag6 = up3.Z < 0.05f;
			bool flag7 = dir3.Z > 0.65f;
			bool flag8 = dir3.Z < -0.65f;
			if (flag6 || flag7 || flag8)
			{
				continue;
			}
			Matrix identity3 = Matrix.Identity;
			((Matrix)(ref identity3)).Right = right3;
			((Matrix)(ref identity3)).Up = dir3;
			((Matrix)(ref identity3)).Forward = -up3;
			Vector3 pos5 = points[(j - 1 < 0) ? (points.Count - 1) : (j - 1)].pos;
			Vector3 pos6 = points[j].pos;
			Vector3 pos7 = points[(j + 1) % points.Count].pos;
			Vector3 pos8 = points[(j + 2) % points.Count].pos;
			if (num - num5 <= 0f && landscape != null)
			{
				Vector3 val2 = Vector3.CatmullRom(pos5, pos6, pos7, pos8, num / num5);
				landscape.AddObjectToRender(RandomHelper.GetRandomInt(6) switch
				{
					4 => "Banner5", 
					3 => "Banner4", 
					2 => "Banner3", 
					1 => "Banner2", 
					0 => "Banner", 
					_ => "Banner6", 
				}, Matrix.CreateScale(points[j].roadWidth) * Matrix.CreateTranslation(new Vector3(0f, 0f, -0.1f)) * identity3 * Matrix.CreateTranslation(val2), isNearTrackForShadowGeneration: true);
				checkpointSegmentPositions.Add(j);
				num += 500f;
			}
			else if (num4 - num5 <= 0f && j >= 25 && landscape != null)
			{
				Vector3 val3 = Vector3.CatmullRom(pos5, pos6, pos7, pos8, num4 / num5);
				Vector3 pos9 = points[(j - 25) % points.Count].pos;
				bool flag9 = points[(j + 60) % points.Count].up.Z < 0.15f;
				Vector3 val4 = Vector3.Normalize(pos9 - points[j].pos);
				float num6 = Vector3Helper.GetAngleBetweenVectors(val4, Vector3.Normalize(-points[j].dir));
				if (Vector3.Distance(points[j].right, val4) < Vector3.Distance(-points[j].right, val4))
				{
					num6 = 0f - num6;
				}
				if (flag9)
				{
					landscape.AddObjectToRender("SignWarning", Matrix.CreateTranslation(new Vector3(points[j].roadWidth * 13.25f * 0.5f - 0.1f, 0f, -0.25f)) * identity3 * Matrix.CreateTranslation(val3), isNearTrackForShadowGeneration: true);
				}
				else if (num6 < (float)Math.PI * -2f / 15f)
				{
					landscape.AddObjectToRender("SignCurveRight", Matrix.CreateRotationZ((float)Math.PI / 2f) * Matrix.CreateTranslation(new Vector3((0f - points[j].roadWidth) * 13.25f * 0.5f - 0.15f, 0f, -0.25f)) * identity3 * Matrix.CreateTranslation(val3), isNearTrackForShadowGeneration: true);
				}
				else if (num6 > (float)Math.PI * 2f / 15f)
				{
					landscape.AddObjectToRender("SignCurveLeft", Matrix.CreateRotationZ(-(float)Math.PI / 2f) * Matrix.CreateTranslation(new Vector3(points[j].roadWidth * 13.25f * 0.5f - 0.15f, 0f, -0.25f)) * identity3 * Matrix.CreateTranslation(val3), isNearTrackForShadowGeneration: true);
				}
				else if (num6 < -(float)Math.PI / 10f || num6 > (float)Math.PI / 10f || RandomHelper.GetRandomInt(9) == 4)
				{
					int randomInt2 = RandomHelper.GetRandomInt(3);
					if (randomInt2 == 0 && Math.Abs(num6) < (float)Math.PI / 24f)
					{
						randomInt2 = RandomHelper.GetRandomInt(3);
					}
					else if (Math.Abs(num6) < (float)Math.PI / 20f && RandomHelper.GetRandomInt(2) == 1)
					{
						num6 *= -1f;
					}
					landscape.AddObjectToRender(randomInt2 switch
					{
						1 => "Sign", 
						0 => (num6 > 0f) ? "SignCurveLeft" : "SignCurveRight", 
						_ => "Sign2", 
					}, Matrix.CreateRotationZ((float)((!(num6 > 0f)) ? 1 : (-1)) * (float)Math.PI / 2f) * Matrix.CreateTranslation(new Vector3((float)((num6 > 0f) ? 1 : (-1)) * points[j].roadWidth * 13.25f * 0.5f - ((randomInt2 == 0) ? 0.15f : 0.005f), 0f, -0.25f)) * identity3 * Matrix.CreateTranslation(val3), isNearTrackForShadowGeneration: true);
				}
				num4 += 24f;
			}
			num -= num5;
			num4 -= num5;
		}
		for (int k = 0; k < points.Count; k += 2)
		{
			if (landscape != null)
			{
				float mapHeight = landscape.GetMapHeight(points[k].pos.X, points[k].pos.Y);
				if (points[k].pos.Z - mapHeight > 60f)
				{
					continue;
				}
			}
			_ = points[k].right;
			Vector3 dir4 = points[k].dir;
			Vector3 up4 = points[k].up;
			bool flag10 = up4.Z < 0.05f;
			bool flag11 = dir4.Z > 0.65f;
			bool flag12 = dir4.Z < -0.65f;
			if (flag10 || flag11 || flag12)
			{
				continue;
			}
			int max = ((!BaseGame.HighDetail) ? 10 : 5);
			if (RandomHelper.GetRandomInt(max) == 0 && landscape != null)
			{
				int randomInt3 = RandomHelper.GetRandomInt(landscape.autoGenerationNames.Length);
				if (randomInt3 >= 6)
				{
					randomInt3 = RandomHelper.GetRandomInt(landscape.autoGenerationNames.Length);
				}
				if (randomInt3 == landscape.autoGenerationNames.Length - 1 && RandomHelper.GetRandomInt(3) < 2)
				{
					randomInt3 = RandomHelper.GetRandomInt(landscape.autoGenerationNames.Length);
				}
				float num7 = RandomHelper.GetRandomFloat(26f, 88f);
				if (randomInt3 == landscape.autoGenerationNames.Length - 1)
				{
					num7 += 20f;
				}
				bool flag13 = RandomHelper.GetRandomInt(2) == 0;
				float randomFloat = RandomHelper.GetRandomFloat(0f, (float)Math.PI * 2f);
				landscape.AddObjectToRender(landscape.autoGenerationNames[randomInt3], randomFloat, points[k].pos, points[k].right, num7 * (float)(flag13 ? 1 : (-1)));
			}
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing)
		{
			roadMaterial.Dispose();
			roadBackMaterial.Dispose();
			roadTunnelMaterial.Dispose();
			roadCementMaterial.Dispose();
			guardRailMaterial.Dispose();
			((GraphicsResource)roadVb).Dispose();
			((GraphicsResource)roadIb).Dispose();
			((GraphicsResource)roadBackVb).Dispose();
			((GraphicsResource)roadBackIb).Dispose();
			((GraphicsResource)roadTunnelVb).Dispose();
			((GraphicsResource)roadTunnelIb).Dispose();
			leftRail.Dispose();
			rightRail.Dispose();
			columns.Dispose();
		}
	}

	public void Render()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Device.VertexDeclaration = TangentVertex.VertexDeclaration;
		BaseGame.WorldMatrix = Matrix.Identity;
		BaseGame.Device.SamplerStates[0].MinFilter = (TextureFilter)3;
		BaseGame.Device.SamplerStates[0].MagFilter = (TextureFilter)3;
		BaseGame.Device.SamplerStates[0].MipFilter = (TextureFilter)2;
		BaseGame.Device.SamplerStates[0].MaxAnisotropy = 8;
		ShaderEffect.normalMapping.Render(roadMaterial, BaseGame.HighDetail ? "SpecularRoad20" : "Specular20", RenderRoadVertices);
		ShaderEffect.normalMapping.Render(roadBackMaterial, BaseGame.HighDetail ? "SpecularRoad20" : "Specular20", RenderRoadBackVertices);
		if (roadTunnelVb != null)
		{
			ShaderEffect.normalMapping.Render(roadTunnelMaterial, "Diffuse20", RenderRoadTunnelVertices);
		}
		leftRail.Render(guardRailMaterial);
		rightRail.Render(guardRailMaterial);
		columns.Render(roadCementMaterial);
	}

	private void RenderRoadVertices()
	{
		BaseGame.Device.Vertices[0].SetSource(roadVb, 0, TangentVertex.SizeInBytes);
		BaseGame.Device.Indices = roadIb;
		BaseGame.Device.DrawIndexedPrimitives((PrimitiveType)4, 0, 0, points.Count * 5, 0, (points.Count - 1) * 8);
	}

	private void RenderRoadBackVertices()
	{
		BaseGame.Device.Vertices[0].SetSource(roadBackVb, 0, TangentVertex.SizeInBytes);
		BaseGame.Device.Indices = roadBackIb;
		BaseGame.Device.DrawIndexedPrimitives((PrimitiveType)4, 0, 0, points.Count * 4, 0, (points.Count - 1) * 6);
	}

	private void RenderRoadTunnelVertices()
	{
		if (roadTunnelVb != null)
		{
			BaseGame.Device.RenderState.CullMode = (CullMode)1;
			BaseGame.Device.Vertices[0].SetSource(roadTunnelVb, 0, TangentVertex.SizeInBytes);
			BaseGame.Device.Indices = roadTunnelIb;
			BaseGame.Device.DrawIndexedPrimitives((PrimitiveType)4, 0, 0, roadTunnelVertices.Length, 0, roadTunnelIndices.Length / 3);
			BaseGame.Device.RenderState.CullMode = (CullMode)3;
		}
	}

	public void GenerateShadow()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		ShaderEffect.shadowMapping.UpdateGenerateShadowWorldMatrix(Matrix.Identity);
		BaseGame.Device.VertexDeclaration = TangentVertex.VertexDeclaration;
		BaseGame.Device.RenderState.CullMode = (CullMode)1;
		RenderRoadVertices();
		RenderRoadTunnelVertices();
		leftRail.GenerateShadow();
		rightRail.GenerateShadow();
	}

	public void UseShadow()
	{
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		ShaderEffect.shadowMapping.UpdateCalcShadowWorldMatrix(Matrix.Identity);
		RenderRoadVertices();
		RenderRoadTunnelVertices();
		leftRail.UseShadow();
		rightRail.UseShadow();
	}

	public Matrix GetTrackPositionMatrix(float trackPositionPercent, out float roadWidth, out float nextRoadWidth)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
		while (trackPositionPercent < 0f)
		{
			trackPositionPercent++;
		}
		while (trackPositionPercent > 1f)
		{
			trackPositionPercent--;
		}
		int num = (int)(trackPositionPercent * (float)points.Count) % points.Count;
		TrackVertex trackVertex = points[(num - 1 < 0) ? (points.Count - 1) : (num - 1)];
		TrackVertex trackVertex2 = points[num];
		TrackVertex trackVertex3 = points[(num + 1) % points.Count];
		TrackVertex trackVertex4 = points[(num + 2) % points.Count];
		float num2 = 1f / (float)points.Count;
		float num3 = (trackPositionPercent - (float)num * num2) / num2;
		Vector3 translation = Vector3.CatmullRom(trackVertex.pos, trackVertex2.pos, trackVertex3.pos, trackVertex4.pos, num3);
		Vector3 forward = Vector3.CatmullRom(trackVertex.dir, trackVertex2.dir, trackVertex3.dir, trackVertex4.dir, num3);
		Vector3 right = Vector3.CatmullRom(trackVertex.right, trackVertex2.right, trackVertex3.right, trackVertex4.right, num3);
		Vector3 up = Vector3.CatmullRom(trackVertex.up, trackVertex2.up, trackVertex3.up, trackVertex4.up, num3);
		Matrix identity = Matrix.Identity;
		((Matrix)(ref identity)).Right = right;
		((Matrix)(ref identity)).Up = up;
		((Matrix)(ref identity)).Forward = forward;
		((Matrix)(ref identity)).Translation = translation;
		roadWidth = MathHelper.Lerp(trackVertex2.roadWidth, trackVertex3.roadWidth, num3) * 13.25f;
		nextRoadWidth = trackVertex4.roadWidth * 13.25f;
		return identity;
	}

	public Matrix GetTrackPositionMatrix(int trackSegmentNum, float trackSegmentPercent, out float roadWidth, out float nextRoadWidth)
	{
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Unknown result type (might be due to invalid IL or missing references)
		if (trackSegmentPercent < 0f)
		{
			trackSegmentPercent = 0f;
		}
		if (trackSegmentPercent > 1f)
		{
			trackSegmentPercent = 1f;
		}
		float num = trackSegmentPercent;
		int num2 = trackSegmentNum % points.Count;
		TrackVertex trackVertex = points[(num2 - 1 < 0) ? (points.Count - 1) : (num2 - 1)];
		TrackVertex trackVertex2 = points[num2];
		TrackVertex trackVertex3 = points[(num2 + 1) % points.Count];
		TrackVertex trackVertex4 = points[(num2 + 2) % points.Count];
		Vector3 translation = Vector3.CatmullRom(trackVertex.pos, trackVertex2.pos, trackVertex3.pos, trackVertex4.pos, num);
		Vector3 forward = Vector3.CatmullRom(trackVertex.dir, trackVertex2.dir, trackVertex3.dir, trackVertex4.dir, num);
		Vector3 right = Vector3.CatmullRom(trackVertex.right, trackVertex2.right, trackVertex3.right, trackVertex4.right, num);
		Vector3 up = Vector3.CatmullRom(trackVertex.up, trackVertex2.up, trackVertex3.up, trackVertex4.up, num);
		Matrix identity = Matrix.Identity;
		((Matrix)(ref identity)).Right = right;
		((Matrix)(ref identity)).Up = up;
		((Matrix)(ref identity)).Forward = forward;
		((Matrix)(ref identity)).Translation = translation;
		roadWidth = MathHelper.Lerp(trackVertex2.roadWidth, trackVertex3.roadWidth, num) * 13.25f;
		nextRoadWidth = MathHelper.Lerp(trackVertex3.roadWidth, trackVertex4.roadWidth, num) * 13.25f;
		return identity;
	}

	public void UpdateCarTrackPosition(Vector3 carPos, ref int trackSegmentNumber, ref float trackSegmentPercent)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		int num = trackSegmentNumber;
		bool flag = false;
		float num2 = 0f;
		float num3 = 1f;
		int num4 = 100;
		do
		{
			TrackVertex trackVertex = points[num];
			TrackVertex trackVertex2 = points[(num + 1) % points.Count];
			num2 = Vector3Helper.SignedDistanceToPlane(carPos, trackVertex.pos, -trackVertex.dir);
			num3 = Vector3Helper.SignedDistanceToPlane(carPos, trackVertex2.pos, trackVertex2.dir);
			if (num2 < 0f)
			{
				num--;
			}
			else if (num3 < 0f)
			{
				num++;
			}
			else
			{
				flag = true;
			}
			if (num < 0)
			{
				num = points.Count - 1;
			}
			if (num >= points.Count)
			{
				num = 0;
			}
			if (num4-- < 0)
			{
				return;
			}
		}
		while (!flag);
		trackSegmentNumber = num;
		if (BaseGame.TotalFrames % 10 == 0)
		{
			disableLensFlareInTunnel = IsTunnel(num);
		}
		float num5 = num2 + num3;
		if (num5 == 0f)
		{
			trackSegmentPercent = 0f;
		}
		else
		{
			trackSegmentPercent = num2 / num5;
		}
	}

	public bool IsTunnel(int trackSegment)
	{
		for (int i = 0; i < helperPositions.Count; i++)
		{
			RoadHelperPosition roadHelperPosition = helperPositions[i];
			if (roadHelperPosition.type == TrackData.RoadHelper.HelperType.Tunnel && trackSegment >= roadHelperPosition.startNum && trackSegment <= roadHelperPosition.endNum)
			{
				return true;
			}
		}
		return false;
	}
}
