using EGEngine;

namespace GameEngine;

public class GameAudioVideoMenu : AudioVideoMenu
{
	public override void LoadMusic()
	{
		AudioVideoMenu.MusicList = new MusicEntry[2];
		for (int i = 0; i < 2; i++)
		{
			AudioVideoMenu.MusicList[i] = new MusicEntry();
		}
		AudioVideoMenu.MusicList[0].Name = "theme";
		AudioVideoMenu.MusicList[1].Name = "theme";
	}
}
