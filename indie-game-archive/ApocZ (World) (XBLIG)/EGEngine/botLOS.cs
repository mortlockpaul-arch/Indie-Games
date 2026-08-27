using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class botLOS
{
	public const int MAX_BOTS = 512;

	public const float MODEL_VIEWDISTANCE = 2000f;

	public const float MAX_UPDATEDISTANCE = 4000f;

	public const float MAX_VIEWDISTANCE = 10000f;

	private static Vector3[] coords = new Vector3[4]
	{
		new Vector3(-0.5f, -0.5f, 0f),
		new Vector3(0.5f, -0.5f, 0f),
		new Vector3(-0.5f, 0.5f, 0f),
		new Vector3(0.5f, 0.5f, 0f)
	};

	private static Vector2[][] TextureCoords;

	public static Effect ShaderEffect;

	private static Texture2D AllZombieLOS;

	private int botCount;

	public int[] VertexBuffCount = new int[2];

	public VERT_BOTLOS[][] VertexBuff = new VERT_BOTLOS[2][];

	private static Matrix matBillBoard = Matrix.Identity;

	private static Matrix view = Matrix.Identity;

	private static Matrix proj = Matrix.Identity;

	private static Matrix tmpRotate = Matrix.Identity;

	private static Vector3 pos = Vector3.Zero;

	private static Vector3 dir = Vector3.Zero;

	private static Vector3 lookAt = Vector3.Zero;

	private static Vector3 axis = new Vector3(0f, 0f, 1f);

	private static Vector3 billPos = new Vector3(0f, 0f, 0f);

	private static Vector3 curScale = new Vector3(1000f, 1000f, 1000f);

	private static Vector3 a;

	private static Vector3 b;

	private static Vector3 c;

	private static Vector3 d;

	private static Vector3 partPos;

	private static float[] movementTimer = new float[5];

	private static float[] movementCos = new float[5];

	private static Vector3 right = Vector3.Zero;

	private static Vector3 vecTo = Vector3.Zero;

	private static Vector3 tmpScale = Vector3.Zero;

	private static BoundingSphere tmpSphere = default(BoundingSphere);

	public static Vector2 ViewSpaceDependantOffset = Vector2.Zero;

	public void Initialize()
	{
		for (int i = 0; i < 2; i++)
		{
			VertexBuffCount[i] = 0;
			VertexBuff[i] = new VERT_BOTLOS[3072];
		}
		TextureCoords = new Vector2[5][];
		int num = 0;
		float num2 = 0.2f;
		for (float num3 = 0f; num3 < 1f; num3 += num2)
		{
			TextureCoords[num] = new Vector2[4];
			TextureCoords[num][0].X = num3 + num2;
			TextureCoords[num][0].Y = 1f;
			TextureCoords[num][1].X = num3;
			TextureCoords[num][1].Y = 1f;
			TextureCoords[num][2].X = num3 + num2;
			TextureCoords[num][2].Y = 0f;
			TextureCoords[num][3].X = num3;
			TextureCoords[num][3].Y = 0f;
			num++;
		}
		AllZombieLOS = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\ZombieAllLOS");
		ShaderEffect = EndGameEngine.ContentMgr.Load<Effect>("shaders\\BotLOS");
		ShaderEffect.Parameters["BasicTexture"].SetValue(AllZombieLOS);
		for (int j = 0; j < 5; j++)
		{
			movementTimer[j] = (float)EndGameEngine.randGenerator.NextDouble() * 3.14f;
		}
	}

	public void Update(float gameTime, int qIndex)
	{
		VertexBuffCount[qIndex] = 0;
		for (int i = 0; i < 5; i++)
		{
			movementTimer[i] += 0.05f;
			movementCos[i] = (float)Math.Cos(movementTimer[i]) * 16f;
		}
	}

	public bool Add(PlayerBase playerRef, int qIndex, ZombieLODEntry e)
	{
		if (VertexBuffCount[qIndex] >= 512)
		{
			return false;
		}
		vecTo = e.pos;
		vecTo -= playerRef.vecPosition;
		float num = vecTo.LengthSquared();
		if (num > 100000000f)
		{
			return false;
		}
		if (num > 360000f && Vector3.Dot(playerRef.CameraDirection, vecTo) < 0f)
		{
			return false;
		}
		if (!playerRef.OverrideCamera)
		{
			pos = playerRef.vecPosition;
			dir = playerRef.CameraDirection;
		}
		else
		{
			pos = AIBase.camOverridePos;
			dir = AIBase.camOverrideDir;
		}
		partPos = e.pos;
		partPos.X -= playerRef.vecHeadPosition[qIndex].X;
		partPos.Z -= playerRef.vecHeadPosition[qIndex].Z;
		tmpSphere.Center = partPos;
		tmpSphere.Radius = 200f;
		ContainmentType result = ContainmentType.Disjoint;
		playerRef.bFrustum[qIndex].Contains(ref tmpSphere, out result);
		if (result == ContainmentType.Contains || result == ContainmentType.Intersects)
		{
			right.X = 0f - playerRef.mDataQueue[qIndex].view.M11;
			right.Y = 0f - playerRef.mDataQueue[qIndex].view.M21;
			right.Z = 0f - playerRef.mDataQueue[qIndex].view.M31;
			int num2 = 0xF & e.zFlags;
			a = right * -44f;
			b = right * 44f;
			c = right * -44f;
			d = right * 44f;
			c += right * movementCos[num2];
			d += right * movementCos[num2];
			c.Y += 145f;
			d.Y += 145f;
			a += partPos;
			b += partPos;
			c += partPos;
			d += partPos;
			int num3 = VertexBuffCount[qIndex] * 6;
			VertexBuff[qIndex][num3].pos = a;
			VertexBuff[qIndex][num3].tex.X = TextureCoords[num2][0].X;
			VertexBuff[qIndex][num3].tex.Y = TextureCoords[num2][0].Y;
			num3++;
			VertexBuff[qIndex][num3].pos = b;
			VertexBuff[qIndex][num3].tex.X = TextureCoords[num2][1].X;
			VertexBuff[qIndex][num3].tex.Y = TextureCoords[num2][1].Y;
			num3++;
			VertexBuff[qIndex][num3].pos = c;
			VertexBuff[qIndex][num3].tex.X = TextureCoords[num2][2].X;
			VertexBuff[qIndex][num3].tex.Y = TextureCoords[num2][2].Y;
			num3++;
			VertexBuff[qIndex][num3].pos = b;
			VertexBuff[qIndex][num3].tex.X = TextureCoords[num2][1].X;
			VertexBuff[qIndex][num3].tex.Y = TextureCoords[num2][1].Y;
			num3++;
			VertexBuff[qIndex][num3].pos = d;
			VertexBuff[qIndex][num3].tex.X = TextureCoords[num2][3].X;
			VertexBuff[qIndex][num3].tex.Y = TextureCoords[num2][3].Y;
			num3++;
			VertexBuff[qIndex][num3].pos = c;
			VertexBuff[qIndex][num3].tex.X = TextureCoords[num2][2].X;
			VertexBuff[qIndex][num3].tex.Y = TextureCoords[num2][2].Y;
			num3++;
			VertexBuffCount[qIndex]++;
			return true;
		}
		return false;
	}

	public void Draw(PlayerBase playerRef, int qIndex)
	{
		int num = VertexBuffCount[qIndex] * 2;
		if (num >= 2)
		{
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
			EndGameEngine.GraphicMgr.GraphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
			ShaderEffect.CurrentTechnique = ShaderEffect.Techniques["T_Basic"];
			ShaderEffect.Parameters["eyePosition"].SetValue(playerRef.mDataQueue[qIndex].cameraEyePos);
			ShaderEffect.Parameters["matViewProj"].SetValue(playerRef.mDataQueue[qIndex].view * playerRef.mDataQueue[qIndex].projection);
			ShaderEffect.CurrentTechnique.Passes[0].Apply();
			EndGameEngine.GraphicMgr.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, VertexBuff[qIndex], 0, num);
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
			EndGameEngine.GraphicMgr.GraphicsDevice.DepthStencilState = EndGameEngine.DepthEnabled;
		}
	}

	public void DrawPost(int qIndex, PlayerBase playerRef)
	{
	}
}
