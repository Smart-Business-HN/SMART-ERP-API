using Ardalis.Specification;
using SMART.ERP.Domain.Entities;

namespace SMART.ERP.Application.Specifications.CountrySpecification
{
    public class FindCountrySpecification : Specification<Country>
    {
        public FindCountrySpecification(string? name, string? abbreviation, string? phoneNumberCode, int? id)
        {
            if (name != null)
            {
                if (id != null && id > 0)
                {
                    Query.Where(x => x.Name == name && x.Id == id).AsNoTracking();
                }
                else
                {
                    Query.Where(x => x.Name == name).AsNoTracking();
                }
            }
            else if (abbreviation != null)
            {
                if (id != null && id > 0)
                {
                    Query.Where(x => x.Abbreviation == abbreviation && x.Id == id).AsNoTracking();
                }
                else
                {
                    Query.Where(x => x.Abbreviation == abbreviation).AsNoTracking();
                }
            }
            else if (phoneNumberCode != null)
            {
                if (id != null && id > 0)
                {
                    Query.Where(x => x.PhoneNumberCode == phoneNumberCode && x.Id == id).AsNoTracking();
                }
                else
                {
                    Query.Where(x => x.PhoneNumberCode == phoneNumberCode).AsNoTracking();
                }
            }
        }
    }
}
