using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ZP2K9.net.bandwidth;

namespace ZP2K9.net;

public class BandwidthManager
{
	public CharHistory[] charHistory;

	private float[] bytesPerSecSent = new float[64];

	private float[] bytesPerSecReceived = new float[64];

	private float[] bytesSent = new float[64];

	private float[] bytesReceived = new float[64];

	public BandwidthManager()
	{
		charHistory = new CharHistory[32];
		for (int i = 0; i < charHistory.Length; i++)
		{
			charHistory[i] = new CharHistory();
		}
	}

	public void UpdateSentReceived(float sentPerSec, float receivedPerSec, float sent, float recv)
	{
		for (int num = bytesPerSecSent.Length - 2; num >= 0; num--)
		{
			bytesPerSecSent[num + 1] = bytesPerSecSent[num];
		}
		bytesPerSecSent[0] = sentPerSec;
		for (int num2 = bytesPerSecReceived.Length - 2; num2 >= 0; num2--)
		{
			bytesPerSecReceived[num2 + 1] = bytesPerSecReceived[num2];
		}
		bytesPerSecReceived[0] = receivedPerSec;
		for (int num3 = bytesSent.Length - 2; num3 >= 0; num3--)
		{
			bytesSent[num3 + 1] = bytesSent[num3];
		}
		bytesSent[0] = sent;
		for (int num4 = bytesReceived.Length - 2; num4 >= 0; num4--)
		{
			bytesReceived[num4 + 1] = bytesReceived[num4];
		}
		bytesReceived[0] = recv;
	}

	public void Draw(SpriteBatch sprite)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < 10; i++)
		{
			charHistory[i].Draw(sprite, new Vector2(1100f, (float)i * 64f + 64f));
		}
		Vector2 val = default(Vector2);
		((Vector2)(ref val))._002Ector(1100f, 500f);
		for (int j = 0; j < bytesPerSecSent.Length; j++)
		{
			float num = bytesPerSecSent[j];
			num /= 100f;
			sprite.Draw(Game1.nullTex, val + new Vector2((float)j, 0f - num), (Rectangle?)new Rectangle(0, 0, 1, 1), new Color(new Vector4(1f, 0f, 1f, 0.5f)), 0f, default(Vector2), new Vector2(1f, num), (SpriteEffects)0, 1f);
			num = bytesSent[j];
			num /= 5f;
			sprite.Draw(Game1.nullTex, val + new Vector2((float)j, 0f - num), (Rectangle?)new Rectangle(0, 0, 1, 1), new Color(new Vector4(1f, 0.75f, 1f, 0.5f)), 0f, default(Vector2), new Vector2(1f, num), (SpriteEffects)0, 1f);
		}
		val.X -= 72f;
		for (int k = 0; k < bytesPerSecReceived.Length; k++)
		{
			float num2 = bytesPerSecReceived[k];
			num2 /= 100f;
			sprite.Draw(Game1.nullTex, val + new Vector2((float)k, 0f - num2), (Rectangle?)new Rectangle(0, 0, 1, 1), new Color(new Vector4(0f, 1f, 1f, 0.5f)), 0f, default(Vector2), new Vector2(1f, num2), (SpriteEffects)0, 1f);
			num2 = bytesReceived[k];
			num2 /= 5f;
			sprite.Draw(Game1.nullTex, val + new Vector2((float)k, 0f - num2), (Rectangle?)new Rectangle(0, 0, 1, 1), new Color(new Vector4(0.75f, 1f, 1f, 0.5f)), 0f, default(Vector2), new Vector2(1f, num2), (SpriteEffects)0, 1f);
		}
	}
}
