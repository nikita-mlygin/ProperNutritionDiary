import React, { useState, useEffect, useRef, useCallback } from "react";
import {
  TextField,
  Box,
  CircularProgress,
  List,
  ListItem,
  Typography,
  Paper,
} from "@mui/material";
import useProductSearch from "../../../Features/Product/Get/useProductSearch";
import ProductSearchItem from "./ProductSearchItem";
import ProductSearchHeader from "./ProductSearchHeader";
import { ProductSummaryDto } from "../../../Features/Product/Get/ProductSummaryDto";

interface ProductSearchProps {
  onSelect: (product: ProductSummaryDto) => void;
}

const ProductSearch: React.FC<ProductSearchProps> = ({ onSelect }) => {
  const {
    searchTerm,
    setSearchTerm,
    visibleResults,
    loadMore,
    isLoading,
    isFetching,
    error,
    endApiList,
  } = useProductSearch();

  const listRef = useRef<HTMLUListElement | null>(null);

  const handleScroll = useCallback(() => {
    if (!listRef.current) return;

    const bottom =
      listRef.current.scrollHeight - listRef.current.scrollTop <=
      listRef.current.clientHeight + 50;
    if (bottom && !isLoading && !isFetching && !endApiList) {
      loadMore();
    }
  }, [isLoading, isFetching, endApiList, loadMore]);

  useEffect(() => {
    const currentListRef = listRef.current;
    if (currentListRef) {
      currentListRef.addEventListener("scroll", handleScroll);
    }

    return () => {
      if (currentListRef) {
        currentListRef.removeEventListener("scroll", handleScroll);
      }
    };
  }, [handleScroll]);

  return (
    <Paper>
      <TextField
        label="Search Products"
        fullWidth
        value={searchTerm}
        onChange={(e) => setSearchTerm(e.target.value)}
        InputProps={{
          endAdornment: (
            <>{isLoading && <CircularProgress color="inherit" size={20} />}</>
          ),
        }}
      />
      <ProductSearchHeader />
      <List ref={listRef} style={{ maxHeight: "400px", overflow: "auto" }}>
        {visibleResults.map((result) => (
          <ProductSearchItem
            key={result.id}
            name={result.name}
            source={result.externalSource.type}
            calories={result.macronutrients.calories}
            proteins={result.macronutrients.proteins}
            fats={result.macronutrients.fats}
            carbohydrates={result.macronutrients.carbohydrates}
            onSelect={() => onSelect(result)} // Pass the onSelect prop
          />
        ))}
        {isFetching && (
          <ListItem>
            <CircularProgress color="inherit" size={20} />
          </ListItem>
        )}
        {endApiList && (
          <ListItem>
            <Typography variant="body2" color="textSecondary">
              No more results
            </Typography>
          </ListItem>
        )}
      </List>
    </Paper>
  );
};

export default ProductSearch;
