namespace Quasar;

public abstract class Behavior
{
	private bool enabled = true;

	public bool Enabled
	{
		get
		{
			return enabled;
		}
		set
		{
			enabled = value;
		}
	}

	public Behavior()
	{
	}

	public virtual bool Compatible(Element element)
	{
		return true;
	}

	public virtual void PrepareElement(Element element)
	{
	}

	public virtual void ClearElement(Element element)
	{
	}

	public void Update(Element element)
	{
		if (enabled)
		{
			DoUpdate(element);
		}
	}

	public abstract void DoUpdate(Element element);
}
