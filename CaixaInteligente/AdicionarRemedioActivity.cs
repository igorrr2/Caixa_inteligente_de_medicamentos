using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaixaInteligente
{
    [Activity(Label = "AdicionarRemedioActivity")]
    internal class AdicionarRemedioActivity : Activity
    {
        private EditText editTextNome;
        private EditText editTextHorario;
        private Button buttonSalvar;
        private SQLiteConnection db;
        ComandosActivity _adicionarRemedioEsp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.AdicionarRemedio);

            editTextNome = FindViewById<EditText>(Resource.Id.edit_text_nome);
            editTextHorario = FindViewById<EditText>(Resource.Id.edit_text_horario);
            buttonSalvar = FindViewById<Button>(Resource.Id.button_salvar);

            buttonSalvar.Click += ButtonSalvar_Click;
        }

        private void ButtonSalvar_Click(object sender, System.EventArgs e)
        {
            string horario = editTextHorario.Text.ToString();
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            if (editTextNome.Text == "" || editTextHorario.Text == "")
            {
                builder.SetTitle("Preencha todos os dados");
                builder.SetMessage("Foram encontrados dados sem ser preenchidos, por favor, preencha tudo corretamente e tente novamente");
                builder.SetNegativeButton("OK", (dialog, which) =>
                {
                    ((AlertDialog)dialog).Dismiss();
                });
                builder.Show();
            }
            else if (editTextHorario.Text.Length != 5 || editTextHorario.Text.Length >= 3 && horario[2] != ':')
            {
                builder.SetTitle("Formato de hora incompatível");
                builder.SetMessage("Formato de hora incompatível, por favor, coloque no formato 00:00");
                builder.SetNegativeButton("OK", (dialog, which) =>
                {
                    ((AlertDialog)dialog).Dismiss();
                });
                builder.Show();
            }
            else if (horario[0] != '0' && horario[0] != '1' && horario[0] != '2')
            {
                builder.SetTitle("Hora incompatível");
                builder.SetMessage("Favor inserir uma hora compatível");
                builder.SetNegativeButton("OK", (dialog, which) =>
                {
                    ((AlertDialog)dialog).Dismiss();
                });
                builder.Show();

            }
            else if (horario[0] == '2' && (horario[1] == '4' || horario[1] == '5' || horario[1] == '6' || horario[1] == '7' || horario[1] == '8' || horario[1] == '9'))
            {
                builder.SetTitle("Hora incompatível");
                builder.SetMessage("Favor inserir uma hora compatível");
                builder.SetNegativeButton("OK", (dialog, which) =>
                {
                    ((AlertDialog)dialog).Dismiss();
                });
                builder.Show();

            }
            else if (horario[3] != '0' && horario[3] != '1' && horario[3] != '2' && horario[3] != '3' && horario[3] != '4' && horario[3] != '5')
            {
                builder.SetTitle("Hora incompatível");
                builder.SetMessage("Favor inserir uma hora compatível");
                builder.SetNegativeButton("OK", (dialog, which) =>
                {
                    ((AlertDialog)dialog).Dismiss();
                });
                builder.Show();

            }
            else
            {
                builder.SetTitle("Confirmação");
                builder.SetMessage("Deseja realmente salvar o remédio?");
                builder.SetPositiveButton("Sim", (dialog, which) =>
                {

                    string caminhoDB = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
                    string nomeDB = "Remedio.db";
                    string caminhoCompletoDB = System.IO.Path.Combine(caminhoDB, nomeDB);
                    db = new SQLiteConnection(caminhoCompletoDB);
                    var remedio = new Remedio() { Nome = editTextNome.Text, Horario = editTextHorario.Text };
                    db.Insert(remedio);
                    _adicionarRemedioEsp = new ComandosActivity();
                    _adicionarRemedioEsp.AdicionarRemedioEsp(remedio.Horario);
                    Toast.MakeText(this, "Remédio adicionado com sucesso", ToastLength.Short).Show();
                });
                builder.SetNegativeButton("Não", (dialog, which) =>
                {
                    ((AlertDialog)dialog).Dismiss();
                });
                builder.Show();
            }
        }
    }
}