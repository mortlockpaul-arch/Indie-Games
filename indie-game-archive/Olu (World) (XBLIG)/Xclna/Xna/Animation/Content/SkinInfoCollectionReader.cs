using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Xclna.Xna.Animation.Content;

internal class SkinInfoCollectionReader : ContentTypeReader<SkinInfoCollection>
{
	protected override SkinInfoCollection Read(ContentReader input, SkinInfoCollection existingInstance)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		int num = ((BinaryReader)(object)input).ReadInt32();
		SkinInfo[] array = new SkinInfo[num];
		for (int i = 0; i < num; i++)
		{
			int boneIndex = ((BinaryReader)(object)input).ReadInt32();
			string name = ((BinaryReader)(object)input).ReadString();
			Matrix inverseBindPoseTransform = input.ReadMatrix();
			int paletteIndex = ((BinaryReader)(object)input).ReadInt32();
			ref SkinInfo reference = ref array[i];
			reference = new SkinInfo(name, inverseBindPoseTransform, paletteIndex, boneIndex);
		}
		return new SkinInfoCollection(array);
	}
}
