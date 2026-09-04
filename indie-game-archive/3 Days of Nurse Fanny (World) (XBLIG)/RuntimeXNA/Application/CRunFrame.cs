using System;
using RuntimeXNA.Banks;
using RuntimeXNA.Events;
using RuntimeXNA.Frame;
using RuntimeXNA.OI;
using RuntimeXNA.Objects;
using RuntimeXNA.RunLoop;
using RuntimeXNA.Services;
using RuntimeXNA.Sprites;

namespace RuntimeXNA.Application;

public class CRunFrame
{
	public const int LEF_DISPLAYNAME = 1;

	public const int LEF_GRABDESKTOP = 2;

	public const int LEF_KEEPDISPLAY = 4;

	public const int LEF_TOTALCOLMASK = 32;

	public const int LEF_RESIZEATSTART = 256;

	public const int LEF_NOSURFACE = 2048;

	public const int LEF_TIMEDMVTS = 32768;

	public const int IPHONEOPT_JOYSTICK_FIRE1 = 1;

	public const int IPHONEOPT_JOYSTICK_FIRE2 = 2;

	public const int IPHONEOPT_JOYSTICK_LEFTHAND = 4;

	public const int IPHONEOPT_MULTITOUCH = 8;

	public const int IPHONEOPT_SCREENLOCKING = 16;

	public const int IPHONEOPT_IPHONEFRAMEIAD = 32;

	public const int JOYSTICK_NONE = 0;

	public const int JOYSTICK_TOUCH = 1;

	public const int JOYSTICK_ACCELEROMETER = 2;

	public const int JOYSTICK_EXT = 3;

	public int leWidth;

	public int leHeight;

	public int leBackground;

	public int leFlags;

	public CRect leVirtualRect;

	public int leEditWinWidth;

	public int leEditWinHeight;

	public string frameName;

	public int nLayers;

	public CLayer[] layers;

	public CLOList LOList;

	public CEventProgram evtProg;

	public short maxObjects = 512;

	public int leX;

	public int leY;

	public int leLastScrlX;

	public int leLastScrlY;

	public CRect fadeIn;

	public CRect fadeOut;

	public int levelQuit;

	public bool rhOK;

	public int startLeX;

	public int startLeY;

	public bool fade;

	public int fadeTimerDelta;

	public int fadeVblDelta;

	public int dwColMaskBits;

	public CColMask colMask;

	public short m_wRandomSeed;

	public int m_dwMvtTimerBase;

	public CRunApp app;

	public CRun rhPtr;

	public short joystick;

	public short iPhoneOptions;

	public short[] mosaicHandles;

	public int[] mosaicX;

	public int[] mosaicY;

	public int mosaicMaxHandle;

	public CRunFrame()
	{
	}

	public CRunFrame(CRunApp pApp)
	{
		app = pApp;
	}

	public bool loadFullFrame(int index)
	{
		app.file.seek(app.frameOffsets[index]);
		evtProg = new CEventProgram();
		LOList = new CLOList();
		leVirtualRect = new CRect();
		CChunk cChunk = new CChunk();
		int num = 0;
		int num2 = 0;
		m_wRandomSeed = -1;
		while (cChunk.chID != 32639)
		{
			cChunk.readHeader(app.file);
			if (cChunk.chSize == 0)
			{
				continue;
			}
			int pos = app.file.getFilePointer() + cChunk.chSize;
			switch (cChunk.chID)
			{
			case 13108:
				loadHeader();
				leEditWinWidth = Math.Min(app.gaCxWin, leWidth);
				leEditWinHeight = Math.Min(app.gaCyWin, leHeight);
				break;
			case 13122:
				leVirtualRect.load(app.file);
				if ((leFlags & 0x100) != 0)
				{
					if (leVirtualRect.right - leVirtualRect.left == num || leVirtualRect.right - leVirtualRect.left < leWidth)
					{
						leVirtualRect.right = leVirtualRect.left + leWidth;
					}
					if (leVirtualRect.bottom - leVirtualRect.top == num2 || leVirtualRect.bottom - leVirtualRect.top < leHeight)
					{
						leVirtualRect.bottom = leVirtualRect.top + leHeight;
					}
				}
				break;
			case 13124:
				m_wRandomSeed = app.file.readAShort();
				break;
			case 13127:
				m_dwMvtTimerBase = app.file.readAInt();
				break;
			case 13109:
				frameName = app.file.readAString();
				break;
			case 13121:
				loadLayers();
				break;
			case 13112:
				LOList.load(app);
				break;
			case 13130:
				joystick = app.file.readAShort();
				iPhoneOptions = app.file.readAShort();
				break;
			case 13117:
				evtProg.load(app);
				maxObjects = evtProg.maxObjects;
				break;
			case 13128:
			{
				int num3 = cChunk.chSize / 6;
				mosaicHandles = new short[num3];
				mosaicX = new int[num3];
				mosaicY = new int[num3];
				mosaicMaxHandle = 0;
				for (int i = 0; i < num3; i++)
				{
					mosaicHandles[i] = app.file.readAShort();
					mosaicMaxHandle = Math.Max(mosaicMaxHandle, mosaicHandles[i]);
					mosaicX[i] = app.file.readAShort();
					mosaicY[i] = app.file.readAShort();
				}
				mosaicMaxHandle++;
				break;
			}
			}
			app.file.seek(pos);
		}
		app.OIList.resetToLoad();
		for (int i = 0; i < LOList.nIndex; i++)
		{
			CLO lOFromIndex = LOList.getLOFromIndex((short)i);
			app.OIList.setToLoad(lOFromIndex.loOiHandle);
		}
		app.imageBank.resetToLoad();
		app.fontBank.resetToLoad();
		app.OIList.load(app.file, app);
		app.OIList.enumElements(app.imageBank, app.fontBank);
		app.imageBank.load();
		app.fontBank.load();
		evtProg.enumSounds(app.soundBank);
		app.soundBank.load();
		app.OIList.resetOICurrent();
		for (int i = 0; i < LOList.nIndex; i++)
		{
			CLO cLO = LOList.list[i];
			if (cLO.loType >= 2)
			{
				app.OIList.setOICurrent(cLO.loOiHandle);
			}
		}
		return true;
	}

	public void loadLayers()
	{
		nLayers = app.file.readAInt();
		layers = new CLayer[nLayers];
		for (int i = 0; i < nLayers; i++)
		{
			layers[i] = new CLayer();
			layers[i].load(app.file);
		}
	}

	public void loadHeader()
	{
		leWidth = app.file.readAInt();
		leHeight = app.file.readAInt();
		leBackground = app.file.readAColor();
		leFlags = app.file.readAInt();
	}

	public int getMaskBits()
	{
		int num = 0;
		for (int i = 0; i < LOList.nIndex; i++)
		{
			CLO lOFromIndex = LOList.getLOFromIndex((short)i);
			if (lOFromIndex.loLayer > 0)
			{
				break;
			}
			COI oIFromHandle = app.OIList.getOIFromHandle(lOFromIndex.loOiHandle);
			if (oIFromHandle.oiType < 2)
			{
				COC oiOC = oIFromHandle.oiOC;
				switch (oiOC.ocObstacleType)
				{
				case 1:
					num |= 1;
					break;
				case 2:
					num |= 2;
					break;
				}
				continue;
			}
			CObjectCommon cObjectCommon = (CObjectCommon)oIFromHandle.oiOC;
			if ((cObjectCommon.ocOEFlags & 2) != 0)
			{
				switch ((cObjectCommon.ocFlags2 & 0x30) >> 4)
				{
				case 1:
					num |= 1;
					break;
				case 2:
					num |= 2;
					break;
				}
			}
		}
		return num;
	}

	public bool bkdLevObjCol_TestPoint(int x, int y, int nTestLayer, int nPlane)
	{
		CRect cRect = new CRect();
		int num;
		int num2;
		if (nTestLayer == -1)
		{
			num = 1;
			num2 = nLayers - 1;
		}
		else
		{
			if (nTestLayer >= nLayers)
			{
				return false;
			}
			num = (num2 = nTestLayer);
		}
		int num3 = leWidth;
		int num4 = leHeight;
		for (int i = num; i <= num2; i++)
		{
			CLayer cLayer = layers[i];
			bool flag = (cLayer.dwOptions & 0x20) != 0;
			bool flag2 = (cLayer.dwOptions & 0x40) != 0;
			bool flag3 = flag | flag2;
			int num5 = leX;
			int num6 = leY;
			if ((cLayer.dwOptions & 3) != 0)
			{
				if ((cLayer.dwOptions & 1) != 0)
				{
					num5 = (int)((float)num5 * cLayer.xCoef);
				}
				if ((cLayer.dwOptions & 2) != 0)
				{
					num6 = (int)((float)num6 * cLayer.yCoef);
				}
			}
			num5 += cLayer.x;
			num6 += cLayer.y;
			if (flag)
			{
				num5 %= num3;
			}
			if (flag2)
			{
				num6 %= num4;
			}
			uint num7 = 0u;
			int num8 = 0;
			int nBkdLOs = cLayer.nBkdLOs;
			for (int j = 0; j < nBkdLOs; j++)
			{
				CLO lOFromIndex = LOList.getLOFromIndex((short)(cLayer.nFirstLOIndex + j));
				CObject cObject = null;
				COI oIFromHandle = app.OIList.getOIFromHandle(lOFromIndex.loOiHandle);
				if (oIFromHandle == null || oIFromHandle.oiOC == null)
				{
					continue;
				}
				COC oiOC = oIFromHandle.oiOC;
				int oiType = oIFromHandle.oiType;
				cRect.left = lOFromIndex.loX - num5;
				cRect.top = lOFromIndex.loY - num6;
				int num9;
				int num10;
				if (oiType < 2)
				{
					num9 = oiOC.ocObstacleType;
					if (num9 == 0 || num9 == 3 || num9 == 4)
					{
						continue;
					}
					num10 = oiOC.ocColMode;
					cRect.right = cRect.left + oiOC.ocCx;
					cRect.bottom = cRect.top + oiOC.ocCy;
				}
				else
				{
					CObjectCommon cObjectCommon = (CObjectCommon)oiOC;
					if ((cObjectCommon.ocOEFlags & 2) == 0 || (cObject = rhPtr.find_HeaderObject(lOFromIndex.loHandle)) == null)
					{
						continue;
					}
					num9 = (cObjectCommon.ocFlags2 & 0x30) >> 4;
					if (num9 == 0 || num9 == 3 || num9 == 4)
					{
						continue;
					}
					num10 = (((cObjectCommon.ocFlags2 & 4) != 0) ? 1 : 0);
					cRect.left = cObject.hoX - leX - cObject.hoImgXSpot;
					cRect.top = cObject.hoY - leY - cObject.hoImgYSpot;
					cRect.right = cRect.left + cObject.hoImgWidth;
					cRect.bottom = cRect.top + cObject.hoImgHeight;
				}
				if (flag3)
				{
					switch (num8)
					{
					case 0:
						if (flag && (cRect.left < 0 || cRect.right > num3))
						{
							if (flag2 && (cRect.top < 0 || cRect.bottom > num4))
							{
								num8 = 3;
								num7 |= 7;
							}
							else
							{
								num8 = 1;
								num7 |= 1;
							}
						}
						else if (flag2 && (cRect.top < 0 || cRect.bottom > num4))
						{
							num8 = 2;
							num7 |= 2;
						}
						break;
					case 1:
						if (cRect.left < 0)
						{
							int num17 = num3;
							cRect.left += num17;
							cRect.right += num17;
						}
						else if (cRect.right > num3)
						{
							int num18 = num3;
							cRect.left -= num18;
							cRect.right -= num18;
						}
						num7 &= 0xFFFFFFFEu;
						num8 = 0;
						if ((num7 & 2) != 0)
						{
							num8 = 2;
						}
						break;
					case 2:
						if (cRect.top < 0)
						{
							int num15 = num4;
							cRect.top += num15;
							cRect.bottom += num15;
						}
						else if (cRect.bottom > num4)
						{
							int num16 = num4;
							cRect.top -= num16;
							cRect.bottom -= num16;
						}
						num7 &= 0xFFFFFFFDu;
						num8 = 0;
						if ((num7 & 1) != 0)
						{
							num8 = 1;
						}
						break;
					case 3:
						if (cRect.left < 0)
						{
							int num11 = num3;
							cRect.left += num11;
							cRect.right += num11;
						}
						else if (cRect.right > num3)
						{
							int num12 = num3;
							cRect.left -= num12;
							cRect.right -= num12;
						}
						if (cRect.top < 0)
						{
							int num13 = num4;
							cRect.top += num13;
							cRect.bottom += num13;
						}
						else if (cRect.bottom > num4)
						{
							int num14 = num4;
							cRect.top -= num14;
							cRect.bottom -= num14;
						}
						num7 &= 0xFFFFFFFBu;
						num8 = 2;
						break;
					}
				}
				if (x >= cRect.left && y >= cRect.top && x < cRect.right && y < cRect.bottom && (num9 != 2 || nPlane != 0))
				{
					if (num10 != 0)
					{
						return true;
					}
					int flags = 0;
					if (num9 == 2)
					{
						flags = 1;
					}
					CMask cMask = null;
					if (oiType < 2)
					{
						CImage imageFromHandle = app.imageBank.getImageFromHandle(((COCBackground)oiOC).ocImage);
						cMask = imageFromHandle.getMask(flags, 0, 1f, 1f);
					}
					else
					{
						cMask = cObject.getCollisionMask(flags);
					}
					if (cMask == null)
					{
						return true;
					}
					if (cMask.testPoint(x - cRect.left, y - cRect.top))
					{
						return true;
					}
				}
				if (num7 != 0)
				{
					j--;
				}
			}
			if (cLayer.pBkd2 == null)
			{
				continue;
			}
			num7 = 0u;
			num8 = 0;
			for (int j = 0; j < cLayer.pBkd2.size(); j++)
			{
				CBkd2 cBkd = (CBkd2)cLayer.pBkd2.get(j);
				cRect.left = cBkd.x - num5;
				cRect.top = cBkd.y - num6;
				int num9 = cBkd.obstacleType;
				if (num9 == 0 || num9 == 3 || num9 == 4)
				{
					continue;
				}
				int num10 = ((cBkd.colMode == 0) ? 1 : 0);
				CImage imageFromHandle = app.imageBank.getImageFromHandle(cBkd.img);
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
					switch (num8)
					{
					case 0:
						if (flag && (cRect.left < 0 || cRect.right > num3))
						{
							if (flag2 && (cRect.top < 0 || cRect.bottom > num4))
							{
								num8 = 3;
								num7 |= 7;
							}
							else
							{
								num8 = 1;
								num7 |= 1;
							}
						}
						else if (flag2 && (cRect.top < 0 || cRect.bottom > num4))
						{
							num8 = 2;
							num7 |= 2;
						}
						break;
					case 1:
						if (cRect.left < 0)
						{
							int num25 = num3;
							cRect.left += num25;
							cRect.right += num25;
						}
						else if (cRect.right > num3)
						{
							int num26 = num3;
							cRect.left -= num26;
							cRect.right -= num26;
						}
						num7 &= 0xFFFFFFFEu;
						num8 = 0;
						if ((num7 & 2) != 0)
						{
							num8 = 2;
						}
						break;
					case 2:
						if (cRect.top < 0)
						{
							int num23 = num4;
							cRect.top += num23;
							cRect.bottom += num23;
						}
						else if (cRect.bottom > num4)
						{
							int num24 = num4;
							cRect.top -= num24;
							cRect.bottom -= num24;
						}
						num7 &= 0xFFFFFFFDu;
						num8 = 0;
						if ((num7 & 1) != 0)
						{
							num8 = 1;
						}
						break;
					case 3:
						if (cRect.left < 0)
						{
							int num19 = num3;
							cRect.left += num19;
							cRect.right += num19;
						}
						else if (cRect.right > num3)
						{
							int num20 = num3;
							cRect.left -= num20;
							cRect.right -= num20;
						}
						if (cRect.top < 0)
						{
							int num21 = num4;
							cRect.top += num21;
							cRect.bottom += num21;
						}
						else if (cRect.bottom > num4)
						{
							int num22 = num4;
							cRect.top -= num22;
							cRect.bottom -= num22;
						}
						num7 &= 0xFFFFFFFBu;
						num8 = 2;
						break;
					}
				}
				if (x >= cRect.left && y >= cRect.top && x < cRect.right && y < cRect.bottom && (num9 != 2 || nPlane != 0))
				{
					if (num10 != 0)
					{
						return true;
					}
					int flags2 = 0;
					if (num9 == 2)
					{
						flags2 = 1;
					}
					imageFromHandle = app.imageBank.getImageFromHandle(cBkd.img);
					CMask cMask = imageFromHandle.getMask(flags2, 0, 1f, 1f);
					if (cMask != null && cMask.testPoint(x - cRect.left, y - cRect.top))
					{
						return true;
					}
				}
				if (num7 != 0)
				{
					j--;
				}
			}
		}
		return false;
	}

	public bool bkdLevObjCol_TestRect(int x, int y, int nWidth, int nHeight, int nTestLayer, int nPlane)
	{
		CRect cRect = new CRect();
		CMask cMask = null;
		int num;
		int num2;
		if (nTestLayer == -1)
		{
			num = 1;
			num2 = nLayers - 1;
		}
		else
		{
			if (nTestLayer >= nLayers)
			{
				return false;
			}
			num = (num2 = nTestLayer);
		}
		int num3 = leWidth;
		int num4 = leHeight;
		for (int i = num; i <= num2; i++)
		{
			CLayer cLayer = layers[i];
			bool flag = (cLayer.dwOptions & 0x20) != 0;
			bool flag2 = (cLayer.dwOptions & 0x40) != 0;
			bool flag3 = flag | flag2;
			int num5 = leX;
			int num6 = leY;
			if ((cLayer.dwOptions & 3) != 0)
			{
				if ((cLayer.dwOptions & 1) != 0)
				{
					num5 = (int)((float)num5 * cLayer.xCoef);
				}
				if ((cLayer.dwOptions & 2) != 0)
				{
					num6 = (int)((float)num6 * cLayer.yCoef);
				}
			}
			num5 += cLayer.x;
			num6 += cLayer.y;
			if (flag)
			{
				num5 %= num3;
			}
			if (flag2)
			{
				num6 %= num4;
			}
			uint num7 = 0u;
			int num8 = 0;
			int nBkdLOs = cLayer.nBkdLOs;
			for (int j = 0; j < nBkdLOs; j++)
			{
				CLO lOFromIndex = LOList.getLOFromIndex((short)(cLayer.nFirstLOIndex + j));
				CObject cObject = null;
				COI oIFromHandle = app.OIList.getOIFromHandle(lOFromIndex.loOiHandle);
				if (oIFromHandle == null || oIFromHandle.oiOC == null)
				{
					continue;
				}
				COC oiOC = oIFromHandle.oiOC;
				int oiType = oIFromHandle.oiType;
				cRect.left = lOFromIndex.loX - num5;
				cRect.top = lOFromIndex.loY - num6;
				int num9;
				int num10;
				if (oiType < 2)
				{
					num9 = oiOC.ocObstacleType;
					if (num9 == 0 || num9 == 3 || num9 == 4)
					{
						continue;
					}
					num10 = oiOC.ocColMode;
					cRect.right = cRect.left + oiOC.ocCx;
					cRect.bottom = cRect.top + oiOC.ocCy;
				}
				else
				{
					CObjectCommon cObjectCommon = (CObjectCommon)oiOC;
					if ((cObjectCommon.ocOEFlags & 2) == 0 || (cObject = rhPtr.find_HeaderObject(lOFromIndex.loHandle)) == null)
					{
						continue;
					}
					num9 = (cObjectCommon.ocFlags2 & 0x30) >> 4;
					if (num9 == 0 || num9 == 3 || num9 == 4)
					{
						continue;
					}
					num10 = (((cObjectCommon.ocFlags2 & 4) != 0) ? 1 : 0);
					cRect.left = cObject.hoX - leX - cObject.hoImgXSpot;
					cRect.top = cObject.hoY - leY - cObject.hoImgYSpot;
					cRect.right = cRect.left + cObject.hoImgWidth;
					cRect.bottom = cRect.top + cObject.hoImgHeight;
				}
				if (flag3)
				{
					switch (num8)
					{
					case 0:
						if (flag && (cRect.left < 0 || cRect.right > num3))
						{
							if (flag2 && (cRect.top < 0 || cRect.bottom > num4))
							{
								num8 = 3;
								num7 |= 7;
							}
							else
							{
								num8 = 1;
								num7 |= 1;
							}
						}
						else if (flag2 && (cRect.top < 0 || cRect.bottom > num4))
						{
							num8 = 2;
							num7 |= 2;
						}
						break;
					case 1:
						if (cRect.left < 0)
						{
							int num17 = num3;
							cRect.left += num17;
							cRect.right += num17;
						}
						else if (cRect.right > num3)
						{
							int num18 = num3;
							cRect.left -= num18;
							cRect.right -= num18;
						}
						num7 &= 0xFFFFFFFEu;
						num8 = 0;
						if ((num7 & 2) != 0)
						{
							num8 = 2;
						}
						break;
					case 2:
						if (cRect.top < 0)
						{
							int num15 = num4;
							cRect.top += num15;
							cRect.bottom += num15;
						}
						else if (cRect.bottom > num4)
						{
							int num16 = num4;
							cRect.top -= num16;
							cRect.bottom -= num16;
						}
						num7 &= 0xFFFFFFFDu;
						num8 = 0;
						if ((num7 & 1) != 0)
						{
							num8 = 1;
						}
						break;
					case 3:
						if (cRect.left < 0)
						{
							int num11 = num3;
							cRect.left += num11;
							cRect.right += num11;
						}
						else if (cRect.right > num3)
						{
							int num12 = num3;
							cRect.left -= num12;
							cRect.right -= num12;
						}
						if (cRect.top < 0)
						{
							int num13 = num4;
							cRect.top += num13;
							cRect.bottom += num13;
						}
						else if (cRect.bottom > num4)
						{
							int num14 = num4;
							cRect.top -= num14;
							cRect.bottom -= num14;
						}
						num7 &= 0xFFFFFFFBu;
						num8 = 2;
						break;
					}
				}
				if (x + nWidth > cRect.left && y + nHeight > cRect.top && x < cRect.right && y < cRect.bottom && (num9 != 2 || nPlane != 0))
				{
					if (num10 != 0)
					{
						return true;
					}
					int flags = 0;
					if (num9 == 2)
					{
						flags = 1;
					}
					if (oiType < 2)
					{
						CImage imageFromHandle = app.imageBank.getImageFromHandle(((COCBackground)oiOC).ocImage);
						cMask = imageFromHandle.getMask(flags, 0, 1f, 1f);
					}
					else
					{
						cMask = cObject.getCollisionMask(flags);
					}
					if (cMask == null)
					{
						return true;
					}
					if (cMask.testRect(0, x - cRect.left, y - cRect.top, nWidth, nHeight))
					{
						return true;
					}
				}
				if (num7 != 0)
				{
					j--;
				}
			}
			if (cLayer.pBkd2 == null)
			{
				continue;
			}
			num7 = 0u;
			num8 = 0;
			for (int j = 0; j < cLayer.pBkd2.size(); j++)
			{
				CBkd2 cBkd = (CBkd2)cLayer.pBkd2.get(j);
				cRect.left = cBkd.x - num5;
				cRect.top = cBkd.y - num6;
				int num9 = cBkd.obstacleType;
				if (num9 == 0 || num9 == 3 || num9 == 4)
				{
					continue;
				}
				int num10 = ((cBkd.colMode == 0) ? 1 : 0);
				CImage imageFromHandle = app.imageBank.getImageFromHandle(cBkd.img);
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
					switch (num8)
					{
					case 0:
						if (flag && (cRect.left < 0 || cRect.right > num3))
						{
							if (flag2 && (cRect.top < 0 || cRect.bottom > num4))
							{
								num8 = 3;
								num7 |= 7;
							}
							else
							{
								num8 = 1;
								num7 |= 1;
							}
						}
						else if (flag2 && (cRect.top < 0 || cRect.bottom > num4))
						{
							num8 = 2;
							num7 |= 2;
						}
						break;
					case 1:
						if (cRect.left < 0)
						{
							int num25 = num3;
							cRect.left += num25;
							cRect.right += num25;
						}
						else if (cRect.right > num3)
						{
							int num26 = num3;
							cRect.left -= num26;
							cRect.right -= num26;
						}
						num7 &= 0xFFFFFFFEu;
						num8 = 0;
						if ((num7 & 2) != 0)
						{
							num8 = 2;
						}
						break;
					case 2:
						if (cRect.top < 0)
						{
							int num23 = num4;
							cRect.top += num23;
							cRect.bottom += num23;
						}
						else if (cRect.bottom > num4)
						{
							int num24 = num4;
							cRect.top -= num24;
							cRect.bottom -= num24;
						}
						num7 &= 0xFFFFFFFDu;
						num8 = 0;
						if ((num7 & 1) != 0)
						{
							num8 = 1;
						}
						break;
					case 3:
						if (cRect.left < 0)
						{
							int num19 = num3;
							cRect.left += num19;
							cRect.right += num19;
						}
						else if (cRect.right > num3)
						{
							int num20 = num3;
							cRect.left -= num20;
							cRect.right -= num20;
						}
						if (cRect.top < 0)
						{
							int num21 = num4;
							cRect.top += num21;
							cRect.bottom += num21;
						}
						else if (cRect.bottom > num4)
						{
							int num22 = num4;
							cRect.top -= num22;
							cRect.bottom -= num22;
						}
						num7 &= 0xFFFFFFFBu;
						num8 = 2;
						break;
					}
				}
				if (x + nWidth > cRect.left && y + nHeight > cRect.top && x < cRect.right && y < cRect.bottom && (num9 != 2 || nPlane != 0))
				{
					if (num10 != 0)
					{
						return true;
					}
					int flags2 = 0;
					if (num9 == 2)
					{
						flags2 = 1;
					}
					imageFromHandle = app.imageBank.getImageFromHandle(cBkd.img);
					cMask = imageFromHandle.getMask(flags2, 0, 1f, 1f);
					if (cMask != null && cMask.testRect(0, x - cRect.left, y - cRect.top, nWidth, nHeight))
					{
						return true;
					}
				}
				if (num7 != 0)
				{
					j--;
				}
			}
		}
		return false;
	}

	public bool bkdLevObjCol_TestSprite(CSprite pSpr, short newImg, int newX, int newY, int newAngle, float newScaleX, float newScaleY, int subHt, int nPlane)
	{
		CRect cRect = new CRect();
		int num = pSpr.sprLayer / 2;
		CLayer cLayer = layers[num];
		bool flag = (cLayer.dwOptions & 0x20) != 0;
		bool flag2 = (cLayer.dwOptions & 0x40) != 0;
		bool flag3 = flag | flag2;
		int num2 = leWidth;
		int num3 = leHeight;
		int num4 = leX;
		int num5 = leY;
		if ((cLayer.dwOptions & 3) != 0)
		{
			if ((cLayer.dwOptions & 1) != 0)
			{
				num4 = (int)((float)num4 * cLayer.xCoef);
			}
			if ((cLayer.dwOptions & 2) != 0)
			{
				num5 = (int)((float)num5 * cLayer.yCoef);
			}
		}
		num4 += cLayer.x;
		num5 += cLayer.y;
		if (flag)
		{
			num4 %= num2;
		}
		if (flag2)
		{
			num5 %= num3;
		}
		uint sprFlags = pSpr.sprFlags;
		bool flag4 = (sprFlags & 0x100) != 0;
		CRect cRect2 = new CRect();
		int num6 = 0;
		int num7 = 0;
		int num8 = newImg;
		cRect2.left = newX;
		cRect2.top = newY;
		if (newImg == 0)
		{
			num8 = pSpr.sprImg;
		}
		CMask cMask = null;
		CMask cMask2 = null;
		int num9 = 0;
		if (!flag4)
		{
			cMask = app.spriteGen.getSpriteMask(pSpr, (short)num8, 0, 0, 1f, 1f);
			if (cMask == null)
			{
				cRect2.left = pSpr.sprX1new;
				cRect2.right = pSpr.sprX2new;
				cRect2.top = pSpr.sprY1new;
				cRect2.bottom = pSpr.sprY2new;
				num6 = cRect2.right - cRect2.left;
				num7 = cRect2.bottom - cRect2.top;
				flag4 = true;
			}
			else
			{
				if ((pSpr.sprFlags & 0x400000) == 0)
				{
					cRect2.left -= cMask.xSpot;
					cRect2.top -= cMask.ySpot;
				}
				num6 = cMask.width;
				num7 = cMask.height;
				cRect2.right = cRect2.left + num6;
				cRect2.bottom = cRect2.top + num7;
			}
		}
		else if (num8 == 0 || num8 == pSpr.sprImg || (sprFlags & 0x2000) != 0)
		{
			cRect2.left = pSpr.sprX1new;
			cRect2.right = pSpr.sprX2new;
			cRect2.top = pSpr.sprY1new;
			cRect2.bottom = pSpr.sprY2new;
			num6 = cRect2.right - cRect2.left;
			num7 = cRect2.bottom - cRect2.top;
		}
		else
		{
			CImage imageFromHandle = app.imageBank.getImageFromHandle((short)num8);
			if (imageFromHandle != null)
			{
				cRect2.left -= imageFromHandle.xSpot;
				cRect2.top -= imageFromHandle.ySpot;
				num6 = imageFromHandle.width;
				num7 = imageFromHandle.height;
				cRect2.right = cRect2.left + num6;
				cRect2.bottom = cRect2.top + num7;
			}
			else
			{
				cRect2.left = pSpr.sprX1new;
				cRect2.right = pSpr.sprX2new;
				cRect2.top = pSpr.sprY1new;
				cRect2.bottom = pSpr.sprY2new;
				num6 = cRect2.right - cRect2.left;
				num7 = cRect2.bottom - cRect2.top;
			}
		}
		if (subHt != 0)
		{
			if (subHt > num7)
			{
				subHt = num7;
			}
			cRect2.top += num7 - subHt;
			if (cMask != null)
			{
				num9 = num7 - subHt;
			}
			num7 = subHt;
		}
		uint num10 = 0u;
		int num11 = 0;
		int nBkdLOs = cLayer.nBkdLOs;
		for (int i = 0; i < nBkdLOs; i++)
		{
			CLO lOFromIndex = LOList.getLOFromIndex((short)(cLayer.nFirstLOIndex + i));
			COI oIFromHandle = app.OIList.getOIFromHandle(lOFromIndex.loOiHandle);
			if (oIFromHandle == null || oIFromHandle.oiOC == null)
			{
				continue;
			}
			COC oiOC = oIFromHandle.oiOC;
			int oiType = oIFromHandle.oiType;
			cRect.left = lOFromIndex.loX - num4;
			cRect.top = lOFromIndex.loY - num5;
			CObject cObject = null;
			int num12;
			int num13;
			if (oiType < 2)
			{
				num12 = oiOC.ocObstacleType;
				if (num12 == 0 || num12 == 3 || num12 == 4)
				{
					continue;
				}
				num13 = oiOC.ocColMode;
				cRect.right = cRect.left + oiOC.ocCx;
				cRect.bottom = cRect.top + oiOC.ocCy;
			}
			else
			{
				CObjectCommon cObjectCommon = (CObjectCommon)oiOC;
				if ((cObjectCommon.ocOEFlags & 2) == 0 || (cObject = rhPtr.find_HeaderObject(lOFromIndex.loHandle)) == null)
				{
					continue;
				}
				num12 = (cObjectCommon.ocFlags2 & 0x30) >> 4;
				if (num12 == 0 || num12 == 3 || num12 == 4)
				{
					continue;
				}
				num13 = (((cObjectCommon.ocFlags2 & 4) != 0) ? 1 : 0);
				cRect.left = cObject.hoX - leX - cObject.hoImgXSpot;
				cRect.top = cObject.hoY - leY - cObject.hoImgYSpot;
				cRect.right = cRect.left + cObject.hoImgWidth;
				cRect.bottom = cRect.top + cObject.hoImgHeight;
			}
			if (flag3)
			{
				switch (num11)
				{
				case 0:
					if (flag && (cRect.left < 0 || cRect.right > num2))
					{
						if (flag2 && (cRect.top < 0 || cRect.bottom > num3))
						{
							num11 = 3;
							num10 |= 7;
						}
						else
						{
							num11 = 1;
							num10 |= 1;
						}
					}
					else if (flag2 && (cRect.top < 0 || cRect.bottom > num3))
					{
						num11 = 2;
						num10 |= 2;
					}
					break;
				case 1:
					if (cRect.left < 0)
					{
						int num20 = num2;
						cRect.left += num20;
						cRect.right += num20;
					}
					else if (cRect.right > num2)
					{
						int num21 = num2;
						cRect.left -= num21;
						cRect.right -= num21;
					}
					num10 &= 0xFFFFFFFEu;
					num11 = 0;
					if ((num10 & 2) != 0)
					{
						num11 = 2;
					}
					break;
				case 2:
					if (cRect.top < 0)
					{
						int num18 = num3;
						cRect.top += num18;
						cRect.bottom += num18;
					}
					else if (cRect.bottom > num3)
					{
						int num19 = num3;
						cRect.top -= num19;
						cRect.bottom -= num19;
					}
					num10 &= 0xFFFFFFFDu;
					num11 = 0;
					if ((num10 & 1) != 0)
					{
						num11 = 1;
					}
					break;
				case 3:
					if (cRect.left < 0)
					{
						int num14 = num2;
						cRect.left += num14;
						cRect.right += num14;
					}
					else if (cRect.right > num2)
					{
						int num15 = num2;
						cRect.left -= num15;
						cRect.right -= num15;
					}
					if (cRect.top < 0)
					{
						int num16 = num3;
						cRect.top += num16;
						cRect.bottom += num16;
					}
					else if (cRect.bottom > num3)
					{
						int num17 = num3;
						cRect.top -= num17;
						cRect.bottom -= num17;
					}
					num10 &= 0xFFFFFFFBu;
					num11 = 2;
					break;
				}
			}
			if (cRect2.right > cRect.left && cRect2.bottom > cRect.top && cRect2.left < cRect.right && cRect2.top < cRect.bottom && (num12 != 2 || nPlane != 0))
			{
				if (num13 != 0)
				{
					if (flag4)
					{
						return true;
					}
					if (cMask == null)
					{
						if (cMask == null)
						{
							return true;
						}
						num9 = 0;
						if (subHt != 0)
						{
							if (subHt > num7)
							{
								subHt = num7;
							}
							num9 = num7 - subHt;
						}
					}
					if (cMask.testRect(num9, cRect.left - cRect2.left, cRect.top - cRect2.top, cRect.right - cRect.left, cRect.bottom - cRect.top))
					{
						return true;
					}
				}
				else
				{
					int flags = 0;
					if (num12 == 2)
					{
						flags = 1;
					}
					cMask2 = null;
					if (oiType < 2)
					{
						CImage imageFromHandle = app.imageBank.getImageFromHandle(((COCBackground)oiOC).ocImage);
						cMask2 = imageFromHandle.getMask(flags, 0, 1f, 1f);
					}
					else
					{
						cMask2 = cObject.getCollisionMask(flags);
					}
					if (flag4)
					{
						if (cMask2 == null)
						{
							return true;
						}
						if (cMask2.testRect(0, cRect2.left - cRect.left, cRect2.top - cRect.top, num6, num7))
						{
							return true;
						}
					}
					else
					{
						num9 = 0;
						if (subHt != 0)
						{
							if (subHt > num7)
							{
								subHt = num7;
							}
							num9 = num7 - subHt;
						}
						if (cMask2 == null)
						{
							if (cMask.testRect(num9, cRect.left - cRect2.left, cRect.top - cRect2.top, cRect.right - cRect.left, cRect.bottom - cRect.top))
							{
								return true;
							}
						}
						else if (cMask == null)
						{
							if (cMask2.testRect(0, cRect2.left - cRect.left, cRect2.top - cRect.top, num6, num7))
							{
								return true;
							}
						}
						else if (cMask2.testMask(0, cRect.left, cRect.top, cMask, num9, cRect2.left, cRect2.top))
						{
							return true;
						}
					}
				}
			}
			if (num10 != 0)
			{
				i--;
			}
		}
		if (cLayer.pBkd2 != null)
		{
			num10 = 0u;
			num11 = 0;
			for (int i = 0; i < cLayer.pBkd2.size(); i++)
			{
				CBkd2 cBkd = (CBkd2)cLayer.pBkd2.get(i);
				cRect.left = cBkd.x - num4;
				cRect.top = cBkd.y - num5;
				int num12 = cBkd.obstacleType;
				if (num12 == 0 || num12 == 3 || num12 == 4)
				{
					continue;
				}
				int num13 = ((cBkd.colMode == 0) ? 1 : 0);
				CImage imageFromHandle = app.imageBank.getImageFromHandle(cBkd.img);
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
					switch (num11)
					{
					case 0:
						if (flag && (cRect.left < 0 || cRect.right > num2))
						{
							if (flag2 && (cRect.top < 0 || cRect.bottom > num3))
							{
								num11 = 3;
								num10 |= 7;
							}
							else
							{
								num11 = 1;
								num10 |= 1;
							}
						}
						else if (flag2 && (cRect.top < 0 || cRect.bottom > num3))
						{
							num11 = 2;
							num10 |= 2;
						}
						break;
					case 1:
						if (cRect.left < 0)
						{
							int num28 = num2;
							cRect.left += num28;
							cRect.right += num28;
						}
						else if (cRect.right > num2)
						{
							int num29 = num2;
							cRect.left -= num29;
							cRect.right -= num29;
						}
						num10 &= 0xFFFFFFFEu;
						num11 = 0;
						if ((num10 & 2) != 0)
						{
							num11 = 2;
						}
						break;
					case 2:
						if (cRect.top < 0)
						{
							int num26 = num3;
							cRect.top += num26;
							cRect.bottom += num26;
						}
						else if (cRect.bottom > num3)
						{
							int num27 = num3;
							cRect.top -= num27;
							cRect.bottom -= num27;
						}
						num10 &= 0xFFFFFFFDu;
						num11 = 0;
						if ((num10 & 1) != 0)
						{
							num11 = 1;
						}
						break;
					case 3:
						if (cRect.left < 0)
						{
							int num22 = num2;
							cRect.left += num22;
							cRect.right += num22;
						}
						else if (cRect.right > num2)
						{
							int num23 = num2;
							cRect.left -= num23;
							cRect.right -= num23;
						}
						if (cRect.top < 0)
						{
							int num24 = num3;
							cRect.top += num24;
							cRect.bottom += num24;
						}
						else if (cRect.bottom > num3)
						{
							int num25 = num3;
							cRect.top -= num25;
							cRect.bottom -= num25;
						}
						num10 &= 0xFFFFFFFBu;
						num11 = 2;
						break;
					}
				}
				if (cRect2.right > cRect.left && cRect2.bottom > cRect.top && cRect2.left < cRect.right && cRect2.top < cRect.bottom && (num12 != 2 || nPlane != 0))
				{
					if (num13 != 0)
					{
						if (flag4)
						{
							return true;
						}
						imageFromHandle = app.imageBank.getImageFromHandle(cBkd.img);
						cMask2 = imageFromHandle.getMask(0, 0, 1f, 1f);
						if (cMask == null)
						{
							return true;
						}
						num9 = 0;
						if (subHt != 0)
						{
							if (subHt > num7)
							{
								subHt = num7;
							}
							num9 = num7 - subHt;
						}
						if (cMask.testRect(num9, cRect.left - cRect2.left, cRect.top - cRect2.top, cRect.right - cRect.left, cRect.bottom - cRect.top))
						{
							return true;
						}
					}
					else
					{
						int flags2 = 0;
						if (num12 == 2)
						{
							flags2 = 1;
						}
						imageFromHandle = app.imageBank.getImageFromHandle(cBkd.img);
						cMask2 = imageFromHandle.getMask(flags2, 0, 1f, 1f);
						if (cMask2 != null)
						{
							if (flag4)
							{
								if (cMask2.testRect(0, cRect2.left - cRect.left, cRect2.top - cRect.top, num6, num7))
								{
									return true;
								}
							}
							else
							{
								if (cMask == null)
								{
									return true;
								}
								num9 = 0;
								if (subHt != 0)
								{
									if (subHt > num7)
									{
										subHt = num7;
									}
									num9 = num7 - subHt;
								}
								if (cMask2.testMask(0, cRect.left, cRect.top, cMask, num9, cRect2.left, cRect2.top))
								{
									return true;
								}
							}
						}
					}
				}
				if (num10 != 0)
				{
					i--;
				}
			}
		}
		return false;
	}

	public bool bkdCol_TestPoint(int x, int y, int nLayer, int nPlane)
	{
		switch (nLayer)
		{
		case -1:
		{
			CLayer cLayer = layers[0];
			if ((leFlags & 0x20) != 0 && (cLayer.dwOptions & 0x60) != 0)
			{
				if (bkdLevObjCol_TestPoint(x, y, 0, nPlane))
				{
					return true;
				}
				return false;
			}
			if (colMask != null && colMask.testPoint(x, y, nPlane))
			{
				return true;
			}
			if (nLayers == 1)
			{
				return false;
			}
			if ((leFlags & 0x20) != 0)
			{
				return bkdLevObjCol_TestPoint(x, y, nLayer, nPlane);
			}
			int num = 8;
			num = ((nPlane != 1) ? (num | 1) : (num | 2));
			return app.spriteGen.spriteCol_TestPoint(null, (short)nLayer, x, y, num) != null;
		}
		case 0:
		{
			CLayer cLayer = layers[0];
			if ((leFlags & 0x20) != 0 && (cLayer.dwOptions & 0x60) != 0)
			{
				if (bkdLevObjCol_TestPoint(x, y, 0, nPlane))
				{
					return true;
				}
				return false;
			}
			return colMask.testPoint(x, y, nPlane);
		}
		default:
		{
			if (nLayers == 1)
			{
				return false;
			}
			if ((leFlags & 0x20) != 0)
			{
				return bkdLevObjCol_TestPoint(x, y, nLayer, nPlane);
			}
			int num = 8;
			num = ((nPlane != 1) ? (num | 1) : (num | 2));
			return app.spriteGen.spriteCol_TestPoint(null, -1, x, y, num) != null;
		}
		}
	}

	public bool bkdCol_TestRect(int x, int y, int nWidth, int nHeight, int nLayer, int nPlane)
	{
		switch (nLayer)
		{
		case -1:
		{
			CLayer cLayer = layers[0];
			if ((leFlags & 0x20) != 0 && (cLayer.dwOptions & 0x60) != 0)
			{
				if (bkdLevObjCol_TestRect(x, y, nWidth, nHeight, 0, nPlane))
				{
					return true;
				}
				return false;
			}
			if (colMask.testRect(x, y, nWidth, nHeight, nPlane))
			{
				return true;
			}
			if (nLayers == 1)
			{
				return false;
			}
			if ((leFlags & 0x20) != 0)
			{
				if (bkdLevObjCol_TestRect(x, y, nWidth, nHeight, nLayer, nPlane))
				{
					return true;
				}
				return false;
			}
			int num = 8;
			num = ((nPlane != 1) ? (num | 1) : (num | 2));
			if (app.spriteGen.spriteCol_TestRect(null, nLayer, x, y, nWidth, nHeight, num) != null)
			{
				return true;
			}
			return false;
		}
		case 0:
		{
			CLayer cLayer = layers[0];
			if ((leFlags & 0x20) != 0 && (cLayer.dwOptions & 0x60) != 0)
			{
				if (bkdLevObjCol_TestRect(x, y, nWidth, nHeight, 0, nPlane))
				{
					return true;
				}
				return false;
			}
			if (colMask.testRect(x, y, nWidth, nHeight, nPlane))
			{
				return true;
			}
			return false;
		}
		default:
		{
			if (nLayers == 1)
			{
				return false;
			}
			if ((leFlags & 0x20) != 0)
			{
				if (bkdLevObjCol_TestRect(x, y, nWidth, nHeight, nLayer, nPlane))
				{
					return true;
				}
				return false;
			}
			int num = 8;
			num = ((nPlane != 1) ? (num | 1) : (num | 2));
			return app.spriteGen.spriteCol_TestRect(null, -1, x, y, nWidth, nHeight, num) != null;
		}
		}
	}

	public bool bkdCol_TestSprite(CSprite pSpr, int newImg, int newX, int newY, int newAngle, float newScaleX, float newScaleY, int subHt, int nPlane)
	{
		if (pSpr.sprLayer / 2 == 0)
		{
			CLayer cLayer = layers[0];
			if ((leFlags & 0x20) != 0 && (cLayer.dwOptions & 0x60) != 0)
			{
				if (bkdLevObjCol_TestSprite(pSpr, (short)newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, nPlane))
				{
					return true;
				}
				return false;
			}
			if (colMask_TestSprite(pSpr, newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, nPlane))
			{
				return true;
			}
			return false;
		}
		if (nLayers == 1)
		{
			return false;
		}
		if ((leFlags & 0x20) != 0)
		{
			if (bkdLevObjCol_TestSprite(pSpr, (short)newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, nPlane))
			{
				return true;
			}
			return false;
		}
		uint num = 8u;
		num = ((nPlane != 1) ? (num | 1) : (num | 2));
		return app.spriteGen.spriteCol_TestSprite(pSpr, (short)newImg, newX, newY, newAngle, newScaleX, newScaleY, subHt, num) != null;
	}

	public bool colMask_TestSprite(CSprite pSpr, int newImg, int newX, int newY, int newAngle, float newScaleX, float newScaleY, int subHt, int nPlane)
	{
		if (pSpr == null || colMask == null)
		{
			return false;
		}
		int num = newImg;
		int num2 = newX;
		int num3 = newY;
		int colMode = app.spriteGen.colMode;
		new CRect();
		if (newImg == 0)
		{
			num = pSpr.sprImg;
		}
		int w;
		int num4;
		if (colMode != 0 && (pSpr.sprFlags & 0x100) == 0)
		{
			CMask cMask = null;
			cMask = app.spriteGen.getSpriteMask(pSpr, (short)num, 0, newAngle, newScaleX, newScaleY);
			if (cMask == null)
			{
				num2 -= pSpr.sprX - pSpr.sprX1;
				num3 -= pSpr.sprY - pSpr.sprY1;
				w = pSpr.sprX2 - pSpr.sprX1;
				num4 = pSpr.sprY2 - pSpr.sprY1;
			}
			else
			{
				if ((pSpr.sprFlags & 0x400000) == 0)
				{
					num2 -= cMask.xSpot;
					num3 -= cMask.ySpot;
				}
				w = cMask.width;
				num4 = cMask.height;
			}
			if (cMask != null)
			{
				int yBase = 0;
				if (subHt != 0)
				{
					if (subHt > num4)
					{
						subHt = num4;
					}
					num3 += num4 - subHt;
					yBase = num4 - subHt;
					num4 = subHt;
				}
				return colMask.testMask(cMask, yBase, num2, num3, nPlane);
			}
		}
		else if (num == 0 || num == pSpr.sprImg || (pSpr.sprFlags & 0x2000) != 0)
		{
			num2 -= pSpr.sprX - pSpr.sprX1;
			num3 -= pSpr.sprY - pSpr.sprY1;
			w = pSpr.sprX2 - pSpr.sprX1;
			num4 = pSpr.sprY2 - pSpr.sprY1;
		}
		else
		{
			CImage imageFromHandle = app.imageBank.getImageFromHandle((short)num);
			if (imageFromHandle != null)
			{
				num2 -= imageFromHandle.xSpot;
				num3 -= imageFromHandle.ySpot;
				w = imageFromHandle.width;
				num4 = imageFromHandle.height;
			}
			else
			{
				num2 -= pSpr.sprX - pSpr.sprX1;
				num3 -= pSpr.sprY - pSpr.sprY1;
				w = pSpr.sprX2 - pSpr.sprX1;
				num4 = pSpr.sprY2 - pSpr.sprY1;
			}
		}
		if (subHt != 0)
		{
			if (subHt > num4)
			{
				subHt = num4;
			}
			num3 += num4 - subHt;
			num4 = subHt;
		}
		return colMask.testRect(num2, num3, w, num4, nPlane);
	}
}
