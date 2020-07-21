using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QVision.Frm;

namespace QVision
{
    public partial class MainFrm : Form
    {        
        public MainFrm()
        {
            InitializeComponent();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (this.ActiveMdiChild == Frames.videoFrm)
                {
                    Cursor.Current = Cursors.Arrow;
                    return;
                }
                Frames.videoFrm.MdiParent = this;
                Frames.videoFrm.Dock = DockStyle.Fill;
                Frames.videoFrm.Show();
                Frames.videoFrm.Activate();
                Cursor.Current = Cursors.Arrow;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (this.ActiveMdiChild == Frames.recipeFrm)
                {
                    Cursor.Current = Cursors.Arrow;
                    return;
                }
                Frames.recipeFrm.MdiParent = this;
                Frames.recipeFrm.Dock = DockStyle.Fill;
                Frames.recipeFrm.Show();
                Frames.recipeFrm.Activate();
                Cursor.Current = Cursors.Arrow;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            Frames.videoFrm = new VideoFrm();
            Frames.recipeFrm = new RecipeFrm();
            Frames.settingFrm = new SettingFrm();
            Frames.videoFrm.MdiParent = this;
            Frames.videoFrm.Dock = DockStyle.Fill;
            Frames.videoFrm.Show();
            Frames.videoFrm.Activate();
            this.WindowState = FormWindowState.Maximized;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (this.ActiveMdiChild == Frames.settingFrm)
                {
                    Cursor.Current = Cursors.Arrow;
                    return;
                }
                Frames.settingFrm.MdiParent = this;
                Frames.settingFrm.Dock = DockStyle.Fill;
                Frames.settingFrm.Show();
                Frames.settingFrm.Activate();
                Cursor.Current = Cursors.Arrow;
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Frm.AboutFrm aboutFrm = new Frm.AboutFrm())
            {
                aboutFrm.ShowDialog();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (MessageBox.Show("退出本系统?", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                Application.Exit();
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
