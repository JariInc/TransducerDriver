using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iRSDKSharp;

namespace TransducerDriver
{
    class iRacing
    {
        // API
        private iRacingSDK sdk;

        // Previous values for delta calculations
        Double prevGx, prevGy, prevGz, prevRoll, prevPitch, prevYaw, prevRF, prevLF, prevRR, prevLR  = new Double();
        Double curGx, curGy, curGz, curRoll, curPitch, curYaw, curRF, curLF, curRR, curLR = new Double();
        Double tickGx, tickGy, tickGz, tickRoll, tickPitch, tickYaw, tickRF, tickLF, tickRR, tickLR = new Double();

        // constants
        private static Double accelerationMultiplier = 10;
        private static Double rotationMultiplier = 1000;
        private static Double shockMultiplier = 10000;


        public iRacing()
        {
            // Forcing US locale for correct string to float conversion
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
        }

        public void initialize()
        {
            sdk = new iRacingSDK();        }

        public Boolean Connect()
        {
            sdk.Startup();
            if (sdk.VarHeaders != null && sdk.VarHeaders.Count > 0 && (Double)sdk.GetData("SessionTime") > 0)
                return true;
            else
                return false;
        }

        private Boolean CheckDelta(Double prevTick) {
            Double curTick = (Double)sdk.GetData("SessionTime");
            if (curTick != prevTick)
                return true;
            else
                return false;
        }

        public Boolean isConnected()
        {
            if (sdk.IsConnected())
                return true;
            else
                return false;
        }

        public Double gx
        {
            get {
                if (isConnected())
                {
                    if (CheckDelta(tickGx))
                    {
                        curGx = (Double)(Single)sdk.GetData("VertAccel") - prevGx;
                        prevGx = (Double)(Single)sdk.GetData("VertAccel");
                        tickGx = (Double)sdk.GetData("SessionTime");
                    }
                    return curGx * accelerationMultiplier;
                }
                else
                    return 0.0; 
            } set { }
        }

        public Double gy
        {
            get
            {
                if (isConnected())
                {
                    if (CheckDelta(tickGy))
                    {
                        curGy = (Double)(Single)sdk.GetData("LatAccel") - prevGy;
                        prevGy = (Double)(Single)sdk.GetData("LatAccel");
                        tickGy = (Double)sdk.GetData("SessionTime");
                    }
                    return curGy;
                }
                else
                    return 0.0 * accelerationMultiplier;
            }
            set { }
        }

        public Double gz
        {
            get
            {
                if (isConnected())
                {
                    if (CheckDelta(tickGz))
                    {
                        curGz = (Double)(Single)sdk.GetData("LongAccel") - prevGz;
                        prevGz = (Double)(Single)sdk.GetData("LongAccel");
                        tickGz = (Double)sdk.GetData("SessionTime");
                    }
                    return curGz * accelerationMultiplier;
                }
                else
                    return 0.0;
            }
            set { }
        }

        public Double RightRearShock
        {
            get
            {
                if (isConnected())
                {
                    if (CheckDelta(tickRR))
                    {
                        curRR = (Double)(Single)sdk.GetData("RRshockDefl") - prevRR;
                        prevRR = (Double)(Single)sdk.GetData("RRshockDefl");
                        tickRR = (Double)sdk.GetData("SessionTime");
                    }
                    return curRR * shockMultiplier;
                }
                else
                    return 0.0;
            }
            set { }
        }

        public Double LeftRearShock
        {
            get
            {
                if (isConnected())
                {
                    if (CheckDelta(tickLR))
                    {
                        curLR = (Double)(Single)sdk.GetData("LRshockDefl") - prevLR;
                        prevLR = (Double)(Single)sdk.GetData("LRshockDefl");
                        tickLR = (Double)sdk.GetData("SessionTime");
                    }
                    return curLR * shockMultiplier;
                }
                else
                    return 0.0;
            }
            set { }
        }

        public Double LeftFrontShock
        {
            get
            {
                if (isConnected())
                {
                    if (CheckDelta(tickLF))
                    {
                        curLF = (Double)(Single)sdk.GetData("LFshockDefl") - prevLF;
                        prevLF = (Double)(Single)sdk.GetData("LFshockDefl");
                        tickLF = (Double)sdk.GetData("SessionTime");
                    }
                    return curLF * shockMultiplier;
                }
                else
                    return 0.0;
            }
            set { }
        }

        public Double RightFrontShock
        {
            get
            {
                if (isConnected())
                {
                    if (CheckDelta(tickRF))
                    {
                        curRF = (Double)(Single)sdk.GetData("RFshockDefl") - prevRF;
                        prevRF = (Double)(Single)sdk.GetData("RFshockDefl");
                        tickRF = (Double)sdk.GetData("SessionTime");
                    }
                    return curRF * shockMultiplier;
                }
                else
                    return 0.0;
            }
            set { }
        }

        public Double Roll
        {
            get
            {
                if (isConnected())
                {
                    if (CheckDelta(tickRoll))
                    {
                        curRoll = (Double)(Single)sdk.GetData("RollRate") - prevRoll;
                        prevRoll = (Double)(Single)sdk.GetData("RollRate");
                        tickRoll = (Double)sdk.GetData("SessionTime");
                    }
                    return curRoll * rotationMultiplier;
                }
                else
                    return 0.0;
            }
            set { }
        }

        public Double Pitch
        {
            get
            {
                if (isConnected())
                {
                    if (CheckDelta(tickPitch))
                    {
                        curPitch = (Double)(Single)sdk.GetData("PitchRate") - prevPitch;
                        prevPitch = (Double)(Single)sdk.GetData("PitchRate");
                        tickPitch = (Double)sdk.GetData("SessionTime");
                    }
                    return curPitch * rotationMultiplier;
                }
                else
                    return 0.0;
            }
            set { }
        }

        public Double Yaw
        {
            get
            {
                if (isConnected())
                {
                    if (CheckDelta(tickYaw))
                    {
                        curYaw = (Double)(Single)sdk.GetData("YawRate") - prevYaw;
                        prevYaw = (Double)(Single)sdk.GetData("YawRate");
                        tickYaw = (Double)sdk.GetData("SessionTime");
                    }
                    return curPitch * rotationMultiplier;
                }
                else
                    return 0.0;
            }
            set { }
        }
    }
}
