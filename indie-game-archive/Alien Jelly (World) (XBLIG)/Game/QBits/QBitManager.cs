using System.Collections.Generic;
using GKEngine.Cameras;
using GKEngine.Entities;
using GKEngine.Scenes;
using Game.Grids;
using Game.Scenes.Play;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.QBits;

public class QBitManager
{
	public PlayUniverse universe;

	public Grid grid;

	public PlayAtomManager atoms;

	public List<QBit> qbits;

	public EntityStack renderStack;

	public QBitConversations conversation;

	public QBitManager(PlayUniverse oUniverse)
	{
		universe = oUniverse;
		grid = universe.grid;
		atoms = universe.atoms;
		Init();
	}

	public void Init()
	{
		qbits = new List<QBit>();
		conversation = new QBitConversations(this);
		renderStack = universe.scene.RenderStacks_FromName(GameMain.RENDERSTACK_ALPHA_SORTED);
	}

	public void Update(GameTime elapsed)
	{
		for (int i = 0; i < qbits.Count; i++)
		{
			qbits[i].Update(elapsed);
		}
		conversation.Update(elapsed);
	}

	public void RenderEffect(ref Effect oEffect)
	{
		for (int i = 0; i < qbits.Count; i++)
		{
			qbits[i].RenderEffect(ref oEffect);
		}
	}

	public void Add(QBit oQBit)
	{
		qbits.Add(oQBit);
		oQBit.Brain_SetPosition();
	}

	public void Remove(QBit oQBit)
	{
		qbits.Remove(oQBit);
	}

	public void Flush()
	{
		while (qbits.Count > 0)
		{
			Remove(qbits[0]);
		}
	}

	public void Dispose()
	{
		for (int i = 0; i < qbits.Count; i++)
		{
			qbits[i].Dispose();
		}
		conversation.Dispose();
		qbits.Clear();
		qbits = null;
		universe = null;
		grid = null;
		atoms = null;
		renderStack = null;
		conversation = null;
	}

	public QBit At(float xX, float xY, float xZ, QBit oException)
	{
		QBit result = null;
		for (int i = 0; i < qbits.Count; i++)
		{
			if (oException != qbits[i] && xX > qbits[i].X - Grid.SPACING.X && xX < qbits[i].X + Grid.SPACING.X && xY > qbits[i].Y - Grid.SPACING.Y && xY < qbits[i].Y + Grid.SPACING.Y && xZ > qbits[i].Z - Grid.SPACING.Z && xZ < qbits[i].Z + Grid.SPACING.Z)
			{
				result = qbits[i];
				break;
			}
		}
		return result;
	}

	public int DeadCount()
	{
		int num = 0;
		for (int i = 0; i < qbits.Count; i++)
		{
			if (qbits[i].dead)
			{
				num++;
			}
		}
		return num;
	}

	public int ActiveCount()
	{
		int num = 0;
		for (int i = 0; i < qbits.Count; i++)
		{
			if (!qbits[i].dead && !qbits[i].home)
			{
				num++;
			}
		}
		return num;
	}

	private int Compare_Depth(IRenderable oEnt1, IRenderable oEnt2)
	{
		Base3D base3D = oEnt1 as Base3D;
		Base3D base3D2 = oEnt2 as Base3D;
		Camera camera = universe.scene.cameras.camera;
		if (base3D == null)
		{
			if (base3D2 == null)
			{
				return 0;
			}
			return -1;
		}
		if (base3D2 == null)
		{
			return 1;
		}
		float value = Vector3.Distance(camera.position, base3D.position);
		return Vector3.Distance(camera.position, base3D2.position).CompareTo(value);
	}

	public int PhasedUnder(QBit pQBit)
	{
		QBit qBit = null;
		int result = grid.fromY;
		for (int num = pQBit.gridY; num >= grid.fromY; num--)
		{
			qBit = At((float)pQBit.gridX * Grid.SPACING.X, (float)num * Grid.SPACING.Y, (float)pQBit.gridZ * Grid.SPACING.Z, pQBit);
			if (qBit != null && qBit.sticky)
			{
				result = num;
				break;
			}
		}
		return result;
	}

	public void Fall_Start()
	{
		for (int i = 0; i < qbits.Count; i++)
		{
			qbits[i].PhysicsStart();
		}
	}

	public void Snap_All()
	{
		for (int i = 0; i < qbits.Count; i++)
		{
			qbits[i].SnapToGrid();
		}
	}

	public void Event_Physics_End(QBit oQBit)
	{
		atoms.Event_QBit_Moved(oQBit);
	}

	public void Event_Flip_End()
	{
		for (int i = 0; i < qbits.Count; i++)
		{
			if (qbits[i].playable && qbits[i].Check_FallToDeath() && qbits[i].player == null)
			{
				universe.players.primaryPlayer.QBit_Set(qbits[i]);
				break;
			}
		}
	}
}
