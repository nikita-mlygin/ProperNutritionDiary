import React, { useState, useEffect, useCallback, useRef } from "react";
import {
  Paper,
  Typography,
  Tabs,
  Tab,
  Box,
  IconButton,
  Fab,
  Button,
} from "@mui/material";
import {
  DetailsDay,
  MenuItemDetails,
  ProductIdentityType,
  UserMenuDetails,
} from "../../../Features/UserMenu/Get/UserMenuDetails";
import DailyMenu from "./DailyMenu";
import CloseIcon from "@mui/icons-material/Close";
import { useDebounce } from "use-debounce";
import ProductSelectionModal from "../../Product/Search/ProductSelectionModalPage";
import AddItem from "@mui/icons-material/Add";
import CancelIcon from "@mui/icons-material/Close";

interface UserMenuDetailsProps {
  userMenuDetails: UserMenuDetails;
}

interface TabPanelProps {
  children?: React.ReactNode;
  index: number;
  value: number;
}

const TabPanel: React.FC<TabPanelProps> = ({
  children,
  value,
  index,
  ...other
}) => {
  return (
    <div
      role="tabpanel"
      hidden={value !== index}
      id={`tabpanel-${index}`}
      aria-labelledby={`tab-${index}`}
      {...other}
    >
      <Box p={3}>{children}</Box>
    </div>
  );
};

const UserMenuDetailsComponent: React.FC<UserMenuDetailsProps> = ({
  userMenuDetails,
}) => {
  const dailyMenusRef = useRef<DetailsDay[]>(userMenuDetails.dailyMenus);
  const [update, setUpdate] = useState(0);
  const [selectedTab, setSelectedTab] = useState(0);
  const [productToAdd, setProductToAdd] = useState<MenuItemDetails | null>(
    null
  );
  const [isOpen, setIsOpen] = useState(false);
  const [addItemNextIndex, setAddItemNextIndex] = useState(0);

  useEffect(() => {
    if (selectedTab >= dailyMenusRef.current.length) {
      setSelectedTab(dailyMenusRef.current.length - 1);
    }
  }, [dailyMenusRef.current.length, selectedTab]);

  const forceUpdate = () => setUpdate((prev) => prev + 1);

  const handleDrop = useCallback(
    (
      item: MenuItemDetails | null,
      itemSource: [number, keyof DetailsDay],
      target: [number, keyof DetailsDay]
    ) => {
      if (!item) return;

      const [sourceIndex, sourceSection] = itemSource;
      const [targetIndex, targetSection] = target;

      dailyMenusRef.current = dailyMenusRef.current.map((x, i) => {
        let fn1 = (x: DetailsDay) => x;
        let fn2 = (x: DetailsDay) => x;

        if (i === sourceIndex) {
          fn1 = (x: DetailsDay) => ({
            ...x,
            [sourceSection]: [
              ...(x[sourceSection] as MenuItemDetails[]).filter(
                (x) => x.id !== item.id
              ),
            ],
          });
        }

        if (i === targetIndex) {
          fn2 = (x: DetailsDay) => ({
            ...x,
            [targetSection]: [
              ...(x[targetSection] as MenuItemDetails[]),
              { ...item },
            ],
          });
        }

        return fn2(fn1(x));
      });

      forceUpdate();
    },
    []
  );

  const handleAddItem = useCallback(
    (item: MenuItemDetails, target: [number, keyof DetailsDay]) => {
      const [targetIndex, targetSection] = target;

      // Clone the target day to update it
      const updatedTargetDay = { ...dailyMenusRef.current[targetIndex] };

      // Add the item to the target section
      updatedTargetDay[targetSection] = [
        ...(updatedTargetDay[targetSection] as MenuItemDetails[]),
        item,
      ];

      // Update the ref with the modified day
      dailyMenusRef.current = dailyMenusRef.current.map((elm, i) =>
        i == targetIndex ? updatedTargetDay : elm
      );

      forceUpdate();
      setProductToAdd(null);
    },
    []
  );

  const handleDeleteItem = useCallback(
    (item: MenuItemDetails, target: [number, keyof DetailsDay]) => {
      const itemId = item.id;

      const [dayIndex, section] = target;

      const updatedDay = { ...dailyMenusRef.current[dayIndex] };
      updatedDay[section] = (updatedDay[section] as MenuItemDetails[]).filter(
        (menuItem) => menuItem.id !== itemId
      );

      dailyMenusRef.current = dailyMenusRef.current.map((day, index) => {
        if (index === dayIndex) return updatedDay;
        return day;
      });

      forceUpdate();
    },
    []
  );

  const [isDrag, setIsDrag] = useState(false);

  const [overSelectedTab, setOverSelectedTab] = useState(selectedTab);

  const [debouncedSelectedTab] = useDebounce(overSelectedTab, 500);

  useEffect(() => {
    setSelectedTab(debouncedSelectedTab);
  }, [debouncedSelectedTab]);

  const handleDelete = useCallback(
    (index: number) => {
      dailyMenusRef.current = dailyMenusRef.current
        .filter((_, i) => i !== index)
        .map((e, i) => ({ ...e, dayNumber: i }));
      if (selectedTab >= dailyMenusRef.current.length) {
        setSelectedTab(selectedTab - 1);
      }
      forceUpdate();
    },
    [selectedTab]
  );

  const handleChange = useCallback(
    (event: React.SyntheticEvent, newValue: number) => {
      setSelectedTab(newValue);
    },
    []
  );

  const handleAddDay = useCallback(() => {
    dailyMenusRef.current = [
      ...dailyMenusRef.current,
      {
        breakfast: [],
        dinner: [],
        lunch: [],
        dayNumber:
          dailyMenusRef.current[dailyMenusRef.current.length - 1].dayNumber + 1,
      },
    ];

    forceUpdate();
  }, []);

  return (
    <div>
      <Typography variant="h4">User Menu Details</Typography>
      <Paper
        elevation={3}
        style={{ padding: 20, marginTop: 20, position: "relative" }}
      >
        <Box display="grid" alignItems="center" gridTemplateColumns="1fr auto">
          <Tabs
            value={selectedTab}
            onChange={handleChange}
            aria-label="daily menus tabs"
            scrollButtons="auto"
            variant="fullWidth"
          >
            {dailyMenusRef.current.map((dailyMenu, index) => (
              <Tab
                key={index}
                label={
                  <Box
                    display="flex"
                    onDragOver={() => {
                      if (isDrag) setOverSelectedTab(index);
                    }}
                    alignItems="center"
                  >
                    <span>Day {dailyMenu.dayNumber}</span>
                    <IconButton
                      size="small"
                      onClick={(event) => {
                        event.stopPropagation();
                        handleDelete(index);
                      }}
                    >
                      <CloseIcon />
                    </IconButton>
                  </Box>
                }
              />
            ))}
          </Tabs>
          <Button
            variant="contained"
            color="primary"
            sx={{
              mx: 2,
            }}
            startIcon={<AddItem />}
            onClick={handleAddDay}
          >
            Add
          </Button>
        </Box>

        {dailyMenusRef.current.map((dailyMenu, index) => (
          <TabPanel key={index} value={selectedTab} index={index}>
            <DailyMenu
              hasItemToAdd={productToAdd !== null}
              dailyMenu={dailyMenu}
              dayNumber={dailyMenu.dayNumber}
              onDrop={handleDrop}
              index={index}
              setItems={(nv) => {
                dailyMenusRef.current[index] = nv(dailyMenusRef.current[index]);
                forceUpdate();
              }}
              onItemAdd={(section) => handleAddItem(productToAdd, section)}
              key={dailyMenu.dayNumber}
              onDeleteItem={handleDeleteItem}
              onDragChange={(nv: boolean) => setIsDrag(nv)}
            />
          </TabPanel>
        ))}
        <ProductSelectionModal
          open={isOpen}
          onClose={() => setIsOpen(false)}
          onSave={(product, weight) => {
            const getNewIdentityType = (): ProductIdentityType => {
              if (product.owner) return ProductIdentityType.SystemItem;
              if (!product.externalSource)
                return ProductIdentityType.SystemItem;
              return product.externalSource.type == "USDA"
                ? ProductIdentityType.USDA
                : ProductIdentityType.Barcode;
            };

            setProductToAdd({
              ...product,
              weight: weight,
              id: "new__" + addItemNextIndex,
              productId: product.id,
              productName: product.name,
              productIdentityType: getNewIdentityType(),
            });

            setAddItemNextIndex((prev) => prev + 1);
          }}
        />
        {!productToAdd && (
          <Fab
            color="primary"
            aria-label="cancel"
            onClick={() => setIsOpen(true)}
            style={{ position: "absolute", bottom: 16, right: 16 }}
          >
            <AddItem />
          </Fab>
        )}
        {productToAdd && (
          <Fab
            aria-label="cancel"
            onClick={() => {
              setIsOpen(false);
              setProductToAdd(null);
            }}
            style={{ position: "absolute", bottom: 16, right: 16 }}
          >
            <CancelIcon />
          </Fab>
        )}
      </Paper>
    </div>
  );
};

export default UserMenuDetailsComponent;
