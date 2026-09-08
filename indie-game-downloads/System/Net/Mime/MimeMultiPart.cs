using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Mime;

internal sealed class MimeMultiPart : MimeBasePart
{
	private static int s_boundary;

	[CompilerGenerated]
	private Collection<MimeBasePart> _003CParts_003Ek__BackingField;

	internal MimeMultiPartType MimeMultiPartType
	{
		set
		{
			if (value > MimeMultiPartType.Related || value < MimeMultiPartType.Mixed)
			{
				throw new NotSupportedException(value.ToString());
			}
			SetType(value);
		}
	}

	internal Collection<MimeBasePart> Parts => _003CParts_003Ek__BackingField ?? (_003CParts_003Ek__BackingField = new Collection<MimeBasePart>());

	internal MimeMultiPart(MimeMultiPartType type)
	{
		MimeMultiPartType = type;
	}

	private void SetType(MimeMultiPartType type)
	{
		base.ContentType.MediaType = "multipart/" + type.ToString().ToLowerInvariant();
		base.ContentType.Boundary = GetNextBoundary();
	}

	internal override async Task SendAsync<TIOAdapter>(BaseWriter writer, bool allowUnicode, CancellationToken cancellationToken = default(CancellationToken))
	{
		PrepareHeaders(allowUnicode);
		writer.WriteHeaders(base.Headers, allowUnicode);
		Stream outputStream = writer.GetContentStream();
		MimeWriter mimeWriter = new MimeWriter(outputStream, base.ContentType.Boundary);
		foreach (MimeBasePart part in Parts)
		{
			await part.SendAsync<TIOAdapter>(mimeWriter, allowUnicode, cancellationToken).ConfigureAwait(continueOnCapturedContext: false);
		}
		mimeWriter.Close();
		outputStream.Close();
	}

	internal static string GetNextBoundary()
	{
		int value = Interlocked.Increment(ref s_boundary) - 1;
		return $"--boundary_{(uint)value}_{Guid.NewGuid()}";
	}
}
