#define DEBUG
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using ProjectMercury.Controllers;
using ProjectMercury.Emitters;

namespace ProjectMercury;

public class ParticleEffect : EmitterCollection
{
	private string _name;

	public string Author;

	public string Description;

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
	public ControllerCollection Controllers { get; set; }

	public int ActiveParticlesCount
	{
		get
		{
			int num = 0;
			for (int i = 0; i < base.Count; i++)
			{
				num += base[i].ActiveParticlesCount;
			}
			return num;
		}
	}

	public event EventHandler NameChanged;

	protected virtual void OnNameChanged(EventArgs e)
	{
		if (NameChanged != null)
		{
			NameChanged(this, e);
		}
	}

	public ParticleEffect()
	{
		Name = "Particle Effect";
		Controllers = new ControllerCollection
		{
			Owner = this
		};
	}

	public virtual ParticleEffect DeepCopy()
	{
		ParticleEffect particleEffect = new ParticleEffect();
		particleEffect.Author = Author;
		particleEffect.Description = Description;
		particleEffect.Name = Name;
		ParticleEffect particleEffect2 = particleEffect;
		using (Enumerator enumerator = GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				Emitter current = enumerator.Current;
				particleEffect2.Add(current.DeepCopy());
			}
		}
		return particleEffect2;
	}

	public virtual void Trigger(Vector2 position)
	{
		if (Controllers.Count > 0)
		{
			for (int i = 0; i < Controllers.Count; i++)
			{
				Controllers[i].Trigger(ref position);
			}
		}
		else
		{
			for (int i = 0; i < base.Count; i++)
			{
				base[i].Trigger(ref position);
			}
		}
	}

	public virtual void Trigger(ref Vector2 position)
	{
		if (Controllers.Count > 0)
		{
			for (int i = 0; i < Controllers.Count; i++)
			{
				Controllers[i].Trigger(ref position);
			}
		}
		else
		{
			for (int i = 0; i < base.Count; i++)
			{
				base[i].Trigger(ref position);
			}
		}
	}

	public virtual void Initialise()
	{
		for (int i = 0; i < base.Count; i++)
		{
			base[i].Initialise();
		}
	}

	public virtual void Terminate()
	{
		for (int i = 0; i < base.Count; i++)
		{
			base[i].Terminate();
		}
	}

	public virtual void LoadContent(ContentManager content)
	{
		for (int i = 0; i < base.Count; i++)
		{
			base[i].LoadContent(content);
		}
	}

	public virtual void Update(float deltaSeconds)
	{
		if (Controllers.Count > 0)
		{
			for (int i = 0; i < Controllers.Count; i++)
			{
				Controllers[i].Update(deltaSeconds);
			}
		}
		else
		{
			for (int i = 0; i < base.Count; i++)
			{
				base[i].Update(deltaSeconds);
			}
		}
	}

	[Obsolete("Use Update(deltaSeconds) instead.", false)]
	public virtual void Update(float totalSeconds, float deltaSeconds)
	{
		Update(deltaSeconds);
	}
}
