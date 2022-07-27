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
        bool controllerFound, accurate = false, chargingOnTick = false;
        int lastcheck = 0;
        TimeSpan chargeInterval, drainInterval;
        DateTime checkChange;
        public DSChargeView() => InitializeComponent();
        private void OnLoad(object sender, EventArgs e)
        {
            SetInfo(new string[0]);
            GetDurations();
            //hide the main window
            
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
            
        }
        async void GetController()
        {
            try //get the first controller
            {
                ds = DualSense.EnumerateControllers().First();
                controllerFound = true;
                ds.Acquire(); ds.BeginPolling(5000);
                ds.OnStatePolled += Ds_OnStatePolled;
            }
            catch
            { 
                controllerFound = false; 
                SetInfo(new string[] { "No Controller Found", "Please connect a controller" });
                while (!controllerFound)
                {
                    await Task.Delay(5000);
                    GetController();
                }
            }
        }

        private void Ds_OnStatePolled(DualSense sender)
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
                            if (accurate)
                            {
                                if (chargingOnTick && oncharge)
                                {
                                    //chargeInterval = time betweenm checkchange and now
                                    chargeInterval = DateTime.Now - checkChange;
                                }
                                else
                                {
                                    drainInterval = DateTime.Now - checkChange;
                                }
                            }
                            lastcheck = (int)state.BatteryStatus.Level;
                            checkChange = DateTime.Now;
                            accurate = true;
                        }
                        string charge;
                        if (!accurate) { charge = $"~{state.BatteryStatus.Level * 10}"; }
                        else { charge = "A"; }
                        string[] stats =
                        {
                            charge,
                            $"Battery Charging: {state.BatteryStatus.IsCharging}",
                            $"Battery Charging Time: {chargeInterval}",
                            $"Battery Drain Time: {drainInterval}"
                        };
                        SetInfo(stats);

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