DROP TABLE IF EXISTS Accounts;
CREATE TABLE Accounts
(
    account_id CHAR(0) PRIMARY KEY,        
    _name TEXT,    
    _CurrentMoneyAmount SINGLE  
);

DROP TABLE IF EXISTS Budgets;
CREATE TABLE Budgets
(
    budget_id CHAR(0) PRIMARY KEY,          
    Lim SINGLE,
    Name TEXT,
    Account_id CHAR(0),    
    FOREIGN KEY(Account_id) REFERENCES Accounts(account_id)
);

DROP TABLE IF EXISTS Categories;
CREATE TABLE Categories
(
    _Category_id CHAR(0) PRIMARY KEY,          
    Name TEXT,    
    FOREIGN KEY (_Category_id) REFERENCES MoneyFlowUnits(_Category_id)
);

DROP TABLE IF EXISTS Actions;
CREATE TABLE Actions
(
    action_id CHAR(0) PRIMARY KEY,  
    Money CURRENCY,        
    _account_id CHAR(0),    
    FOREIGN KEY(_account_id) REFERENCES MoneyFlowUnits(account_ID)
);

DROP TABLE IF EXISTS MoneyFlowUnits;
CREATE TABLE MoneyFlowUnits
(
    MoneyFlowUnit_id CHAR(0) PRIMARY KEY,  
    account_ID CHAR(0),     
    _Count CURRENCY,
    _Category_id CHAR(0),
    _Comment TEXT,
    _Date DATETIME,
    _type TEXT
);
