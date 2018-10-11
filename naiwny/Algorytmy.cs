using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace naiwny
{
    class Algorytmy
    {
        public Punkt[,] Mapa, NowaMapa;
        private readonly int _szerokosc;
        private readonly int _wysokosc;
        private static int _startingAmountOfGrain;
        private int _sasiedztwo = 0;
        private bool _periodyczne = true;
        private int _ukladZiaren = 0;
        private readonly Random _rand = new Random();
        private int prom;
        private Punkt[] pula;

        public Algorytmy(int w, int h, int startingAmountOfGrain,int ukladZiaren, int promien)
        {
            this._szerokosc = w + 2;                                   //w - rozmiar picturebox, tablica - rozmiaru pixelbox+2
            this._wysokosc = h + 2;
            this.prom = promien;
            _startingAmountOfGrain = startingAmountOfGrain;
            Mapa = new Punkt[_szerokosc + 2, _wysokosc + 2];
            NowaMapa = new Punkt[_szerokosc + 2, _wysokosc + 2];
            this._ukladZiaren = ukladZiaren;
            for (var i = 0; i <_szerokosc+2; i++)
            {
                for (var j = 0; j < _wysokosc+2; j++)
                {
                    Mapa[i, j] = new Punkt();                           //inicjalizacja obiektu w tablicy 
                    NowaMapa[i, j] = new Punkt();
                }
            }
            pula = new Punkt[startingAmountOfGrain];
            for (int k = 0; k < startingAmountOfGrain; k++)
            {
                pula[k] = new Punkt();
                pula[k].UstawKolor(Color.FromArgb(_rand.Next(0, 255), _rand.Next(0, 255), _rand.Next(0, 255)));
                pula[k].SetId(k);
            }
        }
        public void doIt() //
        {
            for (int i = 1; i < _szerokosc - 1; i++)
            {
                for (int j = 1; j < _wysokosc - 1; j++)
                {
                    obliczEnergie(i,j,Mapa);
                    int staraEnergia = Mapa[i, j].GetEnergy();
                    if (Mapa[i,j].GetEnergy() > 0)
                    {
                        int stareId = Mapa[i, j].GetId();

                        int x = _rand.Next(0, pula.Length);
                        NowaMapa[i,j].SetId(x);
                        NowaMapa[i,j].UstawKolor(pula[x].DajKolor());

                        obliczEnergie(i,j,NowaMapa);
                        int nowaEnergia = NowaMapa[i, j].GetEnergy();

                        if (nowaEnergia > staraEnergia) //
                        {
                            NowaMapa[i, j].SetId(stareId);
                            NowaMapa[i, j].UstawKolor(pula[stareId].DajKolor());
                            NowaMapa[i,j].SetEnergy(staraEnergia);
                        }

                    }
                    UpdateMapy();
                }
                
            }
        }

        
        public void ustawEnergie()
        {

            for (int i = 1; i < _szerokosc; i++)
            {
                for (int j = 1; j < _wysokosc; j++)
                {
                    obliczEnergie(i, j, Mapa);
                    obliczEnergie(i, j, NowaMapa);
                }
            }
        }
        public void obliczEnergie(int x, int y, Punkt[,] tab)
        {
            int counter = 0;
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (x + i == x && y + j == y)
                        continue;
                    else
                    {
                        if (tab[x, y].GetId() != tab[x + i, y + j].GetId())
                        {
                            counter++;
                        }
                    }
                }
            }
            tab[x, y].SetEnergy(counter);
        }

        private void UpdateMapy()
        {
            for (int i = 0; i < _szerokosc; i++)
            {
                for (int j = 0; j < _wysokosc; j++)
                {
                    Mapa[i,j].UstawStan(NowaMapa[i,j].CzyZajeta());             // przeniesienie z nowej mapy do naszej mapy koloru i stanu
                    Mapa[i,j].UstawKolor(NowaMapa[i,j].DajKolor());
                    Mapa[i,j].SetId(NowaMapa[i,j].GetId());
                    Mapa[i,j].SetEnergy(NowaMapa[i,j].GetEnergy());
                }
            }
        }

        public void NoweZiarno(int x, int y)                                //nowe ziarno - stan i randomowy kolor
        {
            Mapa[y + 1,x + 1].UstawStan(true);
            Mapa[y + 1,x + 1].UstawKolor(Color.FromArgb(_rand.Next(255), _rand.Next(255), _rand.Next(255)));
        }
        public void Start()                             
        {
            if (_ukladZiaren == 0)
            {
                for (int i = 0; i < _startingAmountOfGrain; i++)                                 //StartingAmountOfGrain - tyle ziaren ile chcemy
                {
                    int x = _rand.Next(_szerokosc - 2);                                // osujemy miejsce
                    int y = _rand.Next(_wysokosc - 2);
                    Mapa[x + 1,y + 1].UstawStan(true);                              //ustawiamy stan i kolor w mapie i nowej mapie
                    NowaMapa[x + 1,y + 1].UstawStan(true);
                    Mapa[x + 1,y + 1].UstawKolor(pula[i].DajKolor());
                    NowaMapa[x + 1,y + 1].UstawKolor(Mapa[x + 1,y + 1].DajKolor());
                    Mapa[x+1,y+1].SetId(pula[i].GetId());
                    NowaMapa[x + 1, y + 1].SetId(pula[i].GetId());
                }
            }   
            else if (_ukladZiaren == 1)                                                         // rownomiernie  
            {   
                double formula = Math.Sqrt(_szerokosc * _wysokosc / _startingAmountOfGrain);                   //zeby wybrac rowne odleglosci z obu stron
                int iIteracja = (int)(_wysokosc / formula +1);                                      //ile razy nam przeiteruje  
                int jIteracja = (int)(_szerokosc / formula +1);
                int polozenieX = _szerokosc / (jIteracja);                                   //pierwsze polozenie ziarna
                int polozenieY = _wysokosc / (iIteracja);

                for (int i = 0; i < iIteracja; i++)
                {
                    for (int j = 0; j < jIteracja; j++)
                    {
                        int x = (int)( polozenieX/2+ j * polozenieX);                             // kolejne ziarna
                        int y = (int)( polozenieY/2 +i * polozenieY);
                        Mapa[x,y].UstawStan(true);
                        NowaMapa[x, y].UstawStan(true);
                        Mapa[x, y].UstawKolor(Color.FromArgb(_rand.Next(255), _rand.Next(255), _rand.Next(255)));
                        NowaMapa[x, y].UstawKolor(Mapa[x, y].DajKolor());
                    }
                }
            }
            else if (_ukladZiaren == 2) //promien R
            {
                int promien = prom;
                int[] tabX = new int[_startingAmountOfGrain];
                int[] tabY = new int[_startingAmountOfGrain];
                int i = 0;

                while (i<_startingAmountOfGrain)
                {
                    var flag = false;
                    int x = _rand.Next(_szerokosc - 2-2*promien) + promien;                                // osujemy miejsce
                    int y = _rand.Next(_wysokosc - 2-2*promien) +promien;
                    for (int j = 0; j < _startingAmountOfGrain; j++)
                    {
                        var odleglosc = Math.Sqrt(Math.Pow(tabX[j] - x, 2) + Math.Pow(tabY[j] - y, 2));
                        if (odleglosc < promien)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (flag == false)
                    {
                        Mapa[x + 1, y + 1].UstawStan(true);                              //ustawiamy stan i kolor w mapie i nowej mapie
                        NowaMapa[x + 1, y + 1].UstawStan(true);
                        Mapa[x + 1, y + 1].UstawKolor(Color.FromArgb(_rand.Next(255), _rand.Next(255), _rand.Next(255)));
                        NowaMapa[x + 1, y + 1].UstawKolor(Mapa[x + 1, y + 1].DajKolor());
                        tabX[i] = x;
                        tabY[i] = y;
                        i++;
                    }
                    
                }
            }
        }
        
        private void Moore(int i, int j)
        {
            if (Mapa[i,j].CzyZajeta())
            {
                if (!Mapa[i - 1,j - 1].CzyZajeta())
                {
                    NowaMapa[i - 1,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j - 1].UstawStan(true);
                    NowaMapa[i - 1, j - 1].SetId(Mapa[i,j].GetId());
                }
                if (!Mapa[i - 1,j].CzyZajeta())
                {
                    NowaMapa[i - 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j].UstawStan(true);
                    NowaMapa[i - 1,j].SetId(Mapa[i, j].GetId());
                }
                if (!Mapa[i - 1,j + 1].CzyZajeta())
                {
                    NowaMapa[i - 1,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j + 1].UstawStan(true);
                    NowaMapa[i - 1, j + 1].SetId(Mapa[i, j].GetId());
                }
                if (!Mapa[i,j - 1].CzyZajeta())
                {
                    NowaMapa[i,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j - 1].UstawStan(true);
                    NowaMapa[i, j - 1].SetId(Mapa[i, j].GetId());
                }
                if (!Mapa[i,j + 1].CzyZajeta())
                {
                    NowaMapa[i,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j + 1].UstawStan(true);
                    NowaMapa[i, j + 1].SetId(Mapa[i, j].GetId());
                }
                if (!Mapa[i + 1,j - 1].CzyZajeta())
                {
                    NowaMapa[i + 1,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j - 1].UstawStan(true);
                    NowaMapa[i + 1, j - 1].SetId(Mapa[i, j].GetId());
                }
                if (!Mapa[i + 1,j].CzyZajeta())
                {
                    NowaMapa[i + 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j].UstawStan(true);
                    NowaMapa[i + 1, j].SetId(Mapa[i, j].GetId());
                }
                if (!Mapa[i + 1,j + 1].CzyZajeta())
                {
                    NowaMapa[i + 1,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j + 1].UstawStan(true);
                    NowaMapa[i + 1, j + 1].SetId(Mapa[i, j].GetId());
                }
            }
        }

        private void Neumann(int i, int j)
        {
            if (Mapa[i,j].CzyZajeta())
            {
                if (!Mapa[i - 1,j].CzyZajeta())
                {
                    NowaMapa[i - 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j].UstawStan(true);
                }
                if (!Mapa[i,j - 1].CzyZajeta())
                {
                    NowaMapa[i,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j - 1].UstawStan(true);
                }
                if (!Mapa[i,j + 1].CzyZajeta())
                {
                    NowaMapa[i,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j + 1].UstawStan(true);
                }
                if (!Mapa[i + 1,j].CzyZajeta())
                {
                    NowaMapa[i + 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j].UstawStan(true);
                }
            }
        }

        private void HexaLewo(int i, int j)
        {
            if (Mapa[i,j].CzyZajeta())
            {
                if (!Mapa[i - 1,j - 1].CzyZajeta())
                {
                    NowaMapa[i - 1,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j - 1].UstawStan(true);
                }
                if (!Mapa[i - 1,j].CzyZajeta())
                {
                    NowaMapa[i - 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j].UstawStan(true);
                }
                if (!Mapa[i,j - 1].CzyZajeta())
                {
                    NowaMapa[i,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j - 1].UstawStan(true);
                }
                if (!Mapa[i,j + 1].CzyZajeta())
                {
                    NowaMapa[i,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j + 1].UstawStan(true);
                }
                if (!Mapa[i + 1,j].CzyZajeta())
                {
                    NowaMapa[i + 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j].UstawStan(true);
                }
                if (!Mapa[i + 1,j + 1].CzyZajeta())
                {
                    NowaMapa[i + 1,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j + 1].UstawStan(true);
                }
            }
        }

        private void HexaPrawo(int i, int j)
        {
            if (Mapa[i,j].CzyZajeta())
            {
                if (!Mapa[i - 1,j].CzyZajeta())
                {
                    NowaMapa[i - 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j].UstawStan(true);
                }
                if (!Mapa[i - 1,j + 1].CzyZajeta())
                {
                    NowaMapa[i - 1,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j + 1].UstawStan(true);
                }
                if (!Mapa[i,j - 1].CzyZajeta())
                {
                    NowaMapa[i,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j - 1].UstawStan(true);
                }
                if (!Mapa[i,j + 1].CzyZajeta())
                {
                    NowaMapa[i,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j + 1].UstawStan(true);
                }
                if (!Mapa[i + 1,j - 1].CzyZajeta())
                {
                    NowaMapa[i + 1,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j - 1].UstawStan(true);
                }
                if (!Mapa[i + 1,j].CzyZajeta())
                {
                    NowaMapa[i + 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j].UstawStan(true);
                }
            }
        }

        private void PentaLewo(int i, int j)
        {
            if (Mapa[i,j].CzyZajeta())
            {
                if (!Mapa[i - 1,j - 1].CzyZajeta())
                {
                    NowaMapa[i - 1,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j - 1].UstawStan(true);
                }
                if (!Mapa[i - 1,j].CzyZajeta())
                {
                    NowaMapa[i - 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j].UstawStan(true);
                }

                if (!Mapa[i,j - 1].CzyZajeta())
                {
                    NowaMapa[i,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j - 1].UstawStan(true);
                }

                if (!Mapa[i + 1,j - 1].CzyZajeta())
                {
                    NowaMapa[i + 1,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j - 1].UstawStan(true);
                }
                if (!Mapa[i + 1,j].CzyZajeta())
                {
                    NowaMapa[i + 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j].UstawStan(true);
                }
            }
        }

        private void PentaPrawo(int i, int j)
        {
            if (Mapa[i,j].CzyZajeta())
            {

                if (!Mapa[i - 1,j].CzyZajeta())
                {
                    NowaMapa[i - 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j].UstawStan(true);
                }
                if (!Mapa[i - 1,j + 1].CzyZajeta())
                {
                    NowaMapa[i - 1,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j + 1].UstawStan(true);
                }
                if (!Mapa[i,j + 1].CzyZajeta())
                {
                    NowaMapa[i,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j + 1].UstawStan(true);
                }
                if (!Mapa[i + 1,j].CzyZajeta())
                {
                    NowaMapa[i + 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j].UstawStan(true);
                }
                if (!Mapa[i + 1,j + 1].CzyZajeta())
                {
                    NowaMapa[i + 1,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j + 1].UstawStan(true);
                }
            }
        }

        private void PentaGora(int i, int j)
        {
            if (Mapa[i,j].CzyZajeta())
            { 
                if (!Mapa[i,j - 1].CzyZajeta())
                {
                    NowaMapa[i,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j - 1].UstawStan(true);
                }
                if (!Mapa[i,j + 1].CzyZajeta())
                {
                    NowaMapa[i,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j + 1].UstawStan(true);
                }
                if (!Mapa[i + 1,j - 1].CzyZajeta())
                {
                    NowaMapa[i + 1,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j - 1].UstawStan(true);
                }
                if (!Mapa[i + 1,j].CzyZajeta())
                {
                    NowaMapa[i + 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j].UstawStan(true);
                }
                if (!Mapa[i + 1,j + 1].CzyZajeta())
                {
                    NowaMapa[i + 1,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i + 1,j + 1].UstawStan(true);
                }
            }
        }

        private void PentaDol(int i, int j)
        {
            if (Mapa[i,j].CzyZajeta())
            {
                if (!Mapa[i - 1,j - 1].CzyZajeta())
                {
                    NowaMapa[i - 1,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j - 1].UstawStan(true);
                }
                if (!Mapa[i - 1,j].CzyZajeta())
                {
                    NowaMapa[i - 1,j].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j].UstawStan(true);
                }
                if (!Mapa[i - 1,j + 1].CzyZajeta())
                {
                    NowaMapa[i - 1,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i - 1,j + 1].UstawStan(true);
                }
                if (!Mapa[i,j - 1].CzyZajeta())
                {
                    NowaMapa[i,j - 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j - 1].UstawStan(true);
                }
                if (!Mapa[i,j + 1].CzyZajeta())
                {
                    NowaMapa[i,j + 1].UstawKolor(Mapa[i,j].DajKolor());
                    NowaMapa[i,j + 1].UstawStan(true);
                }
            }
        }

        private void ZrobPeriodyczne()
        {
            for (int i = 0; i < (_wysokosc -1); i++) //poziom
            {
                Mapa[0,i] = Mapa[_szerokosc - 2,i];                      
                Mapa[_szerokosc - 1,i] = Mapa[1,i];

            }
            for (int i = 0; i < (_szerokosc -1); i++) //pion
            {
                Mapa[i,0] = Mapa[i, _wysokosc - 2];
                Mapa[i, _wysokosc - 1] = Mapa[i,1];
            }

            

        }
        public void NextStep()
        {
                                                                        //kolejne kroki rozrostu
            if (_periodyczne)
                ZrobPeriodyczne();
            for (int i = 1; i < _szerokosc - 1; i++)
            {
                for (int j = 1; j < _wysokosc - 1; j++)
                {
                    if (_sasiedztwo == 0) Moore(i, j);
                    else if (_sasiedztwo == 1) Neumann(i, j);
                    else if (_sasiedztwo == 2) HexaLewo(i, j);
                    else if (_sasiedztwo == 3) HexaPrawo(i, j);
                    else if (_sasiedztwo == 4)
                    {
                        int tmp = _rand.Next(0, 2);
                        if (tmp==1) HexaLewo(i, j);
                        else HexaPrawo(i, j);
                    }
                    else if (_sasiedztwo == 5)
                    {
                        int tmp = RandPenta();
                        if (tmp == 0) PentaDol(i, j);
                        else if (tmp == 1) PentaGora(i, j);
                        else if (tmp == 2) PentaLewo(i, j);
                        else if (tmp == 3) PentaPrawo(i, j);
                    }
                }
            }
            UpdateMapy();
        }

        private int RandPenta()
        {
            int a = _rand.Next(0,3);
            return a;
        }

        public void UstawSasiedztwo(int sasiedztwo)
        {
            this._sasiedztwo = sasiedztwo;
        }

        public void UstawPeriodyczne(bool periodyczne)
        {
            this._periodyczne = periodyczne;
        }

        public void UstawUkladZiaren(int ukladZiaren)
        {
            this._ukladZiaren = ukladZiaren;
        }
    }
}
