using AvatarFarmOnline.Items.Game;
using AvatarFarmOnline.Logic.Stage;
using Quasar;
using Quasar.Elements;

namespace AvatarFarmOnline.Scenes;

internal class StageAfterScene : Scene
{
	private AvatarFarmOnline.Logic.Stage.Stage stage;

	public StageAfterScene(AvatarFarmOnline.Logic.Stage.Stage stage, Camera camera)
	{
		this.stage = stage;
		Camera = camera;
		Add(new AvatarFarmOnline.Items.Game.TileIconsItem(stage));
		AvatarFarmOnline.Items.Game.ActionResultsItem e = new AvatarFarmOnline.Items.Game.ActionResultsItem(stage);
		Add(e);
	}

	public override void Update()
	{
		base.Update();
	}

	public override void Render()
	{
		base.Render();
	}

	public override void Dispose()
	{
		stage = null;
		base.Dispose();
	}
}
