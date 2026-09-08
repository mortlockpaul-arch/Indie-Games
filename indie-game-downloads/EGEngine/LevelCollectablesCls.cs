namespace EGEngine;

public class LevelCollectablesCls
{
	public string name = "";

	public float starScaleTimer;

	public float hoopScaleTimer;

	public float coinScaleTimer;

	public float heartScaleTimer;

	public int timeBonus;

	public int levelMaxTime;

	public DifficultyCls[] difficulty = new DifficultyCls[3];

	public int hearts;

	public LevelCollectablesCls()
	{
		for (int i = 0; i < 3; i++)
		{
			difficulty[i] = new DifficultyCls();
		}
	}
}
