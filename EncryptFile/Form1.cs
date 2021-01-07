using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace EncryptFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //CommonDialog BrowserForFolder = new CommonDialog();
                FolderBrowserDialog BrowserForFolder = new FolderBrowserDialog();
                BrowserForFolder.ShowDialog();
                //BrowserForFolder.Filter = "Image Files|*.BMP;*.GIF;*.JPG;*.JPEG;*.PNG|All files (*.*)|*.*";
                textBox1.Text = BrowserForFolder.SelectedPath;
            } catch (Exception es)
            {
                Console.WriteLine(es);
            }
            
        }

        private void ShowWaitDialog(Action codeToRun)
        {
            ManualResetEvent dialogLoadedFlag = new ManualResetEvent(false);

            // open the dialog on a new thread so that the dialog window gets
            // drawn. otherwise our long running code will run and the dialog
            // window never renders.
            (new Thread(() =>
            {
                Form waitDialog = new Form()
                {
                    Name = "WaitForm",
                    Text = "Please Wait...",
                    ControlBox = false,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    Width = 240,
                    Height = 50,
                    Enabled = true
                };

                ProgressBar ScrollingBar = new ProgressBar()
                {
                    Style = ProgressBarStyle.Marquee,
                    Parent = waitDialog,
                    Dock = DockStyle.Fill,
                    Enabled = true
                };

                waitDialog.Load += new EventHandler((x, y) =>
                {
                    dialogLoadedFlag.Set();
                });

                waitDialog.Shown += new EventHandler((x, y) =>
                {
                    // note: if codeToRun function takes a while it will 
                    // block this dialog thread and the loading indicator won't draw
                    // so launch it too in a different thread
                    (new Thread(() =>
                    {
                        codeToRun();

                        // after that code completes, kill the wait dialog which will unblock 
                        // the main thread
                        this.Invoke((MethodInvoker)(() => waitDialog.Close()));
                    })).Start();
                });

                this.Invoke((MethodInvoker)(() => waitDialog.ShowDialog(this)));
            })).Start();

            while (dialogLoadedFlag.WaitOne(100, true) == false)
                Application.DoEvents(); // note: this will block the main thread once the wait dialog shows
        }

        private void cmdEncrypt_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(string.Concat(textBox1.Text, "\\Encrypt")))
                {
                    Directory.CreateDirectory(string.Concat(textBox1.Text, "\\Encrypt"));
                }
                ShowWaitDialog(enc);
                MessageBox.Show("Proses Encrypt PB Selesai", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            catch (Exception exception)
            {
                ProjectData.SetProjectError(exception);
                Exception ex = exception;
                MessageBox.Show(string.Concat("Proses Encrypt Error", ex.Message, ex.StackTrace, ex.Source), "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ProjectData.ClearProjectError();
            }
        }

        public void enc()
        {
            Encrypt_ALL(textBox1.Text, string.Concat(textBox1.Text, "\\Encrypt"));
        }

        public bool Encrypt_ALL(object cPath, object cHasil)
        {
            bool flag = false;
            //string[] files = Directory.GetFiles(Conversions.ToString(cPath));
            string[] files = Directory.GetFiles(Conversions.ToString(cPath), "*.CSV");
            if (checked((int)files.Length) > 0)
            {
                object now = DateAndTime.Now;
                int num3 = checked(checked((int)files.Length) - 1);
                for (int i = 0; i <= num3; i = checked(i + 1))
                {
                    string fileName = Path.GetFileName(files[i]);
                    if ((Operators.CompareString(Path.GetExtension(files[i]).ToUpper(), ".CSV", true) == 0))
                    {
                        object obj4 = Path.GetFileName(files[i]);
                        StreamReader reader = new StreamReader(string.Concat(cPath.ToString(), "\\", obj4.ToString()));
                        string str3 = "";
                       // this.lblStatus.Text = string.Concat("Encrypt file PB : ", fileName);
                        //Application.DoEvents();
                        ClassEncDec s = new ClassEncDec();
                        if (File.Exists(Conversions.ToString(Operators.AddObject(Operators.AddObject(cHasil, "\\"), obj4))))
                        {
                            File.Delete(Conversions.ToString(Operators.AddObject(Operators.AddObject(cHasil, "\\"), obj4)));
                        }
                        string[] strArray2 = reader.ReadToEnd().Trim().Split(new char[] { '\r' });
                        int num4 = checked(checked((int)strArray2.Length) - 1);
                        if (num4 > 0)
                        {
                            for (int j = 0; j <= num4; j = checked(j + 1))
                            {
                                str3 = s.Encrypt(strArray2[j].Trim(), "idm123");
                                StreamWriter writer2 = new StreamWriter(Conversions.ToString(Operators.AddObject(Operators.AddObject(cHasil, "\\"), obj4)), true);
                                writer2.WriteLine(str3);
                                writer2.Flush();
                                writer2.Close();
                            }
                        } 
                        
                    } else
                    {
                        Console.WriteLine("Ayeyeyeye");
                    }
                    //this.ToolStripProgressBar.Value = checked((int)Math.Round((double)i / (double)num3 * 100));
                   // this.lblprs.Text = string.Concat(Conversions.ToString(i), " of ", Conversions.ToString(num3), " file(s)");
                    //Application.DoEvents();
                }
                object right = DateAndTime.Now;
                if (File.Exists("Proses.log"))
                {
                    File.Delete("proses.log");
                }
                StreamWriter writer = new StreamWriter("Proses.log", true);
                writer.WriteLine("Jumlah Toko  : ", checked((int)files.Length), true);
                writer.Flush();
                writer.Close();
            }
            return flag;
        }

        private void cmdDecrypt_Click(object sender, EventArgs e)
        {
            try
            {
               // this.lblStatus.Text = "Decrypt File PB...";
                //Application.DoEvents();
                if (!Directory.Exists(string.Concat(textBox1.Text, "\\Decrypt")))
                {
                    Directory.CreateDirectory(string.Concat(textBox1.Text, "\\Decrypt"));
                }
                ShowWaitDialog(dec);
                //Application.DoEvents();
                MessageBox.Show("Proses Decrypt PB Selesai", "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            catch (Exception exception)
            {
                ProjectData.SetProjectError(exception);
                Exception ex = exception;
                MessageBox.Show(string.Concat("Proses Decrypt PB Selesai", ex.Message, ex.StackTrace, ex.Source), "Message", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ProjectData.ClearProjectError();
            }
        }

        public void dec()
        {
            Decrypt_ALL(textBox1.Text, string.Concat(textBox1.Text, "\\Decrypt"));
        }

        public bool Decrypt_ALL(object cPath, object cHasil)
        {
            bool flag = false;
            string[] files = Directory.GetFiles(Conversions.ToString(cPath), "*.CSV");
            if (checked((int)files.Length) > 0)
            {
                object now = DateAndTime.Now;
                int num3 = checked(checked((int)files.Length) - 1);
                for (int i = 0; i <= num3; i = checked(i + 1))
                {
                    Path.GetFileName(files[i]);
                    //if ((Operators.CompareString(Path.GetExtension(files[i]).ToUpper(), ".CSV", true) != 0))
                    //{
                    //Operators.CompareString()
                    object obj4 = Path.GetFileName(files[i]);
                    StreamReader reader = new StreamReader(string.Concat(cPath.ToString(), "\\", obj4.ToString()));
                    // this.lblStatus.Text = string.Concat("DeCrypt file PB : ", obj4.ToString());
                    //Application.DoEvents();
                    string str3 = "";
                    ClassEncDec s_ = new ClassEncDec();
                    if (File.Exists(Conversions.ToString(Operators.AddObject(Operators.AddObject(cHasil, "\\"), obj4))))
                    {
                        File.Delete(Conversions.ToString(Operators.AddObject(Operators.AddObject(cHasil, "\\"), obj4)));
                    }
                    string[] strArray2 = reader.ReadToEnd().Trim().Split(new char[] { '\r' });
                    int num4 = checked(checked((int)strArray2.Length) - 1);
                    if (num4 > 0)
                    {
                        for (int j = 0; j <= num4; j = checked(j + 1))
                        {
                            str3 = s_.Decrypt(strArray2[j].Trim(), "idm123");
                            StreamWriter writer2 = new StreamWriter(Conversions.ToString(Operators.AddObject(Operators.AddObject(cHasil, "\\"), obj4)), true);
                            writer2.WriteLine(str3);
                            writer2.Flush();
                            writer2.Close();
                        }
                    } else
                    {
                        //MessageBox.Show("Ksosososo");
                    }
                    
                    //} else
                    //{
                      //  Console.WriteLine("Ayeyeyeye");
                    //}
                   // this.ToolStripProgressBar.Value = checked((int)Math.Round((double)i / (double)num3 * 100));
                   // this.lblprs.Text = string.Concat(Conversions.ToString(i), " of ", Conversions.ToString(num3), " file(s)");
                    //Application.DoEvents();
                }
                object right = DateAndTime.Now;
                if (File.Exists("Proses.log"))
                {
                    File.Delete("proses.log");
                }
                StreamWriter writer = new StreamWriter("Proses.log", true);
                writer.WriteLine("Jumlah Toko  : ", checked((int)files.Length), true);
                writer.Flush();
                writer.Close();
            }
            return flag;
        }
    }
}
