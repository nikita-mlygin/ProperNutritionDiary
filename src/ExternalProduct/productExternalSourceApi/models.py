from pydantic import BaseModel, Field
from typing import List, Dict, Optional


class StandardFood(BaseModel):
    id: str
    name: str
    source: str
    brand: Optional[str] = None
    nutrients: Dict[str, float]
    ingredients: List[str]
    allergens: List[str]
    serving_size: Optional[float] = Field(
        alias="servingSize", default=0
    )  # Поле размера порции как число
    serving_size_unit: Optional[str] = Field(
        alias="servingSizeUnit", default="g"
    )  # Поле единицы измерения размера порции

    class Config:
        allow_population_by_field_name = True
        populate_by_name = True
        use_enum_values = True


class FoodWithRecipe(StandardFood):
    recipe_link: str  # Поле для ссылки на рецепт
