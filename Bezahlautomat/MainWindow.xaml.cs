using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;


namespace Bezahlautomat
{
    /// <summary>
    /// Hauptklasse für die Nutzeroberfläche
    /// </summary>
    public partial class MainWindow : Window
    {
        private Automatenlogik Automatenlogik { get; set; } = new();

        private bool BezahlVorgaengeAnzeigenToggle = false;
        public MainWindow()
        {
            InitializeComponent();
            Update();
        }

        private string BetragString()
        {
            return BetragFormatieren(Automatenlogik.GetOffenerBetrag());
        }

        private string WechselGeldString()
        {
            return MuenzenString(Automatenlogik.WechselGeld);
        }

        private string MuenzVorratString()
        {
            return MuenzenString(Automatenlogik.MuenzVorrat);
        }

        private string MuenzenString(int[] muenzen)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Automatenlogik.MuenzTyp typ in Enum.GetValues(typeof(Automatenlogik.MuenzTyp)))
            {
                int anzahl = Automatenlogik.Anzahl(typ, muenzen);
                if (anzahl == 0)
                {
                    continue;
                }
                sb.Append(String.Format("{0}x{1}  ", anzahl, Automatenlogik.Symbol(typ)));
            }
            return sb.ToString();
        }


        private string BetragFormatieren(int betragInCent)
        {
            return String.Format("{0:D2},{1:D2}€", betragInCent / 100, betragInCent % 100);
        }

        private void BezahlVorgaengeAnzeigen()
        {
            BezahlVorgaengeText.Text = "";
            if (!BezahlVorgaengeAnzeigenToggle)
            { 
                return;
            }
            Dictionary<DateTime, int> bezahlVorgaenge = Automatenlogik.BezahlVorgaenge();
            foreach (var item in bezahlVorgaenge)
            {
                BezahlVorgaengeText.Text += item.Key.ToString();
                BezahlVorgaengeText.Text += ": ";
                BezahlVorgaengeText.Text += BetragFormatieren(item.Value);
                BezahlVorgaengeText.Text += "\n";
            }
        }

        /// <summary>
        /// Aktualisiert die Benutzeroberfläche  
        /// nach einer erfolgten Aktion
        /// </summary>
        private void Update()
        {
            Betrag.Content = BetragString();
            WechselGeld.Content = WechselGeldString();
            MuenzVorrat.Content = MuenzVorratString();
            FehlerMeldung.Content = Automatenlogik.FehlerMeldung;
            BezahlVorgaengeAnzeigen();
        }

        private void AutomatLeeren_Button_Click(object sender, RoutedEventArgs e)
        {
            Automatenlogik.AutomatLeeren();
            Update();
        }

        private void ZweiEuro_Button_Click(object sender, RoutedEventArgs e)
        {
            Automatenlogik.GeldEinwerfen(Automatenlogik.MuenzTyp.ZWEI_EURO);
            Update();
        }
        private void EinEuro_Button_Click(object sender, RoutedEventArgs e)
        {
            Automatenlogik.GeldEinwerfen(Automatenlogik.MuenzTyp.EIN_EURO);
            Update();
        }
        private void FuenfzigCent_Button_Click(object sender, RoutedEventArgs e)
        {
            Automatenlogik.GeldEinwerfen(Automatenlogik.MuenzTyp.FUENFZIG_CENT);
            Update();
        }
        private void ZwanzigCent_Button_Click(object sender, RoutedEventArgs e)
        {
            Automatenlogik.GeldEinwerfen(Automatenlogik.MuenzTyp.ZWANZIG_CENT);
            Update();
        }
        private void ZehnCent_Button_Click(object sender, RoutedEventArgs e)
        {
            Automatenlogik.GeldEinwerfen(Automatenlogik.MuenzTyp.ZEHN_CENT);
            Update();
        }
        private void FuenfCent_Button_Click(object sender, RoutedEventArgs e)
        {
            Automatenlogik.GeldEinwerfen(Automatenlogik.MuenzTyp.FUENF_CENT);
            Update();
        }
        private void ZweiCent_Button_Click(object sender, RoutedEventArgs e)
        {
            Automatenlogik.GeldEinwerfen(Automatenlogik.MuenzTyp.ZWEI_CENT);
            Update();
        }
        private void EinCent_Button_Click(object sender, RoutedEventArgs e)
        {
            Automatenlogik.GeldEinwerfen(Automatenlogik.MuenzTyp.EIN_CENT);
            Update();
        }

        private void WechselgeldEntnehmen_Button_Click(object sender, RoutedEventArgs e)
        {
            Automatenlogik.WechselGeldEntnehmen();
            Update();
        }
        private void Abbrechen_Button_Click(object sender, RoutedEventArgs e)
        {
            Automatenlogik.BezahlVorgangAbbrechen();
            Update();
        }
        private void Starten_Button_Click(object sender, RoutedEventArgs e)
        {
            Automatenlogik.BezahlVorgangStarten();
            Update();
        }

        private void BezahlVorgaengeAnzeigen_Button_Click(object sender, RoutedEventArgs e)
        {
            BezahlVorgaengeAnzeigenToggle ^= true;
            Update();
        }

    }
}
