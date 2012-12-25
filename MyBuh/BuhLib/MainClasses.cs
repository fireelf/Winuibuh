using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SQLite;
using Windows.Storage;
using System.IO;


namespace BuhLib
{
    enum Actions { AddDeposit, AddWithdrawal };

    public class MoneyFlowUnit
    {
        public int Id { get; set; }
        public string Date { get { return _Date.ToString("dd/MM/yyyy"); } }
        public int  Account { get { return _Account_Id; } }
        public string Comment { get { return _Comment; } }
        public string Summ { get { return _Count.ToString(); } }
        public string CatName { get; set; }
        public string AccName { get; set; }

        public int _Account_Id {get; set;}    // cчет
        public double _Count {get; set;}      // сумма
        public int _Category {get; set;}      // ссылка на категорию
        public string _Comment { get; set; }  // комментарий пользователя
        public DateTime _Date { get; set; }    // дата платежа
        public Windows.UI.Xaml.HorizontalAlignment Alignment { get; set; }
        

        public MoneyFlowUnitTable Export()
        {
            MoneyFlowUnitTable res = new MoneyFlowUnitTable();
            res._Account_Id = Account;
            res._Category = _Category;
            res._Count = _Count;
            res._Date = _Date;
            res._MFU_Id = Id;
            res.Comment = Comment;
            res.Date = DateTime.Now;
            return res;
        }

        public MoneyFlowUnit(int ID, int AccID, double Count, int CategoryID, string UserComment, DateTime Date)
        {
            Id = ID;
            _Account_Id = AccID;
            _Count = Count;
            _Category = CategoryID;
            _Comment = UserComment;
            _Date = Date;
            if (Count >= 0)
                Alignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            else
                Alignment = Windows.UI.Xaml.HorizontalAlignment.Center;
        }

        //public MoneyFlowUnit(int AccID, double Count, int CategoryID, string UserComment, DateTime Date)
        //{
        //    _Account_Id = AccID;
        //    _Count = Count;
        //    _Category = CategoryID;
        //    _Comment = UserComment;
        //    _Date = Date;
        //}

        
        /// <summary>
        /// Элемент расхода/дохода можно полностью изменить.
        /// </summary>
        /// <param name="AccID">Ссылка на счет</param>
        /// <param name="Count">Сумма</param>
        /// <param name="CategoryID">Ссылка на категорию</param>
        /// <param name="UserComment">Комментарий пользователя</param>
        public void Change(int AccID, double Count, int CategoryID, string UserComment, DateTime Date)
        {
            if (AccID != null)
                _Account_Id = AccID;
            if (Count != null)
                _Count = Count;
            if (CategoryID != null)
                _Category = CategoryID;
            if (UserComment != null)
                _Comment = UserComment;
            if (Date != null)
                _Date = Date;
        }
    }

    public class MoneyFlowUnitTable
    {
        [PrimaryKey, AutoIncrement]
        public int _MFU_Id { get; set; }
        [MaxLength(250)] 
        public DateTime Date { get; set; }      // Дата платежа
        [MaxLength(250)]
        public string Comment { get; set; }
        [MaxLength(250)]
        public int _Account_Id { get; set; }    // cчет
        [MaxLength(250)]
        public double _Count { get; set; }      // сумма
        [MaxLength(250)]
        public int _Category { get; set; }      // ссылка на категорию
        [MaxLength(250)]
        public DateTime _Date { get; set; }    // дата занесения
    }



    public class Account
    {
        public int Id { get; set; }

        private string _Name {get; set; }
        public double _CurrentAmount;
        public double CurrentMoneyAmount { get { return _CurrentAmount; } set{_CurrentAmount = value;} }
        public string CurMoneyAmount { get { return _CurrentAmount.ToString(); } }
        public string Name { get { return _Name; } set { _Name = value; } }

        // export Account to AccountTAble
        public AccountTable Export()
        {
            AccountTable res = new AccountTable();
            res._account_Id = Id;
            res._Name = Name;
            res.CurrentMoneyAmount = CurrentMoneyAmount;
            return res;
        }

        public Account(int id, string Name)        
        {
            Id = id;
            _Name = Name;
        }

        public Account(int id, string Name, double CurrentAmount)
        {
            Id = id;
            _Name = Name;
            _CurrentAmount = CurrentAmount;
        }

        /// <summary>
        /// Пополнение счета
        /// </summary>
        /// <param name="Money">Деньги-дребеденьги</param>
        /// <returns>Возвращает true, если счет пополнен, false иначе</returns>
        public bool Deposit(double Money)
        {
            if ((Money != null) && (Money > 0))
                _CurrentAmount += Money;
            else
                return false;
            return true;
        }
        /// <summary>
        /// Списание со счета
        /// </summary>
        /// <param name="Money">Деньги-дребеденьги</param>
        /// <returns>Возвращает true, если списание успешно, false иначе</returns>
        public bool Withdrawal(double Money)
        {
            if ((Money != null) && (Money > 0))
                _CurrentAmount -= Money;
            else
                return false;
            return true;
        }
    }

    public class  AccountTable
    {
        [PrimaryKey, AutoIncrement] public int _account_Id { get; set; }
        [MaxLength(250)] public string _Name { get; set; }
        public double CurrentMoneyAmount { get; set; }
    }


    public class Category
    {
        private int _id;
        public int ID { get { return _id; } }

        public string Name { get; set; }

        public Category(int Id, string name)
        {
            Name = name;
            _id = Id;
        }
    }

    public class CategoryTable
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        [MaxLength(250)]
        public String _Name {get; set;}

    }

    public class Budget
    {
        private int _id;
        public int Id { get { return _id; } }

        public double Limit { get; set; }
        public string Name { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string TimePeriod { get; set; }  

        public Budget(int Id, string Name_, double Limit_, int CatID, string TP)
        {
            _id = Id;
            Limit = Limit_;
            Name = Name_;
            CategoryId = CatID;
            TimePeriod = TP;
        }
    }

    public class BudgetTable
    {
        [PrimaryKey, AutoIncrement]
        public int _ID { get; set; }
        [MaxLength(250)]
        public double _Limit {get; set;}
        [MaxLength(250)]
        public String _Name { get; set; }
        [MaxLength(250)]
        public int _CategoryID { get; set; }     
        [MaxLength(250)]
        public string _TimePeriod { get; set; } 
        
    }

    

    public class Action
    {
        private double _Money;
        public double Money { get { return _Money; } }
        private Account _Account;

        public Action(double Money_, Account Acc)
        {
            _Money = Money_;
            _Account = Acc;
        }

        /// <summary>
        /// Выполняет указанное действие с привязанным счетом - списание, пополнение
        /// </summary>
        /// <returns>Успешно - true, false - иначе</returns>
        public bool Activate()
        {
            bool _result = false;

            if (_Money > 0)
                _result = _Account.Deposit(_Money);
            if (_Money < 0)
                _result = _Account.Withdrawal(_Money);

            return _result;
        }
    }

    public class ActionTable
    {
        [PrimaryKey, AutoIncrement] public int _action_id { get; set; }
        public double Money { get; set; }
        public int AccountID { get; set; }
        public int CategoryID { get; set; } 
    }

    public class ChangesTable
    {
        [PrimaryKey, AutoIncrement]
        public int _change_id { get; set; }
        public string _DBString { get; set; }
        public DateTime _ChangesDatetime { get; set; }
    }

    public class SyncData
    {
        public string ServerAddr { get; set; }
        public DateTime LastSync { get; set; }
    }
    
}
