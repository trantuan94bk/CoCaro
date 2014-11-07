using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace Caro_ai
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        private void sgbtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Board.xaml?mode=1", UriKind.Relative));
        }

        private void mtbtn_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Board.xaml?mode=2", UriKind.Relative));
        }

        private void optbtn_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void abbtn_Click_3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Game Caro \n Đồ án 2 Phân tích thiết kế hệ thống thông tin \n Thực hiện:" +
                " \n\t Hà khánh Tùng \n\t Trần Văn Tuấn.");
        }

        private void exbtn_Click_4(object sender, RoutedEventArgs e)
        {

        }

        private void multiplay(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Caro_ai/Board.xaml",
       UriKind.Relative));

        }

        private void singleplay(object sender, System.Windows.RoutedEventArgs e)
        {
        	// TODO: Add event handler implementation here.
            NavigationService.Navigate(new Uri("/Caro_ai/Board.xaml", UriKind.Relative));
        }
    }
}