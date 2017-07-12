using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NARA.Common_p.Model
{
    /// <summary>
    /// Mapping class for retrieving InstitutionUnitOwner from
    /// museu.ms platform API
    /// </summary>
    public class InstitutionUnitOwner
    {
        public string Title { get; set; }
        public string DashedTitle { get; set; }
        public string FullAddress { get; set; }
        public object PhoneNumber { get; set; }
        public object Email { get; set; }
        public object Website { get; set; }
        public object InstitutionOpeningHours { get; set; }
        public object Admissions { get; set; }
        public object ShortDescription { get; set; }
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int TypeOfDocument { get; set; }
        public string ImageUrl { get; set; }
        public string OriginalImageUrl { get; set; }
        public string LastChangeDate { get; set; }
        public object Description { get; set; }
        public object DocumentLocation { get; set; }
        public object CountryCode { get; set; }
        public double AverageRating { get; set; }
        public int NumberOfFavorites { get; set; }
        public int NumberOfFavoritesLastWeek { get; set; }
        public int NumberOfReviews { get; set; }
        public object DocumentReviews { get; set; }
        public object DocumentMedias { get; set; }
        public string CalcField { get; set; }
        public string CalcField2 { get; set; }
        public object DocumentProvider { get; set; }
        public object DocumentSource { get; set; }
        public object DocumentTags { get; set; }
        public object CopyrightComment { get; set; }
        public bool Participation { get; set; }
        public bool InEu { get; set; }
        public object DocumentsLinks { get; set; }
        public object LinkedDocuments { get; set; }
        public bool IsExternal { get; set; }
        public int Status { get; set; }
        public object DocumentAudios { get; set; }
    }
}
