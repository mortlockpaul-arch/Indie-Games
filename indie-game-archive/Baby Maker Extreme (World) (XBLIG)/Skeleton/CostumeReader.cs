using System.IO;
using ImportData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Skeleton;

public class CostumeReader : ContentTypeReader<CostumeImportData>
{
	protected override CostumeImportData Read(ContentReader input, CostumeImportData existingInstance)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		CostumeImportData costumeImportData = new CostumeImportData();
		int num = ((BinaryReader)(object)input).ReadInt32();
		Rectangle bound = default(Rectangle);
		for (int i = 0; i < num; i++)
		{
			int id = ((BinaryReader)(object)input).ReadInt32();
			string page = ((BinaryReader)(object)input).ReadString();
			((Rectangle)(ref bound))._002Ector(((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32());
			Vector2 scale = input.ReadVector2();
			Vector2 val = input.ReadVector2();
			float x = val.X;
			float y = val.Y;
			Vector2 offset = input.ReadVector2();
			int type = ((BinaryReader)(object)input).ReadInt32();
			SkeletalCostumeRawData data = new SkeletalCostumeRawData(id, page, bound, x, scale, y, offset, type);
			costumeImportData.Add(data);
		}
		return costumeImportData;
	}
}
