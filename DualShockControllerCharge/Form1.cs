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
        int maxtick = 8; //edit this if estimate is more or less than you think its supposed to be
        public DSChargeView()
        {
            InitializeComponent();
        }
        void GetController()
        {
            try //get the first controller
            {
                ds = DualSense.EnumerateControllers().First();
                controllerFound = true;
                ds.Acquire();
                ds.BeginPolling(10);
            }
            catch
            { controllerFound = false; }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GetController();
        }

        private void timer1_Tick(object from, EventArgs e)
        {
            //called every 2 seconds
            //math says about 10% every 60 mins
            subtick++; 
            
            if(controllerFound)
            {
                state = ds.InputState; //get the state of input
                bool oncharge = state.BatteryStatus.IsCharging; //are we charging?
                if (oncharge)
                {
                    //if we are charging show the base value
                    TextBox.Text = $"~{state.BatteryStatus.Level * 10}%, No Estimate";
                    estimate = (int)state.BatteryStatus.Level * 10;
                }
                else
                {
                    //not charging so do estimation math
                    if (subtick >= maxtick) { estimate--; subtick = 0; } //if its been a minute (assuming 30 maxtick / 1% per min) reduce the estimate
                    if (lastcheck != state.BatteryStatus.Level)
                    {
                        estimate = (int)(state.BatteryStatus.Level * 10);
                        lastcheck = (int)state.BatteryStatus.Level;
                    }

                    TextBox.Text = $"~{state.BatteryStatus.Level * 10}%, Estimate {estimate}%";
                }
                
                //if charging then show charging icon
                ChargeIcon.Visible = oncharge;
            }
            else
            { TextBox.Text = "DualSense: Not connected"; }
        }

        private void DSChargeView_Enter(object sender, EventArgs e)
        {
            GetController();            
        }

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