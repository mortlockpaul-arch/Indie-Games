using System;
using Quasar.Global;

namespace Quasar.GameUtils.Logic;

public class SingleScoreManager
{
	private long score;

	private Func<long, string> scoreTextFunc;

	public Func<long, string> ScoreToStringFunc
	{
		set
		{
			scoreTextFunc = value;
		}
	}

	public long Score => score;

	public void SetScore(long score)
	{
		this.score = score;
	}

	public void AddScore(long score)
	{
		this.score += score;
	}

	public string GetScoreText()
	{
		if (scoreTextFunc != null)
		{
			return scoreTextFunc(Score);
		}
		return GameMath.LongToString(Score, "#,0");
	}
}
