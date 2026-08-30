using System;
using System.Collections.Generic;
using System.IO;
using SheetEdit.TextureSheet;

namespace xCharEdit.Character;

public class CharDef
{
	public enum DefType
	{
		Charlie,
		Lester,
		Tommy,
		Rex,
		Kelly,
		Zombie,
		Ninja,
		Fatty,
		Tepes,
		Hockey,
		Baal,
		Babe,
		Circe,
		Mortimer
	}

	private Animation[] animation;

	private Frame[] frame;

	public string path;

	public string texName = "";

	public List<string> tex;

	public int texVars;

	public CharDef()
	{
		animation = new Animation[128];
		for (int i = 0; i < animation.Length; i++)
		{
			animation[i] = new Animation();
		}
		frame = new Frame[2048];
		for (int j = 0; j < frame.Length; j++)
		{
			frame[j] = new Frame();
		}
		path = "char";
	}

	public CharDef(string path, Dictionary<string, XTexture> textures)
	{
		animation = new Animation[128];
		frame = new Frame[2048];
		this.path = path;
		ReadShort(abs: true);
		tex = new List<string>();
		tex.Add(texName);
		int num = 1;
		texVars = 1;
		while (true)
		{
			num++;
			if (textures.ContainsKey(texName + num))
			{
				tex.Add(texName + num);
				texVars = num;
				continue;
			}
			break;
		}
	}

	public Animation GetAnimation(int idx)
	{
		return animation[idx];
	}

	public void SetAnimation(int idx, Animation _animation)
	{
		animation[idx] = _animation;
	}

	public Animation[] GetAnimationArray()
	{
		return animation;
	}

	public Frame GetFrame(int idx)
	{
		if (idx < 0)
		{
			idx = 0;
		}
		return frame[idx];
	}

	public void SetFrame(int idx, Frame _frame)
	{
		frame[idx] = _frame;
	}

	public Frame[] GetFrameArray()
	{
		return frame;
	}

	public void WriteBackup()
	{
		Write("data/" + path + ".zmx");
		Write("data/" + path + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + "-" + DateTime.Now.Year + "-" + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".zmx");
	}

	public void Write()
	{
		Write("data/" + path + ".zmx");
	}

	public void Write(string writePath)
	{
		BinaryWriter binaryWriter = new BinaryWriter(File.Open(writePath, FileMode.Create));
		binaryWriter.Write(path);
		binaryWriter.Write(texName);
		for (int i = 0; i < animation.Length; i++)
		{
			binaryWriter.Write(animation[i].name);
			if (!(animation[i].name != ""))
			{
				continue;
			}
			int num = 0;
			for (int j = 0; j < animation[i].getKeyFrameArray().Length; j++)
			{
				if (animation[i].GetKeyFrame(j).frameRef < 0)
				{
					num = j;
					break;
				}
			}
			binaryWriter.Write(num);
			for (int k = 0; k < num; k++)
			{
				KeyFrame keyFrame = animation[i].GetKeyFrame(k);
				binaryWriter.Write(keyFrame.frameRef);
				binaryWriter.Write(keyFrame.duration);
				binaryWriter.Write(keyFrame.lerp);
				string[] scriptArray = keyFrame.getScriptArray();
				byte b = (byte)keyFrame.GetScriptLength();
				binaryWriter.Write(b);
				for (byte b2 = 0; b2 < b; b2++)
				{
					binaryWriter.Write(scriptArray[b2]);
				}
			}
		}
		for (int l = 0; l < frame.Length; l++)
		{
			binaryWriter.Write(frame[l].name);
			if (frame[l].name != "")
			{
				for (int m = 0; m < frame[l].GetPartArray().Length; m++)
				{
					Part part = frame[l].GetPart(m);
					binaryWriter.Write(part.idx);
					binaryWriter.Write(part.location.X);
					binaryWriter.Write(part.location.Y);
					binaryWriter.Write(part.rotation);
					binaryWriter.Write(part.scaling.X);
					binaryWriter.Write(part.scaling.Y);
					binaryWriter.Write(part.flip);
				}
			}
		}
		binaryWriter.Close();
		Console.WriteLine("Saved: " + writePath);
	}

	public void WriteShort(string writePath)
	{
		BinaryWriter binaryWriter = new BinaryWriter(File.Open(writePath, FileMode.Create));
		binaryWriter.Write(path);
		binaryWriter.Write(texName);
		int num = 0;
		for (int i = 0; i < animation.Length; i++)
		{
			if (animation[i].name != "")
			{
				num++;
			}
		}
		binaryWriter.Write(num);
		for (int j = 0; j < animation.Length; j++)
		{
			if (!(animation[j].name != ""))
			{
				continue;
			}
			binaryWriter.Write(animation[j].name);
			int num2 = 0;
			for (int k = 0; k < animation[j].getKeyFrameArray().Length; k++)
			{
				if (animation[j].GetKeyFrame(k).frameRef < 0)
				{
					num2 = k;
					break;
				}
			}
			binaryWriter.Write(num2);
			for (int l = 0; l < num2; l++)
			{
				KeyFrame keyFrame = animation[j].GetKeyFrame(l);
				binaryWriter.Write(keyFrame.frameRef);
				binaryWriter.Write(keyFrame.duration);
				binaryWriter.Write(keyFrame.lerp);
				string[] scriptArray = keyFrame.getScriptArray();
				byte b = (byte)keyFrame.GetScriptLength();
				binaryWriter.Write(b);
				for (byte b2 = 0; b2 < b; b2++)
				{
					binaryWriter.Write(scriptArray[b2]);
				}
			}
		}
		for (int m = 0; m < frame.Length; m++)
		{
			if (frame[m].name != "")
			{
				binaryWriter.Write(value: true);
			}
			else
			{
				binaryWriter.Write(value: false);
			}
			if (!(frame[m].name != ""))
			{
				continue;
			}
			int num3 = 0;
			for (int n = 0; n < frame[m].GetPartArray().Length; n++)
			{
				if (frame[m].GetPart(n).idx > -1)
				{
					num3++;
				}
			}
			binaryWriter.Write(num3);
			for (int num4 = 0; num4 < frame[m].GetPartArray().Length; num4++)
			{
				if (frame[m].GetPart(num4).idx > -1)
				{
					Part part = frame[m].GetPart(num4);
					binaryWriter.Write(part.idx);
					binaryWriter.Write(part.location.X);
					binaryWriter.Write(part.location.Y);
					binaryWriter.Write(part.rotation);
					binaryWriter.Write(part.scaling.X);
					binaryWriter.Write(part.scaling.Y);
					binaryWriter.Write(part.flip);
				}
			}
		}
		binaryWriter.Close();
		Console.WriteLine("Saved: " + writePath);
	}

	public void ReadShort(bool abs)
	{
		BinaryReader binaryReader = (abs ? new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)) : new BinaryReader(File.Open("data/" + path + ".zmx", FileMode.Open, FileAccess.Read)));
		path = binaryReader.ReadString();
		texName = binaryReader.ReadString();
		int num = binaryReader.ReadInt32();
		for (int i = 0; i < num; i++)
		{
			animation[i] = new Animation();
			animation[i].name = binaryReader.ReadString();
			if (!(animation[i].name != ""))
			{
				continue;
			}
			int num2 = binaryReader.ReadInt32();
			for (int j = 0; j < num2; j++)
			{
				animation[i].getKeyFrameArray()[j] = new KeyFrame();
				KeyFrame keyFrame = animation[i].GetKeyFrame(j);
				keyFrame.frameRef = binaryReader.ReadInt32();
				keyFrame.duration = binaryReader.ReadInt32();
				keyFrame.lerp = binaryReader.ReadBoolean();
				keyFrame.getScriptArray();
				byte b = binaryReader.ReadByte();
				for (byte b2 = 0; b2 < b; b2++)
				{
					keyFrame.SetScript(b2, binaryReader.ReadString());
				}
			}
		}
		for (int k = 0; k < frame.Length; k++)
		{
			if (binaryReader.ReadBoolean())
			{
				frame[k] = new Frame();
				frame[k].parts = binaryReader.ReadInt32();
				for (int l = 0; l < frame[k].parts; l++)
				{
					Part part = frame[k].GetPart(l);
					part.idx = binaryReader.ReadInt32();
					part.location.X = binaryReader.ReadSingle();
					part.location.Y = binaryReader.ReadSingle();
					part.rotation = binaryReader.ReadSingle();
					part.scaling.X = binaryReader.ReadSingle();
					part.scaling.Y = binaryReader.ReadSingle();
					part.flip = binaryReader.ReadInt32();
				}
			}
		}
		binaryReader.Close();
	}

	public void Read()
	{
		Read(abs: false);
	}

	public void Read(bool abs)
	{
		BinaryReader binaryReader = (abs ? new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read)) : new BinaryReader(File.Open("data/" + path + ".zmx", FileMode.Open, FileAccess.Read)));
		path = binaryReader.ReadString();
		texName = binaryReader.ReadString();
		for (int i = 0; i < animation.Length; i++)
		{
			animation[i] = new Animation();
			animation[i].name = binaryReader.ReadString();
			if (!(animation[i].name != ""))
			{
				continue;
			}
			int num = binaryReader.ReadInt32();
			for (int j = 0; j < num; j++)
			{
				KeyFrame keyFrame = animation[i].GetKeyFrame(j);
				keyFrame.frameRef = binaryReader.ReadInt32();
				keyFrame.duration = binaryReader.ReadInt32();
				keyFrame.lerp = binaryReader.ReadBoolean();
				keyFrame.getScriptArray();
				byte b = binaryReader.ReadByte();
				for (byte b2 = 0; b2 < b; b2++)
				{
					keyFrame.SetScript(b2, binaryReader.ReadString());
				}
			}
			for (int k = num; k < animation[i].getKeyFrameArray().Length; k++)
			{
				animation[i].ClearKey(k);
			}
		}
		for (int l = 0; l < frame.Length; l++)
		{
			frame[l] = new Frame();
			frame[l].name = binaryReader.ReadString();
			if (frame[l].name != "")
			{
				for (int m = 0; m < frame[l].GetPartArray().Length; m++)
				{
					Part part = frame[l].GetPart(m);
					part.idx = binaryReader.ReadInt32();
					part.location.X = binaryReader.ReadSingle();
					part.location.Y = binaryReader.ReadSingle();
					part.rotation = binaryReader.ReadSingle();
					part.scaling.X = binaryReader.ReadSingle();
					part.scaling.Y = binaryReader.ReadSingle();
					part.flip = binaryReader.ReadInt32();
				}
			}
		}
		binaryReader.Close();
	}
}
