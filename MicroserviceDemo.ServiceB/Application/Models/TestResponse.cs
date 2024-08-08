namespace MicroserviceDemo.ServiceB.Application.Models
{
    public class TestResponse<T>
    {
        public int StatusCode { get; set; }
        public T Data { get; set; }
    }
}
