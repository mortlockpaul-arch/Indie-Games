using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quasar.GameUtils.Tasks;
using Quasar.GameUtils.Template;
using Quasar.Global;
using Quasar.Language;
using Quasar.Meshes;
using Quasar.Meshes.Text;
using Quasar.Shaders;
using Quasar.Textures;

namespace Quasar.GameUtils.OtherGames;

internal class OtherGamesItem : RenderItem
{
	private int index;

	private OtherGames otherGames;

	private static object lockObject = new object();

	private int lastDiffIndex = 1;

	public OtherGamesItem(OtherGames otherGames, int index)
	{
		this.index = index;
		this.otherGames = otherGames;
		OtherGames.OtherGamesItem otherGamesItem = otherGames.Items[index];
		Vector3 color = otherGamesItem.Color;
		Vector3 vector = Vector3.Lerp(color, Vector3.One, 0.5f);
		XMesh xMesh = new XMesh("GameCase/GameCaseMesh");
		xMesh.Materials[0].Ambient = new Vector3(0.5f, 1f, 0.45f);
		xMesh.Materials[0].Diffuse = new Vector3(0.5f, 1f, 0.45f);
		xMesh.Materials[0].Specular = new Vector3(1f);
		xMesh.Materials[0].Shininess = 40f;
		xMesh.Materials[1].Texture = TextureManager.Textures["OtherGames/BGPattern"];
		TaskManager.Post(LoadTexture, new KeyValuePair<string, Material>(otherGamesItem.BoxArt, xMesh.Materials[1]));
		xMesh.Materials[1].Ambient = Vector3.One;
		xMesh.Materials[1].Diffuse = Vector3.One;
		xMesh.Materials[1].Specular = new Vector3(1f);
		xMesh.Materials[1].Shininess = 40f;
		xMesh.Shader = ShaderManager.Shaders["BaseClamp"];
		for (int i = 0; i < 4; i++)
		{
			xMesh.Materials[i + 2].Texture = TextureManager.Textures["OtherGames/BGPattern"];
			TaskManager.Post(LoadTexture, new KeyValuePair<string, Material>(otherGamesItem.Screenshot(i), xMesh.Materials[i + 2]));
			xMesh.Materials[i + 2].Ambient = new Vector3(1f);
			xMesh.Materials[i + 2].Diffuse = new Vector3(1f);
			xMesh.Materials[i + 2].Specular = new Vector3(0.8f);
			xMesh.Materials[i + 2].Shininess = 40f;
		}
		addMesh(xMesh);
		RenderItem renderItem = new RenderItem();
		renderItem.Transform.Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI);
		addChild(renderItem);
		TextBoxMesh textBoxMesh = new TextBoxMesh(GameTemplate.StandardFont, new TextBoxDrawProperties(new Vector2(0f, -0.26f), new Vector2(0.75f, 0.35f), 0.001f, HorizontalAlignment.Left, VerticalAlignment.Top), 512, useStringBuilder: false);
		textBoxMesh.ZValue = 0.035f;
		textBoxMesh.Text = otherGamesItem.Description;
		textBoxMesh.Diffuse = vector;
		textBoxMesh.FirstMaterial.AlphaTest = 0.5f;
		textBoxMesh.Shader = ShaderManager.Shaders["BaseNoNormal"];
		textBoxMesh.FirstMaterial.Ambient = vector;
		textBoxMesh.FirstMaterial.Shininess = 40f;
		renderItem.addMesh(textBoxMesh);
		TextMesh textMesh = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-0.375f, 0f), 0.002f, HorizontalAlignment.Left), 64, useStringBuilder: false);
		textMesh.ZValue = 0.035f;
		textMesh.Text = otherGamesItem.Name;
		textMesh.Diffuse = color;
		textMesh.Shader = ShaderManager.Shaders["BaseNoNormal"];
		textMesh.FirstMaterial.AlphaTest = 0.5f;
		textMesh.FirstMaterial.Ambient = color;
		textMesh.FirstMaterial.Shininess = 40f;
		renderItem.addMesh(textMesh);
		TextMesh textMesh2 = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(-0.375f, -0.4f), 0.0015f, HorizontalAlignment.Left), 32, useStringBuilder: false);
		textMesh2.ZValue = 0.035f;
		textMesh2.Text = string.Format("OTHER_GAMES_RELEASED_ON_{0:00}/{1:0000}".Translate(), otherGamesItem.Month, otherGamesItem.Year);
		textMesh2.Diffuse = color;
		textMesh2.Shader = ShaderManager.Shaders["BaseNoNormal"];
		textMesh2.FirstMaterial.AlphaTest = 0.5f;
		textMesh2.FirstMaterial.Ambient = color;
		textMesh2.FirstMaterial.Shininess = 40f;
		renderItem.addMesh(textMesh2);
		TextMesh textMesh3 = new TextMesh(GameTemplate.StandardFont, new TextDrawProperties(new Vector2(0.375f, -0.4f), 0.0015f, HorizontalAlignment.Right), 32, useStringBuilder: false);
		textMesh3.ZValue = 0.035f;
		textMesh3.Text = string.Format("OTHER_GAMES_PRICE_{0}_POINTS".Translate(), otherGamesItem.Price);
		textMesh3.FirstMaterial.Ambient = color;
		textMesh3.Diffuse = color;
		textMesh3.Shader = ShaderManager.Shaders["BaseNoNormal"];
		textMesh3.FirstMaterial.AlphaTest = 0.5f;
		textMesh3.FirstMaterial.Shininess = 40f;
		renderItem.addMesh(new Sized2DRectangleMesh(new Vector2(0.1f), new Vector2(0.325f, -0.4f), new Vector4(0f, 0f, 0f, 1f))
		{
			ZValue = 0.035f
		});
		Sized2DRectangleMesh sized2DRectangleMesh = new Sized2DRectangleMesh(new Vector2(0.095f), new Vector2(0.325f, -0.4f), TextureManager.Textures["OtherGames/BGPattern"]);
		sized2DRectangleMesh.Shader = ShaderManager.Shaders["PointGUI"];
		sized2DRectangleMesh.ZValue = 0.035f;
		sized2DRectangleMesh.FirstMaterial.SetForcedAlpha(alpha: true);
		sized2DRectangleMesh.FirstMaterial.RenderPriority = Material.Priority.Low;
		TaskManager.Post(LoadQR, new KeyValuePair<string, Sized2DRectangleMesh>(otherGamesItem.QR, sized2DRectangleMesh));
		renderItem.addMesh(sized2DRectangleMesh);
		Transform.Translation = new Vector3(1.5f * (float)index, 0f, 0f);
	}

	private void LoadTexture(object parameters)
	{
		KeyValuePair<string, Material> keyValuePair = (KeyValuePair<string, Material>)parameters;
		lock (lockObject)
		{
			keyValuePair.Value.Texture = TextureManager.Textures["OtherGames/" + keyValuePair.Key];
		}
	}

	private void LoadQR(object parameters)
	{
		KeyValuePair<string, Sized2DRectangleMesh> keyValuePair = (KeyValuePair<string, Sized2DRectangleMesh>)parameters;
		lock (lockObject)
		{
			keyValuePair.Value.Texture = TextureManager.Textures["OtherGames/" + keyValuePair.Key];
		}
	}

	protected override void DoUpdate()
	{
		base.DoUpdate();
		int num = index - otherGames.CurrentItemIndex;
		if (num > 0 && Math.Abs(num) > Math.Abs(num - otherGames.Items.Count))
		{
			num -= otherGames.Items.Count;
		}
		else if (num < 0 && Math.Abs(num) > Math.Abs(num + otherGames.Items.Count))
		{
			num += otherGames.Items.Count;
		}
		bool flag = Math.Sign(lastDiffIndex) * Math.Sign(num) != -1;
		Vector3 vector = new Vector3(0.55f * (float)Math.Sign(num) + 0.4f * (float)num, 0f, 0f);
		bool flag2 = otherGames.CurrentItemIndex == index && !otherGames.IsShowingFront;
		Quaternion quaternion = Quaternion.CreateFromAxisAngle(Vector3.UnitY, (float)Math.PI * -5f / 32f * (float)Math.Sign(num) - (float)num * ((float)Math.PI / 4f) * 0.2f + (flag2 ? ((float)Math.PI) : 0f));
		if (flag)
		{
			Transform.Translation = GameMath.Damping(Transform.Translation, vector, 0.02f, Timer.DefaultTimer.LastIntervalSeconds);
			Transform.Rotation = Quaternion.Lerp(Transform.Rotation, quaternion, GameMath.Damping(0f, 1f, 0.02f, Timer.DefaultTimer.LastIntervalSeconds));
		}
		else
		{
			Transform.Translation = vector;
			Transform.Rotation = quaternion;
		}
		lastDiffIndex = num;
	}
}
