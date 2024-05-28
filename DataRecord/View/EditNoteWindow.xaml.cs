using DataRecord.Model;
using DataRecord.ViewModel.Helpers;
using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DataRecord.View
{
	public partial class EditNoteWindow : Window
	{
		private Note note;
		private string appImagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

		public EditNoteWindow(Note note)
		{
			InitializeComponent();
			this.note = note;
			titleTextBox.Text = note.Title;
			descriptionTextBox.Text = note.Description;

			if (!string.IsNullOrEmpty(note.ImagePath))
			{
				string imagePath = System.IO.Path.Combine(appImagePath, note.ImagePath);
				if (File.Exists(imagePath))
				{
					noteImage.Source = new BitmapImage(new Uri(imagePath));
				}
			}
		}

		private void UploadButton_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";

			if (openFileDialog.ShowDialog() == true)
			{
				string selectedImagePath = openFileDialog.FileName;
				string fileName = System.IO.Path.GetFileName(selectedImagePath);

				// 用來儲存圖片的路徑
				if (!Directory.Exists(appImagePath))
				{
					Directory.CreateDirectory(appImagePath);
				}

				string destinationPath = System.IO.Path.Combine(appImagePath, fileName);
				File.Copy(selectedImagePath, destinationPath, true);

				note.ImagePath = destinationPath; // 使用完整路徑
				noteImage.Source = new BitmapImage(new Uri(destinationPath));
			}
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			note.Title = titleTextBox.Text;
			note.Description = descriptionTextBox.Text;
			DialogResult = true; // 確保父視窗能檢測到變更並更新集合
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to delete this note?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				DatabaseHelper.Delete(note);
				DialogResult = false;
			}
		}

		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.DragMove();
			}
		}
	}
}
