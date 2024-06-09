from typing import Dict
from logger import logger, _


def extract_nutrients_from_usda(food_data: Dict) -> Dict[str, float]:
    logger.info(
        _(
            "try extract nutrients from food nutrients: {foodNutrients}",
            foodNutrients=food_data["foodNutrients"],
        )
    )

    def get_nutrient_amount(nutrient_list, nutrient_number):
        for nutrient in nutrient_list:
            if (
                "nutrient" in nutrient
                and nutrient["nutrient"].get("number") == nutrient_number
            ):
                return nutrient["amount"]
            elif (
                "nutrientNumber" in nutrient
                and nutrient["nutrientNumber"] == nutrient_number
            ):
                return nutrient["value"]
        return 0

    nutrients = {
        "calories": get_nutrient_amount(food_data["foodNutrients"], "208"),
        "protein": get_nutrient_amount(food_data["foodNutrients"], "203"),
        "fat": get_nutrient_amount(food_data["foodNutrients"], "204"),
        "carbohydrates": get_nutrient_amount(food_data["foodNutrients"], "205"),
        "sugar": get_nutrient_amount(food_data["foodNutrients"], "269"),
        "fiber": get_nutrient_amount(food_data["foodNutrients"], "291"),
        "sodium": get_nutrient_amount(food_data["foodNutrients"], "307"),
        "cholesterol": get_nutrient_amount(food_data["foodNutrients"], "601"),
        "calcium": get_nutrient_amount(food_data["foodNutrients"], "301"),
        "iron": get_nutrient_amount(food_data["foodNutrients"], "303"),
        "potassium": get_nutrient_amount(food_data["foodNutrients"], "306"),
        "vitamin_a": get_nutrient_amount(food_data["foodNutrients"], "318"),
        "vitamin_c": get_nutrient_amount(food_data["foodNutrients"], "401"),
        "vitamin_d": get_nutrient_amount(food_data["foodNutrients"], "324"),
        "vitamin_e": get_nutrient_amount(food_data["foodNutrients"], "323"),
        "vitamin_k": get_nutrient_amount(food_data["foodNutrients"], "430"),
        "vitamin_b6": get_nutrient_amount(food_data["foodNutrients"], "415"),
        "vitamin_b12": get_nutrient_amount(food_data["foodNutrients"], "418"),
        "magnesium": get_nutrient_amount(food_data["foodNutrients"], "304"),
        "phosphorus": get_nutrient_amount(food_data["foodNutrients"], "305"),
        "zinc": get_nutrient_amount(food_data["foodNutrients"], "309"),
    }

    logger.info(
        _(
            "end processing nutrients, result: {nutrients}",
            nutrients=nutrients,
        )
    )

    return nutrients
