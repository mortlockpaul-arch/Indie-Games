using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace ImportData;

public class BackdropImportData
{
	private List<BackgroundPropImportData> staticProps;

	private string[,] backgrounds;

	public int Width => backgrounds.GetUpperBound(0) + 1;

	public int Height => backgrounds.GetUpperBound(1) + 1;

	public BackdropImportData(XmlReader reader)
	{
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		staticProps = new List<BackgroundPropImportData>();
		reader.ReadStartElement("root");
		int num = int.Parse(reader.ReadElementString("width"));
		int num2 = int.Parse(reader.ReadElementString("height"));
		backgrounds = new string[num, num2];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				backgrounds[i, j] = reader.ReadElementString("backgroundimage");
			}
		}
		Rectangle bound = default(Rectangle);
		Vector2 scale = default(Vector2);
		Vector2 pos = default(Vector2);
		while (reader.Name != "root")
		{
			string page = reader.ReadElementString("propPageName");
			((Rectangle)(ref bound))._002Ector(int.Parse(reader.ReadElementString("boundingX")), int.Parse(reader.ReadElementString("boundingY")), int.Parse(reader.ReadElementString("boundingW")), int.Parse(reader.ReadElementString("boundingH")));
			float depth = float.Parse(reader.ReadElementString("depth"));
			((Vector2)(ref scale))._002Ector(float.Parse(reader.ReadElementString("scaleX")), float.Parse(reader.ReadElementString("scaleY")));
			float rot = float.Parse(reader.ReadElementString("rotation"));
			((Vector2)(ref pos))._002Ector(float.Parse(reader.ReadElementString("posX")), float.Parse(reader.ReadElementString("posY")));
			staticProps.Add(new BackgroundPropImportData(page, bound, depth, scale, rot, pos));
		}
		reader.Close();
	}

	public string[,] GetBackgrounds()
	{
		return backgrounds;
	}

	public BackdropImportData(ContentReader input)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		int num = ((BinaryReader)(object)input).ReadInt32();
		int num2 = ((BinaryReader)(object)input).ReadInt32();
		backgrounds = new string[num, num2];
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				backgrounds[i, j] = ((BinaryReader)(object)input).ReadString();
			}
		}
		int num3 = ((BinaryReader)(object)input).ReadInt32();
		Rectangle bound = default(Rectangle);
		for (int k = 0; k < num3; k++)
		{
			string page = ((BinaryReader)(object)input).ReadString();
			((Rectangle)(ref bound))._002Ector(((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32(), ((BinaryReader)(object)input).ReadInt32());
			float depth = ((BinaryReader)(object)input).ReadSingle();
			Vector2 scale = input.ReadVector2();
			float rot = ((BinaryReader)(object)input).ReadSingle();
			Vector2 pos = input.ReadVector2();
			staticProps.Add(new BackgroundPropImportData(page, bound, depth, scale, rot, pos));
		}
	}

	public string GetImageName(int i, int j)
	{
		return backgrounds[i, j];
	}

	public List<BackgroundPropImportData> GetStaticProps()
	{
		return staticProps;
	}
}
