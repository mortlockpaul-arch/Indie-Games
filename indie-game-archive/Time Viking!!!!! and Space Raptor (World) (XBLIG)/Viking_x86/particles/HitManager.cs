using Viking_x86.character;

namespace Viking_x86.particles;

public class HitManager
{
	public static bool CheckHit(Particle p)
	{
		return CheckHit(p, 0f);
	}

	public static bool CheckHit(Particle p, float buf)
	{
		Character[] character = Game1.vgame.charMgr.character;
		for (int i = 0; i < character.Length; i++)
		{
			if (character[i].exists && character[i].team != character[p.owner].team && character[i].CheckHit(p.loc, buf))
			{
				character[i].Hit(p.owner);
				return true;
			}
		}
		if (Game1.vgame.charMgr.moon.active && character[p.owner].team == 0 && Game1.vgame.charMgr.moon.GetDif() > 300f && Game1.vgame.charMgr.moon.CheckHit(p.loc, p.owner))
		{
			return true;
		}
		return false;
	}
}
