using Android.App;
using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;
using System.Threading.Tasks;

namespace CaixaInteligente
{
    internal class RemedioBanco
    {
        readonly SQLiteAsyncConnection _database;

        public RemedioBanco(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Remedio>().Wait();
        }
        public Task<List<Remedio>> GetRemediosAsync()
        {
            return _database.Table<Remedio>().ToListAsync();
        }
        public Task<int> SaveRemedioAsync(Remedio remedio)
        {
            if (remedio.Id != 0)
            {
                return _database.UpdateAsync(remedio);
            }
            else
            {
                return _database.InsertAsync(remedio);
            }
        }
        public Task<int> DeleteRemedioAsync(Remedio remedio)
        {
            return _database.DeleteAsync(remedio);
        }
    }
}