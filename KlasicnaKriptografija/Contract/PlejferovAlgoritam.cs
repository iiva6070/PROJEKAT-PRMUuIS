using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Contract
{
    public class PlejferovAlgoritam
    {
        private string tekst;
        private string kljuc;
        private char[,] matrica;
        private Dictionary<char, (int, int)> pozicije;

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
                    InicijalizujMatricu(value);
                }
            }
        }

        public PlejferovAlgoritam()
        {
            matrica = new char[5, 5];
            pozicije = new Dictionary<char, (int, int)>();
        }

        public PlejferovAlgoritam(string tekst, string kljuc)
        {
            this.tekst = tekst;
            this.kljuc = kljuc;
            matrica = new char[5, 5];
            pozicije = new Dictionary<char, (int, int)>();
            InicijalizujMatricu(kljuc);
        }

        
        private void InicijalizujMatricu(string kljucMatrica)
        {
            pozicije.Clear();

            string alfabet = "ABCDEFGHIKLMNOPQRSTUVWXYZ"; // bez J
            StringBuilder sb = new StringBuilder();

            
            foreach (char c in kljucMatrica.ToUpper())
            {
                if (char.IsLetter(c))
                {
                    char ch = (c == 'J') ? 'I' : c;
                    if (!sb.ToString().Contains(ch))
                        sb.Append(ch);
                }
            }

            
            foreach (char c in alfabet)
            {
                if (!sb.ToString().Contains(c))
                    sb.Append(c);
            }

            
            int index = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    matrica[i, j] = sb[index];
                    pozicije[sb[index]] = (i, j);
                    index++;
                }
            }
        }


        public string Enkripcija()
        {
            if (string.IsNullOrEmpty(tekst))
                return "";

            string pripremljena = tekst.ToUpper().Replace('J', 'I'); 
            StringBuilder rezultat = new StringBuilder();

            int i = 0;
            while (i < pripremljena.Length)
            {
                if (pripremljena[i] == ' ')
                {
                    rezultat.Append(' '); 
                    i++;
                    continue;
                }

                char a = pripremljena[i];
                char b = 'X'; 
                int j = i + 1;

                
                while (j < pripremljena.Length && pripremljena[j] == ' ')
                    j++;

                if (j < pripremljena.Length)
                    b = pripremljena[j];

                var (r1, c1) = pozicije[a];
                var (r2, c2) = pozicije[b];

                if (c1 == c2) { r1 = (r1 + 1) % 5; r2 = (r2 + 1) % 5; }
                else if (r1 == r2) { c1 = (c1 + 1) % 5; c2 = (c2 + 1) % 5; }
                else { int temp = c1; c1 = c2; c2 = temp; }

                rezultat.Append(matrica[r1, c1]);
                rezultat.Append(matrica[r2, c2]);

                i = j + 1;
            }

            return rezultat.ToString();
        }



        public string Dekripcija(string sifrovana)
        {
            if (string.IsNullOrEmpty(sifrovana))
                return "";

            StringBuilder rezultat = new StringBuilder();
            int i = 0;

            while (i < sifrovana.Length)
            {
                if (sifrovana[i] == ' ')
                {
                    rezultat.Append(' '); 
                    i++;
                    continue;
                }

                char a = sifrovana[i];
                char b = 'X';
                int j = i + 1;

                
                while (j < sifrovana.Length && sifrovana[j] == ' ')
                    j++;

                if (j < sifrovana.Length)
                    b = sifrovana[j];

                var (r1, c1) = pozicije[a];
                var (r2, c2) = pozicije[b];

                if (c1 == c2) { r1 = (r1 - 1 + 5) % 5; r2 = (r2 - 1 + 5) % 5; }
                else if (r1 == r2) { c1 = (c1 - 1 + 5) % 5; c2 = (c2 - 1 + 5) % 5; }
                else { int temp = c1; c1 = c2; c2 = temp; }

                rezultat.Append(matrica[r1, c1]);
                rezultat.Append(matrica[r2, c2]);

                i = j + 1;
            }

            return OcistiPlayfairX(rezultat.ToString());
        }


        private string PrepraviPoruku(string poruka)
        {
            StringBuilder cist = new StringBuilder();

            foreach (char c in poruka.ToUpper())
            {
                if (char.IsLetter(c))
                    cist.Append(c == 'J' ? 'I' : c);
                else if (c == ' ')
                    cist.Append(' '); // zadrži razmake
            }

            StringBuilder rezultat = new StringBuilder();
            int i = 0;

            while (i < cist.Length)
            {
                if (cist[i] == ' ')
                {
                    rezultat.Append(' ');
                    i++;
                    continue;
                }

                char a = cist[i];
                char b = 'X';
                int j = i + 1;

                // nađi sledeće slovo, preskoči razmake
                while (j < cist.Length && cist[j] == ' ')
                    j++;

                if (j < cist.Length)
                    b = cist[j];

                if (a == b)
                {
                    rezultat.Append(a);
                    rezultat.Append('X');
                    i++;
                }
                else
                {
                    rezultat.Append(a);
                    rezultat.Append(b);
                    i = j + 1;
                }
            }

            // padding samo za slova, ignorise razmake
            int brojSlova = rezultat.ToString().Count(c => char.IsLetter(c));
            if (brojSlova % 2 != 0)
                rezultat.Append('X');

            return rezultat.ToString();
        }




        
        private string UkloniPadding(string tekst)
        {
            if (tekst.EndsWith("X"))
                return tekst.TrimEnd('X');
            return tekst;
        }

        private string OcistiPlayfairX(string tekst)
        {
            StringBuilder rezultat = new StringBuilder();

            for (int i = 0; i < tekst.Length; i++)
            {
                
                if (i > 0 && i < tekst.Length - 1 &&
                    tekst[i] == 'X' &&
                    tekst[i - 1] == tekst[i + 1])
                {
                    continue;
                }

                rezultat.Append(tekst[i]);
            }

            
            if (rezultat.Length > 0 && rezultat[rezultat.Length - 1] == 'X')
                rezultat.Length--;

            return rezultat.ToString();
        }



       
    }
}
