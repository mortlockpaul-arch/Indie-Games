using System.Xml.Linq;
using Quasar.Global;

namespace Quasar.Animation;

public class SpriteAnimation
{
	private string id;

	private int period;

	private int length;

	private bool loop;

	private bool flipX;

	private bool flipY;

	private string nextId;

	private int priority;

	private SpriteAnimation nextAnimation;

	public int[] Frames;

	public int Period
	{
		get
		{
			return period;
		}
		set
		{
			if (value >= 0)
			{
				period = value;
			}
		}
	}

	public string Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public int Length => length;

	public int FirstFrame => Frames[0];

	public bool Loop
	{
		get
		{
			return loop;
		}
		set
		{
			loop = value;
		}
	}

	public int Priority
	{
		get
		{
			return priority;
		}
		set
		{
			priority = value;
		}
	}

	public bool FlipX
	{
		get
		{
			return flipX;
		}
		set
		{
			flipX = value;
		}
	}

	public bool FlipY
	{
		get
		{
			return flipY;
		}
		set
		{
			flipY = value;
		}
	}

	public string NextId => nextId;

	public SpriteAnimation NextAnimation => nextAnimation;

	public void setNextAnimation(SpriteAnimation animation)
	{
		nextAnimation = animation;
	}

	public SpriteAnimation(string id, int period, bool loop, int[] frames, bool flipX, bool flipY, string nextId, int priority)
	{
		this.id = id;
		this.period = period;
		length = period * frames.Length;
		this.loop = loop;
		Frames = frames;
		this.nextId = nextId;
		this.flipX = flipX;
		this.flipY = flipY;
		this.priority = priority;
	}

	public static SpriteAnimation Parse(XElement xe)
	{
		int[] frames = XDocHelper.ParseIntListAttribute(xe, "frames").ToArray();
		return new SpriteAnimation(XDocHelper.GetAttribute(xe, "id"), XDocHelper.ParseIntAttribute(xe, "period", 1000), XDocHelper.ParseBoolAttribute(xe, "loop", defaultValue: false), frames, XDocHelper.ParseBoolAttribute(xe, "flipX"), XDocHelper.ParseBoolAttribute(xe, "flipY"), XDocHelper.GetAttribute(xe, "next"), XDocHelper.ParseIntAttribute(xe, "priority"));
	}
}
