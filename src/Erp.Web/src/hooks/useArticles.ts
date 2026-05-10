import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { articleApi } from '../api/articles';
import type { ArticlesParams, UpdateBomComponentRequest } from '../api/articles';
import type { CreateArticleRequest, UpdateArticleRequest, AddBomComponentRequest } from '../types/api';

export const articleKeys = {
  all: ['articles'] as const,
  lists: () => [...articleKeys.all, 'list'] as const,
  list: (filters: object) => [...articleKeys.lists(), filters] as const,
  detail: (id: string) => [...articleKeys.all, 'detail', id] as const,
  history: (id: string) => [...articleKeys.all, 'history', id] as const,
  bom: (id: string) => [...articleKeys.all, 'bom', id] as const,
  categories: () => [...articleKeys.all, 'categories'] as const,
  unitsOfMeasure: () => [...articleKeys.all, 'units-of-measure'] as const,
};

export const useArticles = (params: ArticlesParams = {}) =>
  useQuery({ queryKey: articleKeys.list(params), queryFn: () => articleApi.getAll(params) });

export const useArticle = (id: string) =>
  useQuery({ queryKey: articleKeys.detail(id), queryFn: () => articleApi.getById(id), enabled: !!id });

export const useArticleHistory = (id: string) =>
  useQuery({ queryKey: articleKeys.history(id), queryFn: () => articleApi.getHistory(id), enabled: !!id });

export const useArticleBom = (id: string) =>
  useQuery({ queryKey: articleKeys.bom(id), queryFn: () => articleApi.getBom(id), enabled: !!id });

export const useArticleCategories = () =>
  useQuery({ queryKey: articleKeys.categories(), queryFn: articleApi.getCategories, staleTime: 1000 * 60 * 5 });

export const useUnitsOfMeasure = () =>
  useQuery({ queryKey: articleKeys.unitsOfMeasure(), queryFn: articleApi.getUnitsOfMeasure, staleTime: 1000 * 60 * 5 });

export const useCreateArticle = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: CreateArticleRequest) => articleApi.create(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: articleKeys.lists() }),
  });
};

export const useUpdateArticle = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdateArticleRequest) => articleApi.update(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: articleKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: articleKeys.lists() });
    },
  });
};

export const useDeactivateArticle = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => articleApi.deactivate(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: articleKeys.all }),
  });
};

export const useAddBomComponent = (articleId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: AddBomComponentRequest) => articleApi.addBomComponent(articleId, request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: articleKeys.bom(articleId) }),
  });
};

export const useUpdateBomComponent = (articleId: string, lineId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdateBomComponentRequest) => articleApi.updateBomComponent(articleId, lineId, request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: articleKeys.bom(articleId) }),
  });
};

export const useRemoveBomComponent = (articleId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (lineId: string) => articleApi.removeBomComponent(articleId, lineId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: articleKeys.bom(articleId) }),
  });
};
