import { apiClient } from './client';
import type {
  PartyListResponse,
  PartyDetailResponse,
  PagedResult,
  CreateOrganizationRequest,
  CreatePersonRequest,
  UpdateOrganizationRequest,
  UpdatePersonRequest,
  PartyHistoryEntry,
  SearchResult,
} from '../types/api';

export interface PartiesParams {
  page?: number;
  pageSize?: number;
  includeInactive?: boolean;
}

export const partyApi = {
  getAll: (params: PartiesParams = {}) =>
    apiClient.get<PagedResult<PartyListResponse>>('/api/parties', { params }).then(r => r.data),

  getCustomers: (includeInactive = false) =>
    apiClient.get<PartyListResponse[]>('/api/parties/customers', { params: { includeInactive } }).then(r => r.data),

  getSuppliers: (includeInactive = false) =>
    apiClient.get<PartyListResponse[]>('/api/parties/suppliers', { params: { includeInactive } }).then(r => r.data),

  getById: (id: string) =>
    apiClient.get<PartyDetailResponse>(`/api/parties/${id}`).then(r => r.data),

  getHistory: (id: string) =>
    apiClient.get<PartyHistoryEntry[]>(`/api/parties/${id}/history`).then(r => r.data),

  createOrganization: (request: CreateOrganizationRequest) =>
    apiClient.post<{ id: string }>('/api/parties/organizations', request).then(r => r.data),

  createPerson: (request: CreatePersonRequest) =>
    apiClient.post<{ id: string }>('/api/parties/persons', request).then(r => r.data),

  updateOrganization: (id: string, request: UpdateOrganizationRequest) =>
    apiClient.put(`/api/parties/${id}/organization`, request),

  updatePerson: (id: string, request: UpdatePersonRequest) =>
    apiClient.put(`/api/parties/${id}/person`, request),

  deactivate: (id: string) =>
    apiClient.delete(`/api/parties/${id}`),
};

export const searchApi = {
  search: (q: string, limit = 5) =>
    apiClient.get<SearchResult[]>('/api/search', { params: { q, limit } }).then(r => r.data),

  reindex: () =>
    apiClient.post<string>('/api/search/reindex').then(r => r.data),
};
