using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public class SceneryObject
{
	public float Z = -1f;

	public int ID = -1;

	public string name = "";

	public string uniqueName = "";

	public bool isVisible = true;

	public int xRoll;

	public int yRoll;

	public int xRollFighters;

	public int yRollFighters;

	public float partialXRoll;

	public float partialYRoll;

	public int DPS;

	public bool isDPSFeetOnly = true;

	public Rectangle rect;

	public NonFighterAI.modes AImode;

	public int AIAmountSpeed;

	public int AIAmountDistance;

	public string AIMemory = "";

	public int AICounter;

	public Vector2 circlePivotPoint = Vector2.Zero;

	public Vector2 circleVelocity = Vector2.Zero;

	public Vector2 circleProgress = Vector2.Zero;

	private float xPartial;

	private float yPartial;

	public bool isFlippedHorizontally;

	public bool isFlippedVertically;

	public float X
	{
		get
		{
			return rect.X;
		}
		set
		{
			rect.X = (int)(value + xPartial);
			xPartial = value - (float)rect.X;
		}
	}

	public float Y
	{
		get
		{
			return rect.Y;
		}
		set
		{
			rect.Y = (int)(value + yPartial);
			yPartial = value - (float)rect.Y;
		}
	}

	public int width
	{
		get
		{
			return rect.Width;
		}
		set
		{
			rect.Width = value;
		}
	}

	public int height
	{
		get
		{
			return rect.Height;
		}
		set
		{
			rect.Height = value;
		}
	}

	public float getLayerDepth()
	{
		if (Z != -1f)
		{
			return Z;
		}
		return RandomStaticGlobals.getLayerDepth((int)Y, height) + (float)ID * 1E-05f;
	}

	public SceneryObject(int ID)
	{
		ID = ID;
	}

	public SceneryObject(int ID, string Name, int x, int y, int width, int height, bool visible)
	{
		ID = ID;
		name = Name;
		rect = new Rectangle(x, y, width, height);
		isVisible = visible;
	}

	public SceneryObject(int ID, string folder, string Name, int x, int y)
	{
		ID = ID;
		name = Name;
		Texture2D texture2D = GraphicsManager.LoadTexture(folder + Name, cacheResult: true);
		rect = new Rectangle(x, y, texture2D.Width, texture2D.Height);
		isVisible = true;
	}

	public SceneryObject Copy()
	{
		return (SceneryObject)MemberwiseClone();
	}
}
