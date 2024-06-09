import os
from typing import Dict, List
import requests
from models import StandardFood
from services.allergens import find_allergens
from services.edamam_recipe_api import divide_nutrients
from utils.main import parse_ingredients
from logger import logger, _

page_size = 20

EDAMAM_FOOD_DATABASE_ID = os.getenv("EDAMAM_FOOD_DATABASE_ID")
EDAMAM_FOOD_DATABASE_KEY = os.getenv("EDAMAM_FOOD_DATABASE_KEY")


def extract_nutrients_from_edamam(food_item: Dict, weight: float) -> Dict[str, float]:
    nutrients_info = food_item.get("food", {}).get("nutrients", {})

    nutrients = {
        "calories": nutrients_info.get("ENERC_KCAL", 0),
        "protein": nutrients_info.get("PROCNT", 0),
        "fat": nutrients_info.get("FAT", 0),
        "carbohydrates": nutrients_info.get("CHOCDF", 0),
        "sugar": nutrients_info.get("SUGAR", 0),
        "fiber": nutrients_info.get("FIBTG", 0),
        "sodium": nutrients_info.get("NA", 0),
        "cholesterol": nutrients_info.get("CHOLE", 0),
        "calcium": nutrients_info.get("CA", 0),
        "iron": nutrients_info.get("FE", 0),
        "potassium": nutrients_info.get("K", 0),
        "vitamin_a": nutrients_info.get("VITA_RAE", 0),
        "vitamin_c": nutrients_info.get("VITC", 0),
        "vitamin_d": nutrients_info.get("VITD", 0),
        "vitamin_e": nutrients_info.get("TOCPHA", 0),
        "vitamin_k": nutrients_info.get("VITK1", 0),
        "vitamin_b6": nutrients_info.get("VITB6A", 0),
        "vitamin_b12": nutrients_info.get("VITB12", 0),
        "magnesium": nutrients_info.get("MG", 0),
        "phosphorus": nutrients_info.get("P", 0),
        "zinc": nutrients_info.get("ZN", 0),
    }

    nutrients = divide_nutrients(nutrients, weight / 100)

    return nutrients


def search_edamam_food(query: str, page: int) -> List[StandardFood]:
    from_index = (page - 1) * page_size
    to_index = from_index + page_size

    url = "https://api.edamam.com/api/food-database/v2/parser"
    headers = {
        "User-Agent": "ProperlyNutritionDiary/0.0.1 (nikita.malygin.work@mail.ru)"
    }
    params = {
        "app_id": EDAMAM_FOOD_DATABASE_ID,
        "app_key": EDAMAM_FOOD_DATABASE_KEY,
        "ingr": query,
        "from": from_index,
        "to": to_index,
    }

    response = requests.get(url, headers=headers, params=params)
    logger.info(_("Response received: {response}", response=response.json()))

    parsed = response.json().get("parsed", [])
    return_data = []

    for item in parsed:
        food = item.get("food", {})

        weight = food.get("totalWeight", 100)

        nutrients = extract_nutrients_from_edamam(item, weight)
        ingredients = parse_ingredients(food.get("ingredients", ""))

        return_data.append(
            StandardFood(
                id=food.get("foodId", ""),
                name=food.get("label", ""),
                brand=food.get("brand", ""),
                nutrients=nutrients,
                ingredients=ingredients,
                allergens=find_allergens(ingredients),
                serving_size=weight,
                serving_size_unit="g",
            )
        )

    return return_data
