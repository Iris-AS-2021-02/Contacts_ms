namespace Support.Dtos.GraphQl
{
    public class GraphQLResponse<T> where T : class
    {
        public T Data { get; set; }
    }
}
