using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Meshes;
using Quasar.Shaders;
using Quasar.Utils;

namespace Quasar.Particles;

public class InstancedGroup : InstancedMesh, IParticleGroup
{
	private struct Particle
	{
		public float RelativeTime;

		public float BirthTime;

		public float Lifespan;

		public float Gravity;

		public float Friction;

		public Vector3 Position;

		public Vector3 Speed;

		public Vector3 StartSpeed;

		public float Rotation;

		public float RotationSpeed;

		public float GravitySpeed;

		public float FrictionValue;

		public Particle(float time, ref Vector3 position, ref Vector3 speed, InstancedGroup group)
		{
			RelativeTime = 0f;
			BirthTime = time;
			Lifespan = group.lifespan.Next();
			Rotation = group.rotation.Next();
			RotationSpeed = group.rotationSpeed.Next();
			Friction = group.friction.Next();
			Gravity = group.gravity.Next();
			Position = position;
			Speed = (StartSpeed = speed);
			GravitySpeed = 0f;
			FrictionValue = 1f;
		}

		internal bool Update(float totalTime, float intervalTime, InstancedGroup group)
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
			}
			else
			{
				Speed = StartSpeed;
			}
			if (group.gravity.Valid)
			{
				GravitySpeed += Gravity * intervalTime;
				Speed.X += group.gravityVector.X * GravitySpeed;
				Speed.Y += group.gravityVector.Y * GravitySpeed;
				Speed.Z += group.gravityVector.Z * GravitySpeed;
			}
			if (group.rotationSpeed.Valid)
			{
				Rotation += RotationSpeed * intervalTime;
			}
			Position.X += Speed.X * intervalTime;
			Position.Y += Speed.Y * intervalTime;
			Position.Z += Speed.Z * intervalTime;
			return true;
		}
	}

	private const string PARTICLES_DIR = "Particles/";

	private string name;

	private ParticleSystem particleSystem;

	public bool WorldTransform;

	public Vector3 gravityVector = new Vector3(0f, -1f, 0f);

	public Vector3 rotationVector = new Vector3(0f, 0f, 1f);

	public float globalScale = 1f;

	public FuzzyValue<float> lifespan;

	public FuzzyValue<float> gravity;

	public FuzzyValue<float> friction;

	public FuzzyValue<float> rotation;

	public FuzzyValue<float> rotationSpeed;

	public KeyedValue<Vector3> color;

	public KeyedValue<float> alpha;

	public KeyedValue<float> scale;

	private int maxParticleCount;

	private Particle[] particles;

	private Vector4[] diffuseData;

	private Vector4[] particleColors;

	private int currentParticleCount;

	private Quaternion tempQ;

	public string Name => name;

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

	public InstancedGroup(string name, ParticleSystem system, string mesh, int maxParticleCount)
		: base(mesh, maxParticleCount)
	{
		this.name = name;
		particleSystem = system;
		diffuseData = new Vector4[16];
		base.FirstMaterial.AddVector4ArrayParameter(diffuseData);
		SetMaxParticleCount(maxParticleCount);
		base.FirstMaterial.AddIntParameter(0);
		DebugColor = Color.LightGray;
	}

	public void SetMaxParticleCount(int maxParticleCount)
	{
		this.maxParticleCount = maxParticleCount;
		SetMaxInstances(maxParticleCount);
		particleColors = new Vector4[maxParticleCount];
		for (int i = 0; i < maxParticleCount; i++)
		{
			ref Matrix reference = ref instanceData[i];
			reference = Matrix.Identity;
		}
		particles = new Particle[maxParticleCount];
		currentParticleCount = 0;
	}

	internal static InstancedGroup ParseXml(XElement xe, ParticleSystem p)
	{
		InstancedGroup instancedGroup = new InstancedGroup(XDocHelper.GetAttribute(xe, "name"), p, XDocHelper.GetAttribute(xe, "mesh"), XDocHelper.ParseIntAttribute(xe, "maxParticles"));
		instancedGroup.WorldTransform = XDocHelper.ParseBoolAttribute(xe, "worldTransform", p.WorldTransform);
		instancedGroup.Shader = ShaderManager.Shaders["Particles/" + XDocHelper.GetAttribute(xe, "shader")];
		instancedGroup.gravityVector = XDocHelper.ParseVector3Attribute(xe, "gravity");
		instancedGroup.rotationVector = XDocHelper.ParseVector3Attribute(xe, "rotationVector");
		instancedGroup.globalScale = XDocHelper.ParseFloatAttribute(xe, "globalScale");
		instancedGroup.rotationSpeed = FuzzyValue.ParseXmlFloat(xe, "RotationSpeed");
		instancedGroup.lifespan = FuzzyValue.ParseXmlFloat(xe, "Lifespan");
		instancedGroup.rotation = FuzzyValue.ParseXmlFloat(xe, "Rotation");
		instancedGroup.gravity = FuzzyValue.ParseXmlFloat(xe, "GravityFactor");
		instancedGroup.friction = FuzzyValue.ParseXmlFloat(xe, "Friction");
		instancedGroup.RenderPriority = XDocHelper.ParseIntAttribute(xe, "renderPriority");
		instancedGroup.color = KeyedValue.ParseXmlVector3(xe, "ColorKeys");
		instancedGroup.alpha = KeyedValue.ParseXmlFloat(xe, "AlphaKeys");
		instancedGroup.scale = KeyedValue.ParseXmlFloat(xe, "ScaleKeys");
		return instancedGroup;
	}

	public void Update()
	{
		float totalTimeSeconds = particleSystem.Timer.TotalTimeSeconds;
		float lastIntervalSeconds = particleSystem.Timer.LastIntervalSeconds;
		for (int i = 0; i < currentParticleCount; i++)
		{
			if (!particles[i].Update(totalTimeSeconds, lastIntervalSeconds, this))
			{
				ref Particle reference = ref particles[i--];
				reference = particles[currentParticleCount - 1];
				currentParticleCount--;
			}
			else
			{
				setParticle(ref particles[i], ref instanceData[i], i);
			}
		}
		instanceNumber = currentParticleCount;
		base.FirstMaterial.SetIntParameter(1, WorldTransform ? 1 : 0);
		UpdateData();
	}

	public void ClearParticles()
	{
		currentParticleCount = 0;
	}

	private void setParticle(ref Particle particle, ref Matrix m, int index)
	{
		color.GetValue(particle.RelativeTime, out var result);
		alpha.GetValue(particle.RelativeTime, out var result2);
		ref Vector4 reference = ref particleColors[index];
		reference = new Vector4(result, result2);
		scale.GetValue(particle.RelativeTime, out var result3);
		Quaternion.CreateFromAxisAngle(ref rotationVector, particle.Rotation, out tempQ);
		Matrix.CreateFromQuaternion(ref tempQ, out m);
		Matrix.Multiply(ref m, result3 * globalScale, out m);
		m.Translation = particle.Position;
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

	protected override void PrepareDrawPart(int begin, int count)
	{
		base.PrepareDrawPart(begin, count);
		Array.Copy(particleColors, begin, diffuseData, 0, count);
	}

	public override void Dispose()
	{
		base.Dispose();
	}
}
