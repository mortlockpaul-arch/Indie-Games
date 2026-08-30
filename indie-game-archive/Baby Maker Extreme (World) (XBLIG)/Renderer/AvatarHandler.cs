using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

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
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		base._002Ector();
		m_fDepth = 0f;
		m_avatarDesc = GetAvatarDescript(controllerIndex);
		m_avatarRenderer = new AvatarRenderer(m_avatarDesc);
		int count = 71;
		avatarBoneTransforms = Enumerable.Repeat<Matrix>(Matrix.Identity, count).ToList();
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
		m_avatarRenderer.World = Matrix.CreateTranslation(0f, (0f - m_avatarDesc.Height) / 2f, 0f) * Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(45f));
		m_bShouldDraw = true;
	}

	public void Draw()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		m_avatarRenderer.View = view;
		m_avatarRenderer.Projection = projection;
		m_avatarRenderer.Draw((IList<Matrix>)avatarBoneTransforms, default(AvatarExpression));
	}

	public void SetRotations(float neckRot, float armRot, float legRot, float bodyRot, Vector2 position, float depth)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0271: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_028e: Unknown result type (might be due to invalid IL or missing references)
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
		float num = 250f / m_avatarDesc.Height;
		m_avatarRenderer.World = Matrix.CreateTranslation(-0.5f, 0.1f - m_avatarDesc.Height * 0.6f, 0f) * Matrix.CreateRotationY(MathHelper.ToRadians(90f)) * Matrix.CreateRotationZ(MathHelper.ToRadians(-45f) + bodyRot) * Matrix.CreateScale(num) * Matrix.CreateRotationY(MathHelper.ToRadians(3f)) * Matrix.CreateTranslation(position.X, position.Y, 0f);
	}

	public void Update()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		float num = 0f;
		float num2 = 0f;
		float num3 = 500f;
		view = Matrix.CreateRotationY(MathHelper.ToRadians(num)) * Matrix.CreateRotationX(MathHelper.ToRadians(num2)) * Matrix.CreateLookAt(new Vector3(0f, 0f, 0f - num3), new Vector3(0f, 0f, 0f), Vector3.Up);
		Viewport viewport = SceneRenderer.GetGraphicsDevice().Viewport;
		_ = ((Viewport)(ref viewport)).AspectRatio;
		projection = Matrix.CreateOrthographic(SceneRenderer.GetScreenDim().X, SceneRenderer.GetScreenDim().Y, 1f, 1000f);
	}

	public static AvatarDescription GetAvatarDescript(int playerId)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		PlayerIndex playerIndex = ControlManager.GetPlayerIndex(playerId);
		SignedInGamer val = null;
		GamerCollectionEnumerator<SignedInGamer> enumerator = ((GamerCollection<SignedInGamer>)(object)Gamer.SignedInGamers).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				SignedInGamer current = enumerator.Current;
				if (current.PlayerIndex == playerIndex)
				{
					val = current;
				}
			}
		}
		finally
		{
			((IDisposable)enumerator/*cast due to constrained. prefix*/).Dispose();
		}
		if (val != null && val.Avatar != null)
		{
			return val.Avatar;
		}
		return AvatarDescription.CreateRandom();
	}

	public bool IsMale()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		return (int)m_avatarDesc.BodyType == 1;
	}
}
