using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Meshes.Generic;
using Quasar.Shaders;
using Quasar.Textures;
using Quasar.Utils;

namespace Quasar.Particles;

public class BillboardGroup : UserIndexVertexMesh<BillboardVertex>, IParticleGroup
{
	private struct Particle
	{
		public float RelativeTime;

		public float BirthTime;

		public float Lifespan;

		public float Rotation;

		public float Gravity;

		public float RotationSpeed;

		public float StartRotationSpeed;

		public float Friction;

		public Vector3 Position;

		public Vector3 Speed;

		public Vector3 StartSpeed;

		public float GravitySpeed;

		public float FrictionValue;

		public int CurrentTile;

		public Particle(float time, ref Vector3 position, ref Vector3 speed, BillboardGroup group)
		{
			RelativeTime = 0f;
			BirthTime = time;
			Lifespan = group.lifespan.Next();
			Rotation = group.rotation.Next();
			Friction = group.friction.Next();
			Gravity = group.gravity.Next();
			StartRotationSpeed = (RotationSpeed = group.rotationSpeed.Next());
			Position = position;
			Speed = (StartSpeed = speed);
			GravitySpeed = 0f;
			FrictionValue = 1f;
			CurrentTile = 0;
			if (group.useTileSize)
			{
				if (group.useRandomTile)
				{
					CurrentTile = GameMath.Random.Next(group.tileCols * group.tileRows);
				}
				else if (group.useTileAnimation)
				{
					CurrentTile = group.tileAnimationOffset;
				}
			}
		}

		internal bool Update(float totalTime, float intervalTime, BillboardGroup group)
		{
			RelativeTime = (totalTime - BirthTime) / Lifespan;
			if (RelativeTime > 1f)
			{
				return false;
			}
			if (group.friction.Valid)
			{
				FrictionValue = Math.Max(0f, 1f - Friction * RelativeTime);
				Speed.X = StartSpeed.X * FrictionValue;
				Speed.Y = StartSpeed.Y * FrictionValue;
				Speed.Z = StartSpeed.Z * FrictionValue;
				if (group.rotationSpeed.Valid)
				{
					RotationSpeed = StartRotationSpeed * FrictionValue;
				}
			}
			else
			{
				Speed = StartSpeed;
				if (group.rotationSpeed.Valid)
				{
					RotationSpeed = StartRotationSpeed;
				}
			}
			if (group.gravity.Valid)
			{
				GravitySpeed += Gravity * intervalTime;
				Speed.X += group.gravityVector.X * GravitySpeed;
				Speed.Y += group.gravityVector.Y * GravitySpeed;
				Speed.Z += group.gravityVector.Z * GravitySpeed;
			}
			Position.X += Speed.X * intervalTime;
			Position.Y += Speed.Y * intervalTime;
			Position.Z += Speed.Z * intervalTime;
			if (group.rotationSpeed.Valid)
			{
				Rotation += RotationSpeed * intervalTime;
			}
			if (group.useTileSize && group.useTileAnimation)
			{
				float num = GameMath.Mod(RelativeTime * (float)group.tileAnimationCycles, 1f);
				CurrentTile = group.tileAnimationOffset + (int)(num * (float)group.tileAnimationLength);
			}
			return true;
		}
	}

	private const string PARTICLES_DIR = "Particles/";

	private string name;

	private ParticleSystem particleSystem;

	public bool WorldTransform;

	public Vector3 gravityVector = new Vector3(0f, -1f, 0f);

	public float globalScale = 1f;

	public FuzzyValue<float> lifespan;

	public FuzzyValue<float> rotation;

	public FuzzyValue<float> gravity;

	public FuzzyValue<float> rotationSpeed;

	public FuzzyValue<float> friction;

	public KeyedValue<Vector3> color;

	public KeyedValue<float> alpha;

	public KeyedValue<float> size;

	public bool useTileSize;

	private int tileCols = 1;

	private int tileRows = 1;

	public bool useTileAnimation;

	private int tileAnimationLength = 1;

	private int tileAnimationOffset;

	private int tileAnimationCycles = 1;

	public bool useRandomTile;

	private bool autoUpdateSphere;

	private int maxParticleCount;

	private Particle[] particles;

	private int currentParticleCount;

	public string Name => name;

	public int TileCols
	{
		get
		{
			return tileCols;
		}
		set
		{
			if (value > 0)
			{
				tileCols = value;
			}
		}
	}

	public int TileRows
	{
		get
		{
			return tileRows;
		}
		set
		{
			if (value > 0)
			{
				tileRows = value;
			}
		}
	}

	public int TileAnimationLength
	{
		get
		{
			return tileAnimationLength;
		}
		set
		{
			if (value > 0)
			{
				tileAnimationLength = value;
			}
		}
	}

	public int TileAnimationOffset
	{
		get
		{
			return tileAnimationOffset;
		}
		set
		{
			if (value >= 0)
			{
				tileAnimationOffset = value;
			}
		}
	}

	public int TileAnimationCycles
	{
		get
		{
			return tileAnimationCycles;
		}
		set
		{
			if (value >= 0)
			{
				tileAnimationCycles = value;
			}
		}
	}

	public bool AutoUpdateSphere
	{
		get
		{
			return autoUpdateSphere;
		}
		set
		{
			autoUpdateSphere = value;
			if (!autoUpdateSphere)
			{
				boundingSphere = new BoundingSphere(Vector3.Zero, -1f);
			}
		}
	}

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

	public int MaxParticleCount => maxParticleCount;

	public int AvailableParticles => maxParticleCount - currentParticleCount;

	Mesh IParticleGroup.Mesh => this;

	public BillboardGroup(string name, ParticleSystem system, int maxParticleCount)
	{
		hasSimpleLayout = true;
		this.name = name;
		particleSystem = system;
		SetMaxParticleCount(maxParticleCount);
		DebugColor = Color.SeaGreen;
		Material material = new Material();
		material.AddIntParameter(0);
		material.AddIntParameter(0);
		material.AddIntParameter(0);
		material.AddFloatParameter(0f);
		material.SetForcedAlpha(alpha: true);
		material.RenderPriority = Material.Priority.Low;
		materials.Add(material);
	}

	public void SetMaxParticleCount(int maxParticleCount)
	{
		maxParticleCount = Math.Min(8191, maxParticleCount);
		this.maxParticleCount = maxParticleCount;
		particles = new Particle[maxParticleCount];
		initMesh(maxParticleCount);
		currentParticleCount = 0;
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

	internal static BillboardGroup ParseXml(XElement xe, ParticleSystem p)
	{
		BillboardGroup billboardGroup = new BillboardGroup(XDocHelper.GetAttribute(xe, "name"), p, Math.Min(536870911, XDocHelper.ParseIntAttribute(xe, "maxParticles")));
		billboardGroup.WorldTransform = XDocHelper.ParseBoolAttribute(xe, "worldTransform", p.WorldTransform);
		billboardGroup.Texture = TextureManager.Textures["Particles/" + XDocHelper.GetAttribute(xe, "texture")];
		billboardGroup.Shader = ShaderManager.Shaders["Particles/" + XDocHelper.GetAttribute(xe, "shader")];
		billboardGroup.gravityVector = XDocHelper.ParseVector3Attribute(xe, "gravity");
		billboardGroup.globalScale = XDocHelper.ParseFloatAttribute(xe, "globalScale");
		billboardGroup.lifespan = FuzzyValue.ParseXmlFloat(xe, "Lifespan");
		billboardGroup.rotation = FuzzyValue.ParseXmlFloat(xe, "Rotation");
		billboardGroup.gravity = FuzzyValue.ParseXmlFloat(xe, "GravityFactor");
		billboardGroup.rotationSpeed = FuzzyValue.ParseXmlFloat(xe, "RotationSpeed");
		billboardGroup.friction = FuzzyValue.ParseXmlFloat(xe, "Friction");
		billboardGroup.useTileSize = XDocHelper.ParseBoolAttribute(xe, "useTileSize");
		billboardGroup.tileCols = XDocHelper.ParseIntAttribute(xe, "tileCols", 1);
		billboardGroup.tileRows = XDocHelper.ParseIntAttribute(xe, "tileRows", 1);
		billboardGroup.useTileAnimation = XDocHelper.ParseBoolAttribute(xe, "useTileAnimation");
		billboardGroup.tileAnimationLength = XDocHelper.ParseIntAttribute(xe, "tileAnimationLength");
		billboardGroup.tileAnimationOffset = XDocHelper.ParseIntAttribute(xe, "tileAnimationOffset");
		billboardGroup.tileAnimationCycles = XDocHelper.ParseIntAttribute(xe, "tileAnimationCycles");
		billboardGroup.useRandomTile = XDocHelper.ParseBoolAttribute(xe, "useRandomTile");
		billboardGroup.RenderPriority = XDocHelper.ParseIntAttribute(xe, "renderPriority");
		billboardGroup.color = KeyedValue.ParseXmlVector3(xe, "ColorKeys");
		billboardGroup.alpha = KeyedValue.ParseXmlFloat(xe, "AlphaKeys");
		billboardGroup.size = KeyedValue.ParseXmlFloat(xe, "SizeKeys");
		billboardGroup.autoUpdateSphere = XDocHelper.ParseBoolAttribute(xe, "autoUpdateSphere");
		return billboardGroup;
	}

	public void Update()
	{
		float totalTimeSeconds = particleSystem.Timer.TotalTimeSeconds;
		float lastIntervalSeconds = particleSystem.Timer.LastIntervalSeconds;
		if (autoUpdateSphere)
		{
			boundingSphere.Radius = 0f;
			boundingSphereIgnoreTransform = WorldTransform;
		}
		for (int i = 0; i < currentParticleCount; i++)
		{
			if (!particles[i].Update(totalTimeSeconds, lastIntervalSeconds, this))
			{
				ref Particle reference = ref particles[i--];
				reference = particles[currentParticleCount - 1];
				currentParticleCount--;
				continue;
			}
			if (autoUpdateSphere)
			{
				if (i == 0)
				{
					boundingSphere = new BoundingSphere(particles[i].Position, 0.0001f);
				}
				else
				{
					boundingSphere = BoundingSphere.CreateMerged(boundingSphere, new BoundingSphere(particles[i].Position, 0.0001f));
				}
			}
			setParticle(ref particles[i], ref verticesBuffer[i * 4], ref verticesBuffer[i * 4 + 1], ref verticesBuffer[i * 4 + 2], ref verticesBuffer[i * 4 + 3]);
		}
		base.PrimitiveCount = 2 * currentParticleCount;
		base.FirstMaterial.SetIntParameter(0, WorldTransform ? 1 : 0);
		base.FirstMaterial.SetIntParameter(1, tileCols);
		base.FirstMaterial.SetIntParameter(2, tileRows);
		base.FirstMaterial.SetFloatParameter(0, globalScale);
	}

	public void ClearParticles()
	{
		currentParticleCount = 0;
	}

	private void setParticle(ref Particle particle, ref BillboardVertex v1, ref BillboardVertex v2, ref BillboardVertex v3, ref BillboardVertex v4)
	{
		v1.Position = (v2.Position = (v3.Position = (v4.Position = particle.Position)));
		color.GetValue(particle.RelativeTime, out v1.Color);
		v2.Color = (v3.Color = (v4.Color = v1.Color));
		alpha.GetValue(particle.RelativeTime, out v1.Alpha);
		v2.Alpha = (v3.Alpha = (v4.Alpha = v1.Alpha));
		size.GetValue(particle.RelativeTime, out v1.ParticleData.X);
		v2.ParticleData.X = (v3.ParticleData.X = (v4.ParticleData.X = v1.ParticleData.X));
		v1.ParticleData.Y = (v2.ParticleData.Y = (v3.ParticleData.Y = (v4.ParticleData.Y = particle.Rotation)));
		v1.ParticleData.Z = (v2.ParticleData.Z = (v3.ParticleData.Z = (v4.ParticleData.Z = particle.RelativeTime)));
		v1.Tile = (v2.Tile = (v3.Tile = (v4.Tile = particle.CurrentTile)));
	}

	public bool SpawnParticle(float time, ref Vector3 position, ref Vector3 speed)
	{
		if (currentParticleCount >= maxParticleCount)
		{
			return false;
		}
		if (WorldTransform)
		{
			particleSystem.Transform.TransformPoint(ref position, out position);
			particleSystem.Transform.RotateVector(ref speed, out speed);
		}
		ref Particle reference = ref particles[currentParticleCount];
		reference = new Particle(time, ref position, ref speed, this);
		float totalTimeSeconds = particleSystem.Timer.TotalTimeSeconds;
		if (time < totalTimeSeconds)
		{
			particles[currentParticleCount].Update(totalTimeSeconds, totalTimeSeconds - time, this);
		}
		currentParticleCount++;
		return true;
	}

	public override void Dispose()
	{
		base.Dispose();
	}
}
