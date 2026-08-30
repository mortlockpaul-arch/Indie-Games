using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cutscene;
using ImportData;
using Microsoft.Xna.Framework.Content;

namespace Skeleton;

public static class CharacterResourceManager
{
	private static Dictionary<string, CharacterManager> sm_characters;

	private static Dictionary<string, SkeletonManager> sm_skeletons;

	private static Dictionary<string, SkeletalCostume> sm_costumes;

	private static Dictionary<string, SkeletalAnimation> sm_animations;

	private static ContentManager content;

	private static MouthImportData mouthData;

	public static void Initialize(ContentManager c)
	{
		sm_characters = new Dictionary<string, CharacterManager>();
		sm_skeletons = new Dictionary<string, SkeletonManager>();
		sm_costumes = new Dictionary<string, SkeletalCostume>();
		sm_animations = new Dictionary<string, SkeletalAnimation>();
		content = c;
		if (c != null)
		{
			mouthData = content.Load<MouthImportData>("mouthanims");
			return;
		}
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
		xmlReaderSettings.IgnoreComments = true;
		xmlReaderSettings.IgnoreWhitespace = true;
		XmlReader xmlReader = XmlReader.Create("mouthanims.xml", xmlReaderSettings);
		mouthData = new MouthImportData(xmlReader);
		xmlReader.Close();
	}

	public static void GetMouthData(List<List<TypedAnimFrame>> dataList)
	{
		dataList.Clear();
		List<List<MouthAnimImportData>> data = mouthData.GetData();
		foreach (List<MouthAnimImportData> item in data)
		{
			dataList.Add(new List<TypedAnimFrame>());
			foreach (MouthAnimImportData item2 in item)
			{
				dataList.Last().Add(new TypedAnimFrame(item2.Time, item2.Index));
			}
		}
	}

	public static CharacterManager LoadCharacter(string filename)
	{
		if (sm_characters.ContainsKey(filename))
		{
			return sm_characters[filename];
		}
		if (content != null)
		{
			sm_characters[filename] = content.Load<CharacterManager>(filename);
			sm_characters[filename].SetResourceFilename(filename);
		}
		else
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreWhitespace = true;
			XmlReader xmlReader = XmlReader.Create(filename, xmlReaderSettings);
			xmlReader.ReadStartElement("root");
			string text = xmlReader.ReadElementString("skeletonFile");
			SkeletonManager skelClone = LoadSkeleton(text);
			string filename2 = xmlReader.ReadElementString("costumeFile");
			SkeletalCostume costumeClone = LoadCostume(filename2, text);
			List<SkeletalAnimation> list = new List<SkeletalAnimation>();
			while (xmlReader.Name != "root")
			{
				string filename3 = xmlReader.ReadElementString("animationFile");
				list.Add(LoadAnim(filename3, text));
			}
			xmlReader.Close();
			sm_characters[filename] = new CharacterManager(skelClone, costumeClone, list, filename);
		}
		return sm_characters[filename];
	}

	public static SkeletonManager LoadSkeleton(string filename)
	{
		if (sm_skeletons.ContainsKey(filename))
		{
			return sm_skeletons[filename];
		}
		if (content != null)
		{
			SkeletonImportData data = content.Load<SkeletonImportData>(filename);
			sm_skeletons[filename] = new SkeletonManager();
			sm_skeletons[filename].Read(data);
		}
		else
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreWhitespace = true;
			XmlReader xmlReader = XmlReader.Create(filename, xmlReaderSettings);
			SkeletonImportData data2 = new SkeletonImportData(xmlReader);
			sm_skeletons[filename] = new SkeletonManager();
			sm_skeletons[filename].Read(data2);
			xmlReader.Close();
		}
		return sm_skeletons[filename];
	}

	public static SkeletalCostume LoadCostume(string filename, string associatedSkeleton)
	{
		if (sm_costumes.ContainsKey(filename))
		{
			return sm_costumes[filename];
		}
		if (content != null)
		{
			CostumeImportData c = content.Load<CostumeImportData>(filename);
			sm_costumes[filename] = new SkeletalCostume();
			sm_costumes[filename].Read(c, LoadSkeleton(associatedSkeleton));
		}
		else
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreWhitespace = true;
			XmlReader xmlReader = XmlReader.Create(filename, xmlReaderSettings);
			sm_costumes[filename] = new SkeletalCostume();
			sm_costumes[filename].Read(xmlReader, LoadSkeleton(associatedSkeleton));
			xmlReader.Close();
		}
		return sm_costumes[filename];
	}

	public static SkeletalAnimation LoadAnim(string filename, string associatedSkeleton)
	{
		if (sm_animations.ContainsKey(filename))
		{
			return sm_animations[filename];
		}
		if (content != null)
		{
			AnimImportData data = content.Load<AnimImportData>(filename);
			sm_animations[filename] = new SkeletalAnimation(LoadSkeleton(associatedSkeleton));
			sm_animations[filename].Read(data);
		}
		else
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.IgnoreWhitespace = true;
			XmlReader xmlReader = XmlReader.Create(filename, xmlReaderSettings);
			AnimImportData data2 = new AnimImportData(xmlReader);
			sm_animations[filename] = new SkeletalAnimation(LoadSkeleton(associatedSkeleton));
			sm_animations[filename].Read(data2);
			xmlReader.Close();
		}
		return sm_animations[filename];
	}

	public static global::Cutscene.Cutscene LoadCutscene(string filename)
	{
		if (content != null)
		{
			CutsceneImportData sceneData = content.Load<CutsceneImportData>(filename);
			global::Cutscene.Cutscene cutscene = new global::Cutscene.Cutscene();
			cutscene.Read(sceneData);
			return cutscene;
		}
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
		xmlReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
		xmlReaderSettings.IgnoreComments = true;
		xmlReaderSettings.IgnoreWhitespace = true;
		XmlReader xmlReader = XmlReader.Create(filename, xmlReaderSettings);
		CutsceneImportData sceneData2 = new CutsceneImportData(xmlReader);
		global::Cutscene.Cutscene cutscene2 = new global::Cutscene.Cutscene();
		cutscene2.Read(sceneData2);
		xmlReader.Close();
		return cutscene2;
	}
}
