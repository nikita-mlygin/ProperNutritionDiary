from typing import List

allergens = [
    "peanut",
    "tree nut",
    "milk",
    "egg",
    "fish",
    "shellfish",
    "soy",
    "wheat",
    "mustard",
    "celery",
    "sulfite",
    "lupin",
]


def find_allergens(ingredients_list: List[str]) -> List[str]:
    found_allergens = []
    for allergen in allergens:
        for ingredient in ingredients_list:
            if allergen in ingredient.lower():
                found_allergens.append(allergen)
                break
    return found_allergens


def check_title_for_allergens(title: str) -> List[str]:
    found_allergens = []
    for allergen in allergens:
        if allergen in title.lower():
            found_allergens.append(allergen)
    return found_allergens
