﻿using System;
using System.Windows.Controls;
using Tauron.Application.Ioc;
using Tauron.JetBrains.Annotations;

namespace Tauron.Application.RadioStreamer.Contracts.Player.Recording
{
    public class ExportEncodingEditor : ExportAttribute, IEncodingEditorMetadata
    {
        public ExportEncodingEditor([NotNull] string id) 
            : base(typeof (UserControl))
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            ContractName = "EncodingEditor";
            EncoderId = id;
        }

        public string EncoderId { get; private set; }

        public override string DebugName => "EncodingEditor" + EncoderId;

        protected override bool HasMetadata => true;
    }
}