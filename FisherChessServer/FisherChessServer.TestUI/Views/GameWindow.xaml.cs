using FisherChessServer.TestUI.ViewModels;
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
using System.Windows.Shapes;

namespace FisherChessServer.TestUI.Views
{
    /// <summary> Interaction logic for MainWindow.xaml </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
            
            DataContext = new GameViewModel(this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult messageBoxResult = App.ShowMessage("Справді завершити гру?", true);
            if (messageBoxResult == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }
    }
}
