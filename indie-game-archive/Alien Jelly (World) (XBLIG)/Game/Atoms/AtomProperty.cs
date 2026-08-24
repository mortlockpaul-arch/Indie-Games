namespace Game.Atoms;

public class AtomProperty
{
	public string title;

	public string desc;

	public string[] options;

	public AtomProperty(string xTitle, string xDesc, string[] aOptions)
	{
		title = xTitle;
		desc = xDesc;
		options = aOptions;
	}
}
