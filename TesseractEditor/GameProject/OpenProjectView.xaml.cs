using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TesseractEditor.GameProject
{
    public partial class OpenProjectView : UserControl
    {
        public OpenProjectView()
        {
            InitializeComponent();
        }

        private void On_Open_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenSelectedProject();
        }
        
        private void OnListBoxItem_Double_Click(object sender, MouseButtonEventArgs e)
        {
            OpenSelectedProject();
        }

        private void OpenSelectedProject()
        {
            var project = OpenProject.Open(ProjectsListBox.SelectedItem as ProjectData);
            bool dialogResult = false;
            var win = Window.GetWindow(this);
            if (project != null)
            {
                dialogResult = true;
            }

            if (win == null) return;
            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}