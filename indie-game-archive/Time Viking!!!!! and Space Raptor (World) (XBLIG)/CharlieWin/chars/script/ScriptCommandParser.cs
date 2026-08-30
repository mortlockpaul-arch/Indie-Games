using System;

namespace CharlieWin.chars.script;

public class ScriptCommandParser
{
	public enum Command
	{
		JoyJump,
		SetAnim,
		GotoFrame,
		SetJump,
		Slide,
		Backup,
		SetAtkGoto,
		SetLowerGoto,
		SetUpperGoto,
		ClearGotos,
		Float,
		KillMe,
		Ethereal,
		Solid,
		Unfloat,
		SetLandAnim,
		SetFall,
		DeathCheck,
		IfUpGoto,
		IfDownGoto,
		SpeedFac,
		Slow,
		Play,
		Vox,
		CanCancel,
		Quake,
		NoBlock,
		AllBlock,
		KillMeNow,
		IfR25Goto,
		IfR50Goto,
		IfR75Goto,
		Width,
		RunSpeed,
		HP,
		HasWeapon,
		RandWeapon,
		DefReach,
		Biggie,
		Boss,
		CarWidth,
		Height,
		CarHeight,
		MidJump,
		SpawnZom,
		AboutFace
	}

	public static ScriptCommandDef[] command;

	public static void Init()
	{
		command = new ScriptCommandDef[64];
		command[0] = new ScriptCommandDef("joyjump", ScriptCommandDef.ParamType.None);
		command[1] = new ScriptCommandDef("setanim", ScriptCommandDef.ParamType.String);
		command[2] = new ScriptCommandDef("gotoframe", ScriptCommandDef.ParamType.Int);
		command[3] = new ScriptCommandDef("setjump", ScriptCommandDef.ParamType.Int);
		command[4] = new ScriptCommandDef("slide", ScriptCommandDef.ParamType.Int);
		command[5] = new ScriptCommandDef("backup", ScriptCommandDef.ParamType.Int);
		command[6] = new ScriptCommandDef("setatkgoto", ScriptCommandDef.ParamType.Int);
		command[9] = new ScriptCommandDef("cleargotos", ScriptCommandDef.ParamType.None);
		command[10] = new ScriptCommandDef("float", ScriptCommandDef.ParamType.None);
		command[14] = new ScriptCommandDef("unfloat", ScriptCommandDef.ParamType.None);
		command[11] = new ScriptCommandDef("killme", ScriptCommandDef.ParamType.None);
		command[28] = new ScriptCommandDef("killmenow", ScriptCommandDef.ParamType.None);
		command[12] = new ScriptCommandDef("ethereal", ScriptCommandDef.ParamType.None);
		command[13] = new ScriptCommandDef("solid", ScriptCommandDef.ParamType.None);
		command[15] = new ScriptCommandDef("setlandanim", ScriptCommandDef.ParamType.String);
		command[16] = new ScriptCommandDef("setfall", ScriptCommandDef.ParamType.Int);
		command[17] = new ScriptCommandDef("deathcheck", ScriptCommandDef.ParamType.None);
		command[18] = new ScriptCommandDef("ifupgoto", ScriptCommandDef.ParamType.Int);
		command[19] = new ScriptCommandDef("ifdowngoto", ScriptCommandDef.ParamType.Int);
		command[20] = new ScriptCommandDef("speedfac", ScriptCommandDef.ParamType.Int);
		command[21] = new ScriptCommandDef("slow", ScriptCommandDef.ParamType.Int);
		command[22] = new ScriptCommandDef("play", ScriptCommandDef.ParamType.String);
		command[23] = new ScriptCommandDef("vox", ScriptCommandDef.ParamType.String);
		command[24] = new ScriptCommandDef("cancancel", ScriptCommandDef.ParamType.None);
		command[25] = new ScriptCommandDef("quake", ScriptCommandDef.ParamType.Int);
		command[26] = new ScriptCommandDef("noblock", ScriptCommandDef.ParamType.None);
		command[27] = new ScriptCommandDef("allblock", ScriptCommandDef.ParamType.None);
		command[29] = new ScriptCommandDef("ifr25goto", ScriptCommandDef.ParamType.Int);
		command[30] = new ScriptCommandDef("ifr50goto", ScriptCommandDef.ParamType.Int);
		command[31] = new ScriptCommandDef("ifr75goto", ScriptCommandDef.ParamType.Int);
		command[32] = new ScriptCommandDef("width", ScriptCommandDef.ParamType.Int);
		command[33] = new ScriptCommandDef("runspeed", ScriptCommandDef.ParamType.Int);
		command[34] = new ScriptCommandDef("hp", ScriptCommandDef.ParamType.Int);
		command[35] = new ScriptCommandDef("hasweapon", ScriptCommandDef.ParamType.Int);
		command[36] = new ScriptCommandDef("randweapon", ScriptCommandDef.ParamType.Int);
		command[37] = new ScriptCommandDef("defreach", ScriptCommandDef.ParamType.Int);
		command[38] = new ScriptCommandDef("biggie", ScriptCommandDef.ParamType.None);
		command[39] = new ScriptCommandDef("boss", ScriptCommandDef.ParamType.None);
		command[40] = new ScriptCommandDef("carwidth", ScriptCommandDef.ParamType.Int);
		command[41] = new ScriptCommandDef("height", ScriptCommandDef.ParamType.Int);
		command[42] = new ScriptCommandDef("carheight", ScriptCommandDef.ParamType.Int);
		command[43] = new ScriptCommandDef("midjump", ScriptCommandDef.ParamType.None);
		command[44] = new ScriptCommandDef("spawnzom", ScriptCommandDef.ParamType.None);
		command[45] = new ScriptCommandDef("aboutface", ScriptCommandDef.ParamType.None);
	}

	public static ScriptCommand ParseString(string line)
	{
		for (int i = 0; i < command.Length; i++)
		{
			if (command[i] != null && line.Length >= command[i].text.Length && line.Substring(0, command[i].text.Length) == command[i].text)
			{
				return command[i].paramType switch
				{
					ScriptCommandDef.ParamType.String => new ScriptCommand((Command)i, -1, line.Substring(command[i].text.Length + 1)), 
					ScriptCommandDef.ParamType.Int => new ScriptCommand((Command)i, Convert.ToInt32(line.Substring(command[i].text.Length + 1)), ""), 
					_ => new ScriptCommand((Command)i, -1, ""), 
				};
			}
		}
		Console.WriteLine("Unrecognized command: " + line);
		return null;
	}
}
