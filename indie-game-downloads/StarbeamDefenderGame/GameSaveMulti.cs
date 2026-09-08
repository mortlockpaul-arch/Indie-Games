namespace StarbeamDefenderGame;

public class GameSaveMulti
{
	public MultiplayerSave[] saveditems;

	public GameSaveMulti()
	{
		saveditems = new MultiplayerSave[20];
		for (int i = 0; i < 20; i++)
		{
			saveditems[i] = default(MultiplayerSave);
		}
	}

	public void AddNewItem(string name, string nametwo, int score, int rounds)
	{
		MultiplayerSave multiplayerSave = default(MultiplayerSave);
		int num = 0;
		for (int i = 0; i < 20; i++)
		{
			if (saveditems[i].score < score)
			{
				multiplayerSave = saveditems[i];
				ref MultiplayerSave reference = ref saveditems[i];
				reference = new MultiplayerSave(name, nametwo, score, rounds);
				break;
			}
			num++;
		}
		for (int j = num + 1; j < 20; j++)
		{
			MultiplayerSave multiplayerSave2 = saveditems[j];
			saveditems[j] = multiplayerSave;
			multiplayerSave = multiplayerSave2;
		}
	}

	public void AddToTopOfStack(string name, string nametwo, int score, int rounds)
	{
		_ = saveditems[1];
		for (int num = 19; num > 0; num--)
		{
			ref MultiplayerSave reference = ref saveditems[num];
			reference = saveditems[num - 1];
		}
		ref MultiplayerSave reference2 = ref saveditems[0];
		reference2 = new MultiplayerSave(name, nametwo, score, rounds);
	}
}
