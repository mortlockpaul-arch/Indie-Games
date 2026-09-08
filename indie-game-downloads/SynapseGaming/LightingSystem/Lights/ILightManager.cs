using Microsoft.Xna.Framework;
using SynapseGaming.LightingSystem.Core;
using SynapseGaming.LightingSystem.Rendering;
using SynapseGaming.LightingSystem.Shadows;

namespace SynapseGaming.LightingSystem.Lights;

/// <summary>
/// Interface that provides access to the scene's light manager. The light manager
/// provides methods for storing and querying scene lights.
/// </summary>
public interface ILightManager : IWorldRenderableManager, IRenderableManager, IUpdatableManager, ILightQuery, IQuery<BaseLight>, IManagerService, IManager, IUnloadable, ISubmit<BaseLight>, ISubmit<IScene>
{
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
	void SubmitStaticDirectionalLight(Vector3 diffusecolor, Vector3 direction, float intensity, ShadowType shadowtype, float shadowquality, float shadowprimarybias, float shadowsecondarybias);

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
	void SubmitStaticPointLight(Vector3 diffusecolor, Vector3 position, float intensity, float radius, bool filllight, float falloffstrength, ShadowType shadowtype, float shadowquality, float shadowprimarybias, float shadowsecondarybias, IShadowSource shadowsource);

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
	void SubmitStaticSpotLight(Vector3 diffusecolor, Vector3 position, float intensity, float radius, Vector3 direction, float angle, bool filllight, float falloffstrength, ShadowType shadowtype, float shadowquality, float shadowprimarybias, float shadowsecondarybias, IShadowSource shadowsource);

	/// <summary>
	/// Helper method that creates and submits a static light
	/// using a method layout similar to SunBurn 1.2.x.
	/// </summary>
	/// <param name="diffusecolor">Direct lighting color given off by the light.</param>
	/// <param name="intensity">Intensity of the light.</param>
	void SubmitStaticAmbientLight(Vector3 diffusecolor, float intensity);

	/// <summary>
	/// Removes all objects from the container. Commonly used while clearing the scene.
	/// </summary>
	new void Clear();
}
