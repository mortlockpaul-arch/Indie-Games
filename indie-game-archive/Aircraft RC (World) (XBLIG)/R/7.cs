using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Lights;

namespace R;

internal class _7
{
	internal static Dictionary<string, ContentRepository.ModelData> _7_0006(SerializationInfo P_0)
	{
		Dictionary<string, ContentRepository.ModelData> dictionary = new Dictionary<string, ContentRepository.ModelData>();
		try
		{
			dictionary = (Dictionary<string, ContentRepository.ModelData>)P_0.GetValue("Models", typeof(Dictionary<string, ContentRepository.ModelData>));
		}
		catch
		{
		}
		try
		{
			List<string> list = (List<string>)P_0.GetValue("Models", typeof(List<string>));
			foreach (string item in list)
			{
				dictionary.Add(item, new ContentRepository.ModelData());
			}
		}
		catch
		{
		}
		return dictionary;
	}

	internal static UpdateType _7o(SerializationInfo P_0)
	{
		try
		{
			string value = (string)P_0.GetValue("UpdateType", typeof(string));
			return (UpdateType)Enum.Parse(typeof(UpdateType), value, ignoreCase: false);
		}
		catch
		{
		}
		try
		{
			string text = (string)P_0.GetValue("ObjectType", typeof(string));
			if (text == "Static")
			{
				return UpdateType.None;
			}
			if (text == "Dynamic")
			{
				return UpdateType.Automatic;
			}
		}
		catch
		{
		}
		return UpdateType.None;
	}

	internal static StaticLightingType _7e(SerializationInfo P_0)
	{
		try
		{
			string value = (string)P_0.GetValue("StaticLightingType", typeof(string));
			return (StaticLightingType)Enum.Parse(typeof(StaticLightingType), value, ignoreCase: false);
		}
		catch
		{
		}
		try
		{
			if ((bool)P_0.GetValue("LightMapped", typeof(bool)))
			{
				return StaticLightingType.BakedDown;
			}
			return StaticLightingType.None;
		}
		catch
		{
		}
		return StaticLightingType.None;
	}
}
