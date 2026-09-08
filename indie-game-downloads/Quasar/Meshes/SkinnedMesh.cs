using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Quasar.Global;
using Quasar.Meshes.SkinnedBodies;
using Quasar.Shaders;

namespace Quasar.Meshes;

public class SkinnedMesh : XMesh
{
	private AnimationPlayer animationPlayer;

	private SkinningData skinningData;

	private float timeScale = 1f;

	private Timer timer = Timer.DefaultTimer;

	private uint lastFrameIndex;

	public Timer Timer
	{
		get
		{
			return timer;
		}
		set
		{
			if (value == null)
			{
				timer = Timer.DefaultTimer;
			}
			else
			{
				timer = value;
			}
		}
	}

	public bool Loop
	{
		get
		{
			return animationPlayer.Loop;
		}
		set
		{
			animationPlayer.Loop = value;
		}
	}

	protected SkinnedMesh(SkinningData data, ModelMesh mesh, bool interpolate)
		: base(mesh)
	{
		skinningData = data;
		_ = skinningData;
		animationPlayer = new AnimationPlayer(skinningData, interpolate);
		base.FirstMaterial.AddMatrixParameter(Matrix.Identity);
		foreach (Material material in base.Materials)
		{
			material.SetMatrixArrayParameter(0, animationPlayer.GetSkinTransforms());
		}
		base.Shader = ShaderManager.Shaders["SkinnedModel"];
	}

	private SkinnedMesh()
	{
	}

	public void SetTimeScale(float t)
	{
		timeScale = t;
	}

	public static SkinnedMesh LoadSkinnedMesh(string filename, bool interpolate)
	{
		string text = filename;
		if (!text.StartsWith("Models/"))
		{
			text = "Models/" + text;
		}
		Model model = Engine.ContentManager.Load<Model>(text);
		return new SkinnedMesh(model.Tag as SkinningData, model.Meshes[0], interpolate);
	}

	public bool IsCurrentAnimation(string animName)
	{
		AnimationClip value = null;
		if (skinningData.AnimationClips.TryGetValue(animName, out value))
		{
			return animationPlayer.CurrentClip == value;
		}
		return false;
	}

	public bool EnqueueAnimation(string animName)
	{
		AnimationClip animationClip = null;
		if (skinningData.AnimationClips.ContainsKey(animName))
		{
			animationClip = skinningData.AnimationClips[animName];
		}
		if (animationClip == null)
		{
			return false;
		}
		return animationPlayer.EnqueueClip(animationClip);
	}

	public bool SetAnimation(string animName)
	{
		return SetAnimation(animName, 0f);
	}

	public bool SetAnimation(string animName, float timeOffset)
	{
		AnimationClip animationClip = null;
		if (skinningData.AnimationClips.ContainsKey(animName))
		{
			animationClip = skinningData.AnimationClips[animName];
		}
		if (animationClip == null)
		{
			return false;
		}
		return animationPlayer.StartClip(animationClip, timeOffset);
	}

	public override void Render(Transform motion)
	{
		if (Engine.Instance.ElapsedFrames != lastFrameIndex)
		{
			animationPlayer.Update(TimeSpan.FromSeconds(timer.LastIntervalSeconds * timeScale), relativeToCurrentTime: true, Matrix.Identity);
			lastFrameIndex = Engine.Instance.ElapsedFrames;
		}
		base.Render(motion);
	}

	protected override Mesh DoClone()
	{
		return new SkinnedMesh();
	}

	protected override void CopyMeshData(Mesh mesh)
	{
		base.CopyMeshData(mesh);
		((SkinnedMesh)mesh).skinningData = skinningData;
		((SkinnedMesh)mesh).timeScale = timeScale;
		((SkinnedMesh)mesh).timer = timer;
		((SkinnedMesh)mesh).animationPlayer = new AnimationPlayer(animationPlayer);
		((SkinnedMesh)mesh).lastFrameIndex = lastFrameIndex;
		foreach (Material material in mesh.Materials)
		{
			material.SetMatrixArrayParameter(0, ((SkinnedMesh)mesh).animationPlayer.GetSkinTransforms());
		}
	}
}
