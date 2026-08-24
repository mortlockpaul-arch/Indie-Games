using System;
using System.Collections.Generic;
using GKEngine.Entities;
using Game.Dialogs;
using Game.Grids;
using Game.Scenes;
using Game.Scenes.Build;
using Game.Scenes.Build.Players;
using Microsoft.Xna.Framework;

namespace Game.Atoms;

public class AtomPainter
{
	private const int ICONS_OFFSET_X = 100;

	private const int ICONS_OFFSET_Y = 30;

	private const int ICONS_SPACING = 30;

	public const string ICONS_PATH = "Content/UI/Atoms/Icons/";

	private const string EFFECT_OVER_PATH = "Content/Effects/Sprite/Sprite_Over";

	private const string DEFAULT_SET_NAME = "Default";

	public BuildScene scene;

	public AtomManager manager;

	public Player player;

	public Quaternion rotation = Quaternion.Identity;

	public List<Atom> undo;

	public AtomDefinition selected;

	public MaxModel cursor;

	private GridPoint[] cursorArea;

	public string currentSet = "Default";

	public AtomPainter(BuildScene oScene, AtomManager oManager)
	{
		manager = oManager;
		scene = oScene;
		Init();
	}

	public void Init()
	{
		undo = new List<Atom>();
		Hide();
	}

	public void Update(GameTime elapsed)
	{
	}

	public void Show(Player oPlayer)
	{
		player = oPlayer;
		if (selected == null)
		{
			selected = AtomCatalog.sets[currentSet][0];
		}
		Brushes_Select(selected);
		undo.Clear();
	}

	public void Hide()
	{
		undo.Clear();
	}

	public void Dispose()
	{
		undo.Clear();
		undo = null;
		selected = null;
		cursor = null;
		cursorArea = null;
	}

	public void Atoms_Add(GridPoint oPoint)
	{
		if ((manager as BuildAtomManager).BuildUnits_Check(selected))
		{
			scene.audio.EventCues_Trigger("Sound_Place");
			Atom atom = selected.MakeAtom(manager, null);
			atom.point.X = oPoint.X;
			atom.point.Y = oPoint.Y;
			atom.point.Z = oPoint.Z;
			manager.Atoms_Add(atom);
			atom.Event_Painted(this);
			Undo_Add(atom);
			Brushes_SetModel();
			(manager as BuildAtomManager).universe.Levels_MarkAsEdited();
			atom.InitBuild();
		}
	}

	public bool Brushes_Move(GridPoint oDelta)
	{
		bool result = true;
		if (!manager.grid.CanFit(cursorArea, player.point.X + oDelta.X, player.point.Y + oDelta.Y, player.point.Z + oDelta.Z))
		{
			result = false;
		}
		return result;
	}

	public void Brushes_Rotate(Vector3 vAxis)
	{
		if ((vAxis.X != 0f || vAxis.Y != 0f || vAxis.Z != 0f) && !selected.rotatable)
		{
			Quaternion quaternion = Quaternion.CreateFromAxisAngle(vAxis, (float)Math.PI / 2f);
			Brushes_SetAngle(rotation * quaternion);
		}
	}

	public void Brushes_SetAngle(Quaternion qRotation)
	{
		rotation = qRotation;
		player.shapeCursor.rotation = rotation;
		Brushes_TransformArea();
	}

	public void Brushes_TransformArea()
	{
		AtomShape atomShape = AtomCatalog.shapes[selected.shape];
		cursorArea = new GridPoint[atomShape.area.Length];
		for (int i = 0; i < cursorArea.Length; i++)
		{
			cursorArea[i] = new GridPoint();
			Vector3 value = atomShape.area[i].ToVector3();
			cursorArea[i].FromVector3(Vector3.Transform(value, player.shapeCursor.rotation));
		}
	}

	public void Brushes_Select(AtomDefinition oDef)
	{
		selected = oDef;
		Brushes_SetModel();
	}

	public void Brushes_SetModel()
	{
		bool flag = false;
		GridPoint oPosition = new GridPoint(player.point.X, player.point.Y, player.point.Z);
		AtomShape atomShape = ((selected.brushProxy == null) ? AtomCatalog.shapes[selected.shape] : AtomCatalog.shapes[selected.brushProxy]);
		cursor = atomShape.model;
		Brushes_SetAngle(rotation);
		if (Brushes_FitModel(cursorArea, ref oPosition))
		{
			player.point.X = oPosition.X;
			player.point.Y = oPosition.Y;
			player.point.Z = oPosition.Z;
			player.point.ToPosition(ref player._position);
			player.position = player._position;
			scene.universe.ui.RenderIcon(selected);
		}
		else
		{
			scene.universe.Modes_SetEdit();
		}
	}

	public void Brushes_PopulateMenu(DialogIconMenu oMenu)
	{
		List<DialogIconMenuOption> list = new List<DialogIconMenuOption>();
		if (AtomCatalog.sets.ContainsKey(currentSet))
		{
			oMenu.Text_SetTitle("PARTS: " + currentSet.ToUpper());
			for (int i = 0; i < AtomCatalog.sets[currentSet].Count; i++)
			{
				AtomDefinition atomDefinition = AtomCatalog.sets[currentSet][i];
				if (!atomDefinition.isDevOnly)
				{
					list.Add(new DialogIconMenuOption(oMenu, "Content/UI/Atoms/Icons/" + atomDefinition.name, delegate
					{
						AtomDefinition oDef = oMenu.options[oMenu.currentIndex].data as AtomDefinition;
						Brushes_Select(oDef);
					}, atomDefinition));
					if (atomDefinition == selected)
					{
						list[list.Count - 1].selected = true;
					}
				}
			}
		}
		int num = 0;
		foreach (KeyValuePair<string, List<AtomDefinition>> set in AtomCatalog.sets)
		{
			if (set.Key == currentSet)
			{
				oMenu.manager.data = num;
			}
			num++;
		}
		oMenu.exit = delegate(Dialog dialog)
		{
			dialog.manager.Show("BrushMenu");
		};
		oMenu.Options_Set(list);
	}

	public bool Brushes_FitModel_Search(GridPoint[] aCursorArea, ref GridPoint oPosition)
	{
		bool flag = false;
		GridPoint gridPoint = new GridPoint();
		int num = 1;
		int num2 = 0;
		while (!flag && num <= 10)
		{
			flag = manager.grid.CanFit(aCursorArea, oPosition.X + gridPoint.X, oPosition.Y + gridPoint.Y, oPosition.Z + gridPoint.Z);
			if (flag)
			{
				continue;
			}
			switch (num2)
			{
			case 0:
				gridPoint.X = 0;
				gridPoint.Z = num;
				num2 = 1;
				break;
			case 1:
				gridPoint.X--;
				if (gridPoint.X <= -num)
				{
					num2 = 2;
				}
				break;
			case 2:
				gridPoint.Z--;
				if (gridPoint.Z <= -num)
				{
					num2 = 3;
				}
				break;
			case 3:
				gridPoint.X++;
				if (gridPoint.X >= num)
				{
					num2 = 4;
				}
				break;
			case 4:
				gridPoint.Z++;
				if (gridPoint.Z >= num)
				{
					num2 = 5;
				}
				break;
			case 5:
				gridPoint.X--;
				if (gridPoint.X >= 0)
				{
					num2 = 0;
					num++;
				}
				break;
			}
		}
		if (flag)
		{
			oPosition.X += gridPoint.X;
			oPosition.Y += gridPoint.Y;
			oPosition.Z += gridPoint.Z;
		}
		return flag;
	}

	public bool Brushes_FitModel(GridPoint[] aCursorArea, ref GridPoint oPosition)
	{
		bool flag = false;
		GridPoint gridPoint = new GridPoint();
		Vector3 vector = default(Vector3);
		int num = 0;
		float num2 = 0f;
		if (undo.Count >= 2)
		{
			vector.X = undo[undo.Count - 1].point.X - undo[undo.Count - 2].point.X;
			vector.Y = undo[undo.Count - 1].point.Y - undo[undo.Count - 2].point.Y;
			vector.Z = undo[undo.Count - 1].point.Z - undo[undo.Count - 2].point.Z;
			num2 = (float)Math.Ceiling(vector.Length() * 2f);
			vector.Normalize();
			if (Math.Abs(vector.X) >= Math.Abs(vector.Y) && Math.Abs(vector.X) >= Math.Abs(vector.Z))
			{
				vector.X = Math.Sign(vector.X);
				vector.Y = 0f;
				vector.Z = 0f;
			}
			else if (Math.Abs(vector.Z) >= Math.Abs(vector.Y) && Math.Abs(vector.Z) >= Math.Abs(vector.X))
			{
				vector.X = 0f;
				vector.Y = 0f;
				vector.Z = Math.Sign(vector.Z);
			}
			else if (Math.Abs(vector.Y) >= Math.Abs(vector.X) && Math.Abs(vector.Y) >= Math.Abs(vector.Z))
			{
				vector.X = 0f;
				vector.Y = Math.Sign(vector.Y);
				vector.Z = 0f;
			}
			while (!flag && (float)num <= num2)
			{
				flag = manager.grid.CanFit(aCursorArea, oPosition.X + gridPoint.X, oPosition.Y + gridPoint.Y, oPosition.Z + gridPoint.Z);
				if (!flag)
				{
					gridPoint.FromVector3(vector * num);
					num++;
				}
			}
		}
		if (flag)
		{
			oPosition.X += gridPoint.X;
			oPosition.Y += gridPoint.Y;
			oPosition.Z += gridPoint.Z;
		}
		else
		{
			flag = Brushes_FitModel_Search(aCursorArea, ref oPosition);
		}
		return flag;
	}

	public void Brushes_Sets_PopulateMenu(DialogMenu oMenu)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		foreach (KeyValuePair<string, List<AtomDefinition>> set in AtomCatalog.sets)
		{
			list.Add(new DialogMenuOption(set.Key.ToUpper(), delegate
			{
				string text = (string)oMenu.options[oMenu.selectedIndex].data;
				currentSet = text;
				oMenu.manager.Show("BrushSelectMenu");
			}, set.Key));
		}
		oMenu.Options_Set(list);
	}

	public void Brushes_Next(int xDir)
	{
		rotation = Quaternion.Identity;
		(manager.scene as BuildScene).audio.EventCues_Trigger("Build Snap");
		Brushes_Select(AtomCatalog.Next(xDir, selected));
	}

	public void Undo_Add(Atom oAtom)
	{
		undo.Add(oAtom);
	}

	public void Undo()
	{
		if (undo.Count > 0)
		{
			player.Atoms_Cursor_Change(undo[undo.Count - 1].point);
			undo[undo.Count - 1].visible = false;
			manager.Atoms_Remove(undo[undo.Count - 1]);
			rotation = undo[undo.Count - 1].rotation;
			Brushes_Select(undo[undo.Count - 1].definition);
			undo.Remove(undo[undo.Count - 1]);
		}
	}
}
