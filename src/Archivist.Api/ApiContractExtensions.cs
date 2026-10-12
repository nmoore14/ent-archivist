using Archivist.Contracts;

namespace Archivist.Api;

internal static class ApiContractExtensions
{
    public static RouteGroupBuilder MapKnowledgeContracts(this IEndpointRouteBuilder endpoints)
    {
        var knowledge = endpoints.MapGroup("/api/v1/knowledge")
            .WithTags("Knowledge");

        knowledge.MapGet("/search", Search)
            .WithName("SearchKnowledge")
            .Produces<KnowledgeSearchPageResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        var chat = endpoints.MapGroup("/api/v1/chat")
            .WithTags("Chat");

        chat.MapPost("/queries", Query)
            .WithName("QueryChat")
            .Accepts<ChatQueryRequest>("application/json")
            .Produces<ChatQueryResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        chat.MapGet("/sessions", Sessions)
            .WithName("ListChatSessions")
            .Produces<IReadOnlyList<ChatSessionSummaryResponse>>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status503ServiceUnavailable);

        return knowledge;
    }

    private static IResult Search(string? query, string? kind) => FeatureUnavailable(
        "Knowledge search is unavailable until identity, authorization, and document storage are configured.");

    private static IResult Query(ChatQueryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                [nameof(request.Question)] = ["A question is required."],
            });
        }

        return FeatureUnavailable("Chat is unavailable until identity, authorization, and grounded retrieval are configured.");
    }

    private static IResult Sessions() => FeatureUnavailable(
        "Chat sessions are unavailable until identity and persistence are configured.");

    private static IResult FeatureUnavailable(string detail) => Results.Problem(
        detail: detail,
        statusCode: StatusCodes.Status503ServiceUnavailable,
        title: "Feature not configured");
}
