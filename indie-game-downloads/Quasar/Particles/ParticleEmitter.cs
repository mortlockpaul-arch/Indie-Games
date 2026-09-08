using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.Global;
using Quasar.Particles.Emitters;
using Quasar.Utils;

namespace Quasar.Particles;

public abstract class ParticleEmitter
{
	private struct BurstData
	{
		public Vector3 origin;

		public Quaternion rotation;

		public float startTime;

		public bool started;

		public float birthOffset;

		public BurstData(Vector3 origin, Quaternion rotation, float startTime)
		{
			this.origin = origin;
			this.startTime = startTime;
			birthOffset = 1f;
			started = false;
			this.rotation = rotation;
		}

		public BurstData(Vector3 origin, float startTime)
		{
			this.origin = origin;
			this.startTime = startTime;
			birthOffset = 1f;
			started = false;
			rotation = Quaternion.Identity;
		}

		public BurstData(float startTime)
		{
			origin = Vector3.Zero;
			this.startTime = startTime;
			birthOffset = 1f;
			started = false;
			rotation = Quaternion.Identity;
		}
	}

	public FuzzyValue<Vector3> baseSpeed;

	public IParticleGroup ParticleGroup;

	public Transform transform = new Transform();

	public float birthRate = 2f;

	private float birthOffset;

	private bool emittedInitialParticles;

	public int initialParticles;

	public int burstCount = 4;

	public float burstTime = 1f;

	public float burstTimeOffset;

	public float burstBirthRate = 1f;

	private List<BurstData> bursts = new List<BurstData>(4);

	protected ParticleSystem particleSystem;

	private static Vector3 zeroVector = Vector3.Zero;

	private static Quaternion zeroQuaternion = Quaternion.Identity;

	protected bool enabled = true;

	public string Name { get; set; }

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
			if (!value)
			{
				birthOffset = 0f;
			}
		}
	}

	public ParticleEmitter(string name, ParticleSystem system)
	{
		Name = name;
		particleSystem = system;
	}

	internal static ParticleEmitter ParseParticleEmitter(XElement xe, ParticleSystem p)
	{
		return xe.Name.LocalName switch
		{
			"OmniEmitter" => OmniEmitter.Load(xe, p), 
			"SphereEmitter" => SphereEmitter.Load(xe, p), 
			"ConeEmitter" => ConeEmitter.Load(xe, p), 
			"DiskEmitter" => DiskEmitter.Load(xe, p), 
			"CircleEmitter" => CircleEmitter.Load(xe, p), 
			"RectangleEmitter" => RectangleEmitter.Load(xe, p), 
			"LineEmitter" => LineEmitter.Load(xe, p), 
			"BoxEmitter" => BoxEmitter.Load(xe, p), 
			_ => null, 
		};
	}

	public void Burst()
	{
		bursts.Add(new BurstData(particleSystem.Timer.TotalTimeSeconds + burstTimeOffset));
	}

	public void Burst(ref Vector3 position)
	{
		bursts.Add(new BurstData(position, particleSystem.Timer.TotalTimeSeconds + burstTimeOffset));
	}

	public void Burst(ref Vector3 position, ref Quaternion rotation)
	{
		bursts.Add(new BurstData(position, rotation, particleSystem.Timer.TotalTimeSeconds + burstTimeOffset));
	}

	protected bool Emit(float time)
	{
		return Emit(ref zeroVector, ref zeroQuaternion, time);
	}

	public void ClearBursts()
	{
		bursts.Clear();
	}

	protected virtual bool Emit(ref Vector3 origin, ref Quaternion rotation, float time)
	{
		Vector3 position = Vector3.Zero;
		Vector3 speed = Vector3.Zero;
		return ParticleGroup.SpawnParticle(time, ref position, ref speed);
	}

	protected virtual void doUpdate()
	{
		float totalTimeSeconds = particleSystem.Timer.TotalTimeSeconds;
		if (bursts.Count > 0)
		{
			for (int i = 0; i < bursts.Count; i++)
			{
				bool flag = false;
				BurstData value = bursts[i];
				if (totalTimeSeconds < value.startTime)
				{
					continue;
				}
				if (!value.started)
				{
					int num = burstCount;
					for (int j = 0; j < num; j++)
					{
						if (!Emit(ref value.origin, ref value.rotation, totalTimeSeconds))
						{
							birthOffset++;
						}
					}
					value.started = true;
					flag = true;
				}
				float num2 = totalTimeSeconds - value.startTime;
				if (num2 > burstTime)
				{
					bursts[i] = bursts[bursts.Count - 1];
					i--;
					bursts.RemoveAt(bursts.Count - 1);
					continue;
				}
				if (burstBirthRate > 0f)
				{
					birthParticles(burstBirthRate, ref value.origin, ref value.rotation, ref value.birthOffset);
					flag = true;
				}
				if (flag)
				{
					bursts[i] = value;
				}
			}
		}
		if (!emittedInitialParticles)
		{
			for (int k = 0; k < initialParticles; k++)
			{
				Emit(totalTimeSeconds);
			}
			emittedInitialParticles = true;
		}
		if (birthRate > 0f)
		{
			birthParticles(birthRate, ref zeroVector, ref zeroQuaternion, ref birthOffset);
		}
		else
		{
			birthOffset = 1f;
		}
	}

	private void birthParticles(float birthRate, ref Vector3 position, ref Quaternion rotation, ref float birthOffset)
	{
		float lastIntervalSeconds = particleSystem.Timer.LastIntervalSeconds;
		birthOffset += birthRate * lastIntervalSeconds;
		int num = (int)birthOffset;
		int availableParticles = ParticleGroup.AvailableParticles;
		if (availableParticles > 0)
		{
			if (num > availableParticles)
			{
				birthOffset -= num;
				birthRate = (float)availableParticles / (float)num;
				num = availableParticles;
				birthOffset += num;
			}
			float totalTimeSeconds = particleSystem.Timer.TotalTimeSeconds;
			for (int num2 = num - 1; num2 >= 0; num2--)
			{
				float num3 = ((birthRate > 0f) ? ((float)num2 / birthRate) : 0f);
				Emit(ref position, ref rotation, totalTimeSeconds - num3);
			}
			birthOffset -= num;
		}
	}

	public void Update()
	{
		if (enabled)
		{
			doUpdate();
		}
	}

	protected virtual void ParseXml(XElement xe)
	{
		XElement xElement = xe.Element("Transform");
		if (xElement != null)
		{
			transform.FromXml(xElement);
		}
		baseSpeed = FuzzyValue.ParseXmlVector3(xe, "BaseSpeed");
		ParticleGroup = particleSystem.getParticleGroup(XDocHelper.GetAttribute(xe, "particle"));
		birthRate = XDocHelper.ParseFloatAttribute(xe, "birthRate");
		burstBirthRate = XDocHelper.ParseFloatAttribute(xe, "burstBirthRate");
		burstCount = XDocHelper.ParseIntAttribute(xe, "burstCount");
		burstTime = XDocHelper.ParseFloatAttribute(xe, "burstTime");
		burstTimeOffset = XDocHelper.ParseFloatAttribute(xe, "burstTimeOffset");
		initialParticles = XDocHelper.ParseIntAttribute(xe, "initialParticles");
	}

	protected abstract ParticleEmitter CloneEmitter();

	public ParticleEmitter Clone()
	{
		ParticleEmitter particleEmitter = CloneEmitter();
		particleEmitter.baseSpeed = baseSpeed.Clone();
		particleEmitter.ParticleGroup = ParticleGroup;
		particleEmitter.birthRate = birthRate;
		particleEmitter.burstTime = burstTime;
		particleEmitter.burstTimeOffset = burstTimeOffset;
		particleEmitter.transform.Assign(transform);
		particleEmitter.enabled = enabled;
		return particleEmitter;
	}
}
