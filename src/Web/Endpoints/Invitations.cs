using SharedCookbook.Application.Common.Models;
using SharedCookbook.Application.Invitations.Commands.CreateInvitation;
using SharedCookbook.Application.Invitations.Commands.DeleteInvitation;
using SharedCookbook.Application.Invitations.Commands.UpdateInvitation;
using SharedCookbook.Application.Invitations.Queries.GetInvitationsWithPagination;
using SharedCookbook.Domain.Entities;

namespace SharedCookbook.Web.Endpoints;

public class Invitations : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetInvitationsWithPagination)
            .MapPost(CreateInvitation)
            .MapPut(UpdateInvitation, "{id}")
            .MapDelete(DeleteInvitation, "{id}");
    }

    public Task<PaginatedList<CookbookInvitation>> GetInvitationsWithPagination(ISender sender, [AsParameters] GetInvitationsWithPaginationQuery query)
    {
        return sender.Send(query);
    }

    public Task<int> CreateInvitation(ISender sender, CreateInvitationCommand command)
    {
        return sender.Send(command);
    }

    public async Task<IResult> UpdateInvitation(ISender sender, int id, UpdateInvitationCommand command)
    {
        if (id != command.Id) return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteInvitation(ISender sender, int id)
    {
        await sender.Send(new DeleteInvitationCommand(id));
        return Results.NoContent();
    }
}
