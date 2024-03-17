using Dalle3UI.classes;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.UI.Xaml.Controls.Primitives;

using System.Collections.ObjectModel;

using static Dalle3UI.Statics;

namespace Dalle3UI
{
    public partial class MainPage : ContentPage
    {
        public ObservableCollection<ImageInfoDisplay> ImageInfoDisplays { get; set; }
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
                        Filename = Path.GetFileName(f).Split(".png")[0].Replace("_", " "),
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
            ImageInfoDisplays = new ObservableCollection<ImageInfoDisplay>();
            LoadImages();
            this.BindingContext = this;
        }

        public void OnClickReloadButton(object sender, EventArgs e)
        {
            LoadImages();
        }

        private ImageInfoDisplay _lastSelectedImage = null;
        public void ImageListImageSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is ImageInfoDisplay selectedImageInfoDisplay)
            {
                if (_lastSelectedImage != null)
                {
                    _lastSelectedImage.IsSelected = false;
                }

                selectedImageInfoDisplay.IsSelected = true;

                _lastSelectedImage = selectedImageInfoDisplay;

                ImageViewer.Source = ImageSource.FromFile(selectedImageInfoDisplay.ImageInfo.FilePath);
            }
        }
    }
}
