using System.Collections.Generic;
using GKEngine.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Scenes;

public class SceneLibrary
{
	private Scene scene;

	private SceneLibraryData data;

	private List<AssetEntity> assets = new List<AssetEntity>();

	public Dictionary<string, Effect> effects = new Dictionary<string, Effect>();

	public Dictionary<string, Model> modles = new Dictionary<string, Model>();

	public Dictionary<string, Texture2D> texture2Ds = new Dictionary<string, Texture2D>();

	public Dictionary<string, Texture3D> texture3Ds = new Dictionary<string, Texture3D>();

	public Dictionary<string, TextureCube> textureCubes = new Dictionary<string, TextureCube>();

	public Dictionary<string, Texture2D[]> textureSequences = new Dictionary<string, Texture2D[]>();

	public SceneLibrary(Scene oScene)
	{
		scene = oScene;
	}

	public void Load()
	{
		ContentManager sceneContent = GameEngine.SceneContent;
		for (int i = 0; i < assets.Count; i++)
		{
			switch (assets[i].type)
			{
			case AssetType.Model:
				modles.Add(assets[i].name, sceneContent.Load<Model>(assets[i].path));
				break;
			case AssetType.Effect:
				effects.Add(assets[i].name, sceneContent.Load<Effect>(assets[i].path));
				break;
			case AssetType.Texture2D:
				texture2Ds.Add(assets[i].name, sceneContent.Load<Texture2D>(assets[i].path));
				break;
			case AssetType.Texture3D:
				texture3Ds.Add(assets[i].name, sceneContent.Load<Texture3D>(assets[i].path));
				break;
			case AssetType.TextureCube:
				textureCubes.Add(assets[i].name, sceneContent.Load<TextureCube>(assets[i].path));
				break;
			}
		}
		for (int i = 0; i < data.TextureSheets.Count; i++)
		{
			Texture2D texture2D = GameEngine.SceneContent.Load<Texture2D>(data.TextureSheets[i].path);
			for (int j = 0; j < data.TextureSheets[i].assets.Count; j++)
			{
				TextureSheetAsset textureSheetAsset = data.TextureSheets[i].assets[j];
				Color[] array = new Color[textureSheetAsset.width * textureSheetAsset.height];
				texture2D.GetData(0, new Rectangle(textureSheetAsset.x, textureSheetAsset.y, textureSheetAsset.width, textureSheetAsset.height), array, 0, textureSheetAsset.width * textureSheetAsset.height);
				Texture2D texture2D2 = new Texture2D(GameEngine.Graphics.GraphicsDevice, textureSheetAsset.width, textureSheetAsset.height);
				texture2D2.SetData(array);
				texture2Ds.Add(textureSheetAsset.name, texture2D2);
			}
			texture2D.Dispose();
		}
		for (int i = 0; i < data.TextureSheetSequences.Count; i++)
		{
			Texture2D oTexture = GameEngine.SceneContent.Load<Texture2D>(data.TextureSheetSequences[i].path);
			textureSequences.Add(data.TextureSheetSequences[i].name, TextureUtils.SheetToTextures(oTexture, data.TextureSheetSequences[i].gridX, data.TextureSheetSequences[i].gridY, data.TextureSheetSequences[i].count));
			oTexture = null;
		}
	}

	public void Unload()
	{
		foreach (KeyValuePair<string, Model> modle in modles)
		{
			_ = modle;
		}
		modles.Clear();
		foreach (KeyValuePair<string, Effect> effect in effects)
		{
			_ = effect;
		}
		effects.Clear();
		foreach (KeyValuePair<string, Texture2D> texture2D in texture2Ds)
		{
			_ = texture2D;
		}
		texture2Ds.Clear();
		foreach (KeyValuePair<string, Texture3D> texture3D in texture3Ds)
		{
			_ = texture3D;
		}
		texture3Ds.Clear();
		foreach (KeyValuePair<string, TextureCube> textureCube in textureCubes)
		{
			_ = textureCube;
		}
		textureCubes.Clear();
		foreach (KeyValuePair<string, Texture2D[]> textureSequence in textureSequences)
		{
			for (int i = 0; i < textureSequence.Value.Length; i++)
			{
				textureSequence.Value[i].Dispose();
				textureSequence.Value[i] = null;
			}
		}
		textureSequences.Clear();
	}

	public void FileLoad(string xFile)
	{
		data = SceneLibraryData.Load(xFile);
		assets = data.Assets;
		for (int i = 0; i < data.AssetSequences.Count; i++)
		{
			for (int j = data.AssetSequences[i].start; j <= data.AssetSequences[i].end; j++)
			{
				string text = j.ToString();
				text = text.PadLeft(data.AssetSequences[i].digits, '0');
				assets.Add(new AssetEntity(data.AssetSequences[i].name + (j - data.AssetSequences[i].start), data.AssetSequences[i].path + text, data.AssetSequences[i].type));
			}
		}
	}
}
