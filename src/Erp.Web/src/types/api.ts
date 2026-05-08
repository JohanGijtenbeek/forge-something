// ============================================================
// Types die 1-op-1 matchen met de backend DTOs
// ============================================================

export interface PartyListResponse {
  id: string;
  name: string;
  partyType: 'Organization' | 'Person';
  isActive: boolean;
  isCustomer: boolean;
  isSupplier: boolean;
  city: string | null;
}

export interface PartyDetailResponse {
  id: string;
  name: string;
  partyType: 'Organization' | 'Person';
  isActive: boolean;
  isCustomer: boolean;
  isSupplier: boolean;
  personDetails: PersonDetailsResponse | null;
  organizationDetails: OrganizationDetailsResponse | null;
  customerRole: CustomerRoleResponse | null;
  supplierRole: SupplierRoleResponse | null;
  addresses: AddressResponse[];
  contactMethods: ContactMethodResponse[];
  bankAccounts: BankAccountResponse[];
}

export interface PersonDetailsResponse {
  firstName: string;
  middleName: string | null;
  lastName: string;
  initials: string | null;
  fullName: string;
}

export interface OrganizationDetailsResponse {
  vatNumber: string | null;
  chamberOfCommerceNumber: string | null;
}

export interface CustomerRoleResponse {
  debtorNumber: number;
  discount: number;
  isVatShifted: boolean;
  paymentTermDays: number;
  creditLimit: number | null;
}

export interface SupplierRoleResponse {
  supplierNumber: number;
  isVatShifted: boolean;
  paymentTermDays: number;
}

export interface AddressResponse {
  addressType: 'Postal' | 'Delivery' | 'Invoice';
  street: string;
  houseNumber: string;
  houseNumberAddition: string | null;
  postalCode: string;
  city: string;
  countryCode: string;
  attention: string | null;
  isDefault: boolean;
}

export interface ContactMethodResponse {
  contactMethodType: 'Phone' | 'Email' | 'Mobile';
  value: string;
  isPrimary: boolean;
}

export interface BankAccountResponse {
  iban: string;
  bic: string | null;
  accountHolder: string | null;
  isPrimary: boolean;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface SearchResult {
  id: string;
  entityType: string;
  displayLabel: string;
  subtitle: string | null;
}

// Request types
export interface CreateOrganizationRequest {
  name: string;
  vatNumber: string | null;
  chamberOfCommerceNumber: string | null;
  registerAsCustomer: boolean;
  registerAsSupplier: boolean;
}

export interface CreatePersonRequest {
  firstName: string;
  middleName: string | null;
  lastName: string;
  initials: string | null;
}

export interface UpdateOrganizationRequest {
  name: string;
  vatNumber: string | null;
  chamberOfCommerceNumber: string | null;
}

export interface UpdatePersonRequest {
  firstName: string;
  middleName: string | null;
  lastName: string;
  initials: string | null;
}

export interface PartyHistoryEntry {
  id: number;
  eventType: string;
  summary: string;
  changedBy: string;
  changedAt: string;
}
