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
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Animation;
using System.Collections.ObjectModel;
using BuhLib;



// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyBuh
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private Worker Logic = new Worker();
        private ObservableCollection<SingleTile> _Tiles;

        public MainPage()
        {
            this.InitializeComponent();
            _Tiles = new ObservableCollection<SingleTile>();
            _Tiles.Add(new SingleTile("Мои счета", @"/Assets/666777762.jpg"));
            _Tiles.Add(new SingleTile("Бюджет", @"/Assets/86ce30d4-fb1b-40ee-bd9e-83043b91b0fd.jpg"));
            _Tiles.Add(new SingleTile("Баланс", @"/Assets/18.jpg"));
            _Tiles.Add(new SingleTile("Информация", @"/Assets/1348047057_kurs_valyut.jpg"));
            _Tiles.Add(new SingleTile("Категории", @"/Assets/booksclock.jpg"));            

            gvMain.ItemsSource = _Tiles;

            
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
            Button thisbtn = (Button)sender;
            string name = thisbtn.Content.ToString();
            switch (name)
            {
                case "Мои счета":
                    this.Frame.Navigate(typeof(AccountPage));
                    break;
                case "Бюджет":
                    this.Frame.Navigate(typeof(BudgetPage));
                    break;
                case "Баланс":
                    this.Frame.Navigate(typeof(BalansPage));
                    break;
                case "Информация":
                    this.Frame.Navigate(typeof(AccountPage));
                    break;
            }


            
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
            Application.Current.Exit();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            
 
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {


            Popup rashod = new Popup();
            rashod.Width = 150;
            rashod.Height = 250;
            rashod.IsLightDismissEnabled = true;

            Grid panel = new Grid();
            Button button = new Button();
            button.VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom;
            button.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center;
            
            panel.Width = 150;
            panel.Height = 250;
            panel.Transitions = new Windows.UI.Xaml.Media.Animation.TransitionCollection();
            panel.Transitions.Add(new PopupThemeTransition());
            panel.Children.Add(button);

            rashod.Child = panel;

            var button2 = (Button)(sender);
            var transform =  button2.TransformToVisual(this);
            var point = transform.TransformPoint(new Point());

            
            rashod.HorizontalOffset = point.X;
            rashod.VerticalOffset = Window.Current.CoreWindow.Bounds.Bottom - 150 - panel.Height;
            rashod.IsOpen = true;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
         
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            
            
        }

        private string GetFormattedString(string[] InputString)
        {
            return String.Format("{1}:10,{2}:10", InputString[0], InputString[1]);
        }
    }
}
