from fastapi import FastAPI, Query, HTTPException
from typing import Dict, List, Optional

from pydantic import BaseModel, Field
from services.edamam_recipe_api import (
    fetch_all_recipes_from_edamam,
    fetch_recipe_from_edamam,
    search_recipes,
)
from services.open_food_facts_api import search_open_food_facts, fetch_open_food_facts
from services.usda_api import search_usda_food, fetch_usda_food
from models import FoodWithRecipe, StandardFood
from logger import logger, _

app = FastAPI()


class SearchResult(BaseModel):
    product_list: List[StandardFood] = Field(alias="productList")
    page_numbers: List[int] = Field(alias="pageNumbers")

    class Config:
        allow_population_by_field_name = True
        populate_by_name = True
        use_enum_values = True


class RecipeSearchResult(BaseModel):
    product_list: List[FoodWithRecipe] = Field(alias="productList")
    next: str | None = Field(alias="next")

    class Config:
        allow_population_by_field_name = True
        populate_by_name = True
        use_enum_values = True


@app.get("/api/search", response_model=SearchResult)
async def search_food(
    q: str | None = None,
    page: int = 1,
    source: Optional[str] = None,
):
    q = q if q is not None else ""

    logger.info(
        "Search request received 2",
        extra={
            "query": q,
            "source": source,
        },
    )
    try:
        if source == "usda":
            results = search_usda_food(q, page)
            results = (results[0], [results[1]])
        elif source == "openfoodfacts":
            results = search_open_food_facts(q, page)
            results = (results[0], [results[1]])
        else:
            results = (
                search_usda_food(q, page)[0] + search_open_food_facts(q, page)[0],
                [search_usda_food(q, page)[1], search_open_food_facts(q, page)[1]],
            )

        logger.info(_("Results received, {results}", results=results))

        return SearchResult(product_list=results[0], page_numbers=results[1])
    except Exception as e:
        logger.error(_("Failed to fetch data, {error}", error=e))
        raise HTTPException(status_code=500, detail="Failed to fetch data")


@app.get("/api", response_model=StandardFood)
async def get_food(
    id: str,
    source: str,
):
    logger.info(
        _(
            "Getting by {id} source {source}",
            id=id,
            source=source,
        )
    )
    try:
        if source == "usda":
            result = fetch_usda_food(id)
        elif source == "openfoodfacts":
            result = fetch_open_food_facts(id)
        elif source == "edamam_recipe":
            result = fetch_recipe_from_edamam([id])

        logger.info(
            _(
                "Getting by {id} source {source} processed success",
                id=id,
                source=source,
            )
        )
        return result
    except Exception as e:
        logger.error(_("Failed to fetch data, {error}", error=str(e)))
        raise HTTPException(status_code=500, detail="Failed to fetch data")


@app.get("/api/recipe/s", response_model=RecipeSearchResult)
async def search_edamam_recipes(q: str | None = None, cont: str | None = None):
    q = q if q is not None else ""
    res = search_recipes(q, cont)
    return RecipeSearchResult(product_list=res[0], next=res[1])


@app.get("/api/by-uri", response_model=List[FoodWithRecipe])
async def get_food_from_edamam(
    uri: List[str] = Query(),
):
    logger.info(
        _(
            "Getting by uri {id}",
            id=id,
        )
    )
    try:
        result = await fetch_all_recipes_from_edamam(uri)

        logger.info(
            _(
                "Received by {id}",
                id=id,
            )
        )
        return result
    except Exception as e:
        logger.error(_("Failed to fetch data, {error}", error=str(e)))
        raise HTTPException(status_code=500, detail="Failed to fetch data")


if __name__ == "__main__":
    import uvicorn

    uvicorn.run(
        app,
        host="0.0.0.0",
        port=8080,
        ssl_keyfile="./certs/dt.key",
        ssl_certfile="./certs/dt.crt",
    )
