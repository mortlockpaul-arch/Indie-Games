using System.Collections.Generic;

namespace MaxScriptDefines;

public class EGE_3dsMaxModifier
{
	public delegate void ContextLoggerCallbackType(string msg);

	public ContextLoggerCallbackType ContextLogger;

	private static List<PredefinedObjectStruct> PredefinedObjects = new List<PredefinedObjectStruct>();

	public string ModifierName = "EndGameEngine";

	public int ModifierVersion = 5;

	public string ObjectType = "Type";

	public string MaterialType = "Material";

	public string OpacityType = "Opacity";

	public string CullingType = "Culling";

	public string CollisionType = "CollisionTest";

	public EGE_3dsMaxModifier()
	{
		ReadDefinitions();
	}

	public EGE_3dsMaxModifier(ContextLoggerCallbackType e)
	{
		ContextLogger = e;
		ReadDefinitions();
	}

	private void ReadDefinitions()
	{
	}

	public string[] RemoveCommits(string[] src)
	{
		int num = src.Length;
		string[] array = new string[num];
		int i = 0;
		int num2 = 0;
		for (; i < num; i++)
		{
			int num3 = src[i].IndexOf("//");
			if (num3 == -1 && src[i].Length > 0)
			{
				array[num2++] = src[i];
			}
			else if (num3 > 0)
			{
				array[num2++] = src[i].Substring(0, num3);
			}
		}
		return array;
	}

	public PredefinedObjectStruct GetPredefined(int e)
	{
		PredefinedObjectStruct result = PredefinedObjects[0];
		foreach (PredefinedObjectStruct predefinedObject in PredefinedObjects)
		{
			if (predefinedObject.typeIndex == e)
			{
				result = predefinedObject;
				result.name = GetObjectTypesStrArray()[predefinedObject.typeIndex];
				break;
			}
		}
		return result;
	}

	public string SelectedAsString(int e)
	{
		return string.Concat((EnumObjectTypes)e);
	}

	public bool isPredefinedType(string e)
	{
		return isPredefinedType(GetPredefinedIndex(e));
	}

	public bool isPredefinedType(int e)
	{
		foreach (PredefinedObjectStruct predefinedObject in PredefinedObjects)
		{
			if (predefinedObject.typeIndex == e)
			{
				return true;
			}
		}
		return false;
	}

	public int GetPredefinedIndex(string e)
	{
		foreach (PredefinedObjectStruct predefinedObject in PredefinedObjects)
		{
			if (predefinedObject.name.Length == e.Length && predefinedObject.name.Contains(e))
			{
				return predefinedObject.typeIndex;
			}
		}
		return -1;
	}

	public string[] GetObjectTypesStrArray()
	{
		string[] array = new string[12];
		for (int i = 0; i < 12; i++)
		{
			array[i] = ((EnumObjectTypes)i).ToString();
		}
		return array;
	}

	public string[] GetMaterialTypesStrArray()
	{
		string[] array = new string[14];
		for (int i = 0; i < 14; i++)
		{
			array[i] = ((EnumMaterialTypes)i).ToString();
		}
		return array;
	}

	public string[] GetOpacityTypesStrArray()
	{
		string[] array = new string[3];
		for (int i = 0; i < 3; i++)
		{
			array[i] = ((EnumOpacityTypes)i).ToString();
		}
		return array;
	}

	public string[] GetCullingTypesStrArray()
	{
		string[] array = new string[3];
		for (int i = 0; i < 3; i++)
		{
			array[i] = ((EnumCullingTypes)i).ToString();
		}
		return array;
	}

	public string[] GetCollisionTypesStrArray()
	{
		string[] array = new string[4];
		for (int i = 0; i < 4; i++)
		{
			array[i] = ((EnumCollisionTypes)i).ToString();
		}
		return array;
	}

	public string GetOmniLightAsString()
	{
		return EnumObjectTypes.OmniLight.ToString();
	}
}
