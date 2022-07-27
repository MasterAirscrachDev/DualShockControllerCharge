using System.IO;
using DualSenseAPI;
using DualSenseAPI.State;
using System.Reactive.Linq;

namespace DualShockControllerCharge
{
    public partial class DSChargeView : Form
    {
        DualSense ds;
        bool controllerFound, accurate = false, startCheck = false, chargingOnTick = false;
        int lastcheck = 0;
        TimeSpan chargeInterval, drainInterval;
        DateTime checkChange;
        public DSChargeView() => InitializeComponent();
        private void OnLoad(object sender, EventArgs e)
        {
            GetDurations();
            GetController();
        }
        void SetInfo(string[] info) {
            statusContext.Items.Clear();
            statusContext.Items.Add("DSChargeViewer");
            for (int i = 0; i < info.Length; i++)
            { statusContext.Items.Add(info[i]); }
        }
        void GetDurations()
        {
            //get a file in the local directory called stats.mem
            try {
                StreamReader sr = new StreamReader($"{Directory.GetCurrentDirectory()}\\stats.mem");
                drainInterval = TimeSpan.FromSeconds(double.Parse(sr.ReadLine()));
                chargeInterval = TimeSpan.FromSeconds(double.Parse(sr.ReadLine()));
                sr.Close();
            }
            catch {
                //it doesnt exist, create it
                StreamWriter sw = new StreamWriter($"{Directory.GetCurrentDirectory()}\\stats.mem");
                sw.WriteLine("3600"); sw.WriteLine("3600");
                sw.Close();
            }
        }
        void SetDurations()
        {
            try {
                //open stats.mem for writing
                StreamWriter sw = new StreamWriter($"{Directory.GetCurrentDirectory()}\\stats.mem");
                sw.WriteLine(Math.Round(drainInterval.TotalSeconds).ToString());
                sw.WriteLine(Math.Round(chargeInterval.TotalSeconds).ToString());
                sw.Close();
            }
            catch { }//shouldn't fail
        }
        async void GetController()
        {
            try { //get the first controller

                ds = DualSense.EnumerateControllers().First();
                controllerFound = true;
                ds.Acquire(); ds.BeginPolling(10000);
                ds.OnStatePolled += Ds_OnStatePolled;
            }
            catch { 
                controllerFound = false; 
                SetInfo(new string[] { "No Controller Found", "Please connect a controller" });
                while (!controllerFound)
                { await Task.Delay(10000); GetController(); }
            }
        }

        private void Ds_OnStatePolled(DualSense sender)
        {
            try {
                if (InvokeRequired) {
                    this.Invoke(new MethodInvoker(delegate {
                        if (controllerFound) {
                            DualSenseInputState state = ds.InputState; //get the state of input
                            bool oncharge = state.BatteryStatus.IsCharging; //are we charging?
                            if ((int)state.BatteryStatus.Level != lastcheck) {
                                if (!startCheck) { startCheck = true; lastcheck = (int)state.BatteryStatus.Level; }
                                else {
                                    if (accurate) {
                                        if (chargingOnTick && oncharge) {
                                            chargeInterval = DateTime.Now - checkChange;
                                            SetDurations();
                                        }
                                        else if (oncharge) { chargingOnTick = true; }
                                        else {
                                            drainInterval = DateTime.Now - checkChange;
                                            chargingOnTick = false;
                                            SetDurations();
                                        }
                                    }
                                    lastcheck = (int)state.BatteryStatus.Level;
                                    checkChange = DateTime.Now;
                                    accurate = true;
                                }
                            }
                            string charge; int chargepercent;
                            if (!accurate) { charge = $"~{state.BatteryStatus.Level * 10}% (Calibrating)"; chargepercent = (int)state.BatteryStatus.Level * 10; }
                            else {
                                if (oncharge) {
                                    TimeSpan t = DateTime.Now - checkChange; //get the time between now and the last check
                                    TimeSpan s = chargeInterval / 10; //devide chargeinterval by 10
                                    int times = (int)t.TotalSeconds / (int)s.TotalSeconds; //get the number of times s devides into t
                                    charge = $"{state.BatteryStatus.Level * 10 + times}%"; //charge = state.BatteryStatus.Level * 10 + times
                                    chargepercent = (int)state.BatteryStatus.Level * 10 + times;
                                }
                                else {
                                    TimeSpan t = DateTime.Now - checkChange; //get the time between now and the last check
                                    TimeSpan s = drainInterval / 10; //devide draininterval by 10
                                    int times = (int)t.TotalSeconds / (int)s.TotalSeconds; //get the number of times s devides into t
                                    charge = $"{state.BatteryStatus.Level * 10 - times}%"; //charge = state.BatteryStatus.Level * 10 - times
                                    chargepercent = (int)state.BatteryStatus.Level * 10 - times;
                                }
                            }
                            if (oncharge) {
                                int remaining = 100 - chargepercent;
                                TimeSpan time = (chargeInterval / 10) * remaining; //time to charge = changeInterval / 10 * remaining
                                string[] stats = {
                                    $"{charge} Charging",
                                    $"{time.Hours}:{time.Minutes.ToString("00")}:{time.Seconds.ToString("00")} Remaining to full",
                                };
                                SetInfo(stats);
                            }
                            else {
                                TimeSpan time = (drainInterval / 10) * chargepercent; //time to drain = drainInterval / 10 * chargepercent
                                string[] stats = {
                                    $"{charge}",
                                    $"{time.Hours}:{time.Minutes.ToString("00")}:{time.Seconds.ToString("00")} Remaining"
                                };
                                SetInfo(stats);
                            }
                        }
                        else {
                            SetInfo(new string[] { "No Controller Found", "Please connect a controller" });
                            GetController();
                        }
                    }));
                }
            }
            catch { GetController(); }
        }
        private void DSChargeView_Enter(object sender, EventArgs e)
        { GetController(); }
        private void DSChargeView_Leave(object sender, EventArgs e)
        { if(controllerFound) { ds.EndPolling(); ds.Release(); } }
    }
}