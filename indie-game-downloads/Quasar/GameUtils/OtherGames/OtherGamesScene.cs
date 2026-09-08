using Microsoft.Xna.Framework;
using Quasar.Elements;

namespace Quasar.GameUtils.OtherGames;

internal class OtherGamesScene : Scene
{
	private Quasar.GameUtils.OtherGames.OtherGamesCamera gamesCamera;

	private OtherGames otherGames;

	public OtherGamesScene(OtherGames otherGames)
	{
		this.otherGames = otherGames;
		gamesCamera = new Quasar.GameUtils.OtherGames.OtherGamesCamera(otherGames);
		Camera = gamesCamera;
		Add(gamesCamera);
		Light light = new Light();
		light.Ambient = new Vector3(0.5f);
		light.Diffuse = new Vector3(0.5f);
		light.Specular = new Vector3(0.5f);
		light.Transform.Translation = new Vector3(0f, 0.25f, 0.2f);
		gamesCamera.addChild(light);
		addLight(light);
		for (int i = 0; i < otherGames.Items.Count; i++)
		{
			Add(new Quasar.GameUtils.OtherGames.OtherGamesItem(otherGames, i));
		}
	}
}
