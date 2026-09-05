using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Renderer;

namespace PlayObjects;

public class PropGeneratorVirtual
{
	public static void CreateRect(PhysicsOutfit outfit, float width, float height, bool glowing)
	{
		List<List<List<Vector2>>> list = new List<List<List<Vector2>>>();
		List<List<SpriteInstance>> list2 = new List<List<SpriteInstance>>();
		List<int> connections = new List<int>();
		List<Vector2> jointPos = new List<Vector2>();
		List<PhysicsJointType> jointTypes = new List<PhysicsJointType>();
		List<int> list3 = new List<int>();
		List<MassTypes> list4 = new List<MassTypes>();
		list2.Add(new List<SpriteInstance>());
		list.Add(new List<List<Vector2>>());
		list2.Last().Add(TextureContainer.GetSprite("images/Spritesheets/boxes", new Rectangle(0, 0, 128, 256), default(Vector2), 0.01f));
		list2.Last().Last().SurfaceScale = new Vector2(width, height);
		list2.Last().Last().Origin = new Vector2(0f, height / 2f);
		list4.Add(MassTypes.FLESH_MASS);
		list.Last().Add(new List<Vector2>());
		list.Last().Last().Add(new Vector2(width / 2f, 0f - height));
		list.Last().Last().Add(new Vector2((0f - width) / 2f, 0f - height));
		list.Last().Last().Add(new Vector2((0f - width) / 2f, 0f));
		list.Last().Last().Add(new Vector2(width / 2f, 0f));
		list3.Add(0);
		list2.Add(new List<SpriteInstance>());
		if (!glowing)
		{
			list2.Last().Add(null);
		}
		else
		{
			list2.Last().Add(TextureContainer.GetSprite("images/Spritesheets/boxes", new Rectangle(128, 0, 128, 256), default(Vector2), -0.99f));
			list2.Last().Last().SurfaceScale = new Vector2(width + 80f, height + 80f);
			list2.Last().Last().Origin = new Vector2(0f, height / 2f);
			list2.Last().Last().FlatColor = true;
			list2.Last().Last().Additive = true;
		}
		outfit.Initialize(list, list2[0], list2[1], connections, jointPos, jointTypes, list4, list3, 1f);
		outfit.SetSelfGlow();
	}

	public static void CreateBiggestNorm(PhysicsOutfit outfit)
	{
		CreateRect(outfit, 2f * SceneRenderer.GetRand(50f, 240f), 2f * SceneRenderer.GetRand(190f, 260f), glowing: false);
	}

	public static void CreateBigNorm(PhysicsOutfit outfit)
	{
		CreateRect(outfit, 2f * SceneRenderer.GetRand(30f, 170f), 2f * SceneRenderer.GetRand(140f, 200f), glowing: false);
	}

	public static void CreateMedNorm(PhysicsOutfit outfit)
	{
		CreateRect(outfit, 2f * SceneRenderer.GetRand(30f, 130f), 2f * SceneRenderer.GetRand(100f, 140f), glowing: false);
	}

	public static void CreateSmallNorm(PhysicsOutfit outfit)
	{
		CreateRect(outfit, 2f * SceneRenderer.GetRand(30f, 100f), 2f * SceneRenderer.GetRand(60f, 100f), glowing: false);
	}

	public static void CreateBigGlow(PhysicsOutfit outfit)
	{
		CreateRect(outfit, 2f * SceneRenderer.GetRand(60f, 200f), 2f * SceneRenderer.GetRand(140f, 250f), glowing: true);
	}

	public static void CreateMedGlow(PhysicsOutfit outfit)
	{
		CreateRect(outfit, 2f * SceneRenderer.GetRand(35f, 140f), 2f * SceneRenderer.GetRand(100f, 200f), glowing: true);
	}

	public static void CreateSmallGlow(PhysicsOutfit outfit)
	{
		CreateRect(outfit, 2f * SceneRenderer.GetRand(20f, 70f), 2f * SceneRenderer.GetRand(40f, 70f), glowing: true);
	}

	public static void CreateSmallestGlow(PhysicsOutfit outfit)
	{
		CreateRect(outfit, 2f * SceneRenderer.GetRand(20f, 50f), 2f * SceneRenderer.GetRand(20f, 50f), glowing: true);
	}
}
