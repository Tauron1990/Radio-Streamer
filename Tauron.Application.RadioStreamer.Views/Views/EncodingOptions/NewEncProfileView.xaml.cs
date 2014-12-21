using System;
using Tauron.Application.RadioStreamer.Contracts;
using Tauron.Application.Views;

namespace Tauron.Application.RadioStreamer.Views.EncodingOptions
{
    /// <summary>
    /// Interaktionslogik für NewEncProfileView.xaml
    /// </summary>
    [ExportWindow(AppConstants.NewEncodingProfileView)]
    public partial class NewEncProfileView
    {
        public NewEncProfileView()
        {
            InitializeComponent();
        }

        private void NewEncProfileView_OnInitialized(object sender, EventArgs e)
        {
            NameTextBox.Focus();
        }
    }
}
