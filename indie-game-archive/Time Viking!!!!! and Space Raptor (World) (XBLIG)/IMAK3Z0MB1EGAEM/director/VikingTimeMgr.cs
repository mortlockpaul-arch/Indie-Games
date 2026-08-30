using System.Collections.Generic;

namespace IMAK3Z0MB1EGAEM.director;

public class VikingTimeMgr : BaseTimeMgr
{
	public const int INTRO_THEME = 0;

	public const int EMO = 1;

	public const int DRONE = 2;

	public const int NERDCORE = 3;

	public const int PUNK = 4;

	public const int OUTTRO = 5;

	public const int END = 6;

	public VikingTimeMgr()
	{
		timeSlice = new List<TimeSlice>();
		timeSlice.Add(new TimeSlice(0, 0.0, 180.0, noadd: true));
		timeSlice.Add(new TimeSlice(1, 46.513, 88.64, noadd: true));
		timeSlice.Add(new TimeSlice(4, 39.781, 120.22, noadd: true));
		timeSlice.Add(new TimeSlice(7, 35.781, 90.49, noadd: true));
		timeSlice.Add(new TimeSlice(10, 25.706, 240.0, noadd: true, 1));
		timeSlice.Add(new TimeSlice(11, 37.716, 180.60818, noadd: true, 1));
		timeSlice.Add(new TimeSlice(14, 59.76, 180.0, noadd: true));
	}
}
