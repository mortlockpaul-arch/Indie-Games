using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RuntimeXNA.Animations;
using RuntimeXNA.Application;
using RuntimeXNA.Banks;
using RuntimeXNA.Events;
using RuntimeXNA.Expressions;
using RuntimeXNA.Frame;
using RuntimeXNA.Movements;
using RuntimeXNA.OI;
using RuntimeXNA.Objects;
using RuntimeXNA.Params;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;
using RuntimeXNA.Values;

namespace RuntimeXNA.RunLoop;

public class CRun
{
	public const short GAMEFLAGS_VBLINDEP = 2;

	public const short GAMEFLAGS_LIMITEDSCROLL = 4;

	public const short GAMEFLAGS_FIRSTLOOPFADEIN = 16;

	public const short GAMEFLAGS_LOADONCALL = 32;

	public const short GAMEFLAGS_REALGAME = 64;

	public const short GAMEFLAGS_PLAY = 128;

	public const short GAMEFLAGS_INITIALISING = 512;

	public const short DLF_DONTUPDATE = 2;

	public const short DLF_DRAWOBJECTS = 4;

	public const short DLF_RESTARTLEVEL = 8;

	public const short DLF_DONTUPDATECOLMASK = 16;

	public const short DLF_COLMASKCLIPPED = 32;

	public const short DLF_SKIPLAYER0 = 64;

	public const short DLF_REDRAWLAYER = 128;

	public const short DLF_STARTLEVEL = 256;

	public const short GAME_XBORDER = 480;

	public const short GAME_YBORDER = 300;

	public const short COLMASK_XMARGIN = 64;

	public const short COLMASK_YMARGIN = 16;

	public const uint WRAP_X = 1u;

	public const uint WRAP_Y = 2u;

	public const uint WRAP_XY = 4u;

	public const int RH3SCROLLING_SCROLL = 1;

	public const int RH3SCROLLING_REDRAWLAYERS = 2;

	public const int RH3SCROLLING_REDRAWALL = 4;

	public const int RH3SCROLLING_REDRAWTOTALCOLMASK = 8;

	public const int OBSTACLE_NONE = 0;

	public const int OBSTACLE_SOLID = 1;

	public const int OBSTACLE_PLATFORM = 2;

	public const int OBSTACLE_LADDER = 3;

	public const int OBSTACLE_TRANSPARENT = 4;

	public const short COF_NOMOVEMENT = 1;

	public const short COF_HIDDEN = 2;

	public const short COF_FIRSTTEXT = 4;

	public const short MAX_FRAMERATE = 10;

	public const short LOOPEXIT_NEXTLEVEL = 1;

	public const short LOOPEXIT_PREVLEVEL = 2;

	public const short LOOPEXIT_GOTOLEVEL = 3;

	public const short LOOPEXIT_NEWGAME = 4;

	public const short LOOPEXIT_PAUSEGAME = 5;

	public const short LOOPEXIT_SAVEAPPLICATION = 6;

	public const short LOOPEXIT_LOADAPPLICATION = 7;

	public const short LOOPEXIT_SAVEFRAME = 8;

	public const short LOOPEXIT_LOADFRAME = 9;

	public const short LOOPEXIT_ENDGAME = -2;

	public const short LOOPEXIT_QUIT = 100;

	public const short LOOPEXIT_RESTART = 101;

	public const short LOOPEXIT_APPLETPAUSE = 102;

	public const short BORDER_LEFT = 1;

	public const short BORDER_RIGHT = 2;

	public const short BORDER_TOP = 4;

	public const short BORDER_BOTTOM = 8;

	public const short BORDER_ALL = 15;

	public const int MAX_INTERMEDIATERESULTS = 128;

	public byte[] plMasks = new byte[20]
	{
		0, 0, 0, 0, 255, 0, 0, 0, 255, 255,
		0, 0, 255, 255, 255, 0, 255, 255, 255, 255
	};

	private short[] Table_InOut = new short[16]
	{
		0, 1, 2, 0, 4, 5, 6, 0, 8, 9,
		10, 0, 0, 0, 0, 0
	};

	public static bool bMoveChanged;

	public CRunApp rhApp;

	public CRunFrame rhFrame;

	public int rhMaxOI;

	public byte rhStopFlag;

	public byte rhEvFlag;

	public int rhNPlayers;

	public byte rhMouseUsed;

	public short rhGameFlags;

	public byte[] rhPlayer = new byte[4];

	public short rhQuit;

	public short rhQuitBis;

	public int rhReturn;

	public int rhQuitParam;

	public int rhNObjects;

	public int rhMaxObjects;

	public CObjInfo[] rhOiList;

	public CEventProgram rhEvtProg;

	public int rhLevelSx;

	public int rhLevelSy;

	public int rhWindowX;

	public int rhWindowY;

	public int rhVBLDeltaOld;

	public int rhVBLObjet;

	public int rhVBLOld;

	public short rhMT_VBLStep;

	public short rhMT_VBLCount;

	public int rhMT_MoveStep;

	public int rhLoopCount;

	public long rhTimer;

	public long rhTimerOld;

	public long rhTimerFPSOld;

	public int rhTimerDelta;

	public int rhOiListPtr;

	public short rhObListNext;

	public short rhDestroyPos;

	public byte[] rh2OldPlayer = new byte[4];

	public byte[] rh2NewPlayer = new byte[4];

	public byte[] rh2InputMask = new byte[4];

	public byte rh2MouseKeys;

	public short rh2CreationCount;

	public int rh2MouseX;

	public int rh2MouseY;

	public int oldMouseKey;

	public int mouseKey;

	public int toucheID;

	public long mouseKeyTime;

	public int rh2MouseSaveX;

	public int rh2MouseSaveY;

	public int rh2PauseState;

	public int rh2PauseCompteur;

	public int rh2PauseTimer;

	public int rh2PauseFPSTimer;

	public int rh2PauseVbl;

	public int rh3DisplayX;

	public int rh3DisplayY;

	public int rh3WindowSx;

	public int rh3WindowSy;

	public short rh3CollisionCount;

	public byte rh3Scrolling;

	public int rh3Panic;

	public int rh3XMinimum;

	public int rh3YMinimum;

	public int rh3XMaximum;

	public int rh3YMaximum;

	public int rh3XMinimumKill;

	public int rh3YMinimumKill;

	public int rh3XMaximumKill;

	public int rh3YMaximumKill;

	public short rh3Graine;

	public Keys rh4PauseKey;

	public bool bCheckResume;

	public string rh4CurrentFastLoop;

	public int rh4EndOfPause;

	public short rh4MouseWheelDelta;

	public int rh4OnMouseWheel;

	public CArrayList rh4FastLoops;

	public CValue rh4ExpValue1;

	public CValue rh4ExpValue2;

	public int rh4KpxReturn;

	public int rh4ObjectCurCreate;

	public int rh4ObjectAddCreate;

	public short rh4FakeKey;

	public byte rh4DoUpdate;

	public bool rh4MenuEaten;

	public int rh4OnCloseCount;

	public bool rh4CursorShown;

	public short rh4ScrMode;

	public int rh4VBLDelta;

	public int rh4LoopTheoric;

	public int rh4EventCount;

	public CArrayList rh4BackDrawRoutines;

	public short rh4LastQuickDisplay;

	public short rh4FirstQuickDisplay;

	public int rh4WindowDeltaX;

	public int rh4WindowDeltaY;

	public long rh4TimeOut;

	public int rh4MouseXCenter;

	public int rh4MouseYCenter;

	public int rh4PosPile;

	public CValue[] rh4Results;

	public CExp[] rh4Operators;

	public CExp rh4OpeNull;

	public int rh4CurToken;

	public CExp[] rh4Tokens;

	public int[] rh4FrameRateArray = new int[10];

	public int rh4FrameRatePos;

	public int rh4FrameRatePrevious;

	public int[] rhDestroyList;

	public int rh4SaveFrame;

	public int rh4SaveFrameCount;

	public double rh4MvtTimerCoef;

	public CObject[] rhObjectList;

	public bool bOperande;

	public KeyboardState keyboardState;

	public byte rhJoystickMask;

	public short[] isColArray = new short[2];

	public MouseState mouseState;

	public bool bAnyKeyDown;

	public CQuestion questionObjectOn;

	public int nSubApps;

	public int nControls;

	public CArrayList controls;

	public IControl currentControl;

	public bool bMouseControlled;

	public int mouseX;

	public int mouseY;

	public PlayerIndex deviceSelectorPlayer;

	public CRun()
	{
	}

	public CRun(CRunApp app)
	{
		rhApp = app;
	}

	public void setFrame(CRunFrame f)
	{
		rhFrame = f;
	}

	public int allocRunHeader()
	{
		rhObjectList = new CObject[rhFrame.maxObjects];
		rhEvtProg = rhFrame.evtProg;
		rhMaxOI = 0;
		for (COI cOI = rhApp.OIList.getFirstOI(); cOI != null; cOI = rhApp.OIList.getNextOI())
		{
			if (cOI.oiType >= 2)
			{
				rhMaxOI++;
			}
		}
		rhOiList = new CObjInfo[rhMaxOI];
		for (int i = 0; i < rhMaxOI; i++)
		{
			rhOiList[i] = null;
		}
		if (rhFrame.m_wRandomSeed == -1)
		{
			Random random = new Random();
			rh3Graine = (short)random.Next(32000);
		}
		else
		{
			rh3Graine = rhFrame.m_wRandomSeed;
		}
		rhApp.spriteGen.setData(rhApp.imageBank, rhApp, rhFrame);
		rhDestroyList = new int[rhFrame.maxObjects / 32 + 1];
		rh4FastLoops = new CArrayList();
		rh4CurrentFastLoop = "";
		rhMaxObjects = rhFrame.maxObjects;
		rhNPlayers = rhEvtProg.nPlayers;
		rhWindowX = rhFrame.leX;
		rhWindowY = rhFrame.leY;
		rhLevelSx = rhFrame.leVirtualRect.right;
		if (rhLevelSx == -1)
		{
			rhLevelSx = 2147479552;
		}
		rhLevelSy = rhFrame.leVirtualRect.bottom;
		if (rhLevelSy == -1)
		{
			rhLevelSy = 2147479552;
		}
		rhNObjects = 0;
		rhStopFlag = 0;
		rhQuit = 0;
		rhQuitBis = 0;
		rhGameFlags &= 128;
		rhGameFlags |= 4;
		rh3Panic = 0;
		rh4FirstQuickDisplay = -1;
		rh4LastQuickDisplay = -1;
		rh4MouseXCenter = rhFrame.leEditWinWidth / 2;
		rh4MouseYCenter = rhFrame.leEditWinHeight / 2;
		rh4FrameRatePos = 0;
		rh4FrameRatePrevious = 0;
		rh4BackDrawRoutines = null;
		rh4SaveFrame = 0;
		rh4SaveFrameCount = -3;
		nSubApps = 0;
		rhGameFlags |= 64;
		rh4Results = new CValue[128];
		rh4Operators = new CExp[128];
		for (int i = 0; i < 128; i++)
		{
			rh4Results[i] = new CValue();
		}
		rh4OpeNull = new EXP_END();
		rh4OpeNull.code = 0;
		rhEvtProg.rh2CurrentClick = -1;
		nControls = 0;
		currentControl = null;
		controls = null;
		bMouseControlled = true;
		mouseKey = -1;
		bMouseControlled = false;
		rhFrame.rhOK = true;
		return 0;
	}

	public void freeRunHeader()
	{
		rhFrame.rhOK = false;
		rhObjectList = null;
		rhOiList = null;
		rhDestroyList = null;
		rh4CurrentFastLoop = null;
		rh4FastLoops = null;
		rh4BackDrawRoutines = null;
		for (int i = 0; i < 128; i++)
		{
			rh4Results[i] = null;
		}
		rh4OpeNull = null;
	}

	public int initRunLoop()
	{
		int num = 0;
		num = allocRunHeader();
		if (num != 0)
		{
			return num;
		}
		initAsmLoop();
		y_InitLevel();
		num = prepareFrame();
		if (num != 0)
		{
			return num;
		}
		num = createFrameObjects();
		if (num != 0)
		{
			return num;
		}
		redrawLevel(258);
		loadGlobalObjectsData();
		rhEvtProg.prepareProgram();
		rhEvtProg.assemblePrograms(this);
		captureMouse();
		rhQuitParam = 0;
		f_InitLoop();
		return 0;
	}

	private bool getPauseKeys()
	{
		bool flag = false;
		GamePadState[] array = new GamePadState[4];
		for (int i = 0; i < 4; i++)
		{
			switch (i)
			{
			case 0:
			{
				ref GamePadState reference4 = ref array[i];
				reference4 = GamePad.GetState(PlayerIndex.One);
				break;
			}
			case 1:
			{
				ref GamePadState reference3 = ref array[i];
				reference3 = GamePad.GetState(PlayerIndex.Two);
				break;
			}
			case 2:
			{
				ref GamePadState reference2 = ref array[i];
				reference2 = GamePad.GetState(PlayerIndex.Three);
				break;
			}
			case 3:
			{
				ref GamePadState reference = ref array[i];
				reference = GamePad.GetState(PlayerIndex.Four);
				break;
			}
			}
			for (int j = 0; j < 4; j++)
			{
				if (array[j].DPad.Left == ButtonState.Pressed)
				{
					flag = true;
					break;
				}
				if (array[j].DPad.Right == ButtonState.Pressed)
				{
					flag = true;
					break;
				}
				if (array[j].DPad.Up == ButtonState.Pressed)
				{
					flag = true;
					break;
				}
				if (array[j].DPad.Down == ButtonState.Pressed)
				{
					flag = true;
					break;
				}
				if ((double)array[j].ThumbSticks.Left.X < -0.5)
				{
					flag = true;
					break;
				}
				if ((double)array[j].ThumbSticks.Left.X > 0.5)
				{
					flag = true;
					break;
				}
				if ((double)array[j].ThumbSticks.Left.Y > 0.5)
				{
					flag = true;
					break;
				}
				if ((double)array[j].ThumbSticks.Left.Y < -0.5)
				{
					flag = true;
					break;
				}
				if (array[j].Buttons.A == ButtonState.Pressed)
				{
					flag = true;
					break;
				}
				if (array[j].Buttons.B == ButtonState.Pressed)
				{
					flag = true;
					break;
				}
				if (array[j].Buttons.X == ButtonState.Pressed)
				{
					flag = true;
					break;
				}
				if (array[j].Buttons.Y == ButtonState.Pressed)
				{
					flag = true;
					break;
				}
				if (array[j].Buttons.Start == ButtonState.Pressed)
				{
					flag = true;
					break;
				}
				if (array[j].Buttons.Back == ButtonState.Pressed)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				break;
			}
		}
		return flag;
	}

	public int doRunLoop()
	{
		rhApp.appRunFlags |= 4;
		int num = f_GameLoop();
		rhApp.appRunFlags &= -5;
		mouseKey = -1;
		mouseState = Mouse.GetState();
		if (mouseState.MiddleButton == ButtonState.Pressed)
		{
			mouseKey = 1;
		}
		if (mouseState.RightButton == ButtonState.Pressed)
		{
			mouseKey = 2;
		}
		if (mouseState.LeftButton == ButtonState.Pressed)
		{
			mouseKey = 0;
		}
		mouseX = mouseState.X;
		mouseY = mouseState.Y;
		getMouseCoords();
		if (mouseKey != oldMouseKey)
		{
			int nClicks = 1;
			if (mouseKey >= 0)
			{
				long num2 = rhApp.timer - mouseKeyTime;
				if (num2 < 500)
				{
					nClicks = 2;
					mouseKeyTime = 0L;
				}
				else
				{
					mouseKeyTime = rhApp.timer;
				}
				rhEvtProg.onMouseButton(mouseKey, nClicks);
				clickControls(nClicks);
			}
			oldMouseKey = mouseKey;
		}
		if ((rhEvtProg.bTestAllKeys || rh2PauseCompteur > 0) && rh2PauseCompteur > 0)
		{
			if (rh2PauseState == 0)
			{
				if (!getPauseKeys())
				{
					rh2PauseState = 1;
				}
			}
			else if (getPauseKeys())
			{
				resume();
				rh4EndOfPause = rhLoopCount;
				rhEvtProg.handle_GlobalEvents(-458755);
			}
		}
		if (num != 0)
		{
			switch (num)
			{
			case 101:
				if (!rhFrame.fade)
				{
					f_StopSamples();
					killFrameObjects();
					y_KillLevel(bLeaveSamples: false);
					rhEvtProg.unBranchPrograms();
					freeMouse();
					freeRunHeader();
					rhFrame.leX = (rhFrame.leLastScrlX = 0);
					rhFrame.leY = (rhFrame.leLastScrlY = 0);
					if (rhFrame.colMask != null)
					{
						rhFrame.colMask.setOrigin(0, 0);
					}
					allocRunHeader();
					initAsmLoop();
					y_InitLevel();
					redrawLevel(10);
					prepareFrame();
					createFrameObjects();
					loadGlobalObjectsData();
					rhEvtProg.prepareProgram();
					rhEvtProg.assemblePrograms(this);
					f_InitLoop();
					captureMouse();
					num = 0;
					rhQuitParam = 0;
				}
				break;
			case -2:
			case 100:
				rhEvtProg.handle_GlobalEvents(-196611);
				break;
			case 102:
				num = rhQuit;
				break;
			}
		}
		return num;
	}

	public int killRunLoop(int quit, bool bLeaveSamples)
	{
		if (quit > 100)
		{
			quit = -2;
		}
		int hi = rhQuitParam;
		saveGlobalObjectsData();
		killFrameObjects();
		y_KillLevel(bLeaveSamples);
		rhEvtProg.unBranchPrograms();
		freeRunHeader();
		return CServices.MAKELONG(quit, hi);
	}

	public void y_InitLevel()
	{
		resetFrameLayers(-1, bDeleteFrame: false);
	}

	public void initAsmLoop()
	{
		rhApp.spriteGen.winSetColMode(1);
		f_ObjMem_Init();
	}

	public void f_ObjMem_Init()
	{
		for (int i = 0; i < rhMaxObjects; i++)
		{
			rhObjectList[i] = null;
		}
	}

	public int prepareFrame()
	{
		if ((rhApp.gaFlags & 8) != 0 && !rhFrame.fade)
		{
			rhGameFlags |= 2;
		}
		else
		{
			rhGameFlags &= -3;
		}
		rhGameFlags |= 32;
		rhGameFlags |= 512;
		rh2CreationCount = 0;
		int num = 0;
		rhOiList = new CObjInfo[rhMaxOI];
		for (COI cOI = rhApp.OIList.getFirstOI(); cOI != null; cOI = rhApp.OIList.getNextOI())
		{
			short oiType = cOI.oiType;
			if (oiType >= 2)
			{
				rhOiList[num] = new CObjInfo();
				rhOiList[num].copyData(cOI);
				rhOiList[num].oilHFII = -1;
				if (oiType == 3 || oiType == 4)
				{
					for (CLO cLO = rhFrame.LOList.first_LevObj(); cLO != null; cLO = rhFrame.LOList.next_LevObj())
					{
						if (cLO.loOiHandle == rhOiList[num].oilOi)
						{
							rhOiList[num].oilHFII = cLO.loHandle;
							break;
						}
					}
				}
				num++;
				CObjectCommon cObjectCommon = (CObjectCommon)cOI.oiOC;
				if ((cObjectCommon.ocOEFlags & 0x10) != 0 && cObjectCommon.ocMovements != null)
				{
					for (short num2 = 0; num2 < cObjectCommon.ocMovements.nMovements; num2++)
					{
						CMoveDef cMoveDef = cObjectCommon.ocMovements.moveList[num2];
						if (cMoveDef.mvType == 1)
						{
							rhMouseUsed |= (byte)(1 << cMoveDef.mvControl - 1);
						}
					}
				}
			}
		}
		for (int i = 0; i < rhFrame.nLayers; i++)
		{
			CLayer cLayer = rhFrame.layers[i];
			cLayer.nZOrderMax = 1;
		}
		return 0;
	}

	public int createFrameObjects()
	{
		int result = 0;
		int i = 0;
		for (CLO cLO = rhFrame.LOList.first_LevObj(); cLO != null; i++, cLO = rhFrame.LOList.next_LevObj())
		{
			COI oIFromHandle = rhApp.OIList.getOIFromHandle(cLO.loOiHandle);
			CObjectCommon cObjectCommon = (CObjectCommon)oIFromHandle.oiOC;
			short oiType = oIFromHandle.oiType;
			short num = 0;
			if (cLO.loParentType != 0)
			{
				continue;
			}
			if (oiType == 3)
			{
				num |= 4;
			}
			if ((cObjectCommon.ocFlags2 & 8) == 0)
			{
				if (oiType == 4)
				{
					continue;
				}
				num |= 2;
			}
			if ((cObjectCommon.ocOEFlags & 0x20000) == 0)
			{
				f_CreateObject(cLO.loHandle, cLO.loOiHandle, int.MaxValue, int.MaxValue, -1, num, -1, -1);
			}
		}
		rhGameFlags &= -513;
		return result;
	}

	public void killFrameObjects()
	{
		short num = 0;
		while (num < rhMaxObjects && rhNObjects != 0)
		{
			f_KillObject(num, bFast: true);
			num++;
		}
		rh4FirstQuickDisplay = -1;
	}

	public void y_KillLevel(bool bLeaveSamples)
	{
		resetFrameLayers(-1, bDeleteFrame: false);
		if (!bLeaveSamples)
		{
			if ((rhApp.gaNewFlags & 1) == 0)
			{
				rhApp.soundPlayer.stopAllSounds();
			}
			else
			{
				rhApp.soundPlayer.keepCurrentSounds();
			}
		}
	}

	public void resetFrameLayers(int nLayer, bool bDeleteFrame)
	{
		int num2;
		if (nLayer == -1)
		{
			int num = 0;
			num2 = rhFrame.nLayers;
		}
		else
		{
			int num = nLayer;
			num2 = nLayer + 1;
		}
		for (int num = 0; num < num2; num++)
		{
			CLayer cLayer = rhFrame.layers[num];
			int nBkdLOs = cLayer.nBkdLOs;
			for (int i = 0; i < nBkdLOs; i++)
			{
				CLO lOFromIndex = rhFrame.LOList.getLOFromIndex((short)(cLayer.nFirstLOIndex + i));
				for (int j = 0; j < 4; j++)
				{
					if (lOFromIndex.loSpr[j] != null)
					{
						rhApp.spriteGen.delSpriteFast(lOFromIndex.loSpr[j]);
						lOFromIndex.loSpr[j] = null;
					}
				}
			}
			if (cLayer.pBkd2 != null)
			{
				for (int i = 0; i < cLayer.pBkd2.size(); i++)
				{
					CBkd2 cBkd = (CBkd2)cLayer.pBkd2.get(i);
					for (int k = 0; k < 4; k++)
					{
						if (cBkd.pSpr[k] != null)
						{
							rhApp.spriteGen.delSpriteFast(cBkd.pSpr[k]);
							cBkd.pSpr[k] = null;
						}
					}
				}
			}
			cLayer.dwOptions = cLayer.backUp_dwOptions;
			cLayer.xCoef = cLayer.backUp_xCoef;
			cLayer.yCoef = cLayer.backUp_yCoef;
			cLayer.nBkdLOs = cLayer.backUp_nBkdLOs;
			cLayer.nFirstLOIndex = cLayer.backUp_nFirstLOIndex;
			cLayer.x = (cLayer.y = (cLayer.dx = (cLayer.dy = 0)));
			cLayer.pBkd2 = null;
			cLayer.pLadders = null;
		}
	}

	private void f_RemoveObjects()
	{
		int i = 0;
		for (int j = 0; j < rhNObjects; j++)
		{
			for (; rhObjectList[i] == null; i++)
			{
			}
			CObject cObject = rhObjectList[i];
			i++;
			if (cObject.ros != null && cObject.roc.rcSprite != null)
			{
				cObject.ros.rsZOrder = cObject.roc.rcSprite.sprZOrder;
				rhApp.spriteGen.delSpriteFast(cObject.roc.rcSprite);
			}
			if ((cObject.hoOEFlags & 0x1000) != 0)
			{
				remove_QuickDisplay(cObject);
			}
		}
	}

	public void captureMouse()
	{
		if (rhMouseUsed != 0)
		{
			MouseState state = Mouse.GetState();
			rh2MouseSaveX = state.X;
			rh2MouseSaveY = state.Y;
			hideMouse();
		}
	}

	public void freeMouse()
	{
		if (rhMouseUsed != 0)
		{
			showMouse();
			Mouse.SetPosition(rh2MouseSaveX, rh2MouseSaveY);
		}
	}

	public void showMouse()
	{
		rh4CursorShown = true;
		rhApp.showCursor(bShown: true);
	}

	public void hideMouse()
	{
		rh4CursorShown = false;
		rhApp.showCursor(bShown: false);
	}

	public void saveGlobalObjectsData()
	{
		for (int i = 0; i < rhOiList.Length; i++)
		{
			CObjInfo cObjInfo = rhOiList[i];
			short num = cObjInfo.oilObject;
			if (cObjInfo.oilOi == short.MaxValue || (num & 0x8000) != 0)
			{
				continue;
			}
			COI oIFromHandle = rhApp.OIList.getOIFromHandle(cObjInfo.oilOi);
			if ((oIFromHandle.oiFlags & 4) == 0)
			{
				continue;
			}
			CObject cObject = rhObjectList[num];
			if (cObjInfo.oilType != 3 && cObjInfo.oilType != 7 && cObject.rov == null)
			{
				continue;
			}
			string text = $"{cObjInfo.oilName:s}::{cObjInfo.oilType:d}";
			if (rhApp.adGO == null)
			{
				rhApp.adGO = new CArrayList();
			}
			bool flag = false;
			CSaveGlobal cSaveGlobal = null;
			for (int j = 0; j < rhApp.adGO.size(); j++)
			{
				cSaveGlobal = (CSaveGlobal)rhApp.adGO.get(j);
				if (text == cSaveGlobal.name)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				cSaveGlobal = new CSaveGlobal();
				cSaveGlobal.name = text;
				cSaveGlobal.objects = new CArrayList();
				rhApp.adGO.add(cSaveGlobal);
			}
			else
			{
				cSaveGlobal.objects.clear();
			}
			do
			{
				cObject = rhObjectList[num];
				if (cObjInfo.oilType == 3)
				{
					CText cText = (CText)cObject;
					CSaveGlobalText cSaveGlobalText = new CSaveGlobalText();
					cSaveGlobalText.text = cText.rsTextBuffer;
					cSaveGlobalText.rsMini = cText.rsMini;
					cSaveGlobal.objects.add(cSaveGlobalText);
				}
				else if (cObjInfo.oilType == 7)
				{
					CCounter cCounter = (CCounter)cObject;
					CSaveGlobalCounter cSaveGlobalCounter = new CSaveGlobalCounter();
					cSaveGlobalCounter.value = new CValue(cCounter.rsValue);
					cSaveGlobalCounter.rsMini = cCounter.rsMini;
					cSaveGlobalCounter.rsMaxi = cCounter.rsMaxi;
					cSaveGlobalCounter.rsMiniDouble = cCounter.rsMiniDouble;
					cSaveGlobalCounter.rsMaxiDouble = cCounter.rsMaxiDouble;
					cSaveGlobal.objects.add(cSaveGlobalCounter);
				}
				else
				{
					CSaveGlobalValues cSaveGlobalValues = new CSaveGlobalValues();
					cSaveGlobalValues.flags = cObject.rov.rvValueFlags;
					cSaveGlobalValues.values = new CValue[26];
					for (int k = 0; k < 26; k++)
					{
						cSaveGlobalValues.values[k] = null;
						if (cObject.rov.rvValues[k] != null)
						{
							cSaveGlobalValues.values[k] = new CValue(cObject.rov.rvValues[k]);
						}
					}
					cSaveGlobalValues.strings = new string[10];
					for (int k = 0; k < 10; k++)
					{
						cSaveGlobalValues.strings[k] = null;
						if (cObject.rov.rvStrings[k] != null)
						{
							cSaveGlobalValues.strings[k] = cObject.rov.rvStrings[k];
						}
					}
					cSaveGlobal.objects.add(cSaveGlobalValues);
				}
				num = cObject.hoNumNext;
			}
			while ((num & 0x8000) == 0);
		}
	}

	public void loadGlobalObjectsData()
	{
		if (rhApp.adGO == null)
		{
			return;
		}
		for (int i = 0; i < rhOiList.Length; i++)
		{
			CObjInfo cObjInfo = rhOiList[i];
			short num = cObjInfo.oilObject;
			if (cObjInfo.oilOi == short.MaxValue || (num & 0x8000) != 0)
			{
				continue;
			}
			COI oIFromHandle = rhApp.OIList.getOIFromHandle(cObjInfo.oilOi);
			if ((oIFromHandle.oiFlags & 4) == 0)
			{
				continue;
			}
			string text = $"{cObjInfo.oilName:s}::{cObjInfo.oilType:d}";
			for (int j = 0; j < rhApp.adGO.size(); j++)
			{
				CSaveGlobal cSaveGlobal = (CSaveGlobal)rhApp.adGO.get(j);
				if (!(text == cSaveGlobal.name))
				{
					continue;
				}
				int num2 = 0;
				do
				{
					CObject cObject = rhObjectList[num];
					if (cObjInfo.oilType == 3)
					{
						CSaveGlobalText cSaveGlobalText = (CSaveGlobalText)cSaveGlobal.objects.get(num2);
						CText cText = (CText)cObject;
						cText.rsTextBuffer = cSaveGlobalText.text;
						cText.rsMini = cSaveGlobalText.rsMini;
					}
					else if (cObjInfo.oilType == 7)
					{
						CSaveGlobalCounter cSaveGlobalCounter = (CSaveGlobalCounter)cSaveGlobal.objects.get(num2);
						CCounter cCounter = (CCounter)cObject;
						cCounter.rsValue = new CValue(cSaveGlobalCounter.value);
						cCounter.rsMini = cSaveGlobalCounter.rsMini;
						cCounter.rsMaxi = cSaveGlobalCounter.rsMaxi;
						cCounter.rsMiniDouble = cSaveGlobalCounter.rsMiniDouble;
						cCounter.rsMaxiDouble = cSaveGlobalCounter.rsMaxiDouble;
					}
					else
					{
						CSaveGlobalValues cSaveGlobalValues = (CSaveGlobalValues)cSaveGlobal.objects.get(num2);
						cObject.rov.rvValueFlags = cSaveGlobalValues.flags;
						for (int k = 0; k < 26; k++)
						{
							if (cSaveGlobalValues.values[k] != null)
							{
								cObject.rov.rvValues[k] = new CValue(cSaveGlobalValues.values[k]);
							}
						}
						for (int k = 0; k < 10; k++)
						{
							if (cSaveGlobalValues.strings[k] != null)
							{
								cObject.rov.rvStrings[k] = cSaveGlobalValues.strings[k];
							}
						}
					}
					num = cObject.hoNumNext;
					if ((num & 0x8000) != 0)
					{
						break;
					}
					num2++;
				}
				while (num2 < cSaveGlobal.objects.size());
				break;
			}
		}
	}

	public int f_CreateObject(short hlo, short oi, int coordX, int coordY, int initDir, short flags, int nLayer, int numCreation)
	{
		CCreateObjectInfo cCreateObjectInfo = new CCreateObjectInfo();
		CLO cLO = null;
		if (hlo != -1)
		{
			cLO = rhFrame.LOList.getLOFromHandle(hlo);
		}
		COI oIFromHandle = rhApp.OIList.getOIFromHandle(oi);
		CObjectCommon cObjectCommon = (CObjectCommon)oIFromHandle.oiOC;
		if ((cObjectCommon.ocFlags2 & 8) == 0)
		{
			flags |= 2;
		}
		if (rhNObjects < rhMaxObjects)
		{
			CObject cObject = null;
			switch (oIFromHandle.oiType)
			{
			case 2:
				cObject = new CActive();
				break;
			case 3:
				cObject = new CText();
				break;
			case 4:
				cObject = new CQuestion();
				break;
			case 5:
				cObject = new CScore();
				break;
			case 6:
				cObject = new CLives();
				break;
			case 7:
				cObject = new CCounter();
				break;
			case 9:
				cObject = new CCCA();
				break;
			default:
				cObject = new CExtension(oIFromHandle.oiType, this);
				if (((CExtension)cObject).ext == null)
				{
					cObject = null;
				}
				break;
			case 8:
				break;
			}
			if (cObject != null)
			{
				if (numCreation < 0)
				{
					numCreation = 0;
					while (numCreation < rhMaxObjects && rhObjectList[numCreation] != null)
					{
						numCreation++;
					}
				}
				if (numCreation >= rhMaxObjects)
				{
					return -1;
				}
				rhObjectList[numCreation] = cObject;
				rhNObjects++;
				cObject.hoIdentifier = cObjectCommon.ocIdentifier;
				cObject.hoOEFlags = cObjectCommon.ocOEFlags;
				if (numCreation > rh4ObjectCurCreate)
				{
					rh4ObjectAddCreate++;
				}
				cObject.hoNumber = (short)numCreation;
				rh2CreationCount++;
				if (rh2CreationCount == 0)
				{
					rh2CreationCount = 1;
				}
				cObject.hoCreationId = rh2CreationCount;
				cObject.hoOi = oi;
				cObject.hoHFII = hlo;
				cObject.hoType = oIFromHandle.oiType;
				oi_Insert(cObject);
				cObject.hoAdRunHeader = this;
				cObject.hoCallRoutine = true;
				cObject.hoCommon = cObjectCommon;
				int num = coordX;
				if (num == int.MaxValue)
				{
					num = cLO.loX;
				}
				cCreateObjectInfo.cobX = num;
				cObject.hoX = num;
				int num2 = coordY;
				if (num2 == int.MaxValue)
				{
					num2 = cLO.loY;
				}
				cCreateObjectInfo.cobY = num2;
				cObject.hoY = num2;
				if (cLO != null)
				{
					if (nLayer == -1)
					{
						nLayer = cLO.loLayer;
					}
				}
				else
				{
					nLayer = 0;
				}
				cCreateObjectInfo.cobLayer = nLayer;
				cObject.hoLayer = nLayer;
				CLayer cLayer = rhFrame.layers[nLayer];
				cLayer.nZOrderMax++;
				cCreateObjectInfo.cobZOrder = cLayer.nZOrderMax;
				cCreateObjectInfo.cobFlags = flags;
				cCreateObjectInfo.cobDir = initDir;
				cCreateObjectInfo.cobLevObj = cLO;
				cObject.roc = null;
				if ((cObject.hoOEFlags & 0x230) != 0)
				{
					cObject.roc = new CRCom();
					cObject.roc.init();
				}
				cObject.rom = null;
				if ((cObject.hoOEFlags & 0x10) != 0)
				{
					cObject.rom = new CRMvt();
					if ((cCreateObjectInfo.cobFlags & 1) == 0)
					{
						cObject.rom.init(0, cObject, cObjectCommon, cCreateObjectInfo, -1);
					}
				}
				cObject.roa = null;
				if ((cObject.hoOEFlags & 0x20) != 0)
				{
					cObject.roa = new CRAni();
					cObject.roa.init(cObject);
				}
				cObject.ros = null;
				if ((cObject.hoOEFlags & 0x200) != 0)
				{
					cObject.ros = new CRSpr();
					cObject.ros.init1(cObject, cObjectCommon, cCreateObjectInfo);
				}
				cObject.rov = null;
				if ((cObject.hoOEFlags & 0x100) != 0)
				{
					cObject.rov = new CRVal();
					cObject.rov.init(cObject, cObjectCommon, cCreateObjectInfo);
				}
				cObject.init(cObjectCommon, cCreateObjectInfo);
				if ((cObject.hoOEFlags & 0x200) != 0)
				{
					cObject.ros.init2(bTransition: true);
				}
				return numCreation;
			}
		}
		return -1;
	}

	public void f_KillObject(int nObject, bool bFast)
	{
		CObject cObject = rhObjectList[nObject];
		if (cObject != null)
		{
			killShootPtr(cObject);
			if (cObject.rom != null)
			{
				cObject.rom.kill(bFast);
			}
			if (cObject.rov != null)
			{
				cObject.rov.kill(bFast);
			}
			if (cObject.ros != null)
			{
				cObject.ros.kill(bFast);
			}
			if (cObject.roc != null)
			{
				cObject.roc.kill(bFast);
			}
			cObject.kill(bFast);
			oi_Delete(cObject);
			cObject.hoCreationId = 0;
			if ((cObject.hoOEFlags & 0x1000) != 0 && cObject.ros.rsLayer == 0)
			{
				remove_QuickDisplay(cObject);
			}
			rhObjectList[nObject] = null;
			rhNObjects--;
			cObject.hoCallRoutine = false;
		}
	}

	public void destroy_Add(int hoNumber)
	{
		rhDestroyList[hoNumber / 32] |= 1 << hoNumber;
		rhDestroyPos++;
	}

	public void destroy_List()
	{
		if (rhDestroyPos == 0)
		{
			return;
		}
		for (int i = 0; i < rhMaxObjects; i += 32)
		{
			int num = rhDestroyList[i / 32];
			if (num == 0)
			{
				continue;
			}
			int num2 = 0;
			while (num != 0 && num2 < 32)
			{
				if ((num & 1) != 0)
				{
					CObject cObject = rhObjectList[i + num2];
					if (cObject != null && cObject.hoOiList.oilNObjects == 1)
					{
						int num3 = -2162688;
						num3 |= cObject.hoType & 0xFFFF;
						rhEvtProg.handle_Event(cObject, num3);
					}
					f_KillObject(i + num2, bFast: false);
					rhDestroyPos--;
				}
				num >>= 1;
				num2++;
			}
			rhDestroyList[i / 32] = 0;
			if (rhDestroyPos == 0)
			{
				break;
			}
		}
	}

	private void killShootPtr(CObject hoSource)
	{
		int i = 0;
		for (int j = 0; j < rhNObjects; j++)
		{
			for (; rhObjectList[i] == null; i++)
			{
			}
			CObject cObject = rhObjectList[i];
			i++;
			if (cObject.rom != null && cObject.roc.rcMovementType == 13)
			{
				CMoveBullet cMoveBullet = (CMoveBullet)cObject.rom.rmMovement;
				if (cMoveBullet.MBul_ShootObject == hoSource && cMoveBullet.MBul_Wait)
				{
					cMoveBullet.startBullet();
				}
			}
		}
	}

	public void oi_Insert(CObject pHo)
	{
		short hoOi = pHo.hoOi;
		int i;
		for (i = 0; i < rhMaxOI && rhOiList[i].oilOi != hoOi; i++)
		{
		}
		CObjInfo cObjInfo = rhOiList[i];
		if ((cObjInfo.oilObject & 0x8000) != 0)
		{
			cObjInfo.oilObject = pHo.hoNumber;
			pHo.hoNumPrev = -1;
			pHo.hoNumNext = -1;
		}
		else
		{
			CObject cObject = rhObjectList[cObjInfo.oilObject];
			pHo.hoNumPrev = cObject.hoNumPrev;
			cObject.hoNumPrev = pHo.hoNumber;
			pHo.hoNumNext = cObject.hoNumber;
			cObjInfo.oilObject = pHo.hoNumber;
		}
		pHo.hoEvents = cObjInfo.oilEvents;
		pHo.hoOiList = cObjInfo;
		pHo.hoLimitFlags = cObjInfo.oilLimitFlags;
		if (pHo.hoHFII == -1)
		{
			pHo.hoHFII = cObjInfo.oilHFII;
		}
		else if (cObjInfo.oilHFII == -1)
		{
			cObjInfo.oilHFII = pHo.hoHFII;
		}
		cObjInfo.oilNObjects++;
	}

	private void oi_Delete(CObject pHo)
	{
		CObjInfo hoOiList = pHo.hoOiList;
		hoOiList.oilNObjects--;
		if (pHo.hoNumPrev >= 0)
		{
			CObject cObject = rhObjectList[pHo.hoNumPrev];
			if (pHo.hoNumNext >= 0)
			{
				CObject cObject2 = rhObjectList[pHo.hoNumNext];
				if (cObject != null)
				{
					cObject.hoNumNext = pHo.hoNumNext;
				}
				if (cObject2 != null)
				{
					cObject2.hoNumPrev = pHo.hoNumPrev;
				}
			}
			else if (cObject != null)
			{
				cObject.hoNumNext = -1;
			}
		}
		else if (pHo.hoNumNext >= 0)
		{
			CObject cObject3 = rhObjectList[pHo.hoNumNext];
			if (cObject3 != null)
			{
				cObject3.hoNumPrev = pHo.hoNumPrev;
				hoOiList.oilObject = cObject3.hoNumber;
			}
		}
		else
		{
			hoOiList.oilObject = -1;
		}
	}

	public void pause()
	{
		rh2PauseCompteur++;
		if (rh2PauseCompteur != 1)
		{
			return;
		}
		rh2PauseTimer = (int)rhApp.timer;
		rh2PauseFPSTimer = (int)rhApp.timer;
		rh2PauseState = 0;
		rh2PauseVbl = rhApp.newGetCptVbl() - rhVBLOld;
		int i = 0;
		for (int j = 0; j < rhNObjects; j++)
		{
			for (; rhObjectList[i] == null; i++)
			{
			}
			CObject cObject = rhObjectList[i];
			i++;
			if (cObject.hoType == 9)
			{
				((CCCA)cObject).pause();
			}
			else if (cObject.hoType >= 32)
			{
				CExtension cExtension = (CExtension)cObject;
				cExtension.ext.pauseRunObject();
			}
		}
		rhApp.soundPlayer.pause();
		showMouse();
	}

	public void resume()
	{
		if (rh2PauseCompteur == 0)
		{
			return;
		}
		rh2PauseCompteur = Math.Max(rh2PauseCompteur - 1, 0);
		if (rh2PauseCompteur != 0)
		{
			return;
		}
		if (rhMouseUsed != 0)
		{
			MouseState state = Mouse.GetState();
			rh2MouseSaveX = state.X;
			rh2MouseSaveY = state.Y;
			hideMouse();
			Mouse.SetPosition(rh4MouseXCenter, rh4MouseYCenter);
		}
		else if (!rh4CursorShown)
		{
			hideMouse();
		}
		int i = 0;
		for (int j = 0; j < rhNObjects; j++)
		{
			for (; rhObjectList[i] == null; i++)
			{
			}
			CObject cObject = rhObjectList[i];
			i++;
			if (cObject.hoType == 9)
			{
				((CCCA)cObject).resume();
			}
			else if (cObject.hoType >= 32)
			{
				CExtension cExtension = (CExtension)cObject;
				cExtension.ext.continueRunObject();
			}
		}
		rhApp.soundPlayer.resume();
		rhTimerOld += (int)(rhApp.timer - rh2PauseTimer);
		rhTimerFPSOld += (int)(rhApp.timer - rh2PauseFPSTimer);
		rhVBLOld = rhApp.newGetCptVbl() - rh2PauseVbl;
		rh4PauseKey = Keys.None;
		bCheckResume = false;
	}

	public void f_StopSamples()
	{
		rhApp.soundPlayer.stopAllSounds();
	}

	public void redrawLevel(int flags)
	{
		bool flag = false;
		CObject cObject = null;
		bool flag2 = (rhFrame.leFlags & 0x20) != 0;
		bool flag3 = (flags & 0x10) == 0;
		bool flag4 = (flags & 0x40) != 0;
		CRect cRect = new CRect();
		bool flag5 = false;
		cRect.left = (cRect.top = 0);
		cRect.right = rhApp.gaCxWin;
		cRect.bottom = rhApp.gaCyWin;
		int right = cRect.right;
		int num = right - 1;
		int bottom = cRect.bottom;
		int num2 = bottom - 1;
		int i;
		if ((flags & 0x10C) != 0)
		{
			for (i = 0; i < rhFrame.nLayers; i++)
			{
				CLayer cLayer = rhFrame.layers[i];
				if ((cLayer.dwOptions & 0x40000) != 0)
				{
					f_ShowAllObjects(i, bShow: true);
				}
				if ((cLayer.dwOptions & 0x20000) != 0)
				{
					f_ShowAllObjects(i, bShow: false);
				}
			}
		}
		if (!flag4 && (flags & 0x80) != 0)
		{
			CLayer cLayer2 = rhFrame.layers[0];
			if ((cLayer2.dwOptions & 0x10000) == 0)
			{
				flag4 = true;
			}
		}
		for (i = 0; i < rhFrame.nLayers; i++)
		{
			CLayer cLayer3 = rhFrame.layers[i];
			if ((cLayer3.dwOptions & 0x20000) == 0)
			{
				continue;
			}
			int nBkdLOs = cLayer3.nBkdLOs;
			for (int j = 0; j < nBkdLOs; j++)
			{
				CLO lOFromIndex = rhFrame.LOList.getLOFromIndex((short)(cLayer3.nFirstLOIndex + j));
				for (int k = 0; k < 4; k++)
				{
					if (lOFromIndex.loSpr[k] != null)
					{
						rhApp.spriteGen.delSprite(lOFromIndex.loSpr[k]);
						lOFromIndex.loSpr[k] = null;
					}
				}
			}
		}
		if ((flags & 4) != 0)
		{
			_ = flags & 0x80;
			f_UpdateWindowPos(rhFrame.leX, rhFrame.leY);
		}
		if (rhFrame.colMask != null && flag3)
		{
			rhFrame.colMask.fillRectangle(-32767, -32767, 32767, 32767, 0);
			flag = true;
		}
		int leWidth = rhFrame.leWidth;
		int leHeight = rhFrame.leHeight;
		i = 0;
		if (flag4)
		{
			i++;
		}
		int num3;
		int num4;
		for (; i < rhFrame.nLayers; i++)
		{
			CLayer cLayer4 = rhFrame.layers[i];
			cLayer4.x += cLayer4.dx;
			cLayer4.y += cLayer4.dy;
			cLayer4.dx = 0;
			cLayer4.dy = 0;
			if ((cLayer4.dwOptions & 0x40000) != 0)
			{
				cLayer4.dwOptions |= 16;
			}
			if ((cLayer4.dwOptions & 0x10) == 0)
			{
				if (!flag3)
				{
					continue;
				}
				flag5 = true;
			}
			if ((flags & 0x80) != 0 && (cLayer4.dwOptions & 0x10000) == 0)
			{
				continue;
			}
			cLayer4.dwOptions &= -65537;
			bool flag6 = (cLayer4.dwOptions & 0x20) != 0;
			bool flag7 = (cLayer4.dwOptions & 0x40) != 0;
			bool flag8 = flag6 | flag7;
			num3 = rhFrame.leX;
			num4 = rhFrame.leY;
			if ((cLayer4.dwOptions & 3) != 0)
			{
				if ((cLayer4.dwOptions & 1) != 0)
				{
					num3 = (int)((float)num3 * cLayer4.xCoef);
				}
				if ((cLayer4.dwOptions & 2) != 0)
				{
					num4 = (int)((float)num4 * cLayer4.yCoef);
				}
			}
			num3 += cLayer4.x;
			num4 += cLayer4.y;
			if (flag6)
			{
				num3 %= leWidth;
			}
			if (flag7)
			{
				num4 %= leHeight;
			}
			y_Ladder_Reset(i);
			int nBkdLOs2 = cLayer4.nBkdLOs;
			if ((cLayer4.dwOptions & 0x20000) != 0)
			{
				f_ShowAllObjects(i, bShow: false);
				if (i == 0)
				{
					flag5 = true;
				}
			}
			if (((cLayer4.dwOptions & 0x10) != 0 && (cLayer4.dwOptions & 0x20000) == 0) || i == 0)
			{
				bool flag9 = (cLayer4.dwOptions & 4) == 0;
				if ((cLayer4.dwOptions & 0x40000) != 0)
				{
					cLayer4.dwOptions &= -262145;
					f_ShowAllObjects(i, bShow: true);
				}
				uint num5 = 0u;
				int num6 = 0;
				for (int num7 = 0; num7 < nBkdLOs2; num7++)
				{
					CLO lOFromIndex = rhFrame.LOList.getLOFromIndex((short)(num7 + cLayer4.nFirstLOIndex));
					bool flag10 = true;
					int num8 = num6;
					int num9 = num6;
					COI cOI = null;
					COC cOC = null;
					CObjectCommon cObjectCommon = null;
					int loType = lOFromIndex.loType;
					if (loType < 2)
					{
						cRect.left = lOFromIndex.loX - num3;
						cRect.top = lOFromIndex.loY - num4;
						goto IL_0573;
					}
					cOI = rhApp.OIList.getOIFromHandle(lOFromIndex.loOiHandle);
					if (cOI == null || cOI.oiOC == null)
					{
						num5 = 0u;
						num6 = 0;
					}
					else
					{
						cOC = cOI.oiOC;
						cObjectCommon = (CObjectCommon)cOC;
						if ((cObjectCommon.ocOEFlags & 2) != 0 && (cObject = find_HeaderObject(lOFromIndex.loHandle)) != null)
						{
							cRect.left = cObject.hoX - rhFrame.leX - cObject.hoImgXSpot;
							cRect.top = cObject.hoY - rhFrame.leY - cObject.hoImgYSpot;
							cObject.getZoneInfos();
							goto IL_0573;
						}
						num5 = 0u;
						num6 = 0;
					}
					goto IL_0f53;
					IL_0573:
					if (!flag2 && !flag8 && (cRect.left >= num + 64 + 32 || cRect.top >= num2 + 16))
					{
						num5 = 0u;
						num6 = 0;
					}
					else
					{
						int num10;
						bool flag11;
						if (loType < 2)
						{
							cOI = rhApp.OIList.getOIFromHandle(lOFromIndex.loOiHandle);
							if (cOI == null || cOI.oiOC == null)
							{
								num5 = 0u;
								num6 = 0;
								goto IL_0f53;
							}
							cOC = cOI.oiOC;
							cRect.right = cRect.left + cOC.ocCx;
							cRect.bottom = cRect.top + cOC.ocCy;
							num10 = cOC.ocObstacleType;
							flag11 = cOC.ocColMode != 0;
						}
						else
						{
							cRect.right = cRect.left + cObject.hoImgWidth;
							cRect.bottom = cRect.top + cObject.hoImgHeight;
							num10 = (cObjectCommon.ocFlags2 & 0x30) >> 4;
							flag11 = (cObjectCommon.ocFlags2 & 4) != 0;
						}
						if (flag8)
						{
							switch (num6)
							{
							case 0:
								if (flag6 && (cRect.left < 0 || cRect.right > leWidth))
								{
									if (flag7 && (cRect.top < 0 || cRect.bottom > leHeight))
									{
										num6 = 3;
										num5 |= 7;
									}
									else
									{
										num6 = 1;
										num5 |= 1;
									}
								}
								else if (flag7 && (cRect.top < 0 || cRect.bottom > leHeight))
								{
									num6 = 2;
									num5 |= 2;
								}
								if ((num5 & 1) == 0 && lOFromIndex.loSpr[1] != null)
								{
									rhApp.spriteGen.delSprite(lOFromIndex.loSpr[1]);
									lOFromIndex.loSpr[1] = null;
								}
								if ((num5 & 2) == 0 && lOFromIndex.loSpr[2] != null)
								{
									rhApp.spriteGen.delSprite(lOFromIndex.loSpr[2]);
									lOFromIndex.loSpr[2] = null;
								}
								if ((num5 & 4) == 0 && lOFromIndex.loSpr[3] != null)
								{
									rhApp.spriteGen.delSprite(lOFromIndex.loSpr[3]);
									lOFromIndex.loSpr[3] = null;
								}
								break;
							case 1:
								if (cRect.left < 0)
								{
									int num17 = leWidth;
									cRect.left += num17;
									cRect.right += num17;
								}
								else if (cRect.right > leWidth)
								{
									int num18 = leWidth;
									cRect.left -= num18;
									cRect.right -= num18;
								}
								num5 &= 0xFFFFFFFEu;
								num6 = 0;
								if ((num5 & 2) != 0)
								{
									num6 = 2;
								}
								break;
							case 2:
								if (cRect.top < 0)
								{
									int num15 = leHeight;
									cRect.top += num15;
									cRect.bottom += num15;
								}
								else if (cRect.bottom > leHeight)
								{
									int num16 = leHeight;
									cRect.top -= num16;
									cRect.bottom -= num16;
								}
								num5 &= 0xFFFFFFFDu;
								num6 = 0;
								if ((num5 & 1) != 0)
								{
									num6 = 1;
								}
								break;
							case 3:
								if (cRect.left < 0)
								{
									int num11 = leWidth;
									cRect.left += num11;
									cRect.right += num11;
								}
								else if (cRect.right > leWidth)
								{
									int num12 = leWidth;
									cRect.left -= num12;
									cRect.right -= num12;
								}
								if (cRect.top < 0)
								{
									int num13 = leHeight;
									cRect.top += num13;
									cRect.bottom += num13;
								}
								else if (cRect.bottom > leHeight)
								{
									int num14 = leHeight;
									cRect.top -= num14;
									cRect.bottom -= num14;
								}
								num5 &= 0xFFFFFFFBu;
								num6 = 2;
								break;
							}
						}
						if (num10 == 3)
						{
							y_Ladder_Add(i, cRect.left, cRect.top, cRect.right, cRect.bottom);
							flag11 = true;
						}
						if (rhFrame.colMask != null && i == 0 && flag3 && num10 != 4 && (flag2 || (cRect.right >= -96 && cRect.bottom >= -16)))
						{
							CMask cMask = null;
							if (flag2)
							{
								cRect.left += num3;
								cRect.top += num4;
								cRect.right += num3;
								cRect.bottom += num4;
							}
							int val = 0;
							if (num10 == 1)
							{
								val = 3;
								flag = false;
							}
							if (!flag)
							{
								if (flag11)
								{
									rhFrame.colMask.fillRectangle(cRect.left, cRect.top, cRect.right - 1, cRect.bottom - 1, val);
								}
								else
								{
									if (cMask == null)
									{
										if (loType < 2)
										{
											short ocImage = ((COCBackground)cOC).ocImage;
											CImage imageFromHandle = rhApp.imageBank.getImageFromHandle(ocImage);
											cMask = imageFromHandle.getMask(0, 0, 1f, 1f);
										}
										else
										{
											cMask = cObject.getCollisionMask(0);
										}
									}
									if (cMask == null)
									{
										rhFrame.colMask.fillRectangle(cRect.left, cRect.top, cRect.right - 1, cRect.bottom - 1, val);
									}
									else
									{
										rhFrame.colMask.orMask(cMask, cRect.left, cRect.top, 3, val);
									}
								}
							}
							if (num10 == 2)
							{
								flag = false;
								if (flag11)
								{
									rhFrame.colMask.fillRectangle(cRect.left, cRect.top, cRect.right - 1, Math.Min(cRect.top + 6, cRect.bottom) - 1, 2);
								}
								else
								{
									if (cMask == null)
									{
										if (loType < 2)
										{
											short ocImage = ((COCBackground)cOC).ocImage;
											CImage imageFromHandle2 = rhApp.imageBank.getImageFromHandle(ocImage);
											cMask = imageFromHandle2.getMask(0, 0, 1f, 1f);
										}
										else
										{
											cMask = cObject.getCollisionMask(0);
										}
									}
									if (cMask == null)
									{
										rhFrame.colMask.fillRectangle(cRect.left, cRect.top, cRect.right - 1, Math.Min(cRect.top + 6, cRect.bottom) - 1, 2);
									}
									else
									{
										rhFrame.colMask.orPlatformMask(cMask, cRect.left, cRect.top);
									}
								}
							}
							if (flag2)
							{
								cRect.left -= num3;
								cRect.top -= num4;
								cRect.right -= num3;
								cRect.bottom -= num4;
							}
						}
						if (cRect.left <= num && cRect.top <= num2 && cRect.right >= 0 && cRect.bottom >= 0)
						{
							flag10 = false;
							if (i > 0 || !flag5)
							{
								uint num19 = 4718600u;
								if (!flag9)
								{
									num19 |= 0x200;
								}
								if (i > 0)
								{
									switch (num10)
									{
									case 1:
										num19 |= 0x10001;
										break;
									case 2:
										num19 |= 0x20001;
										break;
									}
								}
								if (lOFromIndex.loSpr[num9] == null)
								{
									switch (loType)
									{
									case 0:
										lOFromIndex.loSpr[num9] = rhApp.spriteGen.addOwnerDrawSprite(cRect.left, cRect.top, cRect.right, cRect.bottom, lOFromIndex.loLayer, num7 * 4 + num8, 0, num19 | 0x100, null, (IDrawing)cOC);
										break;
									case 1:
										lOFromIndex.loSpr[num9] = rhApp.spriteGen.addSprite(cRect.left, cRect.top, ((COCBackground)cOC).ocImage, lOFromIndex.loLayer, num7 * 4 + num8, 0, num19, null);
										rhApp.spriteGen.modifSpriteEffect(lOFromIndex.loSpr[num6], cOI.oiInkEffect, cOI.oiInkEffectParam);
										break;
									default:
										if (cObject != null)
										{
											lOFromIndex.loSpr[num9] = rhApp.spriteGen.addOwnerDrawSprite(cRect.left, cRect.top, cRect.right, cRect.bottom, lOFromIndex.loLayer, num7 * 4 + num8, 0, num19 | 0x100, null, cObject);
										}
										break;
									}
								}
								else
								{
									switch (loType)
									{
									case 0:
									{
										CRect spriteRect = lOFromIndex.loSpr[num9].getSpriteRect();
										if (cRect.left != spriteRect.left || cRect.top != spriteRect.top || cRect.right != spriteRect.right || cRect.bottom != spriteRect.bottom)
										{
											rhApp.spriteGen.modifOwnerDrawSprite(lOFromIndex.loSpr[num9], cRect.left, cRect.top, cRect.right, cRect.bottom);
										}
										break;
									}
									case 1:
										rhApp.spriteGen.modifSprite(lOFromIndex.loSpr[num9], cRect.left, cRect.top, ((COCBackground)cOC).ocImage);
										break;
									default:
										if (cObject != null)
										{
											rhApp.spriteGen.modifOwnerDrawSprite(lOFromIndex.loSpr[num9], cRect.left, cRect.top, cRect.right, cRect.bottom);
										}
										break;
									}
								}
							}
						}
					}
					goto IL_0f53;
					IL_0f53:
					if (flag10 && lOFromIndex.loSpr[num9] != null)
					{
						rhApp.spriteGen.delSprite(lOFromIndex.loSpr[num9]);
						lOFromIndex.loSpr[num9] = null;
					}
					if (num5 != 0)
					{
						num7--;
					}
				}
			}
			if (cLayer4.pBkd2 != null)
			{
				displayBkd2Layer(cLayer4, i, flags, num, num2, flag);
			}
			if ((cLayer4.dwOptions & 0x20000) != 0)
			{
				cLayer4.dwOptions &= -131089;
			}
		}
		if (!flag2)
		{
			return;
		}
		CLayer cLayer5 = rhFrame.layers[0];
		num3 = rhFrame.leX;
		num4 = rhFrame.leY;
		if ((cLayer5.dwOptions & 3) != 0)
		{
			if ((cLayer5.dwOptions & 1) != 0)
			{
				num3 = (int)((float)num3 * cLayer5.xCoef);
			}
			if ((cLayer5.dwOptions & 2) != 0)
			{
				num4 = (int)((float)num4 * cLayer5.yCoef);
			}
		}
		num3 += cLayer5.x;
		num4 += cLayer5.y;
		if (rhFrame.colMask != null)
		{
			rhFrame.colMask.setOrigin(num3, num4);
		}
	}

	public void ohRedrawLevel(bool bRedrawTotalColMask)
	{
		rh3Scrolling |= 4;
		if (bRedrawTotalColMask)
		{
			rh3Scrolling |= 8;
		}
	}

	private void scrollLevel()
	{
		int leEditWinWidth = rhFrame.leEditWinWidth;
		int leEditWinHeight = rhFrame.leEditWinHeight;
		float num = 1f;
		float num2 = 1f;
		if (rhFrame.nLayers > 0)
		{
			CLayer cLayer = rhFrame.layers[0];
			num = cLayer.xCoef;
			num2 = cLayer.yCoef;
		}
		int num3 = rhFrame.leLastScrlX;
		int num4 = rh3DisplayX;
		if (num != 1f)
		{
			num3 = (int)((float)num3 * num);
			num4 = (int)((float)num4 * num);
		}
		int num5;
		int num6;
		if (num4 < num3)
		{
			num5 = 0;
			num6 = num3 - num4;
			rhFrame.leLastScrlX = rh3DisplayX;
		}
		else
		{
			num5 = num4 - num3;
			num6 = 0;
			if (num5 != 0)
			{
				rhFrame.leLastScrlX = rh3DisplayX;
			}
		}
		int num7 = rhFrame.leLastScrlY;
		int num8 = rh3DisplayY;
		if (num2 != 1f)
		{
			num7 = (int)((float)num7 * num2);
			num8 = (int)((float)num8 * num2);
		}
		int num9;
		int num10;
		if (num8 < num7)
		{
			num9 = 0;
			num10 = num7 - num8;
			rhFrame.leLastScrlY = rh3DisplayY;
		}
		else
		{
			num9 = num8 - num7;
			num10 = 0;
			if (num9 != 0)
			{
				rhFrame.leLastScrlY = rh3DisplayY;
			}
		}
		int num11 = leEditWinWidth - num5 - num6;
		int num12 = leEditWinHeight - num9 - num10;
		rhFrame.leX = rh3DisplayX;
		rhFrame.leY = rh3DisplayY;
		rhApp.spriteGen.activeSprite(null, 1, null);
		for (int i = 0; i < rhFrame.nLayers; i++)
		{
			CLayer cLayer2 = rhFrame.layers[i];
			if ((cLayer2.dwOptions & 0x40000) != 0)
			{
				f_ShowAllObjects(i, bShow: true);
			}
			if ((cLayer2.dwOptions & 0x20000) != 0)
			{
				f_ShowAllObjects(i, bShow: false);
			}
		}
		f_UpdateWindowPos(rhFrame.leX, rhFrame.leY);
		bool flag2;
		bool flag = (flag2 = false);
		if (num11 > leEditWinWidth / 4 && num12 > leEditWinHeight / 4)
		{
			if (num11 == leEditWinWidth && num12 == leEditWinHeight)
			{
				flag = true;
				flag2 = true;
			}
			else if (num11 > 0 && num12 > 0)
			{
				flag = true;
			}
		}
		if (!flag)
		{
			redrawLevel(18);
			return;
		}
		bool flag3 = false;
		if (num5 != 0 || num6 != 0)
		{
			if (flag2)
			{
				redrawLevel(34);
			}
			else
			{
				redrawLevel(18);
			}
			flag3 = true;
		}
		if (num9 != 0 || num10 != 0)
		{
			if (flag2)
			{
				redrawLevel(34);
			}
			else
			{
				redrawLevel(18);
			}
			flag3 = true;
		}
		if (!flag3 && rhFrame.nLayers > 0)
		{
			if ((rhFrame.layers[0].dwOptions & 0x10000) != 0)
			{
				redrawLevel(18);
			}
			else
			{
				redrawLevel(82);
			}
		}
	}

	private void updateScrollLevelPos()
	{
		float num = 1f;
		float num2 = 1f;
		if (rhFrame.nLayers > 0)
		{
			CLayer cLayer = rhFrame.layers[0];
			num = cLayer.xCoef;
			num2 = cLayer.yCoef;
		}
		int num3 = rhFrame.leLastScrlX;
		int num4 = rh3DisplayX;
		if (num != 1f)
		{
			num3 = (int)((float)num3 * num);
			num4 = (int)((float)num4 * num);
		}
		if (num4 < num3)
		{
			int num5 = 0;
			rhFrame.leLastScrlX = rh3DisplayX;
		}
		else if (num4 - num3 != 0)
		{
			rhFrame.leLastScrlX = rh3DisplayX;
		}
		int num6 = rhFrame.leLastScrlY;
		int num7 = rh3DisplayY;
		if (num2 != 1f)
		{
			num6 = (int)((float)num6 * num2);
			num7 = (int)((float)num7 * num2);
		}
		if (num7 < num6)
		{
			int num8 = 0;
			rhFrame.leLastScrlY = rh3DisplayY;
		}
		else if (num7 - num6 != 0)
		{
			rhFrame.leLastScrlY = rh3DisplayY;
		}
		rhFrame.leX = rh3DisplayX;
		rhFrame.leY = rh3DisplayY;
	}

	public void screen_Update()
	{
		int rgb = ((rhApp.frame == null) ? rhApp.gaBorderColour : rhApp.frame.leBackground);
		Color color = CServices.getColor(rgb);
		if (rhApp.parentApp == null)
		{
			rhApp.graphicsDevice.Clear(color);
		}
		else
		{
			if (!rhApp.bSubAppShown)
			{
				return;
			}
			rhApp.services.drawFilledRectangleSub(rhApp.spriteBatch, rhApp.xOffset, rhApp.yOffset, rhApp.parentWidth, rhApp.parentHeight, color, 0, 0);
		}
		if (rh3Scrolling != 0)
		{
			if ((rh3Scrolling & 4) != 0)
			{
				if (rhFrame.leX != rh3DisplayX || rhFrame.leY != rh3DisplayY)
				{
					updateScrollLevelPos();
				}
				int num = 4;
				if ((rh3Scrolling & 8) == 0 && (rhFrame.leFlags & 0x20) != 0)
				{
					num |= 0x10;
				}
				redrawLevel(num);
				rh3DisplayX = rhWindowX;
				rh3DisplayY = rhWindowY;
			}
			else if ((rh3Scrolling & 1) != 0)
			{
				if (rhFrame.leX != rh3DisplayX || rhFrame.leY != rh3DisplayY)
				{
					scrollLevel();
				}
			}
			else
			{
				redrawLevel(148);
			}
		}
		rhApp.spriteGen.spriteUpdate();
		rhApp.spriteGen.spriteDraw(rhApp.spriteBatch);
		rh3Scrolling = 0;
		if (questionObjectOn != null)
		{
			questionObjectOn.draw(rhApp.spriteBatch);
		}
		if (nSubApps != 0)
		{
			int i = 0;
			for (int j = 0; j < rhNObjects; j++)
			{
				for (; rhObjectList[i] == null; i++)
				{
				}
				CObject cObject = rhObjectList[i];
				i++;
				if (cObject.hoType == 9)
				{
					((CCCA)cObject).draw(rhApp.spriteBatch);
				}
			}
		}
		if (nControls != 0)
		{
			for (int k = 0; k < nControls; k++)
			{
				IControl control = (IControl)controls.get(k);
				control.drawControl(rhApp.spriteBatch);
			}
		}
	}

	public CObject find_HeaderObject(short hlo)
	{
		int i = 0;
		for (int j = 0; j < rhNObjects; j++)
		{
			for (; rhObjectList[i] == null; i++)
			{
			}
			if (hlo == rhObjectList[i].hoHFII)
			{
				return rhObjectList[i];
			}
			i++;
		}
		return null;
	}

	public void f_UpdateWindowPos(int newX, int newY)
	{
		short num = 0;
		rh4WindowDeltaX = newX - rhWindowX;
		if (rh4WindowDeltaX != 0)
		{
			num++;
		}
		rh4WindowDeltaY = newY - rhWindowY;
		if (rh4WindowDeltaY != 0)
		{
			num++;
		}
		if (num == 0)
		{
			for (int i = 0; i < rhFrame.nLayers; i++)
			{
				CLayer cLayer = rhFrame.layers[i];
				if (cLayer.dx != 0 || cLayer.dy != 0)
				{
					num++;
					break;
				}
			}
		}
		int num2 = rhWindowX;
		int num3 = rhWindowY;
		int num4 = rh4WindowDeltaX;
		int num5 = rh4WindowDeltaY;
		rhWindowX = newX;
		rh3XMinimum = newX - 64;
		if (rh3XMinimum < 0)
		{
			rh3XMinimum = rh3XMinimumKill;
		}
		rhWindowY = newY;
		rh3YMinimum = newY - 16;
		if (rh3YMinimum < 0)
		{
			rh3YMinimum = rh3YMinimumKill;
		}
		rh3XMaximum = newX + rh3WindowSx + 64;
		if (rh3XMaximum > rhLevelSx)
		{
			rh3XMaximum = rh3XMaximumKill;
		}
		rh3YMaximum = newY + rh3WindowSy + 16;
		if (rh3YMaximum > rhLevelSy)
		{
			rh3YMaximum = rh3YMaximumKill;
		}
		rh4FirstQuickDisplay = -1;
		rh4LastQuickDisplay = -1;
		int j = 0;
		for (int k = 0; k < rhNObjects; k++)
		{
			for (; rhObjectList[j] == null; j++)
			{
			}
			CObject cObject = rhObjectList[j];
			j++;
			if (num != 0)
			{
				if ((cObject.hoOEFlags & 0x800) != 0)
				{
					int num6 = num4;
					int num7 = num5;
					if (cObject.rom == null)
					{
						cObject.hoX += num6;
						cObject.hoY += num7;
					}
					else
					{
						num6 += cObject.hoX;
						num7 += cObject.hoY;
						cObject.rom.rmMovement.setXPosition(num6);
						cObject.rom.rmMovement.setYPosition(num7);
					}
				}
				else
				{
					int hoLayer = cObject.hoLayer;
					if (hoLayer < rhFrame.nLayers)
					{
						int num8 = num2;
						int num9 = num3;
						int num10 = newX;
						int num11 = newY;
						CLayer cLayer2 = rhFrame.layers[hoLayer];
						if ((cLayer2.dwOptions & 1) != 0)
						{
							num8 = (int)(cLayer2.xCoef * (float)num8);
							num10 = (int)(cLayer2.xCoef * (float)num10);
						}
						if ((cLayer2.dwOptions & 2) != 0)
						{
							num9 = (int)(cLayer2.yCoef * (float)num9);
							num11 = (int)(cLayer2.yCoef * (float)num11);
						}
						int num12 = cObject.hoX + num8 - num10 + num4 - cLayer2.dx;
						int num13 = cObject.hoY + num9 - num11 + num5 - cLayer2.dy;
						if ((cObject.hoOEFlags & 0x10) == 0)
						{
							cObject.hoX = num12;
							cObject.hoY = num13;
						}
						else
						{
							cObject.rom.rmMovement.setXPosition(num12);
							cObject.rom.rmMovement.setYPosition(num13);
						}
					}
				}
				if ((cObject.hoOEFlags & 2) == 0)
				{
					cObject.modif();
				}
			}
			else if ((cObject.hoOEFlags & 2) == 0)
			{
				cObject.display();
			}
		}
	}

	public void f_ShowAllObjects(int nLayer, bool bShow)
	{
		int i = 0;
		for (int j = 0; j < rhNObjects; j++)
		{
			for (; rhObjectList[i] == null; i++)
			{
			}
			CObject cObject = rhObjectList[i];
			i++;
			if ((nLayer != cObject.hoLayer && nLayer != -1) || cObject.ros == null)
			{
				continue;
			}
			if (cObject.roc.rcSprite != null)
			{
				rhApp.spriteGen.activeSprite(cObject.roc.rcSprite, 1, null);
			}
			if (bShow)
			{
				if ((cObject.ros.rsFlags & 0x20) != 0)
				{
					CLayer cLayer = rhFrame.layers[cObject.hoLayer];
					int dwOptions = cLayer.dwOptions;
					cLayer.dwOptions = (cLayer.dwOptions & -393217) | 0x10;
					cObject.ros.obShow();
					cLayer.dwOptions = dwOptions;
				}
			}
			else
			{
				cObject.ros.obHide();
			}
			cObject.ros.rsFlash = 0;
		}
	}

	public void setDisplay(int x, int y, int nLayer, int flags)
	{
		x -= rh3WindowSx / 2;
		y -= rh3WindowSy / 2;
		float num = x;
		float num2 = y;
		if (nLayer != -1 && nLayer < rhFrame.nLayers)
		{
			CLayer cLayer = rhFrame.layers[nLayer];
			if (cLayer.xCoef > 1f)
			{
				float num3 = num - (float)rhWindowX;
				num3 /= cLayer.xCoef;
				num = (float)rhWindowX + num3;
			}
			if (cLayer.yCoef > 1f)
			{
				float num4 = num2 - (float)rhWindowY;
				num4 /= cLayer.yCoef;
				num2 = (float)rhWindowY + num4;
			}
		}
		x = (int)num;
		y = (int)num2;
		if (x < 0)
		{
			x = 0;
		}
		if (y < 0)
		{
			y = 0;
		}
		int num5 = x + rh3WindowSx;
		int num6 = y + rh3WindowSy;
		if (num5 > rhLevelSx)
		{
			num5 = rhLevelSx - rh3WindowSx;
			if (num5 < 0)
			{
				num5 = 0;
			}
			x = num5;
		}
		if (num6 > rhLevelSy)
		{
			num6 = rhLevelSy - rh3WindowSy;
			if (num6 < 0)
			{
				num6 = 0;
			}
			y = num6;
		}
		if ((flags & 1) != 0 && x != rhWindowX)
		{
			rh3DisplayX = x;
			rh3Scrolling |= 1;
		}
		if ((flags & 2) != 0 && y != rhWindowY)
		{
			rh3DisplayY = y;
			rh3Scrolling |= 1;
		}
	}

	public void y_Ladder_Reset(int nLayer)
	{
		if (nLayer >= 0 && nLayer < rhFrame.nLayers)
		{
			CLayer cLayer = rhFrame.layers[nLayer];
			cLayer.pLadders = null;
		}
	}

	public void y_Ladder_Add(int nLayer, int x1, int y1, int x2, int y2)
	{
		if (nLayer >= 0 && nLayer < rhFrame.nLayers)
		{
			CLayer cLayer = rhFrame.layers[nLayer];
			CRect cRect = new CRect();
			cRect.left = Math.Min(x1, x2);
			cRect.top = Math.Min(y1, y2);
			cRect.right = Math.Max(x1, x2);
			cRect.bottom = Math.Max(y1, y2);
			if (cLayer.pLadders == null)
			{
				cLayer.pLadders = new CArrayList();
			}
			cLayer.pLadders.add(cRect);
		}
	}

	public void y_Ladder_Sub(int nLayer, int x1, int y1, int x2, int y2)
	{
		if (nLayer < 0 || nLayer >= rhFrame.nLayers)
		{
			return;
		}
		CLayer cLayer = rhFrame.layers[nLayer];
		if (cLayer.pLadders == null)
		{
			return;
		}
		CRect cRect = new CRect();
		cRect.left = Math.Min(x1, x2);
		cRect.top = Math.Min(y1, y2);
		cRect.right = Math.Max(x1, x2);
		cRect.bottom = Math.Max(y1, y2);
		for (int i = 0; i < cLayer.pLadders.size(); i++)
		{
			CRect cRect2 = (CRect)cLayer.pLadders.get(i);
			if (cRect2.intersectRect(cRect))
			{
				cLayer.pLadders.remove(i);
				i--;
			}
		}
	}

	public CRect y_GetLadderAt(int nLayer, int x, int y)
	{
		int i;
		int num;
		if (nLayer == -1)
		{
			i = 0;
			num = rhFrame.nLayers;
		}
		else
		{
			i = nLayer;
			num = nLayer + 1;
		}
		for (; i < num; i++)
		{
			CLayer cLayer = rhFrame.layers[i];
			if (cLayer.pLadders == null)
			{
				continue;
			}
			for (int j = 0; j < cLayer.pLadders.size(); j++)
			{
				CRect cRect = (CRect)cLayer.pLadders.get(j);
				if (x >= cRect.left && y >= cRect.top && x < cRect.right && y < cRect.bottom)
				{
					return cRect;
				}
			}
		}
		return null;
	}

	public CRect y_GetLadderAt_Absolute(int nLayer, int x, int y)
	{
		x -= rhFrame.leX;
		y -= rhFrame.leY;
		return y_GetLadderAt(nLayer, x, y);
	}

	public void activeToBackdrop(CObject hoPtr, int nTypeObst, bool bTrueObject)
	{
		CBkd2 cBkd = new CBkd2();
		cBkd.img = hoPtr.roc.rcImage;
		CImage imageFromHandle = rhApp.imageBank.getImageFromHandle(cBkd.img);
		cBkd.loHnd = 0;
		cBkd.oiHnd = 0;
		cBkd.x = hoPtr.hoX - imageFromHandle.xSpot;
		cBkd.y = hoPtr.hoY - imageFromHandle.ySpot;
		cBkd.nLayer = (short)hoPtr.hoLayer;
		cBkd.obstacleType = (short)nTypeObst;
		cBkd.colMode = 1;
		if ((hoPtr.ros.rsCreaFlags & 0x100) != 0)
		{
			cBkd.colMode = 0;
		}
		for (int i = 0; i < 4; i++)
		{
			cBkd.pSpr[i] = null;
		}
		cBkd.inkEffect = hoPtr.ros.rsEffect;
		cBkd.inkEffectParam = hoPtr.ros.rsEffectParam;
		addBackdrop2(cBkd);
	}

	public void addBackdrop2(CBkd2 toadd)
	{
		if (toadd.nLayer < 0 || toadd.nLayer >= rhFrame.nLayers)
		{
			return;
		}
		CLayer cLayer = rhFrame.layers[toadd.nLayer];
		CBkd2 cBkd;
		if (cLayer.pBkd2 != null)
		{
			for (int i = 0; i < cLayer.pBkd2.size(); i++)
			{
				cBkd = (CBkd2)cLayer.pBkd2.get(i);
				if (cBkd.x != toadd.x || cBkd.y != toadd.y || cBkd.nLayer != toadd.nLayer || cBkd.img != toadd.img || (cBkd.inkEffect & 0xFFF) != 0)
				{
					continue;
				}
				if (i != cLayer.pBkd2.size() - 1)
				{
					for (int j = 0; j < 4; j++)
					{
						if (cBkd.pSpr[j] != null)
						{
							rhApp.spriteGen.moveSpriteToFront(cBkd.pSpr[j]);
						}
					}
					cLayer.pBkd2.remove(i);
					cLayer.pBkd2.add(cBkd);
				}
				cBkd.colMode = toadd.colMode;
				cBkd.obstacleType = toadd.obstacleType;
				if (cBkd.inkEffect == toadd.inkEffect && cBkd.inkEffectParam == toadd.inkEffectParam)
				{
					return;
				}
				cBkd.inkEffect = toadd.inkEffect;
				cBkd.inkEffectParam = toadd.inkEffectParam;
				for (int k = 0; k < 4; k++)
				{
					if (cBkd.pSpr[k] != null)
					{
						rhApp.spriteGen.modifSpriteEffect(cBkd.pSpr[k], cBkd.inkEffect, cBkd.inkEffectParam);
					}
				}
				return;
			}
		}
		else
		{
			cLayer.pBkd2 = new CArrayList();
		}
		int num = cLayer.pBkd2.size();
		cLayer.pBkd2.add(toadd);
		cBkd = toadd;
		CRect cRect = new CRect();
		int num2 = rhFrame.leX;
		int num3 = rhFrame.leY;
		bool flag = (cLayer.dwOptions & 0x20) != 0;
		bool flag2 = (cLayer.dwOptions & 0x40) != 0;
		bool flag3 = false;
		if (flag || flag2)
		{
			flag3 = true;
		}
		int leWidth = rhFrame.leWidth;
		int leHeight = rhFrame.leHeight;
		if ((cLayer.dwOptions & 3) != 0)
		{
			if ((cLayer.dwOptions & 1) != 0)
			{
				num2 = (int)((float)num2 * cLayer.xCoef);
			}
			if ((cLayer.dwOptions & 2) != 0)
			{
				num3 = (int)((float)num3 * cLayer.yCoef);
			}
		}
		num2 += cLayer.x;
		num3 += cLayer.y;
		if (flag)
		{
			num2 %= leWidth;
		}
		if (flag2)
		{
			num3 %= leHeight;
		}
		if ((cLayer.dwOptions & 0x20010) != 16)
		{
			return;
		}
		bool flag4 = (cLayer.dwOptions & 4) == 0;
		uint num4 = 0u;
		int num5 = 0;
		do
		{
			int num6 = num5;
			cRect.left = cBkd.x - num2;
			cRect.top = cBkd.y - num3;
			int num7 = rhFrame.leEditWinWidth - 1;
			int num8 = rhFrame.leEditWinHeight - 1;
			short img = cBkd.img;
			CImage imageFromHandle = rhApp.imageBank.getImageFromHandle(img);
			if (imageFromHandle != null)
			{
				cRect.right = cRect.left + imageFromHandle.width;
				cRect.bottom = cRect.top + imageFromHandle.height;
			}
			else
			{
				cRect.right = cRect.left + 1;
				cRect.bottom = cRect.top + 1;
			}
			if (flag3)
			{
				switch (num5)
				{
				case 0:
					if (flag && (cRect.left < 0 || cRect.right > leWidth))
					{
						if (flag2 && (cRect.top < 0 || cRect.bottom > leHeight))
						{
							num5 = 3;
							num4 |= 7;
						}
						else
						{
							num5 = 1;
							num4 |= 1;
						}
					}
					else if (flag2 && (cRect.top < 0 || cRect.bottom > leHeight))
					{
						num5 = 2;
						num4 |= 2;
					}
					if ((num4 & 1) == 0 && cBkd.pSpr[1] != null)
					{
						rhApp.spriteGen.delSprite(cBkd.pSpr[1]);
						cBkd.pSpr[1] = null;
					}
					if ((num4 & 2) == 0 && cBkd.pSpr[2] != null)
					{
						rhApp.spriteGen.delSprite(cBkd.pSpr[2]);
						cBkd.pSpr[2] = null;
					}
					if ((num4 & 4) == 0 && cBkd.pSpr[3] != null)
					{
						rhApp.spriteGen.delSprite(cBkd.pSpr[3]);
						cBkd.pSpr[3] = null;
					}
					break;
				case 1:
					if (cRect.left < 0)
					{
						int num15 = leWidth;
						cRect.left += num15;
						cRect.right += num15;
					}
					else if (cRect.right > leWidth)
					{
						int num16 = leWidth;
						cRect.left -= num16;
						cRect.right -= num16;
					}
					num4 &= 0xFFFFFFFEu;
					num5 = 0;
					if ((num4 & 2) != 0)
					{
						num5 = 2;
					}
					break;
				case 2:
					if (cRect.top < 0)
					{
						int num13 = leHeight;
						cRect.top += num13;
						cRect.bottom += num13;
					}
					else if (cRect.bottom > leHeight)
					{
						int num14 = leHeight;
						cRect.top -= num14;
						cRect.bottom -= num14;
					}
					num4 &= 0xFFFFFFFDu;
					num5 = 0;
					if ((num4 & 1) != 0)
					{
						num5 = 1;
					}
					break;
				case 3:
					if (cRect.left < 0)
					{
						int num9 = leWidth;
						cRect.left += num9;
						cRect.right += num9;
					}
					else if (cRect.right > leWidth)
					{
						int num10 = leWidth;
						cRect.left -= num10;
						cRect.right -= num10;
					}
					if (cRect.top < 0)
					{
						int num11 = leHeight;
						cRect.top += num11;
						cRect.bottom += num11;
					}
					else if (cRect.bottom > leHeight)
					{
						int num12 = leHeight;
						cRect.top -= num12;
						cRect.bottom -= num12;
					}
					num4 &= 0xFFFFFFFBu;
					num5 = 2;
					break;
				}
			}
			if (rhFrame.colMask != null && cBkd.nLayer == 0 && cBkd.colMode != 4 && cRect.right >= -96 && cRect.bottom >= -16)
			{
				CMask cMask = null;
				cRect.left += num2;
				cRect.top += num3;
				cRect.right += num2;
				cRect.bottom += num3;
				int val = 0;
				if (cBkd.colMode == 1)
				{
					val = 3;
				}
				CImage imageFromHandle2 = rhApp.imageBank.getImageFromHandle(toadd.img);
				cMask = imageFromHandle2.getMask(0, 0, 1f, 1f);
				if (cBkd.obstacleType == 0)
				{
					rhFrame.colMask.fillRectangle(cRect.left, cRect.top, cRect.right - 1, cRect.bottom - 1, val);
				}
				else if (cMask == null)
				{
					rhFrame.colMask.fillRectangle(cRect.left, cRect.top, cRect.right - 1, cRect.bottom - 1, val);
				}
				else
				{
					rhFrame.colMask.orMask(cMask, cRect.left, cRect.top, 3, val);
				}
				if (cBkd.colMode == 2)
				{
					if (cBkd.obstacleType == 0)
					{
						rhFrame.colMask.fillRectangle(cRect.left, cRect.top, cRect.right - 1, Math.Min(cRect.top + 6, cRect.bottom) - 1, 2);
					}
					else if (cMask == null)
					{
						rhFrame.colMask.fillRectangle(cRect.left, cRect.top, cRect.right - 1, Math.Min(cRect.top + 6, cRect.bottom) - 1, 2);
					}
					else
					{
						rhFrame.colMask.orPlatformMask(cMask, cRect.left, cRect.top);
					}
				}
				cRect.left -= num2;
				cRect.top -= num3;
				cRect.right -= num2;
				cRect.bottom -= num3;
			}
			if (cRect.left >= num7 + 64 + 32 || cRect.top >= num8 + 16)
			{
				continue;
			}
			int obstacleType = cBkd.obstacleType;
			if (obstacleType == 3)
			{
				y_Ladder_Add(cBkd.nLayer, cRect.left, cRect.top, cRect.right, cRect.bottom);
			}
			if (cRect.left > num7 || cRect.top > num8 || cRect.right < 0 || cRect.bottom < 0)
			{
				continue;
			}
			uint num17 = 524296u;
			if (!flag4)
			{
				num17 |= 0x200;
			}
			if (cBkd.nLayer > 0)
			{
				if (obstacleType == 1)
				{
					num17 |= 0x10001;
				}
				if (obstacleType == 2)
				{
					num17 |= 0x20001;
				}
			}
			rhApp.imageBank.getImageFromHandle(toadd.img);
			int left = cRect.left;
			int top = cRect.top;
			num17 |= 0x400000;
			cBkd.pSpr[num6] = rhApp.spriteGen.addSprite(left, top, img, cBkd.nLayer, 268435456 + num * 4 + num6, 0, num17, null);
			rhApp.spriteGen.modifSpriteEffect(cBkd.pSpr[num6], cBkd.inkEffect, cBkd.inkEffectParam);
		}
		while (num4 != 0);
	}

	public void deleteAllBackdrop2(int nLayer)
	{
		if (nLayer < 0 || nLayer >= rhFrame.nLayers)
		{
			return;
		}
		CLayer cLayer = rhFrame.layers[nLayer];
		if (cLayer.pBkd2 == null)
		{
			return;
		}
		for (int i = 0; i < cLayer.pBkd2.size(); i++)
		{
			CBkd2 cBkd = (CBkd2)cLayer.pBkd2.get(i);
			for (int j = 0; j < 4; j++)
			{
				if (cBkd.pSpr[j] != null)
				{
					rhApp.spriteGen.delSprite(cBkd.pSpr[j]);
					cBkd.pSpr[j] = null;
				}
			}
		}
		cLayer.pBkd2 = null;
		cLayer.dwOptions |= 65536;
		rh3Scrolling |= 2;
	}

	public void deleteBackdrop2At(int nLayer, int x, int y, bool bFineDetection)
	{
		if (nLayer < 0 || nLayer >= rhFrame.nLayers)
		{
			return;
		}
		CLayer cLayer = rhFrame.layers[nLayer];
		if (cLayer.pBkd2 == null)
		{
			return;
		}
		bool flag = false;
		bool flag2 = (cLayer.dwOptions & 0x20) != 0;
		bool flag3 = (cLayer.dwOptions & 0x40) != 0;
		bool flag4 = flag2 | flag3;
		int leWidth = rhFrame.leWidth;
		int leHeight = rhFrame.leHeight;
		int num = rhFrame.leX;
		int num2 = rhFrame.leY;
		if ((cLayer.dwOptions & 3) != 0)
		{
			if ((cLayer.dwOptions & 1) != 0)
			{
				num = (int)((float)num * cLayer.xCoef);
			}
			if ((cLayer.dwOptions & 2) != 0)
			{
				num2 = (int)((float)num2 * cLayer.yCoef);
			}
		}
		num += cLayer.x;
		num2 += cLayer.y;
		if (flag2)
		{
			num %= leWidth;
		}
		if (flag3)
		{
			num2 %= leHeight;
		}
		uint num3 = 0u;
		int num4 = 0;
		for (int i = 0; i < cLayer.pBkd2.size(); i++)
		{
			CBkd2 cBkd = (CBkd2)cLayer.pBkd2.get(i);
			if (cBkd.nLayer != nLayer)
			{
				continue;
			}
			bool flag5 = false;
			CRect cRect = new CRect();
			bool flag6 = cBkd.colMode == 0;
			cRect.left = cBkd.x - num;
			cRect.top = cBkd.y - num2;
			CImage imageFromHandle = rhApp.imageBank.getImageFromHandle(cBkd.img);
			if (imageFromHandle != null)
			{
				cRect.right = cRect.left + imageFromHandle.width;
				cRect.bottom = cRect.top + imageFromHandle.height;
			}
			else
			{
				cRect.right = cRect.left + 1;
				cRect.bottom = cRect.top + 1;
			}
			if (flag4)
			{
				switch (num4)
				{
				case 0:
					if (flag2 && (cRect.left < 0 || cRect.right > leWidth))
					{
						if (flag3 && (cRect.top < 0 || cRect.bottom > leHeight))
						{
							num4 = 3;
							num3 |= 7;
						}
						else
						{
							num4 = 1;
							num3 |= 1;
						}
					}
					else if (flag3 && (cRect.top < 0 || cRect.bottom > leHeight))
					{
						num4 = 2;
						num3 |= 2;
					}
					break;
				case 1:
					if (cRect.left < 0)
					{
						int num11 = leWidth;
						cRect.left += num11;
						cRect.right += num11;
					}
					else if (cRect.right > leWidth)
					{
						int num12 = leWidth;
						cRect.left -= num12;
						cRect.right -= num12;
					}
					num3 &= 0xFFFFFFFEu;
					num4 = 0;
					if ((num3 & 2) != 0)
					{
						num4 = 2;
					}
					break;
				case 2:
					if (cRect.top < 0)
					{
						int num9 = leHeight;
						cRect.top += num9;
						cRect.bottom += num9;
					}
					else if (cRect.bottom > leHeight)
					{
						int num10 = leHeight;
						cRect.top -= num10;
						cRect.bottom -= num10;
					}
					num3 &= 0xFFFFFFFDu;
					num4 = 0;
					if ((num3 & 1) != 0)
					{
						num4 = 1;
					}
					break;
				case 3:
					if (cRect.left < 0)
					{
						int num5 = leWidth;
						cRect.left += num5;
						cRect.right += num5;
					}
					else if (cRect.right > leWidth)
					{
						int num6 = leWidth;
						cRect.left -= num6;
						cRect.right -= num6;
					}
					if (cRect.top < 0)
					{
						int num7 = leHeight;
						cRect.top += num7;
						cRect.bottom += num7;
					}
					else if (cRect.bottom > leHeight)
					{
						int num8 = leHeight;
						cRect.top -= num8;
						cRect.bottom -= num8;
					}
					num3 &= 0xFFFFFFFBu;
					num4 = 2;
					break;
				}
			}
			if (x >= cRect.left && y >= cRect.top && x < cRect.right && y < cRect.bottom)
			{
				if (!bFineDetection || flag6)
				{
					flag5 = true;
				}
				else
				{
					CMask mask = rhApp.imageBank.getImageFromHandle(cBkd.img).getMask(0, 0, 1f, 1f);
					if (mask != null && mask.testPoint(x - cRect.left, y - cRect.top))
					{
						flag5 = true;
					}
				}
			}
			if (flag5)
			{
				flag = true;
				for (int j = 0; j < 4; j++)
				{
					if (cBkd.pSpr[j] != null)
					{
						rhApp.spriteGen.delSprite(cBkd.pSpr[j]);
						cBkd.pSpr[j] = null;
					}
				}
				cLayer.pBkd2.remove(i);
				num3 = 0u;
				i--;
			}
			if (num3 != 0)
			{
				i--;
			}
		}
		if (flag)
		{
			cLayer.dwOptions |= 65536;
			rh3Scrolling |= 2;
		}
	}

	public void displayBkd2Layer(CLayer pLayer, int nLayer, int flags, int x2edit, int y2edit, bool flgColMaskEmpty)
	{
		CRect cRect = new CRect();
		bool flag = (rhFrame.leFlags & 0x20) != 0;
		bool flag2 = (flags & 0x10) == 0;
		int num = rhFrame.leX;
		int num2 = rhFrame.leY;
		bool flag3 = (pLayer.dwOptions & 0x20) != 0;
		bool flag4 = (pLayer.dwOptions & 0x40) != 0;
		bool flag5 = flag3 | flag4;
		int leWidth = rhFrame.leWidth;
		int leHeight = rhFrame.leHeight;
		if ((pLayer.dwOptions & 3) != 0)
		{
			if ((pLayer.dwOptions & 1) != 0)
			{
				num = (int)((float)num * pLayer.xCoef);
			}
			if ((pLayer.dwOptions & 2) != 0)
			{
				num2 = (int)((float)num2 * pLayer.yCoef);
			}
		}
		num += pLayer.x;
		num2 += pLayer.y;
		if (flag3)
		{
			num %= leWidth;
		}
		if (flag4)
		{
			num2 %= leHeight;
		}
		if ((pLayer.dwOptions & 0x20000) != 0)
		{
			for (int i = 0; i < pLayer.pBkd2.size(); i++)
			{
				CBkd2 cBkd = (CBkd2)pLayer.pBkd2.get(i);
				for (int j = 0; j < 4; j++)
				{
					if (cBkd.pSpr[j] != null)
					{
						rhApp.spriteGen.delSprite(cBkd.pSpr[j]);
						cBkd.pSpr[j] = null;
					}
				}
			}
		}
		if ((pLayer.dwOptions & 0x20000) != 0)
		{
			return;
		}
		bool flag6 = (pLayer.dwOptions & 4) == 0;
		uint num3 = 0u;
		int num4 = 0;
		for (int i = 0; i < pLayer.pBkd2.size(); i++)
		{
			CBkd2 cBkd = (CBkd2)pLayer.pBkd2.get(i);
			int num5 = num4;
			cRect.left = cBkd.x - num;
			cRect.top = cBkd.y - num2;
			if (!flag && !flag5 && (cRect.left >= x2edit + 64 + 32 || cRect.top >= y2edit + 16))
			{
				if (cBkd.pSpr[num5] != null)
				{
					rhApp.spriteGen.delSprite(cBkd.pSpr[num5]);
					cBkd.pSpr[num5] = null;
				}
				continue;
			}
			short img = cBkd.img;
			CImage imageFromHandle = rhApp.imageBank.getImageFromHandle(img);
			if (imageFromHandle != null)
			{
				cRect.right = cRect.left + imageFromHandle.width;
				cRect.bottom = cRect.top + imageFromHandle.height;
			}
			else
			{
				cRect.right = cRect.left + 1;
				cRect.bottom = cRect.top + 1;
			}
			if (flag5)
			{
				switch (num4)
				{
				case 0:
					if (flag3 && (cRect.left < 0 || cRect.right > leWidth))
					{
						if (flag4 && (cRect.top < 0 || cRect.bottom > leHeight))
						{
							num4 = 3;
							num3 |= 7;
						}
						else
						{
							num4 = 1;
							num3 |= 1;
						}
					}
					else if (flag4 && (cRect.top < 0 || cRect.bottom > leHeight))
					{
						num4 = 2;
						num3 |= 2;
					}
					if ((num3 & 1) == 0 && cBkd.pSpr[1] != null)
					{
						rhApp.spriteGen.delSprite(cBkd.pSpr[1]);
						cBkd.pSpr[1] = null;
					}
					if ((num3 & 2) == 0 && cBkd.pSpr[2] != null)
					{
						rhApp.spriteGen.delSprite(cBkd.pSpr[2]);
						cBkd.pSpr[2] = null;
					}
					if ((num3 & 4) == 0 && cBkd.pSpr[3] != null)
					{
						rhApp.spriteGen.delSprite(cBkd.pSpr[3]);
						cBkd.pSpr[3] = null;
					}
					break;
				case 1:
					if (cRect.left < 0)
					{
						int num12 = leWidth;
						cRect.left += num12;
						cRect.right += num12;
					}
					else if (cRect.right > leWidth)
					{
						int num13 = leWidth;
						cRect.left -= num13;
						cRect.right -= num13;
					}
					num3 &= 0xFFFFFFFEu;
					num4 = 0;
					if ((num3 & 2) != 0)
					{
						num4 = 2;
					}
					break;
				case 2:
					if (cRect.top < 0)
					{
						int num10 = leHeight;
						cRect.top += num10;
						cRect.bottom += num10;
					}
					else if (cRect.bottom > leHeight)
					{
						int num11 = leHeight;
						cRect.top -= num11;
						cRect.bottom -= num11;
					}
					num3 &= 0xFFFFFFFDu;
					num4 = 0;
					if ((num3 & 1) != 0)
					{
						num4 = 1;
					}
					break;
				case 3:
					if (cRect.left < 0)
					{
						int num6 = leWidth;
						cRect.left += num6;
						cRect.right += num6;
					}
					else if (cRect.right > leWidth)
					{
						int num7 = leWidth;
						cRect.left -= num7;
						cRect.right -= num7;
					}
					if (cRect.top < 0)
					{
						int num8 = leHeight;
						cRect.top += num8;
						cRect.bottom += num8;
					}
					else if (cRect.bottom > leHeight)
					{
						int num9 = leHeight;
						cRect.top -= num9;
						cRect.bottom -= num9;
					}
					num3 &= 0xFFFFFFFBu;
					num4 = 2;
					break;
				}
			}
			int obstacleType = cBkd.obstacleType;
			bool flag7 = cBkd.colMode == 0;
			if (obstacleType == 3)
			{
				y_Ladder_Add(nLayer, cRect.left, cRect.top, cRect.right, cRect.bottom);
				flag7 = true;
			}
			if (nLayer == 0 && flag2 && obstacleType != 4 && (flag || (cRect.right >= -96 && cRect.bottom >= -16)))
			{
				if (flag)
				{
					cRect.left += num;
					cRect.top += num2;
					cRect.right += num;
					cRect.bottom += num2;
				}
				int val = 0;
				if (obstacleType == 1)
				{
					val = 3;
					flgColMaskEmpty = false;
				}
				if (!flgColMaskEmpty)
				{
					if (flag7)
					{
						rhFrame.colMask.fillRectangle(cRect.left, cRect.top, cRect.right, cRect.bottom, val);
					}
					else
					{
						CMask mask = rhApp.imageBank.getImageFromHandle(img).getMask(0, 0, 1f, 1f);
						rhFrame.colMask.orMask(mask, cRect.left, cRect.top, 3, val);
					}
				}
				if (obstacleType == 2)
				{
					flgColMaskEmpty = false;
					if (flag7)
					{
						rhFrame.colMask.fillRectangle(cRect.left, cRect.top, cRect.right, Math.Min(cRect.top + 6, cRect.bottom), 2);
					}
					else
					{
						CMask mask = rhApp.imageBank.getImageFromHandle(img).getMask(0, 0, 1f, 1f);
						rhFrame.colMask.orPlatformMask(mask, cRect.left, cRect.top);
					}
				}
				if (flag)
				{
					cRect.left -= num;
					cRect.top -= num2;
					cRect.right -= num;
					cRect.bottom -= num2;
				}
			}
			if (cRect.left <= x2edit && cRect.top <= y2edit && cRect.right >= 0 && cRect.bottom >= 0)
			{
				uint num14 = 4718600u;
				if (!flag6)
				{
					num14 |= 0x200;
				}
				if (obstacleType == 1)
				{
					num14 |= 0x10001;
				}
				if (obstacleType == 2)
				{
					num14 |= 0x20001;
				}
				if (cBkd.pSpr[num5] == null)
				{
					cBkd.pSpr[num5] = rhApp.spriteGen.addSprite(cRect.left, cRect.top, img, cBkd.nLayer, 268435456 + i * 4 + num5, 0, num14, null);
					rhApp.spriteGen.modifSpriteEffect(cBkd.pSpr[num5], cBkd.inkEffect, cBkd.inkEffectParam);
				}
				else
				{
					rhApp.spriteGen.modifSprite(cBkd.pSpr[num5], cRect.left, cRect.top, img);
				}
			}
			else if (cBkd.pSpr[num5] != null)
			{
				rhApp.spriteGen.delSprite(cBkd.pSpr[num5]);
				cBkd.pSpr[num5] = null;
			}
			if (num3 != 0)
			{
				i--;
			}
		}
	}

	public void f_InitLoop()
	{
		rhTimerFPSOld = (rhTimerOld = rhApp.timer);
		rhTimer = 0L;
		rhLoopCount = 0;
		rh4LoopTheoric = 0;
		rhVBLOld = rhApp.newGetCptVbl() - 1;
		rh4VBLDelta = 0;
		rhQuit = 0;
		rhQuitBis = 0;
		rhDestroyPos = 0;
		for (int i = 0; i < (rhMaxObjects + 31) / 32; i++)
		{
			rhDestroyList[i] = 0;
		}
		rh3WindowSx = rhFrame.leEditWinWidth;
		rh3WindowSy = rhFrame.leEditWinHeight;
		rh3XMinimumKill = -480;
		rh3YMinimumKill = -300;
		rh3XMaximumKill = rhLevelSx + 480;
		rh3YMaximumKill = rhLevelSy + 300;
		int num = (rh3DisplayX = rhWindowX) - 64;
		if (num < 0)
		{
			num = rh3XMinimumKill;
		}
		rh3XMinimum = num;
		int num2 = (rh3DisplayY = rhWindowY) - 16;
		if (num2 < 0)
		{
			num2 = rh3YMinimumKill;
		}
		rh3YMinimum = num2;
		int num3 = rhWindowX;
		num3 += rh3WindowSx + 64;
		if (num3 > rhLevelSx)
		{
			num3 = rh3XMaximumKill;
		}
		rh3XMaximum = num3;
		int num4 = rhWindowY;
		num4 += rh3WindowSy + 16;
		if (num4 > rhLevelSy)
		{
			num4 = rh3YMaximumKill;
		}
		rh3YMaximum = num4;
		rh3Scrolling = 0;
		rh4DoUpdate = 0;
		rh4EventCount = 0;
		rh4TimeOut = 0L;
		rh2PauseCompteur = 0;
		rh4FakeKey = 0;
		for (int i = 0; i < 4; i++)
		{
			rhPlayer[i] = 0;
			rh2OldPlayer[i] = 0;
			rh2InputMask[i] = byte.MaxValue;
		}
		rh2MouseKeys = 0;
		oldMouseKey = -1;
		toucheID = -1;
		mouseKeyTime = 0L;
		if (rhMouseUsed != 0)
		{
			rh4MouseXCenter = rhApp.gaCxWin / 2;
			rh4MouseYCenter = rhApp.gaCyWin / 2;
			Mouse.SetPosition(rh4MouseXCenter, rh4MouseYCenter);
		}
		rhEvtProg.rh2ActionEndRoutine = false;
		rh4OnCloseCount = -1;
		rh4EndOfPause = -1;
		rhEvtProg.rh4CheckDoneInstart = false;
		rh4PauseKey = Keys.None;
		bCheckResume = false;
		rhApp.soundPlayer.reset();
		for (int i = 0; i < 10; i++)
		{
			rh4FrameRateArray[i] = 20;
		}
		rh4FrameRatePos = 0;
		if (rhEvtProg.bTestAllKeys)
		{
			int i = 0;
			do
			{
				if (keyboardState.IsKeyDown(CKeyConvert.xnaKeys[i]))
				{
					bAnyKeyDown = true;
					break;
				}
				i++;
			}
			while (CKeyConvert.pcKeys[i] >= 0);
		}
		rhJoystickMask = byte.MaxValue;
	}

	public void handleFrameRaate()
	{
		long timer = rhApp.timer;
		long num = timer - rhTimerFPSOld;
		rhTimerFPSOld = timer;
		rh4FrameRateArray[rh4FrameRatePos] = (int)num;
		rh4FrameRatePos++;
		if (rh4FrameRatePos >= 10)
		{
			rh4FrameRatePos = 0;
		}
	}

	public int f_GameLoop()
	{
		keyboardState = Keyboard.GetState();
		if (rh2PauseCompteur != 0)
		{
			if (questionObjectOn != null)
			{
				questionObjectOn.handleQuestion();
			}
			return 0;
		}
		rhApp.soundPlayer.checkSounds();
		long timer = rhApp.timer;
		long num = timer - rhTimerOld;
		long num2 = rhTimer;
		rhTimer = num;
		num -= num2;
		rhTimerDelta = (int)num;
		rh4TimeOut += num;
		rhLoopCount++;
		rh4MvtTimerCoef = (double)rhTimerDelta * (double)rhFrame.m_dwMvtTimerBase / 1000.0;
		for (int i = 0; i < 4; i++)
		{
			rh2OldPlayer[i] = rhPlayer[i];
		}
		joyTest();
		byte b = 0;
		if (rhMouseUsed != 0)
		{
			rh2MouseX = mouseState.X - rh4MouseXCenter;
			rh2MouseY = mouseState.Y - rh4MouseYCenter;
			if (rh2MouseX != 0 || rh2MouseY != 0)
			{
				Mouse.SetPosition(rh4MouseXCenter, rh4MouseYCenter);
			}
			b = 0;
			if ((rh2MouseKeys & 1) != 0)
			{
				b |= 0x10;
			}
			if ((rh2MouseKeys & 2) != 0)
			{
				b |= 0x20;
			}
			byte b2 = rhMouseUsed;
			for (int i = 0; i < rhNPlayers; i++)
			{
				if ((b2 & 1) != 0)
				{
					byte b3 = (byte)(rhPlayer[i] & 0xCF);
					b3 |= b;
					rhPlayer[i] = b3;
				}
				b2 >>= 1;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			byte b4 = (byte)(rhPlayer[i] & plMasks[rhNPlayers * 4 + i]);
			b4 &= rh2InputMask[i];
			rhPlayer[i] = b4;
			b4 ^= rh2OldPlayer[i];
			rh2NewPlayer[i] = b4;
			if (b4 == 0)
			{
				continue;
			}
			if (!bMouseControlled && i == 0)
			{
				newKey();
			}
			b4 &= rhPlayer[i];
			if ((b4 & 0xF0) != 0)
			{
				rhEvtProg.rhCurOi = (short)i;
				if ((b4 & 0xF0) != 0)
				{
					rhEvtProg.rhCurParam0 = b4;
					rhEvtProg.handle_GlobalEvents(-196615);
				}
				if ((b4 & 0xF) != 0)
				{
					rhEvtProg.rhCurParam0 = b4;
					rhEvtProg.handle_GlobalEvents(-196615);
				}
			}
			else
			{
				int num3 = rhEvtProg.listPointers[rhEvtProg.rhEvents[7] + 4];
				if (num3 != 0)
				{
					rhEvtProg.rhCurParam0 = b4;
					rhEvtProg.computeEventList(num3, null);
				}
			}
		}
		if (rhNObjects != 0)
		{
			int num4 = rhNObjects;
			int j = 0;
			do
			{
				for (rh4ObjectAddCreate = 0; rhObjectList[j] == null; j++)
				{
				}
				CObject cObject = rhObjectList[j];
				cObject.hoPrevNoRepeat = cObject.hoBaseNoRepeat;
				cObject.hoBaseNoRepeat = null;
				if (cObject.hoCallRoutine)
				{
					rh4ObjectCurCreate = j;
					cObject.handle();
				}
				num4 += rh4ObjectAddCreate;
				j++;
				num4--;
			}
			while (num4 != 0);
		}
		rh3CollisionCount++;
		rhEvtProg.compute_TimerEvents();
		if (rhEvtProg.rhEventAlways && (rhGameFlags & 0x10) == 0)
		{
			rhEvtProg.computeEventList(0, null);
		}
		rhEvtProg.handle_PushedEvents();
		modif_ChangedObjects();
		destroy_List();
		rhEvtProg.rh2CurrentClick = -1;
		rhEvtProg.rh3CurrentMenu = 0;
		rh4EventCount++;
		rh4FakeKey = 0;
		if (rhQuit == 0)
		{
			return rhQuitBis;
		}
		if (rhQuit == 1 || rhQuit == 2 || rhQuit == -2 || rhQuit == 3 || rhQuit == 100 || rhQuit == 4)
		{
			rhEvtProg.handle_GlobalEvents(-65539);
		}
		return rhQuit;
	}

	public void modif_ChangedObjects()
	{
		int i = 0;
		for (int j = 0; j < rhNObjects; j++)
		{
			for (; rhObjectList[i] == null; i++)
			{
			}
			CObject cObject = rhObjectList[i];
			i++;
			if ((cObject.hoOEFlags & 0x230) != 0 && cObject.roc.rcChanged)
			{
				cObject.modif();
				cObject.roc.rcChanged = false;
			}
		}
	}

	public void draw()
	{
		if ((rhGameFlags & 0x10) == 0 && rhApp.parentApp == null)
		{
			screen_Update();
		}
	}

	private void joyTest()
	{
		GamePadState[] array = new GamePadState[4];
		for (int i = 0; i < 4; i++)
		{
			rhPlayer[i] = 0;
			switch (i)
			{
			case 0:
			{
				ref GamePadState reference4 = ref array[i];
				reference4 = GamePad.GetState(PlayerIndex.One);
				break;
			}
			case 1:
			{
				ref GamePadState reference3 = ref array[i];
				reference3 = GamePad.GetState(PlayerIndex.Two);
				break;
			}
			case 2:
			{
				ref GamePadState reference2 = ref array[i];
				reference2 = GamePad.GetState(PlayerIndex.Three);
				break;
			}
			case 3:
			{
				ref GamePadState reference = ref array[i];
				reference = GamePad.GetState(PlayerIndex.Four);
				break;
			}
			}
		}
		short[] ctrlType = rhApp.getCtrlType();
		Keys[] ctrlKeys = rhApp.getCtrlKeys();
		for (int i = 0; i < 4; i++)
		{
			short num = ctrlType[i];
			if (num != 5)
			{
				for (int j = 0; j < 4; j++)
				{
					if ((num & (1 << j)) != 0)
					{
						if (array[j].DPad.Left == ButtonState.Pressed)
						{
							rhPlayer[i] |= 4;
						}
						if (array[j].DPad.Right == ButtonState.Pressed)
						{
							rhPlayer[i] |= 8;
						}
						if (array[j].DPad.Up == ButtonState.Pressed)
						{
							rhPlayer[i] |= 1;
						}
						if (array[j].DPad.Down == ButtonState.Pressed)
						{
							rhPlayer[i] |= 2;
						}
						if ((double)array[j].ThumbSticks.Left.X < -0.5)
						{
							rhPlayer[i] |= 4;
						}
						if ((double)array[j].ThumbSticks.Left.X > 0.5)
						{
							rhPlayer[i] |= 8;
						}
						if ((double)array[j].ThumbSticks.Left.Y > 0.5)
						{
							rhPlayer[i] |= 1;
						}
						if ((double)array[j].ThumbSticks.Left.Y < -0.5)
						{
							rhPlayer[i] |= 2;
						}
						if (array[j].Buttons.A == ButtonState.Pressed)
						{
							rhPlayer[i] |= 16;
						}
						if (array[j].Buttons.B == ButtonState.Pressed)
						{
							rhPlayer[i] |= 32;
						}
						if (array[j].Buttons.X == ButtonState.Pressed)
						{
							rhPlayer[i] |= 64;
						}
						if (array[j].Buttons.Y == ButtonState.Pressed)
						{
							rhPlayer[i] |= 128;
						}
					}
				}
				continue;
			}
			for (int k = 0; k < 8; k++)
			{
				if (isKeyDown(ctrlKeys[i * 4 + k]))
				{
					rhPlayer[i] |= (byte)(1 << k);
				}
			}
		}
	}

	public bool isKeyDown(Keys key)
	{
		if (keyboardState.IsKeyDown(key))
		{
			return true;
		}
		if (key == Keys.LeftShift && keyboardState.IsKeyDown(Keys.RightShift))
		{
			return true;
		}
		if (key == Keys.LeftControl && keyboardState.IsKeyDown(Keys.RightControl))
		{
			return true;
		}
		return false;
	}

	private void getMouseCoords()
	{
		rh2MouseX = mouseX + rhWindowX;
		rh2MouseY = mouseY + rhWindowY;
		if (rhApp.parentApp != null)
		{
			rh2MouseX -= rhApp.xOffset;
			rh2MouseY -= rhApp.yOffset;
		}
		rh2MouseKeys = 0;
		if (mouseState.LeftButton == ButtonState.Pressed)
		{
			rh2MouseKeys |= 1;
		}
		if (mouseState.RightButton == ButtonState.Pressed)
		{
			rh2MouseKeys |= 2;
		}
		if (mouseState.MiddleButton == ButtonState.Pressed)
		{
			rh2MouseKeys |= 4;
		}
	}

	public bool newHandle_Collisions(CObject pHo)
	{
		pHo.rom.rmMoveFlag = false;
		pHo.rom.rmEventFlags = 0;
		bMoveChanged = false;
		if ((pHo.hoLimitFlags & 0x400) != 0)
		{
			int num = quadran_In(pHo.roc.rcOldX1, pHo.roc.rcOldY1, pHo.roc.rcOldX2, pHo.roc.rcOldY2);
			if (num != 0)
			{
				int num2 = quadran_In(pHo.hoX - pHo.hoImgXSpot, pHo.hoY - pHo.hoImgYSpot, pHo.hoX - pHo.hoImgXSpot + pHo.hoImgWidth, pHo.hoY - pHo.hoImgYSpot + pHo.hoImgHeight);
				if (num2 == 0)
				{
					int num3 = num ^ num2;
					if (num3 != 0)
					{
						pHo.rom.rmEventFlags |= 1;
						rhEvtProg.rhCurParam0 = num3;
						rhEvtProg.handle_Event(pHo, -720896 | (pHo.hoType & 0xFFFF));
					}
				}
			}
			int num4 = quadran_In(pHo.hoX - pHo.hoImgXSpot, pHo.hoY - pHo.hoImgYSpot, pHo.hoX - pHo.hoImgXSpot + pHo.hoImgWidth, pHo.hoY - pHo.hoImgYSpot + pHo.hoImgHeight);
			if ((num4 & pHo.rom.rmWrapping) != 0)
			{
				if ((num4 & 1) != 0)
				{
					pHo.rom.rmMovement.setXPosition(pHo.hoX + rhLevelSx);
				}
				else if ((num4 & 2) != 0)
				{
					pHo.rom.rmMovement.setXPosition(pHo.hoX - rhLevelSx);
				}
				if ((num4 & 4) != 0)
				{
					pHo.rom.rmMovement.setYPosition(pHo.hoY + rhLevelSy);
				}
				else if ((num4 & 8) != 0)
				{
					pHo.rom.rmMovement.setYPosition(pHo.hoY - rhLevelSy);
				}
			}
			num = quadran_Out(pHo.roc.rcOldX1, pHo.roc.rcOldY1, pHo.roc.rcOldX2, pHo.roc.rcOldY2);
			if (num != 15)
			{
				int num5 = quadran_Out(pHo.hoX - pHo.hoImgXSpot, pHo.hoY - pHo.hoImgYSpot, pHo.hoX - pHo.hoImgXSpot + pHo.hoImgWidth, pHo.hoY - pHo.hoImgYSpot + pHo.hoImgHeight);
				int num6 = ~num & num5;
				if (num6 != 0)
				{
					pHo.rom.rmEventFlags |= 2;
					rhEvtProg.rhCurParam0 = num6;
					rhEvtProg.handle_Event(pHo, -786432 | (pHo.hoType & 0xFFFF));
				}
			}
		}
		if ((pHo.hoLimitFlags & 0x200) != 0)
		{
			if (pHo.roc.rcMovementType == 9)
			{
				CMovePlatform cMovePlatform = (CMovePlatform)pHo.rom.rmMovement;
				cMovePlatform.mpHandle_Background();
			}
			else
			{
				int num7 = colMask_TestObject_IXY(pHo, pHo.roc.rcImage, pHo.roc.rcAngle, pHo.roc.rcScaleX, pHo.roc.rcScaleY, pHo.hoX, pHo.hoY, 0, 1);
				if (num7 != 0)
				{
					rhEvtProg.handle_Event(pHo, num7);
				}
			}
		}
		if ((pHo.hoLimitFlags & 0x80) != 0)
		{
			CArrayList cArrayList = objectAllCol_IXY(pHo, pHo.roc.rcImage, pHo.roc.rcAngle, pHo.roc.rcScaleX, pHo.roc.rcScaleY, pHo.hoX, pHo.hoY, pHo.hoOiList.oilColList);
			if (cArrayList != null)
			{
				for (int i = 0; i < cArrayList.size(); i++)
				{
					CObject cObject = (CObject)cArrayList.get(i);
					if ((cObject.hoFlags & 1) == 0)
					{
						short hoType = pHo.hoType;
						CObject cObject2 = pHo;
						CObject cObject3 = cObject;
						if (cObject2.hoType > cObject3.hoType)
						{
							cObject2 = cObject;
							cObject3 = pHo;
							hoType = cObject2.hoType;
						}
						rhEvtProg.rhCurParam0 = cObject3.hoOi;
						rhEvtProg.rh1stObjectNumber = cObject3.hoNumber;
						rhEvtProg.handle_Event(cObject2, -917504 | (hoType & 0xFFFF));
					}
				}
			}
		}
		return bMoveChanged;
	}

	public CArrayList objectAllCol_IXY(CObject pHo, short newImg, int newAngle, float newScaleX, float newScaleY, int newX, int newY, short[] pOiColList)
	{
		CArrayList cArrayList = null;
		int num = newX - pHo.hoImgXSpot;
		int num2 = num + pHo.hoImgWidth;
		int num3 = newY - pHo.hoImgYSpot;
		int num4 = num3 + pHo.hoImgHeight;
		if ((pHo.hoFlags & 0x2000) != 0)
		{
			return cArrayList;
		}
		bool flag = false;
		CMask cMask = null;
		int num5 = -1;
		CSprite cSprite = null;
		if (pHo.hoType == 2)
		{
			cSprite = pHo.roc.rcSprite;
			if (cSprite != null && (cSprite.sprFlags & 0x100) == 0)
			{
				flag = true;
			}
			num5 = pHo.ros.rsLayer;
		}
		short hoFlags = pHo.hoFlags;
		pHo.hoFlags |= 8192;
		int i = 0;
		if (pOiColList != null)
		{
			int num6 = 0;
			for (num6 = 0; num6 < pOiColList.Length; num6 += 2)
			{
				CObjInfo cObjInfo = rhOiList[pOiColList[num6 + 1]];
				int num7 = cObjInfo.oilObject;
				while (num7 >= 0)
				{
					CObject cObject = rhObjectList[num7];
					num7 = cObject.hoNumNext;
					if ((cObject.hoFlags & 0x2000) != 0)
					{
						continue;
					}
					int num8 = cObject.hoX - cObject.hoImgXSpot;
					int num9 = cObject.hoY - cObject.hoImgYSpot;
					if (num8 >= num2 || num8 + cObject.hoImgWidth <= num || num9 >= num4 || num9 + cObject.hoImgHeight <= num3)
					{
						continue;
					}
					switch (cObject.hoType)
					{
					case 2:
					{
						if (num5 >= 0 && (num5 < 0 || num5 != cObject.ros.rsLayer))
						{
							break;
						}
						CSprite rcSprite = cObject.roc.rcSprite;
						if (rcSprite == null || (rcSprite.sprFlags & 1) == 0)
						{
							break;
						}
						if (!flag || (rcSprite.sprFlags & 0x100) != 0)
						{
							if (cArrayList == null)
							{
								cArrayList = new CArrayList();
							}
							cArrayList.add(cObject);
							break;
						}
						if (cMask == null)
						{
							CImage imageFromHandle = rhApp.imageBank.getImageFromHandle(newImg);
							if (imageFromHandle != null)
							{
								cMask = imageFromHandle.getMask(0, newAngle, newScaleX, newScaleY);
							}
						}
						CMask cMask2 = null;
						CImage imageFromHandle2 = rhApp.imageBank.getImageFromHandle(cObject.roc.rcImage);
						if (imageFromHandle2 != null)
						{
							cMask2 = imageFromHandle2.getMask(0, cObject.roc.rcAngle, cObject.roc.rcScaleX, cObject.roc.rcScaleY);
						}
						if (cMask != null && cMask2 != null && cMask.testMask(0, num, num3, cMask2, 0, num8, num9))
						{
							if (cArrayList == null)
							{
								cArrayList = new CArrayList();
							}
							cArrayList.add(cObject);
						}
						break;
					}
					case 3:
					case 5:
					case 6:
					case 7:
					case 9:
						if (cArrayList == null)
						{
							cArrayList = new CArrayList();
						}
						cArrayList.add(cObject);
						break;
					default:
						if (cArrayList == null)
						{
							cArrayList = new CArrayList();
						}
						cArrayList.add(cObject);
						break;
					}
				}
			}
		}
		else
		{
			for (int j = 0; j < rhNObjects; j++)
			{
				for (; rhObjectList[i] == null; i++)
				{
				}
				CObject cObject = rhObjectList[i];
				i++;
				if ((cObject.hoFlags & 0x2000) != 0)
				{
					continue;
				}
				int num8 = cObject.hoX - cObject.hoImgXSpot;
				int num9 = cObject.hoY - cObject.hoImgYSpot;
				if (num8 >= num2 || num8 + cObject.hoImgWidth <= num || num9 >= num4 || num9 + cObject.hoImgHeight <= num3)
				{
					continue;
				}
				switch (cObject.hoType)
				{
				case 2:
				{
					if (num5 >= 0 && (num5 < 0 || num5 != cObject.ros.rsLayer))
					{
						break;
					}
					CSprite rcSprite = cObject.roc.rcSprite;
					if (rcSprite == null || (rcSprite.sprFlags & 1) == 0)
					{
						break;
					}
					if (!flag || (rcSprite.sprFlags & 0x100) != 0)
					{
						if (cArrayList == null)
						{
							cArrayList = new CArrayList();
						}
						cArrayList.add(cObject);
						break;
					}
					if (cMask == null)
					{
						CImage imageFromHandle = rhApp.imageBank.getImageFromHandle(newImg);
						if (imageFromHandle != null)
						{
							cMask = imageFromHandle.getMask(0, newAngle, newScaleX, newScaleY);
						}
					}
					CImage imageFromHandle2 = rhApp.imageBank.getImageFromHandle(cObject.roc.rcImage);
					CMask cMask2 = null;
					if (imageFromHandle2 != null)
					{
						cMask2 = imageFromHandle2.getMask(0, cObject.roc.rcAngle, cObject.roc.rcScaleX, cObject.roc.rcScaleY);
					}
					if (cMask != null && cMask2 != null && cMask.testMask(0, num, num3, cMask2, 0, num8, num9))
					{
						if (cArrayList == null)
						{
							cArrayList = new CArrayList();
						}
						cArrayList.add(cObject);
					}
					break;
				}
				case 3:
				case 5:
				case 6:
				case 7:
				case 9:
					if (cArrayList == null)
					{
						cArrayList = new CArrayList();
					}
					cArrayList.add(cObject);
					break;
				default:
					if (cArrayList == null)
					{
						cArrayList = new CArrayList();
					}
					cArrayList.add(cObject);
					break;
				}
			}
		}
		pHo.hoFlags = hoFlags;
		return cArrayList;
	}

	public int colMask_TestObject_IXY(CObject pHo, short newImg, int newAngle, float newScaleX, float newScaleY, int newX, int newY, int htfoot, int plan)
	{
		int result = 0;
		int num = newX - rhWindowX;
		int num2 = newY - rhWindowY;
		bool flag = false;
		if ((pHo.hoFlags & 0x24) != 0 && (pHo.ros.rsCreaFlags & 0x100) == 0)
		{
			flag = true;
		}
		if (flag)
		{
			CSprite rcSprite = pHo.roc.rcSprite;
			if (rcSprite != null && rhFrame.bkdCol_TestSprite(rcSprite, newImg, num, num2, newAngle, newScaleX, newScaleY, htfoot, plan))
			{
				result = -851968 | (pHo.hoType & 0xFFFF);
			}
		}
		else
		{
			num -= pHo.hoImgXSpot;
			num2 -= pHo.hoImgYSpot;
			if (htfoot != 0)
			{
				num2 += pHo.hoImgHeight;
				num2 -= htfoot;
				if (rhFrame.bkdCol_TestRect(num, num2, pHo.hoImgWidth, htfoot, pHo.hoLayer, plan))
				{
					result = -851968 | (pHo.hoType & 0xFFFF);
				}
			}
			else if (rhFrame.bkdCol_TestRect(num, num2, pHo.hoImgWidth, pHo.hoImgHeight, pHo.hoLayer, plan))
			{
				result = -851968 | (pHo.hoType & 0xFFFF);
			}
		}
		return result;
	}

	public int quadran_Out(int x1, int y1, int x2, int y2)
	{
		int num = 0;
		if (x1 < 0)
		{
			num |= 1;
		}
		if (y1 < 0)
		{
			num |= 4;
		}
		if (x2 > rhLevelSx)
		{
			num |= 2;
		}
		if (y2 > rhLevelSy)
		{
			num |= 8;
		}
		return Table_InOut[num];
	}

	public int quadran_In(int x1, int y1, int x2, int y2)
	{
		int num = 15;
		if (x1 < rhLevelSx)
		{
			num &= -3;
		}
		if (y1 < rhLevelSy)
		{
			num &= -9;
		}
		if (x2 > 0)
		{
			num &= -2;
		}
		if (y2 > 0)
		{
			num &= -5;
		}
		return Table_InOut[num];
	}

	public short random(short wMax)
	{
		int num = rh3Graine * 31415 + 1;
		rh3Graine = (short)num;
		num &= 0xFFFF;
		return (short)(num * wMax >> 16);
	}

	public int get_Direction(int dir)
	{
		if (dir == 0 || dir == -1)
		{
			return random(32);
		}
		int result = 0;
		int num = 0;
		int num2 = dir;
		for (int i = 0; i < 32; i++)
		{
			if ((num2 & 1) != 0)
			{
				num++;
				result = i;
			}
			num2 = (num2 >> 1) & 0x7FFFFFFF;
		}
		if (num == 1)
		{
			return result;
		}
		num = random((short)num);
		num2 = dir;
		for (int i = 0; i < 32; i++)
		{
			if ((num2 & 1) != 0)
			{
				num--;
				if (num < 0)
				{
					return i;
				}
			}
			num2 = (num2 >> 1) & 0x7FFFFFFF;
		}
		return 0;
	}

	public CValue get_EventExpressionAny(CParamExpression pExp)
	{
		rh4Tokens = pExp.tokens;
		rh4CurToken = 0;
		return new CValue(getExpression());
	}

	public int get_EventExpressionInt(CParamExpression pExp)
	{
		rh4Tokens = pExp.tokens;
		rh4CurToken = 0;
		return getExpression().getInt();
	}

	public double get_EventExpressionDouble(CParamExpression pExp)
	{
		rh4Tokens = pExp.tokens;
		rh4CurToken = 0;
		return getExpression().getDouble();
	}

	public string get_EventExpressionString(CParamExpression pExp)
	{
		rh4Tokens = pExp.tokens;
		rh4CurToken = 0;
		return getExpression().getString();
	}

	public int get_ExpressionInt()
	{
		return getExpression().getInt();
	}

	public double get_ExpressionDouble()
	{
		return getExpression().getDouble();
	}

	public string get_ExpressionString()
	{
		return getExpression().getString();
	}

	public CValue get_ExpressionAny()
	{
		return new CValue(getExpression());
	}

	public CValue getExpression()
	{
		int num = rh4PosPile;
		rh4Operators[rh4PosPile] = rh4OpeNull;
		do
		{
			rh4PosPile++;
			bOperande = true;
			rh4Tokens[rh4CurToken].evaluate(this);
			bOperande = false;
			rh4CurToken++;
			while (true)
			{
				CExp cExp = rh4Tokens[rh4CurToken];
				if (cExp.code > 0 && cExp.code < 1310720)
				{
					if (cExp.code > rh4Operators[rh4PosPile - 1].code)
					{
						rh4Operators[rh4PosPile] = cExp;
						rh4CurToken++;
						rh4PosPile++;
						bOperande = true;
						rh4Tokens[rh4CurToken].evaluate(this);
						bOperande = false;
						rh4CurToken++;
					}
					else
					{
						rh4PosPile--;
						rh4Operators[rh4PosPile].evaluate(this);
					}
				}
				else
				{
					rh4PosPile--;
					if (rh4PosPile == num)
					{
						break;
					}
					rh4Operators[rh4PosPile].evaluate(this);
				}
			}
		}
		while (rh4PosPile > num + 1);
		return rh4Results[num + 1];
	}

	public CValue getCurrentResult()
	{
		return rh4Results[rh4PosPile];
	}

	public CValue getPreviousResult()
	{
		return rh4Results[rh4PosPile - 1];
	}

	public CValue getNextResult()
	{
		return rh4Results[rh4PosPile + 1];
	}

	public static bool compareTo(CValue pValue1, CValue pValue2, short comp)
	{
		return comp switch
		{
			0 => pValue1.equal(pValue2), 
			1 => pValue1.notEqual(pValue2), 
			2 => pValue1.lower(pValue2), 
			3 => pValue1.lowerThan(pValue2), 
			4 => pValue1.greater(pValue2), 
			5 => pValue1.greaterThan(pValue2), 
			_ => false, 
		};
	}

	public static bool compareTer(int value1, int value2, short comparaison)
	{
		return comparaison switch
		{
			0 => value1 == value2, 
			1 => value1 != value2, 
			2 => value1 <= value2, 
			3 => value1 < value2, 
			4 => value1 >= value2, 
			5 => value1 > value2, 
			_ => false, 
		};
	}

	public void update_PlayerObjects(int joueur, short type, int value)
	{
		joueur++;
		int i = 0;
		for (int j = 0; j < rhNObjects; j++)
		{
			for (; rhObjectList[i] == null; i++)
			{
			}
			CObject cObject = rhObjectList[i];
			if (cObject.hoType == type)
			{
				switch (type)
				{
				case 5:
				{
					CScore cScore = (CScore)cObject;
					if (cScore.rsPlayer == joueur)
					{
						cScore.rsValue.forceInt(value);
					}
					break;
				}
				case 6:
				{
					CLives cLives = (CLives)cObject;
					if (cLives.rsPlayer == joueur)
					{
						cLives.rsValue.forceInt(value);
					}
					break;
				}
				}
				cObject.roc.rcChanged = true;
				cObject.modif();
			}
			i++;
		}
	}

	public void actPla_FinishLives(int joueur, int live)
	{
		int[] lives = rhApp.getLives();
		if (live != lives[joueur])
		{
			if (live == 0 && lives[joueur] != 0)
			{
				rhEvtProg.push_Event(0, -262151, 0, null, (short)joueur);
			}
			lives[joueur] = live;
			update_PlayerObjects(joueur, 6, live);
		}
	}

	public bool getMouseOnObjectsEDX(short oiList, bool nega)
	{
		CObject cObject = rhEvtProg.evt_FirstObject(oiList);
		if (cObject == null)
		{
			if (nega)
			{
				return true;
			}
			return false;
		}
		int num = rhEvtProg.evtNSelectedObjects;
		int num2 = rh2MouseX - rhWindowX;
		int num3 = rh2MouseY - rhWindowY;
		CArrayList cArrayList = new CArrayList();
		for (CSprite cSprite = rhApp.spriteGen.spriteCol_TestPoint(null, -1, num2, num3, 0); cSprite != null; cSprite = rhApp.spriteGen.spriteCol_TestPoint(cSprite, -1, num2, num3, 0))
		{
			CObject sprExtraInfo = cSprite.sprExtraInfo;
			if ((sprExtraInfo.hoFlags & 1) == 0)
			{
				cArrayList.add(sprExtraInfo);
			}
		}
		int i = 0;
		for (int j = 0; j < rhNObjects; j++)
		{
			for (; rhObjectList[i] == null; i++)
			{
			}
			CObject sprExtraInfo = rhObjectList[i];
			i++;
			if ((sprExtraInfo.hoFlags & 0x2004) == 0)
			{
				int num4 = sprExtraInfo.hoX - rhWindowX - sprExtraInfo.hoImgXSpot;
				int num5 = num4 + sprExtraInfo.hoImgWidth;
				int num6 = sprExtraInfo.hoY - rhWindowY - sprExtraInfo.hoImgYSpot;
				int num7 = num6 + sprExtraInfo.hoImgHeight;
				if (num2 >= num4 && num2 < num5 && num3 >= num6 && num3 < num7 && (sprExtraInfo.hoFlags & 1) == 0)
				{
					cArrayList.add(sprExtraInfo);
				}
			}
		}
		if (cArrayList.size() == 0)
		{
			if (nega)
			{
				return true;
			}
			return false;
		}
		if (!nega)
		{
			do
			{
				for (i = 0; i < cArrayList.size(); i++)
				{
					CObject sprExtraInfo = (CObject)cArrayList.get(i);
					if (sprExtraInfo == cObject)
					{
						break;
					}
				}
				if (i == cArrayList.size())
				{
					num--;
					rhEvtProg.evt_DeleteCurrentObject();
				}
				cObject = rhEvtProg.evt_NextObject();
			}
			while (cObject != null);
			return num != 0;
		}
		do
		{
			for (i = 0; i < cArrayList.size(); i++)
			{
				CObject sprExtraInfo = (CObject)cArrayList.get(i);
				if (sprExtraInfo == cObject)
				{
					return false;
				}
			}
			cObject = rhEvtProg.evt_NextObject();
		}
		while (cObject != null);
		return true;
	}

	public int txtDisplay(CEvent pe, short oi, int txtNumber)
	{
		PARAM_CREATE pARAM_CREATE = (PARAM_CREATE)pe.evtParams[0];
		CPositionInfo cPositionInfo = new CPositionInfo();
		if (pARAM_CREATE.read_Position(this, 16, cPositionInfo))
		{
			int i = 0;
			for (int j = 0; j < rhNObjects; j++)
			{
				for (; rhObjectList[i] == null; i++)
				{
				}
				CObject cObject = rhObjectList[i];
				i++;
				if (cObject.hoType == 3 && cObject.hoOi == oi && cObject.hoX == cPositionInfo.x && cObject.hoY == cPositionInfo.y)
				{
					cObject.ros.obShow();
					cObject.hoFlags &= -8193;
					CText cText = (CText)cObject;
					cText.rsMini = -2;
					cText.txtChange(txtNumber);
					cObject.roc.rcChanged = true;
					cObject.display();
					cObject.ros.rsFlash = 0;
					cObject.ros.rsFlags |= 32;
					return cObject.hoNumber;
				}
			}
			int num = f_CreateObject(-1, oi, cPositionInfo.x, cPositionInfo.y, 0, 0, rhFrame.nLayers - 1, -1);
			if (num >= 0)
			{
				((CText)rhObjectList[num]).txtChange(txtNumber);
				return num;
			}
		}
		return -1;
	}

	public int txtDoDisplay(CEvent pe, int txtNumber)
	{
		if (pe.evtOiList >= 0)
		{
			return txtDisplay(pe, pe.evtOi, txtNumber);
		}
		if (pe.evtOiList == -1)
		{
			return -1;
		}
		int num = pe.evtOiList & 0x7FFF;
		CQualToOiList cQualToOiList = rhEvtProg.qualToOiList[num];
		for (int i = 0; i < cQualToOiList.qoiList.Length; i += 2)
		{
			txtDisplay(pe, cQualToOiList.qoiList[i], txtNumber);
		}
		return -1;
	}

	public static CFontInfo getObjectFont(CObject hoPtr)
	{
		CFontInfo cFontInfo = null;
		if (hoPtr.hoType >= 32)
		{
			CExtension cExtension = (CExtension)hoPtr;
			cFontInfo = cExtension.ext.getRunObjectFont();
		}
		else
		{
			switch (hoPtr.hoType)
			{
			case 3:
			{
				CText cText = (CText)hoPtr;
				cFontInfo = cText.getFont();
				break;
			}
			case 5:
			{
				CScore cScore = (CScore)hoPtr;
				cFontInfo = cScore.getFont();
				break;
			}
			case 6:
			{
				CLives cLives = (CLives)hoPtr;
				cFontInfo = cLives.getFont();
				break;
			}
			case 7:
			{
				CCounter cCounter = (CCounter)hoPtr;
				cFontInfo = cCounter.getFont();
				break;
			}
			}
		}
		if (cFontInfo == null)
		{
			cFontInfo = new CFontInfo();
		}
		return cFontInfo;
	}

	public static void setObjectFont(CObject hoPtr, CFontInfo pLf, CRect pNewSize)
	{
		if (hoPtr.hoType >= 32)
		{
			CExtension cExtension = (CExtension)hoPtr;
			cExtension.ext.setRunObjectFont(pLf, pNewSize);
			return;
		}
		switch (hoPtr.hoType)
		{
		case 3:
		{
			CText cText = (CText)hoPtr;
			cText.setFont(pLf, pNewSize);
			break;
		}
		case 5:
		{
			CScore cScore = (CScore)hoPtr;
			cScore.setFont(pLf, pNewSize);
			break;
		}
		case 6:
		{
			CLives cLives = (CLives)hoPtr;
			cLives.setFont(pLf, pNewSize);
			break;
		}
		case 7:
		{
			CCounter cCounter = (CCounter)hoPtr;
			cCounter.setFont(pLf, pNewSize);
			break;
		}
		case 4:
			break;
		}
	}

	public static int getObjectTextColor(CObject hoPtr)
	{
		if (hoPtr.hoType >= 32)
		{
			CExtension cExtension = (CExtension)hoPtr;
			return cExtension.ext.getRunObjectTextColor();
		}
		switch (hoPtr.hoType)
		{
		case 3:
		{
			CText cText = (CText)hoPtr;
			return cText.getFontColor();
		}
		case 5:
		{
			CScore cScore = (CScore)hoPtr;
			return cScore.getFontColor();
		}
		case 6:
		{
			CLives cLives = (CLives)hoPtr;
			return cLives.getFontColor();
		}
		case 7:
		{
			CCounter cCounter = (CCounter)hoPtr;
			return cCounter.getFontColor();
		}
		default:
			return 0;
		}
	}

	public static void setObjectTextColor(CObject hoPtr, int rgb)
	{
		if (hoPtr.hoType >= 32)
		{
			CExtension cExtension = (CExtension)hoPtr;
			cExtension.ext.setRunObjectTextColor(rgb);
			return;
		}
		switch (hoPtr.hoType)
		{
		case 3:
		{
			CText cText = (CText)hoPtr;
			cText.setFontColor(rgb);
			break;
		}
		case 5:
		{
			CScore cScore = (CScore)hoPtr;
			cScore.setFontColor(rgb);
			break;
		}
		case 6:
		{
			CLives cLives = (CLives)hoPtr;
			cLives.setFontColor(rgb);
			break;
		}
		case 7:
		{
			CCounter cCounter = (CCounter)hoPtr;
			cCounter.setFontColor(rgb);
			break;
		}
		case 4:
			break;
		}
	}

	public static void setXPosition(CObject hoPtr, int x)
	{
		if (hoPtr.rom != null)
		{
			hoPtr.rom.rmMovement.setXPosition(x);
		}
		else if (hoPtr.hoX != x)
		{
			hoPtr.hoX = x;
			if (hoPtr.roc != null)
			{
				hoPtr.roc.rcChanged = true;
				hoPtr.roc.rcCheckCollides = true;
			}
		}
	}

	public static void setYPosition(CObject hoPtr, int y)
	{
		if (hoPtr.rom != null)
		{
			hoPtr.rom.rmMovement.setYPosition(y);
		}
		else if (hoPtr.hoY != y)
		{
			hoPtr.hoY = y;
			if (hoPtr.roc != null)
			{
				hoPtr.roc.rcChanged = true;
				hoPtr.roc.rcCheckCollides = true;
			}
		}
	}

	public static int get_DirFromPente(int x, int y)
	{
		if (x == 0)
		{
			if (y >= 0)
			{
				return 24;
			}
			return 8;
		}
		if (y == 0)
		{
			if (x >= 0)
			{
				return 0;
			}
			return 16;
		}
		bool flag = false;
		bool flag2 = false;
		if (x < 0)
		{
			flag = true;
			x = -x;
		}
		if (y < 0)
		{
			flag2 = true;
			y = -y;
		}
		int num = x * 256 / y;
		int i;
		for (i = 0; num < CMove.CosSurSin32[i]; i += 2)
		{
		}
		int num2 = CMove.CosSurSin32[i + 1];
		if (flag2)
		{
			num2 = -num2 + 32;
			num2 &= 0x1F;
		}
		if (flag)
		{
			num2 -= 8;
			num2 &= 0x1F;
			num2 = -num2;
			num2 &= 0x1F;
			num2 += 8;
			num2 &= 0x1F;
		}
		return num2;
	}

	public void init_Disappear(CObject hoPtr)
	{
		bool flag = false;
		int num = 0;
		if ((hoPtr.hoFlags & 8) == 0)
		{
			if (hoPtr.ros.initFadeOut())
			{
				return;
			}
			if (hoPtr.roa != null && hoPtr.roa.anim_Exist(4))
			{
				num = 1;
			}
		}
		if (num == 0)
		{
			flag = true;
		}
		if (flag)
		{
			hoPtr.hoCallRoutine = false;
			destroy_Add(hoPtr.hoNumber);
			return;
		}
		if (hoPtr.roc.rcSprite != null)
		{
			hoPtr.roc.rcSprite.setSpriteColFlag(0u);
		}
		if (hoPtr.rom != null)
		{
			hoPtr.rom.initSimple(hoPtr, 11, bRestore: false);
			hoPtr.roc.rcSpeed = 0;
		}
		if ((num & 1) != 0)
		{
			hoPtr.roa.animation_Force(4);
			hoPtr.roa.animation_OneLoop();
		}
	}

	public void add_QuickDisplay(CObject hoPtr)
	{
		if (rh4FirstQuickDisplay < 0)
		{
			rh4FirstQuickDisplay = hoPtr.hoNumber;
			hoPtr.hoPreviousQuickDisplay = -1;
		}
		else if (rh4LastQuickDisplay >= 0)
		{
			CObject cObject = rhObjectList[rh4LastQuickDisplay];
			cObject.hoNextQuickDisplay = hoPtr.hoNumber;
			hoPtr.hoPreviousQuickDisplay = cObject.hoNumber;
		}
		rh4LastQuickDisplay = hoPtr.hoNumber;
		hoPtr.hoNextQuickDisplay = -1;
	}

	public void draw_QuickDisplay(SpriteBatchEffect batch)
	{
		int hoNextQuickDisplay = rh4FirstQuickDisplay;
		while (hoNextQuickDisplay >= 0)
		{
			CObject cObject = rhObjectList[hoNextQuickDisplay];
			if ((cObject.ros.rsFlags & 5) == 0)
			{
				cObject.draw(batch);
			}
			hoNextQuickDisplay = cObject.hoNextQuickDisplay;
		}
	}

	public void remove_QuickDisplay(CObject hoPtr)
	{
		short hoNextQuickDisplay = hoPtr.hoNextQuickDisplay;
		short hoPreviousQuickDisplay = hoPtr.hoPreviousQuickDisplay;
		if (hoPreviousQuickDisplay >= 0)
		{
			CObject cObject = rhObjectList[hoPreviousQuickDisplay];
			cObject.hoNextQuickDisplay = hoNextQuickDisplay;
		}
		else
		{
			rh4FirstQuickDisplay = hoNextQuickDisplay;
		}
		if (hoNextQuickDisplay >= 0)
		{
			CObject cObject2 = rhObjectList[hoNextQuickDisplay];
			cObject2.hoPreviousQuickDisplay = hoPreviousQuickDisplay;
		}
		else
		{
			rh4LastQuickDisplay = hoPreviousQuickDisplay;
		}
	}

	public bool isMouseOn()
	{
		return rh4CursorShown;
	}

	public static void objectHide(CObject pHo)
	{
		if (pHo.ros != null)
		{
			pHo.ros.obHide();
			pHo.ros.rsFlags &= -33;
			pHo.ros.rsFlash = 0;
		}
	}

	public static void objectShow(CObject pHo)
	{
		if (pHo.ros != null)
		{
			pHo.ros.obShow();
			pHo.ros.rsFlags |= 32;
			pHo.ros.rsFlash = 0;
		}
	}

	public void setFrameRate(int value)
	{
		if (value >= 1 && value <= 1000)
		{
			CRunApp parentApp = rhApp;
			while (parentApp.parentApp != null)
			{
				parentApp = parentApp.parentApp;
			}
			parentApp.gaFrameRate = value;
		}
	}

	public int getXMouse()
	{
		if (rhMouseUsed != 0)
		{
			return 0;
		}
		return rh2MouseX;
	}

	public int getYMouse()
	{
		if (rhMouseUsed != 0)
		{
			return 0;
		}
		return rh2MouseY;
	}

	public int getRGBAt(CObject hoPtr, int x, int y)
	{
		return 0;
	}

	public CExtStorage getStorage(int id)
	{
		if (rhApp.extensionStorage != null)
		{
			for (int i = 0; i < rhApp.extensionStorage.size(); i++)
			{
				CExtStorage cExtStorage = (CExtStorage)rhApp.extensionStorage.get(i);
				if (cExtStorage.id == id)
				{
					return cExtStorage;
				}
			}
		}
		return null;
	}

	public void delStorage(int id)
	{
		if (rhApp.extensionStorage == null)
		{
			return;
		}
		for (int i = 0; i < rhApp.extensionStorage.size(); i++)
		{
			CExtStorage cExtStorage = (CExtStorage)rhApp.extensionStorage.get(i);
			if (cExtStorage.id == id)
			{
				rhApp.extensionStorage.remove(i);
			}
		}
	}

	public void addStorage(CExtStorage data, int id)
	{
		CExtStorage storage = getStorage(id);
		if (storage == null)
		{
			if (rhApp.extensionStorage == null)
			{
				rhApp.extensionStorage = new CArrayList();
			}
			data.id = id;
			rhApp.extensionStorage.add(data);
		}
	}

	public void callEventExtension(CExtension hoPtr, int code, int param)
	{
		if (rh2PauseCompteur == 0)
		{
			int rhCurParam = rhEvtProg.rhCurParam0;
			rhEvtProg.rhCurParam0 = param;
			code = -(code + 80 + 1) << 16;
			code |= hoPtr.hoType & 0xFFFF;
			rhEvtProg.handle_Event(hoPtr, code);
			rhEvtProg.rhCurParam0 = rhCurParam;
		}
	}

	public void addControl(IControl c)
	{
		nControls++;
		if (controls == null)
		{
			controls = new CArrayList();
		}
		controls.add(c);
		c.setMouseControlled(bMouseControlled);
	}

	public void delControl(IControl c)
	{
		nControls--;
		controls.remove(c);
	}

	public void clickControls(int nClicks)
	{
		for (int i = 0; i < nControls; i++)
		{
			((IControl)controls.get(i)).click(nClicks);
		}
	}

	public void newKey()
	{
		if (nControls <= 0)
		{
			return;
		}
		if ((rh2NewPlayer[0] & 4) != 0 && (rhPlayer[0] & 4) != 0)
		{
			IControl control;
			int num;
			int num2;
			if (currentControl == null)
			{
				control = null;
				num = 1000;
				num2 = 1000;
			}
			else
			{
				control = currentControl;
				num = currentControl.getX();
				num2 = currentControl.getY();
				control.setFocus(bFlag: false);
			}
			int num3 = -1000;
			int num4 = -1000;
			IControl control2 = null;
			for (int i = 0; i < nControls; i++)
			{
				IControl control3 = (IControl)controls.get(i);
				if (control3 != control)
				{
					int x = control3.getX();
					int y = control3.getY();
					if ((y < num2 || (y == num2 && x < num)) && (y > num4 || (y == num4 && x > num3)))
					{
						num3 = x;
						num4 = y;
						control2 = control3;
					}
				}
			}
			currentControl = control2;
		}
		if ((rh2NewPlayer[0] & 8) != 0 && (rhPlayer[0] & 8) != 0)
		{
			IControl control;
			int num;
			int num2;
			if (currentControl == null)
			{
				control = null;
				num = -1000;
				num2 = -1000;
			}
			else
			{
				control = currentControl;
				num = currentControl.getX();
				num2 = currentControl.getY();
				control.setFocus(bFlag: false);
			}
			int num3 = 1000;
			int num4 = 1000;
			IControl control2 = null;
			for (int i = 0; i < nControls; i++)
			{
				IControl control3 = (IControl)controls.get(i);
				if (control3 != control)
				{
					int x = control3.getX();
					int y = control3.getY();
					if ((y > num2 || (y == num2 && x > num)) && (y < num4 || (y == num4 && x < num3)))
					{
						num3 = x;
						num4 = y;
						control2 = control3;
					}
				}
			}
			currentControl = control2;
		}
		if (currentControl != null)
		{
			currentControl.setFocus(bFlag: true);
		}
	}
}
