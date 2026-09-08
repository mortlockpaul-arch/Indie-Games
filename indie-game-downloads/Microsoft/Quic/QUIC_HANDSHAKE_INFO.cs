namespace Microsoft.Quic;

internal struct QUIC_HANDSHAKE_INFO
{
	internal QUIC_TLS_PROTOCOL_VERSION TlsProtocolVersion;

	internal QUIC_CIPHER_ALGORITHM CipherAlgorithm;

	internal int CipherStrength;

	internal QUIC_HASH_ALGORITHM Hash;

	internal int HashStrength;

	internal QUIC_KEY_EXCHANGE_ALGORITHM KeyExchangeAlgorithm;

	internal int KeyExchangeStrength;

	internal QUIC_CIPHER_SUITE CipherSuite;
}
