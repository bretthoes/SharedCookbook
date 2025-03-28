﻿namespace SharedCookbook.Domain.Entities;

public class CookbookInvitation : BaseAuditableEntity
{
    public required int CookbookId { get; set; }

    public string? RecipientPersonId { get; set; }

    public required CookbookInvitationStatus InvitationStatus { get; set; }

    public DateTime? ResponseDate { get; set; }

    public Cookbook? Cookbook { get; set; }
    
    public void Accept()
    {
        if (InvitationStatus == CookbookInvitationStatus.Accepted) return;

        InvitationStatus = CookbookInvitationStatus.Accepted;
        AddDomainEvent(new InvitationAcceptedEvent(this));
    }
}
