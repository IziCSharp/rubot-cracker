using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Extreme.Net
{
	internal class ProgressStreamContent : System.Net.Http.StreamContent
	{
		private class ProgressStream : Stream
		{
			private CancellationToken token;

			public Action<long> ReadCallback
			{
				get;
				set;
			}

			public Action<long> WriteCallback
			{
				get;
				set;
			}

			public Stream ParentStream
			{
				get;
				private set;
			}

			public override bool CanRead => ParentStream.CanRead;

			public override bool CanSeek => ParentStream.CanSeek;

			public override bool CanWrite => ParentStream.CanWrite;

			public override bool CanTimeout => ParentStream.CanTimeout;

			public override long Length => ParentStream.Length;

			public override long Position
			{
				get
				{
					return ParentStream.Position;
				}
				set
				{
					ParentStream.Position = value;
				}
			}

			public ProgressStream(Stream stream, CancellationToken token)
			{
				ParentStream = stream;
				this.token = token;
				ReadCallback = delegate
				{
				};
				WriteCallback = delegate
				{
				};
			}

			public override void Flush()
			{
				ParentStream.Flush();
			}

			public override Task FlushAsync(CancellationToken cancellationToken)
			{
				return ParentStream.FlushAsync(cancellationToken);
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				token.ThrowIfCancellationRequested();
				int num = ParentStream.Read(buffer, offset, count);
				ReadCallback(num);
				return num;
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				token.ThrowIfCancellationRequested();
				return ParentStream.Seek(offset, origin);
			}

			public override void SetLength(long value)
			{
				token.ThrowIfCancellationRequested();
				ParentStream.SetLength(value);
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				token.ThrowIfCancellationRequested();
				ParentStream.Write(buffer, offset, count);
				WriteCallback(count);
			}

			public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				token.ThrowIfCancellationRequested();
				CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, cancellationToken);
				int num = await ParentStream.ReadAsync(buffer, offset, count, cancellationTokenSource.Token);
				ReadCallback(num);
				return num;
			}

			public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
			{
				token.ThrowIfCancellationRequested();
				CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, cancellationToken);
				Task result = ParentStream.WriteAsync(buffer, offset, count, cancellationTokenSource.Token);
				WriteCallback(count);
				return result;
			}

			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					ParentStream.Dispose();
				}
			}
		}

		private long _totalBytes;

		private long _totalBytesExpected = -1L;

		private ProgressDelegate _progress;

		public ProgressDelegate Progress
		{
			get
			{
				return _progress;
			}
			set
			{
				if (value == null)
				{
					_progress = delegate
					{
					};
				}
				else
				{
					_progress = value;
				}
			}
		}

		public ProgressStreamContent(Stream stream, CancellationToken token)
			: this(new ProgressStream(stream, token))
		{
		}

		public ProgressStreamContent(Stream stream, int bufferSize)
			: this(new ProgressStream(stream, CancellationToken.None), bufferSize)
		{
		}

		private ProgressStreamContent(ProgressStream stream)
			: base(stream)
		{
			init(stream);
		}

		private ProgressStreamContent(ProgressStream stream, int bufferSize)
			: base(stream, bufferSize)
		{
			init(stream);
		}

		private void init(ProgressStream stream)
		{
			stream.ReadCallback = readBytes;
			Progress = delegate
			{
			};
		}

		private void reset()
		{
			_totalBytes = 0L;
		}

		private void readBytes(long bytes)
		{
			if (_totalBytesExpected == -1)
			{
				_totalBytesExpected = (base.Headers.ContentLength ?? (-1));
			}
			if (_totalBytesExpected == -1 && TryComputeLength(out long length))
			{
				_totalBytesExpected = ((length == 0L) ? (-1) : length);
			}
			_totalBytesExpected = Math.Max(-1L, _totalBytesExpected);
			_totalBytes += bytes;
			Progress(bytes, _totalBytes, _totalBytesExpected);
		}

		protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			reset();
			return base.SerializeToStreamAsync(stream, context);
		}

		protected override bool TryComputeLength(out long length)
		{
			bool result = base.TryComputeLength(out length);
			_totalBytesExpected = length;
			return result;
		}
	}
}
