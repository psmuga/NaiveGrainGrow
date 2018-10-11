using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace naiwny
{
    class Punkt
    {
        private int _id;
        private Color _kolor;
        private bool _stan;
        private int _energia;
        public Punkt()
        {
            this._kolor = Color.Black;
            this._stan = false;
            this._id = 0;
            _energia = 0;
        }

        public int GetEnergy()
        {
            return _energia;
        }

        public void SetEnergy(int i)
        {
            _energia = i;
        }

        public int GetId()
        {
            return this._id;
        }

        public void SetId(int i)
        {
            _id = i;
        }

        public Color DajKolor()
        {
            return _kolor;
        }

        public void UstawKolor(Color kolor)
        {
            this._kolor = kolor;
        }

        public bool CzyZajeta()
        {
            return _stan;
        }

        public void UstawStan(bool stan)
        {
            this._stan = stan;
        }
        
    }
}
