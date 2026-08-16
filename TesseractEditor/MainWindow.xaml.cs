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

namespace TesseractEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnMainWindowLoaded; // Unsubscribe from the Loaded event to prevent multiple calls
            // Open the Project Browser window when the main window is loaded
            var projectBrowser = new GameProject.ProjectBrowser();
            if (projectBrowser.ShowDialog() == false)
            {
                Application.Current.Shutdown(); // Close the application if the user cancels the project selection
            }
            else
            {

            }
        }
    }
}
