using System.Collections.Generic;

namespace BureauNewPDA.Data;

public class StoryData
{
	public class GenericPair
	{
		public string text = "";

		public int id;
	}

	public string chapter = "BLANK";

	public int sceneId;

	public string sceneRefName = "BLANK";

	public string headerText = "";

	public string bodyText = "";

	public string sceneType = "BLANK";

	public int displayStateId;

	public string soundFileName = "";

	public List<QuestionData> questions = new List<QuestionData>();

	public List<GenericPair> facts = new List<GenericPair>();
}
