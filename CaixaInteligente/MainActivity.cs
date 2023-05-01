using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;

namespace CaixaInteligente
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        EditText txtUsuario;
        EditText txtSenha;
        Button btnCriar;
        Button btnLogin;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            

            btnLogin = FindViewById<Button>(Resource.Id.btnLogin);
            btnCriar = FindViewById <Button>(Resource.Id.btnRegistrar);
            txtUsuario = FindViewById<EditText>(Resource.Id.txtUsuario);
            txtSenha = FindViewById<EditText>(Resource.Id.txtSenha);

            btnLogin.Click += BtnLogin_Click;
            btnCriar.Click += BtnCriar_Click;
        }
        private void BtnCriar_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegistrarActivity));
        }
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            try{

                if (txtUsuario.Text == "" && txtSenha.Text == "")
                {
                    Toast.MakeText(this, "Login realizado com sucesso", ToastLength.Short).Show();
                    var TelaInicial = new Intent(this, typeof(LoginActivity));
                    StartActivity(TelaInicial);
                }
                else
                {
                    Toast.MakeText(this, "Nome de usuário e/ou senha inválidos", ToastLength.Short).Show();

                }
            }
            catch (Exception)
            {
                
            }
            
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        
    }
}