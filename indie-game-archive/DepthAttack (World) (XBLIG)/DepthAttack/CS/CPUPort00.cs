using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DepthAttack.CS;

public class CPUPort00(Game game) : GameComponent(game)
{
	private int intStage;

	private int intTime;

	private int intCPUTime;

	private int intCPUBOSSTime;

	private int intCPUCount;

	private int intCPUGekihaCount;

	private Random rnd = new Random();

	private bool flgSubBossSyutugen = false;

	private bool flgSubBossTaosita = false;

	private bool flgBossSyutugen = false;

	public int pGetTime()
	{
		return intTime;
	}

	public int pGetCpuCount()
	{
		return intCPUCount;
	}

	public void pCpuGekihaCount()
	{
		if (!flgBossSyutugen)
		{
			intCPUGekihaCount++;
		}
	}

	public int pGetGekihaCount()
	{
		return intCPUGekihaCount;
	}

	public override void Initialize()
	{
		intStage = 0;
		intCPUTime = 0;
		intCPUBOSSTime = 0;
		intTime = 0;
		flgSubBossSyutugen = false;
		flgSubBossTaosita = false;
		flgBossSyutugen = false;
		intCPUCount = 0;
		intCPUGekihaCount = 0;
		base.Initialize();
	}

	public void pStage(int aintStage)
	{
		intStage = aintStage;
	}

	public void pSubBossTaosita()
	{
		flgSubBossTaosita = true;
	}

	public void pPortUpDate()
	{
		switch (intStage)
		{
		case 0:
			Stage00PortUpDate();
			break;
		case 1:
			Stage01PortUpDate();
			break;
		case 2:
			Stage02PortUpDate();
			break;
		case 3:
			Stage03PortUpDate();
			break;
		case 4:
			Stage04PortUpDate();
			break;
		}
	}

	public bool pIsStageClear()
	{
		if (intStage != 0)
		{
			if (intStage == 1)
			{
				if (intCPUTime > 9960 && !Game1.cPUBOSS00.psrcCPUBOSS00Core[0].pflgEnable)
				{
					return true;
				}
			}
			else if (intStage == 2)
			{
				if (intCPUTime > 9600 && !Game1.cPUBOSS00.psrcCPUBOSS00Core[0].pflgEnable)
				{
					return true;
				}
			}
			else if (intStage == 3)
			{
				if (intCPUTime > 8400 && !Game1.cPUBOSS00.psrcCPUBOSS00Core[0].pflgEnable)
				{
					return true;
				}
			}
			else if (intStage == 4 && intCPUTime > 6960 && !Game1.cPUBOSS00.psrcCPUBOSS00Core[0].pflgEnable)
			{
				return true;
			}
		}
		return false;
	}

	private void Stage00PortUpDate()
	{
		if (intTime > 27600)
		{
			intTime = 0;
		}
		else
		{
			intTime++;
		}
		if (intTime == 180)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 0f, 1f), CPU00.enuCPU00Type.intNormal00);
		}
		if (intTime == 210)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 0f, 1f), CPU00.enuCPU00Type.intNormal00);
		}
		if (intTime == 240)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 0f, 1f), CPU00.enuCPU00Type.intNormal00);
		}
		if (intTime == 360)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 0f, 0.5f), CPU00.enuCPU00Type.intNormal00);
		}
		if (intTime == 390)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 0f, 0.5f), CPU00.enuCPU00Type.intNormal00);
		}
		if (intTime == 420)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 0f, 0.5f), CPU00.enuCPU00Type.intNormal00);
		}
		if (intTime == 540)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00);
			Game1.cPU00.pCPU00Enable(new Vector3(350f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00);
			Game1.cPU00.pCPU00Enable(new Vector3(200f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00);
		}
		if (intTime == 660)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, -150f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, 0f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, 150f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90);
		}
		if (intTime == 780)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-200f, 300f, 0.2f), CPU00.enuCPU00Type.intHoudai00);
		}
		if (intTime == 870)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-350f, 300f, 0.2f), CPU00.enuCPU00Type.intHoudai00);
		}
		if (intTime == 960)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-200f, 300f, 0.2f), CPU00.enuCPU00Type.intHoudai00);
		}
		if (intTime == 1140)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(500f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
		}
		if (intTime == 1230)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(350f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
		}
		if (intTime == 1320)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(200f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
		}
		if (intTime == 1500)
		{
			for (int i = 0; i < 3; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-200 + i * 300, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
			}
		}
		if (intTime == 1620)
		{
			for (int i = 0; i < 3; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-200 + i * 300, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
			}
		}
		if (intTime == 1740)
		{
			for (int i = 0; i < 3; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-200 + i * 300, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 1800 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-200f, -300f, 0.15f), CPU00.enuCPU00Type.intInvader00);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 2160 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(200f, -300f, 0.15f), CPU00.enuCPU00Type.intInvaderX00);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 2520 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBack00);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 2820 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBack00_y);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 3180 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBackX00);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 3540 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBackX00_y);
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (intTime == 3900 + i * 30)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * 300, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (intTime == 4140 + i * 30)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * -300, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 4440 + i * 30)
			{
				Game1.item.pItemEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Item.enuItemType.intLifeRecover00);
			}
		}
		if (intTime == 4830)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 - 300, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBee00);
				}
			}
		}
		if (intTime == 5310)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 + 100, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBeeX00);
				}
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 5880 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFront00);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 6120 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_y);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 6420 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 6600 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_y);
			}
		}
		if (intTime == 6900)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(350f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(200f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
		}
		if (intTime == 7080)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, -150f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90_y);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, 0f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90_y);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, 150f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90_y);
		}
		if (intTime == 7380)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, -150f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, 0f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, 150f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270);
		}
		if (intTime == 7680)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, -150f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270_y);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, 0f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270_y);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, 150f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270_y);
		}
		if (intTime == 7980)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, -150f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270_r);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, 0f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270_r);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, 150f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270_r);
		}
		if (intTime == 8220)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, -150f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90_r);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, 0f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90_r);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, 150f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90_r);
		}
		if (intTime == 8460)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_r);
			Game1.cPU00.pCPU00Enable(new Vector3(350f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_r);
			Game1.cPU00.pCPU00Enable(new Vector3(200f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_r);
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 8700 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBack00_r);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 9000 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBackX00_r);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 9240 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_r);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intTime == 9480 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_r);
			}
		}
		if (intTime == 9720)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(300f, 150f, 0f), CPU00.enuCPU00Type.intBeetle00);
		}
		if (intTime == 9720)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, 150f, 0f), CPU00.enuCPU00Type.intBeetleX00);
		}
		if (intTime == 9960)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(300f, 0f, 0f), CPU00.enuCPU00Type.intBeetle00);
		}
		if (intTime == 9960)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, 0f, 0f), CPU00.enuCPU00Type.intBeetleX00);
		}
		if (intTime == 10200)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(300f, -150f, 0f), CPU00.enuCPU00Type.intBeetle00);
		}
		if (intTime == 10200)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, -150f, 0f), CPU00.enuCPU00Type.intBeetleX00);
		}
		for (int i = 0; i < 3; i++)
		{
			if (intTime == 10500 + i * 30)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(1200f, -150 + i * 150, 0.7f), CPU00.enuCPU00Type.intTonbo00);
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (intTime == 10740 + i * 30)
			{
				Game1.cPU00.pCPU00SPEnable(new Vector3(-1200f, -150 + i * 150, 0.7f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intTonboX00);
			}
		}
		if (intTime == 11160)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(0f, 0f, 0.04609375f), CPUBOSS00.enuCPUBOSS00Type.intOSPREY00, 500f, 10f);
		}
		if (intTime == 12900)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(300f, 50f, 0.1f), CPUBOSS00.enuCPUBOSS00Type.intDragon, 1000f, 10f);
		}
		if (intTime == 15300)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(0f, -2250f, 0.2f), CPUBOSS00.enuCPUBOSS00Type.intHatiNosu, 500f, 10f);
		}
		if (intTime == 16800)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(0f, -2250f, 0.2f), CPUBOSS00.enuCPUBOSS00Type.intQueenBee, 1000f, 10f);
		}
		if (intTime == 18720)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(2000f, -350f, 0.3f), CPUBOSS00.enuCPUBOSS00Type.intArship, 1000f, 10f);
		}
		if (intTime == 22320)
		{
			Game1.bG.penuBGSelect = BG.enuBGScene.Main02;
		}
		if (intTime == 22560)
		{
			Game1.bakuhatu.psrcBakuhatuCoreEnable(new Vector3(0f, -200f, 0.6f), 0f, Bakuhatu.enuBakuhatuType.intKaminari00, 0);
			Game1.bakuhatu.psrcBakuhatuCoreEnable(new Vector3(500f, -200f, 0.9f), 0f, Bakuhatu.enuBakuhatuType.intKaminari01, 60);
			Game1.bakuhatu.psrcBakuhatuCoreEnable(new Vector3(400f, -200f, 0.9f), 0f, Bakuhatu.enuBakuhatuType.intKaminari01, 70);
			for (int i = 0; i < 10; i++)
			{
				Game1.bakuhatu.psrcBakuhatuCoreEnable(new Vector3(-1000 + rnd.Next(1, 10) * 50, -400 + rnd.Next(1, 10) * 20, (float)rnd.Next(1, 20) / 20f), 0f, Bakuhatu.enuBakuhatuType.intKaminari00, rnd.Next(1, 90));
			}
			for (int i = 0; i < 10; i++)
			{
				Game1.bakuhatu.psrcBakuhatuCoreEnable(new Vector3(rnd.Next(1, 20) * 50, -400 + rnd.Next(1, 10) * 20, (float)rnd.Next(1, 20) / 20f), 0f, Bakuhatu.enuBakuhatuType.intKaminari01, rnd.Next(1, 90));
			}
		}
		if (intTime == 22580)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(0f, -1250f, 0f), CPUBOSS00.enuCPUBOSS00Type.intESP, 1000f, 10f);
		}
	}

	private void Stage01PortUpDate()
	{
		intTime++;
		if (!flgSubBossSyutugen || flgSubBossTaosita)
		{
			intCPUTime++;
		}
		if (!flgSubBossSyutugen || (flgSubBossTaosita && !flgBossSyutugen))
		{
			Game1.score.pScoreUp(1L);
		}
		if (intCPUTime == 5)
		{
			Game1.bGM.pflgBGMON(0);
		}
		if (intCPUTime == 120)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 200f, 0.3f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 200f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 200f, -0.3f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		if (intCPUTime == 360)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-700f, -150f, 0f), 90f, CPU00.enuCPU00Type.intTower00R90);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-700f, 0f, 0.15f), 90f, CPU00.enuCPU00Type.intTower00R90);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-700f, 150f, 0.3f), 90f, CPU00.enuCPU00Type.intTower00R90);
			intCPUCount += 3;
		}
		if (intCPUTime == 600)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0.3f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 200f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 200f, -0.3f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 300f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 200f, -0.3f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		if (intCPUTime == 720)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(700f, -150f, 0f), 270f, CPU00.enuCPU00Type.intTower00R270);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(700f, 0f, 0.15f), 270f, CPU00.enuCPU00Type.intTower00R270);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(700f, 150f, 0.3f), 270f, CPU00.enuCPU00Type.intTower00R270);
			intCPUCount += 3;
		}
		if (intCPUTime == 900)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-700f, -150f, 0f), 90f, CPU00.enuCPU00Type.intTower00R90);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-700f, 0f, 0.15f), 90f, CPU00.enuCPU00Type.intTower00R90);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-700f, 150f, 0.3f), 90f, CPU00.enuCPU00Type.intTower00R90);
			intCPUCount += 3;
		}
		if (intCPUTime == 1080)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-200f, 300f, 0.1f), CPU00.enuCPU00Type.intHoudai00);
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 300f, 0.3f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			intCPUCount++;
		}
		if (intCPUTime == 1170)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-350f, 300f, 0.1f), CPU00.enuCPU00Type.intHoudai00);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 200f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-100f, 200f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			intCPUCount++;
		}
		if (intCPUTime == 1260)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-200f, 300f, 0.1f), CPU00.enuCPU00Type.intHoudai00);
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			intCPUCount++;
		}
		if (intCPUTime == 1440)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(500f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			intCPUCount++;
		}
		if (intCPUTime == 1530)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(350f, 300f, 0.3f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 200f, -0.1f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 200f, -0.1f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			intCPUCount++;
		}
		if (intCPUTime == 1620)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(200f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			intCPUCount++;
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 1800 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBackX00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 2220 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBack00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 2640 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBackX00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 3120 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(200f, -300f, 0.15f), CPU00.enuCPU00Type.intInvaderX00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 3360 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-200f, -300f, 0.15f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 3600 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-200f, -300f, 0.15f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 3840)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 200f, 0.3f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 200f, -0.3f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 3960 + i * 50)
			{
				Game1.item.pItemEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Item.enuItemType.intLifeRecover00);
			}
		}
		if (intCPUTime == 4320)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(0f, 0f, 0.04609375f), CPUBOSS00.enuCPUBOSS00Type.intOSPREY00, 750f, 10f);
			intCPUTime++;
			flgSubBossSyutugen = true;
		}
		if (intCPUTime == 4620)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, -150f, -0.15f), 90f, CPU00.enuCPU00Type.intTower00R90_y);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, 0f, 0f), 90f, CPU00.enuCPU00Type.intTower00R90_y);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, 150f, 0.15f), 90f, CPU00.enuCPU00Type.intTower00R90_y);
			intCPUCount += 3;
		}
		if (intCPUTime == 4800)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, -150f, -0.15f), 270f, CPU00.enuCPU00Type.intTower00R270_y);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, 0f, 0f), 270f, CPU00.enuCPU00Type.intTower00R270_y);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, 150f, 0.15f), 270f, CPU00.enuCPU00Type.intTower00R270_y);
			intCPUCount += 3;
		}
		if (intCPUTime == 4920)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 200f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 200f, -0.3f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 5160 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFront00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 5580)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 5640 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 6000)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 6120 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFront00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 6600)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(500f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			intCPUCount++;
		}
		if (intCPUTime == 6690)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(250f, 300f, 0.3f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 200f, -0.1f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 200f, -0.1f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			intCPUCount++;
		}
		if (intCPUTime == 6780)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(300f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			intCPUCount++;
		}
		if (intCPUTime == 6960)
		{
			for (int i = 0; i < 3; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-200 + i * 300, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 7140)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 200f, 0.3f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 200f, -0.3f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		if (intCPUTime == 7320)
		{
			for (int i = 0; i < 3; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-200 + i * 300, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 7500)
		{
			for (int i = 0; i < 3; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-200 + i * 300, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 7680)
		{
			for (int i = 0; i < 3; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(200 + i * -300, -300f, 0.2f), CPU00.enuCPU00Type.intInvaderX00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 7860)
		{
			for (int i = 0; i < 3; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(200 + i * -300, -300f, 0.2f), CPU00.enuCPU00Type.intInvaderX00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 8100 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_y);
				intCPUCount++;
			}
		}
		if (intCPUTime == 8520)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 8580 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_y);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 9360 + i * 50)
			{
				Game1.item.pItemEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Item.enuItemType.intLifeRecover00);
			}
		}
		if (intCPUTime == 9720)
		{
			Game1.bGM.pflgBGMON(1);
		}
		if (intCPUTime == 9900)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(300f, 50f, 0.1f), CPUBOSS00.enuCPUBOSS00Type.intDragon, 1000f, 10f);
			flgBossSyutugen = true;
		}
	}

	private void Stage02PortUpDate()
	{
		intTime++;
		if (!flgSubBossSyutugen || flgSubBossTaosita)
		{
			intCPUTime++;
		}
		if (!flgSubBossSyutugen || (flgSubBossTaosita && !flgBossSyutugen))
		{
			Game1.score.pScoreUp(1L);
		}
		if (intCPUTime == 5)
		{
			Game1.bGM.pflgBGMON(0);
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 240 + i * 30)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(1200 - i * 60, -150 + i * 150, 0.7f), CPU00.enuCPU00Type.intTonbo00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 480)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, 300f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, 300f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 600 + i * 30)
			{
				Game1.cPU00.pCPU00SPEnable(new Vector3(-1200 + i * 60, -150 + i * 150, 0.7f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intTonboX00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 780)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(500f, 300f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 900 + i * 30)
			{
				Game1.cPU00.pCPU00SPEnable(new Vector3(-1200 + i * 60, -150 + i * 150, 0.7f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intTonboX00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 1050)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 300f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 1140)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(300f, 150f, 0f), CPU00.enuCPU00Type.intBeetle00);
			intCPUCount++;
		}
		if (intCPUTime == 1200)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, -150f, 0f), CPU00.enuCPU00Type.intBeetleX00);
			intCPUCount++;
		}
		if (intCPUTime == 1260)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, 150f, 0f), CPU00.enuCPU00Type.intBeetle00);
			intCPUCount++;
		}
		if (intCPUTime == 1380)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(300f, 0f, 0f), CPU00.enuCPU00Type.intBeetle00);
			intCPUCount++;
		}
		if (intCPUTime == 1440)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, 150f, 0f), CPU00.enuCPU00Type.intBeetleX00);
			intCPUCount++;
		}
		if (intCPUTime == 1500)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, -150f, 0f), CPU00.enuCPU00Type.intBeetleX00);
			intCPUCount++;
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 1740 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBack00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 2100 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_y);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 2125 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50 + i * 10, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_y);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 2520 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(900 + i * 5, 400f, 1f), CPU00.enuCPU00Type.intTowerBackX00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 2910)
		{
			for (int i = 0; i < 2; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * 150 - 100, 300f, 0f), CPU00.enuCPU00Type.intBee00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 3150)
		{
			for (int i = 0; i < 2; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * 150 + 100, 300f, 0f), CPU00.enuCPU00Type.intBeeX00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 3300)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(300f, 150f, 0f), CPU00.enuCPU00Type.intBeetle00);
			intCPUCount++;
		}
		if (intCPUTime == 3360)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, -150f, 0f), CPU00.enuCPU00Type.intBeetleX00);
			intCPUCount++;
		}
		if (intTime == 3570)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 - 300, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBee00);
					intCPUCount++;
				}
			}
		}
		if (intCPUTime == 3810)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 + 100, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBeeX00);
					intCPUCount++;
				}
			}
		}
		if (intCPUTime == 3840)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(200f, 150f, 0f), CPU00.enuCPU00Type.intBeetle00);
			intCPUCount++;
		}
		if (intCPUTime == 3900)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(100f, 0f, 0f), CPU00.enuCPU00Type.intBeetle00);
			intCPUCount++;
		}
		if (intCPUTime == 3960)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, 150f, 0f), CPU00.enuCPU00Type.intBeetle00);
			intCPUCount++;
		}
		if (intCPUTime == 4020)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, 0f, 0f), CPU00.enuCPU00Type.intBeetleX00);
			intCPUCount++;
		}
		if (intCPUTime == 4080)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, 150f, 0f), CPU00.enuCPU00Type.intBeetleX00);
			intCPUCount++;
		}
		if (intCPUTime == 4140)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, -150f, 0f), CPU00.enuCPU00Type.intBeetleX00);
			intCPUCount++;
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 4320 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50 - i * 10, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_y);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 4345 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50 + i * 10, 0f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_y);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 4680 + i * 50)
			{
				Game1.item.pItemEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Item.enuItemType.intLifeRecover00);
			}
		}
		if (intCPUTime == 5040)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(0f, -2250f, 0.2f), CPUBOSS00.enuCPUBOSS00Type.intHatiNosu, 750f, 10f);
			intCPUTime++;
			flgSubBossSyutugen = true;
		}
		if (intCPUTime == 5190)
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 - 300, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBee00);
					intCPUCount++;
				}
			}
		}
		if (intCPUTime == 5370)
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 + 100, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBeeX00);
					intCPUCount++;
				}
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 5580 + i * 30)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(1200 - i * 60, -150 + i * 150, 0.7f), CPU00.enuCPU00Type.intTonbo00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 5760 + i * 30)
			{
				Game1.cPU00.pCPU00SPEnable(new Vector3(-1200 + i * 60, -150 + i * 150, 0.7f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intTonboX00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 6000)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00);
			Game1.cPU00.pCPU00Enable(new Vector3(350f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00);
			Game1.cPU00.pCPU00Enable(new Vector3(200f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00);
			intCPUCount += 3;
		}
		if (intCPUTime == 6210)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 + 100, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBeeX00);
					intCPUCount++;
				}
			}
		}
		if (intCPUTime == 6390)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 - 300, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBee00);
					intCPUCount++;
				}
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 6600 + i * 30)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(1200 - i * 60, -150 + i * 150, 0.7f), CPU00.enuCPU00Type.intTonbo00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 6780 + i * 30)
			{
				Game1.cPU00.pCPU00SPEnable(new Vector3(-1200 + i * 60, -150 + i * 150, 0.7f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intTonboX00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 6930)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 300f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 7050)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 300f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(100f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 300f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 7170)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 300f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 7350)
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 - 300, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBee00);
					intCPUCount++;
				}
			}
		}
		if (intCPUTime == 7590)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 300f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 7830)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 300f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 8130)
		{
			for (int i = 0; i < 8; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 + 100, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBeeX00);
					intCPUCount++;
				}
			}
		}
		if (intCPUTime == 8310)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 300f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 8550)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 300f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(100f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 300f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 8670)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 300f, -0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intKusa);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0.15f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-400f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 8880 + i * 50)
			{
				Game1.item.pItemEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Item.enuItemType.intLifeRecover00);
			}
		}
		if (intCPUTime == 155)
		{
			Game1.bGM.pflgBGMON(1);
		}
		if (intCPUTime == 9480)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(0f, -2250f, 0.2f), CPUBOSS00.enuCPUBOSS00Type.intQueenBee, 1000f, 10f);
			flgBossSyutugen = true;
		}
	}

	private void Stage03PortUpDate()
	{
		intTime++;
		if (!flgSubBossSyutugen || flgSubBossTaosita)
		{
			intCPUTime++;
		}
		if (!flgSubBossSyutugen || (flgSubBossTaosita && !flgBossSyutugen))
		{
			Game1.score.pScoreUp(1L);
		}
		if (intCPUTime == 5)
		{
			Game1.bGM.pflgBGMON(0);
		}
		if (flgBossSyutugen)
		{
			intCPUBOSSTime++;
			if (intCPUBOSSTime > 1920)
			{
				intCPUBOSSTime = 0;
			}
		}
		if (intCPUTime == 180)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		if (intCPUTime == 270)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-700f, -150f, 0f), 90f, CPU00.enuCPU00Type.intTower00R90);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-700f, 0f, 0.15f), 90f, CPU00.enuCPU00Type.intTower00R90);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-700f, 150f, 0.3f), 90f, CPU00.enuCPU00Type.intTower00R90);
			intCPUCount += 3;
		}
		if (intCPUTime == 360)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		if (intCPUTime == 510)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(700f, -150f, 0f), 270f, CPU00.enuCPU00Type.intTower00R270);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(700f, 0f, 0.15f), 270f, CPU00.enuCPU00Type.intTower00R270);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(700f, 150f, 0.3f), 270f, CPU00.enuCPU00Type.intTower00R270);
			intCPUCount += 3;
		}
		if (intCPUTime == 600)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 750 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_y);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 775 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50 + i * 10, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_y);
				intCPUCount++;
			}
		}
		if (intCPUTime == 1080)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 1140 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_r);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 1170 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_r);
				intCPUCount++;
			}
		}
		if (intCPUTime == 1440)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 1530 + i * 40)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * -200, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 1710)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 1860 + i * 40)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * 200, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 2010)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 200f, -0.2f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.cPU00.pCPU00SPEnable(new Vector3(500f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
			intCPUCount++;
		}
		if (intCPUTime == 2040)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(350f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
			intCPUCount++;
		}
		if (intCPUTime == 2100)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 200f, -0.2f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.cPU00.pCPU00SPEnable(new Vector3(200f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
			intCPUCount++;
		}
		if (intCPUTime == 2190)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(500f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudaiX00);
			intCPUCount++;
		}
		if (intCPUTime == 2250)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 200f, -0.2f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.cPU00.pCPU00SPEnable(new Vector3(350f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudaiX00);
			intCPUCount++;
		}
		if (intCPUTime == 2310)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(200f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudaiX00);
			intCPUCount++;
		}
		if (intCPUTime == 2370)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, -256f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 2400 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_r);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 2485 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_r);
				intCPUCount++;
			}
		}
		if (intCPUTime == 2790)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 2850 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_y);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 2875 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_y);
				intCPUCount++;
			}
		}
		if (intCPUTime == 3300)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		if (intCPUTime == 3390)
		{
			for (int i = 0; i < 4; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-200 + i * 300, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 3480)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, 0f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		if (intCPUTime == 3570)
		{
			for (int i = 0; i < 4; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-300 + i * 280, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 3660)
		{
			for (int i = 0; i < 4; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(300 + i * -280, 0f, 0.2f), CPU00.enuCPU00Type.intInvaderX00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 3750)
		{
			for (int i = 0; i < 4; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-300 + i * 280, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 3900 + i * 40)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * 200, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 4080 + i * 40)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * -200, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 4380 + i * 50)
			{
				Game1.item.pItemEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Item.enuItemType.intLifeRecover00);
			}
		}
		if (intCPUTime == 4800)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(0f, 50f, 0.04609375f), CPUBOSS00.enuCPUBOSS00Type.intOSPREY00, 1300f, 10f);
			intCPUTime++;
			flgSubBossSyutugen = true;
		}
		if (intCPUTime == 4920)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(350f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(500f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(50f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-100f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		if (intCPUTime == 5010)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(350f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(200f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			intCPUCount += 3;
		}
		if (intCPUTime == 5100)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-500f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(-350f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(-200f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			intCPUCount += 3;
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 5220 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_r);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 5245 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_r);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 5520 + i * 30)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * 200, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 5760 + i * 30)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * -200, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 5940)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 200f, -0.2f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.cPU00.pCPU00SPEnable(new Vector3(500f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
			intCPUCount++;
		}
		if (intCPUTime == 5970)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(350f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
			intCPUCount++;
		}
		if (intCPUTime == 6000)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 200f, -0.2f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.cPU00.pCPU00SPEnable(new Vector3(200f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudai00);
			intCPUCount++;
		}
		if (intCPUTime == 6030)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(500f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudaiX00);
			intCPUCount++;
		}
		if (intCPUTime == 6060)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 200f, -0.2f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.cPU00.pCPU00SPEnable(new Vector3(350f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudaiX00);
			intCPUCount++;
		}
		if (intCPUTime == 6090)
		{
			Game1.cPU00.pCPU00SPEnable(new Vector3(200f, 300f, 0.2f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intHoudaiX00);
			intCPUCount++;
		}
		if (intCPUTime == 6120)
		{
			for (int i = 0; i < 4; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(300 + i * -280, 0f, 0.2f), CPU00.enuCPU00Type.intInvaderX00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 6210)
		{
			for (int i = 0; i < 4; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-300 + i * 280, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 6300)
		{
			for (int i = 0; i < 4; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(300 + i * -280, 0f, 0.2f), CPU00.enuCPU00Type.intInvaderX00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 6420)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, -150f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, 0f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, 150f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90);
			intCPUCount += 3;
		}
		if (intCPUTime == 6540)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, -150f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, 0f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, 150f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270);
			intCPUCount += 3;
		}
		if (intCPUTime == 6630)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, -150f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270_r);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, 0f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270_r);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(600f, 150f, 0.2f), 270f, CPU00.enuCPU00Type.intTower00R270_r);
			intCPUCount += 3;
		}
		if (intCPUTime == 6660)
		{
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, -150f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90_r);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, 0f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90_r);
			Game1.cPU00.pCPU00ItiREnable(new Vector3(-600f, 150f, 0.2f), 90f, CPU00.enuCPU00Type.intTower00R90_r);
			intCPUCount += 3;
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 6720 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * -180, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 6900 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * 180, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 7080)
		{
			for (int i = 0; i < 5; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(350 + i * -260, 0f, 0.2f), CPU00.enuCPU00Type.intInvaderX00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 7200)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(350f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(500f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(50f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-100f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		if (intCPUTime == 7260)
		{
			for (int i = 0; i < 5; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-350 + i * 260, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 7350 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_y);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 7375 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_y);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 7860 + i * 50)
			{
				Game1.item.pItemEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Item.enuItemType.intLifeRecover00);
			}
		}
		if (intCPUTime == 137)
		{
			Game1.bGM.pflgBGMON(1);
		}
		if (intCPUTime == 8340)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(2000f, -350f, 0.3f), CPUBOSS00.enuCPUBOSS00Type.intArship, 1000f, 10f);
			flgBossSyutugen = true;
		}
		if (intCPUBOSSTime == 60)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(350f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(200f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUBOSSTime == 540 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFront00);
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUBOSSTime == 565 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00);
			}
		}
		if (intCPUBOSSTime == 1200)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(500f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-100f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		if (intCPUBOSSTime == 1380)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(350f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(50f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
	}

	private void Stage04PortUpDate()
	{
		intTime++;
		if (!flgSubBossSyutugen || flgSubBossTaosita)
		{
			intCPUTime++;
		}
		if (!flgSubBossSyutugen || (flgSubBossTaosita && !flgBossSyutugen))
		{
			Game1.score.pScoreUp(1L);
		}
		if (intCPUTime == 5)
		{
			Game1.bGM.pflgBGMON(2);
		}
		if (flgBossSyutugen)
		{
			intCPUBOSSTime++;
			if (intCPUBOSSTime > 1320)
			{
				intCPUBOSSTime = 0;
			}
		}
		for (int i = 0; i < 12; i += 2)
		{
			if (intCPUTime == 120 + i * 15)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(500 - i * 75, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
				intCPUCount++;
			}
			if (intCPUTime == 120 + (i + 1) * 15)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(500 - (i + 1) * 75, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_r);
				intCPUCount++;
			}
		}
		if (intCPUTime == 240)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(600f, 300f, 1.5f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 300)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 1.5f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 360)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(350f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_r);
			Game1.cPU00.pCPU00Enable(new Vector3(200f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(50f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_r);
			Game1.cPU00.pCPU00Enable(new Vector3(-100f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			intCPUCount += 5;
		}
		if (intCPUTime == 420)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(350f, 100f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(500f, 300f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(650f, 100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(50f, 100f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-100f, 300f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-250f, 100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 480 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_r);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 510 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_y);
				intCPUCount++;
			}
		}
		if (intCPUTime == 570)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 - 300, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBee00);
					intCPUCount++;
				}
			}
		}
		if (intCPUTime == 690)
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 + 100, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBeeX00);
					intCPUCount++;
				}
			}
		}
		if (intCPUTime == 960)
		{
			for (int i = 0; i < 3; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-200 + i * 300, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 1050)
		{
			for (int i = 0; i < 4; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-300 + i * 300, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 1140)
		{
			for (int i = 0; i < 5; i++)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-400 + i * 300, -300f, 0.2f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 5; i++)
		{
			if (intCPUTime == 1290 + i * 20)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(1200f, -150 + i * 100, 0.7f), CPU00.enuCPU00Type.intTonbo00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 5; i++)
		{
			if (intCPUTime == 1440 + i * 20)
			{
				Game1.cPU00.pCPU00SPEnable(new Vector3(-1200f, -150 + i * 100, 0.7f), SpriteEffects.FlipHorizontally, CPU00.enuCPU00Type.intTonboX00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 5; i++)
		{
			if (intCPUTime == 1560 + i * 20 + 30)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * 200 - 400, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 1680)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(350f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(500f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(650f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(50f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-100f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-250f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		if (intCPUTime == 1740)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(450f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-450f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		if (intCPUTime == 1800)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(350f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(500f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(650f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(50f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-100f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-250f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		for (int i = 0; i < 16; i += 2)
		{
			if (intCPUTime == 1890 + i * 10)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-500 + i * 60, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
				intCPUCount++;
			}
			if (intCPUTime == 1890 + (i + 1) * 10)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-500 + (i + 1) * 60, 200f, 0.15f), CPU00.enuCPU00Type.intTower00_r);
				intCPUCount++;
			}
		}
		if (intCPUTime == 2070)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-500f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(-350f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_r);
			Game1.cPU00.pCPU00Enable(new Vector3(-200f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(-50f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_r);
			Game1.cPU00.pCPU00Enable(new Vector3(100f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
			intCPUCount += 5;
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 2400 + i * 50)
			{
				Game1.item.pItemEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Item.enuItemType.intLifeRecover00);
			}
		}
		if (intCPUTime == 2760)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(300f, 50f, 0.1f), CPUBOSS00.enuCPUBOSS00Type.intDragon, 1150f, 10f);
			intCPUTime++;
			flgSubBossSyutugen = true;
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 2940 + i * 35)
			{
				Game1.item.pItemEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Item.enuItemType.intLifeRecover00);
			}
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 3060 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBack00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 3180 + i * 20 + 30)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * 200 - 400, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 3300 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBackX00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 3420 + i * 20 + 30)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * -200 + 400, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 3540 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBack00_y);
				intCPUCount++;
			}
		}
		if (intCPUTime == 3600)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(400f, 300f, 1.5f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		for (int i = 0; i < 3; i++)
		{
			if (intCPUTime == 3660 + i * 20 + 30)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(i * 200 - 400, -200f, 0.2f), CPU00.enuCPU00Type.intPlane00);
				intCPUCount++;
			}
		}
		if (intCPUTime == 3780)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 300f, 1.5f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 3780 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(900f, 400f, 1f), CPU00.enuCPU00Type.intTowerBackX00_y);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 3900 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFront00_y);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 3930 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(50f, -100f, 0.1f), CPU00.enuCPU00Type.intTowerFrontX00_r);
				intCPUCount++;
			}
		}
		if (intCPUTime == 4110)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(350f, 100f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(500f, 300f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(650f, 100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(50f, 100f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-100f, 300f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-250f, 100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(200f, -100f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(500f, -100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-100f, -100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 4170)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, 100f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, 300f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(450f, 100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, 100f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, 300f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-450f, 100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(0f, -100f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, -100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-300f, -100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 4230)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(300f, 300f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(450f, 100f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(600f, 300f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(750f, 100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(150f, 100f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(0f, 300f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(-150f, 100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(300f, -100f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(600f, -100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
			Game1.syougai.pSyougaiEnable(new Vector3(0f, -100f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intIwa);
		}
		if (intCPUTime == 4290)
		{
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 150 - 300, j * 150 - 300, 0f), CPU00.enuCPU00Type.intBee00);
					intCPUCount++;
				}
			}
		}
		if (intCPUTime == 4440)
		{
			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 140 + 100, j * 140 - 300, 0f), CPU00.enuCPU00Type.intBeeX00);
					intCPUCount++;
				}
			}
		}
		if (intCPUTime == 4590)
		{
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					Game1.cPU00.pCPU00Enable(new Vector3(i * 130 - 300, j * 130 - 300, 0f), CPU00.enuCPU00Type.intBee00);
					intCPUCount++;
				}
			}
		}
		if (intCPUTime == 4800)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 300f, 0f), CPU00.enuCPU00Type.intBeetle00);
			Game1.cPU00.pCPU00Enable(new Vector3(200f, 150f, 0f), CPU00.enuCPU00Type.intBeetle00);
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 0f, 0f), CPU00.enuCPU00Type.intBeetle00);
			Game1.cPU00.pCPU00Enable(new Vector3(200f, -150f, 0f), CPU00.enuCPU00Type.intBeetle00);
			intCPUCount += 4;
		}
		if (intCPUTime == 4920)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(-500f, -300f, 0f), CPU00.enuCPU00Type.intBeetleX00);
			Game1.cPU00.pCPU00Enable(new Vector3(-200f, -150f, 0f), CPU00.enuCPU00Type.intBeetleX00);
			Game1.cPU00.pCPU00Enable(new Vector3(-500f, 0f, 0f), CPU00.enuCPU00Type.intBeetleX00);
			Game1.cPU00.pCPU00Enable(new Vector3(-200f, 150f, 0f), CPU00.enuCPU00Type.intBeetleX00);
			intCPUCount += 4;
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 5040 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-200f, -300f, 0.15f), CPU00.enuCPU00Type.intInvader00);
				Game1.cPU00.pCPU00Enable(new Vector3(50f, -300f, 0.15f), CPU00.enuCPU00Type.intInvader00);
				intCPUCount += 2;
			}
		}
		for (int i = 0; i < 6; i++)
		{
			if (intCPUTime == 5220 + i * 25)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(200f, -300f, 0.15f), CPU00.enuCPU00Type.intInvaderX00);
				Game1.cPU00.pCPU00Enable(new Vector3(-50f, -300f, 0.15f), CPU00.enuCPU00Type.intInvaderX00);
				intCPUCount += 2;
			}
		}
		for (int i = 0; i < 12; i += 2)
		{
			if (intCPUTime == 53430 + i * 15)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(500 - i * 75, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
				intCPUCount++;
			}
			if (intCPUTime == 5430 + (i + 1) * 15)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(500 - (i + 1) * 75, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_r);
				intCPUCount++;
			}
		}
		for (int i = 0; i < 16; i += 2)
		{
			if (intCPUTime == 5670 + i * 10)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-500 + i * 60, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_y);
				intCPUCount++;
			}
			if (intCPUTime == 5670 + (i + 1) * 10)
			{
				Game1.cPU00.pCPU00Enable(new Vector3(-500 + (i + 1) * 60, 200f, 0.15f), CPU00.enuCPU00Type.intTower00_r);
				intCPUCount++;
			}
		}
		if (intCPUTime == 5790)
		{
			Game1.cPU00.pCPU00Enable(new Vector3(500f, 200f, -0.1f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(350f, 200f, 0f), CPU00.enuCPU00Type.intTower00_r);
			Game1.cPU00.pCPU00Enable(new Vector3(200f, 200f, 0.1f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(50f, 200f, 0.2f), CPU00.enuCPU00Type.intTower00_r);
			Game1.cPU00.pCPU00Enable(new Vector3(-100f, 200f, 0.1f), CPU00.enuCPU00Type.intTower00_y);
			Game1.cPU00.pCPU00Enable(new Vector3(-150f, 200f, 0f), CPU00.enuCPU00Type.intTower00_r);
			Game1.cPU00.pCPU00Enable(new Vector3(-300f, 200f, -0.1f), CPU00.enuCPU00Type.intTower00_y);
			intCPUCount += 7;
		}
		for (int i = 0; i < 4; i++)
		{
			if (intCPUTime == 6000 + i * 50)
			{
				Game1.item.pItemEnable(new Vector3(0f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Item.enuItemType.intLifeRecover00);
			}
		}
		if (intCPUTime == 6480)
		{
			Game1.bG.penuBGSelect = BG.enuBGScene.Main02;
		}
		if (intCPUTime == 6600)
		{
			Game1.bGM.pflgBGMON(3);
		}
		if (intCPUTime == 6720)
		{
			Game1.bGM.pflgSEBakuhatu[3] = true;
			Game1.bakuhatu.psrcBakuhatuCoreEnable(new Vector3(0f, -200f, 0.6f), 0f, Bakuhatu.enuBakuhatuType.intKaminari00, 0);
			Game1.bakuhatu.psrcBakuhatuCoreEnable(new Vector3(500f, -200f, 0.9f), 0f, Bakuhatu.enuBakuhatuType.intKaminari01, 60);
			Game1.bakuhatu.psrcBakuhatuCoreEnable(new Vector3(400f, -200f, 0.9f), 0f, Bakuhatu.enuBakuhatuType.intKaminari01, 70);
			for (int i = 0; i < 10; i++)
			{
				Game1.bakuhatu.psrcBakuhatuCoreEnable(new Vector3(-1000 + rnd.Next(1, 10) * 50, -400 + rnd.Next(1, 10) * 20, (float)rnd.Next(1, 20) / 20f), 0f, Bakuhatu.enuBakuhatuType.intKaminari00, rnd.Next(1, 90));
			}
			for (int i = 0; i < 10; i++)
			{
				Game1.bakuhatu.psrcBakuhatuCoreEnable(new Vector3(rnd.Next(1, 20) * 50, -400 + rnd.Next(1, 10) * 20, (float)rnd.Next(1, 20) / 20f), 0f, Bakuhatu.enuBakuhatuType.intKaminari01, rnd.Next(1, 90));
			}
		}
		if (intCPUTime == 6860)
		{
			Game1.cPUBOSS00.pCPUBOSS00Enable(new Vector3(0f, -1250f, 0f), CPUBOSS00.enuCPUBOSS00Type.intESP, 1250f, 10f);
			flgBossSyutugen = true;
		}
		if (intCPUBOSSTime == 60)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(200f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(350f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(500f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(50f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-100f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
		for (int i = 0; i < 8; i++)
		{
			if (intCPUBOSSTime == 240 + i * 40)
			{
				Game1.syougai.pSyougaiEnable(new Vector3(500 + i * -150, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			}
		}
		for (int i = 0; i < 8; i++)
		{
			if (intCPUBOSSTime == 420 + i * 40)
			{
				Game1.syougai.pSyougaiEnable(new Vector3(-500 + i * 150, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			}
		}
		if (intCPUBOSSTime == 840)
		{
			Game1.syougai.pSyougaiEnable(new Vector3(-200f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-50f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(100f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-350f, 200f, 0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
			Game1.syougai.pSyougaiEnable(new Vector3(-500f, 200f, -0f), new Vector3(0f, 0f, 1f / 128f), Syougai.enuSyougaiType.intHasira);
		}
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}
}
