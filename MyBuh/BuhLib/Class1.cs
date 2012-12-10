using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuhLib
{
    enum MoneyType {plus, minus};
    enum Actions { AddDeposit, AddWithdrawal };

    public class MoneyFlowUnit
    {
        private int _Account_Id {get; set;}    // номер счета
        private double _Count {get; set;}      // сумма
        private int _Category {get; set;}      // ссылка на категорию
        private string _Comment { get; set; }  // комментарий пользователя
        private DateTime _Date { get; set; }    // дата платежа
        private MoneyType _type;
        public MoneyType Type { get { return _type; } }
        
        public MoneyFlowUnit(int AccID, double Count, int CategoryID, string UserComment, DateTime Date)
        {
            _Account_Id = AccID;
            _Count = Count;
            _Category = CategoryID;
            _Comment = UserComment;
            _Date = Date;
            _type = MoneyType.minus;
        }

        public MoneyFlowUnit(int AccID, double Count, int CategoryID, string UserComment, DateTime Date, MoneyType type)
        {
            _Account_Id = AccID;
            _Count = Count;
            _Category = CategoryID;
            _Comment = UserComment;
            _Date = Date;
            _type = type;
        }
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

    public class Account
    {
        private string _Name {get; set; }
        private double _CurrentAmount;
        public double CurrentMoneyAmount { get { return _CurrentAmount; } }

        public Account(string Name)
        {
            _Name = Name;
        }

        public Account(string Name, double CurrentAmount)
        {
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

    public class Category
    {
        private int _id;
        public int ID { get { return _id; } }

        private string Name { get; set; }

        public Category(int Id, string name)
        {
            Name = name;
            _id = Id;
        }
    }

    public class Budget
    {
        private int _id;
        public int Id { get { return _id; } }

        private double Limit { get; set; }
        private string Name { get; set; }

        public Budget(int Id, string Name_, double Limit_)
        {
            _id = Id;
            Limit = Limit_;
            Name = Name_;
        }
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
    
}
