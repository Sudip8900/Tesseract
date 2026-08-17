using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using TesseractEditor.Utilities;

namespace TesseractEditor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }
        
        public byte[]  ScreenShots { get; set; }
        public string ScreenShotFilepath { get; set; }
        public string ProjectFilepath { get; set; }
    }

    public class NewProject : ViewModelbase
    {
        //TODO get the path from the installation location
        private readonly string _templatePath = @"..\..\TesseractEditor\ProjectTemplates";
        private string _projectName = "New Project";
        public string ProjectName
        {
            get => _projectName;
            set
            {
                if (_projectName == value) return;
                _projectName = value;
                OnPropertyChanged(nameof(ProjectName));
            }
        }
        
        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\Tesseract\";

        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                if (_projectPath == value) return;
                _projectPath = value;
                OnPropertyChanged(nameof(ProjectPath));
            }
        }
        
        private ObservableCollection<ProjectTemplate> _projectTemplates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);
            try
            {
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());
                foreach (var file in templateFiles)
                { 
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    template.ScreenShotFilepath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file) ?? throw new InvalidOperationException(), "TemplatePLH.png"));//TODO:change the placeholder image for the project templates
                    template.ScreenShots = File.ReadAllBytes(template.ScreenShotFilepath);
                    template.ProjectFilepath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file) ?? throw new InvalidOperationException(), template.ProjectFile));//TODO:change the placeholder image for the project templates
                    template.ProjectFilepath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file) ?? throw new InvalidOperationException(), template.ProjectFile));
                    _projectTemplates.Add(template);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
                //TODO: log error
            }
        }
    }
}