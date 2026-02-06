using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
    public class HomofoniAlgoritam
    {
        private string tekst;
        private string kljuc;
        private Dictionary<char, List<string>> sifrovanjeMapa;
        private Dictionary<string, char> desiforvanjeMapa;
        private Random random;

      
        public string Tekst
        {
            get { return tekst; }
            set { tekst = value; }
        }

        public string Kljuc
        {
            get { return kljuc; }
            set
            {
                kljuc = value;
                if (!string.IsNullOrEmpty(value))
                {
                    ParsirajKljuc(value);
                }
            }
        }

        public HomofoniAlgoritam(string tekst)
        {
            this.tekst = tekst;
            this.random = new Random();
            sifrovanjeMapa = new Dictionary<char, List<string>>();
            desiforvanjeMapa = new Dictionary<string, char>();
            GenerisiKljuc();
        }

        public HomofoniAlgoritam()
        {
            this.random = new Random();
            sifrovanjeMapa = new Dictionary<char, List<string>>();
            desiforvanjeMapa = new Dictionary<string, char>();
        }


        private void ParsirajKljuc(string kljucString)
        {
            sifrovanjeMapa.Clear();
            desiforvanjeMapa.Clear();


            string[] parovi = kljucString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string par in parovi)
            {
                string[] delovi = par.Split(':');
                if (delovi.Length == 2)
                {
                    char slovo = delovi[0][0];
                    string[] brojevi = delovi[1].Split(',');
                    List<string> listaBrojeva = new List<string>(brojevi);
                    sifrovanjeMapa[slovo] = listaBrojeva;

                    foreach (string broj in brojevi)
                    {
                        desiforvanjeMapa[broj] = slovo;
                    }
                }
            }
        }





        //generisanje ključa za homofonu supstitucionu šifru
        private void GenerisiKljuc()
        {
            sifrovanjeMapa.Clear();
            desiforvanjeMapa.Clear();

            string alfabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string samoglasnici = "AEIOU";


            int pocetni = random.Next(10, 100);
            int trenutni = pocetni;


            foreach (char slovo in alfabet)
            {
                List<string> brojevi = new List<string>();


                if (samoglasnici.Contains(slovo))
                {
                    brojevi.Add(trenutni.ToString());
                    sifrovanjeMapa[slovo] = brojevi;
                    desiforvanjeMapa[trenutni.ToString()] = slovo;


                    trenutni -= 3;
                    if (trenutni < 10)
                    {
                        trenutni = 99;
                    }


                    brojevi.Add(trenutni.ToString());
                    desiforvanjeMapa[trenutni.ToString()] = slovo;
                }
                else
                {
                    brojevi.Add(trenutni.ToString());
                    sifrovanjeMapa[slovo] = brojevi;
                    desiforvanjeMapa[trenutni.ToString()] = slovo;
                }


                trenutni -= 3;
                if (trenutni < 10)
                {
                    trenutni = 99;
                }
            }


            StringBuilder sb = new StringBuilder();
            foreach (var kvp in sifrovanjeMapa.OrderBy(x => x.Key))
            {
                sb.Append($"{kvp.Key}:");
                sb.Append(string.Join(",", kvp.Value));
                sb.Append(";");
            }
            kljuc = sb.ToString();
        }
   
        
        public string Enkripcija()
        {
            if (string.IsNullOrEmpty(tekst))
                return "";

            StringBuilder rezultat = new StringBuilder();
            foreach (char c in tekst.ToUpper())
            {
                if (char.IsLetter(c))
                {
                    if (sifrovanjeMapa.ContainsKey(c))
                    {
                        List<string> brojevi = sifrovanjeMapa[c];
                        string izbraniBroj = brojevi[random.Next(brojevi.Count)];
                        rezultat.Append(izbraniBroj + " ");
                    }
                }
                else if (char.IsWhiteSpace(c))
                {
                    rezultat.Append("/ ");
                }
                else
                {
                    rezultat.Append(c + " ");
                }
            }

            return rezultat.ToString().Trim();
        }

       
        public string Dekripcija(string sifrovana)
        {
            if (string.IsNullOrEmpty(sifrovana))
                return "";

            StringBuilder rezultat = new StringBuilder();
            string[] dijelovi = sifrovana.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string dio in dijelovi)
            {
                if (dio == "/")
                {
                    rezultat.Append(" ");
                }
                else if (desiforvanjeMapa.ContainsKey(dio))
                {
                    rezultat.Append(desiforvanjeMapa[dio]);
                }
                else
                {
                    rezultat.Append(dio);
                }
            }

            return rezultat.ToString();
        }
    }
}
