using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Meshes.Generic;
using Quasar.Shaders;
using Quasar.Textures;
using Quasar.Utils;

namespace Quasar.Particles;

public class TrailGroup : UserIndexVertexMesh<BillboardVertex>, IParticleGroup
{
	internal struct Trail
	{
		public float RelativeTime;

		public float BirthTime;

		public float Lifespan;

		public float Friction;

		public float Gravity;

		public Vector3 Position;

		public Vector3 TrailPosition;

		public Vector3 Speed;

		public Vector3 TrailSpeed;

		public Vector3 StartSpeed;

		public float GravitySpeed;

		public float TrailGravitySpeed;

		public float FrictionValue;

		public Trail(float time, ref Vector3 position, ref Vector3 speed, TrailGroup group)
		{
			RelativeTime = 0f;
			BirthTime = time;
			Lifespan = group.lifespan.Next();
			Friction = group.friction.Next();
			Gravity = group.gravity.Next();
			TrailPosition = (Position = position);
			TrailSpeed = (Speed = (StartSpeed = speed));
			FrictionValue = 1f;
			TrailGravitySpeed = (GravitySpeed = 0f);
		}

		internal bool Update(float totalTime, float intervalTime, TrailGroup group)
		{
			RelativeTime = (totalTime - BirthTime) / Lifespan;
			float num = (totalTime - BirthTime - group.trailDuration) / Lifespan;
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
				if (num >= 0f)
				{
					float num2 = Math.Max(0f, 1f - Friction * num);
					TrailSpeed.X = StartSpeed.X * num2;
					TrailSpeed.Y = StartSpeed.Y * num2;
					TrailSpeed.Z = StartSpeed.Z * num2;
				}
			}
			else
			{
				Speed = StartSpeed;
				TrailSpeed = StartSpeed;
			}
			if (group.gravity.Valid)
			{
				GravitySpeed += Gravity * intervalTime;
				Speed.X += group.gravityVector.X * GravitySpeed;
				Speed.Y += group.gravityVector.Y * GravitySpeed;
				Speed.Z += group.gravityVector.Z * GravitySpeed;
				if (num >= 0f)
				{
					TrailGravitySpeed += Gravity * intervalTime;
					TrailSpeed.X += group.gravityVector.X * TrailGravitySpeed;
					TrailSpeed.Y += group.gravityVector.Y * TrailGravitySpeed;
					TrailSpeed.Z += group.gravityVector.Z * TrailGravitySpeed;
				}
			}
			Position.X += Speed.X * intervalTime;
			Position.Y += Speed.Y * intervalTime;
			Position.Z += Speed.Z * intervalTime;
			if (num >= 0f)
			{
				TrailPosition.X += TrailSpeed.X * intervalTime;
				TrailPosition.Y += TrailSpeed.Y * intervalTime;
				TrailPosition.Z += TrailSpeed.Z * intervalTime;
			}
			return true;
		}
	}

	private const string PARTICLES_DIR = "Particles/";

	private Quasar.Particles.TrailMesh trailMesh;

	private string name;

	private ParticleSystem particleSystem;

	public bool WorldTransform;

	public Vector3 gravityVector = new Vector3(0f, -1f, 0f);

	public float globalScale = 1f;

	public float trailSize = 1f;

	public FuzzyValue<float> lifespan;

	public FuzzyValue<float> friction;

	public FuzzyValue<float> gravity;

	public KeyedValue<Vector3> color;

	public KeyedValue<float> alpha;

	public KeyedValue<float> size;

	public float trailDuration = float.MaxValue;

	private bool autoUpdateSphere;

	private int maxParticleCount;

	internal Trail[] trails;

	internal int currentParticleCount;

	public string Name => name;

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

	public int TrailRenderPriority
	{
		get
		{
			return (int)(trailMesh.FirstMaterial.RenderPriority - 50);
		}
		set
		{
			trailMesh.FirstMaterial.RenderPriority = (Material.Priority)(50 + value);
		}
	}

	public int MaxParticleCount => maxParticleCount;

	public int AvailableParticles => maxParticleCount - currentParticleCount;

	Mesh IParticleGroup.Mesh => this;

	public TrailGroup(string name, ParticleSystem system, int maxParticleCount)
	{
		hasSimpleLayout = true;
		this.name = name;
		particleSystem = system;
		trailMesh = new Quasar.Particles.TrailMesh(this);
		SetMaxParticleCount(maxParticleCount);
		DebugColor = Color.SeaGreen;
		Material material = new Material();
		material.AddIntParameter(0);
		material.AddIntParameter(1);
		material.AddIntParameter(1);
		material.AddFloatParameter(0f);
		material.SetForcedAlpha(alpha: true);
		material.RenderPriority = Material.Priority.Low;
		materials.Add(material);
	}

	public void SetMaxParticleCount(int maxParticleCount)
	{
		maxParticleCount = Math.Min(16383, maxParticleCount);
		this.maxParticleCount = maxParticleCount;
		trails = new Trail[maxParticleCount];
		initMesh(maxParticleCount);
		currentParticleCount = 0;
		trailMesh.InitMesh(maxParticleCount);
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

	internal static TrailGroup ParseXml(XElement xe, ParticleSystem p)
	{
		TrailGroup trailGroup = new TrailGroup(XDocHelper.GetAttribute(xe, "name"), p, Math.Min(268435455, XDocHelper.ParseIntAttribute(xe, "maxParticles")));
		trailGroup.WorldTransform = XDocHelper.ParseBoolAttribute(xe, "worldTransform", p.WorldTransform);
		trailGroup.Texture = TextureManager.Textures["Particles/" + XDocHelper.GetAttribute(xe, "texture")];
		trailGroup.trailMesh.Texture = TextureManager.Textures["Particles/" + XDocHelper.GetAttribute(xe, "trail_texture")];
		trailGroup.Shader = ShaderManager.Shaders["Particles/" + XDocHelper.GetAttribute(xe, "shader")];
		trailGroup.trailMesh.Shader = ShaderManager.Shaders["Particles/" + XDocHelper.GetAttribute(xe, "trail_shader")];
		trailGroup.gravityVector = XDocHelper.ParseVector3Attribute(xe, "gravity");
		trailGroup.globalScale = XDocHelper.ParseFloatAttribute(xe, "globalScale");
		trailGroup.trailSize = XDocHelper.ParseFloatAttribute(xe, "trail_size");
		trailGroup.lifespan = FuzzyValue.ParseXmlFloat(xe, "Lifespan");
		trailGroup.gravity = FuzzyValue.ParseXmlFloat(xe, "GravityFactor");
		trailGroup.friction = FuzzyValue.ParseXmlFloat(xe, "Friction");
		trailGroup.RenderPriority = XDocHelper.ParseIntAttribute(xe, "renderPriority");
		trailGroup.TrailRenderPriority = XDocHelper.ParseIntAttribute(xe, "trail_renderPriority");
		trailGroup.color = KeyedValue.ParseXmlVector3(xe, "ColorKeys");
		trailGroup.alpha = KeyedValue.ParseXmlFloat(xe, "AlphaKeys");
		trailGroup.size = KeyedValue.ParseXmlFloat(xe, "SizeKeys");
		trailGroup.autoUpdateSphere = XDocHelper.ParseBoolAttribute(xe, "autoUpdateSphere");
		trailGroup.trailDuration = XDocHelper.ParseFloatAttribute(xe, "trail_duration", trailGroup.trailDuration);
		return trailGroup;
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
			if (!trails[i].Update(totalTimeSeconds, lastIntervalSeconds, this))
			{
				ref Trail reference = ref trails[i--];
				reference = trails[currentParticleCount - 1];
				currentParticleCount--;
				continue;
			}
			if (autoUpdateSphere)
			{
				if (i == 0)
				{
					boundingSphere = new BoundingSphere(trails[i].Position, 0.0001f);
				}
				else
				{
					boundingSphere = BoundingSphere.CreateMerged(boundingSphere, new BoundingSphere(trails[i].Position, 0.0001f));
				}
			}
			setParticle(ref trails[i], ref verticesBuffer[i * 4], ref verticesBuffer[i * 4 + 1], ref verticesBuffer[i * 4 + 2], ref verticesBuffer[i * 4 + 3]);
			setTrail(ref trails[i], ref trailMesh.Vertices[i * 4], ref trailMesh.Vertices[i * 4 + 1], ref trailMesh.Vertices[i * 4 + 2], ref trailMesh.Vertices[i * 4 + 3]);
		}
		base.PrimitiveCount = 2 * currentParticleCount;
		trailMesh.PrimitiveCount = 2 * currentParticleCount;
		base.FirstMaterial.SetIntParameter(0, WorldTransform ? 1 : 0);
		base.FirstMaterial.SetFloatParameter(0, globalScale);
		trailMesh.FirstMaterial.SetIntParameter(0, WorldTransform ? 1 : 0);
		trailMesh.FirstMaterial.SetFloatParameter(0, trailSize);
	}

	public void ClearParticles()
	{
		currentParticleCount = 0;
	}

	private void setParticle(ref Trail trail, ref BillboardVertex v1, ref BillboardVertex v2, ref BillboardVertex v3, ref BillboardVertex v4)
	{
		v1.Position = (v2.Position = (v3.Position = (v4.Position = trail.Position)));
		color.GetValue(trail.RelativeTime, out v1.Color);
		v2.Color = (v3.Color = (v4.Color = v1.Color));
		alpha.GetValue(trail.RelativeTime, out v1.Alpha);
		v2.Alpha = (v3.Alpha = (v4.Alpha = v1.Alpha));
		size.GetValue(trail.RelativeTime, out v1.ParticleData.X);
		v2.ParticleData.X = (v3.ParticleData.X = (v4.ParticleData.X = v1.ParticleData.X));
		v1.ParticleData.Z = (v2.ParticleData.Z = (v3.ParticleData.Z = (v4.ParticleData.Z = trail.RelativeTime)));
	}

	private void setTrail(ref Trail trail, ref TrailVertex v1, ref TrailVertex v2, ref TrailVertex v3, ref TrailVertex v4)
	{
		v1.Position = (v3.Position = trail.Position);
		v2.Position = (v4.Position = trail.TrailPosition);
		color.GetValue(trail.RelativeTime, out v1.Color);
		v2.Color = (v3.Color = (v4.Color = v1.Color));
		alpha.GetValue(trail.RelativeTime, out v1.Alpha);
		v4.Alpha = (v2.Alpha = (v3.Alpha = v1.Alpha));
		size.GetValue(trail.RelativeTime, out v1.ParticleData.X);
		v2.ParticleData.X = (v3.ParticleData.X = (v4.ParticleData.X = v1.ParticleData.X));
		v1.ParticleData.Y = (v2.ParticleData.Y = (v3.ParticleData.Y = (v4.ParticleData.Y = trail.RelativeTime)));
	}

	public override void Render(Transform motion)
	{
		base.Render(motion);
		trailMesh.Render(motion);
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
		ref Trail reference = ref trails[currentParticleCount];
		reference = new Trail(time, ref position, ref speed, this);
		float totalTimeSeconds = particleSystem.Timer.TotalTimeSeconds;
		if (time < totalTimeSeconds)
		{
			trails[currentParticleCount].Update(totalTimeSeconds, totalTimeSeconds - time, this);
		}
		currentParticleCount++;
		return true;
	}

	public override void Dispose()
	{
		base.Dispose();
	}
}
