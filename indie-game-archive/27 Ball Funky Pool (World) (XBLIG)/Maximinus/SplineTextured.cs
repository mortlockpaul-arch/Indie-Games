using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Maximinus;

public class SplineTextured
{
	private int detail = 20;

	private List<Vector3> extendedTrackPoints = new List<Vector3>();

	private Texture2D tex;

	private VertexPositionNormalTexture[] trackVertices;

	private GraphicsDevice device;

	private BasicEffect basicEffect;

	private VertexDeclaration myVertexDeclaration;

	private Vector3 upVector = Vector3.Up;

	private int verticesOffset;

	public SplineTextured(Texture2D tex, Drawing2D draw2D, Vector3 up, int detail)
		: this(tex, draw2D, up)
	{
		this.detail = detail;
	}

	public SplineTextured(Texture2D tex, Drawing2D draw2D, Vector3 upVector)
		: this(tex, draw2D)
	{
		this.upVector = upVector;
	}

	public SplineTextured(Texture2D tex, Drawing2D draw2D)
	{
		this.tex = tex;
		device = draw2D.Device;
		basicEffect = new BasicEffect(device);
		myVertexDeclaration = new VertexDeclaration(VertexPositionNormalTexture.VertexDeclaration.GetVertexElements());
	}

	public void UpdateBasePoints(List<Vector3> basePoints)
	{
		extendedTrackPoints = GenerateTrackPoints(basePoints);
		trackVertices = GenerateTrackVertices(extendedTrackPoints);
		verticesOffset = 0;
	}

	private List<Vector3> GenerateTrackPoints(List<Vector3> basePoints)
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 1; i < basePoints.Count - 2; i++)
		{
			List<Vector3> collection = InterpolateCR(basePoints[i - 1], basePoints[i], basePoints[i + 1], basePoints[i + 2]);
			list.AddRange(collection);
		}
		return list;
	}

	private List<Vector3> InterpolateCR(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
	{
		List<Vector3> list = new List<Vector3>();
		for (int i = 0; i < detail; i++)
		{
			Vector3 item = Utils.CatmullRom3D(v1, v2, v3, v4, (float)i / (float)detail);
			list.Add(item);
		}
		return list;
	}

	private VertexPositionNormalTexture[] GenerateTrackVertices(List<Vector3> basePoints)
	{
		float num = 0.2f;
		float num2 = 0.5f;
		float num3 = 0f;
		List<VertexPositionNormalTexture> list = new List<VertexPositionNormalTexture>();
		for (int i = 1; i < basePoints.Count - 1; i++)
		{
			Vector3 vector = basePoints[i + 1] - basePoints[i];
			Vector3 vector2 = Vector3.Cross(upVector, vector);
			vector2.Normalize();
			Vector3 position = basePoints[i] + vector2 * num;
			Vector3 position2 = basePoints[i] - vector2 * num;
			VertexPositionNormalTexture item = new VertexPositionNormalTexture(position2, upVector, new Vector2(0f, num3 / num2));
			list.Add(item);
			item = new VertexPositionNormalTexture(position, upVector, new Vector2(1f, num3 / num2));
			list.Add(item);
			num3 += vector.Length();
		}
		return list.ToArray();
	}

	public void Draw(Matrix viewMat, Matrix projMat)
	{
		Draw(viewMat, projMat, Color.White);
	}

	public void Draw(Matrix viewMat, Matrix projMat, Color color)
	{
		basicEffect.World = Matrix.Identity;
		basicEffect.View = viewMat;
		basicEffect.Projection = projMat;
		basicEffect.Texture = tex;
		basicEffect.TextureEnabled = true;
		basicEffect.VertexColorEnabled = false;
		basicEffect.AmbientLightColor = color.ToVector3();
		basicEffect.LightingEnabled = true;
		foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
		{
			pass.Apply();
			int primitiveCount = trackVertices.Length - 2 - verticesOffset;
			device.DrawUserPrimitives(PrimitiveType.TriangleStrip, trackVertices, verticesOffset, primitiveCount);
		}
	}

	public void UpdateBallTrajectory(bool discard, Vector2 ballPos, float radius, int launchToGhostSignX)
	{
		if (discard)
		{
			verticesOffset = trackVertices.Length - 3;
			return;
		}
		int num = -2;
		bool flag = false;
		do
		{
			num += 2;
			Vector2 vector = new Vector2(trackVertices[num].Position.X, trackVertices[num].Position.Y);
			Vector2 vector2 = new Vector2(trackVertices[num + 1].Position.X, trackVertices[num + 1].Position.Y);
			Vector2 value = (vector + vector2) / 2f;
			flag = Vector2.Distance(value, ballPos) < radius || Math.Sign(value.X - ballPos.X) == launchToGhostSignX;
		}
		while (num < trackVertices.Length - 3 && !flag);
		if (flag)
		{
			verticesOffset = Math.Min(num, trackVertices.Length - 3);
		}
	}
}
