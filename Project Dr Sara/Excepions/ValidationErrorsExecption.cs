namespace Project_Dr_Sara.Excepions
{
    public class ValidationErrorsExecption(IEnumerable<string> errors)
        : Exception("Validation Errors Occured")
    {
       public List<string> Errors { get; } = errors.ToList();
    }
}
