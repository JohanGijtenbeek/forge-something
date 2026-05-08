namespace Erp.Domain.Parties;

public enum PartyType
{
    Organization = 1,
    Person = 2
}

public enum AddressType
{
    Postal = 1,
    Delivery = 2,
    Invoice = 3
}

public enum ContactMethodType
{
    Phone = 1,
    Email = 2,
    Mobile = 3
}

public enum PartyRelationshipType
{
    ContactPerson = 1,
    Subsidiary = 2
}
