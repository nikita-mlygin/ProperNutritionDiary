from typing import Dict
from logger import logger, _


def extract_nutrients_from_usda(food_data: Dict) -> Dict[str, float]:
    logger.info(
        _(
            "try extract nutrients from food nutrients: {foodNutrients}",
            foodNutrients=food_data["foodNutrients"],
        )
    )

    nutrient_map = {
        "208": "calories",
        "203": "protein",
        "204": "fat",
        "205": "carbohydrates",
        "269": "sugar",
        "291": "fiber",
        "307": "sodium",
        "601": "cholesterol",
        "301": "calcium",
        "303": "iron",
        "306": "potassium",
        "318": "vitamin_a",
        "401": "vitamin_c",
        "324": "vitamin_d",
        "323": "vitamin_e",
        "430": "vitamin_k",
        "415": "vitamin_b6",
        "418": "vitamin_b12",
        "304": "magnesium",
        "305": "phosphorus",
        "309": "zinc",
    }

    nutrients = {v: 0 for v in nutrient_map.values()}

    for nutrient in food_data["foodNutrients"]:
        nutrient_number = nutrient.get("nutrientNumber") or nutrient.get(
            "nutrient", {}
        ).get("number")
        if nutrient_number in nutrient_map:
            nutrient_name = nutrient_map[nutrient_number]
            amount = nutrient.get("amount") or nutrient.get("value", 0)
            nutrients[nutrient_name] = amount

    logger.info(
        _(
            "end processing nutrients, result: {nutrients}",
            nutrients=nutrients,
        )
    )

    return nutrients
