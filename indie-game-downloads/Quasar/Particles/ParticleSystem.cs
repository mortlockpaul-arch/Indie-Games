using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Quasar.ContentPipeline;
using Quasar.Global;

namespace Quasar.Particles;

public class ParticleSystem : RenderItem
{
	public const string PARTICLE_DIR = "Particles/";

	public bool EnableEmitters = true;

	public bool WorldTransform;

	private Dictionary<string, IParticleGroup> groups;

	private Dictionary<string, ParticleEmitter> emitters;

	private List<ParticleEmitter> unnamedEmitters;

	private List<IParticleGroup> unnamedGroups;

	private Timer timer;

	public Dictionary<string, IParticleGroup> Groups => groups;

	public Dictionary<string, ParticleEmitter> Emitters => emitters;

	public Timer Timer
	{
		get
		{
			return timer;
		}
		set
		{
			if (value == null)
			{
				timer = Timer.DefaultTimer;
			}
			else
			{
				timer = value;
			}
		}
	}

	public ParticleSystem()
	{
		groups = new Dictionary<string, IParticleGroup>(4);
		emitters = new Dictionary<string, ParticleEmitter>(4);
		unnamedEmitters = new List<ParticleEmitter>(4);
		unnamedGroups = new List<IParticleGroup>(4);
		Timer = null;
	}

	public void AddParticleGroup(IParticleGroup pg)
	{
		if (pg.Name.Length > 0)
		{
			groups.Add(pg.Name, pg);
		}
		else
		{
			unnamedGroups.Add(pg);
		}
		addMesh(pg.Mesh);
	}

	public void AddParticleEmitter(ParticleEmitter pe)
	{
		if (pe.Name.Length > 0)
		{
			emitters.Add(pe.Name, pe);
		}
		else
		{
			unnamedEmitters.Add(pe);
		}
	}

	public void RemoveParticleEmitter(string emitterName)
	{
		emitters.Remove(emitterName);
	}

	public void RemoveParticleEmitter(ParticleEmitter emitter)
	{
		if (emitter.Name.Length > 0)
		{
			RemoveParticleEmitter(emitter.Name);
		}
		else
		{
			unnamedEmitters.Remove(emitter);
		}
	}

	public void RemoveParticleGroup(string groupName)
	{
		IParticleGroup particleGroup = getParticleGroup(groupName);
		if (particleGroup != null)
		{
			removeMesh(particleGroup.Mesh);
			groups.Remove(groupName);
		}
	}

	public void RemoveParticleGroup(IParticleGroup group)
	{
		if (group.Name.Length > 0)
		{
			RemoveParticleGroup(group.Name);
			return;
		}
		unnamedGroups.Remove(group);
		removeMesh(group.Mesh);
	}

	public static ParticleSystem Load(string name)
	{
		try
		{
			XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>("Particles/" + name);
			XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
			ParticleSystem particleSystem = new ParticleSystem();
			particleSystem.WorldTransform = XDocHelper.ParseBoolAttribute(xDocument.Root, "worldTransform", defaultValue: true);
			XElement xElement = xDocument.Root.Element("Particles");
			foreach (XElement item in xElement.Elements())
			{
				IParticleGroup particleGroup = ParseParticleGroup(item, particleSystem);
				if (particleGroup != null)
				{
					particleSystem.AddParticleGroup(particleGroup);
				}
			}
			XElement xElement2 = xDocument.Root.Element("Emitters");
			foreach (XElement item2 in xElement2.Elements())
			{
				ParticleEmitter particleEmitter = ParticleEmitter.ParseParticleEmitter(item2, particleSystem);
				if (particleEmitter != null)
				{
					particleSystem.AddParticleEmitter(particleEmitter);
				}
			}
			return particleSystem;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public IParticleGroup getParticleGroup(string name)
	{
		IParticleGroup value = null;
		if (groups.TryGetValue(name, out value))
		{
			return value;
		}
		return null;
	}

	public ParticleEmitter getEmitter(string name)
	{
		ParticleEmitter value = null;
		if (emitters.TryGetValue(name, out value))
		{
			return value;
		}
		return null;
	}

	public void ClearBursts()
	{
		foreach (KeyValuePair<string, ParticleEmitter> emitter in emitters)
		{
			emitter.Value.ClearBursts();
		}
		foreach (ParticleEmitter unnamedEmitter in unnamedEmitters)
		{
			unnamedEmitter.ClearBursts();
		}
	}

	public void ClearParticles()
	{
		ClearBursts();
		foreach (KeyValuePair<string, IParticleGroup> group in groups)
		{
			group.Value.ClearParticles();
		}
		foreach (IParticleGroup unnamedGroup in unnamedGroups)
		{
			unnamedGroup.ClearParticles();
		}
	}

	public void Burst(Vector3 position)
	{
		Burst(ref position);
	}

	public void Burst(ref Vector3 position)
	{
		foreach (KeyValuePair<string, ParticleEmitter> emitter in emitters)
		{
			emitter.Value.Burst(ref position);
		}
		foreach (ParticleEmitter unnamedEmitter in unnamedEmitters)
		{
			unnamedEmitter.Burst(ref position);
		}
	}

	public void Burst(Vector3 position, Quaternion rotation)
	{
		Burst(ref position, ref rotation);
	}

	public void Burst(ref Vector3 position, ref Quaternion rotation)
	{
		foreach (KeyValuePair<string, ParticleEmitter> emitter in emitters)
		{
			emitter.Value.Burst(ref position, ref rotation);
		}
		foreach (ParticleEmitter unnamedEmitter in unnamedEmitters)
		{
			unnamedEmitter.Burst(ref position, ref rotation);
		}
	}

	public void Burst()
	{
		foreach (KeyValuePair<string, ParticleEmitter> emitter in emitters)
		{
			emitter.Value.Burst();
		}
		foreach (ParticleEmitter unnamedEmitter in unnamedEmitters)
		{
			unnamedEmitter.Burst();
		}
	}

	protected override void DoUpdate()
	{
		if (EnableEmitters)
		{
			foreach (KeyValuePair<string, ParticleEmitter> emitter in emitters)
			{
				emitter.Value.Update();
			}
			foreach (ParticleEmitter unnamedEmitter in unnamedEmitters)
			{
				unnamedEmitter.Update();
			}
		}
		foreach (KeyValuePair<string, IParticleGroup> group in groups)
		{
			group.Value.Update();
		}
		foreach (IParticleGroup unnamedGroup in unnamedGroups)
		{
			unnamedGroup.Update();
		}
		base.DoUpdate();
	}

	private static IParticleGroup ParseParticleGroup(XElement xe, ParticleSystem pe)
	{
		return xe.Name.LocalName switch
		{
			"Billboard" => BillboardGroup.ParseXml(xe, pe), 
			"Trail" => TrailGroup.ParseXml(xe, pe), 
			"Instanced" => InstancedGroup.ParseXml(xe, pe), 
			_ => null, 
		};
	}

	public override void Dispose()
	{
		base.Dispose();
	}
}
