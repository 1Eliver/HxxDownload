using HxxServerDownloader.Models;
using HxxServerDownloader.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace HxxServerDownloader
{
    public partial class Form1 : Form
{
    public DataManager dataManager;
    private DownloadTool tool;
    private int taskSum = 1;
    private int[] taskProgresses;
    public static Form1 Instance { get; set; }

    public Form1()
    {
        try
        {
            InitializeComponent();
            Instance = this;
        }
        catch (Exception ex)
        {
            var er = new ErrorForm(ex.Message);
            er.Show();
            Close();
        }
    }

    private void Downloader_ProgressChanged(int taskIndex, int progress)
    {
        Task.Run(() =>
        {
            taskProgresses[taskIndex] = progress;
            int totalProgress = 0;
            foreach (var taskProgress in taskProgresses)
            {
                totalProgress += taskProgress;
            }

            progressBar1.Value = totalProgress / taskSum;
        });
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        InitUtil.InitServerFolder();
        dataManager = new DataManager();
        tool = new DownloadTool();
        load_servers();
    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {
        
    }

    private void progressBar1_Click(object sender, EventArgs e)
    {

    }

    protected override void OnClosing(CancelEventArgs e)
    {
        dataManager?.Save();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        var tasks = new List<Task>();
        if (textBox2.Text == "")
        {
            var er = new ErrorForm("请输入下载字符串！");
            er.Show();
            return;
        }
        try
        {
            var decode = DecodeUtil.Decode(textBox2.Text);
            var dauD = JsonConvert.DeserializeObject<DAUModels.DownloadAndUploadData>(decode);
            
            if (dauD.Download.Uid != "" && dauD.Download.Name != "" && dauD.Download.Url != "")
            {
                MessageBox.Show($"开始下载资源...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                taskSum = 1;
                taskProgresses = new int[taskSum];
                tasks.Add(tool.Download(dauD.Download.Url, DownloadResourse.Server, dauD.Download.Uid));
                dataManager.Data.Servers.Add(new DataModels.ServerItem
                {
                    Name = dauD.Download.Name,
                    Uid = dauD.Download.Uid
                });
            }
            else
            {
                if (dauD.Upload.Uid == "")
                {
                    MessageBox.Show("请检查输入的下载字符串是否正确！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (dauD.Upload.Mods.Url == "")
                {
                    MessageBox.Show($"开始下载资源 once...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    taskSum = 1;
                    taskProgresses = new int[taskSum];
                    tasks.Add(tool.Download(dauD.Upload.Configs.Url, DownloadResourse.Configs, dauD.Upload.Uid));
                }
                else if (dauD.Upload.Configs.Url == "")
                {
                    MessageBox.Show($"开始下载资源 once...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    taskSum = 1;
                    taskProgresses = new int[taskSum];
                    tasks.Add(tool.Download(dauD.Upload.Mods.Url, DownloadResourse.Mods, dauD.Upload.Uid));
                }
                else if (dauD.Upload.Mods.Url == "" && dauD.Upload.Configs.Url == "")
                {
                    MessageBox.Show("请检查输入的下载字符串是否正确！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"开始下载资源 twice...", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    taskSum = 2;
                    taskProgresses = new int[taskSum];
                    tasks.Add(tool.Download(dauD.Upload.Configs.Url, DownloadResourse.Configs, dauD.Upload.Uid));
                    tasks.Add(tool.Download(dauD.Upload.Mods.Url, DownloadResourse.Mods, dauD.Upload.Uid));
                }
            }

            for (int i = 0; i < tasks.Count; i++)
            {
                int taskIndex = i;
                tool.Downloader.ProgressChanged += (progress) => Downloader_ProgressChanged(taskIndex, progress);
            }

            Task.Run(() =>
            {
                try
                {
                    Task.WaitAll(tasks.ToArray());
                    progressBar1.Value = 0;
                    taskSum = 0;
                    MessageBox.Show("下载/更新完成！", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    load_servers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

        public void load_servers()
        {
            listBox1.Items.Clear();
            foreach (var server in dataManager.Data.Servers)
            {
                listBox1.Items.Add(server.Name);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("请先选择服务器！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var path = Path.Combine("./servers", dataManager.Data.Servers[listBox1.SelectedIndex].Uid);
            Process.Start(path);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("请先选择服务器！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var path = Path.Combine("./servers", dataManager.Data.Servers[listBox1.SelectedIndex].Uid, "PCL2.exe");
            Process.Start(path);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("请先选择服务器！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var path = Path.Combine("./servers", dataManager.Data.Servers[listBox1.SelectedIndex].Uid);
            Directory.Delete(path, true);
            dataManager.Data.Servers.Remove(dataManager.Data.Servers[listBox1.SelectedIndex]);
            load_servers();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                var add = new AddHadServerForm();
                add.Show();
            }));
        }
    }
}
