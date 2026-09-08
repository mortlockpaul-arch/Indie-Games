using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;

namespace XnaToFna.ContentTransformers;

public class SoundEffectTransformer : ContentTypeReader<SoundEffect>
{
	private static readonly Type t_SoundEffect = typeof(SoundEffect);

	private static readonly FieldInfo f_Instances = typeof(SoundEffect).GetField("Instances", BindingFlags.Instance | BindingFlags.NonPublic);

	private static readonly List<WeakReference> DummyReferences = new List<WeakReference>();

	protected override SoundEffect Read(ContentReader input, SoundEffect existing)
	{
		CopyingStream copyingStream = (CopyingStream)input.BaseStream;
		copyingStream.Copy = false;
		long position = input.BaseStream.Position;
		input.BaseStream.Seek(3L, SeekOrigin.Begin);
		char c = input.ReadChar();
		bool x360 = c == 'x';
		input.BaseStream.Seek(position, SeekOrigin.Begin);
		using BinaryWriter binaryWriter = new BinaryWriter(copyingStream.Output, Encoding.UTF8, leaveOpen: true);
		uint fmtLength = input.ReadUInt32();
		ushort num = ContentHelper.SwapEndian(x360, input.ReadUInt16());
		ushort value = ContentHelper.SwapEndian(x360, input.ReadUInt16());
		uint value2 = ContentHelper.SwapEndian(x360, input.ReadUInt32());
		if (num == 353 || num == 358)
		{
			input.BaseStream.Seek(fmtLength - 2 - 2 - 4, SeekOrigin.Current);
			int dataLength = input.ReadInt32();
			binaryWriter.Write(18u);
			binaryWriter.Write((ushort)1);
			binaryWriter.Write((ushort)1);
			binaryWriter.Write(value2);
			binaryWriter.Write(0u);
			binaryWriter.Write((ushort)0);
			binaryWriter.Write((ushort)8);
			binaryWriter.Write((ushort)0);
			long position2 = binaryWriter.BaseStream.Position;
			binaryWriter.Write(0u);
			input.BaseStream.Seek(position, SeekOrigin.Begin);
			ContentHelper.ConvertAudio(input.BaseStream, copyingStream.Output, ContentHelper.GenerateSoundEffectFeeder(input, (num == 353) ? "XWMA" : "WAVE", (num == 358) ? 52u : fmtLength, (uint)dataLength, 0u, x360, num switch
			{
				358 => delegate(BinaryWriter ffmpegWriter)
				{
					ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt16()));
					ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt16()));
					ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
					ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
					ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
					ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
					ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
					ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
					ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
					ffmpegWriter.Write(input.ReadByte());
					ffmpegWriter.Write(input.ReadByte());
					ffmpegWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt16()));
					input.BaseStream.Seek(fmtLength - 18 - 34, SeekOrigin.Current);
				}, 
				353 => null, 
				_ => null, 
			}), 0L);
			long position3 = binaryWriter.BaseStream.Position;
			binaryWriter.BaseStream.Seek(position2, SeekOrigin.Begin);
			binaryWriter.Write((uint)(position3 - position2 - 4));
			binaryWriter.BaseStream.Seek(position3, SeekOrigin.Begin);
		}
		else
		{
			binaryWriter.Write(fmtLength);
			binaryWriter.Write(num);
			binaryWriter.Write(value);
			binaryWriter.Write(value2);
			binaryWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt32()));
			binaryWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt16()));
			binaryWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt16()));
			binaryWriter.Write(ContentHelper.SwapEndian(x360, input.ReadUInt16()));
			binaryWriter.Write(input.ReadBytes((int)(fmtLength - 18)));
			int dataLength = input.ReadInt32();
			binaryWriter.Write(dataLength);
			binaryWriter.Write(input.ReadBytes(dataLength));
		}
		binaryWriter.Write(input.ReadUInt32());
		binaryWriter.Write(input.ReadUInt32());
		binaryWriter.Write(input.ReadUInt32());
		copyingStream.Copy = true;
		if (existing != null)
		{
			return existing;
		}
		existing = (SoundEffect)FormatterServices.GetUninitializedObject(t_SoundEffect);
		f_Instances.SetValue(existing, DummyReferences);
		return existing;
	}
}
