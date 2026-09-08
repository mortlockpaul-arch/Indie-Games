using System;
using System.Xml.Linq;
using Quasar.ContentPipeline;
using Quasar.Global;

namespace Quasar.Trails;

public class TrailSystem : RenderItem
{
	public const string TRAIL_DIR = "Trails/";

	private ITrailGroup group;

	private Trail[] trails;

	private int trailCount;

	private float minTrailDistance = 0.1f;

	private int trailLength = 1;

	private float trailDuration = 1f;

	private float generationPeriod = 0.1f;

	private Timer timer;

	public ITrailGroup Group => group;

	public Trail[] Trails => trails;

	public int MaxTrailCount => trails.Length;

	public int TrailCount => trailCount;

	public float MinTrailDistance => minTrailDistance;

	public int TrailLength => trailLength;

	public float TrailDuration => trailDuration;

	public float GenerationPeriod => generationPeriod;

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

	public TrailSystem()
	{
		Timer = null;
	}

	public void SetTrailGroup(ITrailGroup tg)
	{
		if (group != null)
		{
			RemoveTrailGroup();
		}
		group = tg;
		addMesh(tg.Mesh);
	}

	public Trail CreateTrail(Transform transform)
	{
		if (trailCount >= trails.Length)
		{
			return null;
		}
		Trail trail = trails[trailCount++];
		trail.Init(transform);
		return trail;
	}

	public void RemoveTrail(Trail t)
	{
		Trail trail = trails[trailCount - 1];
		trails[t.Index] = trail;
		trails[trail.Index] = t;
		int index = trail.Index;
		trail.SetIndex(t.Index);
		t.SetIndex(index);
		trailCount--;
	}

	public void DisableTrail(Trail t)
	{
		t.Disable();
	}

	private void InitializeTrails(int trailCount)
	{
		trails = new Trail[trailCount];
		for (int i = 0; i < trailCount; i++)
		{
			trails[i] = new Trail(this, i);
		}
	}

	public void RemoveTrailGroup()
	{
		if (group != null)
		{
			removeMesh(group.Mesh);
			group = null;
		}
	}

	public static TrailSystem Load(string name)
	{
		try
		{
			XmlSource xmlSource = Engine.ContentManager.Load<XmlSource>("Trails/" + name);
			XDocument xDocument = XDocument.Parse(xmlSource.XmlCode);
			TrailSystem trailSystem = new TrailSystem();
			trailSystem.trailLength = XDocHelper.ParseIntAttribute(xDocument.Root, "trailLength", 1);
			trailSystem.minTrailDistance = XDocHelper.ParseFloatAttribute(xDocument.Root, "minTrailDistance", 0f);
			trailSystem.trailDuration = XDocHelper.ParseFloatAttribute(xDocument.Root, "trailDuration", 1f);
			trailSystem.generationPeriod = XDocHelper.ParseFloatAttribute(xDocument.Root, "generationPeriod", 0.1f);
			trailSystem.InitializeTrails(XDocHelper.ParseIntAttribute(xDocument.Root, "maxTrails", 1));
			foreach (XElement item in xDocument.Root.Elements())
			{
				ITrailGroup trailGroup = ParseTrailGroup(item, trailSystem);
				if (trailGroup != null)
				{
					trailSystem.SetTrailGroup(trailGroup);
					break;
				}
			}
			return trailSystem;
		}
		catch (Exception)
		{
			return null;
		}
	}

	protected override void DoUpdate()
	{
		for (int i = 0; i < trailCount; i++)
		{
			trails[i].Update();
			if (!trails[i].Enabled && trails[i].PointCount <= 1)
			{
				RemoveTrail(trails[i--]);
			}
		}
		if (group != null)
		{
			group.Update();
		}
		base.DoUpdate();
	}

	private static ITrailGroup ParseTrailGroup(XElement xe, TrailSystem pe)
	{
		string localName;
		if ((localName = xe.Name.LocalName) != null && localName == "Polygon")
		{
			return PolygonGroup.ParseXml(xe, pe);
		}
		return null;
	}
}
