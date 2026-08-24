using GKEngine.Entities;
using Game.Grids;
using Game.Scenes.Play;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Game.Atoms;

public class AtomSign : AtomSingle, IGridable, IRenderable
{
	private const float TIME = 8000f;

	public static string TITLE = "Help Sign Board";

	public static string DESCRIPTION = "A holographic signboard that gives the player a hand on what to do. This item has properties.";

	public static string PROPERTIES_DESCRIPTION = "This item has properties that allow you to set the color of the sign board.";

	public static AtomProperty[] PROPERTIES = new AtomProperty[2]
	{
		new AtomProperty("Color", "Set the color of the signboard hologram.", new string[5] { "White", "Red", "Green", "Blue", "Yellow" }),
		new AtomProperty("Size", "Set the size of the signboard hologram.", new string[3] { "Normal", "Small", "Tiny" })
	};

	public static int[] PROPERTIES_DEFAULT;

	public static Color[] COLORS;

	private static Range TIME_OFF;

	private static Range TIME_ON;

	private static Range MULTI;

	public static Vector3[] SIZES;

	private EffectParameter effectRatio;

	private EffectParameter effectMulti;

	private EffectParameter effectColor;

	private Color color = Color.White;

	private float time;

	private float multiTime;

	private float multiTimeTotal;

	private float multiValue;

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
				color = COLORS[value[0]];
				if (effectColor != null)
				{
					effectColor.SetValue(color.ToVector4());
				}
			}
			if (value.Length > 1)
			{
				scale = SIZES[value[1]];
			}
		}
	}

	public AtomSign(AtomManager oManager, AtomDefinition oDefinition, string xGUID)
		: base(oManager, oDefinition, xGUID)
	{
	}

	public override void Load()
	{
		base.Load();
		effectRatio = model.modelParts[0].material.effect.Parameters["Ratio"];
		effectMulti = model.modelParts[0].material.effect.Parameters["Multi"];
		effectColor = model.modelParts[0].material.effect.Parameters["Color"];
		effectColor.SetValue(color.ToVector4());
	}

	public override void Dispose()
	{
		base.Dispose();
	}

	public override void Update(GameTime oGameTime)
	{
		base.Update(oGameTime);
		if (visible)
		{
			float num = (float)oGameTime.ElapsedGameTime.TotalMilliseconds;
			time = (time + num) % 8000f;
			multiTime += num;
			if (multiTime >= multiTimeTotal)
			{
				multiTime = 0f;
				if (multiValue == 0f)
				{
					multiValue = MULTI.random;
					multiTimeTotal = TIME_ON.random;
				}
				else
				{
					multiValue = 0f;
					multiTimeTotal = TIME_OFF.random;
				}
			}
			effectRatio.SetValue(time / 8000f);
			effectMulti.SetValue(multiValue);
		}
		if (play)
		{
			visible = (manager as PlayAtomManager).universe.players.hinting;
		}
	}

	static AtomSign()
	{
		int[] pROPERTIES_DEFAULT = new int[2];
		PROPERTIES_DEFAULT = pROPERTIES_DEFAULT;
		COLORS = new Color[5]
		{
			new Color(255, 255, 255),
			new Color(212, 0, 64),
			new Color(0, 212, 64),
			new Color(0, 102, 255),
			new Color(212, 190, 0)
		};
		TIME_OFF = new Range(250f, 5000f);
		TIME_ON = new Range(200f, 500f);
		MULTI = new Range(-1f, 1f);
		SIZES = new Vector3[3]
		{
			Vector3.One,
			new Vector3(0.5f, 0.5f, 0.5f),
			new Vector3(0.3f, 0.3f, 0.3f)
		};
	}
}
