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

// ── Articles ──────────────────────────────────────────────────────────────

export type ArticleType = 'raw_material' | 'manufactured' | 'bought_out' | 'service';

export interface ArticleListResponse {
  id: string;
  articleNumber: number;
  code: string;
  name: string;
  articleType: ArticleType;
  category: string | null;
  unitOfMeasure: string | null;
  purchasePrice: number | null;
  isActive: boolean;
}

export interface ArticleDetailResponse {
  id: string;
  articleNumber: number;
  code: string;
  name: string;
  articleType: ArticleType;
  description: string | null;
  categoryId: string | null;
  category: string | null;
  unitOfMeasureId: string | null;
  unitOfMeasure: string | null;
  purchasePrice: number | null;
  revision: string | null;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

export interface ArticleHistoryEntry {
  id: number;
  eventType: string;
  summary: string;
  changedBy: string;
  changedAt: string;
}

export interface ArticleCategoryResponse {
  id: string;
  name: string;
  sortOrder: number;
  isActive: boolean;
}

export interface UnitOfMeasureResponse {
  id: string;
  name: string;
  abbreviation: string;
  isActive: boolean;
}

export interface BomLineResponse {
  id: string;
  childArticleId: string;
  childCode: string;
  childName: string;
  childArticleType: ArticleType;
  quantity: number;
  unitOfMeasureId: string | null;
  unitOfMeasure: string | null;
  sortOrder: number;
}

export interface CreateArticleRequest {
  code: string;
  name: string;
  articleType: ArticleType;
  description: string | null;
  categoryId: string | null;
  unitOfMeasureId: string | null;
  purchasePrice: number | null;
  revision?: string | null;
}

export interface UpdateArticleRequest {
  code: string;
  name: string;
  articleType: ArticleType;
  description: string | null;
  categoryId: string | null;
  unitOfMeasureId: string | null;
  purchasePrice: number | null;
  revision?: string | null;
}

export interface ArticleOperationResponse {
  id: string;
  sequenceNumber: number;
  operationTypeId: string;
  operationTypeName: string;
  isSubcontracted: boolean;
  estimatedMinutes: number | null;
  notes: string | null;
  isConditional: boolean;
}

export interface AddArticleOperationRequest {
  sequenceNumber: number;
  operationTypeId: string;
  estimatedMinutes: number | null;
  notes: string | null;
  isConditional: boolean;
}

export interface UpdateArticleOperationRequest {
  sequenceNumber: number;
  estimatedMinutes: number | null;
  notes: string | null;
  isConditional: boolean;
}

export interface OperationTypeResponse {
  id: string;
  name: string;
  isSubcontracted: boolean;
  machineTypeId: string | null;
  machineTypeName: string | null;
  isActive: boolean;
}

export interface MachineTypeResponse {
  id: string;
  name: string;
  isActive: boolean;
}

export interface AddBomComponentRequest {
  childArticleId: string;
  quantity: number;
  unitOfMeasureId: string | null;
  sortOrder: number;
}

// ── Parties ────────────────────────────────────────────────────────────────

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
