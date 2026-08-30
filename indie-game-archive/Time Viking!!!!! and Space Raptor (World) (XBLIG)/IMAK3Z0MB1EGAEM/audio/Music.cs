using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Viking_x86.director;

namespace IMAK3Z0MB1EGAEM.audio;

internal class Music
{
	public const int ZOMBIE_SONG = 0;

	public const int VIKING_SONG = 1;

	public const int ENDLESS_SONG = 2;

	public static string[] SONG = new string[3] { "epicopus", "timeviking", "endless" };

	public static int song;

	private static Song songCue;

	public static void Init(ContentManager Content)
	{
		songCue = Content.Load<Song>("sfx/music/timeviking");
	}

	public static void Update(int _song)
	{
		song = _song;
		TimeMgr.time = _song;
		TimeMgr.CurTMgr().time = MediaPlayer.PlayPosition.TotalSeconds;
		if (MediaPlayer.State != MediaState.Playing)
		{
			if (TimeMgr.CurTMgr().playNum <= 0)
			{
				MediaPlayer.Play(songCue);
				MediaPlayer.Volume = 2f;
				TimeMgr.CurTMgr().Start();
			}
		}
		else if (MediaPlayer.PlayPosition.TotalSeconds > 901.0)
		{
			MediaPlayer.Stop();
		}
	}

	public static void Stop()
	{
		try
		{
			MediaPlayer.Stop();
		}
		catch
		{
		}
	}

	public static void Start()
	{
		try
		{
			MediaPlayer.Play(songCue);
			MediaPlayer.Volume = 2f;
		}
		catch
		{
		}
		TimeMgr.CurTMgr().Start();
	}

	public static void Pause()
	{
		try
		{
			MediaPlayer.Pause();
		}
		catch
		{
		}
	}

	public static void Resume()
	{
		try
		{
			MediaPlayer.Resume();
		}
		catch
		{
		}
	}
}
