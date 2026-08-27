using System.Collections.Generic;

namespace MaxScriptDefines;

public class EGE_3dsMaxEmitter
{
	public delegate void ContextLoggerCallbackType(string msg);

	public ContextLoggerCallbackType ContextLogger;

	private static List<PredefinedEmitterStruct> PredefinedObjects = new List<PredefinedEmitterStruct>();

	public string EmitterName = "EGE_Emitter";

	public int EmitterVersion = 5;

	public string EmitterType = "Type";

	public string EmitterFlicker = "Flicker";

	public string EmitterScale = "Scale";

	public string EmitterColor = "Color";

	public EGE_3dsMaxEmitter()
	{
		ReadDefinitions();
	}

	public EGE_3dsMaxEmitter(ContextLoggerCallbackType e)
	{
		ContextLogger = e;
		ReadDefinitions();
	}

	private void ReadDefinitions()
	{
	}

	public string SelectedAsString(int e)
	{
		return string.Concat((EnumEmitterTypes)e);
	}

	public string[] GetEmitterTypesStrArray()
	{
		string[] array = new string[4];
		for (int i = 0; i < 4; i++)
		{
			array[i] = ((EnumEmitterTypes)i).ToString();
		}
		return array;
	}
}
