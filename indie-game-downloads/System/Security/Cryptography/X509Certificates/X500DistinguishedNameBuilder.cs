using System.Collections.Generic;
using System.Formats.Asn1;
using System.Runtime.CompilerServices;
using System.Text;

namespace System.Security.Cryptography.X509Certificates;

public sealed class X500DistinguishedNameBuilder
{
	private readonly List<byte[]> _encodedComponents = new List<byte[]>();

	private readonly AsnWriter _writer = new AsnWriter(AsnEncodingRules.DER);

	public void Add(string oidValue, string value, UniversalTagNumber? stringEncodingType = null)
	{
		ArgumentNullException.ThrowIfNull(value, "value");
		ArgumentException.ThrowIfNullOrEmpty(oidValue, "oidValue");
		UniversalTagNumber andValidateTagNumber = GetAndValidateTagNumber(stringEncodingType);
		EncodeComponent(oidValue, value.AsSpan(), andValidateTagNumber, "value");
	}

	public void Add(Oid oid, string value, UniversalTagNumber? stringEncodingType = null)
	{
		ArgumentNullException.ThrowIfNull(oid, "oid");
		ArgumentNullException.ThrowIfNull(value, "value");
		ArgumentException.ThrowIfNullOrEmpty(oid.Value, "oid.Value");
		UniversalTagNumber andValidateTagNumber = GetAndValidateTagNumber(stringEncodingType);
		EncodeComponent(oid.Value, value.AsSpan(), andValidateTagNumber, "value");
	}

	public void AddEmailAddress(string emailAddress)
	{
		ArgumentException.ThrowIfNullOrEmpty(emailAddress, "emailAddress");
		if (emailAddress.Length > 255)
		{
			throw new ArgumentException(System.SR.Argument_X500_EmailTooLong, "emailAddress");
		}
		EncodeComponent("1.2.840.113549.1.9.1", emailAddress.AsSpan(), UniversalTagNumber.IA5String, "emailAddress");
	}

	public void AddCommonName(string commonName)
	{
		ArgumentException.ThrowIfNullOrEmpty(commonName, "commonName");
		EncodeComponent("2.5.4.3", commonName.AsSpan(), UniversalTagNumber.UTF8String, "commonName");
	}

	public void AddLocalityName(string localityName)
	{
		ArgumentException.ThrowIfNullOrEmpty(localityName, "localityName");
		EncodeComponent("2.5.4.7", localityName.AsSpan(), UniversalTagNumber.UTF8String, "localityName");
	}

	public void AddCountryOrRegion(string twoLetterCode)
	{
		ArgumentException.ThrowIfNullOrEmpty(twoLetterCode, "twoLetterCode");
		ReadOnlySpan<char> source = twoLetterCode.AsSpan();
		if (twoLetterCode.Length != 2 || !char.IsAsciiLetter(twoLetterCode[0]) || !char.IsAsciiLetter(twoLetterCode[1]))
		{
			throw new ArgumentException(System.SR.Argument_X500_InvalidCountryOrRegion, "twoLetterCode");
		}
		Span<char> span = stackalloc char[2];
		source.ToUpperInvariant(span);
		EncodeComponent("2.5.4.6", span, UniversalTagNumber.PrintableString, "twoLetterCode");
	}

	public void AddOrganizationName(string organizationName)
	{
		ArgumentException.ThrowIfNullOrEmpty(organizationName, "organizationName");
		EncodeComponent("2.5.4.10", organizationName.AsSpan(), UniversalTagNumber.UTF8String, "organizationName");
	}

	public void AddOrganizationalUnitName(string organizationalUnitName)
	{
		ArgumentException.ThrowIfNullOrEmpty(organizationalUnitName, "organizationalUnitName");
		EncodeComponent("2.5.4.11", organizationalUnitName.AsSpan(), UniversalTagNumber.UTF8String, "organizationalUnitName");
	}

	public void AddStateOrProvinceName(string stateOrProvinceName)
	{
		ArgumentException.ThrowIfNullOrEmpty(stateOrProvinceName, "stateOrProvinceName");
		EncodeComponent("2.5.4.8", stateOrProvinceName.AsSpan(), UniversalTagNumber.UTF8String, "stateOrProvinceName");
	}

	public void AddDomainComponent(string domainComponent)
	{
		ArgumentException.ThrowIfNullOrEmpty(domainComponent, "domainComponent");
		EncodeComponent("0.9.2342.19200300.100.1.25", domainComponent.AsSpan(), UniversalTagNumber.IA5String, "domainComponent");
	}

	public X500DistinguishedName Build()
	{
		_writer.Reset();
		using (_writer.PushSequence())
		{
			for (int num = _encodedComponents.Count - 1; num >= 0; num--)
			{
				_writer.WriteEncodedValue(_encodedComponents[num]);
			}
		}
		return _writer.Encode<X500DistinguishedName>((Func<ReadOnlySpan<byte>, X500DistinguishedName>)((ReadOnlySpan<byte> encoded) => new X500DistinguishedName(encoded)));
	}

	private void EncodeComponent(string oid, ReadOnlySpan<char> value, UniversalTagNumber stringEncodingType, [CallerArgumentExpression("value")] string paramName = null)
	{
		_writer.Reset();
		using (_writer.PushSetOf())
		{
			using (_writer.PushSequence())
			{
				_writer.WriteObjectIdentifier(oid);
				try
				{
					_writer.WriteCharacterString(stringEncodingType, value);
				}
				catch (EncoderFallbackException)
				{
					throw new ArgumentException(System.SR.Format(System.SR.Argument_Asn1_InvalidStringContents, stringEncodingType), paramName);
				}
			}
		}
		_encodedComponents.Add(_writer.Encode());
	}

	private static UniversalTagNumber GetAndValidateTagNumber(UniversalTagNumber? stringEncodingType)
	{
		switch (stringEncodingType)
		{
		case null:
			return UniversalTagNumber.UTF8String;
		case UniversalTagNumber.UTF8String:
		case UniversalTagNumber.NumericString:
		case UniversalTagNumber.PrintableString:
		case UniversalTagNumber.TeletexString:
		case UniversalTagNumber.IA5String:
		case UniversalTagNumber.VisibleString:
		case UniversalTagNumber.BMPString:
			return stringEncodingType.GetValueOrDefault();
		default:
			throw new ArgumentException(System.SR.Argument_Asn1_InvalidCharacterString, "stringEncodingType");
		}
	}
}
