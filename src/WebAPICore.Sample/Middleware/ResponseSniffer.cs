using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace  WebAPICore.Sample.Middleware
{
    public class ResponseSniffer : Stream
    {
        private readonly Stream ResponseStream;

        public MemoryStream RecordStream { get; set; }

        #region Implements of Stream
        public override bool CanRead
        {
            get { return ResponseStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return ResponseStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return ResponseStream.CanWrite; }
        }

        public override void Flush()
        {
            ResponseStream.Flush();
        }

        public override long Length
        {
            get { return ResponseStream.Length; }
        }

        public override long Position
        {
            get
            {
                return ResponseStream.Position;
            }
            set
            {
                ResponseStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return ResponseStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            ResponseStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            RecordStream.Write(buffer, offset, count);
            ResponseStream.Write(buffer, offset, count);
        }
        #endregion

        public ResponseSniffer(Stream stream)
        {
            RecordStream = new MemoryStream();
            ResponseStream = stream;
        }
    }
}
