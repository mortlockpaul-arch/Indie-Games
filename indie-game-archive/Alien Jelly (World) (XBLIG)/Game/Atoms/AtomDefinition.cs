using System.Collections.Generic;
using Game.Dialogs;
using Game.Scenes;
using Game.Scenes.Build;

namespace Game.Atoms;

public class AtomDefinition
{
	public enum Type
	{
		Normal,
		Exit,
		Switch,
		Filter,
		Collect,
		QBit,
		Robot,
		Crate,
		Marker,
		Platform,
		Dissapearing,
		Portal,
		Pain,
		Hologram,
		Info,
		Sign
	}

	public string title;

	public string desc;

	public string name;

	public string surface;

	public string shape;

	public string shapeProxy;

	public string brushProxy;

	public string renderStack = "Solid";

	public Type type;

	public uint cost;

	public string[] sets;

	public bool instanced;

	public bool playGrid = true;

	public bool autoRotate;

	public bool rotatable = true;

	public bool hueable = true;

	public bool clonable = true;

	public bool camCull = true;

	public bool isDevOnly;

	public bool timed;

	public float timedTime;

	public string propertiesDesc = "";

	public AtomProperty[] properties = new AtomProperty[0];

	public int[] propertiesDefault = new int[0];

	public AtomDefinition(string xTitle, string xDesc, string xName, string xSurface, string xShape, bool xInstanced, string xRanderStack, Type oType, uint xCost, bool xPlayGrid, string[] aSets, bool xAutoRotate)
	{
		title = xTitle;
		desc = xDesc;
		name = xName;
		surface = xSurface;
		shape = xShape;
		instanced = xInstanced;
		renderStack = xRanderStack;
		type = oType;
		cost = xCost;
		playGrid = xPlayGrid;
		sets = aSets;
		autoRotate = xAutoRotate;
	}

	public AtomInstancer MakeInstancer(AtomManager oManager)
	{
		AtomInstancer result = null;
		string text;
		if (instanced && ((text = name) == null || !(text == "Exotic Name Here")))
		{
			result = new AtomInstancer(oManager, this);
		}
		return result;
	}

	public Atom MakeAtom(AtomManager oManager, string xGUID)
	{
		if (instanced)
		{
			if (type == Type.Collect)
			{
				return new AtomInstancedCollect(oManager, this, xGUID);
			}
			if (type == Type.Hologram)
			{
				return new AtomInstancedHologram(oManager, this, xGUID);
			}
			return new AtomInstanced(oManager, this, xGUID);
		}
		if (type == Type.Collect)
		{
			return new AtomCollect(oManager, this, xGUID);
		}
		if (type == Type.Exit)
		{
			return new AtomExit(oManager, this, xGUID);
		}
		if (type == Type.Portal)
		{
			return new AtomPortal(oManager, this, xGUID);
		}
		if (type == Type.Marker)
		{
			return new AtomMarker(oManager, this, xGUID);
		}
		if (type == Type.Switch)
		{
			return new AtomSwitch(oManager, this, xGUID);
		}
		if (type == Type.Filter)
		{
			return new AtomFilter(oManager, this, xGUID);
		}
		if (type == Type.Dissapearing)
		{
			return new AtomDissapearing(oManager, this, xGUID);
		}
		if (type == Type.Info)
		{
			return new AtomInfo(oManager, this, xGUID);
		}
		if (type == Type.Sign)
		{
			return new AtomSign(oManager, this, xGUID);
		}
		return new AtomSingle(oManager, this, xGUID);
	}

	public static void Properties_PopulateMenu(DialogMenuBuild oMenu, Atom oAtom)
	{
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		if (oAtom != null)
		{
			oMenu.title = oAtom.definition.title;
			if (oAtom.definition.properties.Length > 0)
			{
				oMenu.desc = oAtom.definition.propertiesDesc;
				for (int i = 0; i < oAtom.definition.properties.Length; i++)
				{
					string xTitle = oAtom.definition.properties[i].title.ToUpper();
					list.Add(new DialogMenuOption(xTitle, delegate
					{
						int selectedIndex = oMenu.selectedIndex;
						oMenu.manager.dialogs["AtomPropertyValueMenu"].data = selectedIndex;
						oMenu.manager.Show("AtomPropertyValueMenu");
					}));
				}
			}
			else if (oAtom is AtomSwitch)
			{
				oMenu.desc = "Please select on option with regards to switched.";
				list.Add(new DialogMenuOption("Set Focus Point", delegate
				{
					(oAtom.manager as BuildAtomManager).universe.Modes_SetFocus_Start();
				}));
				if ((oAtom as AtomSwitch).focus != null)
				{
					list.Add(new DialogMenuOption("Clear Focus Point", delegate
					{
						(oAtom.manager as BuildAtomManager).universe.Modes_SetFocus_Clear();
					}));
				}
				if ((oAtom as AtomSwitch).type == AtomSwitch.Types.Holograms)
				{
					oMenu.desc = "Please select on option with regards to linking hologram blocks.";
					list.Add(new DialogMenuOption("Link selected Hologram blocks", delegate
					{
						(oAtom.manager as BuildAtomManager).Switch_LinkSelected();
					}));
					list.Add(new DialogMenuOption("Select linked Hologram blocks", delegate
					{
						(oAtom.manager as BuildAtomManager).Switch_ShowSelected();
					}));
					list.Add(new DialogMenuOption("Clear Hologram blocks links", delegate
					{
						(oAtom.manager as BuildAtomManager).Switch_ClearLinked();
					}));
				}
				list.Add(new DialogMenuOption("Back", null));
			}
			else
			{
				oMenu.desc = "This item type does not have any properties.";
				list.Add(new DialogMenuOption("OK", null));
			}
		}
		else
		{
			oMenu.title = "Properties Menu";
			oMenu.desc = "To view and set the properties of an item, move your cursor inside the item and try again. Only certain items will have properties.";
			list.Add(new DialogMenuOption("OK", null));
		}
		oMenu.Options_Set(list);
	}

	public static void Properties_PopulateValueMenu(DialogMenuBuild oMenu, Atom oAtom)
	{
		int num = (int)oMenu.data;
		if (oAtom != null && oAtom.definition.properties[num].options.Length > 0)
		{
			string text;
			if ((text = oAtom.definition.properties[num].options[0]) != null && text == "#MARKERS")
			{
				Properties_PopulateValueMarkerMenu(oMenu, oAtom);
			}
			else
			{
				Properties_PopulateValueStringMenu(oMenu, oAtom);
			}
		}
	}

	public static void Properties_PopulateValueStringMenu(DialogMenuBuild oMenu, Atom oAtom)
	{
		int xIndex = (int)oMenu.data;
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		oMenu.title = oAtom.definition.properties[xIndex].title;
		oMenu.desc = oAtom.definition.properties[xIndex].desc;
		for (int i = 0; i < oAtom.definition.properties[xIndex].options.Length; i++)
		{
			if (oAtom.properties[xIndex] == i)
			{
				oMenu.selectedIndex = i;
			}
			list.Add(new DialogMenuOption(oAtom.definition.properties[xIndex].options[i].ToUpper(), delegate
			{
				int num = (int)oMenu.options[oMenu.selectedIndex].data;
				oAtom.properties[num] = oMenu.selectedIndex;
				oAtom.properties = oAtom.properties;
				List<Atom> selected = (oMenu.manager.scene as BuildScene).universe.atoms.selected;
				for (int j = 0; j < selected.Count; j++)
				{
					if (selected[j].GetType() == oAtom.GetType())
					{
						selected[j].properties[num] = oMenu.selectedIndex;
						selected[j].properties = selected[j].properties;
					}
				}
				(oAtom.manager as BuildAtomManager).universe.Levels_MarkAsEdited();
				oMenu.manager.data = xIndex;
				oMenu.manager.Show("AtomPropertyMenu");
			}, xIndex));
		}
		oMenu.Options_Set(list);
	}

	public static void Properties_PopulateValueMarkerMenu(DialogMenuBuild oMenu, Atom oAtom)
	{
		int xIndex = (int)oMenu.data;
		List<DialogMenuOption> list = new List<DialogMenuOption>();
		oMenu.title = oAtom.definition.properties[xIndex].title;
		oMenu.desc = oAtom.definition.properties[xIndex].desc;
		for (int i = 0; i < oAtom.manager.lengthAtoms; i++)
		{
			if (oAtom.manager.atoms[i].definition.type == Type.Marker)
			{
				if (oAtom.properties[xIndex] == i)
				{
					oMenu.selectedIndex = i;
				}
				list.Add(new DialogMenuOption((AtomMarker.PROPERTIES[0].options[(oAtom.manager.atoms[i] as AtomMarker).type] + " Marker").ToUpper(), delegate
				{
					int num = (int)oMenu.options[oMenu.selectedIndex].data;
					oAtom.properties[num] = oMenu.selectedIndex;
					(oAtom.manager as BuildAtomManager).universe.Levels_MarkAsEdited();
					oMenu.manager.data = xIndex;
					oMenu.manager.Show("AtomPropertyMenu");
				}, xIndex));
			}
		}
		oMenu.Options_Set(list);
	}
}
