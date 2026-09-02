using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using RacingGame.Graphics;
using RacingGame.Helpers;

namespace RacingGame.Shaders;

public class PreScreenSkyCubeMapping : ShaderEffect
{
	private const string Filename = "PreScreenSkyCubeMapping.fx";

	private const string SkyCubeMapFilename = "SkyCubeMap";

	private static readonly Color DefaultSkyColor;

	private TextureCube skyCubeMapTexture;

	private float time;

	private Model cube;

	public TextureCube SkyCubeMapTexture => skyCubeMapTexture;

	public PreScreenSkyCubeMapping()
		: base("PreScreenSkyCubeMapping.fx")
	{
		cube = BaseGame.Content.Load<Model>("Content\\models\\Cube");
	}

	protected override void GetParameters()
	{
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		base.GetParameters();
		skyCubeMapTexture = BaseGame.Content.Load<TextureCube>(Path.Combine(Directories.ContentDirectory + "\\textures", "SkyCubeMap"));
		diffuseTexture.SetValue((Texture)(object)skyCubeMapTexture);
		base.AmbientColor = DefaultSkyColor;
	}

	public void RenderSky(Color setSkyColor)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (base.Valid)
		{
			BaseGame.Device.RenderState.DepthBufferEnable = false;
			BaseGame.Device.RenderState.DepthBufferWriteEnable = false;
			BaseGame.Device.RenderState.CullMode = (CullMode)1;
			BaseGame.Device.RenderState.AlphaBlendEnable = false;
			base.AmbientColor = setSkyColor;
			effect.Parameters["view"].SetValue(BaseGame.ViewMatrix);
			base.ProjectionMatrix = BaseGame.ProjectionMatrix;
			if (time <= 1f)
			{
				time += 0.01f;
			}
			else
			{
				time = 0f;
			}
			if (currTime != null)
			{
				effect.Parameters["time"].SetValue(time);
			}
			((ReadOnlyCollection<ModelMeshPart>)(object)((ReadOnlyCollection<ModelMesh>)(object)cube.Meshes)[0].MeshParts)[0].Effect = effect;
			((ReadOnlyCollection<ModelMesh>)(object)cube.Meshes)[0].Draw();
			BaseGame.Device.RenderState.DepthBufferEnable = true;
			BaseGame.Device.RenderState.DepthBufferWriteEnable = true;
			BaseGame.Device.RenderState.CullMode = (CullMode)3;
		}
	}

	public void RenderSky()
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		RenderSky(lastUsedAmbientColor);
	}

	static PreScreenSkyCubeMapping()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		DefaultSkyColor = new Color((byte)232, (byte)232, (byte)232);
	}
}
