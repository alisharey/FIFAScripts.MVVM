using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using FIFAScripts.MVVM.ViewModels;

namespace FIFAScripts.MVVM.Views
{
    /// <summary>
    /// Interaction logic for GitHubView.xaml
    /// </summary>
    public partial class GitHubView : UserControl
    {
        public GitHubView()
        {
            InitializeComponent();
        }

        private void GitHub_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var sInfo = new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true,
            };
            System.Diagnostics.Process.Start(sInfo);
            
        }
    }
}
