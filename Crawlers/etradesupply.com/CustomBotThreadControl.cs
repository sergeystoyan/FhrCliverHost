//********************************************************************************************
//Author: Sergey Stoyan
//        sergey.stoyan@gmail.com
//        sergey_stoyan@yahoo.com
//        http://www.cliversoft.com
//        26 September 2006
//Copyright: (C) 2006, Sergey Stoyan
//********************************************************************************************
using System;
using System.Threading;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Cliver.Bot;

namespace Cliver.BotCustomization
{
    public partial class CustomBotThreadControl : BotGui.BotThreadControl
    {
        public CustomBotThreadControl(int id)
            : base(id)
        {
            InitializeComponent();
        }

        public WebRoutine WR
        {
            set
            {
                this.wr = value;
                if (wr == null)
                    return;
                wr.OnLoading += WebRoutine_Loading;
                wr.OnProgress += WebRoutine_Progress;
            }
        }
        WebRoutine wr;

        void WebRoutine_Progress(int max, int point)
        {
            this.Invoke(() =>
            {
                if (point < 0)
                {
                    progressBar.Maximum = 1;
                    progressBar.Value = 0;
                    return;
                }
                if (max <= 0)
                    progressBar.Maximum = point * 2;
                else if (max < point)
                    progressBar.Maximum = point;
                else
                    progressBar.Maximum = max;
                progressBar.Value = point;
            }
                );
        }

        void WebRoutine_Loading(string m)
        {
            labelState.SetText(m);
        }
    }
}