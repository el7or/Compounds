using System;
using System.Collections.Generic;
using System.Text;

namespace Puzzle.Compound.Models.Services
{
    public class ServiceSubTypeOutputViewModel
    {
        public Guid ServiceSubTypeId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid CompoundServiceId { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
        public int Order { get; set; }
        public decimal Cost { get; set; }
    }
    public class ServiceSubTypeInputViewModel
    {
        public Guid? ServiceSubTypeId { get; set; }
        public Guid CompoundId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public string ArabicName { get; set; }
        public string EnglishName { get; set; }
        public int Order { get; set; }
        public decimal Cost { get; set; }
    }
    public class ServiceRequestSubTypeOutPutViewModel
    {
        public Guid ServiceRequestSubTypeId { get; set; }
        public Guid ServiceRequestId { get; set; }
        public Guid ServiceSubTypeId { get; set; }
        public decimal ServiceSubTypeCost { get; set; }
        public int ServiceSubTypeQuantity { get; set; }
        public int Order { get; set; }
        public string ServiceSubTypeEnglish { get; set; }
        public string ServiceSubTypeArabic { get; set; }
    }
    public class ServiceRequestSubTypeInPutViewModel
    {
        public Guid? ServiceRequestSubTypeId { get; set; }
        public Guid ServiceRequestId { get; set; }
        public Guid ServiceSubTypeId { get; set; }
        public decimal ServiceSubTypeCost { get; set; }
        public int ServiceSubTypeQuantity { get; set; }
        public int Order { get; set; }
    }
}
