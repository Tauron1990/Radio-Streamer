using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tauron.Application.RadioStreamer.PlugIns
{
    public sealed class AggregareFileAdder : FileAdder
    {
        public AggregareFileAdder(params  FileAdder[] fileAdder)
        {
            FileAdder = new List<FileAdder>(fileAdder);
        }

        public List<FileAdder> FileAdder { get; }

        public override string AddFile(string effiectivePath, Stream stream)
        {
            if (!stream.CanSeek)
                throw new InvalidOperationException("Stream not Seekable");

            var returns = new List<string>();

            foreach (var fileAdder in FileAdder)
            {
                returns.Add(fileAdder.AddFile(effiectivePath, stream));

                stream.Position = 0;
            }

            returns.Reverse();
            return returns.FirstOrDefault(r => !string.IsNullOrEmpty(r));
        }

        public override void Dispose()
        {
            foreach (var fileAdder in FileAdder)
            {
                fileAdder.Dispose();
            }
        }
    }
}
