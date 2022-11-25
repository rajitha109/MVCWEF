using System.ComponentModel;

namespace MVCWEF.Models
{
    public class ProductModel
    {
        public int ProductID { get; set; }

        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
    }
}
