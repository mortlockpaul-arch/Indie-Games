using System;
using DataContent;
using MaxScriptDefines;
using Microsoft.Xna.Framework;

namespace EGEngine;

public struct eLevelSegment
{
	public EnumObjectTypes SegType;

	public string Name;

	public int LightIndex;

	public Vector3 Min;

	public Vector3 Max;

	public Vector3 Center;

	public Vector3 Extents;

	public float CurrentRayCastDistance;

	public bool[][] Render;

	private bool currentraycasthit;

	public GeometryMesh[] Geometry;

	public eOOBB[] Physics;

	public TriggerData[] Triggers;

	public void CurrentRayCastHit(bool e)
	{
		currentraycasthit = e;
	}

	public void AddChildren(LevelModel model, eMesh root, Matrix[] transforms)
	{
		Render = new bool[2][];
		LightIndex = 0;
		Min = Vector3.Zero;
		Max = Vector3.Zero;
		for (int i = 0; i < 2; i++)
		{
			Render[i] = new bool[4];
			Render[i][0] = false;
			Render[i][1] = false;
			Render[i][2] = false;
			Render[i][3] = false;
		}
		if (root.Name == "segment_northwest")
		{
			Name = root.Name;
		}
		else
		{
			Name = root.Name;
		}
		OOBB oOBB = new OOBB(MeshTools.GetPositionsFromeMesh(root, VertexType.PosNormTexTan), Matrix.Identity);
		Vector3 a = Vector3.Transform(oOBB.Min, Matrix.Identity);
		Vector3 b = Vector3.Transform(oOBB.Max, Matrix.Identity);
		Min = math.MinVector(a, b);
		Max = math.MaxVector(a, b);
		Extents = (Max - Min) * 0.5f;
		Center = Min + Extents;
		if (root.Name.Contains("TRGS"))
		{
			Physics = ((GeometryData)root.Tag).oobb;
		}
		else
		{
			Physics = ((GeometryData)root.Tag).oobb;
		}
		int num = 0;
		for (int j = 0; j < root.Children.Count; j++)
		{
			eMesh eMesh2 = root.Children[j];
			if (!eMesh2.Name.Contains("light") && !eMesh2.Name.Contains("trigger") && !eMesh2.Name.Contains("segment"))
			{
				num++;
			}
		}
		Geometry = new GeometryMesh[num];
		num = 0;
		int num2 = 0;
		for (int k = 0; k < root.Children.Count; k++)
		{
			eMesh eMesh3 = root.Children[k];
			if (eMesh3.Name.Contains("trigger"))
			{
				num2++;
			}
		}
		Triggers = new TriggerData[num2];
		num2 = 0;
		for (int l = 0; l < root.Children.Count; l++)
		{
			eMesh eMesh4 = root.Children[l];
			if (eMesh4.Name.Contains("trigger"))
			{
				Triggers[num2].oobb.SetFromeMesh(eMesh4, Matrix.Identity, VertexType.Position);
				Triggers[num2].Reset();
				if (eMesh4.Name.Contains("ladder") || eMesh4.Name.Contains("ldrbottom"))
				{
					if (eMesh4.Name.Contains("ladder"))
					{
						Triggers[num2].eType = TriggerTypes.Ladder;
					}
					if (eMesh4.Name.Contains("ldrbottom"))
					{
						Triggers[num2].eType = TriggerTypes.LadderBottom;
					}
					Triggers[num2].direction = Vector3.Transform(Vector3.UnitX, Matrix.Identity);
					Triggers[num2].direction.Normalize();
					Vector3 position = math.MinVector(Triggers[num2].oobb.Min, Triggers[num2].oobb.Max);
					position += Triggers[num2].oobb.extents;
					position = Vector3.Transform(position, Matrix.Identity);
					Triggers[num2].center = position;
				}
				else if (eMesh4.Name.Contains("TRGS"))
				{
					Triggers[num2].eType = TriggerTypes.TargetPractice;
					Triggers[num2].direction = Vector3.Transform(Vector3.UnitX, Matrix.Identity);
					Triggers[num2].direction.Normalize();
					Vector3 position2 = math.MinVector(Triggers[num2].oobb.Min, Triggers[num2].oobb.Max);
					position2 += Triggers[num2].oobb.extents;
					position2 = Vector3.Transform(position2, Matrix.Identity);
					int num3 = eMesh4.Name.IndexOf('_') + 1;
					string text = eMesh4.Name.Substring(num3, eMesh4.Name.Length - num3);
					Triggers[num2].center = position2;
					Triggers[num2].SegmentName = "segment_" + text;
				}
				num2++;
			}
			else
			{
				if (eMesh4.Name.Contains("segment") || eMesh4.Name.Contains("light"))
				{
					continue;
				}
				Geometry[num].Render = new bool[2][];
				Geometry[num].RenderLOD = new bool[2][];
				for (int m = 0; m < 2; m++)
				{
					Geometry[num].Render[m] = new bool[4];
					Geometry[num].Render[m][0] = false;
					Geometry[num].Render[m][1] = false;
					Geometry[num].Render[m][2] = false;
					Geometry[num].Render[m][3] = false;
					Geometry[num].RenderLOD[m] = new bool[4];
					Geometry[num].RenderLOD[m][0] = false;
					Geometry[num].RenderLOD[m][1] = false;
					Geometry[num].RenderLOD[m][2] = false;
					Geometry[num].RenderLOD[m][3] = false;
				}
				Geometry[num].Mesh = eMesh4;
				Geometry[num].MeshLOD = null;
				for (int n = 0; n < eMesh4.MeshParts.Count; n++)
				{
					eMeshPart eMeshPart2 = eMesh4.MeshParts[n];
					eMeshPart2.Tag = new EffectParams(eMeshPart2.Effect, eMesh4.Name);
				}
				if (eMesh4.Children.Count > 0)
				{
					Geometry[num].MeshLOD = eMesh4.Children[0];
					for (int num4 = 0; num4 < eMesh4.Children[0].MeshParts.Count; num4++)
					{
						eMeshPart eMeshPart3 = eMesh4.Children[0].MeshParts[num4];
						eMeshPart3.Tag = new EffectParams(eMeshPart3.Effect, eMesh4.Children[0].Name);
					}
				}
				num++;
			}
		}
	}

	public bool TestSegment(ref IntersectSegmentParams segment)
	{
		Vector3 vector = segment.SegmentMidpoint - Center;
		float num = Math.Abs(segment.SegmentHalflength.X);
		if (Math.Abs(vector.X) > Extents.X + num)
		{
			return false;
		}
		float num2 = Math.Abs(segment.SegmentHalflength.Y);
		if (Math.Abs(vector.Y) > Extents.Y + num2)
		{
			return false;
		}
		float num3 = Math.Abs(segment.SegmentHalflength.Z);
		if (Math.Abs(vector.Z) > Extents.Z + num3)
		{
			return false;
		}
		num += 1E-05f;
		num2 += 1E-05f;
		num3 += 1E-05f;
		if (Math.Abs(vector.Y * segment.SegmentHalflength.Z - vector.Z * segment.SegmentHalflength.Y) > Extents.Y * num3 + Extents.Z * num2)
		{
			return false;
		}
		if (Math.Abs(vector.Z * segment.SegmentHalflength.X - vector.X * segment.SegmentHalflength.Z) > Extents.X * num3 + Extents.Z * num)
		{
			return false;
		}
		if (Math.Abs(vector.X * segment.SegmentHalflength.Y - vector.Y * segment.SegmentHalflength.X) > Extents.X * num2 + Extents.Y * num)
		{
			return false;
		}
		return true;
	}
}
