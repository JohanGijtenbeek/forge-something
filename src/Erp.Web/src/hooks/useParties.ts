import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { partyApi, searchApi } from '../api/parties';
import type { CreateOrganizationRequest, CreatePersonRequest, UpdateOrganizationRequest, UpdatePersonRequest } from '../types/api';

export const partyKeys = {
  all: ['parties'] as const,
  lists: () => [...partyKeys.all, 'list'] as const,
  list: (filters: object) => [...partyKeys.lists(), filters] as const,
  customers: (includeInactive: boolean) => [...partyKeys.all, 'customers', { includeInactive }] as const,
  suppliers: (includeInactive: boolean) => [...partyKeys.all, 'suppliers', { includeInactive }] as const,
  detail: (id: string) => [...partyKeys.all, 'detail', id] as const,
  history: (id: string) => [...partyKeys.all, 'history', id] as const,
};

export const searchKeys = {
  search: (q: string) => ['search', q] as const,
};

export const useParties = (includeInactive = false) =>
  useQuery({ queryKey: partyKeys.list({ includeInactive }), queryFn: () => partyApi.getAll(includeInactive) });

export const useCustomers = (includeInactive = false) =>
  useQuery({ queryKey: partyKeys.customers(includeInactive), queryFn: () => partyApi.getCustomers(includeInactive) });

export const useSuppliers = (includeInactive = false) =>
  useQuery({ queryKey: partyKeys.suppliers(includeInactive), queryFn: () => partyApi.getSuppliers(includeInactive) });

export const useParty = (id: string) =>
  useQuery({ queryKey: partyKeys.detail(id), queryFn: () => partyApi.getById(id), enabled: !!id });

export const usePartyHistory = (id: string) =>
  useQuery({ queryKey: partyKeys.history(id), queryFn: () => partyApi.getHistory(id), enabled: !!id });

export const useSearch = (q: string) =>
  useQuery({
    queryKey: searchKeys.search(q),
    queryFn: () => searchApi.search(q),
    enabled: q.length >= 2,
    staleTime: 1000 * 30,
  });

export const useCreateOrganization = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: CreateOrganizationRequest) => partyApi.createOrganization(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: partyKeys.lists() }),
  });
};

export const useCreatePerson = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: CreatePersonRequest) => partyApi.createPerson(request),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: partyKeys.lists() }),
  });
};

export const useUpdateOrganization = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdateOrganizationRequest) => partyApi.updateOrganization(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: partyKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: partyKeys.lists() });
    },
  });
};

export const useUpdatePerson = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdatePersonRequest) => partyApi.updatePerson(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: partyKeys.detail(id) });
      queryClient.invalidateQueries({ queryKey: partyKeys.lists() });
    },
  });
};

export const useDeactivateParty = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => partyApi.deactivate(id),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: partyKeys.all }),
  });
};
