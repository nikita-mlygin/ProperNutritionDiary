import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { ProductIdentityType } from "../UserMenu/Get/UserMenuDetails";
import { prepareHeaders } from "../User/Token/prepareHeaders";
import { baseQueryWithReauthBuilder } from "../User/Token/baseQueryWithReauthBuilder";

const baseUrl = "https://localhost:8081/diary-api";

export interface CreateDiaryRequest {
  date: string;
}

export interface UpdateDiaryRequest {
  date: string;
}

export interface AddDiaryEntryRequest {
  productIdType: ProductIdentityType;
  productIdValue: string;
  weight: number;
  consumptionTime: string;
}

export interface UpdateDiaryEntryRequest {
  newWeight: number;
  consumptionType: string;
}

export interface DiaryResponse {
  id: string;
  userId: string;
  date: string;
  diaryEntries: DiaryEntryResponse[];
}

export interface DiaryEntryResponse {
  id: string;
  idType: ProductIdentityType;
  idValue: string;
  productName: string;
  carbohydrates: number;
  proteins: number;
  fats: number;
  calories: number;
  weight: number;
  consumptionTime: string;
  consumptionType: number;
}

export const diaryApi = createApi({
  reducerPath: "diaryApi",
  baseQuery: baseQueryWithReauthBuilder(
    fetchBaseQuery({
      baseUrl: `${baseUrl}/diary`,
      prepareHeaders: prepareHeaders,
    })
  ),
  endpoints: (builder) => ({
    createDiary: builder.mutation<string, CreateDiaryRequest>({
      query: (data) => ({
        url: "",
        method: "POST",
        body: data,
      }),
    }),
    getDiary: builder.query<DiaryResponse, string>({
      query: (diaryId) => ({ url: `/${diaryId}`, method: "POST" }),
    }),
    addDiaryEntry: builder.mutation<
      void,
      { diaryId: string; data: AddDiaryEntryRequest }
    >({
      query: ({ diaryId, data }) => ({
        url: `/${diaryId}/entry`,
        method: "POST",
        body: data,
      }),
    }),
    addDiaryEntryFromDay: builder.mutation<string, AddDiaryEntryRequest>({
      query: (data) => ({
        url: "/entry/byDate",
        method: "POST",
        body: data,
      }),
    }),
    deleteDiaryEntry: builder.mutation<void, string>({
      query: (entryId) => ({
        url: `/${entryId}`,
        method: "DELETE",
      }),
    }),
    getByDate: builder.query<DiaryResponse, string>({
      query: (date) => ({
        url: `byDate?date=${encodeURIComponent(date)}`,
        method: "GET",
      }),
    }),
    updateDiaryEntry: builder.mutation<
      void,
      { diaryId: string; entryId: string; data: UpdateDiaryEntryRequest }
    >({
      query: ({ diaryId, entryId, data }) => ({
        url: `/${diaryId}/entry/${entryId}`,
        method: "PUT",
        body: data,
      }),
    }),
  }),
});

export const {
  useCreateDiaryMutation,
  useGetDiaryQuery,
  useLazyGetDiaryQuery,
  useAddDiaryEntryMutation,
  useAddDiaryEntryFromDayMutation,
  useDeleteDiaryEntryMutation,
  useGetByDateQuery,
  useUpdateDiaryEntryMutation,
} = diaryApi;
