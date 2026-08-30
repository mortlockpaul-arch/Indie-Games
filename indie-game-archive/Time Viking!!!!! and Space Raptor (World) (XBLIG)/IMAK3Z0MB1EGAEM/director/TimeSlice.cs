namespace IMAK3Z0MB1EGAEM.director;

public class TimeSlice
{
	private const double add = 4.0;

	public double start;

	public double bpm;

	public int boff;

	public TimeSlice(int minutes, double seconds, double bpm)
	{
		start = (double)minutes * 60.0 + seconds + 4.0;
		this.bpm = bpm;
	}

	public TimeSlice(int minutes, double seconds, double bpm, bool noadd)
	{
		start = (double)minutes * 60.0 + seconds;
		this.bpm = bpm;
	}

	public TimeSlice(int minutes, double seconds, double bpm, bool noadd, int boff)
	{
		start = (double)minutes * 60.0 + seconds;
		this.bpm = bpm;
		this.boff = boff;
	}
}
