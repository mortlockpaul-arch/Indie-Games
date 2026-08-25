using System;
using BunnyOfWar.AI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BunnyOfWar;

public class ObstacleObject
{
	public int ID = -1;

	public Rectangle rect;

	public OnDestructionCallback onDestruction;

	public Texture2D image;

	public string name = "";

	public string uniqueName = "";

	public int hp = 10;

	public bool isActive = true;

	public bool isDestructible = true;

	public bool isOnGround = true;

	public int xRoll;

	public int yRoll;

	public float partialXRoll;

	public float partialYRoll;

	public int pixelsInTheAir;

	public bool isFalling;

	public int fallSpeedPerFrame = Definitions.ObstaclePixelsToFallPerFrame;

	public int fallDamageAfterLanding = Definitions.ObstacleFallDamageAfterLanding;

	public int DPS;

	public bool isPickupable = true;

	public bool isInPickupableRange;

	public bool isReallyScenery;

	public bool isFlippedVertically;

	public bool isFlippedHorizontally;

	public FighterObject isBeingCarriedBy;

	public DateTime dtLastTimeInPickupableRange = DateTime.MinValue;

	public Vector2 circlePivotPoint = Vector2.Zero;

	public Vector2 circleVelocity = Vector2.Zero;

	public Vector2 circleProgress = Vector2.Zero;

	public NonFighterAI.modes AImode;

	public int AIAmountSpeed;

	public int AIAmountDistance;

	public string AIMemory = "";

	private float xPartial;

	private float yPartial;

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
		float layerDepth = RandomStaticGlobals.getLayerDepth((int)Y, height);
		if (layerDepth > Definitions.LayerDepthFourthHighest)
		{
			return Definitions.LayerDepthFourthHighest;
		}
		return layerDepth;
	}

	public ObstacleObject()
	{
	}

	public ObstacleObject(Rectangle Rect, OnDestructionCallback callback, Texture2D Image, int HP, bool IsActive, bool IsDestructable, bool IsOnGround)
	{
		rect = Rect;
		onDestruction = callback;
		image = Image;
		hp = HP;
		isActive = IsActive;
		isDestructible = IsDestructable;
		isOnGround = IsOnGround;
	}

	public ObstacleObject(Rectangle Rect, Texture2D Image, int HP)
	{
		rect = Rect;
		onDestruction = null;
		image = Image;
		hp = HP;
		isActive = true;
		isDestructible = true;
		isOnGround = true;
	}

	public ObstacleObject(Rectangle Rect, Texture2D Image)
	{
		rect = Rect;
		onDestruction = null;
		image = Image;
		hp = 1;
		isActive = true;
		isDestructible = false;
		isOnGround = true;
	}

	public void takeDamage(int amount, bool broadcast)
	{
		if (isDestructible && isActive)
		{
			hp -= amount;
			if (hp < 0)
			{
				isActive = false;
				TriggerManager.SetTriggerEvent(name + "Destroyed");
				TriggerManager.SetTriggerEvent(uniqueName + "Destroyed");
				AwardmentsManager.CheckForAwardments(name + "Destroyed");
				AwardmentsManager.CheckForAwardments(uniqueName + "Destroyed");
			}
			if (broadcast)
			{
				NetworkGameplayManager.SendObjectDamage(ID, amount);
			}
		}
	}

	public ObstacleObject Copy()
	{
		return (ObstacleObject)MemberwiseClone();
	}
}
