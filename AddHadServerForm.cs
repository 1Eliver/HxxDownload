using HxxServerDownloader.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static HxxServerDownloader.Utils.FilesUtils;

namespace HxxServerDownloader
{
    public partial class AddHadServerForm : Form
    {
        private ErrorProvider error = new ErrorProvider();
        private List<bool> hasError = new List<bool> { false, false, false };
        private FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        private FilesUtils.DirectoryMover mover = new FilesUtils.DirectoryMover();
        public AddHadServerForm()
        {
            InitializeComponent();
            addValidate();
            mover.ProgressChanged += (p) =>
            {
                Task.Run(() =>
                {
                    progressBar1.Value = p;
                });
            };
        }

        private void AddHadServerForm_Load(object sender, EventArgs e)
        {

        }

        private void addValidate()
        {
            textBox1.Validating += (sender, e) =>
            {
                var tb = sender as TextBox;
                if (tb.Text == "" && folderBrowserDialog.SelectedPath == "")
                {
                    hasError[0] = true;
                    error.SetError(tb, "路径不可为空");
                }
                else
                {
                    hasError[0] = false;
                    error.SetError(tb, "");
                }
            };
            textBox2.Validating += (sender, e) =>
            {
                var tb = sender as TextBox;
                if (tb.Text == "")
                {
                    hasError[1] = true;
                    error.SetError(tb, "Uid为必填项！");
                }
                else
                {
                    string uidWithoutHyphens = tb.Text.Replace("-", "");
                    if (uidWithoutHyphens.Length != 32)
                    {
                        hasError[1] = true;
                        error.SetError(tb, "请输入正确的Uid");
                    }
                    else
                    {
                        hasError[1] = false;
                        error.SetError(tb, "");
                    }
                }
            };
            textBox3.Validating += (sender, e) =>
            {
                var tb = sender as TextBox;
                if (tb.Text == "" && folderBrowserDialog.SelectedPath == "")
                {
                    hasError[2] = true;
                    error.SetError(tb, "服务器名不可为空");
                }
                else
                {
                    hasError[2] = false;
                    error.SetError(tb, "");
                }
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog.SelectedPath;
                if (hasError[0] == true)
                {
                    hasError[0] = false;
                    error.SetError(textBox1, "");
                }
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private bool hasErrorIn()
        {
            foreach (var e in hasError)
            {
                if (e)
                {
                    return true;
                }
            }
            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (hasErrorIn())
            {
                MessageBox.Show("请确认所有参数填写正确", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Task.Run(() =>
            {
                mover.MoveDirectory(textBox1.Text, PathUtils.GetServerPath(textBox2.Text));
                Form1.Instance.dataManager.Data.Servers.Add(new Models.DataModels.ServerItem
                {
                    Uid = textBox2.Text,
                    Name = textBox3.Text
                });
                Form1.Instance.load_servers();
                MessageBox.Show("移动完成！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            });
        }
    }
}
