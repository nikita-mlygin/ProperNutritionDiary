import { useState, useEffect, useCallback, useRef } from "react";
import { useDebounce } from "use-debounce";
import { ProductSummaryDto } from "./ProductSummaryDto";
import { useLazySearchProductsQuery } from "../ProductApi";

const RESULTS_PER_PAGE = 10;

const useProductSearch = () => {
  const [searchTerm, setSearchTerm] = useState("");
  const [debouncedSearchTerm] = useDebounce(searchTerm, 500);

  const [visibleResults, setVisibleResults] = useState<ProductSummaryDto[]>([]);
  const pageRef = useRef(1);
  const lastIndexRef = useRef(0);
  const lastLoadedPage = useRef(-1);
  const [endApiList, setEndApiList] = useState(false);
  const [triggerApiCall, setTriggerApiCall] = useState(false);
  const [searchReason, setSearchReason] = useState("initial");

  const cacheRef = useRef<ProductSummaryDto[]>([]);

  const [
    executeSearch,
    { data: searchData, isLoading, isFetching, error, isSuccess },
  ] = useLazySearchProductsQuery();

  const tryUpdateVisibleResultsFromCurrentCache = useCallback(
    (startIndex: number) => {
      return cacheRef.current.slice(startIndex, startIndex + RESULTS_PER_PAGE);
    },
    []
  );

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
    pageRef.current = 1;
    lastIndexRef.current = 0;
    setVisibleResults([]);
    setEndApiList(false);
    setTriggerApiCall(true);
    setSearchReason("newSearch");
  }, [debouncedSearchTerm]);

  useEffect(() => {
    console.log(endApiList, triggerApiCall);
    if (triggerApiCall && !endApiList) {
      executeSearch({ query: debouncedSearchTerm, page: pageRef.current });
    }
  }, [triggerApiCall, endApiList, executeSearch, debouncedSearchTerm]);

  const loadMore = useCallback(() => {
    const newResults = tryUpdateVisibleResultsFromCurrentCache(
      lastIndexRef.current
    );

    setVisibleResults((prev) => [...prev, ...newResults]);
    lastIndexRef.current += newResults.length;

    console.log("loadMore");

    if (
      newResults.length < RESULTS_PER_PAGE &&
      !(triggerApiCall && searchReason === "loadMore")
    ) {
      pageRef.current = lastLoadedPage.current + 1;
      console.log(pageRef.current);
      setTriggerApiCall(true);
      setSearchReason("loadMore");
    }
  }, [searchReason, triggerApiCall, tryUpdateVisibleResultsFromCurrentCache]);

  useEffect(() => {
    if (isLoading || isFetching) return;

    if (searchData && isSuccess) {
      lastLoadedPage.current = pageRef.current;

      if (pageRef.current === 1 && searchReason === "newSearch") {
        cacheRef.current = searchData;
        const newResults = tryUpdateVisibleResultsFromCurrentCache(0);
        setVisibleResults(newResults);
        lastIndexRef.current = newResults.length;

        if (searchData.length < RESULTS_PER_PAGE) {
          setEndApiList(true);
        }
      } else if (searchReason === "loadMore") {
        updateCacheWithApiPage(searchData);
        const newResults = tryUpdateVisibleResultsFromCurrentCache(
          lastIndexRef.current
        );
        setVisibleResults((prev) => [...prev, ...newResults]);
        lastIndexRef.current += newResults.length;

        if (searchData.length < RESULTS_PER_PAGE) {
          setEndApiList(true);
        }
      }

      setTriggerApiCall(false);
    }
  }, [
    isLoading,
    isFetching,
    isSuccess,
    searchData,
    tryUpdateVisibleResultsFromCurrentCache,
    updateCacheWithApiPage,
    searchReason,
  ]);

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
