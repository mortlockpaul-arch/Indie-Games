using System.Text;
using Microsoft.Xna.Framework.Content;
using SynapseGaming.LightingSystem.Rendering;
using V;

namespace SynapseGaming.LightingSystem.Processors;

/// <summary />
public class SceneReader_Indie : ContentTypeReader<Scene>
{
	/// <summary />
	protected override Scene Read(ContentReader input, Scene instance)
	{
		string text = string.Empty;
		int num = input.ReadInt32();
		if (num > 0)
		{
			byte[] array = input.ReadBytes(num);
			text = Encoding.UTF8.GetString(array, 0, array.Length);
		}
		Scene scene = Scene._0002F(text);
		scene._0002N(input.ReadString());
		scene.FileName = input.ReadString();
		scene.ProjectFile = input.ReadString();
		V.B.H_0005(input);
		return scene;
	}
}
