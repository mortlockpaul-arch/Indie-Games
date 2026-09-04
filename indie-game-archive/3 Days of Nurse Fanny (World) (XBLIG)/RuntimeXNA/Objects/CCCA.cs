using RuntimeXNA.Application;
using RuntimeXNA.Expressions;
using RuntimeXNA.OI;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Objects;

internal class CCCA : CObject
{
	public const int CCAF_SHARE_GLOBALVALUES = 1;

	public const int CCAF_SHARE_LIVES = 2;

	public const int CCAF_SHARE_SCORES = 4;

	public const int CCAF_SHARE_WINATTRIB = 8;

	public const int CCAF_STRETCH = 16;

	public const int CCAF_POPUP = 32;

	public const int CCAF_CAPTION = 64;

	public const int CCAF_TOOLCAPTION = 128;

	public const int CCAF_BORDER = 256;

	public const int CCAF_WINRESIZE = 512;

	public const int CCAF_SYSMENU = 1024;

	public const int CCAF_DISABLECLOSE = 2048;

	public const int CCAF_MODAL = 4096;

	public const int CCAF_DIALOGFRAME = 8192;

	public const int CCAF_INTERNAL = 16384;

	public const int CCAF_HIDEONCLOSE = 32768;

	public const int CCAF_CUSTOMSIZE = 65536;

	public const int CCAF_INTERNALABOUTBOX = 131072;

	public const int CCAF_CLIPSIBLINGS = 262144;

	public const int CCAF_SHARE_PLAYERCTRLS = 524288;

	public const int CCAF_MDICHILD = 1048576;

	public const int CCAF_DOCKED = 2097152;

	public const int CCAF_DOCKING_AREA = 12582912;

	public const int CCAF_DOCKED_LEFT = 0;

	public const int CCAF_DOCKED_TOP = 4194304;

	public const int CCAF_DOCKED_RIGHT = 8388608;

	public const int CCAF_DOCKED_BOTTOM = 12582912;

	public const int CCAF_REOPEN = 16777216;

	public const int CCAF_MDIRUNEVENIFNOTACTIVE = 33554432;

	public const int CCAF_HIDDENATSTART = 67108864;

	internal int flags;

	internal int odOptions;

	internal CRunApp subApp;

	internal int level;

	internal int oldLevel;

	private bool bPaused;

	public void startCCA(CObjectCommon ocPtr, bool bInit, int nStartFrame)
	{
		CDefCCA cDefCCA = (CDefCCA)ocPtr.ocObject;
		hoImgWidth = cDefCCA.odCx;
		hoImgHeight = cDefCCA.odCy;
		odOptions = cDefCCA.odOptions;
		if ((odOptions & 0x10) != 0)
		{
			odOptions |= 65536;
		}
		if (nStartFrame == -1)
		{
			nStartFrame = 0;
			if ((odOptions & 0x4000) != 0)
			{
				nStartFrame = cDefCCA.odNStartFrame;
			}
		}
		if (cDefCCA.odName != null && cDefCCA.odName.Length == 0 && (odOptions & 0x4000) != 0 && nStartFrame < hoAdRunHeader.rhApp.gaNbFrames && nStartFrame != hoAdRunHeader.rhApp.currentFrame)
		{
			CFile f = new CFile(hoAdRunHeader.rhApp.file);
			subApp = new CRunApp(hoAdRunHeader.rhApp.game, f);
			subApp.setParentApp(hoAdRunHeader.rhApp, nStartFrame, odOptions, hoX - hoAdRunHeader.rhWindowX, hoY - hoAdRunHeader.rhWindowY, hoImgWidth, hoImgHeight);
			subApp.showSubApp((ocPtr.ocFlags2 & 8) != 0);
			subApp.load();
			subApp.startApplication();
			subApp.playApplication(bOnlyRestartApp: true, hoAdRunHeader.rhApp.timeDouble);
			hoAdRunHeader.nSubApps++;
		}
	}

	public override void init(CObjectCommon ocPtr, CCreateObjectInfo cob)
	{
		startCCA(ocPtr, bInit: true, -1);
	}

	public override void handle()
	{
		rom.move();
		if (subApp != null)
		{
			subApp.setOffsets(hoX - hoAdRunHeader.rhWindowX, hoY - hoAdRunHeader.rhWindowY);
			if (!subApp.playApplication(bOnlyRestartApp: false, hoAdRunHeader.rhApp.timeDouble))
			{
				subApp.endApplication();
				subApp = null;
			}
			else
			{
				oldLevel = level;
				level = subApp.currentFrame;
			}
		}
	}

	public override void kill(bool bFast)
	{
		if (subApp != null)
		{
			int appRunningState = subApp.appRunningState;
			if (appRunningState == 3)
			{
				subApp.endFrame();
			}
			subApp.endApplication();
			subApp = null;
			hoAdRunHeader.nSubApps--;
		}
	}

	public virtual void restartApp()
	{
		if (subApp != null)
		{
			if (subApp.run != null)
			{
				subApp.run.rhQuit = 4;
				return;
			}
			kill(bFast: true);
			hoAdRunHeader.nSubApps--;
		}
		startCCA(hoCommon, bInit: false, -1);
	}

	public virtual void endApp()
	{
		if (subApp != null && subApp.run != null)
		{
			subApp.run.rhQuit = -2;
		}
	}

	public virtual void hide()
	{
		if (subApp != null)
		{
			subApp.showSubApp(bShown: false);
		}
	}

	public virtual void show()
	{
		if (subApp != null)
		{
			subApp.showSubApp(bShown: true);
		}
	}

	public virtual void jumpFrame(int frame)
	{
		if (subApp != null && subApp.run != null)
		{
			subApp.run.rhQuit = 3;
			subApp.run.rhQuitParam = 0x8000 | frame;
		}
	}

	public virtual void nextFrame()
	{
		if (subApp != null && subApp.run != null)
		{
			subApp.run.rhQuit = 1;
		}
	}

	public virtual void previousFrame()
	{
		if (subApp != null && subApp.run != null)
		{
			subApp.run.rhQuit = 2;
		}
	}

	public virtual void restartFrame()
	{
		if (subApp != null && subApp.run != null)
		{
			subApp.run.rhQuit = 101;
		}
	}

	public virtual void pause()
	{
		if (subApp != null)
		{
			bPaused = true;
			if (subApp.run != null)
			{
				subApp.run.pause();
			}
		}
	}

	public virtual void resume()
	{
		if (subApp != null)
		{
			bPaused = false;
			if (subApp.run != null)
			{
				subApp.run.resume();
			}
		}
	}

	public virtual void setGlobalValue(int number, CValue value_Renamed)
	{
		if (subApp != null)
		{
			subApp.setGlobalValueAt(number, value_Renamed);
		}
	}

	public virtual void setGlobalString(int number, string value_Renamed)
	{
		if (subApp != null)
		{
			subApp.setGlobalStringAt(number, value_Renamed);
		}
	}

	public virtual bool appFinished()
	{
		return subApp == null;
	}

	public virtual bool frameChanged()
	{
		return level != oldLevel;
	}

	public virtual string getGlobalString(int num)
	{
		if (subApp != null)
		{
			return subApp.getGlobalStringAt(num);
		}
		return "";
	}

	public virtual CValue getGlobalValue(int num)
	{
		if (subApp != null)
		{
			return subApp.getGlobalValueAt(num);
		}
		return new CValue(0);
	}

	public bool isVisible()
	{
		if (subApp != null)
		{
			return subApp.bSubAppShown;
		}
		return false;
	}

	public bool isPaused()
	{
		return bPaused;
	}

	public override void draw(SpriteBatchEffect batch)
	{
		if (subApp != null && subApp.run != null)
		{
			subApp.run.screen_Update();
		}
	}
}
