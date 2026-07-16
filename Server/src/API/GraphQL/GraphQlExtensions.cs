namespace API.GraphQL;

public static class GraphQlExtensions
{
    public static T ToGraphQl<T>(this SharedKernel.Result<T> result)
    {
        if (result.IsFailure)
        {
            throw new GraphQLException(result.Error.Description);
        }

        return result.Value;
    }
}
