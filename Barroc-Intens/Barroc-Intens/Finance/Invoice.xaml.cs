using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.Globalization;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Barroc_Intens
{
    public sealed partial class Invoice : Page
    {
        public Invoice()
        {
            this.InitializeComponent();
        }
        private void GenereerFactuur_Click(object sender, RoutedEventArgs e)
        {
      
            string klantNaam = txtNaam.Text;
            string klantAdres = txtAdres.Text;

            
            if (decimal.TryParse(txtAansluitkosten.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal aansluitkosten) &&
                decimal.TryParse(txtBtwPercentage.Text, out decimal btwPercentage))
            {
                
                decimal btwBedrag = (aansluitkosten * btwPercentage) / 100;
                decimal totaal = aansluitkosten + btwBedrag;

                
                lblResult.Text = $"Factuur voor: {klantNaam}\n" +
                                 $"Adres: {klantAdres}\n\n" +
                                 $"Aansluitkosten: €{aansluitkosten:F2}\n" +
                                 $"BTW ({btwPercentage}%): €{btwBedrag:F2}\n" +
                                 $"Totaal: €{totaal:F2}";
            }

        }
    }
}
  

