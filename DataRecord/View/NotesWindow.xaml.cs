using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using DataRecord.Model;
using System.Windows.Input;
using DataRecord.ViewModel;
using DataRecord.ViewModel.Helpers;

namespace DataRecord.View
{
	public partial class NotesWindow : Window
	{
		NotesVM viewModel;
		private string appImagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

		public NotesWindow()
		{
			InitializeComponent();

			viewModel = Resources["vm"] as NotesVM;
			Application.Current.MainWindow = this;
			viewModel.SelectedNoteChanged += ViewModel_SelectedNoteChanged;
		}

		private void ViewModel_SelectedNoteChanged(object sender, EventArgs e)
		{
			// 待處理，但不處理也暫時沒影響
		}



		private void CreateNewNote_Click(object sender, RoutedEventArgs e)
		{
			if (viewModel.SelectedNotebook == null)
			{
				MessageBox.Show("Please select a notebook before creating a new note.", "No Notebook Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}

			var newNote = new Note
			{
				NotebookId = viewModel.SelectedNotebook.Id,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now
			};

			EditNoteWindow editNoteWindow = new EditNoteWindow(newNote);
			if (editNoteWindow.ShowDialog() == true)
			{
				DatabaseHelper.Insert(newNote);
				viewModel.Notes.Add(newNote);
			}
		}

		private void NotebookListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (viewModel.SelectedNotebook != null)
			{
				var originalNotebook = new Notebook
				{
					Id = viewModel.SelectedNotebook.Id,
					Name = viewModel.SelectedNotebook.Name,
					UserId = viewModel.SelectedNotebook.UserId
				};

				EditNotebookWindow editNotebookWindow = new EditNotebookWindow(viewModel.SelectedNotebook);
				if (editNotebookWindow.ShowDialog() == true)
				{
					DatabaseHelper.Update(viewModel.SelectedNotebook);
				}
				else if (editNotebookWindow.DialogResult == false)
				{
					DatabaseHelper.Delete(viewModel.SelectedNotebook);
					viewModel.Notebooks.Remove(viewModel.SelectedNotebook);
					viewModel.SelectedNotebook = null;
				}
				else
				{
					viewModel.SelectedNotebook.Name = originalNotebook.Name;
				}
			}
		}

		private void NoteListView_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (viewModel.SelectedNote != null)
			{
				var originalNote = new Note
				{
					Id = viewModel.SelectedNote.Id,
					NotebookId = viewModel.SelectedNote.NotebookId,
					Title = viewModel.SelectedNote.Title,
					Description = viewModel.SelectedNote.Description,
					ImagePath = viewModel.SelectedNote.ImagePath,
					CreatedAt = viewModel.SelectedNote.CreatedAt,
					UpdatedAt = viewModel.SelectedNote.UpdatedAt
				};

				EditNoteWindow editNoteWindow = new EditNoteWindow(viewModel.SelectedNote);
				if (editNoteWindow.ShowDialog() == true)
				{
					viewModel.SelectedNote.UpdatedAt = DateTime.Now;
					DatabaseHelper.Update(viewModel.SelectedNote);
				}
				else if (editNoteWindow.DialogResult == false)
				{
					DatabaseHelper.Delete(viewModel.SelectedNote);
					viewModel.Notes.Remove(viewModel.SelectedNote);
					viewModel.SelectedNote = null;
				}
				else
				{
					viewModel.SelectedNote.Title = originalNote.Title;
					viewModel.SelectedNote.Description = originalNote.Description;
					viewModel.SelectedNote.ImagePath = originalNote.ImagePath;
				}
			}
		}

		private void ListView_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{

		}


		private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
			{
				this.DragMove();
			}
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

	}
}
