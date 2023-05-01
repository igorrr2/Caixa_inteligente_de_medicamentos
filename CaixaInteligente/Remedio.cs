using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SQLite;

namespace CaixaInteligente
{
    public class Remedio
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public string Nome { get; set; }
        
        public string Horario{ get; set; }

        public static implicit operator Java.Lang.Object(Remedio v)
        {
            throw new NotImplementedException();
        }
    }
}