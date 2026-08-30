using IMAK3Z0MB1EGAEM.audio;
using Microsoft.Xna.Framework;
using Viking_x86.vikinggame.character;

namespace Viking_x86.character;

public class CharacterMgr
{
	public Character[] character;

	public Moon moon;

	internal void Reset()
	{
		for (int i = 0; i < character.Length; i++)
		{
			character[i].exists = false;
		}
		moon.active = false;
		Sound.Play("warpin");
		for (int j = 0; j < 2; j++)
		{
			if (VikingGame.mainPlayerIdx[j] > -1)
			{
				character[j].Init(new Vector2(0f + (float)j * 40f, 800f), (j != 0) ? 1 : 0, 1, 0);
				character[j].nameIn = 0;
				character[j].respawnFrame = 0f;
				character[j].SetAnimation("warpin", 0, overRide: true);
				character[j].delta = 1f;
				character[j].score = 0L;
				character[j].SetShot(0);
			}
		}
	}

	public CharacterMgr()
	{
		moon = new Moon();
		character = new Character[64];
		for (int i = 0; i < character.Length; i++)
		{
			character[i] = new Character(i);
		}
	}

	public void Init(int def, Vector2 loc, Vector2 traj, int state, int face, int team)
	{
		int num = ((team == 1) ? 2 : 0);
		for (int i = num; i < character.Length; i++)
		{
			if (character[i].exists)
			{
				continue;
			}
			character[i].Init(loc, def, face, team);
			character[i].state = state;
			character[i].traj = traj;
			switch (def)
			{
			default:
				switch (state)
				{
				case 1:
					character[i].SetAnimation("idle", 0, overRide: true);
					break;
				case 0:
					character[i].SetAnimation("fly", 0, overRide: true);
					break;
				}
				break;
			case 3:
			case 4:
			case 5:
			case 6:
			case 7:
			case 8:
				break;
			}
			break;
		}
	}

	public void Update()
	{
		for (int i = 0; i < character.Length; i++)
		{
			if (character[i].exists)
			{
				character[i].Update();
			}
		}
		if (moon.active)
		{
			moon.Update();
		}
	}

	public void Draw()
	{
		if (moon.active)
		{
			moon.Draw();
		}
		for (int i = 0; i < character.Length; i++)
		{
			if (character[i].exists)
			{
				character[i].Draw();
			}
		}
	}

	internal void ClearMonsters()
	{
		for (int i = 2; i < character.Length; i++)
		{
			character[i].exists = false;
		}
	}
}
