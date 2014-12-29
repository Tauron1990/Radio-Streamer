using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Database.IEEngine.Content
{
    [Serializable]
    public sealed class ExportScript
    {
        [NotNull]
        public string FileName { get; private set; }

        [NotNull]
        public byte[] Content { get; private set; }

        public ExportScript([NotNull] string fileName, [NotNull] byte[] content)
        {
            if (fileName == null) throw new ArgumentNullException("fileName");
            if (content == null) throw new ArgumentNullException("content");
            FileName = fileName;
            Content = content;
        }
    }
}
