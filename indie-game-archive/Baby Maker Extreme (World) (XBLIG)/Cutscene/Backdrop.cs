using System.Collections.Generic;
using System.Xml;
using ImportData;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Renderer;

namespace Cutscene;

public class Backdrop
{
	private RenderSprite[,] backgrounds;

	private List<RenderSprite> staticProps;

	public Vector2 Size
	{
		get
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			return backgrounds[0, 0].SurfaceScale * new Vector2((float)(backgrounds.GetUpperBound(0) + 1), (float)(backgrounds.GetUpperBound(1) + 1));
		}
	}

	public Backdrop(string backdropFile, ContentManager content)
	{
		//IL_0091: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0180: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		staticProps = new List<RenderSprite>();
		BackdropImportData backdropImportData;
		if (content == null)
		{
			XmlReader xmlReader = XmlReader.Create(backdropFile, new XmlReaderSettings
			{
				ConformanceLevel = ConformanceLevel.Fragment,
				IgnoreComments = true,
				IgnoreWhitespace = true
			});
			backdropImportData = new BackdropImportData(xmlReader);
			xmlReader.Close();
		}
		else
		{
			backdropImportData = content.Load<BackdropImportData>(backdropFile);
		}
		int width = backdropImportData.Width;
		int height = backdropImportData.Height;
		backgrounds = new RenderSprite[width, height];
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				backgrounds[i, j] = SpriteManager.GetSprite(backdropImportData.GetImageName(i, j), default(Vector2), -10f);
				backgrounds[i, j].Origin = -backgrounds[0, 0].SurfaceScale / 2f;
				backgrounds[i, j].Position = new Vector2((float)i, (float)j) * backgrounds[0, 0].SurfaceScale;
			}
		}
		foreach (BackgroundPropImportData staticProp in backdropImportData.GetStaticProps())
		{
			string pageName = staticProp.PageName;
			Rectangle bounding = staticProp.Bounding;
			float depth = staticProp.Depth;
			Vector2 scale = staticProp.Scale;
			float rotation = staticProp.Rotation;
			Vector2 position = staticProp.Position;
			RenderSprite sprite = SpriteManager.GetSprite(pageName, bounding, default(Vector2), depth);
			sprite.SurfaceScale = scale;
			sprite.Rotation = rotation;
			sprite.Position = position;
			staticProps.Add(sprite);
		}
	}

	public void Draw(TimeTracker gameTime)
	{
		for (int i = 0; i <= backgrounds.GetUpperBound(0); i++)
		{
			for (int j = 0; j <= backgrounds.GetUpperBound(1); j++)
			{
				backgrounds[i, j].Draw(gameTime);
			}
		}
		foreach (RenderSprite staticProp in staticProps)
		{
			staticProp.Draw(gameTime);
		}
	}

	public RenderSprite[,] GetBackgrounds()
	{
		return backgrounds;
	}
}
