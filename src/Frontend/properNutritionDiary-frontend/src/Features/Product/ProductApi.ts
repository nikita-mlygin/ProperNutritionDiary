import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { baseQueryWithReauthBuilder } from "../User/Token/baseQueryWithReauthBuilder";
import { prepareHeaders } from "../User/Token/prepareHeaders";
import { ProductSummaryDto } from "./Get/ProductSummaryDto";
import { CreateProductRequest } from "./Create/CreateProductRequest";

const baseUrl = "https://localhost:8081/product-api/";

export const productApi = createApi({
  reducerPath: "productApi",
  baseQuery: baseQueryWithReauthBuilder(
    fetchBaseQuery({
      baseUrl: baseUrl,
      prepareHeaders: prepareHeaders,
    })
  ),
  tagTypes: ["Product", "Search"],

  endpoints: (builder) => ({
    getProductById: builder.query<ProductSummaryDto, string>({
      query: (id) => `/product/${id}`,
    }),
    searchProducts: builder.query<
      ProductSummaryDto[],
      { query: string; page: number }
    >({
      query: ({ query, page }) =>
        query == ""
          ? `/product/s/?page=${page}`
          : `/product/s/${query}?page=${page}`,
      providesTags: (result, error, { query, page }) => [
        { type: "Search", id: `${query}-${page}` },
      ],
    }),
    createProduct: builder.mutation<void, CreateProductRequest>({
      query: (newProduct) => ({
        url: "/product",
        method: "POST",
        body: newProduct,
      }),
    }),
  }),
});

export const {
  useGetProductByIdQuery,
  useSearchProductsQuery,
  useLazySearchProductsQuery,
  useCreateProductMutation,
} = productApi;

// realize UI searh input
// search must have dealy for user end input
// search must update (create api call) only when stored values does not contain require entities for filter (for example, if get 200 values of search "choc", and user enter "chocolate", we must check, have we any results, and if its ok (count), we didn't call api). When api call, we didnt override, we add to current array, and then filter
// in end of result list must be button "load more" and then we get second page
