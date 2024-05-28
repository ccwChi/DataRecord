using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace DataRecord.Model
{
	public class Notebook : INotifyPropertyChanged
	{
		private int id;
		private int userId;
		private string name;

		[PrimaryKey, AutoIncrement]
		public int Id
		{
			get { return id; }
			set { SetProperty(ref id, value); }
		}

		[Indexed]
		public int UserId
		{
			get { return userId; }
			set { SetProperty(ref userId, value); }
		}

		public string Name
		{
			get { return name; }
			set { SetProperty(ref name, value); }
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			OnPropertyChanged(propertyName);
			return true;
		}
	}
}
