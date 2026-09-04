using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Infinity.Messages;
using Infinity.Scenes;
using InfinityLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XSIXNARuntime;
using XnaLibrary;
using XnaLibrary.Audio;
using XnaLibrary.Graphics;
using XnaLibrary.Input;

namespace Infinity;

public class Game1 : GameBase
{
	protected override void Initialize()
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Expected O, but got Unknown
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_01df: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		UIMessage.Culture = CultureInfo.CurrentCulture;
		((Game)this).Services.AddService(typeof(PadVibrationComponent), (object)new PadVibrationComponent((Game)(object)this));
		((Collection<IGameComponent>)(object)((Game)this).Components).Add((IGameComponent)(object)(PadVibrationComponent)((Game)this).Services.GetService(typeof(PadVibrationComponent)));
		SceneManagerComponent sceneManagerComponent = (SceneManagerComponent)((Game)this).Services.GetService(typeof(SceneManagerComponent));
		sceneManagerComponent.AddScene(new Logo((Game)(object)this));
		((Game)this).Initialize();
		FadeComponent fadeComponent = (FadeComponent)((Game)this).Services.GetService(typeof(FadeComponent));
		fadeComponent.FadeTime = new TimeSpan(0, 0, 0, 0, 200);
		fadeComponent.Color = Color.White;
		fadeComponent.BlendMode = (SpriteBlendMode)2;
		SoundComponent soundComponent = (SoundComponent)((Game)this).Services.GetService(typeof(SoundComponent));
		soundComponent.EntryBank("StreamBGM", "Content/Audio/StreamBGM.xwb", "Content/Audio/StreamBGM.xsb", isStream: false);
		Global.SaveData.AnaglyphSettings = ((Game)this).Content.Load<AnaglyphSettings>("AnaglyphSettings");
		Global.AsyncLoader = new AsyncLoader(new ContentManager((IServiceProvider)((Game)this).Services, "Content"));
		LightSettings[] lights = Global.SaveData.AnaglyphSettings.Lights;
		foreach (LightSettings lightSettings in lights)
		{
			XSISASPointLight xSISASPointLight = new XSISASPointLight();
			xSISASPointLight.Color = lightSettings.Color;
			xSISASPointLight.Position = lightSettings.Position;
			xSISASPointLight.Range = lightSettings.Range;
			XSISASPointLight item = xSISASPointLight;
			Global.SASData.PointLights.Add(item);
		}
		float fieldOfView = Global.SaveData.AnaglyphSettings.FieldOfView;
		Global.SASData.Camera.NearFarClipping.X = 1f;
		Global.SASData.Camera.NearFarClipping.Y = 10000f;
		XSISASContainer sASData = Global.SASData;
		float num = MathHelper.ToRadians(fieldOfView);
		Viewport viewport = ((Game)this).GraphicsDevice.Viewport;
		sASData.Projection = Matrix.CreatePerspectiveFieldOfView(num, ((Viewport)(ref viewport)).AspectRatio, Global.SASData.Camera.NearFarClipping.X, Global.SASData.Camera.NearFarClipping.Y);
		Global.TimeRuler = timerRuler;
	}
}
