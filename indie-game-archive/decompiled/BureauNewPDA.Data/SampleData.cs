using System.Collections.Generic;

namespace BureauNewPDA.Data;

public class SampleData
{
	public List<StoryData> myStoryList = new List<StoryData>();

	public bool hasChanged;

	private int lastId = 2;

	public int findRef(int id)
	{
		foreach (StoryData myStory in myStoryList)
		{
			foreach (QuestionData question in myStory.questions)
			{
				if (question.sceneId == id)
				{
					return myStory.sceneId;
				}
			}
		}
		return id;
	}

	public int getNextSceneId()
	{
		for (int i = lastId; i < 1000; i++)
		{
			bool flag = true;
			foreach (StoryData myStory in myStoryList)
			{
				if (myStory.sceneId == i)
				{
					flag = false;
				}
				foreach (QuestionData question in myStory.questions)
				{
					if (question.sceneId == i)
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				lastId = i;
				return i;
			}
		}
		return 0;
	}

	public bool findRecord(int id)
	{
		foreach (StoryData myStory in myStoryList)
		{
			if (myStory.sceneId == id)
			{
				return true;
			}
		}
		return false;
	}

	public void addScene(StoryData newStory)
	{
		hasChanged = true;
		bool flag = false;
		foreach (StoryData myStory in myStoryList)
		{
			if (myStory.sceneId == newStory.sceneId)
			{
				myStoryList.Remove(myStory);
				myStoryList.Add(newStory);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			myStoryList.Add(newStory);
		}
	}

	public void deleteScene(StoryData deleteScene)
	{
		hasChanged = true;
	}
}
