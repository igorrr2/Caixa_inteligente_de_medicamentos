using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CaixaInteligente.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CaixaInteligente
{
    [Activity(Label = "RegistrarActivity")]
    internal class RegistrarActivity: Activity
    {
        EditText txtNovoUsuario;
        EditText txtSenhaNovoUsuario;
        Android.Widget.Button btnCriarNovoUsuario;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.NovoUsuario);
            txtNovoUsuario = FindViewById<EditText>(Resource.Id.txtNovoUsuario); 
            txtSenhaNovoUsuario = FindViewById<EditText>(Resource.Id.txtSenhaNovoUsuario);
            btnCriarNovoUsuario = FindViewById<Android.Widget.Button>(Resource.Id.btnRegistrar);

            btnCriarNovoUsuario.Click += BtnCriarNovoUsuario_Click;
        }
        private void BtnCriarNovoUsuario_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtNovoUsuario.Text != "" && txtSenhaNovoUsuario.Text != "")
                    RegisterAsync(txtNovoUsuario.Text, txtSenhaNovoUsuario.Text);
                else
                {
                    string titulo = "Erro";
                    string mensagem = "Campo login ou senha não preenchidos";

                    Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                    builder.SetTitle(titulo);
                    builder.SetMessage(mensagem);
                    builder.SetPositiveButton("OK", (sender, args) => { });

                    Android.App.AlertDialog dialog = builder.Create();
                    dialog.Show();
                }
            }
            catch (Exception ex)
            {
                string titulo = "Erro";
                string mensagem = ex.Message;

                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                builder.SetTitle(titulo);
                builder.SetMessage(mensagem);
                builder.SetPositiveButton("OK", (sender, args) => { });

                Android.App.AlertDialog dialog = builder.Create();
                dialog.Show();
                
            }
            
        }
        private async Task RegisterAsync(string nome, string senha)
        {
            var userService = new UserService();
            bool result = await userService.RegisterUser(nome, senha);
            if (result)
            {
                string titulo = "Sucesso";
                string mensagem = "Usuário registrado com sucesso";

                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                builder.SetTitle(titulo);
                builder.SetMessage(mensagem);
                builder.SetPositiveButton("OK", (sender, args) => { });

                Android.App.AlertDialog dialog = builder.Create();
                dialog.Show();
            }
            else
            {
                string titulo = "Erro";
                string mensagem = "Falha ao registrar usuário";

                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                builder.SetTitle(titulo);
                builder.SetMessage(mensagem);
                builder.SetPositiveButton("OK", (sender, args) => { });

                Android.App.AlertDialog dialog = builder.Create();
                dialog.Show();
            }
        }
    }
}