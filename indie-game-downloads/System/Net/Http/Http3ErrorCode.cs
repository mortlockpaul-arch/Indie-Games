namespace System.Net.Http;

internal enum Http3ErrorCode : long
{
	NoError = 256L,
	ProtocolError = 257L,
	InternalError = 258L,
	StreamCreationError = 259L,
	ClosedCriticalStream = 260L,
	UnexpectedFrame = 261L,
	FrameError = 262L,
	ExcessiveLoad = 263L,
	IdError = 264L,
	SettingsError = 265L,
	MissingSettings = 266L,
	RequestRejected = 267L,
	RequestCancelled = 268L,
	RequestIncomplete = 269L,
	MessageError = 270L,
	ConnectError = 271L,
	VersionFallback = 272L,
	QPackDecompressionFailed = 512L,
	QPackEncoderStreamError = 513L,
	QPackDecoderStreamError = 514L
}
