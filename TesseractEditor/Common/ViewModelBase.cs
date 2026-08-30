using System.ComponentModel;
using System.Runtime.Serialization;

namespace TesseractEditor
{
    [DataContract(IsReference = true)]
    public class ViewModelbase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}