using System.Collections.Generic;
using GKEngine.Entities;
using GKEngine.Scenes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GKEngine.Lights;

public class LightManager
{
	public static string EFFECT_PRIMARY_POSITION = "LightPrimaryPos";

	public static string EFFECT_PRIMARY_COLOR = "LightPrimaryColor";

	public static string EFFECT_SECONDARY_POSITION = "LightSecondaryPos";

	public static string EFFECT_SECONDARY_COLOR = "LightSecondaryColor";

	public static string EFFECT_AMBIENT_COLOR = "LightAmbientColor";

	public Scene scene;

	public List<Light> lights = new List<Light>();

	public Light primary;

	public Light secondary;

	public Vector3 ambient;

	public LightManager(Scene oScene)
	{
		scene = oScene;
		primary = AddNew();
		secondary = AddNew();
		ambient = default(Vector3);
	}

	public void Add(Light oLight)
	{
		lights.Add(oLight);
	}

	public Light AddNew()
	{
		Light light = new Light(scene);
		lights.Add(light);
		return light;
	}

	public void SetAmbientColor(byte xR, byte xG, byte xB)
	{
		ambient.X = (float)(int)xR / 255f;
		ambient.Y = (float)(int)xG / 255f;
		ambient.Z = (float)(int)xB / 255f;
	}

	public void SetEffect(ref Effect oEffect)
	{
		if (oEffect.Parameters[EFFECT_PRIMARY_POSITION] != null)
		{
			oEffect.Parameters[EFFECT_PRIMARY_POSITION].SetValue(primary.position);
		}
		if (oEffect.Parameters[EFFECT_PRIMARY_COLOR] != null)
		{
			oEffect.Parameters[EFFECT_PRIMARY_COLOR].SetValue(primary.color);
		}
		if (oEffect.Parameters[EFFECT_SECONDARY_POSITION] != null)
		{
			oEffect.Parameters[EFFECT_SECONDARY_POSITION].SetValue(secondary.position);
		}
		if (oEffect.Parameters[EFFECT_SECONDARY_COLOR] != null)
		{
			oEffect.Parameters[EFFECT_SECONDARY_COLOR].SetValue(secondary.color);
		}
		if (oEffect.Parameters[EFFECT_AMBIENT_COLOR] != null)
		{
			oEffect.Parameters[EFFECT_AMBIENT_COLOR].SetValue(ambient);
		}
	}

	public void SetEffect(ref MaxModel oModel)
	{
		for (int i = 0; i < oModel.modelParts.Count; i++)
		{
			SetEffect(ref oModel.modelParts[i].material.effect);
		}
	}
}
