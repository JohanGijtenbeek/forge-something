import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { orderApi } from '../api/orders';
import type { OrdersParams } from '../api/orders';
import type { CreateOrderRequest, UpdateOrderStatusRequest } from '../types/api';

export const orderKeys = {
  all: ['orders'] as const,
  lists: () => [...orderKeys.all, 'list'] as const,
  list: (filters: object) => [...orderKeys.lists(), filters] as const,
  detail: (id: string) => [...orderKeys.all, 'detail', id] as const,
  history: (id: string) => [...orderKeys.all, 'history', id] as const,
};

export const useOrders = (params: OrdersParams = {}) =>
  useQuery({ queryKey: orderKeys.list(params), queryFn: () => orderApi.getAll(params) });

export const useOrder = (id: string) =>
  useQuery({ queryKey: orderKeys.detail(id), queryFn: () => orderApi.getById(id), enabled: !!id });

export const useOrderHistory = (id: string) =>
  useQuery({ queryKey: orderKeys.history(id), queryFn: () => orderApi.getHistory(id), enabled: !!id });

export const useCreateOrder = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: CreateOrderRequest) => orderApi.create(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: orderKeys.lists() }),
  });
};

export const useUpdateOrderStatus = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdateOrderStatusRequest) => orderApi.updateStatus(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: orderKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: orderKeys.lists() });
    },
  });
};

export const useCancelOrder = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => orderApi.cancel(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: orderKeys.all }),
  });
};
