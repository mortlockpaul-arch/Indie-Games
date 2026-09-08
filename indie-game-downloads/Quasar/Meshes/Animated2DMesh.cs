using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Global.Vertex;
using Quasar.Meshes.Generic;
using Quasar.Shaders;

namespace Quasar.Meshes;

public class Animated2DMesh : UserVertexMesh<VertexPositionUV>
{
	public const string ANIMATION_SHADER = "animation";

	private Animation[] animations;

	private Texture[] textures;

	private int currentAnimation;

	private int currentStep;

	private long stepStartTime;

	private int frameGridWidth;

	private bool stopped;

	private Animation CurrentAnimation => animations[currentAnimation];

	private AnimationStep CurrentStep => CurrentAnimation.steps[currentStep];

	private int FramesPerTexture => frameGridWidth * frameGridWidth;

	public Animated2DMesh(int animationNumber, int textureNumber, int frameGridWidth)
	{
		initializeBuffer();
		this.frameGridWidth = frameGridWidth;
		animations = new Animation[animationNumber];
		textures = new Texture[textureNumber];
		Material material = new Material();
		material.AddVector4Parameter(Vector4.Zero);
		shader = ShaderManager.Shaders["animation"];
		materials.Add(material);
	}

	public void setAnimation(int index, Animation animation)
	{
		animations[index] = animation;
	}

	public void setTexture(int index, Texture texture)
	{
		textures[index] = texture;
	}

	private void initializeBuffer()
	{
		initMesh(4);
		verticesBuffer[0].Position = new Vector3(-0.5f, -0.5f, 0f);
		verticesBuffer[0].UV = new Vector2(0f, 1f);
		verticesBuffer[1].Position = new Vector3(-0.5f, 0.5f, 0f);
		verticesBuffer[1].UV = new Vector2(0f, 0f);
		verticesBuffer[2].Position = new Vector3(0.5f, -0.5f, 0f);
		verticesBuffer[2].UV = new Vector2(1f, 1f);
		verticesBuffer[3].Position = new Vector3(0.5f, 0.5f, 0f);
		verticesBuffer[3].UV = new Vector2(1f, 0f);
		base.PrimitiveCount = 2;
		base.PrimitiveType = PrimitiveType.TriangleStrip;
	}

	public void startAnimation(int animation)
	{
		currentAnimation = animation;
		currentStep = CurrentAnimation.firstStep;
		stepStartTime = Timer.DefaultTimer.TotalTime;
		stopped = false;
	}

	private void setNextStep()
	{
		if (stopped)
		{
			return;
		}
		int num = CurrentStep.nextStep;
		if (num == -1)
		{
			int nextId = CurrentAnimation.nextId;
			if (nextId == -1)
			{
				stopped = true;
				return;
			}
			currentAnimation = nextId;
			num = CurrentAnimation.firstStep;
		}
		currentStep = num;
		stepStartTime = Timer.DefaultTimer.TotalTime;
	}

	private void UpdateAnimation()
	{
		long num = Timer.DefaultTimer.TotalTime - stepStartTime;
		if (num > animations[currentAnimation].steps[currentStep].length)
		{
			setNextStep();
		}
		base.FirstMaterial.Texture = textures[CurrentStep.frameId / FramesPerTexture];
		float num2 = 1f / (float)frameGridWidth;
		int num3 = CurrentStep.frameId % FramesPerTexture;
		Vector2 vector = new Vector2(num2 * (float)(num3 % frameGridWidth), num2 * (float)(num3 / frameGridWidth));
		base.FirstMaterial.SetVector4Parameter(0, new Vector4(num2, num2, vector.X, vector.Y));
	}

	public override void Render(Transform motion)
	{
		UpdateAnimation();
		base.Render(motion);
	}
}
