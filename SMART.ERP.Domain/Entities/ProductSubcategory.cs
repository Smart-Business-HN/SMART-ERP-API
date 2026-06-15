namespace SMART.ERP.Domain.Entities
{
    /// <summary>
    /// Tabla puente (muchos-a-muchos) entre Product y Subcategory. Contiene una fila por cada
    /// subcategoría a la que pertenece el producto, INCLUYENDO la principal (Product.SubCategoryId).
    /// La principal se conserva en Product.SubCategoryId como canónica (URL, breadcrumb, SEO).
    /// </summary>
    public class ProductSubcategory
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        public int SubcategoryId { get; set; }
        public virtual Subcategory? Subcategory { get; set; }
    }
}
