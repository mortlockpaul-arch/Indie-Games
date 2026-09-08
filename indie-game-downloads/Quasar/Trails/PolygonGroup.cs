using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Meshes.Generic;
using Quasar.Render;
using Quasar.Shaders;
using Quasar.Textures;
using Quasar.Utils;

namespace Quasar.Trails;

public class PolygonGroup : UserIndexVertexMesh<PolygonVertex>, ITrailGroup
{
	private const string TRAILS_DIR = "Trails/";

	private TrailSystem trailSystem;

	public KeyedValue<Vector3> color;

	public KeyedValue<float> alpha;

	public KeyedValue<float> size;

	public int RenderPriority
	{
		get
		{
			return (int)(base.FirstMaterial.RenderPriority - 50);
		}
		set
		{
			base.FirstMaterial.RenderPriority = (Material.Priority)(50 + value);
		}
	}

	Mesh ITrailGroup.Mesh => this;

	public PolygonGroup(TrailSystem system)
	{
		hasSimpleLayout = true;
		trailSystem = system;
		SetMaxTrailCount();
		DebugColor = Color.LightGray;
		Material material = new Material();
		material.AddIntParameter(0);
		material.AddIntParameter(0);
		material.AddIntParameter(0);
		material.AddFloatParameter(0f);
		material.SetForcedAlpha(alpha: true);
		material.RenderPriority = Material.Priority.Low;
		materials.Add(material);
	}

	public void SetMaxTrailCount()
	{
		initMesh(trailSystem.TrailLength * trailSystem.MaxTrailCount);
	}

	private new void initMesh(int trailPoints)
	{
		initMesh(trailPoints * 3, trailPoints * 6, useLongIndices: false);
		for (int i = 0; i < trailPoints; i++)
		{
			verticesBuffer[i * 3].UV = new Vector2(0f, 0f);
			verticesBuffer[i * 3 + 1].UV = new Vector2(0f, 0.5f);
			verticesBuffer[i * 3 + 2].UV = new Vector2(0f, 1f);
		}
	}

	internal static PolygonGroup ParseXml(XElement xe, TrailSystem p)
	{
		PolygonGroup polygonGroup = new PolygonGroup(p);
		polygonGroup.Texture = TextureManager.Textures["Trails/" + XDocHelper.GetAttribute(xe, "texture")];
		polygonGroup.Shader = ShaderManager.Shaders["Trails/" + XDocHelper.GetAttribute(xe, "shader")];
		polygonGroup.RenderPriority = XDocHelper.ParseIntAttribute(xe, "renderPriority");
		polygonGroup.color = KeyedValue.ParseXmlVector3(xe, "ColorKeys");
		polygonGroup.alpha = KeyedValue.ParseXmlFloat(xe, "AlphaKeys");
		polygonGroup.size = KeyedValue.ParseXmlFloat(xe, "SizeKeys");
		return polygonGroup;
	}

	public void Update()
	{
		short num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = 0; i < trailSystem.TrailCount; i++)
		{
			Trail trail = trailSystem.Trails[i];
			int pointCount = trail.PointCount;
			if (pointCount <= 1)
			{
				continue;
			}
			float progress = trail.Points[0].Progress;
			float num4 = trail.CurrentTrailLength - progress;
			for (int j = 0; j < pointCount; j++)
			{
				float progress2 = (trail.Points[j].Progress - progress) / num4;
				setPoint(ref trail.Points[j], progress2, ref verticesBuffer[num], ref verticesBuffer[num + 1], ref verticesBuffer[num + 2]);
				if (j > 0)
				{
					indicesBuffer[num2] = (short)(num - 3);
					indicesBuffer[num2 + 1] = num;
					indicesBuffer[num2 + 2] = (short)(num + 1);
					indicesBuffer[num2 + 3] = (short)(num - 2);
					indicesBuffer[num2 + 4] = (short)(num - 3);
					indicesBuffer[num2 + 5] = (short)(num + 1);
					indicesBuffer[num2 + 6] = (short)(num - 2);
					indicesBuffer[num2 + 7] = (short)(num + 1);
					indicesBuffer[num2 + 8] = (short)(num + 2);
					indicesBuffer[num2 + 9] = (short)(num - 1);
					indicesBuffer[num2 + 10] = (short)(num - 2);
					indicesBuffer[num2 + 11] = (short)(num + 2);
					num2 += 12;
					num3 += 4;
				}
				num += 3;
			}
		}
		base.PrimitiveCount = num3;
	}

	protected override void PrepareRender()
	{
		short num = 0;
		Vector3 vector = SceneRenderData.CurrentRenderData.Camera.Transform.WorldZ;
		Vector3 vector2 = SceneRenderData.CurrentRenderData.Camera.Transform.WorldY;
		Vector3 result = Vector3.Zero;
		Vector3 result2 = Vector3.Zero;
		for (int i = 0; i < trailSystem.TrailCount; i++)
		{
			Trail trail = trailSystem.Trails[i];
			int pointCount = trail.PointCount;
			if (pointCount <= 1)
			{
				continue;
			}
			for (int j = 0; j < pointCount; j++)
			{
				if (j == 0)
				{
					Vector3.Subtract(ref trail.Points[1].Point, ref trail.Points[0].Point, out result);
				}
				else if (j == pointCount - 1)
				{
					Vector3.Subtract(ref trail.Points[pointCount - 1].Point, ref trail.Points[pointCount - 2].Point, out result);
				}
				else
				{
					result = trail.Points[j + 1].Point - trail.Points[j - 1].Point;
				}
				Vector3.Normalize(ref result, out result);
				Vector3.Dot(ref vector, ref result, out var result3);
				result3 = Math.Abs(result3);
				if (result3 < 0.98f)
				{
					Vector3.Cross(ref vector, ref result, out result);
				}
				else
				{
					Vector3.Cross(ref vector2, ref result, out result2);
					Vector3.Cross(ref vector, ref result, out result);
					Vector3.Lerp(ref result, ref result2, (result3 - 0.98f) / 0.02f, out result);
				}
				Vector3.Normalize(ref result, out result);
				setNormal(ref verticesBuffer[num], ref verticesBuffer[num + 1], ref verticesBuffer[num + 2], ref result);
				num += 3;
			}
		}
		base.PrepareRender();
	}

	private void setNormal(ref PolygonVertex v1, ref PolygonVertex v2, ref PolygonVertex v3, ref Vector3 normal)
	{
		v1.Normal = (v2.Normal = (v3.Normal = normal));
	}

	private void setPoint(ref Trail.PointData point, float progress, ref PolygonVertex v1, ref PolygonVertex v2, ref PolygonVertex v3)
	{
		v1.Position = (v2.Position = (v3.Position = point.Point));
		v3.UV.X = (v1.UV.X = (v2.UV.X = progress));
		color.GetValue(progress, out v1.Color);
		v3.Color = (v2.Color = v1.Color);
		alpha.GetValue(progress, out v1.Alpha);
		v3.Alpha = (v2.Alpha = v1.Alpha);
		size.GetValue(progress, out v1.TrailData.X);
		v3.TrailData.X = (v2.TrailData.X = v1.TrailData.X);
		v1.TrailData.Y = (v2.TrailData.Y = (v3.TrailData.Y = progress));
	}

	public override void Dispose()
	{
		base.Dispose();
	}
}
