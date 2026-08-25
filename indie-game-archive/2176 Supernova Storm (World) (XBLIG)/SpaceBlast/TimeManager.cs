using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceBlast;

internal static class TimeManager
{
	private static double m_TotalAdjustmentTime;

	private static double m_PauseStarted;

	public static double RawTime;

	private static bool m_Paused;

	public static int FrameNumber;

	public static double TotalSeconds;

	public static double DeltaSeconds;

	public static bool IsPaused => m_Paused;

	public static void Reset(GameTime now)
	{
		m_TotalAdjustmentTime = 0.0;
		m_PauseStarted = 0.0;
		RawTime = now.TotalGameTime.TotalSeconds;
		m_Paused = false;
		TotalSeconds = 0.0;
		DeltaSeconds = 0.0;
	}

	public static void Pause()
	{
		if (!m_Paused)
		{
			m_PauseStarted = RawTime;
		}
		m_Paused = true;
		GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
		GamePad.SetVibration(PlayerIndex.Two, 0f, 0f);
		GamePad.SetVibration(PlayerIndex.Three, 0f, 0f);
		GamePad.SetVibration(PlayerIndex.Four, 0f, 0f);
	}

	public static void Resume()
	{
		if (m_Paused)
		{
			m_TotalAdjustmentTime += RawTime - m_PauseStarted;
		}
		m_Paused = false;
	}

	public static void UpdateTime(GameTime now)
	{
		RawTime = now.TotalGameTime.TotalSeconds;
		FrameNumber++;
		if (!m_Paused)
		{
			DeltaSeconds = now.ElapsedGameTime.TotalSeconds;
			TotalSeconds = now.TotalGameTime.TotalSeconds + m_TotalAdjustmentTime;
		}
	}

	public static void SetTime(double newtime)
	{
		m_TotalAdjustmentTime = newtime - RawTime;
		TotalSeconds = newtime;
	}
}
