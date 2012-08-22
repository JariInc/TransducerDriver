using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TransducerDriver
{
    class Filter
    {
        public Double[] h;
        private Double[] buf;
        public Int32 length;

        public Filter(Int32 len)
        {
            length = len;
            h = new Double[length];
            buf = new Double[length];
        }

        public void setLowPassFilter(Int32 cutoff, Int32 samplerate) 
        {
            Int32 M = length - 1;
            Double f_t = (Double)cutoff / (Double)samplerate;

            for (Int32 n = 0; n < length; n++)
            {
                if (n == M / 2)
                    h[n] = 2.0 * f_t;
                else
                {
                    Double i = (n - M / 2);
                    //h[n] =  Math.Sin(2.0 * Math.PI * f_t * i) / (Math.PI * i) * (0.54 - 0.46 * Math.Cos(2.0 * Math.PI * n / M));
                    h[n] = Math.Sin(2.0 * Math.PI * f_t * i) / (Math.PI * i) * (0.42 + 0.5 * Math.Cos(2.0 * Math.PI * n / M) + 0.08 * Math.Cos(4.0 * Math.PI * n / M));
                }
            }
        }
    }
}
