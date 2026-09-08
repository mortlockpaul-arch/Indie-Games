using Quasar.Global;

namespace Quasar.GUI.Controls;

public class GroupConfigurationManager : Manager<GroupConfiguration>
{
	private static GroupConfigurationManager instance;

	public static GroupConfigurationManager Config
	{
		get
		{
			if (instance == null)
			{
				instance = new GroupConfigurationManager();
			}
			return instance;
		}
	}

	protected GroupConfigurationManager()
	{
	}

	public void AddConfig(string id, GroupConfiguration gc)
	{
		items.Add(id, gc);
	}

	protected override GroupConfiguration createDefaultItem()
	{
		return new GroupConfiguration();
	}

	protected override GroupConfiguration LoadItem(string name)
	{
		return base.DefaultItem;
	}

	public override void Dispose()
	{
		instance = null;
		base.Dispose();
	}
}
