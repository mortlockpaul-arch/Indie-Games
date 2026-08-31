using System.Collections.Generic;

namespace BureauNewPDA.Data;

public class QuestionData
{
	public int id = 1;

	public string questionText = "";

	public int sceneId = -1;

	public List<VariableData> variableAdded = new List<VariableData>();

	public int startFrame;

	public int endFrame;

	public double startTime;

	public double endTime;

	public int triggerType = -1;

	public void addVideoControlLogic(string type, int start, int end)
	{
		startFrame = start;
		endFrame = end;
		startTime = getTimeFromFrames(start);
		endTime = getTimeFromFrames(end);
		switch (type)
		{
		case "RT":
			triggerType = 1;
			break;
		case "LT":
			triggerType = 2;
			break;
		case "AB":
			triggerType = 3;
			break;
		case "BB":
			triggerType = 4;
			break;
		case "XB":
			triggerType = 5;
			break;
		case "YB":
			triggerType = 5;
			break;
		}
	}

	private double getTimeFromFrames(int frames)
	{
		return (float)frames / 24f * 1000f;
	}
}
