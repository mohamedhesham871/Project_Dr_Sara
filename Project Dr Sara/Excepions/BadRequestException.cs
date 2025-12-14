using System.Globalization;

namespace Project_Dr_Sara.Excepions
{
    public class BadRequestException(string message) : Exception(message)
    {
    }
}
