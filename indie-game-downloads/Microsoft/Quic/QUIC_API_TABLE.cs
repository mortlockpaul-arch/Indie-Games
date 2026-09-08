namespace Microsoft.Quic;

internal struct QUIC_API_TABLE
{
	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void*, void> SetContext;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void*> GetContext;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void*, void*, void> SetCallbackHandler;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, uint, uint, void*, int> SetParam;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, uint, uint*, void*, int> GetParam;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_REGISTRATION_CONFIG*, QUIC_HANDLE**, int> RegistrationOpen;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void> RegistrationClose;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_CONNECTION_SHUTDOWN_FLAGS, ulong, void> RegistrationShutdown;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_BUFFER*, uint, QUIC_SETTINGS*, uint, void*, QUIC_HANDLE**, int> ConfigurationOpen;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void> ConfigurationClose;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_CREDENTIAL_CONFIG*, int> ConfigurationLoadCredential;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void*, QUIC_LISTENER_EVENT*, int>, void*, QUIC_HANDLE**, int> ListenerOpen;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void> ListenerClose;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_BUFFER*, uint, QuicAddr*, int> ListenerStart;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void> ListenerStop;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void*, QUIC_CONNECTION_EVENT*, int>, void*, QUIC_HANDLE**, int> ConnectionOpen;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void> ConnectionClose;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_CONNECTION_SHUTDOWN_FLAGS, ulong, void> ConnectionShutdown;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_HANDLE*, ushort, sbyte*, ushort, int> ConnectionStart;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_HANDLE*, int> ConnectionSetConfiguration;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_SEND_RESUMPTION_FLAGS, ushort, byte*, int> ConnectionSendResumptionTicket;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_STREAM_OPEN_FLAGS, delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void*, QUIC_STREAM_EVENT*, int>, void*, QUIC_HANDLE**, int> StreamOpen;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, void> StreamClose;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_STREAM_START_FLAGS, int> StreamStart;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_STREAM_SHUTDOWN_FLAGS, ulong, int> StreamShutdown;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_BUFFER*, uint, QUIC_SEND_FLAGS, void*, int> StreamSend;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, ulong, void> StreamReceiveComplete;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, byte, int> StreamReceiveSetEnabled;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, QUIC_BUFFER*, uint, QUIC_SEND_FLAGS, void*, int> DatagramSend;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, byte, int> ConnectionResumptionTicketValidationComplete;

	internal unsafe delegate* unmanaged[Cdecl]<QUIC_HANDLE*, byte, QUIC_TLS_ALERT_CODES, int> ConnectionCertificateValidationComplete;
}
