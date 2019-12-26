using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace babytrigger
{
	public partial class Form1 : Form
	{
		private string trigFile = null;
		private int seconds = 0;
		private bool fileRunning = false;

		private ToolTip helperTip = new ToolTip();
		private int counter = 0;
		private Point mousePos = Cursor.Position;


		public Form1()
		{
			InitializeComponent();
			helperTip.SetToolTip(pictureBox1, "拖入一个文件");
			helperTip.SetToolTip(button1, "最小化并开始计时");
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			RegistryKey key = Registry.CurrentUser.CreateSubKey(
				@"Software\PeerSoft\BabyTrigger",
				RegistryKeyPermissionCheck.ReadWriteSubTree
			);

			numericUpDown1.Value = (int)key.GetValue("Seconds", 60);
			string filePath = (string)key.GetValue("FilePath", null);
			if (filePath != null)
			{
				SetFile(filePath);
			}

			bool StartMinimized = (int)key.GetValue("StartMinimized", 0) == 1;

			if (StartMinimized)
			{
				Program.StartMinimized = StartMinimized;
				启动时后台运行ToolStripMenuItem.Checked = Program.StartMinimized;
			}

			if (Program.StartMinimized)
			{
				button1_Click(this, null);
			}
		}

		private void SetFile(string filePath)
		{
			trigFile = filePath;
			pictureBox1.Image = Bitmap.FromHicon(Icon.ExtractAssociatedIcon(trigFile).Handle);
			helperTip.SetToolTip(pictureBox1, trigFile);
		}

		private void Form1_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.All;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void Form1_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (File.Exists(files[0]))
			{
				SetFile(files[0]);
			}
			else
			{
				MessageBox.Show("请拖入一个文件", "提示");
			}
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			// 未选择执行文件
			if (trigFile == null || seconds == 0) return;

			// 鼠标移动则重新计数，并且退出文件运行状态
			if (Math.Abs(Cursor.Position.Y - mousePos.Y) > 1)
			{
				fileRunning = false;
				counter = 0;
			}

			// 如果文件处于运行状态，则不计数
			if (fileRunning)
			{
				counter = 0;
			}

			mousePos = Cursor.Position;
			counter++;
			if (counter > seconds)
			{
				fileRunning = true;
				System.Diagnostics.Process.Start(trigFile);
			}

		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (trigFile == null)
			{
				MessageBox.Show("请先拖入一个文件", "提示");
				return;
			}

			seconds = (int)numericUpDown1.Value;

			RegistryKey key = Registry.CurrentUser.CreateSubKey(
				@"Software\PeerSoft\BabyTrigger",
				RegistryKeyPermissionCheck.ReadWriteSubTree
			);

			key.SetValue("Seconds", (int)numericUpDown1.Value);
			key.SetValue("FilePath", trigFile);

			Visible = false;
			ShowInTaskbar = false;
			notifyIcon1.Visible = true;
		}

		private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			seconds = 0;

			Visible = true;
			ShowInTaskbar = true;
			notifyIcon1.Visible = false;
		}

		private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void 显示ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			notifyIcon1_MouseDoubleClick(this, null);
		}

		private void 启动时后台运行ToolStripMenuItem_Click(object sender, EventArgs e)
		{
			RegistryKey key = Registry.CurrentUser.CreateSubKey(
				@"Software\PeerSoft\BabyTrigger",
				RegistryKeyPermissionCheck.ReadWriteSubTree
			);


			if (启动时后台运行ToolStripMenuItem.Checked)
			{
				key.SetValue("StartMinimized", 1);
			}
			else
			{
				key.SetValue("StartMinimized", 0);
			}
		}
	}
}
