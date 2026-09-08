using System.Xml.Serialization;

namespace Deep_waters;

public class Score
{
	[XmlElement]
	public string name;

	[XmlElement]
	public int score;

	public Score(string n, int s)
	{
		name = n;
		score = s;
	}

	public Score()
	{
	}
}
