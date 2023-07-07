using System.ComponentModel.DataAnnotations;

namespace EntityLayer.CustomValidation
{

    public class DateRangeAttribute : ValidationAttribute
    {
        private readonly string _otherPropertyName;

        public DateRangeAttribute(string otherPropertyName)
        {
            _otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(_otherPropertyName);
            var otherPropertyValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);


           if(value != null && otherPropertyValue !=null)
            {
                DateTime startDate = (DateTime)value;
                DateTime endDate = (DateTime)otherPropertyValue;

                if (startDate < endDate)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
            else
            {
                return new ValidationResult(ErrorMessage);

            }
        }
    }
}
