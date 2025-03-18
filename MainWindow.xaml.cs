using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ShopProductManagerApp.Logic;

namespace ShopProductManagerApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _dbContext = new AppDbContext(); // Один контекст на всё окно
        private ObservableCollection<Product> _products;

        public MainWindow()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            _products = new ObservableCollection<Product>(_dbContext.Products.ToList());
            ProductList.ItemsSource = _products;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Неправильное значение для цены!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Product newProduct = new Product
            {
                ProductName = NameTextBox.Text,
                Price = price,
                Description = DescriptionTextBox.Text
            };

            _dbContext.Products.Add(newProduct);
            _dbContext.SaveChanges();

            LoadProducts();
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Product selectedProduct)
            {
                if (MessageBox.Show($"Удалить товар '{selectedProduct.ProductName}'?", "Подтверждение",
                                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    // Решение 1: Загрузить объект через текущий контекст
                    var productToDelete = _dbContext.Products.Find(selectedProduct.ProductID);
                    _dbContext.Products.Remove(productToDelete);

                    // Решение 2: Принудительно прикрепить объект к контексту
                    // _dbContext.Products.Attach(selectedProduct);
                    // _dbContext.Entry(selectedProduct).State = EntityState.Deleted;

                    _dbContext.SaveChanges();
                    LoadProducts();
                }
            }
        }
        private void ProductList_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit)
            {
                var product = (Product)e.Row.DataContext;
                _dbContext.Entry(product).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
