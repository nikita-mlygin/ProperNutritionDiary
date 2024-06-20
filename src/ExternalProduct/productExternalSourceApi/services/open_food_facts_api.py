from math import ceil
import os
import requests
from typing import List, Dict
from utils.main import parse_ingredients
from models import StandardFood
from logger import logger, _

OPEN_FOOD_FACTS_API_URL = os.getenv("OPEN_FOOD_FACTS_API_URL")
OPEN_FOOD_FACTS_SEARCH_API_URL = os.getenv("OPEN_FOOD_FACTS_SEARCH_API_URL")


def extract_nutrients_from_open_food_facts(product: Dict) -> Dict[str, float]:
    logger.info(
        _(
            "try extract nutrients from product: {product}",
            product=product.get("nutriments", {}),
        )
    )

    nutriments = product.get("nutriments", {})
    nutriments_estimated = product.get("nutriments_estimated", {})

    def get_nutrient_value(nutrient_key: str) -> float:
        return nutriments.get(nutrient_key, nutriments_estimated.get(nutrient_key, 0))

    nutrients = {
        "calories": get_nutrient_value("energy-kcal_100g"),
        "protein": get_nutrient_value("proteins_100g"),
        "fat": get_nutrient_value("fat_100g"),
        "carbohydrates": get_nutrient_value("carbohydrates_100g"),
        "sugar": get_nutrient_value("sugars_100g"),
        "fiber": get_nutrient_value("fiber_100g"),
        "sodium": get_nutrient_value("sodium_100g"),
        "cholesterol": get_nutrient_value("cholesterol_100g"),
        "calcium": get_nutrient_value("calcium_100g"),
        "iron": get_nutrient_value("iron_100g"),
        "potassium": get_nutrient_value("potassium_100g"),
        "vitamin_a": get_nutrient_value("vitamin-a_100g"),
        "vitamin_c": get_nutrient_value("vitamin-c_100g"),
        "vitamin_d": get_nutrient_value("vitamin-d_100g"),
        "vitamin_e": get_nutrient_value("vitamin-e_100g"),
        "vitamin_k": get_nutrient_value("vitamin-k_100g"),
        "vitamin_b6": get_nutrient_value("vitamin-b6_100g"),
        "vitamin_b12": get_nutrient_value("vitamin-b12_100g"),
        "magnesium": get_nutrient_value("magnesium_100g"),
        "phosphorus": get_nutrient_value("phosphorus_100g"),
        "zinc": get_nutrient_value("zinc_100g"),
    }

    logger.info(
        _(
            "end processing nutrients, result: {nutrients}",
            nutrients=nutrients,
        )
    )

    return nutrients


def search_open_food_facts(query: str, page: int) -> tuple[List[StandardFood], int]:
    headers = {
        "User-Agent": "ProperlyNutritionDiary/0.0.1 (nikita.malygin.work@mail.ru)"
    }
    params = {"search_terms": query, "search_simple": 1, "json": 1, "page": page}
    response = requests.get(
        f"{OPEN_FOOD_FACTS_SEARCH_API_URL}", headers=headers, params=params
    )
    logger.info(_("Response received: {response}", response=response.json()))

    response = response.json()
    products = response["products"]
    return_data = []

    for product in products:
        nutrients = extract_nutrients_from_open_food_facts(product)

        serving_size_value = product.get("serving_quantity", 100)
        serving_size_unit = product.get("serving_quantity_unit", "g")

        logger.info(
            _(
                "Serving sizes: {servingSizeValue} {servingSizeUnit}",
                servingSizeValue=serving_size_value,
                servingSizeUnit=serving_size_unit,
            )
        )

        return_data.append(
            StandardFood(
                id=product["_id"],
                name=product.get("product_name", "UNDEFINED"),
                brand=product.get("brands", ""),
                nutrients=nutrients,
                ingredients=parse_ingredients(product.get("ingredients_text", "")),
                allergens=[
                    tag.replace("en:", "") for tag in product.get("allergens_tags", [])
                ],
                serving_size=serving_size_value,
                serving_size_unit=serving_size_unit,
                source="openfoodfacts",
            )
        )

    return (
        return_data,
        min(ceil(response.get("count", -1) / response.get("page_size", 1)), 1000),
    )


def fetch_open_food_facts(barcode: str) -> StandardFood:
    logger.info("Start processing food with barcode {barcode}")
    headers = {
        "User-Agent": "ProperlyNutritionDiary/0.0.1 (nikita.malygin.work@mail.ru)"
    }
    response = requests.get(
        f"{OPEN_FOOD_FACTS_API_URL}/product/{barcode}.json", headers=headers
    )
    logger.info(_("Response received: {response}", response=response.text))
    product = response.json()["product"]

    nutrients = extract_nutrients_from_open_food_facts(product)

    serving_size_value = product.get("serving_quantity", 100)
    serving_size_unit = product.get("serving_quantity_unit", "g")

    logger.info(
        _(
            "Serving sizes: {servingSizeValue} {servingSizeUnit}",
            servingSizeValue=serving_size_value,
            servingSizeUnit=serving_size_unit,
        )
    )

    return StandardFood(
        id=product["_id"],
        name=product["product_name"],
        brand=product.get("brands", ""),
        nutrients=nutrients,
        ingredients=parse_ingredients(product.get("ingredients_text", "")),
        allergens=[tag.replace("en:", "") for tag in product.get("allergens_tags", [])],
        serving_size=serving_size_value,
        serving_size_unit=serving_size_unit,
        source="openfoodfacts",
    )
