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
using BuhLib;
using Windows.UI;
using System.Collections.ObjectModel;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyBuh
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class BudgetPage : Page
    {
        private Worker worker = new Worker();
        private ObservableCollection<Budget> _budgets = new ObservableCollection<Budget>();
        private ObservableCollection<Category> _categories = new ObservableCollection<Category>();


        public BudgetPage()
        {
            this.InitializeComponent();

            foreach (Category cat in worker.GetCategories())
                _categories.Add(cat);

            CategoryCmbBx.ItemsSource = _categories;

            OpenWeekBudget(null, null);            
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

        private void OpenWeekBudget(object sender, RoutedEventArgs e)
        {
            BudgetTypeTxtbx.Text = "Неделя";
            _budgets.Clear();
            foreach (Budget budg in worker.GetBudgets("week"))
            {
                foreach (Category cat in _categories)
                    if (cat.ID == budg.CategoryId)
                        budg.CategoryName = cat.Name;
                _budgets.Add(budg);                
            }
            ListViewBudgets.ItemsSource = _budgets;           
        }

        private void OpenMonthBudget(object sender, RoutedEventArgs e)
        {
            BudgetTypeTxtbx.Text = "Месяц";
            _budgets.Clear();
            foreach (Budget budg in worker.GetBudgets("month"))
            {
                foreach (Category cat in _categories)
                    if (cat.ID == budg.CategoryId)
                        budg.CategoryName = cat.Name;
                _budgets.Add(budg);                
            }
            ListViewBudgets.ItemsSource = _budgets;

        }

        private void OpenYearBudget(object sender, RoutedEventArgs e)
        {
            BudgetTypeTxtbx.Text = "Год";
            _budgets.Clear();
            foreach (Budget budg in worker.GetBudgets("year"))
            {
                foreach (Category cat in _categories)
                    if (cat.ID == budg.CategoryId)
                        budg.CategoryName = cat.Name;
                _budgets.Add(budg);                
            }
            ListViewBudgets.ItemsSource = _budgets;

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                Worker worker = new Worker();
                Budget budg =  (Budget)ListViewBudgets.SelectedItem;
                Category ActiveCategory = (Category)CategoryCmbBx.SelectedItem;
                string[] results = new string[2];
                switch (BudgetTypeTxtbx.Text)
                {
                    case "Неделя":
                        results = worker.ChangeBudget(budg.Id, NameTxtBx.Text, double.Parse(LimitTxtBx.Text), "week", ActiveCategory.ID).Split(';');
                        break;
                    case "Месяц":
                        results = worker.ChangeBudget(budg.Id, NameTxtBx.Text, double.Parse(LimitTxtBx.Text), "month", ActiveCategory.ID).Split(';');
                        break;
                    case "Год":
                        results = worker.ChangeBudget(budg.Id, NameTxtBx.Text, double.Parse(LimitTxtBx.Text), "year", ActiveCategory.ID).Split(';');
                        break;
                }
               
               // string[] results = new string[2];

                if (results[0] == "1")
                {
                    StatusTxt.Foreground = new SolidColorBrush(Colors.Black);
                    StatusTxt.Text = "Изменения внесены";

                    switch (BudgetTypeTxtbx.Text)
                    {
                        case "Неделя":
                            OpenWeekBudget(null, null);
                            break;
                        case "Месяц":
                            OpenMonthBudget(null, null);
                            break;
                        case "Год":
                            OpenYearBudget(null, null);
                            break;
                    }
                }
                else
                {
                    StatusTxt.Foreground = new SolidColorBrush(Colors.Red);
                    StatusTxt.Text = "Возникла ошибка: " + results[1];
                }
            }
            catch (Exception ex)
            {
                int i = 0;
                i = i + 1;
            }
        }

        private void Button_Click_AddBudget(object sender, RoutedEventArgs e)
        {
            try
            {
                Worker worker = new Worker();
                Budget budg = (Budget)ListViewBudgets.SelectedItem;
                Category ActiveCategory = (Category)CategoryCmbBx.SelectedItem;
                string[] results = new string[2];
                switch (BudgetTypeTxtbx.Text)
                {
                    case "Неделя":
                        results = worker.AddBudget(NameTxtBx.Text, double.Parse(LimitTxtBx.Text), ActiveCategory.ID, "week").Split(';');
                        break;
                    case "Месяц":
                        results = worker.AddBudget(NameTxtBx.Text, double.Parse(LimitTxtBx.Text), ActiveCategory.ID, "month").Split(';');
                        break;
                    case "Год":
                        results = worker.AddBudget(NameTxtBx.Text, double.Parse(LimitTxtBx.Text), ActiveCategory.ID, "year").Split(';');
                        break;
                }
                
                // string[] results = new string[2];

                if (results[0] == "1")
                {
                    StatusTxt.Foreground = new SolidColorBrush(Colors.Black);
                    StatusTxt.Text = "Добавлен новый бюджет";

                    switch (BudgetTypeTxtbx.Text)
                    {
                        case "Неделя":
                            OpenWeekBudget(null, null);
                            break;
                        case "Месяц":
                            OpenMonthBudget(null, null);
                            break;
                        case "Год":
                            OpenYearBudget(null, null);
                            break;
                    }
                }
                else
                {
                    StatusTxt.Foreground = new SolidColorBrush(Colors.Red);
                    StatusTxt.Text = "Возникла ошибка: " + results[1];
                }
            }
            catch
            { }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AddButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
            SaveButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            NameTxtBx.Text = "";
            LimitTxtBx.Text = "";
            CategoryCmbBx.SelectedIndex = -1;
            DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void ListViewBudgets_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                AddButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                SaveButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                Budget selected = (Budget)e.AddedItems[0];
                NameTxtBx.Text = selected.Name;
                LimitTxtBx.Text = selected.Limit.ToString();
                DeleteButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
                foreach (Category cat in _categories)
                {
                    if (cat.ID == selected.CategoryId)
                        CategoryCmbBx.SelectedItem = cat;
                }
            }
            catch
            {
            }
            
        }

        private void DeleteButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Budget budg = (Budget)ListViewBudgets.SelectedItem;
                worker.DeleteBudget(budg.Id);

                _budgets.Clear();
                foreach (Budget bud in worker.GetBudgets(budg.TimePeriod))
                    _budgets.Add(bud);
            }
            catch
            {
            }
            
        }

    }
}
