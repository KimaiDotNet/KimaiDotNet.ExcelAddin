using System;
using System.Collections.Generic;
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

namespace MarkZither.KimaiDotNet.ExcelAddin
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public string OWAAdress { get; set; }
        public SettingsWindow()
        {
            ViewModels.Calendar.CategoryViewModel vm = new ViewModels.Calendar.CategoryViewModel();
            if (vm.CloseAction == null)
            {
                vm.CloseAction = new Action(() => Do(this, sw => sw.Close()));
            }
            DataContext = vm;

            InitializeComponent();
            if(!string.IsNullOrEmpty(Settings.Default.OWAUrl))
            {
                txtOWAUrl.Text = Settings.Default.OWAUrl;
            }
            if (!string.IsNullOrEmpty(Settings.Default.OWAUsername))
            {
                txtOWAUsername.Text = Settings.Default.OWAUsername;
            }
        }
        public static void Do<TControl>(TControl control, Action<TControl> action) where TControl : Control
        {
            control.Dispatcher.BeginInvoke(action, control);
        }
    }
}
