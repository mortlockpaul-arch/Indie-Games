namespace MaxScriptDefines;

public class TestObject
{
	private string Name;

	public TestObject(string thisName)
	{
		Name = thisName;
	}

	public string GetLongName()
	{
		return "You can call me " + Name + "!";
	}
}
