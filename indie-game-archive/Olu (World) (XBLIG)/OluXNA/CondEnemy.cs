using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;

namespace OluXNA;

internal class CondEnemy : Enemy
{
	public List<XmlNode> nodeList;

	public ConditionSet csRequired;

	public string mode;

	public CondEnemy()
	{
		nodeList = new List<XmlNode>();
		csRequired = new ConditionSet();
	}

	public CondEnemy(Dictionary<string, string> attributes, XmlNode node)
		: this()
	{
		foreach (XmlNode childNode in node.SelectSingleNode("enems").ChildNodes)
		{
			nodeList.Add(childNode);
		}
		mode = attributes["mode"];
	}

	public override void draw(GameTime gametime)
	{
	}

	public override void act(GameTime gametime)
	{
		leave();
	}

	public override void start()
	{
		base.start();
		BaseGame.Get().actualEnem--;
		switch (mode)
		{
		case "tutorial":
			SetupTutorial();
			break;
		case "nottutorial":
			SetupNotTutorial();
			break;
		}
		if (csRequired.ConditionsMet())
		{
			SpawnEnems();
		}
	}

	public void SetupTutorial()
	{
		csRequired.set.Add(new TutorialCondition());
		csRequired.Start();
	}

	public void SetupNotTutorial()
	{
		csRequired.set.Add(new NotTutorialCondition());
		csRequired.Start();
	}

	private void SpawnEnems()
	{
		for (int num = nodeList.Count - 1; num >= 0; num--)
		{
			EnemyQueuePart enemyQueuePart = LevelLoader.MakeEnemy(nodeList[num]);
			enemyQueuePart.enem.start();
			BaseGame.Get().enems.Add(enemyQueuePart.enem);
		}
	}

	public override Vector3 getPos()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		return Vector3.Zero;
	}

	public override string name()
	{
		return "[]";
	}

	public override bool Check(int numEnem)
	{
		return false;
	}

	public override void HitSound(int lockNum, float volume)
	{
	}

	public override void die()
	{
		BaseGame.Get().actualEnem++;
		base.die();
	}

	public override void leave()
	{
		BaseGame.Get().actualEnem++;
		base.leave();
	}
}
