namespace Quasar.GameUtils.Sections;

public abstract class GameSection : Section
{
	private int id;

	public int Id => id;

	public GameSection(int id)
	{
		this.id = id;
	}
}
