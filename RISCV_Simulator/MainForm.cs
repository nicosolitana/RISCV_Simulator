using RISCV_Simulator.Controller;
using RISCV_Simulator.Data;
using RISCV_Simulator.Validator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace RISCV_Simulator
{
    public partial class MainForm : Form
    {
        public List<Register> regList = new List<Register>();
        public DataTable registerTbl = new DataTable();
        public DataTable textSegment = new DataTable();

        public MainForm()
        {
            InitializeComponent();
            PopulateRegister();
        }

        #region UI Related
        //*************************************************************************************
        // THE FOLLOWING SOURCE CODES ARE UI RELATED EVENTS
        //*************************************************************************************

        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr one, int two, int three, int four);
        private void WinMouseDownEvt(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void ExitMouseClickEvt(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        private void MaximizeMouseClickEvt(object sender, MouseEventArgs e)
        {
            if (WindowState != FormWindowState.Maximized)
            {
                var workingArea = Screen.FromHandle(Handle).WorkingArea;
                MaximizedBounds = new Rectangle(0, 0, workingArea.Width, workingArea.Height);
                WindowState = FormWindowState.Maximized;
            }
            else
                WindowState = FormWindowState.Normal;
        }

        private void MinimizedMouseClickEvt(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void saveBtnMouseHoverEvt(object sender, System.EventArgs e)
        {
            saveBtn.BackColor = ColorTranslator.FromHtml("#8EBC00");
            saveBtn.Image = Properties.Resources.b_save;
        }

        private void saveBtnMouseLeaveEvt(object sender, System.EventArgs e)
        {
            saveBtn.BackColor = ColorTranslator.FromHtml("#2D2D30");
            saveBtn.Image = Properties.Resources.save;
        }

        private void singleStepBtnMouseHoverEvt(object sender, System.EventArgs e)
        {
            singleStepBtn.BackColor = ColorTranslator.FromHtml("#8EBC00");
            singleStepBtn.Image = Properties.Resources.b_sstep;
        }

        private void singleStepBtnMouseLeaveEvt(object sender, System.EventArgs e)
        {
            singleStepBtn.BackColor = ColorTranslator.FromHtml("#2D2D30");
            singleStepBtn.Image = Properties.Resources.sstep;
        }

        private void fullExecBtnMouseHoverEvt(object sender, System.EventArgs e)
        {
            fullExecBtn.BackColor = ColorTranslator.FromHtml("#8EBC00");
            fullExecBtn.Image = Properties.Resources.b_full_exec;
        }

        private void fullExecBtnMouseLeaveEvt(object sender, System.EventArgs e)
        {
            fullExecBtn.BackColor = ColorTranslator.FromHtml("#2D2D30");
            fullExecBtn.Image = Properties.Resources.full_exec;
        }

        private void buildBtnMouseHoverEvt(object sender, System.EventArgs e)
        {
            buildBtn.BackColor = ColorTranslator.FromHtml("#8EBC00");
            buildBtn.Image = Properties.Resources.b_build;
        }

        private void buildBtnMouseLeaveEvt(object sender, System.EventArgs e)
        {
            buildBtn.BackColor = ColorTranslator.FromHtml("#2D2D30");
            buildBtn.Image = Properties.Resources.build;
        }

        private void lruBtnMouseClickEvt(object sender, System.EventArgs e)
        {
            lruBtn.BackColor = ColorTranslator.FromHtml("#8EBC00");
            mruBtn.BackColor = ColorTranslator.FromHtml("#58585A");
        }

        private void mruBtnMouseClickEvt(object sender, System.EventArgs e)
        {
            lruBtn.BackColor = ColorTranslator.FromHtml("#58585A");
            mruBtn.BackColor = ColorTranslator.FromHtml("#8EBC00");
        }

        private void ExitMouseHoverEvt(object sender, System.EventArgs e)
        {
            extBtn.BackColor = Color.Red;
        }

        private void ExitMouseLeaveEvt(object sender, System.EventArgs e)
        {
            extBtn.BackColor = ColorTranslator.FromHtml("#323233");
        }

        private void blkSizeTxtEnterEvt(object sender, System.EventArgs e)
        {
            if (blkSizeTxt.Text == "Enter Block Size")
            {
                blkSizeTxt.Text = "";
            }
        }

        private void blkSizeTxtLeaveEvt(object sender, System.EventArgs e)
        {
            if (blkSizeTxt.Text == "")
            {
                blkSizeTxt.Text = "Enter Block Size";
                blkSizeTxt.ForeColor = Color.LightGray;
            }
        }

        private void cacheSizeTxtEnterEvt(object sender, System.EventArgs e)
        {
            if (cacheSizeTxt.Text == "Enter Cache Size")
            {
                cacheSizeTxt.Text = "";
            }
        }

        private void cacheSizeTxtLeaveEvt(object sender, System.EventArgs e)
        {
            if (cacheSizeTxt.Text == "")
            {
                cacheSizeTxt.Text = "Enter Cache Size";
                cacheSizeTxt.ForeColor = Color.Silver;
            }
        }

        private void searchBtnMouseHoverEvt(object sender, System.EventArgs e)
        {
            searchFileBtn.BackColor = ColorTranslator.FromHtml("#8EBC00");
            searchFileBtn.Image = Properties.Resources.b_search;
        }

        private void searchBtnMouseLeaveEvt(object sender, System.EventArgs e)
        {
            searchFileBtn.BackColor = ColorTranslator.FromHtml("#58585A");
            searchFileBtn.Image = Properties.Resources.search;
        }
        #endregion

        #region Functionality Related
        //*************************************************************************************
        // THE FOLLOWING SOURCE CODES ARE UI RELATED EVENTS
        //*************************************************************************************
        private void PopulateRegister()
        {
            registerTbl.Columns.Add("Name", typeof(string));
            registerTbl.Columns.Add("Value", typeof(string));

            for (int i = 1; i < 32; i++)
            {
                registerTbl.Rows.Add("x" + i.ToString(), "0x00000000");
            }
            registerDataGrid.DataSource = registerTbl;
            registerDataGrid.Columns[0].ReadOnly = true;

        }
        private void searchFileBtnClickEvt(object sender, System.EventArgs e)
        {
            OpenFileDialog opDialogBox = new OpenFileDialog
            {
                InitialDirectory = @"C:\",
                Title = "Browse RISC-V Files",

                CheckFileExists = true,
                CheckPathExists = true,

                DefaultExt = "asm",
                Filter = "asm files (*.asm)|*.asm",
                FilterIndex = 2,
                RestoreDirectory = true,

                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (opDialogBox.ShowDialog() == DialogResult.OK)
            {
                filePathTxt.Text = opDialogBox.FileName;
                filePathTxt.Focus();
                filePathTxt.SelectionStart = filePathTxt.Text.Length;
            } else
            {
                filePathTxt.Text = "Select *.asm file.";
            }
        }

        private void BuildClickEvt(object sender, System.EventArgs e)
        {
            if(filePathTxt.Text != "Select *.asm file.")
            {
                List<string> code = FileController.ReadFile(filePathTxt.Text);
                List<string> text = new List<string>();
                bool isTextPart = false;
                foreach (string c in code)
                {
                    if (isTextPart)
                        text.Add(c);

                    if (c == ".text")
                        isTextPart = true;
                }

                List<string> OpCode = OpCodeController.GetOpCode(text);

                textSegment.Columns.Add("Address", typeof(string));
                textSegment.Columns.Add("Opcode", typeof(string));
                textSegment.Columns.Add("Source", typeof(string));

                string hx = "1000";
                for (int i = 0; i < text.Count; i++)
                {
                    string addr = "0x0000" + hx;
                    textSegment.Rows.Add(addr, OpCode[i], text[i]);
                    int decval = Convert.ToInt32(hx, 16) + 4;
                    hx = Convert.ToString(decval, 16).ToUpper();
                }
                textSegmentDataGrid.DataSource = textSegment;
                textSegmentDataGrid.ReadOnly = true;
            }
            else
            {
                errorLogTxt.Text = errorLogTxt.Text 
                    + "ERROR: No *.asm file has been selected." + "\r\n";
                errorLogTxt.SelectionStart = errorLogTxt.Text.Length;
                errorLogTxt.ScrollToCaret();
            }

        }


        #endregion
    }
}
