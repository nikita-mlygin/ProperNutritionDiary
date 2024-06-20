import React, { useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogContentText,
  DialogActions,
  Button,
  Typography,
  TextField,
  Chip,
  Tabs,
  Tab,
  Box,
  IconButton,
  Collapse,
} from "@mui/material";
import { ProductSummaryDto } from "../../../Features/Product/Get/ProductSummaryDto";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import ExpandLessIcon from "@mui/icons-material/ExpandLess";
import NutrientToggleButton from "../../Macronutrient/NutrientToggleButton";

interface ProductDialogProps {
  product: ProductSummaryDto | null;
  weight: number | null;
  onClose: () => void;
}

const ProductDialog: React.FC<ProductDialogProps> = ({
  product,
  onClose,
  weight,
}) => {
  const [tabIndex, setTabIndex] = useState(0);
  const [searchTerm, setSearchTerm] = useState("");
  const [isIngredientsVisible, setIngredientsVisible] = useState(true);
  const [showPer100g, setShowPer100g] = useState(false);

  if (!product) {
    return null;
  }

  const handleTabChange = (event: React.ChangeEvent<any>, newValue: number) => {
    setTabIndex(newValue);
  };

  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(event.target.value.toLowerCase());
  };

  const toggleIngredientsVisibility = () => {
    setIngredientsVisible(!isIngredientsVisible);
  };

  const filteredNutrients = [
    ...Object.entries(product.otherNutrients),
    ...Object.entries(product.macronutrients),
  ].filter(([key]) => key.toLowerCase().includes(searchTerm));

  const calculatePer100g = (value: number) => {
    if (!weight) return value;
    return (value / weight) * 100;
  };

  return (
    <Dialog
      open={product !== null}
      onClose={onClose}
      fullWidth
      aria-labelledby="product-dialog-title"
    >
      <DialogTitle id="product-dialog-title">{product.name}</DialogTitle>
      <DialogContent>
        <DialogContentText>
          <div>
            {product.allergens.map((allergen, index) => (
              <Chip key={index} label={allergen} style={{ margin: "2px" }} />
            ))}
          </div>
          <Box
            display="flex"
            alignItems="center"
            justifyContent="space-between"
          >
            <Typography variant="subtitle1">Ingredients:</Typography>
            <IconButton onClick={toggleIngredientsVisibility} size="small">
              {isIngredientsVisible ? <ExpandLessIcon /> : <ExpandMoreIcon />}
            </IconButton>
          </Box>
          <Collapse in={isIngredientsVisible}>
            <Typography variant="body2">
              {product.ingredients.join(", ")}
            </Typography>
          </Collapse>
        </DialogContentText>
        {weight && (
          <Box
            display="flex"
            alignItems="center"
            justifyContent="space-between"
            mt={2}
          >
            <Typography variant="subtitle1">Display:</Typography>
            <NutrientToggleButton
              onClick={(e) => setShowPer100g(e == "per100g")}
              value={showPer100g ? "per100g" : "total"}
            />
          </Box>
        )}
        <Tabs
          value={tabIndex}
          onChange={handleTabChange}
          aria-label="nutrient tabs"
        >
          <Tab label="Macronutrients" />
          <Tab label="All Nutrients" />
        </Tabs>
        <Box p={3}>
          {tabIndex === 0 && (
            <div>
              <Typography variant="subtitle1">Macronutrients:</Typography>
              <Typography variant="body2">
                Proteins:{" "}
                {showPer100g
                  ? calculatePer100g(product.macronutrients.proteins).toFixed(2)
                  : product.macronutrients.proteins}{" "}
                g
              </Typography>
              <Typography variant="body2">
                Calories:{" "}
                {showPer100g
                  ? calculatePer100g(product.macronutrients.calories).toFixed(2)
                  : product.macronutrients.calories}{" "}
                kcal
              </Typography>
              <Typography variant="body2">
                Fats:{" "}
                {showPer100g
                  ? calculatePer100g(product.macronutrients.fats).toFixed(2)
                  : product.macronutrients.fats}{" "}
                g
              </Typography>
              <Typography variant="body2">
                Carbohydrates:{" "}
                {showPer100g
                  ? calculatePer100g(
                      product.macronutrients.carbohydrates
                    ).toFixed(2)
                  : product.macronutrients.carbohydrates}{" "}
                g
              </Typography>
            </div>
          )}
          {tabIndex === 1 && (
            <div>
              <TextField
                fullWidth
                margin="normal"
                label="Search Nutrients"
                variant="outlined"
                value={searchTerm}
                onChange={handleSearchChange}
              />
              <Typography variant="subtitle1">Other Nutrients:</Typography>
              {filteredNutrients.length > 0 ? (
                filteredNutrients.map(([key, value]) => (
                  <Typography key={key} variant="body2">
                    {key}:{" "}
                    {showPer100g ? calculatePer100g(value).toFixed(2) : value}{" "}
                    mg
                  </Typography>
                ))
              ) : (
                <Typography variant="body2">No nutrients found.</Typography>
              )}
            </div>
          )}
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} color="primary">
          Close
        </Button>
      </DialogActions>
    </Dialog>
  );
};

export default ProductDialog;
