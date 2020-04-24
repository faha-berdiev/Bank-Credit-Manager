/*
    Разработчик: _faha_
    Дата: 21.04.2020
    Класс: Authentication
    Файл: Authentication.cs
    Описание: Осуществление регистрацию и авторизацию пользователя
*/
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Bank_Credit_Manager
{
    public class Authentication : IAuthentication
    {
        private string username = string.Empty;
        private string userpassword = string.Empty;
        public string dateOfBirth = string.Empty;
        public string homePath = string.Empty;
        public string seria = string.Empty;
        private SQLManager _sqlManage = new SQLManager();
        public Authentication(string _name, string _password)
        {
            username = _name;
            userpassword = _password;
        }

        ///<summary>
        ///Регистрация аккаунта
        ///</summary>
        public bool RegistrateAccount(string _table_name)
        {
            if(this.isPreviouslyCreated(true))
                return false;
            else
            {
                try
                {
                    SQLManager _sqlManager = new SQLManager();
                    if(_table_name == "users_list_table")
                        _sqlManager.InsertData("users_list_table", "_name, _password, _date_of_birth, _home_path, _seria", $"'{username}', '{userpassword}', '{dateOfBirth}', '{homePath}', '{seria}'");
                    else
                        _sqlManager.InsertData($"{_table_name}", "_name, _password", "'{username}', '{userpassword}'");
                    _sqlManager._sqlConn.Close();
                    return true;
                }
                catch(Exception ex)
                {
                    Log.Error(ex.Message);
                    return false;
                }
            }
        }

        ///<summary>
        ///Возвращает True если пользователь с заданым логином существует, иначе Else
        ///</summary>
        public bool isPreviouslyCreated(bool _admin)
        {
            string _query = string.Empty;
            if(_admin)
                _query = $"select _name from admin_list_table where _name={username}";
            else
                _query = $"select _name from users_list_table where _name={username}";
            bool _created = false;
            int _counted = 0;
            SqlConncection _sqlConn = new SqlConncection(_sqlManage.ConnectionString());
            _sqlConn.Open();
            if(_sqlConn.State == ConnectionState.Open)
            {
                SqlCommand _sqlCmd = new SqlCommand(_query, _sqlConn);
                SqlDataReader _reader = _sqlCmd.ExecuteReader();
                while(_reader.Read())
                {
                    _counted++;
                }
                if(_counted > 0)
                    _created = true;
                else
                    _created = false;
                _reader.Close();
                _sqlConn.Close();
            }
            return _created;
        }

        ///<summary>
        ///Авторизация
        ///</summary>
        public bool Login(string _table_name)
        {
            bool _logged = false;
            int _counted = 0;
            SqlConncection _sqlConn = new SqlConncection(_sqlManage.ConnectionString());
            _sqlConn.Open();
            if(_sqlConn.State == ConnectionState.Open)
            {
                SqlCommand _sqlCmd = new SqlCommand($"select (_name, _password) from {_table_name} where (_name={username} and _password={password})", _sqlConn);
                SqlDataReader _reader = _sqlCmd.ExecuteReader();
                while(_reader.Read())
                {
                    _counted++;
                }
                if(_counted > 0)
                    _logged = true;
                else
                    _logged = false;
                _reader.Close();
                _sqlConn.Close();
            }
            return _logged;
        }

        ///<summary>
        ///Проверка правильности пароля
        ///</summary>
        public bool PasswordVerification()
        {
            return Regex.IsMatch(userpassword, "^[a-zA-Z0-9]+$");
        }
    }
}