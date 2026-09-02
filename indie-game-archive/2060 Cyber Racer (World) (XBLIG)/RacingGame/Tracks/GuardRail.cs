using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Graphics;
using RacingGame.Landscapes;
using RacingGame.Shaders;

namespace RacingGame.Tracks;

internal class GuardRail : IDisposable
{
	public enum Modes
	{
		Left,
		Right
	}

	private const float CorrectionScale = 0.0019f;

	private const float HolderGap = 15f;

	private const float GuardRailHeight = 0.860625f;

	public const float InsideRoadDistance = 0.5f;

	private readonly TangentVertex[] GuardRailVertices;

	private static readonly Vector3 HolderPileCorrectionVector;

	private TrackVertex[] railPoints;

	private TangentVertex[] railVertices;

	private VertexBuffer railVb;

	private IndexBuffer railIb;

	public GuardRail(List<TrackVertex> points, Modes mode, Landscape landscape)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0340: Unknown result type (might be due to invalid IL or missing references)
		//IL_0367: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0423: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0446: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0490: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04df: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_054c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_0574: Unknown result type (might be due to invalid IL or missing references)
		//IL_059b: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0608: Unknown result type (might be due to invalid IL or missing references)
		//IL_061c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0630: Unknown result type (might be due to invalid IL or missing references)
		//IL_0726: Unknown result type (might be due to invalid IL or missing references)
		//IL_0733: Unknown result type (might be due to invalid IL or missing references)
		//IL_073d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0742: Unknown result type (might be due to invalid IL or missing references)
		//IL_0747: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0702: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_079c: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_07b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_07be: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_084c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0851: Unknown result type (might be due to invalid IL or missing references)
		//IL_0853: Unknown result type (might be due to invalid IL or missing references)
		//IL_0855: Unknown result type (might be due to invalid IL or missing references)
		//IL_085c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0861: Unknown result type (might be due to invalid IL or missing references)
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a8c: Expected O, but got Unknown
		//IL_087d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0887: Unknown result type (might be due to invalid IL or missing references)
		//IL_088c: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0890: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Unknown result type (might be due to invalid IL or missing references)
		//IL_089a: Unknown result type (might be due to invalid IL or missing references)
		//IL_089f: Unknown result type (might be due to invalid IL or missing references)
		//IL_094e: Unknown result type (might be due to invalid IL or missing references)
		//IL_095c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b81: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b8b: Expected O, but got Unknown
		//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08db: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0902: Unknown result type (might be due to invalid IL or missing references)
		//IL_0917: Unknown result type (might be due to invalid IL or missing references)
		//IL_0919: Unknown result type (might be due to invalid IL or missing references)
		//IL_09a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_09b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_09eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_09f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_09fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a1f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a29: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a30: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a35: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a37: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a3c: Unknown result type (might be due to invalid IL or missing references)
		GuardRailVertices = new TangentVertex[17]
		{
			new TangentVertex(new Vector3(10f, 0f, -105f), new Vector2(0f, 0.557123f), new Vector3(-0.382683f, 0f, -0.92388f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(20f, 0f, -105f), new Vector2(0f, 0.567119f), new Vector3(0.92388f, 0f, -0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(-10f, 0f, -75f), new Vector2(0f, 0.597107f), new Vector3(0.92388f, 0f, 0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(-10f, 0f, -45f), new Vector2(0f, 0.627095f), new Vector3(0.92388f, 0f, -0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(20f, 0f, -15f), new Vector2(0f, 0.65708303f), new Vector3(0.92388f, 0f, -0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(20f, 0f, 15f), new Vector2(0f, 0.68707097f), new Vector3(0.92388f, 0f, 0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(-10f, 0f, 45f), new Vector2(0f, 0.717059f), new Vector3(0.92388f, 0f, 0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(-10f, 0f, 75f), new Vector2(0f, 0.747047f), new Vector3(0.92388f, 0f, -0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(20f, 0f, 105f), new Vector2(0f, 0.777035f), new Vector3(0.92388f, 0f, 0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(10f, 0f, 105f), new Vector2(0f, 0.787031f), new Vector3(-0.92388f, 0f, 0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(-20f, 0f, 75f), new Vector2(0f, 0.817019f), new Vector3(-0.92388f, 0f, 0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(-20f, 0f, 45f), new Vector2(0f, 0.84700704f), new Vector3(-0.92388f, 0f, -0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(10f, 0f, 15f), new Vector2(0f, 0.87699497f), new Vector3(-0.92388f, 0f, -0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(10f, 0f, -15f), new Vector2(0f, 0.906983f), new Vector3(-0.92388f, 0f, 0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(-20f, 0f, -45f), new Vector2(0f, 0.936971f), new Vector3(-0.92388f, 0f, 0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(-20f, 0f, -75f), new Vector2(0f, 0.966959f), new Vector3(-0.92388f, 0f, -0.382683f), new Vector3(0f, -1f, 0f)),
			new TangentVertex(new Vector3(10f, 0f, -105f), new Vector2(0f, 0.996947f), new Vector3(-0.382683f, 0f, -0.92388f), new Vector3(0f, -1f, 0f))
		};
		base._002Ector();
		railPoints = new TrackVertex[points.Count / 2 + 1];
		for (int i = 0; i < railPoints.Length; i++)
		{
			int num = i * 2;
			if (num >= points.Count - 1)
			{
				num = points.Count - 1;
			}
			if (mode == Modes.Left)
			{
				railPoints[i] = points[num].LeftTrackVertex;
				railPoints[i].right = -railPoints[i].right;
				railPoints[i].dir = -railPoints[i].dir;
				TrackVertex obj = railPoints[i];
				obj.pos -= railPoints[i].right * 0.5f;
			}
			else
			{
				railPoints[i] = points[num].RightTrackVertex;
				TrackVertex obj2 = railPoints[i];
				obj2.pos -= railPoints[i].right * 0.5f;
			}
		}
		railVertices = new TangentVertex[railPoints.Length * GuardRailVertices.Length];
		float num2 = 0.5f;
		float num3 = 0f;
		for (int j = 0; j < railPoints.Length; j++)
		{
			Vector3 right = railPoints[j].right;
			Vector3 dir = railPoints[j].dir;
			Vector3 up = railPoints[j].up;
			Matrix identity = Matrix.Identity;
			identity.M11 = right.X;
			identity.M12 = right.Y;
			identity.M13 = right.Z;
			identity.M21 = dir.X;
			identity.M22 = dir.Y;
			identity.M23 = dir.Z;
			identity.M31 = up.X;
			identity.M32 = up.Y;
			identity.M33 = up.Z;
			Vector3 pos = railPoints[j].pos;
			pos += up * 0.860625f;
			for (int k = 0; k < GuardRailVertices.Length; k++)
			{
				Vector3 setPos = Vector3.Transform(GuardRailVertices[k].pos * 0.0019f, identity * Matrix.CreateTranslation(pos));
				Vector3 setNormal = Vector3.TransformNormal((float)((mode != Modes.Left) ? 1 : (-1)) * GuardRailVertices[k].normal, identity);
				Vector3 setTangent = Vector3.TransformNormal(-GuardRailVertices[k].tangent, identity);
				ref TangentVertex reference = ref railVertices[j * GuardRailVertices.Length + k];
				reference = new TangentVertex(setPos, num2, GuardRailVertices[k].V, setNormal, setTangent);
			}
			float num4 = Vector3.Distance(railPoints[(j + 1) % railPoints.Length].pos, railPoints[j].pos);
			num2 += 1f / 15f * num4 * 2f;
			if (num3 - num4 <= 0f)
			{
				Vector3 pos2 = railPoints[(j - 1 < 0) ? (railPoints.Length - 1) : (j - 1)].pos;
				Vector3 pos3 = railPoints[j].pos;
				Vector3 pos4 = railPoints[(j + 1) % railPoints.Length].pos;
				Vector3 pos5 = railPoints[(j + 2) % railPoints.Length].pos;
				Vector3 val = Vector3.CatmullRom(pos2, pos3, pos4, pos5, num3 / num4);
				if (landscape != null && BaseGame.HighDetail)
				{
					landscape.AddObjectToRender("GuardRailHolder", Matrix.CreateScale(1.125f) * Matrix.CreateTranslation(HolderPileCorrectionVector) * identity * Matrix.CreateTranslation(val), isNearTrackForShadowGeneration: false);
				}
				num3 += 15f;
			}
			num3 -= num4;
		}
		railVb = new VertexBuffer(BaseGame.Device, typeof(TangentVertex), railVertices.Length, (BufferUsage)8);
		railVb.SetData<TangentVertex>(railVertices);
		int num5 = GuardRailVertices.Length - 1;
		int[] array = new int[6 * num5 * (railPoints.Length - 1)];
		int num6 = 0;
		int num7 = 0;
		for (int l = 0; l < railPoints.Length - 1; l++)
		{
			for (int m = 0; m < num5; m++)
			{
				num7 = 6 * (l * num5 + m);
				array[num7] = num6 + m;
				array[num7 + 1] = num6 + 1 + m;
				array[num7 + 2] = num6 + 1 + GuardRailVertices.Length + m;
				array[num7 + 3] = array[num7 + 2];
				array[num7 + 4] = num6 + GuardRailVertices.Length + m;
				array[num7 + 5] = array[num7];
			}
			num6 += GuardRailVertices.Length;
		}
		railIb = new IndexBuffer(BaseGame.Device, typeof(int), array.Length, (BufferUsage)8);
		railIb.SetData<int>(array);
	}

	public void Dispose()
	{
		((GraphicsResource)railVb).Dispose();
		((GraphicsResource)railIb).Dispose();
	}

	public void Render(Material guardRailMaterial)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		BaseGame.Device.VertexDeclaration = TangentVertex.VertexDeclaration;
		BaseGame.WorldMatrix = Matrix.Identity;
		ShaderEffect.normalMapping.Render(guardRailMaterial, "Specular20", RenderGuardRailVertices);
		BaseGame.WorldMatrix = Matrix.Identity;
	}

	private void RenderGuardRailVertices()
	{
		BaseGame.Device.Vertices[0].SetSource(railVb, 0, TangentVertex.SizeInBytes);
		BaseGame.Device.Indices = railIb;
		BaseGame.Device.DrawIndexedPrimitives((PrimitiveType)4, 0, 0, railVertices.Length, 0, (GuardRailVertices.Length - 1) * (railPoints.Length - 1) * 2);
	}

	public void GenerateShadow()
	{
		RenderGuardRailVertices();
	}

	public void UseShadow()
	{
		RenderGuardRailVertices();
	}

	static GuardRail()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		HolderPileCorrectionVector = new Vector3(0.225f, 0f, 0f);
	}
}
