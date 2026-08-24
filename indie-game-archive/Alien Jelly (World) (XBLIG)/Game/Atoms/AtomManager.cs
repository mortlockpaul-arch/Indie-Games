using System;
using System.Collections.Generic;
using GKEngine.Cameras;
using GKEngine.Entities;
using GKEngine.Scenes;
using Game.Data;
using Game.Grids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Atoms;

public class AtomManager
{
	public enum Mode
	{
		Build,
		Play
	}

	protected const int Y_ERROR_POP = 10;

	protected const int ATOM_RANDOMIZE_ATTEMPTS = 30;

	private Base3D _flipBase = new Base3D();

	public Matrix _flipRotationMatrix = default(Matrix);

	public Scene scene;

	public Mode mode;

	public Dictionary<string, AtomInstancer> instancers;

	public List<AtomSingle> singles;

	public List<Atom> atoms;

	public int lengthAtoms;

	public int lengthSingles;

	public Grid grid;

	public Matrix inverse;

	public AtomManager(Scene oScene, Grid oGrid, Mode oMode)
	{
		scene = oScene;
		grid = oGrid;
		mode = oMode;
		Init();
	}

	public virtual void Init()
	{
		instancers = new Dictionary<string, AtomInstancer>();
		atoms = new List<Atom>();
		singles = new List<AtomSingle>();
		lengthAtoms = 0;
		lengthSingles = 0;
		inverse = Matrix.Invert(Matrix.Identity);
	}

	public virtual void Update(GameTime elapsed)
	{
	}

	public void RenderDepthEffect(ref Effect oEffectInstanced, ref Effect oEffectSingle)
	{
		foreach (KeyValuePair<string, AtomInstancer> instancer in instancers)
		{
			if (instancer.Value.renderDepth)
			{
				instancer.Value.RenderEffect(ref oEffectInstanced);
			}
		}
		for (int i = 0; i < lengthSingles; i++)
		{
			if (singles[i].renderDepth)
			{
				singles[i].RenderEffect(ref oEffectSingle);
			}
		}
	}

	public void RenderShadowEffect(ref Effect oEffectInstanced, ref Effect oEffectSingle)
	{
		foreach (KeyValuePair<string, AtomInstancer> instancer in instancers)
		{
			if (instancer.Value.renderDepth)
			{
				instancer.Value.RenderEffect(ref oEffectInstanced);
			}
		}
		for (int i = 0; i < lengthSingles; i++)
		{
			if (singles[i].renderDepth)
			{
				singles[i].RenderEffect(ref oEffectSingle);
			}
		}
	}

	public virtual void Dispose()
	{
		foreach (KeyValuePair<string, AtomInstancer> instancer in instancers)
		{
			instancer.Value.Dispose();
		}
		for (int i = 0; i < lengthSingles; i++)
		{
			singles[i].Dispose();
		}
		instancers.Clear();
		atoms.Clear();
		singles.Clear();
		lengthAtoms = 0;
		lengthSingles = 0;
	}

	private int Compare_Depth(IRenderable oEnt1, IRenderable oEnt2)
	{
		Atom atom = oEnt1 as Atom;
		Atom atom2 = oEnt2 as Atom;
		Camera camera = scene.cameras.camera;
		if (atom == null)
		{
			if (atom2 == null)
			{
				return 0;
			}
			return -1;
		}
		if (atom2 == null)
		{
			return 1;
		}
		float value = Vector3.Distance(camera.position, atom.position);
		return Vector3.Distance(camera.position, atom2.position).CompareTo(value);
	}

	public bool Atoms_Add(Atom oAtom)
	{
		bool result = true;
		if (oAtom.definition.instanced)
		{
			AtomInstanced atomInstanced = oAtom as AtomInstanced;
			atomInstanced.AddToInstancer();
		}
		else
		{
			singles.Add(oAtom as AtomSingle);
		}
		atoms.Add(oAtom);
		lengthAtoms = atoms.Count;
		lengthSingles = singles.Count;
		grid.Add(oAtom);
		return result;
	}

	public virtual void Atoms_Remove(Atom oAtom)
	{
		atoms.Remove(oAtom);
		grid.Remove(oAtom.guid);
		if (oAtom.definition.instanced)
		{
			AtomInstanced atomInstanced = oAtom as AtomInstanced;
			atomInstanced.RemoveFromInstancer();
			if (atomInstanced.instancer.count <= 0)
			{
				instancers.Remove(atomInstanced.definition.name);
				atomInstanced.instancer.Dispose();
				atomInstanced.instancer = null;
			}
		}
		else
		{
			AtomSingle atomSingle = oAtom as AtomSingle;
			singles.Remove(oAtom as AtomSingle);
			atomSingle.Dispose();
		}
		lengthAtoms = atoms.Count;
		lengthSingles = singles.Count;
		oAtom = null;
	}

	public void Atoms_Refresh()
	{
		for (int i = 0; i < lengthAtoms; i++)
		{
			atoms[i].point.LinkRefresh();
			if (atoms[i].definition.instanced)
			{
				AtomInstanced atomInstanced = atoms[i] as AtomInstanced;
				atomInstanced.PopulateInstancer();
			}
		}
	}

	protected virtual void Atoms_Flush()
	{
		while (lengthAtoms > 0)
		{
			Atoms_Remove(atoms[0]);
		}
		atoms.Clear();
		singles.Clear();
		lengthAtoms = 0;
		lengthSingles = 0;
		grid.Flush();
	}

	public virtual void Atoms_FromData(DataLevel oLevel)
	{
		List<Atom> list = new List<Atom>();
		Atoms_Flush();
		for (int i = 0; i < oLevel.atoms.Count; i++)
		{
			Atoms_FromData_Atom(AtomCatalog.atoms[oLevel.atoms[i].definition], oLevel.atoms[i]);
		}
		for (int i = 0; i < atoms.Count; i++)
		{
			if (!(atoms[i] is AtomSwitch) || (atoms[i] as AtomSwitch).type != AtomSwitch.Types.Holograms || atoms[i].dataRef.children == null)
			{
				continue;
			}
			AtomSwitch atomSwitch = atoms[i] as AtomSwitch;
			list.Clear();
			for (int j = 0; j < atomSwitch.dataRef.children.Length; j++)
			{
				for (int k = 0; k < atoms.Count; k++)
				{
					if (atomSwitch.dataRef.children[j] == atoms[k].guid.value)
					{
						list.Add(atoms[k]);
					}
				}
			}
			atomSwitch.children = list.ToArray();
		}
	}

	protected virtual Atom Atoms_FromData_Atom(AtomDefinition oDef, DataAtom oData)
	{
		Atom atom = oDef.MakeAtom(this, oData.guid);
		atom.point.FromVector3(oData.point);
		atom.rotation = oData.rotation;
		atom.AreaRotate();
		atom.data = oData.data;
		atom.dataRef = oData;
		atom.StateSet(oData.state);
		atom.properties = oData.properties;
		if (atom is AtomSwitch && oData.focus != null && oData.focus.Length > 0)
		{
			AtomSwitch atomSwitch = atom as AtomSwitch;
			atomSwitch.focus = new Base3D(oData.focus[0].position, oData.focus[0].rotation, new Vector3(1f));
		}
		Atoms_Add(atom);
		return atom;
	}

	public Atom Atoms_FromGUID(string xGUID)
	{
		Atom result = null;
		for (int i = 0; i < lengthAtoms; i++)
		{
			if (atoms[i].guid.value == xGUID)
			{
				result = atoms[i];
				break;
			}
		}
		return result;
	}

	public uint Atoms_Count_Type(AtomDefinition.Type xDefinitionType)
	{
		uint num = 0u;
		for (int i = 0; i < atoms.Count; i++)
		{
			if (atoms[i].definition.type == xDefinitionType)
			{
				num++;
			}
		}
		return num;
	}

	public AtomInstancer Instancers_Make(AtomDefinition oDef)
	{
		AtomInstancer atomInstancer = oDef.MakeInstancer(this);
		instancers.Add(oDef.name, atomInstancer);
		return atomInstancer;
	}

	public void Flip(Vector3 vAxis, float xAmount)
	{
		Matrix.CreateFromAxisAngle(ref vAxis, (float)Math.PI / 2f * xAmount, out _flipRotationMatrix);
		for (int i = 0; i < lengthAtoms; i++)
		{
			_flipBase.matrix = Matrix.Multiply(atoms[i]._base.matrix, _flipRotationMatrix);
			atoms[i].position = _flipBase.position;
			if (atoms[i].definition.rotatable)
			{
				atoms[i].rotation = _flipBase.rotation;
			}
			if (atoms[i].definition.instanced)
			{
				(atoms[i] as AtomInstanced).PopulateInstancer();
			}
			atoms[i].Event_Flip_Update();
		}
	}
}
