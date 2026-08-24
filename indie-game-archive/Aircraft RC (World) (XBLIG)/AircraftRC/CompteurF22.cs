using Microsoft.Xna.Framework;

namespace AircraftRC;

public class CompteurF22
{
	public float timerS;

	public int timecounterSA;

	public int timecounterMA;

	public int timecounterHA;

	public int timecounterSA1;

	public int timecounterMA1;

	public int timecounterHA1;

	private int minCH;

	private int minCM;

	private int minCHi;

	private int minCMi;

	private int totalCH;

	private int totalCHi;

	public int totalCrash;

	public bool couleurT;

	public bool couleurG;

	public CompteurF22(CustomPhysicsGame game)
	{
		timecounterSA = 0;
		timecounterMA = 0;
		timecounterHA = 0;
	}

	public void Load(CustomPhysicsGame game)
	{
	}

	public void Unload(CustomPhysicsGame game)
	{
	}

	public void UpdateTime(CustomPhysicsGame game, GameTime gameTime)
	{
		timerS += (float)gameTime.ElapsedGameTime.TotalSeconds;
		checked
		{
			if (game.avion6.altitude >= 2f && !game.avion6.Avioncasse && game.gamemode == CustomPhysicsGame.GameMode.M0)
			{
				timecounterSA += (int)timerS;
			}
			if (timerS >= 1f)
			{
				timerS = 0f;
			}
			if (timecounterSA >= 60)
			{
				timecounterSA = 0;
				timecounterMA++;
				if (timecounterMA >= 60)
				{
					timecounterMA = 0;
					timecounterHA++;
				}
				if (timecounterHA >= 99)
				{
					timecounterHA = 0;
					timecounterMA = 0;
					timecounterSA = 0;
				}
			}
			minCH = timecounterHA * 60;
			minCM = timecounterMA * 60;
			minCHi = timecounterHA1 * 60;
			minCMi = timecounterMA1 * 60;
			totalCH = minCH + minCM + timecounterSA;
			totalCHi = minCHi + minCMi + timecounterSA1;
			if (totalCHi <= totalCH)
			{
				timecounterSA1 = timecounterSA;
				timecounterMA1 = timecounterMA;
				timecounterHA1 = timecounterHA;
				totalCrash = game.avion6.compteCrash;
				if (game.avion6.compteCrash <= totalCrash)
				{
					totalCrash = game.avion6.compteCrash;
				}
			}
			if (totalCH >= totalCHi)
			{
				couleurT = true;
			}
			else
			{
				couleurT = false;
			}
		}
	}
}
