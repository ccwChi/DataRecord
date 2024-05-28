using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DataRecord.Model
{
	public class Note : INotifyPropertyChanged
	{
		private int id;
		private int notebookId;
		private string title;
		private string description;
		private string imagePath;
		private DateTime createdAt;
		private DateTime updatedAt;

		[PrimaryKey, AutoIncrement]
		public int Id
		{
			get { return id; }
			set { SetProperty(ref id, value); }
		}

		[Indexed]
		public int NotebookId
		{
			get { return notebookId; }
			set { SetProperty(ref notebookId, value); }
		}

		public string Title
		{
			get { return title; }
			set { SetProperty(ref title, value); }
		}

		public string Description
		{
			get { return description; }
			set { SetProperty(ref description, value); }
		}

		public string ImagePath
		{
			get { return imagePath; }
			set { SetProperty(ref imagePath, value); }
		}

		public DateTime CreatedAt
		{
			get { return createdAt; }
			set { SetProperty(ref createdAt, value); }
		}

		public DateTime UpdatedAt
		{
			get { return updatedAt; }
			set { SetProperty(ref updatedAt, value); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetProperty<T>(ref T field, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}
