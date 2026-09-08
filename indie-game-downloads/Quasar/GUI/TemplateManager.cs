using Quasar.Global;

namespace Quasar.GUI;

public class TemplateManager : Manager<Template>
{
	private static TemplateManager instance;

	public static TemplateManager Templates
	{
		get
		{
			if (instance == null)
			{
				instance = new TemplateManager();
			}
			return instance;
		}
	}

	protected TemplateManager()
	{
	}

	public void AddTemplate(string id, Template t)
	{
		items.Add(id, t);
	}

	protected override Template createDefaultItem()
	{
		return null;
	}

	protected override Template LoadItem(string name)
	{
		return null;
	}

	public override void Dispose()
	{
		instance = null;
		base.Dispose();
	}
}
