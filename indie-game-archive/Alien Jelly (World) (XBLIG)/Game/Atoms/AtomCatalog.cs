using System.Collections.Generic;
using GKEngine;
using GKEngine.Entities;
using Game.Interactable;
using Game.QBits;
using Game.Robots;

namespace Game.Atoms;

public static class AtomCatalog
{
	public const string SETS_DEFAULT = "Default";

	public const string SETS_PROPS = "Props";

	public const string SETS_LIVE = "Jelly & Robots";

	public const string SETS_SPECIAL = "Crates & Special";

	public const string SETS_SWITCHES = "Switches";

	public const string SETS_COLLECTS = "Collects";

	public static Dictionary<string, AtomDefinition> atoms;

	public static Dictionary<string, AtomShape> shapes;

	public static Dictionary<string, List<AtomDefinition>> sets;

	private static List<AtomDefinition> definitions;

	public static void Init()
	{
		Sets_Init();
		Atoms_Init();
		Shapes_Init();
	}

	public static void Add(AtomDefinition oDef)
	{
		atoms.Add(oDef.name, oDef);
		definitions.Add(oDef);
		Sets_Add(oDef);
	}

	public static AtomDefinition Next(int xDir, AtomDefinition oDef)
	{
		int num = definitions.IndexOf(oDef);
		num += xDir;
		num %= definitions.Count;
		num = ((num < 0) ? (definitions.Count - 1) : num);
		if (!definitions[num].isDevOnly)
		{
			return definitions[num];
		}
		return Next(xDir, definitions[num]);
	}

	private static void Sets_Init()
	{
		sets = new Dictionary<string, List<AtomDefinition>>();
		sets.Add("Default", new List<AtomDefinition>());
		sets.Add("Props", new List<AtomDefinition>());
		sets.Add("Jelly & Robots", new List<AtomDefinition>());
		sets.Add("Crates & Special", new List<AtomDefinition>());
		sets.Add("Switches", new List<AtomDefinition>());
		sets.Add("Collects", new List<AtomDefinition>());
	}

	private static void Sets_Add(AtomDefinition oDef)
	{
		for (int i = 0; i < oDef.sets.Length; i++)
		{
			if (sets.ContainsKey(oDef.sets[i]))
			{
				sets[oDef.sets[i]].Add(oDef);
			}
		}
	}

	public static void Atoms_Init()
	{
		atoms = new Dictionary<string, AtomDefinition>();
		definitions = new List<AtomDefinition>();
		Add(new AtomDefinition("Rock Block", "A simple building block made of rock", "Rock_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Rock_0_1x1x1:RimAmount=0.2:RimMix=0.5", "1x1x1 Rock", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("Rock with Grass", "A simple rock building block with grass", "RockGrass_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Rock_0_1x1x1:RimAmount=0.2:RimMix=0.5", "1x1x1 Rock Grass", xInstanced: true, GameMain.RENDERSTACK_ALPHA_HARD, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("Smooth Rock Block", "A simple building block made of rock", "Rock_1_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Rock_1_1x1x1:Ks=0.05:RimAmount=0.2:RimMix=0.5", "1x1x1 Old Rock", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("Soil & Rock Block", "A simple building block made of soil and rock", "Rock_2_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Rock_3_1x1x1:RimAmount=0.2:RimMix=0.5", "1x1x1 Rock Soil", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("Soil & Lawn Block", "A simple building block made of suburban soil and lawn", "Rock_3_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Rock_4_1x1x1:RimAmount=0.2:RimMix=0.5", "1x1x1 Rock Soil Grass", xInstanced: true, GameMain.RENDERSTACK_ALPHA_HARD, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("Snow Block", "A simple building block made of snow", "Snow_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/RockSnow_0_1x1x1:Bump=1:ShadowColor=0.65|0.85|1:Ks=0.05:RimAmount=0.5:RimMix=0.5", "1x1x1 Rock", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("Snow Grass", "A simple snow building block with frosty grass", "SnowGrass_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/RockSnow_0_1x1x1:Bump=1:ShadowColor=0.65|0.85|1:Ks=0.05:RimAmount=0.5:RimMix=0.7", "1x1x1 Rock Grass", xInstanced: true, GameMain.RENDERSTACK_ALPHA_HARD, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("Ice Block", "A Simple building block make from ice", "Ice_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Ice_0_1x1x1:ShadowColor=0.65|0.85|1:Ks=0.05:RimAmount=0.5:RimMix=0.7", "1x1x1 Ice", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("Sand Block", "A simple building block made from sand an Rock", "Sand_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Sand_0_1x1x1:RimAmount=0.2:RimMix=0.5", "1x1x1 Sand", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("Large Sand Block", "A large simple sand block", "Sand_0_1x2x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Sand_0_1x1x1:RimAmount=0.2:RimMix=0.5", "1x2x1 Sand", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("Broken Pillar", "A portion of broken pillar that can be used for building", "Pillar_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Pillar_0_1x1x1:RimAmount=0.2:RimMix=0.5", "1x1x1 Pillar", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("City Concrete", "A broken piece of reinforced city concrete", "Concrete_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Concrete_0_1x1x1:Ks=1:RimAmount=0.2:RimMix=0.5:Bump=2", "1x1x1 Concrete", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("City Building Cafe", "The bottom cafe portion of a skyscraper", "Building_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Building_A_1x1x1:Ks=1:RimAmount=0.2:RimMix=0.5", "1x1x1 Building 0", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: false));
		Add(new AtomDefinition("City Building Office", "The office portion of a skyscraper", "Building_1_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Building_A_1x1x1:Ks=1:RimAmount=0.2:RimMix=0.5", "1x1x1 Building 1", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: false));
		Add(new AtomDefinition("City Town House", "The bottom portion of a apartment building", "Building_2_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Building_B_1x1x1:Ks=1:RimAmount=0.2:RimMix=0.5", "1x1x1 Building 2", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: false));
		Add(new AtomDefinition("City Apartment", "The upper floors of a apartment building", "Building_3_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Building_B_1x1x1:Ks=1:RimAmount=0.2:RimMix=0.5", "1x1x1 Building 3", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: false));
		Add(new AtomDefinition("Alien Ground Block A", "An alien crystal landscape block", "Alien_0_1x1x1", "Atom_Instanced_Alien:Path=Materials/Atoms/Alien_0_1x1x1:SpecularColor=0|1|1", "1x1x1 Alien 0", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("Alien Ground Block B", "An alien crystal landscape block", "Alien_1_1x1x1", "Atom_Instanced_Alien:Path=Materials/Atoms/Alien_1_1x1x1:SpecularColor=1|1|0", "1x1x1 Alien 0", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		Add(new AtomDefinition("Alien Ground Block C", "An alien crystal landscape block", "Alien_2_1x1x1", "Atom_Instanced_Alien:Path=Materials/Atoms/Alien_2_1x1x1:SpecularColor=0.5|0|1", "1x1x1 Alien 0", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true));
		AtomDefinition atomDefinition = new AtomDefinition("Alien Ship Part A", "A part of the jelly ship", "Ship_0_1x1x1", "Atom_Instanced_DSNE:Path=Materials/Atoms/Ship_0_1x1x1:Bump=1:RimAmount=0.5:RimMix=0.7", "1x1x1 Ship 0", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true);
		atomDefinition.timed = true;
		Add(atomDefinition);
		atomDefinition = new AtomDefinition("Alien Ship Part B", "A part of the jelly ship", "Ship_1_1x1x1", "Atom_Instanced_DSNE:Path=Materials/Atoms/Ship_1_1x1x1:Bump=1:RimAmount=0.5:RimMix=0.7", "1x1x1 Ship 0", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true);
		atomDefinition.timed = true;
		Add(atomDefinition);
		atomDefinition = new AtomDefinition("Alien Ship Part A Large", "A large part of the jelly ship", "Ship_0_3x1x3", "Atom_Instanced_DSNE:Path=Materials/Atoms/Ship_0_3x1x3:Bump=1:RimAmount=0.5:RimMix=0.7", "3x1x3 Ship 0", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true);
		atomDefinition.timed = true;
		Add(atomDefinition);
		atomDefinition = new AtomDefinition("Alien Ship Part B Large", "A large part of the jelly ship", "Ship_1_3x1x3", "Atom_Instanced_DSNE:Path=Materials/Atoms/Ship_1_3x1x3:Bump=1:RimAmount=0.5:RimMix=0.7", "3x1x3 Ship 0", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: true);
		atomDefinition.timed = true;
		Add(atomDefinition);
		Add(new AtomDefinition("Jungle Tree", "Jungle tree with some flowers", "Tree_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Garden_0_1x1x1:Bump=4:Ks=1", "1x1x1 Tree 0", xInstanced: true, GameMain.RENDERSTACK_ALPHA_HARD, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Props" }, xAutoRotate: true));
		Add(new AtomDefinition("Tree Roots", "Jungle Tree Roots with some flowers", "Roots_0_1x1x1", "Atom_Single_DSN:Path=Materials/Atoms/Garden_0_1x1x1", "1x1x1 Roots 0", xInstanced: false, GameMain.RENDERSTACK_ALPHA_HARD, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: true));
		Add(new AtomDefinition("Flower Patch", "A patch of flowers", "Flowers_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Garden_0_1x1x1", "1x1x1 Flowers 0", xInstanced: true, GameMain.RENDERSTACK_ALPHA_HARD, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: true));
		Add(new AtomDefinition("Mushroom Patch", "A small garden mushroom patch", "Mushrooms_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Mushrooms_0_1x1x1", "1x1x1 Mushrooms", xInstanced: true, GameMain.RENDERSTACK_ALPHA_HARD, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: true));
		Add(new AtomDefinition("Fence", "A fence with a warning sign", "Fence_0_1x2x1", "Atom_Single_DSN:Path=Materials/Atoms/Fence_0_1x2x1", "1x2x1 Fence", xInstanced: false, GameMain.RENDERSTACK_ALPHA_HARD, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Props" }, xAutoRotate: false));
		Add(new AtomDefinition("Fridge", "A refrigerator... for some reson", "Fridge_0_1x2x1", "Atom_Single_DSN:Path=Materials/Atoms/Fridge_0_1x2x1:Bump=2:Ks=1:ShadowColor=0.65|0.85|1", "1x2x1 Fridge", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Props" }, xAutoRotate: false));
		Add(new AtomDefinition("Snowman", "A snowman unlike any you have ever seen before", "Snowman_0_1x1x1", "Atom_Single_DSN:Path=Materials/Atoms/Snowman_0_1x1x1:Bump=2:ShadowColor=0.65|0.85|1", "1x1x1 Snowman", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: true, new string[1] { "Props" }, xAutoRotate: false));
		Add(new AtomDefinition("Roof Top A", "A collection of roof top props for a city building.", "Roof_0_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Roof_0_1x1x1", "1x1x1 Roof 0", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: true));
		Add(new AtomDefinition("Roof Top B", "A collection of roof top props for a city building.", "Roof_1_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Roof_0_1x1x1", "1x1x1 Roof 1", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: true));
		Add(new AtomDefinition("Roof Top C", "A collection of roof top props for a city building.", "Roof_2_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Roof_0_1x1x1", "1x1x1 Roof 2", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: true));
		Add(new AtomDefinition("Roof Top D", "A collection of roof top props for a city building.", "Roof_3_1x1x1", "Atom_Instanced_DSN:Path=Materials/Atoms/Roof_0_1x1x1", "1x1x1 Roof 3", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: true));
		Add(new AtomDefinition("Road", "A tar road", "Road_0_1x1x1", "Atom_Single_DSN:Path=Materials/Atoms/Road_0_1x1x1", "1x1x1 Road 0", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: false));
		Add(new AtomDefinition("Cross Road", "A city cross road.", "Road_1_1x1x1", "Atom_Single_DSN:Path=Materials/Atoms/Road_0_1x1x1", "1x1x1 Road 1", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: false));
		Add(new AtomDefinition("Road With Lamp", "A tar road with a street lamp", "Road_2_1x1x1", "Atom_Single_DSN:Path=Materials/Atoms/Road_0_1x1x1", "1x1x1 Road 2", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: false));
		Add(new AtomDefinition("Alien Plant A", "A crazy alien plant", "Alien_Plant_0_1x1x1", "Atom_Single_DSN:Path=Materials/Atoms/Alien_Plant_0_1x1x1:Ks=1:SpecExpon=75:Bump=4", "1x1x1 Alien Plant 0", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: true));
		Add(new AtomDefinition("Alien Plant B", "A crazy alien plant", "Alien_Plant_1_1x1x1", "Atom_Single_DSN:Path=Materials/Atoms/Alien_Plant_0_1x1x1:Ks=1:SpecExpon=75:Bump=4", "1x1x1 Alien Plant 1", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: true));
		Add(new AtomDefinition("Alien Plant C", "A crazy alien plant", "Alien_Plant_2_1x1x1", "Atom_Single_DSN:Path=Materials/Atoms/Alien_Plant_0_1x1x1:Ks=1:SpecExpon=75:Bump=4", "1x1x1 Alien Plant 2", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Normal, 1u, xPlayGrid: false, new string[1] { "Props" }, xAutoRotate: true));
		Add(new AtomSignDefinition("Left Arrow Sign", "A holographic arrow sign to help players.", "Sign_Arrow_0", "Atom_Single_Sign:Path=Materials/Atoms/Sign_Arrow"));
		Add(new AtomSignDefinition("Right Arrow Sign", "A holographic arrow sign to help players.", "Sign_Arrow_3", "Atom_Single_Sign:Path=Materials/Atoms/Sign_Arrow_Right"));
		Add(new AtomSignDefinition("Up Arrow Sign", "A holographic arrow sign to help players.", "Sign_Arrow_4", "Atom_Single_Sign:Path=Materials/Atoms/Sign_Arrow_Up"));
		Add(new AtomSignDefinition("Down Arrow Sign", "A holographic arrow sign to help players.", "Sign_Arrow_5", "Atom_Single_Sign:Path=Materials/Atoms/Sign_Arrow_Down"));
		Add(new AtomSignDefinition("Rewind Sign", "A holographic rewind sign to help players.", "Sign_Arrow_1", "Atom_Single_Sign:Path=Materials/Atoms/Sign_Rewind"));
		Add(new AtomSignDefinition("Phase Out Sign", "A holographic sign to tell players to phase out.", "Sign_Arrow_2", "Atom_Single_Sign:Path=Materials/Atoms/Sign_PhaseOut"));
		Add(new AtomSignDefinition("Jelly Sign", "A holographic sign to tell players to use a particular jelly.", "Sign_QBit_0", "Atom_Single_Sign:Path=Materials/Atoms/Sign_QBit"));
		Add(new AtomSignDefinition("Crate Push Sign", "A holographic sign to tell players to push a crate.", "Sign_QBit_1", "Atom_Single_Sign:Path=Materials/Atoms/Sign_CratePush"));
		Add(new AtomSignDefinition("Crate Ferry Sign", "A holographic sign to tell players to ferry a crate.", "Sign_QBit_2", "Atom_Single_Sign:Path=Materials/Atoms/Sign_CrateFerry"));
		Add(new AtomSignDefinition("Stack QBits Sign", "A holographic sign to tell players to stack jellies.", "Sign_QBit_3", "Atom_Single_Sign:Path=Materials/Atoms/Sign_Stack"));
		Add(new AtomQBitDefinition("Red Jelly", "Red alien jelly", "QBit_Red", "Atom_Single_Color:Path=:Color=1|0|0|1", QBit.QBitType.Red));
		Add(new AtomQBitDefinition("Green Jelly", "Green alien jelly", "QBit_Green", "Atom_Single_Color:Path=:Color=0|1|0|1", QBit.QBitType.Green));
		Add(new AtomQBitDefinition("Blue Jelly", "Blue alien jelly", "QBit_Blue", "Atom_Single_Color:Path=:Color=0|0|1|1", QBit.QBitType.Blue));
		Add(new AtomQBitDefinition("Yellow Jelly", "Yellow alien jelly", "QBit_Yellow", "Atom_Single_Color:Path=:Color=1|1|0|1", QBit.QBitType.Yellow));
		atomDefinition = new AtomDefinition(Robot.TITLE, Robot.DESCRIPTION, "QByte_Default", "Atom_Single_DSN:Path=Materials/Robots/Default", "Robot", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Robot, 3u, xPlayGrid: false, new string[1] { "Jelly & Robots" }, xAutoRotate: false);
		atomDefinition.propertiesDesc = Robot.PROPERTIES_DESCRIPTION;
		atomDefinition.properties = Robot.PROPERTIES;
		atomDefinition.propertiesDefault = Robot.PROPERTIES_DEFAULT;
		Add(atomDefinition);
		atomDefinition = new AtomDefinition(AtomMarker.TITLE, AtomMarker.DESCRIPTION, "Marker", "Atom_Single_Color:Path=", "1x1x1 Marker", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Marker, 0u, xPlayGrid: false, new string[1] { "Crates & Special" }, xAutoRotate: false);
		atomDefinition.propertiesDesc = AtomMarker.PROPERTIES_DESCRIPTION;
		atomDefinition.properties = AtomMarker.PROPERTIES;
		atomDefinition.propertiesDefault = AtomMarker.PROPERTIES_DEFAULT;
		Add(atomDefinition);
		atomDefinition = new AtomDefinition(AtomPortal.TITLE, AtomPortal.DESCRIPTION, "Portal", "", "1x2x1 Portal", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Portal, 2u, xPlayGrid: false, new string[1] { "Crates & Special" }, xAutoRotate: false);
		atomDefinition.propertiesDesc = AtomPortal.PROPERTIES_DESCRIPTION;
		atomDefinition.properties = AtomPortal.PROPERTIES;
		atomDefinition.propertiesDefault = AtomPortal.PROPERTIES_DEFAULT;
		Add(atomDefinition);
		atomDefinition = new AtomDefinition("Spikey Block", "This block will destroy any Alien Jelly that touches it!", "Spikes", "Atom_Instanced_DSN:Path=Materials/Spikes:Ks=1:SpecExpon=88:Bump=4:RimAmount=0.3:RimMix=0.5", "1x1x1 Spikes", xInstanced: true, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Pain, 1u, xPlayGrid: true, new string[1] { "Crates & Special" }, xAutoRotate: true);
		Add(atomDefinition);
		atomDefinition = new AtomDefinition(MovingSpikes.TITLE, MovingSpikes.DESCRIPTION, "MovingSpikes", "Atom_Single_DSN:Path=Materials/Spikes:Ks=1:SpecExpon=88:Bump=4:RimAmount=0.3:RimMix=0.5", "1x1x1 Spikes Moving", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Platform, 3u, xPlayGrid: false, new string[1] { "Crates & Special" }, xAutoRotate: false);
		atomDefinition.propertiesDesc = MovingSpikes.PROPERTIES_DESCRIPTION;
		atomDefinition.properties = MovingSpikes.PROPERTIES;
		atomDefinition.propertiesDefault = MovingSpikes.PROPERTIES_DEFAULT;
		Add(atomDefinition);
		atomDefinition = new AtomDefinition("Crate", "", "Crate", "Atom_Single_DSN:Path=Materials/Crate", "1x1x1 Crate", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Crate, 1u, xPlayGrid: false, new string[1] { "Crates & Special" }, xAutoRotate: false);
		atomDefinition.camCull = false;
		Add(atomDefinition);
		atomDefinition = new AtomDefinition(AtomExit.TITLE, AtomExit.DESCRIPTION, "Exit", "Atom_Single_DSN:Path=Materials/Atoms/Exit", "Exit", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Exit, 2u, xPlayGrid: false, new string[1] { "Crates & Special" }, xAutoRotate: false);
		atomDefinition.propertiesDesc = AtomExit.PROPERTIES_DESCRIPTION;
		atomDefinition.properties = AtomExit.PROPERTIES;
		atomDefinition.propertiesDefault = AtomExit.PROPERTIES_DEFAULT;
		Add(atomDefinition);
		atomDefinition = new AtomDefinition(AtomDissapearing.TITLE, AtomDissapearing.DESCRIPTION, "Dissapearing_0_1x1x1", "Atom_Single_DSN:Path=Materials/Atoms/Rock_0_1x1x1:Ks=0.05", "1x1x1 Rock", xInstanced: false, GameMain.RENDERSTACK_SOLID, AtomDefinition.Type.Dissapearing, 2u, xPlayGrid: true, new string[1] { "Crates & Special" }, xAutoRotate: true);
		atomDefinition.propertiesDesc = AtomDissapearing.PROPERTIES_DESCRIPTION;
		atomDefinition.properties = AtomDissapearing.PROPERTIES;
		atomDefinition.propertiesDefault = AtomDissapearing.PROPERTIES_DEFAULT;
		Add(atomDefinition);
		Add(new AtomFilterDefinition("Red Jelly Filter", "Filter_Red_1x1", "", "1x1x1 Filter", QBit.QBitType.Red));
		Add(new AtomFilterDefinition("Green Jelly Filter", "Filter_Green_1x1", "", "1x1x1 Filter", QBit.QBitType.Green));
		Add(new AtomFilterDefinition("Blue Jelly Filter", "Filter_Blue_1x1", "", "1x1x1 Filter", QBit.QBitType.Blue));
		Add(new AtomFilterDefinition("Yellow Jelly Filter", "Filter_Yellow_1x1", "", "1x1x1 Filter", QBit.QBitType.Yellow));
		Add(new AtomSwitchDefinition("Red 90 Flip Button", "This button flips the world 90 degrees when a red Jelly steps on it. This item has properties.", "Switch_Red_90", "Atom_Single_DSN:Path=Materials/Atoms/Buttons", "Button", QBit.QBitType.Red, AtomSwitch.Types.Flip, 1));
		Add(new AtomSwitchDefinition("Green 90 Flip Button", "This button flips the world 90 degrees when a green Jelly steps on it. This item has properties.", "Switch_Green_90", "Atom_Single_DSN:Path=Materials/Atoms/Buttons", "Button", QBit.QBitType.Green, AtomSwitch.Types.Flip, 1));
		Add(new AtomSwitchDefinition("Blue 90 Flip Button", "This button flips the world 90 degrees when a blue Jelly steps on it. This item has properties.", "Switch_Blue_90", "Atom_Single_DSN:Path=Materials/Atoms/Buttons", "Button", QBit.QBitType.Blue, AtomSwitch.Types.Flip, 1));
		Add(new AtomSwitchDefinition("Yellow 90 Flip Button", "This button flips the world 90 degrees when a yellow Jelly steps on it. This item has properties.", "Switch_Yellow_90", "Atom_Single_DSN:Path=Materials/Atoms/Buttons", "Button", QBit.QBitType.Yellow, AtomSwitch.Types.Flip, 1));
		Add(new AtomSwitchDefinition("Red 180 Flip Button", "This button flips the world upside-down degrees when a red Jelly steps on it. This item has properties.", "Switch_Red_180", "Atom_Single_DSN:Path=Materials/Atoms/Buttons", "Button", QBit.QBitType.Red, AtomSwitch.Types.Flip, 2));
		Add(new AtomSwitchDefinition("Green 180 Flip Button", "This button flips the world upside-down degrees when a green Jelly steps on it. This item has properties.", "Switch_Green_180", "Atom_Single_DSN:Path=Materials/Atoms/Buttons", "Button", QBit.QBitType.Green, AtomSwitch.Types.Flip, 2));
		Add(new AtomSwitchDefinition("Blue 180 Flip Button", "This button flips the world upside-down degrees when a blue Jelly steps on it. This item has properties.", "Switch_Blue_180", "Atom_Single_DSN:Path=Materials/Atoms/Buttons", "Button", QBit.QBitType.Blue, AtomSwitch.Types.Flip, 2));
		Add(new AtomSwitchDefinition("Yellow 180 Flip Button", "This button flips the world upside-down degrees when a yellow Jelly steps on it. This item has properties.", "Switch_Yellow_180", "Atom_Single_DSN:Path=Materials/Atoms/Buttons", "Button", QBit.QBitType.Yellow, AtomSwitch.Types.Flip, 2));
		atomDefinition = new AtomDefinition("Hologram Block", "A Holographic block that's turned on and off by using the hologram buttons.", "Hologram_1x1x1", "Atom_Instanced_Hologram:Path=", "1x1x1", xInstanced: true, GameMain.RENDERSTACK_ADD, AtomDefinition.Type.Hologram, 1u, xPlayGrid: true, new string[1] { "Default" }, xAutoRotate: false);
		atomDefinition.propertiesDesc = AtomInstancedHologram.PROPERTIES_DESCRIPTION;
		atomDefinition.properties = AtomInstancedHologram.PROPERTIES;
		atomDefinition.propertiesDefault = AtomInstancedHologram.PROPERTIES_DEFAULT;
		Add(atomDefinition);
		Add(new AtomSwitchDefinition("Red Hologram Button", "A Button that turns the hologram blocks on or off when activated by a red Alien Jelly. ", "Switch_Red_Hologram", "Atom_Single_DSN:Path=Materials/Atoms/Buttons", "Button", QBit.QBitType.Red, AtomSwitch.Types.Holograms, 3));
		Add(new AtomSwitchDefinition("Green Hologram Button", "A Button that turns the hologram blocks on or off when activated by a green Alien Jelly. ", "Switch_Green_Hologram", "Atom_Single_DSN:Path=Materials/Atoms/Buttons", "Button", QBit.QBitType.Green, AtomSwitch.Types.Holograms, 3));
		Add(new AtomSwitchDefinition("Blue Hologram Button", "A Button that turns the hologram blocks on or off when activated by a blue Alien Jelly. ", "Switch_Blue_Hologram", "Atom_Single_DSN:Path=Materials/Atoms/Buttons", "Button", QBit.QBitType.Blue, AtomSwitch.Types.Holograms, 3));
		Add(new AtomSwitchDefinition("Yellow Hologram Button", "A Button that turns the hologram blocks on or off when activated by a yellow Alien Jelly. ", "Switch_Yellow_Hologram", "Atom_Single_DSN:Path=Materials/Atoms/Buttons", "Button", QBit.QBitType.Yellow, AtomSwitch.Types.Holograms, 3));
		Add(new AtomCollectDefinition("Gold", "Gold score collect worth 100 score", "CollectScore_0", "", "1x1x1 Collect 0", xInstanced: false, GameMain.RENDERSTACK_SOLID, 2u, 100));
		Add(new AtomCollectDefinition("Emerald", "Emerald score collect worth 250 score", "CollectScore_1", "", "1x1x1 Collect 1", xInstanced: false, GameMain.RENDERSTACK_SOLID, 2u, 250));
		Add(new AtomCollectDefinition("Ruby", "Ruby score collect worth 1000 score", "CollectScore_2", "", "1x1x1 Collect 2", xInstanced: false, GameMain.RENDERSTACK_SOLID, 2u, 1000));
		AtomCollectDefinition atomCollectDefinition = new AtomCollectDefinition("Shiny", "Score collect worth 10 score", "CollectScore_3", "Atom_Instanced_Sequence:Path=Materials/Atoms/Collect_Score_3_1x1x1", "1x1x0", xInstanced: true, GameMain.RENDERSTACK_ADD_FIRST, 1u, 10);
		atomCollectDefinition.timed = true;
		Add(atomCollectDefinition);
		AtomDefinition atomDefinition2 = new AtomDefinition(AtomInfo.TITLE, AtomInfo.DESCRIPTION, "Info_1x1x1", "Atom_Single_Hologram:Path=:Color=0.4218|0.3867|0.6679|1:BaseColor=0.0859|0.0039|0.0703|1", "1x1x1 Info", xInstanced: false, GameMain.RENDERSTACK_ADD, AtomDefinition.Type.Info, 1u, xPlayGrid: false, new string[1] { "Collects" }, xAutoRotate: false);
		atomDefinition2.propertiesDesc = AtomInfo.PROPERTIES_DESCRIPTION;
		atomDefinition2.properties = AtomInfo.PROPERTIES;
		atomDefinition2.propertiesDefault = AtomInfo.PROPERTIES_DEFAULT;
		atomDefinition2.isDevOnly = true;
		Add(atomDefinition2);
	}

	public static void Shapes_Init()
	{
		shapes = new Dictionary<string, AtomShape>();
		shapes.Add("1x1x1", new AtomShape(GameEngine.Content.Load<MaxModel>("Content/Models/Atoms/1x1x1/Model")));
		foreach (KeyValuePair<string, AtomDefinition> atom in atoms)
		{
			if (!shapes.ContainsKey(atom.Value.shape))
			{
				AtomShape value = new AtomShape(GameEngine.Content.Load<MaxModel>("Content/Models/Atoms/" + atom.Value.shape + "/Model"));
				shapes.Add(atom.Value.shape, value);
			}
		}
	}
}
