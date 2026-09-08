using Quasar.Global;

namespace Quasar.Animation;

public class SpriteAnimationManager : Manager<SpriteAnimationSet>
{
	private static SpriteAnimationManager instance;

	public static SpriteAnimationManager Sets
	{
		get
		{
			if (instance == null)
			{
				instance = new SpriteAnimationManager();
			}
			return instance;
		}
	}

	private SpriteAnimationManager()
	{
	}

	protected override SpriteAnimationSet createDefaultItem()
	{
		return null;
	}

	protected override SpriteAnimationSet LoadItem(string name)
	{
		return SpriteAnimationSet.Load(name);
	}
}
