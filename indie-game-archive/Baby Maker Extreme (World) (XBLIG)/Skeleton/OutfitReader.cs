using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;

namespace Skeleton;

public class OutfitReader : ContentTypeReader<CharacterManager>
{
	protected override CharacterManager Read(ContentReader input, CharacterManager existingInstance)
	{
		string text = ((BinaryReader)(object)input).ReadString();
		string filename = ((BinaryReader)(object)input).ReadString();
		List<string> list = new List<string>();
		int num = ((BinaryReader)(object)input).ReadInt16();
		for (int i = 0; i < num; i++)
		{
			list.Add(((BinaryReader)(object)input).ReadString());
		}
		SkeletonManager skelClone = CharacterResourceManager.LoadSkeleton(text);
		SkeletalCostume costumeClone = CharacterResourceManager.LoadCostume(filename, text);
		List<SkeletalAnimation> list2 = new List<SkeletalAnimation>(num);
		for (int j = 0; j < num; j++)
		{
			list2.Add(CharacterResourceManager.LoadAnim(list[j], text));
		}
		return new CharacterManager(skelClone, costumeClone, list2, null);
	}
}
