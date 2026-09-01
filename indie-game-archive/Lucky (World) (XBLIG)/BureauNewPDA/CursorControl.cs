using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BureauNewPDA;

public class CursorControl
{
	private enum displayTypeEnum
	{
		normal,
		overAction,
		arrowUp,
		arrowDown,
		arrowLeft,
		arrowRight,
		inventorySelect,
		pickUp
	}

	private enum puzzleGameState
	{
		A,
		B,
		C,
		D,
		E
	}

	private enum puzzleSelectedColor
	{
		R,
		G,
		N
	}

	public CoreDisplayElements myCoreDisplayElements = new CoreDisplayElements();

	private int cursorA;

	private int cursorB;

	public Vector2 cursorLocation = new Vector2(640f, 360f);

	private int timerCursor;

	private int timerButterfly;

	private int i;

	private displayTypeEnum displayType;

	private List<string> inventory = new List<string>();

	private int inventoryFrameCount;

	private bool inventoryClosed = true;

	private int inventoryOpenTimerCount;

	private int inventoryDisplayCounter;

	public bool inventoryItemTarget;

	private bool wheelOuterReverse = true;

	private bool wheelInnerReverse;

	private string textMessage = "";

	private float cursorScale;

	private float arrowScale;

	private string arrowImage;

	private Vector2 textPosition = new Vector2(120f, 500f);

	private int butteryFlyA;

	private Vector2 butteryFlyAPos = new Vector2(300f, 500f);

	private int butteryFlyB = 2;

	private Vector2 butteryFlyBPos = new Vector2(700f, 600f);

	private int butteryFlyC = 5;

	private Vector2 butteryFlyCPos = new Vector2(450f, 550f);

	private SpriteEffects butterFlyAEffect;

	private SpriteEffects butterFlyBEffect;

	private SpriteEffects butterFlyCEffect;

	private Vector2 piece9Location = new Vector2(880f, 450f);

	private Vector2 piece7Location = new Vector2(740f, 480f);

	private Vector2 piece5Location = new Vector2(740f, 480f);

	private float piece5Rotation;

	private bool piece9LocationLeft;

	private bool turnOnButterFly;

	private bool turnOnMoneyFly;

	private bool turnOnPiece9;

	private bool turnOnPiece7;

	private bool turnOnPiece1;

	private bool turnOnPiece2;

	private bool turnOnPiece3;

	private bool turnOnPiece5;

	private bool turnOnPiece6;

	private bool turnOnPiece8;

	private bool turnOnWheel;

	private bool turnOnTopWheelControl;

	private bool turnOnBottomWheelControl;

	private bool topCounterClockwise = true;

	private bool bottomCounterClockwise;

	private float rotationIn;

	private float rotationOut;

	private float rotationInTarget;

	private float rotationOutTarget;

	private float rotationInCurrent;

	private float rotationOutCurrent;

	private bool isWheelSpin;

	public bool spinWheelSuccess;

	public bool isPuzzleGame;

	public bool isCheckingPuzzle;

	private puzzleGameState currentPuzzleState;

	private int puzzleACount;

	private int puzzleBCount;

	private int puzzleCCount;

	private int puzzleDCount;

	private int puzzleECount;

	private int puzzleTimer;

	private int puzzleTimerLoop;

	public int correctAnswers;

	private puzzleSelectedColor puzzleAColor = puzzleSelectedColor.N;

	private puzzleSelectedColor puzzleBColor = puzzleSelectedColor.N;

	private puzzleSelectedColor puzzleCColor = puzzleSelectedColor.N;

	private puzzleSelectedColor puzzleDColor = puzzleSelectedColor.N;

	private puzzleSelectedColor puzzleEColor = puzzleSelectedColor.N;

	private int puzzleACorrect = -1;

	private int puzzleBCorrect = -1;

	private int puzzleCCorrect = -1;

	private int puzzleDCorrect = -1;

	private int puzzleECorrect = -1;

	public bool turnOnMap;

	private int mapSelectionPause;

	private bool mapPieceSelected;

	public bool mapIsComplete;

	public bool mapCompleteDisplayed;

	private Random myRandom = new Random();

	private RectangleActionData x = new RectangleActionData();

	public bool canClick;

	public RectangleActionData currentActionData = new RectangleActionData();

	private bool displayCursor;

	private Vector2 casePosition = new Vector2(220f, 540f);

	private string case1 = "PuzzleA";

	private string case2 = "PuzzleAs";

	private string case3 = "screwDriver";

	private string case4 = "shovel";

	private bool case1On;

	private bool case2On;

	private bool case3On;

	private bool case4On;

	private int currentSelectionTarget = -1;

	private int currentSelection = -1;

	private float elapsedTime;

	public string currentScene = "";

	private Vector2 casePosition1 = new Vector2(340f, 535f);

	private Vector2 casePosition1a = new Vector2(370f, 585f);

	private Vector2 casePosition2 = new Vector2(440f, 535f);

	private Vector2 casePosition2a = new Vector2(470f, 585f);

	private Vector2 casePosition3 = new Vector2(540f, 535f);

	private Vector2 casePosition3a = new Vector2(570f, 585f);

	private Vector2 casePosition4 = new Vector2(640f, 535f);

	private Vector2 casePosition4a = new Vector2(670f, 585f);

	public List<RectangleActionData> rad = new List<RectangleActionData>();

	private int clockWise;

	private int counterClockWise;

	private Vector2 tempVector;

	private bool displayInfo;

	private Vector2 oldDirection;

	private int Duration = 3000;

	private float tempI;

	private float tempO;

	public bool playSFX;

	public string SFXName = "Game Over 01";

	private float xDiff;

	private float yDiff;

	private byte c;

	private int increment = 86;

	private Color highLightColor = new Color(155, 155, 155, 155);

	private bool matchFound;

	private float distance = 1000f;

	private string matchingName = "";

	private float tempDistance;

	private float mapDepth = 0.97f;

	private bool mapPieceCorrect;

	private int bx;

	public void activateCursor()
	{
		inventory.Count();
		displayCursor = true;
	}

	public int inventoryCount()
	{
		return inventory.Count();
	}

	public void deactiveCursor()
	{
		displayCursor = false;
		inventoryClosed = true;
	}

	private int getRandom(int min, int max)
	{
		return myRandom.Next(min, max);
	}

	private void turnOffPiece()
	{
		if (turnOnPiece7)
		{
			turnOnPiece7 = false;
		}
		if (turnOnPiece9)
		{
			turnOnPiece9 = false;
		}
		if (turnOnPiece1)
		{
			turnOnPiece1 = false;
		}
		if (turnOnPiece2)
		{
			turnOnPiece2 = false;
		}
		if (turnOnPiece3)
		{
			turnOnPiece3 = false;
		}
		if (turnOnPiece5)
		{
			turnOnPiece5 = false;
		}
		if (turnOnPiece6)
		{
			turnOnPiece6 = false;
		}
		if (turnOnPiece8)
		{
			turnOnPiece8 = false;
		}
	}

	public void spinWheel()
	{
		if (textMessage != "")
		{
			textMessage = "";
		}
		isWheelSpin = true;
		rotationOutCurrent = MathHelper.WrapAngle(rotationOut);
		rotationInCurrent = MathHelper.WrapAngle(rotationIn);
		clockWise = getRandom(6, 20) + 5;
		counterClockWise = getRandom(-25, -11);
		if ((bottomCounterClockwise != topCounterClockwise) & (clockWise == Math.Abs(counterClockWise)))
		{
			clockWise += 2;
		}
		if (bottomCounterClockwise)
		{
			rotationInTarget = rotationInCurrent + (float)counterClockWise;
		}
		else
		{
			rotationInTarget = rotationInCurrent + (float)clockWise;
		}
		if (topCounterClockwise)
		{
			rotationOutTarget = rotationOutCurrent + (float)counterClockWise;
		}
		else
		{
			rotationOutTarget = rotationOutCurrent + (float)clockWise;
		}
	}

	public void addSceneData(string sceneName, SaveData saveData)
	{
		textMessage = "";
		currentScene = sceneName;
		rad.Clear();
		turnOnButterFly = false;
		turnOnMoneyFly = false;
		turnOnPiece9 = false;
		turnOnPiece7 = false;
		turnOnPiece1 = false;
		turnOnPiece2 = false;
		turnOnPiece3 = false;
		turnOnPiece5 = false;
		turnOnPiece6 = false;
		turnOnPiece8 = false;
		turnOnMap = false;
		turnOnWheel = false;
		isPuzzleGame = false;
		currentSelection = -1;
		switch (sceneName)
		{
		case "PathConversationA1":
			textPosition = new Vector2(120f, 500f);
			textMessage = "You can inverse the controller by pressing the <Back> button.";
			turnOnButterFly = true;
			turnOnMoneyFly = false;
			x = new RectangleActionData();
			x.rect = new Rectangle(830, 146, 158, 359);
			x.id = "TalkToNessa";
			x.nextRefId = 2;
			rad.Add(x);
			return;
		case "PathConversationA21":
			turnOnButterFly = true;
			turnOnMoneyFly = true;
			return;
		case "Path1":
			if (!saveData.checkForLocation("Piece4"))
			{
				saveData.addLocationData("Piece4");
				saveData.addVariables("Piece4");
				addItem(saveData, "Piece4");
				updateInventory();
			}
			else if (inventoryCount() == 0)
			{
				saveData.addVariables("Piece4");
				addItem(saveData, "Piece4");
				updateInventory();
			}
			turnOnButterFly = true;
			turnOnMoneyFly = false;
			x = new RectangleActionData();
			x.rect = new Rectangle(830, 146, 158, 359);
			x.id = "TalkToNessa";
			x.nextRefId = 29;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(449, 157, 347, 214);
			x.id = "ToPathB";
			x.nextRefId = 2;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			return;
		case "Path2":
			x = new RectangleActionData();
			x.rect = new Rectangle(536, 231, 141, 138);
			x.id = "ToPathBA";
			x.nextRefId = 4;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(888, 209, 392, 323);
			x.id = "ToPathC";
			x.nextRefId = 3;
			x.collisionType = RectangleActionData.RectCollisionType.Right;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathA";
			x.nextRefId = 1;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path3":
			if (!saveData.checkForVariable("Piece9"))
			{
				turnOnPiece9 = true;
			}
			x = new RectangleActionData();
			x.rect = new Rectangle(275, 26, 701, 239);
			x.id = "DesertNeedMap";
			x.nextRefId = 1;
			x.chapterRef = "Mirage";
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			if (turnOnPiece9)
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(704, 397, 172, 23);
				x.id = "ToPathBB";
				x.nextRefId = 26;
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathB";
			x.nextRefId = 2;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path26":
			x = new RectangleActionData();
			x.rect = new Rectangle(629, 288, 117, 56);
			x.id = "Piece9";
			x.nextRefId = -1;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathB";
			x.nextRefId = 3;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path27":
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathB";
			x.nextRefId = 3;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path4":
			if (!saveData.checkForVariable("Piece7"))
			{
				turnOnPiece7 = true;
			}
			if (turnOnPiece7)
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(644, 439, 30, 22);
				x.id = "Piece7";
				x.nextRefId = -1;
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			x = new RectangleActionData();
			x.rect = new Rectangle(229, 119, 242, 275);
			x.id = "ToPathCA";
			x.nextRefId = 5;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(808, 131, 301, 268);
			x.id = "ToPathD";
			x.nextRefId = 8;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathB";
			x.nextRefId = 2;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path5":
			if (!saveData.checkForVariable("Piece3"))
			{
				turnOnPiece3 = true;
			}
			if (turnOnPiece3)
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(977, 571, 35, 23);
				x.id = "Piece3";
				x.nextRefId = -1;
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			x = new RectangleActionData();
			x.rect = new Rectangle(538, 198, 413, 226);
			x.id = "ToPathD";
			x.nextRefId = 6;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathC";
			x.nextRefId = 4;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path6":
			x = new RectangleActionData();
			x.rect = new Rectangle(448, 383, 60, 90);
			x.id = "ToPathD";
			x.nextRefId = 7;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathC";
			x.nextRefId = 5;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path7":
			if (!saveData.checkForVariable("Piece1"))
			{
				turnOnPiece1 = true;
			}
			if (turnOnPiece1)
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(900, 567, 86, 32);
				x.id = "Piece1";
				x.nextRefId = -1;
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			if (!saveData.checkForVariable("IanConversationA"))
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(340, 100, 340, 419);
				x.id = "ToIanConversation";
				x.nextRefId = 1;
				x.chapterRef = "IanConversationA";
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			else if (saveData.checkForVariable("NeedScrewDriver") & !saveData.checkForVariable("screwDriver"))
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(340, 100, 340, 419);
				x.id = "ToIanConversation";
				x.nextRefId = 1;
				x.chapterRef = "IanConversationScrew";
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			else if (saveData.checkForVariable("screwDriver"))
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(340, 100, 340, 419);
				x.id = "ToIanConversation";
				x.nextRefId = 1;
				x.chapterRef = "IanConversationC";
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			else
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(340, 100, 340, 419);
				x.id = "ToIanConversation";
				x.nextRefId = 1;
				x.chapterRef = "IanConversationB";
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 599, 740, 123);
			x.id = "ToPathCB";
			x.nextRefId = 6;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path8":
			if (!saveData.checkForVariable("Piece6"))
			{
				turnOnPiece6 = true;
			}
			if (turnOnPiece6)
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(877, 356, 71, 67);
				x.id = "Piece6";
				x.nextRefId = -1;
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			x = new RectangleActionData();
			x.rect = new Rectangle(476, 170, 229, 230);
			x.id = "ToPathD";
			x.nextRefId = 9;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathC";
			x.nextRefId = 4;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path9":
			if (!saveData.checkForVariable("Piece8"))
			{
				turnOnPiece8 = true;
			}
			if (turnOnPiece8)
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(605, 507, 28, 16);
				x.id = "Piece8";
				x.nextRefId = -1;
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			x = new RectangleActionData();
			x.rect = new Rectangle(425, 214, 197, 162);
			x.id = "ToPathF";
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			if (saveData.checkForVariable("GateFOpen"))
			{
				x.nextRefId = 25;
			}
			else
			{
				x.nextRefId = 12;
			}
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(980, 229, 206, 206);
			x.id = "ToPathEA";
			x.nextRefId = 10;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathC";
			x.nextRefId = 8;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path10":
			if (!saveData.checkForVariable("Piece2"))
			{
				turnOnPiece2 = true;
			}
			if (turnOnPiece2)
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(252, 323, 52, 54);
				x.id = "Piece2";
				x.nextRefId = -1;
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			x = new RectangleActionData();
			x.rect = new Rectangle(455, 23, 214, 175);
			x.id = "ToPathEB";
			if (saveData.checkForVariable("shovel"))
			{
				x.nextRefId = 17;
			}
			else
			{
				x.nextRefId = 11;
			}
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathE";
			x.nextRefId = 9;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path17":
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathC";
			x.nextRefId = 10;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path11":
			if (!saveData.checkForVariable("shovel"))
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(601, 279, 177, 265);
				x.id = "OpenDoor";
				x.nextRefId = 16;
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathC";
			x.nextRefId = 10;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path16":
			if (!saveData.checkForVariable("shovel"))
			{
				addInventoryData("shovel");
				saveData.addVariables("shovel");
			}
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathC";
			x.nextRefId = 10;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path12":
			x = new RectangleActionData();
			x.rect = new Rectangle(569, 450, 146, 100);
			x.id = "Wheel";
			x.nextRefId = 20;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathC";
			x.nextRefId = 9;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path24":
			x = new RectangleActionData();
			x.rect = new Rectangle(557, 334, 207, 166);
			x.id = "GotoG";
			x.nextRefId = 13;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathC";
			x.nextRefId = 9;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path25":
			x = new RectangleActionData();
			x.rect = new Rectangle(557, 334, 207, 166);
			x.id = "GotoG";
			x.nextRefId = 13;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathC";
			x.nextRefId = 9;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path20":
			turnOnWheel = true;
			textPosition = new Vector2(120f, 500f);
			textMessage = "I can turn the outer ring with the left joystick.";
			x = new RectangleActionData();
			x.rect = new Rectangle(491, 216, 295, 155);
			x.id = "UnscrewCover";
			x.nextRefId = 21;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(604, 413, 71, 75);
			x.id = "Wheel";
			x.nextRefId = 20;
			x.collisionType = RectangleActionData.RectCollisionType.Spin;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathF";
			x.nextRefId = 12;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path13":
			if (!saveData.checkForVariable("Piece5"))
			{
				piece5Location = new Vector2(-100f, 350f);
				turnOnPiece5 = true;
			}
			if (turnOnPiece5)
			{
				x = new RectangleActionData();
				x.rect = new Rectangle(704, 397, 172, 23);
				x.id = "Piece5";
				x.nextRefId = -1;
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
			}
			x = new RectangleActionData();
			x.rect = new Rectangle(476, 170, 229, 130);
			x.id = "ToPathH";
			x.nextRefId = 14;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathF";
			x.nextRefId = 25;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path14":
			x = new RectangleActionData();
			x.rect = new Rectangle(476, 170, 229, 230);
			x.id = "ToPathA";
			x.nextRefId = 18;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathC";
			x.nextRefId = 13;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Path15":
			textPosition = new Vector2(120f, 500f);
			textMessage = "Select a map piece to move by pressing (A)";
			turnOnMap = true;
			return;
		case "Path19":
			turnOnMap = false;
			return;
		case "Path23":
			x = new RectangleActionData();
			x.rect = new Rectangle(502, 174, 459, 132);
			x.id = "Top";
			x.nextRefId = 22;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(502, 408, 459, 87);
			x.id = "Bottom";
			x.nextRefId = 22;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(236, 566, 740, 156);
			x.id = "ToPathC";
			x.nextRefId = 22;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "DesertWalkA1":
			textMessage = "View your map to see the current riddle";
			addRecForWalking(2, 9, 0, 0, 0, 0, "", 11);
			return;
		case "DesertWalkA2":
			addRecForWalking(3, 6, 522, 484, 803, 589, "You see several Monarch butterflies.", 11);
			return;
		case "DesertWalkA3":
			addRecForWalking(4, 7, 387, 583, 498, 633, "You see some pennies lying on the ground.", 10);
			return;
		case "DesertWalkA4":
			addRecForWalking(5, 8, 334, 572, 380, 616, "You see an old pocket watch lying on the ground.", 11);
			return;
		case "DesertWalkA5":
			addRecForWalking(2, 9, 0, 0, 0, 0, "", 11);
			return;
		case "DesertWalkA6":
			addRecForWalking(2, 9, 0, 0, 0, 0, "", 11);
			return;
		case "DesertWalkA7":
			addRecForWalking(3, 6, 522, 484, 803, 589, "You see several Monarch butterflies.", 11);
			return;
		case "DesertWalkA8":
			addRecForWalking(4, 7, 387, 583, 498, 633, "You see some pennies lying on the ground.", 10);
			return;
		case "DesertWalkA9":
			addRecForWalking(5, 8, 334, 572, 380, 616, "You see an old pocket watch lying on the ground.", 11);
			return;
		case "DesertWalkC1":
			addRecForWalking(2, 9, 445, 523, 569, 614, "You see some clover.", 10);
			return;
		case "DesertWalkC2":
			addRecForWalking(3, 6, 225, 353, 563, 550, "Hmmm...it is a large anvil.", 11);
			return;
		case "DesertWalkC3":
			addRecForWalking(4, 7, 361, 532, 401, 614, "You see a rusty knife on the ground.", 11);
			return;
		case "DesertWalkC4":
			addRecForWalking(5, 8, 695, 440, 825, 546, "Darn...only dust in that watering can.", 11);
			return;
		case "DesertWalkC5":
			addRecForWalking(2, 9, 445, 523, 569, 614, "You see some clover.", 10);
			return;
		case "DesertWalkC6":
			addRecForWalking(2, 9, 445, 523, 569, 614, "You see some clover.", 10);
			return;
		case "DesertWalkC7":
			addRecForWalking(3, 6, 225, 353, 563, 550, "Hmmm...it is a large anvil.", 11);
			return;
		case "DesertWalkC8":
			addRecForWalking(4, 7, 361, 532, 401, 614, "You see a rusty knife on the ground.", 11);
			return;
		case "DesertWalkC9":
			addRecForWalking(5, 8, 695, 440, 825, 546, "Darn...only dust in that watering can.", 11);
			return;
		case "DesertWalkD1":
			addRecForWalking(2, 9, 857, 451, 1053, 623, "Odd to see trafic cones out here.", 11);
			return;
		case "DesertWalkD2":
			addRecForWalking(3, 6, 264, 409, 424, 560, "It is a trash bag full of...yeah - trash.", 11);
			return;
		case "DesertWalkD3":
			addRecForWalking(4, 7, 274, 419, 969, 517, "It is some sort of old statue.", 11);
			return;
		case "DesertWalkD4":
			addRecForWalking(5, 8, 657, 494, 967, 564, "You see an old rake.", 10);
			return;
		case "DesertWalkD5":
			addRecForWalking(2, 9, 857, 451, 1053, 623, "Odd to see trafic cones out here.", 11);
			return;
		case "DesertWalkD6":
			addRecForWalking(2, 9, 857, 451, 1053, 623, "Odd to see trafic cones out here.", 11);
			return;
		case "DesertWalkD7":
			addRecForWalking(3, 6, 264, 409, 424, 560, "It is a trash bag full of...yeah - trash.", 11);
			return;
		case "DesertWalkD8":
			addRecForWalking(4, 7, 274, 419, 969, 517, "It is some sort of old statue.", 11);
			return;
		case "DesertWalkD9":
			addRecForWalking(5, 8, 657, 494, 967, 564, "You see an old rake.", 10);
			return;
		case "DesertWalkB1":
			addRecForWalking(2, 9, 0, 0, 0, 0, "", 11);
			return;
		case "DesertWalkB2":
			addRecForWalking(3, 6, 826, 548, 915, 633, "No time for games now.", 11);
			return;
		case "DesertWalkB3":
			addRecForWalking(4, 7, 625, 586, 686, 642, "Odd place for a game of horseshoes.", 10);
			return;
		case "DesertWalkB4":
			addRecForWalking(5, 8, 0, 0, 0, 0, "", 11);
			return;
		case "DesertWalkB5":
			addRecForWalking(2, 9, 0, 0, 0, 0, "", 11);
			return;
		case "DesertWalkB6":
			addRecForWalking(2, 9, 0, 0, 0, 0, "", 11);
			return;
		case "DesertWalkB7":
			addRecForWalking(3, 6, 826, 548, 915, 633, "No time for games now.", 11);
			return;
		case "DesertWalkB8":
			addRecForWalking(4, 7, 625, 586, 686, 642, "Odd place for a game of horseshoes.", 10);
			return;
		case "DesertWalkB9":
			addRecForWalking(5, 8, 0, 0, 0, 0, "", 11);
			return;
		case "Pit1":
			inventory.Remove("I_PitKeyA");
			x = new RectangleActionData();
			x.rect = new Rectangle(466, 220, 317, 215);
			x.id = "Up";
			x.nextRefId = 2;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			return;
		case "Pit2":
			textPosition = new Vector2(120f, 130f);
			x = new RectangleActionData();
			x.rect = new Rectangle(627, 363, 48, 31);
			x.id = "Get Key";
			x.nextRefId = 3;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(301, 326, 72, 48);
			x.id = "Messsage";
			x.nextRefId = -1;
			x.displayText = "The plaque says:" + Environment.NewLine + Environment.NewLine + "Your father was a liar and a cheat. His fortune was built on a" + Environment.NewLine + "mountain of dishonesty.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(914, 328, 75, 43);
			x.id = "Messsage";
			x.nextRefId = -1;
			x.displayText = "The plaque says:" + Environment.NewLine + Environment.NewLine + "Your father was an honest and diligent man. He was admired by many" + Environment.NewLine + "for these traits.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(390, 372, 49, 50);
			x.id = "DoorA";
			x.nextRefId = -1;
			x.displayText = "Door is locked.  There is a plaque on the door.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(922, 371, 118, 50);
			x.id = "DoorB";
			x.nextRefId = -1;
			x.displayText = "Door is locked.  There is a plaque on the door.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			return;
		case "Pit3":
			textPosition = new Vector2(120f, 130f);
			addInventoryData("I_PitKeyA");
			x = new RectangleActionData();
			x.rect = new Rectangle(627, 363, 48, 31);
			x.id = "Get Key";
			x.nextRefId = 3;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(301, 326, 72, 48);
			x.id = "Messsage";
			x.nextRefId = -1;
			x.displayText = "The plaque says:" + Environment.NewLine + Environment.NewLine + "Your father was a liar and a cheat. His fortune was built on a" + Environment.NewLine + "mountain of dishonesty.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(914, 328, 75, 43);
			x.id = "Messsage";
			x.nextRefId = -1;
			x.displayText = "The plaque says:" + Environment.NewLine + Environment.NewLine + "Your father was an honest and diligent man. He was admired by many" + Environment.NewLine + "for these traits.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(390, 372, 49, 50);
			x.id = "DoorA";
			x.nextRefId = 4;
			x.displayText = "Door is locked.  There is a plaque on the door.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(922, 371, 118, 50);
			x.id = "DoorB";
			x.nextRefId = 7;
			x.displayText = "Door is locked.  There is a plaque on the door.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			return;
		case "Pit9":
			textPosition = new Vector2(120f, 130f);
			x = new RectangleActionData();
			x.rect = new Rectangle(301, 455, 47, 23);
			x.id = "Messsage";
			x.nextRefId = -1;
			x.displayText = "The plaque says:" + Environment.NewLine + Environment.NewLine + "Your father was a faithful husband.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(934, 454, 41, 21);
			x.id = "Messsage";
			x.nextRefId = -1;
			x.displayText = "The plaque says:" + Environment.NewLine + Environment.NewLine + "Your father cheated on your mother.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(418, 3, 81, 581);
			x.id = "UpA";
			x.nextRefId = 10;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(753, 5, 90, 582);
			x.id = "UpB";
			x.nextRefId = 12;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			return;
		case "Pit11":
			textPosition = new Vector2(120f, 130f);
			x = new RectangleActionData();
			x.rect = new Rectangle(382, 219, 43, 22);
			x.id = "Messsage";
			x.nextRefId = -1;
			x.displayText = "The plaque says:" + Environment.NewLine + Environment.NewLine + "Your father bribed your way into the elite universities you attended.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(916, 243, 44, 23);
			x.id = "Messsage";
			x.nextRefId = -1;
			x.displayText = "The plaque says:" + Environment.NewLine + Environment.NewLine + "Your hard work and test scores got you where you are today.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(432, 150, 188, 188);
			x.id = "UpA";
			x.nextRefId = 13;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(945, 166, 144, 76);
			x.id = "UpB";
			x.nextRefId = 15;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(776, 270, 133, 86);
			x.id = "UpB";
			x.nextRefId = 15;
			x.collisionType = RectangleActionData.RectCollisionType.Up;
			rad.Add(x);
			return;
		case "Pit16":
			textPosition = new Vector2(120f, 130f);
			x = new RectangleActionData();
			x.rect = new Rectangle(406, 148, 63, 28);
			x.id = "Messsage";
			x.nextRefId = -1;
			x.displayText = "The plaque says:" + Environment.NewLine + Environment.NewLine + "Your current wealth is based on luck.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(953, 150, 128, 76);
			x.id = "Messsage";
			x.nextRefId = -1;
			x.displayText = "The plaque says:" + Environment.NewLine + Environment.NewLine + "Your current wealth is based on skill.";
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(149, 339, 302, 76);
			x.id = "DownA";
			x.nextRefId = 17;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(755, 409, 399, 146);
			x.id = "DownB";
			x.nextRefId = 20;
			x.collisionType = RectangleActionData.RectCollisionType.Down;
			rad.Add(x);
			return;
		case "Maze2":
			isPuzzleGame = true;
			currentPuzzleState = puzzleGameState.A;
			puzzleACount = 0;
			puzzleTimer = 0;
			puzzleTimerLoop = 0;
			puzzleAColor = puzzleSelectedColor.N;
			puzzleBColor = puzzleSelectedColor.N;
			puzzleCColor = puzzleSelectedColor.N;
			puzzleDColor = puzzleSelectedColor.N;
			puzzleEColor = puzzleSelectedColor.N;
			puzzleACorrect = -1;
			puzzleBCorrect = -1;
			puzzleCCorrect = -1;
			puzzleDCorrect = -1;
			puzzleECorrect = -1;
			inventory.Clear();
			x = new RectangleActionData();
			x.rect = new Rectangle(970, 104, 73, 116);
			x.id = "Green";
			x.nextRefId = -1;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(973, 334, 74, 112);
			x.id = "Red";
			x.nextRefId = -1;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(874, 535, 160, 68);
			x.id = "Next";
			x.nextRefId = -1;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(584, 535, 161, 68);
			x.id = "Done";
			x.nextRefId = -1;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(303, 535, 164, 68);
			x.id = "Back";
			x.nextRefId = -1;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			return;
		case "Maze42":
			isPuzzleGame = true;
			currentPuzzleState = puzzleGameState.A;
			puzzleACount = 0;
			puzzleTimer = 0;
			correctAnswers = 0;
			puzzleTimerLoop = 0;
			puzzleAColor = puzzleSelectedColor.N;
			puzzleBColor = puzzleSelectedColor.N;
			puzzleCColor = puzzleSelectedColor.N;
			puzzleDColor = puzzleSelectedColor.N;
			puzzleEColor = puzzleSelectedColor.N;
			puzzleACorrect = -1;
			puzzleBCorrect = -1;
			puzzleCCorrect = -1;
			puzzleDCorrect = -1;
			puzzleECorrect = -1;
			puzzleACount = 0;
			puzzleBCount = 0;
			puzzleCCount = 0;
			puzzleDCount = 0;
			puzzleECount = 0;
			inventory.Clear();
			x = new RectangleActionData();
			x.rect = new Rectangle(970, 104, 73, 116);
			x.id = "Green";
			x.nextRefId = -1;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(973, 334, 74, 112);
			x.id = "Red";
			x.nextRefId = -1;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(874, 535, 160, 68);
			x.id = "Next";
			x.nextRefId = -1;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(584, 535, 161, 68);
			x.id = "Done";
			x.nextRefId = -1;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			x = new RectangleActionData();
			x.rect = new Rectangle(303, 535, 164, 68);
			x.id = "Back";
			x.nextRefId = -1;
			x.collisionType = RectangleActionData.RectCollisionType.Action;
			rad.Add(x);
			return;
		}
		if (!((sceneName == "MazeA1") | (sceneName == "MazeB1") | (sceneName == "MazeC1")))
		{
			switch (sceneName)
			{
			default:
				return;
			case "MazeD1":
				break;
			case "MazeX1":
				x = new RectangleActionData();
				x.rect = new Rectangle(585, 280, 152, 159);
				x.id = "UpA";
				x.nextRefId = 2;
				x.collisionType = RectangleActionData.RectCollisionType.Up;
				rad.Add(x);
				return;
			case "MazeX3":
				x = new RectangleActionData();
				x.rect = new Rectangle(588, 262, 112, 163);
				x.id = "Face";
				x.nextRefId = 4;
				x.collisionType = RectangleActionData.RectCollisionType.Action;
				rad.Add(x);
				return;
			case "MazeA3":
				if (!saveData.checkForVariable("FailedMazeA"))
				{
					x = new RectangleActionData();
					x.rect = new Rectangle(588, 262, 112, 163);
					x.id = "Face";
					x.nextRefId = 4;
					x.collisionType = RectangleActionData.RectCollisionType.Action;
					rad.Add(x);
				}
				else if (saveData.checkForVariable("CompleteMazeA"))
				{
					x = new RectangleActionData();
					x.rect = new Rectangle(588, 262, 112, 163);
					x.id = "Face";
					x.nextRefId = 42;
					x.collisionType = RectangleActionData.RectCollisionType.Action;
					rad.Add(x);
				}
				x = new RectangleActionData();
				x.rect = new Rectangle(226, 139, 230, 447);
				x.id = "LEft";
				x.nextRefId = 43;
				x.collisionType = RectangleActionData.RectCollisionType.Left;
				rad.Add(x);
				x = new RectangleActionData();
				x.rect = new Rectangle(833, 139, 180, 447);
				x.id = "Right";
				x.nextRefId = 37;
				x.collisionType = RectangleActionData.RectCollisionType.Right;
				rad.Add(x);
				return;
			case "MazeB3":
				if (!saveData.checkForVariable("FailedMazeB"))
				{
					x = new RectangleActionData();
					x.rect = new Rectangle(588, 262, 112, 163);
					x.id = "Face";
					x.nextRefId = 4;
					x.collisionType = RectangleActionData.RectCollisionType.Action;
					rad.Add(x);
				}
				else if (saveData.checkForVariable("CompleteMazeB"))
				{
					x = new RectangleActionData();
					x.rect = new Rectangle(588, 262, 112, 163);
					x.id = "Face";
					x.nextRefId = 14;
					x.collisionType = RectangleActionData.RectCollisionType.Action;
					rad.Add(x);
				}
				x = new RectangleActionData();
				x.rect = new Rectangle(226, 139, 230, 447);
				x.id = "LEft";
				x.nextRefId = 42;
				x.collisionType = RectangleActionData.RectCollisionType.Left;
				rad.Add(x);
				x = new RectangleActionData();
				x.rect = new Rectangle(833, 139, 180, 447);
				x.id = "Right";
				x.nextRefId = 43;
				x.collisionType = RectangleActionData.RectCollisionType.Right;
				rad.Add(x);
				return;
			case "MazeC3":
				if (!saveData.checkForVariable("FailedMazeC"))
				{
					x = new RectangleActionData();
					x.rect = new Rectangle(588, 262, 112, 163);
					x.id = "Face";
					x.nextRefId = 4;
					x.collisionType = RectangleActionData.RectCollisionType.Action;
					rad.Add(x);
				}
				else if (saveData.checkForVariable("CompleteMazeC"))
				{
					x = new RectangleActionData();
					x.rect = new Rectangle(588, 262, 112, 163);
					x.id = "Face";
					x.nextRefId = 16;
					x.collisionType = RectangleActionData.RectCollisionType.Action;
					rad.Add(x);
				}
				x = new RectangleActionData();
				x.rect = new Rectangle(226, 139, 230, 447);
				x.id = "LEft";
				x.nextRefId = 42;
				x.collisionType = RectangleActionData.RectCollisionType.Left;
				rad.Add(x);
				x = new RectangleActionData();
				x.rect = new Rectangle(833, 139, 180, 447);
				x.id = "Right";
				x.nextRefId = 43;
				x.collisionType = RectangleActionData.RectCollisionType.Right;
				rad.Add(x);
				return;
			case "MazeD3":
				x = new RectangleActionData();
				x.rect = new Rectangle(585, 280, 152, 159);
				x.id = "UpA";
				x.nextRefId = 4;
				x.collisionType = RectangleActionData.RectCollisionType.Up;
				rad.Add(x);
				return;
			}
		}
		saveData.removeVariable("MazeAStart");
		saveData.removeVariable("MazeBStart");
		saveData.removeVariable("MazeCStart");
		switch (sceneName)
		{
		case "MazeA1":
			saveData.addVariables("MazeAStart");
			break;
		case "MazeB1":
			saveData.addVariables("MazeBStart");
			break;
		case "MazeC1":
			saveData.addVariables("MazeCStart");
			break;
		}
		x = new RectangleActionData();
		x.rect = new Rectangle(585, 280, 152, 159);
		x.id = "UpA";
		x.nextRefId = 2;
		x.collisionType = RectangleActionData.RectCollisionType.Up;
		rad.Add(x);
	}

	private void addRecForWalking(int left, int right, int x1, int y1, int x2, int y2, string message, int nextId)
	{
		if (x1 != x2)
		{
			textPosition = new Vector2(120f, 500f);
			x = new RectangleActionData();
			x.rect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
			x.id = "Messsage";
			x.nextRefId = -1;
			x.displayText = message;
			x.collisionType = RectangleActionData.RectCollisionType.Info;
			rad.Add(x);
		}
		x = new RectangleActionData();
		x.rect = new Rectangle(0, 0, 200, 720);
		x.id = "Left";
		x.nextRefId = left;
		x.collisionType = RectangleActionData.RectCollisionType.Left;
		rad.Add(x);
		x = new RectangleActionData();
		x.rect = new Rectangle(1000, 0, 1280, 720);
		x.id = "Right";
		x.nextRefId = right;
		x.collisionType = RectangleActionData.RectCollisionType.Right;
		rad.Add(x);
		x = new RectangleActionData();
		x.rect = new Rectangle(274, 51, 710, 299);
		x.id = "Up";
		x.nextRefId = nextId;
		x.collisionType = RectangleActionData.RectCollisionType.Up;
		rad.Add(x);
	}

	private void testData()
	{
		RectangleActionData rectangleActionData = new RectangleActionData();
		rectangleActionData.rect = new Rectangle(640, 100, 200, 300);
		rad.Add(rectangleActionData);
		rectangleActionData = new RectangleActionData();
		rectangleActionData.rect = new Rectangle(0, 0, 1280, 200);
		rectangleActionData.collisionType = RectangleActionData.RectCollisionType.Up;
		rectangleActionData = new RectangleActionData();
		rectangleActionData.rect = new Rectangle(0, 520, 1280, 200);
		rectangleActionData.collisionType = RectangleActionData.RectCollisionType.Down;
		rectangleActionData = new RectangleActionData();
		rectangleActionData.rect = new Rectangle(0, 0, 200, 720);
		rectangleActionData.collisionType = RectangleActionData.RectCollisionType.Left;
		rectangleActionData = new RectangleActionData();
		rectangleActionData.rect = new Rectangle(1000, 0, 200, 720);
		rectangleActionData.collisionType = RectangleActionData.RectCollisionType.Right;
	}

	private bool checkRectangle(Vector2 c, Rectangle r)
	{
		return r.Contains((int)c.X, (int)c.Y);
	}

	private void checkInventoryTargets()
	{
		inventoryItemTarget = false;
		currentSelectionTarget = -1;
		if (!(!inventoryClosed & case1On & (cursorLocation.Y > 544f) & (cursorLocation.Y < 631f)))
		{
			return;
		}
		if ((cursorLocation.X > 322f) & (cursorLocation.X < 409f))
		{
			displayType = displayTypeEnum.inventorySelect;
			currentSelectionTarget = 1;
			inventoryItemTarget = true;
			canClick = true;
		}
		else if (case2On & (cursorLocation.X > 422f) & (cursorLocation.X < 509f))
		{
			displayType = displayTypeEnum.inventorySelect;
			currentSelectionTarget = 2;
			inventoryItemTarget = true;
			canClick = true;
		}
		else if (case3On & (cursorLocation.X > 522f) & (cursorLocation.X < 609f))
		{
			displayType = displayTypeEnum.inventorySelect;
			currentSelectionTarget = 3;
			inventoryItemTarget = true;
			canClick = true;
		}
		else if (case4On & (cursorLocation.X > 622f) & (cursorLocation.X < 709f))
		{
			displayType = displayTypeEnum.inventorySelect;
			currentSelectionTarget = 4;
			inventoryItemTarget = true;
			canClick = true;
		}
		if (!(getCurrentInventoryItem() == "I_PitKeyA"))
		{
			return;
		}
		foreach (RectangleActionData item in rad)
		{
			if ((item.id == "DoorA") | (item.id == "DoorB"))
			{
				item.collisionType = RectangleActionData.RectCollisionType.Action;
			}
		}
	}

	private void addInventoryData(string item)
	{
		bool flag = false;
		foreach (string item2 in inventory)
		{
			if (item2 == item)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			inventory.Add(item);
		}
	}

	public void addItem(SaveData saveData, string item)
	{
		if ((item == "screwDriver") | (item == "shovel") | (item == "I_PitKeyA"))
		{
			addInventoryData(item);
			saveData.inventorySaveList = inventory;
			updateInventoryData();
		}
		else if (item == "MapComplete")
		{
			foreach (string item2 in inventory)
			{
				if (item2 == "PuzzleA")
				{
					inventory.Remove(item2);
					break;
				}
			}
			foreach (string item3 in inventory)
			{
				if (item3 == "PuzzleAs")
				{
					inventory.Remove(item3);
					break;
				}
			}
			addInventoryData(item);
			saveData.inventorySaveList = inventory;
			updateInventoryData();
		}
		else
		{
			turnOffPiece();
			bool flag = false;
			if (inventory.Count() == 0)
			{
				addInventoryData("PuzzleA");
			}
			else
			{
				foreach (string item4 in inventory)
				{
					if (item4 == "PuzzleA")
					{
						inventory.Remove(item4);
						flag = true;
						break;
					}
				}
				if (flag)
				{
					addInventoryData("PuzzleAs");
				}
			}
			saveData.inventorySaveList = inventory;
		}
		if (!isPuzzleGame)
		{
			rad.Remove(currentActionData);
		}
		currentActionData = new RectangleActionData();
	}

	public void resetInventory(SaveData saveData)
	{
		saveData.inventorySaveList = inventory;
		inventory.Clear();
	}

	public void loadInventory(SaveData saveData)
	{
		inventory.Clear();
		inventory = saveData.inventorySaveList;
		updateInventory();
	}

	public bool selectInventoryItem(string chapter)
	{
		currentSelection = currentSelectionTarget;
		switch (currentSelection)
		{
		case 1:
			if ((case1 == "PuzzleA") | (case1 == "PuzzleAs"))
			{
				return true;
			}
			break;
		case 2:
			if ((case2 == "PuzzleA") | (case2 == "PuzzleAs"))
			{
				return true;
			}
			break;
		case 3:
			if ((case2 == "PuzzleA") | (case3 == "PuzzleAs"))
			{
				return true;
			}
			break;
		case 4:
			if ((case2 == "PuzzleA") | (case4 == "PuzzleAs"))
			{
				return true;
			}
			break;
		}
		if ((chapter != "Path") & (chapter != "Pit") & (chapter != "Maze"))
		{
			switch (currentSelection)
			{
			case 1:
				if (case1 == "MapComplete")
				{
					return true;
				}
				break;
			case 2:
				if (case2 == "MapComplete")
				{
					return true;
				}
				break;
			case 3:
				if (case3 == "MapComplete")
				{
					return true;
				}
				break;
			case 4:
				if (case4 == "MapComplete")
				{
					return true;
				}
				break;
			}
		}
		return false;
	}

	private string getCurrentInventoryItem()
	{
		return currentSelection switch
		{
			1 => case1, 
			2 => case2, 
			3 => case3, 
			4 => case4, 
			_ => "NA", 
		};
	}

	private void checkInTarget()
	{
		displayType = displayTypeEnum.normal;
		canClick = false;
		checkInventoryTargets();
		if (displayType != displayTypeEnum.inventorySelect)
		{
			displayInfo = false;
			foreach (RectangleActionData item in rad)
			{
				if (item.id == "Piece5")
				{
					item.rect = new Rectangle(Convert.ToInt32(piece5Location.X) - 25, Convert.ToInt32(piece5Location.Y) - 25, 70, 70);
				}
				if (checkRectangle(cursorLocation, item.rect))
				{
					if (item.collisionType == RectangleActionData.RectCollisionType.Action)
					{
						displayType = displayTypeEnum.overAction;
						canClick = true;
						currentActionData = item;
					}
					else if (item.collisionType == RectangleActionData.RectCollisionType.Spin)
					{
						displayType = displayTypeEnum.overAction;
						canClick = true;
						currentActionData = item;
					}
					else if (item.collisionType == RectangleActionData.RectCollisionType.Up)
					{
						displayType = displayTypeEnum.arrowUp;
						arrowImage = "arrowUp";
						canClick = true;
						currentActionData = item;
					}
					else if (item.collisionType == RectangleActionData.RectCollisionType.Down)
					{
						displayType = displayTypeEnum.arrowDown;
						arrowImage = "arrowDown";
						canClick = true;
						currentActionData = item;
					}
					else if (item.collisionType == RectangleActionData.RectCollisionType.Left)
					{
						displayType = displayTypeEnum.arrowLeft;
						arrowImage = "arrowLeft";
						canClick = true;
						currentActionData = item;
					}
					else if (item.collisionType == RectangleActionData.RectCollisionType.Right)
					{
						displayType = displayTypeEnum.arrowRight;
						arrowImage = "arrowRight";
						canClick = true;
						currentActionData = item;
					}
					else if (item.collisionType == RectangleActionData.RectCollisionType.Info)
					{
						displayInfo = true;
						textMessage = item.displayText;
						displayType = displayTypeEnum.normal;
						canClick = false;
						currentActionData = item;
					}
				}
			}
		}
		if (!turnOnMoneyFly)
		{
			return;
		}
		if (cursorLocation.X > butteryFlyAPos.X)
		{
			tempVector.X = cursorLocation.X - butteryFlyAPos.X;
		}
		else
		{
			tempVector.X = butteryFlyAPos.X - cursorLocation.X;
		}
		if (tempVector.X < 30f)
		{
			if (cursorLocation.Y > butteryFlyAPos.Y)
			{
				tempVector.Y = cursorLocation.Y - butteryFlyAPos.Y;
			}
			else
			{
				tempVector.Y = butteryFlyAPos.Y - cursorLocation.Y;
			}
			if (tempVector.Y < 30f)
			{
				displayType = displayTypeEnum.overAction;
				canClick = true;
				currentActionData.id = "MoneyFly";
				currentActionData.collisionType = RectangleActionData.RectCollisionType.Action;
				currentActionData.nextRefId = 22;
			}
		}
	}

	private void updateInventoryData()
	{
		case1On = false;
		case2On = false;
		case3On = false;
		case4On = false;
		int num = 0;
		foreach (string item in inventory)
		{
			num++;
			switch (num)
			{
			case 1:
				case1 = item;
				case1On = true;
				break;
			case 2:
				case2 = item;
				case2On = true;
				break;
			case 3:
				case3 = item;
				case3On = true;
				break;
			case 4:
				case4 = item;
				case4On = true;
				break;
			}
		}
	}

	private void updatePiece9()
	{
		if (!turnOnPiece9)
		{
			return;
		}
		if (piece9LocationLeft)
		{
			piece9Location.X -= 0.1f;
			if (piece9Location.X < 800f)
			{
				piece9LocationLeft = false;
			}
		}
		else
		{
			piece9Location.X += 0.1f;
			if (piece9Location.X > 940f)
			{
				piece9LocationLeft = true;
			}
		}
	}

	private void updatePiece5()
	{
		if (turnOnPiece5)
		{
			if (piece5Location.X < 1280f)
			{
				piece5Location.X += 4f;
			}
			else
			{
				turnOnPiece5 = false;
			}
			if ((piece5Location.X > 100f) & (piece5Location.X < 600f))
			{
				piece5Rotation += 0.1f;
				piece5Location.Y -= 0.35f;
				piece5Location.X += 0.05f;
			}
			else if ((piece5Location.X > 600f) & (piece5Location.X < 1280f))
			{
				piece5Rotation += 0.05f;
				piece5Location.Y += 0.45f;
				piece5Location.X -= 0.05f;
			}
		}
	}

	public bool toggleInventory()
	{
		if ((inventoryFrameCount == 0) | (inventoryFrameCount == 12))
		{
			if (inventoryClosed)
			{
				updateInventoryData();
				inventoryClosed = false;
				return true;
			}
			inventoryClosed = true;
		}
		return false;
	}

	private void updateInventory()
	{
		if (inventory.Count() != 0)
		{
			if (inventoryClosed & (inventoryFrameCount != 0))
			{
				inventoryFrameCount--;
			}
			else if (!inventoryClosed & (inventoryFrameCount != 12))
			{
				inventoryFrameCount++;
			}
			if (inventoryFrameCount < 0)
			{
				inventoryFrameCount = 0;
			}
			else if (inventoryFrameCount > 12)
			{
				inventoryFrameCount = 12;
			}
			if (!inventoryClosed & (inventoryFrameCount == 12) & (inventoryDisplayCounter != 100))
			{
				inventoryDisplayCounter++;
			}
			if (inventoryClosed & (inventoryDisplayCounter != 0))
			{
				inventoryDisplayCounter = 0;
			}
		}
	}

	public void updateCursor(GamePadControl gamePad, GameTime gameTime, SaveData saveData, bool inverseY)
	{
		if (isWheelSpin)
		{
			elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
			float num = MathHelper.Clamp(elapsedTime / (float)Duration, 0f, 1f);
			rotationIn = MathHelper.SmoothStep(rotationInCurrent, rotationInTarget, num);
			rotationOut = MathHelper.SmoothStep(rotationOutCurrent, rotationOutTarget, num);
			if (num != 1f)
			{
				return;
			}
			isWheelSpin = false;
			elapsedTime = 0f;
			rotationInCurrent = MathHelper.WrapAngle(rotationIn);
			rotationOutCurrent = MathHelper.WrapAngle(rotationOut);
			tempI = Math.Abs(rotationInCurrent);
			tempO = Math.Abs(rotationOutCurrent);
			SFXName = "Game Over 01";
			playSFX = true;
			if (tempI > tempO)
			{
				if ((double)(tempI - tempO) <= 0.04)
				{
					spinWheelSuccess = true;
					playSFX = false;
				}
			}
			else if ((double)(tempO - tempI) <= 0.04)
			{
				spinWheelSuccess = true;
				playSFX = false;
			}
		}
		else if (displayCursor)
		{
			updateButterFly();
			updatePiece9();
			updatePiece5();
			updateInventory();
			i = 6;
			if (cursorLocation.X < 50f)
			{
				cursorLocation.X = 50f;
			}
			else if (cursorLocation.X > 1220f)
			{
				cursorLocation.X = 1220f;
			}
			if (cursorLocation.Y < 40f)
			{
				cursorLocation.Y = 40f;
			}
			else if (cursorLocation.Y > 660f)
			{
				cursorLocation.Y = 660f;
			}
			if (currentScene != "Path20")
			{
				if (gamePad.joyBothVector.X < -0.1f)
				{
					cursorLocation.X -= (float)i * Math.Abs(gamePad.joyBothVector.X);
				}
				else if (gamePad.joyBothVector.X > 0.1f)
				{
					cursorLocation.X += (float)i * Math.Abs(gamePad.joyBothVector.X);
				}
				if (gamePad.joyBothVector.Y < -0.1f)
				{
					if (inverseY)
					{
						cursorLocation.Y -= (float)i * Math.Abs(gamePad.joyBothVector.Y);
					}
					else
					{
						cursorLocation.Y += (float)i * Math.Abs(gamePad.joyBothVector.Y);
					}
				}
				else if (gamePad.joyBothVector.Y > 0.1f)
				{
					if (inverseY)
					{
						cursorLocation.Y += (float)i * Math.Abs(gamePad.joyBothVector.Y);
					}
					else
					{
						cursorLocation.Y -= (float)i * Math.Abs(gamePad.joyBothVector.Y);
					}
				}
			}
			else
			{
				if (gamePad.joyRightVector.X < -0.1f)
				{
					cursorLocation.X -= (float)i * Math.Abs(gamePad.joyRightVector.X);
				}
				else if (gamePad.joyRightVector.X > 0.1f)
				{
					cursorLocation.X += (float)i * Math.Abs(gamePad.joyRightVector.X);
				}
				if (gamePad.joyRightVector.Y < -0.1f)
				{
					if (inverseY)
					{
						cursorLocation.Y -= (float)i * Math.Abs(gamePad.joyRightVector.Y);
					}
					else
					{
						cursorLocation.Y += (float)i * Math.Abs(gamePad.joyRightVector.Y);
					}
				}
				else if (gamePad.joyRightVector.Y > 0.1f)
				{
					if (inverseY)
					{
						cursorLocation.Y += (float)i * Math.Abs(gamePad.joyRightVector.Y);
					}
					else
					{
						cursorLocation.Y -= (float)i * Math.Abs(gamePad.joyRightVector.Y);
					}
				}
			}
			if (currentScene == "Path20")
			{
				if (gamePad.joyLeftDirection == GamePadControl.direction.E)
				{
					rotationOut -= Math.Abs(gamePad.joyLeftDirectionAmount) * 0.025f;
				}
				else if (gamePad.joyLeftDirection == GamePadControl.direction.W)
				{
					rotationOut += Math.Abs(gamePad.joyLeftDirectionAmount) * 0.025f;
				}
			}
			checkInTarget();
			updatePuzzleGame();
		}
		else if (turnOnMap)
		{
			i = 6;
			oldDirection = cursorLocation;
			if (gamePad.joyBothVector.X < -0.1f)
			{
				cursorLocation.X -= (float)i * Math.Abs(gamePad.joyBothVector.X);
			}
			else if (gamePad.joyBothVector.X > 0.1f)
			{
				cursorLocation.X += (float)i * Math.Abs(gamePad.joyBothVector.X);
			}
			if (gamePad.joyBothVector.Y < -0.1f)
			{
				if (inverseY)
				{
					cursorLocation.Y -= (float)i * Math.Abs(gamePad.joyBothVector.Y);
				}
				else
				{
					cursorLocation.Y += (float)i * Math.Abs(gamePad.joyBothVector.Y);
				}
			}
			else if (gamePad.joyBothVector.Y > 0.1f)
			{
				if (inverseY)
				{
					cursorLocation.Y += (float)i * Math.Abs(gamePad.joyBothVector.Y);
				}
				else
				{
					cursorLocation.Y -= (float)i * Math.Abs(gamePad.joyBothVector.Y);
				}
			}
			checkForSelectedPiece(saveData);
			mapSelectionPause++;
			if (((cursorLocation != oldDirection) & !mapPieceSelected) && mapSelectionPause > 10)
			{
				checkForNewPiece(oldDirection, cursorLocation, saveData);
			}
		}
		else if (isCheckingPuzzle)
		{
			updatePuzzleGame();
		}
	}

	public bool puzzleGameToggle()
	{
		if (puzzleECorrect != -1)
		{
			return true;
		}
		if (!isCheckingPuzzle)
		{
			if (currentPuzzleState == puzzleGameState.A)
			{
				puzzleColorSelected(ref puzzleAColor);
			}
			else if (currentPuzzleState == puzzleGameState.B)
			{
				puzzleColorSelected(ref puzzleBColor);
			}
			else if (currentPuzzleState == puzzleGameState.C)
			{
				puzzleColorSelected(ref puzzleCColor);
			}
			else if (currentPuzzleState == puzzleGameState.D)
			{
				puzzleColorSelected(ref puzzleDColor);
			}
			else if (currentPuzzleState == puzzleGameState.E)
			{
				puzzleColorSelected(ref puzzleEColor);
			}
			if ((currentActionData.id == "Next") & puzzleNextChecker())
			{
				incrementCurrentPuzzle(down: true);
			}
			else if ((currentActionData.id == "Back") & puzzleBackChecker())
			{
				incrementCurrentPuzzle(down: false);
			}
			else if ((currentActionData.id == "Done") & (puzzleEColor != puzzleSelectedColor.N))
			{
				isCheckingPuzzle = true;
				deactiveCursor();
				currentPuzzleState = puzzleGameState.A;
				puzzleTimer = 0;
				puzzleTimerLoop = 0;
				correctAnswers = 0;
				playSFX = true;
				SFXName = "UI_Misc15";
			}
		}
		return false;
	}

	private bool puzzleNextChecker()
	{
		if ((currentPuzzleState == puzzleGameState.A) & (puzzleAColor != puzzleSelectedColor.N))
		{
			return true;
		}
		if ((currentPuzzleState == puzzleGameState.B) & (puzzleBColor != puzzleSelectedColor.N))
		{
			return true;
		}
		if ((currentPuzzleState == puzzleGameState.C) & (puzzleCColor != puzzleSelectedColor.N))
		{
			return true;
		}
		if ((currentPuzzleState == puzzleGameState.D) & (puzzleDColor != puzzleSelectedColor.N))
		{
			return true;
		}
		if ((currentPuzzleState == puzzleGameState.E) & (puzzleDColor != puzzleSelectedColor.N))
		{
			return true;
		}
		return false;
	}

	private bool puzzleBackChecker()
	{
		if (currentPuzzleState == puzzleGameState.A)
		{
			return false;
		}
		if (currentPuzzleState == puzzleGameState.B)
		{
			return true;
		}
		if (currentPuzzleState == puzzleGameState.C)
		{
			return true;
		}
		if (currentPuzzleState == puzzleGameState.D)
		{
			return true;
		}
		if (currentPuzzleState == puzzleGameState.E)
		{
			return true;
		}
		return false;
	}

	private bool puzzleColorSelected(ref puzzleSelectedColor currentPuzzleColor)
	{
		if (currentActionData.id == "Green")
		{
			currentPuzzleColor = puzzleSelectedColor.G;
			return true;
		}
		if (currentActionData.id == "Red")
		{
			currentPuzzleColor = puzzleSelectedColor.R;
			return true;
		}
		return false;
	}

	private void incrementCurrentPuzzle(bool down)
	{
		if (down)
		{
			if (currentPuzzleState == puzzleGameState.A)
			{
				currentPuzzleState = puzzleGameState.B;
			}
			else if (currentPuzzleState == puzzleGameState.B)
			{
				currentPuzzleState = puzzleGameState.C;
			}
			else if (currentPuzzleState == puzzleGameState.C)
			{
				currentPuzzleState = puzzleGameState.D;
			}
			else if (currentPuzzleState == puzzleGameState.D)
			{
				currentPuzzleState = puzzleGameState.E;
			}
			else if (currentPuzzleState == puzzleGameState.E)
			{
				currentPuzzleState = puzzleGameState.E;
			}
		}
		else if (currentPuzzleState == puzzleGameState.E)
		{
			currentPuzzleState = puzzleGameState.D;
		}
		else if (currentPuzzleState == puzzleGameState.D)
		{
			currentPuzzleState = puzzleGameState.C;
		}
		else if (currentPuzzleState == puzzleGameState.C)
		{
			currentPuzzleState = puzzleGameState.B;
		}
		else if (currentPuzzleState == puzzleGameState.B)
		{
			currentPuzzleState = puzzleGameState.A;
		}
		else if (currentPuzzleState == puzzleGameState.A)
		{
			currentPuzzleState = puzzleGameState.A;
		}
	}

	private void updatePuzzleGame()
	{
		if (isPuzzleGame & !isCheckingPuzzle)
		{
			puzzleTimer++;
			if (currentPuzzleState == puzzleGameState.A)
			{
				if ((puzzleACount == 0) & (puzzleTimer > 100))
				{
					puzzleACount = 1;
					puzzleTimer = 0;
				}
				else if ((puzzleACount != 0) & (puzzleACount < 37) & (puzzleTimer > 2))
				{
					puzzleACount++;
					puzzleTimer = 0;
				}
				else if (puzzleACount > 37)
				{
					puzzleACount = 37;
				}
			}
			else if (currentPuzzleState == puzzleGameState.B)
			{
				if ((puzzleBCount < 19) & (puzzleTimer > 2))
				{
					puzzleBCount++;
					puzzleTimer = 0;
				}
				else if (puzzleBCount > 19)
				{
					puzzleBCount = 19;
				}
			}
			else if (currentPuzzleState == puzzleGameState.C)
			{
				if ((puzzleCCount < 19) & (puzzleTimer > 2))
				{
					puzzleCCount++;
					puzzleTimer = 0;
				}
				else if (puzzleCCount > 19)
				{
					puzzleCCount = 19;
				}
			}
			else if (currentPuzzleState == puzzleGameState.D)
			{
				if ((puzzleDCount < 19) & (puzzleTimer > 2))
				{
					puzzleDCount++;
					puzzleTimer = 0;
				}
				else if (puzzleDCount > 19)
				{
					puzzleDCount = 19;
				}
			}
			else if (currentPuzzleState == puzzleGameState.E)
			{
				if ((puzzleECount < 19) & (puzzleTimer > 2))
				{
					puzzleECount++;
					puzzleTimer = 0;
				}
				else if (puzzleECount > 19)
				{
					puzzleECount = 19;
				}
			}
		}
		else
		{
			if (!isCheckingPuzzle || puzzleECorrect != -1)
			{
				return;
			}
			puzzleTimer++;
			c += 2;
			highLightColor = new Color(c, c, c, c);
			if (puzzleTimer <= 50)
			{
				return;
			}
			playSFX = true;
			SFXName = "UI_Misc15";
			puzzleTimer = 0;
			puzzleTimerLoop++;
			c = 0;
			highLightColor = new Color(0, 0, 0, 0);
			if (puzzleTimerLoop > 3)
			{
				puzzleTimerLoop = 0;
				checkCurrentPuzzle();
				if (currentPuzzleState == puzzleGameState.E)
				{
					int num = 5 - correctAnswers;
					textPosition = new Vector2(350f, 550f);
					textMessage = "Correct = " + correctAnswers + " / Wrong = " + num + " (Press (A) to continue)";
				}
				incrementCurrentPuzzle(down: true);
			}
		}
	}

	private void checkCurrentPuzzle()
	{
		SFXName = "Game Over 01";
		if (currentPuzzleState == puzzleGameState.A)
		{
			if (puzzleAColor == puzzleSelectedColor.G)
			{
				SFXName = "GuitatHit01reverb";
				correctAnswers++;
				puzzleACorrect = 1;
			}
			else
			{
				puzzleACorrect = 0;
			}
		}
		else if (currentPuzzleState == puzzleGameState.B)
		{
			if (puzzleBColor == puzzleSelectedColor.G)
			{
				puzzleBCorrect = 0;
				return;
			}
			SFXName = "GuitatHit01reverb";
			puzzleBCorrect = 1;
			correctAnswers++;
		}
		else if (currentPuzzleState == puzzleGameState.C)
		{
			if (puzzleCColor == puzzleSelectedColor.G)
			{
				puzzleCCorrect = 0;
				return;
			}
			SFXName = "GuitatHit01reverb";
			puzzleCCorrect = 1;
			correctAnswers++;
		}
		else if (currentPuzzleState == puzzleGameState.D)
		{
			if (puzzleDColor == puzzleSelectedColor.G)
			{
				puzzleDCorrect = 0;
				return;
			}
			SFXName = "GuitatHit01reverb";
			puzzleDCorrect = 1;
			correctAnswers++;
		}
		else if (currentPuzzleState == puzzleGameState.E)
		{
			if (puzzleEColor == puzzleSelectedColor.G)
			{
				puzzleECorrect = 0;
				return;
			}
			SFXName = "GuitatHit01reverb";
			correctAnswers++;
			puzzleECorrect = 1;
		}
	}

	private void drawPuzzleGame(SpriteBatch spriteBatch)
	{
		if (!isPuzzleGame || puzzleACount == 0)
		{
			return;
		}
		spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(350f, 120f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "PatternA", puzzleACount), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
		if (puzzleACount == 37)
		{
			if (!isCheckingPuzzle)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(990f, 120f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "GreenButton", 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(1000f, 350f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "RedButton", 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
			}
			if (puzzleAColor == puzzleSelectedColor.G)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(867f, 137f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "greenSelected", 0), Color.White, 0f, new Vector2(30f, 26f), 1f, SpriteEffects.None, 0.981f);
			}
			else if (puzzleAColor == puzzleSelectedColor.R)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(867f, 137f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "redSelected", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.981f);
			}
			if (currentPuzzleState == puzzleGameState.A)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "Checking", 0), highLightColor, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
			}
		}
		if (puzzleBCount != 0)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(350f, 120 + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "PatternB", puzzleBCount), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
			if (puzzleBColor == puzzleSelectedColor.G)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(867f, 137 + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "greenSelected", 0), Color.White, 0f, new Vector2(30f, 26f), 1f, SpriteEffects.None, 0.981f);
			}
			else if (puzzleBColor == puzzleSelectedColor.R)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(867f, 137 + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "redSelected", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.981f);
			}
			if (currentPuzzleState == puzzleGameState.B)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122 + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "Checking", 0), highLightColor, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
			}
		}
		if (puzzleCCount != 0)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(350f, 120 + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "PatternC", puzzleCCount), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
			if (puzzleCColor == puzzleSelectedColor.G)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(867f, 137 + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "greenSelected", 0), Color.White, 0f, new Vector2(30f, 26f), 1f, SpriteEffects.None, 0.981f);
			}
			else if (puzzleCColor == puzzleSelectedColor.R)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(867f, 137 + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "redSelected", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.981f);
			}
			if (currentPuzzleState == puzzleGameState.C)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122 + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "Checking", 0), highLightColor, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
			}
		}
		if (puzzleDCount != 0)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(350f, 120 + increment + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "PatternD", puzzleDCount), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
			if (puzzleDColor == puzzleSelectedColor.G)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(867f, 137 + increment + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "greenSelected", 0), Color.White, 0f, new Vector2(30f, 26f), 1f, SpriteEffects.None, 0.981f);
			}
			else if (puzzleDColor == puzzleSelectedColor.R)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(867f, 137 + increment + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "redSelected", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.981f);
			}
			if (currentPuzzleState == puzzleGameState.D)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122 + increment + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "Checking", 0), highLightColor, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
			}
		}
		if (puzzleECount != 0)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(350f, 120 + increment + increment + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "PatternE", puzzleECount), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
			if (puzzleEColor == puzzleSelectedColor.G)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(867f, 137 + increment + increment + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "greenSelected", 0), Color.White, 0f, new Vector2(30f, 26f), 1f, SpriteEffects.None, 0.981f);
			}
			else if (puzzleEColor == puzzleSelectedColor.R)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(867f, 137 + increment + increment + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "redSelected", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.981f);
			}
			if (currentPuzzleState == puzzleGameState.E)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122 + increment + increment + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "Checking", 0), highLightColor, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
			}
		}
		if (!isCheckingPuzzle)
		{
			if ((currentPuzzleState == puzzleGameState.A) & (puzzleAColor != puzzleSelectedColor.N))
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(900f, 550f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "Next", 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
			}
			else if ((currentPuzzleState == puzzleGameState.B) & (puzzleBColor != puzzleSelectedColor.N))
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(900f, 550f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "Next", 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
			}
			else if ((currentPuzzleState == puzzleGameState.C) & (puzzleCColor != puzzleSelectedColor.N))
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(900f, 550f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "Next", 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
			}
			else if ((currentPuzzleState == puzzleGameState.D) & (puzzleDColor != puzzleSelectedColor.N))
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(900f, 550f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "Next", 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
			}
			if (currentPuzzleState != puzzleGameState.A)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(330f, 550f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "Back", 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
			}
			if (puzzleEColor != puzzleSelectedColor.N)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(610f, 550f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "Done", 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.98f);
			}
			return;
		}
		if (puzzleACorrect == 0)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "wrong", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
		}
		else if (puzzleACorrect == 1)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "correct", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
		}
		if (puzzleBCorrect == 0)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122 + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "wrong", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
		}
		else if (puzzleBCorrect == 1)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122 + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "correct", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
		}
		if (puzzleCCorrect == 0)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122 + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "wrong", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
		}
		else if (puzzleCCorrect == 1)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122 + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "correct", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
		}
		if (puzzleDCorrect == 0)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122 + increment + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "wrong", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
		}
		else if (puzzleDCorrect == 1)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122 + increment + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "correct", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
		}
		if (puzzleECorrect == 0)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122 + increment + increment + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "wrong", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
		}
		else if (puzzleECorrect == 1)
		{
			spriteBatch.Draw(myCoreDisplayElements.getTexture("patternPuzzle"), new Vector2(347f, 122 + increment + increment + increment + increment), myCoreDisplayElements.spriteRDM.getSpriteRectangle("patternPuzzle", "correct", 0), Color.White, 0f, new Vector2(31f, 26f), 1f, SpriteEffects.None, 0.979f);
		}
	}

	private void checkForNewPiece(Vector2 o, Vector2 n, SaveData saveData)
	{
		xDiff = n.X - o.X;
		yDiff = o.Y - n.Y;
		if (((double)xDiff < 0.8) & (xDiff > -0.8f))
		{
			xDiff = 0f;
		}
		if (((double)yDiff < 0.8) & (yDiff > -0.8f))
		{
			yDiff = 0f;
		}
		cursorLocation = o;
		Console.WriteLine("Distanance x = " + xDiff + " y = " + yDiff);
		if ((xDiff < 0f) & (yDiff == 0f))
		{
			findNextPiece(left: true, right: false, up: false, down: false, saveData);
		}
		else if ((xDiff > 0f) & (yDiff == 0f))
		{
			findNextPiece(left: false, right: true, up: false, down: false, saveData);
		}
		else if ((xDiff == 0f) & (yDiff > 0f))
		{
			findNextPiece(left: false, right: false, up: true, down: false, saveData);
		}
		else if ((xDiff == 0f) & (yDiff < 0f))
		{
			findNextPiece(left: false, right: false, up: false, down: true, saveData);
		}
		else if ((xDiff < 0f) & (yDiff < 0f))
		{
			findNextPiece(left: true, right: false, up: false, down: true, saveData);
		}
		else if ((xDiff < 0f) & (yDiff > 0f))
		{
			findNextPiece(left: true, right: false, up: true, down: false, saveData);
		}
		else if ((xDiff < 0f) & (yDiff > 0f))
		{
			findNextPiece(left: false, right: true, up: true, down: false, saveData);
		}
		else if ((xDiff < 0f) & (yDiff > 0f))
		{
			findNextPiece(left: false, right: true, up: false, down: true, saveData);
		}
	}

	private void findNextPiece(bool left, bool right, bool up, bool down, SaveData saveData)
	{
		distance = 1000f;
		matchingName = "";
		foreach (SaveData.mapSaveData currentMapSaveDatum in saveData.currentMapSaveData)
		{
			if (currentMapSaveDatum.isOver)
			{
				continue;
			}
			matchFound = true;
			if ((left & matchFound) && cursorLocation.X < currentMapSaveDatum.currentPosition.X)
			{
				matchFound = false;
			}
			if ((right & matchFound) && cursorLocation.X > currentMapSaveDatum.currentPosition.X)
			{
				matchFound = false;
			}
			if ((up & matchFound) && cursorLocation.Y < currentMapSaveDatum.currentPosition.Y)
			{
				matchFound = false;
			}
			if ((down & matchFound) && cursorLocation.Y > currentMapSaveDatum.currentPosition.Y)
			{
				matchFound = false;
			}
			if (matchFound)
			{
				tempDistance = Vector2.Distance(cursorLocation, currentMapSaveDatum.currentPosition);
				if (tempDistance < distance)
				{
					matchingName = currentMapSaveDatum.mapPieceName;
					distance = tempDistance;
				}
			}
		}
		if (matchingName != "")
		{
			mapSelectionPause = 0;
			getNewMapItem(matchingName, saveData);
		}
	}

	private void getNewMapItem(string matchingName, SaveData saveData)
	{
		foreach (SaveData.mapSaveData currentMapSaveDatum in saveData.currentMapSaveData)
		{
			if (currentMapSaveDatum.mapPieceName == matchingName)
			{
				currentMapSaveDatum.isOver = true;
				cursorLocation = currentMapSaveDatum.currentPosition;
			}
			else if (currentMapSaveDatum.isOver)
			{
				currentMapSaveDatum.isOver = false;
			}
		}
	}

	private void checkForSelectedPiece(SaveData saveData)
	{
		bool flag = false;
		foreach (SaveData.mapSaveData currentMapSaveDatum in saveData.currentMapSaveData)
		{
			if (currentMapSaveDatum.isOver)
			{
				if (!currentMapSaveDatum.isCorrect)
				{
					flag = true;
					break;
				}
				currentMapSaveDatum.isOver = false;
			}
		}
		if (flag)
		{
			return;
		}
		foreach (SaveData.mapSaveData currentMapSaveDatum2 in saveData.currentMapSaveData)
		{
			if (!currentMapSaveDatum2.isCorrect)
			{
				currentMapSaveDatum2.isOver = true;
				cursorLocation = currentMapSaveDatum2.currentPosition;
				break;
			}
		}
	}

	private void adjScale()
	{
		if ((displayType == displayTypeEnum.normal) | (displayType == displayTypeEnum.overAction) | (displayType == displayTypeEnum.inventorySelect))
		{
			if ((cursorScale < 0.8f) & (arrowScale == 0f))
			{
				cursorScale += 0.15f;
			}
			else
			{
				arrowScale -= 0.15f;
			}
			if (cursorScale > 0.8f)
			{
				cursorScale = 0.8f;
			}
			if (arrowScale < 0f)
			{
				arrowScale = 0f;
			}
			return;
		}
		if (cursorScale != 0f)
		{
			cursorScale -= 0.15f;
		}
		else if (arrowScale < 1f)
		{
			arrowScale += 0.15f;
		}
		if (cursorScale <= 0f)
		{
			cursorScale = 0f;
		}
		if (arrowScale > 1f)
		{
			arrowScale = 1f;
		}
	}

	public bool specialCheck()
	{
		if (currentScene == "Path20")
		{
			if (currentActionData.id == "UnscrewCover")
			{
				if (getCurrentInventoryItem() == "screwDriver")
				{
					return true;
				}
				if (getCurrentInventoryItem() == "NA")
				{
					textPosition = new Vector2(120f, 500f);
					textMessage = "If I had the right tool, I could open this lid.";
					return false;
				}
				if (getCurrentInventoryItem() == "shovel")
				{
					textPosition = new Vector2(120f, 500f);
					textMessage = "Ahhh...no...bashing it with the shovel won't open the lid.";
					return false;
				}
				textPosition = new Vector2(120f, 500f);
				textMessage = "If I had the right tool, I could open this lid.";
				return false;
			}
		}
		else if (currentScene == "Path23")
		{
			if (currentActionData.id == "Top")
			{
				if (turnOnTopWheelControl)
				{
					topCounterClockwise = true;
					turnOnTopWheelControl = false;
				}
				else
				{
					topCounterClockwise = false;
					turnOnTopWheelControl = true;
				}
				return false;
			}
			if (currentActionData.id == "Bottom")
			{
				if (turnOnBottomWheelControl)
				{
					bottomCounterClockwise = false;
					turnOnBottomWheelControl = false;
				}
				else
				{
					bottomCounterClockwise = true;
					turnOnBottomWheelControl = true;
				}
				return false;
			}
		}
		else if (currentScene == "Path3")
		{
			if (currentActionData.id == "DesertNeedMap")
			{
				if (getCurrentInventoryItem() == "MapComplete")
				{
					return true;
				}
				if (getCurrentInventoryItem() == "screwDriver")
				{
					textPosition = new Vector2(120f, 500f);
					textMessage = "Not sure how I could use the screwdriver to get the map.";
					return false;
				}
				textPosition = new Vector2(120f, 500f);
				textMessage = "No way.  I am sticking to the path.";
				return false;
			}
		}
		else if (currentScene == "Path26")
		{
			if (currentActionData.id == "Piece9")
			{
				if (getCurrentInventoryItem() == "shovel")
				{
					return true;
				}
				if (getCurrentInventoryItem() == "screwDriver")
				{
					textPosition = new Vector2(120f, 500f);
					textMessage = "Not sure how I could use the screwdriver to get the map.";
					return false;
				}
				textPosition = new Vector2(120f, 500f);
				textMessage = "I cannot reach it and I'm not going to go for a swim.";
				return false;
			}
		}
		else if (currentScene == "Path3")
		{
			if (currentActionData.id == "DoorA")
			{
				if (getCurrentInventoryItem() == "I_PitKeyA")
				{
					currentActionData.nextRefId = 4;
					return true;
				}
				currentActionData.nextRefId = -1;
			}
			else if (currentActionData.id == "DoorB")
			{
				if (getCurrentInventoryItem() == "I_PitKeyA")
				{
					currentActionData.nextRefId = 4;
					return true;
				}
				currentActionData.nextRefId = -1;
			}
		}
		return true;
	}

	public bool toggleMapSelection(SaveData saveData)
	{
		mapPieceCorrect = false;
		if (textMessage != "")
		{
			textMessage = "";
		}
		foreach (SaveData.mapSaveData currentMapSaveDatum in saveData.currentMapSaveData)
		{
			if (currentMapSaveDatum.isOver)
			{
				if (currentMapSaveDatum.isSelected & !currentMapSaveDatum.isCorrect)
				{
					currentMapSaveDatum.isSelected = false;
					mapPieceSelected = false;
					currentMapSaveDatum.currentPosition = cursorLocation;
					mapPieceCorrect = checkForMapMatch(currentMapSaveDatum);
					checkComplete(saveData);
				}
				else if (!currentMapSaveDatum.isCorrect)
				{
					currentMapSaveDatum.isSelected = true;
					cursorLocation = currentMapSaveDatum.currentPosition;
					mapPieceSelected = true;
				}
				break;
			}
		}
		return mapPieceCorrect;
	}

	private bool checkForMapMatch(SaveData.mapSaveData m)
	{
		bool result = false;
		if (m.mapPieceName == "Piece1")
		{
			tempDistance = Vector2.Distance(m.currentPosition, new Vector2(359f, 184f));
			if (tempDistance < 30f)
			{
				m.currentPosition = new Vector2(359f, 184f);
				m.isCorrect = true;
				m.isOver = false;
				m.isSelected = false;
				result = true;
			}
		}
		else if (m.mapPieceName == "Piece2")
		{
			tempDistance = Vector2.Distance(m.currentPosition, new Vector2(382f, 400f));
			if (tempDistance < 30f)
			{
				m.currentPosition = new Vector2(382f, 400f);
				m.isCorrect = true;
				m.isOver = false;
				m.isSelected = false;
				result = true;
			}
		}
		else if (m.mapPieceName == "Piece3")
		{
			tempDistance = Vector2.Distance(m.currentPosition, new Vector2(605f, 432f));
			if (tempDistance < 30f)
			{
				m.currentPosition = new Vector2(605f, 432f);
				m.isCorrect = true;
				m.isOver = false;
				m.isSelected = false;
				result = true;
			}
		}
		else if (m.mapPieceName == "Piece4")
		{
			tempDistance = Vector2.Distance(m.currentPosition, new Vector2(550f, 282f));
			if (tempDistance < 30f)
			{
				m.currentPosition = new Vector2(550f, 282f);
				m.isCorrect = true;
				m.isOver = false;
				m.isSelected = false;
				result = true;
			}
		}
		else if (m.mapPieceName == "Piece5")
		{
			tempDistance = Vector2.Distance(m.currentPosition, new Vector2(523f, 170f));
			if (tempDistance < 30f)
			{
				m.currentPosition = new Vector2(523f, 170f);
				m.isCorrect = true;
				m.isOver = false;
				m.isSelected = false;
				result = true;
			}
		}
		else if (m.mapPieceName == "Piece6")
		{
			tempDistance = Vector2.Distance(m.currentPosition, new Vector2(730f, 234f));
			if (tempDistance < 30f)
			{
				m.currentPosition = new Vector2(730f, 234f);
				m.isCorrect = true;
				m.isOver = false;
				m.isSelected = false;
				result = true;
			}
		}
		else if (m.mapPieceName == "Piece7")
		{
			tempDistance = Vector2.Distance(m.currentPosition, new Vector2(802f, 418f));
			if (tempDistance < 30f)
			{
				m.currentPosition = new Vector2(802f, 418f);
				m.isCorrect = true;
				m.isOver = false;
				m.isSelected = false;
				result = true;
			}
		}
		else if (m.mapPieceName == "Piece8")
		{
			tempDistance = Vector2.Distance(m.currentPosition, new Vector2(917f, 404f));
			if (tempDistance < 30f)
			{
				m.currentPosition = new Vector2(917f, 404f);
				m.isCorrect = true;
				m.isOver = false;
				m.isSelected = false;
				result = true;
			}
		}
		else if (m.mapPieceName == "Piece9")
		{
			tempDistance = Vector2.Distance(m.currentPosition, new Vector2(910f, 197f));
			if (tempDistance < 30f)
			{
				m.currentPosition = new Vector2(910f, 197f);
				m.isCorrect = true;
				m.isOver = false;
				m.isSelected = false;
				result = true;
			}
		}
		return result;
	}

	private void checkComplete(SaveData saveData)
	{
		mapIsComplete = false;
		int num = 0;
		int num2 = 0;
		foreach (SaveData.mapSaveData currentMapSaveDatum in saveData.currentMapSaveData)
		{
			if (currentMapSaveDatum.isCorrect)
			{
				num++;
			}
			if (currentMapSaveDatum.isOver)
			{
				num2 = 1;
			}
		}
		if (num == 9)
		{
			mapIsComplete = true;
			mapCompleteDisplayed = false;
			currentActionData.id = "MapComplete";
		}
		else
		{
			if (num2 != 0)
			{
				return;
			}
			foreach (SaveData.mapSaveData currentMapSaveDatum2 in saveData.currentMapSaveData)
			{
				if (!currentMapSaveDatum2.isCorrect)
				{
					currentMapSaveDatum2.isOver = true;
					break;
				}
			}
		}
	}

	public void drawCursor(SpriteBatch spriteBatch, SaveData saveData)
	{
		if (displayCursor)
		{
			spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, textMessage, textPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9941f);
			spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, textMessage, new Vector2(textPosition.X + 1f, textPosition.Y + 1f), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.994f);
			if (displayType == displayTypeEnum.normal)
			{
				adjScale();
				if (arrowScale == 0f)
				{
					spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), cursorLocation, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "cursorNew", cursorA), Color.White, 0f, new Vector2(94.5f, 82f), cursorScale, SpriteEffects.None, 0.99f);
				}
				else
				{
					spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), cursorLocation, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", arrowImage, cursorA), Color.White, 0f, new Vector2(98f, 44f), arrowScale, SpriteEffects.None, 0.99f);
				}
			}
			else if ((displayType == displayTypeEnum.overAction) | (displayType == displayTypeEnum.inventorySelect))
			{
				adjScale();
				if (arrowScale == 0f)
				{
					spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), cursorLocation, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "cursorActiveNew", cursorA), Color.White, 0f, new Vector2(94.5f, 82f), cursorScale, SpriteEffects.None, 0.99f);
				}
				else
				{
					spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), cursorLocation, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", arrowImage, cursorA), Color.White, 0f, new Vector2(98f, 44f), arrowScale, SpriteEffects.None, 0.99f);
				}
			}
			else
			{
				adjScale();
				if (arrowScale == 0f)
				{
					spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), cursorLocation, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "cursorNew", cursorA), Color.White, 0f, new Vector2(94.5f, 82f), cursorScale, SpriteEffects.None, 0.99f);
				}
				else
				{
					spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), cursorLocation, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", arrowImage, cursorA), Color.White, 0f, new Vector2(98f, 44f), arrowScale, SpriteEffects.None, 0.99f);
				}
			}
			if (turnOnPiece9)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), piece9Location, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "Piece9Pool", 0), Color.White, 0f, new Vector2(98f, 44f), 1f, SpriteEffects.None, 0.97f);
			}
			else if (turnOnPiece7)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), piece7Location, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "Piece7Ground", 0), Color.White, 0f, new Vector2(98f, 44f), 1f, SpriteEffects.None, 0.97f);
			}
			else if (turnOnPiece1)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), new Vector2(997f, 595f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "Piece1Table", 0), Color.White, 0f, new Vector2(98f, 44f), 1f, SpriteEffects.None, 0.97f);
			}
			else if (turnOnPiece2)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), new Vector2(322f, 351f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "MoneyFlower", 0), Color.White, 0f, new Vector2(98f, 44f), 0.75f, SpriteEffects.None, 0.97f);
			}
			else if (turnOnPiece3)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), new Vector2(980f, 573f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "Piece3", 0), Color.White, 0f, new Vector2(0f, 0f), 0.1f, SpriteEffects.None, 0.97f);
			}
			else if (turnOnPiece5)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), piece5Location, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "Piece5", 0), Color.White, piece5Rotation, new Vector2(175f, 120f), 0.15f, SpriteEffects.None, 0.97f);
			}
			else if (turnOnPiece6)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), new Vector2(971f, 390f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "MoneyFlower", 0), Color.White, 0f, new Vector2(98f, 44f), 1f, SpriteEffects.None, 0.97f);
			}
			else if (turnOnPiece8)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), new Vector2(703f, 547f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "Piece8Ground", 0), Color.White, 0f, new Vector2(98f, 44f), 1f, SpriteEffects.None, 0.97f);
			}
			else if (turnOnWheel)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), new Vector2(640f, 360f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "OutsideWheel", 0), Color.White, rotationOut, new Vector2(298.5f, 300f), 1.24f, SpriteEffects.None, 0.97f);
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), new Vector2(640f, 360f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "InsideWheel", 0), Color.White, rotationIn, new Vector2(298.5f, 300f), 1.24f, SpriteEffects.None, 0.97f);
			}
			if (currentScene == "Path23")
			{
				if (turnOnBottomWheelControl)
				{
					spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), new Vector2(494f, 354f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "WireBoxCross", 0), Color.White, 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 0.97f);
				}
				if (turnOnTopWheelControl)
				{
					spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), new Vector2(494f, 161f), myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "WireBoxStraight", 0), Color.White, 0f, new Vector2(0f, 0f), 1f, SpriteEffects.None, 0.97f);
				}
			}
			timerCursor++;
			if (timerCursor > 2)
			{
				cursorA++;
				if (cursorA > 23)
				{
					cursorA = 0;
				}
				cursorB = cursorA;
				timerCursor = 0;
			}
			drawButterflyA(spriteBatch);
			drawInventory(spriteBatch);
			drawPuzzleGame(spriteBatch);
			return;
		}
		if (turnOnMap)
		{
			mapDepth = 0.95f;
			mapPieceSelected = false;
			spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, textMessage, textPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9941f);
			spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, textMessage, new Vector2(textPosition.X + 1f, textPosition.Y + 1f), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.994f);
			{
				foreach (SaveData.mapSaveData currentMapSaveDatum in saveData.currentMapSaveData)
				{
					if (!currentMapSaveDatum.isCorrect)
					{
						mapDepth += 0.001f;
						if (currentMapSaveDatum.isOver & !currentMapSaveDatum.isSelected)
						{
							spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), currentMapSaveDatum.currentPosition, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", currentMapSaveDatum.mapPieceName + "s", 0), Color.White, 0f, currentMapSaveDatum.origin, 0.5f, SpriteEffects.None, mapDepth);
						}
						else if (currentMapSaveDatum.isSelected)
						{
							mapPieceSelected = true;
							spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), cursorLocation, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", currentMapSaveDatum.mapPieceName + "s", 0), Color.White, 0f, currentMapSaveDatum.origin, 0.75f, SpriteEffects.None, 0.99f);
						}
						else
						{
							spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), currentMapSaveDatum.currentPosition, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", currentMapSaveDatum.mapPieceName, 0), Color.White, 0f, currentMapSaveDatum.origin, 0.5f, SpriteEffects.None, mapDepth + 0.01f);
						}
					}
					else
					{
						spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), currentMapSaveDatum.currentPosition, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", currentMapSaveDatum.mapPieceName, 0), Color.White, 0f, currentMapSaveDatum.origin, 0.75f, SpriteEffects.None, 0.94f);
					}
				}
				return;
			}
		}
		if (isCheckingPuzzle)
		{
			spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, textMessage, textPosition, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.9941f);
			spriteBatch.DrawString(myCoreDisplayElements.MainFontRegular, textMessage, new Vector2(textPosition.X + 1f, textPosition.Y + 1f), Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.994f);
			drawPuzzleGame(spriteBatch);
		}
	}

	private Vector2 updateButterFlyPath(Vector2 c, SpriteEffects s, int id)
	{
		if (c.X < 200f)
		{
			s = SpriteEffects.FlipHorizontally;
		}
		else if (c.X > 1000f)
		{
			s = SpriteEffects.None;
		}
		else if (getRandom(0, 100) > 98)
		{
			s = ((s == SpriteEffects.None) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
		}
		if (s == SpriteEffects.None)
		{
			c.X -= getRandom(0, 5);
			switch (id)
			{
			case 1:
				butterFlyAEffect = SpriteEffects.None;
				break;
			case 2:
				butterFlyBEffect = SpriteEffects.None;
				break;
			case 3:
				butterFlyCEffect = SpriteEffects.None;
				break;
			}
		}
		else
		{
			c.X += getRandom(0, 5);
			switch (id)
			{
			case 1:
				butterFlyAEffect = SpriteEffects.FlipHorizontally;
				break;
			case 2:
				butterFlyBEffect = SpriteEffects.FlipHorizontally;
				break;
			case 3:
				butterFlyCEffect = SpriteEffects.FlipHorizontally;
				break;
			}
		}
		if (c.Y < 300f)
		{
			c.Y += 2f;
		}
		else if (c.Y > 600f)
		{
			c.Y -= 2f;
		}
		else
		{
			c.Y += getRandom(0, 3) - 1;
		}
		return c;
	}

	public void updateButterFly()
	{
		if (!turnOnButterFly)
		{
			return;
		}
		timerButterfly++;
		if (timerButterfly > 2)
		{
			butteryFlyA++;
			butteryFlyB++;
			butteryFlyC++;
			if (butteryFlyA > 6)
			{
				butteryFlyA = 0;
			}
			if (butteryFlyB > 6)
			{
				butteryFlyB = 0;
			}
			if (butteryFlyC > 6)
			{
				butteryFlyC = 0;
			}
			timerButterfly = 0;
		}
		butteryFlyAPos = updateButterFlyPath(butteryFlyAPos, butterFlyAEffect, 1);
		butteryFlyBPos = updateButterFlyPath(butteryFlyBPos, butterFlyBEffect, 2);
		butteryFlyCPos = updateButterFlyPath(butteryFlyCPos, butterFlyCEffect, 3);
	}

	private string getDisplayBoxInventory(int caseId)
	{
		if (caseId == currentSelection)
		{
			return "InventoryBoxSelected";
		}
		return "InventoryBox";
	}

	public void drawInventory(SpriteBatch spriteBatch)
	{
		if (inventory.Count() == 0)
		{
			return;
		}
		spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), casePosition, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "case", inventoryFrameCount), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.982f);
		if (inventoryFrameCount == 12)
		{
			if ((inventoryDisplayCounter > 10) & case1On)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), casePosition1, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", getDisplayBoxInventory(1), 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.982f);
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), casePosition1a, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", case1, 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.983f);
			}
			if ((inventoryDisplayCounter > 20) & case2On)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), casePosition2, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", getDisplayBoxInventory(2), 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.982f);
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), casePosition2a, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", case2, 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.983f);
			}
			if ((inventoryDisplayCounter > 30) & case3On)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), casePosition3, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", getDisplayBoxInventory(3), 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.982f);
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), casePosition3a, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", case3, 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.983f);
			}
			if ((inventoryDisplayCounter > 40) & case4On)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), casePosition4, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", getDisplayBoxInventory(4), 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.982f);
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), casePosition4a, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", case4, 0), Color.White, 0f, new Vector2(32f, 25f), 1f, SpriteEffects.None, 0.983f);
			}
		}
	}

	public void drawButterflyA(SpriteBatch spriteBatch)
	{
		if (turnOnButterFly)
		{
			if (turnOnMoneyFly)
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), butteryFlyAPos, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "MoneyButterfly", butteryFlyA), Color.White, 0f, new Vector2(32f, 25f), 1f, butterFlyAEffect, 0.98f);
			}
			else
			{
				spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), butteryFlyAPos, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "Monarch", butteryFlyA), Color.White, 0f, new Vector2(32f, 25f), 1f, butterFlyAEffect, 0.98f);
			}
			spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), butteryFlyBPos, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "Monarch", butteryFlyB), Color.White, 0f, new Vector2(32f, 25f), 1f, butterFlyBEffect, 0.98f);
			spriteBatch.Draw(myCoreDisplayElements.getTexture("arrows"), butteryFlyCPos, myCoreDisplayElements.spriteRDM.getSpriteRectangle("arrows", "Monarch", butteryFlyC), Color.White, 0f, new Vector2(32f, 25f), 1f, butterFlyCEffect, 0.98f);
		}
	}
}
