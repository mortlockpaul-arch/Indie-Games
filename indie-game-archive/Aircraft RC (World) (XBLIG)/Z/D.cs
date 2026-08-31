using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using X;

namespace Z
{
	internal class D
	{
		private static int HCB;

		private static float HC_0002;

		private static double HC_0012;

		private static StringBuilder HCH = new StringBuilder();

		private void _00023(StringBuilder P_0, int P_1, int P_2)
		{
			if (P_1 <= 0)
			{
				P_0.Append("0");
				return;
			}
			int length = P_0.Length;
			int num = 0;
			while (P_1 > 0)
			{
				int num2 = P_1 / 10;
				switch (P_1 - num2 * 10)
				{
				case 0:
					P_0.Insert(length, "0");
					break;
				case 1:
					P_0.Insert(length, "1");
					break;
				case 2:
					P_0.Insert(length, "2");
					break;
				case 3:
					P_0.Insert(length, "3");
					break;
				case 4:
					P_0.Insert(length, "4");
					break;
				case 5:
					P_0.Insert(length, "5");
					break;
				case 6:
					P_0.Insert(length, "6");
					break;
				case 7:
					P_0.Insert(length, "7");
					break;
				case 8:
					P_0.Insert(length, "8");
					break;
				case 9:
					P_0.Insert(length, "9");
					break;
				}
				num++;
				if (num == P_2 && num != 0 && P_1 > 0)
				{
					P_0.Insert(length, ".");
				}
				P_1 = num2;
			}
			if (num < P_2)
			{
				for (int i = num; i < P_2; i++)
				{
					P_0.Insert(length, "0");
				}
				P_0.Insert(length, ".");
			}
		}

		internal Vector2 b(SpriteBatch P_0, SpriteFont P_1, ref Vector2 P_2, Vector2 P_3, Color P_4)
		{
			P_0.DrawString(P_1, HCH, P_2, P_4, 0f, Vector2.Zero, P_3, SpriteEffects.None, 0f);
			P_2.Y += 14f * P_3.Y;
			return new Vector2((float)HCH.Length * 8f * P_3.X, 14f * P_3.Y);
		}

		internal void _0002E(string P_0)
		{
			HCH.Length = 0;
			HCH.Append(P_0);
		}

		internal void _00029(string P_0, int P_1)
		{
			HCH.Length = 0;
			HCH.Append(P_0);
			HCH.Append(": ");
			_00023(HCH, P_1, 0);
		}

		private void _00029(string P_0, float P_1)
		{
			HCH.Length = 0;
			HCH.Append(P_0);
			HCH.Append(": ");
			_00023(HCH, (int)(P_1 * 100f), 2);
		}

		internal void _0002_0004(string P_0, GameTime P_1, bool P_2)
		{
			double totalSeconds = P_1.TotalGameTime.TotalSeconds;
			if (totalSeconds - HC_0012 >= 0.5)
			{
				HC_0002 = (float)HCB / (float)(totalSeconds - HC_0012);
				HCB = 0;
				HC_0012 = totalSeconds;
			}
			_00029(P_0, HC_0002);
			if (P_2)
			{
				HCB++;
			}
		}

		public override string ToString()
		{
			return HCH.ToString();
		}
	}
}
namespace z
{
	internal sealed class D : _0002
	{
		private const int HCB = 1;

		private bool HC_0002 = true;

		private bool HC_0012;

		private global::X._0012 HCH;

		public override string KeyExchangeAlgorithm => "RSA-PKCS1-KeyEx";

		public override int KeySize
		{
			get
			{
				if (HCH == null)
				{
					return KeySizeValue;
				}
				return HCH.KeySize;
			}
		}

		public bool PersistKeyInCsp
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		internal bool PublicOnly => HCH.PublicOnly;

		public override string SignatureAlgorithm => "http://www.w3.org/2000/09/xmldsig#rsa-sha1";

		public D()
		{
			Ht(1024);
		}

		private void Ht(int P_0)
		{
			LegalKeySizesValue = new _000F[1];
			LegalKeySizesValue[0] = new _000F(384, 16384, 8);
			base.KeySize = P_0;
			HCH = new global::X._0012(KeySize);
		}

		~D()
		{
			Dispose(disposing: false);
		}

		public byte[] Decrypt(byte[] rgb, bool fOAEP)
		{
			_0012 obj = null;
			obj = ((!fOAEP) ? ((_0012)new _0003(HCH)) : ((_0012)new _0011(HCH)));
			return obj.DecryptKeyExchange(rgb);
		}

		public override byte[] DecryptValue(byte[] rgb)
		{
			if (!HCH.IsCrtPossible)
			{
				throw new _7("Incomplete private key - missing CRT.");
			}
			return HCH.DecryptValue(rgb);
		}

		public override byte[] EncryptValue(byte[] rgb)
		{
			return HCH.EncryptValue(rgb);
		}

		public override K ExportParameters(bool includePrivateParameters)
		{
			if (includePrivateParameters && !HC_0002)
			{
				throw new _7("cannot export private key");
			}
			return HCH.ExportParameters(includePrivateParameters);
		}

		public override void ImportParameters(K parameters)
		{
			HCH.ImportParameters(parameters);
		}

		private Z HB(object P_0)
		{
			if (P_0 == null)
			{
				throw new ArgumentNullException("halg");
			}
			Z z = null;
			if (P_0 is string)
			{
				return new _0013();
			}
			if (P_0 is Z)
			{
				return (Z)P_0;
			}
			if (P_0 is Type)
			{
				return (Z)Activator.CreateInstance((Type)P_0);
			}
			throw new ArgumentException("halg");
		}

		public byte[] SignHash(byte[] rgbHash, string str)
		{
			if (rgbHash == null)
			{
				throw new ArgumentNullException("rgbHash");
			}
			Z hash = new _0013();
			return global::X._0002.Sign_v15(this, hash, rgbHash);
		}

		public bool VerifyData(byte[] buffer, object halg, byte[] signature)
		{
			if (signature == null)
			{
				throw new ArgumentNullException("signature");
			}
			Z z = HB(halg);
			byte[] hashValue = z.ComputeHash(buffer);
			return global::X._0002.Verify_v15(this, z, hashValue, signature);
		}

		public bool VerifyHash(byte[] rgbHash, string str, byte[] rgbSignature)
		{
			if (rgbHash == null)
			{
				throw new ArgumentNullException("rgbHash");
			}
			if (rgbSignature == null)
			{
				throw new ArgumentNullException("rgbSignature");
			}
			Z hash = new _0013();
			return global::X._0002.Verify_v15(this, hash, rgbHash, rgbSignature);
		}

		protected override void Dispose(bool disposing)
		{
			if (!HC_0012)
			{
				if (HCH != null)
				{
					HCH.Clear();
				}
				HC_0012 = true;
			}
		}

		private void HQ(object P_0, EventArgs P_1)
		{
		}

		public void ImportCspBlob(byte[] rawData)
		{
			if (rawData == null)
			{
				throw new ArgumentNullException("rawData");
			}
			_0002 obj = global::X.B.FromCapiKeyBlob(rawData);
			if (obj is D)
			{
				K parameters = obj.ExportParameters(!(obj as D).PublicOnly);
				ImportParameters(parameters);
				return;
			}
			try
			{
				K parameters2 = obj.ExportParameters(include: true);
				ImportParameters(parameters2);
			}
			catch
			{
				K parameters3 = obj.ExportParameters(include: false);
				ImportParameters(parameters3);
			}
		}
	}
}
