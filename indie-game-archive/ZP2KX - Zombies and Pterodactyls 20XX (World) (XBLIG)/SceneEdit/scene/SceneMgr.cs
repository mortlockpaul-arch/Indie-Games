using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9;

namespace SceneEdit.scene;

public class SceneMgr
{
	public Dictionary<string, Texture2D> texture;

	public Video video;

	public int curScene;

	public int selLayer;

	public int selBubble;

	public int selKeyframe;

	public string path;

	private EffectPass pass;

	private bool flicker;

	private bool spazz;

	private bool strobe;

	private bool creep;

	public bool smoothcam;

	public bool miniAdjust;

	private Vector3 camLoc;

	private Vector2 camAngle;

	private bool hasMask;

	public SceneMgr(ContentManager Content)
	{
		video = new Video();
		texture = new Dictionary<string, Texture2D>();
		DirectoryInfo directoryInfo = new DirectoryInfo("Content/gfx/scene/");
		FileInfo[] files = directoryInfo.GetFiles("*.xnb");
		FileInfo[] array = files;
		foreach (FileInfo fileInfo in array)
		{
			string key = fileInfo.Name.Substring(0, fileInfo.Name.Length - 4);
			texture.Add(key, Content.Load<Texture2D>(fileInfo.FullName.Substring(0, fileInfo.FullName.Length - 4)));
		}
		SceneCam.location.Z = 1f;
	}

	private void DrawLayers(bool mask, SpriteBatch sprite, Scene scene)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0386: Unknown result type (might be due to invalid IL or missing references)
		//IL_0388: Unknown result type (might be due to invalid IL or missing references)
		//IL_038d: Unknown result type (might be due to invalid IL or missing references)
		//IL_038f: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0426: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		//IL_0688: Unknown result type (might be due to invalid IL or missing references)
		//IL_0689: Unknown result type (might be due to invalid IL or missing references)
		//IL_0478: Unknown result type (might be due to invalid IL or missing references)
		//IL_048e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0493: Unknown result type (might be due to invalid IL or missing references)
		//IL_0498: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_0533: Unknown result type (might be due to invalid IL or missing references)
		//IL_0538: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0514: Unknown result type (might be due to invalid IL or missing references)
		//IL_0519: Unknown result type (might be due to invalid IL or missing references)
		//IL_058c: Unknown result type (might be due to invalid IL or missing references)
		//IL_05be: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0639: Unknown result type (might be due to invalid IL or missing references)
		//IL_0626: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		Vector3 location = SceneCam.location;
		foreach (Layer item in scene.layer)
		{
			if (item.keyframe.Count <= 0)
			{
				continue;
			}
			switch (item.name)
			{
			case "cam":
			case "master":
				continue;
			}
			if ((!mask || !(item.name == "mask")) && (mask || !(item.name != "mask")))
			{
				continue;
			}
			if (mask)
			{
				hasMask = true;
			}
			int num = 0;
			float num2 = 0f;
			for (int i = 0; i < item.keyframe.Count; i++)
			{
				Keyframe keyframe = item.keyframe[i];
				if (keyframe.time <= video.time && keyframe.time > num2)
				{
					num = i;
					num2 = keyframe.time;
				}
			}
			Keyframe keyframe2 = item.keyframe[num];
			if (keyframe2.texture == null)
			{
				continue;
			}
			Vector3 val = keyframe2.loc;
			if (mask)
			{
				val.Z = 0.1f;
				if (miniAdjust)
				{
					val.Z = 0.01f;
				}
			}
			Vector2 val2 = keyframe2.scale;
			float num3 = keyframe2.r;
			float num4 = keyframe2.g;
			float num5 = keyframe2.b;
			float num6 = keyframe2.a;
			float num7 = keyframe2.angle;
			if (keyframe2.tween && num < item.keyframe.Count - 1)
			{
				Keyframe keyframe3 = item.keyframe[num + 1];
				Vector3 loc = keyframe3.loc;
				if (mask)
				{
					val.Z = 0.1f;
				}
				float num8 = (video.time - keyframe2.time) / (keyframe3.time - keyframe2.time);
				val += (loc - val) * num8;
				val2 = keyframe2.scale + (keyframe3.scale - keyframe2.scale) * num8;
				num3 = keyframe2.r + (keyframe3.r - keyframe2.r) * num8;
				num4 = keyframe2.g + (keyframe3.g - keyframe2.g) * num8;
				num5 = keyframe2.b + (keyframe3.b - keyframe2.b) * num8;
				num6 = keyframe2.a + (keyframe3.a - keyframe2.a) * num8;
				num7 = keyframe2.angle + (keyframe3.angle - keyframe2.angle) * num8;
			}
			num3 *= scene.r;
			num4 *= scene.g;
			num5 *= scene.b;
			if (item.name.Length > 4)
			{
				try
				{
					if (item.name.Substring(0, 4) == "rot-")
					{
						val.X += (float)Math.Cos(video.time * 3.14f + keyframe2.loc.X + keyframe2.loc.Y) * 20f;
						val.Y += (float)Math.Sin(video.time * 3.14f + keyframe2.loc.X + keyframe2.loc.Y) * 20f;
					}
				}
				catch
				{
				}
			}
			Vector2 screenLoc = SceneCam.GetScreenLoc(val);
			Vector2 val3 = screenLoc - new Vector2(640f, 360f);
			num7 += SceneCam.rotation;
			screenLoc = new Vector2(640f, 360f) + new Vector2((float)Math.Cos(SceneCam.rotation) * val3.X, (float)Math.Sin(SceneCam.rotation) * val3.X) + new Vector2((float)Math.Cos(SceneCam.rotation + 1.57f) * val3.Y, (float)Math.Sin(SceneCam.rotation + 1.57f) * val3.Y);
			val2 *= val.Z;
			try
			{
				if (flicker)
				{
					float randomFloat = Rand.GetRandomFloat(0.5f, 1f);
					num3 *= randomFloat;
					num4 *= randomFloat;
					num5 *= randomFloat;
				}
				if (spazz)
				{
					screenLoc += Rand.GetRandomVec2(-10f, 10f, -10f, 10f);
				}
				if (item.name == "zap")
				{
					screenLoc += Rand.GetRandomVec2(-10f, 10f, -10f, 10f);
					num7 += Rand.GetRandomFloat(0f, 6.28f);
				}
				if (item.name == "mask" && miniAdjust)
				{
					val2 *= 1f + SceneCam.location.Z * 0.02f;
					num7 *= 0.3f;
				}
				else
				{
					val2 *= SceneCam.location.Z;
				}
				if (!video.playing && scene.name != video.scenes[curScene].name)
				{
					num6 /= 10f;
				}
				sprite.Draw(texture[keyframe2.texture], screenLoc, (Rectangle?)new Rectangle(0, 0, texture[keyframe2.texture].Width, texture[keyframe2.texture].Height), new Color(num3, num4, num5, num6), num7, new Vector2((float)texture[keyframe2.texture].Width / 2f, (float)texture[keyframe2.texture].Height / 2f), (Vector2)((val2.X < 0f) ? new Vector2(0f - val2.X, val2.Y) : val2), (SpriteEffects)(val2.X < 0f), 1f);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
		SceneCam.location = location;
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		if (video.scenes.Count > 0)
		{
			Scene scene = video.scenes[curScene];
			hasMask = false;
			DrawLayers(mask: false, sprite, scene);
			if (strobe && (int)(video.time * 20f) % 2 == 0)
			{
				sprite.Draw(Game1.nullTex, new Rectangle(0, 0, 1280, 720), new Color(1f, 1f, 1f, 0.8f));
			}
			if (creep)
			{
				sprite.Draw(Game1.nullTex, new Rectangle(0, 0, 1280, 720), new Color(1f, 1f, 1f, Rand.GetRandomFloat(0f, 0.2f)));
			}
			flicker = false;
			spazz = false;
			strobe = false;
			creep = false;
			smoothcam = true;
			miniAdjust = false;
		}
	}

	internal void Read(string path)
	{
		this.path = path;
		BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read));
		video.Read(binaryReader);
		binaryReader.Close();
	}

	internal void Append(string path)
	{
		this.path = path;
		BinaryReader binaryReader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read));
		video.Append(binaryReader);
		binaryReader.Close();
	}

	internal void Write(string path)
	{
		this.path = path;
		Write();
	}

	internal void Write()
	{
		BinaryWriter binaryWriter = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate, FileAccess.Write));
		video.Write(binaryWriter);
		binaryWriter.Close();
	}

	internal void Update()
	{
		video.playing = true;
		if (video.scenes.Count > 0)
		{
			SceneCam.Update(video, video.scenes[curScene], video.playing && smoothcam, Game1.frameTime);
			SceneMaster.Update(video, video.scenes[curScene]);
			if (!video.playing)
			{
				foreach (Scene scene in video.scenes)
				{
					scene.r = (scene.g = (scene.b = 1f));
				}
			}
		}
		if (!video.playing)
		{
			return;
		}
		video.time += Game1.frameTime;
		if (video.time > video.scenes[curScene].duration)
		{
			curScene++;
			selLayer = 0;
			selBubble = 0;
			selKeyframe = 0;
			if (curScene >= video.scenes.Count)
			{
				curScene = 0;
			}
			video.time = 0f;
		}
	}
}
