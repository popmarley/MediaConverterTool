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
			this.AllowDrop = true;
			this.DragEnter += new DragEventHandler(this.Main_DragEnter);
			this.DragDrop += new DragEventHandler(this.Main_DragDrop);
		}

		private void btnSelectAndConvert_Click(object sender, EventArgs e)
		{
			while (true) // Sonsuz döngüye giriyoruz
			{
				openFileDialog1.FileName = ""; // Dosya adı alanını boş bırak
				
				// Dosya seçimi
				if (openFileDialog1.ShowDialog() == DialogResult.OK)
				{
					string sourcePath = openFileDialog1.FileName;

					saveFileDialog1.Filter = "Video Files (*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.flv;*.webm;*.3gp;*.m4v)|*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.flv;*.webm;*.3gp;*.m4v|" +
								 "Image Files (*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff;*.tif;*.svg;*.eps)|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff;*.tif;*.svg;*.eps|" +
								 "Audio Files (*.mp3;*.wav;*.flac;*.aac;*.ogg;*.wma;*.m4a;*.midi;*.mid)|*.mp3;*.wav;*.flac;*.aac;*.ogg;*.wma;*.m4a;*.midi;*.mid|" +
								 "All Files (*.*)|*.*";

					

					// Kaydetme diyalogunu aç
					if (saveFileDialog1.ShowDialog() == DialogResult.OK)
					{
						string targetPath = saveFileDialog1.FileName;

						saveFileDialog1.FileName = ""; // Dosya adı alanını boş bırak
													  
						// Desteklenmeyen dönüşüm kontrolü
						if (IsUnsupportedConversion(Path.GetExtension(sourcePath).ToLower(), Path.GetExtension(targetPath).ToLower()))
						{
							MessageBox.Show("Bu tür dönüşüm desteklenmiyor.");
							continue; // Kullanıcıya tekrar deneme şansı ver
						}

						// Desteklenen dönüşüm varsa, dönüşüm işlemini başlat ve döngüden çık
						ConvertFile(sourcePath, targetPath);
						break;
					}
					else
					{
						// Kullanıcı 'Kaydet' diyalogunda iptal butonuna basarsa döngüden çık
						break;
					}
				}
				else
				{
					// Kullanıcı 'Aç' diyalogunda iptal butonuna basarsa döngüden çık
					break;
				}
			}
		}

		private void ConvertFile(string sourcePath, string targetPath)
		{
			// Kaynak ve hedef dosya uzantılarını kontrol et
			string sourceExtension = Path.GetExtension(sourcePath).ToLower();
			string targetExtension = Path.GetExtension(targetPath).ToLower();

			// Desteklenmeyen dönüşüm kontrolü
			if (IsUnsupportedConversion(sourceExtension, targetExtension))
			{
				MessageBox.Show("Bu tür dönüşüm desteklenmiyor.");
				return;
			}

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
				//video dosyaları
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

				//resim dosyaları
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
				
				//ses dosyaları
				case ".mp3":
				case ".wav":
				case ".flac":
				case ".aac":
				case ".ogg":
				case ".wma":
				case ".m4a":
				case ".midi":
				case ".mid":
					return $"-i \"{sourcePath}\" \"{targetPath}\"";

				default:
					throw new InvalidOperationException("Desteklenmeyen dosya türü.");
			}
		}

		private bool IsUnsupportedConversion(string sourceExt, string targetExt)
		{
			// Resim uzantıları
			var imageExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif", ".svg", ".eps" };

			// Video uzantıları
			var videoExtensions = new HashSet<string> { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".3gp", ".m4v" };

			// Ses uzantıları
			var audioExtensions = new HashSet<string> { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a", ".midi", ".mid" };

			// Resimden videoya veya sese dönüşüm kontrolü
			if (imageExtensions.Contains(sourceExt) && (videoExtensions.Contains(targetExt) || audioExtensions.Contains(targetExt)))
			{
				return true;
			}

			// Videodan resime dönüşüm kontrolü
			if (videoExtensions.Contains(sourceExt) && imageExtensions.Contains(targetExt))
			{
				return true;
			}

			// Burada diğer desteklenmeyen dönüşümler için de kontrol ekleyebilirsiniz

			return false;
		}

		private void Main_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}

		private void Main_DragDrop(object sender, DragEventArgs e)
		{
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (files != null && files.Length != 0)
			{
				string sourcePath = files[0]; // İlk dosyanın yolunu al
				ProcessFile(sourcePath);
			}
		}

		private void ProcessFile(string sourcePath)
		{
			while (true)
			{
				saveFileDialog1.Filter = "Video Files (*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.flv;*.webm;*.3gp;*.m4v)|*.mp4;*.avi;*.mkv;*.mov;*.wmv;*.flv;*.webm;*.3gp;*.m4v|" +
										 "Image Files (*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff;*.tif;*.svg;*.eps)|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.tiff;*.tif;*.svg;*.eps|" +
										 "Audio Files (*.mp3;*.wav;*.flac;*.aac;*.ogg;*.wma;*.m4a;*.midi;*.mid)|*.mp3;*.wav;*.flac;*.aac;*.ogg;*.wma;*.m4a;*.midi;*.mid|" +
										 "All Files (*.*)|*.*";

				saveFileDialog1.FileName = ""; // Dosya adı alanını boş bırak

				// Kaydetme diyalogunu aç
				if (saveFileDialog1.ShowDialog() == DialogResult.OK)
				{
					string targetPath = saveFileDialog1.FileName;

					// Desteklenmeyen dönüşüm kontrolü
					if (IsUnsupportedConversion(Path.GetExtension(sourcePath).ToLower(), Path.GetExtension(targetPath).ToLower()))
					{
						MessageBox.Show("Bu tür dönüşüm desteklenmiyor.");
						continue; // Kullanıcıya tekrar deneme şansı ver
					}

					// Desteklenen dönüşüm varsa, dönüşüm işlemini başlat ve döngüden çık
					ConvertFile(sourcePath, targetPath);
					break;
				}
				else
				{
					// Kullanıcı kaydetme diyalogunda iptal butonuna basarsa döngüden çık
					break;
				}
			}
		}

	}
}



