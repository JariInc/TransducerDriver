using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using IrrKlang;

namespace TransducerDriver
{
    public partial class TransducerDriver : Form
    {
        // constants
        private const Int32 samplerate = 44100;

        // IrrKlang
        ISoundEngine engine = new ISoundEngine();
        ISoundDeviceList dlist = new IrrKlang.ISoundDeviceList(IrrKlang.SoundDeviceListType.PlaybackDevice);

        // Sim
        iRacing ir = new iRacing();

        // Data
        Double[] leftSamples = new Double[3];
        Double[] rightSamples = new Double[3];

        // 16ms sink
        AudioSource sink = new AudioSource("sink", samplerate / 60, samplerate);

        // debug
        BinaryWriter output = new BinaryWriter(File.Open("output.raw", FileMode.Create));

        public TransducerDriver()
        {
            InitializeComponent();
            ir.initialize();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Add each device to a combo box.
            for (int i = 0; i < dlist.DeviceCount; i++)
            {
                OutputDevice.Items.Add(dlist.getDeviceDescription(i) + "\n");

                // select last one
                if (dlist.getDeviceID(i) == Properties.Settings.Default.DeviceID)
                    OutputDevice.SelectedItem = dlist.getDeviceDescription(i) + "\n";
            }

            // upgrade settings from previous versions
            if (Properties.Settings.Default.UpdateSettings)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpdateSettings = false;
                Properties.Settings.Default.Save();
            }

            // load previous settings
            // gains
            gxGain.Value = Properties.Settings.Default.gxGain;
            gyGain.Value = Properties.Settings.Default.gyGain;
            gzGain.Value = Properties.Settings.Default.gzGain;

            rollGain.Value = Properties.Settings.Default.rollGain;
            pitchGain.Value = Properties.Settings.Default.pitchGain;
            yawGain.Value = Properties.Settings.Default.yawGain;

            LFShockGain.Value = Properties.Settings.Default.LFShockGain;
            RFShockGain.Value = Properties.Settings.Default.RFShockGain;
            LRShockGain.Value = Properties.Settings.Default.LRShockGain;
            RRShockGain.Value = Properties.Settings.Default.RRShockGain;

            // Enables
            gxLeftChannelEnable.Checked = Properties.Settings.Default.gxLeftChannelEnable;
            gxRightChannelEnable.Checked = Properties.Settings.Default.gxRightChannelEnable;

            gyLeftChannelEnable.Checked = Properties.Settings.Default.gyLeftChannelEnable;
            gyRightChannelEnable.Checked = Properties.Settings.Default.gyRightChannelEnable;

            gzLeftChannelEnable.Checked = Properties.Settings.Default.gzLeftChannelEnable;
            gzRightChannelEnable.Checked = Properties.Settings.Default.gzRightChannelEnable;

            rollLeftChannelEnable.Checked = Properties.Settings.Default.rollLeftChannelEnable;
            rollRightChannelEnable.Checked = Properties.Settings.Default.rollRightChannelEnable;

            pitchLeftChannelEnable.Checked = Properties.Settings.Default.pitchLeftChannelEnable;
            pitchRightChannelEnable.Checked = Properties.Settings.Default.pitchRightChannelEnable;

            yawLeftChannelEnable.Checked = Properties.Settings.Default.yawLeftChannelEnable;
            yawRightChannelEnable.Checked = Properties.Settings.Default.yawRightChannelEnable;

            LFLeftChannelEnable.Checked = Properties.Settings.Default.LFLeftChannelEnable;
            LFRightChannelEnable.Checked = Properties.Settings.Default.LFRightChannelEnable;

            RFLeftChannelEnable.Checked = Properties.Settings.Default.RFLeftChannelEnable;
            RFRightChannelEnable.Checked = Properties.Settings.Default.RFRightChannelEnable;

            LRLeftChannelEnable.Checked = Properties.Settings.Default.LRLeftChannelEnable;
            LRRightChannelEnable.Checked = Properties.Settings.Default.LRRightChannelEnable;

            RRLeftChannelEnable.Checked = Properties.Settings.Default.RRLeftChannelEnable;
            RRRightChannelEnable.Checked = Properties.Settings.Default.RRRightChannelEnable;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            AudioSource sine = new AudioSource("sine", samplerate, samplerate);

            for (Int32 i = 0; i < sine.length; i++)
                sine.values[i] = Math.Sin(2.0 * Math.PI * i / (samplerate / 3000)) * (Int16.MaxValue * 0.707);

            sine.LPF((Int32)commonGain.Value, 64);
            sine.Play(engine);
        }

        private void OutputDevice_SelectedIndexChanged(object sender, EventArgs e)
        {
            engine = new ISoundEngine(
                SoundOutputDriver.AutoDetect,
                SoundEngineOptionFlag.DefaultOptions,
                dlist.getDeviceID(OutputDevice.SelectedIndex));

            // save
            Properties.Settings.Default.DeviceID = dlist.getDeviceID(OutputDevice.SelectedIndex);
            Properties.Settings.Default.Save();
        }

        private void dataUpdate_Tick(object sender, EventArgs e)
        {
            if (ir.isConnected())
            {
                // acceleration
                gxValue.Value = (Int32)Math.Min(Math.Abs(ir.gx), gxValue.Maximum); gxValueLabel.Text = ir.gx.ToString("+0.000;-0.000"); checkClip(gxValue);
                gyValue.Value = (Int32)Math.Min(Math.Abs(ir.gy), gyValue.Maximum); gyValueLabel.Text = ir.gy.ToString("+0.000;-0.000"); checkClip(gyValue);
                gzValue.Value = (Int32)Math.Min(Math.Abs(ir.gz), gzValue.Maximum); gzValueLabel.Text = ir.gz.ToString("+0.000;-0.000"); checkClip(gzValue);

                // rotation
                rollValue.Value = (Int32)Math.Min(Math.Abs(ir.Roll), rollValue.Maximum); rollValueLabel.Text = ir.Roll.ToString("+0.000;-0.000"); checkClip(rollValue);
                pitchValue.Value = (Int32)Math.Min(Math.Abs(ir.Pitch), pitchValue.Maximum); pitchValueLabel.Text = ir.Pitch.ToString("+0.000;-0.000"); checkClip(pitchValue);
                yawValue.Value = (Int32)Math.Min(Math.Abs(ir.Yaw), yawValue.Maximum); yawValueLabel.Text = ir.Yaw.ToString("+0.000;-0.000"); checkClip(yawValue);

                // shock deflection
                LFShockValue.Value = (Int32)Math.Min(Math.Abs(ir.LeftFrontShock), LFShockValue.Maximum); LFShockValueLabel.Text = ir.LeftFrontShock.ToString("+0.000;-0.000"); checkClip(LFShockValue);
                RFShockValue.Value = (Int32)Math.Min(Math.Abs(ir.RightFrontShock), RFShockValue.Maximum); RFShockValueLabel.Text = ir.RightFrontShock.ToString("+0.000;-0.000"); checkClip(RFShockValue);
                LRShockValue.Value = (Int32)Math.Min(Math.Abs(ir.LeftRearShock), LRShockValue.Maximum); LRShockValueLabel.Text = ir.LeftRearShock.ToString("+0.000;-0.000"); checkClip(LRShockValue);
                RRShockValue.Value = (Int32)Math.Min(Math.Abs(ir.RightRearShock), RRShockValue.Maximum); RRShockValueLabel.Text = ir.RightRearShock.ToString("+0.000;-0.000"); checkClip(RRShockValue);

                // samples
                Double leftChannelSample = new Double();
                Double rightChannelSample = new Double();
                Int32 leftChannelScale = 0;
                Int32 rightChannelScale = 0;

                // sum enabled
                if (gxLeftChannelEnable.Checked || gxRightChannelEnable.Checked)
                {
                    Double sample = ir.gx * (Double)gxGain.Value * 3.2768;

                    if (gxLeftChannelEnable.Checked)
                    {
                        leftChannelSample += sample;
                        leftChannelScale++;
                    }
                    if (gxRightChannelEnable.Checked)
                    {
                        rightChannelSample += sample;
                        rightChannelScale++;
                    }
                }

                if (gyLeftChannelEnable.Checked || gyRightChannelEnable.Checked)
                {
                    Double sample = ir.gy * (Double)gyGain.Value * 3.2768;

                    if (gyLeftChannelEnable.Checked) 
                    {
                        leftChannelSample += sample;
                        leftChannelScale++;
                    }
                    if (gyRightChannelEnable.Checked)
                    {
                        rightChannelSample += sample;
                        rightChannelScale++;
                    }
                }

                if (gzLeftChannelEnable.Checked || gzRightChannelEnable.Checked)
                {
                    Double sample = ir.gz * (Double)gzGain.Value * 3.2768;

                    if (gzLeftChannelEnable.Checked)
                    {
                        leftChannelSample += sample;
                        leftChannelScale++;
                    }
                    if (gzRightChannelEnable.Checked)
                    {
                        rightChannelSample += sample;
                        rightChannelScale++;
                    }
                }

                // scale down
                leftChannelSample /= leftChannelScale;
                rightChannelSample /= rightChannelScale;

                // buffer mangling
                leftSamples[1] = leftSamples[0];
                leftSamples[2] = leftSamples[1];
                leftSamples[0] = leftChannelSample;

                rightSamples[1] = rightSamples[0];
                rightSamples[2] = rightSamples[1];
                rightSamples[0] = rightChannelSample;

                /*
                Double[] tmp = new Double[5] {1.0, 0.0, 0.0, 0.0, 0.0};
                tmp = upsample(tmp, 2);
                */
                
                Double[] tmp = new Double[0];
                
                tmp = upsample(leftSamples, 15);
                tmp = upsample(tmp, 7);
                tmp = upsample(tmp, 7);
                Array.Copy(tmp, samplerate / 60, sink.values, 0, samplerate / 60);
                /*
                 
                sink.values = new Double[sink.values.Length];
                sink.values[0] = leftSamples[1] * samplerate / 60;
                sink.values[samplerate / 60] = leftSamples[1] * samplerate / 60;
                sink.values[2 * samplerate / 60] = leftSamples[0] * samplerate / 60;

                sink.LPF(30, samplerate / 30);
                */

                sink.Play(engine);

                List<Double> values = sink.values.ToList();

                values.ForEach(delegate(Double value)
                {
                    Int16 s16 = (Int16)value;
                    output.Write(s16);
                });

                // output graph
                leftChannelOutput.Value = (Int32)Math.Min(Math.Abs(leftChannelSample), leftChannelOutput.Maximum); checkClip(leftChannelOutput);
                rightChannelOutput.Value = (Int32)Math.Min(Math.Abs(rightChannelSample), rightChannelOutput.Maximum); checkClip(rightChannelOutput);
                
            }
            else
                ir.Connect();
        }

        private Double[] upsample(Double[] src, Int32 L) 
        {
            // reset destination
            Double[] dst = new Double[src.Length * L];
            Double[] result = new Double[src.Length * L];

            // copy values
            for (Int32 i = 0; i < src.Length; i++)
                dst[i * L] = src[i] * L;

            // anti-imaging filter
            Double[] h = genLowPassFilter(2 * L, 1.0 / L);

            //y[n]=b0x[n]+b1x[n-1]+....bmx[n-M]
            for (Int32 i = 0; i < dst.Length; i++)
            {
                for (Int32 j = 0; j < h.Length; j++)
                {
                    if (i - j >= 0)
                        result[i] += dst[i - j] * h[j] / L;
                    else
                        result[i] += dst[i - j + dst.Length] * h[j] * L;
                }
            }

            return result;
        }

        private Double[] genLowPassFilter(Int32 length, Double cutoff)
        {
            Int32 M = length - 1;
            Double[] h = new Double[length+1];

            for (Int32 n = 0; n < length; n++)
            {
                /*
                if (n == M / 2)
                    h[n] = 2.0 * cutoff;
                else
                {
                    Double i = (n - M / 2);
                    //h[n] =  Math.Sin(2.0 * Math.PI * cutoff * i) / (Math.PI * i) * (0.54 - 0.46 * Math.Cos(2.0 * Math.PI * n / M));
                    h[length/2 + n] = Math.Sin(2.0 * Math.PI * cutoff * i) / (Math.PI * i) * (0.42 + 0.5 * Math.Cos(2.0 * Math.PI * n / M) + 0.08 * Math.Cos(4.0 * Math.PI * n / M));
                    h[length / 2 - n] = h[length / 2 + n];
                }
                */
                h[n] = 0.5 * (1 - Math.Cos(2 * Math.PI * n / (length)));
            }

            return h;
        }

        private void checkClip(ProgressBar pb)
        {
            if (pb.ForeColor == SystemColors.ActiveCaption && pb.Value >= pb.Maximum)
                pb.ForeColor = Color.FromArgb(255, 0, 0);
            else if(pb.ForeColor == Color.FromArgb(255, 0, 0))
                pb.ForeColor = SystemColors.ActiveCaption;
        }

        private void gxGain_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.gxGain = gxGain.Value;
            Properties.Settings.Default.Save();
        }

        private void gyGain_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.gyGain = gyGain.Value;
            Properties.Settings.Default.Save();
        }

        private void gzGain_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.gzGain = gzGain.Value;
            Properties.Settings.Default.Save();
        }

        private void rollGain_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.rollGain = rollGain.Value;
            Properties.Settings.Default.Save();
        }

        private void pitchGain_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.pitchGain = pitchGain.Value;
            Properties.Settings.Default.Save();
        }

        private void yawGain_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.yawGain = yawGain.Value;
            Properties.Settings.Default.Save();
        }

        private void LFShockGain_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LFShockGain = LFShockGain.Value;
            Properties.Settings.Default.Save();
        }

        private void RFShockGain_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RFShockGain = RFShockGain.Value;
            Properties.Settings.Default.Save();
        }

        private void LRShockGain_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LRShockGain = LRShockGain.Value;
            Properties.Settings.Default.Save();
        }

        private void RRShockGain_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RRShockGain = RRShockGain.Value;
            Properties.Settings.Default.Save();
        }

        private void gxLeftChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.gxLeftChannelEnable = gxLeftChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (gxLeftChannelEnable.Checked || gxRightChannelEnable.Checked)
                gxValue.ForeColor = SystemColors.ActiveCaption;
            else
                gxValue.ForeColor = SystemColors.ControlDark;
        }

        private void gxRightChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.gxRightChannelEnable = gxRightChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (gxLeftChannelEnable.Checked || gxRightChannelEnable.Checked)
                gxValue.ForeColor = SystemColors.ActiveCaption;
            else
                gxValue.ForeColor = SystemColors.ControlDark;
        }

        private void gyLeftChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.gyLeftChannelEnable = gyLeftChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (gyLeftChannelEnable.Checked || gyRightChannelEnable.Checked)
                gyValue.ForeColor = SystemColors.ActiveCaption;
            else
                gyValue.ForeColor = SystemColors.ControlDark;
        }

        private void gyRightChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.gyRightChannelEnable = gyRightChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (gyLeftChannelEnable.Checked || gyRightChannelEnable.Checked)
                gyValue.ForeColor = SystemColors.ActiveCaption;
            else
                gyValue.ForeColor = SystemColors.ControlDark;
        }

        private void gzLeftChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.gzLeftChannelEnable = gzLeftChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (gzLeftChannelEnable.Checked || gzRightChannelEnable.Checked)
                gzValue.ForeColor = SystemColors.ActiveCaption;
            else
                gzValue.ForeColor = SystemColors.ControlDark;
        }

        private void gzRightChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.gzRightChannelEnable = gzRightChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (gzLeftChannelEnable.Checked || gzRightChannelEnable.Checked)
                gzValue.ForeColor = SystemColors.ActiveCaption;
            else
                gzValue.ForeColor = SystemColors.ControlDark;
        }

        private void rollLeftChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.rollLeftChannelEnable = rollLeftChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (rollLeftChannelEnable.Checked || rollRightChannelEnable.Checked)
                rollValue.ForeColor = SystemColors.ActiveCaption;
            else
                rollValue.ForeColor = SystemColors.ControlDark;
        }

        private void rollRightChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.rollRightChannelEnable = rollRightChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (rollLeftChannelEnable.Checked || rollRightChannelEnable.Checked)
                rollValue.ForeColor = SystemColors.ActiveCaption;
            else
                rollValue.ForeColor = SystemColors.ControlDark;
        }

        private void pitchLeftChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.pitchLeftChannelEnable = pitchLeftChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (pitchLeftChannelEnable.Checked || pitchRightChannelEnable.Checked)
                pitchValue.ForeColor = SystemColors.ActiveCaption;
            else
                pitchValue.ForeColor = SystemColors.ControlDark;
        }

        private void pitchRightChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.pitchRightChannelEnable = pitchRightChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (pitchLeftChannelEnable.Checked || pitchRightChannelEnable.Checked)
                pitchValue.ForeColor = SystemColors.ActiveCaption;
            else
                pitchValue.ForeColor = SystemColors.ControlDark;
        }

        private void yawLeftChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.yawLeftChannelEnable = yawLeftChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (yawLeftChannelEnable.Checked || yawRightChannelEnable.Checked)
                yawValue.ForeColor = SystemColors.ActiveCaption;
            else
                yawValue.ForeColor = SystemColors.ControlDark;
        }

        private void yawRightChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.yawRightChannelEnable = yawRightChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (yawLeftChannelEnable.Checked || yawRightChannelEnable.Checked)
                yawValue.ForeColor = SystemColors.ActiveCaption;
            else
                yawValue.ForeColor = SystemColors.ControlDark;
        }

        private void LFLeftChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LFLeftChannelEnable = LFLeftChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (LFLeftChannelEnable.Checked || LFRightChannelEnable.Checked)
                LFShockValue.ForeColor = SystemColors.ActiveCaption;
            else
                LFShockValue.ForeColor = SystemColors.ControlDark;
        }

        private void LFRightChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LFRightChannelEnable = LFRightChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (LFLeftChannelEnable.Checked || LFRightChannelEnable.Checked)
                LFShockValue.ForeColor = SystemColors.ActiveCaption;
            else
                LFShockValue.ForeColor = SystemColors.ControlDark;
        }

        private void RFLeftChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RFLeftChannelEnable = RFLeftChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (RFLeftChannelEnable.Checked || RFRightChannelEnable.Checked)
                RFShockValue.ForeColor = SystemColors.ActiveCaption;
            else
                RFShockValue.ForeColor = SystemColors.ControlDark;
        }

        private void RFRightChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RFRightChannelEnable = RFRightChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (RFLeftChannelEnable.Checked || RFRightChannelEnable.Checked)
                RFShockValue.ForeColor = SystemColors.ActiveCaption;
            else
                RFShockValue.ForeColor = SystemColors.ControlDark;
        }

        private void LRLeftChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LRLeftChannelEnable = LRLeftChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (LRLeftChannelEnable.Checked || LRRightChannelEnable.Checked)
                LRShockValue.ForeColor = SystemColors.ActiveCaption;
            else
                LRShockValue.ForeColor = SystemColors.ControlDark;
        }

        private void LRRightChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.LRRightChannelEnable = LRRightChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (LRLeftChannelEnable.Checked || LRRightChannelEnable.Checked)
                LRShockValue.ForeColor = SystemColors.ActiveCaption;
            else
                LRShockValue.ForeColor = SystemColors.ControlDark;
        }

        private void RRLeftChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RRLeftChannelEnable = RRLeftChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (RRLeftChannelEnable.Checked || RRRightChannelEnable.Checked)
                RRShockValue.ForeColor = SystemColors.ActiveCaption;
            else
                RRShockValue.ForeColor = SystemColors.ControlDark;
        }

        private void RRRightChannelEnable_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.RRRightChannelEnable = RRRightChannelEnable.Checked;
            Properties.Settings.Default.Save();

            if (RRLeftChannelEnable.Checked || RRRightChannelEnable.Checked)
                RRShockValue.ForeColor = SystemColors.ActiveCaption;
            else
                RRShockValue.ForeColor = SystemColors.ControlDark;
        }

        private void commonGain_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.commonGain = commonGain.Value;
            Properties.Settings.Default.Save();
        }
    }
}
