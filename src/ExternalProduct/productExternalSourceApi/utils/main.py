from typing import List


def parse_ingredients(ingredients: str) -> List[str]:
    return [ingredient.strip() for ingredient in ingredients.split(",")]
