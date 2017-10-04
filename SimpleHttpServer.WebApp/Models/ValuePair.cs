using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHttpServer.WebApp.Models
{
    public class ValuePair<TLeft, TRight>
    {
        public TLeft LeftValue;
        public TRight RightValue;

        public ValuePair(TLeft l, TRight r)
        {
            SetLeft(l);
            SetRight(r);
        }

        public void SetRight(TRight r)
        {
            if (r != null)
                RightValue = r;
            else
                throw new NullReferenceException();
        }

        public void SetLeft(TLeft l)
        {
            if (l != null)
                LeftValue = l;
            else
                throw new NullReferenceException();
        }
    }
}
