import os
from typing import Dict, List, Union
from urllib.parse import parse_qs, urlparse
from logger import logger, _
import asyncio
import aiohttp

import requests

from models import FoodWithRecipe
from services.allergens import find_allergens


EDAMAM_RECIPE_APP_ID = os.getenv("EDAMAM_RECIPE_APP_ID")
EDAMAM_RECIPE_APP_KEY = os.getenv("EDAMAM_RECIPE_APP_KEY")


def divide_nutrients(
    nutrients: Dict[str, Union[float, None]], divisor: float
) -> Dict[str, float]:
    return {
        key: value / divisor if value is not None else None
        for key, value in nutrients.items()
    }


def extract_recipe_info(recipe: Dict) -> FoodWithRecipe:
    logger.info(_("Extract recipe info start, {recipe}", recipe=recipe))
    ingredients = [ingredient["text"] for ingredient in recipe.get("ingredients", [])]
    nutrients_info = recipe.get("totalNutrients", {})

    nutrients = {
        "calories": nutrients_info.get("ENERC_KCAL", {}).get("quantity", 0),
        "protein": nutrients_info.get("PROCNT", {}).get("quantity", 0),
        "fat": nutrients_info.get("FAT", {}).get("quantity", 0),
        "carbohydrates": nutrients_info.get("CHOCDF", {}).get("quantity", 0),
        "sugar": nutrients_info.get("SUGAR", {}).get("quantity", 0),
        "fiber": nutrients_info.get("FIBTG", {}).get("quantity", 0),
        "sodium": nutrients_info.get("NA", {}).get("quantity", 0),
        "cholesterol": nutrients_info.get("CHOLE", {}).get("quantity", 0),
        "calcium": nutrients_info.get("CA", {}).get("quantity", 0),
        "iron": nutrients_info.get("FE", {}).get("quantity", 0),
        "potassium": nutrients_info.get("K", {}).get("quantity", 0),
        "vitamin_a": nutrients_info.get("VITA_RAE", {}).get("quantity", 0),
        "vitamin_c": nutrients_info.get("VITC", {}).get("quantity", 0),
        "vitamin_d": nutrients_info.get("VITD", {}).get("quantity", 0),
        "vitamin_e": nutrients_info.get("TOCPHA", {}).get("quantity", 0),
        "vitamin_k": nutrients_info.get("VITK1", {}).get("quantity", 0),
        "vitamin_b6": nutrients_info.get("VITB6A", {}).get("quantity", 0),
        "vitamin_b12": nutrients_info.get("VITB12", {}).get("quantity", 0),
        "magnesium": nutrients_info.get("MG", {}).get("quantity", 0),
        "phosphorus": nutrients_info.get("P", {}).get("quantity", 0),
        "zinc": nutrients_info.get("ZN", {}).get("quantity", 0),
    }

    weight = recipe.get("totalWeight")

    logger.info(_("Getting weight {weight}", weight=weight))

    nutrients = divide_nutrients(nutrients, weight / 100)

    return FoodWithRecipe(
        id=recipe.get("uri", ""),
        name=recipe.get("label", ""),
        ingredients=ingredients,
        nutrients=nutrients,
        allergens=find_allergens(ingredients),
        serving_size=weight,
        serving_size_unit="g",
        source="edamam_recipe",
        recipe_link=recipe.get("url", ""),
    )


def search_recipes(query: str, cont: str | None) -> tuple[List[FoodWithRecipe], str]:
    url = "https://api.edamam.com/api/recipes/v2"

    headers = {
        "User-Agent": "ProperlyNutritionDiary/0.0.1 (nikita.malygin.work@mail.ru)",
        "Edamam-Account-User": EDAMAM_RECIPE_APP_ID,
    }

    params = {
        "type": "public",
        "app_id": EDAMAM_RECIPE_APP_ID,
        "app_key": EDAMAM_RECIPE_APP_KEY,
        "q": query,
    }

    if cont is not None:
        params["_cont"] = cont

    response = requests.get(url, headers=headers, params=params)
    logger.info(_("Response received: {response}", response=response.json()))

    response_json = response.json()

    hits = response_json.get("hits", [])
    return_data = []

    for hit in hits:
        recipe = hit.get("recipe", {})
        recipe_info = extract_recipe_info(recipe)
        return_data.append(recipe_info)

    next_url = response_json["_links"].get("next", None).get("href", None)

    if next_url is not None:
        parsed_url = urlparse(next_url)

        # Извлекаем параметры запроса
        query_params = parse_qs(parsed_url.query)

        # Получаем значение параметра _cont
        next_url = query_params.get("_cont", [None])[0]

    logger.info(
        _(
            "Received, {data}",
            data=(
                return_data,
                next_url,
            ),
        )
    )

    return (return_data, next_url)


async def fetch_recipe_from_edamam(
    session: aiohttp.ClientSession, uris: List[str]
) -> List[FoodWithRecipe]:
    url = "https://api.edamam.com/api/recipes/v2/by-uri"
    headers = {
        "User-Agent": "ProperlyNutritionDiary/0.0.1 (nikita.malygin.work@mail.ru)",
        "Edamam-Account-User": EDAMAM_RECIPE_APP_ID,
    }

    params = {
        "type": "public",
        "app_id": EDAMAM_RECIPE_APP_ID,
        "app_key": EDAMAM_RECIPE_APP_KEY,
        "uri": uris,
    }
    async with session.get(url, headers=headers, params=params) as response:
        response_json = await response.json()
        logger.info(_("Response received: {response}", response=response_json))

        hits = response_json.get("hits", [])
        return_data = []

        for hit in hits:
            recipe = hit.get("recipe", {})
            recipe_info = extract_recipe_info(recipe)
            return_data.append(recipe_info)

        return return_data


async def fetch_all_recipes_from_edamam(uri_list: List[str]) -> List[FoodWithRecipe]:
    tasks = []
    async with aiohttp.ClientSession() as session:
        for i in range(0, len(uri_list), 20):
            uris_chunk = uri_list[i : i + 20]
            logger.info(_("List: {list}", list=uri_list))
            tasks.append(fetch_recipe_from_edamam(session, uris_chunk))

        results = await asyncio.gather(*tasks)
        # Flatten the list of lists
        all_recipes = [recipe for result in results for recipe in result]
        return all_recipes
