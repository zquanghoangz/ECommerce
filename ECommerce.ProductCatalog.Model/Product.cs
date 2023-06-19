namespace ECommerce.ProductCatalog.Model
{
    public class Product
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = null!;
        public double Price { get; set; } = 0;
        public int Availability { get; set; }
    }
}