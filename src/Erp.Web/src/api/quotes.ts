import { apiClient } from './client';
import type {
  QuoteSummaryResponse,
  QuoteDetailResponse,
  QuoteHistoryEntry,
  ConvertQuoteResponse,
  CreateQuoteRequest,
  UpdateQuoteHeaderRequest,
  UpdateQuoteStatusRequest,
  AddQuoteLineRequest,
  UpdateQuoteLineRequest,
  PagedResult,
} from '../types/api';

export interface QuotesParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: string;
}

export const quoteApi = {
  getAll: (params: QuotesParams = {}) =>
    apiClient.get<PagedResult<QuoteSummaryResponse>>('/api/quotes', { params }).then(r => r.data),

  getById: (id: string) =>
    apiClient.get<QuoteDetailResponse>(`/api/quotes/${id}`).then(r => r.data),

  getHistory: (id: string) =>
    apiClient.get<QuoteHistoryEntry[]>(`/api/quotes/${id}/history`).then(r => r.data),

  create: (request: CreateQuoteRequest) =>
    apiClient.post<{ id: string }>('/api/quotes', request).then(r => r.data),

  updateHeader: (id: string, request: UpdateQuoteHeaderRequest) =>
    apiClient.put(`/api/quotes/${id}`, request),

  updateStatus: (id: string, request: UpdateQuoteStatusRequest) =>
    apiClient.put(`/api/quotes/${id}/status`, request),

  delete: (id: string) =>
    apiClient.delete(`/api/quotes/${id}`),

  addLine: (quoteId: string, request: AddQuoteLineRequest) =>
    apiClient.post<{ id: string }>(`/api/quotes/${quoteId}/lines`, request).then(r => r.data),

  updateLine: (quoteId: string, lineId: string, request: UpdateQuoteLineRequest) =>
    apiClient.put(`/api/quotes/${quoteId}/lines/${lineId}`, request),

  removeLine: (quoteId: string, lineId: string) =>
    apiClient.delete(`/api/quotes/${quoteId}/lines/${lineId}`),

  acceptLine: (quoteId: string, lineId: string) =>
    apiClient.put(`/api/quotes/${quoteId}/lines/${lineId}/accept`),

  convertToOrders: (quoteId: string) =>
    apiClient.post<ConvertQuoteResponse>(`/api/quotes/${quoteId}/convert`).then(r => r.data),
};
