using System;
using System.Collections.Generic;

namespace Bezahlautomat
{
    /// <summary>
    /// Hauptklasse für die Programmlogik
    /// </summary>
    internal class Automatenlogik
    {
        public Automatenlogik()
        {
            MuenzVorrat = Datenbank.GetMuenzVorrat();
        }

        /// <summary>
        /// Gibt die verschiedenen Arbeitszustände des Automaten an.
        /// </summary>
        public enum AutomatenStatus
        {
            BEREIT,
            ERWARTE_GELDEINWURF,
            BEZAHLVORGANG_BEENDET,
            BEZAHLVORGANG_ABGEBROCHEN,
            NICHT_GENÜGEND_WECHSELGELD
        }

        /// <summary>
        /// Definiert die acht verschiedenen Münztypen.
        /// Um den Greedy-Algorithmus zur Wechselgeldrückgabe 
        /// ohne zusätzliches Sortieren nutzen zu können,
        /// werden die Münztypen nach Wert absteigend(!) 
        /// vorsortiert.
        /// </summary>
        public enum MuenzTyp : int
        {
            ZWEI_EURO = 0,
            EIN_EURO = 1,
            FUENFZIG_CENT = 2,
            ZWANZIG_CENT = 3,
            ZEHN_CENT = 4,
            FUENF_CENT = 5,
            ZWEI_CENT = 6,
            EIN_CENT = 7
        }

        public static readonly int[] MuenzWerte = {200,100,50,20,10,5,2,1 };
        public static readonly string[] MuenzSymbole = {"2€", "1€", "50c", "20c", "10c", "5c", "2c", "1c" };

        public static readonly string BEREIT = "";
        public static readonly string ABBRUCH = "Bezahlvorgang abgebrochen";
        public static readonly string NICHT_GENUG_WECHSELGELD = "Automat kann nicht wechseln";
        public string FehlerMeldung = BEREIT;

        /// <summary>
        /// Gibt den Wert einer Münze bestimmten Typs
        /// </summary>
        /// <param name="typ">Typ der Münze</param>
        /// <returns>Wert der Münze</returns>
        public int Wert(MuenzTyp typ)
        {
            return MuenzWerte[(int)typ];
        }

        /// <summary>
        /// Gibt das string-Symbol der Münze zurück
        /// </summary>
        /// <param name="typ">Typ der Münze</param>
        /// <returns>Symbol der Münze</returns>
        public string Symbol(MuenzTyp typ)
        {
            return MuenzSymbole[(int)typ];
        }

        public int Anzahl(MuenzTyp typ, int[] muenzen)
        {
            return muenzen[(int)typ];
        }

        private readonly int MinBetrag = 1;
        private readonly int MaxBetrag = 500;

        private int[] EingeworfenesGeld = { 0,0,0,0,0,0,0,0};
        public int[] MuenzVorrat { get; private set; } = { 20, 20, 20, 20, 20, 20, 20, 20 };

        /// <summary>
        /// Bestand in Wechselgeldausgabe
        /// </summary>
        public int[] WechselGeld { get; private set; } = { 0,0,0,0,0,0,0,0};

        /// <summary>
        /// Aktuell zu bezahlender Betrag.
        /// Um den noch offenen Betrag zu erhalten, nutze
        /// die entsprechende Methode
        /// </summary>
        public int BetragInCent { get; private set; } = 0;

        public int GeldSumme(int[] muenzen)
        {
            int summe = 0;
            foreach (MuenzTyp typ in Enum.GetValues(typeof(MuenzTyp)))
            {
                summe += Wert(typ) * Anzahl(typ, muenzen);
            }
            return summe;
        }

        /// <summary>
        /// Ruft erfolgte Bezahlvorgänge aus der Datenbank ab
        /// </summary>
        /// <returns>Dictionary der Bezahlvorgänge {Datum => Betrag}</returns>
        public Dictionary<DateTime, int> BezahlVorgaenge()
        {
            return Datenbank.BezahlVorgaengeAbrufen();
        }

        Datenbank Datenbank =  new();

        private void BezahlVorgangSpeichern()
        {
            Datenbank.BezahlVorgangSpeichern(BetragInCent);
        }

        /// <summary>
        /// Betriebszustand des Automaten
        /// </summary>
        public AutomatenStatus Status { get; private set; } = AutomatenStatus.BEREIT;

        /// <summary>
        /// Startet einen neuen Bezahlvorgang mit einem
        /// Zufallsbetrag zwischen Minbetrag und Maxbetrag
        /// </summary>
        public void BezahlVorgangStarten()
        {
            if (Status != AutomatenStatus.BEREIT && Status != AutomatenStatus.BEZAHLVORGANG_ABGEBROCHEN)
            {
                return;
            }
            BetragInCent = new Random().Next(MinBetrag, MaxBetrag+1);
            Status = AutomatenStatus.ERWARTE_GELDEINWURF;
            FehlerMeldung = BEREIT;
            ZurueckSetzen(EingeworfenesGeld);
            ZurueckSetzen(WechselGeld);
        }

        /// <summary>
        /// Fügt eine Münze dem eingeworfenen 
        /// Geldbetrag hinzu
        /// </summary>
        /// <param name="typ">Die eingeworfene Münze</param>
        public void GeldEinwerfen(MuenzTyp typ)
        {
            if (Status != AutomatenStatus.ERWARTE_GELDEINWURF)
            {
                return;
            }
            EingeworfenesGeld[(int)typ]++;
            if (GeldSumme(EingeworfenesGeld) >= BetragInCent)
            {
                WechselGeldBestimmen();
            }
        }

        private int Min(int a, int b)
        {
            return (a <= b) ? a : b;
        }

        private int Max(int a, int b)
        {
            return (a >= b) ? a : b;
        }

        /// <summary>
        /// Berechnet das Wechselgeld mittels Greedy-Algorithmus.
        /// Dieser liefert für 1,2,5-Münzsysteme beweisbar
        /// immer optimale Ergebnisse.
        /// </summary>
        /// <param name="wechselGeldBetrag"></param>
        /// <returns></returns>
        private int[] WechselGeldBerechnen(int wechselGeldBetrag)
        {
            if (wechselGeldBetrag > GeldSumme(MuenzVorrat))
            {
                return null;
            }
            int[] wechselGeld = { 0,0,0,0,0,0,0,0};
            foreach (MuenzTyp typ in Enum.GetValues(typeof(MuenzTyp)))
            {
                int anzahl = Min(wechselGeldBetrag / Wert(typ), Anzahl(typ, MuenzVorrat));
                wechselGeldBetrag -= anzahl * Wert(typ);
                wechselGeld[(int)typ] += anzahl;
            }
            if (wechselGeldBetrag != 0)
            {
                return null;
            }
            return wechselGeld;
        }

        private void WechselGeldBestimmen()
        {
            int wechselGeldBetrag = GeldSumme(EingeworfenesGeld) - BetragInCent;
            int[] wechselGeld = WechselGeldBerechnen(wechselGeldBetrag);
            if (wechselGeld == null)
            {
                NichtGenuegendWechselGeld();
                return;
            }
            WechselGeld = wechselGeld;
            foreach (MuenzTyp typ in Enum.GetValues(typeof(MuenzTyp)))
            {
                MuenzVorrat[(int)typ] -= WechselGeld[(int)typ];
            }
            Status = AutomatenStatus.BEZAHLVORGANG_BEENDET;
            Datenbank.MuenzVorratSpeichern(MuenzVorrat);
        }

        private void NichtGenuegendWechselGeld()
        {
            Status = AutomatenStatus.NICHT_GENÜGEND_WECHSELGELD;
            FehlerMeldung = NICHT_GENUG_WECHSELGELD;
            Uebertragen(EingeworfenesGeld, WechselGeld);
            ZurueckSetzen(EingeworfenesGeld);
        }

        private void ZurueckSetzen(int[] muenzen)
        {
            foreach (MuenzTyp typ in Enum.GetValues(typeof(MuenzTyp)))
            {
                muenzen[(int)typ] = 0;
            }
        }

        private void Uebertragen(int[] muenzenEingang, int[] muenzenAusgang)
        {
            foreach (MuenzTyp typ in Enum.GetValues(typeof(MuenzTyp)))
            {
                muenzenAusgang[(int)typ] += muenzenEingang[(int)typ];
                muenzenEingang[(int)typ] = 0;
            }
        }

        /// <summary>
        /// Leert die Wechselgeldrückgabe und setzt den Automaten 
        /// in den passenden Zustand zurück
        /// </summary>
        public void WechselGeldEntnehmen()
        {
            if (Status == AutomatenStatus.NICHT_GENÜGEND_WECHSELGELD)
            {
                Status = AutomatenStatus.ERWARTE_GELDEINWURF;
            }
            else if (Status == AutomatenStatus.BEZAHLVORGANG_BEENDET)
            {
                BezahlVorgangSpeichern();
                Status = AutomatenStatus.BEREIT;
                BetragInCent = 0;
                Uebertragen(EingeworfenesGeld, MuenzVorrat);
            } else if (Status == AutomatenStatus.BEZAHLVORGANG_ABGEBROCHEN)
            {
                Status = AutomatenStatus.BEREIT;
                BetragInCent = 0;
            }
            ZurueckSetzen(EingeworfenesGeld);
            ZurueckSetzen(WechselGeld);
            FehlerMeldung = BEREIT;
        }

        /// <summary>
        /// Bestimmt den noch zu bezahlenden Betrag
        /// </summary>
        /// <returns>Zu bezahlender Betrag in Cent</returns>
        public int GetOffenerBetrag()
        {
            return Max(BetragInCent - GeldSumme(EingeworfenesGeld),0);
        }

        /// <summary>
        /// Bricht den Bezahlvorgang ab und gibt ggf. bereits
        /// eingeworfenes Geld als "Wechselgeld" zurück.
        /// </summary>
        public void BezahlVorgangAbbrechen()
        {
            if (Status == AutomatenStatus.ERWARTE_GELDEINWURF)
            {
                if (GeldSumme(EingeworfenesGeld) == 0)
                {
                    Status = AutomatenStatus.BEREIT;
                    BetragInCent = 0;
                } else
                {
                    Uebertragen(EingeworfenesGeld, WechselGeld);
                    Status = AutomatenStatus.BEZAHLVORGANG_ABGEBROCHEN;
                    FehlerMeldung = ABBRUCH;
                }
            }
        }

        private void MuenzVorratZuruecksetzen()
        {
            foreach (MuenzTyp typ in Enum.GetValues(typeof(MuenzTyp)))
            {
                MuenzVorrat[(int)typ] = 20;
                Datenbank.MuenzVorratSpeichern(MuenzVorrat);
            }
        }

        /// <summary>
        /// Leere den Automaten. 
        /// Muenzvorrat wird auf 20 Einheiten pro Typ zurückgesetzt.
        /// Liste der Bezahlvorgänge wird gelöscht.
        /// </summary>
        public void AutomatLeeren()
        {
            if (Status != AutomatenStatus.BEREIT)
            {
                return;
            }
            MuenzVorratZuruecksetzen();
            Datenbank.BezahlVorgaengeLoeschen();
        }


    }
}
