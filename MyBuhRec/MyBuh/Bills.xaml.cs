using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Pickers;
using Windows.Storage; 
using Windows.Storage.Streams;
using Microsoft.Hawaii.Ocr.Client;
using Microsoft.Hawaii.Ocr;

using MyBuh.OcrUtils;
// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyBuh
{



    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class BillsPage : Page
    {
        private class TextArea
        {
            public int Top, Left, Width, Height;
            public string Text;


            public TextArea(int Top, int Left, int Width, int Height, string Text)
            {
                this.Top = Top;
                this.Left = Left;
                this.Width = Width;
                this.Height = Height;
                this.Text = Text;
            }
        }

        public class BillItem
        {
            string Name;
            double Count;
            double Price;

            public BillItem(string Name, double Count, double Price)
            {
                this.Name = Name;
                this.Count = Count;
                this.Price = Price;
            }
        }

        TextBox SelectedBox;
        StorageFile CurImgFile;
        List<TextArea> Areas;
        List<BillItem> BillItems;

        public BillsPage()
        {
            SelectedBox = tbxName;
            this.InitializeComponent();
            Areas = new List<TextArea>();
        }

        /// <summary>
        /// Вызывается перед отображением этой страницы во фрейме.
        /// </summary>
        /// <param name="e">Данные о событиях, описывающие, каким образом была достигнута эта страница.  Свойство Parameter
        /// обычно используется для настройки страницы.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));

        }

        /// <summary>
        /// Распознавание
        /// </summary>
        private async void StartOcrConversion()
        {
            Areas.Clear();
            using (IRandomAccessStream fileStream = await CurImgFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                OcrService.RecognizeImageAsync(
                    HawaiiClient.HawaiiApplicationId,
                    await OcrClientUtils.GetPhotoBits(fileStream),
                    async (output) =>
                    {
                        // This section defines the body of what is known as an anonymous method. 
                        // This anonymous method is the callback method 
                        // called on the completion of the OCR process.
                        // Using Dispatcher.BeginInvoke ensures that 
                        // OnOcrCompleted is invoked on the Main UI thread.
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => OnOcrCompleted(output));
                    });
            }

        }

        /// <summary>
        /// Результат распознавания
        /// </summary>
        /// <param name="result">OCR has been completed</param>
        private async void OnOcrCompleted(
                Microsoft.Hawaii.Ocr.Client.OcrServiceResult result)
        {

            // Ensure the stream is disposed once the image is loaded 
            using (IRandomAccessStream fileStream = await CurImgFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
            {
                // Set the image source to the selected bitmap 
                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(fileStream);

                for (int i = 0; i < result.OcrResult.OcrTexts.Count; i++)
                    for (int j = 0; j < result.OcrResult.OcrTexts[i].Words.Count; j++ )
                    {
                        OcrWord CurWord = result.OcrResult.OcrTexts[i].Words[j];
                        string[] Cords = CurWord.Box.Split(',');
                        Areas.Add(new TextArea(Convert.ToInt32(Cords[1]), Convert.ToInt32(Cords[0]),
                            Convert.ToInt32(Cords[2]), Convert.ToInt32(Cords[3]), CurWord.Text));
                    }

                imgBill.Source = bitmapImage;
            }
        }


        /// <summary>
        /// Обработчик открытия картинки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Добавить_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".bmp");

            CurImgFile = await openPicker.PickSingleFileAsync();
 
            // Ensure a file was selected 
            if (CurImgFile != null)
            {
                StartOcrConversion();
            }
        }

        private void tbxName_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedBox = tbxName;
        }

        private void tbxCount_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedBox = tbxCount;
        }


        private void tbxPrice_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedBox = tbxPrice;
        }

        private void tbxPrice_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnLeaveName_Click(object sender, RoutedEventArgs e)
        {
            tbxName.Text = "";

        }

        private void btnLeaveCount_Click(object sender, RoutedEventArgs e)
        {
            tbxCount.Text = "";
        }

        private void btnLeavePrice_Click(object sender, RoutedEventArgs e)
        {
            tbxPrice.Text = "";
        }

        private void btnLeavePrice_Copy_Click(object sender, RoutedEventArgs e)
        {
            BillItems.Add(new BillItem(tbxName.Text, Convert.ToDouble(tbxCount.Text), Convert.ToDouble(tbxPrice.Text)));
        }
    }
}
