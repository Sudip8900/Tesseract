using System.Diagnostics;
using System.IO.Packaging;
using System.Runtime.Serialization;

namespace TesseractEditor.GameProject;

[DataContract]
public class Scene : ViewModelbase
{
    private string _name;

    [DataMember]
    public string Name
    {
        get => _name;
        set
        {
            if(_name==value) return;
            _name = value;
            OnPropertyChanged(nameof(Name));
        }
    }
    
    [DataMember]
    public Project Project { get; private set; }

    private bool _isActive;
    [DataMember]
    public bool IsActive
    {
        get => _isActive;
        set
        {
            if(_isActive==value) return;
            _isActive = value;
            OnPropertyChanged(nameof(IsActive));
        }
    }
    
    public Scene(Project project,  string name)
    {
        Debug.Assert(project != null);
        Project = project;
        Name = name;
    }
}