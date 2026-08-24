using Microsoft.Xna.Framework.Audio;

namespace ZP2K9;

internal class Music
{
	private enum MusicPhase
	{
		StartRock,
		Rock,
		StartBeat,
		Beat,
		Quiet
	}

	private const string menuString = "slow";

	private const string beatString = "drone";

	public static bool playing = false;

	public static Cue rockCue;

	public static Cue menuCue;

	public static Cue beatCue;

	public static bool ready = false;

	private static string[] rockString = new string[5] { "ravey", "blar", "jungo", "goa", "whoami" };

	public static bool runOut = false;

	public static int curRock = 0;

	private static MusicPhase musicPhase;

	public static void Init()
	{
		for (int i = 0; i < 8; i++)
		{
			int randomInt = Rand.GetRandomInt(0, rockString.Length);
			int randomInt2 = Rand.GetRandomInt(0, rockString.Length);
			string text = rockString[randomInt2];
			rockString[randomInt2] = rockString[randomInt];
			rockString[randomInt] = text;
		}
		rockCue = Sound.GetCue(rockString[curRock]);
		menuCue = Sound.GetCue("slow");
		beatCue = Sound.GetCue("drone");
		musicPhase = MusicPhase.Quiet;
		ready = true;
	}

	public static void Reset()
	{
		switch (musicPhase)
		{
		case MusicPhase.StartBeat:
		case MusicPhase.Beat:
		case MusicPhase.Quiet:
			musicPhase = MusicPhase.StartRock;
			break;
		}
	}

	public static void Update()
	{
		try
		{
			if (playing)
			{
				switch (musicPhase)
				{
				case MusicPhase.StartRock:
					if (beatCue.IsPlaying)
					{
						beatCue.Stop((AudioStopOptions)1);
					}
					if (!rockCue.IsPlaying)
					{
						rockCue = Sound.GetCue(rockString[curRock]);
						curRock = (curRock + 1) % rockString.Length;
						rockCue.Play();
					}
					if (rockCue.IsPlaying)
					{
						musicPhase = MusicPhase.Rock;
					}
					break;
				case MusicPhase.Rock:
					if (beatCue.IsPlaying)
					{
						beatCue.Stop((AudioStopOptions)1);
					}
					if (!rockCue.IsPlaying)
					{
						musicPhase = MusicPhase.StartBeat;
					}
					break;
				case MusicPhase.StartBeat:
					if (!beatCue.IsPlaying)
					{
						beatCue = Sound.GetCue("drone");
						beatCue.Play();
						musicPhase = MusicPhase.Beat;
					}
					break;
				case MusicPhase.Beat:
					if (!beatCue.IsPlaying)
					{
						musicPhase = MusicPhase.Quiet;
					}
					break;
				}
				if (menuCue.IsPlaying)
				{
					menuCue.Stop((AudioStopOptions)1);
				}
			}
			else
			{
				if (!menuCue.IsPlaying)
				{
					menuCue = Sound.GetCue("slow");
					menuCue.Play();
				}
				if (rockCue.IsPlaying)
				{
					rockCue.Stop((AudioStopOptions)1);
				}
				if (beatCue.IsPlaying)
				{
					beatCue.Stop((AudioStopOptions)1);
				}
				musicPhase = MusicPhase.Quiet;
			}
		}
		catch
		{
		}
	}
}
