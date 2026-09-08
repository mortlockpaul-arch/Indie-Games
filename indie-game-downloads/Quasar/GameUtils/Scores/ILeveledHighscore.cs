namespace Quasar.GameUtils.Scores;

public interface ILeveledHighscore
{
	int Level { get; }

	long Score { get; }
}
