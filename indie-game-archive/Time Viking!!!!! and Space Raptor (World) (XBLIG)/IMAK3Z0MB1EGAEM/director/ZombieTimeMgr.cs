using System.Collections.Generic;

namespace IMAK3Z0MB1EGAEM.director;

public class ZombieTimeMgr : BaseTimeMgr
{
	public const int INTRO_THEME = 0;

	public const int SKA = 1;

	public const int SPACE = 2;

	public const int ROCK = 3;

	public const int JUNGLE = 4;

	public const int METAL = 5;

	public const int OUTTRO = 6;

	public const int END = 7;

	public ZombieTimeMgr()
	{
		timeSlice = new List<TimeSlice>();
		timeSlice.Add(new TimeSlice(0, 0.0, 120.0));
		timeSlice.Add(new TimeSlice(2, 0.0, 110.0));
		timeSlice.Add(new TimeSlice(3, 36.0, 120.0));
		timeSlice.Add(new TimeSlice(5, 32.0, 100.0));
		timeSlice.Add(new TimeSlice(8, 44.0, 180.0));
		timeSlice.Add(new TimeSlice(9, 58.666667, 110.0));
		timeSlice.Add(new TimeSlice(11, 8.485, 120.0));
		timeSlice.Add(new TimeSlice(13, 18.485, 120.0));
	}
}
