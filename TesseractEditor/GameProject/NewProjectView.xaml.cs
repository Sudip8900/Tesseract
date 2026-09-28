using System.Windows;
using System.Windows.Controls;

namespace TesseractEditor.GameProject
{
    public partial class NewProjectView : UserControl
    {
        public NewProjectView()
        {
            InitializeComponent();
        }

        private void onCreate_Button_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as NewProject;
            var projectPath = vm.CreateProject(TemplateListBox.SelectedItem as ProjectTemplate);
            bool dialogResult = false;
            var win = Window.GetWindow(this);
            if (!string.IsNullOrEmpty(projectPath))
            {
                dialogResult = true;
                var project = OpenProject.Open(new ProjectData() {ProjectName = vm.ProjectName, ProjectPath = projectPath});
            }

            if (win == null) return;
            win.DialogResult = dialogResult;
            win.Close();
        }
    }
}