using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Microsoft.XboxLive.Avatars.Internal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Quasar.ContentPipeline;
using Quasar.Global;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;
using Quasar.Render;
using Quasar.Shaders.Fixed;
using XnaToFna.StubXDK.GamerServices;

namespace Quasar.GameUtils.XBLIG.Avatar;

public class Avatar : UserIndexVertexMesh<VertexPositionColor>
{
	public enum AnimationType
	{
		BuiltIn,
		SkinnedModel,
		Collada
	}

	private AvatarDescription description;

	private AvatarColladaAnimation currentColladaAnimation;

	private AvatarAnimationPreset currentAnimationPreset;

	private AnimationType currentAnimationType;

	private XnaToFna.StubXDK.GamerServices.AvatarExpression expression;

	private Timer timer;

	private float animationSpeed = 1f;

	private bool loopAnimation = true;

	private AvatarRenderer realRenderer;

	private AvatarAnimation realAnimation;

	private float realAnimationStartOffset;

	private bool realRendererInitAttempted;

	private static readonly object RealAvatarInitLock = new object();

	private static bool realAvatarServicesReady;

	private static int renderLogBudget = 12;

	private static readonly string RealAvatarAssetDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "AvatarAssets");

	private static readonly string RuntimeDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UserData");

	private static readonly string[] RealAvatarComponentIds = new string[10] { "00000004-01d3-0003-c1c8-f109a19cb2e0", "00000008-004b-0001-c1c8-f109a19cb2e0", "00000010-0096-0001-c1c8-f109a19cb2e0", "00000020-002d-0001-c1c8-f109a19cb2e0", "00002000-029e-0003-c1c8-f109a19cb2e0", "00004000-0262-0003-c1c8-f109a19cb2e0", "00008000-02e7-0003-c1c8-f109a19cb2e0", "00080000-0326-0003-c1c8-f109a19cb2e0", "00100000-031b-0003-c1c8-f109a19cb2e0", "00200000-0336-0003-c1c8-f109a19cb2e0" };

	public float AnimationSpeed
	{
		get
		{
			return animationSpeed;
		}
		set
		{
			animationSpeed = value;
		}
	}

	public AvatarAnimationPreset CurrentAnimationPreset => currentAnimationPreset;

	public AnimationType CurrentAnimationType => currentAnimationType;

	public AvatarColladaAnimation CurrentColladaAnimation => currentColladaAnimation;

	public XnaToFna.StubXDK.GamerServices.AvatarExpression Expression
	{
		get
		{
			return expression;
		}
		set
		{
			expression = value;
		}
	}

	public Timer Timer
	{
		set
		{
			timer = value;
		}
	}

	public Avatar(AvatarDescription description)
	{
		this.description = description ?? AvatarDescription.CreateRandom();
		InitializeOfficialRendererHostState();
		base.Alpha = 1f;
		base.Ambient = new Vector3(1f, 1f, 1f);
		base.Diffuse = new Vector3(1f, 1f, 1f);
		InflateBoundingSphere();
		CompatAvatarLog("official-avatar-renderer-only");
	}

	public void StartAnimation(AvatarAnimationPreset preset, AvatarAnimation animation, bool loopAnimation, float timeOffset, bool blend)
	{
		currentAnimationPreset = preset;
		currentColladaAnimation = null;
		currentAnimationType = AnimationType.BuiltIn;
		this.loopAnimation = loopAnimation;
		realAnimationStartOffset = timeOffset;
		ReplaceRealPresetAnimation();
		CompatAvatarLog("game-animation-built-in-" + preset);
	}

	public void StartAnimation(AvatarColladaAnimation animation, bool loopAnimation, float timeOffset, bool cloneAnimation, bool blend)
	{
		currentColladaAnimation = animation;
		currentAnimationType = AnimationType.Collada;
		this.loopAnimation = loopAnimation;
		if (currentColladaAnimation != null)
		{
			currentColladaAnimation.CurrentPosition = TimeSpan.FromSeconds(timeOffset);
		}
		CompatAvatarLog("game-animation-collada-start");
	}

	public void Update()
	{
		if (timer == null)
		{
			return;
		}
		float num = timer.LastIntervalSeconds * animationSpeed;
		if (num > 0f)
		{
			float num2 = (float)Math.Sin((double)timer.TotalTimeSeconds * 3.0) * 0.03f;
			base.Ambient = new Vector3(0.62f + num2, 0.58f + num2, 0.52f + num2);
			TimeSpan elapsedAnimationTime = TimeSpan.FromSeconds(num);
			if (currentAnimationType == AnimationType.BuiltIn && realAnimation != null)
			{
				realAnimation.Update(elapsedAnimationTime, loopAnimation);
			}
			else if (currentAnimationType == AnimationType.Collada && currentColladaAnimation != null)
			{
				currentColladaAnimation.Update(elapsedAnimationTime, loopAnimation);
			}
		}
	}

	public override void Render(Transform motion)
	{
		if (renderLogBudget > 0)
		{
			renderLogBudget--;
			CompatAvatarLog("avatar-mesh-render-" + renderLogBudget + "-pos-" + FormatVector3(motion.Translation) + "-scale-" + FormatVector3(motion.Scale));
		}
		RenderRealAvatar(motion);
	}

	private bool RenderRealAvatar(Transform motion)
	{
		try
		{
			EnsureRealAvatarRenderer();
			if (realRenderer == null)
			{
				return false;
			}
			switch (realRenderer.State)
			{
			case AvatarRendererState.Unavailable:
				return false;
			default:
				return true;
			case AvatarRendererState.Ready:
			{
				SceneRenderData currentRenderData = SceneRenderData.CurrentRenderData;
				if (currentRenderData == null || currentRenderData.Camera == null)
				{
					return false;
				}
				Transform transform = new Transform();
				transform.Assign(motion);
				transform.Scale = Vector3.One;
				realRenderer.World = transform.WorldMatrix;
				realRenderer.View = currentRenderData.Camera.View;
				realRenderer.Projection = currentRenderData.Camera.Projection;
				realRenderer.AmbientLightColor = new Vector3(0.7f, 0.7f, 0.7f);
				realRenderer.LightColor = new Vector3(0.8f, 0.8f, 0.8f);
				realRenderer.LightDirection = Vector3.Normalize(new Vector3(-0.35f, -1f, -0.45f));
				Microsoft.Xna.Framework.GamerServices.AvatarExpression avatarExpression = ConvertRealExpression(expression);
				if (currentAnimationType == AnimationType.Collada && currentColladaAnimation != null && currentColladaAnimation.BoneTransforms != null && currentColladaAnimation.BoneTransforms.Length == 71)
				{
					realRenderer.Draw(currentColladaAnimation.BoneTransforms, avatarExpression);
				}
				else
				{
					if (realAnimation == null)
					{
						ReplaceRealPresetAnimation();
					}
					if (realAnimation != null)
					{
						realRenderer.Draw(realAnimation.BoneTransforms, avatarExpression);
					}
					else
					{
						realRenderer.Draw(realRenderer.BindPose, avatarExpression);
					}
				}
				return true;
			}
			}
		}
		catch (Exception ex)
		{
			CompatAvatarLog("real-avatar-render-failed-" + ex.GetType().Name + "-" + ex.Message);
			if (realRenderer != null)
			{
				realRenderer.Dispose();
				realRenderer = null;
			}
			return false;
		}
	}

	private static Microsoft.Xna.Framework.GamerServices.AvatarExpression ConvertRealExpression(XnaToFna.StubXDK.GamerServices.AvatarExpression source)
	{
		return new Microsoft.Xna.Framework.GamerServices.AvatarExpression
		{
			Mouth = (Microsoft.Xna.Framework.GamerServices.AvatarMouth)Enum.Parse(typeof(Microsoft.Xna.Framework.GamerServices.AvatarMouth), source.Mouth.ToString(), ignoreCase: false),
			LeftEye = (Microsoft.Xna.Framework.GamerServices.AvatarEye)Enum.Parse(typeof(Microsoft.Xna.Framework.GamerServices.AvatarEye), source.LeftEye.ToString(), ignoreCase: false),
			RightEye = (Microsoft.Xna.Framework.GamerServices.AvatarEye)Enum.Parse(typeof(Microsoft.Xna.Framework.GamerServices.AvatarEye), source.RightEye.ToString(), ignoreCase: false),
			LeftEyebrow = (Microsoft.Xna.Framework.GamerServices.AvatarEyebrow)Enum.Parse(typeof(Microsoft.Xna.Framework.GamerServices.AvatarEyebrow), source.LeftEyebrow.ToString(), ignoreCase: false),
			RightEyebrow = (Microsoft.Xna.Framework.GamerServices.AvatarEyebrow)Enum.Parse(typeof(Microsoft.Xna.Framework.GamerServices.AvatarEyebrow), source.RightEyebrow.ToString(), ignoreCase: false)
		};
	}

	private void EnsureRealAvatarRenderer()
	{
		if (realRenderer != null || realRendererInitAttempted)
		{
			return;
		}
		realRendererInitAttempted = true;
		lock (RealAvatarInitLock)
		{
			if (!realAvatarServicesReady)
			{
				AvatarResources.SetDefaultGraphicsDevice(Engine.Device);
				AvatarResources.EnableRenderableModelCaching();
				AvatarResources.SetAnimationResourceLocation(typeof(Avatar).Assembly, "AvatarFarmCompat.AnimationResources");
				AssetDataManager dataManager = AvatarEditingExtensions.GetDataManager();
				dataManager.ClearAssetProviders();
				dataManager.AddAssetProvider(new AssetUrlDataProvider(Path.Combine(RealAvatarAssetDir, "{0}.strb"), Path.Combine(RealAvatarAssetDir, "{1}.strb")));
				realAvatarServicesReady = true;
			}
			AssetDataManager dataManager2 = AvatarEditingExtensions.GetDataManager();
			AssetLoader assetLoader = AvatarEditingExtensions.GetAssetLoader();
			AvatarManifest manifest = AvatarManifest.CreateRandom(AvatarGender.Male, 1)[0];
			AvatarManifestEditor avatarManifestEditor = new AvatarManifestEditor(manifest, dataManager2, assetLoader);
			avatarManifestEditor.RemoveAllComponents();
			string[] realAvatarComponentIds = RealAvatarComponentIds;
			foreach (string g in realAvatarComponentIds)
			{
				avatarManifestEditor.ReplaceAsset(new EditorAssetInfo(new Guid(g)));
			}
			AvatarDescription avatarDescription = AvatarEditingExtensions.CreateAvatarDescriptor(avatarManifestEditor.Manifest);
			realRenderer = new AvatarRenderer(avatarDescription);
			ReplaceRealPresetAnimation();
			CompatAvatarLog("real-avatar-renderer-created-assets-" + RealAvatarComponentIds.Length);
		}
	}

	private void ReplaceRealPresetAnimation()
	{
		try
		{
			if (realAvatarServicesReady)
			{
				if (realAnimation != null)
				{
					realAnimation.Dispose();
					realAnimation = null;
				}
				if (!Enum.TryParse<AvatarAnimationPreset>(currentAnimationPreset.ToString(), ignoreCase: false, out var result))
				{
					result = AvatarAnimationPreset.Stand2;
				}
				if (result == AvatarAnimationPreset.Stand2)
				{
					result = AvatarAnimationPreset.Stand0;
				}
				realAnimation = new AvatarAnimation(result);
				realAnimation.CurrentPosition = TimeSpan.FromSeconds(realAnimationStartOffset);
				CompatAvatarLog("real-avatar-animation-" + result.ToString() + "-for-game-" + currentAnimationPreset);
			}
		}
		catch (Exception ex)
		{
			CompatAvatarLog("real-avatar-animation-failed-" + ex.GetType().Name + "-" + ex.Message);
			if (realAnimation != null)
			{
				realAnimation.Dispose();
				realAnimation = null;
			}
		}
	}

	private void InitializeOfficialRendererHostState()
	{
		shader = new VertexColorShader();
		Material material = new Material();
		material.SetForcedAlpha(alpha: false);
		material.AlphaTest = 0f;
		material.Ambient = Vector3.One;
		material.Diffuse = Vector3.One;
		material.Specular = Vector3.Zero;
		materials.Clear();
		materials.Add(material);
	}

	private void InflateBoundingSphere()
	{
		try
		{
			Type typeFromHandle = typeof(Mesh);
			FieldInfo field = typeFromHandle.GetField("boundingSphere", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (field != null)
			{
				field.SetValue(this, new BoundingSphere(Vector3.Zero, 10000f));
			}
		}
		catch (Exception ex)
		{
			CompatAvatarLog("avatar-bounds-failed-" + ex.GetType().Name + "-" + ex.Message);
		}
	}

	private static void CompatAvatarLog(string text)
	{
		try
		{
			File.AppendAllText(Path.Combine(RuntimeDataDir, "compat-avatar-mesh.log"), DateTime.UtcNow.ToString("O") + " " + text + Environment.NewLine);
		}
		catch
		{
		}
	}

	private static string FormatVector3(Vector3 value)
	{
		return value.X.ToString("0.###", CultureInfo.InvariantCulture) + "," + value.Y.ToString("0.###", CultureInfo.InvariantCulture) + "," + value.Z.ToString("0.###", CultureInfo.InvariantCulture);
	}
}
