using Quasar.GameUtils.Awards;

namespace Quasar.GameUtils.Sections;

public class AwardsSection : ExtraSection
{
	private static AwardsSection instance;

	public static AwardsSection Instance => instance;

	static AwardsSection()
	{
		instance = new AwardsSection();
	}

	public AwardsSection()
		: base(Priority.Foreground)
	{
	}

	protected override void initScenes()
	{
		AddScene(AwardsScene.Instance, isDefault: true);
	}
}
