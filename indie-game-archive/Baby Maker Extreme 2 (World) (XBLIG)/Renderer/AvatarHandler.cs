using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;

namespace Renderer;

public class AvatarHandler
{
	private AvatarRenderer m_avatarRenderer;

	private List<Matrix> avatarBoneTransforms;

	private List<Matrix> avatarWorldTransforms;

	private List<Matrix> avatarLocalTransforms;

	private Matrix view;

	private Matrix projection;

	private AvatarDescription m_avatarDesc;

	private float m_fDepth;

	public bool m_bShouldDraw;

	public bool ShouldDraw
	{
		get
		{
			return m_bShouldDraw;
		}
		set
		{
			m_bShouldDraw = value;
		}
	}

	public float Depth => m_fDepth;

	public AvatarHandler(int controllerIndex)
	{
		m_fDepth = 0f;
		int count = 71;
		avatarBoneTransforms = Enumerable.Repeat(Matrix.Identity, count).ToList();
		avatarWorldTransforms = avatarBoneTransforms.ToList();
		avatarLocalTransforms = avatarBoneTransforms.ToList();
		avatarBoneTransforms[14] = Matrix.CreateRotationX(MathHelper.ToRadians(30f));
		avatarBoneTransforms[19] = Matrix.CreateRotationX(MathHelper.ToRadians(-10f));
		avatarBoneTransforms[20] = Matrix.CreateRotationY(MathHelper.ToRadians(30f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-80f));
		avatarBoneTransforms[22] = Matrix.CreateRotationY(MathHelper.ToRadians(-30f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(80f));
		avatarBoneTransforms[25] = Matrix.CreateRotationY(MathHelper.ToRadians(-115f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-30f));
		avatarBoneTransforms[28] = Matrix.CreateRotationY(MathHelper.ToRadians(115f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(30f));
		avatarBoneTransforms[2] = Matrix.CreateRotationX(MathHelper.ToRadians(-140f));
		avatarBoneTransforms[3] = Matrix.CreateRotationX(MathHelper.ToRadians(-140f));
		avatarBoneTransforms[6] = Matrix.CreateRotationX(MathHelper.ToRadians(110f));
		avatarBoneTransforms[8] = Matrix.CreateRotationX(MathHelper.ToRadians(110f));
		m_avatarDesc = GetAvatarDescript(controllerIndex);
		if (m_avatarDesc == null)
		{
			m_avatarRenderer = null;
		}
		else
		{
			FinishInit();
		}
	}

	private void FinishInit()
	{
		if (m_avatarDesc != null)
		{
			m_avatarRenderer = new AvatarRenderer(m_avatarDesc);
			m_avatarRenderer.World = Matrix.CreateTranslation(0f, (0f - m_avatarDesc.Height) / 2f, 0f) * Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(45f));
			m_bShouldDraw = true;
		}
	}

	public void Draw()
	{
		if (m_avatarRenderer != null)
		{
			m_avatarRenderer.View = view;
			m_avatarRenderer.Projection = projection;
			m_avatarRenderer.Draw(avatarBoneTransforms, default(AvatarExpression));
		}
	}

	public void SetRotations(float neckRot, float armRot, float legRot, float bodyRot, Vector2 position, float depth, float worldScale)
	{
		m_fDepth = depth;
		avatarBoneTransforms[1] = Matrix.CreateRotationX(MathHelper.ToRadians(20f));
		avatarBoneTransforms[5] = Matrix.CreateRotationX(MathHelper.ToRadians(20f));
		while ((double)neckRot > Math.PI * 2.0)
		{
			neckRot -= (float)Math.PI * 2f;
		}
		while ((double)neckRot < Math.PI * -2.0)
		{
			neckRot += (float)Math.PI * 2f;
		}
		avatarBoneTransforms[19] = Matrix.CreateTranslation(new Vector3(0f, -0.1f, 0f)) * Matrix.CreateRotationX(MathHelper.ToRadians(10f) + neckRot);
		avatarBoneTransforms[20] = Matrix.CreateRotationY(MathHelper.ToRadians(30f) + armRot) * Matrix.CreateRotationZ(MathHelper.ToRadians(-80f));
		avatarBoneTransforms[22] = Matrix.CreateRotationY(MathHelper.ToRadians(-30f) - armRot) * Matrix.CreateRotationZ(MathHelper.ToRadians(80f));
		avatarBoneTransforms[25] = Matrix.CreateRotationY(MathHelper.ToRadians(-115f - armRot / 10f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-30f));
		avatarBoneTransforms[28] = Matrix.CreateRotationY(MathHelper.ToRadians(115f + armRot / 10f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(30f));
		avatarBoneTransforms[2] = Matrix.CreateRotationX(MathHelper.ToRadians(-150f) + legRot);
		avatarBoneTransforms[3] = Matrix.CreateRotationX(MathHelper.ToRadians(-150f) + legRot);
		avatarBoneTransforms[6] = Matrix.CreateRotationX(MathHelper.ToRadians(150f));
		avatarBoneTransforms[8] = Matrix.CreateRotationX(MathHelper.ToRadians(150f));
		float scale = 250f / m_avatarDesc.Height;
		if (m_avatarRenderer != null)
		{
			m_avatarRenderer.World = Matrix.CreateTranslation(-0.5f, 0.1f - m_avatarDesc.Height * 0.6f, 0f) * Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-45f) + bodyRot) * Matrix.CreateScale(scale) * Matrix.CreateRotationY(MathHelper.ToRadians(3f)) * Matrix.CreateTranslation(position.X, position.Y, 0f) * Matrix.CreateScale(worldScale);
		}
	}

	public void Update()
	{
		if (m_avatarRenderer == null)
		{
			FinishInit();
		}
		float degrees = 0f;
		float degrees2 = 0f;
		float num = 500f;
		view = Matrix.CreateRotationY(MathHelper.ToRadians(degrees)) * Matrix.CreateRotationX(MathHelper.ToRadians(degrees2)) * Matrix.CreateLookAt(new Vector3(0f, 0f, 0f - num), new Vector3(0f, 0f, 0f), Vector3.Up);
		_ = SceneRenderer.GetGraphicsDevice().Viewport.AspectRatio;
		projection = Matrix.CreateOrthographic(SceneRenderer.GetScreenDim().X, SceneRenderer.GetScreenDim().Y, 1f, 1000f);
	}

	public AvatarDescription GetAvatarDescript(int playerId)
	{
		PlayerIndex playerIndex = ControlManager.GetPlayerIndex(playerId);
		SignedInGamer signedInGamer = null;
		foreach (SignedInGamer signedInGamer2 in Gamer.SignedInGamers)
		{
			if (signedInGamer2.PlayerIndex == playerIndex)
			{
				signedInGamer = signedInGamer2;
			}
		}
		if (signedInGamer != null)
		{
			AvatarDescription.BeginGetFromGamer(signedInGamer, LoadAvatarDescription, null);
			return null;
		}
		return AvatarDescription.CreateRandom();
	}

	private void LoadAvatarDescription(IAsyncResult result)
	{
		AvatarDescription avatarDescription = AvatarDescription.EndGetFromGamer(result);
		if (avatarDescription.IsValid)
		{
			m_avatarDesc = avatarDescription;
		}
		else
		{
			m_avatarDesc = AvatarDescription.CreateRandom();
		}
	}

	public bool IsMale()
	{
		return m_avatarDesc.BodyType == AvatarBodyType.Male;
	}
}
