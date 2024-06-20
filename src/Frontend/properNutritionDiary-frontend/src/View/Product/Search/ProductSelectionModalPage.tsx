import React, {
  useState,
  useMemo,
  useCallback,
  useEffect,
  useRef,
} from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  Tabs,
  Tab,
  Box,
  Button,
  TextField,
  Typography,
  CircularProgress,
  List,
  ListItem,
} from "@mui/material";
import useProductSearch from "../../../Features/Product/Get/useProductSearch";
import ProductSearchItem from "./ProductSearchItem";
import ProductSearchHeader from "./ProductSearchHeader";
import { ProductSummaryDto } from "../../../Features/Product/Get/ProductSummaryDto";
import { sourceToString } from "../../../Features/Product/Get/sourceToString";

interface ProductSelectionModalProps {
  open: boolean;
  onClose: () => void;
  onSave: (selectedProduct: ProductSummaryDto, weight: number) => void;
}

const ProductSelectionModal: React.FC<ProductSelectionModalProps> = ({
  open,
  onClose,
  onSave,
}) => {
  const [tabIndex, setTabIndex] = useState(0);
  const [selectedProduct, setSelectedProduct] =
    useState<ProductSummaryDto | null>(null);
  const [weight, setWeight] = useState<number>(0);

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

  useEffect(() => {
    if (!open) {
      // Reset state when the modal is closed
      setSelectedProduct(null);
      setSearchTerm("");
      setWeight(0);
      setTabIndex(0);
    }
  }, [open, setSearchTerm]);

  const handleSelectProduct = (product: ProductSummaryDto) => {
    setSelectedProduct(product);
    setTabIndex(1);
  };

  const handleSave = () => {
    if (selectedProduct && weight > 0) {
      onSave(selectedProduct, weight);
      onClose(); // Close the modal after saving
    }
  };

  const totalMacros = useMemo(() => {
    if (!selectedProduct)
      return { calories: 0, proteins: 0, fats: 0, carbohydrates: 0 };
    return {
      calories: (selectedProduct.macronutrients.calories * weight) / 100,
      proteins: (selectedProduct.macronutrients.proteins * weight) / 100,
      fats: (selectedProduct.macronutrients.fats * weight) / 100,
      carbohydrates:
        (selectedProduct.macronutrients.carbohydrates * weight) / 100,
    };
  }, [selectedProduct, weight]);

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="md">
      <DialogTitle>Select Product and Weight</DialogTitle>
      <DialogContent>
        <Tabs
          value={tabIndex}
          onChange={(e, newValue) => setTabIndex(newValue)}
        >
          <Tab label="Select Product" />
          <Tab label="Select Weight" disabled={!selectedProduct} />
        </Tabs>
        <Box mt={2}>
          {tabIndex === 0 && (
            <Box>
              <TextField
                label="Search Products"
                fullWidth
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                InputProps={{
                  endAdornment: (
                    <>
                      {isLoading && (
                        <CircularProgress color="inherit" size={20} />
                      )}
                    </>
                  ),
                }}
              />
              <ProductSearchHeader />
              <List
                ref={listRef}
                style={{ maxHeight: "400px", overflow: "auto" }}
              >
                {visibleResults.map((result) => (
                  <ProductSearchItem
                    key={result.id.value}
                    name={result.name}
                    source={sourceToString(result.id.type)}
                    calories={result.macronutrients.calories}
                    proteins={result.macronutrients.proteins}
                    fats={result.macronutrients.fats}
                    carbohydrates={result.macronutrients.carbohydrates}
                    onSelect={() => handleSelectProduct(result)}
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
            </Box>
          )}
          {tabIndex === 1 && selectedProduct && (
            <Box>
              <Typography variant="h6">{selectedProduct.name}</Typography>
              <TextField
                label="Weight (g)"
                type="number"
                fullWidth
                value={weight}
                onChange={(e) => setWeight(parseFloat(e.target.value))}
                sx={{ mt: 2 }}
              />
              <Box mt={2}>
                <Typography variant="body2">
                  Total Calories: {totalMacros.calories.toFixed(2)}
                </Typography>
                <Typography variant="body2">
                  Total Proteins: {totalMacros.proteins.toFixed(2)}g
                </Typography>
                <Typography variant="body2">
                  Total Fats: {totalMacros.fats.toFixed(2)}g
                </Typography>
                <Typography variant="body2">
                  Total Carbohydrates: {totalMacros.carbohydrates.toFixed(2)}g
                </Typography>
              </Box>
              <Box mt={2} display="flex" justifyContent="space-between">
                <Button onClick={() => setTabIndex(0)}>Back</Button>
                <Button
                  onClick={handleSave}
                  color="primary"
                  variant="contained"
                >
                  Save
                </Button>
              </Box>
            </Box>
          )}
        </Box>
      </DialogContent>
    </Dialog>
  );
};

export default ProductSelectionModal;
