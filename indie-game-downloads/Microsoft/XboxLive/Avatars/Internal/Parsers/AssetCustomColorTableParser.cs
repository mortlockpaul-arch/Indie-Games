using Microsoft.XboxLive.Avatars.Internal.Assets;

namespace Microsoft.XboxLive.Avatars.Internal.Parsers;

public class AssetCustomColorTableParser
{
	public class ColorTableParser : DataUnpackerGeneric<ComponentColors>
	{
		public IntegerDataUnpacker m_Red = new IntegerDataUnpacker();

		public IntegerDataUnpacker m_Green = new IntegerDataUnpacker();

		public IntegerDataUnpacker m_Blue = new IntegerDataUnpacker();

		public override void UnpackHeader(BitStream bitStream)
		{
			m_Red.UnpackHeader(bitStream);
			m_Green.UnpackHeader(bitStream);
			m_Blue.UnpackHeader(bitStream);
		}

		public override void UnpackData(BitStream bitStream, out ComponentColors data)
		{
			m_Red.UnpackData(bitStream, out var data2);
			data.CustomColor0 = Utilities.Vector4FromInt(data2);
			m_Green.UnpackData(bitStream, out data2);
			data.CustomColor1 = Utilities.Vector4FromInt(data2);
			m_Blue.UnpackData(bitStream, out data2);
			data.CustomColor2 = Utilities.Vector4FromInt(data2);
		}
	}

	public static ComponentColorTable Parse(BlockIterator blockIterator)
	{
		ComponentColorTable componentColorTable = new ComponentColorTable();
		ByteStreamUnpacker<ComponentColors> byteStreamUnpacker = new ByteStreamUnpacker<ComponentColors>(blockIterator, new ColorTableParser());
		byteStreamUnpacker.Unpack(out componentColorTable.Colors);
		return componentColorTable;
	}
}
