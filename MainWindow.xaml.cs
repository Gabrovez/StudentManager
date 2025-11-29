using Microsoft.Data.Sqlite;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace StudentManager
{ // Я не много не уверен в том что просили ибо 'Оформить проект StudentManager' как я понял это конкретно этот ибо ещё
  // есть проект LoginDemo, они как бы оба в 1 solution и может вы просто перепутали и назвали солушн проектом,вы писали про то что
  // LoginDemo это другое приложение внутри этого решения поэтому я и сделал ток studentmanager
    public partial class MainWindow : Window{
        private const string ConnectionString = @"Data Source=D:\bd\students.db";
        public MainWindow(){
            InitializeComponent();
            try
            {
                LoadData();
            } catch (Exception ex){
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}","Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Add_click(object sender, RoutedEventArgs e){
            try{
                var name = InputName.Text?.Trim();
                if (string.IsNullOrEmpty(name)){
                    MessageBox.Show("Введите имя перед добавлением.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();
                using var command = new SqliteCommand($"INSERT INTO Students (Name) VALUES ($name);", connection);
                command.Parameters.AddWithValue("$name", name);
                command.ExecuteNonQuery();
                LoadData();
                InputName.Clear();
            }catch(Exception ex){
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Remove_selected_click(object sender, RoutedEventArgs e) {
            if (DataGridPeople.SelectedItem is not DataRowView row) { 
                MessageBox.Show( "Выберите запись для удаления.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            long idLong;
            try {
                idLong = Convert.ToInt64(row["Id"]);
            }catch{
                MessageBox.Show($"Не удалось прочитать id выбранной записи.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var answer = MessageBox.Show($"Удалить запись с Id = {idLong}?", "Подтвердите удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (answer != MessageBoxResult.Yes) return;

            try{
                using var connection = new SqliteConnection(ConnectionString);
                connection.Open();

                var command = new SqliteCommand($"DELETE FROM Students WHERE Id = $id;", connection);
                command.Parameters.AddWithValue("$id", idLong);
                var affected = command.ExecuteNonQuery();
                if (affected == 0){
                    MessageBox.Show("Ничего не удалено (запись не найдена).", "информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                LoadData();
            }catch (Exception ex){
                MessageBox.Show($"Ошибка при добавлении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadData(){
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = new SqliteCommand("SELECT Id, Name FROM Students ORDER BY Id", connection);
            using var reader = command.ExecuteReader();
            var dt = new DataTable();
            dt.Load(reader);
            DataGridPeople.ItemsSource = dt.DefaultView;
        }
    }
}