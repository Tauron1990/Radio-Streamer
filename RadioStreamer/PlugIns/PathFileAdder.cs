using System;
using System.Collections.Generic;
using System.IO;

namespace Tauron.Application.RadioStreamer.PlugIns
{
    public sealed class PathFileAdder : FileAdder
    {
        private readonly string _root;
        private Action<PathFileAdder> _disposeAction;

        private readonly List<string> _files; 

        public PathFileAdder(string root, Action<PathFileAdder> disposeAction = null)
        {
            _root = root;
            _disposeAction = disposeAction;
            _files = new List<string>();
        }

        public IEnumerable<string> Files
        {
            get { return _files; }
        }

        public override string AddFile(string effiectivePath, Stream stream)
        {
            string newFile = Path.Combine(_root, effiectivePath);
            string newdic = Path.GetDirectoryName(newFile);

            if(string.IsNullOrEmpty(newdic)) return null;

            if (!Directory.Exists(newdic))
                Directory.CreateDirectory(newdic);

            using (var fileStream = new FileStream(newFile, FileMode.Create))
            {
                stream.CopyTo(fileStream);
            }

            _files.Add(newFile);

            return newFile;
        }

        public override void Dispose()
        {
            if(_disposeAction == null)
                return;

            _disposeAction(this);
            _disposeAction = null;
        }
    }
}