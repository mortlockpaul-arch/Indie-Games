using GKEngine;
using GKEngine.Entities;
using Game.Grids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Atoms;

public class AtomMarker : AtomSingle, IGridable, IRenderable
{
	public static uint OUTOFBOUNDS = 9999u;

	public static string TITLE = "Portal Marker";

	public static string DESCRIPTION = "This object allows you to set the place where the Jelly goes to after entering a portal. Need to link to the portal in the portal properties.";

	public static string PROPERTIES_DESCRIPTION = "";

	public static AtomProperty[] PROPERTIES = new AtomProperty[1]
	{
		new AtomProperty("Marker Names", "This option allows you to set the name and color of the marker so that you dont get confused.", new string[26]
		{
			"Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel", "India", "Juliet",
			"Kilo", "Lima", "Mike", "November", "Oscar", "Papa", "Quebec", "Romeo", "Sierra", "Tango",
			"Uniform", "Victor", "Whiskey", "X-Ray", "Yankee", "Zulu"
		})
	};

	public static int[] PROPERTIES_DEFAULT;

	public static Color[] COLORS;

	private EffectParameter effectTint;

	public int type;

	public override int[] properties
	{
		get
		{
			return base.properties;
		}
		set
		{
			base.properties = value;
			if (value.Length > 0)
			{
				type = value[0];
				if (effectTint != null)
				{
					effectTint.SetValue(COLORS[type].ToVector4());
				}
			}
		}
	}

	public AtomMarker(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
	}

	public override void Load()
	{
		base.Load();
		effectTint = model.modelParts[0].material.effect.Parameters["Color"];
		effectTint.SetValue(COLORS[type].ToVector4());
	}

	public void SetUnique()
	{
		bool flag = false;
		int i;
		for (i = 0; i < PROPERTIES[0].options.Length; i++)
		{
			flag = false;
			for (int j = 0; j < manager.atoms.Count; j++)
			{
				if (manager.atoms[j] is AtomMarker && manager.atoms[j] != this)
				{
					int num = (manager.atoms[j] as AtomMarker).type;
					if (num == i)
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				break;
			}
		}
		if (!flag)
		{
			properties = new int[1] { i };
		}
		else
		{
			properties = new int[1] { GameEngine.random.Next(PROPERTIES[0].options.Length) };
		}
	}

	public override void Event_Painted(AtomPainter oPainter)
	{
		base.Event_Painted(oPainter);
		SetUnique();
	}

	static AtomMarker()
	{
		int[] pROPERTIES_DEFAULT = new int[1];
		PROPERTIES_DEFAULT = pROPERTIES_DEFAULT;
		COLORS = new Color[26]
		{
			new Color(255, 0, 0),
			new Color(0, 255, 0),
			new Color(0, 0, 255),
			new Color(255, 255, 0),
			new Color(255, 0, 255),
			new Color(0, 255, 255),
			new Color(128, 0, 0),
			new Color(0, 128, 0),
			new Color(0, 0, 128),
			new Color(128, 128, 0),
			new Color(128, 0, 128),
			new Color(0, 128, 128),
			new Color(255, 128, 128),
			new Color(128, 255, 128),
			new Color(128, 128, 255),
			new Color(255, 255, 128),
			new Color(255, 128, 255),
			new Color(128, 255, 255),
			new Color(255, 128, 0),
			new Color(255, 0, 128),
			new Color(0, 255, 128),
			new Color(128, 255, 0),
			new Color(0, 128, 255),
			new Color(128, 0, 255),
			new Color(32, 32, 32),
			new Color(192, 192, 192)
		};
	}
}
