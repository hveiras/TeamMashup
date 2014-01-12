namespace TeamMashup.Core.Contracts
{
    public class JsonResponse<T>
    {
        public T Data { get; set; }

        public bool Success { get; set; }
    }
}