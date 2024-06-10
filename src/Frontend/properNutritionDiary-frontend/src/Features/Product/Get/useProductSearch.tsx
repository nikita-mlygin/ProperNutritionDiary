import { useState, useEffect, useCallback, useRef } from "react";
import { useDebounce } from "use-debounce";
import { ProductSummaryDto } from "./ProductSummaryDto";
import { useLazySearchProductsQuery } from "../ProductApi";

const RESULTS_PER_PAGE = 10;

function tryUpdateVisibleResultsFromCurrentCache(
  startIndex: number,
  cache: ProductSummaryDto[]
) {
  return cache.slice(startIndex, startIndex + RESULTS_PER_PAGE);
}

const useProductSearch = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const [debouncedSearchTerm, setDebouncedSearchTerm] = useDebounce(
    searchTerm,
    500
  );
  const [visibleResults, setVisibleResults] = useState<ProductSummaryDto[]>([]);
  const [endApiList, setEndApiList] = useState(false);

  const lastIndexRef = useRef(0);
  const cacheRef = useRef<ProductSummaryDto[]>([]);
  const next = useRef<string | null>(null);
  const endApi = useRef<boolean>(false);

  const [
    executeSearch,
    { data: searchData, isLoading, isFetching, error, isSuccess },
  ] = useLazySearchProductsQuery();

  const loadMore = useCallback(() => {
    console.log(
      "Load more. Must update visible results from cache. If not, need load more results. If api end, set state api end, else update. Disable if loading or fetching"
    );

    if (isFetching || isLoading || endApiList) return;

    const newData = tryUpdateVisibleResultsFromCurrentCache(
      lastIndexRef.current,
      cacheRef.current
    );
    lastIndexRef.current += newData.length;
    setVisibleResults((prev) => [...prev, ...newData]);

    if (newData.length < RESULTS_PER_PAGE)
      executeSearch({ query: debouncedSearchTerm, next: next.current });
  }, [debouncedSearchTerm, endApiList, executeSearch, isFetching, isLoading]);

  useEffect(() => {
    console.log("debounced state changed. Must rerender visible results");
    setVisibleResults([]);
    next.current = null;
    endApi.current = false;
    setEndApiList(false);
    executeSearch({ query: debouncedSearchTerm, next: null });
  }, [debouncedSearchTerm, executeSearch]);

  const updateCacheWithApiPage = useCallback(
    (newValues: ProductSummaryDto[]) => {
      cacheRef.current = [
        ...cacheRef.current,
        ...newValues.filter(
          (newItem) => !cacheRef.current.some((item) => item.id === newItem.id)
        ),
      ];
    },
    []
  );

  useEffect(() => {
    console.log(
      "If data is null, return. Data is loaded. If page is next, must add results to cache. Else rewrite cache"
    );

    if (!searchData || !isSuccess) {
      console.log("return");
      return;
    }

    next.current = searchData.next;
    if (!next.current) endApi.current = true;

    if (!next.current) {
      cacheRef.current = searchData.products;
      const newResults = tryUpdateVisibleResultsFromCurrentCache(
        0,
        cacheRef.current
      );
      lastIndexRef.current = newResults.length;
      setVisibleResults(newResults);
      return;
    }

    updateCacheWithApiPage(searchData.products);
    const newData = tryUpdateVisibleResultsFromCurrentCache(
      lastIndexRef.current,
      cacheRef.current
    );
    lastIndexRef.current += newData.length;
    setVisibleResults((prev) => [...prev, ...newData]);
  }, [isSuccess, searchData, updateCacheWithApiPage]);

  useEffect(() => {
    if (!error) return;
    setEndApiList(true);
    console.log("set end api list while error");
  }, [error]);

  return {
    searchTerm,
    setSearchTerm,
    visibleResults,
    loadMore,
    isLoading,
    isFetching,
    error,
    endApiList,
  };
};

export default useProductSearch;
