using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Relojes.Functions.Entities
{
    public class Table1Entity : TableEntity
    {
        public string ID { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public bool Consolidado { get; set; }
    }
}
