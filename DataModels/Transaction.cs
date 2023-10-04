using System.ComponentModel;

namespace TransactionManager.Models
{
    public class Transaction
    {
        [DefaultValue("3f2b12b8-2a06-45b4-b057-45949279b4e5")]
        public Guid Id { get; set; }
        [DefaultValue(197104)]
        public int ApplicationId { get; set; }
        [DefaultValue("Debit")]
        public string Type { get; set; }
        [DefaultValue("Payment")]
        public string Summary { get; set; }
        [DefaultValue(58.26)]
        public decimal Amount { get; set; }
        [DefaultValue("2016-07-01T00:00:00")]
        public DateTime PostingDate { get; set; }
        [DefaultValue("false")]
        public bool IsCleared { get; set; }
        [DefaultValue("2016-07-01T00:00:00")]
        public DateTime? ClearedDate { get; set; }
    }
}
