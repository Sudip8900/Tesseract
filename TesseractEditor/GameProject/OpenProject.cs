using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using TesseractEditor.Utilities;

namespace TesseractEditor.GameProject;

[DataContract]
public class ProjectData
{
    [DataMember]
    public string ProjectName { get; set; }

    [DataMember]
    public string ProjectPath { get; set; }

    [DataMember]
    public DateTime Date { get; set; }

    public string FullPath
    {
        get => Path.Combine(
            ProjectPath,
            ProjectName + Project.Extension
        );
    }

    public byte[] Screenshot { get; set; }
}

[DataContract]
public class ProjectDataList
{
    [DataMember]
    public List<ProjectData> Projects { get; set; } = new();
}

class OpenProject
{
    private static readonly string _applicationDataPath =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "TesseractEditor"
        );

    private static readonly string _projectDataPath;

    private static readonly ObservableCollection<ProjectData> _projects =
        new();

    public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

    private static void ReadProjectData()
    {
        if (!File.Exists(_projectDataPath))
            return;

        var data = Serializer.FromFile<ProjectDataList>(_projectDataPath);

        if (data?.Projects == null)
            return;

        var projects = data.Projects
            .OrderByDescending(x => x.Date)
            .ToList();

        _projects.Clear();

        foreach (var project in projects)
        {
            // Make sure the project itself still exists
            if (!File.Exists(project.FullPath))
                continue;

            // Screenshot is optional
            string screenshotPath = Path.Combine(
                project.ProjectPath,
                ".Tesseract",
                "TemplatePLH.png"
            );

            if (File.Exists(screenshotPath))
            {
                try
                {
                    project.Screenshot = File.ReadAllBytes(screenshotPath);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(
                        $"Failed to load screenshot for {project.ProjectName}: {e.Message}"
                    );

                    project.Screenshot = null;
                }
            }

            _projects.Add(project);
        }
    }

    private static void WriteProjectData()
    {
        var projects = _projects
            .OrderByDescending(x => x.Date)
            .ToList();

        Serializer.ToFile(
            new ProjectDataList
            {
                Projects = projects
            },
            _projectDataPath
        );
    }

    public static Project Open(ProjectData projectData)
    {
        ReadProjectData();

        var project = _projects.FirstOrDefault(
            x => x.FullPath.Equals(
                projectData.FullPath,
                StringComparison.OrdinalIgnoreCase
            )
        );

        if (project != null)
        {
            project.Date = DateTime.Now;
        }
        else
        {
            project = projectData;
            project.Date = DateTime.Now;

            _projects.Add(project);
        }

        WriteProjectData();

        return null;
    }

    static OpenProject()
    {
        try
        {
            if (!Directory.Exists(_applicationDataPath))
                Directory.CreateDirectory(_applicationDataPath);

            _projectDataPath = Path.Combine(
                _applicationDataPath,
                "ProjectData.xml"
            );

            Projects = new ReadOnlyObservableCollection<ProjectData>(
                _projects
            );

            ReadProjectData();
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}