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
    public sealed partial class AccountPage : Page
    {
        
        private ObservableCollection<Account> _accs = new ObservableCollection<Account>();

        private Worker worker = new Worker();

        public AccountPage()
        {
            this.InitializeComponent();
            
            foreach (Account acc in worker.GetAccounts())
                _accs.Add(acc);
            AccList.ItemsSource = _accs;
            
        }

        /// <summary>
        /// Вызывается перед отображением этой страницы во фрейме.
        /// </summary>
        /// <param name="e">Данные о событиях, описывающие, каким образом была достигнута эта страница.  Свойство Parameter
        /// обычно используется для настройки страницы.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Назад_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void Добавить_Click_1(object sender, RoutedEventArgs e)
        {
            AddButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            SaveButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;

            DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            NameTxtbx.Text = "";
            AmountTxtb.Text = "";
            
        }

        private void addAcc_Button_click(object sender, RoutedEventArgs e)
        {
            string[] res = worker.CreateAccount(double.Parse(AmountTxtb.Text), NameTxtbx.Text).Split(';');
            if (res[0] == "1")
            {
                int NewId = Int32.Parse(res[1]);
                _accs.Add(new Account(NewId, NameTxtbx.Text, double.Parse(AmountTxtb.Text)));


            }

        }

        private void AccList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                Account AddedAcc = (Account)e.AddedItems[0];
                NameTxtbx.Text = AddedAcc.Name;
                AmountTxtb.Text = AddedAcc.CurMoneyAmount;
                AddButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                SaveButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            catch 
            {
            }                       
        }

        private void saveAcc_Button_click(object sender, RoutedEventArgs e)
        {
            Account CurAcc = (Account)AccList.SelectedItem;
            foreach (Account acc in _accs)
            {
                if (acc.Name == CurAcc.Name)
                {
                    acc.Name = NameTxtbx.Text;
                    acc.CurrentMoneyAmount = double.Parse(AmountTxtb.Text);
                    worker.ChangeAccount(CurAcc.Id, acc.CurrentMoneyAmount, acc.Name);                    
                }                    
            }
            ReLoadAcc();
        }

        private void ReLoadAcc()
        {
            _accs.Clear();
            foreach (Account _acc in worker.GetAccounts())
                _accs.Add(_acc);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                Account acc = (Account)AccList.SelectedItem;
                worker.DeleteAccount(acc.Id);
                ReLoadAcc();
            }
            catch
            {}
        }
    }
}
