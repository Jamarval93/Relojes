using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace Relojes.Functions.Entities
{
    public class Table1Entity : TableEntity
    {
        public int ID { get; set; }
        public DateTime Fecha { get; set; }
        public int Tipo { get; set; }
        public bool Consolidado { get; set; }
    }
}
