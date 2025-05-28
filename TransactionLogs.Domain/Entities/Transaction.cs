using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TransactionLogs.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        public string OperationType { get; set; } //Create, Update, Delete
        public string TableName { get; set; }//User , Product, etc.
        public string PrimaryKeyValue { get; set; }// This could be a UserId, ProductId, etc. depending on the TableName
        public string EntityData { get; set; }// JSON representation of the entity data involved in the transaction
        public string? TransactionId { get; set; }// Nullable for cases where the transaction is not part of a larger transaction context
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;// Default to current UTC time



    }
}
