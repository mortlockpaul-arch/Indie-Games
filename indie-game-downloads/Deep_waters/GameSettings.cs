namespace Deep_waters;

public class GameSettings
{
	public int percX = 90;

	public int percY = 90;

	public int sfx = 100;

	public int music = 100;

	public GameDiff gameDifficulty = GameDiff.Normal;

	public bool gamefinishedonce;

	public bool enablesaving = true;

	public bool unlockrobot;

	public bool unlockxenomorph;

	public Score[] highscores;

	public GameSettings()
	{
		highscores = new Score[10];
		highscores[0] = new Score("Vittorio", 1000);
		highscores[1] = new Score("Claudio", 900);
		highscores[2] = new Score("Robb", 800);
		highscores[3] = new Score("Max", 700);
		highscores[4] = new Score("Jules", 600);
		highscores[5] = new Score("Paul", 500);
		highscores[6] = new Score("Manu", 400);
		highscores[7] = new Score("Zed", 300);
		highscores[8] = new Score("Shaun", 200);
		highscores[9] = new Score("Freeze", 100);
		percX = 100;
		percY = 100;
		sfx = 100;
		music = 100;
	}

	public void addScore(string n, int s)
	{
		int num = 9;
		while (num >= 0 && s > highscores[num].score)
		{
			if (num < 9)
			{
				highscores[num + 1] = highscores[num];
			}
			highscores[num] = new Score(n, s);
			num--;
		}
	}
}
