using Microsoft.Xna.Framework;

namespace DepthAttack.CS;

public class CPU_AI00 : GameComponent
{
	public CPU_AI00(Game game)
		: base(game)
	{
	}

	public override void Initialize()
	{
		base.Initialize();
	}

	public void pkakuCPU00(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		switch (asrcCPU00Core.penuType)
		{
		case CPU00.enuCPU00Type.intNormal00:
			Normal00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTower00:
			Tower00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTower00R90:
			Tower00R90AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTower00R270:
			Tower00R270AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTowerBack00:
			TowerBack00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTowerBackX00:
			TowerBackX00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intHoudai00:
			Houdai00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intHoudaiX00:
			Houdai00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intInvader00:
			Invader00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intInvaderX00:
			InvaderX00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intPlane00:
			Plane00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intBee00:
			Bee00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intBeeX00:
			BeeX00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTowerFront00:
			TowerFront00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTowerFrontX00:
			TowerFrontX00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTower00_y:
			Tower00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTower00_r:
			Tower00_rAI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTower00R90_y:
			Tower00R90AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTower00R90_r:
			Tower00R90_rAI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTowerBack00_y:
			TowerBack00_yAI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTowerBack00_r:
			TowerBack00_rAI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTowerBackX00_y:
			TowerBackX00_yAI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTowerBackX00_r:
			TowerBackX00_rAI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTowerFront00_y:
			TowerFront00_yAI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTowerFront00_r:
			TowerFront00_rAI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTowerFrontX00_y:
			TowerFrontX00_yAI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTowerFrontX00_r:
			TowerFrontX00_rAI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTower00R270_y:
			Tower00R270AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTower00R270_r:
			Tower00R270_rAI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intBeetle00:
			Bee00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intBeetleX00:
			BeeX00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTonbo00:
			Tonbo00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intTonboX00:
			TonboX00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intQueenChildBee00:
			Bee00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intQueenChildBeeX00:
			BeeX00AI(ref asrcCPU00Core);
			break;
		case CPU00.enuCPU00Type.intNormal01:
			break;
		}
	}

	private void Normal00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[1].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[2].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[2].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
		}
	}

	private void Tower00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 15;
			asrcCPU00Core.psrcAI[1].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			asrcCPU00Core.psrcAI[1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[2].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 120;
			asrcCPU00Core.psrcAI[3].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			asrcCPU00Core.psrcAI[3].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[4].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[4].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void Tower00_rAI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 15;
			asrcCPU00Core.psrcAI[1].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			asrcCPU00Core.psrcAI[1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[2].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			for (int i = 0; i < 16; i += 2)
			{
				asrcCPU00Core.psrcAI[3 + i].pintLoopCount = 7;
				asrcCPU00Core.psrcAI[3 + i].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
				asrcCPU00Core.psrcAI[3 + i].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
				asrcCPU00Core.psrcAI[3 + i].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[3 + i + 1].pintLoopCount = 7;
				asrcCPU00Core.psrcAI[3 + i + 1].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
				asrcCPU00Core.psrcAI[3 + i + 1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[3 + i + 1].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			}
			asrcCPU00Core.psrcAI[20].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[20].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void Tower00R90AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 10;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 45;
			asrcCPU00Core.psrcAI[1].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[2].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 120;
			asrcCPU00Core.psrcAI[3].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[3].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[4].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[4].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void Tower00R90_rAI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 10;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			for (int i = 0; i < 6; i += 2)
			{
				asrcCPU00Core.psrcAI[1 + i].pintLoopCount = 7;
				asrcCPU00Core.psrcAI[1 + i].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
				asrcCPU00Core.psrcAI[1 + i].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
				asrcCPU00Core.psrcAI[1 + i].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[1 + i + 1].pintLoopCount = 7;
				asrcCPU00Core.psrcAI[1 + i + 1].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
				asrcCPU00Core.psrcAI[1 + i + 1].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
				asrcCPU00Core.psrcAI[1 + i + 1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			}
			asrcCPU00Core.psrcAI[7].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[7].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			for (int i = 0; i < 12; i += 2)
			{
				asrcCPU00Core.psrcAI[8 + i].pintLoopCount = 7;
				asrcCPU00Core.psrcAI[8 + i].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
				asrcCPU00Core.psrcAI[8 + i].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
				asrcCPU00Core.psrcAI[8 + i].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[8 + i + 1].pintLoopCount = 7;
				asrcCPU00Core.psrcAI[8 + i + 1].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
				asrcCPU00Core.psrcAI[8 + i + 1].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
				asrcCPU00Core.psrcAI[8 + i + 1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			}
			asrcCPU00Core.psrcAI[21].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[21].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void Tower00R270AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 10;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 45;
			asrcCPU00Core.psrcAI[1].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[2].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 120;
			asrcCPU00Core.psrcAI[3].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[3].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[4].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[4].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void Tower00R270_rAI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 10;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 7;
			asrcCPU00Core.psrcAI[1].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[1].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 7;
			asrcCPU00Core.psrcAI[2].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[2].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[2].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 7;
			asrcCPU00Core.psrcAI[3].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[3].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[3].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			asrcCPU00Core.psrcAI[4].pintLoopCount = 7;
			asrcCPU00Core.psrcAI[4].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[4].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[4].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[5].pintLoopCount = 7;
			asrcCPU00Core.psrcAI[5].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[5].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[5].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			asrcCPU00Core.psrcAI[6].pintLoopCount = 7;
			asrcCPU00Core.psrcAI[6].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[6].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[6].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[7].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[7].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			for (int i = 0; i < 12; i += 2)
			{
				asrcCPU00Core.psrcAI[8 + i].pintLoopCount = 7;
				asrcCPU00Core.psrcAI[8 + i].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
				asrcCPU00Core.psrcAI[8 + i].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[8 + i].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
				asrcCPU00Core.psrcAI[8 + i + 1].pintLoopCount = 7;
				asrcCPU00Core.psrcAI[8 + i + 1].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
				asrcCPU00Core.psrcAI[8 + i + 1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[8 + i + 1].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			}
			asrcCPU00Core.psrcAI[14].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[14].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void Houdai00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 40;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[1].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 40;
			asrcCPU00Core.psrcAI[2].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[4].pintLoopCount = 90;
			asrcCPU00Core.psrcAI[4].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[5].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[5].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TowerBack00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 120;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[0].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[1].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 120;
			asrcCPU00Core.psrcAI[2].penuTypeMovZDash = CPU00.OperationTypeMovZDash.ZmDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovXDash = CPU00.OperationTypeMovXDash.XpDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovYDash = CPU00.OperationTypeMovYDash.YmDash;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TowerBack00_yAI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 90;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[0].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[1].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 140;
			asrcCPU00Core.psrcAI[2].penuTypeMovZDash = CPU00.OperationTypeMovZDash.ZmDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovXDash = CPU00.OperationTypeMovXDash.XpDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovYDash = CPU00.OperationTypeMovYDash.YmDash;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TowerBack00_rAI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			for (int i = 0; i < 12; i += 2)
			{
				asrcCPU00Core.psrcAI[i].pintLoopCount = 12;
				asrcCPU00Core.psrcAI[i].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
				asrcCPU00Core.psrcAI[i].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
				asrcCPU00Core.psrcAI[i].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
				asrcCPU00Core.psrcAI[i + 1].pintLoopCount = 6;
				asrcCPU00Core.psrcAI[i + 1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
				asrcCPU00Core.psrcAI[i + 1].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
				asrcCPU00Core.psrcAI[i + 1].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			}
			asrcCPU00Core.psrcAI[13].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[13].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			for (int i = 0; i < 14; i += 2)
			{
				asrcCPU00Core.psrcAI[14 + i].pintLoopCount = 14;
				asrcCPU00Core.psrcAI[14 + i].penuTypeMovZDash = CPU00.OperationTypeMovZDash.ZmDash;
				asrcCPU00Core.psrcAI[14 + i].penuTypeMovXDash = CPU00.OperationTypeMovXDash.XpDash;
				asrcCPU00Core.psrcAI[14 + i].penuTypeMovYDash = CPU00.OperationTypeMovYDash.YmDash;
				asrcCPU00Core.psrcAI[14 + i + 1].pintLoopCount = 7;
				asrcCPU00Core.psrcAI[14 + i + 1].penuTypeMovZDash = CPU00.OperationTypeMovZDash.ZmDash;
				asrcCPU00Core.psrcAI[14 + i + 1].penuTypeMovXDash = CPU00.OperationTypeMovXDash.XmDash;
				asrcCPU00Core.psrcAI[14 + i + 1].penuTypeMovYDash = CPU00.OperationTypeMovYDash.YmDash;
			}
			asrcCPU00Core.psrcAI[29].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[29].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TowerBackX00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 120;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[0].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[1].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 120;
			asrcCPU00Core.psrcAI[2].penuTypeMovZDash = CPU00.OperationTypeMovZDash.ZmDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovXDash = CPU00.OperationTypeMovXDash.XmDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovYDash = CPU00.OperationTypeMovYDash.YmDash;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TowerBackX00_yAI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 90;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[0].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[1].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 140;
			asrcCPU00Core.psrcAI[2].penuTypeMovZDash = CPU00.OperationTypeMovZDash.ZmDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovXDash = CPU00.OperationTypeMovXDash.XmDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovYDash = CPU00.OperationTypeMovYDash.YmDash;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TowerBackX00_rAI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			for (int i = 0; i < 12; i += 2)
			{
				asrcCPU00Core.psrcAI[i].pintLoopCount = 12;
				asrcCPU00Core.psrcAI[i].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
				asrcCPU00Core.psrcAI[i].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
				asrcCPU00Core.psrcAI[i].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
				asrcCPU00Core.psrcAI[i + 1].pintLoopCount = 6;
				asrcCPU00Core.psrcAI[i + 1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
				asrcCPU00Core.psrcAI[i + 1].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
				asrcCPU00Core.psrcAI[i + 1].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			}
			asrcCPU00Core.psrcAI[13].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[13].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			for (int i = 0; i < 14; i += 2)
			{
				asrcCPU00Core.psrcAI[14 + i].pintLoopCount = 14;
				asrcCPU00Core.psrcAI[14 + i].penuTypeMovZDash = CPU00.OperationTypeMovZDash.ZmDash;
				asrcCPU00Core.psrcAI[14 + i].penuTypeMovXDash = CPU00.OperationTypeMovXDash.XmDash;
				asrcCPU00Core.psrcAI[14 + i].penuTypeMovYDash = CPU00.OperationTypeMovYDash.YmDash;
				asrcCPU00Core.psrcAI[14 + i + 1].pintLoopCount = 7;
				asrcCPU00Core.psrcAI[14 + i + 1].penuTypeMovZDash = CPU00.OperationTypeMovZDash.ZmDash;
				asrcCPU00Core.psrcAI[14 + i + 1].penuTypeMovXDash = CPU00.OperationTypeMovXDash.XpDash;
				asrcCPU00Core.psrcAI[14 + i + 1].penuTypeMovYDash = CPU00.OperationTypeMovYDash.YmDash;
			}
			asrcCPU00Core.psrcAI[29].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[29].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void Invader00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 15;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 7;
			asrcCPU00Core.psrcAI[1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[1].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 20;
			asrcCPU00Core.psrcAI[2].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[2].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[4].pintLoopCount = 10;
			asrcCPU00Core.psrcAI[4].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[4].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[5].pintLoopCount = 25;
			asrcCPU00Core.psrcAI[5].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[5].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[6].pintLoopCount = 13;
			asrcCPU00Core.psrcAI[6].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[6].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[7].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[7].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[7].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[8].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[8].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void InvaderX00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 15;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 7;
			asrcCPU00Core.psrcAI[1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[1].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 20;
			asrcCPU00Core.psrcAI[2].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[2].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[4].pintLoopCount = 10;
			asrcCPU00Core.psrcAI[4].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[4].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[5].pintLoopCount = 25;
			asrcCPU00Core.psrcAI[5].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[5].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[6].pintLoopCount = 13;
			asrcCPU00Core.psrcAI[6].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[6].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[7].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[7].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[7].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[8].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[8].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void Plane00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[1].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 90;
			asrcCPU00Core.psrcAI[2].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[2].penuTypeMovY = CPU00.OperationTypeMovY.Ym;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void Bee00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 80;
			asrcCPU00Core.psrcAI[1].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[2].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[3].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[4].pintLoopCount = 80;
			asrcCPU00Core.psrcAI[4].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[5].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[5].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[6].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[6].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[7].pintLoopCount = 80;
			asrcCPU00Core.psrcAI[7].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[8].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[8].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[9].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[9].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[10].pintLoopCount = 150;
			asrcCPU00Core.psrcAI[10].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[11].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[11].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void BeeX00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 80;
			asrcCPU00Core.psrcAI[1].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[2].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[3].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[4].pintLoopCount = 80;
			asrcCPU00Core.psrcAI[4].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[5].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[5].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[6].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[6].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[7].pintLoopCount = 80;
			asrcCPU00Core.psrcAI[7].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[8].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[8].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[9].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[9].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[10].pintLoopCount = 150;
			asrcCPU00Core.psrcAI[10].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[11].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[11].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void Tonbo00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[1].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[2].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[2].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[4].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[4].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[4].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[5].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[5].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[5].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
			asrcCPU00Core.psrcAI[6].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[6].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[6].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[7].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[7].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[7].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
			asrcCPU00Core.psrcAI[8].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[8].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TonboX00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 30;
			asrcCPU00Core.psrcAI[1].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[2].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[2].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[4].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[4].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[4].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[5].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[5].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[5].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
			asrcCPU00Core.psrcAI[6].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[6].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[6].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[7].pintLoopCount = 60;
			asrcCPU00Core.psrcAI[7].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[7].penuTypeMovZ = CPU00.OperationTypeMovZ.Zm;
			asrcCPU00Core.psrcAI[8].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[8].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TowerFront00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 70;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[0].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[1].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 140;
			asrcCPU00Core.psrcAI[2].penuTypeMovZDash = CPU00.OperationTypeMovZDash.ZpDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovXDash = CPU00.OperationTypeMovXDash.XmDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovYDash = CPU00.OperationTypeMovYDash.YpDash;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TowerFront00_yAI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 55;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
			asrcCPU00Core.psrcAI[0].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[1].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 140;
			asrcCPU00Core.psrcAI[2].penuTypeMovZDash = CPU00.OperationTypeMovZDash.ZpDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovXDash = CPU00.OperationTypeMovXDash.XmDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovYDash = CPU00.OperationTypeMovYDash.YpDash;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TowerFront00_rAI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			for (int i = 0; i < 12; i += 2)
			{
				asrcCPU00Core.psrcAI[i].pintLoopCount = 12;
				asrcCPU00Core.psrcAI[i].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[i].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
				asrcCPU00Core.psrcAI[i].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
				asrcCPU00Core.psrcAI[i + 1].pintLoopCount = 6;
				asrcCPU00Core.psrcAI[i + 1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[i + 1].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
				asrcCPU00Core.psrcAI[i + 1].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			}
			asrcCPU00Core.psrcAI[13].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[13].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			for (int i = 0; i < 14; i += 2)
			{
				asrcCPU00Core.psrcAI[14 + i].pintLoopCount = 12;
				asrcCPU00Core.psrcAI[14 + i].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[14 + i].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
				asrcCPU00Core.psrcAI[14 + i].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
				asrcCPU00Core.psrcAI[14 + i + 1].pintLoopCount = 6;
				asrcCPU00Core.psrcAI[14 + i + 1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[14 + i + 1].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
				asrcCPU00Core.psrcAI[14 + i + 1].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			}
			asrcCPU00Core.psrcAI[29].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[29].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TowerFrontX00AI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 70;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[0].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[1].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 140;
			asrcCPU00Core.psrcAI[2].penuTypeMovZDash = CPU00.OperationTypeMovZDash.ZpDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovXDash = CPU00.OperationTypeMovXDash.XpDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovYDash = CPU00.OperationTypeMovYDash.YpDash;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TowerFrontX00_yAI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			asrcCPU00Core.psrcAI[0].pintLoopCount = 55;
			asrcCPU00Core.psrcAI[0].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
			asrcCPU00Core.psrcAI[0].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
			asrcCPU00Core.psrcAI[0].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			asrcCPU00Core.psrcAI[1].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[1].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			asrcCPU00Core.psrcAI[2].pintLoopCount = 140;
			asrcCPU00Core.psrcAI[2].penuTypeMovZDash = CPU00.OperationTypeMovZDash.ZpDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovXDash = CPU00.OperationTypeMovXDash.XpDash;
			asrcCPU00Core.psrcAI[2].penuTypeMovYDash = CPU00.OperationTypeMovYDash.YpDash;
			asrcCPU00Core.psrcAI[3].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[3].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private void TowerFrontX00_rAI(ref CPU00.srcCPU00Core asrcCPU00Core)
	{
		if (AIEndHantei(asrcCPU00Core.psrcAI))
		{
			for (int i = 0; i < 12; i += 2)
			{
				asrcCPU00Core.psrcAI[i].pintLoopCount = 12;
				asrcCPU00Core.psrcAI[i].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[i].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
				asrcCPU00Core.psrcAI[i].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
				asrcCPU00Core.psrcAI[i + 1].pintLoopCount = 6;
				asrcCPU00Core.psrcAI[i + 1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[i + 1].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
				asrcCPU00Core.psrcAI[i + 1].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			}
			asrcCPU00Core.psrcAI[13].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[13].penuTypeMovAttack = CPU00.OperationTypeMovAttack.Attack;
			for (int i = 0; i < 14; i += 2)
			{
				asrcCPU00Core.psrcAI[14 + i].pintLoopCount = 12;
				asrcCPU00Core.psrcAI[14 + i].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[14 + i].penuTypeMovX = CPU00.OperationTypeMovX.Xp;
				asrcCPU00Core.psrcAI[14 + i].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
				asrcCPU00Core.psrcAI[14 + i + 1].pintLoopCount = 6;
				asrcCPU00Core.psrcAI[14 + i + 1].penuTypeMovZ = CPU00.OperationTypeMovZ.Zp;
				asrcCPU00Core.psrcAI[14 + i + 1].penuTypeMovX = CPU00.OperationTypeMovX.Xm;
				asrcCPU00Core.psrcAI[14 + i + 1].penuTypeMovY = CPU00.OperationTypeMovY.Yp;
			}
			asrcCPU00Core.psrcAI[29].pintLoopCount = 0;
			asrcCPU00Core.psrcAI[29].penuTypeDead = CPU00.OperationTypeDead.Dead;
		}
	}

	private bool AIEndHantei(CPU00.structCPU00AI[] asrcCPU00AI)
	{
		for (int i = 0; i < asrcCPU00AI.Length; i++)
		{
			if (asrcCPU00AI[i].pintLoopCount != 0 || asrcCPU00AI[i].penuTypeMovX != CPU00.OperationTypeMovX.None || asrcCPU00AI[i].penuTypeMovY != CPU00.OperationTypeMovY.None || asrcCPU00AI[i].penuTypeMovZ != CPU00.OperationTypeMovZ.None || asrcCPU00AI[i].penuTypeMovXDash != CPU00.OperationTypeMovXDash.None || asrcCPU00AI[i].penuTypeMovYDash != CPU00.OperationTypeMovYDash.None || asrcCPU00AI[i].penuTypeMovZDash != CPU00.OperationTypeMovZDash.None || asrcCPU00AI[i].penuTypeMovAttack != CPU00.OperationTypeMovAttack.None || asrcCPU00AI[i].penuTypeMuki != CPU00.OperationTypeMukiX.None)
			{
				return false;
			}
		}
		return true;
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}
}
