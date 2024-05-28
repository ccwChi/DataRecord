using DataRecord.Model;
using DataRecord.ViewModel.Helpers;
using System.Windows;
using System.Windows.Input;

namespace DataRecord.View
{
	public partial class EditNotebookWindow : Window
	{
		private Notebook notebook;

		public EditNotebookWindow(Notebook notebook)
		{
			InitializeComponent();
			this.notebook = notebook;
			nameTextBox.Text = notebook.Name;
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			notebook.Name = nameTextBox.Text;
			DialogResult = true;
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			if (MessageBox.Show("Are you sure you want to delete this notebook?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
			{
				DatabaseHelper.Delete(notebook);
				DialogResult = false; // 设置为 false 表示删除操作，主窗口将更新列表
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
