using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Xclna.Xna.Animation.Content;

public class PaletteEffectReader : ContentTypeReader<BasicPaletteEffect>
{
	protected override BasicPaletteEffect Read(ContentReader input, BasicPaletteEffect existingInstance)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		ContentManager contentManager = input.ContentManager;
		IGraphicsDeviceService val = (IGraphicsDeviceService)contentManager.ServiceProvider.GetService(typeof(IGraphicsDeviceService));
		byte[] byteCode = input.ReadRawObject<byte[]>();
		int paletteSize = ((BinaryReader)(object)input).ReadInt32();
		BasicPaletteEffect basicPaletteEffect = new BasicPaletteEffect(val.GraphicsDevice, byteCode, paletteSize);
		if (((BinaryReader)(object)input).ReadBoolean())
		{
			basicPaletteEffect.Texture = input.ReadExternalReference<Texture2D>();
			basicPaletteEffect.TextureEnabled = true;
		}
		if (((BinaryReader)(object)input).ReadBoolean())
		{
			basicPaletteEffect.SpecularPower = ((BinaryReader)(object)input).ReadSingle();
		}
		else
		{
			basicPaletteEffect.SpecularPower = 8f;
		}
		Color black;
		if (((BinaryReader)(object)input).ReadBoolean())
		{
			basicPaletteEffect.SpecularColor = input.ReadVector3();
		}
		else
		{
			black = Color.Black;
			basicPaletteEffect.SpecularColor = ((Color)(ref black)).ToVector3();
		}
		if (((BinaryReader)(object)input).ReadBoolean())
		{
			basicPaletteEffect.EmissiveColor = input.ReadVector3();
		}
		else
		{
			black = Color.Black;
			basicPaletteEffect.EmissiveColor = ((Color)(ref black)).ToVector3();
		}
		if (((BinaryReader)(object)input).ReadBoolean())
		{
			basicPaletteEffect.DiffuseColor = input.ReadVector3();
		}
		else
		{
			black = Color.Black;
			basicPaletteEffect.DiffuseColor = ((Color)(ref black)).ToVector3();
		}
		if (((BinaryReader)(object)input).ReadBoolean())
		{
			basicPaletteEffect.Alpha = ((BinaryReader)(object)input).ReadSingle();
		}
		else
		{
			basicPaletteEffect.Alpha = 1f;
		}
		return basicPaletteEffect;
	}
}
