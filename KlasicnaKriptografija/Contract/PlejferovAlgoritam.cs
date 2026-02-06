using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
    public class PlejferovAlgoritam
    {

        private string tekst;
        private string kljuc;
        private char[,] matrica;
        private Dictionary<char, (int, int)> pozicije;


        public string Teskt
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


        public PlejferovAlgoritam(string tekst, string kljucMatrica)
        {
            this.tekst = tekst;
            this.kljuc = kljucMatrica;
            this.matrica = new char[5, 5];
            this.pozicije = new Dictionary<char, (int, int)>();
            InicijalizujMatricu(kljucMatrica);
        }


        public PlejferovAlgoritam()
        {
            this.matrica = new char[5, 5];
            this.pozicije = new Dictionary<char, (int, int)>();
        }



        private void ParsirajKljuc(string kljucString)
        {
            pozicije.Clear();


            if (kljucString.Length >= 25)
            {
                int index = 0;
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        matrica[i, j] = kljucString[index];
                        pozicije[kljucString[index]] = (i, j);
                        index++;
                    }
                }
            }
        }


        private string GenerisiKljuc()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    sb.Append(matrica[i, j]);
                }
            }
            return sb.ToString();
        }

        private void InicijalizujMatricu(string kljucMatrica)
        {
            string slova = "ABCDEFGHIKLMNOPQRSTUVWXYZ";
            List<char> alfabet = new List<char>(slova.ToCharArray());

            int index = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (index < alfabet.Count)
                    {
                        matrica[i, j] = alfabet[index];
                        pozicije[alfabet[index]] = (i, j);
                        index++;
                    }
                }
            }

            kljuc = GenerisiKljuc();
        }


        public string Enkripcija()
        {
            if (string.IsNullOrEmpty(tekst))
                return "";

            string pripremljena = PrepraviPoruku(tekst);
            StringBuilder rezultat = new StringBuilder();

            for (int i = 0; i < pripremljena.Length; i += 2)
            {
                if (i + 1 < pripremljena.Length)
                {
                    char a = pripremljena[i];
                    char b = pripremljena[i + 1];

                    if (a == 'J') a = 'I';
                    if (b == 'J') b = 'I';

                    if (pozicije.ContainsKey(a) && pozicije.ContainsKey(b))
                    {
                        var (r1, c1) = pozicije[a];
                        var (r2, c2) = pozicije[b];

                        if (c1 == c2) // Ista kolona
                        {
                            r1 = (r1 + 1) % 5;
                            r2 = (r2 + 1) % 5;
                        }
                        else if (r1 == r2) // Isti red
                        {
                            c1 = (c1 + 1) % 5;
                            c2 = (c2 + 1) % 5;
                        }
                        else // Pravougaonik
                        {
                            int temp = c1;
                            c1 = c2;
                            c2 = temp;
                        }

                        rezultat.Append(matrica[r1, c1]);
                        rezultat.Append(matrica[r2, c2]);
                    }
                }
            }

            return rezultat.ToString();
        }


        public string Dekripcija(string sifrovana)
        {
            if (string.IsNullOrEmpty(sifrovana))
                return "";

            StringBuilder rezultat = new StringBuilder();

            for (int i = 0; i < sifrovana.Length; i += 2)
            {
                if (i + 1 < sifrovana.Length)
                {
                    char a = sifrovana[i];
                    char b = sifrovana[i + 1];

                    if (pozicije.ContainsKey(a) && pozicije.ContainsKey(b))
                    {
                        var (r1, c1) = pozicije[a];
                        var (r2, c2) = pozicije[b];

                        if (c1 == c2) // Ista kolona
                        {
                            r1 = (r1 - 1 + 5) % 5;
                            r2 = (r2 - 1 + 5) % 5;
                        }
                        else if (r1 == r2) // Isti red
                        {
                            c1 = (c1 - 1 + 5) % 5;
                            c2 = (c2 - 1 + 5) % 5;
                        }
                        else // Pravougaonik
                        {
                            int temp = c1;
                            c1 = c2;
                            c2 = temp;
                        }

                        rezultat.Append(matrica[r1, c1]);
                        rezultat.Append(matrica[r2, c2]);
                    }
                }
            }

            return rezultat.ToString();
        }


        private string PrepraviPoruku(string poruka)
        {
            StringBuilder rezultat = new StringBuilder();
            foreach (char c in poruka.ToUpper())
            {
                if (char.IsLetter(c))
                {
                    rezultat.Append(c);
                }
            }


            if (rezultat.Length % 2 != 0)
            {
                rezultat.Append('X');
            }

            return rezultat.ToString();
        }
    }
}