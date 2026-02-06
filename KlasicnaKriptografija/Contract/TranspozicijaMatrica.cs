using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{

        public class TranspozicijaMatrica
        {
            private string tekst;
            private string kljuc;
            private int red;
            private int kolona;
            private List<int> redosledTranspoz;


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

            public TranspozicijaMatrica(string tekst, int red, int kolona)
            {
                this.tekst = tekst;
                this.red = red;
                this.kolona = kolona;
                this.redosledTranspoz = new List<int>();
                GenerisiKljuc();
            }


            public TranspozicijaMatrica()
            {
                this.redosledTranspoz = new List<int>();
            }

            private void ParsirajKljuc(string kljucString)
            {

                string[] dijelovi = kljucString.Split(',');

                foreach (string dio in dijelovi)
                {
                    if (dio.StartsWith("Redova:"))
                    {
                        red = int.Parse(dio.Substring(7));
                    }
                    else if (dio.StartsWith("Kolona:"))
                    {
                        kolona = int.Parse(dio.Substring(7));
                    }
                    else if (dio.StartsWith("Redosled:"))
                    {
                        string redosledStr = kljucString.Substring(kljucString.IndexOf("Redosled:") + 9);
                        string[] redosledDijelovi = redosledStr.Split(',');
                        redosledTranspoz.Clear();
                        foreach (string r in redosledDijelovi)
                        {
                            if (!string.IsNullOrEmpty(r))
                            {
                                redosledTranspoz.Add(int.Parse(r));
                            }
                        }
                        break;
                    }
                }
            }
            private void GenerisiKljuc()
            {
                redosledTranspoz.Clear();
                Random random = new Random();


                List<int> redosled = new List<int>();
                for (int i = 0; i < kolona; i++)
                {
                    redosled.Add(i);
                }


                for (int i = redosled.Count - 1; i > 0; i--)
                {
                    int randomIndex = random.Next(i + 1);
                    int temp = redosled[i];
                    redosled[i] = redosled[randomIndex];
                    redosled[randomIndex] = temp;
                }

                redosledTranspoz = redosled;


                StringBuilder sb = new StringBuilder();
                sb.Append($"Redova:{red},Kolona:{kolona},Redosled:");
                foreach (int i in redosledTranspoz)
                {
                    sb.Append(i + ",");
                }
                kljuc = sb.ToString().TrimEnd(',');
            }


            public string Enkripcija()
            {
                if (string.IsNullOrEmpty(tekst))
                    return "";


                string pripremljena = tekst.ToUpper();
                while (pripremljena.Length < red * kolona)
                {
                    pripremljena += " ";
                }


                char[,] matrica = new char[red, kolona];
                int index = 0;
                for (int i = 0; i < red; i++)
                {
                    for (int j = 0; j < kolona; j++)
                    {
                        matrica[i, j] = pripremljena[index++];
                    }
                }


                StringBuilder rezultat = new StringBuilder();
                foreach (int kolona in redosledTranspoz)
                {
                    for (int reD = 0; reD < red; red++)
                    {
                        rezultat.Append(matrica[red, kolona]);
                    }
                }

                return rezultat.ToString();
            }


            public string Dekripcija(string sifrovana)
            {
                if (string.IsNullOrEmpty(sifrovana))
                    return "";


                char[,] matrica = new char[red, kolona];


                List<int> obrnutiRedosled = new List<int>();
                for (int i = 0; i < kolona; i++)
                {
                    obrnutiRedosled.Add(0);
                }

                for (int i = 0; i < redosledTranspoz.Count; i++)
                {
                    obrnutiRedosled[redosledTranspoz[i]] = i;
                }


                int poz = 0;
                for (int k = 0; k < kolona; k++)
                {
                    int originalnaKolona = redosledTranspoz[k];
                    for (int reD = 0; reD < red; red++)
                    {
                        matrica[reD, originalnaKolona] = sifrovana[poz++];
                    }
                }


                StringBuilder rezultat = new StringBuilder();
                for (int i = 0; i < red; i++)
                {
                    for (int j = 0; j < kolona; j++)
                    {
                        rezultat.Append(matrica[i, j]);
                    }
                }

                return rezultat.ToString();
            }


            public void PostaviRedosledTranspozicije(List<int> redosled)
            {
                redosledTranspoz = new List<int>(redosled);
                StringBuilder sb = new StringBuilder();
                sb.Append($"Redova:{red},Kolona:{kolona},Redosled:");
                foreach (int i in redosledTranspoz)
                {
                    sb.Append(i + ",");
                }
                kljuc = sb.ToString().TrimEnd(',');
            }
        }
    }


