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
    public class Worker
    {
        private string dbname = "data.db";
        /// <summary>
        /// Кто сказал, что не нужен нулевой конструктор? - обращение к существующей БД при пустом конструкторе
        /// </summary>
        public Worker()
        {
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                db.CreateTable<AccountTable>();
                db.CreateTable<MoneyFlowUnitTable>();
                db.CreateTable<CategoryTable>();
                db.CreateTable<BudgetTable>();
                db.CreateTable<ActionTable>();
                db.CreateTable<ChangesTable>();
                db.CreateTable<SyncData>();
            }

        }

        // создать БД локальную
        public Worker(String _dbname)
        {
            dbname = _dbname;
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                db.CreateTable<AccountTable>();
                db.CreateTable<MoneyFlowUnitTable>();
                db.CreateTable<CategoryTable>();
                db.CreateTable<BudgetTable>();
                db.CreateTable<ActionTable>();
                db.CreateTable<ChangesTable>();
                db.CreateTable<SyncData>();
            }
        }

        #region AccountWork
        public List<Account> GetAccounts()
        {
            /// ЗАГЛУШКА!!!
            List<Account> _accounts = new List<Account>();
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                var accs = db.Query<AccountTable>("SELECT * FROM AccountTable"); 
                foreach (AccountTable at  in accs)
                {
                    _accounts.Add(new Account (at._account_Id, at._Name, at.CurrentMoneyAmount));
                }
            }
            return _accounts;
        }

        public string CreateAccount(double _sum, String _name)
        {
            string res = "1;1";
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var _acc = new AccountTable() { _Name = _name, CurrentMoneyAmount = _sum };
                db.Insert(_acc);
                var _change = new ChangesTable() { _DBString = "INSERT INTO AccountTable(_Name, CurrentMoneyAmount) VALUES('" + _name + "', '" + _sum + "');", _ChangesDatetime = DateTime.Now };
                db.Insert(_change);
            }

            return res;
        }

        // устанавливаем значения из параметров
        public string ChangeAccount(int _acc_id, double _sum, String _name)
        {
            string res = "";
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД     
                var _acc = new AccountTable() {_account_Id = _acc_id, _Name = _name, CurrentMoneyAmount = _sum };
                db.Update(_acc);
                var _change = new ChangesTable() { _DBString = "UPDATE AccountTable SET _Name='" + _name + "', CurrentMoneyAmount='" + _sum + "' WHERE _account_ID='" + _acc_id + "';", _ChangesDatetime = DateTime.Now };
                db.Insert(_change);
            }
            return res;
        }

        // применяем к счету значение из параметра (меняется только значение суммы на счете, запоминаем экшн)
        public string ChangeAccount(int _acc_id, double _sum, int _cat_id)
        {
            string res = "";
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var command = db.CreateCommand("SELECT CurrentMoneyAmount from AccountTable WHERE _account_Id =" + _acc_id);
                var current_sum = command.ExecuteScalar<double>();
                var command1 = db.CreateCommand("Select _Name from AccountTable WHERE _account_Id =" + _acc_id);
                var _name = command1.ExecuteScalar<string>();
                var _acc = new AccountTable() { _account_Id = _acc_id, CurrentMoneyAmount = current_sum + _sum, _Name = _name };
                db.Update(_acc);
                var _act = new ActionTable() {AccountID = _acc_id, Money =  _sum, CategoryID = _cat_id};
                db.Insert(_act);
                // Если до этого был просто быдлокод, то теперь будет быдлокод в квадрате!!!
                var _change_acc = new ChangesTable() { _DBString = "UPDATE AccountTable SET _Name='" + _name + "', CurrentMoneyAmount='" + (current_sum + _sum) + "' WHERE _account_ID='" + _acc_id + "';", _ChangesDatetime = DateTime.Now };
                db.Insert(_change_acc);
                var _change_act = new ChangesTable() { _DBString = "INSERT INTO ActionTable(AccountID, Money, CategoryID) VALUES('" + _acc_id + "', '" + _sum + "', '" + _cat_id + "');", _ChangesDatetime = DateTime.Now };
                db.Insert(_change_act);
            }
            return res;
        }

        // удаление счета
        public void DeleteAccount(int _acc_id)
        {
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                db.Delete<AccountTable>(_acc_id);
                var _change = new ChangesTable() { _DBString = "DELETE FROM AccountTable WHERE _account_ID =" + _acc_id + "');", _ChangesDatetime = DateTime.Now };
                db.Insert(_change);
            }
        }
        #endregion

        #region MFUWork
        /// <summary>
        /// Добавить зачисление или списание
        /// </summary>
        /// <param name="AccId">Id счета</param>
        /// <param name="Summ">Сумма (положительная при зачислении и отрицательная при списании)</param>
        /// <param name="CategoryID">Id категории</param>
        /// <returns>Возвращает строку в формате -код ошибки;доп. информация-</returns>
        public string AddMoneyFlow(int AccId, double Summ, int CategoryID, string comment, DateTime date)
        {
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var _mfu = new MoneyFlowUnitTable() {Date = date,                                                      
                                                     _Account_Id = AccId, 
                                                     _Category = CategoryID,   
                                                     Comment = comment,
                                                     _Count = Summ,
                                                     _Date = DateTime.Now};
                db.Insert(_mfu);
                var _change = new ChangesTable() { _DBString = "INSERT INTO MoneyFlowUnitTable(Date, _Account_Id, _Category, Comment, _Count, _Date) VALUES('" + date + "', '" 
                    + AccId + "', '" + CategoryID + "', '" + comment + "', '" + Summ + "', '" + DateTime.Now + "');", _ChangesDatetime = DateTime.Now };
                db.Insert(_change);
                ChangeAccount(AccId, Summ, CategoryID);
            }
            return "1;1";
        }

        public List<MoneyFlowUnit> GetMoneyFlows()
        {
            List<MoneyFlowUnit> res = new List<MoneyFlowUnit>();

            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                var buds = db.Query<MoneyFlowUnitTable>("SELECT * FROM MoneyFlowUnitTable");
                foreach (MoneyFlowUnitTable bt in buds)
                {
                    
                    res.Add(new MoneyFlowUnit(bt._MFU_Id, bt._Account_Id, bt._Count, bt._Category, bt.Comment, bt._Date));
                }
            }
            return res;
        }

        public string DeleteMFU(int id)
        {
            string result = "1;1";

            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var command = db.CreateCommand("Select _Account_Id from MoneyFlowUnitTable WHERE _MFU_Id =" + id);
                var _acc_id = command.ExecuteScalar<int>();
                var command1 = db.CreateCommand("Select _Count from MoneyFlowUnitTable WHERE _MFU_Id =" + id);
                var count = command1.ExecuteScalar<double>();
                var command2 = db.CreateCommand("Select _Category from MoneyFlowUnitTable WHERE _MFU_Id =" + id);
                var _cat_id = command2.ExecuteScalar<int>();
                ChangeAccount(_acc_id, -count, _cat_id);
                db.Delete<MoneyFlowUnitTable>(id);
                var _change = new ChangesTable() { _DBString = "DELETE FROM MoneyFlowUnitTable WHERE _MFU_Id = '" + id + "';", _ChangesDatetime = DateTime.Now };
                db.Insert(_change);
            }
            return result;
        }

        public string ChangeMFU(int _id, int AccId, double Summ, int CategoryID, string comment, DateTime date)
        {
            string res = "";
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД     
                var command = db.CreateCommand("Select _Account_Id from MoneyFlowUnitTable WHERE _MFU_Id =" + _id);
                var _acc_id = command.ExecuteScalar<int>();
                var command1 = db.CreateCommand("Select _Count from MoneyFlowUnitTable WHERE _MFU_Id =" + _id);
                var count = command1.ExecuteScalar<double>();
                var command2 = db.CreateCommand("Select _Category from MoneyFlowUnitTable WHERE _MFU_Id =" + _id);
                var _cat_id = command2.ExecuteScalar<int>();
                ChangeAccount(_acc_id, -count, _cat_id);

                var _mfu = new MoneyFlowUnitTable() {_MFU_Id=_id, _Date=DateTime.Now, Date = date, _Account_Id = AccId,
                 _Category = CategoryID, _Count = Summ, Comment = comment};
                ChangeAccount(AccId, Summ, CategoryID);
                db.Update(_mfu);
                var _change = new ChangesTable() { _DBString = "UPDATE MoneyFlowUnitTable SET _Date='" + DateTime.Now + "', Date='" + date + "', _Account_Id='" + AccId
                    + "',  _Category='" + CategoryID + "', _Count='" + Summ + "', Comment='" + comment + "' WHERE _MFU_Id='" + _id + "';", _ChangesDatetime = DateTime.Now };
                db.Insert(_change);
            }
            return res;
        }

        #endregion

        #region BudgetWork
        public string AddBudget(string Name, double Limit,  int CategoryId, string TP)
        {
            string result = "1;1";
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var _bud = new BudgetTable() { _CategoryID = CategoryId,
                                               _Limit = Limit,
                                               _Name = Name,
                                               _TimePeriod = TP};
                db.Insert(_bud);
                var _change = new ChangesTable() { _DBString = "INSERT INTO BudgetTable(_ID, _Limit, _Name, _TimePeriod) VALUES('" + CategoryId + "', '" + Limit + "', '" + Name + "', '" + TP + "');", _ChangesDatetime = DateTime.Now };
                db.Insert(_change);
            }
            return result;
        }

        /// <summary>
        /// Метод изменения параметров бюджета
        /// </summary>
        /// <param name="id">Уникальный Id</param>
        /// <param name="Name">Название текущее или старое</param>
        /// <param name="NewName">Если указано, то бюджет переименовывается</param>
        /// <param name="Limit">Лимит по данному бюджету</param>
        /// <param name="TimePeriod">Период, на который расчитан бюджет</param>
        /// <param name="CategoryId">ID категории списания</param>
        /// <returns>Возвращает строку в формате -код ошибки;доп. информация-</returns>
        public string ChangeBudget(int id, string Name,  double Limit, string TimePeriod, int CategoryId)
        {
            string result = "1;1";
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД                                        
                var _bud = new BudgetTable()
                {
                    _ID = id,
                    _CategoryID = CategoryId,
                    _Limit = Limit,
                    _Name = Name,
                    _TimePeriod = TimePeriod
                };
                db.Update(_bud);
                var _change = new ChangesTable() { _DBString = "UPDATE BudgetTable SET _CategoryID='" + CategoryId + "', _Limit='" + Limit + "', _Name='" + (Name) + "', _TimePeriod='" + (TimePeriod) + "' WHERE _ID='" + id + "';", _ChangesDatetime = DateTime.Now };
                db.Insert(_change);
            }
            return result;
        }



        public string DeleteBudget(int id)
        {
            string result = "1;1";

            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                db.Delete<BudgetTable>(id);
                var _change = new ChangesTable() { _DBString = "DELETE FROM BudgetTable WHERE _ID =" + id + "');", _ChangesDatetime = DateTime.Now };
                db.Insert(_change);
            }

            return result;
        }
        /// <summary>
        /// Получение списка всех бюджетов
        /// </summary>
        /// <param name="type">week/month/year</param>
        /// <returns>Возвращает список бюджетов</returns>
        public List<Budget> GetBudgets(string type)
        {
            List<Budget> res = new List<Budget>();

            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                var buds = db.Query<BudgetTable>("SELECT * FROM BudgetTable WHERE _TimePeriod= '"+type+"'");
                foreach (BudgetTable bt in buds)
                {
                    res.Add(new Budget(bt._ID, bt._Name, bt._Limit, bt._CategoryID, bt._TimePeriod));
                }
            }
            return res;
        }
        #endregion

        #region Category work
        /// <summary>
        /// Получить список всех категорий
        /// </summary>
        /// <returns></returns>
        public List<Category> GetCategories()
        {
            List<Category> Categories = new List<Category>();
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                var buds = db.Query<CategoryTable>("SELECT * FROM CategoryTable");
                if (buds.Count == 0)
                {
                    AddCategory("Питание");
                    AddCategory("Досуг");
                    AddCategory("Транспорт");
                    AddCategory("Коммунальные платежи");
                    AddCategory("Одежда, обувь");
                    buds = db.Query<CategoryTable>("SELECT * FROM CategoryTable");
                }
                foreach (CategoryTable bt in buds)
                {
                    Categories.Add(new Category(bt.ID, bt._Name));
                }
            }
            return Categories;
        }


        /// <summary>
        /// Добавление новой категории
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public string AddCategory(string Name)
        {
            string result = "1:1";
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var _bud = new CategoryTable()
                {
                    _Name = Name
                };
                db.Insert(_bud);
                var _change = new ChangesTable() { _DBString = "INSERT INTO CategoryTable(_Name) VALUES('" + Name + "');", _ChangesDatetime = DateTime.Now };
                db.Insert(_change);
            }
            return result;
        }

        public string ChangeCategory(int id, string name)
        {
            string result = "1:1";
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var _bud = new CategoryTable()
                {
                    _Name = name, ID = id
                };
                db.Update(_bud);
                var _change = new ChangesTable() { _DBString = "UPDATE CategoryTable SET _Name='" + name +"' WHERE ID='" + id + "';", _ChangesDatetime = DateTime.Now };
                db.Insert(_change);
            }
            return result;
        }

        public string DeleteCategory(int id)
        {
            string result = "1:1";
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                db.Delete<CategoryTable>(id);
                var _change = new ChangesTable() { _DBString = "DELETE FROM CategoryTable WHERE ID =" + id + "');", _ChangesDatetime = DateTime.Now };
                db.Insert(_change);
            }
            return result;
        }
        #endregion

        #region Statistik
        // возвращает сумму затрат на указанную категорию, с указанной даты, за указанный период (type = week/year/month)
        public double SumOnCat(int _cat_id, DateTime from, string type)
        {
            double res = 0;
            DateTime to=DateTime.Today;
            switch (type)
            {
                case "week":
                    to = from.AddDays(7);
                    break;

                case "month":
                    to = from.AddMonths(1);
                    break;

                case "year":
                    to = from.AddYears(1);
                    break;
            }

            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var cats = db.Query<MoneyFlowUnitTable>("SELECT * FROM MoneyFlowUnitTable WHERE _Category= " + _cat_id + " AND Date>=" + from + " AND Date<=");
                foreach (MoneyFlowUnitTable mn in cats)
                {
                    res += mn._Count;
                }
            }

            return res;
        }

        // сколько рекомендуется тратить в неделю на категорию
        public double WeekBudgOnCat(int _cat_id, DateTime from)
        {
            double res = 0;
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var cats = db.Query<BudgetTable>("SELECT _Limit FROM BudgetTable WHERE CategoryID= " + _cat_id+ " AND _TimePeriod = 'week'");
                foreach (BudgetTable mn in cats)
                {
                    res = mn._Limit;
                }
            }
            return res;
        }

        // сколько рекомендуется тратить в месяц на категорию
        public double MonthBudgOnCat(int _cat_id, DateTime from)
        {
            double res = 0;
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var cats = db.Query<BudgetTable>("SELECT _Limit FROM BudgetTable WHERE CategoryID= " + _cat_id + " AND _TimePeriod = 'month'");
                foreach (BudgetTable mn in cats)
                {
                    res = mn._Limit;
                }
            }
            return res;
        }

        // сколько рекомендуется тратить в год на категорию
        public double YearBudgOnCat(int _cat_id, DateTime from)
        {
            double res = 0;
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var cats = db.Query<BudgetTable>("SELECT _Limit FROM BudgetTable WHERE CategoryID= " + _cat_id + " AND _TimePeriod = 'year'");
                foreach (BudgetTable mn in cats)
                {
                    res = mn._Limit;
                }
            }
            return res;
        }

        // возвращает разницу (PeriodBudgOnCat - SumOnCat)
        public double StatOnCat(int _cat_id, string type, DateTime from) // type = "week", "month", "year"
        {
            double res = 0;
            switch (type)
            {
                case "week":
                    res = WeekBudgOnCat(_cat_id, from) - SumOnCat(_cat_id, from, "week");
                    break;

                case "month":
                    res = MonthBudgOnCat(_cat_id, from) - SumOnCat(_cat_id, from, "month");
                    break;

                case "year":
                    res = YearBudgOnCat(_cat_id, from) - SumOnCat(_cat_id, from, "year");
                    break;
            }
            return res;
        }

        // неведомая хуйня возвращает список строк в формате Категория@#!Тип бюджета@#!Сумма Бюджета@#!Превышение
        public List<string> StatAllCatsAllTypes(DateTime from)
        {
            List<string> res = new List<string>();
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbname);
            using (var db = new SQLiteConnection(dbPath))
            {
                var cats = db.Query<CategoryTable>("SELECT * FROM CategoryTable");
                foreach (CategoryTable cat in cats)
                {
                    string buf = "";
                    buf = cat._Name + "@#!week@#!" + (WeekBudgOnCat(cat.ID, from)).ToString() +"@#!"+ (StatOnCat(cat.ID, "week", from)).ToString();
                    res.Add(buf);
                    buf = cat._Name + "@#!month" + (MonthBudgOnCat(cat.ID, from)).ToString() + "@#!"+(StatOnCat(cat.ID, "month", from)).ToString();
                    res.Add(buf);
                    buf = cat._Name + "@#!year@#!" + (YearBudgOnCat(cat.ID, from)).ToString() + "@#!"+ (StatOnCat(cat.ID, "year", from)).ToString();
                    res.Add(buf);
                }
            }
            return res;
        }

        #endregion

    }
}
