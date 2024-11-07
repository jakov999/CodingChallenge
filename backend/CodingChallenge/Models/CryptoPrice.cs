using System.ComponentModel.DataAnnotations.Schema;

namespace CodingChallenge.Models
{
    public class CryptoPrice
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public DateTime DateReceived { get; set; }
    }
}
