using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;

namespace System.Net.Security;

internal abstract class SafeDeleteContext : SafeHandle
{
	internal global::Interop.SspiCli.CredHandle _handle;

	protected SafeFreeCredentials _EffectiveCredential;

	public override bool IsInvalid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (!base.IsClosed)
			{
				return _handle.IsZero;
			}
			return true;
		}
	}

	protected SafeDeleteContext()
		: base(IntPtr.Zero, ownsHandle: true)
	{
		_handle = default(global::Interop.SspiCli.CredHandle);
	}

	public override string ToString()
	{
		return _handle.ToString();
	}

	internal unsafe static int InitializeSecurityContext(ref SafeFreeCredentials inCredentials, ref SafeDeleteSslContext refContext, string targetName, global::Interop.SspiCli.ContextFlags inFlags, global::Interop.SspiCli.Endianness endianness, ref InputSecurityBuffers inSecBuffers, ref ProtocolToken outToken, ref global::Interop.SspiCli.ContextFlags outFlags)
	{
		ArgumentNullException.ThrowIfNull(inCredentials, "inCredentials");
		global::Interop.SspiCli.SecBufferDesc secBufferDesc = new global::Interop.SspiCli.SecBufferDesc(inSecBuffers.Count);
		global::Interop.SspiCli.SecBufferDesc outputBuffer = new global::Interop.SspiCli.SecBufferDesc(1);
		bool flag = (inFlags & global::Interop.SspiCli.ContextFlags.AllocateMemory) != 0;
		int result = -1;
		bool flag2 = true;
		if (refContext != null)
		{
			flag2 = refContext._handle.IsZero;
		}
		nint num = IntPtr.Zero;
		try
		{
			Span<global::Interop.SspiCli.SecBuffer> span = stackalloc global::Interop.SspiCli.SecBuffer[3];
			fixed (global::Interop.SspiCli.SecBuffer* ptr = span)
			{
				void* pBuffers = ptr;
				fixed (byte* ptr2 = inSecBuffers._item0.Token)
				{
					void* pvBuffer = ptr2;
					fixed (byte* ptr3 = inSecBuffers._item1.Token)
					{
						void* pvBuffer2 = ptr3;
						fixed (byte* ptr4 = inSecBuffers._item2.Token)
						{
							void* pvBuffer3 = ptr4;
							secBufferDesc.pBuffers = pBuffers;
							if (inSecBuffers.Count > 2)
							{
								span[2].BufferType = inSecBuffers._item2.Type;
								if (inSecBuffers._item2.UnmanagedToken != null)
								{
									span[2].pvBuffer = inSecBuffers._item2.UnmanagedToken.DangerousGetHandle();
									span[2].cbBuffer = ((ChannelBinding)inSecBuffers._item2.UnmanagedToken).Size;
								}
								else
								{
									span[2].cbBuffer = inSecBuffers._item2.Token.Length;
									span[2].pvBuffer = (nint)pvBuffer3;
								}
							}
							if (inSecBuffers.Count > 1)
							{
								span[1].BufferType = inSecBuffers._item1.Type;
								if (inSecBuffers._item1.UnmanagedToken != null)
								{
									span[1].pvBuffer = inSecBuffers._item1.UnmanagedToken.DangerousGetHandle();
									span[1].cbBuffer = ((ChannelBinding)inSecBuffers._item1.UnmanagedToken).Size;
								}
								else
								{
									span[1].cbBuffer = inSecBuffers._item1.Token.Length;
									span[1].pvBuffer = (nint)pvBuffer2;
								}
							}
							if (inSecBuffers.Count > 0)
							{
								span[0].BufferType = inSecBuffers._item0.Type;
								if (inSecBuffers._item0.UnmanagedToken != null)
								{
									span[0].pvBuffer = inSecBuffers._item0.UnmanagedToken.DangerousGetHandle();
									span[0].cbBuffer = ((ChannelBinding)inSecBuffers._item0.UnmanagedToken).Size;
								}
								else
								{
									span[0].cbBuffer = inSecBuffers._item0.Token.Length;
									span[0].pvBuffer = (nint)pvBuffer;
								}
							}
							fixed (byte* payload = outToken.Payload)
							{
								global::Interop.SspiCli.SecBuffer secBuffer = default(global::Interop.SspiCli.SecBuffer);
								outputBuffer.pBuffers = &secBuffer;
								secBuffer.cbBuffer = outToken.Size;
								secBuffer.BufferType = SecurityBufferType.SECBUFFER_TOKEN;
								secBuffer.pvBuffer = ((outToken.Payload == null || outToken.Size == 0) ? IntPtr.Zero : ((nint)payload));
								if ((refContext == null || refContext.IsInvalid) && flag2)
								{
									refContext?.Dispose();
									refContext = new SafeDeleteSslContext();
								}
								fixed (char* targetName2 = targetName)
								{
									result = MustRunInitializeSecurityContext(ref inCredentials, flag2, (byte*)targetName2, inFlags, endianness, &secBufferDesc, refContext, ref outputBuffer, ref outFlags, null);
									if (flag)
									{
										num = secBuffer.pvBuffer;
									}
									if (flag && secBuffer.cbBuffer > 0)
									{
										outToken.EnsureAvailableSpace(secBuffer.cbBuffer);
										new Span<byte>((void*)secBuffer.pvBuffer, secBuffer.cbBuffer).CopyTo(outToken.AvailableSpan);
									}
									outToken.Size = secBuffer.cbBuffer;
									if (inSecBuffers.Count > 1 && span[1].BufferType == SecurityBufferType.SECBUFFER_EXTRA && inSecBuffers._item1.Type == SecurityBufferType.SECBUFFER_EMPTY)
									{
										inSecBuffers._item1.Type = span[1].BufferType;
										inSecBuffers._item1.Token = inSecBuffers._item0.Token.Slice(inSecBuffers._item0.Token.Length - span[1].cbBuffer);
									}
								}
							}
						}
					}
				}
			}
		}
		finally
		{
			if (num != IntPtr.Zero)
			{
				global::Interop.SspiCli.FreeContextBuffer(num);
			}
		}
		return result;
	}

	private unsafe static int MustRunInitializeSecurityContext(ref SafeFreeCredentials inCredentials, bool isContextAbsent, byte* targetName, global::Interop.SspiCli.ContextFlags inFlags, global::Interop.SspiCli.Endianness endianness, global::Interop.SspiCli.SecBufferDesc* inputBuffer, SafeDeleteContext outContext, ref global::Interop.SspiCli.SecBufferDesc outputBuffer, ref global::Interop.SspiCli.ContextFlags attributes, SafeFreeContextBuffer handleTemplate)
	{
		int num = -2146893055;
		bool success = false;
		bool success2 = false;
		try
		{
			inCredentials.DangerousAddRef(ref success);
			outContext.DangerousAddRef(ref success2);
			global::Interop.SspiCli.CredHandle credentialHandle = inCredentials._handle;
			global::Interop.SspiCli.CredHandle credHandle = outContext._handle;
			void* ptr = (credHandle.IsZero ? null : (&credHandle));
			isContextAbsent = ptr == null;
			num = global::Interop.SspiCli.InitializeSecurityContextW(ref credentialHandle, ptr, targetName, inFlags, 0, endianness, inputBuffer, 0, ref outContext._handle, ref outputBuffer, ref attributes, out var _);
		}
		finally
		{
			if (outContext._EffectiveCredential != inCredentials && (num & 0x80000000u) == 0L)
			{
				outContext._EffectiveCredential?.DangerousRelease();
				outContext._EffectiveCredential = inCredentials;
			}
			else if (success)
			{
				inCredentials.DangerousRelease();
			}
			if (success2)
			{
				outContext.DangerousRelease();
			}
		}
		if (handleTemplate != null)
		{
			handleTemplate.Set(((global::Interop.SspiCli.SecBuffer*)outputBuffer.pBuffers)->pvBuffer);
			if (handleTemplate.IsInvalid)
			{
				handleTemplate.SetHandleAsInvalid();
			}
		}
		if (isContextAbsent && (num & 0x80000000u) != 0L)
		{
			outContext._handle.SetToInvalid();
		}
		return num;
	}

	internal unsafe static int AcceptSecurityContext(ref SafeFreeCredentials inCredentials, ref SafeDeleteSslContext refContext, global::Interop.SspiCli.ContextFlags inFlags, global::Interop.SspiCli.Endianness endianness, ref InputSecurityBuffers inSecBuffers, ref ProtocolToken outToken, ref global::Interop.SspiCli.ContextFlags outFlags)
	{
		ArgumentNullException.ThrowIfNull(inCredentials, "inCredentials");
		global::Interop.SspiCli.SecBufferDesc secBufferDesc = new global::Interop.SspiCli.SecBufferDesc(inSecBuffers.Count);
		global::Interop.SspiCli.SecBufferDesc outputBuffer = new global::Interop.SspiCli.SecBufferDesc(2);
		bool flag = (inFlags & global::Interop.SspiCli.ContextFlags.AllocateMemory) != 0;
		int result = -1;
		bool flag2 = true;
		if (refContext != null)
		{
			flag2 = refContext._handle.IsZero;
		}
		Span<global::Interop.SspiCli.SecBuffer> span = stackalloc global::Interop.SspiCli.SecBuffer[2];
		span[1].pvBuffer = IntPtr.Zero;
		try
		{
			Span<global::Interop.SspiCli.SecBuffer> span2 = stackalloc global::Interop.SspiCli.SecBuffer[3];
			fixed (global::Interop.SspiCli.SecBuffer* ptr = span2)
			{
				void* pBuffers = ptr;
				fixed (global::Interop.SspiCli.SecBuffer* ptr2 = span)
				{
					void* pBuffers2 = ptr2;
					fixed (byte* ptr3 = inSecBuffers._item0.Token)
					{
						void* pvBuffer = ptr3;
						fixed (byte* ptr4 = inSecBuffers._item1.Token)
						{
							void* pvBuffer2 = ptr4;
							fixed (byte* ptr5 = inSecBuffers._item2.Token)
							{
								void* pvBuffer3 = ptr5;
								secBufferDesc.pBuffers = pBuffers;
								if (inSecBuffers.Count > 2)
								{
									span2[2].BufferType = inSecBuffers._item2.Type;
									if (inSecBuffers._item2.UnmanagedToken != null)
									{
										span2[2].pvBuffer = inSecBuffers._item2.UnmanagedToken.DangerousGetHandle();
										span2[2].cbBuffer = ((ChannelBinding)inSecBuffers._item2.UnmanagedToken).Size;
									}
									else
									{
										span2[2].cbBuffer = inSecBuffers._item2.Token.Length;
										span2[2].pvBuffer = (nint)pvBuffer3;
									}
								}
								if (inSecBuffers.Count > 1)
								{
									span2[1].BufferType = inSecBuffers._item1.Type;
									if (inSecBuffers._item1.UnmanagedToken != null)
									{
										span2[1].pvBuffer = inSecBuffers._item1.UnmanagedToken.DangerousGetHandle();
										span2[1].cbBuffer = ((ChannelBinding)inSecBuffers._item1.UnmanagedToken).Size;
									}
									else
									{
										span2[1].cbBuffer = inSecBuffers._item1.Token.Length;
										span2[1].pvBuffer = (nint)pvBuffer2;
									}
								}
								if (inSecBuffers.Count > 0)
								{
									span2[0].BufferType = inSecBuffers._item0.Type;
									if (inSecBuffers._item0.UnmanagedToken != null)
									{
										span2[0].pvBuffer = inSecBuffers._item0.UnmanagedToken.DangerousGetHandle();
										span2[0].cbBuffer = ((ChannelBinding)inSecBuffers._item0.UnmanagedToken).Size;
									}
									else
									{
										span2[0].cbBuffer = inSecBuffers._item0.Token.Length;
										span2[0].pvBuffer = (nint)pvBuffer;
									}
								}
								fixed (byte* payload = outToken.Payload)
								{
									outputBuffer.pBuffers = pBuffers2;
									span[0].cbBuffer = outToken.Size;
									span[0].BufferType = SecurityBufferType.SECBUFFER_TOKEN;
									span[0].pvBuffer = ((outToken.Payload == null || outToken.Payload.Length == 0) ? IntPtr.Zero : ((nint)payload));
									span[1].cbBuffer = 0;
									span[1].BufferType = SecurityBufferType.SECBUFFER_ALERT;
									if ((refContext == null || refContext.IsInvalid) && flag2)
									{
										refContext = new SafeDeleteSslContext();
									}
									result = MustRunAcceptSecurityContext_SECURITY(ref inCredentials, flag2, &secBufferDesc, inFlags, endianness, refContext, ref outputBuffer, ref outFlags, null);
									int index = ((span[0].cbBuffer == 0 && span[1].cbBuffer > 0) ? 1 : 0);
									int cbBuffer = span[index].cbBuffer;
									if (flag && cbBuffer > 0)
									{
										outToken.EnsureAvailableSpace(cbBuffer);
										new Span<byte>((void*)span[index].pvBuffer, cbBuffer).CopyTo(outToken.AvailableSpan);
									}
									outToken.Size = cbBuffer;
									if (inSecBuffers.Count > 1 && span2[1].BufferType == SecurityBufferType.SECBUFFER_EXTRA && inSecBuffers._item1.Type == SecurityBufferType.SECBUFFER_EMPTY)
									{
										inSecBuffers._item1.Type = span2[1].BufferType;
										inSecBuffers._item1.Token = inSecBuffers._item0.Token.Slice(inSecBuffers._item0.Token.Length - span2[1].cbBuffer);
									}
								}
							}
						}
					}
				}
			}
		}
		finally
		{
			if (flag && span[0].pvBuffer != IntPtr.Zero)
			{
				global::Interop.SspiCli.FreeContextBuffer(span[0].pvBuffer);
			}
			if (span[1].pvBuffer != IntPtr.Zero)
			{
				global::Interop.SspiCli.FreeContextBuffer(span[1].pvBuffer);
			}
		}
		return result;
	}

	private unsafe static int MustRunAcceptSecurityContext_SECURITY(ref SafeFreeCredentials inCredentials, bool isContextAbsent, global::Interop.SspiCli.SecBufferDesc* inputBuffer, global::Interop.SspiCli.ContextFlags inFlags, global::Interop.SspiCli.Endianness endianness, SafeDeleteContext outContext, ref global::Interop.SspiCli.SecBufferDesc outputBuffer, ref global::Interop.SspiCli.ContextFlags outFlags, SafeFreeContextBuffer handleTemplate)
	{
		int num = -2146893055;
		bool success = false;
		bool success2 = false;
		try
		{
			inCredentials.DangerousAddRef(ref success);
			outContext.DangerousAddRef(ref success2);
			global::Interop.SspiCli.CredHandle credentialHandle = inCredentials._handle;
			global::Interop.SspiCli.CredHandle credHandle = outContext._handle;
			void* ptr = (credHandle.IsZero ? null : (&credHandle));
			isContextAbsent = ptr == null;
			num = global::Interop.SspiCli.AcceptSecurityContext(ref credentialHandle, ptr, inputBuffer, inFlags, endianness, ref outContext._handle, ref outputBuffer, ref outFlags, out var _);
		}
		finally
		{
			if (outContext._EffectiveCredential != inCredentials && (num & 0x80000000u) == 0L)
			{
				outContext._EffectiveCredential?.DangerousRelease();
				outContext._EffectiveCredential = inCredentials;
			}
			else if (success)
			{
				inCredentials.DangerousRelease();
			}
			if (success2)
			{
				outContext.DangerousRelease();
			}
		}
		if (handleTemplate != null)
		{
			handleTemplate.Set(((global::Interop.SspiCli.SecBuffer*)outputBuffer.pBuffers)->pvBuffer);
			if (handleTemplate.IsInvalid)
			{
				handleTemplate.SetHandleAsInvalid();
			}
		}
		if (isContextAbsent && (num & 0x80000000u) != 0L)
		{
			outContext._handle.SetToInvalid();
		}
		return num;
	}

	internal unsafe static int CompleteAuthToken(ref SafeDeleteSslContext refContext, in InputSecurityBuffer inSecBuffer)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(null, $"refContext = {refContext}", "CompleteAuthToken");
		}
		global::Interop.SspiCli.SecBufferDesc inputBuffers = new global::Interop.SspiCli.SecBufferDesc(1);
		int result = -2146893055;
		global::Interop.SspiCli.SecBuffer secBuffer = default(global::Interop.SspiCli.SecBuffer);
		inputBuffers.pBuffers = &secBuffer;
		fixed (byte* ptr = inSecBuffer.Token)
		{
			secBuffer.cbBuffer = inSecBuffer.Token.Length;
			secBuffer.BufferType = inSecBuffer.Type;
			secBuffer.pvBuffer = (inSecBuffer.Token.IsEmpty ? IntPtr.Zero : ((nint)ptr));
			global::Interop.SspiCli.CredHandle credHandle = ((refContext != null) ? refContext._handle : default(global::Interop.SspiCli.CredHandle));
			if ((refContext == null || refContext.IsInvalid) && credHandle.IsZero)
			{
				refContext = new SafeDeleteSslContext();
			}
			bool success = false;
			try
			{
				refContext.DangerousAddRef(ref success);
				result = global::Interop.SspiCli.CompleteAuthToken(credHandle.IsZero ? null : (&credHandle), ref inputBuffers);
			}
			finally
			{
				if (success)
				{
					refContext.DangerousRelease();
				}
			}
		}
		return result;
	}

	internal unsafe static int ApplyControlToken(ref SafeDeleteSslContext refContext, in SecurityBuffer inSecBuffer)
	{
		if (System.Net.NetEventSource.Log.IsEnabled())
		{
			System.Net.NetEventSource.Info(null, $"refContext = {refContext}, inSecBuffer = {inSecBuffer}", "ApplyControlToken");
		}
		int result = -2146893055;
		fixed (byte* token = inSecBuffer.token)
		{
			global::Interop.SspiCli.SecBufferDesc inputBuffers = new global::Interop.SspiCli.SecBufferDesc(1);
			global::Interop.SspiCli.SecBuffer secBuffer = default(global::Interop.SspiCli.SecBuffer);
			inputBuffers.pBuffers = &secBuffer;
			secBuffer.cbBuffer = inSecBuffer.size;
			secBuffer.BufferType = inSecBuffer.type;
			secBuffer.pvBuffer = ((inSecBuffer.unmanagedToken != null) ? inSecBuffer.unmanagedToken.DangerousGetHandle() : ((inSecBuffer.token == null || inSecBuffer.token.Length == 0) ? IntPtr.Zero : ((nint)(token + inSecBuffer.offset))));
			global::Interop.SspiCli.CredHandle credHandle = ((refContext != null) ? refContext._handle : default(global::Interop.SspiCli.CredHandle));
			if ((refContext == null || refContext.IsInvalid) && credHandle.IsZero)
			{
				refContext = new SafeDeleteSslContext();
			}
			bool success = false;
			try
			{
				refContext.DangerousAddRef(ref success);
				result = global::Interop.SspiCli.ApplyControlToken(credHandle.IsZero ? null : (&credHandle), ref inputBuffers);
			}
			finally
			{
				if (success)
				{
					refContext.DangerousRelease();
				}
			}
		}
		return result;
	}
}
