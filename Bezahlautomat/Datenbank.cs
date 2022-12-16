using Bezahlautomat.MuenzVorratTableAdapters;
using Bezahlautomat.VorgangsDatenTableAdapters;
using System;
using System.Collections.Generic;

namespace Bezahlautomat
{
    /// <summary>
    /// Einfache Helper-Klasse für die Datenbankoperationen 
    /// Speichern und Abrufen
    /// </summary>
    internal class Datenbank
    {
        // In VorgangsDaten.Designer.cs muss der Connection string
        // angepasst werden, da der autogenerierte Code
        // die korrekte Datei nicht findet.
        public static readonly string CONNECTION_STRING = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\thoma\\source\\repos\\Bezahlautomat\\Bezahlautomat\\App_Data\\Database.mdf;" +
            "Integrated Security=True";

        VORGANGS_DATENTableAdapter VORGANGS_DATENTableAdapter = new();
        MUENZ_VORRATTableAdapter MUENZ_VORRATTableAdapter = new();

        /// <summary>
        /// Erstellt ein Dictionary aller in der DB gespeicherten
        /// Bezahlvorgänge mit Datum als Schlüssel und dem
        /// Betrag in Cent als Wert
        /// </summary>
        /// <returns>Datenbanktable als Dictionary</returns>
        public Dictionary<DateTime, int> BezahlVorgaengeAbrufen()
        {
            Dictionary<DateTime, int> bezahlVorgaenge = new();
            foreach (var row in VORGANGS_DATENTableAdapter.GetData())
            {
                bezahlVorgaenge.Add(row.Datum, row.BetragInCent);
            }
            return bezahlVorgaenge;
        }

        /// <summary>
        /// Fügt einen bezahlten Betrag mit Datum und Uhrzeit
        /// in die Datenbank ein
        /// </summary>
        /// <param name="betragInCent"></param>
        public void BezahlVorgangSpeichern(int betragInCent)
        {
            VORGANGS_DATENTableAdapter.Insert(DateTime.Now, betragInCent);
        }

        /// <summary>
        /// Lösche alle Einträge in der Datenbank
        /// </summary>
        public void BezahlVorgaengeLoeschen()
        {
            foreach (var row in VORGANGS_DATENTableAdapter.GetData())
            {
                VORGANGS_DATENTableAdapter.Delete(row.Id,row.Datum, row.BetragInCent);
            }
        }

        /// <summary>
        /// Lies den Münzvorrat aus der Datenbank aus
        /// </summary>
        /// <returns></returns>
        public int[] GetMuenzVorrat()
        {
            int[] muenzVorrat = { 0, 0, 0, 0, 0, 0, 0, 0 };
            foreach (var row in MUENZ_VORRATTableAdapter.GetData())
            {
                muenzVorrat[row.MuenzTyp] = row.Anzahl;
            }
            return muenzVorrat;
        }

        /// <summary>
        /// Speichere den aktuellen Münzvorrat in der Datenbank
        /// </summary>
        /// <param name="muenzVorrat"></param>
        public void MuenzVorratSpeichern(int[] muenzVorrat)
        {
            foreach (var row in MUENZ_VORRATTableAdapter.GetData())
            {
                int typ = row.MuenzTyp;
                MUENZ_VORRATTableAdapter.Update(muenzVorrat[typ], typ, row.Anzahl);
            }
        }

    }
}