using System;
namespace BillTracker.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string parameter) : base($"Argument '{parameter}' invalid.")
        { }
    }
}
