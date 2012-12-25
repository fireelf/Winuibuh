using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using SQLite;
using Windows.Storage;
using System.IO;
using BuhLib;

namespace BuhLibTest
{
    [TestClass]
    public class WorkerTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            int res = 0;
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "testDB.db");
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                db.CreateTable<AccountTable>();
                res++;
                db.CreateTable<MoneyFlowUnitTable>();
                res++;
                db.CreateTable<CategoryTable>();
                res++;
                db.CreateTable<BudgetTable>();
                res++;
                db.CreateTable<ActionTable>();
                res++;
            }
            Assert.IsTrue(res > 0, res.ToString());
        }

        [TestMethod]
        public void CreateAccountTest()
        {
            int res = 0;
            // параметры генерации
            int iterationCount = 10;
            int rand1 = 100;
            int rand2 = 10000;

            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "testDB.db");
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                for (int i = 0; i != iterationCount; i++)
                {
                    Random rand = new Random();
                    var _acc = new AccountTable() { _Name = "TestAccount" + rand.Next(0, rand1).ToString(), CurrentMoneyAmount = rand.Next(0, rand2) };
                    db.Insert(_acc);
                    res++;
                }
            }
            Assert.IsTrue(res > 0, res.ToString());
        }

        [TestMethod]
        public void GetAccountsTest()
        {
            int res = 0;
            List<Account> _accounts = new List<Account>();
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "testDB.db");
            using (var db = new SQLiteConnection(dbPath))
            {
                var accs = db.Query<AccountTable>("SELECT * FROM AccountTable");
                foreach (AccountTable at in accs)
                {
                    _accounts.Add(new Account(at._account_Id, at._Name, at.CurrentMoneyAmount));
                    res++;
                }
            }
            Assert.IsTrue(res > 0, res.ToString());
        }

        [TestMethod]
        public void DeleteAccountTest()
        {
            int res = 0;
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "testDB.db");
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var ids = db.Query<AccountTable>("SELECT * FROM AccountTable");
                foreach (AccountTable aid in ids)
                {
                    db.Delete<AccountTable>(aid._account_Id);
                    res++;
                }
            }
            Assert.IsTrue(res > 0, res.ToString());
        }

        [TestMethod]
        public void AddBudgetTest()
        {
            int CategoryId = 12345;
            double Limit = 10000.00;
            string Name = "TestBudget";
            string TP = "Month";
            
            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "testDB.db");
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var _bud = new BudgetTable()
                {
                    _CategoryID = CategoryId,
                    _Limit = Limit,
                    _Name = Name,
                    _TimePeriod = TP
                };
                db.Insert(_bud);
                Assert.IsNotNull(_bud, "New Budget is NULL");
            }
        }

        [TestMethod]
        public void AddMFUTest()
        {
            int AccId = 12345;
            int CategoryID = 1;
            double Summ = 10000.00;
            DateTime testDate = DateTime.Now;

            var dbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "testDB.db");
            using (var db = new SQLiteConnection(dbPath))
            {
                // Работа с БД
                var _mfu = new MoneyFlowUnitTable()
                {
                    _Date = DateTime.Now,
                    Date = testDate,
                    _Account_Id = AccId,
                    _Category = CategoryID,
                    Comment = "",
                    _Count = Summ
                };
                db.Insert(_mfu);
                Assert.IsNotNull(_mfu, "New MoneyFLow is NULL");
            }
        }


    }
}
