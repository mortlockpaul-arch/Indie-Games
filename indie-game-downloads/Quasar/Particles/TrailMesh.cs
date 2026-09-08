using System;
using Microsoft.Xna.Framework;
using Quasar.Meshes.Generic;
using Quasar.Render;

namespace Quasar.Particles;

internal class TrailMesh : UserIndexVertexMesh<TrailVertex>
{
	private TrailGroup group;

	public TrailMesh(TrailGroup group)
	{
		this.group = group;
		hasSimpleLayout = true;
		DebugColor = Color.SeaGreen;
		Material material = new Material();
		material.AddIntParameter(0);
		material.AddFloatParameter(0f);
		material.SetForcedAlpha(alpha: true);
		material.RenderPriority = Material.Priority.Low;
		materials.Add(material);
	}

	protected override void PrepareRender()
	{
		short num = 0;
		Vector3 vector = SceneRenderData.CurrentRenderData.Camera.Transform.WorldZ;
		Vector3 vector2 = SceneRenderData.CurrentRenderData.Camera.Transform.WorldY;
		Vector3 result = Vector3.Zero;
		Vector3 result2 = Vector3.Zero;
		for (int i = 0; i < group.currentParticleCount; i++)
		{
			TrailGroup.Trail trail = group.trails[i];
			Vector3.Subtract(ref trail.TrailPosition, ref trail.Position, out result);
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
			for (int j = 0; j < 2; j++)
			{
				setNormal(ref verticesBuffer[num], ref verticesBuffer[num + 1], ref result);
				num += 2;
			}
		}
		base.PrepareRender();
	}

	private void setNormal(ref TrailVertex v1, ref TrailVertex v2, ref Vector3 normal)
	{
		v1.Normal = (v2.Normal = normal);
	}

	private new void initMesh(int maxParticles)
	{
		initMesh(maxParticles * 4, maxParticles * 6, useLongIndices: false);
		for (int i = 0; i < maxParticles; i++)
		{
			verticesBuffer[i * 4].UV = new Vector2(0f, 0f);
			verticesBuffer[i * 4 + 1].UV = new Vector2(1f, 0f);
			verticesBuffer[i * 4 + 2].UV = new Vector2(0f, 1f);
			verticesBuffer[i * 4 + 3].UV = new Vector2(1f, 1f);
			indicesBuffer[i * 6] = (short)(i * 4);
			indicesBuffer[i * 6 + 1] = (short)(i * 4 + 1);
			indicesBuffer[i * 6 + 2] = (short)(i * 4 + 2);
			indicesBuffer[i * 6 + 3] = (short)(i * 4 + 2);
			indicesBuffer[i * 6 + 4] = (short)(i * 4 + 1);
			indicesBuffer[i * 6 + 5] = (short)(i * 4 + 3);
		}
	}

	public void InitMesh(int maxParticleCount)
	{
		initMesh(Math.Min(16383, maxParticleCount));
	}
}
