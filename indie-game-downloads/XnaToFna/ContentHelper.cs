using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using MonoMod.Utils;
using XnaToFna.ContentTransformers;

namespace XnaToFna;

public static class ContentHelper
{
	public enum SoundBankEventType : uint
	{
		Stop = 0u,
		PlayWave = 1u,
		PlayWaveTrackVariation = 3u,
		PlayWaveEffectVariation = 4u,
		PlayWaveTrackEffectVariation = 6u,
		Pitch = 7u,
		Volume = 8u,
		Marker = 9u,
		PitchRepeating = 16u,
		VolumeRepeating = 17u,
		MarkerRepeating = 18u
	}

	public enum CrossfadeType : byte
	{
		Linear,
		Logarithmic,
		EqualPower
	}

	public static class XWMAInfo
	{
		public static readonly int[] BytesPerSecond = new int[6] { 12000, 24000, 4000, 6000, 8000, 20000 };

		public static readonly short[] BlockAlign = new short[16]
		{
			929, 1487, 1280, 2230, 8917, 8192, 4459, 5945, 2304, 1536,
			1485, 1008, 2731, 4096, 6827, 5462
		};
	}

	public static class XMAInfo
	{
		public static readonly int[] BytesPerSecond = new int[6] { 12000, 24000, 4000, 6000, 8000, 20000 };

		public static readonly short[] BlockAlign = new short[16]
		{
			929, 1487, 1280, 2230, 8917, 8192, 4459, 5945, 2304, 1536,
			1485, 1008, 2731, 4096, 6827, 5462
		};
	}

	public static bool XNBCompressGZip = true;

	private static Type t_GZipContentReader = typeof(GZipContentReader<>);

	public const uint XSBHeader = 1262634067u;

	public const uint XSBHeaderX360 = 1396982347u;

	public const uint XGSHeader = 1179862872u;

	public const uint XGSHeaderX360 = 1481069382u;

	public static ContentHelperGame Game;

	public const uint XWBHeader = 1145979479u;

	public const uint XWBHeaderX360 = 1463963204u;

	public static bool IsFFMPEGAvailable
	{
		get
		{
			try
			{
				Process process = new Process();
				process.StartInfo = new ProcessStartInfo
				{
					FileName = (((PlatformHelper.Current & Platform.Windows) == Platform.Windows) ? "where" : "which"),
					Arguments = "ffmpeg",
					CreateNoWindow = true,
					UseShellExecute = false
				};
				process.Start();
				process.WaitForExit();
				return process.ExitCode == 0;
			}
			catch (Exception ex)
			{
				Log("Could not determine if FFMPEG available: " + ex);
				return false;
			}
		}
	}

	public static void TransformContent(string path)
	{
		if (Game == null || !File.Exists(path))
		{
			return;
		}
		Log($"[TransformContent] Transforming {path}");
		object obj = Game.Content.Load<object>(path);
		try
		{
			(obj as IDisposable)?.Dispose();
			Game.Content.Unload();
		}
		catch
		{
		}
		UpdateXNBSize(path + ".tmp");
		File.Delete(path);
		if (!XNBCompressGZip)
		{
			File.Move(path + ".tmp", path);
			return;
		}
		using (Stream stream = File.Open(path + ".tmp", FileMode.Open, FileAccess.Read))
		{
			using Stream stream2 = File.Open(path, FileMode.Create, FileAccess.Write);
			using (BinaryReader binaryReader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true))
			{
				using BinaryWriter binaryWriter = new BinaryWriter(stream2, Encoding.ASCII, leaveOpen: true);
				binaryWriter.Write(binaryReader.ReadBytes(6));
				binaryWriter.Write(0u);
				binaryWriter.Write((byte)1);
				binaryWriter.Write(t_GZipContentReader.MakeGenericType(obj.GetType()).AssemblyQualifiedName);
				binaryWriter.Write(0u);
				binaryWriter.Write((byte)0);
				binaryWriter.Write((byte)1);
			}
			stream.Seek(0L, SeekOrigin.Begin);
			using GZipStream destination = new GZipStream(stream2, CompressionMode.Compress, leaveOpen: true);
			stream.CopyTo(destination);
		}
		File.Delete(path + ".tmp");
		UpdateXNBSize(path);
	}

	public static void UpdateXNBSize(string path, uint size = 0u)
	{
		using Stream stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
		using BinaryWriter binaryWriter = new BinaryWriter(stream);
		if (size == 0)
		{
			size = (uint)stream.Length;
		}
		stream.Position = 6L;
		binaryWriter.Write(size);
	}

	public static void UpdateVideo(string path, BinaryReader reader = null, BinaryWriter writer = null)
	{
		if (!IsFFMPEGAvailable)
		{
			Log("[UpdateVideo] FFMPEG is missing - won't convert unsupported video files");
			if (reader != null && writer != null)
			{
				reader.BaseStream.CopyTo(writer.BaseStream);
			}
			return;
		}
		string text = Path.ChangeExtension(path, "xnb");
		if (File.Exists(text + "_"))
		{
			File.Delete(text + "_");
		}
		if (File.Exists(text))
		{
			File.Move(text, text + "_");
		}
		string text2 = Path.ChangeExtension(path, "ogv");
		if (writer != null || string.IsNullOrEmpty(path) || !File.Exists(text2))
		{
			Log($"[UpdateVideo] Updating video {path}");
			if (writer == null)
			{
				RunFFMPEG(string.Format("-i {0} -acodec libvorbis -vcodec libtheora \"{1}\"", (reader == null) ? $"\"{path}\"" : "-", text2), reader?.BaseStream, null, null, 0L);
			}
			else
			{
				RunFFMPEG("-y -i - -acodec libvorbis -vcodec libtheora -", reader.BaseStream, writer.BaseStream, null, 0L);
			}
		}
	}

	public static void UpdateAudio(string path, BinaryReader reader = null, BinaryWriter writer = null)
	{
		if (!IsFFMPEGAvailable)
		{
			Log("[UpdateAudio] FFMPEG is missing - won't convert unsupported audio files");
			if (reader != null && writer != null)
			{
				reader.BaseStream.CopyTo(writer.BaseStream);
			}
			return;
		}
		string text = Path.ChangeExtension(path, "xnb");
		if (File.Exists(text + "_"))
		{
			File.Delete(text + "_");
		}
		if (File.Exists(text))
		{
			File.Move(text, text + "_");
		}
		string text2 = Path.ChangeExtension(path, "ogg");
		if (writer != null || string.IsNullOrEmpty(path) || !File.Exists(text2))
		{
			Log($"[UpdateAudio] Updating audio {path}");
			if (writer == null)
			{
				RunFFMPEG(string.Format("-i {0} -acodec libvorbis \"{1}\"", (reader == null) ? $"\"{path}\"" : "-", text2), reader?.BaseStream, null, null, 0L);
			}
			else
			{
				RunFFMPEG("-y -i - -acodec libvorbis -", reader.BaseStream, writer.BaseStream, null, 0L);
			}
		}
	}

	public static void UpdateSoundBank(string path, BinaryReader reader, BinaryWriter writer)
	{
		Log($"[UpdateSoundBank] Updating sound bank {path}");
		bool flag = reader.ReadUInt32() == 1396982347;
		writer.Write(1262634067u);
		if (!flag)
		{
			reader.BaseStream.CopyTo(writer.BaseStream);
			return;
		}
		writer.Write(SwapEndian(flag, reader.ReadUInt16()));
		writer.Write(SwapEndian(flag, reader.ReadUInt16()));
		writer.Write(SwapEndian(flag, reader.ReadUInt16()));
		writer.Write(SwapEndian(flag, reader.ReadUInt64()));
		byte b = reader.ReadByte();
		writer.Write((byte)1);
		if ((flag && b != 3) || (!flag && b != 1))
		{
			Log(string.Format("[UpdateSoundBank] Possible platform mismatch! Platform: 0x{0}; Big endian (X360): {1}", b.ToString("X2"), flag));
		}
		ushort num = SwapEndian(flag, reader.ReadUInt16());
		writer.Write(num);
		ushort num2 = SwapEndian(flag, reader.ReadUInt16());
		writer.Write(num2);
		writer.Write(SwapEndian(flag, reader.ReadUInt16()));
		writer.Write(SwapEndian(flag, reader.ReadUInt16()));
		writer.Write(reader.ReadByte());
		ushort num3 = SwapEndian(flag, reader.ReadUInt16());
		writer.Write(num3);
		long position = reader.BaseStream.Position;
		ushort value = SwapEndian(flag, reader.ReadUInt16());
		writer.Write(value);
		writer.Write(SwapEndian(flag, reader.ReadUInt16()));
		long position2 = reader.BaseStream.Position;
		uint num4 = SwapEndian(flag, reader.ReadUInt32());
		writer.Write(num4);
		long position3 = reader.BaseStream.Position;
		uint num5 = SwapEndian(flag, reader.ReadUInt32());
		writer.Write(num5);
		long position4 = reader.BaseStream.Position;
		uint num6 = SwapEndian(flag, reader.ReadUInt32());
		writer.Write(num6);
		writer.Write(SwapEndian(flag, reader.ReadUInt32()));
		long position5 = reader.BaseStream.Position;
		uint num7 = SwapEndian(flag, reader.ReadUInt32());
		writer.Write(num7);
		writer.Write(SwapEndian(flag, reader.ReadUInt32()));
		long position6 = reader.BaseStream.Position;
		uint value2 = SwapEndian(flag, reader.ReadUInt32());
		writer.Write(value2);
		writer.Write(SwapEndian(flag, reader.ReadUInt32()));
		long position7 = reader.BaseStream.Position;
		uint num8 = SwapEndian(flag, reader.ReadUInt32());
		writer.Write(num8);
		uint num9 = SwapEndian(flag, reader.ReadUInt32());
		writer.Write(num9);
		writer.Write(reader.ReadBytes(64));
		writer.Write(reader.ReadBytesUntil(num9));
		for (ushort num10 = 0; num10 < num3; num10++)
		{
			long position8 = reader.BaseStream.Position;
			byte b2 = reader.ReadByte();
			writer.Write(b2);
			byte b3 = 1;
			writer.Write(SwapEndian(flag, reader.ReadUInt16()));
			writer.Write(reader.ReadByte());
			writer.Write(SwapEndian(flag, reader.ReadUInt16()));
			writer.Write(reader.ReadByte());
			ushort num11 = SwapEndian(flag, reader.ReadUInt16());
			writer.Write(num11);
			if ((b2 & 1) != 0)
			{
				writer.Write(b3 = reader.ReadByte());
			}
			else
			{
				writer.Write(SwapEndian(flag, reader.ReadUInt16()));
				writer.Write(reader.ReadByte());
			}
			if ((b2 & 0xE) != 0)
			{
				writer.Write(SwapEndian(flag, reader.ReadUInt16()));
				ushort num12 = 0;
				if ((b2 & 2) != 0)
				{
					num12++;
				}
				if ((b2 & 4) != 0)
				{
					num12 += b3;
				}
				for (ushort num13 = 0; num13 < num12; num13++)
				{
					byte b4 = reader.ReadByte();
					writer.Write(b4);
					for (byte b5 = 0; b5 < b4; b5++)
					{
						writer.Write(SwapEndian(flag, reader.ReadUInt32()));
					}
				}
			}
			if ((b2 & 0x10) != 0)
			{
				writer.Write(SwapEndian(flag, reader.ReadUInt16()));
				byte b6 = reader.ReadByte();
				writer.Write(b6);
				for (byte b7 = 0; b7 < b6; b7++)
				{
					writer.Write(SwapEndian(flag, reader.ReadUInt32()));
				}
			}
			if ((b2 & 1) != 0)
			{
				for (byte b8 = 0; b8 < b3; b8++)
				{
					writer.Write(reader.ReadByte());
					writer.Write(SwapEndian(flag, reader.ReadUInt32()));
					writer.Write(reader.ReadByte());
					writer.Write(reader.ReadByte());
					writer.Write(SwapEndian(flag, reader.ReadUInt16()));
				}
				for (byte b9 = 0; b9 < b3; b9++)
				{
					byte b10 = reader.ReadByte();
					writer.Write(b10);
					for (byte b11 = 0; b11 < b10; b11++)
					{
						uint num14 = SwapEndian(flag, reader.ReadUInt32());
						writer.Write(num14);
						writer.Write(SwapEndian(flag, reader.ReadUInt16()));
						SoundBankEventType soundBankEventType = (SoundBankEventType)(num14 & 0x1F);
						byte b12 = reader.ReadByte();
						if (b12 != byte.MaxValue)
						{
							Log(string.Format("[UpdateSoundBank] Expected 0xFF between event info and data, got 0x{0} instead! ({1}, {2}, {3})", b12.ToString("X2"), num10, b9, b11));
						}
						writer.Write(b12);
						ushort num15 = 0;
						ushort num16 = 0;
						switch (soundBankEventType)
						{
						case SoundBankEventType.Stop:
							writer.Write(reader.ReadByte());
							break;
						case SoundBankEventType.PlayWave:
							writer.Write(reader.ReadByte());
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(reader.ReadByte());
							writer.Write(reader.ReadByte());
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							break;
						case SoundBankEventType.PlayWaveTrackVariation:
						{
							writer.Write(reader.ReadByte());
							writer.Write(reader.ReadByte());
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							if (b == 1)
							{
								num15 = SwapEndian(flag, reader.ReadUInt16());
								num16 = SwapEndian(flag, reader.ReadUInt16());
							}
							else
							{
								num16 = SwapEndian(flag, reader.ReadUInt16());
								num15 = SwapEndian(flag, reader.ReadUInt16());
							}
							writer.Write(num15);
							writer.Write(num16);
							writer.Write(reader.ReadUInt32());
							for (ushort num17 = 0; num17 < num15; num17++)
							{
								writer.Write(SwapEndian(flag, reader.ReadUInt16()));
								writer.Write(reader.ReadByte());
								writer.Write(reader.ReadByte());
								writer.Write(reader.ReadByte());
							}
							break;
						}
						case SoundBankEventType.PlayWaveEffectVariation:
							writer.Write(reader.ReadByte());
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(reader.ReadByte());
							writer.Write(reader.ReadByte());
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(reader.ReadByte());
							writer.Write(reader.ReadByte());
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							break;
						case SoundBankEventType.PlayWaveTrackEffectVariation:
						{
							writer.Write(reader.ReadByte());
							writer.Write(reader.ReadByte());
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(reader.ReadByte());
							writer.Write(reader.ReadByte());
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							if (b == 1)
							{
								num15 = SwapEndian(flag, reader.ReadUInt16());
								num16 = SwapEndian(flag, reader.ReadUInt16());
							}
							else
							{
								num16 = SwapEndian(flag, reader.ReadUInt16());
								num15 = SwapEndian(flag, reader.ReadUInt16());
							}
							writer.Write(num15);
							writer.Write(num16);
							writer.Write(reader.ReadUInt32());
							for (ushort num18 = 0; num18 < num15; num18++)
							{
								writer.Write(SwapEndian(flag, reader.ReadUInt16()));
								writer.Write(reader.ReadByte());
								writer.Write(reader.ReadByte());
								writer.Write(reader.ReadByte());
							}
							break;
						}
						case SoundBankEventType.Pitch:
						case SoundBankEventType.Volume:
						case SoundBankEventType.PitchRepeating:
						case SoundBankEventType.VolumeRepeating:
						{
							byte b13 = reader.ReadByte();
							writer.Write(b13);
							if ((b13 & 1) != 0)
							{
								writer.Write(SwapEndian(flag, reader.ReadUInt32()));
								writer.Write(SwapEndian(flag, reader.ReadUInt32()));
								writer.Write(SwapEndian(flag, reader.ReadUInt32()));
								writer.Write(SwapEndian(flag, reader.ReadUInt16()));
								break;
							}
							writer.Write(reader.ReadByte());
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(reader.ReadUInt32());
							writer.Write(reader.ReadByte());
							if (soundBankEventType == SoundBankEventType.PitchRepeating || soundBankEventType == SoundBankEventType.VolumeRepeating)
							{
								writer.Write(SwapEndian(flag, reader.ReadUInt16()));
								writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							}
							break;
						}
						case SoundBankEventType.Marker:
						case SoundBankEventType.MarkerRepeating:
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							break;
						}
					}
				}
			}
			if (reader.BaseStream.Position < position8 + num11)
			{
				reader.BaseStream.Seek(position8 + num11, SeekOrigin.Begin);
			}
			else if (reader.BaseStream.Position > position8 + num11)
			{
				Log($"[UpdateSoundBank] Warning: Length of sound data didn't match read data! Expect further errors with this soundbank. ({num10})");
			}
		}
		if (num != 0)
		{
			writer.Write(reader.ReadBytesUntil(num4));
			writer.Flush();
			num4 = (uint)writer.BaseStream.Position;
			for (ushort num19 = 0; num19 < num; num19++)
			{
				writer.Write(reader.ReadByte());
				writer.Write(SwapEndian(flag, reader.ReadUInt32()));
			}
		}
		if (num2 != 0)
		{
			ushort num20 = 0;
			writer.Write(reader.ReadBytesUntil(num5));
			writer.Flush();
			num5 = (uint)writer.BaseStream.Position;
			for (ushort num21 = 0; num21 < num2; num21++)
			{
				byte b14 = reader.ReadByte();
				if ((b14 & 4) == 0)
				{
					num20++;
				}
				writer.Write(b14);
				writer.Write(SwapEndian(flag, reader.ReadUInt32()));
				writer.Write(SwapEndian(flag, reader.ReadUInt32()));
				writer.Write(reader.ReadByte());
				writer.Write(SwapEndian(flag, reader.ReadUInt16()));
				writer.Write(SwapEndian(flag, reader.ReadUInt16()));
				writer.Write(reader.ReadByte());
			}
			if (num20 != 0)
			{
				writer.Write(reader.ReadBytesUntil(num7));
				writer.Flush();
				num7 = (uint)writer.BaseStream.Position;
				for (ushort num22 = 0; num22 < num20; num22++)
				{
					ushort num23;
					ushort num24;
					if (b == 1)
					{
						num23 = SwapEndian(flag, reader.ReadUInt16());
						num24 = SwapEndian(flag, reader.ReadUInt16());
					}
					else
					{
						num24 = SwapEndian(flag, reader.ReadUInt16());
						num23 = SwapEndian(flag, reader.ReadUInt16());
					}
					writer.Write(num23);
					writer.Write(num24);
					writer.Write(SwapEndian(flag, reader.ReadUInt16()));
					writer.Write(SwapEndian(flag, reader.ReadUInt16()));
					switch ((num24 >> 3) & 7)
					{
					case 0:
					{
						for (ushort num28 = 0; num28 < num23; num28++)
						{
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(reader.ReadByte());
							writer.Write(reader.ReadByte());
							writer.Write(reader.ReadByte());
						}
						break;
					}
					case 1:
					{
						for (ushort num26 = 0; num26 < num23; num26++)
						{
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(reader.ReadByte());
							writer.Write(reader.ReadByte());
						}
						break;
					}
					case 3:
					{
						for (ushort num27 = 0; num27 < num23; num27++)
						{
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
							writer.Write(SwapEndian(flag, reader.ReadUInt32()));
						}
						break;
					}
					case 4:
					{
						for (ushort num25 = 0; num25 < num23; num25++)
						{
							writer.Write(SwapEndian(flag, reader.ReadUInt16()));
							writer.Write(reader.ReadByte());
						}
						break;
					}
					}
				}
			}
		}
		uint[] array = new uint[num + num2];
		writer.Write(reader.ReadBytesUntil(num6));
		writer.Flush();
		num6 = (uint)writer.BaseStream.Position;
		value = 0;
		for (int i = 0; i < array.Length; i++)
		{
			writer.Flush();
			array[i] = (uint)writer.BaseStream.Position;
			if (reader.PeekChar() != 0)
			{
				while (reader.PeekChar() != 0)
				{
					writer.Write(reader.ReadByte());
					value++;
				}
				writer.Write(reader.ReadByte());
				value++;
			}
			else
			{
				string text = $"Nameless Cue #{i}";
				value += (ushort)text.Length;
				writer.Write(text.ToCharArray());
				writer.Write(reader.ReadByte());
			}
		}
		writer.Flush();
		long position9 = writer.BaseStream.Position;
		writer.BaseStream.Seek(position, SeekOrigin.Begin);
		writer.Write(value);
		writer.Flush();
		writer.BaseStream.Seek(num8, SeekOrigin.Begin);
		for (int j = 0; j < array.Length; j++)
		{
			writer.Write(array[j]);
		}
		writer.Flush();
		writer.BaseStream.Seek(position2, SeekOrigin.Begin);
		writer.Write(num4);
		writer.Flush();
		writer.BaseStream.Seek(position3, SeekOrigin.Begin);
		writer.Write(num5);
		writer.Flush();
		writer.BaseStream.Seek(position4, SeekOrigin.Begin);
		writer.Write(num6);
		writer.Flush();
		writer.BaseStream.Seek(position5, SeekOrigin.Begin);
		writer.Write(num7);
		writer.Flush();
		writer.BaseStream.Seek(position6, SeekOrigin.Begin);
		writer.Write(value2);
		writer.Flush();
		writer.BaseStream.Seek(position7, SeekOrigin.Begin);
		writer.Write(num8);
		writer.Flush();
		writer.BaseStream.Seek(position9, SeekOrigin.Begin);
		reader.BaseStream.CopyTo(writer.BaseStream);
	}

	public static void UpdateXACTSettings(string path, BinaryReader reader, BinaryWriter writer)
	{
		Log($"[UpdateXACTSettings] Updating XACT global settings {path}");
		bool flag = reader.ReadUInt32() == 1481069382;
		writer.Write(1179862872u);
		writer.Write(SwapEndian(flag, reader.ReadUInt16()));
		writer.Write(SwapEndian(flag, reader.ReadUInt16()));
		writer.Write(SwapEndian(flag, reader.ReadUInt16()));
		writer.Write(SwapEndian(flag, reader.ReadUInt64()));
		writer.Write(reader.ReadByte());
		ushort num = SwapEndian(flag, reader.ReadUInt16());
		writer.Write(num);
		if (!flag)
		{
			writer.Write(reader.ReadBytes(12));
			uint num2 = reader.ReadUInt32();
			writer.Write(num2);
			writer.Write(reader.ReadBytesUntil(num2));
			for (int i = 0; i < num; i++)
			{
				writer.Write(reader.ReadBytes(5));
				byte b = reader.ReadByte();
				CrossfadeType crossfadeType = (CrossfadeType)(b & 7);
				if (crossfadeType != CrossfadeType.Linear)
				{
					Log($"[UpdateXACTSettings] Category #{i + 1} uses unsupported crossfade type {Enum.GetName(typeof(CrossfadeType), crossfadeType)} ({(byte)crossfadeType}) - replacing with Linear");
				}
				writer.Write((byte)((b & -8) | 0));
				writer.Write(reader.ReadBytes(4));
			}
		}
		else
		{
			ushort num3 = SwapEndian(flag, reader.ReadUInt16());
			writer.Write(num3);
			writer.Write(SwapEndian(flag, reader.ReadUInt16()));
			writer.Write(SwapEndian(flag, reader.ReadUInt16()));
			ushort num4 = SwapEndian(flag, reader.ReadUInt16());
			writer.Write(num4);
			ushort num5 = SwapEndian(flag, reader.ReadUInt16());
			writer.Write(num5);
			ushort num6 = SwapEndian(flag, reader.ReadUInt16());
			writer.Write(num6);
			uint num7 = SwapEndian(flag, reader.ReadUInt32());
			writer.Write(num7);
			uint num8 = SwapEndian(flag, reader.ReadUInt32());
			writer.Write(num8);
			writer.Write(SwapEndian(flag, reader.ReadUInt32()));
			writer.Write(SwapEndian(flag, reader.ReadUInt32()));
			writer.Write(SwapEndian(flag, reader.ReadUInt32()));
			writer.Write(SwapEndian(flag, reader.ReadUInt32()));
			writer.Write(SwapEndian(flag, reader.ReadUInt32()));
			writer.Write(SwapEndian(flag, reader.ReadUInt32()));
			uint num9 = SwapEndian(flag, reader.ReadUInt32());
			writer.Write(num9);
			uint num10 = SwapEndian(flag, reader.ReadUInt32());
			writer.Write(num10);
			uint num11 = SwapEndian(flag, reader.ReadUInt32());
			writer.Write(num11);
			writer.Write(reader.ReadBytesUntil(num7));
			for (int j = 0; j < num; j++)
			{
				writer.Write(reader.ReadByte());
				writer.Write(SwapEndian(flag, reader.ReadUInt16()));
				writer.Write(SwapEndian(flag, reader.ReadUInt16()));
				byte b2 = reader.ReadByte();
				CrossfadeType crossfadeType2 = (CrossfadeType)(b2 & 7);
				if (crossfadeType2 != CrossfadeType.Linear)
				{
					Log($"[UpdateXACTSettings] Category #{j + 1} uses unsupported crossfade type {Enum.GetName(typeof(CrossfadeType), crossfadeType2)} ({(byte)crossfadeType2}) - replacing with Linear");
				}
				writer.Write((byte)((b2 & -8) | 0));
				writer.Write(SwapEndian(flag, reader.ReadUInt16()));
				writer.Write(reader.ReadBytes(2));
			}
			if (num8 != uint.MaxValue)
			{
				writer.Write(reader.ReadBytesUntil(num8));
				for (int k = 0; k < num3; k++)
				{
					writer.Write(reader.ReadByte());
					writer.Write(SwapEndian(flag, reader.ReadUInt32()));
					writer.Write(SwapEndian(flag, reader.ReadUInt32()));
					writer.Write(SwapEndian(flag, reader.ReadUInt32()));
				}
			}
			if (num9 != uint.MaxValue)
			{
				writer.Write(reader.ReadBytesUntil(num9));
				for (int l = 0; l < num4; l++)
				{
					writer.Write(SwapEndian(flag, reader.ReadUInt16()));
					byte b3 = reader.ReadByte();
					writer.Write(b3);
					writer.Write(SwapEndian(flag, reader.ReadUInt16()));
					for (int m = 0; m < b3; m++)
					{
						writer.Write(SwapEndian(flag, reader.ReadUInt32()));
						writer.Write(SwapEndian(flag, reader.ReadUInt32()));
						writer.Write(reader.ReadByte());
					}
				}
			}
			if (num10 < num11)
			{
				writer.Write(reader.ReadBytesUntil(num10));
				for (int n = 0; n < num5; n++)
				{
					writer.Write(reader.ReadByte());
					writer.Write(SwapEndian(flag, reader.ReadUInt16()));
					writer.Write(SwapEndian(flag, reader.ReadUInt16()));
				}
			}
			if (num11 != uint.MaxValue)
			{
				writer.Write(reader.ReadBytesUntil(num11));
				for (int num12 = 0; num12 < num6; num12++)
				{
					writer.Write(reader.ReadByte());
					writer.Write(SwapEndian(flag, reader.ReadUInt32()));
					writer.Write(SwapEndian(flag, reader.ReadUInt32()));
					writer.Write(SwapEndian(flag, reader.ReadUInt32()));
					writer.Write(SwapEndian(flag, reader.ReadUInt16()));
				}
			}
			if (num10 > num11 && num10 != uint.MaxValue)
			{
				writer.Write(reader.ReadBytesUntil(num10));
				for (int num13 = 0; num13 < num5; num13++)
				{
					writer.Write(reader.ReadByte());
					writer.Write(SwapEndian(flag, reader.ReadUInt32()));
				}
			}
		}
		reader.BaseStream.CopyTo(writer.BaseStream);
	}

	public static void Log(string txt)
	{
		Console.Write("[XnaToFna] [ContentHelper] ");
		Console.WriteLine(txt);
	}

	public static void UpdateContent(string path, bool patchXNB = true, bool patchXACT = true, bool patchWindowsMedia = true)
	{
		if (patchXNB && path.EndsWith(".xnb"))
		{
			TransformContent(path);
		}
		else if (patchXACT && path.EndsWith(".xwb"))
		{
			PatchContent(path, UpdateWaveBank);
		}
		else if (patchXACT && path.EndsWith(".xsb"))
		{
			PatchContent(path, UpdateSoundBank);
		}
		else if (patchXACT && path.EndsWith(".xgs"))
		{
			PatchContent(path, UpdateXACTSettings);
		}
		else if (patchWindowsMedia && path.EndsWith(".wmv"))
		{
			UpdateVideo(path);
		}
		else if (patchWindowsMedia && path.EndsWith(".wma"))
		{
			UpdateAudio(path);
		}
	}

	public static void PatchContent(string path, Action<string, BinaryReader, BinaryWriter> patcher, bool writeToTmp = true, string pathOutput = null)
	{
		pathOutput = pathOutput ?? path;
		if (writeToTmp)
		{
			File.Delete(path + ".tmp");
		}
		if (pathOutput != path)
		{
			File.Delete(pathOutput);
		}
		using (Stream input = File.OpenRead(path))
		{
			using BinaryReader arg = new BinaryReader(input);
			if (writeToTmp)
			{
				using Stream output = File.OpenWrite(path + ".tmp");
				using BinaryWriter arg2 = new BinaryWriter(output);
				patcher(path, arg, arg2);
			}
			else
			{
				patcher(path, arg, null);
			}
		}
		if (writeToTmp)
		{
			if (pathOutput == path)
			{
				File.Delete(path);
			}
			File.Move(path + ".tmp", pathOutput);
		}
	}

	public static byte[] SwapEndian(bool swap, byte[] data)
	{
		if (!swap)
		{
			return data;
		}
		for (int num = data.Length / 2; num > -1; num--)
		{
			int num2 = data.Length - 1 - num;
			byte b = data[num];
			data[num] = data[num2];
			data[num2] = b;
		}
		return data;
	}

	public static ushort SwapEndian(bool swap, ushort data)
	{
		if (!swap)
		{
			return data;
		}
		return (ushort)((ushort)((data & 0xFF) << 8) | (ushort)((data >> 8) & 0xFF));
	}

	public static uint SwapEndian(bool swap, uint data)
	{
		if (!swap)
		{
			return data;
		}
		return ((data & 0xFF) << 24) | (((data >> 8) & 0xFF) << 16) | (((data >> 16) & 0xFF) << 8) | ((data >> 24) & 0xFF);
	}

	public static ulong SwapEndian(bool swap, ulong data)
	{
		if (!swap)
		{
			return data;
		}
		return ((data & 0xFF) << 56) | (((data >> 8) & 0xFF) << 48) | (((data >> 16) & 0xFF) << 40) | (((data >> 24) & 0xFF) << 32) | (((data >> 32) & 0xFF) << 24) | (((data >> 40) & 0xFF) << 16) | (((data >> 48) & 0xFF) << 8) | ((data >> 56) & 0xFF);
	}

	public static void RunFFMPEG(string args, Stream input, Stream output, Action<Process> feeder = null, long inputLength = 0L)
	{
		Process ffmpeg = new Process();
		ffmpeg.StartInfo = new ProcessStartInfo
		{
			FileName = "ffmpeg",
			Arguments = args,
			UseShellExecute = false,
			RedirectStandardOutput = true,
			RedirectStandardInput = true,
			RedirectStandardError = true
		};
		ffmpeg.Start();
		ffmpeg.AsyncPipeErr();
		((input == null) ? null : new Thread((feeder != null) ? ((ThreadStart)delegate
		{
			feeder(ffmpeg);
		}) : ((inputLength == 0L) ? ((ThreadStart)delegate
		{
			input.CopyTo(ffmpeg.StandardInput.BaseStream);
			ffmpeg.StandardInput.BaseStream.Flush();
			ffmpeg.StandardInput.BaseStream.Close();
		}) : ((ThreadStart)delegate
		{
			byte[] array2 = new byte[4096];
			Stream baseStream2 = ffmpeg.StandardInput.BaseStream;
			long num = 0L;
			while (!ffmpeg.HasExited && num < inputLength)
			{
				int count2;
				num += (count2 = input.Read(array2, 0, Math.Min(array2.Length, (int)(inputLength - num))));
				baseStream2.Write(array2, 0, count2);
				baseStream2.Flush();
			}
			baseStream2.Close();
		})))
		{
			IsBackground = true
		})?.Start();
		if (output == null)
		{
			ffmpeg.AsyncPipeOut();
			ffmpeg.WaitForExit();
			return;
		}
		Stream baseStream = ffmpeg.StandardOutput.BaseStream;
		byte[] array = new byte[1024];
		int count;
		while ((count = baseStream.Read(array, 0, array.Length)) > 0)
		{
			output.Write(array, 0, count);
		}
	}

	public static void UpdateWaveBank(string path, BinaryReader reader, BinaryWriter writer)
	{
		if (!IsFFMPEGAvailable)
		{
			Log("[UpdateWaveBank] FFMPEG is missing - won't convert unsupported WaveBanks");
			reader.BaseStream.CopyTo(writer.BaseStream);
			return;
		}
		Log($"[UpdateWaveBank] Updating wave bank {path}");
		bool flag = reader.ReadUInt32() == 1463963204;
		writer.Write(1145979479u);
		writer.Write(SwapEndian(flag, reader.ReadUInt32()));
		writer.Write(SwapEndian(flag, reader.ReadUInt32()));
		uint[] array = new uint[5];
		uint[] array2 = new uint[5];
		long position = reader.BaseStream.Position;
		for (int i = 0; i < 5; i++)
		{
			array[i] = SwapEndian(flag, reader.ReadUInt32());
			writer.Write(array[i]);
			array2[i] = SwapEndian(flag, reader.ReadUInt32());
			writer.Write(array2[i]);
		}
		writer.Write(reader.ReadBytesUntil(array[0]));
		uint num = SwapEndian(flag, reader.ReadUInt32());
		writer.Write(num);
		if ((num & 2) == 2)
		{
			if (flag)
			{
				throw new InvalidDataException("Can't handle compact mode Xbox 360 wave banks - Content directory left in unstable state");
			}
			reader.BaseStream.CopyTo(writer.BaseStream);
			return;
		}
		uint num2 = SwapEndian(flag, reader.ReadUInt32());
		writer.Write(num2);
		writer.Write(reader.ReadBytes(64));
		uint num3 = SwapEndian(flag, reader.ReadUInt32());
		writer.Write(num3);
		writer.Write(SwapEndian(flag, reader.ReadUInt32()));
		writer.Write(SwapEndian(flag, reader.ReadUInt32()));
		uint num4 = array[4];
		if (num4 == 0)
		{
			num4 = array[1] + num2 * num3;
		}
		uint[] array3 = new uint[num2];
		long[] array4 = new long[num2];
		uint[] array5 = new uint[num2];
		uint[] array6 = new uint[num2];
		long[] array7 = new long[num2];
		int[] array8 = new int[num2];
		int[] array9 = new int[num2];
		long[] array10 = new long[num2];
		uint[] array11 = new uint[num2];
		uint[] array12 = new uint[num2];
		uint[] array13 = new uint[num2];
		uint[] array14 = new uint[num2];
		uint[] array15 = new uint[num2];
		uint num5 = array[1];
		uint num6 = 0u;
		for (int j = 0; j < num2; j++)
		{
			writer.Write(reader.ReadBytesUntil(num5));
			if (num3 >= 4)
			{
				uint num7 = SwapEndian(flag, reader.ReadUInt32());
				writer.Write(num7);
				array3[j] = num7 >> 4;
			}
			if (num3 >= 8)
			{
				array10[j] = reader.BaseStream.Position;
				writer.Write(num6 = SwapEndian(flag, reader.ReadUInt32()));
			}
			if (num3 >= 12)
			{
				array4[j] = reader.BaseStream.Position;
				writer.Write(array5[j] = (array6[j] = SwapEndian(flag, reader.ReadUInt32())));
			}
			if (num3 >= 16)
			{
				array7[j] = reader.BaseStream.Position;
				writer.Write((uint)(array8[j] = (array9[j] = (int)SwapEndian(flag, reader.ReadUInt32()))));
			}
			if (num3 >= 20)
			{
				writer.Write(SwapEndian(flag, reader.ReadUInt32()));
			}
			if (num3 >= 24)
			{
				writer.Write(SwapEndian(flag, reader.ReadUInt32()));
			}
			else if (array8[j] != 0)
			{
				array8[j] = (int)array2[4];
			}
			num5 += num3;
			array5[j] += num4;
			array11[j] = num6 & 3;
			array12[j] = (num6 >> 2) & 7;
			array13[j] = (num6 >> 5) & 0x3FFFF;
			array14[j] = (num6 >> 23) & 0xFF;
			array15[j] = num6 >> 31;
		}
		uint[][] array16 = new uint[num2][];
		if ((num & 0x80000) == 524288)
		{
			writer.Write(reader.ReadBytesUntil(array[2]));
			uint[] array17 = new uint[num2];
			for (int k = 0; k < num2; k++)
			{
				array17[k] = SwapEndian(flag, reader.ReadUInt32());
				writer.Write(array17[k]);
			}
			writer.Flush();
			num5 = (uint)writer.BaseStream.Position;
			for (int l = 0; l < num2; l++)
			{
				writer.Write(reader.ReadBytesUntil(num5 + array17[l]));
				uint num8 = SwapEndian(flag, reader.ReadUInt32());
				writer.Write(num8);
				uint[] array18 = (array16[l] = new uint[num8]);
				for (int m = 0; m < num8; m++)
				{
					array18[m] = SwapEndian(flag, reader.ReadUInt32());
					writer.Write(array18[m]);
				}
			}
		}
		for (int n = 0; n < num2; n++)
		{
			writer.Write(reader.ReadBytesUntil(array5[n]));
			if (array11[n] != 1 && array11[n] != 3)
			{
				writer.Write(reader.ReadBytes(array8[n]));
				continue;
			}
			writer.Flush();
			num5 = (uint)writer.BaseStream.Position;
			Action<Process> feeder = null;
			if (array11[n] == 3)
			{
				feeder = GenerateXWMAFeeder(reader, array14[n], array8[n], array3[n], array12[n], array13[n]);
			}
			else if (array11[n] == 1)
			{
				feeder = GenerateXMA2Feeder(reader, array14[n], array8[n], array3[n], array12[n], array13[n], array16[n]);
			}
			Log($"[UpdateWaveBank] Converting #{n}");
			ConvertAudio(reader.BaseStream, writer.BaseStream, feeder, array8[n]);
			array12[n] = 1u;
			writer.Flush();
			uint num9 = (uint)(int)writer.BaseStream.Position - num5;
			num5 = (uint)writer.BaseStream.Position;
			uint num10 = num9 - (uint)array8[n];
			array11[n] = 0u;
			array15[n] = 0u;
			array14[n] = 0u;
			if (array10[n] != 0L)
			{
				writer.Flush();
				writer.BaseStream.Seek(array10[n], SeekOrigin.Begin);
				writer.Write((array11[n] & 3) | ((array12[n] & 7) << 2) | ((array13[n] & 0x3FFFF) << 5) | ((array14[n] & 0xFF) << 23) | (array15[n] << 31));
			}
			if (array7[n] != 0L)
			{
				writer.Flush();
				writer.BaseStream.Seek(array7[n], SeekOrigin.Begin);
				writer.Write(array9[n] = (int)num9);
			}
			for (int num11 = n + 1; num11 < num2; num11++)
			{
				if (array4[num11] != 0L)
				{
					writer.Flush();
					writer.BaseStream.Seek(array4[num11], SeekOrigin.Begin);
					writer.Write(array6[num11] += num10);
				}
			}
			writer.Flush();
			writer.BaseStream.Seek(num5, SeekOrigin.Begin);
		}
		writer.Flush();
		num5 = (uint)writer.BaseStream.Position;
		array2[4] = num5 - array[4];
		writer.Flush();
		writer.BaseStream.Seek(position, SeekOrigin.Begin);
		for (int num12 = 0; num12 < 5; num12++)
		{
			writer.Write(array[num12]);
			writer.Write(array2[num12]);
		}
		writer.Flush();
		writer.BaseStream.Seek(num5, SeekOrigin.Begin);
		reader.BaseStream.CopyTo(writer.BaseStream);
	}

	public static void ConvertAudio(Stream input, Stream output, Action<Process> feeder, long length)
	{
		RunFFMPEG("-y -i - -f u8 -ac 1 -", input, output, feeder, length);
	}

	public static Action<Process> GenerateSoundEffectFeeder(BinaryReader reader, string format, uint fmtLength, uint dataLength, uint extraLength, bool x360 = false, Action<BinaryWriter> fmtExtraWriter = null)
	{
		return delegate(Process ffmpeg)
		{
			Stream baseStream = ffmpeg.StandardInput.BaseStream;
			using (BinaryWriter binaryWriter = new BinaryWriter(baseStream, Encoding.ASCII, leaveOpen: true))
			{
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(dataLength + 4 + 4 + 8 + 4 + fmtLength + extraLength + 4 + 4 - 8);
				binaryWriter.Write(format.ToCharArray());
				binaryWriter.Write("fmt ".ToCharArray());
				reader.ReadUInt32();
				binaryWriter.Write(fmtLength);
				binaryWriter.Write(SwapEndian(x360, reader.ReadUInt16()));
				binaryWriter.Write(SwapEndian(x360, reader.ReadUInt16()));
				binaryWriter.Write(SwapEndian(x360, reader.ReadUInt32()));
				binaryWriter.Write(SwapEndian(x360, reader.ReadUInt32()));
				binaryWriter.Write(SwapEndian(x360, reader.ReadUInt16()));
				binaryWriter.Write(SwapEndian(x360, reader.ReadUInt16()));
				if (fmtExtraWriter == null)
				{
					ushort num = SwapEndian(x360, reader.ReadUInt16());
					binaryWriter.Write(num);
					binaryWriter.Write(reader.ReadBytes(num));
				}
				else
				{
					fmtExtraWriter?.Invoke(binaryWriter);
				}
				binaryWriter.Write("data".ToCharArray());
				reader.ReadUInt32();
				binaryWriter.Write(dataLength);
				binaryWriter.Flush();
			}
			byte[] array = new byte[4096];
			long num2 = reader.BaseStream.Position + dataLength;
			while (!ffmpeg.HasExited && reader.BaseStream.Position < num2)
			{
				int count = reader.BaseStream.Read(array, 0, Math.Min(array.Length, (int)(num2 - reader.BaseStream.Position)));
				baseStream.Write(array, 0, count);
				baseStream.Flush();
			}
			baseStream.Close();
		};
	}

	public static Action<Process> GenerateXWMAFeeder(BinaryReader reader, uint align, int playLength, uint duration, uint channels, uint rate)
	{
		return delegate(Process ffmpeg)
		{
			Stream baseStream = ffmpeg.StandardInput.BaseStream;
			using (BinaryWriter binaryWriter = new BinaryWriter(baseStream, Encoding.ASCII, leaveOpen: true))
			{
				short num = ((align >= XWMAInfo.BlockAlign.Length) ? XWMAInfo.BlockAlign[align & 0xF] : XWMAInfo.BlockAlign[align]);
				int num2 = playLength / num;
				int num3 = (int)Math.Ceiling((double)duration / 2048.0);
				int num4 = num3 / num2;
				int num5 = num3 - num4 * num2;
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(playLength + 4 + 4 + 8 + 4 + 2 + 2 + 4 + 4 + 2 + 2 + 2 + 4 + 4 + num2 * 4 + 4 + 4 - 8);
				binaryWriter.Write("XWMAfmt ".ToCharArray());
				binaryWriter.Write(18);
				binaryWriter.Write((short)353);
				binaryWriter.Write((short)channels);
				binaryWriter.Write(rate);
				binaryWriter.Write((align >= XWMAInfo.BytesPerSecond.Length) ? XWMAInfo.BytesPerSecond[align >> 5] : XWMAInfo.BytesPerSecond[align]);
				binaryWriter.Write(num);
				binaryWriter.Write((short)15);
				binaryWriter.Write((short)0);
				binaryWriter.Write("dpds".ToCharArray());
				binaryWriter.Write(num2 * 4);
				int i = 0;
				int num6 = 0;
				for (; i < num2; i++)
				{
					num6 += num4 * 4096;
					if (num5 > 0)
					{
						num6 += 4096;
						num5--;
					}
					binaryWriter.Write(num6);
				}
				binaryWriter.Write("data".ToCharArray());
				binaryWriter.Write(playLength);
				binaryWriter.Flush();
			}
			byte[] array = new byte[4096];
			long num7 = reader.BaseStream.Position + playLength;
			while (!ffmpeg.HasExited && reader.BaseStream.Position < num7)
			{
				int count = reader.BaseStream.Read(array, 0, Math.Min(array.Length, (int)(num7 - reader.BaseStream.Position)));
				baseStream.Write(array, 0, count);
				baseStream.Flush();
			}
			baseStream.Close();
		};
	}

	public static Action<Process> GenerateXMA2Feeder(BinaryReader reader, uint align, int playLength, uint duration, uint channels, uint rate, uint[] seekData)
	{
		return delegate(Process ffmpeg)
		{
			Stream baseStream = ffmpeg.StandardInput.BaseStream;
			using (BinaryWriter binaryWriter = new BinaryWriter(baseStream, Encoding.ASCII, leaveOpen: true))
			{
				binaryWriter.Write("RIFF".ToCharArray());
				binaryWriter.Write(playLength + 4 + 4 + 8 + 4 + 2 + 2 + 4 + 4 + 2 + 2 + 2 + 2 + 4 + 24 + 1 + 1 + 2 + 4 + 4 + seekData.Length * 4 + 4 + 4 - 8);
				binaryWriter.Write("WAVEfmt ".ToCharArray());
				binaryWriter.Write(52);
				binaryWriter.Write((short)358);
				binaryWriter.Write((short)channels);
				binaryWriter.Write(rate);
				binaryWriter.Write((align >= XMAInfo.BytesPerSecond.Length) ? XMAInfo.BytesPerSecond[align >> 5] : XMAInfo.BytesPerSecond[align]);
				binaryWriter.Write((align >= XMAInfo.BlockAlign.Length) ? XMAInfo.BlockAlign[align & 0xF] : XMAInfo.BlockAlign[align]);
				binaryWriter.Write((short)15);
				binaryWriter.Write((short)34);
				binaryWriter.Write((short)1);
				binaryWriter.Write((channels == 2) ? 3u : 0u);
				binaryWriter.Write(0u);
				binaryWriter.Write(0u);
				binaryWriter.Write(0u);
				binaryWriter.Write(0u);
				binaryWriter.Write(0u);
				binaryWriter.Write(0u);
				binaryWriter.Write((byte)0);
				binaryWriter.Write((byte)4);
				binaryWriter.Write((short)1);
				binaryWriter.Write("seek".ToCharArray());
				binaryWriter.Write(seekData.Length * 4);
				for (int i = 0; i < seekData.Length; i++)
				{
					binaryWriter.Write(seekData[i]);
				}
				binaryWriter.Write("data".ToCharArray());
				binaryWriter.Write(playLength);
				binaryWriter.Flush();
			}
			byte[] array = new byte[4096];
			long num = reader.BaseStream.Position + playLength;
			while (!ffmpeg.HasExited && reader.BaseStream.Position < num)
			{
				int count = reader.BaseStream.Read(array, 0, Math.Min(array.Length, (int)(num - reader.BaseStream.Position)));
				baseStream.Write(array, 0, count);
				baseStream.Flush();
			}
			baseStream.Close();
		};
	}
}
