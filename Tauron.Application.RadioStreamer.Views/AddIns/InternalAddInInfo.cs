#region Usings

using System;
using Tauron.Application.Modules;
using Tauron.Application.RadioStreamer.Contracts.Core;
using Tauron.JetBrains.Annotations;

#endregion

namespace Tauron.Application.RadioStreamer.Views.AddIns
{
    public sealed class ModuleInfo
    {
        private readonly AddinDescription _description;

        public ModuleInfo([NotNull] AddinDescription description)
        {
            if (description == null) throw new ArgumentNullException("description");

            _description = description;
        }

        [NotNull]
        public string Description { get { return _description.Description; } }

        [NotNull]
        public string Version { get { return _description.Version.ToString(); } }

        [NotNull]
        public string Name { get { return _description.Name; } }
    }

    public class InternalAddInInfo
    {
        private readonly Action<IPackInfo> _action;
        private readonly IPackInfo _packInfo;

        public InternalAddInInfo(bool canPerformAction, [NotNull] Action<IPackInfo> action, 
            [NotNull] IPackInfo packInfo, [NotNull] string buttonLabel)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (packInfo == null) throw new ArgumentNullException("packInfo");
            if (buttonLabel == null) throw new ArgumentNullException("buttonLabel");

            _action = action;
            _packInfo = packInfo;
            CanPerformAction = canPerformAction;
            ButtonLabel = buttonLabel;
        }

        public bool CanPerformAction { get; set; }
        public string ButtonLabel { get; set; }

        [CommandTarget]
        public void UnInstall()
        {
            _action(_packInfo);
        }

        [NotNull]
        public string Name { get { return _packInfo.Name; } }
        [NotNull]
        public string Description { get { return _packInfo.Description; } }
        [NotNull]
        public string Version { get { return _packInfo.Version.ToString(); } }
    }
}