using InfinityLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Infinity.Scenes;

public class MainGameLoader : InitializeScene
{
	public MainGameLoader(Game game)
		: base(game)
	{
	}

	protected override void LoadContents()
	{
		StageSettings[] array = base.Content.Load<StageSettings[]>("StageSettings");
		ContentManager content = Global.AsyncLoader.Content;
		StageSettings[] array2 = array;
		int num = 0;
		if (num < array2.Length)
		{
			StageSettings stageSettings = array2[num];
			ChapterSettings[] chapters = stageSettings.Chapters;
			foreach (ChapterSettings chapterSettings in chapters)
			{
				Chache<Model>(content, chapterSettings.StageModelAsset);
				Chache<Model>(content, chapterSettings.CollisionModelAsset);
				Chache<Model>(content, chapterSettings.BgModelAsset);
			}
			Chache<Model>(content, "Models/Models/boss/boss_bg");
			Chache<Model>(content, stageSettings.Boss.MotionAppearAsset);
			Chache<Model>(content, stageSettings.Boss.MotionBattleAsset);
		}
		Chache<Model>("Models/Models/player/player");
		Chache<Model>("Models/Models/player/player_col");
		Chache<Model>("Models/Models/player/player_shield");
		Chache<Model>("Models/Models/player/player_sight");
		Chache<Model>("Models/Models/player/player_item_energy");
		Chache<Model>("Models/Models/player/player_item_energy_col");
		for (int j = 0; j < 3; j++)
		{
			string text = $"Models/Models/enemy/enemy{j + 1:00}";
			Chache<Model>(text);
			Chache<Model>(text + "_col");
		}
		Chache<Model>("Models/Models/boss/boss_core");
		Chache<Model>("Models/Models/boss/boss_core_breakmotion");
		Chache<Model>("Models/Models/boss/boss_core_col");
		Chache<Model>("Models/Models/boss/boss_shield");
		Chache<Model>("Models/Models/boss/boss_shield_breakmotion");
		Chache<Model>("Models/Models/boss/boss_shield_break");
		Chache<Model>("Models/Models/boss/boss_shield_col");
		Chache<Model>("Models/Models/boss/boss_hand");
		Chache<Model>("Models/Models/boss/boss_hand_breakmotion");
		Chache<Model>("Models/Models/boss/boss_hand_col");
		for (int k = 0; k < 10; k++)
		{
			string asset = $"Models/Models/font_number/font_num{k}";
			Chache<Model>(asset);
		}
		Chache<Model>("Models/Models/screen/screen_score");
		Chache<Model>("Models/Models/screen/screen_lifegauge");
		Chache<Model>("Models/Models/screen/screen_gameover");
		base.SceneManager.AddScene(new MainGame(base.Game));
	}

	private void Chache<T>(ContentManager content, string asset)
	{
		content.Load<T>(asset);
	}

	private void Chache<T>(string asset)
	{
		Chache<T>(base.Content, asset);
	}
}
