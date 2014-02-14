﻿using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer.Views.RadioManager.Controls
{
    /// <summary>
    /// Interaktionslogik für RadioCreateView.xaml
    /// </summary>
    [ExportView(AppConstants.RadioCreateViewModel)]
    public partial class RadioCreateView
    {
        public RadioCreateView()
        {
            InitializeComponent();
        }
    }
}