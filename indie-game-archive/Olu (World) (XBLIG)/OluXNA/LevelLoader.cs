using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OluXNA;

internal class LevelLoader
{
	public static char[] delimits = new char[2] { ' ', ',' };

	public static void LoadLevel(string filename, out Level toLoad)
	{
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0171: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		toLoad = new Level();
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.Load(filename);
		XmlElement documentElement = xmlDocument.DocumentElement;
		Dictionary<string, string> attributeDictionary = GetAttributeDictionary(documentElement);
		num = 0;
		if (documentElement.Attributes["startzone"] != null && !BaseGame.release)
		{
			num = int.Parse(documentElement.Attributes["startzone"].Value);
		}
		toLoad.maxBeats = GetIntFromAtt(attributeDictionary, "beat", 16);
		toLoad.tempo = GetFloatFromAtt(attributeDictionary, "tempo", 164.926f);
		BaseGame.Get().zoneEndTime = toLoad.maxBeats * 2;
		BaseGame.Get().maxBeat = toLoad.maxBeats;
		BaseGame.BEAT = 60f / toLoad.tempo / 4f;
		BaseGame.contentRoot = "Content\\";
		if (documentElement.Attributes["rootdir"] != null)
		{
			BaseGame.contentRoot = documentElement.Attributes["rootdir"].Value;
		}
		toLoad.playerModelPath = "Player/LevelOne";
		if (documentElement.Attributes["player"] != null)
		{
			toLoad.playerModelPath = documentElement.Attributes["player"].Value;
		}
		toLoad.baseColor = GetVectorFromAtt(attributeDictionary, "basecol", Vector3.Zero);
		toLoad.flashColor = GetVectorFromAtt(attributeDictionary, "flashcol", new Vector3(1f, 0.5f, 0f));
		toLoad.effectColor = GetVectorFromAtt(attributeDictionary, "spincol", new Vector3(1f, 0.5f, 0f));
		toLoad.fogStart = GetIntFromAtt(attributeDictionary, "fogstart", 100);
		toLoad.fogEnd = GetIntFromAtt(attributeDictionary, "fogend", 150);
		XmlNodeList elementsByTagName = documentElement.GetElementsByTagName("zone");
		foreach (XmlNode item in elementsByTagName)
		{
			int num2 = ParseInt(item, "id", -1);
			if (num2 == -1)
			{
				throw new Exception("Each zone must have an id attribute");
			}
			toLoad.AddZone(num2);
			toLoad.zones[num2].eq = new EnemyQueue(0f);
			toLoad.activeZone = num2;
			toLoad.ActiveZone.zoneEndTime = ((item.Attributes["endtime"] == null) ? BaseGame.Get().zoneEndTime : int.Parse(item.Attributes["endtime"].Value));
			toLoad.ActiveZone.muteSound = item.Attributes["muteend"] != null && bool.Parse(item.Attributes["muteend"].Value);
			foreach (XmlNode childNode in item.SelectSingleNode("paths").ChildNodes)
			{
				if (childNode.Name != "#comment")
				{
					BuildPath(childNode, out var toBuild, num2);
					attributeDictionary = GetAttributeDictionary(childNode);
					toLoad.zones[num2].paths.Add(int.Parse(attributeDictionary["id"]), toBuild);
				}
			}
			foreach (XmlNode childNode2 in item.SelectSingleNode("BG").ChildNodes)
			{
				if (childNode2.Name != "#comment")
				{
					toLoad.zones[num2].background.Add((IDrawable)MakeObj(childNode2));
				}
			}
			foreach (XmlNode childNode3 in item.SelectSingleNode("music").ChildNodes)
			{
				if (childNode3.Name != "#comment")
				{
					attributeDictionary = GetAttributeDictionary(childNode3);
					toLoad.zones[num2].music.Add(new MusicPart(attributeDictionary, childNode3));
				}
			}
			if (item.SelectSingleNode("endmusic") != null)
			{
				foreach (XmlNode childNode4 in item.SelectSingleNode("endmusic").ChildNodes)
				{
					if (childNode4.Name != "#comment")
					{
						attributeDictionary = GetAttributeDictionary(childNode4);
						toLoad.zones[num2].endMusic.Add(new MusicPart(attributeDictionary, childNode4));
					}
				}
			}
			else
			{
				toLoad.zones[num2].endMusic.Add(new MusicPart(0, "BTPattern5", 0, 9));
			}
			if (item.SelectSingleNode("channel") != null)
			{
				foreach (XmlNode childNode5 in item.SelectSingleNode("channel").ChildNodes)
				{
					if (childNode5.Name != "#comment")
					{
						attributeDictionary = GetAttributeDictionary(childNode5);
						toLoad.zones[num2].channel.Add(new ChannelPart(attributeDictionary, childNode5));
					}
				}
			}
			if (item.SelectSingleNode("endchannel") != null)
			{
				foreach (XmlNode childNode6 in item.SelectSingleNode("endchannel").ChildNodes)
				{
					if (childNode6.Name != "#comment")
					{
						attributeDictionary = GetAttributeDictionary(childNode6);
						toLoad.zones[num2].endChannel.Add(new ChannelPart(attributeDictionary, childNode6));
					}
				}
			}
			if (item.SelectSingleNode("player") != null)
			{
				foreach (XmlNode childNode7 in item.SelectSingleNode("player").ChildNodes)
				{
					if (childNode7.Name != "#comment")
					{
						toLoad.zones[num2].playerPath = (PAbstractSet)MakeObj(childNode7);
					}
				}
			}
			foreach (XmlNode childNode8 in item.SelectSingleNode("enemies").ChildNodes)
			{
				if (!(childNode8.Name != "#comment"))
				{
					continue;
				}
				if (childNode8.Name == "Copy")
				{
					int num3 = int.Parse(childNode8.Attributes["copies"].Value);
					for (int i = 0; i < num3; i++)
					{
						foreach (XmlNode childNode9 in childNode8.ChildNodes)
						{
							toLoad.zones[num2].eq.Push(MakeEnemy(childNode9));
						}
					}
				}
				else
				{
					toLoad.zones[num2].eq.Push(MakeEnemy(childNode8));
				}
			}
		}
		toLoad.LoadZone(num, clear: false);
	}

	public static EnemyQueuePart MakeEnemy(XmlNode elementNode)
	{
		Dictionary<string, string> attributeDictionary = GetAttributeDictionary(elementNode);
		double time = (attributeDictionary.ContainsKey("waittime") ? double.Parse(attributeDictionary["waittime"], CultureInfo.InvariantCulture) : 0.0);
		if (attributeDictionary.ContainsKey("waitbeat"))
		{
			if (attributeDictionary.ContainsKey("waitforenem") && attributeDictionary["waitforenem"] == "1")
			{
				return new EnemyQueuePart((Enemy)MakeObj(elementNode), new BeatCondition(int.Parse(attributeDictionary["waitbeat"])), new NoEnemCondition());
			}
			return new EnemyQueuePart((Enemy)MakeObj(elementNode), new BeatCondition(int.Parse(attributeDictionary["waitbeat"])));
		}
		if (attributeDictionary.ContainsKey("waitforenem") && attributeDictionary["waitforenem"] == "1")
		{
			return new EnemyQueuePart((Enemy)MakeObj(elementNode), new TimeCondition(time), new NoEnemCondition());
		}
		return new EnemyQueuePart((Enemy)MakeObj(elementNode), new TimeCondition(time));
	}

	public static object MakeObj(XmlNode elementNode)
	{
		Dictionary<string, string> attributeDictionary = GetAttributeDictionary(elementNode);
		Type type = Type.GetType("OluXNA." + elementNode.Name);
		int num = 2;
		Type[] array = new Type[num];
		object[] array2 = new object[num];
		array[0] = typeof(Dictionary<string, string>);
		array[1] = typeof(XmlNode);
		ConstructorInfo constructor = type.GetConstructor(array);
		array2[0] = attributeDictionary;
		array2[1] = elementNode;
		return constructor.Invoke(array2);
	}

	public static void BuildPath(XmlNode pathNode, out PathList toBuild, int zoneID)
	{
		Dictionary<string, string> attributeDictionary = GetAttributeDictionary(pathNode);
		toBuild = new PathList();
		if (attributeDictionary.ContainsKey("loop"))
		{
			toBuild.SetLoop(int.Parse(attributeDictionary["loop"]));
		}
		int num = 2;
		Type[] array = new Type[num];
		object[] array2 = new object[num];
		array[0] = typeof(Dictionary<string, string>);
		array[1] = typeof(XmlNode);
		foreach (XmlNode childNode in pathNode.ChildNodes)
		{
			PathList toBuild2;
			if (childNode.Name == "ComboList")
			{
				attributeDictionary = GetAttributeDictionary(childNode.ChildNodes[1]);
				array2[0] = attributeDictionary;
				array2[1] = childNode.ChildNodes[1];
				Type type = Type.GetType("OluXNA." + childNode.ChildNodes[1].Name);
				ConstructorInfo constructor = type.GetConstructor(array);
				IPath comboPart = (IPath)constructor.Invoke(array2);
				BuildPath(childNode.FirstChild, out toBuild2, zoneID);
				toBuild.addPathComboList(toBuild2.publicPaths, comboPart);
			}
			else if (childNode.Name == "CopyList")
			{
				attributeDictionary = GetAttributeDictionary(childNode);
				toBuild2 = BaseGame.Get().level.zones[zoneID].paths[int.Parse(attributeDictionary["id"])].Clone();
				attributeDictionary = GetAttributeDictionary(childNode.FirstChild);
				array2[0] = attributeDictionary;
				array2[1] = childNode.FirstChild;
				Type type = Type.GetType("OluXNA." + childNode.FirstChild.Name);
				ConstructorInfo constructor = type.GetConstructor(array);
				IPath comboPart = (IPath)constructor.Invoke(array2);
				toBuild.addPathComboList(toBuild2.publicPaths, comboPart);
				toBuild._loopIndex = toBuild2.loopIndex;
			}
			else if (childNode.Name != "#comment")
			{
				attributeDictionary = GetAttributeDictionary(childNode);
				array2[0] = attributeDictionary;
				array2[1] = childNode;
				Type type = Type.GetType("OluXNA." + childNode.Name);
				ConstructorInfo constructor = type.GetConstructor(array);
				toBuild.Add((IPath)constructor.Invoke(array2));
			}
		}
	}

	public static void BuildTransform(XmlNode node, out TransformSet ts, int zoneID)
	{
		Dictionary<string, string> attributeDictionary = GetAttributeDictionary(node);
		ts = new TransformSet();
		ts.usePath = true;
		if (attributeDictionary.ContainsKey("usepath") && attributeDictionary["usepath"].Equals("0"))
		{
			ts.usePath = false;
		}
		int num = 2;
		Type[] array = new Type[num];
		object[] array2 = new object[num];
		array[0] = typeof(Dictionary<string, string>);
		array[1] = typeof(XmlNode);
		foreach (XmlNode childNode in node.ChildNodes)
		{
			if (childNode.Name == "Transform")
			{
				attributeDictionary = GetAttributeDictionary(childNode);
				array2[0] = attributeDictionary;
				array2[1] = childNode;
				Type type = Type.GetType("OluXNA." + childNode.Name);
				ConstructorInfo constructor = type.GetConstructor(array);
				ts.tSet.Add((Transform)constructor.Invoke(array2));
			}
		}
	}

	public static int GetIntFromAtt(Dictionary<string, string> att, string name, int defVal)
	{
		if (att.ContainsKey(name))
		{
			return int.Parse(att[name]);
		}
		return defVal;
	}

	public static bool GetBoolFromAtt(Dictionary<string, string> att, string name, bool defVal)
	{
		if (att.ContainsKey(name))
		{
			if (!(att[name] == "true"))
			{
				return false;
			}
			return true;
		}
		return defVal;
	}

	public static double GetDoubleFromAtt(Dictionary<string, string> att, string name, double defVal)
	{
		if (att.ContainsKey(name))
		{
			return double.Parse(att[name], CultureInfo.InvariantCulture);
		}
		return defVal;
	}

	public static float GetFloatFromAtt(Dictionary<string, string> att, string name, float defVal)
	{
		if (att.ContainsKey(name))
		{
			return float.Parse(att[name], CultureInfo.InvariantCulture);
		}
		return defVal;
	}

	public static Vector3 GetVectorFromAtt(Dictionary<string, string> att, string name)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		string[] array = att[name].Split(delimits);
		return new Vector3(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture), float.Parse(array[2], CultureInfo.InvariantCulture));
	}

	public static Vector3 GetVectorFromAtt(Dictionary<string, string> att, string name, Vector3 defVect)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (att.ContainsKey(name))
		{
			return GetVectorFromAtt(att, name);
		}
		return defVect;
	}

	public static Vector4 GetVector4FromAtt(Dictionary<string, string> att, string name)
	{
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		string[] array = att[name].Split(delimits);
		return new Vector4(float.Parse(array[0], CultureInfo.InvariantCulture), float.Parse(array[1], CultureInfo.InvariantCulture), float.Parse(array[2], CultureInfo.InvariantCulture), float.Parse(array[3], CultureInfo.InvariantCulture));
	}

	public static Vector4 GetVector4FromAtt(Dictionary<string, string> att, string name, Vector4 defVect)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (att.ContainsKey(name))
		{
			return GetVector4FromAtt(att, name);
		}
		return defVect;
	}

	public static Color GetColorFromAtt(Dictionary<string, string> att, string name, Color col)
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		if (att.ContainsKey(name))
		{
			string text = att[name];
			byte b = (byte)ushort.Parse(text.Substring(0, 2), NumberStyles.HexNumber);
			byte b2 = (byte)ushort.Parse(text.Substring(2, 2), NumberStyles.HexNumber);
			byte b3 = (byte)ushort.Parse(text.Substring(4, 2), NumberStyles.HexNumber);
			byte b4 = (byte)ushort.Parse(text.Substring(6, 2), NumberStyles.HexNumber);
			return new Color(b, b2, b3, b4);
		}
		return col;
	}

	public static Dictionary<string, string> GetAttributeDictionary(XmlNode node)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		foreach (XmlAttribute attribute in node.Attributes)
		{
			dictionary[attribute.Name] = attribute.Value;
		}
		return dictionary;
	}

	public static int ParseInt(XmlNode node, string name, int defaultValue)
	{
		int result = defaultValue;
		XmlAttribute xmlAttribute = (XmlAttribute)node.Attributes.GetNamedItem(name);
		if (xmlAttribute != null)
		{
			result = int.Parse(xmlAttribute.Value);
		}
		return result;
	}

	public static double ParseDouble(XmlNode node, string name, double defaultValue)
	{
		double result = defaultValue;
		XmlAttribute xmlAttribute = (XmlAttribute)node.Attributes.GetNamedItem(name);
		if (xmlAttribute != null)
		{
			result = double.Parse(xmlAttribute.Value, CultureInfo.InvariantCulture);
		}
		return result;
	}
}
