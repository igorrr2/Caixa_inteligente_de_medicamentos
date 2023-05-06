using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CaixaInteligente
{
    public class RemedioAdapter : BaseAdapter<Remedio>
    {
        private List<Remedio> remedios;
        private Context context;
        private EventHandler apagarClickHandler;
        ComandosActivity _removerRemedioEsp;
        public RemedioAdapter(Context context, List<Remedio> remedios)
        {
            this.context = context;
            this.remedios = remedios;

            apagarClickHandler = (sender, args) =>
            {
                var button = (Button)sender;
                int position = (int)button.Tag;
                Remedio remedio = remedios[position];

                new AlertDialog.Builder(context)
                    .SetTitle("Apagar Remédio")
                    .SetMessage("Tem certeza que deseja apagar este remédio?")
                    .SetPositiveButton("Sim", (dialog, args) =>
                    {
                        remedios.RemoveAt(position);
                        string caminhoBanco = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "Remedio.db");
                        var db = new SQLiteConnection(caminhoBanco);
                        db.Delete<Remedio>(remedio.Id);
                        //Notifica o esp sobrer o remédio apagado e envia o horário para remoção
                        _removerRemedioEsp = new ComandosActivity();
                        _removerRemedioEsp.RemoverRemedioEsp(remedio.Horario);
                        NotifyDataSetChanged();
                        Toast.MakeText(context, "Remédio apagado", ToastLength.Short).Show();
                    })
                    .SetNegativeButton("Não", (dialog, args) =>
                    {
                        ((AlertDialog)dialog).Dismiss();
                    })
                    .Create()
                    .Show();
            };
        }

        public override int Count
        {
            get { return remedios.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override Remedio this[int position]
        {
            get { return remedios[position]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                LayoutInflater inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
                view = inflater.Inflate(Resource.Layout.item_remedio, parent, false);
            }

            Remedio remedio = remedios[position];
            TextView textViewNome = view.FindViewById<TextView>(Resource.Id.text_view_nome);
            TextView textViewHorario = view.FindViewById<TextView>(Resource.Id.text_view_horario);
            Button buttonApagar = view.FindViewById<Button>(Resource.Id.button_apagar);
            textViewNome.Text = remedio.Nome;
            textViewHorario.Text = remedio.Horario;

            // Remove o manipulador de clique atual do botão "Apagar" antes de adicionar o novo manipulador
            buttonApagar.Click -= apagarClickHandler;

            // Adiciona o manipulador de clique do botão "Apagar" com a variável de evento global
            buttonApagar.Click += apagarClickHandler;

            // Define a posição do item na Tag do botão "Apagar"
            buttonApagar.Tag = position;

            return view;
        }
    }
}