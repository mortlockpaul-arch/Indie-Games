using System;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class StickersClass
{
	public struct Sticker_Struct
	{
		public bool active;

		public float disSqr;

		public Vector3 center;

		public Vector3[] position;

		public Vector2[] textureCoords;

		public Vector3 normal;
	}

	public struct StickerQueue_Struct
	{
		public bool active;

		public float scale;

		public Vector3 position;

		public Vector3 normal;

		public MaterialType material;
	}

	public const int MAX_STICKERS = 64;

	private static Vector3[] coords = new Vector3[4]
	{
		new Vector3(-1f, -1f, 0f),
		new Vector3(1f, -1f, 0f),
		new Vector3(-1f, 1f, 0f),
		new Vector3(1f, 1f, 0f)
	};

	private static Vector2[,] uvs = new Vector2[4, 4]
	{
		{
			new Vector2(0.5f, 0.5f),
			new Vector2(0f, 0.5f),
			new Vector2(0.5f, 0f),
			new Vector2(0f, 0f)
		},
		{
			new Vector2(1f, 0.5f),
			new Vector2(0.5f, 0.5f),
			new Vector2(1f, 0f),
			new Vector2(0.5f, 0f)
		},
		{
			new Vector2(0.5f, 0.5f),
			new Vector2(0f, 0.5f),
			new Vector2(0.5f, 0.5f),
			new Vector2(0f, 0.5f)
		},
		{
			new Vector2(1f, 0.5f),
			new Vector2(0.5f, 0.5f),
			new Vector2(1f, 0.5f),
			new Vector2(0.5f, 0.5f)
		}
	};

	private static int NumStickers;

	private static int InsertIndex;

	private static Sticker_Struct[] Stickers = new Sticker_Struct[64];

	private Texture2D StickerTexture;

	private VertexBuffer VertBuff;

	private int[] TmpBuffCount = new int[2];

	private VERT_STICKERS[][] TmpBuff = new VERT_STICKERS[2][];

	private static int NumStickerInQueue = 0;

	private static StickerQueue_Struct[] StickerQueue = new StickerQueue_Struct[64];

	private static Random stickerRand = new Random(1);

	private static Matrix tmpMat;

	private static Sticker_Struct tmpSticker;

	public void Initialize()
	{
		NumStickers = 0;
		InsertIndex = 0;
		StickerTexture = EndGameEngine.GameAssetMgr.Load<Texture2D>("textures\\particles\\stickers");
		EndGameEngine.MaterialParams.stickersTexture.SetValue(StickerTexture);
		VertBuff = new VertexBuffer(EndGameEngine.GraphicMgr.GraphicsDevice, VERT_STICKERS.VertexDeclaration, 384, BufferUsage.None);
		Stickers = new Sticker_Struct[64];
		for (int i = 0; i < 2; i++)
		{
			TmpBuffCount[i] = 0;
			TmpBuff[i] = new VERT_STICKERS[384];
		}
		for (int j = 0; j < 64; j++)
		{
			Stickers[j] = default(Sticker_Struct);
			Stickers[j].active = false;
			Stickers[j].disSqr = 1E+10f;
			Stickers[j].center = Vector3.Zero;
			Stickers[j].position = new Vector3[4];
			Stickers[j].textureCoords = new Vector2[4];
		}
		for (int k = 0; k < 64; k++)
		{
			StickerQueue[k] = default(StickerQueue_Struct);
			StickerQueue[k].active = false;
		}
		for (int l = 0; l < 64; l++)
		{
			ref Vector3 reference = ref Stickers[l].position[0];
			reference = Vector3.Zero;
			ref Vector3 reference2 = ref Stickers[l].position[1];
			reference2 = Vector3.Zero;
			ref Vector3 reference3 = ref Stickers[l].position[2];
			reference3 = Vector3.Zero;
			ref Vector3 reference4 = ref Stickers[l].position[3];
			reference4 = Vector3.Zero;
		}
	}

	public void Spawn(ref StickerQueue_Struct sticker)
	{
		if (NumStickerInQueue < 63)
		{
			ref StickerQueue_Struct reference = ref StickerQueue[NumStickerInQueue];
			reference = sticker;
			NumStickerInQueue++;
		}
	}

	public void Update(Vector3 playerPos, float gameTime, int qIndex)
	{
		if (NumStickerInQueue > 0)
		{
			for (int num = NumStickerInQueue - 1; num >= 0; num--)
			{
				bool flag = false;
				int num2 = InsertIndex;
				Vector3 position = StickerQueue[num].position;
				for (int i = 0; i < 64; i++)
				{
					float num3 = (Stickers[i].position[0] - position).LengthSquared();
					if (num3 < 400f)
					{
						flag = true;
						num2 = i;
						break;
					}
				}
				if (flag)
				{
					StickerQueue[num].active = false;
				}
				else
				{
					tmpMat = Matrix.Identity;
					tmpMat.Forward = StickerQueue[num].normal;
					if (StickerQueue[num].normal.Y < -0.5f || StickerQueue[num].normal.Y > 0.5f)
					{
						tmpMat.Right = Vector3.Cross(tmpMat.Forward, Vector3.UnitZ);
					}
					else
					{
						tmpMat.Right = Vector3.Cross(tmpMat.Forward, Vector3.UnitY);
					}
					tmpMat.Up = Vector3.Cross(tmpMat.Forward, tmpMat.Right);
					tmpMat.Right = Vector3.Cross(tmpMat.Forward, tmpMat.Up);
					tmpMat *= Matrix.CreateFromAxisAngle(StickerQueue[num].normal, (float)stickerRand.NextDouble() * 3.14f);
					StickerQueue[num].scale = (StickerQueue[num].scale + StickerQueue[num].scale * 0.1f * (float)stickerRand.NextDouble()) * 0.5f;
					ref Vector3 reference = ref Stickers[num2].position[0];
					reference = Vector3.Transform(coords[0] * StickerQueue[num].scale, tmpMat);
					ref Vector3 reference2 = ref Stickers[num2].position[1];
					reference2 = Vector3.Transform(coords[1] * StickerQueue[num].scale, tmpMat);
					ref Vector3 reference3 = ref Stickers[num2].position[2];
					reference3 = Vector3.Transform(coords[2] * StickerQueue[num].scale, tmpMat);
					ref Vector3 reference4 = ref Stickers[num2].position[3];
					reference4 = Vector3.Transform(coords[3] * StickerQueue[num].scale, tmpMat);
					Stickers[num2].center = position;
					Stickers[num2].position[0] += position;
					Stickers[num2].position[1] += position;
					Stickers[num2].position[2] += position;
					Stickers[num2].position[3] += position;
					Stickers[num2].normal = StickerQueue[num].normal;
					int num4 = 0;
					if (StickerQueue[num].material == MaterialType.Metal)
					{
						num4 = 1;
					}
					ref Vector2 reference5 = ref Stickers[num2].textureCoords[0];
					reference5 = uvs[num4, 0];
					ref Vector2 reference6 = ref Stickers[num2].textureCoords[1];
					reference6 = uvs[num4, 1];
					ref Vector2 reference7 = ref Stickers[num2].textureCoords[2];
					reference7 = uvs[num4, 2];
					ref Vector2 reference8 = ref Stickers[num2].textureCoords[3];
					reference8 = uvs[num4, 3];
					StickerQueue[num].active = false;
					if (!flag)
					{
						InsertIndex++;
						if (InsertIndex >= 64)
						{
							InsertIndex = 0;
						}
						if (NumStickers < 63)
						{
							NumStickers++;
						}
					}
				}
			}
			NumStickerInQueue = 0;
		}
		for (int j = 0; j < NumStickers; j++)
		{
			Stickers[j].disSqr = (Stickers[j].center - playerPos).LengthSquared();
		}
		for (int k = 0; k < NumStickers; k++)
		{
			for (int l = k + 1; l < NumStickers; l++)
			{
				if (Stickers[l].disSqr > Stickers[k].disSqr)
				{
					tmpSticker = Stickers[k];
					ref Sticker_Struct reference9 = ref Stickers[k];
					reference9 = Stickers[l];
					ref Sticker_Struct reference10 = ref Stickers[l];
					reference10 = tmpSticker;
				}
			}
		}
		int num5 = 0;
		TmpBuffCount[qIndex] = 0;
		for (int m = 0; m < NumStickers; m++)
		{
			TmpBuff[qIndex][num5].pos = Stickers[m].position[0];
			TmpBuff[qIndex][num5].norm = Stickers[m].normal;
			TmpBuff[qIndex][num5++].texCoord = Stickers[m].textureCoords[0];
			TmpBuff[qIndex][num5].pos = Stickers[m].position[1];
			TmpBuff[qIndex][num5].norm = Stickers[m].normal;
			TmpBuff[qIndex][num5++].texCoord = Stickers[m].textureCoords[1];
			TmpBuff[qIndex][num5].pos = Stickers[m].position[2];
			TmpBuff[qIndex][num5].norm = Stickers[m].normal;
			TmpBuff[qIndex][num5++].texCoord = Stickers[m].textureCoords[2];
			TmpBuff[qIndex][num5].pos = Stickers[m].position[1];
			TmpBuff[qIndex][num5].norm = Stickers[m].normal;
			TmpBuff[qIndex][num5++].texCoord = Stickers[m].textureCoords[1];
			TmpBuff[qIndex][num5].pos = Stickers[m].position[3];
			TmpBuff[qIndex][num5].norm = Stickers[m].normal;
			TmpBuff[qIndex][num5++].texCoord = Stickers[m].textureCoords[3];
			TmpBuff[qIndex][num5].pos = Stickers[m].position[2];
			TmpBuff[qIndex][num5].norm = Stickers[m].normal;
			TmpBuff[qIndex][num5++].texCoord = Stickers[m].textureCoords[2];
			TmpBuffCount[qIndex]++;
		}
	}

	public void Draw(PlayerBase playerRef, int qIndex)
	{
		Matrix view = playerRef.mDataQueue[qIndex].view;
		Matrix projection = playerRef.mDataQueue[qIndex].projection;
		_ = playerRef.mDataQueue[qIndex].cameraPos;
		_ = playerRef.mDataQueue[qIndex].cameraDirN;
		int num = TmpBuffCount[qIndex] * 2;
		if (num >= 2)
		{
			EndGameEngine.MaterialParams.Texture7.SetValue(LevelBaseMenu.DepthRenderTarget);
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = EndGameEngine.BlendStickers;
			EndGameEngine.GraphicMgr.GraphicsDevice.DepthStencilState = EndGameEngine.DepthDisabled;
			EndGameEngine.GraphicMgr.GraphicsDevice.RasterizerState = EndGameEngine.RasterCullCC;
			EndGameEngine.MaterialEffect.CurrentTechnique = EndGameEngine.MaterialParams.T_DrawStickers;
			EndGameEngine.MaterialParams.matViewProj.SetValue(view * projection);
			EndGameEngine.MaterialParams.uvDisplacement.SetValue(new Vector4(1f, 1f, 0f, 0f));
			EndGameEngine.MaterialEffect.CurrentTechnique.Passes[0].Apply();
			EndGameEngine.GraphicMgr.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, TmpBuff[qIndex], 0, num);
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
			EndGameEngine.GraphicMgr.GraphicsDevice.DepthStencilState = EndGameEngine.DepthEnabled;
		}
	}
}
