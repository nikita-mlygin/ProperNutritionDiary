import os
import requests
from typing import List, Dict
from services.allergens import check_title_for_allergens, find_allergens
from services.nutrients import extract_nutrients_from_usda
from utils.main import parse_ingredients
from models import StandardFood
from logger import logger, _

USDA_API_KEY = os.getenv("USDA_API_KEY")
USDA_API_URL = os.getenv("USDA_API_URL")


def process_food_data(food_data: Dict) -> StandardFood:
    logger.info(_("Process food data started: {item}", item=food_data))

    ingredients = parse_ingredients(food_data.get("ingredients", ""))
    title = food_data["description"]
    serving_size = food_data.get("servingSize", 100)
    serving_size_unit = food_data.get("servingSizeUnit")

    allergens_in_ingredients = find_allergens(ingredients)
    allergens_in_title = (
        []
        if food_data.get("dataType", "Foundation") == "Branded"
        else check_title_for_allergens(title)
    )

    all_found_allergens = set(allergens_in_ingredients + allergens_in_title)

    return StandardFood(
        id=str(food_data["fdcId"]),
        name=title,
        brand=food_data.get("brandOwner", ""),
        nutrients=extract_nutrients_from_usda(food_data),
        ingredients=ingredients,
        allergens=list(all_found_allergens),
        serving_size=serving_size,
        serving_size_unit=serving_size_unit,
        source="usda",
    )


def search_usda_food(query: str, page: int) -> tuple[List[StandardFood], int]:
    response = requests.get(
        f"{USDA_API_URL}/foods/search?query={query}&api_key={USDA_API_KEY}&dataType=Branded,Foundation&pageNumber={page}&pageSize=20"
    )
    data = response.json()
    logger.info(_("Usda response received, result: {result}", result=data))
    foods = []
    for item in data["foods"]:
        foods.append(process_food_data(item))

    totalPages = data.get("totalPages", -1)

    totalPages = (10000 / 20) if totalPages > (10000 / 20) else totalPages

    return (foods, totalPages)


def fetch_usda_food(fdc_id: str) -> StandardFood:
    response = requests.get(f"{USDA_API_URL}/food/{fdc_id}/?api_key={USDA_API_KEY}")
    product = response.json()
    return process_food_data(product)
