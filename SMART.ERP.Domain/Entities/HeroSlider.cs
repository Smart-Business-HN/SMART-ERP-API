namespace SMART.ERP.Domain.Entities
{
    public class HeroSlider
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
    }
}
