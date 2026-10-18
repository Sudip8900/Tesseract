using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows;
using TesseractEditor.Utilities;

namespace TesseractEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelbase
    {
        public static string Extension { get; } = ".tesseract";
        [DataMember]
        public string Name { get; private set; } = "New Project";
        [DataMember]
        public string Path { get; private set; }
        
        public string FullPah => $"{Path}{Name}{Extension}";
    
        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        private Scene _activeScene;
        public Scene ActiveScene
        {
            get => _activeScene;
            set
            {
                if (_activeScene != value)
                {
                    _activeScene = value;
                    foreach (var scene in _scenes)
                    {
                        scene.IsActive = scene == value;
                    }
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }
        
        public static Project Current = Application.Current.MainWindow.DataContext as Project;

        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }
        
        public void Unload() {}

        public static void Save(Project project)
        {
            Serializer.ToFile(project,  project.FullPah);
        }

        public Scene AddScene()
        {
            var sceneNumber = 1;
            while (_scenes.Any(scene => scene.Name == $"Scene {sceneNumber}"))
            {
                sceneNumber++;
            }

            var scene = new Scene(this, $"Scene {sceneNumber}");
            _scenes.Add(scene);
            ActiveScene = scene;
            return scene;
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (_scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(_scenes);
                OnPropertyChanged(nameof(Scenes));
            }
            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);
        }

        public Project(string name,  string path)
        {
            Name = name;
            Path = path;
            
            OnDeserialized(new StreamingContext());
        }
    }
}