using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Eyehook.Framework;
using Loot.Dungeon;

namespace Loot.Encounters;

public static class EncounterRegistry
{
	private static TypeRegistry<Encounter> registry = new TypeRegistry<Encounter>();

	private static List<Type> db = new List<Type>();

	private static Type[] constructorParams = new Type[1] { typeof(Location) };

	private static object[] constructorParamValues = new object[1];

	public static List<Type> dbClone => new List<Type>(db);

	public static void Initialize()
	{
		Register<EncounterCards>("E0");
		Register<EncounterNecro>("E1");
		Register<EncounterSarcophagus>("E2");
		Register<EncounterInjuredGoblin>("E3");
		Register<EncounterNicePlace>("E4");
		Register<EncounterExcalibur>("E5");
		Register<EncounterIdol>("E6");
		Register<EncounterSpiderQueen>("E7");
		Register<EncounterHangedMan>("E8");
		Register<EncounterDrinkingContest>("E9");
		Register<EncounterGiants>("E10");
		Register<EncounterPuzzleChest>("E11");
		Register<EncounterWitchesBrew>("E12");
		Register<EncounterGoldenHarp>("E13");
		Register<EncounterSecretStash>("E14");
		Register<EncounterCollapsingFloor>("E15");
		Register<EncounterHotSprings>("E16");
		Register<EncounterOrcChoir>("E17");
		Register<EncounterCrazyGoblin>("E18");
		Register<EncounterAnvil>("E19");
		Register<EncounterDuel>("E20");
		Register<EncounterPrincess>("E21");
	}

	private static void Register<T>(string id) where T : Encounter
	{
		registry.Register<T>(id);
		db.Add(typeof(T));
	}

	public static Encounter CreateEncounter(Type t, Location loc)
	{
		ConstructorInfo constructor = t.GetConstructor(constructorParams);
		constructorParamValues[0] = loc;
		return (Encounter)constructor.Invoke(constructorParamValues);
	}

	public static void WriteType(BinaryWriter writer, Type type)
	{
		registry.WriteType(writer, type);
	}

	public static Type ReadType(BinaryReader reader)
	{
		return registry.ReadType(reader);
	}

	public static void Save(BinaryWriter writer, Encounter encounter)
	{
		registry.Save(writer, encounter);
	}

	public static Encounter Load(BinaryReader reader)
	{
		return registry.Load(reader);
	}
}
