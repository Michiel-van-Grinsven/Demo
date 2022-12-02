using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.Validators
{
    public class RequiredGreaterThanZero : ValidationAttribute
    {
        /// <summary>
        /// Greater than zero validation attribute.
        /// </summary>
        /// <param name="value">The double value of the selection</param>
        /// <returns>True if value is greater than zero</returns>
        public override bool IsValid(object value)
        {
            // return true if value is a non-null number > 0, otherwise return false
            double i;
            return value != null && double.TryParse(value.ToString(), out i) && i > 0;
        }
    }
}
