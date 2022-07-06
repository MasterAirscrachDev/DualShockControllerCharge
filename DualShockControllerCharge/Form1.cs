using System;
using DualSenseAPI;
using DualSenseAPI.State;
using static System.Windows.Forms.AxHost;
using System.Linq;
using System.Reactive.Linq;

namespace DualShockControllerCharge
{
    public partial class DSChargeView : Form
    {
        DualSense ds;
        DualSenseInputState state;
        bool controllerFound;
        int estimate = 0, lastcheck = 0, subtick = 0;
        int maxtick = 30; //edit this if estimate is more or less than you think its supposed to be
        public DSChargeView()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                ds = DualSense.EnumerateControllers().First();
                controllerFound = true;
                ds.Acquire();
                ds.BeginPolling(10);
            }
            catch
            { controllerFound = false; }
        }

        private void timer1_Tick(object from, EventArgs e)
        {
            //called every 2 seconds
            //math says about 10% every 60 mins
            subtick++;
            
            
            if(controllerFound)
            {
                state = ds.InputState;
                bool oncharge = state.BatteryStatus.IsCharging;
                if (oncharge)
                {
                    TextBox.Text = $"~{state.BatteryStatus.Level * 10}%, No Estimate";
                    estimate = (int)state.BatteryStatus.Level * 10;
                }
                else
                {
                    if (subtick >= 30) { estimate--; subtick = 0; }
                    if (lastcheck != state.BatteryStatus.Level)
                    {
                        estimate = (int)(state.BatteryStatus.Level * 10);
                        lastcheck = (int)state.BatteryStatus.Level;
                    }

                    TextBox.Text = $"~{state.BatteryStatus.Level * 10}%, Estimate {estimate}% {subtick}";
                }
                
                //if charging then show charging icon
                ChargeIcon.Visible = oncharge;
            }
            else
            { TextBox.Text = "DualSense: Not connected"; }
        }

        private void DSChargeView_Enter(object sender, EventArgs e)
        {
            try
            {
                ds = DualSense.EnumerateControllers().First();
                controllerFound = true;
                ds.Acquire();
                ds.BeginPolling(10);
            }
            catch
            { controllerFound = false; }

        }

        private void DSChargeView_Leave(object sender, EventArgs e)
        {
            if(controllerFound)
            {
                ds.EndPolling();
                ds.Release();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ds = DualSense.EnumerateControllers().First();
                controllerFound = true;
                ds.Acquire();
                ds.BeginPolling(10);
            }
            catch
            { controllerFound = false; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (controllerFound)
            {
                ds.EndPolling();
                ds.Release();
            }
        }
    }
}