using System;
using System.Runtime.InteropServices;

namespace Microsoft.XboxLive.MathUtilities;

public class DxtDecoder
{
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	private struct Colorb
	{
		[FieldOffset(0)]
		public byte b;

		[FieldOffset(1)]
		public byte g;

		[FieldOffset(2)]
		public byte r;

		[FieldOffset(3)]
		public byte a;

		[FieldOffset(0)]
		public uint argb;
	}

	private byte[] mInput;

	private int[] mOutput;

	private Colorb[] mClut = new Colorb[4];

	private Colorb[] mAlut = new Colorb[8];

	private Colorb[] mAlphas = new Colorb[16];

	private TextureDataFormat mFormat;

	private int mAlignedWidth;

	private int mAlignedHeight;

	private int m_Width;

	private int m_Height;

	private int mOffset;

	private int mBlockOffset;

	private void UnpackBlockColor(int outBlockW, int outBlockH)
	{
		ushort num = (ushort)(mInput[mOffset] | (mInput[mOffset + 1] << 8));
		mOffset += 2;
		ushort num2 = (ushort)(mInput[mOffset] | (mInput[mOffset + 1] << 8));
		mOffset += 2;
		uint num3 = (uint)(-16777216 | (((num << 3) & 0xF8) | ((num << 5) & 0xFC00) | ((num << 8) & 0xF80000)));
		num3 |= (num3 >> 5) & 0x70007;
		num3 |= (num3 >> 6) & 0x300;
		mClut[0].argb = num3;
		uint num4 = (uint)(-16777216 | (((num2 << 3) & 0xF8) | ((num2 << 5) & 0xFC00) | ((num2 << 8) & 0xF80000)));
		num4 |= (num4 >> 5) & 0x70007;
		num4 |= (num4 >> 6) & 0x300;
		mClut[1].argb = num4;
		if (mFormat == TextureDataFormat.Dxt1 && num <= num2)
		{
			mClut[2].r = (byte)(mClut[0].r + mClut[1].r + 1 >> 1);
			mClut[2].g = (byte)(mClut[0].g + mClut[1].g + 1 >> 1);
			mClut[2].b = (byte)(mClut[0].b + mClut[1].b + 1 >> 1);
			mClut[2].a = byte.MaxValue;
			mClut[3].argb = 0u;
		}
		else
		{
			mClut[2].r = (byte)((4 * mClut[0].r + 2 * mClut[1].r + 3) / 6);
			mClut[2].g = (byte)((4 * mClut[0].g + 2 * mClut[1].g + 3) / 6);
			mClut[2].b = (byte)((4 * mClut[0].b + 2 * mClut[1].b + 3) / 6);
			mClut[2].a = byte.MaxValue;
			mClut[3].r = (byte)((2 * mClut[0].r + 4 * mClut[1].r + 3) / 6);
			mClut[3].g = (byte)((2 * mClut[0].g + 4 * mClut[1].g + 3) / 6);
			mClut[3].b = (byte)((2 * mClut[0].b + 4 * mClut[1].b + 3) / 6);
			mClut[3].a = byte.MaxValue;
		}
		int num5 = 0;
		int num6 = mBlockOffset;
		if (mFormat == TextureDataFormat.Dxt2 || mFormat == TextureDataFormat.Dxt4)
		{
			Colorb colorb = default(Colorb);
			for (int i = 0; i < outBlockH; i++)
			{
				int num7 = mInput[mOffset++];
				for (int j = 0; j < outBlockW; j++)
				{
					int num8 = mAlphas[num5 + j].a + 1;
					colorb.argb = mClut[num7 & 3].argb;
					mOutput[num6 + j] = (int)(((((colorb.argb >> 8) & 0xFF00FF) * num8) & 0xFF00FF00u) | (((colorb.argb & 0xFF00FF) * num8 >> 8) & 0xFF00FF));
					num7 >>= 2;
				}
				num6 += m_Width;
				num5 += 4;
			}
			return;
		}
		for (int k = 0; k < outBlockH; k++)
		{
			int num9 = mInput[mOffset++];
			for (int l = 0; l < outBlockW; l++)
			{
				mOutput[num6 + l] = (int)(mClut[num9 & 3].argb & mAlphas[num5 + l].argb);
				num9 >>= 2;
			}
			num6 += m_Width;
			num5 += 4;
		}
	}

	private void UnpackBlockAlphaDXT23()
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			uint num2 = mInput[mOffset++];
			mAlphas[num++].a = (byte)((num2 << 4) | (num2 & 0xF));
			mAlphas[num++].a = (byte)((num2 & 0xF0) | (num2 >> 4));
		}
	}

	private void UnpackBlockAlphaDXT45()
	{
		byte b = mInput[mOffset++];
		byte b2 = mInput[mOffset++];
		mAlut[0].a = b;
		mAlut[1].a = b2;
		if (b <= b2)
		{
			mAlut[2].a = (byte)((8 * b + 2 * b2 + 5) / 10);
			mAlut[3].a = (byte)((6 * b + 4 * b2 + 5) / 10);
			mAlut[4].a = (byte)((4 * b + 6 * b2 + 5) / 10);
			mAlut[5].a = (byte)((2 * b + 8 * b2 + 5) / 10);
			mAlut[6].a = 0;
			mAlut[7].a = byte.MaxValue;
		}
		else
		{
			mAlut[2].a = (byte)((12 * b + 2 * b2 + 7) / 14);
			mAlut[3].a = (byte)((10 * b + 4 * b2 + 7) / 14);
			mAlut[4].a = (byte)((8 * b + 6 * b2 + 7) / 14);
			mAlut[5].a = (byte)((6 * b + 8 * b2 + 7) / 14);
			mAlut[6].a = (byte)((4 * b + 10 * b2 + 7) / 14);
			mAlut[7].a = (byte)((2 * b + 12 * b2 + 7) / 14);
		}
		int num = 0;
		for (int i = 0; i < 2; i++)
		{
			int num2 = mInput[mOffset] | (mInput[mOffset + 1] << 8) | (mInput[mOffset + 2] << 16);
			mOffset += 3;
			mAlphas[num].argb = mAlut[num2 & 7].argb;
			mAlphas[num + 1].argb = mAlut[(num2 >> 3) & 7].argb;
			mAlphas[num + 2].argb = mAlut[(num2 >> 6) & 7].argb;
			mAlphas[num + 3].argb = mAlut[(num2 >> 9) & 7].argb;
			mAlphas[num + 4].argb = mAlut[(num2 >> 12) & 7].argb;
			mAlphas[num + 5].argb = mAlut[(num2 >> 15) & 7].argb;
			mAlphas[num + 6].argb = mAlut[(num2 >> 18) & 7].argb;
			mAlphas[num + 7].argb = mAlut[(num2 >> 21) & 7].argb;
			num += 8;
		}
	}

	private void UnpackBlock(int outBlockW, int outBlockH)
	{
		switch (mFormat)
		{
		case TextureDataFormat.Dxt4:
		case TextureDataFormat.Dxt5:
			UnpackBlockAlphaDXT45();
			break;
		case TextureDataFormat.Dxt2:
		case TextureDataFormat.Dxt3:
			UnpackBlockAlphaDXT23();
			break;
		}
		UnpackBlockColor(outBlockW, outBlockH);
	}

	public int[] UnpackImage(byte[] image, int width, int height, TextureDataFormat format)
	{
		mInput = image;
		mAlignedWidth = (width + 3) & -4;
		mAlignedHeight = (height + 3) & -4;
		mFormat = format;
		mOffset = 0;
		if (format >= TextureDataFormat.RGBA)
		{
			throw new ArgumentException("Invalid data format", "format");
		}
		if (format == TextureDataFormat.Dxt1)
		{
			if (mAlignedWidth * mAlignedHeight != image.Length * 2)
			{
				throw new ArgumentException("Compressed data length does not match", "image");
			}
		}
		else if (mAlignedWidth * mAlignedHeight != image.Length)
		{
			throw new ArgumentException("Compressed data length does not match", "image");
		}
		mOutput = new int[width * height];
		m_Width = width;
		m_Height = height;
		switch (mFormat)
		{
		case TextureDataFormat.Dxt1:
		{
			for (int j = 0; j < 16; j++)
			{
				mAlphas[j].argb = uint.MaxValue;
			}
			break;
		}
		case TextureDataFormat.Dxt4:
		case TextureDataFormat.Dxt5:
		{
			for (int i = 0; i < 8; i++)
			{
				mAlut[i].argb = uint.MaxValue;
			}
			break;
		}
		}
		int num = mAlignedHeight - 4;
		int num2 = mAlignedWidth - 4;
		int k;
		int l;
		for (k = 0; k < num; k += 4)
		{
			for (l = 0; l < num2; l += 4)
			{
				mBlockOffset = k * m_Width + l;
				UnpackBlock(4, 4);
			}
			mBlockOffset = k * m_Width + l;
			UnpackBlock(4 + m_Width - mAlignedWidth, 4);
		}
		l = 0;
		int outBlockH = 4 + m_Height - mAlignedHeight;
		for (; l < num2; l += 4)
		{
			mBlockOffset = k * m_Width + l;
			UnpackBlock(4, outBlockH);
		}
		mBlockOffset = k * m_Width + mAlignedWidth - 4;
		UnpackBlock(4 + m_Width - mAlignedWidth, outBlockH);
		return mOutput;
	}
}
