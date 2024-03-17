using System.ComponentModel;

namespace Dalle3UI.classes
{
    /// <summary>
    /// Info on an image in an image list based on a directory.
    /// </summary>
    public class ImageInfo
    {
        public string Filename { get; set; }
        public string Size { get; set; }
        public string FilePath { get; set; }

    }

    public class ImageInfoDisplay
    {
        public ImageInfo ImageInfo { get; set; }

        /// <summary>
        /// it HAS to be wrong to have this as part of the data object.
        /// Who the heck cares.
        /// </summary>
        private bool isSelected;
        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value)
                {
                    isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
