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
        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();
        private ObservableCollection<Account> _accounts = new ObservableCollection<Account>();

        private Worker worker = new Worker();

        public BalansPage()
        {
            this.InitializeComponent();   
            foreach (Category cat in worker.GetCategories())
                _categories.Add(cat);
            foreach (Account acc in worker.GetAccounts())
                _accounts.Add(acc);
            ReLoadMoney();      
            listview.ItemsSource = _money;
            CatCmbBx.ItemsSource = _categories;
            AccountCmbBx.ItemsSource = _accounts;
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
        //Commit
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DateTime date = new DateTime(Int32.Parse(dtxt3.Text), Int32.Parse(dtxt2.Text), Int32.Parse(dtxt1.Text));
            double summ = double.Parse(SummTxt.Text);
            string Comment = Commenttxt.Text;
            int _AccId = -1;
            int _CatId = -1;
            if (AccountCmbBx.SelectedIndex != -1)
            {
                Account acc = (Account)AccountCmbBx.SelectedItem;
                _AccId = acc.Id;
            }
            if (CatCmbBx.SelectedIndex != -1)
            {
                Category cat = (Category)CatCmbBx.SelectedItem;
                _CatId = cat.ID;
            }
                        
            worker.AddMoneyFlow(_AccId, summ, _CatId, Comment, date);

            ReLoadMoney();

        }

        private void ReLoadMoney()
        {
            _money.Clear();
            foreach (MoneyFlowUnit mfu in worker.GetMoneyFlows())
                _money.Add(mfu);

            foreach (MoneyFlowUnit mfu in _money)
            {
                foreach (Category cat in _categories)
                    if (mfu._Category == cat.ID)
                        mfu.CatName = cat.Name;
                foreach (Account acc in _accounts)
                    if (mfu.Account == acc.Id)
                        mfu.AccName = acc.Name;
            }

        }

        private void listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MoneyFlowUnit mfu = (MoneyFlowUnit)e.AddedItems[0];

                dtxt1.Text = DateTime.Parse(mfu.Date).Day.ToString();
                dtxt2.Text = DateTime.Parse(mfu.Date).Month.ToString();
                dtxt3.Text = DateTime.Parse(mfu.Date).Year.ToString();

                SummTxt.Text = mfu.Summ.ToString();
                Commenttxt.Text = mfu.Comment;

                foreach (Account acc in _accounts)
                {
                    if (acc.Id == mfu.Account)
                        AccountCmbBx.SelectedItem = acc;
                }

                foreach (Category cat in _categories)
                {
                    if (cat.ID == mfu._Category)
                        CatCmbBx.SelectedItem = cat;
                }

                DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                CommitButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                SaveButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            catch (Exception ex)
            {
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                dtxt1.Text = DateTime.Now.Day.ToString();
                dtxt2.Text = DateTime.Now.Month.ToString();
                dtxt3.Text = DateTime.Now.Year.ToString();

                SummTxt.Text = "";
                CatCmbBx.SelectedIndex = -1;
                Commenttxt.Text = "";
                AccountCmbBx.SelectedIndex = 1;
                
                CommitButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;                
                SaveButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                CommitButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                SaveButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }

        private void DeleteButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                MoneyFlowUnit mfu = (MoneyFlowUnit)listview.SelectedItem;
                worker.DeleteMFU(mfu.Id);
                ReLoadMoney();
            }
            catch (Exception ex)
            {
            }
        }

        private void SaveButton_Click_1(object sender, RoutedEventArgs e)
        {
            DateTime date = new DateTime(Int32.Parse(dtxt3.Text), Int32.Parse(dtxt2.Text), Int32.Parse(dtxt1.Text));
            double summ = double.Parse(SummTxt.Text);
            string Comment = Commenttxt.Text;
            int _AccId = -1;
            int _CatId = -1;
            if (AccountCmbBx.SelectedIndex != -1)
            {
                Account acc = (Account)AccountCmbBx.SelectedItem;
                _AccId = acc.Id;
            }
            if (CatCmbBx.SelectedIndex != -1)
            {
                Category cat = (Category)CatCmbBx.SelectedItem;
                _CatId = cat.ID;
            }
            MoneyFlowUnit mfu = (MoneyFlowUnit)listview.SelectedItem;
            Account CurAcc = (Account)AccountCmbBx.SelectedItem;
            Category CurCat = (Category)CatCmbBx.SelectedItem;
            foreach (MoneyFlowUnit u in _money)
            {
                if (u.Id == mfu.Id)
                {
                    u._Account_Id = CurAcc.Id;
                    u._Count = double.Parse(SummTxt.Text);
                    u._Category = CurCat.ID;
                    u._Comment = Commenttxt.Text;
                    u._Date = new DateTime(int.Parse(dtxt3.Text),int.Parse(dtxt2.Text),int.Parse(dtxt1.Text));
                    worker.ChangeMFU(mfu.Id, mfu._Account_Id, mfu._Count, mfu._Category, mfu._Comment, mfu._Date);
                }                    
            }
            ReLoadMoney();
        }
    }

    
}
