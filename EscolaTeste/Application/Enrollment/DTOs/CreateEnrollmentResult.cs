namespace EscolaTeste.Application.Enrollment.DTOs
{
    public class CreateEnrollmentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int EnrollmentId { get; set; } = 0;
    }
}