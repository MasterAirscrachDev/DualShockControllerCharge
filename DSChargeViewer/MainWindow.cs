using System.IO;
using DualSenseAPI;
using DualSenseAPI.State;
using System.Reactive.Linq;

namespace DualShockControllerCharge
{
    public partial class DSChargeView : Form
    {
        // this is stupid re-write
        DualSense ds;
        DualSenseInputState state;
        bool controllerFound, accurate = false, startCheck = false, chargingOnTick = false;
        int lastcheck = 0;
        TimeSpan chargeInterval, drainInterval;
        DateTime checkChange;
        public DSChargeView() => InitializeComponent();
        private void OnLoad(object sender, EventArgs e)
        {
            SetInfo(new string[0]);
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
                //round drain interval to the nearest second
                drainInterval = new TimeSpan(drainInterval.Hours, drainInterval.Minutes, drainInterval.Seconds);
                //round charge interval to the nearest second
                chargeInterval = new TimeSpan(chargeInterval.Hours, chargeInterval.Minutes, chargeInterval.Seconds);

                
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
            try
            {
                //open stats.mem for writing
                StreamWriter sw = new StreamWriter($"{Directory.GetCurrentDirectory()}\\stats.mem");
                sw.WriteLine(drainInterval.TotalSeconds.ToString());
                sw.WriteLine(chargeInterval.TotalSeconds.ToString());
                sw.Close();
            }
            catch
            {

            }
        }
        async void GetController()
        {
            try //get the first controller
            {
                ds = DualSense.EnumerateControllers().First();
                controllerFound = true;
                ds.Acquire(); ds.BeginPolling(10000);
                ds.OnStatePolled += Ds_OnStatePolled;
            }
            catch
            { 
                controllerFound = false; 
                SetInfo(new string[] { "No Controller Found", "Please connect a controller" });
                while (!controllerFound)
                {
                    await Task.Delay(10000);
                    GetController();
                }
            }
        }

        private void Ds_OnStatePolled(DualSense sender)
        {
            try
            {
                if (InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(delegate
                    {
                        //called every 5 seconds

                        if (controllerFound)
                        {
                            state = ds.InputState; //get the state of input
                            bool oncharge = state.BatteryStatus.IsCharging; //are we charging?
                            if ((int)state.BatteryStatus.Level != lastcheck)
                            {
                                if (!startCheck)
                                { startCheck = true; lastcheck = (int)state.BatteryStatus.Level; }
                                else
                                {
                                    if (accurate)
                                    {
                                        if (chargingOnTick && oncharge)
                                        {
                                            chargeInterval = DateTime.Now - checkChange;
                                            SetDurations();
                                        }
                                        else if (oncharge)
                                        { chargingOnTick = true; }
                                        else
                                        {
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
                            if (!accurate) { charge = $"~{state.BatteryStatus.Level * 10}%"; chargepercent = (int)state.BatteryStatus.Level * 10; }
                            else {
                                if (oncharge)
                                {
                                    //get the time between now and the last check
                                    TimeSpan t = DateTime.Now - checkChange;
                                    //devide chargeinterval by 10
                                    TimeSpan s = chargeInterval / 10;
                                    //get the number of times s devides into t
                                    int times = (int)t.TotalSeconds / (int)s.TotalSeconds;
                                    //charge = state.BatteryStatus.Level * 10 + times
                                    charge = $"{state.BatteryStatus.Level * 10 + times}%";
                                    chargepercent = (int)state.BatteryStatus.Level * 10 + times;
                                }
                                else
                                {
                                    //get the time between now and the last check
                                    TimeSpan t = DateTime.Now - checkChange;
                                    //devide draininterval by 10
                                    TimeSpan s = drainInterval / 10;
                                    //get the number of times s devides into t
                                    int times = (int)t.TotalSeconds / (int)s.TotalSeconds;
                                    //charge = state.BatteryStatus.Level * 10 - times
                                    charge = $"{state.BatteryStatus.Level * 10 - times}%";
                                    chargepercent = (int)state.BatteryStatus.Level * 10 - times;
                                }
                                //charge = "A"; 
                            }
                            if (oncharge)
                            {
                                int remaining = 100 - chargepercent;
                                //time to charge = changeInterval / 10 * remaining
                                TimeSpan time = chargeInterval / 10 * remaining;
                                string[] stats =
                                {
                                    $"{charge} Charging",
                                    $"{time.Hours}:{time.Minutes}:{time.Seconds} Remaining to full",
                                };
                                SetInfo(stats);
                            }
                            else{
                                //time to drain = changeInterval / 10 * chargepercent
                                TimeSpan time = drainInterval / 10 * chargepercent;
                                string[] stats =
                                {
                                    $"{charge}",
                                    $"{time.Hours}:{time.Minutes}:{time.Seconds} Remaining",

                                };
                                SetInfo(stats);
                            }
                            //string[] stats =
                            //{
                            //    charge,
                            //    $"Battery Charging: {state.BatteryStatus.IsCharging}",
                            //    $"Battery Charging Time: {chargeInterval.Minutes}:{chargeInterval.Seconds}",
                            //    $"Battery Drain Time: {drainInterval.Hours}:{drainInterval.Minutes}:{drainInterval.Seconds}",
                            //    "---------------DEBUG",
                            //    $"Accurate? {accurate}",
                            //    $"FirstTick? {startCheck}",
                            //    $"chanrge ~{state.BatteryStatus.Level * 10}%",
                            //    $"LastTick {lastcheck}"
                            //};
                            //SetInfo(stats);
                        }
                        else
                        {
                            SetInfo(new string[] { "No Controller Found", "Please connect a controller" });
                            GetController();
                        }
                    }));

                }
                else
                {
                    // Your code here, like set text box content or get text box contents etc..

                    // SAME CODE AS ABOVE
                }
            }
            catch
            {
                GetController();
            }
            
        }
        private void DSChargeView_Enter(object sender, EventArgs e)
        { GetController(); }

        private void DSChargeView_Leave(object sender, EventArgs e)
        {
            if(controllerFound)
            {
                ds.EndPolling();
                ds.Release();
            }
        }
    }
}