namespace Kobingo.Xna.Library.Data;

public class SettingsEntry
{
	public string Value { get; set; }

	public string[] Options { get; private set; }

	public SettingsEntry(params string[] options)
	{
		Options = options;
		Value = options[0];
	}
}
