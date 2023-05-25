using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using CaixaInteligente.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CaixaInteligente
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText txtUsuario;
        EditText txtSenha;
        Android.Widget.Button btnCriar;
        Android.Widget.Button btnLogin;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            

            btnLogin = FindViewById<Android.Widget.Button>(Resource.Id.btnLogin);
            btnCriar = FindViewById <Android.Widget.Button>(Resource.Id.btnRegistrar);
            txtUsuario = FindViewById<EditText>(Resource.Id.txtUsuario);
            txtSenha = FindViewById<EditText>(Resource.Id.txtSenha);

            btnLogin.Click += BtnLogin_ClickAsync;
            btnCriar.Click += BtnCriar_Click;
        }
        private void BtnCriar_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrarActivity));
        }
        private void BtnLogin_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                LoginAsync(txtUsuario.Text, txtSenha.Text);
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
        private async Task LoginAsync(string nome, string senha){

            var userService = new UserService();
            bool result = await userService.LoginUser(nome, senha);
            if (result)
            {
                Toast.MakeText(this, "Login realizado com sucesso", ToastLength.Short).Show();
                var TelaInicial = new Intent(this, typeof(LoginActivity));
                StartActivity(TelaInicial);
            }
            else
            {
                string titulo = "Erro";
                string mensagem = "Usuário/Senha inválidos";

                Android.App.AlertDialog.Builder builder = new Android.App.AlertDialog.Builder(this);
                builder.SetTitle(titulo);
                builder.SetMessage(mensagem);
                builder.SetPositiveButton("OK", (sender, args) => { });

                Android.App.AlertDialog dialog = builder.Create();
                dialog.Show();
            }
                
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        
    }
}