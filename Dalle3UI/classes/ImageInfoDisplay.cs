using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace Dalle3UI
{
    public class ImageInfoDisplay
    {
        public ImageInfo ImageInfo { get; set; }
        public string CleanFilename => ImageInfo?.CleanFilename;
        public string Size => ImageInfo?.Size;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
