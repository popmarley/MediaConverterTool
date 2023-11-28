using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace Coverter
{
	public partial class Main : Form
	{
		private string ffmpegPath = @"D:\ffmpeg-2023-11-27-git-0ea9e26636-full_build\bin\ffmpeg.exe"; // FFmpeg yolunu buraya girin veya bir ayar/config dosyasından alın
		public Main()
		{
			InitializeComponent();
		}

		private void btnSelectAndConvert_Click(object sender, EventArgs e)
		{
			// Dosya seçimi
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string sourcePath = openFileDialog1.FileName;

				// Kaydetme dialogunu aç
				if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					string targetPath = saveFileDialog1.FileName;
					// Burada dönüşüm işlemlerini gerçekleştirin
					ConvertFile(sourcePath, targetPath);
				}
			}
		}

		private void ConvertFile(string sourcePath, string targetPath)
		{
			try
			{
				string arguments = BuildFfmpegArguments(sourcePath, targetPath);

				var process = new Process
				{
					StartInfo = new ProcessStartInfo
					{
						FileName = ffmpegPath,
						Arguments = arguments,
						UseShellExecute = false,
						CreateNoWindow = true,
						RedirectStandardOutput = true,
						RedirectStandardError = true
					}
				};

				process.Start();
				string output = process.StandardOutput.ReadToEnd();
				string error = process.StandardError.ReadToEnd();
				process.WaitForExit();

				if (process.ExitCode != 0)
				{
					MessageBox.Show("Dönüştürme sırasında hata oluştu: " + error);
				}
				else
				{
					MessageBox.Show("Dönüştürme başarıyla tamamlandı.");
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Bir hata oluştu: " + ex.Message);
			}
		}



		private string BuildFfmpegArguments(string sourcePath, string targetPath)
		{
			string extension = Path.GetExtension(sourcePath).ToLower();
			switch (extension)
			{
				case ".mp4":
				case ".avi":
				case ".mkv":
				case ".mov":
				case ".wmv":
				case ".flv":
				case ".webm":
				case ".3gp":
				case ".m4v":
					return $"-i \"{sourcePath}\" \"{targetPath}\""; // Video dönüştürme için özelleştirilmiş komutlar eklenebilir
				case ".jpg":
				case ".jpeg":
				case ".png":
				case ".gif":
				case ".bmp":
				case ".tiff":
				case ".tif":
				case ".svg":
				case ".eps":
					return $"-i \"{sourcePath}\" \"{targetPath}\""; // Resim dönüştürme için özelleştirilmiş komutlar eklenebilir
																	// Diğer dosya türleri için ek komutlar eklenebilir
				default:
					throw new InvalidOperationException("Desteklenmeyen dosya türü.");
			}
		}
	}
}



