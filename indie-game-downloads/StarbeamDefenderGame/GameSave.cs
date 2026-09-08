namespace StarbeamDefenderGame;

public class GameSave
{
	public SinglePlayerSave[] saveditems;

	public GameSave()
	{
		saveditems = new SinglePlayerSave[20];
		for (int i = 0; i < 20; i++)
		{
			saveditems[i] = default(SinglePlayerSave);
		}
	}

	public void AddNewItem(string name, int score, int rounds)
	{
		SinglePlayerSave singlePlayerSave = default(SinglePlayerSave);
		int num = 0;
		for (int i = 0; i < 20; i++)
		{
			if (saveditems[i].score < score)
			{
				singlePlayerSave = saveditems[i];
				ref SinglePlayerSave reference = ref saveditems[i];
				reference = new SinglePlayerSave(name, score, rounds);
				break;
			}
			num++;
		}
		for (int j = num + 1; j < 20; j++)
		{
			SinglePlayerSave singlePlayerSave2 = saveditems[j];
			saveditems[j] = singlePlayerSave;
			singlePlayerSave = singlePlayerSave2;
		}
	}

	public void AddToTopOfStack(string name, int score, int rounds)
	{
		_ = saveditems[1];
		for (int num = 19; num > 0; num--)
		{
			ref SinglePlayerSave reference = ref saveditems[num];
			reference = saveditems[num - 1];
		}
		ref SinglePlayerSave reference2 = ref saveditems[0];
		reference2 = new SinglePlayerSave(name, score, rounds);
	}
}
