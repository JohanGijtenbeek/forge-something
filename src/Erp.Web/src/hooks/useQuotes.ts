import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { quoteApi } from '../api/quotes';
import type { QuotesParams } from '../api/quotes';
import type {
  CreateQuoteRequest,
  UpdateQuoteHeaderRequest,
  UpdateQuoteStatusRequest,
  AddQuoteLineRequest,
  UpdateQuoteLineRequest,
} from '../types/api';

export const quoteKeys = {
  all: ['quotes'] as const,
  lists: () => [...quoteKeys.all, 'list'] as const,
  list: (filters: object) => [...quoteKeys.lists(), filters] as const,
  detail: (id: string) => [...quoteKeys.all, 'detail', id] as const,
  history: (id: string) => [...quoteKeys.all, 'history', id] as const,
};

export const useQuotes = (params: QuotesParams = {}) =>
  useQuery({ queryKey: quoteKeys.list(params), queryFn: () => quoteApi.getAll(params) });

export const useQuote = (id: string) =>
  useQuery({ queryKey: quoteKeys.detail(id), queryFn: () => quoteApi.getById(id), enabled: !!id });

export const useQuoteHistory = (id: string) =>
  useQuery({ queryKey: quoteKeys.history(id), queryFn: () => quoteApi.getHistory(id), enabled: !!id });

export const useCreateQuote = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: CreateQuoteRequest) => quoteApi.create(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: quoteKeys.lists() }),
  });
};

export const useUpdateQuoteHeader = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdateQuoteHeaderRequest) => quoteApi.updateHeader(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: quoteKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: quoteKeys.lists() });
    },
  });
};

export const useUpdateQuoteStatus = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdateQuoteStatusRequest) => quoteApi.updateStatus(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: quoteKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: quoteKeys.lists() });
    },
  });
};

export const useDeleteQuote = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => quoteApi.delete(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: quoteKeys.all }),
  });
};

export const useAddQuoteLine = (quoteId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: AddQuoteLineRequest) => quoteApi.addLine(quoteId, request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: quoteKeys.detail(quoteId) }),
  });
};

export const useUpdateQuoteLine = (quoteId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ lineId, request }: { lineId: string; request: UpdateQuoteLineRequest }) =>
      quoteApi.updateLine(quoteId, lineId, request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: quoteKeys.detail(quoteId) }),
  });
};

export const useRemoveQuoteLine = (quoteId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (lineId: string) => quoteApi.removeLine(quoteId, lineId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: quoteKeys.detail(quoteId) }),
  });
};

export const useAcceptQuoteLine = (quoteId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (lineId: string) => quoteApi.acceptLine(quoteId, lineId),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: quoteKeys.detail(quoteId) }),
  });
};

export const useConvertQuote = (quoteId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: () => quoteApi.convertToOrders(quoteId),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: quoteKeys.detail(quoteId) });
      queryClient.invalidateQueries({ queryKey: quoteKeys.lists() });
    },
  });
};
