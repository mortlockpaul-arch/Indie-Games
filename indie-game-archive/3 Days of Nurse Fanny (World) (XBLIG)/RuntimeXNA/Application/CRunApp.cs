using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using RuntimeXNA.Banks;
using RuntimeXNA.Expressions;
using RuntimeXNA.Extensions;
using RuntimeXNA.OI;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Application;

public class CRunApp
{
	public const short RUNTIME_VERSION = 770;

	public const short MAX_PLAYER = 4;

	public const short MAX_KEY = 8;

	public const short GA_NOHEADING = 2;

	public const short GA_SPEEDINDEPENDANT = 8;

	public const short GA_STRETCH = 16;

	public const short GA_MENUHIDDEN = 128;

	public const short GA_MENUBAR = 256;

	public const short GA_MAXIMISE = 512;

	public const short GA_MIX = 1024;

	public const short GA_FULLSCREENATSTART = 2048;

	public const short GANF_SAMPLESOVERFRAMES = 1;

	public const short GANF_RUNFRAME = 4;

	public const short GANF_NOTHICKFRAME = 64;

	public const short GANF_DONOTCENTERFRAME = 128;

	public const short GANF_DISABLE_CLOSE = 512;

	public const short GANF_HIDDENATSTART = 1024;

	public const short GANF_MDI = 16384;

	public const short GAOF_JAVASWING = 4096;

	public const short GAOF_JAVAAPPLET = 8192;

	public const short SL_RESTART = 0;

	public const short SL_STARTFRAME = 1;

	public const short SL_FRAMEFADEINLOOP = 2;

	public const short SL_FRAMELOOP = 3;

	public const short SL_FRAMEFADEOUTLOOP = 4;

	public const short SL_ENDFRAME = 5;

	public const short SL_QUIT = 6;

	public const int MAX_VK = 523;

	public const short CTRLTYPE_MOUSE = 0;

	public const short CTRLTYPE_JOY1 = 1;

	public const short CTRLTYPE_JOY2 = 2;

	public const short CTRLTYPE_JOY3 = 3;

	public const short CTRLTYPE_JOY4 = 4;

	public const short CTRLTYPE_KEYBOARD = 5;

	public const short ARF_INGAMELOOP = 4;

	public const int AH2OPT_STATUSLINE = 64;

	public const int AH2OPT_EDITPRESENT = 1024;

	public GraphicsDeviceManager graphicsDeviceManager;

	public SpriteBatchEffect spriteBatch;

	public GraphicsDevice graphicsDevice;

	public ContentManager content;

	public int displayType;

	public int[] frameOffsets;

	public int frameMaxIndex;

	public string[] framePasswords;

	public string appName;

	public string appCopyright;

	public string appAboutText;

	public string appDoc;

	public short nGlobalValuesInit;

	public byte[] globalValuesInitTypes;

	public int[] globalValuesInit;

	public short nGlobalStringsInit;

	public string[] globalStringsInit;

	public COIList OIList;

	public CImageBank imageBank;

	public CFontBank fontBank;

	public CSoundBank soundBank;

	public CSoundPlayer soundPlayer;

	public int appRunningState;

	public int[] lives;

	public int[] scores;

	public string[] playerNames;

	public CArrayList gValues;

	public CArrayList gStrings;

	public CValue tempGValue;

	public int startFrame;

	public int nextFrame;

	public int currentFrame;

	public CRunFrame frame;

	public CFile file;

	public CRunApp parentApp;

	public int parentOptions;

	public int parentX;

	public int parentY;

	public int parentWidth;

	public int parentHeight;

	public bool redrawBack = true;

	public short gaFlags;

	public short gaNewFlags;

	public short gaMode;

	public short gaOtherFlags;

	public int gaCxWin;

	public int gaCyWin;

	public int gaScoreInit;

	public int gaLivesInit;

	public int gaBorderColour;

	public int gaNbFrames;

	public int gaFrameRate;

	public short[] pcCtrlType = new short[4];

	public Keys[] pcCtrlKeys = new Keys[32];

	public short[] frameHandleToIndex;

	public short frameMaxHandle;

	public short appRunFlags;

	public CArrayList adGO;

	public CArrayList sysEvents;

	private bool quit;

	public CExtLoader extLoader;

	public bool m_bLoading;

	public bool bVisible;

	public bool bPositionWindow;

	public bool bResizeWindow;

	public int debug;

	public CArrayList extensionStorage;

	public CEmbeddedFile[] embeddedFiles;

	public bool internalPaintFlag;

	public bool bUnicode;

	public int VBL;

	public long timer;

	public double timeDouble;

	public CSpriteGen spriteGen;

	public CRun run;

	public CServices services;

	public Rectangle tempRect;

	public Game1 game;

	public int xOffset;

	public int yOffset;

	public bool bSubAppShown;

	public int numberOfTouches;

	public int hdr2Options;

	public int hdr2Orientation;

	public bool bSignedIn;

	public CArrayList advertisements;

	public StorageDevice storageDevice;

	public CRunApp()
	{
	}

	public CRunApp(Game1 gam, CFile f)
	{
		game = gam;
		file = f;
		content = gam.Content;
		graphicsDeviceManager = gam.graphics;
		graphicsDevice = gam.GraphicsDevice;
		spriteBatch = gam.spriteBatch;
	}

	public void setParentApp(CRunApp pApp, int sFrame, int options, int x, int y, int width, int height)
	{
		parentApp = pApp;
		parentOptions = options;
		startFrame = sFrame;
		xOffset = x;
		yOffset = y;
		parentWidth = width;
		parentHeight = height;
	}

	public void setOffsets(int x, int y)
	{
		xOffset = x;
		yOffset = y;
		spriteGen.setOffsets(x, y);
	}

	public void showSubApp(bool bShown)
	{
		bSubAppShown = bShown;
	}

	public bool load()
	{
		byte[] array = new byte[4];
		file.read(array);
		bool flag = false;
		if (array[0] == 80 && array[1] == 65 && array[2] == 77 && array[3] == 69)
		{
			flag = true;
			bUnicode = false;
		}
		if (array[0] == 80 && array[1] == 65 && array[2] == 77 && array[3] == 85)
		{
			flag = true;
			bUnicode = true;
		}
		if (!flag)
		{
			return false;
		}
		file.setUnicode(bUnicode);
		short num = file.readAShort();
		if (num != 770)
		{
			return false;
		}
		num = file.readAShort();
		file.readAInt();
		int num2 = file.readAInt();
		if (num2 < 249)
		{
			return false;
		}
		OIList = new COIList();
		imageBank = new CImageBank(this);
		fontBank = new CFontBank(this);
		soundBank = new CSoundBank();
		soundPlayer = new CSoundPlayer(this);
		CChunk cChunk = new CChunk();
		int num3 = 0;
		while (cChunk.chID != 32639)
		{
			cChunk.readHeader(file);
			if (cChunk.chSize == 0)
			{
				continue;
			}
			int pos = file.getFilePointer() + cChunk.chSize;
			switch (cChunk.chID)
			{
			case 8739:
			{
				loadAppHeader(file);
				frameOffsets = new int[gaNbFrames];
				framePasswords = new string[gaNbFrames];
				for (int i = 0; i < gaNbFrames; i++)
				{
					framePasswords[i] = null;
				}
				break;
			}
			case 8773:
				hdr2Options = file.readAInt();
				file.skipBytes(10);
				hdr2Orientation = file.readAShort();
				break;
			case 8740:
				appName = file.readAString();
				break;
			case 8763:
				appCopyright = file.readAString();
				break;
			case 8762:
				appAboutText = file.readAString();
				break;
			case 8752:
				appDoc = file.readAString();
				break;
			case 8754:
				loadGlobalValues(file);
				break;
			case 8755:
				loadGlobalStrings(file);
				break;
			case 8745:
			case 8767:
				OIList.preLoad(file);
				break;
			case 8747:
				loadFrameHandles(file, cChunk.chSize);
				break;
			case 13107:
			{
				frameOffsets[frameMaxIndex] = file.getFilePointer();
				CChunk cChunk2 = new CChunk();
				while (cChunk2.chID != 32639)
				{
					cChunk2.readHeader(file);
					if (cChunk2.chSize != 0)
					{
						int pos2 = file.getFilePointer() + cChunk2.chSize;
						switch (cChunk2.chID)
						{
						case 13110:
						{
							string text = file.readAString();
							framePasswords[frameMaxIndex] = text;
							num3++;
							break;
						}
						}
						file.seek(pos2);
					}
				}
				frameMaxIndex++;
				break;
			}
			case 8756:
				extLoader = new CExtLoader(this);
				extLoader.loadList(file);
				break;
			case 8760:
			{
				int num4 = file.readAInt();
				embeddedFiles = new CEmbeddedFile[num4];
				for (int i = 0; i < num4; i++)
				{
					embeddedFiles[i] = new CEmbeddedFile(this);
					embeddedFiles[i].preLoad();
				}
				break;
			}
			case 26214:
				imageBank.preLoad();
				break;
			case 26215:
				fontBank.preLoad();
				break;
			case 26216:
				soundBank.preLoad(this);
				break;
			}
			file.seek(pos);
		}
		soundPlayer.setMultipleSounds((gaFlags & 0x400) != 0);
		return true;
	}

	public bool startApplication()
	{
		sysEvents = new CArrayList();
		graphicsDeviceManager.PreferredBackBufferWidth = gaCxWin;
		graphicsDeviceManager.PreferredBackBufferHeight = gaCyWin;
		if ((gaFlags & 0x800) != 0)
		{
			setFullScreen(flag: true);
		}
		graphicsDeviceManager.ApplyChanges();
		spriteGen = new CSpriteGen();
		spriteGen.setOffsets(xOffset, yOffset);
		run = new CRun(this);
		services = new CServices();
		tempRect = default(Rectangle);
		setFrameRate(gaFrameRate);
		numberOfTouches = 0;
		displayType = -1;
		appRunningState = 0;
		currentFrame = -2;
		return true;
	}

	public bool playApplication(bool bOnlyRestartApp, double time)
	{
		int num = 0;
		bool flag = true;
		bool result = true;
		VBL++;
		timeDouble = time;
		timer = (long)time;
		do
		{
			switch (appRunningState)
			{
			case 0:
				initGlobal();
				nextFrame = startFrame;
				appRunningState = 1;
				killGlobalData();
				if (bOnlyRestartApp)
				{
					flag = false;
					break;
				}
				goto case 1;
			case 1:
				num = startTheFrame();
				break;
			case 3:
				if (!loopFrame())
				{
					endFrame();
				}
				else
				{
					flag = false;
				}
				break;
			case 5:
				endFrame();
				break;
			default:
				flag = false;
				break;
			}
		}
		while (flag && num == 0 && !quit);
		if (num != 0)
		{
			appRunningState = 6;
		}
		if (appRunningState == 6)
		{
			result = false;
		}
		return result;
	}

	public void endApplication()
	{
	}

	public int startTheFrame()
	{
		int num = 0;
		if (nextFrame != currentFrame)
		{
			frame = new CRunFrame(this);
			if (!frame.loadFullFrame(nextFrame))
			{
				num = -1;
				goto IL_0159;
			}
			currentFrame = nextFrame;
		}
		frame.leX = (frame.leY = 0);
		frame.leLastScrlX = (frame.leLastScrlY = 0);
		frame.rhOK = false;
		frame.levelQuit = 0;
		int leEditWinWidth = Math.Min(gaCxWin, frame.leWidth);
		int leEditWinHeight = Math.Min(gaCyWin, frame.leHeight);
		frame.leEditWinWidth = leEditWinWidth;
		frame.leEditWinHeight = leEditWinHeight;
		int collisionFlags = frame.evtProg.getCollisionFlags();
		collisionFlags |= frame.getMaskBits();
		frame.leFlags |= 32;
		frame.colMask = null;
		if ((collisionFlags & 3) != 0)
		{
			frame.colMask = CColMask.create(-64, -16, frame.leWidth + 64, frame.leHeight + 16, collisionFlags);
		}
		setLevelTitle();
		newResetCptVbl();
		goto IL_0159;
		IL_0159:
		bResizeWindow = true;
		run.setFrame(frame);
		run.initRunLoop();
		frame.rhPtr = run;
		if (frame.fadeIn != null)
		{
			if (!loopFrame())
			{
				appRunningState = 5;
			}
			else if (!startFrameFadeIn())
			{
				appRunningState = 3;
			}
		}
		else
		{
			appRunningState = 3;
		}
		if (num != 0)
		{
			appRunningState = 6;
		}
		return num;
	}

	public bool loopFrame()
	{
		if (frame.levelQuit == 0)
		{
			frame.levelQuit = run.doRunLoop();
		}
		return frame.levelQuit == 0;
	}

	public void endFrame()
	{
		int ul = run.killRunLoop(frame.levelQuit, bLeaveSamples: false);
		if ((gaNewFlags & 4) != 0)
		{
			appRunningState = 6;
		}
		else
		{
			switch (CServices.LOWORD(ul))
			{
			case 1:
				nextFrame = currentFrame + 1;
				appRunningState = 1;
				break;
			case 2:
				nextFrame = Math.Max(0, currentFrame - 1);
				appRunningState = 1;
				break;
			case 3:
				appRunningState = 1;
				if ((CServices.HIWORD(ul) & 0x8000) != 0)
				{
					nextFrame = CServices.HIWORD(ul) & 0x7FFF;
					if (nextFrame >= gaNbFrames)
					{
						nextFrame = gaNbFrames - 1;
					}
					if (nextFrame < 0)
					{
						nextFrame = 0;
					}
				}
				else if (CServices.HIWORD(ul) < frameMaxHandle)
				{
					nextFrame = frameHandleToIndex[CServices.HIWORD(ul)];
					if (nextFrame == -1)
					{
						nextFrame = currentFrame + 1;
					}
				}
				else
				{
					nextFrame = currentFrame + 1;
				}
				break;
			case 4:
				appRunningState = 0;
				nextFrame = startFrame;
				break;
			default:
				appRunningState = 6;
				break;
			}
		}
		if (appRunningState == 1 && (nextFrame < 0 || nextFrame >= gaNbFrames))
		{
			appRunningState = 6;
		}
		if (appRunningState != 1 || nextFrame != currentFrame)
		{
			currentFrame = -1;
		}
	}

	public void draw()
	{
		spriteBatch.Begin();
		run.draw();
		spriteBatch.End();
	}

	public void killGlobalData()
	{
		adGO = null;
	}

	public bool startFrameFadeIn()
	{
		return false;
	}

	public bool loopFrameFadeIn()
	{
		return false;
	}

	public bool endFrameFadeIn()
	{
		return true;
	}

	public bool startFrameFadeOut()
	{
		return false;
	}

	public bool loopFrameFadeOut()
	{
		return false;
	}

	public bool endFrameFadeOut()
	{
		return true;
	}

	public void initGlobal()
	{
		if (parentApp == null || parentApp != null)
		{
			lives = new int[4];
			for (int i = 0; i < 4; i++)
			{
				lives[i] = gaLivesInit ^ -1;
			}
		}
		else
		{
			lives = null;
		}
		if (parentApp == null || parentApp != null)
		{
			scores = new int[4];
			for (int i = 0; i < 4; i++)
			{
				scores[i] = gaScoreInit ^ -1;
			}
		}
		else
		{
			scores = null;
		}
		playerNames = new string[4];
		for (int i = 0; i < 4; i++)
		{
			playerNames[i] = "";
		}
		if (parentApp == null || parentApp != null)
		{
			gValues = new CArrayList();
			for (int i = 0; i < nGlobalValuesInit; i++)
			{
				gValues.add(new CValue(globalValuesInit[i]));
			}
		}
		else
		{
			gValues = null;
		}
		tempGValue = new CValue();
		if (parentApp == null || parentApp != null)
		{
			gStrings = new CArrayList();
			for (int i = 0; i < nGlobalStringsInit; i++)
			{
				gStrings.add(globalStringsInit[i]);
			}
		}
		else
		{
			gStrings = null;
		}
	}

	public int[] getLives()
	{
		CRunApp cRunApp = this;
		while (cRunApp.lives == null)
		{
			cRunApp = cRunApp.parentApp;
		}
		return cRunApp.lives;
	}

	public int[] getScores()
	{
		CRunApp cRunApp = this;
		while (cRunApp.scores == null)
		{
			cRunApp = cRunApp.parentApp;
		}
		return cRunApp.scores;
	}

	public short[] getCtrlType()
	{
		CRunApp cRunApp = this;
		while (cRunApp.parentApp != null)
		{
			cRunApp = cRunApp.parentApp;
		}
		return cRunApp.pcCtrlType;
	}

	public Keys[] getCtrlKeys()
	{
		CRunApp cRunApp = this;
		while (cRunApp.parentApp != null)
		{
			cRunApp = cRunApp.parentApp;
		}
		return cRunApp.pcCtrlKeys;
	}

	public CArrayList getGlobalValues()
	{
		CRunApp cRunApp = this;
		while (cRunApp.gValues == null)
		{
			cRunApp = cRunApp.parentApp;
		}
		return cRunApp.gValues;
	}

	public int getNGlobalValues()
	{
		if (gValues != null)
		{
			return gValues.size();
		}
		return 0;
	}

	public CArrayList getGlobalStrings()
	{
		CRunApp cRunApp = this;
		while (cRunApp.gStrings == null)
		{
			cRunApp = cRunApp.parentApp;
		}
		return cRunApp.gStrings;
	}

	public int getNGlobalStrings()
	{
		if (gStrings != null)
		{
			return gStrings.size();
		}
		return 0;
	}

	public CArrayList checkGlobalValue(int num)
	{
		CArrayList globalValues = getGlobalValues();
		if (num < 0 || num > 1000)
		{
			return null;
		}
		int num2 = globalValues.size();
		if (num >= num2)
		{
			globalValues.ensureCapacity(num);
			for (int i = num2; i <= num; i++)
			{
				globalValues.add(new CValue());
			}
		}
		return globalValues;
	}

	public CValue getGlobalValueAt(int num)
	{
		CArrayList cArrayList = checkGlobalValue(num);
		if (cArrayList != null)
		{
			return (CValue)cArrayList.get(num);
		}
		return tempGValue;
	}

	public void setGlobalValueAt(int num, CValue value)
	{
		CArrayList cArrayList = checkGlobalValue(num);
		if (cArrayList != null)
		{
			((CValue)cArrayList.get(num)).forceValue(value);
		}
	}

	public CArrayList checkGlobalString(int num)
	{
		CArrayList globalStrings = getGlobalStrings();
		if (num < 0 || num > 1000)
		{
			return null;
		}
		int num2 = globalStrings.size();
		if (num >= num2)
		{
			globalStrings.ensureCapacity(num);
			for (int i = num2; i <= num; i++)
			{
				globalStrings.add("");
			}
		}
		return globalStrings;
	}

	public string getGlobalStringAt(int num)
	{
		CArrayList cArrayList = checkGlobalString(num);
		if (cArrayList != null)
		{
			return (string)cArrayList.get(num);
		}
		return "";
	}

	public void setGlobalStringAt(int num, string value)
	{
		checkGlobalString(num)?.set(num, string.Concat(value));
	}

	public void loadAppHeader(CFile file)
	{
		file.skipBytes(4);
		gaFlags = file.readAShort();
		gaNewFlags = file.readAShort();
		gaMode = file.readAShort();
		gaOtherFlags = file.readAShort();
		gaCxWin = file.readAShort();
		gaCyWin = file.readAShort();
		gaScoreInit = file.readAInt();
		gaLivesInit = file.readAInt();
		for (int i = 0; i < 4; i++)
		{
			short num = file.readAShort();
			if (num == 0)
			{
				num = 5;
			}
			if (num < 5)
			{
				num = (short)((1 << num - 1) | 0x80);
			}
			pcCtrlType[i] = num;
		}
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 8; j++)
			{
				pcCtrlKeys[i * 8 + j] = CKeyConvert.getXnaKey(file.readAShort());
			}
		}
		for (int i = 0; i < 4; i++)
		{
			pcCtrlType[i] = (short)(1 << i);
		}
		gaBorderColour = file.readAColor();
		gaNbFrames = file.readAInt();
		gaFrameRate = file.readAInt();
		file.skipBytes(1);
		file.skipBytes(3);
	}

	public void loadGlobalValues(CFile file)
	{
		nGlobalValuesInit = file.readAShort();
		globalValuesInit = new int[nGlobalValuesInit];
		globalValuesInitTypes = new byte[nGlobalValuesInit];
		for (int i = 0; i < nGlobalValuesInit; i++)
		{
			globalValuesInit[i] = file.readAInt();
		}
		file.read(globalValuesInitTypes);
	}

	public void loadGlobalStrings(CFile file)
	{
		nGlobalStringsInit = (short)file.readAInt();
		globalStringsInit = new string[nGlobalStringsInit];
		for (int i = 0; i < nGlobalStringsInit; i++)
		{
			globalStringsInit[i] = file.readAString();
		}
	}

	public void loadFrameHandles(CFile file, int size)
	{
		frameMaxHandle = (short)(size / 2);
		frameHandleToIndex = new short[frameMaxHandle];
		for (int i = 0; i < frameMaxHandle; i++)
		{
			frameHandleToIndex[i] = file.readAShort();
		}
	}

	public short HCellToNCell(short hCell)
	{
		if (frameHandleToIndex == null || hCell == -1 || hCell >= frameMaxHandle)
		{
			return -1;
		}
		return frameHandleToIndex[hCell];
	}

	public void showCursor(bool bShown)
	{
		game.IsMouseVisible = bShown;
	}

	public int newGetCptVbl()
	{
		return VBL;
	}

	public void newResetCptVbl()
	{
		VBL = 0;
	}

	public void setFrameRate(int fps)
	{
		gaFrameRate = fps;
		double value = 1000.0 / (double)fps;
		TimeSpan targetElapsedTime = TimeSpan.FromMilliseconds(value);
		game.TargetElapsedTime = targetElapsedTime;
	}

	public void setFullScreen(bool flag)
	{
		try
		{
			if (flag)
			{
				if (!graphicsDeviceManager.IsFullScreen)
				{
					graphicsDeviceManager.ToggleFullScreen();
					graphicsDeviceManager.ApplyChanges();
				}
			}
			else if (graphicsDeviceManager.IsFullScreen)
			{
				graphicsDeviceManager.ToggleFullScreen();
				graphicsDeviceManager.ApplyChanges();
			}
		}
		catch (NoSuitableGraphicsDeviceException ex)
		{
			ex.GetType();
		}
		catch (InvalidOperationException ex2)
		{
			ex2.GetType();
		}
		catch (ArgumentException ex3)
		{
			ex3.GetType();
		}
	}

	private void setLevelTitle()
	{
	}

	public CEmbeddedFile getEmbeddedFile(string path)
	{
		if (embeddedFiles != null)
		{
			for (int i = 0; i < embeddedFiles.Length; i++)
			{
				if (string.Compare(embeddedFiles[i].path, path) == 0)
				{
					return embeddedFiles[i];
				}
			}
		}
		return null;
	}
}
