using System;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Microsoft.Xna.Framework.Content;

internal class EffectReader : ContentTypeReader<Effect>
{
	protected internal override Effect Read(ContentReader input, Effect existingInstance)
	{
		int count = input.ReadInt32();
		byte[] array = input.ReadBytes(count);
		if (input.platform == 'x')
		{
			string text = Path.Combine(AppContext.BaseDirectory, "ApocZ-shader-dump");
			Directory.CreateDirectory(text);
			File.WriteAllBytes(Path.Combine(text, input.AssetName.Replace('\\', '_').Replace('/', '_') + ".cso"), array);
			string text2 = input.AssetName;
			int num = text2.LastIndexOfAny(new char[2] { '\\', '/' });
			if (num >= 0)
			{
				text2 = text2.Substring(num + 1);
			}
			string text3 = Path.Combine(AppContext.BaseDirectory, "Real" + text2 + ".fxb");
			string text4 = (File.Exists(text3) ? text3 : Path.Combine(AppContext.BaseDirectory, "ApocZCompatibility.fxb"));
			if (File.Exists(text4))
			{
				array = File.ReadAllBytes(text4);
			}
			File.AppendAllText(Path.Combine(AppContext.BaseDirectory, "ApocZ-effect-load.log"), "asset=" + input.AssetName + " simpleName=" + text2 + " realPath=" + text3 + " realExists=" + File.Exists(text3) + " using=" + text4 + Environment.NewLine);
		}
		Effect effect = new Effect(input.ContentManager.GetGraphicsDevice(), array);
		effect.Name = input.AssetName;
		return effect;
	}
}
