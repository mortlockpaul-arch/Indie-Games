using System;
using System.Collections.Generic;
using GKEngine.Scenes;
using Game.Atoms;
using Game.Data;
using Game.Grids;
using Microsoft.Xna.Framework;

namespace Game.Scenes.Build;

public class BuildAtomManager : AtomManager
{
	public enum ProcessingAction
	{
		Move,
		Rotate
	}

	private const int FLIP_TIME = 500;

	private const uint BUILD_UNITS_MAX = 300u;

	private const uint PROCESSING_THRESHOLD = 8u;

	public BuildUniverse universe;

	public Atom over;

	public List<Atom> selected;

	public bool flipping;

	public float flipTime;

	public Vector3 flipAxis = default(Vector3);

	public bool processing;

	private int processingIndex;

	private ProcessingAction processingAction;

	private GridPoint processingMoveDelta;

	private Vector3 processingRotationPivot;

	private Quaternion processingRotation;

	public BuildAtomManager(Scene oScene, Grid oGrid, BuildUniverse oUniverse)
		: base(oScene, oGrid, Mode.Build)
	{
		universe = oUniverse;
	}

	public override void Init()
	{
		base.Init();
		selected = new List<Atom>();
	}

	public override void Dispose()
	{
		base.Dispose();
		selected.Clear();
	}

	public override void Update(GameTime elapsed)
	{
		if (flipping)
		{
			Flip_Update(elapsed);
		}
		for (int i = 0; i < atoms.Count; i++)
		{
			if (atoms[i] is AtomSwitch)
			{
				atoms[i].Update(elapsed);
			}
		}
	}

	public void UpdateProcessing(GameTime elapsed)
	{
		Atom oAtom = selected[processingIndex];
		switch (processingAction)
		{
		case ProcessingAction.Move:
			Select_Move_Atom(processingMoveDelta, ref oAtom);
			break;
		case ProcessingAction.Rotate:
			Select_Rotate_Atom(processingRotationPivot, processingRotation, ref oAtom);
			break;
		}
		processingIndex++;
		if (processingIndex >= selected.Count)
		{
			processing = false;
			processingIndex = 0;
		}
	}

	protected override void Atoms_Flush()
	{
		selected.Clear();
		over = null;
		base.Atoms_Flush();
	}

	public void Atoms_Delete()
	{
		if (selected.Count > 0)
		{
			for (int i = 0; i < selected.Count; i++)
			{
				Atoms_Remove(selected[i]);
			}
			selected.Clear();
			universe.Levels_MarkAsEdited();
			over = null;
		}
		else if (over != null)
		{
			Atoms_Remove(over);
			universe.Levels_MarkAsEdited();
			over = null;
		}
	}

	public void Atoms_Deselect(Atom oAtom)
	{
		if (selected.Contains(oAtom))
		{
			selected.Remove(oAtom);
		}
		oAtom.selected = false;
		oAtom.data.W = ((over == oAtom) ? 1 : 0);
		if (oAtom.definition.instanced)
		{
			AtomInstanced atomInstanced = oAtom as AtomInstanced;
			atomInstanced.PopulateInstancer();
		}
	}

	public void Atoms_Select(Atom oAtom)
	{
		if (!selected.Contains(oAtom))
		{
			selected.Add(oAtom);
		}
		oAtom.selected = true;
		oAtom.data.W = ((over == oAtom) ? 3 : 2);
		if (oAtom.definition.instanced)
		{
			AtomInstanced atomInstanced = oAtom as AtomInstanced;
			atomInstanced.PopulateInstancer();
		}
	}

	public void Atoms_ToData(DataLevel oLevel)
	{
		DataKeyFrame[] array = new DataKeyFrame[0];
		oLevel.atoms = new List<DataAtom>();
		for (int i = 0; i < lengthAtoms; i++)
		{
			atoms[i].data.W = 0f;
			int[] array2 = new int[atoms[i].properties.Length];
			atoms[i].properties.CopyTo(array2, 0);
			string[] array3;
			if (atoms[i] is AtomSwitch && (atoms[i] as AtomSwitch).type == AtomSwitch.Types.Holograms && (atoms[i] as AtomSwitch).children != null)
			{
				AtomSwitch atomSwitch = atoms[i] as AtomSwitch;
				array3 = new string[atomSwitch.children.Length];
				for (int j = 0; j < atomSwitch.children.Length; j++)
				{
					array3[j] = atomSwitch.children[j].guid.value;
				}
			}
			else
			{
				array3 = new string[0];
			}
			array = new DataKeyFrame[0];
			if (atoms[i] is AtomSwitch && (atoms[i] as AtomSwitch).focus != null)
			{
				AtomSwitch atomSwitch = atoms[i] as AtomSwitch;
				array = new DataKeyFrame[1]
				{
					new DataKeyFrame(atomSwitch.focus.position, atomSwitch.focus.rotation)
				};
			}
			DataAtom item = new DataAtom(atoms[i].definition.name, atoms[i].guid.value, atoms[i].point.ToVector3(), atoms[i].data, atoms[i].rotation, atoms[i].state, array2, array3, array);
			oLevel.atoms.Add(item);
		}
	}

	protected override Atom Atoms_FromData_Atom(AtomDefinition oDef, DataAtom oData)
	{
		Atom atom = base.Atoms_FromData_Atom(oDef, oData);
		atom.InitBuild();
		return atom;
	}

	public void Atoms_CenterAll()
	{
		Select_SelectAll();
		GridPoint gridPoint = Select_GetCenter();
		Select_Move(new GridPoint(gridPoint.X * -1, gridPoint.Y * -1, gridPoint.Z * -1));
		Select_Deselect();
	}

	public int BuildUnits_Count()
	{
		int num = 0;
		for (int i = 0; i < atoms.Count; i++)
		{
			num += (int)atoms[i].definition.cost;
		}
		return num;
	}

	public int BuildUnits_Left()
	{
		return 300 - BuildUnits_Count();
	}

	public bool BuildUnits_Check(AtomDefinition oDef)
	{
		return BuildUnits_Count() + oDef.cost <= 300;
	}

	public void Cursor_Change(GridPoint oPoint)
	{
		Atom atom = grid.At(oPoint.X, oPoint.Y, oPoint.Z) as Atom;
		if (atom == over)
		{
			return;
		}
		if (over != null)
		{
			over.Unover();
		}
		over = atom;
		if (atom != null)
		{
			atom.data.W = ((!atom.selected) ? 1 : 3);
			if (atom.definition.instanced)
			{
				AtomInstanced atomInstanced = atom as AtomInstanced;
				atomInstanced.PopulateInstancer();
			}
		}
	}

	public GridPoint[] Select_GetArea(GridPoint oAxis)
	{
		List<GridPoint> list = new List<GridPoint>();
		for (int i = 0; i < selected.Count; i++)
		{
			for (int j = 0; j < selected[i].area.Length; j++)
			{
				list.Add(new GridPoint(selected[i].area[j].X + (selected[i].point.X - oAxis.X), selected[i].area[j].Y + (selected[i].point.Y - oAxis.Y), selected[i].area[j].Z + (selected[i].point.Z - oAxis.Z)));
			}
		}
		return list.ToArray();
	}

	public void Select_Select(GridPoint oPoint)
	{
		if (grid.At(oPoint.X, oPoint.Y, oPoint.Z) is Atom oAtom)
		{
			Atoms_Select(oAtom);
		}
	}

	public void Select_DeselectAt(GridPoint oPoint)
	{
		if (grid.At(oPoint.X, oPoint.Y, oPoint.Z) is Atom oAtom)
		{
			Atoms_Deselect(oAtom);
		}
	}

	public void Select_Toggle(GridPoint oPoint)
	{
		if (grid.At(oPoint.X, oPoint.Y, oPoint.Z) is Atom atom)
		{
			if (atom.selected)
			{
				Atoms_Deselect(atom);
			}
			else
			{
				Atoms_Select(atom);
			}
		}
	}

	public GridPoint Select_GetCenter()
	{
		GridPoint gridPoint = null;
		if (selected.Count > 0)
		{
			gridPoint = new GridPoint();
			for (int i = 0; i < selected.Count; i++)
			{
				gridPoint.X += selected[i].point.X;
				gridPoint.Y += selected[i].point.Y;
				gridPoint.Z += selected[i].point.Z;
			}
			gridPoint.X /= selected.Count;
			gridPoint.Y /= selected.Count;
			gridPoint.Z /= selected.Count;
		}
		return gridPoint;
	}

	public void Select_Deselect()
	{
		while (selected.Count > 0)
		{
			Atoms_Deselect(selected[0]);
		}
	}

	public void Select_SelectAll()
	{
		for (int i = 0; i < lengthAtoms; i++)
		{
			Atoms_Select(atoms[i]);
		}
	}

	public bool Select_Move(GridPoint oDelta)
	{
		bool flag = true;
		List<IGridable> list = new List<IGridable>(selected.Count);
		for (int i = 0; i < selected.Count; i++)
		{
			list.Add(selected[i]);
		}
		for (int i = 0; i < selected.Count; i++)
		{
			Atom atom = selected[i];
			if (!grid.CanFit(atom.area, atom.point.X + oDelta.X, atom.point.Y + oDelta.Y, atom.point.Z + oDelta.Z, list))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			(scene as BuildScene).audio.EventCues_Trigger("Sound_Place");
			for (int i = 0; i < selected.Count; i++)
			{
				grid.Remove(selected[i]);
			}
			for (int i = 0; i < selected.Count; i++)
			{
				Atom oAtom = selected[i];
				Select_Move_Atom(oDelta, ref oAtom);
			}
		}
		return flag;
	}

	private void Select_Move_Atom(GridPoint oDelta, ref Atom oAtom)
	{
		oAtom.point.X += oDelta.X;
		oAtom.point.Y += oDelta.Y;
		oAtom.point.Z += oDelta.Z;
		grid.Add(oAtom);
		if (oAtom.definition.instanced)
		{
			AtomInstanced atomInstanced = oAtom as AtomInstanced;
			atomInstanced.PopulateInstancer();
		}
	}

	public bool Select_Rotate(GridPoint oPivot, Vector3 vAxis)
	{
		bool flag = false;
		if (vAxis.X != 0f || vAxis.Y != 0f || vAxis.Z != 0f)
		{
			Quaternion quaternion = Quaternion.CreateFromAxisAngle(vAxis, (float)Math.PI / 2f);
			List<IGridable> list = new List<IGridable>(selected.Count);
			for (int i = 0; i < selected.Count; i++)
			{
				list.Add(selected[i]);
			}
			GridPoint[] array = Select_GetArea(oPivot);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].FromVector3(Vector3.Transform(array[i].ToVector3(), quaternion));
			}
			flag = grid.CanFit(array, oPivot.X, oPivot.Y, oPivot.Z, list);
			if (flag)
			{
				Vector3 vPivot = oPivot.ToVector3();
				for (int i = 0; i < selected.Count; i++)
				{
					grid.Remove(selected[i]);
				}
				if ((long)selected.Count >= 8L)
				{
					processingRotationPivot = vPivot;
					processingRotation = quaternion;
					processingIndex = 0;
					processingAction = ProcessingAction.Rotate;
					processing = true;
				}
				else
				{
					for (int i = 0; i < selected.Count; i++)
					{
						Atom oAtom = selected[i];
						Select_Rotate_Atom(vPivot, quaternion, ref oAtom);
					}
				}
			}
		}
		return flag;
	}

	private void Select_Rotate_Atom(Vector3 vPivot, Quaternion qValue, ref Atom oAtom)
	{
		oAtom.point.FromVector3(vPivot + Vector3.Transform(oAtom.point.ToVector3() - vPivot, qValue));
		if (oAtom.definition.rotatable)
		{
			oAtom.RotateAndUpdate(qValue * oAtom.rotation);
		}
	}

	public void Switch_LinkSelected()
	{
		List<AtomInstancedHologram> list = new List<AtomInstancedHologram>();
		if (!(over is AtomSwitch) || (over as AtomSwitch).type != AtomSwitch.Types.Holograms)
		{
			return;
		}
		AtomSwitch atomSwitch = over as AtomSwitch;
		for (int i = 0; i < selected.Count; i++)
		{
			if (selected[i] is AtomInstancedHologram)
			{
				list.Add(selected[i] as AtomInstancedHologram);
			}
		}
		atomSwitch.children = list.ToArray();
	}

	public void Switch_ShowSelected()
	{
		if (over is AtomSwitch && (over as AtomSwitch).type == AtomSwitch.Types.Holograms && (over as AtomSwitch).children != null)
		{
			Select_Deselect();
			AtomSwitch atomSwitch = over as AtomSwitch;
			for (int i = 0; i < atomSwitch.children.Length; i++)
			{
				Atoms_Select(atomSwitch.children[i]);
			}
		}
	}

	public void Switch_ClearLinked()
	{
		if (over is AtomSwitch && (over as AtomSwitch).type == AtomSwitch.Types.Holograms)
		{
			Select_Deselect();
			AtomSwitch atomSwitch = over as AtomSwitch;
			atomSwitch.children = new Atom[0];
		}
	}

	public void Flip_Start(Vector3 vAxis)
	{
		universe.player.inputPaused = true;
		flipTime = 0f;
		flipAxis = vAxis;
		grid.Flush();
		for (int i = 0; i < lengthAtoms; i++)
		{
			atoms[i].Event_Flip_Start();
		}
		flipping = true;
	}

	public void Flip_Update(GameTime oGameTime)
	{
		flipTime += oGameTime.ElapsedGameTime.Milliseconds;
		if (flipTime >= 500f)
		{
			Flip_End();
		}
		else
		{
			Flip(flipAxis, flipTime / 500f);
		}
	}

	public void Flip_End()
	{
		flipping = false;
		Flip(flipAxis, 1f);
		for (int i = 0; i < lengthAtoms; i++)
		{
			atoms[i].Event_Flip_End();
		}
		universe.player.inputPaused = false;
		universe.Levels_MarkAsEdited();
	}

	public void Event_Select_ChangeStart()
	{
	}

	public void Event_Select_ChangeEnd()
	{
		if (selected.Count > 0)
		{
			universe.Levels_MarkAsEdited();
		}
		if (selected.Count == 1)
		{
			Select_Deselect();
		}
	}
}
