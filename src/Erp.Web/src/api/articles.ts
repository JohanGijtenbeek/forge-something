import { apiClient } from './client';
import type {
  ArticleListResponse,
  ArticleDetailResponse,
  ArticleHistoryEntry,
  ArticleCategoryResponse,
  UnitOfMeasureResponse,
  BomLineResponse,
  ArticleOperationResponse,
  AddArticleOperationRequest,
  UpdateArticleOperationRequest,
  OperationTypeResponse,
  MachineTypeResponse,
  CreateArticleRequest,
  UpdateArticleRequest,
  AddBomComponentRequest,
  PagedResult,
} from '../types/api';

export interface ArticlesParams {
  page?: number;
  pageSize?: number;
  search?: string;
  categoryId?: string;
  articleType?: string;
  includeInactive?: boolean;
}

export interface UpdateBomComponentRequest {
  quantity: number;
  unitOfMeasureId: string | null;
  sortOrder: number;
}

export const articleApi = {
  getAll: (params: ArticlesParams = {}) =>
    apiClient.get<PagedResult<ArticleListResponse>>('/api/articles', { params }).then(r => r.data),

  getById: (id: string) =>
    apiClient.get<ArticleDetailResponse>(`/api/articles/${id}`).then(r => r.data),

  getHistory: (id: string) =>
    apiClient.get<ArticleHistoryEntry[]>(`/api/articles/${id}/history`).then(r => r.data),

  getBom: (id: string) =>
    apiClient.get<BomLineResponse[]>(`/api/articles/${id}/bom`).then(r => r.data),

  create: (request: CreateArticleRequest) =>
    apiClient.post<{ id: string }>('/api/articles', request).then(r => r.data),

  update: (id: string, request: UpdateArticleRequest) =>
    apiClient.put(`/api/articles/${id}`, request),

  deactivate: (id: string) =>
    apiClient.delete(`/api/articles/${id}`),

  addBomComponent: (id: string, request: AddBomComponentRequest) =>
    apiClient.post<{ id: string }>(`/api/articles/${id}/bom`, request).then(r => r.data),

  updateBomComponent: (id: string, lineId: string, request: UpdateBomComponentRequest) =>
    apiClient.put(`/api/articles/${id}/bom/${lineId}`, request),

  removeBomComponent: (id: string, lineId: string) =>
    apiClient.delete(`/api/articles/${id}/bom/${lineId}`),

  getCategories: () =>
    apiClient.get<ArticleCategoryResponse[]>('/api/article-categories').then(r => r.data),

  getUnitsOfMeasure: () =>
    apiClient.get<UnitOfMeasureResponse[]>('/api/units-of-measure').then(r => r.data),

  getOperations: (id: string) =>
    apiClient.get<ArticleOperationResponse[]>(`/api/articles/${id}/operations`).then(r => r.data),

  addOperation: (id: string, request: AddArticleOperationRequest) =>
    apiClient.post<{ id: string }>(`/api/articles/${id}/operations`, request).then(r => r.data),

  updateOperation: (id: string, opId: string, request: UpdateArticleOperationRequest) =>
    apiClient.put(`/api/articles/${id}/operations/${opId}`, request),

  removeOperation: (id: string, opId: string) =>
    apiClient.delete(`/api/articles/${id}/operations/${opId}`),

  getOperationTypes: () =>
    apiClient.get<OperationTypeResponse[]>('/api/operation-types').then(r => r.data),

  getMachineTypes: () =>
    apiClient.get<MachineTypeResponse[]>('/api/machine-types').then(r => r.data),
};
