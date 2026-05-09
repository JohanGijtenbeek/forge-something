using Erp.Domain.Parties;
using Erp.Domain.Parties.Commands;
using Erp.Domain.Parties.Events;
using MassTransit;
using MediatR;

namespace Erp.Infrastructure.Handlers;

public class CreateOrganizationHandler : IRequestHandler<CreateOrganizationCommand, Guid>
{
    private readonly IPartyRepository _repository;
    private readonly IBus _bus;

    public CreateOrganizationHandler(IPartyRepository repository, IBus bus)
    {
        _repository = repository;
        _bus = bus;
    }

    public async Task<Guid> Handle(CreateOrganizationCommand command, CancellationToken ct)
    {
        var party = new Party(PartyType.Organization, command.Name);
        party.AddOrganizationDetails(command.VatNumber, command.ChamberOfCommerceNumber);

        if (command.RegisterAsCustomer)
            party.RegisterAsCustomer(0, 0, false, 30, null);

        if (command.RegisterAsSupplier)
            party.RegisterAsSupplier(0, false, 30);

        await _repository.AddAsync(party, ct);

        await _bus.Publish(new PartyCreatedEvent(
            party.Id, party.Name, party.PartyType,
            party.IsCustomer, party.IsSupplier,
            DateTime.UtcNow), ct);

        return party.Id;
    }
}

public class CreatePersonHandler : IRequestHandler<CreatePersonCommand, Guid>
{
    private readonly IPartyRepository _repository;
    private readonly IBus _bus;

    public CreatePersonHandler(IPartyRepository repository, IBus bus)
    {
        _repository = repository;
        _bus = bus;
    }

    public async Task<Guid> Handle(CreatePersonCommand command, CancellationToken ct)
    {
        var fullName = string.IsNullOrEmpty(command.MiddleName)
            ? $"{command.FirstName} {command.LastName}"
            : $"{command.FirstName} {command.MiddleName} {command.LastName}";

        var party = new Party(PartyType.Person, fullName);
        party.AddPersonDetails(command.FirstName, command.MiddleName, command.LastName, command.Initials);

        await _repository.AddAsync(party, ct);

        await _bus.Publish(new PartyCreatedEvent(
            party.Id, party.Name, party.PartyType,
            false, false, DateTime.UtcNow), ct);

        return party.Id;
    }
}

public class DeactivatePartyHandler : IRequestHandler<DeactivatePartyCommand>
{
    private readonly IPartyRepository _repository;
    private readonly IBus _bus;

    public DeactivatePartyHandler(IPartyRepository repository, IBus bus)
    {
        _repository = repository;
        _bus = bus;
    }

    public async Task Handle(DeactivatePartyCommand command, CancellationToken ct)
    {
        var party = await _repository.GetByIdAsync(command.PartyId, ct)
            ?? throw new KeyNotFoundException($"Party {command.PartyId} niet gevonden.");

        await _repository.DeactivateAsync(command.PartyId, ct);

        await _bus.Publish(new PartyDeactivatedEvent(
            party.Id, party.Name, DateTime.UtcNow), ct);
    }
}

public class UpdateOrganizationHandler : IRequestHandler<UpdateOrganizationCommand>
{
    private readonly IPartyRepository _repository;
    private readonly IBus _bus;

    public UpdateOrganizationHandler(IPartyRepository repository, IBus bus)
    {
        _repository = repository;
        _bus = bus;
    }

    public async Task Handle(UpdateOrganizationCommand command, CancellationToken ct)
    {
        var party = await _repository.GetByIdWithDetailsAsync(command.PartyId, ct)
            ?? throw new KeyNotFoundException($"Party {command.PartyId} niet gevonden.");

        party.UpdateOrganization(command.Name, command.VatNumber, command.ChamberOfCommerceNumber);

        await _repository.UpdateAsync(party, ct);

        await _bus.Publish(new PartyUpdatedEvent(
            party.Id, party.Name, DateTime.UtcNow), ct);
    }
}

public class UpdatePersonHandler : IRequestHandler<UpdatePersonCommand>
{
    private readonly IPartyRepository _repository;
    private readonly IBus _bus;

    public UpdatePersonHandler(IPartyRepository repository, IBus bus)
    {
        _repository = repository;
        _bus = bus;
    }

    public async Task Handle(UpdatePersonCommand command, CancellationToken ct)
    {
        var party = await _repository.GetByIdWithDetailsAsync(command.PartyId, ct)
            ?? throw new KeyNotFoundException($"Party {command.PartyId} niet gevonden.");

        party.UpdatePerson(command.FirstName, command.MiddleName, command.LastName, command.Initials);

        await _repository.UpdateAsync(party, ct);

        await _bus.Publish(new PartyUpdatedEvent(
            party.Id, party.Name, DateTime.UtcNow), ct);
    }
}

public class AddPartyRelationshipHandler : IRequestHandler<AddPartyRelationshipCommand>
{
    private readonly IPartyRepository _repository;

    public AddPartyRelationshipHandler(IPartyRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(AddPartyRelationshipCommand command, CancellationToken ct)
        => await _repository.AddRelationshipAsync(command.FromPartyId, command.ToPartyId, command.RelationshipTypeId, ct);
}
