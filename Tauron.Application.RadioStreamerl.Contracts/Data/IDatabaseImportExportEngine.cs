using System;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Data
{
    public interface IDatabaseImportExportEngine
    {
        [NotNull]
        string FileFilter { get; }

        [NotNull]
        string DefaultExtension { get; }

        void ImportFiles([NotNull] string file, bool merge, [NotNull] ImportExportSettings settings, [NotNull] IProgress<ActiveProgress> progress);

        void ExportFiles([NotNull] string filename, [NotNull] ImportExportSettings settings, [NotNull] IProgress<ActiveProgress> progress);
    }
}