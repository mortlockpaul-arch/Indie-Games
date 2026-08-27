using System;
using System.Collections.Generic;
using System.Diagnostics;
using DataContent;
using MaxScriptDefines;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace EGEngine;

public class LevelOutside
{
	private static bool Valid = false;

	private static LevelModel shadowData;

	private static Matrix[] shadowTransforms;

	public static CollisionMesh CollisionDataMesh;

	public static PathingBase PathingData;

	public static Texture2D WaterHeightMap;

	public static Texture2D WaterNormalMap;

	public static TextureCube Cubemap;

	public static int SegmentHitIndex = 0;

	public static int[] CurrentSegmentHitCount = new int[4];

	public static int[] DebugNumberVerticesDraw = new int[4];

	public static OOBBListEntry[] CurrentOOBBHit = new OOBBListEntry[64];

	public static string TimerOutput = "";

	private static Stopwatch TimerStopwatch = Stopwatch.StartNew();

	public static eLevelSegment[] Segments;

	public static eLevelLight[] Lights;

	public static eLevelEmitter[] Emitters;

	private static int TotalNumGeometry = 0;

	private static float[] CurrentDepthArray;

	private static RenderDepthSortStruct[][] RenderArray = new RenderDepthSortStruct[2][];

	public static SphereIntersectList MyShereIntersectList = default(SphereIntersectList);

	private static int NumOcluders = 0;

	private static OcluderStruct[] Ocluders;

	public static Vector4 SunPosition = Vector4.Zero;

	public static Vector4 SunOffset = Vector4.Zero;

	public static Vector4 SunColor = Vector4.Zero;

	public static Vector4 SunShadowColor = Vector4.Zero;

	public static float VideoBrightness = 1f;

	private static int SkyDomeRquestedIndex = 0;

	private static int SkyDomeCurrentIndex = 1;

	private static Texture2D[] SkyDomeMaps = new Texture2D[2];

	private static bool SkyDomesSet = false;

	private static int frameCount = 0;

	private static int NetworkUpdateFrameCount = 0;

	public static float SunAngle = 7.2f;

	private static Vector3 DayColor = new Vector3(2f, 2.31f, 2.35f);

	private static Vector3 DawnDuskColor = new Vector3(2.5f, 1.5f, 0.8f);

	private static Vector3 DayAmbient = new Vector3(0.17f, 0.184f, 0.2f);

	private static Vector3 NightColor = new Vector3(0.031f, 0.031f, 0.041f);

	private static Vector3 NightAmbient = new Vector3(0.01f, 0.01f, 0.0125f);

	public static float DayLightScalar = 1f;

	public static float NightTimeScalar = 0f;

	public static Vector3 CurrentColor = new Vector3(1f, 1f, 1f);

	public static Vector3 CurrentAmbient = new Vector3(0.1f, 0.1f, 0.12f);

	private static ContainmentType tmpUpdateCont;

	private static BoundingBox updateBBox = default(BoundingBox);

	private static BoundingSphere updateBSphere = default(BoundingSphere);

	private static Vector3 tmpUpdateLightDir = Vector3.Zero;

	private static float losDistance;

	private static Vector3 losPlayer = Vector3.Zero;

	private static Vector3 losOther = Vector3.Zero;

	private static Vector3 losDirection = Vector3.Zero;

	private static Ray tmpLOSRay = default(Ray);

	private static BoundingBox tmpLOSBBox = default(BoundingBox);

	private static IntersectSegmentParams tmpLOSSegment = default(IntersectSegmentParams);

	private static Vector2 BathWaterUVScroll = Vector2.Zero;

	private static float InfiniteZPlane = 0.001f;

	private static eMesh drawMesh;

	private static eMeshPart drawMeshPart;

	private static Matrix drawTexProj = Matrix.Identity;

	private static Vector4[] veclights = new Vector4[4];

	private static Vector3[] veclightsColors = new Vector3[4];

	private static Vector4 FinalSunDirection = Vector4.Zero;

	private static Vector4 FinalSunColor = Vector4.Zero;

	private static Vector4 FinalAmbientColor = Vector4.Zero;

	private static GraphicsDevice drawDevice;

	private static Effect drawEffect;

	private static EndGameEngine.MaterialEffectParams drawEffectParams;

	private static Vector4 drawLightInfo = Vector4.Zero;

	private static Vector3 drawLightPos = Vector3.Zero;

	private static Vector3 eyePosition = Vector3.Zero;

	private static Vector2 drawUVScroll = Vector2.Zero;

	private static Matrix playerView = Matrix.Identity;

	private static Matrix playerProj = Matrix.Identity;

	private static Vector3 tmpTrigPos = Vector3.Zero;

	private static BoundingBox tmpIntersectBBox = default(BoundingBox);

	private static CollisionStruct tmpTriggerCollision = default(CollisionStruct);

	private static MaterialType tmpHitMaterial = MaterialType.Concrete;

	private static Vector3 tmpHitPosition = Vector3.Zero;

	private static Vector3 tmpHitNormal = Vector3.Zero;

	private static eTriangleMesh tmpMesh;

	public static Vector3 RaycastSecondHit = Vector3.Zero;

	public static float RaycastHitDistance = 0f;

	private static IntersectSegmentParams tmpSegmentParams = default(IntersectSegmentParams);

	private static Vector3 closestPoint = Vector3.Zero;

	private static Vector3 vecUnitY = Vector3.UnitY;

	private static Vector3 vecUnitNegY = Vector3.UnitY * -1f;

	private static Vector3 TorsoCenter = Vector3.Zero;

	private static Vector3 vecMovement = Vector3.UnitY;

	public static int NumberTrisIntersect = 0;

	private static CollisionStruct tmpSphereCollision = default(CollisionStruct);

	public static int[] SphereIntersectList = new int[2048];

	private static Vector3 closestPointRG = Vector3.Zero;

	private static Vector3 vecP1 = Vector3.Zero;

	private static Vector3 vecP2 = Vector3.Zero;

	private static Vector3 vecP3 = Vector3.Zero;

	private static Vector3 rab = Vector3.Zero;

	private static Vector3 rac = Vector3.Zero;

	private static Vector3 rap = Vector3.Zero;

	private static Vector3 rbp = Vector3.Zero;

	private static Vector3 rcp = Vector3.Zero;

	private static void ProcessSegments(eMesh mesh, List<eLevelSegment> tmpSegments, LevelModel sourceData, Matrix[] tmpTransforms)
	{
		if (mesh.Name != null)
		{
			if (mesh.MeshType == EnumObjectTypes.Segment.ToString())
			{
				eLevelSegment item = default(eLevelSegment);
				item.SegType = EnumObjectTypes.Segment;
				item.AddChildren(sourceData, mesh, tmpTransforms);
				tmpSegments.Add(item);
			}
			else if (mesh.MeshType == EnumObjectTypes.DrawLast.ToString())
			{
				eLevelSegment item2 = default(eLevelSegment);
				item2.SegType = EnumObjectTypes.DrawLast;
				item2.AddChildren(sourceData, mesh, tmpTransforms);
				tmpSegments.Add(item2);
			}
		}
		foreach (eMesh child in mesh.Children)
		{
			ProcessSegments(child, tmpSegments, sourceData, tmpTransforms);
		}
	}

	private static void DebugTextureUse(eMesh mesh)
	{
		foreach (eMeshPart meshPart in mesh.MeshParts)
		{
			for (int i = 0; i < meshPart.Effect.Parameters.Count; i++)
			{
				EffectParameter effectParameter = meshPart.Effect.Parameters[i];
				if (effectParameter.ParameterType == EffectParameterType.Texture || effectParameter.ParameterType == EffectParameterType.Texture2D)
				{
					Texture2D valueTexture2D = effectParameter.GetValueTexture2D();
				}
			}
		}
		foreach (eMesh child in mesh.Children)
		{
			DebugTextureUse(child);
		}
	}

	public static void LoadContent(MyContentManager contMgr)
	{
		LoadContent(contMgr, EndGameEngine.GameSettings.LevelOutsideName);
	}

	public static void LoadContent(MyContentManager contMgr, string levelName)
	{
		if (EndGameEngine.GameSettings.LevelOutsideName == "null")
		{
			return;
		}
		GC.Collect();
		GC.WaitForPendingFinalizers();
		GC.GetTotalMemory(forceFullCollection: false);
		LevelBaseMenu.LoadProgressCounter++;
		LevelData levelData = contMgr.Load<LevelData>("level\\" + levelName);
		LevelModel level = levelData.level;
		CollisionDataMesh = levelData.collision;
		CollisionDataMesh.SetParameters();
		PathingData = levelData.pathing;
		LevelBaseMenu.LoadProgressCounter++;
		foreach (eMesh mesh in levelData.level.Meshes)
		{
			DebugTextureUse(mesh);
		}
		GC.GetTotalMemory(forceFullCollection: false);
		contMgr.OutputMemoryUse();
		GC.Collect();
		LevelBaseMenu.LoadProgressCounter++;
		Lights = new eLevelLight[level.Lights.Count];
		level.Lights.CopyTo(Lights);
		if (EndGameEngine.GameSettings.GameName.Contains("ToyPlane"))
		{
			for (int i = 0; i < Lights.Length; i++)
			{
				Matrix transform = Lights[i].Transform;
				transform.Translation = Vector3.Zero;
				Lights[i].DecayRadius = Vector3.Transform(Vector3.UnitX * 60000f, transform).Length();
			}
		}
		else
		{
			for (int j = 0; j < Lights.Length; j++)
			{
				Matrix transform2 = Lights[j].Transform;
				transform2.Translation = Vector3.Zero;
				Lights[j].DecayRadius = Vector3.Transform(Vector3.UnitX * 100f, Lights[j].Transform).Length();
			}
		}
		if (Emitters == null && level.Emitters != null)
		{
			Emitters = new eLevelEmitter[level.Emitters.Count];
			level.Emitters.CopyTo(Emitters);
		}
		for (int k = 0; k < Lights.Length; k++)
		{
			if (Lights[k].eType == LightTypes.TargetDirectionallight)
			{
				SunPosition.X = Lights[k].Position.X;
				SunPosition.Y = Lights[k].Position.Y;
				SunPosition.Z = Lights[k].Position.Z;
				SunPosition.W = 0f;
				SunOffset = SunPosition;
				SunColor.X = (float)(int)Lights[k].LightColor.R / 255f;
				SunColor.Y = (float)(int)Lights[k].LightColor.G / 255f;
				SunColor.Z = (float)(int)Lights[k].LightColor.B / 255f;
				SunColor.W = (float)(int)Lights[k].LightColor.A / 255f;
				SunColor *= Lights[k].Multiplyer;
				SunShadowColor.X = (float)(int)Lights[k].ShadowColor.R / 255f;
				SunShadowColor.Y = (float)(int)Lights[k].ShadowColor.G / 255f;
				SunShadowColor.Z = (float)(int)Lights[k].ShadowColor.B / 255f;
				SunShadowColor.W = (float)(int)Lights[k].ShadowColor.A / 255f;
				SunShadowColor *= Lights[k].ShadowMultiplyer;
			}
		}
		Matrix[] array = new Matrix[level.Meshes.Count];
		for (int l = 0; l < level.Meshes.Count; l++)
		{
			ref Matrix reference = ref array[l];
			reference = Matrix.Identity;
		}
		List<eLevelSegment> list = new List<eLevelSegment>();
		ProcessSegments(level.Meshes[0], list, level, array);
		Segments = list.ToArray();
		LevelBaseMenu.LoadProgressCounter++;
		Vector4 zero = Vector4.Zero;
		Vector4 zero2 = Vector4.Zero;
		EndGameEngine.MaterialParams.vecDefLightPosition.SetValue(zero);
		EndGameEngine.MaterialParams.vecDefLightColor.SetValue(zero2);
		LevelBaseMenu.LoadProgressCounter++;
		for (int m = 0; m < Segments.Length; m++)
		{
			eLevelSegment eLevelSegment2 = Segments[m];
			if (!eLevelSegment2.Name.Contains("AIObject"))
			{
				continue;
			}
			AIBase.AddAIObjects(m);
			for (int n = 0; n < eLevelSegment2.Geometry.Length; n++)
			{
				if (eLevelSegment2.Geometry[n].Mesh.Name.Contains("AI_Climb"))
				{
					((eTriangleMesh)eLevelSegment2.Geometry[n].Mesh.Tag).Flags |= GeometryFlags.AI_Climb;
				}
				if (eLevelSegment2.Geometry[n].Mesh.Name.Contains("AI_Window"))
				{
					((eTriangleMesh)eLevelSegment2.Geometry[n].Mesh.Tag).Flags |= GeometryFlags.AI_Window;
				}
			}
		}
		for (int num = 0; num < Segments.Length; num++)
		{
			eLevelSegment eLevelSegment3 = Segments[num];
			for (int num2 = 0; num2 < eLevelSegment3.Triggers.Length; num2++)
			{
				for (int num3 = 0; num3 < Segments.Length; num3++)
				{
					if (!(Segments[num3].Name == eLevelSegment3.Triggers[num2].SegmentName))
					{
						continue;
					}
					int num4 = 0;
					for (int num5 = 0; num5 < Segments[num3].Geometry.Length; num5++)
					{
						if (!((eTriangleMesh)Segments[num3].Geometry[num5].Mesh.Tag).oobb.Name.Contains("oobb"))
						{
							num4++;
						}
					}
					eLevelSegment3.Triggers[num2].targets = new TargetPracticeStruct[num4];
					num4 = 0;
					for (int num6 = 0; num6 < Segments[num3].Geometry.Length; num6++)
					{
						if (((eTriangleMesh)Segments[num3].Geometry[num6].Mesh.Tag).oobb.Name.Contains("oobb"))
						{
							continue;
						}
						TargetPracticeStruct targetPracticeStruct = new TargetPracticeStruct
						{
							Name = ((eTriangleMesh)Segments[num3].Geometry[num6].Mesh.Tag).oobb.Name,
							NumberHits = 0,
							TargetAngle = (float)Math.PI / 2f,
							CurrentAngle = 0f,
							TargetOffset = Vector3.UnitY * 8f,
							TargetTimer = 0f,
							model = Segments[num3].Geometry[num6].Mesh,
							transform = Matrix.Invert(((eTriangleMesh)Segments[num3].Geometry[num6].Mesh.Tag).oobb.inversTransform),
							TriMesh = (eTriangleMesh)Segments[num3].Geometry[num6].Mesh.Tag
						};
						int num7 = targetPracticeStruct.Name.IndexOf('_');
						int num8 = targetPracticeStruct.Name.LastIndexOf('_');
						string value = targetPracticeStruct.Name.Substring(num7, num8 - num7);
						for (int num9 = 0; num9 < Segments[num3].Physics.Length; num9++)
						{
							if (Segments[num3].Physics[num9].Name.Contains(value))
							{
								targetPracticeStruct.PhysicsBox = Segments[num3].Physics[num9];
								break;
							}
						}
						eLevelSegment3.Triggers[num2].targets[num4++] = targetPracticeStruct;
					}
					eLevelSegment3.Triggers[num2].SegmentIndex = num3;
					break;
				}
			}
		}
		LevelBaseMenu.LoadProgressCounter++;
		NumOcluders = 0;
		for (int num10 = 0; num10 < level.Meshes.Count; num10++)
		{
			eMesh eMesh2 = level.Meshes[num10];
			if (eMesh2.Name.Contains("OCLUDER"))
			{
				NumOcluders++;
			}
		}
		Ocluders = new OcluderStruct[NumOcluders];
		int num11 = 0;
		for (int num12 = 0; num12 < level.Meshes.Count; num12++)
		{
			eMesh eMesh3 = level.Meshes[num12];
			if (eMesh3.Name.Contains("OCLUDER"))
			{
				Ocluders[num11++].Initialize(eMesh3, level);
			}
		}
		LevelBaseMenu.LoadProgressCounter++;
		for (int num13 = 0; num13 < Segments.Length; num13++)
		{
			for (int num14 = 0; num14 < Segments[num13].Geometry.Length; num14++)
			{
				for (int num15 = 0; num15 < NumOcluders; num15++)
				{
					Ocluders[num15].AddOcclusionReference(Segments[num13].Geometry[num14]);
				}
			}
		}
		TotalNumGeometry = 0;
		for (int num16 = 0; num16 < Segments.Length; num16++)
		{
			TotalNumGeometry += Segments[num16].Geometry.Length;
		}
		TotalNumGeometry++;
		CurrentDepthArray = new float[TotalNumGeometry];
		RenderArray[0] = new RenderDepthSortStruct[TotalNumGeometry];
		RenderArray[1] = new RenderDepthSortStruct[TotalNumGeometry];
		for (int num17 = 0; num17 < TotalNumGeometry; num17++)
		{
			RenderArray[0][num17] = default(RenderDepthSortStruct);
			RenderArray[1][num17] = default(RenderDepthSortStruct);
			RenderArray[0][num17].SegmentIndex = -1;
			RenderArray[1][num17].SegmentIndex = -1;
		}
		LevelBaseMenu.LoadProgressCounter++;
		MyShereIntersectList.SegPhysics = new int[64];
		MyShereIntersectList.SegPhysicsIndex = new int[64];
		MyShereIntersectList.SegGeometry = new int[64];
		MyShereIntersectList.SegGeometryIndex = new int[64];
		shadowData = contMgr.Load<LevelData>("level\\level_shadow").level;
		shadowTransforms = new Matrix[1];
		for (int num18 = 0; num18 < shadowData.Meshes.Count; num18++)
		{
			eMesh eMesh4 = shadowData.Meshes[num18];
			for (int num19 = 0; num19 < eMesh4.Children.Count; num19++)
			{
				eMesh eMesh5 = eMesh4.Children[num19];
				for (int num20 = 0; num20 < eMesh5.MeshParts.Count; num20++)
				{
					eMeshPart eMeshPart2 = eMesh5.MeshParts[num20];
					eMeshPart2.Tag = new EffectParams(eMeshPart2.Effect, eMesh5.Name);
				}
			}
		}
		LevelBaseMenu.LoadProgressCounter++;
		SetEffectLights();
		if (PlayerBase.ApocalypseZ_Hack)
		{
			SkyDomeMaps[0] = contMgr.Load<Texture2D>("textures\\SkyDomeDay");
			SkyDomeMaps[1] = contMgr.Load<Texture2D>("textures\\SkyDomeNight");
		}
		Valid = true;
	}

	public static void Reset()
	{
		SunAngle = 7.2f;
		for (int i = 0; i < Segments.Length; i++)
		{
			for (int j = 0; j < Segments[i].Triggers.Length; j++)
			{
				Segments[i].Triggers[j].ReSpawn();
			}
		}
	}

	public static void SetSkyDome(Vector3 sunPos, Color sunColor, Color ambientColor, int texIndex)
	{
		SunPosition.X = sunPos.X;
		SunPosition.Y = sunPos.Y;
		SunPosition.Z = sunPos.Z;
		SunPosition.W = 0f;
		SunOffset = SunPosition;
		SunColor.X = (float)(int)sunColor.R / 255f;
		SunColor.Y = (float)(int)sunColor.G / 255f;
		SunColor.Z = (float)(int)sunColor.B / 255f;
		SunColor.W = (float)(int)sunColor.A / 255f;
		SunColor *= 2f;
		SunShadowColor.X = (float)(int)ambientColor.R / 255f;
		SunShadowColor.Y = (float)(int)ambientColor.G / 255f;
		SunShadowColor.Z = (float)(int)ambientColor.B / 255f;
		SunShadowColor.W = (float)(int)ambientColor.A / 255f;
		SkyDomeRquestedIndex = texIndex;
		SkyDomeCurrentIndex = -1;
	}

	public static void Update(int qIndex, PlayerBase playerRef, int playerIndex)
	{
		if (EGENetWorkNext.networkSession != null && EGENetWorkNext.networkSession.IsHost)
		{
			NetworkUpdateFrameCount--;
			if (NetworkUpdateFrameCount < 0)
			{
				NetworkUpdateFrameCount = 900;
				PacketWriter packetWriter = EGENetWorkNext.packetWriter;
				packetWriter.Write((byte)134);
				packetWriter.Write(SunAngle);
			}
		}
		drawUVScroll.X += 0.0001f;
		drawUVScroll.Y += 0.00015f;
		SunAngle += 0.00015f * AIBase.TimeOfDayMultiplyer;
		float num = MathHelper.ToDegrees(SunAngle) % 360f;
		num = ((num < 90f) ? num : ((num > 180f) ? 0f : (180f - num)));
		num /= 90f;
		Matrix matrix = Matrix.CreateRotationZ(SunAngle);
		Vector3 vector = Vector3.Transform(Vector3.UnitX, matrix) * 80000f;
		vector.Z += 40000f;
		DayLightScalar = vector.Y / 40000f;
		DayLightScalar = ((DayLightScalar < 0f) ? 0f : DayLightScalar);
		DayLightScalar = ((DayLightScalar > 1f) ? 1f : DayLightScalar);
		Vector3 vector2 = DayColor * DayLightScalar + DawnDuskColor * (1f - DayLightScalar);
		CurrentColor = (vector2 + vector2 * num * 0.25f) * DayLightScalar;
		if (vector.Y < 0f)
		{
			NightTimeScalar = vector.Y * -1f / 40000f;
			NightTimeScalar = ((NightTimeScalar < 0f) ? 0f : NightTimeScalar);
			NightTimeScalar = ((NightTimeScalar > 1f) ? 1f : NightTimeScalar);
			CurrentColor += NightColor * NightTimeScalar;
			if (NightTimeScalar >= 1f)
			{
				vector *= -1f;
			}
			AIBase.TimeOfDayMultiplyer = 0.6f;
		}
		else
		{
			NightTimeScalar = 0f;
			AIBase.TimeOfDayMultiplyer = 0.45f;
		}
		CurrentAmbient = DayAmbient * DayLightScalar + NightAmbient * (1f - DayLightScalar);
		SunOffset.X = vector.X;
		SunOffset.Y = vector.Y;
		SunOffset.Z = vector.Z;
		SunPosition.X = SunOffset.X;
		SunPosition.Y = SunOffset.Y;
		SunPosition.Z = SunOffset.Z;
		for (int i = 0; i < Segments.Length; i++)
		{
			for (int j = 0; j < Segments[i].Triggers.Length; j++)
			{
				Segments[i].Triggers[j].Update(qIndex, 0.01667f);
			}
		}
		DebugNumberVerticesDraw[qIndex] = 0;
		frameCount++;
		int num2 = 0;
		for (int k = 0; k < Segments.Length; k++)
		{
			updateBBox.Min = Segments[k].Min;
			updateBBox.Max = Segments[k].Max;
			tmpUpdateCont = playerRef.bFrustum[qIndex].Contains(updateBBox);
			if (tmpUpdateCont == ContainmentType.Contains || tmpUpdateCont == ContainmentType.Intersects || Segments[k].SegType == EnumObjectTypes.DrawLast)
			{
				Segments[k].Render[qIndex][playerIndex] = true;
				for (int l = 0; l < Segments[k].Geometry.Length; l++)
				{
					updateBBox.Min = ((eTriangleMesh)Segments[k].Geometry[l].Mesh.Tag).oobb.Min;
					updateBBox.Max = ((eTriangleMesh)Segments[k].Geometry[l].Mesh.Tag).oobb.Max;
					tmpUpdateCont = playerRef.bFrustum[qIndex].Contains(updateBBox);
					if (tmpUpdateCont == ContainmentType.Contains || tmpUpdateCont == ContainmentType.Intersects)
					{
						int num3 = 0;
						float num4 = (playerRef.vecPosition - ((eTriangleMesh)Segments[k].Geometry[l].Mesh.Tag).oobb.center).LengthSquared();
						for (int m = 0; m < Segments[k].Geometry[l].Mesh.MeshParts.Count; m++)
						{
							num3 += Segments[k].Geometry[l].Mesh.MeshParts[m].PrimitiveCount;
						}
						Segments[k].Geometry[l].Render[qIndex][playerIndex] = true;
						Segments[k].Geometry[l].RenderLOD[qIndex][playerIndex] = false;
						if (num4 > 4000000f)
						{
							if (Segments[k].Geometry[l].MeshLOD != null)
							{
								num3 = 0;
								for (int n = 0; n < Segments[k].Geometry[l].MeshLOD.MeshParts.Count; n++)
								{
									num3 += Segments[k].Geometry[l].MeshLOD.MeshParts[n].PrimitiveCount;
								}
							}
							Segments[k].Geometry[l].RenderLOD[qIndex][playerIndex] = true;
						}
						CurrentDepthArray[num2] = num4;
						RenderArray[qIndex][num2].SegmentIndex = k;
						RenderArray[qIndex][num2].GeometryIndex = l;
						num2++;
						DebugNumberVerticesDraw[qIndex] += num3;
					}
					else
					{
						Segments[k].Geometry[l].Render[qIndex][playerIndex] = false;
						Segments[k].Geometry[l].RenderLOD[qIndex][playerIndex] = false;
					}
				}
			}
			else
			{
				Segments[k].Render[qIndex][playerIndex] = false;
			}
		}
		if (frameCount > 3)
		{
			frameCount = 0;
		}
		for (int num5 = 1; num5 < num2 - 1; num5++)
		{
			float num6 = CurrentDepthArray[num5];
			int segmentIndex = RenderArray[qIndex][num5].SegmentIndex;
			int geometryIndex = RenderArray[qIndex][num5].GeometryIndex;
			int num7 = num5;
			while (num7 > 0 && CurrentDepthArray[num7 - 1] > num6)
			{
				CurrentDepthArray[num7] = CurrentDepthArray[num7 - 1];
				RenderArray[qIndex][num7].SegmentIndex = RenderArray[qIndex][num7 - 1].SegmentIndex;
				RenderArray[qIndex][num7].GeometryIndex = RenderArray[qIndex][num7 - 1].GeometryIndex;
				num7--;
			}
			CurrentDepthArray[num7] = num6;
			RenderArray[qIndex][num7].SegmentIndex = segmentIndex;
			RenderArray[qIndex][num7].GeometryIndex = geometryIndex;
		}
		RenderArray[qIndex][num2].SegmentIndex = -1;
		for (int num8 = 0; num8 < NumOcluders; num8++)
		{
			Ocluders[num8].CalculateOcluder(playerRef);
			Ocluders[num8].ToggleOcclusionGeometry(qIndex, playerIndex);
		}
		UpdateSetFPSLights(playerIndex, qIndex);
	}

	public static void UpdateSetFPSLights(int playerIndex, int qIndex)
	{
		eyePosition.X = LevelBaseMenu.Players[playerIndex].vecPosition.X;
		eyePosition.Y = LevelBaseMenu.Players[playerIndex].vecPosition.Y + 20f;
		eyePosition.Z = LevelBaseMenu.Players[playerIndex].vecPosition.Z;
		LevelBaseMenu.Players[playerIndex].fpsWeapon.vecFPSLightPosition[qIndex].W = 0f;
	}

	public static bool UpdateLineOfSight(PlayerBase player, PlayerBase other, int qIndex)
	{
		losPlayer = player.vecPosition;
		losPlayer.Y += 80f;
		losOther = other.vecPosition;
		losOther.Y += 80f;
		losDirection = losOther - losPlayer;
		if (Vector3.Dot(losDirection, player.vecDirection) <= 0f)
		{
			return false;
		}
		losDistance = losDirection.Length() - 80f;
		losDirection.Normalize();
		tmpLOSSegment.SegmentStart = losPlayer;
		tmpLOSSegment.SegmentEnd = losPlayer + losDirection * (losDistance + 200f);
		tmpLOSSegment.PreComputeParameters();
		for (int i = 0; i < Segments.Length; i++)
		{
			tmpLOSRay.Position = losPlayer;
			tmpLOSRay.Direction = losDirection;
			tmpLOSBBox.Min = Segments[i].Min;
			tmpLOSBBox.Max = Segments[i].Max;
			if (tmpLOSBBox.Intersects(tmpLOSRay).HasValue)
			{
				for (int j = 0; j < Segments[i].Geometry.Length; j++)
				{
					tmpMesh = (eTriangleMesh)Segments[i].Geometry[j].Mesh.Tag;
					tmpLOSBBox.Min = tmpMesh.oobb.Min;
					tmpLOSBBox.Max = tmpMesh.oobb.Max;
					_ = tmpLOSBBox.Intersects(tmpLOSRay).HasValue;
				}
			}
		}
		return true;
	}

	public static void Draw(RenderPass currentPass, PlayerBase playerRef, int playerIndex, int qIndex)
	{
		if (!Valid)
		{
			return;
		}
		drawDevice = EndGameEngine.GraphicMgr.GraphicsDevice;
		drawEffect = EndGameEngine.MaterialEffect;
		drawEffectParams = EndGameEngine.MaterialParams;
		playerView = playerRef.mDataQueue[qIndex].view;
		playerProj = playerRef.mDataQueue[qIndex].projection;
		switch (currentPass)
		{
		case RenderPass.Normal:
		{
			drawDevice.BlendState = BlendState.Opaque;
			drawDevice.DepthStencilState = DepthStencilState.Default;
			drawTexProj = playerRef.mDataQueue[qIndex].lightView * playerRef.mDataQueue[qIndex].lightProj * LevelBaseMenu.matTextureProj;
			if (LevelBaseMenu.LoadState == LevelLoadState.Loaded)
			{
				for (int k = 0; k < Segments.Length; k++)
				{
					for (int l = 0; l < Segments[k].Triggers.Length; l++)
					{
						if (Segments[k].Triggers[l].targets != null)
						{
							Segments[k].Triggers[l].Draw(ref playerView, ref playerProj, ref drawTexProj);
						}
					}
				}
			}
			drawDevice.BlendState = BlendState.Opaque;
			drawDevice.DepthStencilState = DepthStencilState.Default;
			for (int m = 0; m < Segments.Length; m++)
			{
				for (int n = 0; n < Segments[m].Geometry.Length; n++)
				{
					drawMesh = Segments[m].Geometry[n].Mesh;
					if (Segments[m].Geometry[n].MeshLOD != null && Segments[m].Geometry[n].RenderLOD[qIndex][playerIndex])
					{
						drawMesh = Segments[m].Geometry[n].MeshLOD;
					}
					if ((((eTriangleMesh)drawMesh.Tag).Flags & GeometryFlags.Renderable) <= GeometryFlags.Clear || (!Segments[m].Geometry[n].Render[qIndex][playerIndex] && !Segments[m].Geometry[n].RenderLOD[qIndex][playerIndex]))
					{
						continue;
					}
					for (int num = 0; num < drawMesh.MeshParts.Count; num++)
					{
						drawMeshPart = drawMesh.MeshParts[num];
						if (drawMeshPart.PrimitiveCount <= 0 || drawMeshPart.Opacity == ShaderOpacity.AlphaBlend || drawMeshPart.ShaderTecnique == ShaderEffect.AlphaWater)
						{
							continue;
						}
						if (drawMeshPart.Culling == CullMode.CullCounterClockwiseFace)
						{
							drawDevice.RasterizerState = EndGameEngine.RasterCullCC;
						}
						else if (drawMeshPart.Culling == CullMode.CullClockwiseFace)
						{
							drawDevice.RasterizerState = EndGameEngine.RasterCullCW;
						}
						else
						{
							drawDevice.RasterizerState = EndGameEngine.RasterCullNone;
						}
						if (drawMeshPart.Name.Contains("ALPHABLEND") || drawMesh.Name.Contains("ALPHABLEND"))
						{
							drawEffect = drawMeshPart.Effect;
						}
						drawEffect = drawMeshPart.Effect;
						drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
						drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
						((EffectParams)drawMeshPart.Tag).EnvMap0.SetValue(LevelBaseMenu.EnvMap);
						eyePosition = Vector3.Transform(-playerView.Translation, Matrix.Transpose(playerView));
						((EffectParams)drawMeshPart.Tag).eyePosition.SetValue(eyePosition);
						((EffectParams)drawMeshPart.Tag).matView.SetValue(playerView);
						((EffectParams)drawMeshPart.Tag).matViewProj.SetValue(playerView * playerProj);
						if (drawMeshPart.ShaderTecnique == ShaderEffect.Metal)
						{
							drawEffect.CurrentTechnique.Passes[0].Apply();
							drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
							continue;
						}
						if (drawMeshPart.ShaderTecnique == ShaderEffect.SolidWindow)
						{
							drawEffect.CurrentTechnique.Passes[1].Apply();
							drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
							continue;
						}
						if (drawMesh.MeshType == EnumObjectTypes.SkyDome.ToString())
						{
							((EffectParams)drawMeshPart.Tag).matViewProj.SetValue(playerView * PlayerBase.matSkyDomeProjection[qIndex]);
							drawEffect.Parameters["FarZPlane"].SetValue(PlayerBase.FarZPlane);
							drawEffect.CurrentTechnique.Passes[11].Apply();
							drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
							continue;
						}
						if (PlayerBase.ToyPlane_Hack)
						{
							Matrix identity = Matrix.Identity;
							if ((drawMeshPart.UserFlags & 2) != 0 || (drawMeshPart.UserFlags & 1) != 0)
							{
								identity.Translation = drawMesh.BoundSphere.Center;
								drawEffect.Parameters["matWorld"].SetValue(identity);
								drawEffect.CurrentTechnique.Passes[4].Apply();
							}
							else
							{
								drawEffect.CurrentTechnique.Passes[0].Apply();
							}
						}
						else
						{
							drawEffect.CurrentTechnique.Passes[0].Apply();
						}
						drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
					}
				}
			}
			break;
		}
		case RenderPass.AlphaBlend:
		{
			drawDevice.BlendState = EndGameEngine.BlendAlphaNoWriteAlpha;
			drawDevice.DepthStencilState = EndGameEngine.DepthEnabled;
			drawDevice.RasterizerState = EndGameEngine.RasterCullCC;
			Vector4 uVDisplacement2 = playerRef.UVDisplacement;
			uVDisplacement2.X += drawUVScroll.X;
			uVDisplacement2.Y += drawUVScroll.Y;
			for (int num10 = 0; num10 < Segments.Length; num10++)
			{
				for (int num11 = 0; num11 < Segments[num10].Geometry.Length; num11++)
				{
					drawMesh = Segments[num10].Geometry[num11].Mesh;
					if ((((eTriangleMesh)drawMesh.Tag).Flags & GeometryFlags.Renderable) <= GeometryFlags.Clear)
					{
						continue;
					}
					for (int num12 = 0; num12 < drawMesh.MeshParts.Count; num12++)
					{
						drawMeshPart = drawMesh.MeshParts[num12];
						if (drawMeshPart.Culling == CullMode.CullCounterClockwiseFace)
						{
							drawDevice.RasterizerState = EndGameEngine.RasterCullCC;
						}
						else if (drawMeshPart.Culling == CullMode.CullClockwiseFace)
						{
							drawDevice.RasterizerState = EndGameEngine.RasterCullCW;
						}
						else
						{
							drawDevice.RasterizerState = EndGameEngine.RasterCullNone;
						}
						if ((drawMeshPart.Opacity != ShaderOpacity.AlphaBlend && drawMeshPart.ShaderTecnique != ShaderEffect.AlphaWater) || drawMesh.Name.Contains("buy"))
						{
							continue;
						}
						drawEffect = drawMeshPart.Effect;
						drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
						drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
						((EffectParams)drawMeshPart.Tag).matView.SetValue(playerView);
						eyePosition = Vector3.Transform(-playerView.Translation, Matrix.Transpose(playerView));
						((EffectParams)drawMeshPart.Tag).eyePosition.SetValue(eyePosition);
						FinalSunColor = SunColor * VideoBrightness;
						FinalAmbientColor = SunShadowColor * VideoBrightness;
						((EffectParams)drawMeshPart.Tag).vecLightColor.SetValue(FinalSunColor);
						((EffectParams)drawMeshPart.Tag).vecAmbientLightColor.SetValue(FinalAmbientColor);
						((EffectParams)drawMeshPart.Tag).vecSunPosition.SetValue(SunPosition);
						((EffectParams)drawMeshPart.Tag).matViewProj.SetValue(playerView * playerProj);
						((EffectParams)drawMeshPart.Tag).matTexProj.SetValue(drawTexProj);
						((EffectParams)drawMeshPart.Tag).EnvMap0.SetValue(LevelBaseMenu.EnvMap);
						((EffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
						((EffectParams)drawMeshPart.Tag).uvDisplacement.SetValue(uVDisplacement2);
						if (drawMesh.Name.Contains("WaterDcl_ALPHAWATER_NOWALK_01"))
						{
							drawEffect.CurrentTechnique.Passes[1].Apply();
						}
						else
						{
							if (drawMesh.Name.Contains("BathWater"))
							{
								continue;
							}
							drawEffect.CurrentTechnique.Passes[0].Apply();
						}
						drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
					}
				}
			}
			drawDevice.BlendState = BlendState.Opaque;
			break;
		}
		case RenderPass.PostPass:
		{
			bool flag2 = false;
			drawDevice.BlendState = BlendState.Opaque;
			drawDevice.DepthStencilState = EndGameEngine.DepthDisabled;
			drawDevice.RasterizerState = EndGameEngine.RasterCullCC;
			Vector4 uVDisplacement = playerRef.UVDisplacement;
			uVDisplacement.X += drawUVScroll.X;
			uVDisplacement.Y += drawUVScroll.Y;
			for (int num4 = 0; num4 < Segments.Length; num4++)
			{
				for (int num5 = 0; num5 < Segments[num4].Geometry.Length; num5++)
				{
					drawMesh = Segments[num4].Geometry[num5].Mesh;
					if ((((eTriangleMesh)drawMesh.Tag).Flags & GeometryFlags.Renderable) <= GeometryFlags.Clear)
					{
						continue;
					}
					for (int num6 = 0; num6 < drawMesh.MeshParts.Count; num6++)
					{
						drawMeshPart = drawMesh.MeshParts[num6];
						if (drawMeshPart.Culling == CullMode.CullCounterClockwiseFace)
						{
							drawDevice.RasterizerState = EndGameEngine.RasterCullCC;
						}
						else if (drawMeshPart.Culling == CullMode.CullClockwiseFace)
						{
							drawDevice.RasterizerState = EndGameEngine.RasterCullCW;
						}
						else
						{
							drawDevice.RasterizerState = EndGameEngine.RasterCullNone;
						}
						if ((drawMeshPart.Opacity != ShaderOpacity.AlphaBlend && drawMeshPart.ShaderTecnique != ShaderEffect.AlphaWater) || drawMesh.Name.Contains("buy"))
						{
							continue;
						}
						drawEffect = drawMeshPart.Effect;
						drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
						drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
						((EffectParams)drawMeshPart.Tag).matView.SetValue(playerView);
						eyePosition = Vector3.Transform(-playerView.Translation, Matrix.Transpose(playerView));
						((EffectParams)drawMeshPart.Tag).eyePosition.SetValue(eyePosition);
						FinalSunColor = SunColor * VideoBrightness;
						FinalAmbientColor = SunShadowColor * VideoBrightness;
						((EffectParams)drawMeshPart.Tag).vecLightColor.SetValue(FinalSunColor);
						((EffectParams)drawMeshPart.Tag).vecAmbientLightColor.SetValue(FinalAmbientColor);
						((EffectParams)drawMeshPart.Tag).vecSunPosition.SetValue(SunPosition);
						((EffectParams)drawMeshPart.Tag).matViewProj.SetValue(playerView * playerProj);
						((EffectParams)drawMeshPart.Tag).matTexProj.SetValue(drawTexProj);
						((EffectParams)drawMeshPart.Tag).EnvMap0.SetValue(LevelBaseMenu.EnvMap);
						((EffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
						((EffectParams)drawMeshPart.Tag).uvDisplacement.SetValue(uVDisplacement);
						if (drawMesh.Name.Contains("BathWater"))
						{
							if (!flag2)
							{
								flag2 = true;
								BathWaterUVScroll.X += 0.001f;
								BathWaterUVScroll.Y += 0.0015f;
								drawEffect.GraphicsDevice.VertexSamplerStates[0] = SamplerState.PointWrap;
								drawEffect.GraphicsDevice.VertexTextures[0] = WaterHeightMap;
							}
							uVDisplacement.X = BathWaterUVScroll.X;
							uVDisplacement.Y = BathWaterUVScroll.Y;
							uVDisplacement.Z = 0f;
							uVDisplacement.W = 0f;
							((EffectParams)drawMeshPart.Tag).uvDisplacement.SetValue(uVDisplacement);
							drawEffect.Parameters["NormalmapTexture"].SetValue(WaterNormalMap);
							drawEffect.Parameters["DiffuseDefferedTexture"].SetValue(LevelBaseMenu.TemparyCompositeRenderTarget);
							drawEffect.Parameters["DepthTexture"].SetValue(LevelBaseMenu.DepthRenderTarget);
							if (drawMesh.Name.Contains("Top"))
							{
								drawEffect.CurrentTechnique.Passes[4].Apply();
							}
							else
							{
								drawEffect.CurrentTechnique.Passes[5].Apply();
							}
							drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
						}
					}
				}
			}
			drawDevice.BlendState = BlendState.Opaque;
			break;
		}
		case RenderPass.SkyDome:
		{
			drawDevice.BlendState = BlendState.Opaque;
			drawDevice.RasterizerState = EndGameEngine.RasterCullNone;
			for (int num7 = 0; num7 < Segments.Length; num7++)
			{
				for (int num8 = 0; num8 < Segments[num7].Geometry.Length; num8++)
				{
					drawMesh = Segments[num7].Geometry[num8].Mesh;
					if (drawMesh.MeshType != EnumObjectTypes.SkyDome.ToString() || (((eTriangleMesh)drawMesh.Tag).Flags & GeometryFlags.Renderable) <= GeometryFlags.Clear)
					{
						continue;
					}
					for (int num9 = 0; num9 < drawMesh.MeshParts.Count; num9++)
					{
						drawMeshPart = drawMesh.MeshParts[num9];
						drawEffect = drawMeshPart.Effect;
						drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
						drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
						((EffectParams)drawMeshPart.Tag).matView.SetValue(playerView);
						eyePosition = Vector3.Transform(-playerView.Translation, Matrix.Transpose(playerView));
						((EffectParams)drawMeshPart.Tag).eyePosition.SetValue(eyePosition);
						((EffectParams)drawMeshPart.Tag).matViewProj.SetValue(playerView * playerProj);
						if (!SkyDomesSet && PlayerBase.ApocalypseZ_Hack)
						{
							SkyDomesSet = true;
							drawMeshPart.Effect.Parameters["DiffuseTexture"].SetValue(SkyDomeMaps[1]);
							drawMeshPart.Effect.Parameters["DiffuseTexture2"].SetValue(SkyDomeMaps[0]);
						}
						drawMeshPart.Effect.Parameters["FarZPlane"].SetValue(PlayerBase.FarZPlane * 0.95f);
						drawMeshPart.Effect.Parameters["fogStart"].SetValue(PlayerBase.FogStart);
						drawMeshPart.Effect.Parameters["fogEnd"].SetValue(PlayerBase.FogEnd);
						drawMeshPart.Effect.Parameters["fogColor"].SetValue(LevelBaseMenu.FogColor * (0.1f + DayLightScalar));
						drawMeshPart.Effect.Parameters["DayLightScalar"].SetValue(NightTimeScalar);
						Vector3 zero = Vector3.Zero;
						zero.X = SunPosition.X;
						zero.Y = SunPosition.Y;
						zero.Z = SunPosition.Z;
						drawMeshPart.Effect.Parameters["vecLightDirection"].SetValue(Vector3.Normalize(zero));
						drawDevice.DepthStencilState = EndGameEngine.DepthEnabled;
						drawEffect.CurrentTechnique.Passes[11].Apply();
						drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
					}
				}
			}
			drawDevice.BlendState = BlendState.Opaque;
			break;
		}
		case RenderPass.ForwardRender:
		{
			drawDevice.BlendState = BlendState.Opaque;
			drawDevice.DepthStencilState = EndGameEngine.DepthEnabled;
			bool flag = true;
			int num2 = 0;
			while (flag && num2 < TotalNumGeometry)
			{
				if (RenderArray[qIndex][num2].SegmentIndex < 0)
				{
					flag = false;
				}
				else
				{
					int segmentIndex = RenderArray[qIndex][num2].SegmentIndex;
					int geometryIndex = RenderArray[qIndex][num2].GeometryIndex;
					drawMesh = Segments[segmentIndex].Geometry[geometryIndex].Mesh;
					if (!(drawMesh.MeshType == EnumObjectTypes.SkyDome.ToString()))
					{
						if (Segments[segmentIndex].Geometry[geometryIndex].MeshLOD != null && Segments[segmentIndex].Geometry[geometryIndex].RenderLOD[qIndex][playerIndex])
						{
							drawMesh = Segments[segmentIndex].Geometry[geometryIndex].MeshLOD;
						}
						if ((((eTriangleMesh)drawMesh.Tag).Flags & GeometryFlags.Renderable) > GeometryFlags.Clear && (Segments[segmentIndex].Geometry[geometryIndex].Render[qIndex][playerIndex] || Segments[segmentIndex].Geometry[geometryIndex].RenderLOD[qIndex][playerIndex]))
						{
							for (int num3 = 0; num3 < drawMesh.MeshParts.Count; num3++)
							{
								drawMeshPart = drawMesh.MeshParts[num3];
								if (drawMeshPart.PrimitiveCount > 0 && drawMeshPart.Opacity != ShaderOpacity.AlphaBlend && drawMeshPart.ShaderTecnique != ShaderEffect.AlphaWater)
								{
									if (drawMeshPart.Culling == CullMode.CullCounterClockwiseFace)
									{
										drawDevice.RasterizerState = EndGameEngine.RasterCullCC;
									}
									else if (drawMeshPart.Culling == CullMode.CullClockwiseFace)
									{
										drawDevice.RasterizerState = EndGameEngine.RasterCullCW;
									}
									else
									{
										drawDevice.RasterizerState = EndGameEngine.RasterCullNone;
									}
									drawEffect = drawMeshPart.Effect;
									drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
									drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
									eyePosition = Vector3.Transform(-playerView.Translation, Matrix.Transpose(playerView));
									((EffectParams)drawMeshPart.Tag).eyePosition.SetValue(eyePosition);
									((EffectParams)drawMeshPart.Tag).EnvMap0.SetValue(LevelBaseMenu.EnvMap);
									((EffectParams)drawMeshPart.Tag).matViewProj.SetValue(playerView * playerProj);
									drawEffect.CurrentTechnique.Passes[1].Apply();
									drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
								}
							}
						}
					}
				}
				num2++;
			}
			break;
		}
		case RenderPass.Depth:
		{
			drawDevice.BlendState = BlendState.Opaque;
			drawDevice.DepthStencilState = EndGameEngine.DepthRender;
			drawDevice.RasterizerState = EndGameEngine.RasterCullCC;
			for (int i = 0; i < shadowData.Meshes.Count; i++)
			{
				drawMesh = shadowData.Meshes[i];
				for (int j = 0; j < drawMesh.MeshParts.Count; j++)
				{
					drawMeshPart = drawMesh.MeshParts[j];
					drawEffect = drawMeshPart.Effect;
					drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
					drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
					((EffectParams)drawMeshPart.Tag).matViewProj.SetValue(playerView * playerProj);
					if (drawMesh.Name.Contains("ALPHATEST"))
					{
						drawEffect.CurrentTechnique.Passes[1].Apply();
					}
					else
					{
						drawEffect.CurrentTechnique.Passes[1].Apply();
					}
					drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
				}
			}
			break;
		}
		}
		switch (currentPass)
		{
		case RenderPass.ZBuffer:
		{
			drawDevice.BlendState = EndGameEngine.BlendOpaqueNoColorChannel;
			drawDevice.DepthStencilState = DepthStencilState.Default;
			drawTexProj = playerRef.mDataQueue[qIndex].lightView * playerRef.mDataQueue[qIndex].lightProj * LevelBaseMenu.matTextureProj;
			bool flag3 = true;
			int num19 = 0;
			while (flag3 && num19 < TotalNumGeometry)
			{
				if (RenderArray[qIndex][num19].SegmentIndex < 0)
				{
					flag3 = false;
				}
				else
				{
					int segmentIndex2 = RenderArray[qIndex][num19].SegmentIndex;
					int geometryIndex2 = RenderArray[qIndex][num19].GeometryIndex;
					drawMesh = Segments[segmentIndex2].Geometry[geometryIndex2].Mesh;
					if (Segments[segmentIndex2].Geometry[geometryIndex2].MeshLOD != null && Segments[segmentIndex2].Geometry[geometryIndex2].RenderLOD[qIndex][playerIndex])
					{
						drawMesh = Segments[segmentIndex2].Geometry[geometryIndex2].MeshLOD;
					}
					if (!(drawMesh.MeshType == EnumObjectTypes.SkyDome.ToString()) && (((eTriangleMesh)drawMesh.Tag).Flags & GeometryFlags.Renderable) > GeometryFlags.Clear && (Segments[segmentIndex2].Geometry[geometryIndex2].Render[qIndex][playerIndex] || Segments[segmentIndex2].Geometry[geometryIndex2].RenderLOD[qIndex][playerIndex]))
					{
						for (int num20 = 0; num20 < drawMesh.MeshParts.Count; num20++)
						{
							drawMeshPart = drawMesh.MeshParts[num20];
							if (drawMeshPart.PrimitiveCount > 0 && drawMeshPart.Opacity != ShaderOpacity.AlphaBlend && drawMeshPart.ShaderTecnique != ShaderEffect.AlphaWater)
							{
								if (drawMeshPart.Culling == CullMode.CullCounterClockwiseFace)
								{
									drawDevice.RasterizerState = EndGameEngine.RasterCullCC;
								}
								else if (drawMeshPart.Culling == CullMode.CullClockwiseFace)
								{
									drawDevice.RasterizerState = EndGameEngine.RasterCullCW;
								}
								else
								{
									drawDevice.RasterizerState = EndGameEngine.RasterCullNone;
								}
								drawEffect = drawMeshPart.Effect;
								drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
								drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
								eyePosition = Vector3.Transform(-playerView.Translation, Matrix.Transpose(playerView));
								((EffectParams)drawMeshPart.Tag).eyePosition.SetValue(eyePosition);
								((EffectParams)drawMeshPart.Tag).matView.SetValue(playerView);
								((EffectParams)drawMeshPart.Tag).matViewProj.SetValue(playerView * playerProj);
								if (drawMeshPart.ShaderTecnique != ShaderEffect.Metal)
								{
									_ = drawMeshPart.ShaderTecnique;
									_ = 5;
								}
								if (drawMesh.MeshType == EnumObjectTypes.SkyDome.ToString())
								{
									drawEffect.CurrentTechnique.Passes[11].Apply();
								}
								else
								{
									drawEffect.CurrentTechnique.Passes[2].Apply();
								}
								drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
							}
						}
					}
				}
				num19++;
			}
			break;
		}
		case RenderPass.Shadow:
		{
			drawDevice.BlendState = BlendState.Opaque;
			drawDevice.DepthStencilState = DepthStencilState.Default;
			drawDevice.RasterizerState = EndGameEngine.RasterCullNone;
			playerView = playerRef.mDataQueue[qIndex].lightView;
			playerProj = playerRef.mDataQueue[qIndex].lightProj;
			for (int num16 = 0; num16 < shadowData.Meshes.Count; num16++)
			{
				drawMesh = shadowData.Meshes[num16];
				for (int num17 = 0; num17 < drawMesh.Children.Count; num17++)
				{
					eMesh eMesh2 = drawMesh.Children[num17];
					for (int num18 = 0; num18 < eMesh2.MeshParts.Count; num18++)
					{
						drawMeshPart = eMesh2.MeshParts[num18];
						drawEffect = drawMeshPart.Effect;
						drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
						drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
						((EffectParams)drawMeshPart.Tag).matLightViewProj.SetValue(playerView * playerProj);
						if (drawMesh.Name.Contains("ALPHATEST"))
						{
							drawEffect.CurrentTechnique.Passes[3].Apply();
						}
						else
						{
							drawEffect.CurrentTechnique.Passes[3].Apply();
						}
						drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
					}
				}
			}
			break;
		}
		case RenderPass.SpecialWeaponBuy:
		{
			drawDevice.BlendState = EndGameEngine.BlendAlphaNoWriteAlpha;
			drawDevice.DepthStencilState = DepthStencilState.Default;
			drawDevice.RasterizerState = EndGameEngine.RasterCullCC;
			Vector4 uVDisplacement3 = playerRef.UVDisplacement;
			uVDisplacement3.X += drawUVScroll.X;
			uVDisplacement3.Y += drawUVScroll.Y;
			for (int num13 = 0; num13 < Segments.Length; num13++)
			{
				for (int num14 = 0; num14 < Segments[num13].Geometry.Length; num14++)
				{
					drawMesh = Segments[num13].Geometry[num14].Mesh;
					if ((((eTriangleMesh)drawMesh.Tag).Flags & GeometryFlags.Renderable) <= GeometryFlags.Clear)
					{
						continue;
					}
					for (int num15 = 0; num15 < drawMesh.MeshParts.Count; num15++)
					{
						drawMeshPart = drawMesh.MeshParts[num15];
						if ((drawMeshPart.Opacity == ShaderOpacity.AlphaBlend || drawMeshPart.ShaderTecnique == ShaderEffect.AlphaWater) && drawMesh.Name.Contains("buy"))
						{
							if (drawMeshPart.Culling == CullMode.CullCounterClockwiseFace)
							{
								drawDevice.RasterizerState = EndGameEngine.RasterCullCC;
							}
							else if (drawMeshPart.Culling == CullMode.CullClockwiseFace)
							{
								drawDevice.RasterizerState = EndGameEngine.RasterCullCW;
							}
							else
							{
								drawDevice.RasterizerState = EndGameEngine.RasterCullNone;
							}
							drawEffect = drawMeshPart.Effect;
							drawEffect.GraphicsDevice.SetVertexBuffer(drawMeshPart.VertexBuffer, drawMeshPart.VertexOffset);
							drawEffect.GraphicsDevice.Indices = drawMeshPart.IndexBuffer;
							((EffectParams)drawMeshPart.Tag).matView.SetValue(playerView);
							eyePosition = Vector3.Transform(-playerView.Translation, Matrix.Transpose(playerView));
							((EffectParams)drawMeshPart.Tag).eyePosition.SetValue(eyePosition);
							((EffectParams)drawMeshPart.Tag).vecSunPosition.SetValue(SunPosition);
							((EffectParams)drawMeshPart.Tag).vecLightColor.SetValue(SunColor);
							((EffectParams)drawMeshPart.Tag).vecAmbientLightColor.SetValue(SunShadowColor);
							((EffectParams)drawMeshPart.Tag).matViewProj.SetValue(playerView * playerProj);
							((EffectParams)drawMeshPart.Tag).matTexProj.SetValue(drawTexProj);
							((EffectParams)drawMeshPart.Tag).EnvMap0.SetValue(LevelBaseMenu.EnvMap);
							((EffectParams)drawMeshPart.Tag).TextureShadowMap.SetValue(LevelBaseMenu.shadowRenderTarget);
							((EffectParams)drawMeshPart.Tag).uvDisplacement.SetValue(uVDisplacement3);
							drawEffect.CurrentTechnique.Passes[2].Apply();
							drawEffect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, drawMeshPart.NumVertices, drawMeshPart.StartIndex, drawMeshPart.PrimitiveCount);
						}
					}
				}
			}
			drawDevice.BlendState = BlendState.Opaque;
			break;
		}
		}
	}

	public static List<eMesh> GetNumberAmmunitionCrate()
	{
		List<eMesh> list = new List<eMesh>();
		if (Valid)
		{
			for (int i = 0; i < Segments.Length; i++)
			{
				for (int j = 0; j < Segments[i].Geometry.Length; j++)
				{
					eMesh mesh = Segments[i].Geometry[j].Mesh;
					if (mesh.Name.Contains("ammocrate"))
					{
						list.Add(mesh);
					}
				}
			}
		}
		return list;
	}

	public static bool IntersectPhysicsSphere(ref BoundingSphere e, ref CollisionStruct c)
	{
		CurrentSegmentHitCount[SegmentHitIndex] = 0;
		bool result = false;
		c.applyResponse = true;
		tmpTriggerCollision.hitTrigger = TriggerTypes.Undeclared;
		tmpTriggerCollision.applyResponse = false;
		for (int i = 0; i < Segments.Length; i++)
		{
			tmpIntersectBBox.Min = Segments[i].Min;
			tmpIntersectBBox.Max = Segments[i].Max;
			if (!tmpIntersectBBox.Intersects(e))
			{
				continue;
			}
			CurrentSegmentHitCount[SegmentHitIndex]++;
			if (c.hitTrigger != TriggerTypes.Ladder)
			{
				for (int j = 0; j < Segments[i].Physics.Length; j++)
				{
					if (Segments[i].Physics[j].CollisionSphere(ref e, ref c))
					{
						result = true;
					}
				}
				for (int k = 0; k < Segments[i].Triggers.Length; k++)
				{
					if (Segments[i].Triggers[k].targets != null && Segments[i].Triggers[k].CollisionSphere(ref e, ref c))
					{
						result = true;
					}
				}
			}
			for (int l = 0; l < Segments[i].Triggers.Length; l++)
			{
				if (Segments[i].Triggers[l].oobb.ContainsPoint(ref e.Center))
				{
					tmpTriggerCollision.hitTrigger = Segments[i].Triggers[l].eType;
					tmpTriggerCollision.hitNormal = Segments[i].Triggers[l].direction;
					tmpTriggerCollision.hitPosition = Segments[i].Triggers[l].center;
				}
			}
		}
		c.hitTrigger = tmpTriggerCollision.hitTrigger;
		if (tmpTriggerCollision.hitTrigger != TriggerTypes.Undeclared)
		{
			c.hitNormal = tmpTriggerCollision.hitNormal;
			c.hitPosition = tmpTriggerCollision.hitPosition;
		}
		SegmentHitIndex++;
		if (SegmentHitIndex > 3)
		{
			SegmentHitIndex = 0;
		}
		return result;
	}

	public static MaterialType RayCast(int qIndex, ref Vector3 origin, ref Vector3 direction, ref Vector3 hitPos, ref Vector3 hitNorm)
	{
		tmpSegmentParams.OnlyWalkable = false;
		tmpSegmentParams.SegmentStart = origin;
		tmpSegmentParams.SegmentEnd = origin + direction * 12000f;
		tmpSegmentParams.PreComputeParameters();
		MaterialType result = RayCast(qIndex, ref tmpSegmentParams, spawnSparks: false);
		hitNorm = tmpSegmentParams.hitNormal;
		hitPos = tmpSegmentParams.hitPosition;
		return result;
	}

	public static MaterialType RayCast(int qIndex, ref IntersectSegmentParams segParams, bool spawnSparks)
	{
		tmpHitMaterial = MaterialType.Undefined;
		float minDistance = 100000f;
		float num = minDistance * minDistance;
		RaycastHitDistance = 1000000f;
		uint tmpHitFlag = 0u;
		segParams.hitNormal = Vector3.UnitY;
		segParams.TargetIndex = -1;
		int num2 = (int)Math.Floor((segParams.SegmentStart.X - CollisionDataMesh.GridMin.X) / (float)CollisionDataMesh.GridCellSize);
		int num3 = (int)Math.Floor((segParams.SegmentStart.Z - CollisionDataMesh.GridMin.Z) / (float)CollisionDataMesh.GridCellSize);
		int num4 = (int)Math.Floor((segParams.SegmentEnd.X - CollisionDataMesh.GridMin.X) / (float)CollisionDataMesh.GridCellSize);
		int num5 = (int)Math.Floor((segParams.SegmentEnd.Z - CollisionDataMesh.GridMin.Z) / (float)CollisionDataMesh.GridCellSize);
		int num6 = ((segParams.SegmentStart.X < segParams.SegmentEnd.X) ? 1 : ((segParams.SegmentStart.X > segParams.SegmentEnd.X) ? (-1) : 0));
		int num7 = ((segParams.SegmentStart.Z < segParams.SegmentEnd.Z) ? 1 : ((segParams.SegmentStart.Z > segParams.SegmentEnd.Z) ? (-1) : 0));
		float num8 = segParams.SegmentStart.X - CollisionDataMesh.GridMin.X;
		float num9 = (float)CollisionDataMesh.GridCellSize * (float)Math.Floor(num8 / (float)CollisionDataMesh.GridCellSize);
		float num10 = num9 + (float)CollisionDataMesh.GridCellSize;
		float num11 = ((segParams.SegmentStart.X > segParams.SegmentEnd.X) ? (num8 - num9) : (num10 - num8)) / Math.Abs(segParams.SegmentEnd.X - segParams.SegmentStart.X);
		float num12 = segParams.SegmentStart.Z - CollisionDataMesh.GridMin.Z;
		float num13 = (float)CollisionDataMesh.GridCellSize * (float)Math.Floor(num12 / (float)CollisionDataMesh.GridCellSize);
		float num14 = num13 + (float)CollisionDataMesh.GridCellSize;
		float num15 = ((segParams.SegmentStart.Z > segParams.SegmentEnd.Z) ? (num12 - num13) : (num14 - num12)) / Math.Abs(segParams.SegmentEnd.Z - segParams.SegmentStart.Z);
		float num16 = (float)CollisionDataMesh.GridCellSize / Math.Abs(segParams.SegmentEnd.X - segParams.SegmentStart.X);
		float num17 = (float)CollisionDataMesh.GridCellSize / Math.Abs(segParams.SegmentEnd.Z - segParams.SegmentStart.Z);
		while (true)
		{
			if (num2 >= 0 && num2 < CollisionDataMesh.NumberGridX && num3 >= 0 && num3 < CollisionDataMesh.NumberGridZ)
			{
				TestSegmentGrid(qIndex, ref segParams, ref minDistance, ref tmpHitFlag, num2, num3, spawnSparks);
			}
			if (num11 <= num15)
			{
				if (num2 == num4)
				{
					break;
				}
				num11 += num16;
				num2 += num6;
			}
			else
			{
				if (num3 == num5)
				{
					break;
				}
				num15 += num17;
				num3 += num7;
			}
		}
		int num18 = -1;
		int num19 = -1;
		int num20 = -1;
		for (int i = 0; i < Segments.Length; i++)
		{
			if (!Segments[i].TestSegment(ref segParams))
			{
				continue;
			}
			for (int j = 0; j < Segments[i].Triggers.Length; j++)
			{
				if (Segments[i].Triggers[j].targets != null)
				{
					float num21 = Segments[i].Triggers[j].RayCast(ref segParams, sqrRootResult: false);
					if (num21 < num)
					{
						RaycastSecondHit = segParams.hitPosition;
						num = num21;
						tmpHitFlag = 131072u;
						tmpHitNormal = segParams.hitNormal;
						tmpHitPosition = segParams.hitPosition;
						num18 = i;
						num19 = j;
						num20 = segParams.TargetIndex;
					}
				}
			}
		}
		if (minDistance < 100000f)
		{
			if ((tmpHitFlag | 0x200000) != 0)
			{
				tmpHitMaterial = MaterialType.Concrete;
			}
			else if ((tmpHitFlag | 0x100000) != 0)
			{
				tmpHitMaterial = MaterialType.Glass;
			}
			else if ((tmpHitFlag | 0x10000) != 0)
			{
				tmpHitMaterial = MaterialType.Metal;
			}
			else if ((tmpHitFlag | 0x40000) != 0)
			{
				tmpHitMaterial = MaterialType.Rock;
			}
			else if ((tmpHitFlag | 0x20000) != 0)
			{
				tmpHitMaterial = MaterialType.Wood;
			}
			segParams.hitNormal = tmpHitNormal;
			segParams.hitPosition = tmpHitPosition;
			RaycastHitDistance = minDistance;
			segParams.hitDistance = RaycastHitDistance;
			if (num20 >= 0)
			{
				Segments[num18].Triggers[num19].ApplyHitOnTarget(num20);
			}
		}
		return tmpHitMaterial;
	}

	private static void TestSegmentGrid(int qIndex, ref IntersectSegmentParams segParams, ref float minDistance, ref uint tmpHitFlag, int i, int j, bool spawnSparks)
	{
		if (CollisionDataMesh.WalkableData[i][j].Indices == null)
		{
			return;
		}
		for (int k = 0; k < CollisionDataMesh.WalkableData[i][j].Indices.Length; k++)
		{
			int num = CollisionDataMesh.WalkableData[i][j].Indices[k];
			if ((CollisionDataMesh.TriangleDataMesh[num].Flags & 8) != 0 || !MyMath.IntersectSegmentTriangle(ref segParams, ref CollisionDataMesh.TriangleDataMesh[num]))
			{
				continue;
			}
			float num2 = segParams.Tparameter * segParams.SegmentLength;
			if (num2 < minDistance)
			{
				if (spawnSparks && (CollisionDataMesh.TriangleDataMesh[num].Flags & 0x10000) != 0)
				{
					particles.SpawnBulletHitMetal(ref segParams.hitPosition, ref segParams.hitNormal);
					continue;
				}
				RaycastSecondHit = segParams.hitPosition;
				tmpHitFlag = CollisionDataMesh.TriangleDataMesh[num].Flags;
				minDistance = num2;
				tmpHitNormal = segParams.hitNormal;
				tmpHitPosition = segParams.hitPosition;
			}
		}
		if (CollisionDataMesh.NoWalkData[i][j].Indices == null)
		{
			return;
		}
		for (int l = 0; l < CollisionDataMesh.NoWalkData[i][j].Indices.Length; l++)
		{
			int num3 = CollisionDataMesh.NoWalkData[i][j].Indices[l];
			if (!MyMath.IntersectSegmentTriangle(ref segParams, ref CollisionDataMesh.TriangleDataMesh[num3]))
			{
				continue;
			}
			float num4 = segParams.Tparameter * segParams.SegmentLength;
			if (num4 < minDistance)
			{
				if (spawnSparks && (CollisionDataMesh.TriangleDataMesh[num3].Flags & 0x10000) != 0)
				{
					particles.SpawnBulletHitMetal(ref segParams.hitPosition, ref segParams.hitNormal);
					continue;
				}
				RaycastSecondHit = segParams.hitPosition;
				tmpHitFlag = CollisionDataMesh.TriangleDataMesh[num3].Flags;
				minDistance = num4;
				tmpHitNormal = segParams.hitNormal;
				tmpHitPosition = segParams.hitPosition;
			}
		}
	}

	public static MaterialType CharacterHeightCast(int qIndex, ref IntersectSegmentParams segParams, bool spawnSparks)
	{
		tmpHitMaterial = MaterialType.Undefined;
		float num = 100000f;
		RaycastHitDistance = 1000000f;
		uint num2 = 0u;
		segParams.hitNormal = Vector3.UnitY;
		segParams.TargetIndex = -1;
		int outX = 0;
		int outZ = 0;
		CollisionDataMesh.GetGridPosition(ref outX, ref outZ, segParams.SegmentStart.X, segParams.SegmentStart.Z);
		if (CollisionDataMesh.WalkableData[outX][outZ].Indices != null)
		{
			for (int i = 0; i < CollisionDataMesh.WalkableData[outX][outZ].Indices.Length; i++)
			{
				int num3 = CollisionDataMesh.WalkableData[outX][outZ].Indices[i];
				if (MyMath.IntersectSegmentTriangle(ref segParams, ref CollisionDataMesh.TriangleDataMesh[num3]))
				{
					float num4 = segParams.Tparameter * segParams.SegmentLength;
					if (num4 < num)
					{
						RaycastSecondHit = segParams.hitPosition;
						num2 = CollisionDataMesh.TriangleDataMesh[num3].Flags;
						num = num4;
						tmpHitNormal = segParams.hitNormal;
						tmpHitPosition = segParams.hitPosition;
					}
				}
			}
		}
		if (num < 100000f)
		{
			if ((num2 | 0x200000) != 0)
			{
				tmpHitMaterial = MaterialType.Concrete;
			}
			else if ((num2 | 0x100000) != 0)
			{
				tmpHitMaterial = MaterialType.Glass;
			}
			else if ((num2 | 0x10000) != 0)
			{
				tmpHitMaterial = MaterialType.Metal;
			}
			else if ((num2 | 0x40000) != 0)
			{
				tmpHitMaterial = MaterialType.Rock;
			}
			else if ((num2 | 0x20000) != 0)
			{
				tmpHitMaterial = MaterialType.Wood;
			}
			segParams.hitNormal = tmpHitNormal;
			segParams.hitPosition = tmpHitPosition;
			RaycastHitDistance = num;
			segParams.hitDistance = RaycastHitDistance;
		}
		return tmpHitMaterial;
	}

	private static void TestWalkableSegmentGrid(int qIndex, ref IntersectSegmentParams segParams, ref float minDistance, ref uint tmpHitFlag, int i, int j, bool spawnSparks)
	{
		if (CollisionDataMesh.WalkableData[i][j].Indices == null)
		{
			return;
		}
		for (int k = 0; k < CollisionDataMesh.WalkableData[i][j].Indices.Length; k++)
		{
			int num = CollisionDataMesh.WalkableData[i][j].Indices[k];
			if (!MyMath.IntersectSegmentTriangle(ref segParams, ref CollisionDataMesh.TriangleDataMesh[num]))
			{
				continue;
			}
			float num2 = segParams.Tparameter * segParams.SegmentLength;
			if (num2 < minDistance)
			{
				if (spawnSparks && (CollisionDataMesh.TriangleDataMesh[num].Flags & 0x10000) != 0)
				{
					particles.SpawnBulletHitMetal(ref segParams.hitPosition, ref segParams.hitNormal);
					continue;
				}
				RaycastSecondHit = segParams.hitPosition;
				tmpHitFlag = CollisionDataMesh.TriangleDataMesh[num].Flags;
				minDistance = num2;
				tmpHitNormal = segParams.hitNormal;
				tmpHitPosition = segParams.hitPosition;
			}
		}
	}

	public static bool IntersectFPSCharacter(ref BoundingSphere e, ref Vector3 lastPos, ref CollisionStruct c, bool Crouched, float yOffset)
	{
		bool result = false;
		float num = e.Radius * e.Radius;
		c.flags = 0u;
		c.applyResponse = true;
		tmpTriggerCollision.hitTrigger = TriggerTypes.Undeclared;
		tmpTriggerCollision.applyResponse = false;
		if (LevelBaseMenu.gameMode == GameMode.CombatTraining)
		{
			for (int i = 0; i < Segments.Length; i++)
			{
				if (Segments[i].SegType == EnumObjectTypes.NumberOf)
				{
					tmpSegmentParams.SegmentDirection = vecUnitNegY;
					tmpSegmentParams.SegmentLength = 500f;
					tmpSegmentParams.SegmentStart = e.Center;
					tmpSegmentParams.SegmentEnd = e.Center + vecUnitNegY * tmpSegmentParams.SegmentLength;
					tmpSegmentParams.PreComputeParameters();
					for (int j = 0; j < Segments[i].Geometry.Length; j++)
					{
						tmpMesh = (eTriangleMesh)Segments[i].Geometry[j].Mesh.Tag;
						result = tmpMesh.IntersectFPSCharacter(ref e, ref c, Crouched, yOffset);
					}
				}
			}
		}
		for (int k = 0; k < Segments.Length; k++)
		{
			if (LevelBaseMenu.gameMode == GameMode.XboxLive && Segments[k].SegType == EnumObjectTypes.NumberOf)
			{
				continue;
			}
			tmpIntersectBBox.Min = Segments[k].Min;
			tmpIntersectBBox.Max = Segments[k].Max;
			if (!tmpIntersectBBox.Intersects(e))
			{
				continue;
			}
			for (int l = 0; l < Segments[k].Triggers.Length; l++)
			{
				if (Segments[k].Triggers[l].oobb.ContainsPoint(ref e.Center) && !Segments[k].Triggers[l].ProcessTrigger())
				{
					tmpTriggerCollision.hitTrigger = Segments[k].Triggers[l].eType;
					tmpTriggerCollision.hitNormal = Segments[k].Triggers[l].direction;
					tmpTriggerCollision.hitPosition = Segments[k].Triggers[l].center;
				}
				if (Segments[k].Triggers[l].IntersectFPSCharacter(ref e))
				{
					result = true;
				}
			}
		}
		c.hitTrigger = tmpTriggerCollision.hitTrigger;
		if (c.hitTrigger == TriggerTypes.Ladder || c.hitTrigger == TriggerTypes.LadderBottom)
		{
			c.hitNormal = tmpTriggerCollision.hitNormal;
			c.hitPosition = tmpTriggerCollision.hitPosition;
			return result;
		}
		tmpSegmentParams.SegmentDirection = -Vector3.UnitY;
		tmpSegmentParams.SegmentLength = 500f;
		tmpSegmentParams.SegmentStart = e.Center;
		tmpSegmentParams.SegmentEnd = e.Center + -Vector3.UnitY * tmpSegmentParams.SegmentLength;
		tmpSegmentParams.PreComputeParameters();
		vecMovement = lastPos - e.Center;
		vecMovement.Normalize();
		int outX = 0;
		int outZ = 0;
		CollisionDataMesh.GetGridPosition(ref outX, ref outZ, e.Center.X, e.Center.Z);
		if (CollisionDataMesh.WalkableData[outX][outZ].Indices != null)
		{
			float num2 = yOffset + e.Radius;
			for (int m = 0; m < CollisionDataMesh.WalkableData[outX][outZ].Indices.Length; m++)
			{
				int num3 = CollisionDataMesh.WalkableData[outX][outZ].Indices[m];
				if (Vector3.Dot(CollisionDataMesh.TriangleDataMesh[num3].Normal, Vector3.UnitY) > 0.6f && MyMath.IntersectSegmentTriangle(ref tmpSegmentParams, ref CollisionDataMesh.TriangleDataMesh[num3]))
				{
					float num4 = tmpSegmentParams.Tparameter * tmpSegmentParams.SegmentLength - 2f;
					if (num4 < num2)
					{
						c.onWalkable = true;
						e.Center.Y += num2 - num4;
					}
				}
				float disSqr = 1000000f;
				if (MyMath.IntersectSphereTriangle(ref e.Center, num, ref CollisionDataMesh.TriangleDataMesh[num3], ref closestPoint, ref disSqr))
				{
					float num5 = disSqr / num;
					closestPoint.Normalize();
					c.depth = e.Radius - num5 * e.Radius;
					c.hitNormal = closestPoint;
					e.Center += c.hitNormal * c.depth;
					c.flags |= CollisionDataMesh.TriangleDataMesh[num3].Flags;
					c.haveCollision = true;
					result = true;
					continue;
				}
				disSqr = 1000000f;
				TorsoCenter = e.Center;
				if (Crouched)
				{
					TorsoCenter.Y += 0f;
				}
				else
				{
					TorsoCenter.Y += 42f;
				}
				if (MyMath.IntersectSphereTriangle(ref TorsoCenter, num, ref CollisionDataMesh.TriangleDataMesh[num3], ref closestPoint, ref disSqr))
				{
					float num6 = disSqr / num;
					closestPoint.Normalize();
					e.Center += closestPoint * (e.Radius - num6 * e.Radius);
					c.haveCollision = true;
					result = true;
				}
			}
		}
		if (c.hitTrigger != TriggerTypes.Undeclared)
		{
			c.hitNormal = tmpTriggerCollision.hitNormal;
			c.hitPosition = tmpTriggerCollision.hitPosition;
		}
		return result;
	}

	public static bool SphereIntersect(ref BoundingSphere e, ref Vector3 hitNormal)
	{
		bool result = SphereIntersect(ref e, ref tmpSphereCollision);
		hitNormal = tmpSphereCollision.hitNormal;
		return result;
	}

	public static bool SphereIntersect(ref BoundingSphere e, ref CollisionStruct c)
	{
		bool result = false;
		float num = e.Radius * e.Radius;
		c.flags = 0u;
		c.haveCollision = false;
		c.applyResponse = true;
		int outX = 0;
		int outZ = 0;
		CollisionDataMesh.GetGridPosition(ref outX, ref outZ, e.Center.X, e.Center.Z);
		if (CollisionDataMesh.WalkableData[outX][outZ].Indices != null)
		{
			for (int i = 0; i < CollisionDataMesh.WalkableData[outX][outZ].Indices.Length; i++)
			{
				int num2 = CollisionDataMesh.WalkableData[outX][outZ].Indices[i];
				float disSqr = 1000000f;
				if (MyMath.IntersectSphereTriangle(ref e.Center, num, ref CollisionDataMesh.TriangleDataMesh[num2], ref closestPoint, ref disSqr))
				{
					float num3 = disSqr / num;
					c.hitNormal = closestPoint;
					c.hitNormal.Normalize();
					c.depth = e.Radius - num3 * e.Radius;
					e.Center += c.hitNormal * c.depth;
					c.flags |= CollisionDataMesh.TriangleDataMesh[num2].Flags;
					c.haveCollision = true;
					result = true;
				}
			}
		}
		return result;
	}

	public static GeometryFlags SphereIntersectSegment(ref BoundingSphere e, ref Vector3 n, int segmentIndex)
	{
		if (segmentIndex >= 0)
		{
			tmpSegmentParams.SegmentDirection = vecUnitNegY;
			tmpSegmentParams.SegmentLength = 500f;
			tmpSegmentParams.SegmentStart = e.Center;
			tmpSegmentParams.SegmentEnd = e.Center + vecUnitNegY * tmpSegmentParams.SegmentLength;
			tmpSegmentParams.PreComputeParameters();
			for (int i = 0; i < Segments[segmentIndex].Geometry.Length; i++)
			{
				tmpMesh = (eTriangleMesh)Segments[segmentIndex].Geometry[i].Mesh.Tag;
				if (tmpMesh.TestSphereTriangle(ref e, ref n))
				{
					return tmpMesh.Flags;
				}
			}
		}
		return GeometryFlags.Clear;
	}

	public static bool SphereIntersectWithList(ref BoundingSphere e)
	{
		bool result = false;
		tmpTriggerCollision.hitTrigger = TriggerTypes.Undeclared;
		tmpTriggerCollision.applyResponse = false;
		for (int i = 0; i < 64 && MyShereIntersectList.SegPhysics[i] >= 0; i++)
		{
			Segments[MyShereIntersectList.SegPhysics[i]].Physics[MyShereIntersectList.SegPhysicsIndex[i]].IntersectSphere(ref e);
		}
		for (int j = 0; j < 64 && MyShereIntersectList.SegGeometry[j] >= 0; j++)
		{
			tmpMesh = (eTriangleMesh)Segments[MyShereIntersectList.SegGeometry[j]].Geometry[MyShereIntersectList.SegGeometryIndex[j]].Mesh.Tag;
			if ((tmpMesh.Flags & GeometryFlags.Walkable) > GeometryFlags.Clear)
			{
				tmpSegmentParams.SegmentDirection = vecUnitNegY;
				tmpSegmentParams.SegmentLength = 500f;
				tmpSegmentParams.SegmentStart = e.Center;
				tmpSegmentParams.SegmentEnd = e.Center + vecUnitNegY * tmpSegmentParams.SegmentLength;
				tmpSegmentParams.PreComputeParameters();
				tmpMesh.IntersectSphere(ref e);
			}
		}
		return result;
	}

	private static void SetEffectLights()
	{
		for (int i = 0; i < Segments.Length; i++)
		{
			int num = 0;
			for (int j = 0; j < Segments[i].Geometry.Length; j++)
			{
				drawMesh = Segments[i].Geometry[j].Mesh;
				for (int k = 0; k < drawMesh.MeshParts.Count; k++)
				{
					drawMeshPart = drawMesh.MeshParts[k];
					((EffectParams)drawMeshPart.Tag).vecSunPosition.SetValue(SunPosition);
					if (num > 0)
					{
						((EffectParams)drawMeshPart.Tag).numberLights.SetValue(num);
						((EffectParams)drawMeshPart.Tag).vecLightPositions.SetValue(veclights);
						((EffectParams)drawMeshPart.Tag).vecLightColors.SetValue(veclightsColors);
					}
				}
			}
		}
	}

	public static bool GetSphereIntersectList(ref BoundingSphere e)
	{
		bool result = false;
		int num = 0;
		SphereIntersectList[num] = -1;
		float radiusSqr = e.Radius * e.Radius;
		int num2 = (int)(Math.Abs(CollisionDataMesh.GridMin.X) + e.Center.X) / CollisionDataMesh.GridCellSize;
		int num3 = (int)(Math.Abs(CollisionDataMesh.GridMin.Z) + e.Center.Z) / CollisionDataMesh.GridCellSize;
		num2 = ((num2 >= 0) ? num2 : 0);
		num3 = ((num3 >= 0) ? num3 : 0);
		num2 = ((num2 < CollisionDataMesh.NumberGridX) ? num2 : (CollisionDataMesh.NumberGridX - 1));
		num3 = ((num3 < CollisionDataMesh.NumberGridZ) ? num3 : (CollisionDataMesh.NumberGridZ - 1));
		if (CollisionDataMesh.WalkableData[num2][num3].Indices != null)
		{
			for (int i = 0; i < CollisionDataMesh.WalkableData[num2][num3].Indices.Length; i++)
			{
				int num4 = CollisionDataMesh.WalkableData[num2][num3].Indices[i];
				float dS = 1000000f;
				if (IntersectRagdollSphereTriangle(ref e.Center, radiusSqr, ref CollisionDataMesh.TriangleDataMesh[num4], ref closestPointRG, ref dS))
				{
					result = true;
					SphereIntersectList[num++] = num4;
					if (num >= 2047)
					{
						num = 2047;
						break;
					}
				}
			}
		}
		SphereIntersectList[num] = -1;
		return result;
	}

	public static float RagdollSphereIntersectList(ref BoundingSphere e, ref Vector3 hitNormal)
	{
		float num = 0f;
		float num2 = e.Radius * e.Radius;
		for (int i = 0; i < 2048 && SphereIntersectList[i] >= 0; i++)
		{
			int num3 = SphereIntersectList[i];
			if (num3 < 0)
			{
				break;
			}
			float dS = 1000000f;
			if (IntersectRagdollSphereTriangle(ref e.Center, num2, ref CollisionDataMesh.TriangleDataMesh[num3], ref closestPointRG, ref dS))
			{
				float num4 = dS / num2;
				hitNormal = closestPointRG;
				hitNormal.Normalize();
				float num5 = e.Radius - num4 * e.Radius;
				e.Center += hitNormal * num5;
				if (num5 > num)
				{
					num = num5;
				}
			}
		}
		return num;
	}

	public static float RagdollSphereIntersect(ref BoundingSphere e, ref Vector3 hitNormal)
	{
		float num = 0f;
		float num2 = e.Radius * e.Radius;
		int num3 = (int)(Math.Abs(CollisionDataMesh.GridMin.X) + e.Center.X) / CollisionDataMesh.GridCellSize;
		int num4 = (int)(Math.Abs(CollisionDataMesh.GridMin.Z) + e.Center.Z) / CollisionDataMesh.GridCellSize;
		num3 = ((num3 >= 0) ? num3 : 0);
		num4 = ((num4 >= 0) ? num4 : 0);
		num3 = ((num3 < CollisionDataMesh.NumberGridX) ? num3 : (CollisionDataMesh.NumberGridX - 1));
		num4 = ((num4 < CollisionDataMesh.NumberGridZ) ? num4 : (CollisionDataMesh.NumberGridZ - 1));
		if (CollisionDataMesh.WalkableData[num3][num4].Indices != null)
		{
			for (int i = 0; i < CollisionDataMesh.WalkableData[num3][num4].Indices.Length; i++)
			{
				int num5 = CollisionDataMesh.WalkableData[num3][num4].Indices[i];
				float dS = 1000000f;
				if (IntersectRagdollSphereTriangle(ref e.Center, num2, ref CollisionDataMesh.TriangleDataMesh[num5], ref closestPointRG, ref dS))
				{
					float num6 = dS / num2;
					hitNormal = closestPointRG;
					hitNormal.Normalize();
					float num7 = e.Radius - num6 * e.Radius;
					e.Center += hitNormal * num7;
					if (num7 > num)
					{
						num = num7;
					}
				}
			}
		}
		return num;
	}

	public static bool IntersectRagdollSphereTriangle(ref Vector3 origin, float radiusSqr, ref TriangleData triangle, ref Vector3 cP, ref float dS)
	{
		vecP1 = MyMath.PositionList[triangle.p1];
		vecP2 = MyMath.PositionList[triangle.p2];
		vecP3 = MyMath.PositionList[triangle.p3];
		RagdollClosestPTPointTriangle(ref origin, ref vecP1, ref vecP2, ref vecP3, ref cP);
		cP.X = origin.X - cP.X;
		cP.Y = origin.Y - cP.Y;
		cP.Z = origin.Z - cP.Z;
		dS = cP.LengthSquared();
		return dS <= radiusSqr;
	}

	private static void RagdollClosestPTPointTriangle(ref Vector3 p, ref Vector3 a, ref Vector3 b, ref Vector3 c, ref Vector3 cPs)
	{
		rab.X = b.X - a.X;
		rab.Y = b.Y - a.Y;
		rab.Z = b.Z - a.Z;
		rac.X = c.X - a.X;
		rac.Y = c.Y - a.Y;
		rac.Z = c.Z - a.Z;
		rap.X = p.X - a.X;
		rap.Y = p.Y - a.Y;
		rap.Z = p.Z - a.Z;
		float num = rab.X * rap.X + rab.Y * rap.Y + rab.Z * rap.Z;
		float num2 = rac.X * rap.X + rac.Y * rap.Y + rac.Z * rap.Z;
		if (num <= 0f && num2 <= 0f)
		{
			cPs = a;
			return;
		}
		rbp.X = p.X - b.X;
		rbp.Y = p.Y - b.Y;
		rbp.Z = p.Z - b.Z;
		float num3 = rab.X * rbp.X + rab.Y * rbp.Y + rab.Z * rbp.Z;
		float num4 = rac.X * rbp.X + rac.Y * rbp.Y + rac.Z * rbp.Z;
		if (num3 >= 0f && num4 <= num3)
		{
			cPs = b;
			return;
		}
		float num5 = num * num4 - num3 * num2;
		if (num5 <= 0f && num >= 0f && num3 <= 0f)
		{
			float num6 = num / (num - num3);
			cPs = a + num6 * rab;
			return;
		}
		rcp.X = p.X - c.X;
		rcp.Y = p.Y - c.Y;
		rcp.Z = p.Z - c.Z;
		float num7 = rab.X * rcp.X + rab.Y * rcp.Y + rab.Z * rcp.Z;
		float num8 = rac.X * rcp.X + rac.Y * rcp.Y + rac.Z * rcp.Z;
		if (num8 >= 0f && num7 <= num8)
		{
			cPs = c;
			return;
		}
		float num9 = num7 * num2 - num * num8;
		if (num9 <= 0f && num2 >= 0f && num8 <= 0f)
		{
			float num10 = num2 / (num2 - num8);
			cPs = a + num10 * rac;
			return;
		}
		float num11 = num3 * num8 - num7 * num4;
		if (num11 <= 0f && num4 - num3 >= 0f && num7 - num8 >= 0f)
		{
			float num12 = (num4 - num3) / (num4 - num3 + (num7 - num8));
			cPs = b + num12 * (c - b);
			return;
		}
		float num13 = 1f / (num11 + num9 + num5);
		float num14 = num9 * num13;
		float num15 = num5 * num13;
		cPs = a + rab * num14 + rac * num15;
	}
}
