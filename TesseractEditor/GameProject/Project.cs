using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace TesseractEditor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelbase
    {
        public static string Extension { get; } = ".tesseract";
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Path { get; private set; }
        
        public string FullPah => $"{Path}{Name}{Extension}";
    
        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> _scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; }

        public Project(string name,  string path)
        {
            Name = name;
            Path = path;
            
            _scenes.Add(new Scene(this, "Default Scene"));
        }
    }
}