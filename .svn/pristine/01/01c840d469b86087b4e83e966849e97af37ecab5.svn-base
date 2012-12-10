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
using System.Collections.ObjectModel;
using BuhLib;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyBuh
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class BalansPage : Page
    {
        private ObservableCollection<MoneyFlowUnit> _money = new ObservableCollection<MoneyFlowUnit>();

        public BalansPage()
        {
            this.InitializeComponent();

            //заглушко!
            _money.Add(new MoneyFlowUnit("Текущий", 827.00, 2, "Магазин, продукты", DateTime.Parse("09/12/2012")));
            _money.Add(new MoneyFlowUnit("Текущий", 25.30, 2, "Булочка с сыром", DateTime.Parse("10/12/2012")));
            _money.Add(new MoneyFlowUnit("Текущий", 600, 2, "Интернет", DateTime.Parse("10/12/2012")));
            _money.Add(new MoneyFlowUnit("Текущий", 125.00, 2, "Телефон", DateTime.Parse("10/12/2012")));
            _money.Add(new MoneyFlowUnit("Текущий", 427.00, 2, "Магазин, продукты", DateTime.Parse("10/12/2012")));


            listview.ItemsSource = _money;
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DateTime date = new DateTime(Int32.Parse(dtxt3.Text), Int32.Parse(dtxt2.Text), Int32.Parse(dtxt1.Text));
            double summ = double.Parse(SummTxt.Text);
            string Comment = Commenttxt.Text;
            _money.Add(new MoneyFlowUnit("Текущий",summ,1,Comment,date));
        }
    }

    
}
