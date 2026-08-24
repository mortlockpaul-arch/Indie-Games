using System;
using System.Collections.Generic;
using GKEngine.Utils;
using Game.Atoms;
using Game.Data;
using Game.Grids;
using Game.Interactable;
using Game.QBits;
using Game.Robots;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Play;

public class PlayAtomManager : AtomManager
{
	public PlayUniverse universe;

	public List<AtomExit> exits;

	public List<Atom> triggers;

	public List<ICollectable> collects;

	public List<Vector3> markers;

	public int lengthTriggers;

	public PlayAtomManager(PlayUniverse oUniverse)
		: base(oUniverse.scene, oUniverse.grid, Mode.Play)
	{
		universe = oUniverse;
	}

	public override void Init()
	{
		base.Init();
		exits = new List<AtomExit>();
		triggers = new List<Atom>();
		collects = new List<ICollectable>();
		markers = new List<Vector3>();
	}

	public override void Dispose()
	{
		base.Dispose();
		exits.Clear();
		triggers.Clear();
		collects.Clear();
		markers.Clear();
	}

	public override void Update(GameTime elapsed)
	{
		for (int i = 0; i < lengthAtoms; i++)
		{
			atoms[i].Update(elapsed);
		}
	}

	public int Jems_Total()
	{
		int num = 0;
		for (int i = 0; i < collects.Count; i++)
		{
			if (collects[i] is AtomCollect || collects[i] is AtomInstancedCollect)
			{
				num++;
			}
		}
		return num;
	}

	public Vector3 Marker_FromIndex(int xIndex)
	{
		Vector3 result = new Vector3(0f, AtomMarker.OUTOFBOUNDS, 0f);
		if (markers.Count > xIndex)
		{
			return markers[xIndex];
		}
		return result;
	}

	private void Markers_Flip()
	{
		for (int i = 0; i < markers.Count; i++)
		{
			markers[i] = Vector3.Transform(markers[i], Quaternion.CreateFromAxisAngle(universe.flippingAxis, (float)Math.PI / 2f * (float)universe.flippingAmount));
			markers[i] = MathUtils.VectSnap(markers[i]);
		}
	}

	protected override void Atoms_Flush()
	{
		base.Atoms_Flush();
		exits.Clear();
		triggers.Clear();
		collects.Clear();
		markers.Clear();
	}

	public void Atoms_InitPlay()
	{
		for (int i = 0; i < lengthAtoms; i++)
		{
			if (atoms[i].trigger != null)
			{
				triggers.Add(atoms[i]);
			}
			if (atoms[i].definition.type == AtomDefinition.Type.Exit)
			{
				exits.Add(atoms[i] as AtomExit);
			}
			else if (atoms[i].definition.type == AtomDefinition.Type.Collect)
			{
				collects.Add(atoms[i] as ICollectable);
			}
			atoms[i].InitPlay();
		}
	}

	public override void Atoms_FromData(DataLevel oStructure)
	{
		base.Atoms_FromData(oStructure);
		Atoms_InitPlay();
	}

	protected override Atom Atoms_FromData_Atom(AtomDefinition oDef, DataAtom oData)
	{
		switch (oDef.type)
		{
		case AtomDefinition.Type.QBit:
		{
			QBit qBit = QBit.FromAtom(universe.qbits, oDef as AtomQBitDefinition);
			qBit.PositionFromPoint(oData.point);
			universe.qbits.Add(qBit);
			break;
		}
		case AtomDefinition.Type.Robot:
		{
			Robot robot = Robot.FromAtom(universe.robots, oDef, oData.properties);
			robot.PositionFromPoint(oData.point);
			robot.rotation = oData.rotation;
			universe.robots.Add(robot);
			robot.Start();
			break;
		}
		case AtomDefinition.Type.Crate:
		{
			Crate crate = Crate.FromAtom(universe.interactables, oDef);
			crate.PositionFromPoint(oData.point);
			universe.interactables.Add(crate);
			break;
		}
		case AtomDefinition.Type.Platform:
		{
			MovingSpikes movingSpikes = MovingSpikes.FromAtom(universe.interactables, oDef);
			movingSpikes.PositionFromPoint(oData.point);
			movingSpikes.rotation = oData.rotation;
			movingSpikes.properties = oData.properties;
			universe.interactables.Add(movingSpikes);
			movingSpikes.Start();
			break;
		}
		case AtomDefinition.Type.Marker:
			markers.Add(oData.point);
			break;
		default:
			base.Atoms_FromData_Atom(oDef, oData);
			break;
		}
		return null;
	}

	public Atom Triggers_Intersect(Vector3 oPosition)
	{
		Atom result = null;
		for (int i = 0; i < triggers.Count; i++)
		{
			if (Math.Abs(oPosition.X - triggers[i].X) < Grid.SPACING.X * 0.5f && Math.Abs(oPosition.Y - triggers[i].Y) < Grid.SPACING.Y * 0.5f && Math.Abs(oPosition.Z - triggers[i].Z) < Grid.SPACING.Z * 0.5f)
			{
				result = triggers[i];
				break;
			}
		}
		return result;
	}

	public Atom Triggers_FallInto(int xX, int xY, int xZ)
	{
		Atom atom = null;
		Vector3 oPosition = new Vector3((float)xX * Grid.SPACING.X, 0f, (float)xZ * Grid.SPACING.Z);
		for (int num = xY; num >= grid.fromY; num--)
		{
			oPosition.Y = (float)num * Grid.SPACING.Y;
			atom = Triggers_Intersect(oPosition);
			if (atom != null)
			{
				break;
			}
		}
		return atom;
	}

	public void Event_QBit_Moved(QBit oQBit)
	{
		if (universe.history.reversing)
		{
			return;
		}
		if (!oQBit.exiting || !oQBit.dying || !oQBit.home || !oQBit.dead)
		{
			Atom atom = oQBit.AtomBelow();
			if (atom != null && ((atom.definition.type == AtomDefinition.Type.Filter && oQBit.type != (atom as AtomFilter).qbit) || atom.definition.type == AtomDefinition.Type.Pain))
			{
				oQBit.Death();
			}
		}
		if (!oQBit.exiting || !oQBit.dying || !oQBit.home || !oQBit.dead)
		{
			Atom atom = Triggers_Intersect(oQBit.position);
			if (atom != null && atom.trigger != null && atom.trigger.triggered(oQBit))
			{
				atom.Event_Triggered_Start(oQBit);
			}
		}
	}

	public virtual void Event_Flip_Start()
	{
		for (int i = 0; i < lengthAtoms; i++)
		{
			atoms[i].Event_Flip_Start();
		}
	}

	public virtual void Event_Flip_End()
	{
		grid.Flush();
		Markers_Flip();
		for (int i = 0; i < lengthAtoms; i++)
		{
			atoms[i].Event_Flip_End();
		}
	}
}
