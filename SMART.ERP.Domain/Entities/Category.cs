namespace SMART.ERP.Domain.Entities
{
    public class Category
    {
        public int Id { get; init; }
        public string Slug { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Image { get; set; } = null!;
        public int Position { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreatedBy { get; set; } = null!;
        public DateTime? ModificationDate { get; set; }
        public string? ModificatedBy { get; set; }
        public bool IsPartCategory { get; set; }
        public bool IsActive { get; set; }
        public List<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
        public List<HeroSlider>? HeroSliders { get; set; }
    }
}
