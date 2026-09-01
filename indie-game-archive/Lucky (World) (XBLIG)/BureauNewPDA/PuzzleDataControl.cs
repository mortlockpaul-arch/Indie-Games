using System;
using System.Collections.Generic;

namespace BureauNewPDA;

public class PuzzleDataControl
{
	public class PuzzleData
	{
		public int puzzleId = -1;

		public string videoName = "";

		public List<objectData> objectList = new List<objectData>();

		public bool isCorrectSelected;

		public bool isFinishedOrder;

		public string finishedText = "";

		public int retries;
	}

	public class objectData
	{
		public string name = "";

		public int order = -1;

		public int correctOrder = -1;

		public bool isUserSelected;

		public bool hasBeenMarkedCorrect;

		public bool hasBeenMarkedInCorrect;

		public bool isOrderCorrect;

		public bool isOrderWrong;

		public int wrongOrderOrder = -1;
	}

	public List<PuzzleData> puzzleDataList = new List<PuzzleData>();

	private Random myRandom = new Random();

	public void resetPuzzle()
	{
	}

	public void addData()
	{
		puzzleDataList.Clear();
		PuzzleData puzzleData = new PuzzleData();
		puzzleData.puzzleId = 1;
		puzzleData.videoName = "KeithPuzzleA";
		puzzleData.objectList = addObjects(puzzleData.puzzleId);
		puzzleData.finishedText = "Good job.  You now know why Keith Karan tried to kill Ramesh.  Keith thinks that Ramesh is a terrorist.  He was provided the info and gun from a Strip Club / Gun shop in Arizona.  You can now solve this case by visiting Ramesh.";
		puzzleDataList.Add(puzzleData);
		puzzleData = new PuzzleData();
		puzzleData.puzzleId = 2;
		puzzleData.videoName = "JacobPuzzle";
		puzzleData.objectList = addObjects(puzzleData.puzzleId);
		puzzleData.finishedText = "Good job.  Jacob admits hitting his sister and his testimony is filled with sexual overtones that don't match the evidence found in the apartment.  He also admits seeing the key to the apartment - a weird detail to share with the police.  Other than the key, he has no physical evidence linking him to the crime.  Obsession with his sister is an easy motive.";
		puzzleDataList.Add(puzzleData);
		puzzleData = new PuzzleData();
		puzzleData.puzzleId = 3;
		puzzleData.videoName = "WilliamPuzzle";
		puzzleData.objectList = addObjects(puzzleData.puzzleId);
		puzzleData.finishedText = "Good job.  William is not a suspect for this crime.  He has no motive and was in the hospital at the time of the crime.  His testimony does conflict with the sexual aspects of Jacob's version of the story.  Based on my own observations of the apartment, I would believe William's version.  William's apartment being robbed adds a wrinkle to the case.  It is also odd that Jacob has William's key.  William had given this key to Molly at the hospital.  Jacob should have had Molly's key.";
		puzzleDataList.Add(puzzleData);
		puzzleData = new PuzzleData();
		puzzleData.puzzleId = 4;
		puzzleData.videoName = "DariMorguePuzzle";
		puzzleData.objectList = addObjects(puzzleData.puzzleId);
		puzzleData.finishedText = "Good job.  Molly was killed instantly from the fall.  However, she has a bruise on her chest that forensics believes came from the killer.  She was kicked out of the window.  The killer was wearing a shoe size of 10 maybe 11.  Molly has a bruise on her eye from being hit by Jacob.  On her person, she has her driver's license and her key.  At the time she left the hospital, she did not have her license and she had William's key.";
		puzzleDataList.Add(puzzleData);
		puzzleData = new PuzzleData();
		puzzleData.puzzleId = 5;
		puzzleData.videoName = "FinalPuzzle";
		puzzleData.objectList = addObjects(puzzleData.puzzleId);
		puzzleData.finishedText = "Good job.  Jacob should have had Molly's keys instead he has William's.  Maybe the keys were planted on him.  The robbery is also odd and none of the stolen goods were found on Jacob including the missing cash.  The security camera was disabled by what looks like a nightstick.";
		puzzleDataList.Add(puzzleData);
	}

	public int getRandom(int max)
	{
		return myRandom.Next(max) + 1;
	}

	public List<objectData> addObjects(int id)
	{
		List<objectData> list = new List<objectData>();
		switch (id)
		{
		case 1:
			list.Add(addObject("Strip Club", 1));
			list.Add(addObject("Ramesh Photo", 2));
			list.Add(addObject("Uzi", 3));
			list.Add(addObject("Divorce", -1));
			list.Add(addObject("Ohio", -1));
			list.Add(addObject("Pink Slip", -1));
			list.Add(addObject("Wrench", -1));
			list.Add(addObject("Coffee", -1));
			list.Add(addObject("Arizona", -1));
			list.Add(addObject("Bat", -1));
			list.Add(addObject("Glasses", -1));
			list.Add(addObject("Beer", -1));
			break;
		case 2:
			list.Add(addObject("Elevator", 1));
			list.Add(addObject("Condoms", 2));
			list.Add(addObject("Key", 3));
			list.Add(addObject("Television", -1));
			list.Add(addObject("Diamonds", -1));
			list.Add(addObject("Wallet", -1));
			list.Add(addObject("Police", -1));
			list.Add(addObject("Ipod", -1));
			list.Add(addObject("Stereo", -1));
			list.Add(addObject("Dildo", -1));
			list.Add(addObject("Glue", -1));
			list.Add(addObject("Newspaper", -1));
			break;
		case 3:
			list.Add(addObject("Diamonds", 1));
			list.Add(addObject("Wallet / MP3 Player", 2));
			list.Add(addObject("Fractured Hand", 3));
			list.Add(addObject("Television", -1));
			list.Add(addObject("Condoms", -1));
			list.Add(addObject("Knife / Rope", -1));
			list.Add(addObject("Drugs", -1));
			list.Add(addObject("Game", -1));
			list.Add(addObject("Whiskey", -1));
			list.Add(addObject("Glass", -1));
			list.Add(addObject("Key", -1));
			list.Add(addObject("Newspaper", -1));
			break;
		case 4:
			list.Add(addObject("Black Eye", 1));
			list.Add(addObject("Bruise (Chest)", 2));
			list.Add(addObject("Molly's Key", 3));
			list.Add(addObject("Saw", -1));
			list.Add(addObject("Wallet", -1));
			list.Add(addObject("Pills", -1));
			list.Add(addObject("Earring ", -1));
			list.Add(addObject("Tattoo", -1));
			list.Add(addObject("Battery", -1));
			list.Add(addObject("William's Key", -1));
			list.Add(addObject("Needle", -1));
			list.Add(addObject("Phone", -1));
			break;
		case 5:
			list.Add(addObject("Keys", 1));
			list.Add(addObject("Bruise", 2));
			list.Add(addObject("Nightstick", 3));
			list.Add(addObject("Saw", -1));
			list.Add(addObject("Condoms", -1));
			list.Add(addObject("Pills", -1));
			list.Add(addObject("Earring ", -1));
			list.Add(addObject("Tattoo", -1));
			list.Add(addObject("Battery", -1));
			list.Add(addObject("Glass", -1));
			list.Add(addObject("Needle", -1));
			list.Add(addObject("Phone", -1));
			break;
		}
		scramble(list);
		return list;
	}

	public List<objectData> scramble(List<objectData> l)
	{
		int num = 1;
		int num2 = -1;
		int num3 = -1;
		foreach (objectData item in l)
		{
			item.order = num;
			num++;
		}
		num = 0;
		while (num < 20)
		{
			num++;
			num2 = getRandom(12);
			num3 = getRandom(12);
			if (num2 != num3)
			{
				replaceValueInObjectList(l, num2, -1);
				replaceValueInObjectList(l, num3, num2);
				replaceValueInObjectList(l, -1, num3);
			}
		}
		return l;
	}

	public void replaceValueInObjectList(List<objectData> l, int findValue, int replaceValue)
	{
		foreach (objectData item in l)
		{
			if (item.order == findValue)
			{
				item.order = replaceValue;
				break;
			}
		}
	}

	public objectData addObject(string name, int correctOrder)
	{
		objectData objectData2 = new objectData();
		objectData2.name = name;
		objectData2.correctOrder = correctOrder;
		return objectData2;
	}
}
