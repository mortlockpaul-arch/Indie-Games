using System.Collections.Specialized;
using System.IO;
using System.Net.Mail;

namespace System.Net.Mime;

internal abstract class BaseWriter
{
	protected readonly BufferBuilder _bufferBuilder;

	protected readonly Stream _stream;

	private readonly EventHandler _onCloseHandler;

	private readonly bool _shouldEncodeLeadingDots;

	private readonly int _lineLength;

	protected Stream _contentStream;

	protected bool _isInContent;

	protected BaseWriter(Stream stream, bool shouldEncodeLeadingDots)
	{
		ArgumentNullException.ThrowIfNull(stream, "stream");
		_stream = stream;
		_shouldEncodeLeadingDots = shouldEncodeLeadingDots;
		_onCloseHandler = OnClose;
		_bufferBuilder = new BufferBuilder();
		_lineLength = 76;
	}

	internal abstract void WriteHeaders(NameValueCollection headers, bool allowUnicode);

	internal void WriteHeader(string name, string value, bool allowUnicode)
	{
		ArgumentNullException.ThrowIfNull(name, "name");
		ArgumentNullException.ThrowIfNull(value, "value");
		if (_isInContent)
		{
			throw new InvalidOperationException(System.SR.MailWriterIsInContent);
		}
		CheckBoundary();
		_bufferBuilder.Append(name);
		_bufferBuilder.Append(": ");
		WriteAndFold(value, name.Length + 2, allowUnicode);
		_bufferBuilder.Append("\r\n"u8);
	}

	private void WriteAndFold(string value, int charsAlreadyOnLine, bool allowUnicode)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < value.Length; i++)
		{
			if (MailBnfHelper.IsFWSAt(value, i))
			{
				i += 2;
				_bufferBuilder.Append(value, num2, i - num2, allowUnicode);
				num2 = i;
				num = i;
				charsAlreadyOnLine = 0;
			}
			else if (i - num2 > _lineLength - charsAlreadyOnLine && num != num2)
			{
				_bufferBuilder.Append(value, num2, num - num2, allowUnicode);
				_bufferBuilder.Append("\r\n"u8);
				num2 = num;
				charsAlreadyOnLine = 0;
			}
			else if (value[i] == ' ' || value[i] == '\t')
			{
				num = i;
			}
		}
		if (value.Length - num2 > 0)
		{
			_bufferBuilder.Append(value, num2, value.Length - num2, allowUnicode);
		}
	}

	internal Stream GetContentStream()
	{
		if (_isInContent)
		{
			throw new InvalidOperationException(System.SR.MailWriterIsInContent);
		}
		_isInContent = true;
		CheckBoundary();
		_bufferBuilder.Append("\r\n"u8);
		Flush();
		return _contentStream = new ClosableStream(new EightBitStream(_stream, _shouldEncodeLeadingDots), _onCloseHandler);
	}

	protected void Flush()
	{
		if (_bufferBuilder.Length > 0)
		{
			_stream.Write(_bufferBuilder.GetBuffer(), 0, _bufferBuilder.Length);
			_bufferBuilder.Reset();
		}
	}

	internal abstract void Close();

	protected abstract void OnClose(object sender, EventArgs args);

	protected virtual void CheckBoundary()
	{
	}
}
