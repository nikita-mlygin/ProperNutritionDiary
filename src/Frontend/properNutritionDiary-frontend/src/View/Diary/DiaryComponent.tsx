import { FC, useState, useEffect, useCallback } from "react";
import MainDiaryView from "./MainDiaryView";
import {
  useAddDiaryEntryMutation,
  AddDiaryEntryRequest,
  useGetByDateQuery,
} from "../../Features/Diary/DiaryApi";
import { DiaryItem } from "../../Features/Diary/DiaryItem";
import { ProductIdentityType } from "../../Features/UserMenu/Get/UserMenuDetails";

const DiaryComponent: FC = () => {
  const [menuItems, setMenuItems] = useState<{
    [key: string]: DiaryItem[];
  } | null>(null);
  const { data, error, isLoading } = useGetByDateQuery(
    "2024-06-20T14:59:26.761"
  );
  const [addDiaryEntry] = useAddDiaryEntryMutation();

  useEffect(() => {
    if (data) {
      setMenuItems({
        breakfast: data.diaryEntries
          .filter((entry) => entry.consumptionTime === 0)
          .map<DiaryItem>((elm) => ({
            product: {
              id: { type: elm.idType, value: elm.idValue },
              macros: { ...elm },
              name: elm.productName,
            },
            weight: elm.weight,
          })),

        lunch: data.diaryEntries
          .filter((entry) => entry.consumptionTime === 1)
          .map<DiaryItem>((elm) => ({
            product: {
              id: { type: elm.idType, value: elm.idValue },
              macros: { ...elm },
              name: elm.productName,
            },
            weight: elm.weight,
          })),
        dinner: data.diaryEntries
          .filter((entry) => entry.consumptionTime === 2)
          .map<DiaryItem>((elm) => ({
            product: {
              id: { type: elm.idType, value: elm.idValue },
              macros: { ...elm },
              name: elm.productName,
            },
            weight: elm.weight,
          })),

        other: data.diaryEntries
          .filter((entry) => entry.consumptionTime === 3)
          .map<DiaryItem>((elm) => ({
            product: {
              id: { type: elm.idType, value: elm.idValue },
              macros: { ...elm },
              name: elm.productName,
            },
            weight: elm.weight,
          })),
      });
    }
  }, [data]);

  const handleApiAdd = useCallback(
    async (
      productType: ProductIdentityType,
      productId: string,
      weight: number,
      section: string
    ) => {
      try {
        const newEntry: AddDiaryEntryRequest = {
          productIdType: productType,
          productIdValue: productId,
          weight,
          consumptionTime: section,
        };
        await addDiaryEntry({
          diaryId: "someDiaryId",
          data: newEntry,
        }).unwrap();
      } catch (error) {
        console.error("Failed to add diary entry", error);
      }
    },
    [addDiaryEntry]
  );

  if (isLoading) return <div>Loading...</div>;
  if (error) return <div>Error loading diary data</div>;

  return (
    menuItems && (
      <MainDiaryView
        menuItems={{ sections: {} }}
        diaryItemsInit={menuItems}
        onApiAdd={handleApiAdd}
      />
    )
  );
};

export default DiaryComponent;
