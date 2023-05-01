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

namespace CaixaInteligente
{
    [Activity(Label = "RegistrarActivity")]
    internal class RegistrarActivity: Activity
    {
        EditText txtNovoUsuario;
        EditText txtSenhaNovoUsuario;
        Button btnCriarNovoUsuario;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.NovoUsuario);
            txtNovoUsuario = FindViewById<EditText>(Resource.Id.txtNovoUsuario); 
            txtSenhaNovoUsuario = FindViewById<EditText>(Resource.Id.txtSenhaNovoUsuario);
            btnCriarNovoUsuario = FindViewById<Button>(Resource.Id.btnRegistrar);

            btnCriarNovoUsuario.Click += BtnCriarNovoUsuario_Click;
        }
        private void BtnCriarNovoUsuario_Click(object sender, EventArgs e)
        {
            //Criar usuario e armazenar no banco
        }
    }
}