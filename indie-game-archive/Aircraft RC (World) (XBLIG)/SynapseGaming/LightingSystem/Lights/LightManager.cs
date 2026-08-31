using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Manages all scene lights and allows querying the scene with
/// a view or bounding box for lights that affect the area
/// (acts as a light scenegraph).
/// </summary>
public class LightManager : BaseLightManager<ILightManager>, ILightManager, IWorldRenderableManager, IRenderableManager, IUpdatableManager, ILightQuery, IQuery<BaseLight>, IManagerService, IManager, IUnloadable, ISubmit<BaseLight>, ISubmit<IScene>
{
	private int HCB = 30;

	private List<BaseLight> HC_0002 = new List<BaseLight>(64);

	/// <summary>
	/// Gets the manager specific Type used as a unique key for storing and
	/// requesting the manager from the IManagerServiceProvider.
	/// </summary>
	public override Type ManagerType => SceneInterface.LightManagerType;

	/// <summary>
	/// Sets the order this manager is processed relative to other managers
	/// in the IManagerServiceProvider. Managers with lower processing order
	/// values are processed first.
	///
	/// In the case of BeginFrameRendering and EndFrameRendering, BeginFrameRendering
	/// is processed in the normal order (lowest order value to highest), however
	/// EndFrameRendering is processed in reverse order (highest to lowest) to ensure
	/// the first manager begun is the last one ended (FILO).
	/// </summary>
	public override int ManagerProcessOrder
	{
		get
		{
			return HCB;
		}
		set
		{
			HCB = value;
		}
	}

	/// <summary>
	/// Creates a new LightManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	/// <param name="worldboundingbox">The smallest bounding area that completely
	/// contains the scene. Helps the LightManager build an optimal scene tree.</param>
	/// <param name="worldtreemaxdepth">Maximum depth for entries in the scene tree. Small
	/// scenes with few objects see better performance with shallow trees. Large complex
	/// scenes often need deeper trees.</param>
	public LightManager(IManagerServiceProvider sceneinterface, BoundingBox worldboundingbox, int worldtreemaxdepth)
		: base(sceneinterface, worldboundingbox, worldtreemaxdepth)
	{
	}

	/// <summary>
	/// Creates a new LightManager instance.
	/// </summary>
	/// <param name="sceneinterface">Service provider used to access all other manager services in this scene.</param>
	public LightManager(IManagerServiceProvider sceneinterface)
		: base(sceneinterface)
	{
	}

	/// <summary>
	/// Use to apply user quality and performance preferences to the resources managed by this object.
	/// </summary>
	/// <param name="preferences"></param>
	public override void ApplyPreferences(ISystemPreferences preferences)
	{
		base.ApplyPreferences(preferences);
	}

	private void HF()
	{
	}

	/// <summary>
	/// Helper method that creates and submits a static light
	/// using a method layout similar to SunBurn 1.2.x.
	/// </summary>
	/// <param name="diffusecolor">Direct lighting color given off by the light.</param>
	/// <param name="direction">Direction the light is pointing.</param>
	/// <param name="intensity">Intensity of the light.</param>
	/// <param name="shadowtype">Defines the type of objects that cast shadows from the light.
	/// Does not affect an object's ability to receive shadows.</param>
	/// <param name="shadowquality">Visual quality of casts shadows.</param>
	/// <param name="shadowprimarybias">Main property used to eliminate shadow artifacts.</param>
	/// <param name="shadowsecondarybias">Additional fine-tuned property used to eliminate shadow artifacts.</param>
	public void SubmitStaticDirectionalLight(Vector3 diffusecolor, Vector3 direction, float intensity, ShadowType shadowtype, float shadowquality, float shadowprimarybias, float shadowsecondarybias)
	{
		DirectionalLight directionalLight = new DirectionalLight();
		directionalLight.DiffuseColor = diffusecolor;
		directionalLight.Intensity = intensity;
		directionalLight.Direction = direction;
		directionalLight.ShadowType = shadowtype;
		directionalLight.ShadowQuality = shadowquality;
		directionalLight.ShadowPrimaryBias = shadowprimarybias;
		directionalLight.ShadowSecondaryBias = shadowsecondarybias;
		Submit(directionalLight);
	}

	/// <summary>
	/// Helper method that creates and submits a static light
	/// using a method layout similar to SunBurn 1.2.x.
	/// </summary>
	/// <param name="diffusecolor">Direct lighting color given off by the light.</param>
	/// <param name="position">Position in world space of the light.</param>
	/// <param name="intensity">Intensity of the light.</param>
	/// <param name="radius">Lighting radius in world space.</param>
	/// <param name="filllight">Provides softer indirect-like illumination without "hot-spots".</param>
	/// <param name="falloffstrength">Controls how quickly lighting falls off over distance (only available in deferred rendering).</param>
	/// <param name="shadowtype">Defines the type of objects that cast shadows from the light.
	/// Does not affect an object's ability to receive shadows.</param>
	/// <param name="shadowquality">Visual quality of casts shadows.</param>
	/// <param name="shadowprimarybias">Main property used to eliminate shadow artifacts.</param>
	/// <param name="shadowsecondarybias">Additional fine-tuned property used to eliminate shadow artifacts.</param>
	/// <param name="shadowsource">Shadow source the light's shadows are generated from.
	/// Allows sharing shadows between point light sources.  Setting the parameter
	/// to null makes the light its own unique shadow source.</param>
	public void SubmitStaticPointLight(Vector3 diffusecolor, Vector3 position, float intensity, float radius, bool filllight, float falloffstrength, ShadowType shadowtype, float shadowquality, float shadowprimarybias, float shadowsecondarybias, IShadowSource shadowsource)
	{
		PointLight pointLight = new PointLight();
		pointLight.DiffuseColor = diffusecolor;
		pointLight.Position = position;
		pointLight.Intensity = intensity;
		pointLight.Radius = radius;
		pointLight.FillLight = filllight;
		pointLight.FalloffStrength = falloffstrength;
		pointLight.ShadowType = shadowtype;
		pointLight.ShadowQuality = shadowquality;
		pointLight.ShadowPrimaryBias = shadowprimarybias;
		pointLight.ShadowSecondaryBias = shadowsecondarybias;
		if (shadowsource != null)
		{
			pointLight.ShadowSource = shadowsource;
		}
		else
		{
			pointLight.ShadowSource = pointLight;
		}
		Submit(pointLight);
	}

	/// <summary>
	/// Helper method that creates and submits a static light
	/// using a method layout similar to SunBurn 1.2.x.
	/// </summary>
	/// <param name="diffusecolor">Direct lighting color given off by the light.</param>
	/// <param name="position">Position in world space of the light.</param>
	/// <param name="intensity">Intensity of the light.</param>
	/// <param name="radius">Lighting radius in world space.</param>
	/// <param name="direction">Direction the light is pointing.</param>
	/// <param name="angle">Angle in degrees of the light's influence.</param>
	/// <param name="filllight">Provides softer indirect-like illumination without "hot-spots".</param>
	/// <param name="falloffstrength">Controls how quickly lighting falls off over distance (only available in deferred rendering).</param>
	/// <param name="shadowtype">Defines the type of objects that cast shadows from the light.
	/// Does not affect an object's ability to receive shadows.</param>
	/// <param name="shadowquality">Visual quality of casts shadows.</param>
	/// <param name="shadowprimarybias">Main property used to eliminate shadow artifacts.</param>
	/// <param name="shadowsecondarybias">Additional fine-tuned property used to eliminate shadow artifacts.</param>
	/// <param name="shadowsource">Shadow source the light's shadows are generated from.
	/// Allows sharing shadows between point light sources.  Setting the parameter
	/// to null makes the light its own unique shadow source.</param>
	public void SubmitStaticSpotLight(Vector3 diffusecolor, Vector3 position, float intensity, float radius, Vector3 direction, float angle, bool filllight, float falloffstrength, ShadowType shadowtype, float shadowquality, float shadowprimarybias, float shadowsecondarybias, IShadowSource shadowsource)
	{
		SpotLight spotLight = new SpotLight();
		spotLight.DiffuseColor = diffusecolor;
		spotLight.Position = position;
		spotLight.Intensity = intensity;
		spotLight.Radius = radius;
		spotLight.Direction = direction;
		spotLight.Angle = angle;
		spotLight.FillLight = filllight;
		spotLight.FalloffStrength = falloffstrength;
		spotLight.ShadowType = shadowtype;
		spotLight.ShadowQuality = shadowquality;
		spotLight.ShadowPrimaryBias = shadowprimarybias;
		spotLight.ShadowSecondaryBias = shadowsecondarybias;
		if (shadowsource != null)
		{
			spotLight.ShadowSource = shadowsource;
		}
		else
		{
			spotLight.ShadowSource = spotLight;
		}
		Submit(spotLight);
	}

	/// <summary>
	/// Helper method that creates and submits a static light
	/// using a method layout similar to SunBurn 1.2.x.
	/// </summary>
	/// <param name="diffusecolor">Direct lighting color given off by the light.</param>
	/// <param name="intensity">Intensity of the light.</param>
	public void SubmitStaticAmbientLight(Vector3 diffusecolor, float intensity)
	{
		AmbientLight ambientLight = new AmbientLight();
		ambientLight.DiffuseColor = diffusecolor;
		ambientLight.Intensity = intensity;
		Submit(ambientLight);
	}

	/// <summary>
	/// Removes all lights and cleans up scene information.
	/// </summary>
	public override void Clear()
	{
		base.Clear();
		OptimizationSystem.Clear();
	}

	/// <summary>
	/// Disposes any graphics resource used internally by this object, and removes
	/// scene resources managed by this object. Commonly used during Game.UnloadContent.
	/// </summary>
	public override void Unload()
	{
		HF();
		base.Unload();
	}
}
