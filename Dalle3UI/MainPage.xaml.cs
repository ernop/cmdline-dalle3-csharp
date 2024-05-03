
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.UI.Xaml.Controls.Primitives;

using System.Collections.ObjectModel;
using System.Diagnostics;

using static Dalle3UI.Statics;

namespace Dalle3UI
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<ImageInfoDisplay> ImageInfoDisplays { get; set; } = new ObservableCollection<ImageInfoDisplay>();
        public static string MainFolder = "d:/proj/dalle3/output/efef/part2";

        public void LoadImages()
        {
            var addCount = 0;
            foreach (var f in Directory.GetFiles(MainFolder, "*.png"))
            {
                var fullPath = Path.GetFullPath(f);
                if (ImageInfoDisplays.FirstOrDefault(el => el.ImageInfo.FilePath == fullPath) == null)
                {
                    var ii = new ImageInfo
                    {
                        CleanFilename = Path.GetFileName(f).Split(".png")[0].Replace("_", " "),
                        Size = (new FileInfo(fullPath).Length / 1024).ToString(),
                        FilePath = fullPath
                    };
                    var dis = new ImageInfoDisplay
                    {
                        ImageInfo = ii
                    };

                    ImageInfoDisplays.Add(dis);
                    addCount++;
                }

                if (addCount > 50)
                {
                    break;
                }
            }

            // You would load your images into the Images collection here
            this.BindingContext = this;
        }

        public MainPage()
        {
            InitializeComponent();
            LoadImages();
            this.BindingContext = this;
        }

        public void OnClickReloadButton(object sender, EventArgs e)
        {
            LoadImages();
        }

        private ImageInfoDisplay _selectedItem;
        public ImageInfoDisplay SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    // Handle selection change here
                    Debug.WriteLine($"Selected item: {_selectedItem?.CleanFilename}");
                }
            }
        }

        public void ImageListImageSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is ImageInfoDisplay selectedImageInfoDisplay)
            {
                ImageViewer.Source = ImageSource.FromFile(selectedImageInfoDisplay.ImageInfo.FilePath);
            }
        }
    }
}
