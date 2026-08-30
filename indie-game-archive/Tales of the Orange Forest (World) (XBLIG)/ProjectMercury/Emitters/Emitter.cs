#define DEBUG
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury.Modifiers;

namespace ProjectMercury.Emitters;

public class Emitter
{
	private static int CreationIndex;

	private string _name;

	private float TotalSeconds;

	public bool Enabled;

	private int _budget;

	private float _term;

	[ContentSerializerIgnore]
	public Particle[] Particles;

	private int Idle;

	private int _releaseQuantity;

	public VariableFloat ReleaseSpeed;

	public VariableFloat3 ReleaseColour;

	public VariableFloat ReleaseOpacity;

	public VariableFloat ReleaseScale;

	public VariableFloat ReleaseRotation;

	[ContentSerializer(Optional = true)]
	public Vector2 ReleaseImpulse;

	[ContentSerializer(Optional = true)]
	public string ParticleTextureAssetName;

	[ContentSerializerIgnore]
	public Texture2D ParticleTexture;

	public ModifierCollection Modifiers;

	public EmitterBlendMode BlendMode;

	[ContentSerializer(Optional = true)]
	public Vector2 TriggerOffset;

	[ContentSerializer(Optional = true)]
	public float MinimumTriggerPeriod;

	private float MostRecentTrigger;

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			Guard.ArgumentNullOrEmpty("Name", value);
			if (Name != value)
			{
				_name = value;
				OnNameChanged(EventArgs.Empty);
			}
		}
	}

	[ContentSerializerIgnore]
	public bool Initialised { get; private set; }

	public int Budget
	{
		get
		{
			return _budget;
		}
		set
		{
			Guard.IsTrue(Initialised, "Cannot alter Budget after Emitter is initialised.");
			Guard.ArgumentLessThan("Budget", value, 1);
			_budget = value;
		}
	}

	public float Term
	{
		get
		{
			return _term;
		}
		set
		{
			Guard.IsTrue(Initialised, "Cannot alter Term after Emitter is initialised.");
			Guard.ArgumentNotFinite("Term", value);
			Guard.ArgumentLessThan("Term", value, float.Epsilon);
			_term = value;
		}
	}

	public int ReleaseQuantity
	{
		get
		{
			return _releaseQuantity;
		}
		set
		{
			Guard.ArgumentLessThan("ReleaseQuantity", value, 1);
			_releaseQuantity = value;
		}
	}

	public int ActiveParticlesCount => Idle;

	public event EventHandler NameChanged;

	private static string NextEmitterName()
	{
		return $"Emitter{CreationIndex++:00}";
	}

	protected virtual void OnNameChanged(EventArgs e)
	{
		if (NameChanged != null)
		{
			NameChanged(this, e);
		}
	}

	public Emitter()
	{
		Name = NextEmitterName();
		Enabled = true;
		Modifiers = new ModifierCollection();
	}

	public virtual Emitter DeepCopy()
	{
		Emitter emitter = new Emitter();
		CopyBaseFields(emitter);
		return emitter;
	}

	protected void CopyBaseFields(Emitter emitter)
	{
		emitter.BlendMode = BlendMode;
		emitter.Budget = Budget;
		emitter.Enabled = Enabled;
		emitter.MinimumTriggerPeriod = MinimumTriggerPeriod;
		emitter.Modifiers = Modifiers.DeepCopy();
		emitter.Name = $"Copy of {Name}";
		emitter.ParticleTexture = ParticleTexture;
		emitter.ParticleTextureAssetName = string.Copy(ParticleTextureAssetName ?? string.Empty);
		emitter.ReleaseColour = ReleaseColour;
		emitter.ReleaseOpacity = ReleaseOpacity;
		emitter.ReleaseQuantity = ReleaseQuantity;
		emitter.ReleaseRotation = ReleaseRotation;
		emitter.ReleaseScale = ReleaseScale;
		emitter.ReleaseSpeed = ReleaseSpeed;
		emitter.ReleaseImpulse = ReleaseImpulse;
		emitter.Term = Term;
		emitter.TriggerOffset = TriggerOffset;
	}

	public virtual void Initialise()
	{
		Guard.IsTrue(Term < float.Epsilon, "Term property has not been assigned a valid value.");
		Guard.IsTrue(Budget < 1, "Budget property has not been assigned a valid value.");
		Particles = new Particle[Budget];
		Idle = 0;
		TotalSeconds = 0f;
		MostRecentTrigger = 0f;
		Initialised = true;
	}

	public void Initialise(int budget, float term)
	{
		Guard.ArgumentNotFinite("Term", term);
		Guard.ArgumentLessThan("budget", budget, 1);
		Guard.ArgumentLessThan("term", term, float.Epsilon);
		Initialised = false;
		Budget = budget;
		Term = term;
		Initialise();
	}

	public void Terminate()
	{
		Idle = 0;
	}

	public void ForceNextTrigger()
	{
		MostRecentTrigger = 0f;
	}

	public virtual void LoadContent(ContentManager content)
	{
		Guard.ArgumentNull("content", content);
		if (string.IsNullOrEmpty(ParticleTextureAssetName))
		{
			return;
		}
		try
		{
			if (ParticleTexture == null)
			{
				ParticleTexture = content.Load<Texture2D>(ParticleTextureAssetName);
			}
		}
		catch (ContentLoadException innerException)
		{
			string message = $"Unable to load the specified content item '{ParticleTextureAssetName}'\r\n                                                    Please check the 'ParticleTextureAssetName' property!";
			throw new ContentLoadException(message, innerException);
		}
	}

	[Obsolete("Old implementation, may still be faster in some scenarios.")]
	private void RetireParticles(int count)
	{
		Array.Copy(Particles, count, Particles, 0, Idle - count);
		Idle -= count;
	}

	private unsafe void RetireParticles(Particle* particleArray, int count)
	{
		Particle* ptr = particleArray + count;
		Particle* ptr2 = particleArray;
		int num = Idle - count;
		for (int i = 0; i < num; i++)
		{
			*ptr2 = *ptr;
			ptr++;
			ptr2++;
		}
		Idle -= count;
	}

	public unsafe void Update(float deltaSeconds)
	{
		Guard.IsFalse(Initialised, "Emitter has not been initialised.");
		Guard.ArgumentNotFinite("deltaSeconds", deltaSeconds);
		TotalSeconds += deltaSeconds;
		fixed (Particle* particles = Particles)
		{
			int num = Idle;
			while (--num >= 0)
			{
				Particle* ptr = particles + num;
				float num2 = TotalSeconds - ptr->Inception;
				if (num2 > Term)
				{
					break;
				}
				ptr->Age = num2 / Term;
				ptr->Momentum.X += ptr->Velocity.X;
				ptr->Momentum.Y += ptr->Velocity.Y;
				ptr->Velocity.X = (ptr->Velocity.Y = 0f);
				ptr->Position.X += ptr->Momentum.X * deltaSeconds;
				ptr->Position.Y += ptr->Momentum.Y * deltaSeconds;
			}
			if (num >= 0)
			{
				RetireParticles(particles, num + 1);
			}
			Modifiers.RunProcessors(deltaSeconds, particles, ActiveParticlesCount);
		}
	}

	public unsafe void Trigger(ref Vector2 triggerPosition)
	{
		Guard.IsFalse(Initialised, "Emitter has not been initialised.");
		if (!Enabled || TotalSeconds - MostRecentTrigger < MinimumTriggerPeriod)
		{
			return;
		}
		Vector2 vector = new Vector2
		{
			X = triggerPosition.X + TriggerOffset.X,
			Y = triggerPosition.Y + TriggerOffset.Y
		};
		int idle = Idle;
		for (int i = idle; i < idle + ReleaseQuantity && i < Budget; i++)
		{
			fixed (Particle* ptr = &Particles[i])
			{
				GenerateOffsetAndForce(out var offset, out var force);
				float num = ReleaseSpeed.Sample();
				ptr->Inception = TotalSeconds;
				ptr->Position.X = vector.X + offset.X;
				ptr->Position.Y = vector.Y + offset.Y;
				ptr->Velocity.X = force.X * num;
				ptr->Velocity.Y = force.Y * num;
				ptr->Momentum = ReleaseImpulse;
				ptr->Age = 0f;
				ptr->Colour = new Vector4(ReleaseColour.Sample(), ReleaseOpacity.Sample());
				ptr->Scale = ReleaseScale.Sample();
				ptr->Rotation = 0f;
				ptr->Rotate(ReleaseRotation.Sample());
			}
			Idle++;
		}
		MostRecentTrigger = TotalSeconds;
	}

	public void Trigger(Vector2 position)
	{
		Trigger(ref position);
	}

	protected virtual void GenerateOffsetAndForce(out Vector2 offset, out Vector2 force)
	{
		offset = Vector2.Zero;
		force = RandomHelper.NextUnitVector();
	}
}
