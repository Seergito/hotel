using System;
using System.Collections.Generic;
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

namespace Hospedaje
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            desglose.Visibility = Visibility.Hidden;

            hospedaje.IsReadOnly = true;
            servicios.IsReadOnly = true;
            total_pagar.IsReadOnly = true;
            tb_iva.IsReadOnly = true;
            total_pagar.IsReadOnly = true;



        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            desglose.Visibility = Visibility.Hidden;

            nombre.Text = "";
            apellidos.Text = "";
            llegada.SelectedDate=DateTime.Today;
            salida.SelectedDate = DateTime.Today;
            label_noches.Content = "";

            habitacion.IsChecked = false;
            habitacion2.IsChecked = false;

            desayuno.IsChecked = false;
            wifi.IsChecked = false;
            spa.IsChecked = false;

            hospedaje.Text = "";
            servicios.Text = "";
            suma.Text = "";
            tb_iva.Text = "";
            total_pagar.Text = "";

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            if(nombre.Text.Length>1 && apellidos.Text.Length>1 && llegada.SelectedDate<salida.SelectedDate && salida.SelectedDate>DateTime.Today && (habitacion.IsChecked==true || habitacion2.IsChecked == true))
            {
          
                decimal importe_hospedaje = 0;
                decimal importe_servicios = 0;
                decimal sum =0;
                decimal iva = 0;
                decimal total = 0;

                TimeSpan intervalo = (TimeSpan)(salida.SelectedDate - llegada.SelectedDate);
                int noches = intervalo.Days;
                label_noches.Content = noches;
                
                if (habitacion.IsChecked == true)
                {
                    importe_hospedaje = importe_hospedaje + 60 * noches;
                   
                }
                else
                {
                    importe_hospedaje = importe_hospedaje + 120 * noches;
                  
                }

                if (desayuno.IsChecked == true)
                {
                    importe_servicios = importe_servicios + (15 * noches);
                }if (wifi.IsChecked == true)
                {
                    importe_servicios = importe_servicios + (5 * noches);

                }
                if (spa.IsChecked == true)
                {
                    importe_servicios = importe_servicios + (30 * noches);
                }

                sum = importe_hospedaje + importe_servicios;
                iva = sum * (decimal)0.21;

                total = sum + iva;

                hospedaje.Text = string.Format("{0:c}", importe_hospedaje);
                servicios.Text= string.Format("{0:c}", importe_servicios);
                suma.Text= string.Format("{0:c}", sum);
                tb_iva.Text= string.Format("{0:c}", iva);
                total_pagar.Text= string.Format("{0:c}", total);

                desglose.Visibility = Visibility.Visible;


            }
            else
            {
                MessageBox.Show("Rellene todos los campos");
            }

        }
    }
}
