namespace Quasar.GameUtils.Sections;

public abstract class ExtraSection : Section
{
	public enum Priority
	{
		Background,
		Foreground
	}

	private Priority priority;

	public Priority DrawPriority => priority;

	public ExtraSection(Priority priority)
	{
		this.priority = priority;
	}
}
