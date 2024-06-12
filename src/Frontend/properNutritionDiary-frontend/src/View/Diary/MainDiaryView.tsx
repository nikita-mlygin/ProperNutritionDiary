import { FC, useState, useMemo, useCallback, useRef } from "react";
import {
  Stack,
  Typography,
  Fab,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Grid,
  Button,
  Box,
  Divider,
} from "@mui/material";
import ExpandMoreIcon from "@mui/icons-material/ExpandMore";
import MacrosChartHeader from "../Macronutrient/Chart/MacrosChartHeader";
import AddIcon from "@mui/icons-material/Add";
import ProductSelectionModal from "../Product/Search/ProductSelectionModalPage";
import { DiaryItem } from "../../Features/Diary/DiaryItem";
import { ProductSummaryDto } from "../../Features/Product/Get/ProductSummaryDto";
import { DragDropContext, DropResult } from "react-beautiful-dnd";
import DraggableDiaryList from "./DraggableDiaryList";
import { DiaryMenuItems } from "./Menu/DiaryMenuItems";
import DiaryMenuItemList from "./Menu/DiaryMenuItemList";
import ProductDialog from "../Product/View/ProductDialog";
import { cloneDeep } from "lodash";

const initialTargetMacros = [203, 90, 270];

const reorder = (
  list: DiaryItem[],
  startIndex: number,
  endIndex: number
): DiaryItem[] => {
  const result = Array.from(list);
  const [removed] = result.splice(startIndex, 1);
  result.splice(endIndex, 0, removed);
  return result;
};

const move = (
  source: DiaryItem[],
  destination: DiaryItem[],
  droppableSource: any,
  droppableDestination: any
): { [key: string]: DiaryItem[] } => {
  const sourceClone = Array.from(source);
  const destClone = Array.from(destination);
  const [removed] = sourceClone.splice(droppableSource.index, 1);
  destClone.splice(droppableDestination.index, 0, removed);

  const result: { [key: string]: DiaryItem[] } = {};
  result[droppableSource.droppableId] = sourceClone;
  result[droppableDestination.droppableId] = destClone;

  return result;
};

interface MainDiaryViewProps {
  menuItems: DiaryMenuItems;
}

const MainDiaryView: FC<MainDiaryViewProps> = ({ menuItems }) => {
  const [items, setItems] = useState<{ [key: string]: DiaryItem[] }>({
    breakfast: [],
    lunch: [],
    dinner: [],
    other: [],
  });

  const [editId, setEditId] = useState<string | null>(null);
  const [newWeight, setNewWeight] = useState<number | null>(null);
  const [isModalOpen, setModalOpen] = useState(false);

  const [expandedSections, setExpandedSections] = useState({
    breakfast: true,
    lunch: true,
    dinner: true,
    other: true,
  });

  const toggleSection = (section: string) => {
    setExpandedSections((prev) => ({
      ...prev,
      [section]: !prev[section],
    }));
  };

  const currentMacros = useMemo(() => {
    return Object.values(items)
      .flat()
      .reduce(
        (acc, item) => {
          acc[0] += (item.product.macros.proteins * item.weight) / 100;
          acc[1] += (item.product.macros.fats * item.weight) / 100;
          acc[2] += (item.product.macros.carbohydrates * item.weight) / 100;
          return acc;
        },
        [0, 0, 0]
      );
  }, [items]);

  const [modalProduct, setModalProduct] = useState<ProductSummaryDto | null>(
    null
  );

  const products = useRef<Map<string, ProductSummaryDto>>(new Map());

  const handleAddProduct = (
    selectedProduct: ProductSummaryDto,
    weight: number,
    section: string
  ) => {
    const newProduct: DiaryItem = {
      product: {
        ...selectedProduct,
        macros: selectedProduct.macronutrients,
      },
      weight,
    };

    products.current.set(
      selectedProduct.id.type + "_" + selectedProduct.id.value,
      cloneDeep(selectedProduct)
    );

    setItems((prevItems) => ({
      ...prevItems,
      [section]: [...prevItems[section], newProduct],
    }));
    setModalOpen(false);
  };

  const currentCalories = useMemo(() => {
    console.log(
      Object.values(items)
        .flat()
        .reduce(
          (acc, item) =>
            acc + (item.product.macros.calories * item.weight) / 100,
          0
        )
    );

    return Object.values(items)
      .flat()
      .reduce(
        (acc, item) => acc + (item.product.macros.calories * item.weight) / 100,
        0
      );
  }, [items]);

  const handleEdit = useCallback((id: string, weight: number) => {
    setEditId(id);
    setNewWeight(weight);
  }, []);

  const handleSave = useCallback(
    (id: string) => {
      setItems((prevItems) => {
        const updatedItems = { ...prevItems };
        Object.keys(updatedItems).forEach((section) => {
          updatedItems[section] = updatedItems[section].map((item) => {
            if (item.product.id === id && newWeight !== null) {
              return {
                ...item,
                weight: newWeight,
              };
            }
            return item;
          });
        });
        return updatedItems;
      });
      setEditId(null);
      setNewWeight(null);
    },
    [newWeight]
  );

  const handleDelete = useCallback((id: string) => {
    setItems((prevItems) => {
      const updatedItems = { ...prevItems };
      Object.keys(updatedItems).forEach((section) => {
        updatedItems[section] = updatedItems[section].filter(
          (item) => item.product.id !== id
        );
      });
      return updatedItems;
    });
  }, []);

  const onDragEnd = (result: DropResult) => {
    const { source, destination } = result;

    // dropped outside the list
    if (!destination) return;

    if (source.droppableId === destination.droppableId) {
      const itemsReordered = reorder(
        items[source.droppableId],
        source.index,
        destination.index
      );

      setItems((prevItems) => ({
        ...prevItems,
        [source.droppableId]: itemsReordered,
      }));
    } else {
      const result = move(
        items[source.droppableId],
        items[destination.droppableId],
        source,
        destination
      );

      setItems((prevItems) => ({
        ...prevItems,
        ...result,
      }));
    }
  };

  const renderSection = (section: string, title: string) => (
    <Accordion
      expanded={expandedSections[section]}
      onChange={() => toggleSection(section)}
    >
      <AccordionSummary expandIcon={<ExpandMoreIcon />}>
        <Typography variant="h6">{title}</Typography>
      </AccordionSummary>
      <AccordionDetails>
        {menuItems.sections[section] &&
          !menuItems.sections[section].isConfirmed &&
          menuItems.sections[section].list.length > 0 && (
            <>
              <Grid container justifyContent="end">
                <Grid item gap={1} component={Stack}>
                  <Button variant="outlined">Dismiss</Button>
                  <Button variant="contained">Confirm</Button>
                </Grid>
              </Grid>
              <DiaryMenuItemList items={menuItems.sections[section].list} />
              <Divider sx={{ mb: 1 }} />
            </>
          )}
        <DraggableDiaryList
          items={items[section]}
          section={section}
          editId={editId}
          newWeight={newWeight}
          onItemClick={(elmId) => {
            if (products.current.has(elmId))
              setModalProduct(products.current.get(elmId) ?? null);
          }}
          handleEdit={handleEdit}
          handleSave={handleSave}
          handleDelete={handleDelete}
          setNewWeight={setNewWeight}
        />
      </AccordionDetails>
    </Accordion>
  );

  return (
    <Box sx={{ position: "relative", pb: 10 }}>
      <MacrosChartHeader
        targetMacros={initialTargetMacros}
        currentMacros={currentMacros}
        targetCalories={2702}
        currentCalories={currentCalories}
        date={new Date()}
      />
      <DragDropContext onDragEnd={onDragEnd}>
        {renderSection("breakfast", "Breakfast")}
        {renderSection("lunch", "Lunch")}
        {renderSection("dinner", "Dinner")}
        {renderSection("other", "Other")}
      </DragDropContext>
      <Fab
        color="primary"
        aria-label="add"
        sx={{
          position: "absolute",
          bottom: 1,
          right: 1,
        }}
        onClick={() => setModalOpen(true)}
      >
        <AddIcon />
      </Fab>
      <ProductSelectionModal
        open={isModalOpen}
        onClose={() => setModalOpen(false)}
        onSave={(product, weight) => handleAddProduct(product, weight, "other")}
      />
      <ProductDialog
        onClose={() => setModalProduct(null)}
        product={modalProduct}
      />
    </Box>
  );
};

export default MainDiaryView;
