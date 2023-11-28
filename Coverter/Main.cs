﻿using System;
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

		// Resim, video ve ses uzantıları için HashSet tanımlamaları
		private HashSet<string> imageExtensions = new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif", ".svg", ".eps" };
		private HashSet<string> videoExtensions = new HashSet<string> { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".3gp", ".m4v" };
		private HashSet<string> audioExtensions = new HashSet<string> { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a", ".midi", ".mid" };

		private List<string> draggedFiles = new List<string>();

		// Kullanıcının seçtiği formatları saklamak için bir sözlük
		private Dictionary<string, string> selectedFormats = new Dictionary<string, string>();


		public Main()
		{
			InitializeComponent();
			this.AllowDrop = true;
			this.DragEnter += new DragEventHandler(this.Main_DragEnter);
			this.DragDrop += new DragEventHandler(this.Main_DragDrop);
		}

		private void btnSelectAndConvert_Click(object sender, EventArgs e)
		{
			openFileDialog1.FileName = ""; // Dosya seçim diyalogunda dosya adı alanını boş bırak

			// Dosya seçimi
			if (openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				using (var folderDialog = new FolderBrowserDialog())
				{
					if (folderDialog.ShowDialog() == DialogResult.OK)
					{
						foreach (string sourcePath in openFileDialog1.FileNames)
						{
							string targetFolder = folderDialog.SelectedPath;
							ProcessFile(sourcePath);
						}
					}
				}
			}
		}


		private void ProcessFile(string sourcePath)
		{
			string sourceExtension = Path.GetExtension(sourcePath).ToLower();
			SetSaveDialogFilter(sourceExtension);

			saveFileDialog1.FileName = Path.GetFileNameWithoutExtension(sourcePath); // Önerilen dosya adı

			// Kaydetme diyalogunu aç
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
				string targetPath = saveFileDialog1.FileName;

				if (IsUnsupportedConversion(sourceExtension, Path.GetExtension(targetPath).ToLower()))
				{
					MessageBox.Show("Bu tür dönüşüm desteklenmiyor.");
				}
				else
				{
					// Desteklenen dönüşüm varsa, dönüşüm işlemini başlat
					ConvertFile(sourcePath, targetPath);
				}
			}

		}


		private void DisplayFilesInPanel(string[] files)
		{
			panel1.Controls.Clear(); // Paneldeki önceki kontrolleri temizle

			int yPos = 10;
			foreach (var file in files)
			{
				Label label = new Label
				{
					Text = Path.GetFileName(file),
					Location = new Point(10, yPos),
					AutoSize = true
				};

				panel1.Controls.Add(label);
				yPos += label.Height + 5; // Her bir etiket için y pozisyonunu güncelle
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
			draggedFiles.Clear(); // Listeyi temizle
			string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

			if (files != null && files.Length != 0)
			{
				draggedFiles.AddRange(files); // Dosya yollarını listeye ekle
				DisplayFilesInPanelWithComboBox(files);   // Panelde dosya isimlerini ve ComboBox'ları göster
			}
		}

		private void DisplayFilesInPanelWithComboBox(string[] files)
		{
			panel1.Controls.Clear(); // Paneldeki önceki kontrolleri temizle

			int yPos = 10;
			foreach (var file in files)
			{
				// Label oluşturma
				Label label = new Label
				{
					Text = Path.GetFileName(file),
					Location = new Point(10, yPos),
					AutoSize = true
				};

				// ComboBox oluşturma ve doldurma
				ComboBox comboBox = new ComboBox
				{
					Location = new Point(200, yPos),
					Width = 100,
					Name = "comboBox_" + file  // Her combobox için benzersiz bir isim atayın
				};
				comboBox.SelectedIndexChanged += new EventHandler(ComboBox_SelectedIndexChanged);
				FillComboBoxWithExtensions(comboBox, file);

				panel1.Controls.Add(label);
				panel1.Controls.Add(comboBox);
				yPos += label.Height + 5;
			}
		}

		private void ComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			ComboBox comboBox = sender as ComboBox;
			if (comboBox != null)
			{
				string file = comboBox.Name.Replace("comboBox_", "");
				string selectedFormat = comboBox.SelectedItem.ToString();
				selectedFormats[file] = selectedFormat;  // Seçilen formatı sözlüğe kaydet
			}
		}

		private void FillComboBoxWithExtensions(ComboBox comboBox, string file)
		{
			string extension = Path.GetExtension(file).ToLower();
			if (videoExtensions.Contains(extension))
			{
				comboBox.Items.AddRange(new string[] { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".3gp", ".m4v" });
			}

			else if (imageExtensions.Contains(extension))
			{
				comboBox.Items.AddRange(new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif" });
			}

			else if (audioExtensions.Contains(extension))
			{
				comboBox.Items.AddRange(new string[] { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a" });
			}

		}




		private void SetSaveDialogFilter(string sourceExtension)
		{
			if (videoExtensions.Contains(sourceExtension))
			{
				saveFileDialog1.Filter = "MP4 File (*.mp4)|*.mp4|" +
										 "AVI File (*.avi)|*.avi|" +
										 "MKV File (*.mkv)|*.mkv|" +
										 // Diğer video formatları...
										 "All Video Files|*.mp4;*.avi;*.mkv;...";
			}
			else if (imageExtensions.Contains(sourceExtension))
			{
				saveFileDialog1.Filter = "JPEG Image (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
										 "PNG Image (*.png)|*.png|" +
										 "GIF Image (*.gif)|*.gif|" +
										 // Diğer resim formatları...
										 "All Image Files|*.jpg;*.jpeg;*.png;*.gif;...";
			}
			else if (audioExtensions.Contains(sourceExtension))
			{
				saveFileDialog1.Filter = "MP3 Audio (*.mp3)|*.mp3|" +
										 "WAV Audio (*.wav)|*.wav|" +
										 "FLAC Audio (*.flac)|*.flac|" +
										 // Diğer ses formatları...
										 "All Audio Files|*.mp3;*.wav;*.flac;...";
			}
			else
			{
				saveFileDialog1.Filter = "All Files (*.*)|*.*";
			}
		}

		private void btnConvert_Click(object sender, EventArgs e)
		{
			using (var folderDialog = new FolderBrowserDialog())
			{
				if (folderDialog.ShowDialog() == DialogResult.OK)
				{
					string targetFolder = folderDialog.SelectedPath;
					foreach (var file in draggedFiles)
					{
						string targetFormat = selectedFormats[file];
						string originalFileName = Path.GetFileNameWithoutExtension(file);
						string newFileName = originalFileName + "(pop converter)";
						string targetPath = Path.Combine(targetFolder, newFileName + targetFormat);

						// Dosya ismi çakışması kontrolü
						int counter = 1;
						while (File.Exists(targetPath))
						{
							newFileName = originalFileName + "(pop converter)" + counter.ToString();
							targetPath = Path.Combine(targetFolder, newFileName + targetFormat);
							counter++;
						}

						// Dönüştürme işlemi
						ConvertFile(file, targetPath);
					}

					MessageBox.Show("Dönüştürme işlemi tamamlandı.");
					// Dosya listesini ve paneldeki görüntülemeyi temizle
					draggedFiles.Clear();
					panel1.Controls.Clear();
				}
			}
		}
	}
	
}



