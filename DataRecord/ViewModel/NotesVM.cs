using DataRecord.Model;
using DataRecord.ViewModel.Helpers;
using DataRecord.View;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DataRecord.ViewModel
{
	public class NotesVM : BaseVM
	{
		public ObservableCollection<Notebook> Notebooks { get; set; }
		public ObservableCollection<Note> Notes { get; set; }

		private Notebook selectedNotebook;
		public Notebook SelectedNotebook
		{
			get { return selectedNotebook; }
			set
			{
				selectedNotebook = value;
				OnPropertyChanged();
				GetNotes();
			}
		}

		private Note selectedNote;
		public Note SelectedNote
		{
			get { return selectedNote; }
			set
			{
				selectedNote = value;
				OnPropertyChanged();
				SelectedNoteChanged?.Invoke(this, new EventArgs());
			}
		}

		private Visibility isVisible;
		public Visibility IsVisible
		{
			get { return isVisible; }
			set
			{
				isVisible = value;
				OnPropertyChanged();
			}
		}

		public ICommand NewNotebookCommand { get; set; }
		public ICommand NewNoteCommand { get; set; }
		public ICommand EditNoteCommand { get; set; }
		public ICommand EditNotebookCommand { get; set; }
		public ICommand EndEditingCommand { get; set; }

		public event EventHandler SelectedNoteChanged;

		public NotesVM()
		{
			NewNoteCommand = new RelayCommand(CreateNewNote);
			NewNotebookCommand = new RelayCommand(CreateNotebook);
			EditNoteCommand = new RelayCommand(StartEditingNote);
			EditNotebookCommand = new RelayCommand(StartEditingNotebook);
			EndEditingCommand = new RelayCommand(StopEditing);

			Notebooks = new ObservableCollection<Notebook>();
			Notes = new ObservableCollection<Note>();

			IsVisible = Visibility.Collapsed;

			GetNotebooks();
			// 確保至少有一個筆記本
			if (Notebooks.Any())
			{
				SelectedNotebook = Notebooks.First();
			}
		}

		public void CreateNotebook(object parameter)
		{
			Notebook newNotebook = new Notebook
			{
				Name = "New Notebook"
			};

			DatabaseHelper.Insert(newNotebook);
			GetNotebooks();
		}

		public void CreateNewNote(object parameter)
		{
			if (SelectedNotebook == null) return;

			Note newNote = new Note
			{
				NotebookId = SelectedNotebook.Id,
				CreatedAt = DateTime.Now,
				UpdatedAt = DateTime.Now,
				Title = "",
				Description = "",
				ImagePath = ""
			};

			var editNoteWindow = new EditNoteWindow(newNote);
			if (editNoteWindow.ShowDialog() == true)
			{
				DatabaseHelper.Insert(newNote);
				GetNotes(); // 確保刷新筆記列表
			}
		}

		public void StartEditingNote(object parameter)
		{
			if (SelectedNote == null) return;

			var originalNote = new Note
			{
				Id = SelectedNote.Id,
				NotebookId = SelectedNote.NotebookId,
				Title = SelectedNote.Title,
				Description = SelectedNote.Description,
				ImagePath = SelectedNote.ImagePath,
				CreatedAt = SelectedNote.CreatedAt,
				UpdatedAt = SelectedNote.UpdatedAt
			};

			var editNoteWindow = new EditNoteWindow(SelectedNote)
			{
				Owner = Application.Current.MainWindow, // 設置 Owner 屬性
				WindowStartupLocation = WindowStartupLocation.CenterOwner // 設置為 CenterOwner
			};

			var result = editNoteWindow.ShowDialog();
			if (result == true)
			{
				SelectedNote.UpdatedAt = DateTime.Now;
				DatabaseHelper.Update(SelectedNote);
				GetNotes(); // 更新資料後重新讀取資料庫
			}
			else if (result == false)
			{
				DatabaseHelper.Delete(SelectedNote);
				Notes.Remove(SelectedNote);
				SelectedNote = null;
				GetNotes(); // 確保刷新筆記列表
			}
			else
			{
				// 恢復原始值
				SelectedNote.Title = originalNote.Title;
				SelectedNote.Description = originalNote.Description;
				SelectedNote.ImagePath = originalNote.ImagePath;
				SelectedNote.CreatedAt = originalNote.CreatedAt;
				SelectedNote.UpdatedAt = originalNote.UpdatedAt;
			}
		}

		public void StartEditingNotebook(object parameter)
		{
			if (SelectedNotebook == null) return;

			var originalNotebook = new Notebook
			{
				Id = SelectedNotebook.Id,
				UserId = SelectedNotebook.UserId,
				Name = SelectedNotebook.Name
			};

			var editNotebookWindow = new EditNotebookWindow(SelectedNotebook)
			{
				Owner = Application.Current.MainWindow, // 設置 Owner 屬性
				WindowStartupLocation = WindowStartupLocation.CenterOwner // 設置為 CenterOwner
			};

			var result = editNotebookWindow.ShowDialog();
			if (result == true)
			{
				DatabaseHelper.Update(SelectedNotebook);
				GetNotebooks(); // 更新資料後重新讀取資料庫
			}
			else
			{
				// 恢復原始值
				SelectedNotebook.Id = originalNotebook.Id;
				SelectedNotebook.UserId = originalNotebook.UserId;
				SelectedNotebook.Name = originalNotebook.Name;
			}
		}

		private void GetNotebooks()
		{
			var notebooks = DatabaseHelper.Read<Notebook>();
			Notebooks.Clear();
			foreach (var notebook in notebooks)
			{
				Notebooks.Add(notebook);
			}
		}

		private void GetNotes()
		{
			if (SelectedNotebook != null)
			{
				var notes = DatabaseHelper.Read<Note>().Where(n => n.NotebookId == SelectedNotebook.Id).ToList();
				Notes.Clear();
				foreach (var note in notes)
				{
					Notes.Add(note);
				}
			}
		}

		private void StopEditing(object parameter)
		{
			IsVisible = Visibility.Collapsed;
			if (parameter is Notebook notebook)
			{
				DatabaseHelper.Update(notebook);
				GetNotebooks();
				GetNotes();
			}
		}
	}
}
