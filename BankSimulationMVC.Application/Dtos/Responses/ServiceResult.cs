namespace BankSimulationMVC.Application.Dtos.Responses
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string? Id { get; set; }
    }
}
