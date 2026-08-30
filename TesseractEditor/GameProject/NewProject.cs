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

        public byte[] ScreenShots { get; set; }
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
                ValidateProjectPath();
                OnPropertyChanged(nameof(ProjectName));
            }
        }

        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\TesseractProjects\";

        public string ProjectPath
        {
            get => _projectPath;
            set
            {
                if (_projectPath == value) return;
                _projectPath = value;
                ValidateProjectPath();
                OnPropertyChanged(nameof(ProjectPath));
            }
        }

        private bool _isValid;

        public bool IsValid
        {
            get => _isValid;
            set
            {
                if (_isValid == value) return;
                _isValid = value;
                OnPropertyChanged(nameof(IsValid));
            }
        }

        private string _error;

        public string Error
        {
            get => _error;
            set
            {
                if (_error == value) return;
                _error = value;
                OnPropertyChanged(nameof(Error));
            }
        }

        private ObservableCollection<ProjectTemplate> _projectTemplates =
            new ObservableCollection<ProjectTemplate>();

        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        private bool ValidateProjectPath()
        {
            var path = ProjectPath;

            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !path.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                path += @"\";
            }

            path += $@"{ProjectName}\";

            IsValid = false;

            if (string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                Error = "Please enter a valid project name.";
            }
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                Error = "Please enter a valid project name.";
            }
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                Error = "Please enter a valid project path.";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                Error = "Please enter a valid project path.";
            }
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                Error = "Selected project folder already exists and not empty";
            }
            else
            {
                Error = string.Empty;
                IsValid = true;
            }

            return IsValid;
        }

        public string CreateProject(ProjectTemplate template)
        {
            ValidateProjectPath();
            if(!IsValid) return string.Empty;

            if (!ProjectPath.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !ProjectPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
            {
                ProjectPath += @"\";
            }

            var path = $@"{ProjectPath}{ProjectName}\";

            try
            {
                if(!Directory.Exists(path)) Directory.CreateDirectory(path);
                foreach (var folder in template.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path) ?? "",  folder)));
                }
                
                var dirInfo = new DirectoryInfo(path + @".Tesseract\");
                dirInfo.Attributes |= FileAttributes.Hidden;
                
                File.Copy(template.ScreenShotFilepath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "TemplatePLH.png")));

                var projectXml = File.ReadAllText(template.ProjectFilepath);
                projectXml = string.Format(projectXml, ProjectName, ProjectPath);
                var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                File.WriteAllText(projectPath, projectXml);
                return path;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                throw;
                //TODO:Log Error
                return string.Empty;
            }
        }

        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplates);

            try
            {
                var templateFiles = Directory.GetFiles(
                    _templatePath,
                    "template.xml",
                    SearchOption.AllDirectories);

                Debug.Assert(templateFiles.Any());

                foreach (var file in templateFiles)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(file);

                    template.ScreenShotFilepath = Path.GetFullPath(
                        Path.Combine(
                            Path.GetDirectoryName(file) ??
                            throw new InvalidOperationException(),
                            "TemplatePLH.png"));

                    template.ScreenShots = File.ReadAllBytes(template.ScreenShotFilepath);

                    template.ProjectFilepath = Path.GetFullPath(
                        Path.Combine(
                            Path.GetDirectoryName(file) ??
                            throw new InvalidOperationException(),
                            template.ProjectFile));

                    template.ProjectFilepath = Path.GetFullPath(
                        Path.Combine(
                            Path.GetDirectoryName(file) ??
                            throw new InvalidOperationException(),
                            template.ProjectFile));

                    _projectTemplates.Add(template);
                }

                ValidateProjectPath();
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