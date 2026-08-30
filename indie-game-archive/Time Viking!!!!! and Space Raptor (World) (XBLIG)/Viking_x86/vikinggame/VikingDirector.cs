using IMAK3Z0MB1EGAEM.audio;
using Viking_x86.director;

namespace Viking_x86.vikinggame;

public class VikingDirector
{
	public const int PHASE_PRE = 0;

	public const int PHASE_WARPING = 1;

	public const int PHASE_MAIN = 2;

	public int phase;

	public float frame;

	public float timeStrFrame;

	public VikingDirector()
	{
		Init();
	}

	public void Init()
	{
		phase = 0;
		frame = 0f;
		timeStrFrame = 0f;
	}

	public void Update()
	{
		float num = timeStrFrame;
		timeStrFrame += Game1.frameTime;
		switch (phase)
		{
		case 0:
		{
			bool flag = true;
			if (num < 1f && timeStrFrame >= 1f)
			{
				Sound.Play("jail");
			}
			if (num < 2f && timeStrFrame >= 2f)
			{
				Sound.Play("jail");
			}
			for (int j = 0; j < 2; j++)
			{
				if (Game1.vgame.charMgr.character[j].exists && Game1.vgame.charMgr.character[j].loc.X < Game1.vgame.world.towerX + 100f)
				{
					flag = false;
				}
			}
			if (!flag)
			{
				break;
			}
			Sound.Play("warpout");
			phase = 1;
			frame = 0f;
			for (int k = 0; k < 2; k++)
			{
				if (Game1.vgame.charMgr.character[k].exists)
				{
					Game1.vgame.charMgr.character[k].SetAnimation("warp", 0, overRide: true);
				}
			}
			break;
		}
		case 1:
		{
			frame += Game1.frameTime;
			if (!(frame >= 1f))
			{
				break;
			}
			Sound.Play("warpin");
			phase = 2;
			timeStrFrame = 0f;
			for (int i = 0; i < 2; i++)
			{
				if (Game1.vgame.charMgr.character[i].exists)
				{
					Game1.vgame.charMgr.character[i].SetAnimation("warpin", 0, overRide: true);
				}
			}
			break;
		}
		case 2:
			if (num < 1f && timeStrFrame >= 1f)
			{
				Sound.Play("jail");
			}
			if (num < 2f && timeStrFrame >= 2f)
			{
				Sound.Play("jail");
			}
			Music.Update(1);
			TimeMgr.VikingTMgr().Update();
			break;
		}
	}
}
