using System.Formats.Asn1;
using System.Runtime.CompilerServices;

namespace System.Security.Cryptography;

internal static class Oids
{
	[CompilerGenerated]
	private static Oid _003CRsaOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CEcPublicKeyOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003Csecp256r1Oid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003Csecp384r1Oid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003Csecp521r1Oid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CContentTypeOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CDocumentDescriptionOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CDocumentNameOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CLocalKeyIdOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CMessageDigestOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CSigningTimeOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CPkcs9ExtensionRequestOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CBasicConstraints2Oid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CEnhancedKeyUsageOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CKeyUsageOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CAuthorityKeyIdentifierOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CSubjectKeyIdentifierOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CSubjectAltNameOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CAuthorityInformationAccessOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CCrlNumberOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CCrlDistributionPointsOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CCommonNameOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CCountryOrRegionNameOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CLocalityNameOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CStateOrProvinceNameOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003COrganizationOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003COrganizationalUnitOid_003Ek__BackingField;

	[CompilerGenerated]
	private static Oid _003CEmailAddressOid_003Ek__BackingField;

	internal static Oid RsaOid => _003CRsaOid_003Ek__BackingField ?? (_003CRsaOid_003Ek__BackingField = InitializeOid("1.2.840.113549.1.1.1"));

	internal static Oid EcPublicKeyOid => _003CEcPublicKeyOid_003Ek__BackingField ?? (_003CEcPublicKeyOid_003Ek__BackingField = InitializeOid("1.2.840.10045.2.1"));

	internal static Oid secp256r1Oid => _003Csecp256r1Oid_003Ek__BackingField ?? (_003Csecp256r1Oid_003Ek__BackingField = new Oid("1.2.840.10045.3.1.7", "nistP256"));

	internal static Oid secp384r1Oid => _003Csecp384r1Oid_003Ek__BackingField ?? (_003Csecp384r1Oid_003Ek__BackingField = new Oid("1.3.132.0.34", "nistP384"));

	internal static Oid secp521r1Oid => _003Csecp521r1Oid_003Ek__BackingField ?? (_003Csecp521r1Oid_003Ek__BackingField = new Oid("1.3.132.0.35", "nistP521"));

	internal static Oid ContentTypeOid => _003CContentTypeOid_003Ek__BackingField ?? (_003CContentTypeOid_003Ek__BackingField = InitializeOid("1.2.840.113549.1.9.3"));

	internal static Oid DocumentDescriptionOid => _003CDocumentDescriptionOid_003Ek__BackingField ?? (_003CDocumentDescriptionOid_003Ek__BackingField = InitializeOid("1.3.6.1.4.1.311.88.2.2"));

	internal static Oid DocumentNameOid => _003CDocumentNameOid_003Ek__BackingField ?? (_003CDocumentNameOid_003Ek__BackingField = InitializeOid("1.3.6.1.4.1.311.88.2.1"));

	internal static Oid LocalKeyIdOid => _003CLocalKeyIdOid_003Ek__BackingField ?? (_003CLocalKeyIdOid_003Ek__BackingField = InitializeOid("1.2.840.113549.1.9.21"));

	internal static Oid MessageDigestOid => _003CMessageDigestOid_003Ek__BackingField ?? (_003CMessageDigestOid_003Ek__BackingField = InitializeOid("1.2.840.113549.1.9.4"));

	internal static Oid SigningTimeOid => _003CSigningTimeOid_003Ek__BackingField ?? (_003CSigningTimeOid_003Ek__BackingField = InitializeOid("1.2.840.113549.1.9.5"));

	internal static Oid Pkcs9ExtensionRequestOid => _003CPkcs9ExtensionRequestOid_003Ek__BackingField ?? (_003CPkcs9ExtensionRequestOid_003Ek__BackingField = InitializeOid("1.2.840.113549.1.9.14"));

	internal static Oid BasicConstraints2Oid => _003CBasicConstraints2Oid_003Ek__BackingField ?? (_003CBasicConstraints2Oid_003Ek__BackingField = InitializeOid("2.5.29.19"));

	internal static Oid EnhancedKeyUsageOid => _003CEnhancedKeyUsageOid_003Ek__BackingField ?? (_003CEnhancedKeyUsageOid_003Ek__BackingField = InitializeOid("2.5.29.37"));

	internal static Oid KeyUsageOid => _003CKeyUsageOid_003Ek__BackingField ?? (_003CKeyUsageOid_003Ek__BackingField = InitializeOid("2.5.29.15"));

	internal static Oid AuthorityKeyIdentifierOid => _003CAuthorityKeyIdentifierOid_003Ek__BackingField ?? (_003CAuthorityKeyIdentifierOid_003Ek__BackingField = InitializeOid("2.5.29.35"));

	internal static Oid SubjectKeyIdentifierOid => _003CSubjectKeyIdentifierOid_003Ek__BackingField ?? (_003CSubjectKeyIdentifierOid_003Ek__BackingField = InitializeOid("2.5.29.14"));

	internal static Oid SubjectAltNameOid => _003CSubjectAltNameOid_003Ek__BackingField ?? (_003CSubjectAltNameOid_003Ek__BackingField = InitializeOid("2.5.29.17"));

	internal static Oid AuthorityInformationAccessOid => _003CAuthorityInformationAccessOid_003Ek__BackingField ?? (_003CAuthorityInformationAccessOid_003Ek__BackingField = InitializeOid("1.3.6.1.5.5.7.1.1"));

	internal static Oid CrlNumberOid => _003CCrlNumberOid_003Ek__BackingField ?? (_003CCrlNumberOid_003Ek__BackingField = InitializeOid("2.5.29.20"));

	internal static Oid CrlDistributionPointsOid => _003CCrlDistributionPointsOid_003Ek__BackingField ?? (_003CCrlDistributionPointsOid_003Ek__BackingField = InitializeOid("2.5.29.31"));

	internal static Oid CommonNameOid => _003CCommonNameOid_003Ek__BackingField ?? (_003CCommonNameOid_003Ek__BackingField = InitializeOid("2.5.4.3"));

	internal static Oid CountryOrRegionNameOid => _003CCountryOrRegionNameOid_003Ek__BackingField ?? (_003CCountryOrRegionNameOid_003Ek__BackingField = InitializeOid("2.5.4.6"));

	internal static Oid LocalityNameOid => _003CLocalityNameOid_003Ek__BackingField ?? (_003CLocalityNameOid_003Ek__BackingField = InitializeOid("2.5.4.7"));

	internal static Oid StateOrProvinceNameOid => _003CStateOrProvinceNameOid_003Ek__BackingField ?? (_003CStateOrProvinceNameOid_003Ek__BackingField = InitializeOid("2.5.4.8"));

	internal static Oid OrganizationOid => _003COrganizationOid_003Ek__BackingField ?? (_003COrganizationOid_003Ek__BackingField = InitializeOid("2.5.4.10"));

	internal static Oid OrganizationalUnitOid => _003COrganizationalUnitOid_003Ek__BackingField ?? (_003COrganizationalUnitOid_003Ek__BackingField = InitializeOid("2.5.4.11"));

	internal static Oid EmailAddressOid => _003CEmailAddressOid_003Ek__BackingField ?? (_003CEmailAddressOid_003Ek__BackingField = InitializeOid("1.2.840.113549.1.9.1"));

	private static Oid InitializeOid(string oidValue)
	{
		Oid oid = new Oid(oidValue, null);
		_ = oid.FriendlyName;
		return oid;
	}

	internal static Oid GetSharedOrNewOid(ref AsnValueReader asnValueReader)
	{
		Oid sharedOrNullOid = GetSharedOrNullOid(ref asnValueReader);
		if (sharedOrNullOid != null)
		{
			return sharedOrNullOid;
		}
		return new Oid(asnValueReader.ReadObjectIdentifier(), null);
	}

	internal static Oid GetSharedOrNullOid(ref AsnValueReader asnValueReader, Asn1Tag? expectedTag = null)
	{
		Asn1Tag asn1Tag = asnValueReader.PeekTag();
		if (asn1Tag.IsConstructed)
		{
			return null;
		}
		Asn1Tag valueOrDefault = expectedTag.GetValueOrDefault(Asn1Tag.ObjectIdentifier);
		if (!asn1Tag.HasSameClassAndValue(valueOrDefault))
		{
			return null;
		}
		ReadOnlySpan<byte> readOnlySpan = asnValueReader.PeekContentBytes();
		int length = readOnlySpan.Length;
		Oid oid;
		if (length != 3)
		{
			if (length == 9 && readOnlySpan[0] == 42)
			{
				byte b = readOnlySpan[1];
				if (b == 134)
				{
					byte b2 = readOnlySpan[2];
					if (b2 == 72 && readOnlySpan[3] == 134 && readOnlySpan[4] == 247 && readOnlySpan[5] == 13 && readOnlySpan[6] == 1 && readOnlySpan[7] == 9 && readOnlySpan[8] == 1)
					{
						oid = EmailAddressOid;
						goto IL_01a7;
					}
				}
			}
		}
		else if (readOnlySpan[0] == 85)
		{
			byte b = readOnlySpan[1];
			if (b == 4)
			{
				switch (readOnlySpan[2])
				{
				case 3:
					break;
				case 6:
					goto IL_0175;
				case 7:
					goto IL_017d;
				case 8:
					goto IL_0185;
				case 10:
					goto IL_018d;
				case 11:
					goto IL_0195;
				default:
					goto IL_01a5;
				}
				oid = CommonNameOid;
				goto IL_01a7;
			}
			if (b == 29)
			{
				byte b2 = readOnlySpan[2];
				if (b2 == 20)
				{
					oid = CrlNumberOid;
					goto IL_01a7;
				}
			}
		}
		goto IL_01a5;
		IL_0185:
		oid = StateOrProvinceNameOid;
		goto IL_01a7;
		IL_0195:
		oid = OrganizationalUnitOid;
		goto IL_01a7;
		IL_01a7:
		Oid oid2 = oid;
		if (oid2 != null)
		{
			asnValueReader.ReadEncodedValue();
		}
		return oid2;
		IL_017d:
		oid = LocalityNameOid;
		goto IL_01a7;
		IL_01a5:
		oid = null;
		goto IL_01a7;
		IL_018d:
		oid = OrganizationOid;
		goto IL_01a7;
		IL_0175:
		oid = CountryOrRegionNameOid;
		goto IL_01a7;
	}

	internal static bool ValueEquals(this Oid oid, Oid other)
	{
		if (oid == other)
		{
			return true;
		}
		if (other == null)
		{
			return false;
		}
		if (oid.Value != null)
		{
			return oid.Value.Equals(other.Value);
		}
		return false;
	}
}
