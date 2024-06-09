from pydantic import BaseModel
from typing import List, Dict, Optional


class StandardFood(BaseModel):
    id: str
    name: str
    source: str
    brand: Optional[str] = None
    nutrients: Dict[str, float]
    ingredients: List[str]
    allergens: List[str]
    serving_size: Optional[float] = None  # Поле размера порции как число
    serving_size_unit: Optional[str] = None  # Поле единицы измерения размера порции


class FoodWithRecipe(StandardFood):
    recipe_link: str  # Поле для ссылки на рецепт
