using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using IrrKlang;

namespace TransducerDriver
{
    class AudioSource
    {
        public Double[] values;
        public Int32 length;
        public String name;
        private ISoundSource source;
        private AudioFormat fmt = new AudioFormat();

        /*
        private FileStream stream = new FileStream("output.raw", FileMode.Create);
        BinaryWriter writer;
        */

        public AudioSource(String name, Int32 length, Int32 samplerate)
        {
            this.length = length;
            this.name = name;
            this.values = new Double[length];
            this.fmt.ChannelCount = 1;
            this.fmt.Format = SampleFormat.Signed16Bit;
            this.fmt.SampleRate = samplerate;
            this.fmt.FrameCount = length;

            //writer = new BinaryWriter(stream);
        }

        public void Play(ISoundEngine engine) {

            Byte[] data = new Byte[this.length * 2]; // signed 16-bit

            for (Int32 i = 0; i < this.length; i++)
            {
                Int16 sample = (Int16)values[i];
                data[i * 2] += (byte)(sample >> 8);
                data[(i * 2) + 1] += (byte)(sample & 0xFF);
                //writer.Write(sample);
            }

            engine.RemoveSoundSource(name);
            this.source = engine.AddSoundSourceFromPCMData(data, name, fmt);
            engine.Play2D(name, true);
        }

        public void LPF(Int32 cutoff, Int32 taps)
        {
            Double[] result = new Double[length];

            Filter f = new Filter(taps);
            f.setLowPassFilter(cutoff, fmt.SampleRate);

            /*
            for (Int32 i = 0; i < result.Length; i++)
            {
                for (Int32 j = 0; j < taps; j++)
                {
                    Int32 validx = i - j;
                    if (validx > 0 && validx < length)
                        result[i] += values[validx] * f.h[j];
                    else if(validx < 0)
                        result[i] += values[length+validx] * f.h[j];
                    else if(validx > length)
                        result[i] += values[validx - length] * f.h[j];
                }
            }
            */

            int M = f.h.Length;
            int n = values.Length;
            //y[n]=b0x[n]+b1x[n-1]+....bmx[n-M]
            var y = new double[n];
            for (int yi = 0; yi < n; yi++)
            {
                double t = 0.0;
                for (int bi = M - 1; bi >= 0; bi--)
                {
                    if (yi - bi < 0) continue;

                    t += f.h[bi] * values[yi - bi];
                }
                result[yi] = t;
            }

            Array.Copy(result, values, length);
        }
    }
}
