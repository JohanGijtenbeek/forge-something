import { apiClient } from './client';
import type {
  OrderSummaryResponse,
  OrderDetailResponse,
  OrderHistoryEntry,
  CreateOrderRequest,
  UpdateOrderStatusRequest,
  PagedResult,
} from '../types/api';

export interface OrdersParams {
  page?: number;
  pageSize?: number;
  search?: string;
  status?: string;
}

export const orderApi = {
  getAll: (params: OrdersParams = {}) =>
    apiClient.get<PagedResult<OrderSummaryResponse>>('/api/orders', { params }).then(r => r.data),

  getById: (id: string) =>
    apiClient.get<OrderDetailResponse>(`/api/orders/${id}`).then(r => r.data),

  getHistory: (id: string) =>
    apiClient.get<OrderHistoryEntry[]>(`/api/orders/${id}/history`).then(r => r.data),

  create: (request: CreateOrderRequest) =>
    apiClient.post<{ id: string }>('/api/orders', request).then(r => r.data),

  updateStatus: (id: string, request: UpdateOrderStatusRequest) =>
    apiClient.put(`/api/orders/${id}/status`, request),

  cancel: (id: string) =>
    apiClient.delete(`/api/orders/${id}`),
};
