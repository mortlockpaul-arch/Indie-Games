using System;
using DataContent;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGEngine;

public class particles
{
	public class Particle_Struct
	{
		public uint flag;

		public float life;

		public float delay;

		public float alpha;

		public float alphaStep;

		public float disSqr;

		public Vector3 scale;

		public Vector3 position;

		public Vector3 velocity;

		public float velocityScale;

		public float sizeScale;

		public float gravity;

		public float rotation;

		public float rotationVelocity;

		public Color diffuse;

		public bool distortion;

		public float uvDistortion;

		public byte textureOffset;

		public byte textureAnimationStartIndex;

		public byte textureAnimationEndIndex;

		public byte softParticle;
	}

	public class Trail_Struct
	{
		public uint flag;

		public float life;

		public Vector3 scale;

		public Vector3 position;

		public Vector3 velocity;
	}

	public const int MAX_TRAILS = 64;

	public const int MAX_PARTICLES = 512;

	public const float TRACER_BULLET_VELOCITY = 400f;

	public const int RENDER_PASS_NORMAL = 1;

	public const int RENDER_PASS_SHADOW = 2;

	public const int RENDER_PASS_REFLECT = 3;

	public const int RENDER_PASS_LOADVERTS = 4;

	public const int RENDER_PASS_POST = 5;

	public const uint FL_HALFLIFE = 1u;

	public const uint FL_TRAIL = 2u;

	public const uint FL_FPS = 4u;

	public const uint FL_NO_PITCH = 8u;

	public const uint FL_ANIMATED = 16u;

	public const uint FL_ANIMATED_LOOPING = 32u;

	public const uint FL_INVERSE_DELAY = 64u;

	public const uint FL_TRACER = 128u;

	public const uint FL_LASER_LIGHT = 256u;

	public const uint FL_SPARK_LIGHT = 512u;

	public const uint FL_LIGHT_PARTICLE = 1024u;

	public const uint FL_TRACER_DISTORTION = 2048u;

	public const uint FL_TRAIL_LASER = 4096u;

	public const uint FL_SPAWN_TRAIL = 8192u;

	public const uint FL_SMOKE_TRAIL = 16384u;

	private static Vector3[] coords = new Vector3[4]
	{
		new Vector3(-0.5f, -0.5f, 0f),
		new Vector3(0.5f, -0.5f, 0f),
		new Vector3(-0.5f, 0.5f, 0f),
		new Vector3(0.5f, 0.5f, 0f)
	};

	private static int m_MaxPlayers = 4;

	private static int m_NumParticles;

	private static Particle_Struct[] m_Particles;

	private Texture2D GrenadeAnimTex;

	private Texture2D m_ParticleTexture;

	private int[] m_TmpBuffCount = new int[2];

	private VERT_PARTICLE[][][] m_TmpBuff = new VERT_PARTICLE[2][][];

	private int[] m_BuffCountPost = new int[2];

	private VERT_PARTICLE[][][] m_BuffPost = new VERT_PARTICLE[2][][];

	private static int m_NumTrails = 0;

	private static Trail_Struct[] m_Trails = new Trail_Struct[64];

	private static Vector2[][] TextureCoords;

	private static int MaxRandValues = 256;

	private static int rFloatIndx = 0;

	private static int rIntIndx = 0;

	private static float[] RandFloat = new float[MaxRandValues];

	private static int[] RandInt = new int[MaxRandValues];

	private static Random Rand = new Random(5);

	private static int muzzleFlashIndex = 0;

	private static Vector4[] vecMuzzleFlash = new Vector4[4];

	private static bool ParticlesInitialized = false;

	private static Particle_Struct tmpPartSwap;

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

	private static Vector3 zBias;

	private static Vector3 partPos;

	private static int AnimatedLoopIndex = 0;

	private static Vector3 tmpScale = Vector3.Zero;

	public static Vector2 ViewSpaceDependantOffset = Vector2.Zero;

	private static Particle_Struct tmpPart = new Particle_Struct();

	private static Vector3 tmpPos = Vector3.Zero;

	private static float NextRandFloat()
	{
		rFloatIndx = ((rFloatIndx + 1 < MaxRandValues) ? (rFloatIndx + 1) : 0);
		return RandFloat[rFloatIndx];
	}

	private static int NextRandInt()
	{
		rIntIndx = ((rIntIndx + 1 < MaxRandValues) ? (rIntIndx + 1) : 0);
		return RandInt[rIntIndx];
	}

	private static int NextRandInt(int max)
	{
		float num = NextRandFloat() * (float)max;
		return (int)num;
	}

	private static int NextRandInt(int min, int max)
	{
		return Rand.Next(min, max);
	}

	public static void AddMuzzleFlash(ref Vector3 pos, float timeValue)
	{
		muzzleFlashIndex = ((muzzleFlashIndex + 1 < 4) ? (muzzleFlashIndex + 1) : 0);
		muzzleFlashIndex = 0;
		vecMuzzleFlash[muzzleFlashIndex].W = 0f;
		vecMuzzleFlash[muzzleFlashIndex].X = pos.X;
		vecMuzzleFlash[muzzleFlashIndex].Y = pos.Y;
		vecMuzzleFlash[muzzleFlashIndex].Z = pos.Z;
		vecMuzzleFlash[muzzleFlashIndex].W = timeValue;
	}

	public static void UpdateMuzzleFlash(float eTime)
	{
		eTime *= 2f;
		vecMuzzleFlash[0].W = ((vecMuzzleFlash[0].W - eTime < 0f) ? 0f : (vecMuzzleFlash[0].W - eTime));
		vecMuzzleFlash[1].W = ((vecMuzzleFlash[1].W - eTime < 0f) ? 0f : (vecMuzzleFlash[1].W - eTime));
		vecMuzzleFlash[2].W = ((vecMuzzleFlash[2].W - eTime < 0f) ? 0f : (vecMuzzleFlash[2].W - eTime));
		vecMuzzleFlash[3].W = ((vecMuzzleFlash[3].W - eTime < 0f) ? 0f : (vecMuzzleFlash[3].W - eTime));
	}

	public static Vector4[] MuzzleFlash()
	{
		return vecMuzzleFlash;
	}

	public void Initialize()
	{
		m_MaxPlayers = 4;
		m_NumParticles = 512;
		GrenadeAnimTex = EndGameEngine.ContentMgr.Load<Texture2D>("textures\\Grenade_Sequence");
		EndGameEngine.MaterialParams.TexParticle0.SetValue(GrenadeAnimTex);
		TextureCoords = new Vector2[128][];
		int num = 0;
		float num2 = 0.125f;
		float num3 = 0.0625f;
		for (float num4 = 0f; num4 < 1f; num4 += num3)
		{
			for (float num5 = 0f; num5 < 1f; num5 += num2)
			{
				TextureCoords[num] = new Vector2[4];
				TextureCoords[num][0].X = num5 + num2;
				TextureCoords[num][0].Y = num4;
				TextureCoords[num][1].X = num5;
				TextureCoords[num][1].Y = num4;
				TextureCoords[num][2].X = num5 + num2;
				TextureCoords[num][2].Y = num4 + num3;
				TextureCoords[num][3].X = num5;
				TextureCoords[num][3].Y = num4 + num3;
				num++;
			}
		}
		m_Particles = new Particle_Struct[512];
		for (int i = 0; i < 2; i++)
		{
			m_TmpBuffCount[i] = 0;
			m_TmpBuff[i] = new VERT_PARTICLE[m_MaxPlayers][];
			for (int j = 0; j < m_MaxPlayers; j++)
			{
				m_TmpBuff[i][j] = new VERT_PARTICLE[3072];
			}
		}
		for (int k = 0; k < 2; k++)
		{
			m_BuffCountPost[k] = 0;
			m_BuffPost[k] = new VERT_PARTICLE[m_MaxPlayers][];
			for (int l = 0; l < m_MaxPlayers; l++)
			{
				m_BuffPost[k][l] = new VERT_PARTICLE[3072];
			}
		}
		for (int m = 0; m < 512; m++)
		{
			m_Particles[m] = new Particle_Struct();
			m_Particles[m].life = 0f;
			m_Particles[m].delay = 0f;
			m_Particles[m].alpha = 0f;
			m_Particles[m].position = Vector3.Zero;
			m_Particles[m].distortion = false;
			m_Particles[m].uvDistortion = 0f;
			m_Particles[m].textureOffset = 0;
			m_Particles[m].textureAnimationStartIndex = 0;
			m_Particles[m].textureAnimationEndIndex = 0;
			m_Particles[m].softParticle = 204;
			m_Particles[m].disSqr = 0f;
			m_Particles[m].scale = Vector3.UnitX;
			m_Particles[m].velocity = Vector3.Zero;
			m_Particles[m].sizeScale = 1f;
			m_Particles[m].velocityScale = 1f;
			m_Particles[m].gravity = 0f;
			m_Particles[m].rotation = 0f;
			m_Particles[m].rotationVelocity = 0f;
		}
		Random random = new Random(3);
		for (int n = 0; n < MaxRandValues; n++)
		{
			RandInt[n] = random.Next(0, 100);
			RandFloat[n] = (float)random.NextDouble();
		}
		ParticlesInitialized = true;
	}

	public static void Spawn(Particle_Struct part)
	{
		if (m_NumParticles < 511)
		{
			m_Particles[m_NumParticles].flag = part.flag;
			m_Particles[m_NumParticles].distortion = part.distortion;
			m_Particles[m_NumParticles].uvDistortion = part.uvDistortion;
			m_Particles[m_NumParticles].alpha = part.alpha;
			m_Particles[m_NumParticles].delay = part.delay;
			m_Particles[m_NumParticles].alphaStep = part.alphaStep;
			m_Particles[m_NumParticles].diffuse = part.diffuse;
			m_Particles[m_NumParticles].disSqr = part.disSqr;
			m_Particles[m_NumParticles].life = part.life;
			m_Particles[m_NumParticles].position = part.position;
			m_Particles[m_NumParticles].scale = part.scale;
			m_Particles[m_NumParticles].textureOffset = part.textureOffset;
			m_Particles[m_NumParticles].textureAnimationStartIndex = part.textureAnimationStartIndex;
			m_Particles[m_NumParticles].textureAnimationEndIndex = part.textureAnimationEndIndex;
			m_Particles[m_NumParticles].softParticle = part.softParticle;
			m_Particles[m_NumParticles].velocity = part.velocity;
			m_Particles[m_NumParticles].sizeScale = part.sizeScale;
			m_Particles[m_NumParticles].velocityScale = part.velocityScale;
			m_Particles[m_NumParticles].gravity = part.gravity;
			m_Particles[m_NumParticles].rotation = part.rotation;
			m_Particles[m_NumParticles].rotationVelocity = part.rotationVelocity;
			m_NumParticles++;
		}
	}

	public static void CancelParticles()
	{
		m_NumTrails = 0;
		m_NumParticles = 0;
	}

	public void Update(float gameTime, int qIndex)
	{
		gameTime = EndGameEngine.fFIXED_TIME_STEP;
		UpdateMuzzleFlash(gameTime);
		for (int i = 0; i < m_NumTrails; i++)
		{
			m_Trails[i].life -= gameTime;
			if (m_Trails[i].life <= 0f)
			{
				m_Trails[i].flag = m_Trails[m_NumTrails - 1].flag;
				m_Trails[i].life = m_Trails[m_NumTrails - 1].life;
				m_Trails[i].position = m_Trails[m_NumTrails - 1].position;
				m_Trails[i].velocity = m_Trails[m_NumTrails - 1].velocity;
				m_Trails[i].scale = m_Trails[m_NumTrails - 1].scale;
				m_NumTrails--;
				if (m_NumTrails < 0)
				{
					m_NumTrails = 0;
				}
				i--;
				continue;
			}
			float num = m_Trails[i].life;
			if (num > 1f)
			{
				num = 1f;
			}
			if (num < 0.25f)
			{
				num = 0.25f;
			}
			SpawnTrialParticle(m_Trails[i].position, m_Trails[i].velocity, num);
			m_Trails[i].position += m_Trails[i].velocity * gameTime * 180f;
			m_Trails[i].velocity -= m_Trails[i].velocity * gameTime;
			m_Trails[i].velocity.Y -= 4f * gameTime;
		}
		m_TmpBuffCount[qIndex] = 0;
		m_BuffCountPost[qIndex] = 0;
		if (m_NumParticles < 0)
		{
			m_NumParticles = 0;
		}
		AnimatedLoopIndex = 0;
		for (int j = 0; j < m_NumParticles; j++)
		{
			if ((m_Particles[j].flag & 0x40) < 1)
			{
				m_Particles[j].delay -= gameTime;
				if (m_Particles[j].delay > 0f)
				{
					continue;
				}
			}
			if ((m_Particles[j].flag & 0x20) == 0)
			{
				m_Particles[j].life -= gameTime;
			}
			if (m_Particles[j].life <= 0f)
			{
				m_Particles[j].flag = m_Particles[m_NumParticles - 1].flag;
				m_Particles[j].distortion = m_Particles[m_NumParticles - 1].distortion;
				m_Particles[j].uvDistortion = m_Particles[m_NumParticles - 1].uvDistortion;
				m_Particles[j].delay = m_Particles[m_NumParticles - 1].delay;
				m_Particles[j].alpha = m_Particles[m_NumParticles - 1].alpha;
				m_Particles[j].alphaStep = m_Particles[m_NumParticles - 1].alphaStep;
				m_Particles[j].diffuse = m_Particles[m_NumParticles - 1].diffuse;
				m_Particles[j].life = m_Particles[m_NumParticles - 1].life;
				m_Particles[j].position = m_Particles[m_NumParticles - 1].position;
				m_Particles[j].scale = m_Particles[m_NumParticles - 1].scale;
				m_Particles[j].textureOffset = m_Particles[m_NumParticles - 1].textureOffset;
				m_Particles[j].textureAnimationStartIndex = m_Particles[m_NumParticles - 1].textureAnimationStartIndex;
				m_Particles[j].textureAnimationEndIndex = m_Particles[m_NumParticles - 1].textureAnimationEndIndex;
				m_Particles[j].softParticle = m_Particles[m_NumParticles - 1].softParticle;
				m_Particles[j].velocity = m_Particles[m_NumParticles - 1].velocity;
				m_Particles[j].sizeScale = m_Particles[m_NumParticles - 1].sizeScale;
				m_Particles[j].velocityScale = m_Particles[m_NumParticles - 1].velocityScale;
				m_Particles[j].gravity = m_Particles[m_NumParticles - 1].gravity;
				m_Particles[j].rotation = m_Particles[m_NumParticles - 1].rotation;
				m_Particles[j].rotationVelocity = m_Particles[m_NumParticles - 1].rotationVelocity;
				m_NumParticles--;
				if (m_NumParticles < 0)
				{
					m_NumParticles = 0;
				}
				j--;
				continue;
			}
			if ((m_Particles[j].flag & 0x10) != 0)
			{
				m_Particles[j].textureOffset++;
				if (m_Particles[j].textureOffset > m_Particles[j].textureAnimationEndIndex)
				{
					m_Particles[j].textureOffset = m_Particles[j].textureAnimationEndIndex;
				}
			}
			else if ((m_Particles[j].flag & 0x20) != 0)
			{
				if (AnimatedLoopIndex == 0)
				{
					m_Particles[j].textureOffset++;
					if (m_Particles[j].textureOffset > m_Particles[j].textureAnimationEndIndex)
					{
						m_Particles[j].textureOffset = m_Particles[j].textureAnimationStartIndex;
					}
				}
			}
			else if ((m_Particles[j].flag & 1) != 0 && m_Particles[j].life < 3f)
			{
				m_Particles[j].alpha += m_Particles[j].alphaStep * gameTime;
				m_Particles[j].alpha = ((m_Particles[j].alpha > 1f) ? 1f : m_Particles[j].alpha);
			}
			else if ((m_Particles[j].flag & 0x40) != 0)
			{
				if (m_Particles[j].life > m_Particles[j].delay)
				{
					m_Particles[j].alpha -= m_Particles[j].alphaStep * gameTime;
					m_Particles[j].alpha = ((m_Particles[j].alpha < 0f) ? 0f : m_Particles[j].alpha);
				}
			}
			else
			{
				m_Particles[j].alpha -= m_Particles[j].alphaStep * gameTime;
				m_Particles[j].alpha = ((m_Particles[j].alpha < 0f) ? 0f : m_Particles[j].alpha);
			}
			if ((m_Particles[j].flag & 0x200) != 0)
			{
				Color color = Color.LightYellow;
				LevelBaseMenu.PointLights.AddDynamicPointLight(ref m_Particles[j].position, ref color, 50f, 2000f, qIndex);
			}
			if ((m_Particles[j].flag & 0x400) != 0)
			{
				LevelBaseMenu.PointLights.AddDynamicPointLight(ref m_Particles[j].position, ref m_Particles[j].diffuse, m_Particles[j].alpha * m_Particles[j].scale.X, 2000f, qIndex);
			}
			if ((m_Particles[j].flag & 0x2000) != 0)
			{
				Vector3 vel = Vector3.Normalize(m_Particles[j].velocity);
				SpawnRPGTrial(ref m_Particles[j].position, ref vel);
			}
			if ((m_Particles[j].flag & 0x80) != 0 || (m_Particles[j].flag & 0x100) != 0 || (m_Particles[j].flag & 0x800) != 0)
			{
				if ((m_Particles[j].flag & 0x100) != 0)
				{
					m_Particles[j].position += m_Particles[j].velocity * 100f * gameTime * 80f;
					LevelBaseMenu.PointLights.AddDynamicPointLight(ref m_Particles[j].position, ref m_Particles[j].diffuse, 1000f, 0.025f, qIndex);
					continue;
				}
				if ((m_Particles[j].flag & 4) != 0)
				{
					m_Particles[j].position += m_Particles[j].velocity * 100f * gameTime * 80f;
					continue;
				}
				m_Particles[j].velocityScale -= 400f;
				if (m_Particles[j].velocityScale <= 0f)
				{
					m_Particles[j].life = -1f;
					m_Particles[j].scale = m_Particles[j].velocity;
				}
				else
				{
					m_Particles[j].scale = m_Particles[j].velocity * 900f;
				}
				continue;
			}
			if ((m_Particles[j].flag & 2) != 0)
			{
				m_Particles[j].position.X += m_Particles[j].sizeScale;
				m_Particles[j].position.Z += m_Particles[j].velocityScale;
				m_Particles[j].position.Y += m_Particles[j].gravity;
				continue;
			}
			if ((m_Particles[j].flag & 0x1000) != 0)
			{
				m_Particles[j].position.X += m_Particles[j].sizeScale;
				m_Particles[j].position.Z += m_Particles[j].velocityScale;
				m_Particles[j].position.Y += m_Particles[j].gravity;
				continue;
			}
			m_Particles[j].position += m_Particles[j].velocity * gameTime * 48f;
			m_Particles[j].scale *= m_Particles[j].sizeScale;
			m_Particles[j].velocity *= m_Particles[j].velocityScale;
			m_Particles[j].velocity.Y -= m_Particles[j].gravity * gameTime;
			if ((m_Particles[j].flag & 0x10) < 1 && m_Particles[j].rotation != 0f)
			{
				if (m_Particles[j].rotation > 0f)
				{
					m_Particles[j].rotation = m_Particles[j].rotation + m_Particles[j].rotationVelocity;
				}
				else
				{
					m_Particles[j].rotation = m_Particles[j].rotation - m_Particles[j].rotationVelocity;
				}
			}
		}
	}

	public void UpdatePlayer(PlayerBase playerRef, int qIndex)
	{
		int playerIndex = (int)playerRef.playerIndex;
		view = playerRef.mDataQueue[qIndex].view;
		proj = playerRef.mDataQueue[qIndex].projection;
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
		m_TmpBuffCount[qIndex] = 0;
		m_BuffCountPost[qIndex] = 0;
		if (m_NumParticles < 0)
		{
			m_NumParticles = 0;
		}
		for (int i = 0; i < m_NumParticles; i++)
		{
			m_Particles[i].disSqr = (pos - m_Particles[i].position).LengthSquared();
		}
		for (int j = 1; j < m_NumParticles; j++)
		{
			tmpPartSwap = m_Particles[j];
			int num = j;
			while (num > 0 && m_Particles[num - 1].disSqr < tmpPartSwap.disSqr)
			{
				m_Particles[num] = m_Particles[num - 1];
				num--;
			}
			m_Particles[num] = tmpPartSwap;
		}
		int num2 = 0;
		zBias = dir * 1.01f;
		for (int k = 0; k < m_NumParticles; k++)
		{
			if (((m_Particles[k].flag & 0x40) < 1 && m_Particles[k].delay > 0f) || m_Particles[k].disSqr > 225000000f || m_Particles[k].distortion || (m_Particles[k].flag & 0x400) != 0)
			{
				continue;
			}
			partPos = m_Particles[k].position;
			partPos.X -= ViewSpaceDependantOffset.X;
			partPos.Z -= ViewSpaceDependantOffset.Y;
			if ((m_Particles[k].flag & 0x80) != 0 || (m_Particles[k].flag & 4) != 0)
			{
				if ((m_Particles[k].flag & 4) != 0)
				{
					Vector3 zero = Vector3.Zero;
					zero = ((!(Math.Abs(m_Particles[k].velocity.Y) > 0.7f)) ? Vector3.Cross(m_Particles[k].velocity, Vector3.UnitY) : ((!(Math.Abs(m_Particles[k].velocity.X) > Math.Abs(m_Particles[k].velocity.Z))) ? Vector3.Cross(m_Particles[k].velocity, Vector3.UnitX) : Vector3.Cross(m_Particles[k].velocity, Vector3.UnitZ)));
					Vector3 vector = Vector3.Cross(m_Particles[k].velocity, zero);
					a = (-zero + -vector) * m_Particles[k].scale;
					b = (zero + -vector) * m_Particles[k].scale;
					c = (-zero + vector) * m_Particles[k].scale;
					d = (zero + vector) * m_Particles[k].scale;
					a += m_Particles[k].velocity * 200f;
					b += m_Particles[k].velocity * 200f;
					c -= m_Particles[k].velocity * 300f;
					d -= m_Particles[k].velocity * 300f;
				}
				else
				{
					Vector3 zero2 = Vector3.Zero;
					zero2 = Vector3.Cross(vector2: (!(Math.Abs(dir.Y) > 0.7f)) ? Vector3.Cross(dir, Vector3.UnitY) : ((!(Math.Abs(dir.X) > Math.Abs(dir.Z))) ? Vector3.Cross(dir, Vector3.UnitX) : Vector3.Cross(dir, Vector3.UnitZ)), vector1: dir);
					a = -zero2 * 8f + m_Particles[k].scale;
					b = zero2 * 8f + m_Particles[k].scale;
					c = -zero2 * 8f;
					d = zero2 * 8f;
					m_Particles[k].position += m_Particles[k].velocity * 400f;
				}
			}
			else if ((m_Particles[k].flag & 2) != 0)
			{
				Vector3 vector2 = pos - m_Particles[k].position;
				vector2.Normalize();
				Vector3 vector3 = Vector3.Cross(m_Particles[k].scale, vector2);
				vector3.Normalize();
				a = -vector3 * 24f + m_Particles[k].velocity;
				b = vector3 * 24f + m_Particles[k].velocity;
				c = -vector3 * 24f;
				d = vector3 * 24f;
			}
			else if ((m_Particles[k].flag & 0x1000) != 0)
			{
				Vector3 vector4 = pos - m_Particles[k].position;
				vector4.Normalize();
				Vector3 vector5 = Vector3.Cross(m_Particles[k].scale, vector4);
				vector5.Normalize();
				a = -vector5 * 16f + m_Particles[k].velocity;
				b = vector5 * 16f + m_Particles[k].velocity;
				c = -vector5 * 16f;
				d = vector5 * 16f;
			}
			else if ((m_Particles[k].flag & 8) != 0)
			{
				Vector3 position = m_Particles[k].position;
				position.Y = 0f;
				position.Normalize();
				tmpRotate = Matrix.CreateFromAxisAngle(position, m_Particles[k].rotation);
				position = Vector3.Cross(position, Vector3.UnitY);
				tmpScale = m_Particles[k].scale * 0.5f;
				a = (-position + -Vector3.UnitY) * tmpScale;
				b = (position + -Vector3.UnitY) * tmpScale;
				c = (-position + Vector3.UnitY) * tmpScale;
				d = (position + Vector3.UnitY) * tmpScale;
				Vector3.Transform(ref a, ref tmpRotate, out a);
				Vector3.Transform(ref b, ref tmpRotate, out b);
				Vector3.Transform(ref c, ref tmpRotate, out c);
				Vector3.Transform(ref d, ref tmpRotate, out d);
			}
			else
			{
				Vector3 zero3 = Vector3.Zero;
				zero3 = ((!(Math.Abs(dir.Y) > 0.7f)) ? Vector3.Cross(dir, Vector3.UnitY) : ((!(Math.Abs(dir.X) > Math.Abs(dir.Z))) ? Vector3.Cross(dir, Vector3.UnitX) : Vector3.Cross(dir, Vector3.UnitZ)));
				Vector3 vector6 = Vector3.Cross(dir, zero3);
				tmpScale = m_Particles[k].scale * 0.5f;
				a = (-zero3 + -vector6) * tmpScale;
				b = (zero3 + -vector6) * tmpScale;
				c = (-zero3 + vector6) * tmpScale;
				d = (zero3 + vector6) * tmpScale;
				tmpRotate = Matrix.CreateFromAxisAngle(dir, m_Particles[k].rotation);
				Vector3.Transform(ref a, ref tmpRotate, out a);
				Vector3.Transform(ref b, ref tmpRotate, out b);
				Vector3.Transform(ref c, ref tmpRotate, out c);
				Vector3.Transform(ref d, ref tmpRotate, out d);
			}
			a += partPos;
			b += partPos;
			c += partPos;
			d += partPos;
			_ = m_Particles[k].uvDistortion;
			byte textureOffset = m_Particles[k].textureOffset;
			byte colorAlpha = (byte)(m_Particles[k].alpha * 255f);
			if ((m_Particles[k].flag & 0x10) != 0)
			{
				_ = m_Particles[k].alpha;
				colorAlpha = byte.MaxValue;
			}
			m_TmpBuff[qIndex][playerIndex][num2].Position = a;
			m_TmpBuff[qIndex][playerIndex][num2].vertColor = m_Particles[k].diffuse;
			m_TmpBuff[qIndex][playerIndex][num2].ColorAlpha = colorAlpha;
			m_TmpBuff[qIndex][playerIndex][num2].tex.X = TextureCoords[textureOffset][0].X;
			m_TmpBuff[qIndex][playerIndex][num2].tex.Y = TextureCoords[textureOffset][0].Y;
			m_TmpBuff[qIndex][playerIndex][num2].tex.Z = (float)(int)m_Particles[k].softParticle * 0.00392157f;
			num2++;
			m_TmpBuff[qIndex][playerIndex][num2].Position = b;
			m_TmpBuff[qIndex][playerIndex][num2].vertColor = m_Particles[k].diffuse;
			m_TmpBuff[qIndex][playerIndex][num2].ColorAlpha = colorAlpha;
			m_TmpBuff[qIndex][playerIndex][num2].tex.X = TextureCoords[textureOffset][1].X;
			m_TmpBuff[qIndex][playerIndex][num2].tex.Y = TextureCoords[textureOffset][1].Y;
			m_TmpBuff[qIndex][playerIndex][num2].tex.Z = (float)(int)m_Particles[k].softParticle * 0.00392157f;
			num2++;
			m_TmpBuff[qIndex][playerIndex][num2].Position = c;
			m_TmpBuff[qIndex][playerIndex][num2].vertColor = m_Particles[k].diffuse;
			m_TmpBuff[qIndex][playerIndex][num2].ColorAlpha = colorAlpha;
			m_TmpBuff[qIndex][playerIndex][num2].tex.X = TextureCoords[textureOffset][2].X;
			m_TmpBuff[qIndex][playerIndex][num2].tex.Y = TextureCoords[textureOffset][2].Y;
			m_TmpBuff[qIndex][playerIndex][num2].tex.Z = (float)(int)m_Particles[k].softParticle * 0.00392157f;
			num2++;
			m_TmpBuff[qIndex][playerIndex][num2].Position = b;
			m_TmpBuff[qIndex][playerIndex][num2].vertColor = m_Particles[k].diffuse;
			m_TmpBuff[qIndex][playerIndex][num2].ColorAlpha = colorAlpha;
			m_TmpBuff[qIndex][playerIndex][num2].tex.X = TextureCoords[textureOffset][1].X;
			m_TmpBuff[qIndex][playerIndex][num2].tex.Y = TextureCoords[textureOffset][1].Y;
			m_TmpBuff[qIndex][playerIndex][num2].tex.Z = (float)(int)m_Particles[k].softParticle * 0.00392157f;
			num2++;
			m_TmpBuff[qIndex][playerIndex][num2].Position = d;
			m_TmpBuff[qIndex][playerIndex][num2].vertColor = m_Particles[k].diffuse;
			m_TmpBuff[qIndex][playerIndex][num2].ColorAlpha = colorAlpha;
			m_TmpBuff[qIndex][playerIndex][num2].tex.X = TextureCoords[textureOffset][3].X;
			m_TmpBuff[qIndex][playerIndex][num2].tex.Y = TextureCoords[textureOffset][3].Y;
			m_TmpBuff[qIndex][playerIndex][num2].tex.Z = (float)(int)m_Particles[k].softParticle * 0.00392157f;
			num2++;
			m_TmpBuff[qIndex][playerIndex][num2].Position = c;
			m_TmpBuff[qIndex][playerIndex][num2].vertColor = m_Particles[k].diffuse;
			m_TmpBuff[qIndex][playerIndex][num2].ColorAlpha = colorAlpha;
			m_TmpBuff[qIndex][playerIndex][num2].tex.X = TextureCoords[textureOffset][2].X;
			m_TmpBuff[qIndex][playerIndex][num2].tex.Y = TextureCoords[textureOffset][2].Y;
			m_TmpBuff[qIndex][playerIndex][num2].tex.Z = (float)(int)m_Particles[k].softParticle * 0.00392157f;
			num2++;
			m_TmpBuffCount[qIndex]++;
		}
		num2 = 0;
		for (int l = 0; l < m_NumParticles; l++)
		{
			if (m_Particles[l].distortion)
			{
				Vector3 zero4 = Vector3.Zero;
				zero4 = ((!(Math.Abs(dir.Y) > 0.7f)) ? Vector3.Cross(dir, Vector3.UnitY) : ((!(Math.Abs(dir.X) > Math.Abs(dir.Z))) ? Vector3.Cross(dir, Vector3.UnitX) : Vector3.Cross(dir, Vector3.UnitZ)));
				Vector3 vector7 = Vector3.Cross(dir, zero4);
				tmpScale = m_Particles[l].scale * 0.5f;
				a = (-zero4 + -vector7) * tmpScale;
				b = (zero4 + -vector7) * tmpScale;
				c = (-zero4 + vector7) * tmpScale;
				d = (zero4 + vector7) * tmpScale;
				tmpRotate = Matrix.CreateFromAxisAngle(dir, m_Particles[l].rotation);
				Vector3.Transform(ref a, ref tmpRotate, out a);
				Vector3.Transform(ref b, ref tmpRotate, out b);
				Vector3.Transform(ref c, ref tmpRotate, out c);
				Vector3.Transform(ref d, ref tmpRotate, out d);
				partPos = m_Particles[l].position;
				_ = m_Particles[l].uvDistortion;
				byte textureOffset2 = m_Particles[l].textureOffset;
				m_BuffPost[qIndex][playerIndex][num2].Position = partPos + a;
				m_BuffPost[qIndex][playerIndex][num2].vertColor = m_Particles[l].diffuse;
				m_BuffPost[qIndex][playerIndex][num2].ColorAlpha = (byte)(m_Particles[l].alpha * 255f);
				m_BuffPost[qIndex][playerIndex][num2].tex.X = TextureCoords[textureOffset2][0].X;
				m_BuffPost[qIndex][playerIndex][num2].tex.Y = TextureCoords[textureOffset2][0].Y;
				m_BuffPost[qIndex][playerIndex][num2].tex.Z = (float)(int)m_Particles[l].softParticle * 0.00392157f;
				num2++;
				m_BuffPost[qIndex][playerIndex][num2].Position = partPos + b;
				m_BuffPost[qIndex][playerIndex][num2].vertColor = m_Particles[l].diffuse;
				m_BuffPost[qIndex][playerIndex][num2].ColorAlpha = (byte)(m_Particles[l].alpha * 255f);
				m_BuffPost[qIndex][playerIndex][num2].tex.X = TextureCoords[textureOffset2][1].X;
				m_BuffPost[qIndex][playerIndex][num2].tex.Y = TextureCoords[textureOffset2][1].Y;
				m_BuffPost[qIndex][playerIndex][num2].tex.Z = (float)(int)m_Particles[l].softParticle * 0.00392157f;
				num2++;
				m_BuffPost[qIndex][playerIndex][num2].Position = partPos + c;
				m_BuffPost[qIndex][playerIndex][num2].vertColor = m_Particles[l].diffuse;
				m_BuffPost[qIndex][playerIndex][num2].ColorAlpha = (byte)(m_Particles[l].alpha * 255f);
				m_BuffPost[qIndex][playerIndex][num2].tex.X = TextureCoords[textureOffset2][2].X;
				m_BuffPost[qIndex][playerIndex][num2].tex.Y = TextureCoords[textureOffset2][2].Y;
				m_BuffPost[qIndex][playerIndex][num2].tex.Z = (float)(int)m_Particles[l].softParticle * 0.00392157f;
				num2++;
				m_BuffPost[qIndex][playerIndex][num2].Position = partPos + b;
				m_BuffPost[qIndex][playerIndex][num2].vertColor = m_Particles[l].diffuse;
				m_BuffPost[qIndex][playerIndex][num2].ColorAlpha = (byte)(m_Particles[l].alpha * 255f);
				m_BuffPost[qIndex][playerIndex][num2].tex.X = TextureCoords[textureOffset2][1].X;
				m_BuffPost[qIndex][playerIndex][num2].tex.Y = TextureCoords[textureOffset2][1].Y;
				m_BuffPost[qIndex][playerIndex][num2].tex.Z = (float)(int)m_Particles[l].softParticle * 0.00392157f;
				num2++;
				m_BuffPost[qIndex][playerIndex][num2].Position = partPos + d;
				m_BuffPost[qIndex][playerIndex][num2].vertColor = m_Particles[l].diffuse;
				m_BuffPost[qIndex][playerIndex][num2].ColorAlpha = (byte)(m_Particles[l].alpha * 255f);
				m_BuffPost[qIndex][playerIndex][num2].tex.X = TextureCoords[textureOffset2][3].X;
				m_BuffPost[qIndex][playerIndex][num2].tex.Y = TextureCoords[textureOffset2][3].Y;
				m_BuffPost[qIndex][playerIndex][num2].tex.Z = (float)(int)m_Particles[l].softParticle * 0.00392157f;
				num2++;
				m_BuffPost[qIndex][playerIndex][num2].Position = partPos + c;
				m_BuffPost[qIndex][playerIndex][num2].vertColor = m_Particles[l].diffuse;
				m_BuffPost[qIndex][playerIndex][num2].ColorAlpha = (byte)(m_Particles[l].alpha * 255f);
				m_BuffPost[qIndex][playerIndex][num2].tex.X = TextureCoords[textureOffset2][2].X;
				m_BuffPost[qIndex][playerIndex][num2].tex.Y = TextureCoords[textureOffset2][2].Y;
				m_BuffPost[qIndex][playerIndex][num2].tex.Z = (float)(int)m_Particles[l].softParticle * 0.00392157f;
				num2++;
				m_BuffCountPost[qIndex]++;
			}
		}
	}

	public void Draw(ref Matrix view, ref Matrix proj, ref Vector3 pos, ref Vector3 dir, int renderPass, int qIndex)
	{
	}

	public void Draw(PlayerBase playerRef, int renderPass, int qIndex)
	{
		int playerIndex = (int)playerRef.playerIndex;
		int num = 0;
		switch (renderPass)
		{
		case 1:
			num = m_TmpBuffCount[qIndex] * 2;
			break;
		case 5:
			num = m_BuffCountPost[qIndex] * 2;
			break;
		}
		if (num >= 2)
		{
			EndGameEngine.MaterialParams.Texture7.SetValue(LevelBaseMenu.DepthRenderTarget);
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
			EndGameEngine.GraphicMgr.GraphicsDevice.DepthStencilState = EndGameEngine.DepthDisabled;
			EndGameEngine.GraphicMgr.GraphicsDevice.RasterizerState = EndGameEngine.RasterCullNone;
			EndGameEngine.MaterialEffectParams materialParams = EndGameEngine.MaterialParams;
			EndGameEngine.MaterialEffect.CurrentTechnique = materialParams.T_Particle;
			Vector3 value = Vector3.Transform(-playerRef.mDataQueue[qIndex].view.Translation, Matrix.Transpose(playerRef.mDataQueue[qIndex].view));
			materialParams.vecEyePosition.SetValue(value);
			materialParams.matViewProj.SetValue(playerRef.mDataQueue[qIndex].view * playerRef.mDataQueue[qIndex].projection);
			switch (renderPass)
			{
			case 1:
				EndGameEngine.MaterialEffect.CurrentTechnique.Passes[2].Apply();
				EndGameEngine.GraphicMgr.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, m_TmpBuff[qIndex][playerIndex], 0, num);
				break;
			case 5:
			{
				playerRef.SetViewPortTestCoOp(PlayerBase.RenderPass.ForwardPass, 0);
				EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
				EndGameEngine.GraphicMgr.GraphicsDevice.DepthStencilState = EndGameEngine.DepthNoWrite;
				EndGameEngine.MaterialParams.Texture6.SetValue(LevelBaseMenu.bloomRenderTarget[0]);
				EndGameEngine.MaterialParams.Texture5.SetValue(LevelBaseMenu.bloomRenderTarget[2]);
				EndGameEngine.MaterialParams.Texture9.SetValue(PostProcessEffects.AvRSniperReticle);
				EndGameEngine.MaterialParams.Texture8.SetValue(LevelBaseMenu.compositeRenderTarget);
				float num2 = playerRef.Health * 0.015f;
				num2 = ((num2 > 1f) ? 1f : num2);
				num2 = ((num2 < 0f) ? 0f : num2);
				EndGameEngine.MaterialEffect.Parameters["fBloodAplha"].SetValue(num2);
				EndGameEngine.MaterialEffect.Parameters["fogStart"].SetValue(PlayerBase.FogStart);
				EndGameEngine.MaterialEffect.Parameters["fogEnd"].SetValue(PlayerBase.FogEnd);
				Vector4 zero = Vector4.Zero;
				float num3 = EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.X + EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.Width;
				zero.X = 1280f / num3;
				zero.Z = (float)EndGameEngine.GraphicMgr.GraphicsDevice.Viewport.X / 1280f;
				EndGameEngine.MaterialEffect.Parameters["ViewPortOverlayScalar"].SetValue(zero);
				EndGameEngine.MaterialEffect.CurrentTechnique.Passes[1].Apply();
				EndGameEngine.GraphicMgr.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, m_BuffPost[qIndex][playerIndex], 0, num);
				break;
			}
			}
			EndGameEngine.GraphicMgr.GraphicsDevice.BlendState = BlendState.Opaque;
			EndGameEngine.GraphicMgr.GraphicsDevice.DepthStencilState = EndGameEngine.DepthEnabled;
		}
	}

	public static void SpawnByType(ParticleTypes partType, Vector3 pos, Vector3 norm)
	{
		switch (partType)
		{
		case ParticleTypes.BulletHitRock:
			SpawnBulletHitRock(ref pos, ref norm);
			break;
		case ParticleTypes.MuzzleFlash2:
			SpawnMuzzleFlash2(ref pos, fps: true);
			break;
		case ParticleTypes.MuzzleSmoke:
			SpawnMuzzleSmoke(ref pos, ref norm, fps: true);
			break;
		case ParticleTypes.GrenadeExplosion:
			SpawnGrenadeExplosion(pos, 1f);
			break;
		case ParticleTypes.TracerBullet:
			SpawnTracerBullet(ref pos, ref norm, fps: false);
			break;
		case ParticleTypes.MuzzleShotgun:
			break;
		}
	}

	public static void SpawnBulletHitDirt(Vector3 hitPoint)
	{
		for (int i = 0; i < 3; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float num = 8.5f * NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 1f;
				tmpPart.delay = NextRandFloat() * 0.2f;
				tmpPart.alpha = 0.5f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity.X = 0f;
				tmpPart.velocity.Y = 1.5f + NextRandFloat() * 1.75f;
				tmpPart.velocity.Z = 0f;
				tmpPart.position = hitPoint;
				tmpPart.scale.X = 8.5f + num;
				tmpPart.scale.Y = 8.5f + num;
				tmpPart.scale.Z = 8.5f;
				tmpPart.textureOffset = 50;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = 206;
				tmpPart.diffuse.G = 198;
				tmpPart.diffuse.B = 175;
				tmpPart.sizeScale = 1.075f;
				tmpPart.velocityScale = 0.8f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 2.5f;
				tmpPart.rotationVelocity = 0.5f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float num2 = NextRandFloat() * 12f;
				tmpPart.flag = 0u;
				tmpPart.life = 1.85f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 0.25f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity.X = 0f;
				tmpPart.velocity.Y = 0f;
				tmpPart.velocity.Z = 0f;
				tmpPart.position = hitPoint;
				tmpPart.position.X += (NextRandFloat() - 0.5f) * 8f;
				tmpPart.position.Z += (NextRandFloat() - 0.5f) * 8f;
				tmpPart.position.Y += 8f;
				tmpPart.scale.X = 12f + num2;
				tmpPart.scale.Y = 12f + num2;
				tmpPart.scale.Z = 12f;
				if (j > 1)
				{
					tmpPart.textureOffset = 48;
				}
				else
				{
					tmpPart.textureOffset = 50;
				}
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = 166;
				tmpPart.diffuse.G = 158;
				tmpPart.diffuse.B = 135;
				tmpPart.sizeScale = 1.05f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 2.5f;
				tmpPart.rotationVelocity = 0.025f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0.05f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
		for (int k = 0; k < 2; k++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 1.5f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = 0f;
				tmpPart.velocity.X = (NextRandFloat() - 0.5f) * 2f;
				tmpPart.velocity.Y = NextRandFloat() * 2f + 2.5f;
				tmpPart.velocity.Z = (NextRandFloat() - 0.5f) * 2f;
				tmpPart.position = hitPoint;
				tmpPart.scale.X = 3f;
				tmpPart.scale.Y = 3f;
				tmpPart.scale.Z = 3f;
				tmpPart.textureOffset = 53;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = 92;
				tmpPart.diffuse.G = 85;
				tmpPart.diffuse.B = 66;
				tmpPart.sizeScale = 0.995f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 6f;
				tmpPart.rotation = NextRandFloat() - 0.5f;
				tmpPart.rotationVelocity = NextRandFloat();
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnBulletHitWater(Vector3 hitPoint)
	{
		Vector3 zero = Vector3.Zero;
		for (int i = 0; i < 6; i++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = NextRandFloat() * 0.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = NextRandFloat() * 8f;
			tmpPart.velocity.Z = 0f;
			zero.X = NextRandFloat() * 1f - 0.5f;
			zero.Y = NextRandFloat() * 1f;
			zero.Z = NextRandFloat() * 1f - 0.5f;
			tmpPart.position = hitPoint + zero * 12f;
			tmpPart.scale.X = 32f;
			tmpPart.scale.Y = 326f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 6;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 210;
			tmpPart.diffuse.G = 230;
			tmpPart.diffuse.B = 210;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int j = 0; j < 3; j++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = 1.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 0.5f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 8f;
			tmpPart.velocity.Z = 0f;
			zero.X = NextRandFloat() * 1f - 0.5f;
			zero.Y = NextRandFloat() * 1f;
			zero.Z = NextRandFloat() * 1f - 0.5f;
			tmpPart.position = hitPoint + zero * 32f;
			tmpPart.scale.X = 64f;
			tmpPart.scale.Y = 64f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 6;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 210;
			tmpPart.diffuse.G = 230;
			tmpPart.diffuse.B = 210;
			tmpPart.sizeScale = 1.025f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 32f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnBulletHitMetal(ref Vector3 spawnPos, ref Vector3 velocity)
	{
		StickersClass.StickerQueue_Struct sticker = new StickersClass.StickerQueue_Struct
		{
			scale = 8f,
			position = spawnPos,
			normal = velocity,
			material = MaterialType.Metal
		};
		LevelBaseMenu.Stickers.Spawn(ref sticker);
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 0u;
			tmpPart.life = 0.25f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = Vector3.Zero;
			tmpPart.position = spawnPos;
			tmpPart.scale.X = 32f;
			tmpPart.scale.Y = 32f;
			tmpPart.scale.Z = 32f;
			tmpPart.textureOffset = 53;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 0.98f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			m_NumParticles++;
		}
		for (int i = 0; i < 1; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float num = NextRandFloat();
				float num2 = NextRandFloat();
				float num3 = NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 1.5f;
				tmpPart.delay = num3 * 0.05f;
				tmpPart.alpha = 0.2f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity = velocity * NextRandFloat();
				tmpPart.position = spawnPos;
				tmpPart.position.X += num * 16f;
				tmpPart.position.Y += num2 * 16f;
				tmpPart.position.Z += num3 * 16f;
				tmpPart.scale.X = 16f + 12f * num;
				tmpPart.scale.Y = 16f + 12f * num;
				tmpPart.scale.Z = 16f + 12f * num;
				tmpPart.textureOffset = 50;
				tmpPart.diffuse.A = 180;
				tmpPart.diffuse.R = 120;
				tmpPart.diffuse.G = 120;
				tmpPart.diffuse.B = 120;
				tmpPart.sizeScale = 1.005f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0.01f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
				tmpPart.rotationVelocity = 0.025f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
		int num4 = NextRandInt(2, 6);
		for (int j = 0; j < num4; j++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				if (j < 3)
				{
					tmpPart.flag = 512u;
				}
				else
				{
					tmpPart.flag = 0u;
				}
				tmpPart.life = (NextRandFloat() + 0.5f) * 0.4f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = 0f;
				tmpPart.velocity.X = (NextRandFloat() - 0.5f) * 24f;
				tmpPart.velocity.Y = (NextRandFloat() - 0.5f) * 12f;
				tmpPart.velocity.Z = (NextRandFloat() - 0.5f) * 24f;
				tmpPart.position = spawnPos;
				tmpPart.scale.X = 8f;
				tmpPart.scale.Y = 8f;
				tmpPart.scale.Z = 8f;
				tmpPart.textureOffset = 52;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = byte.MaxValue;
				tmpPart.diffuse.G = byte.MaxValue;
				tmpPart.diffuse.B = 220;
				tmpPart.sizeScale = 1f;
				tmpPart.velocityScale = 0.92f;
				tmpPart.gravity = 4f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnBulletHitWood(ref Vector3 spawnPos, ref Vector3 velocity)
	{
		tmpPart.flag = 0u;
		tmpPart.life = 1.5f;
		tmpPart.delay = 0f;
		tmpPart.alpha = 0.25f;
		tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
		tmpPart.velocity = Vector3.Zero;
		tmpPart.position = spawnPos;
		tmpPart.scale.X = 128f;
		tmpPart.scale.Y = 128f;
		tmpPart.scale.Z = 128f;
		tmpPart.textureOffset = 0;
		tmpPart.diffuse.A = byte.MaxValue;
		tmpPart.diffuse.R = 120;
		tmpPart.diffuse.G = 96;
		tmpPart.diffuse.B = 96;
		tmpPart.sizeScale = 0.995f;
		tmpPart.velocityScale = 1f;
		tmpPart.gravity = 0f;
		tmpPart.distortion = false;
		tmpPart.uvDistortion = 0f;
		tmpPart.softParticle = 216;
		Spawn(tmpPart);
		for (int i = 0; i < 4; i++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = 0.75f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = velocity * (NextRandFloat() * 16f);
			tmpPart.velocity.X *= NextRandFloat();
			tmpPart.velocity.Y *= NextRandFloat();
			tmpPart.velocity.Z *= NextRandFloat();
			tmpPart.velocity = velocity * (NextRandFloat() * 16f);
			tmpPart.position = spawnPos;
			tmpPart.scale.X = 16f + 8f * NextRandFloat();
			tmpPart.scale.Y = 16f;
			tmpPart.scale.Z = 16f + 8f * NextRandFloat();
			tmpPart.textureOffset = 7;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 160;
			tmpPart.diffuse.G = 60;
			tmpPart.diffuse.B = 60;
			tmpPart.sizeScale = 0.975f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 32f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnBulletHitRock(ref Vector3 spawnPos, ref Vector3 velocity)
	{
		if (!ParticlesInitialized)
		{
			return;
		}
		StickersClass.StickerQueue_Struct sticker = new StickersClass.StickerQueue_Struct
		{
			scale = 20f,
			position = spawnPos,
			normal = velocity,
			material = MaterialType.Concrete
		};
		LevelBaseMenu.Stickers.Spawn(ref sticker);
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 0u;
			tmpPart.life = 0.1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = Vector3.Zero;
			tmpPart.position = spawnPos;
			tmpPart.scale.X = 32f;
			tmpPart.scale.Y = 32f;
			tmpPart.scale.Z = 32f;
			tmpPart.textureOffset = 53;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = 120;
			tmpPart.sizeScale = 1.05f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = 0.001f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			m_NumParticles++;
		}
		for (int i = 0; i < 4; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float num = NextRandFloat();
				float num2 = NextRandFloat();
				float num3 = NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 1f;
				tmpPart.delay = num3 * 0.05f;
				tmpPart.alpha = 0.2f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity = velocity * NextRandFloat();
				tmpPart.position = spawnPos;
				tmpPart.position.X += num * 16f;
				tmpPart.position.Y += num2 * 16f;
				tmpPart.position.Z += num3 * 16f;
				tmpPart.scale.X = 16f + 12f * num;
				tmpPart.scale.Y = 16f + 12f * num;
				tmpPart.scale.Z = 16f + 12f * num;
				tmpPart.textureOffset = 50;
				tmpPart.diffuse.A = 180;
				tmpPart.diffuse.R = 120;
				tmpPart.diffuse.G = 120;
				tmpPart.diffuse.B = 120;
				tmpPart.sizeScale = 1.0015f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0.01f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
				tmpPart.rotationVelocity = 0.025f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
		for (int j = 0; j < 4; j++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float num4 = 16.5f * NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 0.75f;
				tmpPart.delay = NextRandFloat() * 0.2f;
				tmpPart.alpha = 0.25f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.position = spawnPos;
				tmpPart.position.X += NextRandFloat() * 6f;
				tmpPart.position.Y += NextRandFloat() * 6f;
				tmpPart.position.Z += NextRandFloat() * 6f;
				tmpPart.velocity = velocity * (4f + NextRandFloat() * 8f);
				tmpPart.scale.X = 16.5f + num4;
				tmpPart.scale.Y = 16.5f + num4;
				tmpPart.scale.Z = 16.5f + num4;
				tmpPart.textureOffset = 50;
				tmpPart.diffuse.A = 180;
				tmpPart.diffuse.R = 180;
				tmpPart.diffuse.G = 160;
				tmpPart.diffuse.B = 140;
				tmpPart.sizeScale = 1.002f;
				tmpPart.velocityScale = 0.8f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 2.5f;
				tmpPart.rotationVelocity = 0.15f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnBulletHitTree(ref Vector3 spawnPos, ref Vector3 velocity)
	{
		tmpPart.flag = 0u;
		tmpPart.life = 1.5f;
		tmpPart.delay = 0f;
		tmpPart.alpha = 0.5f;
		tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
		tmpPart.velocity = Vector3.Zero;
		tmpPart.position = spawnPos;
		tmpPart.scale.X = 160f;
		tmpPart.scale.Y = 160f;
		tmpPart.scale.Z = 160f;
		tmpPart.textureOffset = 0;
		tmpPart.diffuse.A = byte.MaxValue;
		tmpPart.diffuse.R = 100;
		tmpPart.diffuse.G = 78;
		tmpPart.diffuse.B = 36;
		tmpPart.sizeScale = 0.995f;
		tmpPart.velocityScale = 1f;
		tmpPart.gravity = 0f;
		tmpPart.distortion = false;
		tmpPart.uvDistortion = 0f;
		tmpPart.softParticle = 216;
		Spawn(tmpPart);
		for (int i = 0; i < 4; i++)
		{
			tmpPart.life = 0.75f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = velocity * (NextRandFloat() * 16f);
			tmpPart.velocity.X *= NextRandFloat();
			tmpPart.velocity.Y *= NextRandFloat();
			tmpPart.velocity.Z *= NextRandFloat();
			tmpPart.velocity = velocity * (NextRandFloat() * 16f);
			tmpPart.position = spawnPos;
			tmpPart.scale.X = 16f + 16f * NextRandFloat();
			tmpPart.scale.Y = 16f + 16f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 7;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 163;
			tmpPart.diffuse.G = 137;
			tmpPart.diffuse.B = 114;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 32f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnBulletHitLeaves(ref Vector3 spawnPos, ref Vector3 velocity)
	{
		for (int i = 0; i < 4; i++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = 2.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = velocity * (NextRandFloat() * -4f);
			tmpPart.velocity.X *= NextRandFloat();
			tmpPart.velocity.Y *= NextRandFloat();
			tmpPart.velocity.Z *= NextRandFloat();
			Vector3 vec = Vector3.Zero;
			math.RandomVector(ref vec);
			tmpPart.position = spawnPos + vec * 32f;
			tmpPart.scale.X = 32f + 24f * NextRandFloat();
			tmpPart.scale.Y = 32f;
			tmpPart.scale.Z = 32f + 24f * NextRandFloat();
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 80;
			tmpPart.diffuse.G = 82;
			tmpPart.diffuse.B = 20;
			tmpPart.sizeScale = 0.99f;
			tmpPart.velocityScale = 1.005f;
			tmpPart.gravity = 2f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnBulletHitMutant(ref Vector3 spawnPos, ref Vector3 velocity)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 16u;
			tmpPart.life = 0.6f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = Vector3.Zero;
			tmpPart.position = spawnPos + velocity * -32f;
			tmpPart.scale.X = 80f + (float)NextRandInt(32);
			tmpPart.scale.Y = tmpPart.scale.X;
			tmpPart.scale.Z = tmpPart.scale.X;
			tmpPart.textureOffset = 112;
			tmpPart.textureAnimationEndIndex = 127;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 1f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			tmpPart.rotation = 0f;
			tmpPart.rotationVelocity = 0f;
			m_NumParticles++;
		}
	}

	public static void SpawnFire(ref Vector3 spawnPos, float fireScale)
	{
		float num = ((fireScale > 1f) ? (fireScale * 0.03f) : 1f);
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			float x = (NextRandFloat() - 0.5f) * fireScale;
			float z = (NextRandFloat() - 0.5f) * fireScale;
			float num2 = (32f + 36f * NextRandFloat()) * fireScale;
			tmpPart.flag = 0u;
			tmpPart.life = 0.75f * fireScale;
			tmpPart.life = ((tmpPart.life < 1.5f) ? tmpPart.life : 1.5f);
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life * 0.5f;
			tmpPart.velocity.X = x;
			tmpPart.velocity.Z = z;
			tmpPart.velocity.Y = (NextRandFloat() + 1f) * fireScale;
			tmpPart.position = spawnPos;
			tmpPart.position.X += (NextRandFloat() - 0.5f) * 16f * fireScale;
			tmpPart.position.Z += (NextRandFloat() - 0.5f) * 16f * fireScale;
			tmpPart.scale.X = num2;
			tmpPart.scale.Y = num2;
			tmpPart.scale.Z = num2;
			tmpPart.textureOffset = (byte)Rand.Next(48, 50);
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 0.968f - num;
			tmpPart.velocityScale = 1.0025f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.065f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 0u;
			tmpPart.life = 1.2f * fireScale;
			tmpPart.life = ((tmpPart.life < 2f) ? tmpPart.life : 2f);
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life * 0.85f;
			tmpPart.velocity.X = (NextRandFloat() - 0.5f) * fireScale;
			tmpPart.velocity.Y = NextRandFloat() * 1.25f + 1f * fireScale;
			tmpPart.velocity.Z = (NextRandFloat() - 0.5f) * fireScale;
			tmpPart.position = spawnPos;
			tmpPart.scale.X = 80f * fireScale;
			tmpPart.scale.Y = 80f * fireScale;
			tmpPart.scale.Z = 80f * fireScale;
			tmpPart.textureOffset = 50;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.0035f - num;
			tmpPart.velocityScale = 1.01f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.05f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 1f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnGroundFire(ref Vector3 spawnPos, ref Vector3 velocity)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			float x = (NextRandFloat() - 0.5f) * 3f;
			float z = (NextRandFloat() - 0.5f) * 3f;
			float num = 64f + 48f * NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 0.75f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life * 0.5f;
			tmpPart.velocity.X = x;
			tmpPart.velocity.Z = z;
			tmpPart.velocity.Y = velocity.Y * ((NextRandFloat() + 1f) * 2f);
			tmpPart.position = spawnPos;
			tmpPart.position.X += (NextRandFloat() - 0.5f) * 128f;
			tmpPart.position.Z += (NextRandFloat() - 0.5f) * 128f;
			tmpPart.scale.X = num;
			tmpPart.scale.Y = num;
			tmpPart.scale.Z = num;
			tmpPart.textureOffset = (byte)Rand.Next(48, 50);
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 0.968f;
			tmpPart.velocityScale = 1.0025f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.065f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 0u;
			tmpPart.life = 1.2f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life * 0.85f;
			tmpPart.velocity.X = NextRandFloat() * 1f - 0.5f;
			tmpPart.velocity.Y = NextRandFloat() * 1.25f + 2f;
			tmpPart.velocity.Z = NextRandFloat() * 1f - 0.5f;
			tmpPart.position = spawnPos;
			tmpPart.position.X += (NextRandFloat() - 1f) * 32f;
			tmpPart.position.Z += (NextRandFloat() - 1f) * 32f;
			tmpPart.scale.X = 80f;
			tmpPart.scale.Y = 80f;
			tmpPart.scale.Z = 80f;
			tmpPart.textureOffset = 50;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.0035f;
			tmpPart.velocityScale = 1.01f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.05f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 1f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnMuzzleHeat(ref Vector3 spawnPos, float lifeSpan)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			float x = (NextRandFloat() - 0.5f) * 2f;
			float z = (NextRandFloat() - 0.5f) * 2f;
			float y = NextRandFloat() + 1f;
			float num = 12f + 6f * NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = lifeSpan;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = x;
			tmpPart.velocity.Z = z;
			tmpPart.velocity.Y = y;
			tmpPart.position = spawnPos;
			tmpPart.position.X += (NextRandFloat() - 0.5f) * 8f;
			tmpPart.position.Z += (NextRandFloat() - 0.5f) * 8f;
			tmpPart.scale.X = num;
			tmpPart.scale.Y = num;
			tmpPart.scale.Z = num;
			tmpPart.textureOffset = 50;
			tmpPart.diffuse.A = 60;
			tmpPart.diffuse.R = 120;
			tmpPart.diffuse.G = 120;
			tmpPart.diffuse.B = 120;
			tmpPart.sizeScale = 1.0085f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.015f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 1f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnLoopingFire(ref Vector3 spawnPos, ref Vector3 velocity, float scale)
	{
		scale *= 2f;
		int num = m_NumParticles;
		if (num >= 511)
		{
			for (int i = 0; i < 512; i++)
			{
				if ((m_Particles[i].flag & 0x20) == 0)
				{
					num = i;
					break;
				}
			}
		}
		else
		{
			m_NumParticles++;
		}
		tmpPart = m_Particles[num];
		float num2 = (NextRandFloat() - 1f) * 60.5f;
		float num3 = (NextRandFloat() - 1f) * 60.5f;
		tmpPart.flag = 32u;
		tmpPart.life = 1f;
		tmpPart.delay = 0f;
		tmpPart.alpha = 1f;
		tmpPart.alphaStep = 0f;
		tmpPart.velocity.X = 0f;
		tmpPart.velocity.Z = 0f;
		tmpPart.velocity.Y = 0f;
		tmpPart.scale.X = scale;
		tmpPart.scale.Y = scale;
		tmpPart.scale.Z = scale;
		tmpPart.position = spawnPos;
		tmpPart.position.X += num2;
		tmpPart.position.Z += num3;
		tmpPart.position.Y += tmpPart.scale.Y * 0.35f;
		tmpPart.textureOffset = 64;
		tmpPart.textureAnimationStartIndex = 64;
		tmpPart.textureAnimationEndIndex = 95;
		tmpPart.diffuse.A = byte.MaxValue;
		tmpPart.diffuse.R = byte.MaxValue;
		tmpPart.diffuse.G = byte.MaxValue;
		tmpPart.diffuse.B = byte.MaxValue;
		tmpPart.sizeScale = 1f;
		tmpPart.velocityScale = 1f;
		tmpPart.gravity = 0f;
		tmpPart.rotation = 0f;
		tmpPart.rotationVelocity = 0f;
		tmpPart.distortion = false;
		tmpPart.uvDistortion = 0f;
		tmpPart.softParticle = 216;
		num = m_NumParticles;
		if (num >= 511)
		{
			for (int j = 0; j < 512; j++)
			{
				if ((m_Particles[j].flag & 0x20) == 0)
				{
					num = j;
					break;
				}
			}
		}
		else
		{
			m_NumParticles++;
		}
		tmpPart = m_Particles[num];
		float num4 = (NextRandFloat() - 1f) * 60.5f;
		float num5 = (NextRandFloat() - 1f) * 60.5f;
		tmpPart.flag = 32u;
		tmpPart.life = 1f;
		tmpPart.delay = 0.6f;
		tmpPart.alpha = 1f;
		tmpPart.alphaStep = 0f;
		tmpPart.velocity.X = 0f;
		tmpPart.velocity.Z = 0f;
		tmpPart.velocity.Y = 0f;
		tmpPart.scale.X = scale;
		tmpPart.scale.Y = scale;
		tmpPart.scale.Z = scale;
		tmpPart.position = spawnPos;
		tmpPart.position.X += num4;
		tmpPart.position.Z += num5;
		tmpPart.position.Y += tmpPart.scale.Y * 0.3f;
		tmpPart.textureOffset = 64;
		tmpPart.textureAnimationStartIndex = 64;
		tmpPart.textureAnimationEndIndex = 95;
		tmpPart.diffuse.A = byte.MaxValue;
		tmpPart.diffuse.R = byte.MaxValue;
		tmpPart.diffuse.G = byte.MaxValue;
		tmpPart.diffuse.B = byte.MaxValue;
		tmpPart.sizeScale = 1f;
		tmpPart.velocityScale = 1f;
		tmpPart.gravity = 0f;
		tmpPart.rotation = 0f;
		tmpPart.rotationVelocity = 0f;
		tmpPart.distortion = false;
		tmpPart.uvDistortion = 0f;
		tmpPart.softParticle = 216;
		num = m_NumParticles;
		if (num >= 511)
		{
			for (int k = 0; k < 512; k++)
			{
				if ((m_Particles[k].flag & 0x20) == 0)
				{
					num = k;
					break;
				}
			}
		}
		else
		{
			m_NumParticles++;
		}
		tmpPart = m_Particles[num];
		NextRandFloat();
		NextRandFloat();
		tmpPart.flag = 32u;
		tmpPart.life = 1f;
		tmpPart.delay = 0.5f;
		tmpPart.alpha = 1f;
		tmpPart.alphaStep = 0f;
		tmpPart.velocity.X = 0f;
		tmpPart.velocity.Z = 0f;
		tmpPart.velocity.Y = 0f;
		tmpPart.scale.X = scale * 1.25f;
		tmpPart.scale.Y = scale * 1.25f;
		tmpPart.scale.Z = scale * 1.25f;
		tmpPart.position = spawnPos;
		tmpPart.position.Y += tmpPart.scale.Y * 0.5f;
		tmpPart.textureOffset = 64;
		tmpPart.textureAnimationStartIndex = 64;
		tmpPart.textureAnimationEndIndex = 95;
		tmpPart.diffuse.A = byte.MaxValue;
		tmpPart.diffuse.R = byte.MaxValue;
		tmpPart.diffuse.G = byte.MaxValue;
		tmpPart.diffuse.B = byte.MaxValue;
		tmpPart.sizeScale = 1f;
		tmpPart.velocityScale = 1f;
		tmpPart.gravity = 0f;
		tmpPart.rotation = 0f;
		tmpPart.rotationVelocity = 0f;
		tmpPart.distortion = true;
		tmpPart.uvDistortion = 1f;
		tmpPart.softParticle = 216;
	}

	public static void SpawnFireSmoke(ref Vector3 spawnPos, ref Vector3 velocity)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			float x = NextRandFloat() - 1f;
			float num = NextRandFloat() - 1f;
			float z = NextRandFloat() - 1f;
			tmpPart.flag = 0u;
			tmpPart.life = 8f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 0.75f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = x;
			tmpPart.velocity.Y = num + 2f;
			tmpPart.velocity.Z = z;
			tmpPart.position = spawnPos;
			tmpPart.scale.X = 160f;
			tmpPart.scale.Y = 160f;
			tmpPart.scale.Z = 160f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = 180;
			tmpPart.diffuse.R = 80;
			tmpPart.diffuse.G = 80;
			tmpPart.diffuse.B = 80;
			tmpPart.sizeScale = 1.0025f;
			tmpPart.velocityScale = 0.9975f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.02f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnDistantSmoke(ref Vector3 spawnPos, ref Vector3 spawnDir)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			byte b = (byte)(NextRandInt(40) + 80);
			float y = (NextRandFloat() + 1f) * 5f;
			float x = (NextRandFloat() - 1f) * 2f;
			float z = NextRandFloat() + 1.5f;
			float num = 800f + 600f * NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 20f;
			tmpPart.delay = 16f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = x;
			tmpPart.velocity.Y = y;
			tmpPart.velocity.Z = z;
			tmpPart.velocity.Y = spawnDir.Y * ((NextRandFloat() + 1f) * 2.5f);
			tmpPart.position = spawnPos;
			tmpPart.position.X += (NextRandFloat() - 1f) * 20f;
			tmpPart.position.Z += (NextRandFloat() - 1f) * 20f;
			tmpPart.scale.X = num;
			tmpPart.scale.Y = num;
			tmpPart.scale.Z = num;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = b;
			tmpPart.diffuse.G = b;
			tmpPart.diffuse.B = b;
			tmpPart.sizeScale = 1.0015f;
			tmpPart.velocityScale = 0.9998f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.005f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnDistantGroundSmoke(ref Vector3 spawnPos, ref Vector3 spawnDir)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			byte b = (byte)(NextRandInt(40) + 180);
			float num = 600f + 400f * NextRandFloat();
			tmpPart.flag = 8u;
			tmpPart.life = 60f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 0.75f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = spawnDir * 3f;
			tmpPart.position = spawnPos;
			tmpPart.position.X += (NextRandFloat() - 1f) * 20f;
			tmpPart.position.Z += (NextRandFloat() - 1f) * 20f;
			tmpPart.scale.X = num;
			tmpPart.scale.Y = num;
			tmpPart.scale.Z = num;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = 180;
			tmpPart.diffuse.R = b;
			tmpPart.diffuse.G = b;
			tmpPart.diffuse.B = b;
			tmpPart.sizeScale = 1.002f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.005f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnCharacterDisolve(ref Vector3 spawnPos)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			float x = (NextRandFloat() - 1f) * 0.5f;
			float z = (NextRandFloat() - 1f) * 0.5f;
			float num = 16f + 16f * NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 0.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life * 0.5f;
			tmpPart.velocity.X = x;
			tmpPart.velocity.Y = (NextRandFloat() + 1f) * 1f;
			tmpPart.velocity.Z = z;
			tmpPart.position = spawnPos;
			tmpPart.position.X += (NextRandFloat() - 1f) * 1f;
			tmpPart.position.Z += (NextRandFloat() - 1f) * 1f;
			tmpPart.scale.X = num;
			tmpPart.scale.Y = num;
			tmpPart.scale.Z = num;
			tmpPart.textureOffset = (byte)Rand.Next(48, 50);
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 0.96f;
			tmpPart.velocityScale = 1.003f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.065f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 0u;
			tmpPart.life = 0.75f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life * 0.85f;
			tmpPart.velocity.X = NextRandFloat() * 1f - 0.5f;
			tmpPart.velocity.Y = NextRandFloat() * 1.25f + 1f;
			tmpPart.velocity.Z = NextRandFloat() * 1f - 0.5f;
			tmpPart.position = spawnPos;
			tmpPart.scale.X = 40f;
			tmpPart.scale.Y = 40f;
			tmpPart.scale.Z = 40f;
			tmpPart.textureOffset = 50;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.0035f;
			tmpPart.velocityScale = 1.01f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.05f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 1f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int i = 0; i < 4; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float x2 = (NextRandFloat() - 0.5f) * 8f;
				float y = (NextRandFloat() - 0.5f) * 8f;
				float z2 = (NextRandFloat() - 0.5f) * 8f;
				float num2 = 4f;
				tmpPart.flag = 0u;
				tmpPart.life = 0.25f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life * 0.5f;
				tmpPart.velocity.X = x2;
				tmpPart.velocity.Y = y;
				tmpPart.velocity.Z = z2;
				tmpPart.position = spawnPos;
				tmpPart.scale.X = num2;
				tmpPart.scale.Y = num2;
				tmpPart.scale.Z = num2;
				tmpPart.textureOffset = 52;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = byte.MaxValue;
				tmpPart.diffuse.G = byte.MaxValue;
				tmpPart.diffuse.B = byte.MaxValue;
				tmpPart.sizeScale = 0.975f;
				tmpPart.velocityScale = 1.003f;
				tmpPart.gravity = 1f;
				tmpPart.rotation = 0f;
				tmpPart.rotationVelocity = 0f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				Spawn(tmpPart);
			}
		}
	}

	public static void SpawnStarTrail(ref Vector3 spawnPos)
	{
		for (int i = 0; i < 4; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float x = (NextRandFloat() - 0.5f) * 80f;
				float y = (NextRandFloat() - 0.5f) * 80f;
				float z = (NextRandFloat() - 0.5f) * 80f;
				float num = 128f;
				tmpPart.flag = 0u;
				tmpPart.life = 0.25f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life * 0.5f;
				tmpPart.velocity.X = x;
				tmpPart.velocity.Y = y;
				tmpPart.velocity.Z = z;
				tmpPart.position = spawnPos;
				tmpPart.scale.X = num;
				tmpPart.scale.Y = num;
				tmpPart.scale.Z = num;
				tmpPart.textureOffset = 52;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = byte.MaxValue;
				tmpPart.diffuse.G = byte.MaxValue;
				tmpPart.diffuse.B = byte.MaxValue;
				tmpPart.sizeScale = 0.997f;
				tmpPart.velocityScale = 1.003f;
				tmpPart.gravity = 1f;
				tmpPart.rotation = 0f;
				tmpPart.rotationVelocity = 0f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				Spawn(tmpPart);
			}
		}
	}

	public static void SpawnHoopTrail(ref Vector3 spawnPos)
	{
		for (int i = 0; i < 4; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float x = (NextRandFloat() - 0.5f) * 80f;
				float y = (NextRandFloat() - 0.5f) * 80f;
				float z = (NextRandFloat() - 0.5f) * 80f;
				float num = 128f;
				tmpPart.flag = 0u;
				tmpPart.life = 0.25f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = 0f;
				tmpPart.velocity.X = x;
				tmpPart.velocity.Y = y;
				tmpPart.velocity.Z = z;
				tmpPart.position = spawnPos;
				tmpPart.scale.X = num;
				tmpPart.scale.Y = num;
				tmpPart.scale.Z = num;
				if (i < 2)
				{
					tmpPart.textureOffset = 59;
				}
				else
				{
					tmpPart.textureOffset = 60;
				}
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = byte.MaxValue;
				tmpPart.diffuse.G = byte.MaxValue;
				tmpPart.diffuse.B = byte.MaxValue;
				tmpPart.sizeScale = 0.997f;
				tmpPart.velocityScale = 1.003f;
				tmpPart.gravity = 1f;
				tmpPart.rotation = 0f;
				tmpPart.rotationVelocity = 0f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				Spawn(tmpPart);
			}
		}
	}

	public static void SpawnTransporter(ref Vector3 spawnPos)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 1.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = Vector3.UnitY * -12f;
			tmpPart.scale.X = 320f;
			tmpPart.scale.Y = 320f;
			tmpPart.scale.Z = 320f;
			tmpPart.position.X = spawnPos.X;
			tmpPart.position.Y = spawnPos.Y + 500f;
			tmpPart.position.Z = spawnPos.Z;
			if (EndGameEngine.randGenerator.Next(100) < 50)
			{
				tmpPart.textureOffset = 55;
			}
			else
			{
				tmpPart.textureOffset = 55;
			}
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 0;
			tmpPart.diffuse.G = 0;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = NextRandFloat() - 0.5f;
			tmpPart.rotationVelocity = 0.015f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnShipEngineDamage(ref Vector3 spawnPos, ref Vector3 spawnVel, float scale)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 0u;
			tmpPart.life = 0.1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = spawnVel;
			tmpPart.scale.X = 820f;
			tmpPart.scale.Y = 820f;
			tmpPart.scale.Z = 820f;
			tmpPart.scale *= scale;
			tmpPart.position.X = spawnPos.X;
			tmpPart.position.Y = spawnPos.Y;
			tmpPart.position.Z = spawnPos.Z;
			tmpPart.textureOffset = 57;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 40;
			tmpPart.diffuse.G = 40;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 6.28f;
			tmpPart.rotationVelocity = 0.015f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
		}
	}

	public static void SpawnShipAntiGravity(ref Vector3 spawnPos)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 0.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = Vector3.UnitY * -8f;
			tmpPart.scale.X = 320f;
			tmpPart.scale.Y = 320f;
			tmpPart.scale.Z = 320f;
			tmpPart.position.X = spawnPos.X;
			tmpPart.position.Y = spawnPos.Y;
			tmpPart.position.Z = spawnPos.Z;
			tmpPart.textureOffset = 50;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 80;
			tmpPart.diffuse.G = 80;
			tmpPart.diffuse.B = 180;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = NextRandFloat() - 0.5f;
			tmpPart.rotationVelocity = 0.015f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 0.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = Vector3.UnitY * -12f;
			tmpPart.scale.X = 600f;
			tmpPart.scale.Y = 600f;
			tmpPart.scale.Z = 600f;
			tmpPart.position.X = spawnPos.X;
			tmpPart.position.Y = spawnPos.Y;
			tmpPart.position.Z = spawnPos.Z;
			tmpPart.textureOffset = 50;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 0;
			tmpPart.diffuse.G = 0;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = NextRandFloat() - 0.5f;
			tmpPart.rotationVelocity = 0.05f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnShipEhaust(ref Vector3 spawnPos, ref Vector3 velocity)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = velocity * 12f;
			tmpPart.scale.X = 220f;
			tmpPart.scale.Y = 220f;
			tmpPart.scale.Z = 220f;
			tmpPart.position.X = spawnPos.X;
			tmpPart.position.Y = spawnPos.Y;
			tmpPart.position.Z = spawnPos.Z;
			tmpPart.textureOffset = 50;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 80;
			tmpPart.diffuse.G = 80;
			tmpPart.diffuse.B = 180;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = NextRandFloat() - 0.5f;
			tmpPart.rotationVelocity = 0.015f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnTracerBullet(ref Vector3 spawnPos, ref Vector3 velocity, bool fps)
	{
		if (fps)
		{
			velocity.Normalize();
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				m_NumParticles++;
				tmpPart.flag = 132u;
				tmpPart.life = 1f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = 0f;
				tmpPart.velocity = velocity;
				tmpPart.position = spawnPos + velocity * 100f;
				tmpPart.scale.X = 4f;
				tmpPart.scale.Y = 4f;
				tmpPart.scale.Z = 4f;
				tmpPart.textureOffset = 14;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = byte.MaxValue;
				tmpPart.diffuse.G = byte.MaxValue;
				tmpPart.diffuse.B = byte.MaxValue;
				tmpPart.sizeScale = 1f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
			}
		}
		else if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 128u;
			tmpPart.life = 1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = velocity;
			tmpPart.velocity.Normalize();
			tmpPart.position = spawnPos;
			tmpPart.scale.X = 0f;
			tmpPart.scale.Y = 0f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 14;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = velocity.Length();
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
		}
	}

	public static void SpawnLaserLight(ref Vector3 spawnPos, ref Vector3 velocity, Color clr, bool fps)
	{
		LevelBaseMenu.PointLights.AddDynamicPointLight(ref spawnPos, ref clr, 1000f, 0.025f, 0);
		velocity.Normalize();
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			if (fps)
			{
				tmpPart.flag = 260u;
			}
			else
			{
				tmpPart.flag = 256u;
			}
			tmpPart.life = 1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = velocity;
			tmpPart.position = spawnPos + velocity * 100f;
			tmpPart.scale.X = 32f;
			tmpPart.scale.Y = 32f;
			tmpPart.scale.Z = 32f;
			tmpPart.textureOffset = 54;
			tmpPart.diffuse = clr;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = NextRandFloat() - 0.5f;
			tmpPart.rotationVelocity = 0.015f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
		}
	}

	public static void SpawnLaserParticle(ref Vector3 spawnPos, ref Vector3 velocity, Color clr)
	{
		velocity.Normalize();
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 0u;
			tmpPart.life = 1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = velocity * 200f;
			tmpPart.position = spawnPos;
			tmpPart.scale.X = 32f;
			tmpPart.scale.Y = 32f;
			tmpPart.scale.Z = 32f;
			tmpPart.textureOffset = 54;
			tmpPart.diffuse = clr;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = NextRandFloat() - 0.5f;
			tmpPart.rotationVelocity = 0.015f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
		}
	}

	public static void SpawnShipLaserParticle(ref Vector3 spawnPos, ref Vector3 velocity)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 8192u;
			tmpPart.life = 1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = velocity;
			tmpPart.velocity.Normalize();
			tmpPart.velocity *= 200f;
			tmpPart.position = spawnPos;
			float num = 32f * NextRandFloat();
			tmpPart.scale.X = 60f + num;
			tmpPart.scale.Y = 60f + num;
			tmpPart.scale.Z = 60f + num;
			tmpPart.textureOffset = 49;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = NextRandFloat() * 3.14f;
			tmpPart.rotationVelocity = 0.05f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
		}
	}

	public static void SpawnMuzzleFlash(ref Vector3 spawnPos, ref Vector3 velocity, bool fps)
	{
		float num = 0f;
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			num = NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 0.2f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 0.65f;
			tmpPart.alphaStep = 0.01f;
			tmpPart.velocity = velocity * 0.9f;
			tmpPart.scale.X = 2.2f + num * 2.5f;
			tmpPart.scale.Y = 2.2f + num * 2.5f;
			tmpPart.scale.Z = 2.2f;
			tmpPart.position.X = spawnPos.X;
			tmpPart.position.Y = spawnPos.Y;
			tmpPart.position.Z = spawnPos.Z;
			tmpPart.textureOffset = 8;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = 180;
			tmpPart.sizeScale = 1.05f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = NextRandFloat() - 0.5f;
			tmpPart.rotationVelocity = 0.025f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			m_NumParticles++;
		}
		for (int i = 0; i < 2; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				num = NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 1.25f;
				tmpPart.delay = (float)i * 0.01f;
				tmpPart.alpha = 0.2f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity = velocity * 0.1f;
				tmpPart.scale.X = 4f + num * 0.1f;
				tmpPart.scale.Y = 4f + num * 0.1f;
				tmpPart.scale.Z = 4f;
				tmpPart.position.X = spawnPos.X + (NextRandFloat() - 0.5f) * 4f;
				tmpPart.position.Y = spawnPos.Y + (NextRandFloat() - 0.5f) * 4f;
				tmpPart.position.Z = spawnPos.Z + (NextRandFloat() - 0.5f) * 4f;
				tmpPart.textureOffset = 13;
				tmpPart.diffuse.A = 200;
				tmpPart.diffuse.R = 200;
				tmpPart.diffuse.G = 200;
				tmpPart.diffuse.B = 200;
				tmpPart.sizeScale = 1.005f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = NextRandFloat() - 0.5f;
				tmpPart.rotationVelocity = 0.025f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnMuzzleFlashShotty(ref Vector3 spawnPos, ref Vector3 velocity, bool fps)
	{
		if (!ParticlesInitialized)
		{
			return;
		}
		NextRandFloat();
		Color color = Color.White;
		LevelBaseMenu.PointLights.AddDynamicPointLight(ref spawnPos, ref color, 800f, 0.05f, 0);
		float num = 0f;
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			num = NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 0.1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0.01f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0f;
			tmpPart.velocity.Z = 0f;
			tmpPart.scale.X = 22f;
			tmpPart.scale.Y = 22f;
			tmpPart.scale.Z = 22f;
			tmpPart.position.X = spawnPos.X;
			tmpPart.position.Y = spawnPos.Y;
			tmpPart.position.Z = spawnPos.Z;
			tmpPart.textureOffset = 52;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.05f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = NextRandFloat() - 0.5f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			m_NumParticles++;
		}
		for (int i = 0; i < 16; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				num = NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 0.5f + NextRandFloat() * 1.2f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = 0f;
				tmpPart.position.X = spawnPos.X;
				tmpPart.position.Y = spawnPos.Y;
				tmpPart.position.Z = spawnPos.Z;
				tmpPos = spawnPos + velocity * 640f;
				tmpPos.X += (NextRandFloat() - 0.5f) * 224f;
				tmpPos.Y += (NextRandFloat() - 0.5f) * 224f;
				tmpPos.Z += (NextRandFloat() - 0.5f) * 224f;
				tmpPart.velocity.X = tmpPos.X - tmpPart.position.X;
				tmpPart.velocity.Y = tmpPos.Y - tmpPart.position.Y;
				tmpPart.velocity.Z = tmpPos.Z - tmpPart.position.Z;
				tmpPart.velocity.Normalize();
				tmpPart.velocity *= 8f;
				tmpPart.scale.X = 1.5f + num;
				tmpPart.scale.Y = 1.5f + num;
				tmpPart.scale.Z = 1.5f + num;
				tmpPart.position.X = spawnPos.X + (NextRandFloat() - 0.5f) * 12f;
				tmpPart.position.Y = spawnPos.Y + (NextRandFloat() - 0.5f) * 12f;
				tmpPart.position.Z = spawnPos.Z + (NextRandFloat() - 0.5f) * 12f;
				tmpPart.textureOffset = 52;
				tmpPart.diffuse.A = 200;
				tmpPart.diffuse.R = 240;
				tmpPart.diffuse.G = 240;
				tmpPart.diffuse.B = 200;
				tmpPart.sizeScale = 0.995f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = NextRandFloat() - 0.5f;
				tmpPart.rotationVelocity = 0.05f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
		for (int j = 0; j < 2; j++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				num = NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 1.25f;
				tmpPart.delay = (float)j * 0.01f;
				tmpPart.alpha = 0.5f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity = velocity * 5f;
				tmpPart.scale.X = 16f + num * 8f;
				tmpPart.scale.Y = 16f + num * 8f;
				tmpPart.scale.Z = 16f + num * 8f;
				tmpPart.position.X = spawnPos.X + (NextRandFloat() - 0.5f) * 4f;
				tmpPart.position.Y = spawnPos.Y + (NextRandFloat() - 0.5f) * 4f;
				tmpPart.position.Z = spawnPos.Z + (NextRandFloat() - 0.5f) * 4f;
				tmpPart.textureOffset = 50;
				tmpPart.diffuse.A = 100;
				tmpPart.diffuse.R = 200;
				tmpPart.diffuse.G = 200;
				tmpPart.diffuse.B = 200;
				tmpPart.sizeScale = 1.03f;
				tmpPart.velocityScale = 0.95f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = NextRandFloat() - 0.5f;
				tmpPart.rotationVelocity = 0.01f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnMFAleinShotty(ref Vector3 spawnPos, ref Vector3 velocity, bool fps)
	{
		if (!ParticlesInitialized)
		{
			return;
		}
		NextRandFloat();
		float num = 0f;
		velocity *= 20f;
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			num = NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 0.1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0.01f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0f;
			tmpPart.velocity.Z = 0f;
			tmpPart.scale.X = 22f;
			tmpPart.scale.Y = 22f;
			tmpPart.scale.Z = 22f;
			tmpPart.position.X = spawnPos.X;
			tmpPart.position.Y = spawnPos.Y;
			tmpPart.position.Z = spawnPos.Z;
			tmpPart.textureOffset = 52;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.05f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = NextRandFloat() - 0.5f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			m_NumParticles++;
		}
		for (int i = 0; i < 16; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				num = NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 0.5f + NextRandFloat() * 0.8f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = 0f;
				tmpPart.position.X = spawnPos.X;
				tmpPart.position.Y = spawnPos.Y;
				tmpPart.position.Z = spawnPos.Z;
				tmpPos = spawnPos + velocity * 32f;
				tmpPos.X += (NextRandFloat() - 0.5f) * 224f;
				tmpPos.Y += (NextRandFloat() - 0.5f) * 224f;
				tmpPos.Z += (NextRandFloat() - 0.5f) * 224f;
				tmpPart.velocity.X = tmpPos.X - tmpPart.position.X;
				tmpPart.velocity.Y = tmpPos.Y - tmpPart.position.Y;
				tmpPart.velocity.Z = tmpPos.Z - tmpPart.position.Z;
				tmpPart.velocity.Normalize();
				tmpPart.velocity *= 8f;
				tmpPart.scale.X = 1.5f + num;
				tmpPart.scale.Y = 1.5f + num;
				tmpPart.scale.Z = 1.5f;
				tmpPart.position.X = spawnPos.X + (NextRandFloat() - 0.5f) * 12f;
				tmpPart.position.Y = spawnPos.Y + (NextRandFloat() - 0.5f) * 12f;
				tmpPart.position.Z = spawnPos.Z + (NextRandFloat() - 0.5f) * 12f;
				tmpPart.textureOffset = 52;
				tmpPart.diffuse = Color.LightYellow;
				tmpPart.sizeScale = 0.995f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = NextRandFloat() - 0.5f;
				tmpPart.rotationVelocity = 0.05f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
		for (int j = 0; j < 3; j++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				num = NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 1.25f;
				tmpPart.delay = (float)j * 0.01f;
				tmpPart.alpha = 0.75f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity = velocity * 0.1f;
				tmpPart.scale.X = 4f + num * 3f;
				tmpPart.scale.Y = 4f + num * 3f;
				tmpPart.scale.Z = 5f;
				tmpPart.position.X = spawnPos.X + (NextRandFloat() - 0.5f) * 4f;
				tmpPart.position.Y = spawnPos.Y + (NextRandFloat() - 0.5f) * 4f;
				tmpPart.position.Z = spawnPos.Z + (NextRandFloat() - 0.5f) * 4f;
				tmpPart.textureOffset = 50;
				tmpPart.diffuse.A = 160;
				tmpPart.diffuse.R = 200;
				tmpPart.diffuse.G = 200;
				tmpPart.diffuse.B = 200;
				tmpPart.sizeScale = 1.05f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = NextRandFloat() - 0.5f;
				tmpPart.rotationVelocity = 0.01f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnNaderMuzzleFlash(ref Vector3 pos, ref Vector3 dir)
	{
		if (!ParticlesInitialized)
		{
			return;
		}
		float num = 0f;
		NextRandFloat();
		for (int i = 0; i < 6; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float num2 = NextRandFloat();
				NextRandFloat();
				float num3 = NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 1f;
				tmpPart.delay = num3 * 0.05f;
				tmpPart.alpha = 0.5f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity = dir * NextRandFloat();
				tmpPart.position = pos;
				tmpPart.scale.X = 16f + 12f * num2;
				tmpPart.scale.Y = 16f + 12f * num2;
				tmpPart.scale.Z = 16f + 12f * num2;
				tmpPart.textureOffset = 50;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = 160;
				tmpPart.diffuse.G = 160;
				tmpPart.diffuse.B = 160;
				tmpPart.sizeScale = 1.0015f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0.01f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
				tmpPart.rotationVelocity = 0.025f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
		for (int j = 0; j < 6; j++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				num = 8.5f * NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 0.75f;
				tmpPart.delay = NextRandFloat() * 0.2f;
				tmpPart.alpha = 0.6f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.position = pos;
				tmpPart.velocity = dir * (4f + NextRandFloat() * 12f);
				tmpPart.scale.X = 8.5f + num;
				tmpPart.scale.Y = 8.5f + num;
				tmpPart.scale.Z = 8.5f + num;
				tmpPart.textureOffset = 50;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = 180;
				tmpPart.diffuse.G = 160;
				tmpPart.diffuse.B = 140;
				tmpPart.sizeScale = 1.0015f;
				tmpPart.velocityScale = 0.8f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 2.5f;
				tmpPart.rotationVelocity = 0.15f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnMuzzleFlash2(ref Vector3 spawnPos, bool fps)
	{
		if (ParticlesInitialized)
		{
			NextRandFloat();
			Color color = Color.Yellow;
			color.B = 220;
			LevelBaseMenu.PointLights.AddDynamicPointLight(ref spawnPos, ref color, 800f, 0.05f, 0);
		}
	}

	public static void SpawnTurretMuzzleFlash(ref Vector3 spawnPos, ref Vector3 velocity)
	{
		if (!ParticlesInitialized)
		{
			return;
		}
		float rotation = NextRandFloat() * 16f;
		float num = 0f;
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			num = NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 0.1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0f;
			tmpPart.velocity.Z = 0f;
			tmpPart.scale.X = 80f;
			tmpPart.scale.Y = 80f;
			tmpPart.scale.Z = 80f;
			tmpPart.position.X = spawnPos.X;
			tmpPart.position.Y = spawnPos.Y;
			tmpPart.position.Z = spawnPos.Z;
			tmpPart.textureOffset = 52;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 0.8f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = rotation;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			m_NumParticles++;
		}
		for (int i = 0; i < 3; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				num = NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 1f;
				tmpPart.delay = (float)i * 0.01f;
				tmpPart.alpha = 0.75f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity = velocity * 2f;
				tmpPart.scale.X = 8f + num * 4f;
				tmpPart.scale.Y = 8f + num * 4f;
				tmpPart.scale.Z = 8f + num * 4f;
				tmpPart.position.X = spawnPos.X + (NextRandFloat() - 0.5f) * 4f;
				tmpPart.position.Y = spawnPos.Y + (NextRandFloat() - 0.5f) * 4f;
				tmpPart.position.Z = spawnPos.Z + (NextRandFloat() - 0.5f) * 4f;
				tmpPart.textureOffset = 50;
				tmpPart.diffuse.A = 160;
				tmpPart.diffuse.R = 200;
				tmpPart.diffuse.G = 200;
				tmpPart.diffuse.B = 200;
				tmpPart.sizeScale = 1.075f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = NextRandFloat() - 0.5f;
				tmpPart.rotationVelocity = 0.01f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnTurretParticle(ref Vector3 spawnPos, ref Vector3 velocity, Color clr, bool fps)
	{
		if (fps)
		{
			velocity.Normalize();
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				m_NumParticles++;
				tmpPart.flag = 0u;
				tmpPart.life = 0.1f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity = velocity * 10f;
				tmpPart.position = spawnPos;
				tmpPart.scale.X = 80f;
				tmpPart.scale.Y = 80f;
				tmpPart.scale.Z = 80f;
				tmpPart.textureOffset = 54;
				tmpPart.diffuse = clr;
				tmpPart.sizeScale = 1f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = NextRandFloat() - 0.5f;
				tmpPart.rotationVelocity = 0.015f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
			}
			return;
		}
		velocity.Normalize();
		float num = 0.025f;
		float num2 = 0.025f;
		for (int i = 0; i < 12; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				m_NumParticles++;
				tmpPart.flag = 0u;
				tmpPart.life = 0.1f;
				tmpPart.delay = 0f;
				tmpPart.alpha = num;
				tmpPart.alphaStep = 0f;
				tmpPart.velocity = velocity * 100f;
				tmpPart.position = spawnPos;
				tmpPart.scale.X = 24f;
				tmpPart.scale.Y = 24f;
				tmpPart.scale.Z = 24f;
				tmpPart.textureOffset = 54;
				tmpPart.diffuse = clr;
				tmpPart.sizeScale = 1f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = NextRandFloat() - 0.5f;
				tmpPart.rotationVelocity = 0.015f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				spawnPos += velocity * 32f;
				num += num2;
				if (num > 1f)
				{
					num = 1f;
				}
				num2 += 0.015f;
			}
		}
	}

	public static void SpawnGunSmoke(ref Vector3 spawnPos, ref Vector3 velocity, bool fps)
	{
		float num = NextRandFloat() * 16f;
		for (int i = 0; i < 3; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				tmpPart.flag = 0u;
				tmpPart.life = 1.5f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 0.25f;
				tmpPart.alphaStep = 0.5f;
				tmpPart.velocity.X = -1.5f;
				tmpPart.velocity.Y = 0.25f + NextRandFloat();
				tmpPart.velocity.Z = -1.5f;
				tmpPart.position = spawnPos;
				tmpPart.position.X += (NextRandFloat() - 0.5f) * 8f;
				tmpPart.position.Z += (NextRandFloat() - 0.5f) * 8f;
				if (fps)
				{
					tmpPart.scale.X = 20f + num * 0.5f;
					tmpPart.scale.Y = 20f + num * 0.5f;
					tmpPart.scale.Z = 20f + num * 0.5f;
				}
				else
				{
					tmpPart.scale.X = 48f + num;
					tmpPart.scale.Y = 48f + num;
					tmpPart.scale.Z = 48f + num;
				}
				tmpPart.textureOffset = 50;
				tmpPart.diffuse = Color.WhiteSmoke;
				tmpPart.sizeScale = 1.01f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = NextRandFloat() - 0.5f;
				tmpPart.rotationVelocity = NextRandFloat() * 0.25f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
			}
		}
	}

	public static void SpawnMuzzleSmoke(ref Vector3 spawnPos, ref Vector3 velocity, bool fps)
	{
	}

	public static void SpawnBlackBirdExhaust(ref Vector3 spawnPos, ref Vector3 velocity)
	{
		if (!ParticlesInitialized)
		{
			return;
		}
		for (int i = 0; i < 1; i++)
		{
			if (m_NumParticles < 511)
			{
				float num = NextRandFloat() * 18f;
				tmpPart = m_Particles[m_NumParticles];
				tmpPart.flag = 0u;
				tmpPart.life = 1f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity.X = velocity.X;
				tmpPart.velocity.Y = velocity.Y;
				tmpPart.velocity.Z = velocity.Z;
				tmpPart.velocity *= NextRandFloat() + 8.5f;
				tmpPart.position = spawnPos;
				tmpPart.position.X += (NextRandFloat() - 0.5f) * 12f;
				tmpPart.position.Z += (NextRandFloat() - 0.5f) * 12f;
				tmpPart.scale.X = 16f + num;
				tmpPart.scale.Y = 16f + num;
				tmpPart.scale.Z = 16f + num;
				tmpPart.textureOffset = 50;
				tmpPart.diffuse.A = 180;
				tmpPart.diffuse.R = 180;
				tmpPart.diffuse.G = 180;
				tmpPart.diffuse.B = 180;
				tmpPart.sizeScale = 1.07f;
				tmpPart.velocityScale = 0.975f;
				tmpPart.gravity = 1f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
				tmpPart.rotationVelocity = NextRandFloat() * 0.05f;
				tmpPart.distortion = true;
				tmpPart.uvDistortion = 1f;
				tmpPart.softParticle = 216;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnBlackBirdGroundDust(ref Vector3 spawnPos)
	{
		if (!ParticlesInitialized)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float num = (float)EndGameEngine.randGenerator.NextDouble();
				EndGameEngine.randGenerator.NextDouble();
				float num2 = (float)EndGameEngine.randGenerator.NextDouble();
				tmpPart.flag = 0u;
				tmpPart.life = 2.15f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 0.15f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity.X = ((float)EndGameEngine.randGenerator.NextDouble() * 2f - 1f) * 16f;
				tmpPart.velocity.Z = ((float)EndGameEngine.randGenerator.NextDouble() * 2f - 1f) * 16f;
				tmpPart.velocity.Y = 0f;
				tmpPart.position = spawnPos;
				tmpPart.position.X += (num * 2f - 1f) * 160f;
				tmpPart.position.Z += (num2 * 2f - 1f) * 160f;
				tmpPart.scale.X = 120f + 64f * num;
				tmpPart.scale.Y = 120f + 64f * num;
				tmpPart.scale.Z = 120f + 64f * num;
				tmpPart.textureOffset = 50;
				tmpPart.diffuse.A = 160;
				tmpPart.diffuse.R = 160;
				tmpPart.diffuse.G = 130;
				tmpPart.diffuse.B = 80;
				tmpPart.sizeScale = 1.0075f;
				tmpPart.velocityScale = 0.98f;
				tmpPart.gravity = -2f;
				tmpPart.rotation = ((float)EndGameEngine.randGenerator.NextDouble() - 0.5f) * 3.14f;
				tmpPart.rotationVelocity = NextRandFloat() * 0.05f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 2;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnVehicleDust(ref Vector3 spawnPos, int nParticles)
	{
		if (!ParticlesInitialized)
		{
			return;
		}
		for (int i = 0; i < nParticles; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float num = (float)EndGameEngine.randGenerator.NextDouble();
				EndGameEngine.randGenerator.NextDouble();
				float num2 = (float)EndGameEngine.randGenerator.NextDouble();
				tmpPart.flag = 0u;
				tmpPart.life = 2.5f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 0.4f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity.X = ((float)EndGameEngine.randGenerator.NextDouble() * 2f - 1f) * 4f;
				tmpPart.velocity.Z = ((float)EndGameEngine.randGenerator.NextDouble() * 2f - 1f) * 4f;
				tmpPart.velocity.Y = 0f;
				tmpPart.position = spawnPos;
				tmpPart.position.X += (num * 2f - 1f) * 100f;
				tmpPart.position.Z += (num2 * 2f - 1f) * 100f;
				tmpPart.scale.X = 120f + 64f * num;
				tmpPart.scale.Y = 120f + 64f * num;
				tmpPart.scale.Z = 120f + 64f * num;
				tmpPart.textureOffset = 62;
				float num3 = ((LevelOutside.DayLightScalar > 0.2f) ? LevelOutside.DayLightScalar : 0.2f);
				tmpPart.diffuse.A = 100;
				tmpPart.diffuse.R = (byte)(230f * num3);
				tmpPart.diffuse.G = (byte)(215f * num3);
				tmpPart.diffuse.B = (byte)(200f * num3);
				tmpPart.sizeScale = 1f;
				tmpPart.velocityScale = 0.9f;
				tmpPart.gravity = -2f;
				tmpPart.rotation = ((float)EndGameEngine.randGenerator.NextDouble() - 0.5f) * 3.14f;
				tmpPart.rotationVelocity = NextRandFloat() * 0.02f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 2;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnSmokeGrenade(Vector3 spawnPos)
	{
		if (!ParticlesInitialized)
		{
			return;
		}
		for (int i = 0; i < 2; i++)
		{
			if (m_NumParticles < 511)
			{
				float num = NextRandFloat() * 120f;
				tmpPart = m_Particles[m_NumParticles];
				tmpPart.flag = 0u;
				tmpPart.life = 16f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity = Vector3.UnitZ;
				tmpPart.position = spawnPos;
				tmpPart.position.X += (NextRandFloat() - 0.5f) * 100f;
				tmpPart.position.Y += 64f;
				tmpPart.position.Z += (NextRandFloat() - 0.5f) * 100f;
				tmpPart.scale.X = 620f + num;
				tmpPart.scale.Y = 420f + num;
				tmpPart.scale.Z = 620f + num;
				tmpPart.textureOffset = 2;
				tmpPart.diffuse.R = byte.MaxValue;
				tmpPart.diffuse.G = byte.MaxValue;
				tmpPart.diffuse.B = byte.MaxValue;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.sizeScale = 1.00025f;
				tmpPart.velocityScale = 0.9f + NextRandFloat() * 0.05f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
				tmpPart.rotationVelocity = NextRandFloat() * 0.001f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 2;
				m_NumParticles++;
			}
		}
		for (int j = 0; j < 16; j++)
		{
			if (m_NumParticles < 511)
			{
				float num2 = NextRandFloat() * 120f;
				tmpPart = m_Particles[m_NumParticles];
				tmpPart.flag = 0u;
				tmpPart.life = 8f + NextRandFloat() * 8f;
				tmpPart.delay = 0.1f + NextRandFloat() * 4f;
				tmpPart.alpha = 0.8f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity = Vector3.UnitZ;
				tmpPart.position = spawnPos;
				tmpPart.position.X += (NextRandFloat() - 0.5f) * 440f;
				tmpPart.position.Y += 64f + (NextRandFloat() - 0.5f) * 180f;
				tmpPart.position.Z += (NextRandFloat() - 0.5f) * 440f;
				tmpPart.scale.X = 180f + num2;
				tmpPart.scale.Y = 180f + num2;
				tmpPart.scale.Z = 180f + num2;
				tmpPart.textureOffset = 2;
				tmpPart.diffuse.R = byte.MaxValue;
				tmpPart.diffuse.G = byte.MaxValue;
				tmpPart.diffuse.B = byte.MaxValue;
				tmpPart.diffuse.A = 220;
				tmpPart.sizeScale = 1.00025f;
				tmpPart.velocityScale = 0.9f + NextRandFloat() * 0.05f;
				tmpPart.gravity = 0f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
				tmpPart.rotationVelocity = NextRandFloat() * 0.01f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 2;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnGrenadeExplosion(Vector3 hitPoint, float scale)
	{
		Vector3 vec = Vector3.Zero;
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 1024u;
			tmpPart.life = 0.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.position = hitPoint;
			tmpPart.position.Y += 80f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0f;
			tmpPart.velocity.Z = 0f;
			tmpPart.scale.X = 2000f;
			tmpPart.scale.Y = 0f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = 224;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = 0f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 2;
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 0u;
			tmpPart.life = 0.25f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.position = hitPoint;
			tmpPart.position.Y += 120f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0.5f;
			tmpPart.velocity.Z = 0f;
			tmpPart.scale.X = 64f;
			tmpPart.scale.Y = 64f;
			tmpPart.scale.Z = 64f;
			tmpPart.textureOffset = 55;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.5f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = 0f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 1f;
			tmpPart.softParticle = 2;
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 16u;
			tmpPart.life = 2f;
			tmpPart.delay = 0.1f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.position = hitPoint;
			tmpPart.position.Y += 100f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0f;
			tmpPart.velocity.Z = 0f;
			tmpPart.scale.X = 300f * scale;
			tmpPart.scale.Y = 300f * scale;
			tmpPart.scale.Z = 300f * scale;
			tmpPart.textureOffset = 0;
			tmpPart.textureAnimationEndIndex = 47;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = 0f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 2;
		}
		for (int i = 0; i < 12; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				m_NumParticles++;
				math.RandomVector(ref vec);
				tmpPart.flag = 0u;
				tmpPart.life = 1f + NextRandFloat();
				tmpPart.delay = 0.2f;
				tmpPart.alpha = 0.75f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity = vec;
				tmpPart.velocity.Y = NextRandFloat();
				tmpPart.velocity.X = tmpPart.velocity.X * 2f * scale;
				tmpPart.velocity.Y = tmpPart.velocity.Y * 2f * scale;
				tmpPart.velocity.Z = tmpPart.velocity.Z * 2f * scale;
				tmpPart.position = hitPoint;
				tmpPart.position.X += vec.X * 64f * scale;
				tmpPart.position.Z += vec.Z * 64f * scale;
				float num = 60f * NextRandFloat();
				tmpPart.scale.X = (60f + num) * scale;
				tmpPart.scale.Y = (60f + num) * scale;
				tmpPart.scale.Z = (60f + num) * scale;
				tmpPart.textureOffset = 50;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = 160;
				tmpPart.diffuse.G = 160;
				tmpPart.diffuse.B = 160;
				tmpPart.sizeScale = 1.025f;
				tmpPart.velocityScale = 0.975f;
				tmpPart.gravity = 1f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
				tmpPart.rotationVelocity = NextRandFloat() * 0.02f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 2;
			}
		}
		for (int j = 0; j < 24; j++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				m_NumParticles++;
				tmpPart.flag = 0u;
				tmpPart.life = 1.5f;
				tmpPart.delay = 0.2f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = 0f;
				tmpPart.velocity.X = (NextRandFloat() * 8f - 2f) * scale;
				tmpPart.velocity.Y = (NextRandFloat() * 16f + 4f) * scale;
				tmpPart.velocity.Z = (NextRandFloat() * 8f - 2f) * scale;
				tmpPart.position = hitPoint + tmpPart.velocity * 8f;
				tmpPart.scale.X = 12f;
				tmpPart.scale.Y = 12f;
				tmpPart.scale.Z = 12f;
				tmpPart.textureOffset = 52;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = 140;
				tmpPart.diffuse.G = 100;
				tmpPart.diffuse.B = 100;
				tmpPart.sizeScale = 1f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 10f;
				tmpPart.rotation = 0f;
				tmpPart.rotationVelocity = 0f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
			}
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 0u;
			tmpPart.life = 1f;
			tmpPart.delay = 0.5f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.position = hitPoint;
			tmpPart.position.Y += 120f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0.5f;
			tmpPart.velocity.Z = 0f;
			tmpPart.scale.X = 32f * scale;
			tmpPart.scale.Y = 32f * scale;
			tmpPart.scale.Z = 32f * scale;
			tmpPart.textureOffset = 50;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.02f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = 0f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 1f;
			tmpPart.softParticle = 216;
		}
	}

	public static void SpawnShipExplosion(Vector3 hitPoint, float scale)
	{
		_ = Vector3.Zero;
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 1024u;
			tmpPart.life = 0.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.position = hitPoint;
			tmpPart.position.Y += 80f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0f;
			tmpPart.velocity.Z = 0f;
			tmpPart.scale.X = 2000f;
			tmpPart.scale.Y = 0f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = 224;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = 0f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 2;
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 0u;
			tmpPart.life = 0.25f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.position = hitPoint;
			tmpPart.position.Y += 120f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0.5f;
			tmpPart.velocity.Z = 0f;
			tmpPart.scale.X = 64f;
			tmpPart.scale.Y = 64f;
			tmpPart.scale.Z = 64f;
			tmpPart.textureOffset = 55;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.5f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = 0f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 1f;
			tmpPart.softParticle = 2;
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 16u;
			tmpPart.life = 2f;
			tmpPart.delay = 0.1f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.position = hitPoint;
			tmpPart.position.Y += 100f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0f;
			tmpPart.velocity.Z = 0f;
			tmpPart.scale.X = 300f * scale;
			tmpPart.scale.Y = 300f * scale;
			tmpPart.scale.Z = 300f * scale;
			tmpPart.textureOffset = 0;
			tmpPart.textureAnimationEndIndex = 47;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = 0f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 2;
		}
		for (int i = 0; i < 8; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				m_NumParticles++;
				tmpPart.flag = 8192u;
				tmpPart.life = 1.5f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = 0f;
				tmpPart.velocity.X = (NextRandFloat() - 0.5f) * 8f * scale;
				tmpPart.velocity.Y = (NextRandFloat() - 0.5f) * 8f * scale;
				tmpPart.velocity.Z = (NextRandFloat() - 0.5f) * 8f * scale;
				tmpPart.position = hitPoint + tmpPart.velocity * 8f;
				tmpPart.scale.X = 24f * scale;
				tmpPart.scale.Y = 24f * scale;
				tmpPart.scale.Z = 24f * scale;
				tmpPart.textureOffset = 48;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = byte.MaxValue;
				tmpPart.diffuse.G = byte.MaxValue;
				tmpPart.diffuse.B = byte.MaxValue;
				tmpPart.sizeScale = 0.99f;
				tmpPart.velocityScale = 1f;
				tmpPart.gravity = 8f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 6.28f;
				tmpPart.rotationVelocity = 0.01f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
			}
		}
	}

	public static void SpawnShipExplosionSmall(Vector3 hitPoint, float scale)
	{
		_ = Vector3.Zero;
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 1024u;
			tmpPart.life = 0.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.position = hitPoint;
			tmpPart.position.Y += 80f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0f;
			tmpPart.velocity.Z = 0f;
			tmpPart.scale.X = 2000f;
			tmpPart.scale.Y = 0f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = 224;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = 0f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 2;
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			m_NumParticles++;
			tmpPart.flag = 16u;
			tmpPart.life = 2f;
			tmpPart.delay = 0.1f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.position = hitPoint;
			tmpPart.position.Y += 100f;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0f;
			tmpPart.velocity.Z = 0f;
			tmpPart.scale.X = 300f * scale;
			tmpPart.scale.Y = 300f * scale;
			tmpPart.scale.Z = 300f * scale;
			tmpPart.textureOffset = 0;
			tmpPart.textureAnimationEndIndex = 47;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = 0f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 2;
		}
	}

	public static void SpawnGrenadeInWater(Vector3 hitPoint)
	{
		Vector3 vec = Vector3.Zero;
		for (int i = 0; i < 4; i++)
		{
			math.RandomVector(ref vec);
			tmpPart.flag = 0u;
			tmpPart.life = 0.5f + NextRandFloat() * 0.2f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vec;
			tmpPart.velocity.Y = NextRandFloat();
			tmpPart.velocity.X *= 4f;
			tmpPart.velocity.Y *= 16f;
			tmpPart.velocity.Z *= 4f;
			tmpPart.position = hitPoint + tmpPart.velocity * 2f;
			tmpPart.scale.X = 256f + 256f * NextRandFloat();
			tmpPart.scale.Y = 256f + 256f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 5;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.01f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 16f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 12;
			Spawn(tmpPart);
		}
		for (int j = 0; j < 4; j++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = 1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 2.5f;
			tmpPart.velocity.Z = 0f;
			tmpPart.position = hitPoint;
			tmpPart.position.X += NextRandFloat() * 32f - 16f;
			tmpPart.position.Y += NextRandFloat() * 128f + 128f;
			tmpPart.position.Z += NextRandFloat() * 32f - 16f;
			tmpPart.scale.X = 300f + 128f * NextRandFloat();
			tmpPart.scale.Y = 400f + 128f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 210;
			tmpPart.diffuse.G = 230;
			tmpPart.diffuse.B = 210;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 8f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 12;
			Spawn(tmpPart);
		}
		for (int k = 0; k < 16; k++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = 2f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity.X = NextRandFloat() * 8f - 4f;
			tmpPart.velocity.Y = NextRandFloat() * 12f + 12f;
			tmpPart.velocity.Z = NextRandFloat() * 8f - 4f;
			tmpPart.position = hitPoint;
			tmpPart.position += tmpPart.velocity * 8f;
			tmpPart.scale.X = 64f;
			tmpPart.scale.Y = 128f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 5;
			tmpPart.diffuse.A = 200;
			tmpPart.diffuse.R = 200;
			tmpPart.diffuse.G = 220;
			tmpPart.diffuse.B = 230;
			tmpPart.sizeScale = 0.995f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 28f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnFlameThrower(ref Vector3 pos, ref Vector3 vel)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 0u;
			tmpPart.life = 1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vel * 24f;
			tmpPart.position = pos;
			float num = 8f * NextRandFloat();
			tmpPart.scale.X = 12f + num;
			tmpPart.scale.Y = 12f + num;
			tmpPart.scale.Z = 12f + num;
			tmpPart.textureOffset = 49;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.075f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.1f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 76;
			m_NumParticles++;
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 0u;
			tmpPart.life = 1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vel * 24f;
			tmpPart.position = pos + tmpPart.velocity * 15f;
			float num2 = 48f * NextRandFloat();
			tmpPart.scale.X = 24f + num2;
			tmpPart.scale.Y = 24f + num2;
			tmpPart.scale.Z = 24f + num2;
			tmpPart.textureOffset = 49;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.05f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.1f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 76;
			m_NumParticles++;
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 0u;
			tmpPart.life = 1.25f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vel * 24f;
			tmpPart.position = pos + tmpPart.velocity * 3f;
			float num3 = 16f * NextRandFloat();
			tmpPart.scale.X = 36f + num3;
			tmpPart.scale.Y = 36f + num3;
			tmpPart.scale.Z = 36f + num3;
			tmpPart.textureOffset = 49;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.02f;
			tmpPart.velocityScale = 0.999f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 1f;
			tmpPart.softParticle = 178;
			m_NumParticles++;
		}
	}

	public static void SpawnRPGExplosion(Vector3 hitPoint)
	{
		Vector3 vec = Vector3.Zero;
		for (int i = 0; i < 4; i++)
		{
			math.RandomVector(ref vec);
			tmpPart.flag = 0u;
			tmpPart.life = 0.15f + NextRandFloat() * 0.15f;
			tmpPart.delay = 0.1f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = vec;
			tmpPart.velocity.Y = NextRandFloat();
			tmpPart.velocity.X *= 8f;
			tmpPart.velocity.Y *= 8f;
			tmpPart.velocity.Z *= 8f;
			tmpPart.position = hitPoint + tmpPart.velocity * 2f;
			tmpPart.scale.X = 200f + 200f * NextRandFloat();
			tmpPart.scale.Y = 200f + 200f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 5;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = 200;
			tmpPart.sizeScale = 1.01f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 12;
			Spawn(tmpPart);
		}
		for (int j = 0; j < 1; j++)
		{
			NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 0.2f + NextRandFloat() * 0.2f;
			tmpPart.delay = 0.1f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = Vector3.Zero;
			tmpPart.position = hitPoint;
			tmpPart.scale.X = 300f + NextRandFloat() * 280f;
			tmpPart.scale.Y = 300f + NextRandFloat() * 280f;
			tmpPart.scale.Z = 300f + NextRandFloat() * 280f;
			tmpPart.textureOffset = 4;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 0.99f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 12;
			Spawn(tmpPart);
		}
		for (int k = 0; k < 16; k++)
		{
			math.RandomVector(ref vec);
			tmpPart.flag = 0u;
			tmpPart.life = 1f + NextRandFloat();
			tmpPart.delay = 0.25f;
			tmpPart.alpha = 0.75f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vec;
			tmpPart.velocity.X *= 8f;
			tmpPart.velocity.Y *= 8f;
			tmpPart.velocity.Z *= 8f;
			tmpPart.position = hitPoint + vec * 180f;
			tmpPart.scale.X = 220f + 200f * NextRandFloat();
			tmpPart.scale.Y = 220f + 200f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 80;
			tmpPart.diffuse.G = 70;
			tmpPart.diffuse.B = 70;
			tmpPart.sizeScale = 0.999f;
			tmpPart.velocityScale = 0.975f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int l = 0; l < 8; l++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = 2.5f;
			tmpPart.delay = 0.25f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = NextRandFloat() * 2f - 1f;
			tmpPart.velocity.Y = NextRandFloat() * 2f - 1f;
			tmpPart.velocity.Z = NextRandFloat() * 2f - 1f;
			tmpPart.position = hitPoint + tmpPart.velocity * 64f;
			tmpPart.scale.X = 128f + NextRandFloat() * 128f;
			tmpPart.scale.Y = 128f + NextRandFloat() * 128f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 80;
			tmpPart.diffuse.G = 80;
			tmpPart.diffuse.B = 80;
			tmpPart.sizeScale = 1.0075f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int m = 0; m < 6; m++)
		{
			NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 1.75f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = NextRandFloat() * 4f - 2f;
			tmpPart.velocity.Y = NextRandFloat() * 4f - 2f;
			tmpPart.velocity.Z = NextRandFloat() * 4f - 2f;
			tmpPart.position = hitPoint + tmpPart.velocity * 48f;
			tmpPart.scale.X = 128f + NextRandFloat() * 128f;
			tmpPart.scale.Y = 128f + NextRandFloat() * 128f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.015f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnRPGTrial(ref Vector3 pos, ref Vector3 vel)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 0u;
			tmpPart.life = 0.2f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0f;
			tmpPart.velocity.Z = 0f;
			tmpPart.position = pos;
			float num = 32f * NextRandFloat();
			tmpPart.scale.X = 60f + num;
			tmpPart.scale.Y = 60f + num;
			tmpPart.scale.Z = 60f + num;
			tmpPart.textureOffset = 49;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 0.95f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.2f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 178;
			m_NumParticles++;
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 0u;
			tmpPart.life = 1f + NextRandFloat();
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vel;
			tmpPart.position = pos;
			float num2 = 60f * NextRandFloat();
			tmpPart.scale.X = 120f + num2;
			tmpPart.scale.Y = 120f + num2;
			tmpPart.scale.Z = 120f + num2;
			tmpPart.textureOffset = 50;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 60;
			tmpPart.diffuse.G = 60;
			tmpPart.diffuse.B = 60;
			tmpPart.sizeScale = 1.025f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.01f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 178;
			m_NumParticles++;
		}
	}

	public static void SpawnSmallRPGTrial(ref Vector3 pos, ref Vector3 vel)
	{
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 0u;
			tmpPart.life = 0.2f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = 0f;
			tmpPart.velocity.Y = 0f;
			tmpPart.velocity.Z = 0f;
			tmpPart.position = pos;
			float num = 32f * NextRandFloat();
			tmpPart.scale.X = 60f + num;
			tmpPart.scale.Y = 60f + num;
			tmpPart.scale.Z = 60f + num;
			tmpPart.textureOffset = 49;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 0.95f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.2f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 178;
			m_NumParticles++;
		}
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 0u;
			tmpPart.life = 0.5f + NextRandFloat() * 0.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vel;
			tmpPart.position = pos;
			float num2 = 60f * NextRandFloat();
			tmpPart.scale.X = 120f + num2;
			tmpPart.scale.Y = 120f + num2;
			tmpPart.scale.Z = 120f + num2;
			tmpPart.textureOffset = 50;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 60;
			tmpPart.diffuse.G = 60;
			tmpPart.diffuse.B = 60;
			tmpPart.sizeScale = 1.025f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = NextRandFloat() * 0.01f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 178;
			m_NumParticles++;
		}
	}

	public static void SpawnRPGExhaust(Vector3 pos, Vector3 vel)
	{
		tmpPart.flag = 0u;
		tmpPart.life = 0.15f;
		tmpPart.delay = 0f;
		tmpPart.alpha = 1f;
		tmpPart.alphaStep = 0f;
		tmpPart.velocity = vel;
		tmpPart.position = pos;
		tmpPart.scale.X = 48f;
		tmpPart.scale.Y = 32f;
		tmpPart.scale.Z = 0f;
		tmpPart.textureOffset = 5;
		tmpPart.diffuse.A = byte.MaxValue;
		tmpPart.diffuse.R = byte.MaxValue;
		tmpPart.diffuse.G = byte.MaxValue;
		tmpPart.diffuse.B = byte.MaxValue;
		tmpPart.sizeScale = 1.01f;
		tmpPart.velocityScale = 1f;
		tmpPart.gravity = 0f;
		tmpPart.distortion = false;
		tmpPart.uvDistortion = 0f;
		tmpPart.softParticle = 216;
		Spawn(tmpPart);
		tmpPart.flag = 0u;
		tmpPart.life = 0.2f;
		tmpPart.delay = 0f;
		tmpPart.alpha = 1f;
		tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
		tmpPart.velocity = vel;
		tmpPart.position = pos;
		tmpPart.scale.X = 32f + 64f * NextRandFloat();
		tmpPart.scale.Y = 32f + 32f * NextRandFloat();
		tmpPart.scale.Z = 0f;
		tmpPart.textureOffset = 4;
		tmpPart.diffuse.A = byte.MaxValue;
		tmpPart.diffuse.R = byte.MaxValue;
		tmpPart.diffuse.G = byte.MaxValue;
		tmpPart.diffuse.B = byte.MaxValue;
		tmpPart.sizeScale = 1.01f;
		tmpPart.velocityScale = 1f;
		tmpPart.gravity = 0f;
		tmpPart.distortion = false;
		tmpPart.uvDistortion = 0f;
		tmpPart.softParticle = 216;
		Spawn(tmpPart);
	}

	public static void SpawnBlastTrial(Vector3 pos, Vector3 vel)
	{
		if (m_NumTrails < 63)
		{
			m_Trails[m_NumTrails].flag = 0u;
			m_Trails[m_NumTrails].life = 0.5f + NextRandFloat();
			m_Trails[m_NumTrails].position = pos;
			m_Trails[m_NumTrails].velocity = vel;
			m_Trails[m_NumTrails].scale = Vector3.Zero;
			m_NumTrails++;
		}
	}

	public static void SpawnSimpleSmoke(ref Vector3 pos, ref Vector3 dir)
	{
		if (ParticlesInitialized)
		{
			NextRandFloat();
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float num = NextRandFloat();
				NextRandFloat();
				NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 2f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
				tmpPart.velocity = dir;
				tmpPart.position = pos;
				tmpPart.scale.X = 260f + 124f * num;
				tmpPart.scale.Y = 260f + 124f * num;
				tmpPart.scale.Z = 260f + 124f * num;
				tmpPart.textureOffset = 61;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = 180;
				tmpPart.diffuse.G = 180;
				tmpPart.diffuse.B = 180;
				tmpPart.sizeScale = 1.025f;
				tmpPart.velocityScale = 0.9985f;
				tmpPart.gravity = 0.01f;
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
				tmpPart.rotationVelocity = 0.025f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 1;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnWaterBubble(ref Vector3 pos, ref Vector3 dir)
	{
		if (ParticlesInitialized)
		{
			NextRandFloat();
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float num = NextRandFloat();
				NextRandFloat();
				NextRandFloat();
				tmpPart.flag = 0u;
				tmpPart.life = 2f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = 0f;
				tmpPart.velocity = dir * (NextRandFloat() + 0.2f);
				tmpPart.position = pos;
				tmpPart.scale.X = 100f + 34f * num;
				tmpPart.scale.Y = 100f + 34f * num;
				tmpPart.scale.Z = 100f + 34f * num;
				tmpPart.textureOffset = 58;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = 140;
				tmpPart.diffuse.G = 140;
				tmpPart.diffuse.B = 210;
				tmpPart.sizeScale = 1.025f;
				tmpPart.velocityScale = 0.9985f;
				tmpPart.gravity = -6f * (NextRandFloat() + 0.5f);
				tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
				tmpPart.rotationVelocity = 0.01f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 127;
				m_NumParticles++;
			}
		}
	}

	public static void SpawnToyPlaneJet(ref Vector3 pos, ref Vector3 dir)
	{
		if (!ParticlesInitialized)
		{
			return;
		}
		NextRandFloat();
		if (m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			float num = NextRandFloat();
			NextRandFloat();
			NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 2f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = dir;
			tmpPart.position = pos;
			tmpPart.scale.X = 200f + 124f * num;
			tmpPart.scale.Y = 200f + 124f * num;
			tmpPart.scale.Z = 200f + 124f * num;
			tmpPart.textureOffset = 50;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 180;
			tmpPart.diffuse.G = 180;
			tmpPart.diffuse.B = 180;
			tmpPart.sizeScale = 1.025f;
			tmpPart.velocityScale = 0.9985f;
			tmpPart.gravity = 0.01f;
			tmpPart.rotation = (NextRandFloat() - 0.5f) * 3.14f;
			tmpPart.rotationVelocity = 0.025f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 127;
			m_NumParticles++;
		}
		for (int i = 0; i < 2; i++)
		{
			if (m_NumParticles < 511)
			{
				tmpPart = m_Particles[m_NumParticles];
				float x = (NextRandFloat() - 0.5f) * 80f;
				float y = (NextRandFloat() - 0.5f) * 80f;
				float z = (NextRandFloat() - 0.5f) * 80f;
				float num2 = 128f;
				tmpPart.flag = 0u;
				tmpPart.life = 0.25f;
				tmpPart.delay = 0f;
				tmpPart.alpha = 1f;
				tmpPart.alphaStep = tmpPart.alpha / tmpPart.life * 0.5f;
				tmpPart.velocity.X = x;
				tmpPart.velocity.Y = y;
				tmpPart.velocity.Z = z;
				tmpPart.position = pos;
				tmpPart.scale.X = num2;
				tmpPart.scale.Y = num2;
				tmpPart.scale.Z = num2;
				tmpPart.textureOffset = 52;
				tmpPart.diffuse.A = byte.MaxValue;
				tmpPart.diffuse.R = byte.MaxValue;
				tmpPart.diffuse.G = byte.MaxValue;
				tmpPart.diffuse.B = byte.MaxValue;
				tmpPart.sizeScale = 0.997f;
				tmpPart.velocityScale = 1.003f;
				tmpPart.gravity = 1f;
				tmpPart.rotation = 0f;
				tmpPart.rotationVelocity = 0f;
				tmpPart.distortion = false;
				tmpPart.uvDistortion = 0f;
				tmpPart.softParticle = 216;
				Spawn(tmpPart);
			}
		}
	}

	public static void SpawnTrialBoard(ref Vector3 start, ref Vector3 end)
	{
		if (ParticlesInitialized && m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 2u;
			tmpPart.life = NextRandFloat() + 0.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.position = start;
			tmpPart.velocity = end - start;
			tmpPart.scale = tmpPart.velocity;
			tmpPart.scale.Normalize();
			tmpPart.textureOffset = 51;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 180;
			tmpPart.diffuse.G = 180;
			tmpPart.diffuse.B = 180;
			tmpPart.sizeScale = 0f;
			tmpPart.velocityScale = 0f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = 0f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			m_NumParticles++;
		}
	}

	public static void SpawnLaserTrialBoard(ref Vector3 start, ref Vector3 end, Color clr)
	{
		if (ParticlesInitialized && m_NumParticles < 511)
		{
			tmpPart = m_Particles[m_NumParticles];
			tmpPart.flag = 4096u;
			tmpPart.life = 0.1f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life * 0.5f;
			tmpPart.position = start;
			tmpPart.velocity = end - start;
			tmpPart.scale = tmpPart.velocity;
			tmpPart.scale.Normalize();
			tmpPart.textureOffset = 56;
			tmpPart.diffuse = clr;
			tmpPart.sizeScale = 0f;
			tmpPart.velocityScale = 0f;
			tmpPart.gravity = 0f;
			tmpPart.rotation = 0f;
			tmpPart.rotationVelocity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			m_NumParticles++;
		}
	}

	public static void SpawnTrialParticle(Vector3 pos, Vector3 vel, float alpha)
	{
		tmpPart.flag = 0u;
		tmpPart.life = 1f + 2f * NextRandFloat();
		tmpPart.delay = 0f;
		tmpPart.alpha = alpha;
		tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
		tmpPart.velocity = Vector3.Zero;
		tmpPart.position = pos;
		tmpPart.scale.X = 128f + 128f * NextRandFloat();
		tmpPart.scale.Y = 128f + 128f * NextRandFloat();
		tmpPart.scale.Z = 0f;
		tmpPart.textureOffset = 0;
		tmpPart.diffuse.A = byte.MaxValue;
		tmpPart.diffuse.R = 180;
		tmpPart.diffuse.G = 180;
		tmpPart.diffuse.B = 180;
		tmpPart.sizeScale = 1.01f;
		tmpPart.velocityScale = 1f;
		tmpPart.gravity = -0.05f;
		tmpPart.distortion = false;
		tmpPart.uvDistortion = 0f;
		tmpPart.softParticle = 216;
		Spawn(tmpPart);
	}

	public static void SpawnHeliHit(Vector3 pos, Vector3 vel)
	{
		Vector3 vec = Vector3.Zero;
		for (int i = 0; i < 4; i++)
		{
			math.RandomVector(ref vec);
			tmpPart.flag = 0u;
			tmpPart.life = 0.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vel * 0.5f;
			tmpPart.position = pos + vec * 128f;
			tmpPart.scale.X = 256f + 256f * NextRandFloat();
			tmpPart.scale.Y = 256f + 256f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 4;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 0.995f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int j = 0; j < 16; j++)
		{
			math.RandomVector(ref vec);
			vec.Y = 0f;
			tmpPart.flag = 0u;
			tmpPart.life = 3f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = vec * 128f;
			tmpPart.position = pos;
			tmpPart.scale.X = 32f + 128f * NextRandFloat();
			tmpPart.scale.Y = 32f + 32f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 7;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 60;
			tmpPart.diffuse.G = 80;
			tmpPart.diffuse.B = 60;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 32f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnMGNestExplosion(Vector3 hitPoint)
	{
		Vector3 vec = Vector3.Zero;
		for (int i = 0; i < 8; i++)
		{
			math.RandomVector(ref vec);
			tmpPart.flag = 0u;
			tmpPart.life = 0.15f + NextRandFloat() * 0.15f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = vec;
			tmpPart.velocity.Y = NextRandFloat();
			tmpPart.velocity.X *= 4f;
			tmpPart.velocity.Y *= 16f;
			tmpPart.velocity.Z *= 4f;
			tmpPart.position = hitPoint + tmpPart.velocity * 2f;
			tmpPart.scale.X = 256f + 256f * NextRandFloat();
			tmpPart.scale.Y = 256f + 256f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 5;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = 200;
			tmpPart.sizeScale = 1.01f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int j = 0; j < 4; j++)
		{
			NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 0.2f + NextRandFloat() * 0.2f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = Vector3.Zero;
			tmpPart.position = hitPoint;
			tmpPart.scale.X = 480f + NextRandFloat() * 456f;
			tmpPart.scale.Y = 480f + NextRandFloat() * 456f;
			tmpPart.scale.Z = 480f + NextRandFloat() * 456f;
			tmpPart.textureOffset = 4;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 0.99f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int k = 0; k < 48; k++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = 4f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity.X = NextRandFloat() * 12f - 6f;
			tmpPart.velocity.Y = NextRandFloat() * 16f + 12f;
			tmpPart.velocity.Z = NextRandFloat() * 12f - 6f;
			tmpPart.position = hitPoint + tmpPart.velocity * 8f;
			tmpPart.scale.X = NextRandFloat() * 96f - 96f;
			tmpPart.scale.Y = tmpPart.scale.X;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 7;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 60;
			tmpPart.diffuse.G = 40;
			tmpPart.diffuse.B = 40;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 32f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int l = 0; l < 48; l++)
		{
			math.RandomVector(ref vec);
			tmpPart.flag = 0u;
			tmpPart.life = 3f + NextRandFloat();
			tmpPart.delay = 0f;
			tmpPart.alpha = 0.75f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vec;
			tmpPart.velocity.Y = NextRandFloat();
			tmpPart.velocity.X *= 12f;
			tmpPart.velocity.Y *= 8f;
			tmpPart.velocity.Z *= 12f;
			tmpPart.position = hitPoint;
			tmpPart.position.X += vec.X * 180f;
			tmpPart.position.Z += vec.Z * 180f;
			tmpPart.scale.X = 400f + 480f * NextRandFloat();
			tmpPart.scale.Y = 400f + 480f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 80;
			tmpPart.diffuse.G = 70;
			tmpPart.diffuse.B = 70;
			tmpPart.sizeScale = 0.999f;
			tmpPart.velocityScale = 0.975f;
			tmpPart.gravity = 8f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int m = 0; m < 16; m++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = 3f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = NextRandFloat() * 2f - 1f;
			tmpPart.velocity.Y = NextRandFloat() + 0.5f;
			tmpPart.velocity.Z = NextRandFloat() * 2f - 1f;
			tmpPart.position = hitPoint + tmpPart.velocity * 32f;
			tmpPart.position.Y += tmpPart.velocity.Y * 128f;
			tmpPart.scale.X = 428f + NextRandFloat() * 428f;
			tmpPart.scale.Y = 428f + NextRandFloat() * 428f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 80;
			tmpPart.diffuse.G = 80;
			tmpPart.diffuse.B = 80;
			tmpPart.sizeScale = 1.005f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnLandMineExplosion(Vector3 hitPoint)
	{
		Vector3 vec = Vector3.Zero;
		for (int i = 0; i < 8; i++)
		{
			math.RandomVector(ref vec);
			tmpPart.flag = 0u;
			tmpPart.life = 0.15f + NextRandFloat() * 0.15f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = vec;
			tmpPart.velocity.Y = NextRandFloat();
			tmpPart.velocity.X *= 4f;
			tmpPart.velocity.Y *= 16f;
			tmpPart.velocity.Z *= 4f;
			tmpPart.position = hitPoint + tmpPart.velocity * 2f;
			tmpPart.scale.X = 180f + 180f * NextRandFloat();
			tmpPart.scale.Y = 180f + 180f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 5;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = 200;
			tmpPart.sizeScale = 1.01f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int j = 0; j < 4; j++)
		{
			NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 0.2f + NextRandFloat() * 0.2f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = Vector3.Zero;
			tmpPart.position = hitPoint;
			tmpPart.scale.X = 280f + NextRandFloat() * 256f;
			tmpPart.scale.Y = 280f + NextRandFloat() * 256f;
			tmpPart.scale.Z = 280f + NextRandFloat() * 256f;
			tmpPart.textureOffset = 4;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 0.99f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int k = 0; k < 48; k++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = 2f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity.X = NextRandFloat() * 8f - 4f;
			tmpPart.velocity.Y = NextRandFloat() * 16f + 8f;
			tmpPart.velocity.Z = NextRandFloat() * 8f - 4f;
			tmpPart.position = hitPoint + tmpPart.velocity * 8f;
			tmpPart.scale.X = 16f;
			tmpPart.scale.Y = 24f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 7;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 60;
			tmpPart.diffuse.G = 40;
			tmpPart.diffuse.B = 40;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 32f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int l = 0; l < 48; l++)
		{
			math.RandomVector(ref vec);
			tmpPart.flag = 0u;
			tmpPart.life = 2f + NextRandFloat();
			tmpPart.delay = 0f;
			tmpPart.alpha = 0.75f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vec;
			tmpPart.velocity.Y = NextRandFloat();
			tmpPart.velocity.X *= 12f;
			tmpPart.velocity.Y *= 8f;
			tmpPart.velocity.Z *= 12f;
			tmpPart.position = hitPoint;
			tmpPart.position.X += vec.X * 180f;
			tmpPart.position.Z += vec.Z * 180f;
			tmpPart.scale.X = 200f + 180f * NextRandFloat();
			tmpPart.scale.Y = 200f + 180f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 80;
			tmpPart.diffuse.G = 70;
			tmpPart.diffuse.B = 70;
			tmpPart.sizeScale = 0.999f;
			tmpPart.velocityScale = 0.975f;
			tmpPart.gravity = 8f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int m = 0; m < 16; m++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = 2f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = NextRandFloat() * 2f - 1f;
			tmpPart.velocity.Y = NextRandFloat() + 0.5f;
			tmpPart.velocity.Z = NextRandFloat() * 2f - 1f;
			tmpPart.position = hitPoint + tmpPart.velocity * 32f;
			tmpPart.position.Y += tmpPart.velocity.Y * 128f;
			tmpPart.scale.X = 128f + NextRandFloat() * 128f;
			tmpPart.scale.Y = 128f + NextRandFloat() * 128f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 80;
			tmpPart.diffuse.G = 80;
			tmpPart.diffuse.B = 80;
			tmpPart.sizeScale = 1.005f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int n = 0; n < 6; n++)
		{
			NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 1.75f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = NextRandFloat() * 4f - 2f;
			tmpPart.velocity.Y = NextRandFloat() * 4f + 2f;
			tmpPart.velocity.Z = NextRandFloat() * 4f - 2f;
			tmpPart.position = hitPoint + tmpPart.velocity * 48f;
			tmpPart.scale.X = 128f + NextRandFloat() * 128f;
			tmpPart.scale.Y = 128f + NextRandFloat() * 128f;
			tmpPart.scale.Z = 128f + NextRandFloat() * 128f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.015f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnCabinExplosion(Vector3 hitPoint)
	{
		Vector3 vec = Vector3.Zero;
		for (int i = 0; i < 8; i++)
		{
			math.RandomVector(ref vec);
			tmpPart.flag = 0u;
			tmpPart.life = 0.5f + NextRandFloat() * 0.15f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = vec;
			tmpPart.velocity.Y = NextRandFloat();
			tmpPart.velocity.X *= 4f;
			tmpPart.velocity.Y *= 16f;
			tmpPart.velocity.Z *= 4f;
			tmpPart.position = hitPoint + tmpPart.velocity * 6f;
			tmpPart.scale.X = 512f + 512f * NextRandFloat();
			tmpPart.scale.Y = 512f + 512f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 5;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = 200;
			tmpPart.sizeScale = 1.02f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int j = 0; j < 12; j++)
		{
			math.RandomVector(ref vec);
			vec.Y = NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 10f;
			tmpPart.delay = NextRandFloat() * 2f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vec * 4f;
			tmpPart.position = hitPoint + vec * 128f;
			tmpPart.scale.X = 1600f + 1024f * NextRandFloat();
			tmpPart.scale.Y = 1600f + 1024f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 120;
			tmpPart.diffuse.G = 120;
			tmpPart.diffuse.B = 120;
			tmpPart.sizeScale = 1.0005f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int k = 0; k < 48; k++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = 4f;
			tmpPart.delay = 0.3f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity.X = NextRandFloat() * 24f - 12f;
			tmpPart.velocity.Y = NextRandFloat() * 32f + 24f;
			tmpPart.velocity.Z = NextRandFloat() * 24f - 12f;
			tmpPart.position = hitPoint + tmpPart.velocity * 12f;
			tmpPart.scale.X = NextRandFloat() * 96f - 96f;
			tmpPart.scale.Y = tmpPart.scale.X;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 7;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 60;
			tmpPart.diffuse.G = 40;
			tmpPart.diffuse.B = 40;
			tmpPart.sizeScale = 1f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 32f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int l = 0; l < 48; l++)
		{
			math.RandomVector(ref vec);
			tmpPart.flag = 0u;
			tmpPart.life = 3f + NextRandFloat();
			tmpPart.delay = 0.3f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vec;
			tmpPart.velocity.Y = NextRandFloat();
			tmpPart.velocity.X *= 20f;
			tmpPart.velocity.Y *= 16f;
			tmpPart.velocity.Z *= 20f;
			tmpPart.position = hitPoint;
			tmpPart.position.X += vec.X * 512f;
			tmpPart.position.Z += vec.Z * 512f;
			tmpPart.scale.X = 400f + 480f * NextRandFloat();
			tmpPart.scale.Y = 400f + 480f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 80;
			tmpPart.diffuse.G = 70;
			tmpPart.diffuse.B = 70;
			tmpPart.sizeScale = 0.999f;
			tmpPart.velocityScale = 0.975f;
			tmpPart.gravity = 8f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int m = 0; m < 16; m++)
		{
			tmpPart.flag = 0u;
			tmpPart.life = 3f;
			tmpPart.delay = 0.3f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = NextRandFloat() * 2f - 1f;
			tmpPart.velocity.Y = NextRandFloat() + 0.25f;
			tmpPart.velocity.Z = NextRandFloat() * 2f - 1f;
			tmpPart.position = hitPoint + tmpPart.velocity * 256f;
			tmpPart.position.Y += tmpPart.velocity.Y * 800f;
			tmpPart.velocity *= 2f;
			tmpPart.scale.X = 428f + NextRandFloat() * 428f;
			tmpPart.scale.Y = 428f + NextRandFloat() * 428f;
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 80;
			tmpPart.diffuse.G = 80;
			tmpPart.diffuse.B = 80;
			tmpPart.sizeScale = 1.005f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int n = 0; n < 6; n++)
		{
			NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 0.5f;
			tmpPart.delay = 0.3f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity.X = NextRandFloat() * 4f - 2f;
			tmpPart.velocity.Y = NextRandFloat() * 4f + 2f;
			tmpPart.velocity.Z = NextRandFloat() * 4f - 2f;
			tmpPart.position = hitPoint + tmpPart.velocity * 48f;
			tmpPart.scale.X = 512f + NextRandFloat() * 512f;
			tmpPart.scale.Y = 512f + NextRandFloat() * 512f;
			tmpPart.scale.Z = 512f + NextRandFloat() * 512f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = byte.MaxValue;
			tmpPart.diffuse.G = byte.MaxValue;
			tmpPart.diffuse.B = byte.MaxValue;
			tmpPart.sizeScale = 1.075f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = true;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnHeliHitSmoke(Vector3 pos, Vector3 vel)
	{
		Vector3 vec = Vector3.Zero;
		math.RandomVector(ref vec);
		tmpPart.flag = 0u;
		tmpPart.life = 2f;
		tmpPart.delay = 0f;
		tmpPart.alpha = 1f;
		tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
		tmpPart.velocity = vel * 24f;
		tmpPart.position = pos + vec * 128f;
		tmpPart.scale.X = 128f + 128f * NextRandFloat();
		tmpPart.scale.Y = 128f + 128f * NextRandFloat();
		tmpPart.scale.Z = 0f;
		tmpPart.textureOffset = 0;
		tmpPart.diffuse.A = byte.MaxValue;
		tmpPart.diffuse.R = 80;
		tmpPart.diffuse.G = 80;
		tmpPart.diffuse.B = 80;
		tmpPart.sizeScale = 1.0125f;
		tmpPart.velocityScale = 0.99f;
		tmpPart.gravity = -4f;
		tmpPart.distortion = false;
		tmpPart.uvDistortion = 0f;
		tmpPart.softParticle = 216;
		Spawn(tmpPart);
	}

	public static void SpawnHeliCrash(Vector3 pos)
	{
		Vector3 vec = Vector3.Zero;
		for (int i = 0; i < 12; i++)
		{
			math.RandomVector(ref vec);
			vec.Y = NextRandFloat();
			tmpPart.flag = 0u;
			tmpPart.life = 10f;
			tmpPart.delay = NextRandFloat() * 2f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = tmpPart.alpha / tmpPart.life;
			tmpPart.velocity = vec * 4f;
			tmpPart.position = pos + vec * 128f;
			tmpPart.scale.X = 1600f + 1024f * NextRandFloat();
			tmpPart.scale.Y = 1600f + 1024f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 0;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 120;
			tmpPart.diffuse.G = 120;
			tmpPart.diffuse.B = 120;
			tmpPart.sizeScale = 1.0005f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 0f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
		for (int j = 0; j < 24; j++)
		{
			math.RandomVector(ref vec);
			vec.Y = NextRandFloat() * 2f;
			tmpPart.flag = 0u;
			tmpPart.life = 3.5f;
			tmpPart.delay = 0f;
			tmpPart.alpha = 1f;
			tmpPart.alphaStep = 0f;
			tmpPart.velocity = vec * 24f;
			tmpPart.position = pos + vec * 16f;
			tmpPart.scale.X = 128f + 128f * NextRandFloat();
			tmpPart.scale.Y = 128f + 128f * NextRandFloat();
			tmpPart.scale.Z = 0f;
			tmpPart.textureOffset = 7;
			tmpPart.diffuse.A = byte.MaxValue;
			tmpPart.diffuse.R = 60;
			tmpPart.diffuse.G = 80;
			tmpPart.diffuse.B = 60;
			tmpPart.sizeScale = 0.9975f;
			tmpPart.velocityScale = 1f;
			tmpPart.gravity = 32f;
			tmpPart.distortion = false;
			tmpPart.uvDistortion = 0f;
			tmpPart.softParticle = 216;
			Spawn(tmpPart);
		}
	}

	public static void SpawnFog(Vector3 pos, Vector3 dir)
	{
		Vector3 zero = Vector3.Zero;
		_ = Vector3.Zero;
		zero.Y = 0f;
		zero.X = NextRandFloat() * 2f - 1f;
		zero.Z = NextRandFloat() * 2f - 1f;
		tmpPart.flag = 1u;
		tmpPart.life = 6f;
		tmpPart.delay = NextRandFloat() * 2f;
		tmpPart.alpha = 0f;
		tmpPart.alphaStep = 0.35f / tmpPart.life * -1f;
		tmpPart.velocity.X = 0.1f + NextRandFloat();
		tmpPart.velocity.Y = -0.05f;
		tmpPart.velocity.Z = 0f;
		tmpPart.position = pos + zero * 1500f;
		tmpPart.position.Y = 0f;
		tmpPart.scale.X = 256f + 256f * NextRandFloat();
		tmpPart.scale.Y = 128f + 128f * NextRandFloat();
		tmpPart.scale.Z = 256f + 256f * NextRandFloat();
		tmpPart.textureOffset = 0;
		tmpPart.diffuse.A = 200;
		tmpPart.diffuse.R = 200;
		tmpPart.diffuse.G = 200;
		tmpPart.diffuse.B = 200;
		tmpPart.rotation = 0f;
		tmpPart.sizeScale = 0.99995f;
		tmpPart.velocityScale = 1f;
		tmpPart.gravity = 0.025f;
		tmpPart.distortion = false;
		tmpPart.uvDistortion = 0f;
		tmpPart.softParticle = 216;
		Spawn(tmpPart);
	}
}
