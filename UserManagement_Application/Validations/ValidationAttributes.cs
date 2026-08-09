using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserManagement_Application.Validations
{
    public class NotFutureDateAttribute : ValidationAttribute
    {
        public NotFutureDateAttribute()
        {
            ErrorMessage = "Date cannot be in the future.";
        }

        public override bool IsValid(object? value)
        {
            if (value is not DateTime dateTime)
            {
                return true;
            }

            return dateTime.Date <= DateTime.UtcNow.Date;
        }
    }
}
