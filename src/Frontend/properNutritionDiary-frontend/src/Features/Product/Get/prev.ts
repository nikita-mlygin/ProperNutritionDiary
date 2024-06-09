import { useState, useEffect, useCallback, useRef } from "react";
import { useDebounce } from "use-debounce";
import { ProductSummaryDto } from "./ProductSummaryDto";
import { useLazySearchProductsQuery } from "../ProductApi";

const RESULTS_PER_PAGE = 10;

const useProductSearch = () => {
  const [searchTerm, setSearchTerm] = useState("");

  // Debounced search term to reduce API calls frequency (500ms debounce time)
  const [debouncedSearchTerm] = useDebounce(searchTerm, 500);

  // State for currently visible results
  const [visibleResults, setVisibleResults] = useState<ProductSummaryDto[]>([]);

  // State for current page number
  const [page, setPage] = useState(1);
  const [cachePage, setCachePage] = useState(1);

  const [apiUpdateReason, setApiUpdateReason] = useState<
    "searchTermChanged" | "pageLoad" | null
  >(null);
  const [apiUpdateLock, setApiUpdateLock] = useState(true);

  const [
    updateVisibleResultsFromCacheReason,
    setUpdateVisibleResultsFromCacheReason,
  ] = useState<"searchTermUpdated" | "pageLoaded" | "newCache" | null>(null);

  const [needCacheUpdate, setNeedCacheUpdate] = useState(false);

  const [lastLoadedPage, setLastLoadedPage] = useState(-1);

  // Flag to indicate if the API has more results to fetch
  const [endApilist, setEndApilist] = useState(false);

  // State to keep track of the number of visible results
  const [visibleResultsCount, setVisibleResultsCount] =
    useState(RESULTS_PER_PAGE);

  // Cache to store results and avoid repeated API calls
  const cacheRef = useRef<ProductSummaryDto[]>([]);
  const globalCacheRef = useRef<ProductSummaryDto[]>([]);

  // Hook to execute search query lazily
  const [executeSearch, { data: searchData, isLoading, isFetching, error }] =
    useLazySearchProductsQuery();

  const tryUpdateVisibleResultsFromCurrentCache = useCallback(
    (cachePage: number) => {
      const newResults = cacheRef.current.slice(
        (cachePage - 1) * RESULTS_PER_PAGE,
        cachePage * RESULTS_PER_PAGE
      );

      if (newResults.length < RESULTS_PER_PAGE) {
        setPage((page) => page + 1);
        setApiUpdateReason("pageLoad");
        return;
      }

      setVisibleResults((prev) => [...prev, ...newResults]);
    },
    []
  );

  const tryGetNewCacheFromGlobalCache = useCallback((searchTerm: string) => {
    const term = searchTerm.toLowerCase();

    // Filter cached results based on search term with prioritization
    const wordResults = globalCacheRef.current.filter((elm) =>
      elm.name.toLowerCase().split(" ").includes(term)
    );
    const startsWithResults = globalCacheRef.current.filter((elm) =>
      elm.name.toLowerCase().startsWith(term)
    );
    const containsResults = globalCacheRef.current.filter((elm) =>
      elm.name.toLowerCase().includes(term)
    );

    // Combine results, ensuring no duplicates
    const combinedResults = [
      ...new Set([
        ...wordResults,
        ...startsWithResults.filter(
          (result) => !wordResults.some((wr) => wr.id === result.id)
        ),
        ...containsResults.filter(
          (result) =>
            !wordResults.some((wr) => wr.id === result.id) &&
            !startsWithResults.some((sr) => sr.id === result.id)
        ),
      ]),
    ];

    cacheRef.current = combinedResults;
  }, []);

  const updateCacheWithApiPage = useCallback(
    (newValues: ProductSummaryDto[]) => {
      globalCacheRef.current = [
        ...globalCacheRef.current,
        ...newValues.filter(
          (newItem) =>
            !globalCacheRef.current.some((item) => item.id === newItem.id)
        ),
      ];
    },
    []
  );

  const [needPageUpdate, setNeedPageUpdate] = useState(false);
  const [needStrUpdate, setNeedStrUpdate] = useState(false);

  // Function to fetch results from cache based on search term
  const fetchFromCache = useCallback(
    (
      searchTerm: string,
      reason: "loadMore" | "editSearchTerm",
      visibleResultsCount: number
    ) => {
      console.log(
        `fetchFromCache called with reason: ${reason}, visibleResultsCount: ${visibleResultsCount}`
      );
      // Filter cached results based on search term and slice to required count
      const term = searchTerm.toLowerCase();

      // Filter cached results based on search term with prioritization
      const wordResults = cacheRef.current.filter((elm) =>
        elm.name.toLowerCase().split(" ").includes(term)
      );
      const startsWithResults = cacheRef.current.filter((elm) =>
        elm.name.toLowerCase().startsWith(term)
      );
      const containsResults = cacheRef.current.filter((elm) =>
        elm.name.toLowerCase().includes(term)
      );

      // Combine results, ensuring no duplicates
      const combinedResults = [
        ...new Set([
          ...wordResults,
          ...startsWithResults.filter(
            (result) => !wordResults.some((wr) => wr.id === result.id)
          ),
          ...containsResults.filter(
            (result) =>
              !wordResults.some((wr) => wr.id === result.id) &&
              !startsWithResults.some((sr) => sr.id === result.id)
          ),
        ]),
      ];

      // Slice to required count
      const visibleResults = combinedResults.slice(0, visibleResultsCount);

      // Update visible results state
      setVisibleResults(visibleResults);

      // If not enough results in cache, set flags to fetch new data from API
      if (visibleResults.length < visibleResultsCount) {
        console.log("In cache no more results");

        if (reason === "editSearchTerm") {
          setPage(1);
          setNeedStrUpdate(true);
        }

        if (reason === "loadMore") {
          setPage(lastLoadedPage + 1);
          console.log(`page increase`);
          setNeedPageUpdate(true);
        }
      }

      setNeedCacheUpdate(false);
    },
    [lastLoadedPage]
  );

  // Function to fetch new results from API
  const fetchNew = useCallback(
    async (
      searchTerm: string,
      page: number,
      reason: "nextPage" | "searchTermUpdate"
    ) => {
      console.log(`fetchNew called with reason: ${reason}, page: ${page}`);

      // Execute search query with current search term
      executeSearch({ query: searchTerm, page: page });

      // Handling of the data response is done in the useEffect below
    },
    [executeSearch]
  );

  // Effect to handle search term changes and fetch results from cache
  useEffect(() => {
    setApiUpdateLock(true);
    if (!needCacheUpdate) return;
    console.log("Effect for searchTerm change, calling fetchFromCache");
    setVisibleResultsCount(RESULTS_PER_PAGE);
    fetchFromCache(searchTerm, "editSearchTerm", RESULTS_PER_PAGE);
  }, [fetchFromCache, searchTerm, needCacheUpdate]);

  useEffect(() => {
    setNeedCacheUpdate(true);
  }, [searchTerm]);

  // Function to load more results and increase visible results count
  const loadMore = useCallback(() => {
    let visibleResultsCountNewValue = 0;
    console.log("loadMore called");
    setVisibleResultsCount(
      (rs) => (visibleResultsCountNewValue = rs + RESULTS_PER_PAGE)
    );
    fetchFromCache(searchTerm, "loadMore", visibleResultsCountNewValue);
  }, [fetchFromCache, searchTerm]);

  // Effect to fetch new data when needed based on flags
  useEffect(() => {
    if (needStrUpdate && !apiUpdateLock) {
      console.log("Effect for needStrUpdate, calling fetchNew");
      fetchNew(debouncedSearchTerm, 1, "searchTermUpdate");
    }

    if (needPageUpdate && !endApilist) {
      console.log("Effect for needPageUpdate, calling fetchNew");
      fetchNew(debouncedSearchTerm, page, "nextPage");
    }
  }, [
    fetchNew,
    needStrUpdate,
    endApilist,
    debouncedSearchTerm,
    page,
    needPageUpdate,
    apiUpdateLock,
  ]);

  // Effect to unlock string update lock after debounced search term changes
  useEffect(() => {
    console.log(
      "Effect for debouncedSearchTerm change, unlocking strUpdateLock"
    );
    setApiUpdateLock(false);
  }, [debouncedSearchTerm]);

  // Effect to handle the API response data
  useEffect(() => {
    if (!searchData) return;

    const data = searchData;

    // If no results from API, set flag indicating end of list
    setLastLoadedPage(page);

    if (data.length === 0) {
      setEndApilist(true);
      return;
    }

    // If fetching new results for a new search term
    if (needStrUpdate) {
      setVisibleResults(data.slice(0, RESULTS_PER_PAGE));
      setVisibleResultsCount(RESULTS_PER_PAGE);
    }

    // Update cache with new results and remove duplicates
    const newCache = [...cacheRef.current, ...data];
    const uniqueCache = newCache.filter(
      (v, i, a) => a.findIndex((v2) => v2.id === v.id) === i
    ); // Remove duplicates
    console.log("Updated cache: ", uniqueCache);
    cacheRef.current = uniqueCache;

    // If fetching new page, update visible results from cache
    if (needPageUpdate) {
      fetchFromCache(searchTerm, "loadMore", visibleResultsCount);
    }

    setNeedCacheUpdate(false);
    setNeedPageUpdate(false);
    setNeedStrUpdate(false);
  }, [
    searchData,
    needStrUpdate,
    needPageUpdate,
    fetchFromCache,
    searchTerm,
    visibleResultsCount,
    page,
  ]);

  return {
    searchTerm,
    setSearchTerm,
    visibleResults,
    loadMore,
    isLoading,
    isFetching,
    error,
    endApilist,
  };
};

export default useProductSearch;
